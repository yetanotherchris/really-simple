using System;
using System.Drawing;
using MonoTouch.UIKit;
using MonoTouch.Dialog;
using ReallySimple.iPhone.Core;
using ReallySimple.Core;
using System.Linq;
using System.Collections.Generic;
using System.IO;
using MonoTouch.MessageUI;
using MonoTouch.Foundation;
using System.Text;

namespace ReallySimple.iPhone.UI.Controllers
{
	public class SettingsController : DialogViewController
	{
		private UIBarButtonItem _doneButton;
		private UIBarButtonItem _clearButton;
		private PickCategoriesController _pickCategories;
		private BindingContext _bindingContext;

		private UIButton _emailButton;
		private UIButton _debugButton;
		private UIView _containerView;

		/// <summary>
		/// Required by MonoTouch.Dialog
		/// </summary>
		public SettingsController(RootElement root) : base(root)
		{
			_bindingContext = new BindingContext(this, Settings.Current.UserSettings, "Settings");
			Root = _bindingContext.Root;
			ReloadData();
		}

		/// <summary>
		/// Required by MonoTouch.Dialog
		/// </summary>
		public SettingsController(RootElement root, bool pushing)
			: base(root, pushing)
		{
			
		}
		
		public override void ViewDidLoad ()
		{
			Title = "Settings";
			base.ViewDidLoad ();
			
			// Hide the navigation bar, back button and toolbar.
			NavigationController.SetToolbarHidden(true,false);
			NavigationItem.HidesBackButton = true;
			
			// 'Done' button in the top right
			_doneButton = new UIBarButtonItem();
			_doneButton.Title = "Done";
			_doneButton.Clicked += delegate(object sender, EventArgs e) 
			{
				UIView.BeginAnimations(null,IntPtr.Zero);
				UIView.SetAnimationDuration(0.5);
				UIView.SetAnimationTransition(UIViewAnimationTransition.FlipFromRight,NavigationController.View,true);
				
				_pickCategories = new PickCategoriesController();
				NavigationController.PushViewController(_pickCategories, false);
				UIView.CommitAnimations();
			};
			
			// Clear cache
			_clearButton = new UIBarButtonItem();
			_clearButton.Title = "Clear cache";
			_clearButton.Clicked += delegate(object sender, EventArgs e) 
			{
				Settings.Current.ClearCache(true);	
				ModalDialog.Alert("Cache cleared","All cached items and images were removed");
			};

			NavigationItem.SetLeftBarButtonItem(_clearButton,false);
			NavigationItem.SetRightBarButtonItem(_doneButton, false);

			AddSettingsButtons();
		}	

		public override void ViewWillDisappear(bool animated)
		{
			base.ViewWillDisappear(animated);

			bool previousShowImages = Settings.Current.UserSettings.ImagesEnabled;

			// Update the UserSettings singleton
			_bindingContext.Fetch();

			// Trigger all feeds reloading again - only if it was false beforehand (as we need the images HTML).
			// If the setting was previously TRUE and we're switching to FALSE, we already have the content and don't need to update again.
			if (previousShowImages == false && Settings.Current.UserSettings.ImagesEnabled)
			{
				Settings.Current.ClearCache(true);
			}

			// Persist the controller name for next load
			Settings.Current.LastControllerId = "Settings";
		}

		private void AddSettingsButtons()
		{
			_containerView = new UIView();
			_containerView.Frame = new RectangleF(10, 280, 300, 100);

			// Add send email button
			_emailButton = UIButton.FromType(UIButtonType.RoundedRect);
			_emailButton.Frame = new RectangleF(10, 0, 300, 40);
			_emailButton.SetTitle("  Send error report by email", UIControlState.Normal);
			_emailButton.HorizontalAlignment = UIControlContentHorizontalAlignment.Center;
			_emailButton.TouchDown += new EventHandler(EmailButtonTouchDown);
			_containerView.AddSubview(_emailButton);

			// Debug details
			_debugButton = UIButton.FromType(UIButtonType.RoundedRect);
			_debugButton.Frame = new RectangleF(10, 45, 300, 40);
			_debugButton.SetTitle(" Debug information", UIControlState.Normal);
			_debugButton.HorizontalAlignment = UIControlContentHorizontalAlignment.Center;
			_debugButton.TouchDown += delegate
			{
				ModalDialog.Alert("Debug", DebugInfo());
			};

			_containerView.AddSubview(_debugButton);

			Section section = new Section(_containerView);
			_bindingContext.Root.Add(section);
		}

		private void EmailButtonTouchDown(object sender, EventArgs e)
		{
			if (File.Exists(Settings.Current.LogFile))
			{
				if (MFMailComposeViewController.CanSendMail)
				{
					NSData data = NSData.FromFile(Settings.Current.LogFile);

					MFMailComposeViewController mailController = new MFMailComposeViewController();
					mailController.SetSubject("Really simple iPhone error report");
					mailController.SetToRecipients(new string[] { "mrshrinkray@gmail.com" });
					mailController.SetMessageBody("Really simple error report", false);
					mailController.AddAttachmentData(data, "text/plain", "error.log");
					mailController.Finished += HandleMailFinished;
					this.PresentModalViewController(mailController, true);
				}
				else
				{
					ModalDialog.Alert("No email account found", "Please configure an email account before sending an error report");
				}
			}
			else
			{
				ModalDialog.Alert("No log file was found", "The error log was empty. Please try sending this again after the error occurs.");
			}
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
			builder.AppendLine(string.Format("Version {0}", Settings.Current.Version));
			builder.AppendLine(string.Format("Last updated {0}", Settings.Current.LastUpdate.ToString("HH:mm")));
			builder.AppendLine(string.Format("Items in the cache {0}", allItems.Count));
			builder.AppendLine(string.Format("Unread items in the cache {0}", readItems.ToList().Count));
			builder.AppendLine(string.Format("Images downloaded {0}", imageCount));
			builder.AppendLine(string.Format("Image downloader working: {0}", ImageDownloader.Current.IsWorking));

			return builder.ToString();
		}

		void HandleMailFinished(object sender, MFComposeResultEventArgs e)
		{
			if (e.Result == MFMailComposeResult.Sent)
				ModalDialog.Alert("Your email was sent", "");

			e.Controller.DismissModalViewControllerAnimated(true);
		}
	}
}
