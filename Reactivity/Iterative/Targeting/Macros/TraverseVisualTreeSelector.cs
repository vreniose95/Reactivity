using System;
using System.Windows;
using System.Windows.Markup;
using Core.Helpers;
using Reactivity.Iterative.Targeting.Core;

namespace Reactivity.Iterative.Targeting.Macros
{
	[ContentProperty("NextSelector")]
	public class TraverseVisualTreeSelector : ElementSelectorBase
	{
		public Type AncestorType { get; set; }

		public int AncestorLevel { get; set; }

		protected override object ResolveImpl(object parent, ref SelectorTreeResolutionContext context)
		{
			if (parent == null)
				throw new ArgumentNullException(nameof(parent));
			
			var fe = parent as FrameworkElement;
			if (fe == null)
				throw new NotSupportedException("Parent must be a FrameworkElement.");

			var resolvedParent = fe.FindParent(AncestorType, AncestorLevel);

			return resolvedParent;
		}
	}
}
