using System;
using MonoTouch.UIKit;
using MonoTouch.CoreAnimation;
using System.Drawing;
using System.Collections.Generic;
using System.Linq;
using MonoTouch.Foundation;
using System.Reflection;

namespace MonoTouch.SlideMenu
{
	public class SlideMenuController : UIViewController
	{
		private float _widthOfPortraitContentViewVisible = 44.0f;
		private float _widthOfLandscapeContentViewVisible = 44.0f;

		const float ANIMATION_DURATION = 0.3f;

		UIViewController leftMenuViewController;
		UIViewController rightMenuViewController;

		UIViewController contentViewController;

		RectangleF contentViewControllerFrame;
		bool leftMenuWasOpenAtPanBegin;
		bool rightMenuWasOpenAtPanBegin;
		bool leftBarButtonClicked = false;
		bool rightBarButtonClicked = false;
		bool shouldResizeLeftMenuView = false;
		bool shouldResizeRightMenuView = false;


		// When the menu is hidden, does the pan gesture trigger ? Default is true.
		bool _panEnabledWhenSlideMenuIsHidden;

		public event EventHandler LeftBarButtonClicked;
		private void OnLeftBarButtonClicked()
		{
			if (LeftBarButtonClicked != null)
			{
				LeftBarButtonClicked (this, EventArgs.Empty);
			}
		}

		public event EventHandler RightBarButtonClicked;
		private void OnRightBarButtonClicked()
		{
			if (RightBarButtonClicked != null)
			{
				RightBarButtonClicked (this, EventArgs.Empty);
			}
		}

		public event EventHandler<SlideMenuBarButtonEventArgs> AdditionalLeftBarButtonClicked;
		private void OnAdditionalLeftBarButtonClicked(int index)
		{
			if (AdditionalLeftBarButtonClicked != null) {
				AdditionalLeftBarButtonClicked (this, new SlideMenuBarButtonEventArgs (index));
			}
		}

		public event EventHandler<SlideMenuBarButtonEventArgs> AdditionalRightBarButtonClicked;
		private void OnAdditionalRightBarButtonClicked(int index)
		{
			if (AdditionalRightBarButtonClicked != null) {
				AdditionalRightBarButtonClicked (this, new SlideMenuBarButtonEventArgs (index));
			}
		}

		public UIBarButtonItem SlideControllerLeftBarButton
		{
			get
			{
				UIBarButtonItem item = null;
				if (contentViewController != null)
				{
					item = this.NavigationItem.LeftBarButtonItem;
				}
				return item;
			}
		}

		public UIBarButtonItem SlideControllerRightBarButton
		{
			get
			{
				UIBarButtonItem item = null;
				if (contentViewController != null)
				{
					item = this.NavigationItem.LeftBarButtonItem;
				}
				return item;
			}
		}

		public float WidthOfPortraitContentViewVisible
		{
			get { return _widthOfPortraitContentViewVisible; }
			set { _widthOfPortraitContentViewVisible = value; }
		}

		public float WidthOfLandscapeContentViewVisible
		{
			get { return _widthOfLandscapeContentViewVisible; }
			set { _widthOfLandscapeContentViewVisible = value; }
		}

		public bool AllowGestureToOpenMenu
		{
			get { return _panEnabledWhenSlideMenuIsHidden; }
			set { _panEnabledWhenSlideMenuIsHidden = value; }
		}

