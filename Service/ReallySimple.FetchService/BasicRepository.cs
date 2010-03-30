using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ReallySimple.Core;
using System.Data.SqlClient;
using System.Data;
using System.Diagnostics;

namespace ReallySimple.FetchService
{
	internal class BasicRepository
	{
		/// <summary>
		/// The connection string.
		/// </summary>
		public static string ConnectionString { get; set; }

		/// <summary>
		/// Saves an item to the database, including a reference retrieve date.
		/// </summary>
		/// <param name="item"></param>
		public static void SaveItem(Item item)
		{
			try
			{
				using (SqlConnection connection = new SqlConnection(ConnectionString))
				{
					connection.Open();
					using (SqlCommand command = new SqlCommand())
					{
						string sql = @"INSERT INTO items (id,feedid,link,title,content,publishdate,retrievedate,imageFilename,imageUrl) ";
						sql += "VALUES (@id,@feedid,@link,@title,@content,@publishdate,@retrievedate,@imageFilename,@imageUrl)";
						command.CommandText = sql;
						command.Connection = connection;

						SqlParameter parameter = new SqlParameter("@id", DbType.Guid);
						parameter.Value = item.Id;
						command.Parameters.Add(parameter);

						parameter = new SqlParameter("@feedid", DbType.Guid);
						parameter.Value = item.Feed.Id;
						command.Parameters.Add(parameter);

						parameter = new SqlParameter("@link", DbType.String);
						parameter.Value = item.Link ?? "";
						command.Parameters.Add(parameter);

						parameter = new SqlParameter("@title", DbType.String);
						parameter.Value = item.Title ?? "";
						command.Parameters.Add(parameter);

						parameter = new SqlParameter("@content", DbType.String);
						parameter.Value = item.Content ?? "";
						command.Parameters.Add(parameter);

						parameter = new SqlParameter("@publishdate", DbType.DateTime);
						parameter.Value = item.PublishDate;
						command.Parameters.Add(parameter);

						parameter = new SqlParameter("@retrievedate", DbType.DateTime);
						parameter.Value = DateTime.Now;
						command.Parameters.Add(parameter);

						parameter = new SqlParameter("@imageFilename", DbType.String);
						parameter.Value = (item.ImageFilename == null) ? "" : item.ImageFilename;
						command.Parameters.Add(parameter);

						parameter = new SqlParameter("@imageUrl", DbType.String);
						parameter.Value = (item.ImageUrl == null) ? "" : item.ImageUrl;
						command.Parameters.Add(parameter);

						command.ExecuteNonQuery();
					}
				}
			}
			catch (SqlException e)
			{
				Logger.WriteLine("A SqlException occured saving '{0}' to the database:\n\n{1}", item.Link, e);
			}
		}

		/// <summary>
		/// All <see cref="Feed"/>s in the database. The Category and Site properties are null.
		/// </summary>
		/// <returns></returns>
		public static List<Feed> AllFeeds()
		{
			List<Feed> list = new List<Feed>();

			try
			{
				using (SqlConnection connection = new SqlConnection(ConnectionString))
				{
					connection.Open();
					using (SqlCommand command = new SqlCommand())
					{
						// Ignore read items if it's setup.
						string sql = "SELECT * FROM feeds ";
						command.Connection = connection;
						command.CommandText = sql;

						using (SqlDataReader reader = command.ExecuteReader())
						{
							while (reader.Read())
							{
								Feed feed = new Feed();
								feed.Id = (Guid)reader["id"];
								feed.Cleaner = (string)reader["cleaner"];
								feed.Url = (string)reader["url"];
								feed.FeedType = (FeedType) ((byte)reader["type"]);

								// Not needed
								feed.Category = null;
								feed.Site = null;

								list.Add(feed);
							}
						}
					}
				}
			}
			catch (SqlException e)
			{
				Logger.WriteLine("A SqlException occured getting AllFeeds from the database:\n\n{0}", e);
			}

			return list;
		}

		/// <summary>
		/// Clears all items that are 3 or more days old
		/// </summary>
		/// <returns></returns>
		public static List<Item> ClearOldItems()
		{
			List<Item> list = new List<Item>();

			try
			{
				using (SqlConnection connection = new SqlConnection(ConnectionString))
				{
					connection.Open();
					using (SqlCommand command = new SqlCommand())
					{
						// Ignore read items if it's setup.
						string sql = "DELETE items WHERE retrievedate > CONVERT(DateTime,'{0}',103)";
						sql = string.Format(sql, DateTime.Now.AddDays(-3).ToString("dd-MM-yyyy HH:mm:00.00"));
						command.Connection = connection;
						command.CommandText = sql;
						command.ExecuteNonQuery();
					}
				}
			}
			catch (SqlException e)
			{
				Logger.WriteLine("A SqlException occured with ClearOldItems:\n\n{0}", e);
			}

			return list;
		}

		/// <summary>
		/// Gets all items from the database where their *publishdate* is the specified hours in the past.
		/// </summary>
		/// <param name="hours"></param>
		/// <returns></returns>
		public static List<Item> ItemsForPast(int hours)
		{
			List<Item> list = new List<Item>();

			try
			{
				using (SqlConnection connection = new SqlConnection(ConnectionString))
				{
					connection.Open();
					using (SqlCommand command = new SqlCommand())
					{
						// Ignore read items if it's setup.
						string sql = "SELECT * FROM items WHERE retrievedate > CONVERT(DateTime,'{0}',103)";
						sql = string.Format(sql, DateTime.Now.AddHours(-hours).ToString("dd-MM-yyyy HH:mm:00.00"));
						command.Connection = connection;
						command.CommandText = sql;

						using (SqlDataReader reader = command.ExecuteReader())
						{
							while (reader.Read())
							{
								Item item = new Item();
								item.Id = (Guid)reader["id"];
								item.Feed = new Feed();
								item.Feed.Id = (Guid)reader["feedid"];
								item.Link = (string)reader["link"];
								item.Content = (string)reader["content"];
								item.Title = (string)reader["title"];
								item.ImageUrl = (string)reader["imageUrl"];
								item.PublishDate = (DateTime)reader["publishdate"];
								list.Add(item);
							}
						}
					}
				}
			}
			catch (SqlException e)
			{
				Logger.WriteLine("A SqlException occured getting ItemsForPast from the database:\n\n{0}", e);
			}

			return list;
		}

		/// <summary>
		/// All <see cref="Item"/>s in the database. Only the Id and Link properties are filled.
		/// </summary>
		/// <returns></returns>
		public static List<Item> AllItems()
		{
			List<Item> list = new List<Item>();

			try
			{
				using (SqlConnection connection = new SqlConnection(ConnectionString))
				{
					connection.Open();
					using (SqlCommand command = new SqlCommand())
					{
						// Ignore read items if it's setup.
						string sql = "SELECT id,feedid,link FROM items";
						command.Connection = connection;
						command.CommandText = sql;

						using (SqlDataReader reader = command.ExecuteReader())
						{
							while (reader.Read())
							{
								Item item = new Item();
								item.Id = reader.GetGuid(0);
								item.Feed = new Feed();
								item.Feed.Id = reader.GetGuid(1);

								// Used for equality checks
								item.Link = reader.GetString(2);

								list.Add(item);
							}
						}
					}
				}
			}
			catch (SqlException e)
			{
				Logger.WriteLine("A SqlException occured getting AllItems from the database:\n\n{0}", e);
			}

			return list;
		}
	}
}
