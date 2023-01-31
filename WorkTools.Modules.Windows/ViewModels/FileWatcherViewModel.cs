using Prism.Commands;
using Prism.Ioc;
using Prism.Mvvm;
using Prism.Regions;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using WorkTools.Infrastructure;

namespace WorkTools.Modules.Windows.ViewModels
{
    public class FileWatcherViewModel : BaseViewModel
    {
        private readonly IRegionManager _regionManager;
        private readonly IContainerExtension _containerExtension;
        private FileSystemWatcher _watcher;

        private string _folderToWatch = string.Empty;
        public string FolderToWatch
        {
            get => _folderToWatch;
            set
            {
                _folderToWatch = value;
                RaisePropertyChanged("FolderToWatch");
            }
        }

        private ObservableCollection<string> _changeEvent = new ObservableCollection<string>();
        public ObservableCollection<string> ChangeEvent
        {
            get => _changeEvent;
            set
            {
                _changeEvent = value;
                RaisePropertyChanged("ChangeEvent");
            }
        }

        public DelegateCommand SelectFolderCommand { get; private set; }
        public DelegateCommand StartToWatchCommand { get; private set; }
        public DelegateCommand StopToWatchCommand { get; private set; }
        public DelegateCommand ClearHistoryCommand { get; private set; }

        public FileWatcherViewModel(IRegionManager regionManager, IContainerExtension containerExtension)
        {
            _regionManager = regionManager;
            _containerExtension = containerExtension;
            InitializeCommands();
        }

        private void InitializeCommands()
        {
            SelectFolderCommand = new DelegateCommand(OnSelectFolder);
            StartToWatchCommand = new DelegateCommand(OnStartToWatch);
            StopToWatchCommand = new DelegateCommand(OnStopToWatch);
            ClearHistoryCommand = new DelegateCommand(OnClearHistory);
        }

        private void CloseWatcher()
        {
            if (null == _watcher)
                return;
            _watcher.Created -= new FileSystemEventHandler(OnChanged);
            _watcher.Changed -= new FileSystemEventHandler(OnChanged);
            _watcher.Deleted -= new FileSystemEventHandler(OnChanged);
            _watcher.Renamed -= new RenamedEventHandler(OnRenamed);
            _watcher.EnableRaisingEvents = false;
            _watcher = null;
        }

        private void OnStartToWatch()
        {
            CloseWatcher();

            if (string.IsNullOrEmpty(FolderToWatch))
            {
                MessageBox.Show("FolderToWatch is empty!");
                return;
            }
            _watcher = new FileSystemWatcher(FolderToWatch);
            _watcher.Created += new FileSystemEventHandler(OnChanged);
            _watcher.Changed += new FileSystemEventHandler(OnChanged);
            _watcher.Deleted += new FileSystemEventHandler(OnChanged);
            _watcher.Renamed += new RenamedEventHandler(OnRenamed);
            _watcher.IncludeSubdirectories = true;
            _watcher.EnableRaisingEvents = true;
        }

        private void OnStopToWatch()
        {
            CloseWatcher();
        }

        private void OnChanged(object sender, FileSystemEventArgs e)
        {
            RunOnUIThreadAsync(() => {
                ChangeEvent.Add($"File change, ChangeType:{e.ChangeType}, Name:{e.Name}, Full path:{e.FullPath}");
            });
        }

        private void OnRenamed(object sender, RenamedEventArgs e)
        {
            RunOnUIThreadAsync(() => {
                ChangeEvent.Add($"File change, ChangeType:{e.ChangeType}, Name:{e.Name}, Full path:{e.FullPath}");
            });
        }

        private void OnClearHistory()
        {
            ChangeEvent.Clear();
        }

        private void OnSelectFolder()
        {
            FolderToWatch = string.Empty;
            FolderBrowserDialog folderBrowserDialog = new FolderBrowserDialog();
            if (folderBrowserDialog.ShowDialog() == DialogResult.OK)
            {
                FolderToWatch = folderBrowserDialog.SelectedPath;
            }
        }


    }
}
