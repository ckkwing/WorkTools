using Google.Apis.Auth.OAuth2;
using Google.Apis.Drive.v3;
using Google.Apis.Drive.v3.Data;
using Google.Apis.Util;
using Google.Apis.Util.Store;
using Prism.Commands;
using SMBLibrary.SMB2;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using WorkTools.Infrastructure;
using WorkTools.Modules.Windows.Models;

using GoogleFile = Google.Apis.Drive.v3.Data.File;

namespace WorkTools.Modules.Windows.ViewModels
{
    public class GoogleAPIViewModel : BaseViewModel
    {
        private UserCredential _credential;
        EncryptedFileDataStore _fileDataStore;
        private const string FILESTORE_KEY = "echen.google.oauth";
        private const string PERSISTENCE_CREDENTIAL_NAME = "echen";
        private const string FILEID_ROOT = "root";

        private const string CLIENT_ID_BIU = "14450100300-pptinqi1g1mf9p0foju8c7487nhe2bbr.apps.googleusercontent.com";
        private const string CLIENT_SECRET_BIU = "QlUqccTF8ptowJKBov21uGNJ";

        private ObservableCollection<UIGoogleFile> _uiGoogleFiles = new ObservableCollection<UIGoogleFile>();
        public ObservableCollection<UIGoogleFile> UIGoogleFiles
        {
            get => _uiGoogleFiles;
            set
            {
                _uiGoogleFiles = value;
            }
        }

        private string _currentDirID = FILEID_ROOT;

        private string _content = string.Empty;
        public string Content
        {
            get => _content;
            set
            {
                _content = value;
                RaisePropertyChanged(nameof(Content));
            }
        }

        public ICommand EnterUIGoogleFileCommand => new DelegateCommand<UIGoogleFile>(OnEnterUIGoogleFileAsync, (uiGoogleFile) => true);
        public ICommand SingInCommand => new DelegateCommand(OnSingInAsync, () => true);
        public ICommand RevokeCommand => new DelegateCommand(OnRevokeAsync, () => true);

        public ICommand ListCommand => new DelegateCommand(OnListAsync, () => true);
        public ICommand ClearContentCommand => new DelegateCommand(() => { Content = string.Empty; }, () => true);



        private async void OnSingInAsync()
        {

            _fileDataStore = new EncryptedFileDataStore(FILESTORE_KEY);
            //var test = new FileDataStore(FileDataStore.GenerateStoredKey("test", GetType()));
            using (FileStream fs = new FileStream("Configuration/client_secret.json", FileMode.Open, FileAccess.Read))
            {
                try
                {
                    _credential = await GoogleWebAuthorizationBroker.AuthorizeAsync(GoogleClientSecrets.FromStream(fs).Secrets, new[] { DriveService.Scope.Drive }, PERSISTENCE_CREDENTIAL_NAME, CancellationToken.None, _fileDataStore);
                    
                    //_credential = await GoogleWebAuthorizationBroker.AuthorizeAsync(
                    //    new ClientSecrets
                    //    {
                    //        ClientId = CLIENT_ID_BIU,
                    //        ClientSecret = CLIENT_SECRET_BIU
                    //    },
                    //    new[] { DriveService.Scope.Drive },
                    //    PERSISTENCE_CREDENTIAL_NAME,
                    //    CancellationToken.None,
                    //    _fileDataStore);

                    if (_credential.Token.IsExpired(SystemClock.Default))
                    {
                        AppendContent("Token is expired, need refresh.");
                        if (!await _credential.RefreshTokenAsync(CancellationToken.None))
                            AppendContent("Failed to refresh token!");
                        else
                            AppendContent("Refresh token successfully!");
                    }
                }
                catch (Exception ex)
                {
                    return;
                }

            }

            StringBuilder stringBuilder = new StringBuilder();
            //stringBuilder.AppendLine($"AccessToken:{await _credential.GetAccessTokenForRequestAsync()}");

            stringBuilder.AppendLine($"AccessToken: {_credential.Token.AccessToken}");
            stringBuilder.AppendLine($"AccessToken expires in seconds: {_credential.Token.ExpiresInSeconds}");
            stringBuilder.AppendLine($"RefreshToken: {_credential.Token.RefreshToken}");
            Content += stringBuilder.ToString();
            _currentDirID = FILEID_ROOT;
            //// Brings this app back to the foreground.
            //this.Activate();
        }

