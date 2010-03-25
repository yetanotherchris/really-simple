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
		/// <summary>
		/// Writes the args to the default logging output using the format provided.
		/// </summary>
		public static void WriteLine(string format,params object[] args)
		{
			var name = new StackFrame(1,false).GetMethod().Name;
			string message = string.Format("(" + name + ") " + format, args);

#if LOGGING
			WriteToFile(message);
			Console.WriteLine(message);
#endif
		}

		private static void WriteToFile(string message)
		{
			try
			{
				File.AppendAllText(Settings.Current.LogFile, string.Format("[{0}] {1}\r\n", DateTime.UtcNow.ToString(), message));
			}
			catch (IOException)
			{
			}
		}
	}
}