		public SlideMenuController (UIViewController leftMenuViewController, UIViewController contentViewController)
		{
			this.SetLeftMenuViewController(leftMenuViewController);
			this.SetContentViewController(contentViewController);

			_panEnabledWhenSlideMenuIsHidden = true;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="MonoTouch.SlideMenu.SlideMenuController"/> class.
		/// </summary>
		/// <param name="leftMenuViewController">Left menu view controller. If no left menu, use null.</param>
		/// <param name="rightMenuViewController">Right menu view controller. If no right menu, use null.</param>
		/// <param name="contentViewController">Content view controller.</param>
		public SlideMenuController (UIViewController leftMenuViewController, UIViewController rightMenuViewController, UIViewController contentViewController)
			: this(leftMenuViewController, contentViewController)
		{
			this.SetRightMenuViewController (rightMenuViewController);
		}

		protected override void Dispose (bool disposing)
		{
			if (disposing) {

				if (leftMenuViewController != null) {
					leftMenuViewController.Dispose();
					leftMenuViewController = null;
				}

				if (rightMenuViewController != null) {
					rightMenuViewController.Dispose ();
					rightMenuViewController = null;
				}

				if (contentViewController != null) {
					contentViewController.Dispose();
					contentViewController = null;
				}
			}

			base.Dispose (disposing);
		}

		public void SetLeftBarButtonForController(UIBarButtonItem item)
		{
			if (item == null)
				throw new ArgumentNullException ("item cannot be null");

			if (this.NavigationItem.LeftBarButtonItem != null)
			{
				this.NavigationItem.LeftBarButtonItem.Clicked -= HandleLeftBarButtonClicked;

				if (this.NavigationItem.LeftBarButtonItem.CustomView is UIButton) {
					var button = this.NavigationItem.LeftBarButtonItem.CustomView as UIButton;
					button.TouchUpInside -= HandleLeftBarButtonClicked;
				}

				this.NavigationItem.LeftBarButtonItem.Dispose ();
				this.NavigationItem.LeftBarButtonItem = null;
			}

			contentViewController.NavigationItem.LeftBarButtonItem = item;
			this.NavigationItem.LeftBarButtonItem = item;

			if (this.NavigationItem.LeftBarButtonItem.CustomView is UIButton) {
				var button = this.NavigationItem.LeftBarButtonItem.CustomView as UIButton;
				button.TouchUpInside += HandleLeftBarButtonClicked;
			}

			this.NavigationItem.LeftBarButtonItem.Clicked += HandleLeftBarButtonClicked;
		}

		public void SetRightBarButtonForController(UIBarButtonItem item)
		{
			if (item == null)
				throw new ArgumentNullException ("item cannot be null");

			if (this.NavigationItem.RightBarButtonItem != null)
			{
				this.NavigationItem.RightBarButtonItem.Clicked -= HandleRightBarButtonClicked;

				if (this.NavigationItem.RightBarButtonItem.CustomView is UIButton) {
					var button = this.NavigationItem.RightBarButtonItem.CustomView as UIButton;
					button.TouchUpInside -= HandleRightBarButtonClicked;
				}

				this.NavigationItem.RightBarButtonItem.Dispose ();
				this.NavigationItem.RightBarButtonItem = null;
			}

			contentViewController.NavigationItem.RightBarButtonItem = item;
			this.NavigationItem.RightBarButtonItem = item;

			if (this.NavigationItem.RightBarButtonItem.CustomView is UIButton) {
				var button = this.NavigationItem.RightBarButtonItem.CustomView as UIButton;
				button.TouchUpInside += HandleRightBarButtonClicked;
			}

			this.NavigationItem.RightBarButtonItem.Clicked += HandleRightBarButtonClicked;
		}

		public int AddAdditionalLeftBarButton(UIBarButtonItem item)
		{
			if (item == null)
				throw new ArgumentNullException("item cannot be null");

			item.Clicked += AdditionalLeftBarButton_Clicked;

			if (item.CustomView is UIButton) {
				var button = item.CustomView as UIButton;
				button.TouchUpInside += AdditionalLeftBarButton_Clicked;
			}

			List<UIBarButtonItem> items = this.NavigationItem.LeftBarButtonItems.ToList ();
			items.Add (item);
			this.NavigationItem.LeftBarButtonItems = items.ToArray();

			return this.NavigationItem.LeftBarButtonItems.Length - 1;
		}

		public int AddAdditionalRightBarButton(UIBarButtonItem item)
		{
			if (item == null)
				throw new ArgumentNullException ("item cannot be null");

			item.Clicked += AdditionalRightBarButton_Clicked;

			if (item.CustomView is UIButton) {
				var button = item.CustomView as UIButton;
				button.TouchUpInside += AdditionalRightBarButton_Clicked;
			}

			List<UIBarButtonItem> items = this.NavigationItem.RightBarButtonItems.ToList ();

			int index = items.Count;

			if (this.NavigationItem.RightBarButtonItem != null) {
				var temp = this.NavigationItem.RightBarButtonItem;

				items.Add (item);

				this.NavigationItem.RightBarButtonItem = temp;
			}

			this.NavigationItem.RightBarButtonItems = items.ToArray ();

			return index;
		}

		public void AddAdditionalLeftBarButtons (List<UIBarButtonItem> items)
		{
			if (items == null)
				return;

			// insert the current left bar button as the first time
			if (this.NavigationItem.LeftBarButtonItem != null) {
				items.Insert (0, this.NavigationItem.LeftBarButtonItem);
			}

			this.NavigationItem.LeftBarButtonItems = items.ToArray();
		}

		public void AddAdditionalRightBarButtons(List<UIBarButtonItem> items)
		{
			if (items == null)
				return;

			if (this.NavigationItem.RightBarButtonItem != null) {
				items.Add (this.NavigationItem.RightBarButtonItem);
			}

			this.NavigationItem.RightBarButtonItems = items.ToArray ();
		}

		public void RemoveAdditionalLeftBarButton(int index)
		{
			if (index < 0)
				return;

			List<UIBarButtonItem> items = this.NavigationItem.LeftBarButtonItems.ToList ();

			var item = items.ElementAtOrDefault (index);
			if (item != null)
			{
				items.RemoveAt(index);

				item.Clicked -= AdditionalLeftBarButton_Clicked;

				if (item.CustomView is UIButton) {
					var button = item.CustomView as UIButton;
					button.TouchUpInside -= AdditionalLeftBarButton_Clicked;
				}

				item.Dispose ();
			}

			if (this.NavigationItem.LeftBarButtonItem != null) {
				items.RemoveAt (0);
			}

			AddAdditionalLeftBarButtons (items);
		}

		public void RemoveAdditionalRightBarButton(int index)
		{
			if (index < 0)
				return;

			List<UIBarButtonItem> items = this.NavigationItem.RightBarButtonItems.ToList ();

			var item = items.ElementAtOrDefault (index);
			if (item != null)
			{
				items.Remove (item);

				if (items.Count == 1) {
					this.NavigationItem.RightBarButtonItem = items [0];
				}
				else if (items.Count > 1){
					this.NavigationItem.RightBarButtonItem = items [items.Count - 1];
				}

				item.Clicked -= AdditionalRightBarButton_Clicked;

				if (item.CustomView is UIButton) {
					var button = item.CustomView as UIButton;
					button.TouchUpInside -= AdditionalRightBarButton_Clicked;
				}

				item.Dispose ();
			}

			if (this.NavigationItem.RightBarButtonItem != null && items.Count > 0) {
				items.RemoveAt (items.Count - 1);
			}

			AddAdditionalRightBarButtons (items);
		}

		private void AdditionalLeftBarButton_Clicked(object sender, EventArgs e)
		{
			var button = sender as UIBarButtonItem;
			int index = this.NavigationItem.LeftBarButtonItems.ToList ().IndexOf (button);
			OnAdditionalLeftBarButtonClicked (index);
		}

		private void AdditionalRightBarButton_Clicked(object sender, EventArgs e)
		{
			var button = sender as UIBarButtonItem;
			int index = this.NavigationItem.RightBarButtonItems.ToList ().IndexOf (button);
			OnAdditionalRightBarButtonClicked (index);
		}

		private void HandleLeftBarButtonClicked (object sender, EventArgs e)
		{
			if (leftMenuViewController == null)
				return;

			leftBarButtonClicked = true;
			rightBarButtonClicked = false;
			ToggleLeftMenuAnimated();
			OnLeftBarButtonClicked ();

			if (this.NavigationItem.LeftBarButtonItems != null && this.NavigationItem.LeftBarButtonItems.Length > 1) {
				OnAdditionalLeftBarButtonClicked (0);
			}
		}

		private void HandleRightBarButtonClicked (object sender, EventArgs e)
		{
			if (rightMenuViewController == null)
				return;

			rightBarButtonClicked = true;
			leftBarButtonClicked = false;
			ToggleRightMenuAnimated();
			OnRightBarButtonClicked ();

			if (this.NavigationItem.RightBarButtonItems != null && this.NavigationItem.RightBarButtonItems.Length > 1) {
				OnAdditionalRightBarButtonClicked (0);
			}
		}

		// - (void)setleftMenuViewController:(UIViewController *)leftMenuViewController
		public void SetLeftMenuViewController (UIViewController controller)
		{
			if (leftMenuViewController != controller) {

				if (leftMenuViewController != null) {
					leftMenuViewController.WillMoveToParentViewController (null);
					leftMenuViewController.RemoveFromParentViewController ();
					leftMenuViewController.Dispose ();
				}

				leftMenuViewController = controller;
				AddChildViewController (leftMenuViewController);
				leftMenuViewController.DidMoveToParentViewController (this);
			}
		}

		public void SetRightMenuViewController(UIViewController controller)
		{
			if (rightMenuViewController != controller) {

				if (rightMenuViewController != null) {
					rightMenuViewController.WillMoveToParentViewController (null);
					rightMenuViewController.RemoveFromParentViewController ();
					rightMenuViewController.Dispose ();
				}

				rightMenuViewController = controller;

				if (controller != null)
				{
					AddChildViewController (rightMenuViewController);
					rightMenuViewController.DidMoveToParentViewController (this);
				}
			}
		}

		// - (void)setContentViewController:(UIViewController *)contentViewController
		public void SetContentViewController (UIViewController controller)
		{
			if (contentViewController != controller) 
			{
				if (contentViewController != null) {
					contentViewController.WillMoveToParentViewController(null);
					contentViewController.RemoveFromParentViewController();
					contentViewController.Dispose();
				}

				contentViewController = controller;
				AddChildViewController(contentViewController);
				contentViewController.DidMoveToParentViewController(this);
			}
		}

		// - (void)setShadowOnContentViewControllerView
		public void SetShadowOnContentViewControllerView ()
		{
			UIView contentView = contentViewController.View;
			CALayer layer = contentView.Layer;
			layer.MasksToBounds = false;
			layer.ShadowColor = UIColor.Black.CGColor;
			layer.ShadowOpacity = 1.0f;
			layer.ShadowOffset = new SizeF(-2.5f, 0.0f);
			layer.ShadowRadius = 5.0f;
			layer.ShadowPath = UIBezierPath.FromRect(contentView.Bounds).CGPath;
		}

		// #pragma mark - View lifecycle

		// - (void)viewDidLoad
		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();

			View.AddSubview(contentViewController.View);
			SetShadowOnContentViewControllerView();

			contentViewController.View.AddGestureRecognizer(TapGesture);
			TapGesture.Enabled = false;

			contentViewController.View.AddGestureRecognizer(PanGesture);
			PanGesture.Enabled = _panEnabledWhenSlideMenuIsHidden;
		}

