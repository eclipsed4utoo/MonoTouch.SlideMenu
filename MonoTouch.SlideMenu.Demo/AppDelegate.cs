
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
			

			SlideMenuController slideMenuViewController = new SlideMenuController();
			UINavigationController cont = new UINavigationController (slideMenuViewController);
			slideMenuViewController.SetContentViewController (detailsViewController);


			slideMenuViewController.SetLeftMenuViewController (menuViewController);

			if (UIDevice.CurrentDevice.UserInterfaceIdiom == UIUserInterfaceIdiom.Pad) {
				slideMenuViewController.WidthOfPortraitContentViewVisible = 300f;
				slideMenuViewController.WidthOfLandscapeContentViewVisible = 556f;
			}

			window.RootViewController = cont;

			window.BackgroundColor = UIColor.White;
			window.MakeKeyAndVisible ();			
			return true;
		}
	}
}

