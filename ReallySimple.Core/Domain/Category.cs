using System;
using System.Linq;
using System.Collections.Generic;
using System.Data;
using System.Xml.Serialization;

namespace ReallySimple.Core
{
	public class Category : IEqualityComparer<Category>
	{
		public Guid Id { get; set; }

		[XmlIgnore]
		public string Title { get; set; }

		public bool Equals(Category x, Category y)
		{
			if (x == null || y == null)
				return false;

			return x.Id.Equals(y.Id);
		}

		public override bool Equals(object obj)
		{
			Category category = obj as Category;
			if (category != null)
				return category.Id.Equals(Id);
			else
				return false;
		}

		public override int GetHashCode()
		{
			return Id.GetHashCode();
		}

		public int GetHashCode(Category obj)
		{
			return obj.GetHashCode();
		}
	}
}
