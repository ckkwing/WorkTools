using Prism.Commands;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Markup;
using System.Xaml;
using System.Xml;
using Utility.Extensions;
using WorkTools.Infrastructure;
using WorkTools.Modules.Windows.Helpers;
using WorkTools.Modules.Windows.Properties;

namespace WorkTools.Modules.Windows.ViewModels
{
    public class ResourceDictionaryCopyViewModel : BaseViewModel
    {
        private string _keyPrefix = string.Empty;
        public string KeyPrefix
        {
            get { return _keyPrefix; }
            set
            {
                _keyPrefix = value;
                RaisePropertyChanged(nameof(KeyPrefix));
            }
        }

        private string _templateFile = string.Empty;
        public string TemplateFile
        {
            get { return _templateFile; }
            set
            {
                _templateFile = value;
                RaisePropertyChanged(nameof(TemplateFile));
            }
        }

        private string _source = string.Empty;
        public string Source
        {
            get { return _source; }
            set
            {
                _source = value;
                RaisePropertyChanged(nameof(Source));
            }
        }

        private string _target = string.Empty;
        public string Target
        {
            get { return _target; }
            set
            {
                _target = value;
                RaisePropertyChanged(nameof(Target));
            }
        }

        private string _newString = string.Empty;
        public string NewString
        {
            get { return _newString; }
            set
            {
                _newString = value;
                RaisePropertyChanged(nameof(NewString));
            }
        }

        private ObservableCollection<string> _newStrings = new ObservableCollection<string>();
        public ObservableCollection<string> NewStrings
        {
            get { return _newStrings; }
            set
            {
                _newStrings = value;
                RaisePropertyChanged(nameof(NewStrings));
            }
        }

        private bool _isUseIDSKey = true;
        public bool IsUseIDSKey
        {
            get { return _isUseIDSKey; }
            set
            {
                _isUseIDSKey = value;
                RaisePropertyChanged(nameof(IsUseIDSKey));
            }
        }

        public DelegateCommand SelectTemplateFileCommand { get; private set; }
        public DelegateCommand<string> SelectFolderCommand { get; private set; }
        public DelegateCommand AddStringCommand { get; private set; }
        public DelegateCommand CopyCommand { get; private set; }
        public ResourceDictionaryCopyViewModel()
        {
            SelectTemplateFileCommand = new DelegateCommand(OnSelectTemplateFile);
            SelectFolderCommand = new DelegateCommand<string>(OnSelectFolder);
            AddStringCommand = new DelegateCommand(OnAddString);
            CopyCommand = new DelegateCommand(OnCopy);

            _ = InitAsync();
        }

        private async Task InitAsync()
        {
            await Task.Delay(3000);
            KeyPrefix = "DVDBRIDGE_";
            TemplateFile = @"D:\Stash\nerodvdplayer\src\Language\Converted\DVDPlayerStrings-ENU.xaml";
            Source = @"D:\Stash\nerodvdplayer\src\Language\Converted";
            Target = @"D:\Stash\nerodvdplayer\submodules\common-dvdplayer\src\DVDPlayerCommon\build\AppRedist\LocalizedResources";
        }

        private void OnSelectTemplateFile()
        {
            string selectedFilePath = string.Empty;
            System.Windows.Forms.OpenFileDialog openFileDialog = new System.Windows.Forms.OpenFileDialog();
            openFileDialog.Filter = "Xaml | *.xaml";
            openFileDialog.Multiselect = false;
            if (openFileDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                selectedFilePath = openFileDialog.FileName;
            }
            TemplateFile = selectedFilePath;
        }

