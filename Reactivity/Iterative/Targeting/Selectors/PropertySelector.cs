using System.Windows;
using System.Windows.Markup;
using Core.Helpers;
using Reactivity.Iterative.Targeting.Core;

namespace Reactivity.Iterative.Targeting.Selectors
{
	[ContentProperty("NextSelector")]
	public class PropertySelector : ElementSelectorBase
	{
		public string PropertyName { get; set; }

		protected override object ResolveImpl(object parent, ref SelectorTreeResolutionContext context)
		{
			if (parent == null)
			{
				//TODO how to handle this?
			}
			if (PropertyName == "Template" || PropertyName == "ContentTemplate")
			{
				(parent as FrameworkElement)?.ApplyTemplate();
			}
			return parent.GetPropertyValue<object>(PropertyName);
		}
	}
}
