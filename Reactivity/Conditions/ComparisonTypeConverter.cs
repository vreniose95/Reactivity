using System;
using System.ComponentModel;
using System.ComponentModel.Design.Serialization;
using System.Globalization;

namespace Reactivity.Conditions
{
	public class ComparisonTypeConverter : TypeConverter
	{
		public Type TargetType => typeof(ComparisonType);

		public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
		{
			if (sourceType == typeof(string) || TargetType.IsAssignableFrom(sourceType))
				return true;
			return false;
		}

		public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
		{
			if (destinationType == typeof(InstanceDescriptor) || destinationType == TargetType)
				return true;
			return false;
		}

		public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
		{
			if (value == null)
				throw new ArgumentNullException(nameof(value));

			switch (value.ToString())
			{
				case "==":
					return ComparisonType.Equals;

				case "!=":
					return ComparisonType.NotEquals;

				case "<":
					return ComparisonType.LessThan;

				case ">":
					return ComparisonType.GreaterThan;

				case "<=":
					return ComparisonType.LessThanOrEqual;

				case ">=":
					return ComparisonType.GreaterThanOrEqual;

				default:
					ComparisonType comparisonType;
					if (!Enum.TryParse(value.ToString(), out comparisonType))
					{
						throw new FormatException();
					}
					return comparisonType;
			}
		}
	}
}
