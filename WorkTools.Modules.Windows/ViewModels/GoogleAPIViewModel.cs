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
        struct ClientInfo
        {
            internal string Id;
            internal string Secret;

            internal ClientInfo(string id, string secret)
            {
                Id = id;
                Secret = secret;
            }
        }

        private static Dictionary<string, ClientInfo> _clientInfos = new Dictionary<string, ClientInfo>()
        {
            {"DVD", new ClientInfo("562722407610-vfdfel9djk3njbt189qvpic7udf8p9f2.apps.googleusercontent.com", "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") },
            {"BIU", new ClientInfo("14450100300-pptinqi1g1mf9p0foju8c7487nhe2bbr.apps.googleusercontent.com", "QlUqccTF8ptowJKBov21uGNJ") },
            {"ERIC", new ClientInfo("893694845153-6u06bgtka7dai0ngkgm718peajlgc7ul.apps.googleusercontent.com", "gAcxJv36tL8Zmwssha8f35kx") },
        };

        private UserCredential _credential;
        FileDataStore _fileDataStore;
        private const string FILESTORE_KEY = "echen.google.oauth";
        private const string PERSISTENCE_CREDENTIAL_NAME = "echen";
        private const string FILEID_ROOT = "root";

        private static ClientInfo _defaultClientInfo = _clientInfos["DVD"];
        private static string CLIENT_ID_BIU = _defaultClientInfo.Id;
        private static string CLIENT_SECRET_BIU = _defaultClientInfo.Secret;

        private const string DOWNLOAD_FOLDER_RELATED_PATH = "1. Download";

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
        public ICommand ReauthorizeCommand => new DelegateCommand(OnReauthorizeAsync, () => true);

        public ICommand ListCommand => new DelegateCommand(OnListAsync, () => true);
        public ICommand ClearContentCommand => new DelegateCommand(() => { Content = string.Empty; }, () => true);



        private async void OnSingInAsync()
        {

            _fileDataStore = new FileDataStore(FILESTORE_KEY); //new EncryptedFileDataStore(FILESTORE_KEY);
            //var test = new FileDataStore(FileDataStore.GenerateStoredKey("test", GetType()));

            //using (FileStream fs = new FileStream("Configuration/client_secret.json", FileMode.Open, FileAccess.Read))
            {
                try
                {
                    //_credential = await GoogleWebAuthorizationBroker.AuthorizeAsync(GoogleClientSecrets.FromStream(fs).Secrets, new[] { DriveService.Scope.Drive }, PERSISTENCE_CREDENTIAL_NAME, CancellationToken.None, _fileDataStore);

                    _credential = await GoogleWebAuthorizationBroker.AuthorizeAsync(
                        new ClientSecrets
                        {
                            ClientId = CLIENT_ID_BIU,
                            ClientSecret = CLIENT_SECRET_BIU
                        },
                        new[] { DriveService.Scope.Drive },
                        PERSISTENCE_CREDENTIAL_NAME,
                        CancellationToken.None,
                        _fileDataStore);

                    if (_credential.Token.IsExpired(SystemClock.Default))
                    {
                        AppendContent("Token is expired, need refresh.");
                        if (!await _credential.RefreshTokenAsync(CancellationToken.None))
                            Content = "Failed to refresh token!";
                        else
                            Content = "Refresh token successfully!";
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
           
            _currentDirID = FILEID_ROOT;
            //// Brings this app back to the foreground.
            //this.Activate();

            About about = await GetUserInfo();
            stringBuilder.AppendLine($"User display name: {about?.User.DisplayName}");
            stringBuilder.AppendLine($"User email address: {about?.User.EmailAddress}");
            stringBuilder.AppendLine($"User photo link: {about?.User.PhotoLink}");
            AppendContent(stringBuilder.ToString());
        }

        private async Task<About> GetUserInfo()
        {
            var driveService = new DriveService(new Google.Apis.Services.BaseClientService.Initializer()
            {
                HttpClientInitializer = _credential,
                ApplicationName = "WPF UI Testbed"
            });

            var userAboutRequest = driveService.About.Get();
            userAboutRequest.Fields = "*";
            return await userAboutRequest.ExecuteAsync(CancellationToken.None);
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
                //await _fileDataStore.DeleteAsync<EncryptedFileDataStore>(PERSISTENCE_CREDENTIAL_NAME);
                await _fileDataStore?.ClearAsync();
                Content += Environment.NewLine + "Revoked";
                _credential = null;
                _currentDirID = FILEID_ROOT;
            }
            else
                Content += Environment.NewLine + "Revoke failed";
        }

        private async void OnReauthorizeAsync()
        {
            if (await _credential?.RevokeTokenAsync(CancellationToken.None))
            {
                await GoogleWebAuthorizationBroker.ReauthorizeAsync(_credential, CancellationToken.None);
                About about = await GetUserInfo();
            }
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
                if (!Directory.Exists(DOWNLOAD_FOLDER_RELATED_PATH))
                    Directory.CreateDirectory(DOWNLOAD_FOLDER_RELATED_PATH);

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
                listRequest.Q = $"'{root.Id}' in parents AND trashed = false";
                //listRequest.Q = $"'{root.Id}' in parents AND mimeType != 'application/vnd.google-apps.folder' AND (fileExtension = 'mp4' OR fileExtension = 'txt')";
                listRequest.PageSize = 1000;
                //listRequest.Fields = "nextPageToken, files(id, name)";
                listRequest.Fields = "*";
                if (result != null)
                    listRequest.PageToken = result.NextPageToken;

                result = listRequest.Execute();
                allFiles.AddRange(result.Files);
            }

            allFiles.ForEach(file =>
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
            using (FileStream fs = new FileStream(Path.Combine(DOWNLOAD_FOLDER_RELATED_PATH, fileName), FileMode.Create, FileAccess.Write))
            {
                exportRequest.MediaDownloader.ProgressChanged += (Google.Apis.Download.IDownloadProgress progress) =>
                {
                    switch (progress.Status)
                    {
                        case Google.Apis.Download.DownloadStatus.Downloading:
                            {
                                RunOnUIThread(() =>
                                {
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
                                RunOnUIThread(() =>
                                {
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

            AppendContent($"ThumbnailLink:{uiGoogleFile.GoogleFile?.ThumbnailLink}");

            var driveService = new DriveService(new Google.Apis.Services.BaseClientService.Initializer()
            {
                HttpClientInitializer = _credential,
                ApplicationName = "WPF UI Testbed"
            });
            string fileName = uiGoogleFile.GoogleFile.Name;
            FilesResource.GetRequest getRequest = driveService.Files.Get(uiGoogleFile.Id);
            using (FileStream fs = new FileStream(Path.Combine(DOWNLOAD_FOLDER_RELATED_PATH, fileName), FileMode.Create, FileAccess.Write))
            {
                getRequest.MediaDownloader.ProgressChanged += (Google.Apis.Download.IDownloadProgress progress) =>
                {
                    switch (progress.Status)
                    {
                        case Google.Apis.Download.DownloadStatus.Downloading:
                            {
                                RunOnUIThread(() =>
                                {
                                    AppendContent($"Downloading '{fileName}' {progress.BytesDownloaded} bytes");
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
                                RunOnUIThread(() =>
                                {
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
                //await getRequest.DownloadRangeAsync(fs, new System.Net.Http.Headers.RangeHeaderValue(0, 587637441));
                await getRequest.DownloadAsync(fs);
            }
        }

        private void AppendContent(string content)
        {
            Content += Environment.NewLine + content;
        }
    }
}
