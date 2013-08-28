
using MonoTouch.Foundation;
using MonoTouch.UIKit;
using System;

namespace MonoTouch.SlideMenu.Demo
{
	[Register ("AppDelegate")]
	public partial class AppDelegate : UIApplicationDelegate
	{
		UIWindow window;

		public override bool FinishedLaunching (UIApplication app, NSDictionary options)
		{
			window = new UIWindow (UIScreen.MainScreen.Bounds);

			MenuViewController menuViewController = new MenuViewController(UITableViewStyle.Grouped);
			DetailsViewController detailsViewController = new DetailsViewController();
			UINavigationController navController = new UINavigationController (detailsViewController);

			SlideMenuController slideMenuViewController = new SlideMenuController();
			slideMenuViewController.SetContentViewController (navController);
			slideMenuViewController.SetLeftMenuViewController (menuViewController);
			slideMenuViewController.SetRightMenuViewController (new MenuViewController (UITableViewStyle.Plain));

			if (UIDevice.CurrentDevice.UserInterfaceIdiom == UIUserInterfaceIdiom.Pad) {
				slideMenuViewController.WidthOfPortraitContentViewVisible = 300f;
				slideMenuViewController.WidthOfLandscapeContentViewVisible = 556f;
			}

			window.RootViewController = slideMenuViewController;

			window.BackgroundColor = UIColor.White;
			window.MakeKeyAndVisible ();			
			return true;
		}
	}
}

