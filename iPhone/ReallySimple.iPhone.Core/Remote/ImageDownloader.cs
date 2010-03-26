using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using MonoTouch.Foundation;
using System.IO;
using ReallySimple.Core;
using System.Net;

namespace ReallySimple.iPhone.Core
{
	/// <summary>
	/// Downloads all images for a list of items, either synchronously or in the background.
	/// This class is a singleton.
	/// </summary>
	public class ImageDownloader
	{
		#region Fields
		private AutoResetEvent _waithandle;
		private bool _isWorking;
		private int _timeout;
		private Thread _thread;
		private int _newImageCount;

		private readonly object _imageSavedLock;
		private readonly object _completeLock;
		private event EventHandler _imageSaved;
		private event EventHandler _complete;

		private static readonly ImageDownloader _instance = new ImageDownloader(); 
		#endregion

		#region Properties
		/// <summary>
		/// For synchronous downloads - avoids a new EventHandler.
		/// </summary>
		public int NewImageCount
		{
			get { return _newImageCount; }
		}

		/// <summary>
		/// The current instance of the <see cref="ImageDownloader"/>
		/// </summary>
		public static ImageDownloader Current
		{
			get
			{
				return _instance;
			}
		}

		/// <summary>
		/// Determines whether a background download is in progress.
		/// </summary>
		public bool IsWorking
		{
			get
			{
				return _isWorking;
			}
		}

		/// <summary>
		/// Occurs when an image is saved to disk.
		/// </summary>
		public event EventHandler ImageSaved
		{
			add
			{
				lock (_imageSavedLock)
				{
					_imageSaved += value;
				}
			}
			remove
			{
				lock (_imageSavedLock)
				{
					_imageSaved -= value;
				}
			}
		}

		/// <summary>
		/// Occurs when downloading is complete.
		/// </summary>
		public event EventHandler Complete
		{
			add
			{
				lock (_completeLock)
				{
					_complete += value;
				}
			}
			remove
			{
				lock (_completeLock)
				{
					_complete -= value;
				}
			}
		} 
		#endregion

		private ImageDownloader()
		{			
			_isWorking = false;
			_timeout = 10 * 1000;
			_imageSavedLock = new object();
			_completeLock = new object();
			_newImageCount = 0;
		}

		/// <summary>
		/// Synchronously downloads the first 7 images for each selected category, using Settings.Current.LastCategories.
		/// </summary>
		public void PreDownloadForCategories()
		{
			List<Item> items = new List<Item>();

			List<Item> allItems = Repository.Default.ListItems().ToList();
			allItems = SortItems(allItems);			
			
            // 5 images from the selected categories
            foreach (Category category in Settings.Current.LastCategories)
            {
               var catItems = allItems.Where(i =>
                    i.Feed.Category.Id.Equals(category.Id) &&
                    !i.ImageDownloaded &&
                    !string.IsNullOrEmpty(i.ImageUrl))
                    .Take(5);

               items.AddRange(catItems);
            }
			
			_newImageCount = items.Count;
			if (items.Count > 0)
			{
				_thread = new Thread(delegate()
				{
					QueueDownloads(items);
				});
				_thread.Name = string.Format("Parent download thread");
				_thread.Start(items);
				_thread.Join(Settings.Current.UserSettings.GetFetchTimeout() * 1000);
			}
		}

		public void BackgroundDownload()
		{
			if (_isWorking)
				return;
			
			List<Item> items = Repository.Default.ListItems().Where(i => !i.ImageDownloaded && !string.IsNullOrEmpty(i.ImageUrl)).ToList();

			// Sort so the selected categories get the images first
			List<Item> categoryItems = new List<Item>();
			List<Item> otherItems = new List<Item>();
			foreach (Category category in Settings.Current.LastCategories)
			{
				categoryItems = items.Where(i => i.Feed.Category.Equals(category)).ToList();
				otherItems = items.Where(i => !i.Feed.Category.Equals(category)).ToList();
				break;
			}

			categoryItems = SortItems(categoryItems);
			List<Item> sortedItems = new List<Item>();
			sortedItems.AddRange(categoryItems);
			sortedItems.AddRange(otherItems);

            if (items.Count > 0)
            {
				_thread = new Thread(delegate()
				{
					QueueDownloads(sortedItems);
				});
                _thread.Name = string.Format("Parent download thread");
                _thread.Start(items);
            }
            else
            {
                OnComplete(EventArgs.Empty);
            }
		}