        private async void OnRevokeAsync()
        {
            bool revoked = true;
            if (!_credential.Token.IsExpired(SystemClock.Default))
            {
                revoked = await _credential?.RevokeTokenAsync(CancellationToken.None);
            }
            else
                AppendContent("Token is expired");

            if (revoked)
            {
                //await _fileDataStore.DeleteAsync(PERSISTENCE_CREDENTIAL_NAME);
                await _fileDataStore?.ClearAsync();
                Content += Environment.NewLine + "Revoked";
                _credential = null;
                _currentDirID = FILEID_ROOT;
            }
            else
                Content += Environment.NewLine + "Revoke failed";
        }

        private async void OnListAsync()
        {
            _currentDirID = FILEID_ROOT;
            await List(_currentDirID);
        }

        private async void OnEnterUIGoogleFileAsync(UIGoogleFile uiGoogleFile)
        {
            if (uiGoogleFile == null)
                return;



            if (uiGoogleFile.IsDirectory)
            {
                _currentDirID = uiGoogleFile.Id;
                await List(_currentDirID);
            }
            else
            {
                if (uiGoogleFile.GoogleFile.MimeType.Contains("application/vnd.google-apps"))
                    await Export(uiGoogleFile);
                else
                    await Download(uiGoogleFile);
            }
        }

        private async Task List(string fileID)
        {
            if (_credential == null)
            {
                AppendContent("revoked, has no ability to access google service");
                return;
            }
            var driveService = new DriveService(new Google.Apis.Services.BaseClientService.Initializer()
            {
                HttpClientInitializer = _credential,
                ApplicationName = "WPF UI Testbed"
            });

            GoogleFile root = await driveService.Files.Get(fileID).ExecuteAsync();

            UIGoogleFiles.Clear();
            List<GoogleFile> allFiles = new List<GoogleFile>();
            FileList result = null;
            while (true)
            {
                if (result != null && string.IsNullOrWhiteSpace(result.NextPageToken))
                    break;

                FilesResource.ListRequest listRequest = driveService.Files.List();
                listRequest.Q = $"'{root.Id}' in parents";
                listRequest.PageSize = 1000;
                //listRequest.Fields = "nextPageToken, files(id, name)";
                listRequest.Fields = "*";
                if (result != null)
                    listRequest.PageToken = result.NextPageToken;

                result = listRequest.Execute();
                allFiles.AddRange(result.Files);
            }

            allFiles.ForEach(async file =>
            {

                //FilesResource.GetRequest getRequest = driveService.Files.Get(file.Id);
                //GoogleFile fileInfo = await getRequest.ExecuteAsync();
                //if (fileInfo != null)
                //    textBoxOutput.Text += Environment.NewLine + $"({(fileInfo.MimeType != "application/vnd.google-apps.folder" ? "File" : "Directory")}){fileInfo.Name}";

                //textBoxOutput.Text += Environment.NewLine + $"({(file.MimeType != "application/vnd.google-apps.folder" ? "File" : "Directory")}){file.Name}";
                UIGoogleFiles.Add(new Models.UIGoogleFile(file));
            });
        }

