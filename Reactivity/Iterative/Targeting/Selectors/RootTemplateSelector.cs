using System.Windows;
using System.Windows.Markup;
using Ccr.Introspective.Extensions;
using Ccr.Introspective.Infrastructure;
using Reactivity.Iterative.Targeting.Core;

namespace Reactivity.Iterative.Targeting.Selectors
{
	[ContentProperty("NextSelector")]
	public class RootTemplateSelector
		: ElementSelectorBase
	{
		public string TemplatePropertyName { get; set; }
		 
		protected override object ResolveImpl(
			object parent,
			ref SelectorTreeResolutionContext context)
		{
			var template = context
				.RootExecutionContext
				.Reflect()
				.GetFieldValue<FrameworkTemplate>(
					MemberDescriptor.Any, 
					TemplatePropertyName);
			
			return template;
		}
	}
}
