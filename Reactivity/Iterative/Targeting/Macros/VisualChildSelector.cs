using System.Windows;
using System.Windows.Markup;
using System.Windows.Media;
using Reactivity.Iterative.Targeting.Core;

namespace Reactivity.Iterative.Targeting.Macros
{
	[ContentProperty("NextSelector")]
	public class VisualChildSelector : ElementSelectorBase
	{
		public int ChildIndex { get; set; } = 0;

		protected override object ResolveImpl(object parent, ref SelectorTreeResolutionContext context)
		{
			var container = parent as FrameworkElement;
			if (container == null)
				return null;
			// TODO check if already applied? is this necessary?
			container.ApplyTemplate();
			//TODO trycatch index out of bounds
			var currentChild = VisualTreeHelper.GetChild(container, ChildIndex);
			// as ContentPresenter;
			if (currentChild == null)
			{
				return null;
			}
			return currentChild;
		}
	}
}
