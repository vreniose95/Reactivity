using System;
using System.ComponentModel;
using System.ComponentModel.Design.Serialization;
using System.Globalization;
using Reactivity.Parsers;

namespace Reactivity.Animation
{
	public class CustomTimeSpanConverter : TypeConverter
	{
		public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
		{
			if (sourceType == typeof(string))
			{
				return true;
			}
			return base.CanConvertFrom(context, sourceType);
		}

		public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
		{
			if (destinationType == typeof(InstanceDescriptor))
			{
				return true;
			}
			return base.CanConvertTo(context, destinationType);
		}

		public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
		{
			if (value is string)
			{
				return CustomTimeSpanParser.Parse((string) value);
			}
			return base.ConvertFrom(context, culture, value);
		}

		public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
		{
			if (destinationType == null)
			{
				throw new ArgumentNullException(nameof(destinationType));
			}

			if (destinationType == typeof(InstanceDescriptor) && value is TimeSpan)
			{
				var method = typeof(TimeSpan).GetMethod("Parse", new[] { typeof(string) });
				if (method != null)
				{
					return new InstanceDescriptor(method, new object[] { value.ToString() });
				}
			}

			return base.ConvertTo(context, culture, value, destinationType);
		}
	}
}
