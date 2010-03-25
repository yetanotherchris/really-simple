using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using ReallySimple.Core;
using Mono.Data.Sqlite;
using System.Data;

namespace ReallySimple.iPhone.Core
{
	public static class Extensions
	{
		/// <summary>
		/// Loads an <see cref="Item"/> from the database. Items hold minimal data by default.
		/// </summary>
		public static void LazyLoad(this Item item)
		{
			((SqliteRepository)Repository.Default).LazyLoadItem(item);
		}

		/// <summary>
		/// Sets the <see cref="Item.IsLoaded"/> property to false, and clears the properties that are lazily loaded.
		/// </summary>
		/// <param name="item"></param>
		public static void UnLoad(this Item item)
		{
			item.IsLoaded = false;
			item.Content = "";
			item.Title = "";
			item.RawHtml = "";
			item.ImageFilename = "";
		}

		/// <summary>
		/// Saves the <see cref="Item.Read"/> property to the database using the provided 'read' parameter.
		/// </summary>
		public static void UpdateReadStatus(this Item item,bool read)
		{
			((SqliteRepository)Repository.Default).UpdateItemReadStatus(item, read);
		}

		/// <summary>
		/// Saves the <see cref="Item.ImageFilename"/> property to the database and sets its <see cref="Item.ImageDownloaded"/>
		/// property to true.. The provided <see cref="Item"/> should have a valid Id property.
		/// </summary>
		public static void SetImageDownloaded(this Item item)
		{
			Repository.Default.UpdateItemImage(item);
		}
	}
}
