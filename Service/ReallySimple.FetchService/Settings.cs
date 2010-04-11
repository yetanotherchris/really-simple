using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ReallySimple.FetchService
{
	public class Settings
	{
		/// <summary>
		/// The number of items to add to from the feed each time.
		/// 
		/// A run down of the costs:
		/// 4 updates a day (every 6 hours)
		/// 1 days maximum retention
		/// 5 items per feed
		/// 5 feeds per category
		/// 8 Categories
		/// 
		/// 4 * 1 * 5 * 5 * 8 = 800
		/// </summary>
		public int MaximumItemsPerFeed { get; private set; }
		public string ConnectionString { get; private set; }

		public bool WebMode { get; set; }
		public string FtpHost { get; private set; }
		public string FtpUsername { get; private set; }
		public string FtpPassword { get; private set; }

		public Settings(int maximumItemsPerFeed,string connectionString,bool webMode, string ftpHost,string ftpUsername,string ftpPassword)
		{
			MaximumItemsPerFeed = maximumItemsPerFeed;
			ConnectionString = connectionString;

			WebMode = webMode;
			FtpHost = ftpHost;
			FtpUsername = ftpUsername;
			FtpPassword = ftpPassword;
		}
	}
}
