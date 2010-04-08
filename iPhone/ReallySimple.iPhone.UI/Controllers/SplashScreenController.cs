using MonoTouch.UIKit;
using System.Linq;
using MonoTouch.Dialog;
using ReallySimple.iPhone.Core;
using System.Drawing;
using System.Threading;
using MonoTouch.CoreAnimation;
using System;
using MonoTouch.ObjCRuntime;
using MonoTouch.Foundation;

namespace ReallySimple.iPhone.UI.Controllers
{
	/// <summary>
	/// Post-Default.png loading screen while the database items are loaded.
	/// </summary>
	public class SplashScreenController : UIViewController
	{
		private Thread _thread;
		private RootController _rootController;
		
		private UIView _containerView;
		private UIImageView _imageView;
		private UIActivityIndicatorView _activityView;
		private UILabel _label;
		
		public override void ViewDidLoad()
		{
			base.ViewDidLoad ();
			
			_thread = new Thread(ThreadEntry);
			
			// Container for the controls
			_containerView = new UIView();
			_containerView.Frame = new RectangleF(0,-20,320,480);
			
			// The background loading image
			_imageView = new UIImageView();
			_imageView.Image = UIImage.FromFile("Default.png");
			_imageView.Frame = new RectangleF(0,0,320,480);
			_containerView.AddSubview(_imageView);
			
			// The pulser
			_activityView = new UIActivityIndicatorView(UIActivityIndicatorViewStyle.White);
			_activityView.Frame = new RectangleF(115,280,20,20);
			_containerView.AddSubview(_activityView);
			_activityView.StartAnimating();
			
			// Label saying wait
			_label = new UILabel();
			_label.Frame = new RectangleF(140,280,250,20);
			_label.Font = UIFont.SystemFontOfSize(14f);
			_label.BackgroundColor = UIColor.Clear;
			_label.TextColor = UIColor.White;
			_label.ShadowColor = UIColor.Black;
			_label.Text = "Loading...";
			_containerView.AddSubview(_label);
			
			View.AddSubview(_containerView);
		}
		
		public override void ViewWillAppear (bool animated)
		{
			base.ViewWillAppear (animated);
		}
		
		public override void ViewDidAppear(bool animated)
		{
			base.ViewDidAppear (animated);
			_thread.Start();
		}
		
		/// <summary>
		/// These are performed inside a thread to stop the view loading from being blocked. 
		/// </summary>
		private void ThreadEntry()
		{
			using (NSAutoreleasePool pool = new NSAutoreleasePool())
			{
				StartupLoader loader = new StartupLoader();
				loader.InitializeDatabase();
				loader.ClearOldItems();
				loader.LoadSettings();
				loader.LoadCategories();
				loader.LoadItems();
				loader.LoadSites();
				loader.LoadHtmlTemplates();
				
				// Fade out container view
				InvokeOnMainThread(delegate
				{
					// Add the UINav controller, send it to the back so it's hidden
					_rootController = new RootController();
					_rootController.View.Frame = new RectangleF(0,-20,320,480);
					View.AddSubview(_rootController.View);
					View.SendSubviewToBack(_rootController.View);
					
					UIView.BeginAnimations(null);
					UIView.SetAnimationDuration(0.5);
					UIView.SetAnimationTransition(UIViewAnimationTransition.None,_containerView,true);
					UIView.SetAnimationDelegate(this);
		        		UIView.SetAnimationDidStopSelector(new Selector("fadeOutDidFinish"));
					_containerView.Alpha = 0;					
					UIView.CommitAnimations();
				});
			}
		}
		
		[Export("fadeOutDidFinish")]
		public void FadeOutDidFinish()
		{
			InvokeOnMainThread(delegate
			{
				View.BringSubviewToFront(_rootController.View);
				View = _rootController.View;
			});
		}
	}
}
