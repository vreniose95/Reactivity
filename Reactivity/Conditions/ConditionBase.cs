using Reactivity.Core;

namespace Reactivity.Conditions
{
	public abstract class ConditionBase : AttachableBase
	{
		public abstract bool Evaluate(object leftValue);
	}
}