        private async Task Export(UIGoogleFile uiGoogleFile)
        {
            if (_credential == null)
            {
                AppendContent("revoked, has no ability to access google service");
                return;
            }
            var driveService = new DriveService(new Google.Apis.Services.BaseClientService.Initializer()
            {
                HttpClientInitializer = _credential,
                ApplicationName = "WPF UI Testbed"
            });
            string ext = string.Empty;
            string mimeType = string.Empty;
            switch (uiGoogleFile.GoogleFile.MimeType)
            {
                case "application/vnd.google-apps.document":
                    {
                        mimeType = "application/vnd.openxmlformats-officedocument.wordprocessingml.document";
                        ext = ".docx";
                    }
                    break;
                case "application/vnd.google-apps.spreadsheet":
                    {
                        mimeType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                        ext = ".xlsx";
                    }
                    break;
                case "application/vnd.google-apps.presentation":
                    {
                        mimeType = "application/vnd.openxmlformats-officedocument.presentationml.presentation";
                        ext = ".pptx";
                    }
                    break;
            }

            if (string.IsNullOrEmpty(ext) || string.IsNullOrEmpty(mimeType))
                return;

            FilesResource.ExportRequest exportRequest = driveService.Files.Export(uiGoogleFile.Id, mimeType);
            string fileName = uiGoogleFile.GoogleFile.Name + ext;
            using (FileStream fs = new FileStream(fileName, FileMode.Create, FileAccess.Write))
            {
                exportRequest.MediaDownloader.ProgressChanged += (Google.Apis.Download.IDownloadProgress progress) =>
                {
                    switch (progress.Status)
                    {
                        case Google.Apis.Download.DownloadStatus.Downloading:
                            {
                                RunOnUIThread(() => {
                                    AppendContent($"Export '{fileName}' {progress.BytesDownloaded} bytes");
                                });
                                break;
                            }
                        case Google.Apis.Download.DownloadStatus.Completed:
                            {
                                RunOnUIThread(() =>
                                {
                                    AppendContent($"Export '{fileName}' complete.");
                                });
                                break;
                            }
                        case Google.Apis.Download.DownloadStatus.Failed:
                            {
                                RunOnUIThread(() => {
                                    AppendContent($"Export '{fileName}' failed, exception: {progress.Exception}");
                                });
                                break;
                            }
                    }
                };
                RunOnUIThread(() =>
                {
                    AppendContent($"Export '{fileName}' started ");
                });
                await exportRequest.DownloadAsync(fs);
            }
        }

        private async Task Download(UIGoogleFile uiGoogleFile)
        {
            if (_credential == null)
            {
                AppendContent("revoked, has no ability to access google service");
                return;
            }
            var driveService = new DriveService(new Google.Apis.Services.BaseClientService.Initializer()
            {
                HttpClientInitializer = _credential,
                ApplicationName = "WPF UI Testbed"
            });
            string fileName = uiGoogleFile.GoogleFile.Name;
            FilesResource.GetRequest getRequest = driveService.Files.Get(uiGoogleFile.Id);
            using (FileStream fs = new FileStream(fileName, FileMode.Create, FileAccess.Write))
            {
                getRequest.MediaDownloader.ProgressChanged += (Google.Apis.Download.IDownloadProgress progress) =>
                {
                    switch (progress.Status)
                    {
                        case Google.Apis.Download.DownloadStatus.Downloading:
                            {
                                RunOnUIThread(() => {
                                    AppendContent($"Download '{fileName}' {progress.BytesDownloaded} bytes");
                                });
                                break;
                            }
                        case Google.Apis.Download.DownloadStatus.Completed:
                            {
                                RunOnUIThread(() =>
                                {
                                    AppendContent($"Download '{fileName}' complete.");
                                });
                                break;
                            }
                        case Google.Apis.Download.DownloadStatus.Failed:
                            {
                                RunOnUIThread(() => {
                                    AppendContent($"Download '{fileName}' failed, exception: {progress.Exception}");
                                });
                                break;
                            }
                    }
                };
                RunOnUIThread(() =>
                {
                    AppendContent($"Download '{fileName}' started ");
                });
                await getRequest.DownloadAsync(fs);
            }
        }

        private void AppendContent(string content)
        {
            Content += Environment.NewLine + content;
        }
    }
}
