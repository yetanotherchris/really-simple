using System;
using System.Linq;
using MonoTouch.UIKit;
using ReallySimple.Core;
using System.Drawing;
using System.Collections.Generic;
using ReallySimple.iPhone.Core;

namespace ReallySimple.iPhone.UI.Controllers
{
	/// <summary>
	/// The home screen list of category buttons, and a view button.
	/// </summary>
	public class PickCategoriesController : ControllerBase
	{			
		private List<Category> _selectedCategories;
		private UIBarButtonItem _rightButton;
		private UIImageView _bgImage;

		private ViewFeedsController _viewFeedsController;
		private List<CategoryButton> _categoryButtons;
		
		protected override string ControllerId
		{
			get
			{
				return "PickCategories";
			}
		}
		
		public PickCategoriesController()
		{
			_selectedCategories = Settings.Current.LastCategories.ToList();
			_categoryButtons = new List<CategoryButton>();
		}
		
		/// <summary>
		/// Shows the toolbar and navigation bar, and hides the back button. 
		/// Adds the striped background image then all the button.
		/// </summary>
		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();
			
			Title = "Categories";
			
			// Re-show the toolbar here for consistency
			NavigationItem.HidesBackButton = true;
			NavigationController.SetToolbarHidden(false,true);
			NavigationController.SetNavigationBarHidden(false,false);
			
			// Bg image
			_bgImage = new UIImageView();
			_bgImage.Image = UIImage.FromFile("Assets/Images/defaultbg.png");
			_bgImage.SizeToFit();
			View.AddSubview(_bgImage);
			
			// View and category buttons
			InitializeButtons();
		}
		
		/// <summary>
		/// Adds the 'View' button and all the category buttons to the page.
		/// </summary>
		private void InitializeButtons()
		{
			// 'View' button in the top right
			_rightButton = new UIBarButtonItem();
			_rightButton.Title = "View";
			_rightButton.Clicked += ViewButtonClicked;
			NavigationItem.SetRightBarButtonItem(_rightButton,false);
			
			// All category buttons
			int x = 7, y = 10;
			int width = 150, height = 50;
			_categoryButtons.Clear();

			// This could be replaced with a static button creation?
			List<Category> selectedCategories = Settings.Current.LastCategories.ToList();
			foreach (var category in Repository.Default.ListCategories())
			{
				// The category button
				CategoryButton button = new CategoryButton(category);
				button.SetTitle(category.Title,UIControlState.Normal);
				button.SetTitleColor(UIColor.White,UIControlState.Normal);
				button.SetTitleColor(UIColor.Orange,UIControlState.Selected);

				button.SetBackgroundImage(UIImage.FromFile("Assets/Images/category-button.png"),UIControlState.Normal);
				button.Frame = new RectangleF(x,y,width,height);
				button.Font = UIFont.BoldSystemFontOfSize(16.0f);
				button.TouchDown += CategoryButtonTouchDown;
				
				// Work out which are highlighted from Settings
				if (selectedCategories.Exists(c => c.Equals(category)))
				    button.Selected = true;
				
				x += width + 5;
				if (x > 220)
				{
					x = 7;
					y += 60;
				}

				_categoryButtons.Add(button);
				View.AddSubview(button);	
			}
		}

		/// <summary>
		/// Selects or deselects the pushed category button from the list.
		/// </summary>
		private void CategoryButtonTouchDown(object sender, EventArgs e)
		{
			// Add or remove from the current list of categories
			CategoryButton button = (CategoryButton) sender;
			button.Selected = !button.Selected;
			
			if (_selectedCategories.Contains(button.Category))
				_selectedCategories.Remove(button.Category);
			else
				_selectedCategories.Add(button.Category);
			
			// Save the selected categories
			Settings.Current.LastCategories = _selectedCategories.ToList();
		}
		
		/// <summary>
		/// Pushes the LoadingFeedsController if categories are selected.
		/// </summary>
		private void ViewButtonClicked (object sender, EventArgs e)
		{
			if (_selectedCategories.Count < 1)
			{
				ModalDialog.Alert("No categories selected","Please select some categories first.");	
				return;
			}

			_viewFeedsController = new ViewFeedsController();
			NavigationController.PushViewController(_viewFeedsController, true);
		}
	}
}
