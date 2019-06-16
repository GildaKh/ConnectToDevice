using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Timers;

namespace ConnectToDevice.Model.TimerMngmt
{
    internal class ManualTimerAsync : ManualTimer
    {
        internal override void OnTimedEvent(object source, ElapsedEventArgs e)
        {
            UnSetTimer();
            base.OnTimedEvent(source, e);
        }
    }
}
