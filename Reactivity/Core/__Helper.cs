using System;
using System.Collections.ObjectModel;
using System.Reflection;
using System.Windows;
using System.Windows.Data;
using System.Windows.Markup;
using System.Xaml;
using Reactivity.Triggers;

namespace Reactivity.Core
{
	internal class __Helper
	{
		internal static void GetBindingElementsFromBinding(BindingBase markupExtension,
			IServiceProvider serviceProvider,
			out DependencyObject targetDependencyObject,
			out DependencyProperty targetDependencyProperty)
		{
			targetDependencyObject = null;
			targetDependencyProperty = null;

			var provideValueTarget = serviceProvider.GetService(typeof(IProvideValueTarget)) as IProvideValueTarget;
			if (provideValueTarget == null)
				return;

			var targetObject = provideValueTarget.TargetObject;
			if (targetObject == null)
				return;

			var type = targetObject.GetType();
			var targetProperty = provideValueTarget.TargetProperty;
			if (targetProperty != null)
			{
				targetDependencyProperty = targetProperty as DependencyProperty;
				if (targetDependencyProperty != null)
				{
					targetDependencyObject = targetObject as DependencyObject;
				}
				else
				{
					var memberInfo = targetProperty as MemberInfo;
					if (memberInfo != null)
					{
						var propertyInfo = memberInfo as PropertyInfo;
						if (propertyInfo != null)
						{
							var schemaContextProvider =
								serviceProvider.GetService(typeof(IXamlSchemaContextProvider)) as IXamlSchemaContextProvider;
							if (schemaContextProvider != null)
								throw new Exception();
						}
						var c = !(propertyInfo != null) ? ((MethodBase)memberInfo).GetParameters()[1].ParameterType : propertyInfo.PropertyType;
						if (!typeof(MarkupExtension).IsAssignableFrom(c) || !c.IsAssignableFrom(markupExtension.GetType()))
							throw new System.Windows.Markup.XamlParseException(__SR.Get("MarkupExtensionDynamicOrBindingOnClrProp",
								markupExtension.GetType().Name, memberInfo.Name, type.Name));
					}
					else if (!typeof(BindingBase).IsAssignableFrom(markupExtension.GetType()) || !typeof(Collection<BindingBase>).IsAssignableFrom(targetProperty.GetType()))
						throw new System.Windows.Markup.XamlParseException(__SR.Get("MarkupExtensionDynamicOrBindingInCollection",
							markupExtension.GetType().Name, targetProperty.GetType().Name));
				}
			}
			else if (!typeof(BindingBase).IsAssignableFrom(markupExtension.GetType()) || !typeof(Collection<BindingBase>).IsAssignableFrom(type))
				throw new System.Windows.Markup.XamlParseException(__SR.Get("MarkupExtensionDynamicOrBindingInCollection",
					markupExtension.GetType().Name, type.Name));
		}


		internal static void CheckCanReceiveMarkupExtension(MarkupExtension markupExtension, IServiceProvider serviceProvider, out DependencyObject targetDependencyObject, out DependencyProperty targetDependencyProperty)
		{
			targetDependencyObject = null;
			targetDependencyProperty = null;

			var provideValueTarget = serviceProvider.GetService(typeof(IProvideValueTarget)) as IProvideValueTarget;
			if (provideValueTarget == null)
				return;
			var targetObject = provideValueTarget.TargetObject;
			if (targetObject == null)
				return;
			var type = targetObject.GetType();
			var targetProperty = provideValueTarget.TargetProperty;
			if (targetProperty != null)
			{
				targetDependencyProperty = targetProperty as DependencyProperty;
				if (targetDependencyProperty != null)
				{
					targetDependencyObject = targetObject as DependencyObject;
				}
				else
				{
					var memberInfo = targetProperty as MemberInfo;
					if (memberInfo != null)
					{
						var propertyInfo = memberInfo as PropertyInfo;
						EventHandler<XamlSetMarkupExtensionEventArgs> eventHandler = LookupSetMarkupExtensionHandler(type);
						if (eventHandler != null && propertyInfo != null)
						{
							var schemaContextProvider = serviceProvider.GetService(typeof(IXamlSchemaContextProvider)) as IXamlSchemaContextProvider;
							if (schemaContextProvider != null)
							{
								var xamlType = schemaContextProvider.SchemaContext.GetXamlType(type);
								if (xamlType != null)
								{
									var member = xamlType.GetMember(propertyInfo.Name);
									if (member != null)
									{
										var e = new XamlSetMarkupExtensionEventArgs(member, markupExtension, serviceProvider);
										eventHandler(targetObject, e);
										if (e.Handled)
											return;
									}
								}
							}
						}
						var c = !(propertyInfo != null) ? ((MethodBase)memberInfo).GetParameters()[1].ParameterType : propertyInfo.PropertyType;
						if (!typeof(MarkupExtension).IsAssignableFrom(c) || !c.IsAssignableFrom(markupExtension.GetType()))
							throw new System.Windows.Markup.XamlParseException(__SR.Get("MarkupExtensionDynamicOrBindingOnClrProp",
								markupExtension.GetType().Name, memberInfo.Name, type.Name));
					}
					else if (!typeof(BindingBase).IsAssignableFrom(markupExtension.GetType()) || !typeof(Collection<BindingBase>).IsAssignableFrom(targetProperty.GetType()))
						throw new System.Windows.Markup.XamlParseException(__SR.Get("MarkupExtensionDynamicOrBindingInCollection",
							markupExtension.GetType().Name, targetProperty.GetType().Name));
				}
			}
			else if (!typeof(BindingBase).IsAssignableFrom(markupExtension.GetType()) || !typeof(Collection<BindingBase>).IsAssignableFrom(type))
				throw new System.Windows.Markup.XamlParseException(__SR.Get("MarkupExtensionDynamicOrBindingInCollection",
					markupExtension.GetType().Name, type.Name));
		}

		private static EventHandler<XamlSetMarkupExtensionEventArgs> LookupSetMarkupExtensionHandler(Type type)
		{
			if (typeof(Setter).IsAssignableFrom(type))
				return Setter.ReceiveMarkupExtension;

			if (typeof(DataTrigger4).IsAssignableFrom(type))
				return DataTrigger4.ReceiveMarkupExtension;

			if (typeof(Condition).IsAssignableFrom(type))
				return Condition.ReceiveMarkupExtension;

			return null;
		}
	}
}
/*/var assembly = Assembly.Load("Assembly: System, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089");
			//var srTypeReference = assembly.GetType("System.SR");
			var assembly = Assembly.Load("Assembly: PresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35");
			var srTypeRefl = assembly.GetType("System.Windows.SR");
			var srGetMethodRefl = srTypeRefl.GetMethod("Get");
			var message = srTypeRefl.InvokeInternalStaticMethod<string>("Get", "CannotConvertType", source.GetType().FullName, typeof(PropertyPath));
			t*/
