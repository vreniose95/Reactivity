using System;
using Core.Extensions;

namespace Reactivity.Iterative.Targeting.Core
{
	public class SelectorMacroMapping
	{
		public string MacroName { get; }

		public Type[] Parameters { get; }

		public Type MappedSelectorType { get; }

		public Func<object[], Type[], ElementSelectorBase> SelectorFunc { get; }

		public SelectorMacroMapping(string macroName, Type[] parameters, Type mappedSelectorType, Func<object[], Type[], ElementSelectorBase> selectorFunc)
		{
			if (macroName.IsNullOrWhiteSpace())
				throw new ArgumentException(
					$"\'{macroName}\' is not a valid macro name.");
			MacroName = macroName;

			Parameters = parameters;

			//if (!mappedSelectorType.IsInstanceOfType(typeof(ElementSelectorBase)))
			//	throw new ArgumentException(
			//		$"\'{mappedSelectorType.Name}\' is not a valid mapped type. " +
			//		$"It must derive from \'ElementSelectorBase\'");
			MappedSelectorType = mappedSelectorType;

			if (selectorFunc == null)
				throw new ArgumentNullException(nameof(selectorFunc));
			SelectorFunc = selectorFunc;
		}

	}
}
