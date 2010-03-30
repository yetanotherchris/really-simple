using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using MonoTouch.Foundation;
using Mono.Data.Sqlite;
using MonoTouch.Dialog;
using ReallySimple.Core;
using System.Reflection;

namespace ReallySimple.iPhone.Core
{
	/// <summary>
	/// Holds application settings and user settings. 
	/// </summary>
	/// <remarks>Settings is responsible for reading and writing all application and user settings
	/// to and from the info.plist file (NSUserDefaults). The class also has contains housekeeping methods
	/// and initializes all defaults on startup. This class is a singleton.</remarks>
	public class Settings
	{
		#region Fields
		private string _imageFolder;
		private string _imageCacheFolder;
		private string _updateUrl;
		private string _logFile;
		private static readonly Settings _current = new Settings(); 
		#endregion

		#region Properties
		public SqliteRepository RepositoryInstance { get; set; }

		/// <summary>
		/// All user-configurable settings.
		/// </summary>
		public UserSettings UserSettings { get; private set; }

		/// <summary>
		/// The categories last selected to view. This is empty if there aren't any/first run.
		/// </summary>
		public IList<Category> LastCategories { get; set; }

		/// <summary>
		/// The last time the updated feeds zip file was downloaded and saved.
		/// </summary>
		public DateTime LastUpdate { get; set; }

		/// <summary>
		/// The ID of the view last viewed.
		/// </summary>
		public string LastControllerId { get; set; }

		/// <summary>
		/// The Guid of the item that was last viewed, if the user was on the webview.
		/// </summary>
		public string LastItemId { get; set; }

		/// <summary>
		/// MyDocuments folder to store the images in.
		/// </summary>
		public string ImageFolder
		{
			get { return _imageFolder; }
		}

		/// <summary>
		/// The full path to the image cache for today. 
		/// </summary>
		public string ImageCacheFolder
		{
			get
			{
				return _imageCacheFolder;
			}
		}
		
		public string LogFile
		{
			get
			{
				return _logFile;	
			}
		}

		/// <summary>
		/// The filename for the items database, stored in MyDocuments.
		/// </summary>
		public string ItemsDatabaseFilename
		{
			get { return "1.0.db"; }
		}

		/// <summary>
		/// The current version of the app (based on this assembly)
		/// </summary>
		public string Version
		{
			get { return Assembly.GetExecutingAssembly().GetName().Version.ToString(); }
		}

		/// <summary>
		/// The current instance of the <see cref="Settings"/>
		/// </summary>
		public static Settings Current
		{
			get { return _current; }
		} 
		#endregion

		#region Ctor and defaults
		Settings()
		{
			// System settings
			_imageFolder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "imagecache");
			_imageCacheFolder = Path.Combine(_imageFolder, DateTime.Today.Ticks.ToString());
			CreateImageCacheFolder();

			_updateUrl = "http://update.rlysimple.com/{0}.bin.gz";
			_logFile = string.Format("{0}/reallysimple.log", Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments));

