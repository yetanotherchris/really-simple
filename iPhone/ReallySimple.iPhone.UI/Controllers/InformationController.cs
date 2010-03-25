using System;
using MonoTouch.UIKit;

namespace ReallySimple.iPhone.UI.Controllers
{
	/// <summary>
	/// The information (help) screen.
	/// </summary>
	public class InformationController : ControllerBase
	{
		protected override string ControllerId
		{
			get
			{
				return "Information";
			}
		}
		
		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();
			
			Title = "Information";
		}
	}
}
