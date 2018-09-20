using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xamarin.Forms;

namespace MotionTrackerApp
{
    /// <summary>
    /// Delegate for signaling net error events
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    public delegate void NetErrPointsEventHandler(object sender, NetErrPointsEventArgs e);

    /// <summary>
    /// Net error event arguments
    /// </summary>
    public class NetErrPointsEventArgs : EventArgs
    {
        public List<Point> pointListOutput { get; set; }
        public List<Point> pointListAim { get; set; }
    }
}
