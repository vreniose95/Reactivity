using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;
using Reactivity.Iterative.Targeting.Core;

namespace Reactivity.Iterative.Targeting.Macros
{
	[ContentProperty("NextSelector")]
	public class FindNameInItemsControlSelector : ElementSelectorBase
	{
		public string TargetName { get; set; }

		protected override object ResolveImpl(object parent, ref SelectorTreeResolutionContext context)
		{
			var itemsControl = context.RootExecutionContext as ItemsControl;
			if (itemsControl == null)
				throw new NotSupportedException("Can only execute FindNameInItemsControlSelector on an ItemsControl.");

			var frameworkElement = parent as FrameworkElement;
			if(frameworkElement == null)
				throw new NotSupportedException("Parent selector must be FrameworkElement.");

			frameworkElement.ApplyTemplate();
			itemsControl.ApplyTemplate();

			var targetElement = itemsControl.ItemTemplate.FindName(TargetName, frameworkElement);
			return targetElement;
		}
	}
}
