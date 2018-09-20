using System;
using System.Collections.Generic;

namespace MotionTrackerApp
{
	public class PhoneMotion
	{
		public Dictionary<int, List<double>> AccelerometerDict;
		public Dictionary<int, List<double>> GyroscopeDict;
					
		public PhoneMotion()
		{
			this.AccelerometerDict = new Dictionary<int, List<double>> ();
			this.GyroscopeDict = new Dictionary<int, List<double>> ();
		}
	}
}

