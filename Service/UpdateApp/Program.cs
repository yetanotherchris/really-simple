using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ReallySimple.FetchService;
using ReallySimple.Core;
using System.Configuration;
using log4net;
using log4net.Config;

namespace Update
{
	class Program
	{
		static void Main(string[] args)
		{
			Settings settings = new Settings(5,ConfigurationManager.ConnectionStrings["local"].ConnectionString);

			Feed feed = new Feed();
			feed.Url = "http://feeds.feedburner.com/Inhabitat";
			feed.FeedType = FeedType.RSS;

			FeedFetcher fetcher = new FeedFetcher(settings, new List<Item>());
			fetcher.Parse(feed);
			return;

			FeedUpdater updater = new FeedUpdater(settings);
			updater.ClearOldItems();
			updater.Update();
		}
	}
}
