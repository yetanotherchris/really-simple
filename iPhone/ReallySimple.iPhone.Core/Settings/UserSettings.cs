using System;
using MonoTouch.Dialog;
using System.Collections.Generic;
using System.Collections;

namespace ReallySimple.iPhone.Core
{
	/// <summary>
	/// Represents all settings that can be configured by the user.
	/// </summary>
	public class UserSettings
	{
		#region Fields
		[Skip]
		private bool _ignoreReadItems;
		[Skip]
		private bool _imagesEnabled;
		[Skip]
		private SortBy _sortItemsBy;
		[Skip]
		private bool _openInSafari; 
		#endregion

		#region Properties
		[Section("General")]

		[Caption("Hide read items")]
		public bool IgnoreReadItems
		{
			get { return _ignoreReadItems; }
			set { _ignoreReadItems = value; }
		}

		[Caption("Download images")]
		public bool ImagesEnabled
		{
			get { return _imagesEnabled; }
			set { _imagesEnabled = value; }
		}

		[Caption("Sort by")]
		public SortBy SortItemsBy
		{
			get { return _sortItemsBy; }
			set { _sortItemsBy = value; }
		}
		
		[Section("Advanced")]

		
		[RadioSelection("Timeout")]
		/// <summary>
		/// Timeout in seconds for fetching items. Do not use this property - use Get/SetFetchTimeout() instead.
		/// </summary>
		public int CurrentFetchTimeout;
		/// <summary>
		/// Friendly names for Fetch timeout. Do not use this property - use Get/SetFetchTimeout() instead.
		/// </summary>
		public List<string> Timeout;

		[RadioSelection("KeepItemsFor")]
		/// <summary>
		/// The number of days to return items for. Do not use this property - use Get/SetKeepItemsFor() instead.
		/// </summary>
		public int CurrentKeepItemsFor;
		/// <summary>
		/// Friendly names for Keep Items For. Do not use this property - use Get/SetKeepItemsFor() instead.
		/// </summary>
		public List<string> KeepItemsFor;

		/*
		// TODO
		[Caption("Open links in Safari")]
		public bool OpenInSafari
		{
			get { return _openInSafari; }
			set { _openInSafari = value; }
		}*/
		#endregion

		public UserSettings()
		{
			Timeout = new List<string>()
			{
				"10 seconds","20 seconds","30 seconds"
			};
			KeepItemsFor = new List<string>()
			{
				"6 hours","12 hours","1 day"
			};

			_ignoreReadItems = true;
			_imagesEnabled = true;
			_sortItemsBy = SortBy.Date;
		}

		public int GetFetchTimeout()
		{
			switch (CurrentFetchTimeout)
			{
				case 0:
					return 10;
				case 1:
					return 20;
				case 2:
				default:
					return 30;
			}
		}

		public int GetKeepItemsFor()
		{
			switch (CurrentKeepItemsFor)
			{
				case 0:
					return 6;
				case 2:
					return 12;
				case 3:
				default:
					return 24;
			}
		}

		public void SetFetchTimeout(int value)
		{
			switch (value)
			{
				case 10:
					CurrentFetchTimeout = 0;
					break;

				case 20:
					CurrentFetchTimeout = 1;
					break;

				case 30:
				default:
					CurrentFetchTimeout = 3;
					break;
			}
		}

		public void SetKeepItemsFor(int value)
		{
			switch (value)
			{
				case 6:
					CurrentKeepItemsFor = 0;
					break;

				case 12:
					CurrentKeepItemsFor = 1;
					break;

				case 24:
				default:
					CurrentKeepItemsFor = 3;
					break;
			}
		}
	}
}
