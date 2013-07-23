using System;
using MonoTouch.UIKit;
using MonoTouch.CoreAnimation;
using System.Drawing;

namespace MonoTouch.SlideMenu
{
	public class SlideMenuController : UIViewController
	{
		private float _widthOfContentViewVisible = 44.0f;
		const float ANIMATION_DURATION = 0.3f;

		UIViewController menuViewController;
		UIViewController contentViewController;

		RectangleF contentViewControllerFrame;
		bool menuWasOpenAtPanBegin;

		// When the menu is hidden, does the pan gesture trigger ? Default is true.
		bool panEnabledWhenSlideMenuIsHidden;

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

		public float WidthofContentViewVisible
		{
			get { return _widthOfContentViewVisible; }
			set { _widthOfContentViewVisible = value; }
		}

		public SlideMenuController (UIViewController menuViewController, UIViewController contentViewController)
		{
			//this.menuViewController = menuViewController;
			//this.contentViewController = contentViewController;

			this.SetMenuViewController(menuViewController);
			this.SetContentViewController(contentViewController);

			panEnabledWhenSlideMenuIsHidden = true;
		}

		protected override void Dispose (bool disposing)
		{
			if (disposing) {
				menuViewController.Dispose();
				menuViewController = null;
				
				contentViewController.Dispose();
				contentViewController = null;
			}

			base.Dispose (disposing);
		}

		public void SetLeftBarButtonForController(UIBarButtonItem item)
		{
			if (this.NavigationItem.LeftBarButtonItem != null)
			{
				this.NavigationItem.LeftBarButtonItem.Clicked -= HandleLeftBarButtonClicked;
				this.NavigationItem.LeftBarButtonItem.Dispose ();
				this.NavigationItem.LeftBarButtonItem = null;
			}

			contentViewController.NavigationItem.LeftBarButtonItem = item;
			this.NavigationItem.LeftBarButtonItem = item;
			this.NavigationItem.LeftBarButtonItem.Clicked += HandleLeftBarButtonClicked;
		}

		public void SetRightBarButtonForController(UIBarButtonItem item)
		{
			if (this.NavigationItem.RightBarButtonItem != null)
			{
				this.NavigationItem.RightBarButtonItem.Clicked -= HandleRightBarButtonClicked;
				this.NavigationItem.RightBarButtonItem.Dispose ();
				this.NavigationItem.RightBarButtonItem = null;
			}

			contentViewController.NavigationItem.RightBarButtonItem = item;
			this.NavigationItem.RightBarButtonItem = item;
			this.NavigationItem.RightBarButtonItem.Clicked += HandleRightBarButtonClicked;
		}

		private void HandleLeftBarButtonClicked (object sender, EventArgs e)
		{
			OnLeftBarButtonClicked ();
		}

		private void HandleRightBarButtonClicked (object sender, EventArgs e)
		{
			OnRightBarButtonClicked ();
		}

