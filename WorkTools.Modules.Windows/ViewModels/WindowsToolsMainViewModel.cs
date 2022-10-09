using Prism.Commands;
using Prism.Ioc;
using Prism.Mvvm;
using Prism.Regions;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Shapes;

namespace WorkTools.Modules.Windows.ViewModels
{
    //[Export]
    //[PartCreationPolicy(CreationPolicy.Shared)]
    public class WindowsToolsMainViewModel : BindableBase
    {
        private readonly IRegionManager _regionManager;
        private readonly IContainerExtension _containerExtension;

        public DelegateCommand LockFileCommand { get; private set; }

        public WindowsToolsMainViewModel(IRegionManager regionManager, IContainerExtension containerExtension)
        {
            _regionManager = regionManager;
            _containerExtension = containerExtension;
            InitializeCommands();
        }

        private void InitializeCommands()
        {
            LockFileCommand = new DelegateCommand(OnLockFile);
        }

        private void OnLockFile()
        {
            string filePath = string.Empty;
            System.Windows.Forms.OpenFileDialog openFileDialog = new System.Windows.Forms.OpenFileDialog();
            if (openFileDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                filePath = openFileDialog.FileName;
            }

            if (!File.Exists(filePath))
            {
                MessageBox.Show("File not found");
                return;
            }

            File.Open(filePath, FileMode.Open, FileAccess.Read, FileShare.None);
        }
    }
}
