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
		private SettingsController _settingsController;

		private UIBarButtonItem _settingsButton;
		private UIBarButtonItem _informationButton;
		private UIBarButtonItem _flexi1;
		private UIBarButtonItem _flexi2;
		
		private UIButton _emailButton;
		private UIButton _debugButton;
		private UIView _containerView;
		
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
			base.ViewDidAppear (animated);
			
			// Persist the controller name for next load
			Settings.Current.LastControllerId = ControllerId;
		}
		
		public UIBarButtonItem[] GetToolBar()
		{			
			// Settings icon
			_settingsButton = new UIBarButtonItem();
			_settingsButton.Image = UIImage.FromFile("Assets/Images/Toolbar/settings.png");
			_settingsButton.Clicked += new EventHandler(SettingsButtonClicked);
			
			// Information icon
			_informationButton = new UIBarButtonItem();
			_informationButton.Image = UIImage.FromFile("Assets/Images/Toolbar/information.png");
			_informationButton.Clicked += delegate
			{
				InformationController controller = new InformationController();
				NavigationController.PushViewController(controller,false);
			};
			
			// Two spacers to go between each icon
			_flexi1 = new UIBarButtonItem(UIBarButtonSystemItem.FlexibleSpace);
			_flexi2 = new UIBarButtonItem(UIBarButtonSystemItem.FixedSpace);
			_flexi2.Width = 20;
			
			return new UIBarButtonItem[] { _flexi1,_settingsButton,_flexi2,_informationButton };
		}

		private void SettingsButtonClicked(object sender, EventArgs e)
		{
			BindingContext bindingContext = new BindingContext(this, Settings.Current.UserSettings, "Settings");
			AddSettingsButtons(bindingContext);
			
			_settingsController = new SettingsController(bindingContext.Root);
			_settingsController.ViewDissapearing += delegate
			{
				bool showImages = Settings.Current.UserSettings.ImagesEnabled;
				bindingContext.Fetch();

				// Trigger all feeds reloading again - only if it was false beforehand (as we need the images HTML).
				// If the setting was previously TRUE and we're switching to FALSE, we already have the content and don't need to update again.
				if (showImages == false && Settings.Current.UserSettings.ImagesEnabled)
				{
					Settings.Current.ClearCache(true);
				}
			};

			NavigationController.PushViewController(_settingsController, false);
		}
			
		private void AddSettingsButtons(BindingContext bindingContext)
		{
			_containerView = new UIView();
			_containerView.Frame = new RectangleF(10,320,300,100);
			
			// Add send email button
			_emailButton = UIButton.FromType(UIButtonType.RoundedRect);
			_emailButton.Frame = new RectangleF(10,100,300,40);
			_emailButton.SetTitle("Send error report by email",UIControlState.Normal);
			_emailButton.HorizontalAlignment = UIControlContentHorizontalAlignment.Left;
			_emailButton.SetTitleColor(UIColor.Black,UIControlState.Normal);
			_emailButton.
			_emailButton.TouchDown += delegate
			{
				
				if (File.Exists(Settings.Current.LogFile))
				{				
					if (MFMailComposeViewController.CanSendMail)
					{
						NSData data = NSData.FromFile(Settings.Current.LogFile);
						
						MFMailComposeViewController mailController = new MFMailComposeViewController();
						mailController.SetSubject("Really simple iPhone error report");
						mailController.SetToRecipients(new string[]{"mrshrinkray@gmail.com"});
						mailController.SetMessageBody("Really simple error report", false);
						mailController.AddAttachmentData(data,"text/plain","error.log");
						mailController.Finished += HandleMailFinished;
						this.PresentModalViewController(mailController, true);
					} 
					else 
					{
						ModalDialog.Alert("No email account found","Please configure an email account before sending an error report");
					}
				}
				else
				{
					ModalDialog.Alert("No log file was found","The error log was empty. Please try sending this again after the error occurs.");
				}
	
			};
			_containerView.AddSubview(_emailButton);
			
			// Debug details (disabled for release builds)
			_debugButton = UIButton.FromType(UIButtonType.RoundedRect);
			_debugButton.Frame = new RectangleF(10,55,300,40);
			_debugButton.SetTitle(" Debug information",UIControlState.Normal);
			_debugButton.HorizontalAlignment = UIControlContentHorizontalAlignment.Left;
			_debugButton.TouchDown += delegate
			{
				ModalDialog.Alert("Debug",DebugInfo());	
			};
			
			_containerView.AddSubview(_debugButton);
			
			Section section = new Section(_containerView);
			bindingContext.Root.Add(section);
		}
		
		private string DebugInfo()
		{
			var allItems = Repository.Default.ListItems();
			var readItems = allItems.Where(i => !i.IsRead);
			
			// Image count
			int imageCount = 0;
            var dir = new DirectoryInfo(Settings.Current.ImageFolder);
            var subdirs = dir.GetDirectories();
            if (subdirs.Length > 0)
            {
                imageCount = subdirs[0].GetFiles().Length;
            }
			
			StringBuilder builder = new StringBuilder();
			builder.AppendLine(string.Format("Version {0}",Settings.Current.Version));
			builder.AppendLine(string.Format("Last updated {0}",Settings.Current.LastUpdate.ToString("HH:mm")));
			builder.AppendLine(string.Format("Items in the cache {0}",allItems.Count));
			builder.AppendLine(string.Format("Unread items in the cache {0}",readItems.ToList().Count));
			builder.AppendLine(string.Format("Images downloaded {0}", imageCount));
			builder.AppendLine(string.Format("Image downloader working: {0}", ImageDownloader.Current.IsWorking));
			
			return builder.ToString();
		}
		
		void HandleMailFinished(object sender, MFComposeResultEventArgs e)
		{
			if (e.Result == MFMailComposeResult.Sent)
			{
				
			}
			else if (e.Result != MFMailComposeResult.Sent)
			{
				
			}
			
			e.Controller.DismissModalViewControllerAnimated(true);
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
