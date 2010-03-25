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

			FeedUpdater updater = new FeedUpdater(settings);
			updater.Update();
		}
	}
}
