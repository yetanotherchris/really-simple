using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MonoTouch.UIKit;
using MonoTouch.Foundation;

namespace ReallySimple.iPhone.UI.Views
{
	/// <summary>
	/// Handles all links clicked in the webview, and directs them to Safari.
	/// </summary>
	public class DefaultWebDelegate : UIWebViewDelegate
	{
		public override bool ShouldStartLoad(UIWebView webView, NSUrlRequest request, UIWebViewNavigationType navigationType)
		{
			// TODO: add Settings.Current.UserSettings.OpenInSafari 

			if (navigationType == UIWebViewNavigationType.LinkClicked)
			{
				UIApplication.SharedApplication.OpenUrl(request.Url);
				return false;
			}

			return true;
		}

		public override void LoadingFinished(UIWebView webView)
		{
			UIView.BeginAnimations("webview");
			webView.Alpha = 1;
			UIView.CommitAnimations();
		}
	}
}
