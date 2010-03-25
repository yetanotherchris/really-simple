using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using log4net;
using log4net.Config;

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

		public static void WriteLine(string format, params object[] args)
		{
			Log.InfoFormat(format, args);
		}
	}
}
