using System.Windows.Markup;
using Reactivity.Iterative.Targeting.Core;

namespace Reactivity.Iterative.Targeting.Macros
{
	[ContentProperty("NextSelector")]
	public class ThisSelector : ElementSelectorBase
	{
		protected override object ResolveImpl(object parent, ref SelectorTreeResolutionContext context)
		{
			return parent;
		}
	}
}
