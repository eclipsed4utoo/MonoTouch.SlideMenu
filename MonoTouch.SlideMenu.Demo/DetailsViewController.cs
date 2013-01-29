
using MonoTouch.UIKit;
using MonoTouch.SlideMenu;
using System;

namespace MonoTouch.SlideMenu.Demo
{
	public partial class DetailsViewController : UIViewController
	{
		public DetailsViewController () : base ("DetailsViewController", null)
		{
		}
		
		public override void DidReceiveMemoryWarning ()
		{
			base.DidReceiveMemoryWarning ();
		}
		
		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();

			if (this.SlideMenuController() != null)
			{
				var menuButton = new UIBarButtonItem(UIBarButtonSystemItem.Rewind, this, null);
				menuButton.Clicked += delegate (object sender, EventArgs e) {
					this.SlideMenuController ().ToggleMenuAnimated ();
				};

				NavigationItem.LeftBarButtonItem = menuButton;
			}

			ShowMenuButton.TouchUpInside += (object sender, EventArgs e) => {
				var slideMenuController = this.SlideMenuController();
				if (slideMenuController != null) {
					slideMenuController.ShowMenuAnimated(true, null);
				}
			};
		}

		[Obsolete]
		public override void ViewDidUnload ()
		{
			base.ViewDidUnload ();
			ReleaseDesignerOutlets ();
		}

		[Obsolete]
		public override bool ShouldAutorotateToInterfaceOrientation (UIInterfaceOrientation toInterfaceOrientation)
		{
			// Return true for supported orientations
			return (toInterfaceOrientation != UIInterfaceOrientation.PortraitUpsideDown);
		}
	}
}

