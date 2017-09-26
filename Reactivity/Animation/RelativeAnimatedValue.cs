using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

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
			valueExpression.
		}

		public override TValue? GetEffectiveValue(TValue? currentValue)
		{
			throw new NotImplementedException();
		}
	}
}