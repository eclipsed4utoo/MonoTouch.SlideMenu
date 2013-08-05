using System;

namespace MonoTouch.SlideMenu
{
	public class SlideMenuBarButtonEventArgs : EventArgs
	{
		public int ButtonIndex { get; set; }

		public SlideMenuBarButtonEventArgs (int buttonIndex)
		{
			this.ButtonIndex = buttonIndex;
		}
	}
}

