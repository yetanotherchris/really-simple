#define LOGGING
using System;
using System.Reflection;
using System.Diagnostics;
using System.IO;

namespace ReallySimple.iPhone.Core
{
	/// <summary>
	/// A basic logging class.
	/// </summary>
	public class Logger
	{
		public static void Info(string format, params object[] args)
		{
			WriteLine(LoggingLevel.Info,format,args);
		}

		public static void Warn(string format, params object[] args)
		{
			WriteLine(LoggingLevel.Warn,format,args);
		}

		public static void Fatal(string format, params object[] args)
		{
			WriteLine(LoggingLevel.Fatal,format,args);
		}

		/// <summary>
		/// Writes the args to the default logging output using the format provided.
		/// </summary>
		public static void WriteLine(LoggingLevel level,string format, params object[] args)
		{
#if LOGGING
			var name = new StackFrame(2,false).GetMethod().Name;

			string prefix = string.Format("[{0} - {1}] ",level,name);
			string message = string.Format(prefix + format, args);

			Console.WriteLine(message);

			//if (level != LoggingLevel.Info)
				WriteToFile(message);
#endif
		}

		private static void WriteToFile(string message)
		{
			try
			{
				File.AppendAllText(Settings.Current.LogFile, string.Format("[{0}] {1}\n", DateTime.UtcNow.ToString(), message));
			}
			catch (IOException)
			{
			}
		}
	}

	public enum LoggingLevel
	{
		Info,
		Warn,
		Fatal
	}
}
