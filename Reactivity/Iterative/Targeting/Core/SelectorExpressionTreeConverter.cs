using System;
using System.ComponentModel;
using System.Globalization;
using System.Windows;
using Reactivity.Core;
using Reactivity.Iterative.Parsers;
using Reactivity.Parsers;

namespace Reactivity.Iterative.Targeting.Core
{
	public sealed class SelectorExpressionTreeConverter : TypeConverter
	{
		public override bool CanConvertFrom(ITypeDescriptorContext typeDescriptorContext, Type sourceType)
		{
			return sourceType == typeof(string);
		}

		public override bool CanConvertTo(ITypeDescriptorContext typeDescriptorContext, Type destinationType)
		{
			return destinationType == typeof(string);
		}

		public override object ConvertFrom(ITypeDescriptorContext typeDescriptorContext, CultureInfo cultureInfo, object source)
		{
			if (source == null)
				throw new ArgumentNullException(nameof(source));
			if (source is string)
			{
				var parser = new SelectorExpressionParser((string)source);
				var selector = parser.Parse();
				return selector;
			}
			//return new SelectorExpressionTree((string)source, typeDescriptorContext);
			throw new ArgumentException(__SR.Get("CannotConvertType", source.GetType().FullName, typeof(SelectorExpressionTree)));
		}

		public override object ConvertTo(ITypeDescriptorContext typeDescriptorContext, CultureInfo cultureInfo, object value, Type destinationType)
		{
			if (value == null)
				throw new ArgumentNullException(nameof(value));
			if (null == destinationType)
				throw new ArgumentNullException(nameof(destinationType));
			if (destinationType != typeof(string))
				throw new ArgumentException(__SR.Get("CannotConvertType", typeof(PropertyPath), destinationType.FullName));
			var selectorExpressionTree = value as SelectorExpressionTree;
			if (selectorExpressionTree == null)
				throw new ArgumentException(__SR.Get("UnexpectedParameterType", value.GetType(), typeof(SelectorExpressionTree)), nameof(value));



			return new SelectorExpressionTree();







			//if (propertyPath.PathParameters.Count == 0)
			//	return propertyPath.Path;
			//var path = propertyPath.Path;
			//var pathParameters = propertyPath.PathParameters;
			//var serializationManager = typeDescriptorContext == null ? null : typeDescriptorContext.GetService(typeof(XamlDesignerSerializationManager)) as XamlDesignerSerializationManager;
			//ValueSerializer valueSerializer = null;
			//IValueSerializerContext context = null;
			//if (serializationManager == null)
			//{
			//	context = typeDescriptorContext as IValueSerializerContext;
			//	if (context != null)
			//		valueSerializer = ValueSerializer.GetSerializerFor(typeof(Type), context);
			//}
			//var stringBuilder = new StringBuilder();
			//var startIndex = 0;
			//for (var index1 = 0; index1 < path.Length; ++index1)
			//{
			//	if (path[index1] == 40)
			//	{
			//		var index2 = index1 + 1;
			//		while (index2 < path.Length && path[index2] != 41)
			//			++index2;
			//		int result;
			//		if (int.TryParse(path.Substring(index1 + 1, index2 - index1 - 1), NumberStyles.Integer, (IFormatProvider)TypeConverterHelper.InvariantEnglishUS.NumberFormat, out result))
			//		{
			//			stringBuilder.Append(path.Substring(startIndex, index1 - startIndex + 1));
			//			var accessor = pathParameters[result];
			//			DependencyProperty dp;
			//			PropertyInfo pi;
			//			PropertyDescriptor pd;
			//			DynamicObjectAccessor doa;
			//			PropertyPath.DowncastAccessor(accessor, out dp, out pi, out pd, out doa);
			//			Type type;
			//			string str1;
			//			if (dp != null)
			//			{
			//				type = dp.OwnerType;
			//				str1 = dp.Name;
			//			}
			//			else if (pi != null)
			//			{
			//				type = pi.DeclaringType;
			//				str1 = pi.Name;
			//			}
			//			else if (pd != null)
			//			{
			//				type = pd.ComponentType;
			//				str1 = pd.Name;
			//			}
			//			else if (doa != null)
			//			{
			//				type = doa.OwnerType;
			//				str1 = doa.PropertyName;
			//			}
			//			else
			//			{
			//				type = accessor.GetType();
			//				str1 = null;
			//			}
			//			if (valueSerializer != null)
			//			{
			//				stringBuilder.Append(valueSerializer.ConvertToString(type, context));
			//			}
			//			else
			//			{
			//				string str2 = null;
			//				if (str2 != null && str2 != string.Empty)
			//				{
			//					stringBuilder.Append(str2);
			//					stringBuilder.Append(':');
			//				}
			//				stringBuilder.Append(type.Name);
			//			}
			//			if (str1 != null)
			//			{
			//				stringBuilder.Append('.');
			//				stringBuilder.Append(str1);
			//				stringBuilder.Append(')');
			//			}
			//			else
			//			{
			//				stringBuilder.Append(')');
			//				var str2 = accessor as string;
			//				if (str2 == null)
			//				{
			//					var converter = TypeDescriptor.GetConverter(type);
			//					if (converter.CanConvertTo(typeof(string)))
			//					{
			//						if (converter.CanConvertFrom(typeof(string)))
			//						{
			//							try
			//							{
			//								str2 = converter.ConvertToString(accessor);
			//							}
			//							catch (NotSupportedException ex)
			//							{
			//							}
			//						}
			//					}
			//				}
			//				stringBuilder.Append(str2);
			//			}
			//			index1 = index2;
			//			startIndex = index2 + 1;
			//		}
			//	}
			//}
			//if (startIndex < path.Length)
			//	stringBuilder.Append(path.Substring(startIndex));
			//return stringBuilder.ToString();
		}
	}
}
