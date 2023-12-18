using Prism.Events;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WorkTools.Infrastructure;
using WorkTools.Infrastructure.Events;

namespace WorkTools
{
    public class ShellViewModel : BaseViewModel
    {
        private bool inProgress = false;
        public bool InProgress
        {
            get
            {
                return inProgress;
            }

            private set
            {
                inProgress = value;
                RaisePropertyChanged("InProgress");
            }
        }

        public IEventAggregator EventAggregator { get; set; }

        [ImportingConstructor]
        public ShellViewModel(IEventAggregator eventAggregator)
        {
            EventAggregator = eventAggregator;
            EventAggregator.GetEvent<ActionStatusEvent>().Subscribe(OnActionStatusChanged, ThreadOption.UIThread);
        }

        ~ShellViewModel()
        {
            EventAggregator.GetEvent<ActionStatusEvent>().Unsubscribe(OnActionStatusChanged);
        }

        private void OnActionStatusChanged(ActionStatusEventArgs obj)
        {
            switch (obj.Status)
            {
                case ActionStatusEventArgs.ActionStatus.InProgress:
                    InProgress = true;
                    break;
                case ActionStatusEventArgs.ActionStatus.Stopped:
                    InProgress = false;
                    break;
            }
        }
    }
}
