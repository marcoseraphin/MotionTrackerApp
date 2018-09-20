using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MotionTrackerApp
{
    /// <summary>
    /// Delegate for signaling learn cycle events
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    public delegate void NetCycleEventHandler(object sender, NetCycleEventArgs e);

    /// <summary>
    /// Learn cycle event arguments
    /// </summary>
    public class NetCycleEventArgs : EventArgs
    {
        public Int64 LearnCycle { get; set; }
    }
}
