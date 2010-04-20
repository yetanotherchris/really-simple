using System;
using System.Linq;
using MonoTouch.UIKit;
using ReallySimple.Core;
using System.Collections.Generic;
using MonoTouch.Foundation;
using System.Threading;
using System.Drawing;
using ReallySimple.iPhone.Core.Remote;
using ReallySimple.iPhone.Core;

namespace ReallySimple.iPhone.UI.Controllers
{
	public class LoadingFeedsView : UIAlertView
	{
		private int _saveCount;
		private int _newFeedCount;
        private Thread _thread;
		
		private UIProgressView _progressView;
		private UIActivityIndicatorView _activityIndicator;

		public event EventHandler LoadingComplete;
		public event EventHandler Stopped;

		public void Initialize()
		{
			_saveCount = 0;
			_newFeedCount = 0;

			Delegate = new AlertViewDelegate(this);
			AddButton("Stop");
			Title = "Getting the latest feeds\nPlease wait...\n\n";
			base.Show();
			
			// Progress bar.
			_progressView = new UIProgressView();
			_progressView.Frame = new RectangleF((Bounds.Width / 2) - 50, 75, 100, 30);
			_progressView.Progress = 0;
			_progressView.Alpha = 0f;
			AddSubview(_progressView);
			
			// Activity indicator, which dissapears once the download is complete
			_activityIndicator = new UIActivityIndicatorView(UIActivityIndicatorViewStyle.White);
			_activityIndicator.Frame = new RectangleF((Bounds.Width / 2) -7, 75, 15, 15);
			_activityIndicator.StartAnimating();
			AddSubview(_activityIndicator);
		}

        public void Start()
        {
            _thread = new Thread(StartThread);
            _thread.IsBackground = true;
            _thread.Start();
        }

		public void Finish()
		{
			DismissWithClickedButtonIndex(0, true);
		}

		private void StartThread()
		{
            using (NSAutoreleasePool pool = new NSAutoreleasePool())
            {
                FeedUpdater updater = new FeedUpdater();
				updater.FeedSaved += new EventHandler(FeedSaved);

                try
                {               
                    List<Item> newItems = updater.Update();

                    // Show the progress bar
                    InvokeOnMainThread(delegate
                    {
                        _activityIndicator.StopAnimating();
                        _progressView.Alpha = 1f;
                    });

                    // Save all the items to SQLite
                    if (newItems.Count > 0)
                    {
                        _newFeedCount = newItems.Count;
                        updater.SaveItems(newItems);
                    }

                    // Download images
                    if (_newFeedCount > 0 && Settings.Current.UserSettings.ImagesEnabled)
                    {
                        _saveCount = 0;

                        ImageDownloader.Current.ImageSaved += DownloaderImageSaved;
                        ImageDownloader.Current.PreDownloadForCategories();
                        UpdateProgressAmount(1);
                    }
                }
                catch (ThreadAbortException)
                {
                    // Stop downloads
                    ImageDownloader.Current.TryStop();
                }
                finally
                {
					OnLoadingComplete(EventArgs.Empty);
                }
            }
		}

		private void FeedSaved(object sender, EventArgs e)
		{
			// Update the progress bar count
			float progress = (1.0f / _newFeedCount) * ++_saveCount;
			UpdateProgressAmount(progress);

			// Change the label text
			UpdateTitleText(string.Format("Getting the latest feeds\nSaved {0} of {1} feeds\n\n", _saveCount, _newFeedCount));
		}
		
		private void DownloaderImageSaved(object sender, EventArgs e)
		{
			// Cheat, for now
			float progress = (1.0f / ImageDownloader.Current.NewImageCount) * ++_saveCount;
			UpdateProgressAmount(progress);

			// Change the label text
			if (_saveCount <= ImageDownloader.Current.NewImageCount)
				UpdateTitleText(string.Format("Getting the latest feeds\nDownloaded {0}/{1} images\n\n", _saveCount,ImageDownloader.Current.NewImageCount));
		}

		private void Stop()
		{
			OnStopped(EventArgs.Empty);
		}

		/// <summary>
		/// Fires the <see cref="LoadingComplete"/> event.
		/// </summary>
		/// <param name="e"></param>
		protected virtual void OnLoadingComplete(EventArgs e)
		{
			// No need to be thread safe
			if (LoadingComplete != null)
				LoadingComplete(this, e);
		}

		/// <summary>
		/// Fires the <see cref="Stopped"/> event.
		/// </summary>
		/// <param name="e"></param>
		protected virtual void OnStopped(EventArgs e)
		{
			// No need to be thread safe
			if (Stopped != null)
				Stopped(this, e);
		}

		private void UpdateTitleText(string text)
		{
			InvokeOnMainThread(delegate
			{
				Title = text;
			});
		}
				
		private void UpdateProgressAmount(float amount)
		{
			InvokeOnMainThread(delegate
			{
				_progressView.Progress = amount;	
			});
		}
		
		public class AlertViewDelegate : UIAlertViewDelegate
	    {
			private LoadingFeedsView _loadingFeedsView;

			public AlertViewDelegate(LoadingFeedsView loadingFeedsView)
			{
				_loadingFeedsView = loadingFeedsView;
			}
			
	        public override void Clicked(UIAlertView alertview, int buttonIndex)
	        {
				_loadingFeedsView.Stop();
	        }
			
			public override void Presented(UIAlertView alertView)
			{
				_loadingFeedsView.Start();
			}
	    }	
	}
}