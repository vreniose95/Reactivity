using System.Windows;
using System.Windows.Markup;
using Core.Helpers;
using Reactivity.Iterative.Targeting.Core;

namespace Reactivity.Iterative.Targeting.Selectors
{
	[ContentProperty("NextSelector")]
	public class RootTemplateSelector : ElementSelectorBase
	{
		public string TemplatePropertyName { get; set; }
		 
		protected override object ResolveImpl(object parent, ref SelectorTreeResolutionContext context)
		{
			var template = context.RootExecutionContext.GetPropertyValue<FrameworkTemplate>(TemplatePropertyName);
			return template;
		}
	}
}
