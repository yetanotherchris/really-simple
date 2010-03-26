using System;
using System.Linq;
using System.Collections.Generic;

namespace ReallySimple.Core
{
	public static class Extensions
	{	
		public static List<Item> SortByDate(this IList<Item> list)  
		{  
			return list.OrderByDescending(i => i.PublishDate.ToString()).ToList(); 
		}
		
		public static List<Item> SortBySite(this IList<Item> list)  
		{  
		    return list.OrderByDescending(i => i.Feed.Site.Title).ToList(); 
		}
	}
}
