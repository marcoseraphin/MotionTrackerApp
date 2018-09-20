using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using Xamarin.Forms;
using DeviceMotion.Plugin;
using DeviceMotion.Plugin.Abstractions;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace MotionTrackerApp
{
	public class MotionTrackerViewModel : INotifyPropertyChanged
	{
		/// <summary>
		/// The navigation service from MainPage (see parameter in ctor)
		/// </summary>
		NavigationService navigationService = null;

		/// <summary>
		/// Property changed event for binding
		/// </summary>
		public event PropertyChangedEventHandler PropertyChanged;

		private List<PhoneMotion> phoneMotionList = new List<PhoneMotion> ();

		private int moveID = 0;

		private DateTime TimerStart;

		private bool canLearnBrain;
		public bool CanLearnBrain 
		{
			get 
			{
				return canLearnBrain;
			}
			set 
			{
				this.canLearnBrain = value;
				OnPropertyChanged();
			}
		}

		private bool canStartMove1;
		public bool CanStartMove1 
		{
			get 
			{
				return canStartMove1;
			}
			set 
			{
				this.canStartMove1 = value;
				OnPropertyChanged();
			}
		}

		private bool canStartMove2;
		public bool CanStartMove2
		{
			get 
			{
				return canStartMove2;
			}
			set 
			{
				this.canStartMove2 = value;
				OnPropertyChanged();
			}
		}

		private bool canStartMove3;
		public bool CanStartMove3 
		{
			get 
			{
				return canStartMove3;
			}
			set 
			{
				this.canStartMove3 = value;
				OnPropertyChanged();
			}
		}

		private bool canStartMove4;
		public bool CanStartMove4 
		{
			get 
			{
				return canStartMove4;
			}
			set 
			{
				this.canStartMove4 = value;
				OnPropertyChanged();
			}
		}

		private bool canStartMove5;
		public bool CanStartMove5 
		{
			get 
			{
				return canStartMove5;
			}
			set 
			{
				this.canStartMove5 = value;
				OnPropertyChanged();
			}
		}

		private Color backColor;
		public Color BackColor 
		{
			get 
			{
				return backColor;
			}
			set 
			{
				this.backColor = value;
				OnPropertyChanged();
			}
		}

		/// <summary>
		/// Bindable property MotionValue 1 for label in XAML
		/// </summary>
		private double valAcc1;
		public double ValAcc1 
		{
			get 
			{
				return valAcc1;
			}
			set 
			{
				this.valAcc1 = value;
				OnPropertyChanged();
			}
		}

		/// <summary>
		/// Bindable property MotionValue 2 for label in XAML
		/// </summary>
		private double valAcc2;
		public double ValAcc2 
		{
			get 
			{
				return valAcc2;
			}
			set 
			{
				this.valAcc2 = value;
				OnPropertyChanged();
			}
		}

		/// <summary>
		/// Bindable property MotionValue 3 for label in XAML
		/// </summary>
		private double valAcc3;
		public double ValAcc3 
		{
			get 
			{
				return valAcc3;
			}
			set 
			{
				this.valAcc3 = value;
				OnPropertyChanged();
			}
		}

		/// <summary>
		/// Bindable property MotionValue 1 for label in XAML
		/// </summary>
		private double valGyro1;
		public double ValGyro1 
		{
			get 
			{
				return valGyro1;
			}
			set 
			{
				this.valGyro1 = value;
				OnPropertyChanged();
			}
		}

		/// <summary>
		/// Bindable property MotionValue 2 for label in XAML
		/// </summary>
		private double valGyro2;
		public double ValGyro2 
		{
			get 
			{
				return valGyro2;
			}
			set 
			{
				this.valGyro2 = value;
				OnPropertyChanged();
			}
		}

		/// <summary>
		/// Bindable property MotionValue 3 for label in XAML
		/// </summary>
		private double valGyro3;
		public double ValGyro3 
		{
			get 
			{
				return valGyro3;
			}
			set 
			{
				this.valGyro3 = value;
				OnPropertyChanged();
			}
		}

		/// <summary>
		/// Start Tracking command
		/// </summary>
		private Command<object> startTrackingCommand;

		/// <summary>
		/// Gets the calculate command
		/// </summary>
		/// <value>The calculate command.</value>
		public Command<object> StartTrackingCommand
		{
			get {
				return this.startTrackingCommand ?? (this.startTrackingCommand = new Command<object>(
					(param) =>
					{
						// Execute delegate
						this.StartTrackingExecute();
					},
					(param) =>
					{
						// CanExecute delegate
						return this.CanExecuteStartTrackingCommand(param);
					}));
			}
		}

		/// <summary>
		/// Stop Tracking command
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

		private Command<object> startMoveCommand;
		public Command<object> StartMoveCommand
		{
			get {
				return this.startMoveCommand ?? (this.startMoveCommand = new Command<object>(
					(param) =>
					{
						// Execute delegate
						this.StartMoveExecute();
					},
					(param) =>
					{
						// CanExecute delegate
						return this.CanExecuteStartMoveCommand(param);
					}));
			}
		}



		/// <summary>
		/// Constructor with Navigation wrapper service class for using Xamarin.Forms navigation inside view model
		/// </summary>
		/// <param name="navigation">Navigation.</param>
		public MotionTrackerViewModel (NavigationService navigation)
		{			
			this.BackColor = Color.Red;
			this.CanLearnBrain = true;
			this.CanStartMove1 = false;
			this.CanStartMove2 = false;
			this.CanStartMove3 = false;
			this.CanStartMove4 = false;
			this.CanStartMove5 = false;

			this.navigationService = navigation;
		}
			
		/// <summary>
		/// Can Execute method for StartTrackingCommand
		/// </summary>
		/// <returns><c>true</c> if this instance can execute calculate command the specified parameter; otherwise, <c>false</c>.</returns>
		/// <param name="parameter">Parameter.</param>
		public bool CanExecuteStartTrackingCommand (object parameter)
		{
			return true;
		}

		/// <summary>
		/// Can Execute method for StopTrackingCommand
		/// </summary>
		/// <returns><c>true</c> if this instance can execute calculate command the specified parameter; otherwise, <c>false</c>.</returns>
		/// <param name="parameter">Parameter.</param>
		public bool CanExecuteLearnBrainCommand (object parameter)
		{
			return true;
			//return this.CanLearnBrain;
		}

		public bool CanExecuteStartMoveCommand (object parameter)
		{
			if ((this.CanStartMove1 == true) || (this.CanStartMove2 == true) ||
			    (this.CanStartMove3 == true) || (this.CanStartMove4 == true) ||
			    (this.CanStartMove5 == true)) 
			{
				return true;
			}

			return false;
		}

		/// <summary>
		/// CalculateCommand execute method 
		/// </summary>
		private async void StartTrackingExecute()
		{
			this.phoneMotionList.Clear();

			DependencyService.Get<ITextToSpeech>().Speak("Move with your phone 5 times as similar as possible, each time for 3 seconds");
			await Task.Delay (5000);

			this.moveID = 1;
			this.CanStartMove1 = true;
			this.SetButtonStatus();
		}

		private async void StartMoveExecute()
		{
			DependencyService.Get<ITextToSpeech> ().Speak ("Start your move number " + this.moveID + "now");	
			await Task.Delay (2000);

			this.BackColor = Color.Green;

			this.TrackMove();
		}

//		private async void StartTrackingAsync()
//		{
//			await this.TrainAsync();
//		}
//
//		public Task TrainAsync()
//		{
//			return Task.Run(() => TrackMove());
//		}

		private void TrackMove()
		{
			PhoneMotion phoneMotion = new PhoneMotion ();
			int takeValueCount = 0;

			phoneMotion.AccelerometerDict.Clear();
			phoneMotion.GyroscopeDict.Clear();

			List<double> XValueListAcc = new List<double> ();
			List<double> YValueListAcc = new List<double> ();
			List<double> ZValueListAcc = new List<double> ();

			List<double> XValueListGyro = new List<double> ();
			List<double> YValueListGyro = new List<double> ();
			List<double> ZValueListGyro = new List<double> ();

			CrossDeviceMotion.Current.Start (MotionSensorType.Accelerometer, MotionSensorDelay.Default);
			CrossDeviceMotion.Current.Start (MotionSensorType.Gyroscope, MotionSensorDelay.Default);
			//CrossDeviceMotion.Current.Start (MotionSensorType.Gyroscope, MotionSensorDelay.Default);

			CrossDeviceMotion.Current.SensorValueChanged += (s, a)=>
			{
				takeValueCount++;

				if ((takeValueCount % 4) == 0)
				{
				switch(a.SensorType)
				{
				case MotionSensorType.Accelerometer:

					this.ValAcc1 = ((MotionVector)a.Value).X;
					this.ValAcc2 = ((MotionVector)a.Value).Y;
					this.ValAcc3 = ((MotionVector)a.Value).Z;	

					XValueListAcc.Add(this.ValAcc1);
					YValueListAcc.Add(ValAcc2);
					ZValueListAcc.Add(this.ValAcc3);				
					break;

				case MotionSensorType.Gyroscope:

					this.ValGyro1 = ((MotionVector)a.Value).X;
					this.ValGyro2 = ((MotionVector)a.Value).Y;
					this.ValGyro3 = ((MotionVector)a.Value).Z;	

					XValueListGyro.Add(this.ValGyro1);
					YValueListGyro.Add(this.ValGyro2);
					ZValueListGyro.Add(this.ValGyro3);	
					break;
				}
			  }
			};

			this.TimerStart = DateTime.Now;

			Device.StartTimer(TimeSpan.FromSeconds(3), () =>
				{
					if (this.TimerStart.AddSeconds(3.0) < DateTime.Now)
					{
						CrossDeviceMotion.Current.Stop (MotionSensorType.Accelerometer);
						CrossDeviceMotion.Current.Stop (MotionSensorType.Gyroscope);


						this.BackColor = Color.Red;

						phoneMotion.AccelerometerDict.Add(0, XValueListAcc);
						phoneMotion.AccelerometerDict.Add(1, YValueListAcc);
						phoneMotion.AccelerometerDict.Add(2, ZValueListAcc);

						phoneMotion.GyroscopeDict.Add(0, XValueListGyro);
						phoneMotion.GyroscopeDict.Add(1, YValueListGyro);
						phoneMotion.GyroscopeDict.Add(2, ZValueListGyro);

						//List<double> XListAcc = this.AccelerometerDict[1];
						//List<double> XListGyro = this.GyroscopeDict[1];

						this.phoneMotionList.Add (phoneMotion);

						DependencyService.Get<ITextToSpeech>().Speak("Stop moving");

						this.moveID = this.moveID + 1;

						this.SetButtonStatus();

						return false;
					}

					return true;
				}); 

		
		}

		private void SetButtonStatus()
		{
			this.CanStartMove1 = false;
			this.CanStartMove2 = false;
			this.CanStartMove3 = false;
			this.CanStartMove4 = false;
			this.CanStartMove5 = false;

			switch (this.moveID) 
			{
			case 1:
				this.CanStartMove1 = true;
				break;
			case 2:
				this.CanStartMove1 = false;
				this.CanStartMove2 = true;
				break;
			case 3:
				this.CanStartMove1 = false;
				this.CanStartMove2 = false;
				this.CanStartMove3 = true;
				break;
			case 4:
				this.CanStartMove1 = false;
				this.CanStartMove2 = false;
				this.CanStartMove3 = false;
				this.CanStartMove4 = true;
				break;
			case 5:
				this.CanStartMove1 = false;
				this.CanStartMove2 = false;
				this.CanStartMove3 = false;
				this.CanStartMove4 = false;
				this.CanStartMove5 = true;
				break;


			default:
				this.CanStartMove1 = false;
				this.CanStartMove2 = false;
				this.CanStartMove3 = false;
				this.CanStartMove4 = false;
				this.CanStartMove5 = false;
				break;
			}

			if (this.moveID == 6) 
			{
				this.CanLearnBrain = true;
			}
		}

		/// <summary>
		/// Execute learn command
		/// </summary>
		private void LearnBrainExecute()
		{ 
//			LearnBrainPageViewModel learnBrainPageViewModel = new LearnBrainPageViewModel (this.phoneMotionList);
//			LearnBrainPage learnBrainPage = new LearnBrainPage ();
//			learnBrainPage.BindingContext = learnBrainPageViewModel;
//			

			App.LearnBrainPageViewModel.PhoneMotionList = this.phoneMotionList;

			this.navigationService.PushAsync (App.LearnBrainPage);
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

