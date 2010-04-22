using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ReallySimple.FetchService;
using ReallySimple.Core;
using System.Configuration;
using log4net;
using log4net.Config;
using System.Diagnostics;
using System.IO;

namespace Update
{
	class Program
	{
		static void Main(string[] args)
		{
			string path = ConfigurationManager.AppSettings["binarypath"];
			string exePath = Path.Combine(path, "Update.exe");
			
			string connection = ConfigurationManager.ConnectionStrings["default"].ConnectionString;
			string localpath = ConfigurationManager.AppSettings["localfolder"];
			bool isEC2 = Convert.ToBoolean(ConfigurationManager.AppSettings["isEC2"]);

			/*
			string ftpHost = ConfigurationManager.AppSettings["ftp-host"];
			string ftpUsername = ConfigurationManager.AppSettings["ftp-username"];
			string ftpPassword = ConfigurationManager.AppSettings["ftp-password"];
			 */

			Settings settings = new Settings(5, connection, "", "", "",false, localpath);

			FeedUpdater updater = new FeedUpdater(settings);
			updater.ClearOldItems();
			updater.Update();

			// Shutdown the PC
			if (isEC2)
			{
				Process.Start("shutdown", "/s");
			}
		}
	}
}
