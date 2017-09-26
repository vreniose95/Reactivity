using System;

namespace Reactivity.Iterative.Parsers
{
	public static class CustomTimeSpanParser// : FlexParser<TimeSpan>
	{
		//protected FlexLexer lexer;
		
		public static TimeSpan Parse(string expression)
		{
			var trimmed = expression.Trim();
			if (trimmed.ToLower().EndsWith("ms"))
			{
				var numericStr = trimmed.Substring(0, trimmed.Length - 3).Trim();
				var numeric = double.Parse(numericStr);

				return TimeSpan.FromMilliseconds(numeric);
			}
			if (trimmed.ToLower().EndsWith("s"))
			{
				var numericStr = trimmed.Substring(0, trimmed.Length - 2).Trim();
				var numeric = double.Parse(numericStr);

				return TimeSpan.FromSeconds(numeric);
			}
			var standardTimeSpan = TimeSpan.Parse(expression);
			return standardTimeSpan;
			//lexer = new FlexLexer(expression);

		}
	}
}
