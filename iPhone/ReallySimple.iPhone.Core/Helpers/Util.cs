using System;
using System.IO;

namespace ReallySimple.iPhone.Core
{
	public class Util
	{
		public static string GetImageFullPath(string filename)
		{
			return Path.Combine(Settings.Current.ImageFolder, filename);
		}
	}
}
