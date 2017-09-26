using System.Windows.Controls;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Animation;
using Reactivity.Exceptions;
using Reactivity.Iterative.Targeting.Core;

namespace Reactivity.Iterative.Targeting.Macros
{
	[ContentProperty("NextSelector")]
	public class FindNameInListViewSelector : ElementSelectorBase
	{
		public string TargetName { get; set; }

		protected override object ResolveImpl(object parent, ref SelectorTreeResolutionContext context)
		{
			var listView = context.RootExecutionContext as ListView;
			if (listView == null)
				throw new ReactionExecutionException("Can only execute FindNameInListViewSelector on an ListView.");

			listView.ApplyTemplate();

			var listViewItem = parent as ListViewItem;
			if (listViewItem == null)
				throw new ReactionExecutionException("Parent selector must be a \'ListViewItem\'.");

			listViewItem.ApplyTemplate();

			var currentChild = VisualTreeHelper.GetChild(listViewItem, 0) as ContentPresenter;
			if (currentChild == null)
				throw new ReactionExecutionException(
					"The root visual child of the ListViewItem must be of Type \'ContentPresenter\'.");

			currentChild.ApplyTemplate();

			//var targetElement = currentChild.FindName(TargetName);
			var targetElement = currentChild.ContentTemplate.FindName(TargetName, currentChild);
			if (targetElement == null)
				throw new ReactionExecutionException(
					$"The name \'{TargetName}\' could not be found in the ListViewItem's ContentPresenter.");

			var targetAnimatable = targetElement as IAnimatable;
			if (targetAnimatable == null)
				throw new ReactionExecutionException(
					$"The targeted element with name \'{TargetName}\' must be of type \'IAnimatable\'. " +
					$"Actual type \'{targetElement.GetType().Name}\' is not supported.");

			return targetAnimatable;
		}
	}
}
/*	[ContentProperty("NextSelector")]
	public class FindNameInListViewSelector : ElementSelectorBase
	{
		public string TargetName { get; set; }

		protected override object ResolveImpl(object parent, ref SelectorTreeResolutionContext context)
		{
			var listView = context.RootExecutionContext as ListView;
			if (listView == null)
				throw new ReactionExecutionException("Can only execute FindNameInListViewSelector on an ListView.");

			var frameworkElement = parent as FrameworkElement;
			if (frameworkElement == null)
				throw new ReactionExecutionException("Parent selector must be FrameworkElement.");

			frameworkElement.ApplyTemplate();
			listView.ApplyTemplate();

			var container = listView.ItemContainerGenerator.ContainerFromItem(parent);
			var listViewItem = container as ListViewItem;
			if (listViewItem == null)
				throw new ReactionExecutionException(
					$"Could not get ListViewItem from data item \'{parent.GetType().Name}\' in ListView.");

			listViewItem.ApplyTemplate();

			var currentChild = VisualTreeHelper.GetChild(listViewItem, 0) as ContentPresenter;
			if (currentChild == null)
				throw new ReactionExecutionException(
					"The root visual child of the ListViewItem must be of Type \'ContentPresenter\'.");

			var targetElement = currentChild.FindName(TargetName) as IAnimatable;
			if (targetElement == null)
				throw new ReactionExecutionException(
					$"The name \'{TargetName}\' could not be found in the ListViewItem's ContentPresenter.");

			return targetElement;
		}
	}
}
*/
