using System;
using MonoTouch.UIKit;
using MonoTouch.Dialog;
using System.Threading;
using MonoTouch.Foundation;
using ReallySimple.iPhone.Core;
using MonoTouch.MessageUI;
using System.Drawing;
using System.Text;
using System.IO;
using ReallySimple.Core;
using System.Linq;

namespace ReallySimple.iPhone.UI.Controllers
{
	/// <summary>
	/// Represents a UIViewController that has a standard bottom toolbar.
	/// </summary>
	public class ControllerBase : UIViewController
	{
		private InformationController _informationController;
		private SettingsController _settingsController;

		private UIBarButtonItem _settingsButton;
		private UIBarButtonItem _informationButton;
		private UIBarButtonItem _flexi1;
		private UIBarButtonItem _flexi2;
		
		/// <summary>
		/// Used for saving in the Settings, so this view is loaded on startup.
		/// </summary>
		protected virtual string ControllerId
		{
			get
			{
				return "";	
			}
		}
		
		/// <summary>
		/// Whether to add the default buttons for the bottom toolbar.
		/// </summary>
		protected virtual bool AddToolbarItems
		{
			get
			{
				return true;
			}
		}
		
		public override void ViewDidLoad ()
		{
			base.ViewDidLoad();
			
			// All visible by default
			NavigationController.SetToolbarHidden(false,false);
			NavigationItem.HidesBackButton = false;
			NavigationController.SetNavigationBarHidden(false,false);
			
			if (AddToolbarItems)
				ToolbarItems = GetToolBar();	
		}
		
		public override void ViewDidAppear (bool animated)
		{
			base.ViewDidAppear(animated);
			
			// Persist the controller name for next load
			Settings.Current.LastControllerId = ControllerId;
		}
		
		public UIBarButtonItem[] GetToolBar()
		{			
			// Settings icon
			_settingsButton = new UIBarButtonItem();
			_settingsButton.Image = UIImage.FromFile("Assets/Images/Toolbar/settings.png");
			_settingsButton.Clicked += delegate
			{
				_settingsController = new SettingsController(null);
				NavigationController.PushViewController(_settingsController, true);
			};
			
			// Information icon
			_informationButton = new UIBarButtonItem();
			_informationButton.Image = UIImage.FromFile("Assets/Images/Toolbar/information.png");
			_informationButton.Clicked += delegate
			{
				_informationController = new InformationController();
				NavigationController.PushViewController(_informationController,true);
			};
			
			// Two spacers to go between each icon
			_flexi1 = new UIBarButtonItem(UIBarButtonSystemItem.FlexibleSpace);
			_flexi2 = new UIBarButtonItem(UIBarButtonSystemItem.FixedSpace);
			_flexi2.Width = 20;
			
			return new UIBarButtonItem[] { _flexi1,_settingsButton,_flexi2,_informationButton };
		}
		
		/// <summary>
        /// Returns true if the device has an internet connection.
        /// </summary>
        public bool IsOnline()
        {
			return Reachability.InternetConnectionStatus() != NetworkStatus.NotReachable;
        }
	}
}
