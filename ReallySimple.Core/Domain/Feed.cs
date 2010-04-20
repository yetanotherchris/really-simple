using System;
using System.Linq;
using System.Collections.Generic;
using System.Data;
using System.Xml.Serialization;

namespace ReallySimple.Core
{
	public class Feed
	{
		public Guid Id { get; set; }

		[XmlIgnore]
		public Category Category { get; set; }

		[XmlIgnore]
		public Site Site { get; set; }

		[XmlIgnore]
		public string Url { get; set; }

		[XmlIgnore]
		public FeedType FeedType { get; set; }
		
		[XmlIgnore]
		public string Cleaner { get; set; }

		public override bool Equals(object obj)
		{
			Feed feed = obj as Feed;
			if (feed != null)
				return feed.Id.Equals(Id);
			else
				return false;
		}

		public override int GetHashCode()
		{
			return Id.GetHashCode();
		}
	}
}