		// - (void)setMenuViewController:(UIViewController *)menuViewController
		public void SetMenuViewController (UIViewController controller)
		{
			if (menuViewController != controller) {

				if (menuViewController != null) {
					menuViewController.WillMoveToParentViewController (null);
					menuViewController.RemoveFromParentViewController ();
					menuViewController.Dispose ();
				}

				menuViewController = controller;
				AddChildViewController (menuViewController);
				menuViewController.DidMoveToParentViewController (this);
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
			PanGesture.Enabled = panEnabledWhenSlideMenuIsHidden;
		}

		// - (void)viewWillAppear:(BOOL)animated
		public override void ViewWillAppear (bool animated)
		{
			base.ViewWillAppear (animated);

			if (!IsMenuOpen ()) {
				contentViewController.View.Frame = View.Bounds;
			}

			contentViewController.BeginAppearanceTransition (true, animated);
			if (menuViewController.IsViewLoaded && menuViewController.View.Superview != null) {
				menuViewController.BeginAppearanceTransition (true, animated);
			}
		}

		// - (void)viewDidAppear:(BOOL)animated
		public override void ViewDidAppear (bool animated)
		{
			base.ViewDidAppear (animated);

			contentViewController.EndAppearanceTransition();
			if (menuViewController.IsViewLoaded && menuViewController.View.Superview != null) {
				menuViewController.EndAppearanceTransition();
			}
		}

		// - (void)viewWillDisappear:(BOOL)animated
		public override void ViewWillDisappear (bool animated)
		{
			base.ViewWillDisappear (animated);

			contentViewController.BeginAppearanceTransition (false, animated);
			if (menuViewController.IsViewLoaded) {
				menuViewController.BeginAppearanceTransition(false, animated);
			}
		}

		// - (void)viewDidDisappear:(BOOL)animated
		public override void ViewDidDisappear (bool animated)
		{
			base.ViewDidDisappear (animated);

			contentViewController.EndAppearanceTransition ();
			if (menuViewController.IsViewLoaded) {
				menuViewController.EndAppearanceTransition();
			}
		}

		// #pragma mark - Appearance & rotation callbacks


		// - (BOOL)shouldAutomaticallyForwardAppearanceMethods
		public override bool ShouldAutomaticallyForwardAppearanceMethods {
			get {
				return false;
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
			return menuViewController.ShouldAutorotate() && contentViewController.ShouldAutorotate();
		}

		// - (NSUInteger)supportedInterfaceOrientations
		public override UIInterfaceOrientationMask GetSupportedInterfaceOrientations ()
		{
			return menuViewController.GetSupportedInterfaceOrientations() & 
				contentViewController.GetSupportedInterfaceOrientations();
		}

		// - (BOOL)shouldAutorotateToInterfaceOrientation:(UIInterfaceOrientation)toInterfaceOrientation
		[Obsolete]
		public override bool ShouldAutorotateToInterfaceOrientation (UIInterfaceOrientation toInterfaceOrientation)
		{
			return menuViewController.ShouldAutorotateToInterfaceOrientation(toInterfaceOrientation) && 
				contentViewController.ShouldAutorotateToInterfaceOrientation(toInterfaceOrientation);
		}

		// - (void)willAnimateRotationToInterfaceOrientation:(UIInterfaceOrientation)toInterfaceOrientation duration:(NSTimeInterval)duration
		public override void WillAnimateRotation (UIInterfaceOrientation toInterfaceOrientation, double duration)
		{
			if (IsMenuOpen ()) {
				RectangleF frame = contentViewController.View.Frame;
				frame.X = OffsetXWhenMenuIsOpen();
				UIView.Animate(duration, () => {
					contentViewController.View.Frame = frame;
				});
			}
		}


		// #pragma mark - Menu view lazy load


		// - (void)loadMenuViewControllerViewIfNeeded
		public void LoadMenuViewControllerViewIfNeeded ()
		{
			if (menuViewController.View.Superview == null) {
				RectangleF menuFrame = View.Bounds;
				menuFrame.Width -= _widthOfContentViewVisible; 
				menuViewController.View.Frame = menuFrame;
				View.InsertSubview(menuViewController.View, 0);
			}
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
			// Remove gestures
			TapGesture.Enabled = false;
			PanGesture.Enabled = panEnabledWhenSlideMenuIsHidden;

			menuViewController.View.UserInteractionEnabled = false;

			var duration = animated ? ANIMATION_DURATION : 0;

			UIView contentView = contentViewController.View;
			RectangleF contentViewFrame = contentView.Frame;
			contentViewFrame.X = 0;

			menuViewController.BeginAppearanceTransition(false, animated);

			UIView.AnimateNotify(duration, 0, UIViewAnimationOptions.CurveEaseInOut, () => {
				contentView.Frame = contentViewFrame;
			}, (finished) => {
				menuViewController.EndAppearanceTransition ();
				menuViewController.View.UserInteractionEnabled = true;

				if (completion != null) {
					completion (finished);
				}
			});
		}

		// - (IBAction)toggleMenuAnimated:(id)sender;
		public void ToggleMenuAnimated ()
		{
			if (IsMenuOpen ()) {
				ShowContentViewControllerAnimated(true, null);
			} else {
				ShowMenuAnimated(true, null);
			}
		}

		// - (void)showMenuAnimated:(BOOL)animated completion:(void(^)(BOOL finished))completion;
		public void ShowMenuAnimated (bool animated, UICompletionHandler completion)
		{
			var duration = animated ? ANIMATION_DURATION : 0;

			UIView contentView = contentViewController.View;
			RectangleF contentViewFrame = contentView.Frame;
			contentViewFrame.X = OffsetXWhenMenuIsOpen();

			LoadMenuViewControllerViewIfNeeded();
			menuViewController.BeginAppearanceTransition(true, true);

			UIView.AnimateNotify(duration, 0, UIViewAnimationOptions.CurveEaseInOut, () => {
				contentView.Frame = contentViewFrame;
			}, (finished) => {
				menuViewController.EndAppearanceTransition ();

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
				ShowContentViewControllerAnimated (true, null);
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

		void PanGestureTriggered ()
		{
			if (PanGesture.State == UIGestureRecognizerState.Began) {
				contentViewControllerFrame = contentViewController.View.Frame;
				menuWasOpenAtPanBegin = IsMenuOpen ();

				if (!menuWasOpenAtPanBegin) {
					LoadMenuViewControllerViewIfNeeded (); // Menu is closed, load it if needed
					contentViewController.View.EndEditing(true); // Dismiss any open keyboards.
					menuViewController.BeginAppearanceTransition (true, true); // Menu is appearing
				}
			}

			PointF translation = PanGesture.TranslationInView (PanGesture.View);

			RectangleF frame = contentViewControllerFrame;
			frame.X += translation.X;

			float offsetXWhenMenuIsOpen = OffsetXWhenMenuIsOpen ();

			if (frame.X < 0) {
				frame.X = 0;
			} else if (frame.X > offsetXWhenMenuIsOpen) {
				frame.X = offsetXWhenMenuIsOpen;
			}

			PanGesture.View.Frame = frame;

			if (PanGesture.State == UIGestureRecognizerState.Ended) {
				PointF velocity = PanGesture.VelocityInView(PanGesture.View);
				float distance = 0;
				double animationDuration = 0;

				if (velocity.X < 0) // close
				{
					// Compute animation duration
					distance = frame.X;
					animationDuration = Math.Abs(distance / velocity.X);

					if (animationDuration > ANIMATION_DURATION) {
						animationDuration = ANIMATION_DURATION;
					}
										
					// Remove gestures
					TapGesture.Enabled = false;
					PanGesture.Enabled = panEnabledWhenSlideMenuIsHidden;
										
					frame.X = 0;
										
					if (!menuWasOpenAtPanBegin) {
						menuViewController.EndAppearanceTransition();
					}
										
					menuViewController.BeginAppearanceTransition(false, true);

					UIView.AnimateNotify(animationDuration, 0, UIViewAnimationOptions.CurveEaseInOut, () => {
						contentViewController.View.Frame = frame;
					}, (finished) => {
						menuViewController.EndAppearanceTransition();
					});

				} 
				else // open
				{
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

						if (!menuWasOpenAtPanBegin){
							menuViewController.EndAppearanceTransition();
						}
					});
				}

				contentViewControllerFrame = frame;
			}
		}

		// - (BOOL)isMenuOpen;
		public bool IsMenuOpen ()
		{
			return contentViewController.View.Frame.X > 0;
		}

		// - (CGFloat)offsetXWhenMenuIsOpen
		public float OffsetXWhenMenuIsOpen ()
		{
			return View.Bounds.Width - _widthOfContentViewVisible;
		}

	}
}
