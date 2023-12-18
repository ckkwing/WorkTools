using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows;

namespace WorkTools.Theme.CustomControl
{

    public class BaseTabItem : TabItem, INotifyPropertyChanged
    {

        protected bool CanExcute(object parameter)
        {
            return true;
        }

        protected bool CanExcute()
        {
            return true;
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(params string[] properties)
        {
            PropertyChangedEventHandler handler = PropertyChanged;

            if (handler != null)
            {
                foreach (var property in properties)
                {
                    handler(this, new PropertyChangedEventArgs(property));
                }
            }
        }
    }
}
