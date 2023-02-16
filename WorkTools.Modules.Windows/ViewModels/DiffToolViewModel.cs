using Algorithms;
using NPOI.OpenXml4Net.OPC.Internal;
using Prism.Commands;
using Prism.Ioc;
using Prism.Regions;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Utility.Extensions;
using WorkTools.Infrastructure;

namespace WorkTools.Modules.Windows.ViewModels
{
    public class DiffToolViewModel : BaseViewModel
    {
        public enum ComparisonMethod
        {
            [Description("Compare by MD5")]
            MD5,
            [Description("Compare one by one in bytes")]
            ByteByByte
        }

        private readonly IRegionManager _regionManager;
        private readonly IContainerExtension _containerExtension;
        private const int BUFFER_READ_SIZE = 1024 * 10; //1024 = 1KB

        public ComparisonMethod ComparisonMethods { get; }

        private ComparisonMethod _currentComparisonMethod = ComparisonMethod.MD5;
        public ComparisonMethod CurrentComparisonMethod
        {
            get => _currentComparisonMethod;
            set
            {
                _currentComparisonMethod = value;
                RaisePropertyChanged("CurrentComparisonMethod");
            }
        }

        private string _leftFilePath = string.Empty;
        public string LeftFilePath
        {
            get => _leftFilePath;
            set
            {
                _leftFilePath = value;
                RaisePropertyChanged("LeftFilePath");
            }
        }

        private string _rightFilePath = string.Empty;
        public string RightFilePath
        {
            get => _rightFilePath;
            set
            {
                _rightFilePath = value;
                RaisePropertyChanged("RightFilePath");
            }
        }

        private string _message = string.Empty;
        public string Message
        {
            get => _message;
            set
            {
                _message = value;
                RaisePropertyChanged("Message");
            }
        }

        private string _htmlString = string.Empty;
        public string HTMLString
        {
            get => _htmlString;
            set
            {
                _htmlString = value;
                RaisePropertyChanged("HTMLString");
            }
        }

        private string _leftString = string.Empty;
        public string LeftString
        {
            get => _leftString;
            set
            {
                _leftString = value;
                RaisePropertyChanged("LeftString");
            }
        }

        private string _rightString = string.Empty;
        public string RightString
        {
            get => _rightString;
            set
            {
                _rightString = value;
                RaisePropertyChanged("RightString");
            }
        }

        private bool _inProgress = false;
        public bool InProgress
        {
            get => _inProgress;
            set
            {
                _inProgress = value;
                RaisePropertyChanged("InProgress");
            }
        }

        private bool _isSetSamePropertiesForFile = false;
        public bool IsSetSamePropertiesForFile
        {
            get => _isSetSamePropertiesForFile;
            set
            {
                _isSetSamePropertiesForFile = value;
                RaisePropertyChanged("IsSetSamePropertiesForFile");
            }
        }


        public DelegateCommand SelectLeftFileCommand { get; private set; }
        public DelegateCommand SelectRightFileCommand { get; private set; }
        public DelegateCommand StartToCompareCommand { get; private set; }

        public DelegateCommand StartToDiffCommand { get; private set; }

        public DiffToolViewModel(IRegionManager regionManager, IContainerExtension containerExtension)
        {
            _regionManager = regionManager;
            _containerExtension = containerExtension;
            InitializeCommands();
        }

        private void InitializeCommands()
        {
            SelectLeftFileCommand = new DelegateCommand(OnSelectLeftFile);
            SelectRightFileCommand = new DelegateCommand(OnSelectRightFile);
            StartToCompareCommand = new DelegateCommand(OnStartToCompare);
            StartToDiffCommand = new DelegateCommand(OnStartToDiff);
        }

