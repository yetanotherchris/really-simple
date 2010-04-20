using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using log4net;
using log4net.Config;
using log4net.Appender;

namespace ReallySimple.FetchService
{
	public class Logger 
	{
		private static ILog Log;

		static Logger()
		{
			XmlConfigurator.Configure();
			Log = LogManager.GetLogger("default");
		}

		public static void Info(string format, params object[] args)
		{
			Log.InfoFormat(format, args);
		}

		public static void Warn(string format, params object[] args)
		{
			Log.WarnFormat(format, args);
		}

		public static void Fatal(string format, params object[] args)
		{
			Log.FatalFormat(format, args);
		}
	}
}
