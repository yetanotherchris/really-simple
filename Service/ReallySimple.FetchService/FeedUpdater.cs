using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ReallySimple.Core;
using System.Diagnostics;
using System.Xml.Serialization;
using System.IO;
using System.IO.Compression;
using System.Data.SqlClient;
using log4net.Repository.Hierarchy;
using log4net;

namespace ReallySimple.FetchService
{
	public class FeedUpdater
	{
		private Settings _settings;

		public FeedUpdater(Settings settings)
		{
			_settings = settings;
		}

		public void ClearOldItems()
		{
			BasicRepository.ConnectionString = _settings.ConnectionString;
			BasicRepository.ClearOldItems();
		}

		public void Update()
		{
			BasicRepository.ConnectionString = _settings.ConnectionString;
			FetchNewFeeds();

			// Last 3 hours
			List<Item> items = BasicRepository.ItemsForPast(3);
			SaveToDisk(items, "3.bin.gz");

			// Last 6 hours
			items = BasicRepository.ItemsForPast(6);
			SaveToDisk(items, "6.bin.gz");

			// Last 12 hours
			items = BasicRepository.ItemsForPast(12);
			SaveToDisk(items, "12.bin.gz");

			// Last 24 hours. 
			// Saturdays and sundays have very low postings so if the application is used on these days the user will get no news - 
			// account for this by simply giving them Friday's.
			int hours = 24;
			if (DateTime.UtcNow.DayOfWeek == DayOfWeek.Saturday)
				hours = 48;
			else if (DateTime.UtcNow.DayOfWeek == DayOfWeek.Sunday)
				hours = 72;

			items = BasicRepository.ItemsForPast(hours);
			SaveToDisk(items, "24.bin.gz");
		}

		private void FetchNewFeeds()
		{
			// Get all the items from the database
			List<Item> currentItems = BasicRepository.AllItems();
			IList<Feed> feeds = BasicRepository.AllFeeds();
			FeedFetcher fetcher = new FeedFetcher(_settings, currentItems);

			// Fetch each one
			List<Item> newItems = new List<Item>();
			foreach (Feed feed in feeds)
			{
				newItems.AddRange(fetcher.Parse(feed));
				Console.WriteLine("Finished {0}", feed.Url);
			}

			if (newItems.Count > 0)
			{
				SaveToDatabase(newItems);
			}
		}

		private void SaveToDatabase(List<Item> newItems)
		{
			foreach (Item item in newItems)
			{
				BasicRepository.SaveItem(item);
			}

			Logger.WriteLine("Saved {0} new items to the database", newItems.Count);
		}

		private void SaveToDisk(List<Item> items,string filename)
		{
			try
			{
				// Blank out each feed
				foreach (Item item in items)
				{
					Guid id = item.Feed.Id;
					item.Feed = new Feed();
					item.Feed.Id = id;
					item.Feed.FeedType = FeedType.RSS;
				}

				using (MemoryStream memoryStream = new MemoryStream())
				{
					XmlSerializer serializer = new XmlSerializer(typeof(List<Item>));
					serializer.Serialize(memoryStream, items);

					using (FileStream fileStream = new FileStream(filename, FileMode.Create))
					{
						byte[] buffer = memoryStream.ToArray();
						using (GZipStream gzip = new GZipStream(fileStream, CompressionMode.Compress))
						{
							gzip.Write(buffer, 0, buffer.Length);
						}
					}
				}
			}
			catch (IOException e)
			{
				Logger.WriteLine("An error occured saving '{0}' to disk:\n\n{1}", filename, e);
			}
		}
	}
}
