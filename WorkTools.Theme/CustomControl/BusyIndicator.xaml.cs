using System;
using System.Collections.Generic;
using System.ComponentModel;
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

namespace WorkTools.Theme.CustomControl
{
    /// <summary>
    /// Interaction logic for BusyIndicator.xaml
    /// </summary>
    public partial class BusyIndicator : UserControl, INotifyPropertyChanged
    {
        public string Information
        {
            get { return (string)GetValue(InformationProperty); }
            set { SetValue(InformationProperty, value); }
        }

        public static readonly DependencyProperty InformationProperty =
            DependencyProperty.Register("Information", typeof(string),
                typeof(BusyIndicator), new UIPropertyMetadata(string.Empty));

        public BusyIndicator()
        {
            InitializeComponent();
            this.DataContext = this;
        }

        #region INotifyPropertyChanged Members

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged(string name)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(name));
        }

        #endregion
    }
}
