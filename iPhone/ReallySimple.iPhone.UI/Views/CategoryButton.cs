using System;
using MonoTouch.UIKit;
using ReallySimple.Core;
using System.Drawing;

namespace ReallySimple.iPhone.UI
{
	/// <summary>
	/// Represents a UIButton that holds a <see cref="Category"/>.
	/// </summary>
	public class CategoryButton : UIButton
	{
		public Category Category { get; set; }
		public UIButton Button { get;set; }
		
		public CategoryButton(Category category)
		{
			Category = category;
		}
	}
}
