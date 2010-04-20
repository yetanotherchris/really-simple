using System;
using System.Collections.Generic;
using ReallySimple.Core;
using System.Xml.Serialization;
using System.IO;
using System.IO.Compression;
using System.Net;

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
			try
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

				// FTP
				if (!_settings.WebMode)
				{
					FtpFile("3.bin.gz");
					FtpFile("6.bin.gz");
					FtpFile("12.bin.gz");
					FtpFile("24.bin.gz");
				}
			}
			catch (Exception e)
			{
				Logger.Fatal("Exception occured in Update: \n{0}", e);
			}
		}

		private void FtpFile(string filename)
		{
			string ftpFilename = string.Format("{0}/{1}", _settings.FtpHost, filename);

			FtpWebRequest request = (FtpWebRequest)WebRequest.Create(ftpFilename);
			request.Credentials = new NetworkCredential(_settings.FtpUsername, _settings.FtpPassword);
			request.Method = WebRequestMethods.Ftp.UploadFile;
			request.UseBinary = true;

			// 3.bin.gz
			byte[] contents = File.ReadAllBytes(filename);
			using (Stream stream = request.GetRequestStream())
			{
				stream.Write(contents, 0, contents.Length);
			}
			FtpWebResponse response = (FtpWebResponse)request.GetResponse();
			Logger.Info("FTP status for {0}: {1}{2}", ftpFilename, response.StatusCode, response.StatusDescription);
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

			Logger.Info("Saved {0} new items to the database", newItems.Count);
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

				if (_settings.WebMode)
					filename = string.Format("{0}/{1}", AppDomain.CurrentDomain.BaseDirectory, filename);

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
				Logger.Fatal("An error occured saving '{0}' to disk:\n\n{1}", filename, e);
			}
		}
	}
}
