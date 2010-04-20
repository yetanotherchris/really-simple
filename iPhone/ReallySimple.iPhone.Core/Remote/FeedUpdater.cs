using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Net;
using System.IO;
using System.Xml.Serialization;
using ReallySimple.Core;
using System.IO.Compression;
using MonoTouch.Foundation;
using Mono.Data.Sqlite;

namespace ReallySimple.iPhone.Core.Remote
{
	/// <summary>
	/// Retrieves the latest feeds from the remote server, and saves them to the SQlite database.
	/// </summary>
	public class FeedUpdater
	{
        private Thread _thread;
		private readonly object _feedSavedLock;
		private event EventHandler _feedDone;

		/// <summary>
		/// Occurs when a feed is saved (or added to the save transaction). This is thread-safe.
		/// </summary>
		public event EventHandler FeedSaved
		{
			add
			{
				lock (_feedSavedLock)
				{
					_feedDone += value;
				}
			}
			remove
			{
				lock (_feedSavedLock)
				{
					_feedDone -= value;
				}
			}

		}

		public FeedUpdater()
		{
			_feedSavedLock = new object();
		}

		/// <summary>
		/// Retrieves a <see cref="List`Item"/> of the latest items from the remote server.
		/// </summary>
		/// <returns></returns>
		public List<Item> Update()
		{
			List<Item> list = new List<Item>();

			_thread = new Thread(delegate()
			{
				using (NSAutoreleasePool pool = new NSAutoreleasePool())
				{
					list = DownloadAndDeserialize();
				}
			});
			_thread.IsBackground = true;
			_thread.Start();

			// Allows for a timeout
			int timeout = Settings.Current.UserSettings.GetFetchTimeout();
			bool terminatedOk = _thread.Join(timeout * 1000);
			if (!terminatedOk)
				Logger.Info("The timeout of {0} seconds was reached when downloading the latest feeds", timeout);

			return list;
		}

		private List<Item> DownloadAndDeserialize()
		{
			using (NSAutoreleasePool pool = new NSAutoreleasePool())
			{
				List<Item> list = new List<Item>();

				try
				{
					// Get the url we need for the number of hours since the update
					string url = Settings.Current.GetUpdateUrl();
					HttpWebRequest webRequest = (HttpWebRequest)WebRequest.Create(url);
					Logger.Info("Using {0} for the feeds.", url);

					// Get the zip file, and decompress and de-serialize
					using (HttpWebResponse webResponse = (HttpWebResponse)webRequest.GetResponse())
					{
						Stream responseStream = webResponse.GetResponseStream();
						GZipStream zipStream = new GZipStream(responseStream, CompressionMode.Decompress);
						XmlSerializer serializer = new XmlSerializer(typeof(List<Item>));
						list = (List<Item>)serializer.Deserialize(zipStream);

						responseStream.Close();

						return list;
					}
				}
				catch (WebException e)
				{
					Logger.Warn("A WebException occured downloading the latest feeds: {0}", e);
					return list;
				}
				catch (IOException e)
				{
					Logger.Warn("An IOException occured downloading the latest feeds: {0}", e);
					return list;
				}
				catch (Exception e)
				{
					// Catch XmlSerializer errors
					Logger.Warn("A general exception occured downloading the latest feeds: {0}", e);
					return list;
				}
			}
		}

		public void SaveItems(List<Item> newItems)
		{
			// There may be zero items, so only call this once to avoid 300+ select queries when the cache is empty.
			List<Item> allItems = Repository.Default.ListItems().ToList();

			using (SqliteConnection connection = new SqliteConnection(Repository.Default.ItemsConnectionString))
			{
				connection.Open();

				SqliteRepository repository = (SqliteRepository)Repository.Default;

				// Perform the inserts inside a transaction to avoid the journal file being constantly opened + closed.
				using (SqliteTransaction transaction = connection.BeginTransaction())
				{
					SqliteCommand command = new SqliteCommand(connection);
					command.Transaction = transaction;
					
					List<Item> addItems = new List<Item>();
					bool hasItems = allItems.Count > 0;

					foreach (Item item in newItems)
					{
//						if (hasItems)
//						{
//							
//						}
//						else
//						{
//							
//						}
						// Check for duplicates in memory
						if (!addItems.Any(i => i.Equals(item)) && !allItems.Any(i => i.Equals(item)))
						{
							repository.SaveItemForTransaction(command, item);
							addItems.Add(item);
						}

						OnFeedSaved(EventArgs.Empty);
					}

					try
					{
						transaction.Commit();
						
						ItemCache.Current.Clear();
						Settings.Current.LastUpdate = DateTime.Now;
					}
					catch (SqliteException e)
					{
						Logger.Warn("An error occured committing the save transaction on new items:{0}", e);
						transaction.Rollback();
					}
				}
			}
		}

		/// <summary>
		/// Fires the <see cref="FeedSaved"/> event.
		/// </summary>
		/// <param name="e"></param>
		protected virtual void OnFeedSaved(EventArgs e)
		{
			EventHandler handler;
			lock (_feedSavedLock)
			{
				handler = _feedDone;
			}
			if (handler != null)
			{
				handler(this, e);
			}
		}
    }
}
