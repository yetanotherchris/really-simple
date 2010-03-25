using System;
using System.Collections.Generic;
using System.Linq;
using System.Data;
using System.Xml.Serialization;

namespace ReallySimple.Core
{
	public class Site
	{
		public Guid Id { get; set; }

		[XmlIgnore]
		public string Title { get; set; }

		[XmlIgnore]
		public string Url { get; set; }
		
		public override bool Equals (object obj)
		{
			Site site = obj as Site;
			if (site != null)
				return site.Id.Equals(Id);
			else
				return false;
		}
		
		public override int GetHashCode()
		{
			return Id.GetHashCode();
		}
	}
}