        private async void OnStartToDiff()
        {
            //MessageBox.Show("Not implement");
            //return;

            if (!IsValidSources())
                return;

            await Task.Run(() => {
                string htmlTemplate = GetHTMLTemplate();
                diff_match_patch dmp = new diff_match_patch();

                bool isBinaryOfF1 = Utility.Common.FileHelper.IsBinary(LeftFilePath);
                bool isBinaryOfF2 = Utility.Common.FileHelper.IsBinary(RightFilePath);

                var leftText = string.Empty;
                try
                {
                    using (StreamReader sr = File.OpenText(LeftFilePath))
                    {
                        while (!sr.EndOfStream)
                        {
                            leftText += sr.ReadLine();
                            if (!sr.EndOfStream)
                            {
                                leftText += Environment.NewLine;
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }

                //var leftText = File.ReadAllText(LeftFilePath);
                var rightText = File.ReadAllText(RightFilePath);

                List<Diff> diffs = dmp.diff_main(leftText, rightText);
                dmp.diff_cleanupSemantic(diffs);
                for (int i = 0; i < diffs.Count; i++)
                {
                    // += diffs[i].text;
                }

                HTMLString = htmlTemplate.Replace("[CONTENT]", dmp.diff_prettyHtml(diffs).Replace("&para;", string.Empty));
                LeftString = dmp.diff_text1(diffs);
                RightString = dmp.diff_text2(diffs);
            });

        }

        private string GetHTMLTemplate()
        {
            return $"<!DOCTYPE html>\r\n<html xmlns=\"http://www.w3.org/1999/xhtml\">\r\n<head>\r\n<meta http-equiv=\"Content-Type\" content=\"text/html; charset=utf-8\">\r\n</head>\r\n<body>\r\n[CONTENT]\r\n</body>\r\n</html>";
        }

        private bool IsValidSources()
        {
            if (LeftFilePath.IsNullOrEmpty() || RightFilePath.IsNullOrEmpty())
            {
                MessageBox.Show("Any of file path can't be empty");
                return false;
            }
            return true;
        }

        private async void OnStartToCompare()
        {
            if (!IsValidSources())
                return;

            bool identical = false;
            InProgress = true;
            Message = "Calculation in progress";
            Stopwatch sw = new Stopwatch();
            sw.Start();
            try
            {
                if (IsSetSamePropertiesForFile)
                    SetSamePropertiesForFiles();

                switch (CurrentComparisonMethod)
                {
                    case ComparisonMethod.MD5:
                        identical = await CompareByMD5(LeftFilePath, RightFilePath);
                        break;
                    case ComparisonMethod.ByteByByte:
                        identical = await CompareByByteArray(LeftFilePath, RightFilePath);
                        break;
                }
                Message = $"Method: {CurrentComparisonMethod.GetDescription()}, Identical: {identical}. Elapsed: {sw.Elapsed}";
            }
            catch (Exception ex)
            {
                Message = ex.Message;
            }

            sw.Stop();
            InProgress = false;
        }

        private void OnSelectLeftFile()
        {
            LeftFilePath = GetSelectedFilePath();

        }

        private void OnSelectRightFile()
        {
            RightFilePath = GetSelectedFilePath();
        }

        private bool SetSamePropertiesForFiles()
        {
            FileInfo leftFileInfo = new FileInfo(LeftFilePath);
            FileInfo rightFileInfo = new FileInfo(RightFilePath);
            if (!leftFileInfo.Exists || !rightFileInfo.Exists)
                return false;

            DateTime dateTime = DateTime.Now;
            File.SetAttributes(LeftFilePath, FileAttributes.Normal);
            File.SetLastWriteTime(LeftFilePath, dateTime);

            File.SetAttributes(RightFilePath, FileAttributes.Normal);
            File.SetLastWriteTime(RightFilePath, dateTime);

            return true;
        }

        private string GetSelectedFilePath()
        {
            string selectedFilePath = string.Empty;
            System.Windows.Forms.OpenFileDialog openFileDialog = new System.Windows.Forms.OpenFileDialog();
            //openFileDialog.Filter = "Excel | *.xls;*.xlsx";
            openFileDialog.Multiselect = false;
            if (openFileDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                selectedFilePath = openFileDialog.FileName;
            }
            return selectedFilePath;
        }

        private async Task<bool> CompareByMD5(string file1, string file2) => await Task.Run(() =>
        {
            using (var md5 = MD5.Create())
            {
                byte[] one, two;
                using (var fs1 = File.Open(file1, FileMode.Open))
                {
                    one = md5.ComputeHash(fs1);
                }
                using (var fs2 = File.Open(file2, FileMode.Open))
                {
                    two = md5.ComputeHash(fs2);
                }
                return BitConverter.ToString(one) == BitConverter.ToString(two);
            }
        });

        private async Task<bool> CompareByByteArray(string file1, string file2) => await Task.Run(() =>
        {
            using (FileStream fs1 = File.Open(file1, FileMode.Open))
            using (FileStream fs2 = File.Open(file2, FileMode.Open))
            {
                if (fs1.Length != fs2.Length)
                {
                    return false;
                }

                int file1Len = 0;
                int file2Len = 0;
                do
                {
                    int index = 0;

                    byte[] file1Bytes = new byte[BUFFER_READ_SIZE];
                    byte[] file2Bytes = new byte[BUFFER_READ_SIZE];

                    file1Len = fs1.Read(file1Bytes, 0, BUFFER_READ_SIZE);
                    file2Len = fs2.Read(file2Bytes, 0, BUFFER_READ_SIZE);
                    while (index < file1Len && index < file2Len)
                    {
                        if (file1Bytes[index] != file2Bytes[index])
                            return false;
                        index++;
                    }
                }
                while (file1Len > 0 && file2Len > 0);

                //byte[] one = new byte[BUFFER_SIZE];
                //byte[] two = new byte[BUFFER_SIZE];
                //while (true)
                //{
                //    int len1 = fs1.Read(one, 0, BUFFER_SIZE);
                //    int len2 = fs2.Read(two, 0, BUFFER_SIZE);
                //    int index = 0;
                //    while (index < len1 && index < len2)
                //    {
                //        if (one[index] != two[index]) return false;
                //        index++;
                //    }
                //    if (len1 == 0 || len2 == 0) break;
                //}
            }

            return true;

            ////Will be out of memory if file is too large
            //byte[] _source = File.ReadAllBytes(file1);
            //byte[] _dest = File.ReadAllBytes(file2);
            //if (_source.Length != _dest.Length)
            //{
            //    return false;
            //}
            //for (int i = 0; i < _source.Length; ++i)
            //{
            //    if (_source[i] != _dest[i])
            //    {
            //        return false;
            //    }
            //}
            //return true;

        });
    }
}
