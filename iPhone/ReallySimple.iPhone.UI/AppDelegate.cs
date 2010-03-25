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
		private RootController _navigationController;
		
		public override bool FinishedLaunching (UIApplication app, NSDictionary options)
		{			
			_window = new UIWindow(UIScreen.MainScreen.Bounds);

			// All startup tasks
			StartupLoader loader = new StartupLoader();
			loader.LoadSettings();
			loader.LoadCategories();
			loader.LoadItems();
			loader.LoadSites();
			loader.LoadHtmlTemplates();
			loader.ClearOldItems();
			
			// Add the the root controller
			_navigationController = new RootController();
			_window.BackgroundColor = UIColor.FromRGBA(0x36, 0x36, 0x36, 1);
			_window.AddSubview(_navigationController.View);
			_window.MakeKeyAndVisible();
			
			return true;
		}

		public override void WillTerminate(UIApplication application)
		{
			Settings.Current.Write();
		}
		
		public override void OnResignActivation (UIApplication application)
		{							
			//var controllers = _navigationController.ViewControllers;
			//var currentController = controllers[controllers.Length -1] as ViewFeedsController;
			
			//if (currentController != null && currentController.IsLoading)
			//{
			//	_navigationController.PopViewControllerAnimated(false);
			//}
		}
		
		// This method is required in iPhoneOS 3.0
		public override void OnActivated (UIApplication application)
		{
		}
	}
}

