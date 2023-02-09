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
using WorkTools.Infrastructure;
using WorkTools.Modules.Windows.Views;

namespace WorkTools.Modules.Windows.ViewModels
{
    //[Export]
    //[PartCreationPolicy(CreationPolicy.Shared)]
    public class WindowsToolsMainViewModel : BaseViewModel
    {
        private readonly IRegionManager _regionManager;
        private readonly IContainerExtension _containerExtension;
        private ConcurrentDictionary<string, FileStream> _lockedDic = new ConcurrentDictionary<string, FileStream>();

        public DelegateCommand LockFileCommand { get; private set; }
        public DelegateCommand UnlockFileCommand { get; private set; }
        public DelegateCommand OpenFileWatcherCommand { get; private set; }
        public DelegateCommand LogonRemoteCommand { get; private set; }
        public DelegateCommand ExportXamlStringtoExcel { get; private set; }
        public DelegateCommand OpenDiffToolCommand { get; private set; }

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
            OpenFileWatcherCommand = new DelegateCommand(OnOpenFileWatcher);
            LogonRemoteCommand = new DelegateCommand(OnLogonRemote);
            ExportXamlStringtoExcel = new DelegateCommand(OnExportXamlStringtoExcel);
            OpenDiffToolCommand = new DelegateCommand(OnOpenDiffTool);
        }

        private void OnOpenDiffTool()
        {
            DiffToolView diffToolView = new DiffToolView();
            diffToolView.ShowDialog();
        }

        private void OnExportXamlStringtoExcel()
        {
            StringWorkbenchView stringWorkbenchView = new StringWorkbenchView();
            stringWorkbenchView.ShowDialog();
        }

        private void OnLogonRemote()
        {
            RemoteLogOnView remoteLogOnView = new RemoteLogOnView();
            remoteLogOnView.ShowDialog();
        }

        private void OnOpenFileWatcher()
        {
            FileWatcherView fileWatcherView = new FileWatcherView();
            fileWatcherView.ShowDialog();
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
