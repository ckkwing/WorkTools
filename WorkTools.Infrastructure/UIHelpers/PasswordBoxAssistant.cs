using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows;

namespace WorkTools.Infrastructure.UIHelpers
{
    public static class PasswordBoxAssistant
    {
        #region Example
        //  .xaml
        //  <Window xmlns:xamlHelpers="clr-namespace:BackItUp.Common.XamlHelpers" ...
        //  ...
        //  <PasswordBox ...
        //    xamlHelpers:PasswordBoxAssistant.IsBound="true"
        //    xamlHelpers:PasswordBoxAssistant.Binding="{Binding Path=Password, Mode=TwoWay, UpdateSourceTrigger=PropertyChanged}"/>
        //    
        //   .cs
        //   public string Password { get { return _password;} set {_password = value; OnPropertyChanged("Password")} }
        //  ...
        // Password = "123456";
        #endregion

        #region Property
        /// <summary>
        /// the property which acts as bridge between Password of UI control and the property of View Model
        /// </summary>
        public static readonly DependencyProperty BindingProperty =
            DependencyProperty.RegisterAttached("Binding", typeof(string), typeof(PasswordBoxAssistant), new FrameworkPropertyMetadata(string.Empty, OnBindingChanged));

        /// <summary>
        /// the property indicates if we use this assistant to implement the binding of password 
        /// </summary>
        public static readonly DependencyProperty IsBoundProperty = DependencyProperty.RegisterAttached(
            "IsBound", typeof(bool), typeof(PasswordBoxAssistant), new PropertyMetadata(false, OnIsBoundChanged));

        /// <summary>
        /// flag to avoid updating many times
        /// </summary>
        private static readonly DependencyProperty UpdatingPassword =
            DependencyProperty.RegisterAttached("UpdatingPassword", typeof(bool), typeof(PasswordBoxAssistant));
        #endregion Property

        #region Public Methods
        /// <summary>
        /// set function
        /// </summary>
        public static void SetIsBound(DependencyObject dp, bool value)
        {
            dp.SetValue(IsBoundProperty, value);
        }

        /// <summary>
        /// get function
        /// </summary>
        public static bool GetIsBound(DependencyObject dp)
        {
            return (bool)dp.GetValue(IsBoundProperty);
        }

        /// <summary>
        /// get function
        /// </summary>
        public static string GetBinding(DependencyObject dp)
        {
            return (string)dp.GetValue(BindingProperty);
        }

        /// <summary>
        /// set function
        /// </summary>
        public static void SetBinding(DependencyObject dp, string value)
        {
            dp.SetValue(BindingProperty, value);
        }
        #endregion Public Methods

        #region Methods
        /// <summary>
        /// Property change event handler
        /// </summary>
        private static void OnBindingChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            PasswordBox box = d as PasswordBox;

            if (d == null || !GetIsBound(d))
            {
                return;
            }

            box.PasswordChanged -= HandlePasswordChanged;

            string newPassword = (string)e.NewValue;

            if (!GetUpdatingPassword(box))
            {
                box.Password = newPassword;
            }

            box.PasswordChanged += HandlePasswordChanged;
        }

        /// <summary>
        /// property changed event handler
        /// </summary>
        private static void OnIsBoundChanged(DependencyObject dp, DependencyPropertyChangedEventArgs e)
        {
            PasswordBox box = dp as PasswordBox;

            if (box == null)
            {
                return;
            }

            bool wasBound = (bool)(e.OldValue);
            bool needToBind = (bool)(e.NewValue);

            if (wasBound)
            {
                box.PasswordChanged -= HandlePasswordChanged;
            }

            if (needToBind)
            {
                box.PasswordChanged += HandlePasswordChanged;
            }
        }

        /// <summary>
        /// password changed event handler
        /// </summary>
        private static void HandlePasswordChanged(object sender, RoutedEventArgs e)
        {
            PasswordBox box = sender as PasswordBox;

            SetUpdatingPassword(box, true);
            SetBinding(box, box.Password);
            SetUpdatingPassword(box, false);
        }

        /// <summary>
        /// get function
        /// </summary>
        private static bool GetUpdatingPassword(DependencyObject dp)
        {
            return (bool)dp.GetValue(UpdatingPassword);
        }

        /// <summary>
        /// set function
        /// </summary>
        private static void SetUpdatingPassword(DependencyObject dp, bool value)
        {
            dp.SetValue(UpdatingPassword, value);
        }
        #endregion Methods
    }
}
