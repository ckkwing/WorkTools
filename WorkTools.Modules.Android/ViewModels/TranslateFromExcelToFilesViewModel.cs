using DBUtility;
using Prism.Commands;
using Prism.Events;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Input;
using WorkTools.Infrastructure.Events;
using WorkTools.Theme.CustomControl;
using WorkTools.Theme.Interface;
using Utility.Extensions;
using WorkTools.Modules.Android.Model;
using Utility.Common;
using NLogger;
using System.Xml.Serialization;
using WorkTools.Infrastructure.Model;

namespace WorkTools.Modules.Android.ViewModels
{
    public class TranslateFromExcelToFilesViewModel : BaseTabItem, IViewProperties
    {
        private const string SQL_GET_TABLE = "SELECT * FROM [ENU$]";
        private const string ID_FLAG = "String";
        private const string TRANSLATABLE_FLAG = "Translatable";

        public string ViewName => "Translate from excel to files"; //this.GetType().Name;

        public Project Projects { get; }

        private Project _currentProject = Project.BIU;
        public Project CurrentProject
        {
            get => _currentProject;
            set
            {
                _currentProject = value;
                OnPropertyChanged("CurrentProject");
            }
        }


        private string _excelFilePath = string.Empty;
        public string ExcelFilePath
        {
            get { return _excelFilePath; }
            set
            {
                _excelFilePath = value;
                OnPropertyChanged("ExcelFilePath");
            }
        }

        private string _parentFolderPath = string.Empty;
        public string ParentFolderPath
        {
            get { return _parentFolderPath; }
            set
            {
                _parentFolderPath = value;
                OnPropertyChanged("ParentFolderPath");
            }
        }

        private IDictionary<string, string> _translationFolderMatchDic = new Dictionary<string, string>();
        public IDictionary<string, string> TranslationFolderMatchDic
        {
            get { return _translationFolderMatchDic; }
            set
            {
                _translationFolderMatchDic = value;
                OnPropertyChanged("TranslationFolderMatchDic");
            }
        }

        public string StringFileName => CurrentProject == Project.BIU ? "strings_BIU_Android.xml" : "strings.xml";


        public ICommand BrowseExcelFileCommand { get; private set; }
        public ICommand BrowseParentFolderCommand { get; private set; }

        public ICommand StartCommand { get; private set; }
        public IEventAggregator EventAggregator { get; }

        [ImportingConstructor]
        public TranslateFromExcelToFilesViewModel(IEventAggregator eventAggregator)
        {
            BrowseExcelFileCommand = new DelegateCommand<object>(OnBrowseExcelFile, CanExcute);
            BrowseParentFolderCommand = new DelegateCommand<object>(OnBrowseParentFolder, CanExcute);
            StartCommand = new DelegateCommand<object>(OnStart, CanExcute);
            EventAggregator = eventAggregator;
        }

        private void OnStart(object obj)
        {
            Task.Factory.StartNew(() => {
                EventAggregator.GetEvent<ActionStatusEvent>().Publish(new ActionStatusEventArgs() { Status = ActionStatusEventArgs.ActionStatus.InProgress });
                switch(CurrentProject)
                {
                    case Project.DriveSpan:
                        TranslationFolderMatchDic= new Dictionary<string, string>(Constant.DriveSpanFolderMatchDic);
                        break;
                    case Project.StreamingPlayer:
                        TranslationFolderMatchDic = new Dictionary<string, string>(Constant.StreamingTranslationFolderMatchDic);
                        break;
                    case Project.BIU:
                        TranslationFolderMatchDic = new Dictionary<string, string>(Constant.BIUTranslationFolderMatchDic);
                        break;
                    default:
                        TranslationFolderMatchDic = new Dictionary<string, string>(Constant.CommonAndroidFolderMatchDic);
                        break;
                }
                Converte();
                EventAggregator.GetEvent<ActionStatusEvent>().Publish(new ActionStatusEventArgs() { Status = ActionStatusEventArgs.ActionStatus.Stopped });
            });

        }

        private void OnBrowseParentFolder(object obj)
        {
            FolderBrowserDialog folderBrowserDialog = new FolderBrowserDialog();
            DialogResult result = folderBrowserDialog.ShowDialog();

            if (result == DialogResult.Cancel)
            {
                return;
            }
            ParentFolderPath = folderBrowserDialog.SelectedPath.Trim();
        }

        private void OnBrowseExcelFile(object obj)
        {
            System.Windows.Forms.OpenFileDialog openFileDialog = new System.Windows.Forms.OpenFileDialog();
            openFileDialog.Filter = "Excel文件(*.xls;*.xlsx)|*.xls;*.xlsx|所有文件|*.*";
            openFileDialog.ValidateNames = true;
            openFileDialog.CheckPathExists = true;
            openFileDialog.CheckFileExists = true;
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                ExcelFilePath = openFileDialog.FileName;
            }
        }

