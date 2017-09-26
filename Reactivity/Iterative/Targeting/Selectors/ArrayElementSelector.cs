using System;
using System.Collections;
using System.Windows.Markup;
using Reactivity.Iterative.Targeting.Core;

namespace Reactivity.Iterative.Targeting.Selectors
{
	[ContentProperty("NextSelector")]
	public class ArrayElementSelector : ElementSelectorBase
	{
		public int ArrayIndex { get; set; }

		protected override object ResolveImpl(object parent, ref SelectorTreeResolutionContext context)
		{
			var list = parent as IList;
			if (list == null)
				throw new NotSupportedException($"ArrayElementSelectors not supported on type \'{parent.GetType().Name}\'. " +
				                                $"Object must be convertable to \'IList\'.");
			var element = list[ArrayIndex];
			return element;
		}
	}

}
