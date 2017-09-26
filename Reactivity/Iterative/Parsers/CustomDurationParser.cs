namespace Reactivity.Iterative.Parsers
{
	public static class CustomDurationParser// : FlexParser<TimeSpan>
	{
		//protected FlexLexer lexer;
		
		public static Duration Parse(string expression)
		{
			return new Duration(CustomTimeSpanParser.Parse(expression));
			//var trimmed = expression.Trim();
			//if (trimmed.ToLower().EndsWith("ms"))
			//{
			//	var numericStr = trimmed.Substring(0, trimmed.Length - 3).Trim();
			//	var numeric = double.Parse(numericStr);

			//	return new Duration(TimeSpan.FromMilliseconds(numeric));
			//}
			//if (trimmed.ToLower().EndsWith("s"))
			//{
			//	var numericStr = trimmed.Substring(0, trimmed.Length - 2).Trim();
			//	var numeric = double.Parse(numericStr);

			//	return new Duration(TimeSpan.FromSeconds(numeric));
			//}
			//var standardTimeSpan = TimeSpan.Parse(expression);
			//return new Duration(standardTimeSpan);

		}
	}
}
