using System;
using System.Globalization;
using System.Reflection;
using Core.Helpers;

namespace Reactivity.Animation
{
	internal static class __TimeSpanParse
	{
		private const string reflTypePath = "System.Globalization.TimeSpanParse";

		private static readonly Type Proxy;

		static __TimeSpanParse()
		{
			var assembly = typeof(TimeSpan).Assembly;
			Proxy = assembly.GetType(reflTypePath);
		}

		internal static void ValidateStyles(TimeSpanStyles style, string parameterName)
		{
			Proxy.CallInternalStaticMethod("ValidateStyles", style, parameterName);
		}

		internal static TimeSpan Parse(string input, IFormatProvider formatProvider)
		{
			var value = Proxy.InvokeInternalStaticMethod<TimeSpan>("Parse", input, formatProvider);
			return value;
		}

		internal static bool TryParse(string input, IFormatProvider formatProvider, out TimeSpan result)
		{
			TimeSpan _result;
			var value = Proxy.InvokeInternalStaticMethodWithOutParameter<bool, TimeSpan>("TryParse", out _result, input, formatProvider);
			result = _result;
			return value;
		}

		internal static TimeSpan ParseExact(string input, string format, IFormatProvider formatProvider, TimeSpanStyles styles)
		{
			var value = Proxy.InvokeInternalStaticMethod<TimeSpan>("ParseExact", input, format, formatProvider, styles);
			return value;
		}

		internal static bool TryParseExact(string input, string format, IFormatProvider formatProvider, TimeSpanStyles styles, out TimeSpan result)
		{
			TimeSpan _result;
			var value = Proxy.InvokeInternalStaticMethodWithOutParameter<bool, TimeSpan>("TryParseExact", out _result, input, format, formatProvider, styles);
			result = _result;
			return value;
		}

		internal static TimeSpan ParseExactMultiple(string input, string[] formats, IFormatProvider formatProvider, TimeSpanStyles styles)
		{
			var value = Proxy.InvokeInternalStaticMethod<TimeSpan>("ParseExactMultiple", input, formats, formatProvider, styles);
			return value;
		}

		internal static bool TryParseExactMultiple(string input, string[] formats, IFormatProvider formatProvider, TimeSpanStyles styles, out TimeSpan result)
		{
			TimeSpan _result;
			var value = Proxy.InvokeInternalStaticMethodWithOutParameter<bool, TimeSpan>("TryParseExactMultiple", out _result, input, formats, formatProvider, styles);
			result = _result;
			return value;
		}
	}
}
