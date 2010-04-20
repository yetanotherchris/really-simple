using System;
using System.Collections.Generic;
using System.Linq;
using Mono.Data.Sqlite;
using System.Data;
using System.Diagnostics;
using System.IO;
using ReallySimple.Core;

namespace ReallySimple.iPhone.Core
{
	/// <summary>
	/// A SQlite implementation of IRepository, optimized for the iPhone.
	/// </summary>
	public class SqliteRepository : IRepository
	{
		private string _itemsDatabaseFilePath;
		public string ItemsConnectionString { get; set; }
		public string FeedConnectionString { get; set; }

		public SqliteRepository(string itemsDatabaseFilePath)
		{
			_itemsDatabaseFilePath = itemsDatabaseFilePath;
		}

		#region Housekeeping
		/// <summary>
		/// Deletes the items database file and recreates it - but does nothing if it exists.
		/// </summary>
		public void WipeUserDatabase()
		{
			// For cache clears
			try
			{
				if (File.Exists(_itemsDatabaseFilePath))
					File.Delete(_itemsDatabaseFilePath);

				CreateDatabase();
			}
			catch (IOException e)
			{
				Logger.Fatal("Unable to create the items database: \n{0}", e);
			}
		}
		
		/// <summary>
		/// Creates the items database file and its schema if it doesn't exist.
		/// </summary>
		public void CreateDatabase()
		{
			// For startup checks
			if (!File.Exists(_itemsDatabaseFilePath))
			{
				// Create the file
				SqliteConnection.CreateFile(_itemsDatabaseFilePath);
				
				// And the schema
				using (SqliteConnection connection = new SqliteConnection(ItemsConnectionString))
				{
					connection.Open();
					using (SqliteCommand command = new SqliteCommand(connection)	)
					{
						command.CommandText = "CREATE TABLE \"items\" ("+
												"\"pkid\" INTEGER PRIMARY KEY, "+
												"\"id\" GUID NOT NULL, "+
												"\"feedid\" GUID NOT NULL,"+
												"\"createdate\" DATETIME," +
												"\"isread\" INTEGER,"+
												"\"link\" TEXT," +
												"\"title\" TEXT," +
												"\"content\" TEXT,"+
												"\"publishdate\" DATETIME,"+
												"\"imageDownloaded\" INTEGER," +
												"\"imageFilename\" TEXT," +
												"\"imageUrl\" TEXT)";
						
						command.ExecuteNonQuery();
					}
				}
			}
		}
		#endregion

		#region Categories
		public IList<Category> ListCategories()
		{
			if (CategoryCache.Current.IsCached())
				return CategoryCache.Current.Items;
			
			IList<Category> list = new List<Category>();

			try
			{
				using (SqliteConnection connection = new SqliteConnection(FeedConnectionString))
				{
					connection.Open();
					using (SqliteCommand command = new SqliteCommand(connection))
					{
						command.CommandText = "SELECT id,title FROM categories";

						using (SqliteDataReader reader = command.ExecuteReader())
						{
							while (reader.Read())
							{
								Category category = new Category();
								category.Id = reader.GetGuid(0);
								category.Title = reader.GetString(1);

								list.Add(category);
							}
						}
					}
				}
			}
			catch (SqliteException e)
			{
				Logger.Warn("SqliteException occured while listing categories: \n{0}",e);
			}

			CategoryCache.Current.Update(list);
			return list;
		}

		public Category GetCategory(Guid id)
		{
			return ListCategories().First(i => i.Id.Equals(id));
		}
		#endregion
		
		#region Feeds
		public IList<Feed> ListFeeds()
		{
			if (FeedCache.Current.IsCached())
				return FeedCache.Current.Items;
			
			IList<Feed> list = new List<Feed>();

			try
			{
				using (SqliteConnection connection = new SqliteConnection(FeedConnectionString))
				{
					connection.Open();
					using (SqliteCommand command = new SqliteCommand(connection))
					{
						command.CommandText = @"SELECT f.*,c.title as categoryTitle,s.title as siteTitle, s.url as siteUrl " +
							"FROM feeds f INNER JOIN categories c ON UPPER(c.id) = UPPER(f.categoryid) " +
							"INNER JOIN sites s on UPPER(s.id) = UPPER(f.siteid)";

						using (SqliteDataReader reader = command.ExecuteReader())
						{
							while (reader.Read())
							{
								Category category = new Category();
								category.Id = (Guid)reader["categoryid"];
								category.Title = (string)reader["categoryTitle"];

								Site site = new Site();
								site.Id = (Guid)reader["siteid"];
								site.Title = (string)reader["siteTitle"];
								site.Url = (string)reader["siteUrl"];

								Feed feed = new Feed();
								feed.Id = (Guid)reader["id"];
								feed.Category = category;
								feed.Site = site;
								feed.Cleaner = (string)reader["cleaner"];
								feed.Url = (string)reader["url"];

								long type = (long)reader["type"];
								feed.FeedType = (FeedType)type;

								list.Add(feed);
							}
						}
					}
				}
			}
			catch (SqliteException e)
			{
				Logger.Warn("SqliteException occured while listing feeds: \n{0}", e);
			}
			
			FeedCache.Current.Update(list);
			return list;
		}