		private void QueueDownloads(List<Item> items)
		{
			using (NSAutoreleasePool pool = new NSAutoreleasePool())
			{
                try
                {
                    _isWorking = true;

                    _waithandle = new AutoResetEvent(false);
                    for (int i = 0; i < items.Count; i++)
                    {
                        // Check if the image is there (repair mode)
                        if (File.Exists(Util.GetImageFullPath(items[i].ImageUrl)))
                        {
							items[i].ImageFilename = GetImageFilename(items[i]);
                            items[i].SetImageDownloaded();
                            continue;
                        }

                        ThreadInfo info = new ThreadInfo();
                        info.Item = items[i];
                        info.WaitHandle = _waithandle;

                        ThreadPool.QueueUserWorkItem(new WaitCallback(ThreadEntry), info);
                        bool success = _waithandle.WaitOne(_timeout);

                        if (!success)
                            Logger.Info("[Warning] ImageDownloader WaitOne timed out for {0}", items[i].Id);

                        // Call regardless of success
                        OnImageSaved(EventArgs.Empty);
                    }
                }
                catch (Exception ex)
                {
                    Logger.Warn("Exception in ImageDownloader.DownloadStart: {0}", ex.ToString());
                }
                finally
                {
                    OnComplete(EventArgs.Empty);
                    _isWorking = false;
                }
			}
		}

		private void ThreadEntry(object state)
		{
			ThreadInfo info = (ThreadInfo)state;

			using (NSAutoreleasePool pool = new NSAutoreleasePool())
			{
				DownloadAndSave(info.Item);
				info.WaitHandle.Set();
			}
		}

		private void DownloadAndSave(Item item)
		{
			if (string.IsNullOrEmpty(item.ImageUrl))
				return;

			try
			{
				item.ImageFilename = GetImageFilename(item);

				string fullPath = Path.Combine(Settings.Current.ImageFolder, item.ImageFilename);
				WebClient client = new WebClient();
				client.DownloadFile(item.ImageUrl, fullPath);

				item.SetImageDownloaded();
				Logger.Info("Saved image {0}", Path.GetFileName(item.ImageFilename));
			}
			catch (WebException e)
			{
				Logger.Warn("WebException downloading an image: {0}", e);
			}
			catch (ArgumentException e)
			{
				// For Path
				Logger.Warn("ArgumentException downloading an image: {0}", e);
			}
			catch (Exception e)
			{
				Logger.Warn("General Exception when downloading an image: {0}", e);
			}
		}

		private string GetImageFilename(Item item)
		{
			string extension = Path.GetExtension(item.ImageUrl);
			string filename = string.Format("{0}/{1}{2}", Settings.Current.ImageCacheFolder, item.Id, extension);
			return filename;
		}

		private class ThreadInfo
		{
			public AutoResetEvent WaitHandle { get; set; }
			public Item Item { get; set; }
		}
		
		private List<Item> SortItems(List<Item> items)
		{
			switch (Settings.Current.UserSettings.SortItemsBy)
			{
				case SortBy.Site:
					return items.SortBySite();

				case SortBy.Date:
				default:
					return items.SortByDate();
			}
		}

        public void TryStop()
        {
			Logger.Info("Attempting to stop the ImageDownloader");
            if (_thread != null)
                _thread.Abort();
        }

		/// <summary>
		/// Fires the <see cref="ImageSaved"/> event.
		/// </summary>
		/// <param name="e"></param>
		protected virtual void OnImageSaved(EventArgs e)
		{
			EventHandler handler;
			lock (_imageSavedLock)
			{
				handler = _imageSaved;
			}
			if (handler != null)
			{
				handler(this, e);
			}
		}

		/// <summary>
		/// Fires the <see cref="Complete"/> event.
		/// </summary>
		/// <param name="e"></param>
		protected virtual void OnComplete(EventArgs e)
		{
			EventHandler handler;
			lock (_completeLock)
			{
				handler = _complete;
			}
			if (handler != null)
			{
				handler(this, e);
			}
		}
    }
}