		// - (void)viewWillAppear:(BOOL)animated
		public override void ViewWillAppear (bool animated)
		{
			base.ViewWillAppear (animated);

			if (!IsLeftMenuOpen () && !IsRightMenuOpen()) {
				contentViewController.View.Frame = View.Bounds;
			}

			contentViewController.BeginAppearanceTransition (true, animated);
			if (leftMenuViewController != null && leftMenuViewController.IsViewLoaded && leftMenuViewController.View.Superview != null) {
				leftMenuViewController.BeginAppearanceTransition (true, animated);
			}

			if (rightMenuViewController != null && rightMenuViewController.IsViewLoaded && rightMenuViewController.View.Superview != null) {
				rightMenuViewController.BeginAppearanceTransition (true, animated);
			}
		}

		// - (void)viewDidAppear:(BOOL)animated
		public override void ViewDidAppear (bool animated)
		{
			base.ViewDidAppear (animated);

			contentViewController.EndAppearanceTransition();
			if (leftMenuViewController != null && leftMenuViewController.IsViewLoaded && leftMenuViewController.View.Superview != null) {
				leftMenuViewController.EndAppearanceTransition();
			}

			if (rightMenuViewController != null && rightMenuViewController.IsViewLoaded && rightMenuViewController.View.Superview != null) {
				rightMenuViewController.EndAppearanceTransition();
			}
		}

