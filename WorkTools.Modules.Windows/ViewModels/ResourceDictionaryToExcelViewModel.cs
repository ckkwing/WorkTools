using OfficeOpenXml;
using Prism.Commands;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Packaging;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Xml;
using WorkTools.Infrastructure;
using WorkTools.Modules.Windows.Helpers;

namespace WorkTools.Modules.Windows.ViewModels
{
    public class ResourceDictionaryToExcelViewModel : BaseViewModel
    {
        string _pattern = @"x:Key=""([^""]+)"""; // 匹配 x:Key="任意非引号字符"

        private string _languageFile = string.Empty;
        public string LanguageFile
        {
            get { return _languageFile; }
            set
            {
                _languageFile = value;
                RaisePropertyChanged(nameof(LanguageFile));
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

        private string _output = string.Empty;
        public string Output
        {
            get { return _output; }
            set
            {
                _output = value;
                RaisePropertyChanged(nameof(Output));
            }
        }

        public DelegateCommand SelectLanguageFileCommand { get; private set; }
        public DelegateCommand SelectTemplateFileCommand { get; private set; }
        public DelegateCommand ExportCommand { get; private set; }

        public ResourceDictionaryToExcelViewModel()
        {
            SelectLanguageFileCommand = new DelegateCommand(OnSelectLanguageFile);
            SelectTemplateFileCommand = new DelegateCommand(OnSelectTemplateFile);
            ExportCommand = new DelegateCommand(OnExport);
        }

        private void OnExport()
        {
            Output = string.Empty;

            FileInfo excelFileInfo = new FileInfo(TemplateFile);
            if (!excelFileInfo.Exists)
            {
                Output += "Excel file not exist!";
                return;
            }

            // 设置许可证（必须放在使用 EPPlus 之前）
            ExcelPackage.License.SetNonCommercialPersonal("Test");//个人
            using (var package = new ExcelPackage(excelFileInfo))
            {
                ExcelWorksheet worksheet = package.Workbook.Worksheets[0];
                if (worksheet == null)
                {
                    Output += "Work sheet not exist!";
                    return;
                }
                int lastRow = worksheet.Dimension.End.Row; // 获取最后一行

                //ResourceDictionary resourceDictionary = Helper.LoadStyleDictionaryFromFile(LanguageFile);
                XmlDocument xmlDoc = new XmlDocument();
                xmlDoc.Load(LanguageFile);
                XmlElement root = xmlDoc.DocumentElement;
                int i = 1;
                foreach (XmlNode node in root.ChildNodes)
                {
                    string outerXml = node.OuterXml;
                    Output += $"Line:{i}, content: {outerXml}";
                    Output += System.Environment.NewLine;
                    Match match = Regex.Match(outerXml, _pattern);
                    if (match.Success)
                    {
                        string keyValue = match.Groups[1].Value; // 提取分组1（括号内内容）
                        Console.WriteLine(keyValue); // 输出: DVDBRIDGE_IDS_NEXT
                        worksheet.Cells[lastRow+1, 2].Value = keyValue;
                        worksheet.Cells[lastRow + 1, 3].Value = node.InnerText;
                        lastRow = worksheet.Dimension.End.Row;
                    }
                    else
                    {
                        Console.WriteLine("未找到匹配项");
                        Output += "##############未找到key，有错误发生";
                        Output += System.Environment.NewLine;
                    }
                    i++;

                }
                package.Save(); // 覆盖原文件
            }
            
        }

        private void OnSelectLanguageFile()
        {
            string selectedFilePath = string.Empty;
            System.Windows.Forms.OpenFileDialog openFileDialog = new System.Windows.Forms.OpenFileDialog();
            openFileDialog.Filter = "Xaml | *.xaml";
            openFileDialog.Multiselect = false;
            if (openFileDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                selectedFilePath = openFileDialog.FileName;
            }
            LanguageFile = selectedFilePath;
        }

        private void OnSelectTemplateFile()
        {
            string selectedFilePath = string.Empty;
            System.Windows.Forms.OpenFileDialog openFileDialog = new System.Windows.Forms.OpenFileDialog();
            openFileDialog.Filter = "xlsx | *.xlsx";
            openFileDialog.Multiselect = false;
            if (openFileDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                selectedFilePath = openFileDialog.FileName;
            }
            TemplateFile = selectedFilePath;
        }
    }
}
