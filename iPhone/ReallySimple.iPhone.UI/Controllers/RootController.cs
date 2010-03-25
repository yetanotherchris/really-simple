using MonoTouch.UIKit;
using System.Linq;
using MonoTouch.Dialog;
using ReallySimple.iPhone.Core;

namespace ReallySimple.iPhone.UI.Controllers
{
	/// <summary>
	/// The container navigation controller for the entire application.
	/// </summary>
	public class RootController : UINavigationController
	{
		private UIViewController _controller;
		private PickCategoriesController _pickCategories;

		/// <summary>
		/// Pushes the splash screen as the first view.
		/// </summary>
		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();
			
			Toolbar.BarStyle = UIBarStyle.Black;
			NavigationBar.BarStyle = UIBarStyle.Black;

			// Use any saved controller
			switch (Settings.Current.LastControllerId)
			{
				case "Information":
					_controller = new InformationController();
					break;
				
				case "ViewFeeds":
				{
					// Add the PickCategories to the stack
					_pickCategories = new PickCategoriesController();
				
					var viewControllers = ViewControllers.ToList();
					viewControllers.Add(_pickCategories);
					ViewControllers = viewControllers.ToArray();

					_controller = new ViewFeedsController(true);
					break;
				}

				default:
					// Default to pick categories
					_controller = new PickCategoriesController();
					break;
			}

			PushViewController(_controller, false);
		}
	}
}
