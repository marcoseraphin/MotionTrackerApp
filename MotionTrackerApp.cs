using System;

using Xamarin.Forms;

namespace MotionTrackerApp
{
	public class App : Application
	{
		public static LearnBrainPageViewModel LearnBrainPageViewModel { get; set; }
		public static Page LearnBrainPage { get; private set; }

		public App ()
		{
			// Set navigation service wrapper for ViewModel and create ViewModel
			NavigationService navigationSrv = new NavigationService();
			MotionTrackerViewModel viewModel = new MotionTrackerViewModel(navigationSrv);

			// Second page
			LearnBrainPageViewModel = new LearnBrainPageViewModel();
			LearnBrainPage = new NavigationPage(new LearnBrainPage());
			LearnBrainPage.BindingContext = LearnBrainPageViewModel;

			// The root page of application
			MotionTrackerPage startPage = new MotionTrackerPage();

			// Set Binding Context for Calculator page to View Model
			startPage.BindingContext = viewModel;

			MainPage = new NavigationPage(startPage);

			// Set navigation service using MainPage navigation
			navigationSrv.Navi = MainPage.Navigation;
		}

		protected override void OnStart ()
		{
			// Handle when your app starts
		}

		protected override void OnSleep ()
		{
			// Handle when your app sleeps
		}

		protected override void OnResume ()
		{
			// Handle when your app resumes
		}
	}
}

