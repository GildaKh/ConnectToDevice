using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Timers;

namespace ConnectToDevice.Model.TimerMngmt
{
    internal class ManualTimerSync : ManualTimer
    {
        internal override void OnTimedEvent(object source, ElapsedEventArgs e)
        {
            UnSetTimer();
            EndofTimer = true;
            base.OnTimedEvent(source, e);
        }
    }
}
