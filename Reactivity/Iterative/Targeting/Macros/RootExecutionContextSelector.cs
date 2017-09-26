using System.Windows.Markup;
using Reactivity.Iterative.Targeting.Core;

namespace Reactivity.Iterative.Targeting.Macros
{
	[ContentProperty("NextSelector")]
	public class RootExecutionContextSelector : ElementSelectorBase
	{
		protected override object ResolveImpl(object parent, ref SelectorTreeResolutionContext context)
		{
			return context.RootExecutionContext;
		}
	}
}
