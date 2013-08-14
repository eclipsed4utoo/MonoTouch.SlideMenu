using MonoTouch.UIKit;
using System;

namespace MonoTouch.SlideMenu
{
	// This is actually an 'Objective-C category' in the original code.
	public static class Extensions
	{
		public static SlideMenuController SlideMenuController(this UIViewController controller) 
		{
			if (controller.Handle == IntPtr.Zero)
				return null;

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

		public static UINavigationController ParentNavController(this UIViewController controller)
		{
			UINavigationController navController = null;
			UIViewController parentViewController = controller.ParentViewController;

			if (controller is UINavigationController) {
				navController = controller as UINavigationController;
			}

			while (navController == null && parentViewController != null)
			{
				if (parentViewController is UINavigationController) {
					navController = parentViewController as UINavigationController;
				} else {
					parentViewController = parentViewController.ParentViewController;
				}
			}

			return navController;
		}
	}
}

