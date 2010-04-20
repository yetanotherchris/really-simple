using System;
using System.Net;
using System.IO;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using HtmlAgilityPack;
using ReallySimple.Core;

namespace ReallySimple.FetchService
{
	public class FeedCleaner
	{
		public static FeedCleaner ResolveCleaner(Feed feed)
		{
			switch (feed.Cleaner)
			{
				case "AolNewsCleaner":
					return new AolNewsCleaner();
				case "SlashdotCleaner":
					return new SlashdotCleaner();
				default:
					return new FeedCleaner();
			}
		}

		public virtual string CleanHtml(string html)
		{
			html = RemoveBadTags(html);

			// Keep the <BR> and <P> tags
			html = html.Replace("<br/>", "``BR``");
			html = html.Replace("<BR/>", "``BR``");
			html = html.Replace("<br />", "``BR``");
			html = html.Replace("<BR />", "``BR``");
			html = html.Replace("<br>", "``BR``");
			html = html.Replace("<BR>", "``BR``");

			html = html.Replace("<p>", "``STARTP``");
			html = html.Replace("<P>", "``STARTP``");
			html = html.Replace("</p>", "``ENDP``");
			html = html.Replace("</P>", "``ENDP``");

			// Use HTML agility to remove all tags.
			HtmlDocument document = new HtmlDocument();
			document.LoadHtml(html);
			html = document.DocumentNode.InnerText;

			// Put the allowed tags back in
			html = html.Replace("``STARTP``","<p>");
			html = html.Replace("``ENDP``","</p>");
			html = html.Replace("``BR``","<br />");

			return html;
		}

		private string RemoveBadTags(string html)
		{
			html = html.Replace("&lt;", "<");
			html = html.Replace("&gt;", ">");

			HtmlDocument document = new HtmlDocument();
			document.LoadHtml(html);

			HtmlNodeCollection nodes = document.DocumentNode.SelectNodes("//script|//style|//link|//iframe|//frameset|//frame|//applet|//object");
			if (nodes != null)
			{
				foreach (HtmlNode node in nodes)
				{
					node.ParentNode.RemoveChild(node, false);
				}
			}

			// Remove hrefs to java/j/vbscript URLs
			nodes = document.DocumentNode.SelectNodes("//a[starts-with(@href, 'javascript')]|//a[starts-with(@href, 'jscript')]|//a[starts-with(@href, 'vbscript')]");
			if (nodes != null)
			{
				foreach (HtmlNode node in nodes)
				{
					node.SetAttributeValue("href", "");
				}
			}

			// Remove img with refs to java/j/vbscript URLs
			nodes = document.DocumentNode.SelectNodes("//img[starts-with(@src, 'javascript')]|//img[starts-with(@src, 'jscript')]|//img[starts-with(@src, 'vbscript')]");
			if (nodes != null)
			{
				foreach (HtmlNode node in nodes)
				{
					node.SetAttributeValue("src", "");
				}
			}

			return document.DocumentNode.InnerText;
		}
	}
}
