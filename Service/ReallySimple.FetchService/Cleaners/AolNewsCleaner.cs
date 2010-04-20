using System;

namespace ReallySimple.FetchService
{
	public class AolNewsCleaner : FeedCleaner
	{
		public override string CleanHtml(string html)
		{
			html = base.CleanHtml(html);
			
			html = html.Replace("Permalink","");
			html = html.Replace("|","");
			html = html.Replace("Email this","");
			html = html.Replace("Comments","");
			
			return html;
		}

	}
}
