using System;
using System.Linq.Expressions;
using Ccr.Core.Extensions;

namespace Reactivity.Animation
{
	public class RelativeAnimatedValue<TValue>
		: AnimatedValueBase<TValue>
			where TValue : struct
	{
		private Expression<Func<TValue, TValue>> _valueExpression;
		public Expression<Func<TValue, TValue>> ValueExpression
		{
			get => _valueExpression;
		}

		public RelativeAnimatedValue(
			Expression<Func<TValue, TValue>> valueExpression)
		{
			valueExpression.IsNotNull(nameof(valueExpression));
		}

		public override TValue? GetEffectiveValue(TValue? currentValue)
		{
			throw new NotImplementedException();
		}
	}
}