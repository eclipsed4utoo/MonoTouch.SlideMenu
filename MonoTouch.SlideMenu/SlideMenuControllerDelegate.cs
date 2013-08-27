using System;
using MonoTouch.UIKit;

namespace MonoTouch.SlideMenu
{
	public class SlideMenuControllerDelegate : UINavigationControllerDelegate
	{
		SlideMenuController slideMenuController;

		public SlideMenuControllerDelegate(SlideMenuController controller)
		{
			slideMenuController = controller;
		}

		public override void WillShowViewController (UINavigationController navigationController, UIViewController viewController, bool animated)
		{

		}
	}
}

