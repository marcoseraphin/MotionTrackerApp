using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Xamarin.Forms;
using System.Collections.Generic;

namespace MotionTrackerApp
{
	public class LearnBrainPageViewModel : INotifyPropertyChanged
	{
		public event PropertyChangedEventHandler PropertyChanged;

		private NetCalc netCalculation = null;
		private Net currentNet = new Net();

		public int LayerCellCount 
		{
			get;
			set;
		}

		private List<PhoneMotion> phoneMotionList;
		public List<PhoneMotion> PhoneMotionList 
		{
			get 
			{
				return phoneMotionList;
			}
			set 
			{
				phoneMotionList = value;
			}
		}

		public LearnBrainPageViewModel ()
		{			
			netCalculation = new NetCalc();
			this.netCalculation.PublishNetError += new NetErrEventHandler(netCalculation_PublishNetError);
			this.netCalculation.PublishLearnCycle += new NetCycleEventHandler(netCalculation_PublishLearnCycle);
			//this.netCalculation.PublishLearnCyclePoints += new NetErrPointsEventHandler(netCalculation_PublishLearnCyclePoints);
		}

		/// <summary>
		/// Bindable property 
		/// </summary>
		private double netError;
		public double NetError 
		{
			get 
			{
				return netError;
			}
			set 
			{
				this.netError = value;
				OnPropertyChanged();
			}
		}

		/// <summary>
		/// Bindable property 
		/// </summary>
		private long learnCycle;
		public long LearnCycle 
		{
			get 
			{
				return learnCycle;
			}
			set 
			{
				this.learnCycle = value;
				OnPropertyChanged();
			}
		}

		/// <summary>
		/// Start Tracking command
		/// </summary>
		private Command<object> learnBrainCommand;

		/// <summary>
		/// Gets the calculate command
		/// </summary>
		/// <value>The calculate command.</value>
		public Command<object> LearnBrainCommand
		{
			get {
				return this.learnBrainCommand ?? (this.learnBrainCommand = new Command<object>(
					(param) =>
					{
						// Execute delegate
						this.LearnBrainExecute();
					},
					(param) =>
					{
						// CanExecute delegate
						return this.CanExecuteLearnBrainCommand(param);
					}));
			}
		}

		/// <summary>
		/// Can Execute method for LearnBrainCommand
		/// </summary>
		/// <returns><c>true</c> if this instance can execute calculate command the specified parameter; otherwise, <c>false</c>.</returns>
		/// <param name="parameter">Parameter.</param>
		public bool CanExecuteLearnBrainCommand (object parameter)
		{
			return true;
		}

		/// <summary>
		/// CalculateCommand execute method 
		/// </summary>
		private void LearnBrainExecute()
		{
			// Net initialize
			this.netCalculation.CurrentNet = this.currentNet;

			this.netCalculation.LearnRate = 0.89;
			this.netCalculation.Epsilon = 0.02;

			// Train net
			// Create sample pattern set
			//this.netCalculation.CreateSamplePatternSet();
			this.netCalculation.CreateMovingPatternSet(phoneMotionList);

			this.netCalculation.MakeNetStruc();

			// Train net
			this.StartTrain();
		}

		/// <summary>
		/// Start async train method
		/// </summary>
		private async void StartTrain()
		{
			await this.netCalculation.BackTrainAsync();
		}

		/// <summary>
		/// Display current net error
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		void netCalculation_PublishNetError(object sender, NetErrEventArgs e)
		{
			this.NetError = e.AllPatErr;
		}

		/// <summary>
		/// Display current learn cycle
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		void netCalculation_PublishLearnCycle(object sender, NetCycleEventArgs e)
		{
			this.LearnCycle = e.LearnCycle;
		}

		/// <summary>
		/// Raises the property changed event
		/// </summary>
		/// <param name="propertyName">Property name.</param>
		void OnPropertyChanged([CallerMemberName] string propertyName = null)
		{
			var handler = PropertyChanged;
			if (handler != null)
			{
				handler(this, new PropertyChangedEventArgs(propertyName));
			}
		}


	}
}

