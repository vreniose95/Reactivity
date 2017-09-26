using System.Collections.Specialized;
using System.Windows;
using System.Windows.Markup;
using Core.Helpers.DependencyHelpers;
using ConditionCollection = Reactivity.Collections.ConditionCollection;

namespace Reactivity.Conditions
{
	[ContentProperty(nameof(Conditions))]
	public class And : ConditionBase
	{
		private object _cachedCurrentLeftValue;

		public static readonly DependencyProperty ConditionsProperty = DP.Register(
			new Meta<And, ConditionCollection>());
		public ConditionCollection Conditions
		{
			get { return (ConditionCollection) GetValue(ConditionsProperty); }
			set { SetValue(ConditionsProperty, value); }
		}

		public override bool Evaluate(object leftValue)
		{
			_cachedCurrentLeftValue = leftValue;
			foreach (var condition in Conditions)
			{
				if (!condition.Evaluate(leftValue))
					return false;
			}
			return true;
		}

		public And()
		{
			Conditions = new ConditionCollection();
			Conditions.CollectionChanged += onConditionsChanged;
		}

		private void onConditionsChanged(object s, NotifyCollectionChangedEventArgs e)
		{
		}
	}
}
