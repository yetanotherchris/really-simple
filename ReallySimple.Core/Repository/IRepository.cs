using System;
using System.Collections.Generic;

namespace ReallySimple.Core
{
	/// <summary>
	/// A repository for all CRUD operations for the domain objects.
	/// </summary>
	public interface IRepository
	{
		/// <summary>
		/// The connection string for all <see cref="Item"/> objects in the database. This may be the same as <see cref="FeedConnectionString"/>.
		/// </summary>
		string ItemsConnectionString { get; set; }

		/// <summary>
		/// The connection string for all <see cref="Category"/>,<see cref="Site"/> and <see cref="Feed"/> 
		/// objects in the database. This may be the same as <see cref="ItemsConnectionString"/>.
		/// </summary>
		string FeedConnectionString { get; set; }

		#region Categories
		/// <summary>
		/// Retrieves a list of all <see cref="Category"/> objects from the database or cache.
		/// </summary>
		IList<Category> ListCategories();

		/// <summary>
		/// Retrieves a particular <see cref="Category"/> from the database or cache.
		/// </summary>
		Category GetCategory(Guid id);
		#endregion

		#region Feeds
		/// <summary>
		/// Retrieves a list of all <see cref="Feed"/> objects from the database or cache.
		/// </summary>
		IList<Feed> ListFeeds();

		/// <summary>
		/// Retrieves a particular <see cref="Feed"/> from the database or cache.
		/// </summary>
		Feed GetFeed(Guid id);

		/// <summary>
		/// Retrieves all <see cref="Feed"/> objects for a particular <see cref="Category"/>, from the database or cache.
		/// </summary>
		IList<Feed> FeedsForCategory(Category category);

		/// <summary>
		/// The current number of <see cref="Feed"/> objects in the database or cache.
		/// </summary>
		long CountFeeds(); 
		#endregion
		
		#region Site
		/// <summary>
		/// Retrieves a list of all <see cref="Site"/> objects from the database or cache.
		/// </summary>
		IList<Site> ListSites();

		/// <summary>
		/// Retrieves a particular <see cref="Site"/> from the database or cache.
		/// </summary>
		Site GetSite(Guid id);
		#endregion

		#region Items
		/// <summary>
		/// Saves a single <see cref="Item"/> objects to the database.
		/// </summary>
		/// <param name="item"></param>
		void SaveItem(Item item);

		/// <summary>
		/// Updates an <see cref="Item"/>'s image, saving the <see cref="Item.ImageFilename"/> property
		/// to the database and setting the <see cref="Item.ImageDownloaded"/> to true. The Id property
		/// is used for the update.
		/// </summary>
		void UpdateItemImage(Item item);

		/// <summary>
		/// Retrieves a particular <see cref="Item"/> from the database or cache.
		/// </summary>
		Item GetItem(Guid id);

		/// <summary>
		/// Retrieves a list of all <see cref="Item"/> objects from the database or cache.
		/// </summary>
		IList<Item> ListItems();

		/// <summary>
		/// Retrieves all <see cref="Item"/> objects for a particular <see cref="Category"/>, from the database or cache.
		/// </summary>
		IList<Item> ItemsForCategories(IList<Category> categories);

		/// <summary>
		/// Removes all <see cref="Item"/> objects from the database before the provided date. This should also
		/// clear all images before this date.
		/// </summary>
		void ClearItemsBeforeDate(DateTime date);
		#endregion
	}
}
