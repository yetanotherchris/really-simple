using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using System.Threading;
using ReallySimple.Core;
using HtmlAgilityPack;
using System.Xml.Serialization;
using System.IO;

namespace ReallySimple.FetchService
{
	/// <summary>
	/// A simple RSS, RDF and ATOM feed parser.
	/// </summary>
	internal class FeedFetcher
	{
		private FeedCleaner _cleaner;
		private Settings _settings;
		public List<Item> AllItems { get; private set; }

		private static List<string> BlackListImages;

		static FeedFetcher()
		{
			ReadBlackList();
		}

		private static void ReadBlackList()
		{
			BlackListImages = new List<string>();

			try
			{
				XmlSerializer serializer = new XmlSerializer(typeof(List<string>));
				BlackListImages = (List<string>) serializer.Deserialize(File.Open("blacklist.xml", FileMode.OpenOrCreate));
			}
			catch (Exception)
			{
				// XmlSerializer
			}
		}

		private static bool IsBlackListed(string url)
		{
			return BlackListImages.Any(item => url.Contains(item));
		}

		public FeedFetcher(Settings settings, List<Item> allItems)
		{
			_settings = settings;
			AllItems = allItems;
		}

		/// <summary>
		/// Parses the given <see cref="FeedType"/> and returns a <see cref="IList&lt;Item&gt;"/>.
		/// </summary>
		/// <returns></returns>
		public IList<Item> Parse(Feed feed)
		{

			try
			{
				_cleaner = FeedCleaner.ResolveCleaner(feed);


				// Make sure we fill the original state object, not the 'info' variable which is a copy.
				switch (feed.FeedType)
				{
					case FeedType.RSS:
						return ParseRss(feed);
					case FeedType.RDF:
						return ParseRdf(feed);
					case FeedType.Atom:
						return ParseAtom(feed);
					default:
						throw new NotSupportedException(string.Format("{0} is not supported", feed.FeedType.ToString()));
				}
			}
			catch (Exception e)
			{
				Logger.WriteLine("An Exception occured with FeedFetcher.Parse:\n\n{0}", e);
				return new List<Item>();
			}
		}

		/// <summary>
		/// Parses an Atom feed and returns a <see cref="IList&lt;Item&gt;"/>.
		/// </summary>
		public virtual IList<Item> ParseAtom(Feed feed)
		{
			try
			{
				XDocument doc = XDocument.Load(feed.Url);

				// Feed/Entry
				var entries = from item in doc.Root.Elements().Where(i => i.Name.LocalName == "entry")
							  select item;

				List<Item> list = FillList(entries, feed, FeedType.Atom);
				ParseImages(list);

				return list;
			}
			catch
			{
				return new List<Item>();
			}
		}

		/// <summary>
		/// Parses an RSS feed and returns a <see cref="IList&lt;Item&gt;"/>.
		/// </summary>
		public virtual IList<Item> ParseRss(Feed feed)
		{
			try
			{
				XDocument doc = XDocument.Load(feed.Url);

				// RSS/Channel/item
				var entries = from item in doc.Root.Descendants().First(i => i.Name.LocalName == "channel").Elements().Where(i => i.Name.LocalName == "item")
							  select item;

				List<Item> list = FillList(entries, feed, FeedType.RSS);
				ParseImages(list);

				return list;
			}
			catch
			{
				return new List<Item>();
			}
		}

		/// <summary>
		/// Parses an RDF feed and returns a <see cref="IList&lt;Item&gt;"/>.
		/// </summary>
		public virtual IList<Item> ParseRdf(Feed feed)
		{
			try
			{
				XDocument doc = XDocument.Load(feed.Url);

				// <item> is under the root
				var entries = from item in doc.Root.Descendants().Where(i => i.Name.LocalName == "item")
							  select item;


				List<Item> list = FillList(entries, feed, FeedType.RDF);
				ParseImages(list);

				return list;
			}
			catch
			{
				return new List<Item>();
			}
		}

