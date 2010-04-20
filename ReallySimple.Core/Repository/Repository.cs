using System;

namespace ReallySimple.Core
{
	public class Repository
	{
		private static IRepository _instance;

		public static IRepository Default
	    {
	        get
	        {
				if (_instance == null)
					throw new NullReferenceException("Repository.Initialize() has not been set");

	            return _instance;
	        }
	    }

		public static void SetInstance(IRepository instance,string feedsConnectionString,string itemsConnectionString)
		{
			_instance = instance;
			_instance.FeedConnectionString = feedsConnectionString;
			_instance.ItemsConnectionString = itemsConnectionString;
		}
	}
}
