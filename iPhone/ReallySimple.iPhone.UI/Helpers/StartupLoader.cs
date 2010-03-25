using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ReallySimple.Core;
using System.Diagnostics;
using System.IO;
using ReallySimple.iPhone.Core;

namespace ReallySimple.iPhone.UI
{
	/// <summary>
	/// Enables timing the various loading tasks, and threading where necessary.
	/// </summary>
	public class StartupLoader
	{
		public void LoadSettings()
		{
			Stopwatch stopwatch = new Stopwatch();
			stopwatch.Start();
			
			// Sqlite
			string itemsDbFilePath = string.Format("{0}/{1}", Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),Settings.Current.ItemsDatabaseFilename);
			string feedsDbFilePath = string.Format("{0}/Assets/Feeds.db",Environment.CurrentDirectory);

			string itemsConnection = string.Format("Data Source={0};Version=3;",itemsDbFilePath);
			string feedsConnection = string.Format("Data Source={0};Version=3;",feedsDbFilePath);

			// The order of these calls is important - the connection strings are needed first.
			SqliteRepository sqliteRepository = new SqliteRepository(itemsDbFilePath);
			Settings.Current.RepositoryInstance = sqliteRepository;
			Repository.SetInstance(Settings.Current.RepositoryInstance,feedsConnection,itemsConnection);
			
			sqliteRepository.CreateDatabase();

			// Main user settings
			Settings.Current.Read();		
			
			stopwatch.Stop();
			Logger.Info("StartupLoader.LoadSettings took {0}ms",stopwatch.ElapsedMilliseconds);
		}

		public void LoadCategories()
		{
			Stopwatch stopwatch = new Stopwatch();
			stopwatch.Start();

			Repository.Default.ListCategories();

			stopwatch.Stop();
			Logger.Info("StartupLoader.LoadCategories took {0}ms", stopwatch.ElapsedMilliseconds);
		}

		public void LoadItems()
		{
			Stopwatch stopwatch = new Stopwatch();
			stopwatch.Start();

			Repository.Default.ListItems();

			stopwatch.Stop();
			Logger.Info("StartupLoader.LoadItems took {0}ms", stopwatch.ElapsedMilliseconds);
		}

		public void LoadSites()
		{
			Stopwatch stopwatch = new Stopwatch();
			stopwatch.Start();

			Repository.Default.ListSites();

			stopwatch.Stop();
			Logger.Info("StartupLoader.LoadSites took {0}ms", stopwatch.ElapsedMilliseconds);
		}

		public void LoadHtmlTemplates()
		{
			Stopwatch stopwatch = new Stopwatch();
			stopwatch.Start();

			HtmlTemplate.Initialize();

			stopwatch.Stop();
			Logger.Info("StartupLoader.LoadHtmlTemplates took {0}ms", stopwatch.ElapsedMilliseconds);
		}
		
		public void ClearOldItems()
		{
			// Old items first
			Repository.Default.ClearItemsBeforeDate(DateTime.Today.AddDays(-Settings.Current.UserSettings.GetKeepItemsFor()));	
			
			// Yesterday ticks
			// Today ticks
			if (Directory.Exists(Settings.Current.ImageFolder))
			{
				string ticksToday = DateTime.Today.Ticks.ToString();
				string ticksYesterday = DateTime.Today.AddDays(-1).Ticks.ToString();
				foreach (string directory in Directory.GetDirectories(Settings.Current.ImageFolder))
				{
					if (!directory.EndsWith(ticksToday) && !directory.EndsWith(ticksYesterday))
					{
						Directory.Delete(directory,true);
					}
				}
			}
		}
	}
}
