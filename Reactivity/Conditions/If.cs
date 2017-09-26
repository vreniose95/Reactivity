using System;
using System.ComponentModel;
using System.Windows;
using Core.Helpers.DependencyHelpers;

namespace Reactivity.Conditions
{
	public class If : ConditionBase
	{
		public static readonly DependencyProperty ComparisonProperty = DP.Register(
			new Meta<If, ComparisonType>());

		public static readonly DependencyProperty ValueProperty = DP.Register(
			new Meta<If, object>(null, onValueChanged));


		public ComparisonType Comparison
		{
			get { return (ComparisonType)GetValue(ComparisonProperty); }
			set { SetValue(ComparisonProperty, value); }
		}
		public object Value
		{
			get { return GetValue(ValueProperty); }
			set { SetValue(ValueProperty, value); }
		}

		private static void onValueChanged(If i, DPChangedEventArgs<object> e)
		{
		}

		public override bool Evaluate(object leftValue)
		{
			return Compare(leftValue, Comparison, Value);
		}

		private static bool Compare(object left, ComparisonType comparisonType, object right)
		{
			var targetType = left.GetType();
			var converter = TypeDescriptor.GetConverter(targetType);
			var convertedRight = converter.ConvertFrom(right);

			var leftComparable = left as IComparable;
			if (leftComparable == null)
				throw new NotSupportedException(
					$"Type \'{left.GetType().Name}\' not supported. Must be of type \'IConvertable\'");

			var compareResult = leftComparable.CompareTo(convertedRight);

			switch (comparisonType)
			{
				case ComparisonType.Equals:
					return left.Equals(convertedRight);

				case ComparisonType.NotEquals:
					return !left.Equals(convertedRight);

				case ComparisonType.GreaterThan:
					return compareResult == 1;

				case ComparisonType.GreaterThanOrEqual:
					return compareResult == 1 || compareResult == 0;

				case ComparisonType.LessThan:
					return compareResult == -1;

				case ComparisonType.LessThanOrEqual:
					return compareResult == -1 || compareResult == 0;

				default:
					throw new InvalidEnumArgumentException(nameof(compareResult));
			}
		}
	}
}