		// - (void)viewWillDisappear:(BOOL)animated
		public override void ViewWillDisappear (bool animated)
		{
			base.ViewWillDisappear (animated);

			contentViewController.BeginAppearanceTransition (false, animated);
			if (leftMenuViewController != null && leftMenuViewController.IsViewLoaded) {
				leftMenuViewController.BeginAppearanceTransition(false, animated);
			}

			if (rightMenuViewController != null && rightMenuViewController.IsViewLoaded) {
				rightMenuViewController.BeginAppearanceTransition(false, animated);
			}
		}

		// - (void)viewDidDisappear:(BOOL)animated
		public override void ViewDidDisappear (bool animated)
		{
			base.ViewDidDisappear (animated);

			contentViewController.EndAppearanceTransition ();
			if (leftMenuViewController != null && leftMenuViewController.IsViewLoaded) {
				leftMenuViewController.EndAppearanceTransition();
			}

			if (rightMenuViewController != null && rightMenuViewController.IsViewLoaded) {
				rightMenuViewController.EndAppearanceTransition();
			}
		}

		// #pragma mark - Appearance & rotation callbacks


		// - (BOOL)shouldAutomaticallyForwardAppearanceMethods
		public override bool ShouldAutomaticallyForwardAppearanceMethods {
			get {
				return true;
			}
		}

		// - (BOOL)shouldAutomaticallyForwardRotationMethods
		public override bool ShouldAutomaticallyForwardRotationMethods {
			get {
				return true;
			}
		}

		// - (BOOL)automaticallyForwardAppearanceAndRotationMethodsToChildViewControllers
		[Obsolete]
		public override bool AutomaticallyForwardAppearanceAndRotationMethodsToChildViewControllers {
			get {
				return false;
			}
		}

		// #pragma mark - Rotation

		// - (BOOL)shouldAutorotate
		public override bool ShouldAutorotate ()
		{
			if (leftMenuViewController != null && rightMenuViewController != null)
			{
				return leftMenuViewController.ShouldAutorotate() && rightMenuViewController.ShouldAutorotate() && contentViewController.ShouldAutorotate();
			}
			else if (leftMenuViewController != null && rightMenuViewController == null)
			{
				return leftMenuViewController.ShouldAutorotate() && contentViewController.ShouldAutorotate();
			}
			else if (leftMenuViewController == null && rightMenuViewController != null)
			{
				return rightMenuViewController.ShouldAutorotate() && contentViewController.ShouldAutorotate();
			}
			else
			{
				return contentViewController.ShouldAutorotate ();
			}
		}

		// - (NSUInteger)supportedInterfaceOrientations
		public override UIInterfaceOrientationMask GetSupportedInterfaceOrientations ()
		{
			if (leftMenuViewController != null && rightMenuViewController != null)
			{
				return (leftMenuViewController.GetSupportedInterfaceOrientations()) & 
					(rightMenuViewController.GetSupportedInterfaceOrientations()) &
						contentViewController.GetSupportedInterfaceOrientations();
			}
			else if (leftMenuViewController != null && rightMenuViewController == null)
			{
				return (leftMenuViewController.GetSupportedInterfaceOrientations()) & 
						contentViewController.GetSupportedInterfaceOrientations();
			}
			else if (leftMenuViewController == null && rightMenuViewController != null)
			{
				return (rightMenuViewController.GetSupportedInterfaceOrientations()) &
						contentViewController.GetSupportedInterfaceOrientations();
			}
			else
			{
				return contentViewController.GetSupportedInterfaceOrientations ();
			}
		}

		// - (BOOL)shouldAutorotateToInterfaceOrientation:(UIInterfaceOrientation)toInterfaceOrientation
		[Obsolete]
		public override bool ShouldAutorotateToInterfaceOrientation (UIInterfaceOrientation toInterfaceOrientation)
		{
			if (leftMenuViewController != null && rightMenuViewController != null)
			{
				return (leftMenuViewController.ShouldAutorotateToInterfaceOrientation(toInterfaceOrientation)) && 
					(rightMenuViewController.ShouldAutorotateToInterfaceOrientation(toInterfaceOrientation)) &&
						contentViewController.ShouldAutorotateToInterfaceOrientation(toInterfaceOrientation);
			}
			else if (leftMenuViewController != null && rightMenuViewController == null)
			{
				return (leftMenuViewController.ShouldAutorotateToInterfaceOrientation(toInterfaceOrientation)) && 
						contentViewController.ShouldAutorotateToInterfaceOrientation(toInterfaceOrientation);
			}
			else if (leftMenuViewController == null && rightMenuViewController != null)
			{
				return (rightMenuViewController.ShouldAutorotateToInterfaceOrientation(toInterfaceOrientation)) &&
						contentViewController.ShouldAutorotateToInterfaceOrientation(toInterfaceOrientation);
			}
			else
			{
				return contentViewController.ShouldAutorotateToInterfaceOrientation (toInterfaceOrientation);
			}
		}

