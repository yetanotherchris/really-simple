using System;
using System.IO;
using System.Reflection;
using ReallySimple.iPhone.Core;
using ReallySimple.Core;

namespace ReallySimple.iPhone.UI
{
	public class HtmlTemplate
	{
		public static string MainTemplate { get; private set; }
		public static string EmptyTemplate { get; private set; }
		public static string InformationTemplate { get; private set; }
		
		/// <summary>
		/// Reads all three embedded resource HTML templates from the assembly.
		/// </summary>
		public static void Initialize()
		{
			try
			{
				Stream stream = Assembly.GetExecutingAssembly().GetManifestResourceStream("ReallySimple.iPhone.UI.Assets.HTML.template.html");
				using (StreamReader reader = new StreamReader(stream))
				{
					MainTemplate = reader.ReadToEnd();
				}

				stream = Assembly.GetExecutingAssembly().GetManifestResourceStream("ReallySimple.iPhone.UI.Assets.HTML.empty-template.html");
				using (StreamReader reader = new StreamReader(stream))
				{
					EmptyTemplate = reader.ReadToEnd();
				}

				stream = Assembly.GetExecutingAssembly().GetManifestResourceStream("ReallySimple.iPhone.UI.Assets.HTML.information.html");
				using (StreamReader reader = new StreamReader(stream))
				{
					InformationTemplate = reader.ReadToEnd();

					// Fix image paths in the HTML
					string logoPath = string.Format("{0}/{1}",Environment.CurrentDirectory, "/Assets/Images/informationlogo.png");
					InformationTemplate = InformationTemplate.Replace("informationlogo.png", logoPath);
				}
			}
			catch (IOException e)
			{
				Logger.Warn("An error occured reading the HTML templates: \n{0}", e);
			}
		}
		
		/// <summary>
		/// Replaces all tokens in the template with the <see cref="Item"/>'s values.
		/// </summary>
		public static string HtmlForItem(Item item)
		{
			item.LazyLoad();
			
			string html = MainTemplate;
			html = html.Replace("#TITLE#",item.Title);
			html = html.Replace("#LINK#",item.Link);
			html = html.Replace("#DATE#",item.PublishDate.ToShortDateString());
			html = html.Replace("#SITE#",item.Feed.Site.Title);
			html = html.Replace("#CONTENT#",item.Content);

			// Replace any image that exists
			string imageHtml = "";
			if (!string.IsNullOrEmpty(item.ImageFilename) && Settings.Current.UserSettings.ImagesEnabled)
			{
				// Can't find the file? The simulator may have a new Guid
				bool imageExists = false;
				string filename = Util.GetImageFullPath(item.ImageFilename);
				if (!string.IsNullOrEmpty(filename))
					imageExists = File.Exists(filename);
				
				if (imageExists)
				{
					imageHtml = string.Format("<br /><div id=\"imagecontainer\"><img src=\"{0}\" /></div><br />", filename);	
				}
			}

			html = html.Replace("#IMAGE#", imageHtml);
			
			return html;
		}
		
		/// <summary>
		/// Replaces the empty template's #CONTENT# token and returns the HTML.
		/// </summary>
		public static string EmptyHtml()
		{
			return EmptyTemplate;
		}
	}
}
