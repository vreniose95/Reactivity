﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reactivity.Animation
{
	public class LiteralAnimatedValue<TValue>
		: AnimatedValueBase<TValue>
			where TValue : struct
	{
		public override TValue? GetEffectiveValue(TValue? currentValue)
		{
			throw new NotImplementedException();
		}
	}
}