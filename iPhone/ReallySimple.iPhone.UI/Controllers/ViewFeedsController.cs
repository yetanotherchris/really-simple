using System;
using System.Linq;
using MonoTouch.UIKit;
using ReallySimple.Core;
using System.Collections.Generic;
using MonoTouch.Foundation;
using System.Drawing;
using ReallySimple.iPhone.Core;
using System.Threading;
using ReallySimple.iPhone.UI.Views;

namespace ReallySimple.iPhone.UI.Controllers
{
	/// <summary>
	/// Displays the feeds using next/previous arrows and a webview. Uses the Item.List() for its items.
	/// </summary>
	public class ViewFeedsController : ControllerBase
	{
		private List<Item> _itemsForSelectedCategories;
		private int _currentItemIndex;
		private bool _loadedFromStartup;
		
		private UIBarButtonItem _previousButton; 
		private UIBarButtonItem _nextButton;
		private UIActivityIndicatorView _activityView;
		private UIBarButtonItem _activitityButton;
		private UIWebView _webView;
		private LoadingFeedsView _loadingView;
		
		protected override string ControllerId
		{
			get
			{
				return "ViewFeeds";
			}
		}
		
		protected override bool AddToolbarItems
		{
			get
			{
				return false;
			}
		}
		
		/// <summary>
		/// 
		/// </summary>
		public ViewFeedsController()
		{
			_loadedFromStartup = false;
		}
		
		public ViewFeedsController(bool loadedFromStartup) : this()
		{
			_loadedFromStartup = loadedFromStartup;
		}

		public override void ViewWillDisappear(bool animated)
		{
			if (_loadingView != null)
				_loadingView.Finish();

			base.ViewWillDisappear(animated);
		}
		
		/// <summary>
		/// Makes the toolbar and back buttons visible, adds the webview as a subview.
		/// </summary>
		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();
			
			View.BackgroundColor = UIColor.FromRGB(54,54,54);
			AddToolbar();

			// Add the webview
			_webView = new UIWebView();
			_webView.Alpha = 0; // no white flashing
			_webView.Delegate = new DefaultWebDelegate();
			
			View.AddSubview(_webView);
			View.BringSubviewToFront(_webView);
		}

		/// <summary>
		/// Adds the previous/next toolbar items, and 2 spacers
		/// </summary>
		private void AddToolbar()
		{
			// Previous button
			_previousButton = new UIBarButtonItem(UIImage.FromFile("Assets/Images/Toolbar/arrow-left.png"), UIBarButtonItemStyle.Plain, PreviousButtonClicked);
			_previousButton.Enabled = false;

			UIBarButtonItem flexi1 = new UIBarButtonItem(UIBarButtonSystemItem.FlexibleSpace);

			// The activity view for bg loading
			_activityView = new UIActivityIndicatorView(UIActivityIndicatorViewStyle.Gray);
			_activitityButton = new UIBarButtonItem(_activityView);

			UIBarButtonItem flexi2 = new UIBarButtonItem(UIBarButtonSystemItem.FlexibleSpace);

			// Next button
			_nextButton = new UIBarButtonItem(UIImage.FromFile("Assets/Images/Toolbar/arrow-right.png"), UIBarButtonItemStyle.Plain, NextButtonClicked);
			_nextButton.Enabled = false;

			ToolbarItems = new UIBarButtonItem[] { _previousButton, flexi1, _activitityButton, flexi2, _nextButton };
		}

		public override void ViewDidAppear (bool animated)
		{
			base.ViewDidAppear (animated);

			// Don't attempt to refresh if we're loading this controller from startup
			if (!_loadedFromStartup && Settings.Current.NeedsUpdate())
			{
				if (!IsOnline())
				{
					ModalDialog.Alert("No or slow internet connection","An update is available for the feeds, but no connection is currently available");
					ShowFirstItem();
				}
				else
				{
					Settings.Current.LastItemId = "";
					Settings.Current.LastControllerId = "";

					// Show the modal update dialog
					_loadingView = new LoadingFeedsView();
					_loadingView.LoadingComplete += new EventHandler(LoadingViewComplete);
					_loadingView.Stopped += LoadingViewStopped;
					_loadingView.Frame = new RectangleF(_loadingView.Bounds.Location, new SizeF(300, 400));
					_loadingView.Initialize();
				}
			}
			else
			{
				ShowFirstItem();
			}
		}

		private void LoadingViewComplete(object sender, EventArgs e)
		{
			// Close the dialog
			_loadingView.Finish();
			ShowFirstItem();
		}

		private void LoadingViewStopped(object sender, EventArgs e)
		{
			InvokeOnMainThread(delegate
			{
				NavigationController.PopViewControllerAnimated(true);
			});
		}

