using System;
using Xamarin.Forms;
using System.Globalization;
using System.Diagnostics;

namespace MotionTrackerApp
{
	/// <summary>
	/// Converts an Entry's Text.Length into a 'flag'
	///  * Entry is empty, returns 1
	/// 
	/// </summary>
	public class MultiTriggerConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{		
			if (value == null) 
			{
				return false;
			}

			double result;
			bool isValid = Double.TryParse (value.ToString().Trim().Replace(",","."), out result); 

			if (isValid) 
			{
				if ((double)result >= 0)
					return true;	// data has been entered
			    else
					return false;	// input is empty
			}

			return false;
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new NotSupportedException ();
		}
	}
}