			// User-generated settings
			Defaults();
		}

		private void CreateImageCacheFolder()
		{
			try
			{
				if (!Directory.Exists(_imageCacheFolder))
					Directory.CreateDirectory(_imageCacheFolder);
			}
			catch (IOException e)
			{
				Logger.Warn("An IOException occured while creating the image cache folder: \n{0}", e);
			}
		}

		/// <summary>
		/// Resets all settings into their default state.
		/// </summary>
		public void Defaults()
		{
			// This method could be replaced with NSUserDefaults.StandardUserDefaults.RegisterDefaults
			LastCategories = new List<Category>();
			LastControllerId = "";
			LastItemId = "";
			LastUpdate = DateTime.Now.AddDays(-1);

			UserSettings = new UserSettings();
			UserSettings.SetFetchTimeout(20);
			UserSettings.SetKeepItemsFor(24);
			UserSettings.IgnoreReadItems = true;
			UserSettings.ImagesEnabled = false;
			//UserSettings.OpenInSafari = true;
		} 
		#endregion

		#region Read, Write
		/// <summary>
		/// Reads the settings from NSUserDefaults.StandardUserDefaults.
		/// </summary>
		public void Read()
		{

			try
			{
				var defaults = NSUserDefaults.StandardUserDefaults;

				// Last view
				LastControllerId = defaults.StringForKey("lastcontrollerid") ?? "";

				// The last item index from the list that was viewed.
				LastItemId = defaults.StringForKey("lastitemid");

				// List of categories, stored as guid;guid;guid;
				LastCategories = new List<Category>();
				string lastCategories = defaults.StringForKey("lastcategories");
				if (!string.IsNullOrEmpty(lastCategories))
				{
					try
					{
						// Even a single category will have a ; at the end.
						string[] ids = lastCategories.Split(';');

						foreach (string id in ids)
						{
							Guid guid = new Guid(id);
							Category category = Repository.Default.GetCategory(guid);
							LastCategories.Add(category);
						}
					}
					catch (Exception ex)
					{
						Logger.Warn("Error reading lastCategories in Settings: {0}", ex.ToString());
					}
				}

				// Last update, as ticks.
				long lastupdate = 0;
				long.TryParse(defaults.StringForKey("lastupdate"), out lastupdate);
				if (lastupdate > 0)
					LastUpdate = new DateTime(lastupdate);
				else
					LastUpdate = DateTime.Now.AddDays(-1);

				// Number of days to keep items
				int keepItemsFor = defaults.IntForKey("keepitemsfor");
				if (keepItemsFor < 1)
					UserSettings.SetKeepItemsFor(24);
				else
					UserSettings.SetKeepItemsFor(keepItemsFor);

				// Timeout
				int timeout = defaults.IntForKey("timeout");
				if (timeout < 1)
					UserSettings.SetFetchTimeout(20);
				else
					UserSettings.SetFetchTimeout(timeout);

				// Sortby
				int sortby = defaults.IntForKey("sortitemsby");
				if (timeout < 1)
					UserSettings.SortItemsBy = SortBy.Date;
				else
					UserSettings.SortItemsBy = (SortBy)sortby;

				// Others
				UserSettings.IgnoreReadItems = defaults.BoolForKey("ignorereaditems");
				UserSettings.ImagesEnabled = defaults.BoolForKey("imagesenabled");
				//UserSettings.OpenInSafari = defaults.BoolForKey("openinsafari");
			}
			catch (Exception e)
			{
				Logger.Warn("An exception occured while reading the settings: \n{0}", e);
			}
		}

		/// <summary>
		/// Writes all NSUserDefaults to the info.plist file.
		/// </summary>
		public void Write()
		{
			try
			{
				// Last controller and the last guid
				NSUserDefaults.StandardUserDefaults.SetString(LastControllerId ?? "", "lastcontrollerid");
				NSUserDefaults.StandardUserDefaults.SetString(LastItemId ?? "", "lastitemid");

				// Last update
				NSUserDefaults.StandardUserDefaults.SetString(LastUpdate.Ticks.ToString(), "lastupdate");

				// The last categories
				var catIds = LastCategories.Select<Category, string>(i => i.Id.ToString());
				NSUserDefaults.StandardUserDefaults.SetString(string.Join(";", catIds.ToArray()), "lastcategories");

				// User configurable settings
				NSUserDefaults.StandardUserDefaults.SetInt(UserSettings.GetKeepItemsFor(), "keepitemsfor");
				NSUserDefaults.StandardUserDefaults.SetInt(UserSettings.GetFetchTimeout(), "timeout");
				NSUserDefaults.StandardUserDefaults.SetBool(UserSettings.IgnoreReadItems, "ignorereaditems");
				NSUserDefaults.StandardUserDefaults.SetBool(UserSettings.ImagesEnabled, "imagesenabled");
				NSUserDefaults.StandardUserDefaults.SetInt((int)UserSettings.SortItemsBy, "sortitemsby");
				//NSUserDefaults.StandardUserDefaults.SetBool(UserSettings.OpenInSafari, "openinsafari");

				NSUserDefaults.StandardUserDefaults.Synchronize();
			}
			catch (Exception e)
			{
				Logger.Warn("An exception occured while writing the settings: \n{0}", e);
			}
		} 
		#endregion

		#region Housekeeping
		public void ClearCache(bool deleteImages)
		{
			ImageDownloader.Current.TryStop();
			RepositoryInstance.WipeUserDatabase();
			ItemCache.Current.Clear();

			LastUpdate = DateTime.Now.AddDays(-1);
			LastControllerId = "";
			LastItemId = "";

			if (deleteImages)
				ClearImageCache();
		}

		public void ClearImageCache()
		{
			try
			{
				if (Directory.Exists(ImageFolder))
				{
					try
					{
						Directory.Delete(ImageFolder, true);
						CreateImageCacheFolder();
					}
					catch (IOException e)
					{
						Logger.Warn("IOException deleting {0}: {1}", ImageFolder, e.Message);
					}
				}
			}
			catch (IOException e)
			{
				Logger.Warn("An exception occured while clearing the image cache folder: \n{0}", e);
			}
		}

		public string GetUpdateUrl()
		{
			TimeSpan timespan = DateTime.Now.Subtract(LastUpdate);
			int hours = 24;

			if (timespan.TotalHours < 3)
				hours = 3;
			else if (timespan.TotalHours > 3 && timespan.TotalHours <= 6)
				hours = 6;
			else if (timespan.TotalHours > 6 && timespan.TotalHours <= 12)
				hours = 12;

			return string.Format(_updateUrl, hours);
		}

		/// <summary>
		/// Determines from the category updates times, if a new fetch is required.
		/// </summary>
		/// <returns></returns>
		public bool NeedsUpdate()
		{
			if (LastUpdate < DateTime.Now.AddHours(-3))
				return true;
			else
				return false;
		}

		/// <summary>
		/// Sets the items database and feeds.db database files to non-deployed paths
		/// </summary>
		public void UnitTestMode()
		{
			var feedDatabaseConnection = string.Format("Data Source={0}/Feeds.db;Version=3;", "/Volumes/Public/Monotouch/ReallySimple/ReallySimple.UI");
			var userDatabaseFilePath = string.Format("{0}/User.db", "/tmp");

			string itemsDbFilePath = string.Format("{0}/{1}", Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), Settings.Current.ItemsDatabaseFilename);
			var userDatabaseConnection = string.Format("Data Source={0};Version=3;", itemsDbFilePath);

			RepositoryInstance.WipeUserDatabase();
		} 
		#endregion
	}
}
