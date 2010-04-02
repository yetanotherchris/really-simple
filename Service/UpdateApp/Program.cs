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

namespace Update
{
	class Program
	{
		static void Main(string[] args)
		{
			if (args.Length == 0)
			{
				ProcessStartInfo info = new ProcessStartInfo(@"C:\ReallySimple-Updater\Update.exe", "launch");
				info.CreateNoWindow = true;
				info.WindowStyle = ProcessWindowStyle.Hidden;
				Process process = Process.Start(info);
			}
			else if (args[0] == "launch")
			{
				ProcessStartInfo info = new ProcessStartInfo(@"C:\ReallySimple-Updater\Update.exe","update");
				info.CreateNoWindow = true;
				info.WindowStyle = ProcessWindowStyle.Hidden;
				Process process = Process.Start(info);
				process.WaitForExit();

				info = new ProcessStartInfo(@"ftp.exe", @"-s:C:\ReallySimple-Updater\ftp.txt");
				info.CreateNoWindow = true;
				info.WindowStyle = ProcessWindowStyle.Hidden;
				process = Process.Start(info);
				process.WaitForExit();
			}
			else if (args[0] == "update")
			{
				Settings settings = new Settings(5, ConfigurationManager.ConnectionStrings["local"].ConnectionString);

				FeedUpdater updater = new FeedUpdater(settings);
				updater.ClearOldItems();
				updater.Update();
			}
		}
	}
}
