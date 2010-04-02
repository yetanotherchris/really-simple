using System;
using MonoTouch.Foundation;
using MonoTouch.UIKit;
using ReallySimple.iPhone.Core;
using ReallySimple.iPhone.UI.Controllers;

namespace ReallySimple.iPhone.UI
{
	[Register("AppDelegate")]
	public partial class AppDelegate : UIApplicationDelegate
	{
		private UIWindow _window;
		private SplashScreenController _splashScreenController;
		
		public override bool FinishedLaunching (UIApplication app, NSDictionary options)
		{			
			_window = new UIWindow(UIScreen.MainScreen.Bounds);
			
			// Add the the splash controller
			_splashScreenController = new SplashScreenController();
			_window.BackgroundColor = UIColor.FromRGBA(0x36, 0x36, 0x36, 1);
			_window.AddSubview(_splashScreenController.View);
			_window.MakeKeyAndVisible();
			
			return true;
		}

		public override void WillTerminate(UIApplication application)
		{
			Settings.Current.Write();
		}
		
		// This method is required in iPhoneOS 3.0
		public override void OnActivated (UIApplication application)
		{
		}
	}
}

