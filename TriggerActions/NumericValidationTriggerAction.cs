using System;
using Xamarin.Forms;

namespace MotionTrackerApp
{
	/// <summary>
	/// Trigger action used in XAML code for TextChanged event
	/// </summary>
	public class NumericValidationTriggerAction : TriggerAction<Entry> 
	{
		protected override void Invoke (Entry entry)
		{
			double result;
			bool isValid = Double.TryParse (entry.Text, out result);
			entry.TextColor = isValid ? Color.Green : Color.Red;
		}
	}
}

