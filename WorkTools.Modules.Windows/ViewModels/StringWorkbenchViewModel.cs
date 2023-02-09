using NPOI.POIFS.NIO;
using NPOI.SS.UserModel;
using Prism.Commands;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using WorkTools.Infrastructure;
using static Utility.WinNative.MprAPI;

namespace WorkTools.Modules.Windows.ViewModels
{
    public class StringWorkbenchViewModel : BaseViewModel
    {
        private string _xamlLocation = string.Empty;
        public string XamlLocation
        {
            get => _xamlLocation;
            set
            {
                _xamlLocation = value;
                RaisePropertyChanged("XamlLocation");
            }
        }

        private string _excelLocation = string.Empty;
        public string ExcelLocation
        {
            get => _excelLocation;
            set
            {
                _excelLocation = value;
                RaisePropertyChanged("ExcelLocation");
            }
        }

        public DelegateCommand SelectXamlFileCommand { get; private set; }
        public DelegateCommand SelectExcelTemplateCommand { get; private set; }
        public DelegateCommand ExportCommand { get; private set; }

        public StringWorkbenchViewModel()
        {
            InitializeCommands();
        }

        private void InitializeCommands()
        {
            SelectXamlFileCommand = new DelegateCommand(OnSelectXamlFile);
            SelectExcelTemplateCommand = new DelegateCommand(OnSelectExcelTemplate);
            ExportCommand = new DelegateCommand(OnExport);
        }

        private async void OnExport()
        {
            await Task.Run(() =>
            {
                try
                {
                    if (string.IsNullOrEmpty(XamlLocation) || string.IsNullOrEmpty(ExcelLocation))
                    {
                        RunOnUIThreadAsync(() =>
                        {
                            MessageBox.Show("Any of path can't be empty");
                        });
                        return;
                    }

                    ResourceDictionary resourceDictionary = new ResourceDictionary();
                    resourceDictionary.Source = new Uri(XamlLocation);

                    int iIDIndex = -1;
                    int iStringIndex = -1;
                    int iEnUsIndex = -1;

                    IWorkbook workbook = WorkbookFactory.Create(ExcelLocation);
                    ISheet sheet = workbook.GetSheetAt(0);//获取第一个工作薄
                    IRow row = (IRow)sheet.GetRow(0);//获取第一行,LastRowNum 是当前表的总行数-1（注意）
                    foreach (var cell in row.Cells)
                    {
                        if (cell == null)
                            continue;
                        string value = cell.StringCellValue;
                        if (value == null)
                            continue;
                        switch (value)
                        {
                            case "ID":
                                iIDIndex = cell.ColumnIndex;
                                break;
                            case "String":
                                iStringIndex = cell.ColumnIndex;
                                break;
                            case "en-US":
                                iEnUsIndex = cell.ColumnIndex;
                                break;
                        }

                    }
                    if (iIDIndex > -1 && iStringIndex > -1 && iEnUsIndex > -1)
                    {
                        int startRow = 1;
                        foreach (var key in resourceDictionary.Keys)
                        {
                            IRow newRow = sheet.GetRow(startRow) ?? (IRow)sheet.CreateRow(startRow);
                            if (null == newRow)
                                return;

                            ICell idCell = newRow.GetCell(iIDIndex) ?? newRow.CreateCell(iIDIndex);
                            idCell.SetCellValue(startRow);
                            ICell stringCell = newRow.GetCell(iStringIndex) ?? newRow.CreateCell(iStringIndex);
                            stringCell.SetCellValue(key.ToString());
                            ICell enCell = newRow.GetCell(iEnUsIndex) ?? newRow.CreateCell(iEnUsIndex);
                            enCell.SetCellValue(resourceDictionary[key].ToString());

                            startRow++;
                        }
                        
                        FileInfo fileInfo = new FileInfo(ExcelLocation);
                        string exportedFilePath = Path.Combine(fileInfo.DirectoryName, $"{fileInfo.Name.Replace(fileInfo.Extension, string.Empty)}_exported{fileInfo.Extension}");
                        using (FileStream fs = new FileStream(exportedFilePath, FileMode.Create, FileAccess.Write))
                        {
                            workbook.Write(fs);
                        }

                        RunOnUIThreadAsync(() =>
                        {
                            MessageBox.Show("Completed");
                        });
                    }
                    workbook.Close();
                }
                catch (Exception e)
                {
                    RunOnUIThreadAsync(() =>
                    {
                        MessageBox.Show(e.ToString());
                    });
                }
            });

        }

        private void OnSelectExcelTemplate()
        {
            System.Windows.Forms.OpenFileDialog openFileDialog = new System.Windows.Forms.OpenFileDialog();
            openFileDialog.Filter = "Excel | *.xls;*.xlsx";
            openFileDialog.Multiselect = false;
            if (openFileDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                ExcelLocation = openFileDialog.FileName;
            }
        }

        private void OnSelectXamlFile()
        {
            System.Windows.Forms.OpenFileDialog openFileDialog = new System.Windows.Forms.OpenFileDialog();
            openFileDialog.Filter = "Xaml | *.xaml";
            openFileDialog.Multiselect = false;
            if (openFileDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                XamlLocation = openFileDialog.FileName;
            }

        }


    }
}
