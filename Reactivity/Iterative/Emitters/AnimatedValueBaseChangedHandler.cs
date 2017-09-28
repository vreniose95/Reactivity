using Reactivity.Animation;

namespace Reactivity.Iterative.Emitters
{
	public delegate void AnimatedValueBaseChangedHandler<TValue>(
		AnimatedValueBase<TValue> newValue)
		where TValue
		: struct;
}