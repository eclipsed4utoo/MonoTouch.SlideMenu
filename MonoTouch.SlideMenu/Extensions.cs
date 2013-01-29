using MonoTouch.UIKit;

namespace MonoTouch.SlideMenu
{
	// This is actually an 'Objective-C category' in the original code.
	public static class Extensions
	{
		public static SlideMenuController SlideMenuController(this UIViewController controller) 
		{
			SlideMenuController slideMenuController = null;
			UIViewController parentViewController = controller.ParentViewController;

			while(slideMenuController == null && parentViewController != null) 
			{
				if (parentViewController is SlideMenuController) {
					slideMenuController = (SlideMenuController)parentViewController;
				} else {
					parentViewController = parentViewController.ParentViewController;
				}
			}

			return slideMenuController;
		}
	}
}

