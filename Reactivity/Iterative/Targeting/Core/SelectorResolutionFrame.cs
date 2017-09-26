using System;

namespace Reactivity.Iterative.Targeting.Core
{
	public class SelectorResolutionFrame
	{
		public ElementSelectorBase SelectorInstance { get; }

		public Type SelectorType => SelectorInstance.GetType();

		public object ResolvedValue { get; }

		public SelectorResolutionFrame(ElementSelectorBase selectorInstance, object resolvedValue)
		{
			SelectorInstance = selectorInstance;
			ResolvedValue = resolvedValue;
		}
	}
}
