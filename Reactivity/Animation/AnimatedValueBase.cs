using System.ComponentModel;

namespace Reactivity.Animation
{
	[TypeConverter(typeof(AnimationValueConverter))]
	public abstract class AnimatedValueBase<TValue>
		: IAnimatedValue
			where TValue : struct
	{
		object IAnimatedValue.Value
		{
			get => Value;
		}
		object IAnimatedValue.GetEffectiveValue(
			object currentValue)
		{
			return GetEffectiveValue((TValue?)currentValue);
		}


		protected TValue? Value { get; set; }

		public abstract TValue? GetEffectiveValue(TValue? currentValue);

		public bool IsUnset
		{
			get => Value == null;
		}
	}
}
