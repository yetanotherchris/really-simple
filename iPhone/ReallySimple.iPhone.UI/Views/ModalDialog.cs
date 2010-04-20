using System;
using MonoTouch.UIKit;

namespace ReallySimple.iPhone.UI
{
	/// <summary>
	/// Represents common modal dialogs.
	/// </summary>
	public class ModalDialog
	{
		/// <summary>
		/// A modal dialog with a title and description, and single "Close" button.
		/// </summary>
		public static void Alert(string title,string message)
		{
		    UIAlertView alert = new UIAlertView();
		    alert.Title = title;
		    alert.AddButton("Close");
		    alert.Message = message;
		    alert.Show();
		}
	}
}