		// - (void)willAnimateRotationToInterfaceOrientation:(UIInterfaceOrientation)toInterfaceOrientation duration:(NSTimeInterval)duration
		public override void WillAnimateRotation (UIInterfaceOrientation toInterfaceOrientation, double duration)
		{
			shouldResizeLeftMenuView = true;
			shouldResizeRightMenuView = true;

			if (IsLeftMenuOpen () || IsRightMenuOpen()) {
				RectangleF frame = contentViewController.View.Frame;
				frame.X = OffsetXWhenMenuIsOpen();
				UIView.Animate(duration, () => {
					contentViewController.View.Frame = frame;
				});
			}
		}


		// #pragma mark - Menu view lazy load


		// - (void)loadleftMenuViewControllerViewIfNeeded
		public void LoadLeftMenuViewControllerViewIfNeeded ()
		{
			if (leftMenuViewController != null && leftMenuViewController.View.Superview == null) {
				RectangleF menuFrame = View.Bounds;
				menuFrame.Width -= GetWidthOfContent(); 
				leftMenuViewController.View.Frame = menuFrame;
				View.InsertSubview (leftMenuViewController.View, 0);
			}
			else if (shouldResizeLeftMenuView) {
				RectangleF menuFrame = View.Bounds;
				menuFrame.Width -= GetWidthOfContent(); 
				leftMenuViewController.View.Frame = menuFrame;
			}

			shouldResizeLeftMenuView = false;
		}

		public void LoadRightMenuViewControllerViewIfNeeded()
		{
			if (rightMenuViewController != null && rightMenuViewController.View.Superview == null) {
				RectangleF menuFrame = View.Bounds;
				menuFrame.Width -= GetWidthOfContent();
				menuFrame.X += GetWidthOfContent();
				rightMenuViewController.View.Frame = menuFrame;
				View.InsertSubview (rightMenuViewController.View, 0);
			}
			else if (shouldResizeRightMenuView) {
				RectangleF menuFrame = View.Bounds;
				menuFrame.Width -= GetWidthOfContent();
				menuFrame.X += GetWidthOfContent();
				rightMenuViewController.View.Frame = menuFrame;
			}

			shouldResizeRightMenuView = false;
		}


		// #pragma mark - Navigation


		// - (void)setContentViewController:(UIViewController *)contentViewController animated:(BOOL)animated completion:(void(^)(BOOL finished))completion
		public void SetContentViewControllerAnimated (UIViewController controller, bool animated, UICompletionHandler completion)
		{
			if (controller == null)
				throw new InvalidOperationException ("Can't show a null content view controller");

			if (contentViewController != controller) 
			{
				// Preserve the frame
				UIBarButtonItem temp = null;

				if (this.NavigationItem.RightBarButtonItem != null) {
					temp = this.NavigationItem.RightBarButtonItem;
				}

				this.NavigationItem.RightBarButtonItems = new UIBarButtonItem[0];

				if (temp != null) {
					this.NavigationItem.RightBarButtonItem = temp;
				}

				RectangleF frame = contentViewController.View.Frame;

				// Remove old content view
				contentViewController.View.RemoveGestureRecognizer(TapGesture);
				contentViewController.View.RemoveGestureRecognizer(PanGesture);
				contentViewController.BeginAppearanceTransition(false, false);
				contentViewController.View.RemoveFromSuperview();
				contentViewController.EndAppearanceTransition();

				// Add the new content view
				SetContentViewController(controller);
				//contentViewController = controller;
				contentViewController.View.Frame = frame;
				contentViewController.View.AddGestureRecognizer(TapGesture);
				contentViewController.View.AddGestureRecognizer(PanGesture);
				SetShadowOnContentViewControllerView();
				contentViewController.BeginAppearanceTransition(true, false);
				View.AddSubview(contentViewController.View);
				contentViewController.EndAppearanceTransition();
			}

			// Perform the show animation
			ShowContentViewControllerAnimated(animated, completion);
		}

		// - (void)showContentViewControllerAnimated:(BOOL)animated completion:(void(^)(BOOL finished))completion
		public void ShowContentViewControllerAnimated (bool animated, UICompletionHandler completion)
		{
			bool isLeftMenuOpen = IsLeftMenuOpen ();
			bool isRightMenuOpen = IsRightMenuOpen ();
			// Remove gestures
			TapGesture.Enabled = false;
			PanGesture.Enabled = _panEnabledWhenSlideMenuIsHidden;

			if (isLeftMenuOpen) {
				leftMenuViewController.View.UserInteractionEnabled = false;
			}

			if (isRightMenuOpen) {
				rightMenuViewController.View.UserInteractionEnabled = false;
			}

			var duration = animated ? ANIMATION_DURATION : 0;

			UIView contentView = contentViewController.View;
			RectangleF contentViewFrame = contentView.Frame;
			contentViewFrame.X = 0;

			if (isLeftMenuOpen) {
				leftMenuViewController.BeginAppearanceTransition (false, animated);
			}

			if (isRightMenuOpen) {
				rightMenuViewController.BeginAppearanceTransition (false, animated);
			}

			UIView.AnimateNotify(duration, 0, UIViewAnimationOptions.CurveEaseInOut, () => {
				contentView.Frame = contentViewFrame;
			}, (finished) => {

				if (isLeftMenuOpen) {
					leftMenuViewController.EndAppearanceTransition ();
					leftMenuViewController.View.UserInteractionEnabled = true;
				}

				if (isRightMenuOpen) {
					rightMenuViewController.EndAppearanceTransition();
					rightMenuViewController.View.UserInteractionEnabled = true;
				}

				if (leftMenuViewController != null && leftMenuViewController.View.Hidden) {
					leftMenuViewController.View.Hidden = false;
				}

				if (rightMenuViewController != null && rightMenuViewController.View.Hidden) {
					rightMenuViewController.View.Hidden = false;
				}

				if (completion != null) {
					completion (finished);
				}
			});
		}