		private void ShowFirstItem()
		{
			if (Settings.Current.UserSettings.ImagesEnabled)
			{
				BackgroundDownloadImages();
			}
			
			_itemsForSelectedCategories = new List<Item>();

			// Use local variables to avoid repeated calls
			var allItems = Repository.Default.ListItems();
			var lastCategories = Settings.Current.LastCategories;
			foreach (Item currentitem in allItems)
			{
				if (lastCategories.Contains(currentitem.Feed.Category))
					_itemsForSelectedCategories.Add(currentitem);
			}

			// Filter out read items, if it's enabled.
			if (Settings.Current.UserSettings.IgnoreReadItems)
				_itemsForSelectedCategories = _itemsForSelectedCategories.Where(i => !i.IsRead).ToList();

			// Nothing to work with, show the empty message
			if (_itemsForSelectedCategories.Count == 0)
			{
				Settings.Current.LastItemId = "";
				_webView.LoadHtmlString(HtmlTemplate.EmptyHtml(), new NSUrl("/"));
				return;
			}
			else
			{
				switch (Settings.Current.UserSettings.SortItemsBy)
				{
					case SortBy.Site:
						_itemsForSelectedCategories = _itemsForSelectedCategories.SortBySite();
						break;

					case SortBy.Date:
					default:
						_itemsForSelectedCategories = _itemsForSelectedCategories.SortByDate();
						break;
				}

				UpdateArrowButtons();

				// Show the first item
				_currentItemIndex = 0;

				// Use the last item that was viewed, if it exists
				Item item = null;
				if (!string.IsNullOrEmpty(Settings.Current.LastItemId))
				{
					try
					{
						Guid guid = new Guid(Settings.Current.LastItemId);
						item = _itemsForSelectedCategories.FirstOrDefault(i => i.Id == guid);
					}
					catch (FormatException)
					{
					}
				}

				if (item != null)
				{
					// Put it at the start of the list, and remove it from wherever it is
					_itemsForSelectedCategories.Remove(item);
					_itemsForSelectedCategories.Insert(0, item);
				}
				else
				{
					// No saved item, use the first
					item = _itemsForSelectedCategories[0];
				}

				Settings.Current.LastItemId = item.Id.ToString();
				_webView.LoadHtmlString(HtmlTemplate.HtmlForItem(item), new NSUrl("/"));
			}
		}

		private void BackgroundDownloadImages()
		{
			// Background download the images.
			_activityView.StartAnimating();

			ImageDownloader.Current.Complete += delegate
			{
				InvokeOnMainThread(delegate
				{
					_activityView.StopAnimating();
				});
			};

			ImageDownloader.Current.BackgroundDownload();
		}

		/// <summary>
		/// Loads the previous item from the items list into the webview.
		/// </summary>
		private void PreviousButtonClicked(object sender, EventArgs e)
		{
			if (_currentItemIndex > 0)
			{
				// Get the current item, unmark it as read and unload it.
				Item item = _itemsForSelectedCategories[_currentItemIndex];
				
				if (Settings.Current.UserSettings.IgnoreReadItems)
					item.UpdateReadStatus(false);

				item.UnLoad();

				// Load the next item
				item = _itemsForSelectedCategories[--_currentItemIndex];
				_webView.LoadHtmlString(HtmlTemplate.HtmlForItem(item),new NSUrl("/"));
				
				Settings.Current.LastItemId = item.Id.ToString();
			}
			
			UpdateArrowButtons();
		}
		
		/// <summary>
		/// Loads the next item from the items list into the webview.
		/// </summary>
		private void NextButtonClicked(object sender, EventArgs e)
		{
			if (_currentItemIndex <= _itemsForSelectedCategories.Count -1)
			{
				// Get the current item, mark it as read and unload it.
				Item item = _itemsForSelectedCategories[_currentItemIndex];
				
				if (Settings.Current.UserSettings.IgnoreReadItems)
					item.UpdateReadStatus(true);

				item.UnLoad();
				
				// Load the next item
				item = _itemsForSelectedCategories[++_currentItemIndex];
				string html = HtmlTemplate.HtmlForItem(item);
				_webView.LoadHtmlString(html, new NSUrl("/"));
				
				// If we're on the last item, we have to mark this item as read too,
				// as the next button will never be triggered.
				if (_currentItemIndex == _itemsForSelectedCategories.Count -1)
				{
					// Mark the last one as read
					if (Settings.Current.UserSettings.IgnoreReadItems)
					{
						item.UpdateReadStatus(true);
						Settings.Current.LastItemId = "";
					}
				}
				else
				{
					Settings.Current.LastItemId = item.Id.ToString();
				}
			}
			
			UpdateArrowButtons();
		}
		
		/// <summary>
		/// Enables/disables the arrows based on the current item index in the list
		/// </summary>
		private void UpdateArrowButtons()
		{
			// The two elseif statements are there for cases where there's only one item

			if (_currentItemIndex == 0) 
				_previousButton.Enabled = false;
			else if (_itemsForSelectedCategories.Count > 0)
				_previousButton.Enabled = true;	
			
			if (_currentItemIndex == _itemsForSelectedCategories.Count -1)
				_nextButton.Enabled = false;
			else if (_itemsForSelectedCategories.Count > 0)
				_nextButton.Enabled = true;

			// Try to stop the UIWebView gobbling memory
			GC.Collect();
		}
		
		/// <summary>
		/// Resizes the webview when the device rotates
		/// </summary>
		public override bool ShouldAutorotateToInterfaceOrientation(UIInterfaceOrientation toInterfaceOrientation)
		{
			if (_webView != null && (toInterfaceOrientation == UIInterfaceOrientation.LandscapeLeft || toInterfaceOrientation == UIInterfaceOrientation.LandscapeRight))
			{
				_webView.Frame = new System.Drawing.RectangleF(0,0,480,320);	
				_webView.Reload();
			}
			else if (_webView != null)
			{
				_webView.Frame = new System.Drawing.RectangleF(0,0,320,480);	
			}
			
			return true;
		}
	}
}