		public Feed GetFeed(Guid id)
		{
			return ListFeeds().First(i => i.Id.Equals(id));
		}
		
		public IList<Feed> FeedsForCategory(Category category)
		{
			var feedCache = ListFeeds().Where(f => f.Category.Id.Equals(category.Id));
			var filtered = feedCache.ToList();
			
			return filtered;
		}		
		
		public long CountFeeds ()
		{
			return ListFeeds().Count();
		}
		#endregion
		
		#region Sites
		public IList<Site> ListSites()
		{
			if (SiteCache.Current.IsCached())
				return SiteCache.Current.Items;
			
			IList<Site> list = new List<Site>();

			try
			{
				using (SqliteConnection connection = new SqliteConnection(FeedConnectionString))
				{
					connection.Open();
					using (SqliteCommand command = new SqliteCommand(connection))
					{
						command.CommandText = "SELECT id,title,url FROM sites";

						using (SqliteDataReader reader = command.ExecuteReader())
						{
							while (reader.Read())
							{
								Site site = new Site();
								site.Id = reader.GetGuid(0);
								site.Title = reader.GetString(1);
								site.Url = reader.GetString(2);

								list.Add(site);
							}
						}
					}
				}
			}
			catch (SqliteException e)
			{
				Logger.Warn("SqliteException occured while listing sites: \n{0}", e);
			}
			
			SiteCache.Current.Update(list);
			return list;
		}

		public Site GetSite(Guid id)
		{
			return ListSites().First(i => i.Id.Equals(id));
		}
		#endregion
		
		#region Items
		/// <summary>
		/// Inserts an item via the provided SqliteCommand, and assumes to be running inside a transaction scope.
		/// </summary>
		internal void SaveItemForTransaction( SqliteCommand command, Item item)
		{
			string sql = @"INSERT INTO items (pkid,id,feedid,createdate,isread,link,title,content,publishdate,imageDownloaded,imageFilename,imageUrl) ";
			sql += "VALUES (null,@id,@feedid,@createdate,0,@link,@title,@content,@publishdate,0,@imageFilename,@imageUrl)";
			command.CommandText = sql;

			SqliteParameter parameter = new SqliteParameter("@id", DbType.String);
			parameter.Value = item.Id.ToString();
			command.Parameters.Add(parameter);

			parameter = new SqliteParameter("@feedid", DbType.String);
			parameter.Value = item.Feed.Id.ToString();
			command.Parameters.Add(parameter);

			parameter = new SqliteParameter("@createdate", DbType.DateTime);
			parameter.Value = DateTime.Now;
			command.Parameters.Add(parameter);

			parameter = new SqliteParameter("@link", DbType.String);
			parameter.Value = item.Link ?? "";
			command.Parameters.Add(parameter);

			parameter = new SqliteParameter("@title", DbType.String);
			parameter.Value = item.Title ?? "";
			command.Parameters.Add(parameter);

			parameter = new SqliteParameter("@content", DbType.String);
			parameter.Value = item.Content ?? "";
			command.Parameters.Add(parameter);

			parameter = new SqliteParameter("@publishdate", DbType.DateTime);
			parameter.Value = item.PublishDate;
			command.Parameters.Add(parameter);

			parameter = new SqliteParameter("@imageFilename", DbType.String);
			parameter.Value = (item.ImageFilename == null) ? "" : item.ImageFilename;
			command.Parameters.Add(parameter);

			parameter = new SqliteParameter("@imageUrl", DbType.String);
			parameter.Value = (item.ImageUrl == null) ? "" : item.ImageUrl;
			command.Parameters.Add(parameter);

			command.ExecuteNonQuery();
		}

