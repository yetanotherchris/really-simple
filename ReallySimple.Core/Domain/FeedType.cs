using System;

namespace ReallySimple.Core
{
	/// <summary>
	/// Represents the XML format of a feed.
	/// </summary>
	public enum FeedType
	{
	    /// <summary>
	    /// Really Simple Syndication format.
	    /// </summary>
	    RSS,
	    /// <summary>
	    /// RDF site summary format.
	    /// </summary>
	    RDF,
	    /// <summary>
	    /// Atom Syndication format.
	    /// </summary>
	    Atom,
	}
}
