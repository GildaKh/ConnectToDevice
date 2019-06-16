using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConnectToDevice.Model.TimerMngmt
{
    internal abstract class ManualTimer
    {
        internal System.Timers.Timer TimeoutTimer { get; set; }
        internal bool EndofTimer;

        internal delegate void TimeoutDelegate();
        internal event TimeoutDelegate Timeout;

        internal ManualTimer()
        {

        }
        internal void InitTimer()
        {
            if (TimeoutTimer == null)
            {
                TimeoutTimer = new System.Timers.Timer();
                TimeoutTimer.Elapsed += OnTimedEvent;
            }
        }
        internal void SetTimer(int second)
        {
            if (second == 0)
            {
                if (second == 0 || second > 120)
                    second = 120;
            }
            EndofTimer = false;
            TimeoutTimer.Interval = second * 1000;
            TimeoutTimer.Enabled = true;
        }
        internal void UnSetTimer()
        {
            if (TimeoutTimer != null)
            {
                TimeoutTimer.Enabled = false;
                TimeoutTimer.Stop();
            }
        }
        internal void ResetTimer()
        {
            if (TimeoutTimer != null)
            {
                TimeoutTimer.Stop();
                TimeoutTimer.Start();
            }
        }
        internal virtual void OnTimedEvent(object source, System.Timers.ElapsedEventArgs e)
        {
            if (Timeout != null) Timeout();
        }   
    }
}
