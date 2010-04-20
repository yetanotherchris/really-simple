using System;
using System.Collections.Generic;
using System.Linq;
using System.Data;
using System.Text;
using System.Diagnostics;
using System.IO;
using System.Xml.Serialization;

namespace ReallySimple.Core
{
	public class Item
	{
		public Guid Id { get; set; }
		public Feed Feed { get; set; }

		public string Link { get; set; }
		public string Title { get; set; }
		public string Content { get; set; }

		[XmlIgnore]
		public string RawHtml { get; set; }
		public DateTime PublishDate { get; set; }
		
		public string ImageFilename { get;set; }
		public string ImageUrl { get; set; }

		[XmlIgnore]
		public bool IsRead { get; set; }
		[XmlIgnore]
		public bool IsLoaded { get;set; }
		[XmlIgnore]
		public bool ImageDownloaded { get; set; }

		public Item()
		{
			Id = Guid.NewGuid();
			Link = "";
			Title = "";
			Content = "";
			PublishDate = DateTime.UtcNow;
			Feed = null;
			IsRead = false;

			ImageUrl = "";
			ImageFilename = "";
			IsLoaded = false;
		}

		public override bool Equals(object obj)
		{
			Item item = obj as Item;
			if (item == null)
				return false;

			string titleA = item.Title.ToLower().Replace(" ","");
			string titleB = Title.ToLower().Replace(" ","");

			return titleA.Equals(titleB);
		}

		public override int GetHashCode()
		{
			string title = Title.ToLower().Replace(" ", "");
			return title.GetHashCode();
		}
	}
}
