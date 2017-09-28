using System.Collections.Generic;
using System.Windows.Markup;
using Ccr.Introspective.Extensions;
using Ccr.Introspective.Infrastructure;
using Reactivity.Iterative.Targeting.Core;

namespace Reactivity.Iterative.Targeting.Selectors
{
	[ContentProperty(nameof(NextSelector))]
	public class MethodCallSelector
		: ElementSelectorBase
	{
		public string MethodName { get; set; }

		public MethodParameterList Parameters { get; set; }

		public MethodCallSelector()
		{
			Parameters = new MethodParameterList();
		}

		//TODO supporting Parameters and parameter selectors
		protected override object ResolveImpl(
			object parent, 
			ref SelectorTreeResolutionContext context)
		{
			return parent
				.Reflect()
				.InvokeMethod<object>(
					MemberDescriptor.Any,
					MethodName);
		}
	}
}