		private List<Item> FillList(IEnumerable<XElement> elements, Feed feed, FeedType feedType)
		{
			List<Item> list = new List<Item>();
			int count = 0;

			foreach (XElement element in elements)
			{
				Item item = new Item();

				item.Id = Guid.NewGuid();
				item.Feed = feed;

				if (feedType == FeedType.Atom)
				{
					string html = element.Elements().First(i => i.Name.LocalName == "content").Value;

					item.Content = CleanContent(html);
					item.RawHtml = html;
					item.Link = element.Elements().First(i => i.Name.LocalName == "link").Attribute("href").Value;
					item.PublishDate = ParseDate(element.Elements().First(i => i.Name.LocalName == "published").Value);
					item.Title = element.Elements().First(i => i.Name.LocalName == "title").Value.Trim();
				}
				else
				{
					string html = element.Elements().First(i => i.Name.LocalName == "description").Value;

					// <content:encoded> contains richer content
					if (element.Elements().Any(i => i.Name.LocalName == "encoded"))
						html = element.Elements().First(i => i.Name.LocalName == "encoded").Value;

					// <enclosure> tag (RSS 2). 
					if (element.Elements().Any(i => i.Name.LocalName == "enclosure"))
					{
						XElement enclosure = element.Elements().First(i => i.Name.LocalName == "enclosure");
						XAttribute enclosureUrl = enclosure.Attribute("url");
						if (enclosureUrl != null)
						{
							item.ImageUrl = NormaliseImageUrl(feed.Url, enclosureUrl.Value);
						}
					}

					item.Content = CleanContent(html);
					item.RawHtml = html;
					item.Link = element.Elements().First(i => i.Name.LocalName == "link").Value;
					item.Title = element.Elements().First(i => i.Name.LocalName == "title").Value.Trim();

					if (feedType == FeedType.RSS)
					{
						item.PublishDate = ParseDate(element.Elements().First(i => i.Name.LocalName == "pubDate").Value);
					}
					else
					{
						item.PublishDate = ParseDate(element.Elements().First(i => i.Name.LocalName == "date").Value);
					}
				}

				// Check it's not already in our list.
				if (!AllItems.Exists(i => i.GetHashCode() == item.GetHashCode()))
				{
					// Restrict the database size over time to something sane
					if (count >= _settings.MaximumItemsPerFeed)
						break;

					list.Add(item);
				}

				count++;
			}

			return list;
		}

		private string CleanContent(string html)
		{
			return _cleaner.CleanHtml(html);
		}

		private void ParseImages(List<Item> list)
		{
			foreach (var item in list)
			{
				// Only parse if the ImageUrl wasn't set previously by an <enclosure>
				if (string.IsNullOrEmpty(item.ImageUrl))
				{
					string url = GetImageUrl(item.RawHtml);
					item.ImageUrl = NormaliseImageUrl(item.Feed.Url, url);
				}
			}
		}

		private string GetImageUrl(string html)
		{
			HtmlDocument document = new HtmlDocument();
			document.LoadHtml(html);

			HtmlNodeCollection nodes = document.DocumentNode.SelectNodes("//img");
			if (nodes != null)
			{
				foreach (HtmlNode node in nodes)
				{
					string src = node.Attributes["src"].Value;
					if (!string.IsNullOrEmpty(src) && (src.StartsWith("/") || src.StartsWith("http://") || src.StartsWith("www.")))
					{
						return src;
					}
				}
			}

			return "";
		}

		private string NormaliseImageUrl(string feedUrl, string url)
		{
			if (string.IsNullOrEmpty(url))
				return "";

			if (IsBlackListed(url))
				return "";

			string lower = url.ToLower();
			if (!lower.EndsWith(".jpg") && !lower.EndsWith(".jpeg") && !lower.EndsWith(".png"))
			{
				return "";
			}

			Uri uri = null;

			try
			{
				// Check if it's relative
				uri = new Uri(url);
				if (!uri.ToString().StartsWith("http://"))
				{
					// Site.Url should never have a trailing slash.
					uri = new Uri(string.Format("{0}/{1}", feedUrl, url));
					return uri.ToString();
				}

				return url;
			}
			catch (UriFormatException e)
			{
				return "";
			}
		}

		private DateTime ParseDate(string date)
		{
			DateTime result;
			if (DateTime.TryParse(date, out result))
				return result;
			else
				return DateTime.Today;
		}
	}
}
