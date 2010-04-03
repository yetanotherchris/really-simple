using System;
using MonoTouch.UIKit;
using ReallySimple.iPhone.UI.Views;
using MonoTouch.Foundation;

namespace ReallySimple.iPhone.UI.Controllers
{
	/// <summary>
	/// The information (help) screen.
	/// </summary>
	public class InformationController : ControllerBase
	{
		private UIWebView _webView;

		protected override string ControllerId
		{
			get
			{
				return "Information";
			}
		}

		public override void ViewDidLoad ()
		{
			Title = "Information";
			base.ViewDidLoad ();

			_webView = new UIWebView();
			_webView.Alpha = 0; // no white flashing
			_webView.Delegate = new DefaultWebDelegate();
			_webView.Frame = new System.Drawing.RectangleF(0, 0, 320, 480);
			_webView.LoadHtmlString(HtmlTemplate.InformationTemplate, new NSUrl("/"));

			View.AddSubview(_webView);
			View.BringSubviewToFront(_webView);
		}
	}
}