		// - (IBAction)toggleMenuAnimated:(id)sender;
		public void ToggleLeftMenuAnimated()
		{
			if (IsRightMenuOpen())
			{
				ShowContentViewControllerAnimated (true, (finished) => {

					if (leftMenuViewController != null && leftMenuViewController.View.Hidden)
						leftMenuViewController.View.Hidden = false;

					if (rightMenuViewController != null && rightMenuViewController.View.Hidden)
						rightMenuViewController.View.Hidden = false;

					if (rightMenuViewController != null)
						rightMenuViewController.View.Hidden = true;

					ShowLeftMenuAnimated (true, null);

					leftBarButtonClicked = false;
				});
			}
			else if (IsLeftMenuOpen()){
				ShowContentViewControllerAnimated (true, (finished) => {

					if (rightMenuViewController != null && rightMenuViewController.View.Hidden)
						rightMenuViewController.View.Hidden = false;

					leftBarButtonClicked = false;
				});
			} else {

				if (rightMenuViewController != null)
					rightMenuViewController.View.Hidden = true;

				ShowLeftMenuAnimated (true, null);

				leftBarButtonClicked = false;
			}
		}

		public void ToggleRightMenuAnimated()
		{
			if (IsLeftMenuOpen())
			{
				ShowContentViewControllerAnimated (true, (finished) => {

					if (leftMenuViewController != null && leftMenuViewController.View.Hidden)
						leftMenuViewController.View.Hidden = false;

					if (rightMenuViewController != null && rightMenuViewController.View.Hidden)
						rightMenuViewController.View.Hidden = false;

					if (leftMenuViewController != null)
						leftMenuViewController.View.Hidden = true;

					ShowRightMenuAnimated (true, null);

					rightBarButtonClicked = false;
				});
			}
			else if (IsRightMenuOpen()){
				ShowContentViewControllerAnimated (true, (finished) => {

					if (leftMenuViewController != null && leftMenuViewController.View.Hidden)
						leftMenuViewController.View.Hidden = false;

					if (rightMenuViewController != null && rightMenuViewController.View.Hidden)
						rightMenuViewController.View.Hidden = false;

					rightBarButtonClicked = false;
				});
			} else {

				if (leftMenuViewController != null)
					leftMenuViewController.View.Hidden = true;

				ShowRightMenuAnimated (true, null);

				rightBarButtonClicked = false;
			}
		}

		// - (void)showMenuAnimated:(BOOL)animated completion:(void(^)(BOOL finished))completion;

		public void ShowLeftMenuAnimated(bool animated, UICompletionHandler completion)
		{
			if (leftMenuViewController == null)
				return;

			var duration = animated ? ANIMATION_DURATION : 0;

			UIView contentView = contentViewController.View;
			RectangleF contentViewFrame = contentView.Frame;
			contentViewFrame.X = OffsetXWhenMenuIsOpen();

			LoadLeftMenuViewControllerViewIfNeeded();
			leftMenuViewController.BeginAppearanceTransition(true, true);

			UIView.AnimateNotify(duration, 0, UIViewAnimationOptions.CurveEaseInOut, () => {
				contentView.Frame = contentViewFrame;
			}, (finished) => {
				leftMenuViewController.EndAppearanceTransition ();

				TapGesture.Enabled = true;
				PanGesture.Enabled = true;

				if (completion != null) {
					completion (finished);
				}
			});
		}

		public void ShowRightMenuAnimated(bool animated, UICompletionHandler completion)
		{
			if (rightMenuViewController == null)
				return;

			var duration = animated ? ANIMATION_DURATION : 0;

			UIView contentView = contentViewController.View;
			RectangleF contentViewFrame = contentView.Frame;
			contentViewFrame.X = OffsetXWhenMenuIsOpen ();

			LoadRightMenuViewControllerViewIfNeeded();
			rightMenuViewController.BeginAppearanceTransition(true, true);

			UIView.AnimateNotify(duration, 0, UIViewAnimationOptions.CurveEaseInOut, () => {
				contentView.Frame = contentViewFrame;
			}, (finished) => {
				rightMenuViewController.EndAppearanceTransition ();

				TapGesture.Enabled = true;
				PanGesture.Enabled = true;

				if (completion != null) {
					completion (finished);
				}
			});
		}


		// #pragma mark - Gestures

		UITapGestureRecognizer _tapGesture;

