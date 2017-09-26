using System.ComponentModel;

namespace Reactivity.Conditions
{
	[TypeConverter(typeof(ComparisonTypeConverter))]
	public enum ComparisonType
	{
		Equals,
		NotEquals,
		LessThan,
		GreaterThan,
		LessThanOrEqual,
		GreaterThanOrEqual
	}
}
