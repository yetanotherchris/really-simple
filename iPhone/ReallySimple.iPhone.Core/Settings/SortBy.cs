using System;

namespace ReallySimple.iPhone.Core
{
	/// <summary>
	/// Determines the sort order of the feed items.
	/// </summary>
	public enum SortBy
	{
		/// <summary>
		/// Sort the items by <see cref="Item.PublishDate"/> property.
		/// </summary>
		Date,
		/// <summary>
		/// Sort the items by the <see cref="Site.Title"/> property.
		/// </summary>
		Site
	}
}
