using System;

namespace Reactivity.Animation
{
	public class LiteralAnimatedValue<TValue>
		: AnimatedValueBase<TValue>
			where TValue : struct
	{
		private TValue _value;

		public LiteralAnimatedValue(
			TValue value)
		{
			_value = value;
		}
		public override TValue? GetEffectiveValue(
			TValue? currentValue)
		{

		}
	}
}
