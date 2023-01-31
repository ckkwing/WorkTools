using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;

namespace WorkTools.Infrastructure
{
    public class BaseViewModel : BindableBase
    {
        private object obj = new object();
        protected void RunOnUIThread(Action action)
        {
            if (null == action || Application.Current == null)
            {
                return;
            }

            try
            {
                Dispatcher dispatcher = Application.Current.Dispatcher;
                bool isSameThread = false;
                lock (obj)
                {
                    isSameThread = dispatcher.CheckAccess();
                }

                if (isSameThread)
                {
                    action();
                }
                else
                {
                    dispatcher.Invoke((Delegate)(action));
                }
            }
            catch (Exception ex)
            {
                //LogHelper.UILogger.Debug(string.Format("RunOnUIThread error:{0}", ex.Message));
            }
        }

        protected void RunOnUIThreadAsync(Action action)
        {
            if (null == action || Application.Current == null)
            {
                return;
            }

            try
            {
                Dispatcher dispatcher = Application.Current.Dispatcher;
                bool isSameThread = false;
                lock (obj)
                {
                    isSameThread = dispatcher.CheckAccess();
                }

                if (isSameThread)
                {
                    action();
                }
                else
                {
                    dispatcher.BeginInvoke(DispatcherPriority.Normal, (Delegate)(action));
                }
            }
            catch (Exception ex)
            {
                //LogHelper.UILogger.Debug(string.Format("RunOnUIThreadAsync error:{0}", ex.Message));
            }
        }
    }
}
