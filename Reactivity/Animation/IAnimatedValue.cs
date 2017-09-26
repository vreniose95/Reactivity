namespace Reactivity.Animation
{
	public interface IAnimatedValue
	{
		object Value { get; }

		object GetEffectiveValue(object currentValue);

		bool IsUnset { get; }
	}
}
