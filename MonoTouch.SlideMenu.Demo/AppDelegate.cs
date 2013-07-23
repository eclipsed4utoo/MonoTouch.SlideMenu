
using MonoTouch.Foundation;
using MonoTouch.UIKit;
using System;

namespace MonoTouch.SlideMenu.Demo
{
	[Register ("AppDelegate")]
	public partial class AppDelegate : UIApplicationDelegate
	{
		UIWindow window;
		SlideMenuController _slideController;

		public override bool FinishedLaunching (UIApplication app, NSDictionary options)
		{
			window = new UIWindow (UIScreen.MainScreen.Bounds);

			MenuViewController menuViewController = new MenuViewController(UITableViewStyle.Grouped);
			DetailsViewController detailsViewController = new DetailsViewController();

			SlideMenuController slideMenuViewController = new SlideMenuController(menuViewController, detailsViewController);

			slideMenuViewController.SetLeftBarButtonForContentView (new UIBarButtonItem (UIBarButtonSystemItem.Bookmarks));
			slideMenuViewController.LeftBarButtonClicked += HandleLeftBarButtonClicked;

			_slideController = slideMenuViewController;
			UINavigationController cont = new UINavigationController (slideMenuViewController);
			window.RootViewController = cont;

			window.BackgroundColor = UIColor.White;
			window.MakeKeyAndVisible ();			
			return true;
		}

		private void HandleLeftBarButtonClicked(object sender, EventArgs e)
		{
			_slideController.ToggleMenuAnimated ();
		}
	}
}