        private void Converte()
        {
            if (ParentFolderPath.IsNullOrEmpty() || ExcelFilePath.IsNullOrEmpty())
            {
                System.Windows.MessageBox.Show("Please fill path");
                return;
            }

            IList<FileInfo> allStringFiles = GetAllStringFiles(_parentFolderPath);

            DataTable dt = ExcelHelper.ExecuteDataTable(ExcelHelper.GetConnectionStringByFilePath(ExcelFilePath, "yes"), CommandType.Text, SQL_GET_TABLE);
            if (!dt.Columns.Contains(ID_FLAG))
            {
                System.Windows.MessageBox.Show("Can not find ID, please check");
                return;
            }

            foreach (string countryName in TranslationFolderMatchDic.Keys)
            {
                if (!dt.Columns.Contains(countryName))
                {
                    System.Windows.MessageBox.Show("Languate: " + countryName + " not exists, please check!");
                    return;
                }
            }

            IList<TranslationItem> translationItems = new List<TranslationItem>();
            foreach (DataRow row in dt.Rows)
            {
                bool? isTranslatable = null;
                if (dt.Columns.Contains(TRANSLATABLE_FLAG))
                {
                    bool convertedValue = true;
                    if (bool.TryParse(row[TRANSLATABLE_FLAG].ToString(), out convertedValue))
                        isTranslatable = convertedValue;
                }
                TranslationItem item = new TranslationItem(row[ID_FLAG].ToString(), isTranslatable);
                foreach (string countryName in TranslationFolderMatchDic.Keys)
                {
                    try
                    {
                        item.DicLanguages.Add(new KeyValuePair<string, string>(countryName.Trim(), row[countryName].ToString().Trim()));
                    }
                    catch (Exception e)
                    {
                        Debug.WriteLine(e.Message);
                    }
                }
                translationItems.Add(item);
            }

            foreach (FileInfo fileInfo in allStringFiles)
            {
                bool isChangedFile = false;
                IDictionary<string, string> newAddedIDs = new Dictionary<string, string>();

                TranslationResourcesEntity entity = XmlSerializeUtility.DeserializeFromXml<TranslationResourcesEntity>(fileInfo.FullName);
                foreach (TranslationItem item in translationItems)
                {
                    bool isFindMatchedLanguage = true;
                    string matchedLanguageCode = string.Empty;
                    isFindMatchedLanguage = item.DicLanguages.Keys.FirstOrDefault(languageCode =>
                    {
                        if (!TranslationFolderMatchDic.ContainsKey(languageCode))
                            return false;
                        string realTargetName = TranslationFolderMatchDic[languageCode];
                        bool isFind = 0 == string.Compare(realTargetName, fileInfo.Directory.Name, true);
                        if (isFind)
                            matchedLanguageCode = languageCode;
                        return isFind;
                    }).IsNull() ? false : true;

                    if (isFindMatchedLanguage)
                    {
                        string strValue = string.Empty;
                        if (item.DicLanguages.TryGetValue(matchedLanguageCode, out strValue))
                        {
                            if (!entity.Strings.FirstOrDefault(str => { return 0 == string.Compare(str.Name, item.StringID, true); }).IsNull())
                            {
                                //System.Windows.MessageBox.Show("Find same key: " + item.StringID + ", do nothing now");
                                if (entity.ReplaceExistStringByName(item.StringID, strValue))
                                {
                                    isChangedFile = true;
                                    LogHelper.UILogger.Debug("Find same key: " + item.StringID + ", replace value to: " + strValue);
                                }
                                else
                                    LogHelper.UILogger.Debug("Find same key: " + item.StringID + ", but some error occured, do nothing!");
                            }
                            else
                            {
                                entity.Add(item.StringID, strValue, item.IsTranslatable.ToString().ToLower());
                                newAddedIDs.Add(item.StringID, strValue);
                                isChangedFile = true;
                            }
                        }
                    }
                }

                if (isChangedFile)
                {
                    XmlSerializeUtility.SerializeToXml<TranslationResourcesEntity>(fileInfo.FullName, entity);
                    //SimpleWriteXml(fileInfo.FullName, newAddedIDs);
                }
            }

            //foreach(TranslationItem item in translationItems)
            //{
            //    foreach(KeyValuePair<string, string> pair in item.DicLanguages)
            //    {
            //        if (!Constant.TranslationFolderMatchDic.ContainsKey(pair.Key))
            //            continue;
            //        string realTargetName = Constant.TranslationFolderMatchDic[pair.Key];
            //        FileInfo find = allStringFiles.FirstOrDefault(fileInfo => { return 0 == string.Compare(realTargetName, fileInfo.Directory.Name, true); });
            //        if (find.IsNull())
            //            continue;

            //        TranslationResourcesEntity entity = XmlSerializeUtility.DeserializeFromXml<TranslationResourcesEntity>(find.FullName);
            //        if (entity.Strings.FirstOrDefault(str => { return 0 == string.Compare(str.Name, pair.Key, true); }).IsNull())
            //        {
            //            System.Windows.MessageBox.Show("Find same key: " + pair.Key + ", break now");
            //            return;
            //        }

            //        entity.Add(pair.Key, pair.Value);
            //    }
            //}
        }

        private IList<FileInfo> GetAllStringFiles(string resRootFolder)
        {
            IList<FileInfo> stringFiles = new List<FileInfo>();
            if (!Directory.Exists(resRootFolder))
                return stringFiles;

            DirectoryInfo rootDir = new DirectoryInfo(resRootFolder);
            DirectoryInfo[] directoryInfos = rootDir.GetDirectories("values*");
            for (int i = 0; i < directoryInfos.Length; i++)
            {
                FileInfo stringFile = new FileInfo(Path.Combine(directoryInfos[i].FullName, StringFileName));
                if (null != stringFile)
                    stringFiles.Add(stringFile);
            }
            return stringFiles;
        }

    }
}
