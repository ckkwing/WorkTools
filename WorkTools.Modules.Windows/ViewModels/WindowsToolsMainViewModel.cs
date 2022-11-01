using Prism.Commands;
using Prism.Ioc;
using Prism.Mvvm;
using Prism.Regions;
using System;
using System.Collections.Concurrent;
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
        private ConcurrentDictionary<string, FileStream> _lockedDic = new ConcurrentDictionary<string, FileStream>();

        public DelegateCommand LockFileCommand { get; private set; }
        public DelegateCommand UnlockFileCommand { get; private set; }

        public WindowsToolsMainViewModel(IRegionManager regionManager, IContainerExtension containerExtension)
        {
            _regionManager = regionManager;
            _containerExtension = containerExtension;
            InitializeCommands();
        }

        private void InitializeCommands()
        {
            LockFileCommand = new DelegateCommand(OnLockFile);
            UnlockFileCommand = new DelegateCommand(OnUnlockFile);
        }

        private void OnLockFile()
        {
            //string filePath = string.Empty;
            IList<string> filePathList = new List<string>();
            System.Windows.Forms.OpenFileDialog openFileDialog = new System.Windows.Forms.OpenFileDialog();
            openFileDialog.Multiselect = true;
            if (openFileDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                //filePath = openFileDialog.FileName;
                openFileDialog.FileNames?.ToList().ForEach(path => filePathList.Add(path));
            }

            //if (!File.Exists(filePath))
            //{
            //    MessageBox.Show("File not found");
            //    return;
            //}
            foreach (string filePath in filePathList)
            {
                if (!File.Exists(filePath))
                    continue;
                FileStream fs = File.Open(filePath, FileMode.Open, FileAccess.Read, FileShare.None);
                _lockedDic.TryAdd(filePath, fs);
            }
        }

        public void OnUnlockFile()
        {
            if (_lockedDic.Count == 0)
                return;

            foreach (FileStream fs in _lockedDic.Values)
            {
                fs.Close();
            }

            _lockedDic.Clear();
            MessageBox.Show("Files unlocked");
        }


    }
}
