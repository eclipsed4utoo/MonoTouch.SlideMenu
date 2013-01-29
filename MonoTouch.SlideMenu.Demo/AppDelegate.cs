
using MonoTouch.Foundation;
using MonoTouch.UIKit;

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

			SlideMenuController slideMenuViewController = new SlideMenuController(menuViewController, detailsViewController);

			window.RootViewController = slideMenuViewController;

			window.BackgroundColor = UIColor.White;
			window.MakeKeyAndVisible ();			
			return true;
		}
	}
}

