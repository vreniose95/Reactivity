using System;
using System.Windows;
using System.Windows.Markup;
using Ccr.Core.Extensions;
using Core.Helpers;
using Reactivity.Iterative.Targeting.Core;

namespace Reactivity.Iterative.Targeting.Macros
{
	[ContentProperty(nameof(NextSelector))]
	public class TraverseVisualTreeSelector
		: ElementSelectorBase
	{
		public Type AncestorType { get; set; }

		public int AncestorLevel { get; set; }


		protected override object ResolveImpl(
			object parent, 
			ref SelectorTreeResolutionContext context)
		{
			parent.IsNotNull(nameof(parent));
			var frameworkElement = parent.IsOfType<FrameworkElement>();
			
			var resolvedParent = frameworkElement
				.FindParent(AncestorType, AncestorLevel);

			return resolvedParent;
		}
	}
}
