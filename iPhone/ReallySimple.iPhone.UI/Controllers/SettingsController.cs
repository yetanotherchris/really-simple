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
	/// <summary>
	/// The settings screen. 
	/// ** The Monotouch.Dialog callback is found in ControllerBase **
	/// </summary>
	public class SettingsController : DialogViewController
	{
		private UIBarButtonItem _doneButton;
		private UIBarButtonItem _clearButton;
		private PickCategoriesController _pickCategories;
		
		protected string ControllerId
		{
			get
			{
				return "";
			}
		}

		/// <summary>
		/// Required by MonoTouch.Dialog
		/// </summary>
		public SettingsController(RootElement root) : base(root)
		{
			
		}

		/// <summary>
		/// Required by MonoTouch.Dialog
		/// </summary>
		public SettingsController(RootElement root, bool pushing) : base(root, pushing)
		{
			
		}

		public override void ViewDidAppear (bool animated)
		{
			base.ViewDidAppear (animated);
			
			// Hide the navigation bar, back button and toolbar.
			NavigationController.SetToolbarHidden(true,false);
			NavigationItem.HidesBackButton = true;
			
			// 'Done' button in the top right
			_doneButton = new UIBarButtonItem();
			_doneButton.Title = "Done";
			_doneButton.Clicked += delegate(object sender, EventArgs e) 
			{
				_pickCategories = new PickCategoriesController();
				NavigationController.PushViewController(_pickCategories, true);
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
		}		

		public override void ViewWillDisappear(bool animated)
		{
			base.ViewWillDisappear(animated);

			// Persist the controller name for next load
			Settings.Current.LastControllerId = ControllerId;
		}
	}
}