        private void OnCopy()
        {
            try
            {
                if (!Directory.Exists(Source) || !Directory.Exists(Target))
                {
                    System.Windows.MessageBox.Show("At least one of the folder doesn't exist");
                    return;
                }
                IList<string> keysInTemplate = GetStringKeysFromTemplateFile();

                IList<string> sourceFiles = Directory.GetFiles(Source).ToList();
                IList<string> targetFiles = Directory.GetFiles(Target).ToList();

                foreach (string sourceFile in sourceFiles)
                {
                    IList<string> languageCodes = Constant.MultilingualIdentificationCodeList.FirstOrDefault(list => !list.FirstOrDefault(code => sourceFile.ToUpper().Contains(code.ToUpper())).IsNullOrEmpty());
                    if (languageCodes.IsNullOrEmpty())
                        continue;
                    string targetFile = null;
                    foreach (var code in languageCodes)
                    {
                        targetFile = targetFiles.FirstOrDefault(filePath => filePath.ToUpper().Contains(code.ToUpper()));
                        if (targetFile != null)
                            break;
                    }

                    if (targetFile.IsNullOrEmpty())
                        continue;

                    ResourceDictionary sourceDictionary = Helper.LoadStyleDictionaryFromFile(sourceFile);
                    ResourceDictionary targetDictionary = Helper.LoadStyleDictionaryFromFile(targetFile);
                    IDictionary<string, string> keyValuePairs = new Dictionary<string, string>();
                    foreach (string key in keysInTemplate)
                    {
                        if (!sourceDictionary.Contains(key))
                            continue;
                        string value = sourceDictionary[key].ToString();
                        //targetDictionary.Add(key, value);
                        string newKey = $"{KeyPrefix}{key}";
                        if (targetDictionary.Contains(newKey))
                            continue;
                        keyValuePairs.Add(newKey, value);
                    }

                    XmlDocument xmlDoc = new XmlDocument();
                    xmlDoc.Load(targetFile);
                    XmlElement root = xmlDoc.DocumentElement;
                    foreach (var pair in keyValuePairs)
                    {
                        XmlNode xmlNode = xmlDoc.CreateElement("System", "String", @"clr-namespace:System;assembly=mscorlib");
                        xmlNode.InnerText = pair.Value;
                        XmlAttribute attribute = xmlDoc.CreateAttribute("x", "Key", @"http://schemas.microsoft.com/winfx/2006/xaml");
                        attribute.Value = $"{pair.Key}";
                        xmlNode.Attributes.Append(attribute);
                        root.AppendChild(xmlNode);
                    }
                    xmlDoc.Save(targetFile);
                }
            }
            catch(Exception e)
            {
                System.Windows.MessageBox.Show(e.Message);
            }

            System.Windows.MessageBox.Show("Copy finished");
        }

        private IList<string> GetStringKeysFromTemplateFile()
        {
            List<string> keys = new List<string>();
            if (IsUseIDSKey)
            {
                keys.AddRange(NewStrings);
            }
            else
            {
                ResourceDictionary resourceDictionary = Helper.LoadStyleDictionaryFromFile(TemplateFile);
                foreach (string strValue in NewStrings)
                {
                    foreach (var key in resourceDictionary.Keys)
                    {
                        object value = resourceDictionary[key];
                        if (value.ToString() == strValue)
                        {
                            keys.Add(key.ToString());
                            break;
                        }
                    }
                }
            }

            return keys;
        }

        //public ResourceDictionary LoadStyleDictionaryFromFile(string inFileName)
        //{
        //    ResourceDictionary resourceDictionary = null;
        //    if (File.Exists(inFileName))
        //    {
        //        try
        //        {
        //            using (var fs = new FileStream(inFileName, FileMode.Open, FileAccess.Read, FileShare.Read))
        //            {
        //                // Read in ResourceDictionary File
        //                resourceDictionary = (ResourceDictionary)System.Windows.Markup.XamlReader.Load(fs);
        //            }
        //        }
        //        catch (Exception ex)
        //        {
        //            Debug.WriteLine("LoadStyleDictionaryFromFile:" + ex.Message);
        //        }
        //    }
        //    return resourceDictionary;
        //}

        private void OnAddString()
        {
            if (NewStrings.Contains(NewString))
                return;

            NewStrings.Add(NewString);
            NewString = string.Empty;
        }

        private void OnSelectFolder(string dest)
        {
            string folder = string.Empty;
            FolderBrowserDialog folderBrowserDialog = new FolderBrowserDialog();
            if (folderBrowserDialog.ShowDialog() == DialogResult.OK)
            {
                folder = folderBrowserDialog.SelectedPath;
            }

            switch (dest)
            {
                case "source":
                    Source = folder;
                    break;
                case "target":
                    Target = folder;
                    break;
            }
        }



    }
}