		public UITapGestureRecognizer TapGesture {
			get {
				if (_tapGesture == null) {
					_tapGesture = new UITapGestureRecognizer(TapGestureTriggered);
				}
				return _tapGesture;
			}
		}

		void TapGestureTriggered()
		{
			if (TapGesture.State == UIGestureRecognizerState.Ended) {
				ShowContentViewControllerAnimated (true, (finished) => {

					if (leftMenuViewController != null && leftMenuViewController.View.Hidden)
						leftMenuViewController.View.Hidden = false;

					if (rightMenuViewController != null && rightMenuViewController.View.Hidden)
						rightMenuViewController.View.Hidden = false;

				});
			}
		}

		UIPanGestureRecognizer _panGesture;

		public UIPanGestureRecognizer PanGesture {
			get {
				if (_panGesture == null) {
					_panGesture = new UIPanGestureRecognizer(PanGestureTriggered);
					_panGesture.RequireGestureRecognizerToFail(TapGesture);
				}
				return _panGesture;
			}
		}

		private float VelocityX
		{
			get { return PanGesture.VelocityInView (PanGesture.View).X; }
		}

		private bool IsPanningLeft()
		{
			PointF velocity = PanGesture.VelocityInView (PanGesture.View);
			return velocity.X > 0;
		}

		private bool IsPanningRight()
		{
			PointF velocity = PanGesture.VelocityInView (PanGesture.View);
			return velocity.X < 0;
		}

		void PanGestureTriggered ()
		{
			if (IsPanningLeft () && !IsRightMenuOpen() && !CanOpenLeftMenu ())
				return;

			if (IsPanningRight () && !IsLeftMenuOpen() && !CanOpenRightMenu ())
				return;

			if (PanGesture.State == UIGestureRecognizerState.Began) {
				contentViewControllerFrame = contentViewController.View.Frame;
				leftMenuWasOpenAtPanBegin = IsLeftMenuOpen ();
				rightMenuWasOpenAtPanBegin = IsRightMenuOpen ();

				if (!leftMenuWasOpenAtPanBegin && IsPanningLeft() && !rightMenuWasOpenAtPanBegin && leftMenuViewController != null) {
					LoadLeftMenuViewControllerViewIfNeeded (); // Menu is closed, load it if needed
					contentViewController.View.EndEditing(true); // Dismiss any open keyboards.
					leftMenuViewController.BeginAppearanceTransition (true, true); // Menu is appearing
				}

				if (!rightMenuWasOpenAtPanBegin && IsPanningRight() && !leftMenuWasOpenAtPanBegin && rightMenuViewController != null) 
				{
					LoadRightMenuViewControllerViewIfNeeded ();
					contentViewController.View.EndEditing (true);
					rightMenuViewController.BeginAppearanceTransition (true, true);
				}

				if (IsPanningLeft() && !IsRightMenuOpen() && rightMenuViewController != null && rightMenuViewController.View.Superview != null)
				{
					rightMenuViewController.View.Hidden = true;
				} 
				else if (IsPanningRight() && !IsLeftMenuOpen() && leftMenuViewController != null && leftMenuViewController.View.Superview != null)
				{
					leftMenuViewController.View.Hidden = true;
				}
			}

			PointF translation = PanGesture.TranslationInView (PanGesture.View);

			RectangleF frame = contentViewControllerFrame;

			frame.X += translation.X;

			float offsetXWhenMenuIsOpen = OffsetXWhenMenuIsOpen ();

			if (IsPanningLeft() && !rightMenuWasOpenAtPanBegin)
			{
				if (frame.X < 0) {
					frame.X = 0;
				} else if (frame.X > offsetXWhenMenuIsOpen) {
					frame.X = offsetXWhenMenuIsOpen;
				}
			}
			else if (IsPanningRight() && !leftMenuWasOpenAtPanBegin)
			{
				if (frame.X > 0) {
					frame.X = 0;
				} else if (frame.X < offsetXWhenMenuIsOpen) {
					frame.X = offsetXWhenMenuIsOpen;
				}
			}

			PanGesture.View.Frame = frame;

			if (PanGesture.State == UIGestureRecognizerState.Ended) {
				PointF velocity = PanGesture.VelocityInView(PanGesture.View);

				if (IsPanningLeft() && !rightMenuWasOpenAtPanBegin)
				{
					frame = LeftMenuOpen (velocity, frame, offsetXWhenMenuIsOpen);
				}
				else if (IsPanningLeft() && rightMenuWasOpenAtPanBegin)
				{
					frame = RightMenuClose (velocity, frame);

					if (leftMenuViewController != null)
						leftMenuViewController.View.Hidden = false;
				}
				else if (IsPanningRight() && !leftMenuWasOpenAtPanBegin)
				{
					frame = RightMenuOpen (velocity, frame, offsetXWhenMenuIsOpen);
				}
				else if (IsPanningRight() && leftMenuWasOpenAtPanBegin)
				{
					frame = LeftMenuClose (velocity, frame);

					if (rightMenuViewController != null)
						rightMenuViewController.View.Hidden = false;
				}

				contentViewControllerFrame = frame;
			}
		}

