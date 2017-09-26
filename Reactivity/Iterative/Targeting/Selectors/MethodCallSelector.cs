using System.Windows.Markup;
using Core.Helpers;
using Reactivity.Iterative.Targeting.Core;

namespace Reactivity.Iterative.Targeting.Selectors
{
	[ContentProperty("NextSelector")]
	public class MethodCallSelector : ElementSelectorBase
	{
		public string MethodName { get; set; }

		public MethodParameterList Parameters { get; set; }

		public MethodCallSelector()
		{
			Parameters = new MethodParameterList();
		}

		//TODO supporting Parameters and parameter selectors
		protected override object ResolveImpl(object parent, ref SelectorTreeResolutionContext context)
		{
			return parent.InvokeMethod<object>(MethodName);
		}
	}
}