		public void SaveItem(Item item)
		{
			// Check for duplicates in memory
			if (ListItems().FirstOrDefault(i => i.Link.Equals(item.Link)) != null)
			    return;

			try
			{
				using (SqliteConnection connection = new SqliteConnection(ItemsConnectionString))
				{
					connection.Open();
					using (SqliteCommand command = new SqliteCommand(connection))
					{
						string sql = @"INSERT INTO items (pkid,id,feedid,createdate,isread,link,title,content,publishdate,imageDownloaded,imageFilename,imageUrl) ";
						sql += "VALUES (null,@id,@feedid,@createdate,0,@link,@title,@content,@publishdate,0,@imageFilename,@imageUrl)";
						command.CommandText = sql;

						SqliteParameter parameter = new SqliteParameter("@id", DbType.String);
						parameter.Value = item.Id.ToString();
						command.Parameters.Add(parameter);

						parameter = new SqliteParameter("@feedid", DbType.String);
						parameter.Value = item.Feed.Id.ToString();
						command.Parameters.Add(parameter);

						parameter = new SqliteParameter("@createdate", DbType.DateTime);
						parameter.Value = DateTime.Now;
						command.Parameters.Add(parameter);

						parameter = new SqliteParameter("@link", DbType.String);
						parameter.Value = item.Link ?? "";
						command.Parameters.Add(parameter);

						parameter = new SqliteParameter("@title", DbType.String);
						parameter.Value = item.Title ?? "";
						command.Parameters.Add(parameter);

						parameter = new SqliteParameter("@content", DbType.String);
						parameter.Value = item.Content ?? "";
						command.Parameters.Add(parameter);

						parameter = new SqliteParameter("@publishdate", DbType.DateTime);
						parameter.Value = item.PublishDate;
						command.Parameters.Add(parameter);

						parameter = new SqliteParameter("@imageFilename", DbType.String);
						parameter.Value = (item.ImageFilename == null) ? "" : item.ImageFilename;
						command.Parameters.Add(parameter);

						parameter = new SqliteParameter("@imageUrl", DbType.String);
						parameter.Value = (item.ImageUrl == null) ? "" : item.ImageUrl;
						command.Parameters.Add(parameter);

						command.ExecuteNonQuery();
					}
				}
			}
			catch (SqliteException e)
			{
				Logger.Warn("SqliteException occured while saving an item {0}: \n{1}", item.Link,e);
			}
		}

		public void UpdateItemImage(Item item)
		{
			try
			{
				using (SqliteConnection connection = new SqliteConnection(ItemsConnectionString))
				{
					connection.Open();
					using (SqliteCommand command = new SqliteCommand(connection))
					{
						string sql = @"UPDATE items SET imageFilename=@imageFilename,imageDownloaded=1 ";
						sql += "WHERE id=@id";
						command.CommandText = sql;

						SqliteParameter parameter = new SqliteParameter("@id", DbType.String);
						parameter.Value = item.Id.ToString();
						command.Parameters.Add(parameter);

						parameter = new SqliteParameter("@imageFilename", DbType.String);
						parameter.Value = item.ImageFilename ?? "";
						command.Parameters.Add(parameter);

						command.ExecuteNonQuery();
					}
				}
			}
			catch (SqliteException e)
			{
				Logger.Warn("SqliteException occured while listing performing UpdateItemImage for {0}: \n{1}", item.Link,e);
			}
			
			item.ImageDownloaded = true;
		}
		
		public Item GetItem(Guid id)
		{
			return ListItems().First(i => i.Id.Equals(id));
		}
		
		/// <summary>
		/// Loads all Feeds initially, and then Item ids. Items are then lazy loaded.
		/// </summary>
		/// <returns></returns>
		public IList<Item> ListItems()
		{
			if (ItemCache.Current.IsCached())
				return ItemCache.Current.Items;
			
			List<Item> list = new List<Item>();
			IList<Feed> allFeeds = ListFeeds(); // faster than relying on Feed.Read()

			try
			{
				using (SqliteConnection connection = new SqliteConnection(ItemsConnectionString))
				{
					connection.Open();
					using (SqliteCommand command = new SqliteCommand(connection))
					{
						// The minimum we can load
						string sql = "SELECT id,feedid,isread,link,publishDate,imageDownloaded,imageUrl FROM items ";
						sql += "ORDER BY pkid";
						command.CommandText = sql;

						using (SqliteDataReader reader = command.ExecuteReader())
						{
							while (reader.Read())
							{
								Item item = new Item();
								item.Id = reader.GetGuid(0);
								Guid feedId = reader.GetGuid(1);
								item.IsRead = reader.GetBoolean(2);
								item.Link = reader.GetString(3);
								item.PublishDate = reader.GetDateTime(4);
								item.ImageDownloaded = reader.GetBoolean(5);
								item.ImageUrl = reader.GetString(6);

								// This ensures that removed feeds are ignored.
								Feed feed = allFeeds.FirstOrDefault(f => f.Id.Equals(feedId));

								if (feed != null)
								{
									item.Feed = feed;
									list.Add(item);
								}
							}
						}
					}
				}
			}
			catch (SqliteException e)
			{
				Logger.Warn("SqliteException occured while listing items: \n{0}", e);
			}
			
			ItemCache.Current.Update(list);
			return list;
		}
		
