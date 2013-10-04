using System;
using MonoTouch.UIKit;

namespace MonoTouch.SlideMenu.Demo
{
	public partial class MenuViewController : UITableViewController
	{
		public MenuViewController(UITableViewStyle tableViewStyle) : base(tableViewStyle) 
		{
		}
		
		public override void DidReceiveMemoryWarning ()
		{
			base.DidReceiveMemoryWarning ();
		}
		
		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();
			TableView.Source = new MenuViewControllerTableViewSource(this);
			ClearsSelectionOnViewWillAppear = false;
		}

		public override void ViewWillAppear (bool animated)
		{
			base.ViewWillAppear (animated);

			if (this.NavigationController != null)
			{
				this.NavigationController.SetNavigationBarHidden (true, false);
				this.NavigationController.SetToolbarHidden (false, false);
			}

			TableView.ScrollToNearestSelected(UITableViewScrollPosition.None, animated);
		}

		[Obsolete]
		public override bool ShouldAutorotateToInterfaceOrientation (UIInterfaceOrientation toInterfaceOrientation)
		{
			return (toInterfaceOrientation != UIInterfaceOrientation.PortraitUpsideDown);
		}
	}
}

