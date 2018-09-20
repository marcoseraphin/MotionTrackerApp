using System;
using Xamarin.Forms;
using System.Threading.Tasks;

namespace MotionTrackerApp
{
	/// <summary>
	/// Navigation service to wrap all methods for passing into View Model
	/// and to navigate inside View Model
	/// </summary>
	public class NavigationService : Xamarin.Forms.INavigation
	{
		public INavigation Navi { get; internal set; }

		public Task<Page> PopAsync()
		{
			return Navi.PopAsync();
		}

		public Task<Page> PopModalAsync()
		{
			return Navi.PopModalAsync();
		}

		public Task PopToRootAsync()
		{
			return Navi.PopToRootAsync();
		}

		public Task PushAsync(Page page)
		{
			return Navi.PushAsync(page);
		}

		public Task PushModalAsync(Page page)
		{
			return Navi.PushModalAsync(page);
		}

		public void RemovePage (Page page)
		{
			Navi.RemovePage (page);
		}

		public void InsertPageBefore (Page page, Page before)
		{
			Navi.InsertPageBefore (page, before);
		}

		public Task PushAsync (Page page, bool animated)
		{
			return Navi.PushAsync (page, animated);
		}

		public Task<Page> PopAsync (bool animated)
		{
			return Navi.PopAsync (animated);
		}

		public Task PopToRootAsync (bool animated)
		{
			return Navi.PopToRootAsync (animated);
		}

		public Task PushModalAsync (Page page, bool animated)
		{
			return Navi.PushModalAsync (page, animated);
		}

		public Task<Page> PopModalAsync (bool animated)
		{
			return Navi.PopModalAsync (animated);
		}

		public System.Collections.Generic.IReadOnlyList<Page> NavigationStack {
			get 
			{
				return Navi.NavigationStack;
			}
		}

		public System.Collections.Generic.IReadOnlyList<Page> ModalStack {
			get 
			{
				return Navi.ModalStack;
			}
		}
	}
}

