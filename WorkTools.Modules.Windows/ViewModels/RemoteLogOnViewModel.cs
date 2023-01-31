using GongSolutions.Shell.Interop;
using GongSolutions.Shell;
using Prism.Commands;
using Prism.Ioc;
using Prism.Regions;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Interop;
using Utility.Network;
using WorkTools.Infrastructure;
using static Utility.WinNative.MprAPI;
using System.Collections;
using System.IO;

namespace WorkTools.Modules.Windows.ViewModels
{
    public class RemoteLogOnViewModel : BaseViewModel
    {
        private readonly IRegionManager _regionManager;
        private readonly IContainerExtension _containerExtension;

        private string _remoteAddress = @"\\192.168.82.69";
        public string RemoteAddress
        {
            get => _remoteAddress;
            set
            {
                _remoteAddress = value;
                RaisePropertyChanged("RemoteAddress");
            }
        }

        private string _userName = string.Empty;
        public string UserName
        {
            get => _userName;
            set
            {
                _userName = value;
                RaisePropertyChanged("UserName");
            }
        }
        private string _password = string.Empty;
        public string Password
        {
            get => _password;
            set
            {
                _password = value;
                RaisePropertyChanged("Password");
            }
        }

        public DelegateCommand ConnectCommand { get; private set; }
        public DelegateCommand DisconnectCommand { get; private set; }

        public RemoteLogOnViewModel(IRegionManager regionManager, IContainerExtension containerExtension)
        {
            _regionManager = regionManager;
            _containerExtension = containerExtension;
            InitializeCommands();
        }

        private void InitializeCommands()
        {
            ConnectCommand = new DelegateCommand(OnConnect);
            DisconnectCommand = new DelegateCommand(OnDisconnect);
        }

        private void OnDisconnect()
        {

            //IEnumerator<ShellItem> list = GetEnumerator();
            ErrorCodes errorCodes = NetworkShareConnect.DisconnectRemote(RemoteAddress);
            MessageBox.Show($"Disconnect error code is: {errorCodes}");
        }

        private void OnConnect()
        {
            //IntPtr ownerPtr = new WindowInteropHelper(Application.Current.MainWindow).Handle;
            ErrorCodes errorCodes = NetworkShareConnect.DisconnectRemote(RemoteAddress);
            if (string.IsNullOrEmpty(UserName) && string.IsNullOrEmpty(Password))
            {
                errorCodes = NetworkShareConnect.ConnectToRemote(IntPtr.Zero, string.Empty, RemoteAddress, true);
            }
            else
            {
                errorCodes = NetworkShareConnect.ConnectToRemote(IntPtr.Zero, string.Empty, RemoteAddress, UserName, Password);
            }

            MessageBox.Show($"Connect error code is: {errorCodes}");
        }


        //private IEnumerator<string> GetEnumerator()
        //{
        //    ShellItem folder = new ShellItem((Environment.SpecialFolder)CSIDL.NETWORK);
        //    IEnumerator<ShellItem> e = folder.GetEnumerator(SHCONTF.FOLDERS);

        //    while (e.MoveNext())
        //    {
        //        Debug.Print(e.Current.ParsingName);
        //        yield return e.Current.ParsingName;
        //    }
        //}

        private IEnumerator<ShellItem> GetEnumerator()
        {
            List<ShellItem> list = new List<ShellItem>();
            ShellItem folder = new ShellItem((Environment.SpecialFolder)CSIDL.NETWORK);
            IEnumerator<ShellItem> e = folder.GetEnumerator(SHCONTF.FOLDERS);
            while (e.MoveNext())
            {
                list.Add(e.Current);
            }
            return list.GetEnumerator();
        }
    }
}
