using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Design.Serialization;
using System.Globalization;

namespace Reactivity.Animation
{
	//TODO support types other than double
	public class AnimationValueConverter : TypeConverter
	{
		private Type TargetType => typeof(AnimationValue<>);

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
				return null;

			var stringValue = value.ToString().Trim();

			RelativeAnimationOperation? operation = null;

			if (stringValue.StartsWith("+="))
				operation = RelativeAnimationOperation.Add;

			if (stringValue.StartsWith("-="))
				operation = RelativeAnimationOperation.Subtract;

			if (stringValue.StartsWith("*="))
				operation = RelativeAnimationOperation.Multiply;

			if (stringValue.StartsWith("/="))
				operation = RelativeAnimationOperation.Divide;

			if (operation.HasValue)
			{
				var rightString = stringValue.Substring(2);
				var rightValue = double.Parse(rightString);

				return new RelativeDoubleAnimationValue(operation.Value, rightValue);
			}
			var literalValue = double.Parse(stringValue);
			return new LiteralDoubleAnimationValue(literalValue);
		}
	}
}
/*		{
			if (value == null)
				return null;

			var stringValue = value.ToString().Trim().ToLower();
			
			if (!stringValue.StartsWith("expr("))
			{
				var numeric = double.Parse(stringValue);
				return new LiteralAnimationValue<double>
				{
					Value = numeric
				};
			}
			if (!stringValue.EndsWith(")"))
			{
				throw new FormatException("expr end parenthesis expected");
			}
			var expressionStringValue = stringValue.Replace("expr(", "").Trim().Substring(0, stringValue.Length - 1);
			if (expressionStringValue.IsNullOrWhiteSpace())
				throw new FormatException("AnimationValue conversion error.");

			return new Percentage(double.Parse(numericValue));*/