		public IList<Item> ItemsForCategories(IList<Category> categories)
		{
			if (categories == null)
				throw new ArgumentNullException("categories", "categories is null");

			var allItems = ListItems();
			var categoryList = categories.ToList();
			var filtered = allItems.Where(i => categoryList.Any(c => c.Equals(i.Feed.Category)));

			if (filtered == null)
			{
				Logger.Warn("ItemsForCategories - the filtered list is null.");
				return new List<Item>();
			}
		
			return filtered.ToList();
		}

		internal void LazyLoadItem(Item item)
		{
			if (!item.IsLoaded)
			{
				try
				{
					using (SqliteConnection connection = new SqliteConnection(ItemsConnectionString))
					{
						connection.Open();

						// Lazy load it
						using (SqliteCommand command = new SqliteCommand(connection))
						{
							command.CommandText = "SELECT title,content,publishdate,imagefilename FROM items WHERE id=@id";

							SqliteParameter parameter = new SqliteParameter("@id", DbType.String);
							parameter.Value = item.Id.ToString();
							command.Parameters.Add(parameter);

							using (SqliteDataReader reader = command.ExecuteReader())
							{
								while (reader.Read())
								{
									item.Title = reader.GetString(0);
									item.Content = reader.GetString(1);
									item.PublishDate = reader.GetDateTime(2);
									item.ImageFilename = reader.GetString(3);
									item.IsRead = false;
								}
							}
						}
					}
				}
				catch (SqliteException e)
				{
					Logger.Warn("SqliteException occured while lazy loading item {0}: \n{1}", item.Link, e);
				}
			}
		}

		internal void UpdateItemReadStatus(Item item, bool read)
		{
			item.IsRead = read;

			try
			{
				using (SqliteConnection connection = new SqliteConnection(Repository.Default.ItemsConnectionString))
				{
					connection.Open();

					// Mark as read/unread
					using (SqliteCommand command = new SqliteCommand(connection))
					{
						command.CommandText = "UPDATE items SET isread=@isread WHERE id=@id";

						SqliteParameter parameter = new SqliteParameter("@id", DbType.String);
						parameter.Value = item.Id.ToString();
						command.Parameters.Add(parameter);

						parameter = new SqliteParameter("@isread", DbType.Int32);
						parameter.Value = Convert.ToInt32(read);
						command.Parameters.Add(parameter);

						command.ExecuteNonQuery();
					}
				}
			}
			catch (SqliteException e)
			{
				Logger.Warn("SqliteException occured while updating read status of item {0}: \n{1}", item.Link, e);
			}
		}
		
		/// <summary>
		/// Clears all items from the database where their PublishDate is before the date provided.
		/// </summary>
		/// <param name="date"></param>
		public void ClearItemsBeforeDate(DateTime date)
		{
			try
			{
				using (SqliteConnection connection = new SqliteConnection(ItemsConnectionString))
				{
					connection.Open();
					using (SqliteCommand command = new SqliteCommand(connection))
					{
						string sql = @"DELETE FROM items WHERE DATETIME(publishdate) <= DATETIME(@date)";
						command.CommandText = sql;

						SqliteParameter parameter = new SqliteParameter("@date", DbType.String);
						parameter.Value = date.ToString("yyyy-MM-dd HH:mm:ss");
						command.Parameters.Add(parameter);

						int rows = command.ExecuteNonQuery();
						Logger.Info("ClearItemsBeforeDate before {0} cleared {1} rows.", date.ToString("yyyy-MM-dd HH:mm:ss"), rows);
					}
				}
			}
			catch (SqliteException e)
			{
				Logger.Warn("SqliteException occured while clearing items before {0}: \n{1}", date, e);
			}
		}
		#endregion
	}
}
