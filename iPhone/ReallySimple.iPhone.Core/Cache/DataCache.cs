using System;
using System.Linq;
using System.Collections.Generic;

namespace ReallySimple.Core
{
	/// <summary>
	/// Represents an in-memory cache of a generic ReallySimple.Core domain object. This is a singleton.
	/// </summary>
	public class DataCache<T>
	{
		private static readonly DataCache<T> _instance = new DataCache<T>();

		/// <summary>
		/// The current instance of the <see cref="DataCache`t"/>
		/// </summary>
		public static DataCache<T> Current
		{
			get
			{
				return _instance;
			}
		}

		/// <summary>
		/// All items in the cache. This list is not thread-safe.
		/// </summary>
		public virtual IList<T> Items { get; protected set; }
		
		internal DataCache()
		{
			Items = new List<T>();
		}
		
		/// <summary>
		/// Removes all cached items.
		/// </summary>
		public void Clear()
		{
			Items.Clear();
		}
		
		/// <summary>
		/// Determines if any items are in the cache.
		/// </summary>
		public bool IsCached()
		{
			return Items.Count > 0;	
		}
		
		/// <summary>
		/// Updates the cache backing list with the provided list. This isn't thread-safe.
		/// </summary>
		public void Update(IList<T> items)
		{
			Items = items;
		}		
	}
}
