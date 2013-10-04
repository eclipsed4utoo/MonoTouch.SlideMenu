// WARNING
//
// This file has been generated automatically by Xamarin Studio to store outlets and
// actions made in the UI designer. If it is removed, they will be lost.
// Manual changes to this file may not be handled correctly.
//
using MonoTouch.Foundation;
using System.CodeDom.Compiler;

namespace MonoTouch.SlideMenu.Demo
{
	[Register ("DetailsViewController")]
	partial class DetailsViewController
	{
		[Outlet]
		MonoTouch.UIKit.UITextView SampleTextView { get; set; }

		[Outlet]
		MonoTouch.UIKit.UIButton ShowMenuButton { get; set; }
		
		void ReleaseDesignerOutlets ()
		{
			if (SampleTextView != null) {
				SampleTextView.Dispose ();
				SampleTextView = null;
			}

			if (ShowMenuButton != null) {
				ShowMenuButton.Dispose ();
				ShowMenuButton = null;
			}
		}
	}
}
