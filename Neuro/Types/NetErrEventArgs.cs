using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MotionTrackerApp
{
    /// <summary>
    /// Delegate for signaling net error events
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    public delegate void NetErrEventHandler(object sender, NetErrEventArgs e);

    /// <summary>
    /// Net error event arguments
    /// </summary>
    public class NetErrEventArgs : EventArgs
    {
        public double AllPatErr { get; set; }
    }
}
