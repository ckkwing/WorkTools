﻿using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using WorkTools.Modules.Windows.ViewModels;

namespace WorkTools.Modules.Windows.Views
{
    /// <summary>
    /// Interaction logic for WindowsToolsMainView.xaml
    /// </summary>
    //[Export]
    //[PartCreationPolicy(CreationPolicy.Shared)]
    public partial class WindowsToolsMainView : UserControl
    {
        //[Import]
        //public WindowsToolsMainViewModel ViewModel
        //{
        //    get { return DataContext as WindowsToolsMainViewModel; }
        //    set { DataContext = value; }
        //}

        public WindowsToolsMainView()
        {
            InitializeComponent();
        }
    }
}