		private RectangleF LeftMenuOpen(PointF velocity, RectangleF frame, float offsetXWhenMenuIsOpen)
		{
			float distance = 0;
			double animationDuration = 0;

			distance = Math.Abs(offsetXWhenMenuIsOpen - frame.X);
			animationDuration = Math.Abs(distance / velocity.X);
			if (animationDuration > ANIMATION_DURATION){
				animationDuration = ANIMATION_DURATION;
			}

			frame.X = offsetXWhenMenuIsOpen;
			UIView.AnimateNotify(animationDuration, 0, UIViewAnimationOptions.CurveEaseInOut, () => {
				contentViewController.View.Frame = frame;
			}, (finished) => {
				TapGesture.Enabled = true;

				if (!leftMenuWasOpenAtPanBegin && leftMenuViewController != null){
					leftMenuViewController.EndAppearanceTransition();
				}
			});

			return frame;
		}

		private RectangleF LeftMenuClose(PointF velocity, RectangleF frame)
		{
			float distance = 0;
			double animationDuration = 0;

			// Compute animation duration
			distance = frame.X;
			animationDuration = Math.Abs(distance / velocity.X);

			if (animationDuration > ANIMATION_DURATION) {
				animationDuration = ANIMATION_DURATION;
			}

			// Remove gestures
			TapGesture.Enabled = false;
			PanGesture.Enabled = _panEnabledWhenSlideMenuIsHidden;

			frame.X = 0;

			if (!leftMenuWasOpenAtPanBegin) {
				leftMenuViewController.EndAppearanceTransition();
			}

			leftMenuViewController.BeginAppearanceTransition(false, true);

			UIView.AnimateNotify(animationDuration, 0, UIViewAnimationOptions.CurveEaseInOut, () => {
				contentViewController.View.Frame = frame;
			}, (finished) => {

				leftMenuViewController.EndAppearanceTransition();
			});

			return frame;
		}

		private RectangleF RightMenuOpen(PointF velocity, RectangleF frame, float offsetXWhenMenuIsOpen)
		{
			float distance = 0;
			double animationDuration = 0;

			distance = offsetXWhenMenuIsOpen - frame.X;
			animationDuration = Math.Abs(distance / velocity.X);
			if (animationDuration > ANIMATION_DURATION){
				animationDuration = ANIMATION_DURATION;
			}

			frame.X = offsetXWhenMenuIsOpen;
			UIView.AnimateNotify(animationDuration, 0, UIViewAnimationOptions.CurveEaseInOut, () => {
				contentViewController.View.Frame = frame;
			}, (finished) => {
				TapGesture.Enabled = true;

				if (!rightMenuWasOpenAtPanBegin){
					rightMenuViewController.EndAppearanceTransition();
				}
			});

			return frame;
		}

		private RectangleF RightMenuClose(PointF velocity, RectangleF frame)
		{
			float distance = 0;
			double animationDuration = 0;

			// Compute animation duration
			distance = frame.X;
			animationDuration = Math.Abs(distance / velocity.X);

			if (animationDuration > ANIMATION_DURATION) {
				animationDuration = ANIMATION_DURATION;
			}

			// Remove gestures
			TapGesture.Enabled = false;
			PanGesture.Enabled = _panEnabledWhenSlideMenuIsHidden;

			frame.X = 0;

			if (!rightMenuWasOpenAtPanBegin) {
				rightMenuViewController.EndAppearanceTransition();
			}

			rightMenuViewController.BeginAppearanceTransition(false, true);

			UIView.AnimateNotify(animationDuration, 0, UIViewAnimationOptions.CurveEaseInOut, () => {
				contentViewController.View.Frame = frame;
			}, (finished) => {
				rightMenuViewController.EndAppearanceTransition();
			});

			return frame;
		}

		// - (BOOL)isMenuOpen;

		public bool IsLeftMenuOpen()
		{
			if (leftMenuViewController == null)
				return false;

			return contentViewController.View.Frame.X > 0;
		}

		public bool IsRightMenuOpen ()
		{
			if (rightMenuViewController == null)
				return false;

			return contentViewController.View.Frame.X < 0;
		}

		public bool CanOpenLeftMenu()
		{
			return leftMenuViewController != null;
		}

		public bool CanOpenRightMenu()
		{
			return rightMenuViewController != null;
		}

		private float GetWidthOfContent()
		{
			if (UIApplication.SharedApplication.StatusBarOrientation == UIInterfaceOrientation.Portrait ||
			    UIApplication.SharedApplication.StatusBarOrientation == UIInterfaceOrientation.PortraitUpsideDown)
			{
				return _widthOfPortraitContentViewVisible;
			}
			else
			{
				return _widthOfLandscapeContentViewVisible;
			}
		}

		// - (CGFloat)offsetXWhenMenuIsOpen
		public float OffsetXWhenMenuIsOpen ()
		{
			float offset = 0f;

			if (this.VelocityX == 0)
			{
				if (leftBarButtonClicked)
					offset = View.Bounds.Width - GetWidthOfContent();
				else {
					if (IsLeftMenuOpen()) {
						offset = View.Bounds.Width - GetWidthOfContent();
					}
					else {
						offset = -(View.Bounds.Width - GetWidthOfContent());
					}
				}
			}
			else
			{
				if (IsPanningLeft())
					offset = View.Bounds.Width - GetWidthOfContent();
				else 
					offset = -(View.Bounds.Width - GetWidthOfContent());
			}

			return offset;
		}

	}
}
