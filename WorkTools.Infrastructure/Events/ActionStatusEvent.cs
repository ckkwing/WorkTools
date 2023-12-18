using Prism.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WorkTools.Infrastructure.Events
{

    public class ActionStatusEventArgs : EventArgs
    {
        public enum ActionStatus
        {
            InProgress,
            Stopped
        }

        public ActionStatus Status { get; set; } = ActionStatus.Stopped;
    }

    public class ActionStatusEvent : PubSubEvent<ActionStatusEventArgs>
    {

    }
}
