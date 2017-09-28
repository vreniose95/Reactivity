using System;

namespace Reactivity.Animation
{
	public static class CustomXamlTimeSpan
	{
		public static TimeSpan Parse(string timeSpanStr)
		{
			return TimeSpan.Parse(timeSpanStr);
		}
	}
}
