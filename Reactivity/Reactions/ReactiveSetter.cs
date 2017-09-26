using System;
using System.ComponentModel;
using System.ComponentModel.Design.Serialization;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Markup;
using System.Xaml;
using Core.Extensions;
using Core.Helpers;
using Core.Helpers.DependencyHelpers;
using Core.Reflection;
using Core.Reflection.Access;
using Reactivity.Core;

namespace Reactivity.Reactions
{
	/// <summary>
	/// FINAL SETTER CLASS
	/// TODO allow for setting property paths for targets (or maybe even Selectors!)
	/// </summary>
	[XamlSetMarkupExtension(nameof(ReceiveMarkupExtension))]
	[XamlSetTypeConverter(nameof(ReceiveTypeConverter))]
	[ContentProperty(nameof(Value))]
	public partial class ReactiveSetter : AttachableReactionBase, ISupportInitialize
	{
		private object _value = DependencyProperty.UnsetValue;
		private DependencyProperty _property;
		private object _unresolvedProperty;
		private object _unresolvedValue;

		private ITypeDescriptorContext _typeDescriptorContext;
		private CultureInfo _typeConverterCultureInfo;

		private bool _deferMarkupExtensionResolve;
		private object _targetObject;
		private XamlSetMarkupExtensionEventArgs _eventArgs;


		public static readonly DependencyProperty TargetObjectProperty = DP.Register(
			new Meta<ReactiveSetter, DependencyObject>(null, onTargetObjectChanged));

		[DefaultValue(null)]
		[Ambient]
		public DependencyObject TargetObject
		{
			get { return (DependencyObject)GetValue(TargetObjectProperty); }
			set { SetValue(TargetObjectProperty, value); }
		}


		public DependencyObject Target
		{
			get
			{
				var obj = AssociatedObject;
				if (TargetObject != null)
					obj = TargetObject;
				else if (IsTargetNameSet)
					obj = SourceNameResolver.Object;
				return obj;
			}
		}

		//TODO check valid dp
		[Ambient]
		[DefaultValue(null)]
		[Localizability(LocalizationCategory.None, Modifiability = Modifiability.Unmodifiable, Readability = Readability.Unreadable)]
		public DependencyProperty Property
		{
			get { return _property; }
			set
			{
				var oldProperty = _property;
				_property = value;
				OnPropertyChanged(oldProperty, _property);
			}
		}

		[DependsOn("Property")]
		[DependsOn("TargetName")]
		[Localizability(LocalizationCategory.None, Readability = Readability.Unreadable)]
		[TypeConverter(typeof(SetterTriggerConditionValueConverter))]
		public object Value
		{
			get { return _value; }
			set
			{
				if (value == DependencyProperty.UnsetValue)
					throw new ArgumentException("ReactiveSetter's Value property cannot be set Unset.");
				if (value is Expression)
					throw new ArgumentException("Expressions cannot be used with ReactiveSetter's Value property.");
				_value = value;
			}
		}

		protected bool IsTargetNameSet
		{
			get
			{
				if (string.IsNullOrEmpty(TargetName))
					return ReadLocalValue(TargetNameProperty) != DependencyProperty.UnsetValue;
				return true;
			}
		}


		private static void onTargetObjectChanged(ReactiveSetter i, DPChangedEventArgs<DependencyObject> e)
		{
			//i.resolveEffectiveTarget();
		}

		protected void OnEffectiveResolvedTargetChanged()
		{
		}

		protected void OnTargetChanged(DependencyObject oldTarget, DependencyObject newTarget)
		{
		}

		protected void OnPropertyChanged(DependencyProperty oldProperty, DependencyProperty newProperty)
		{
		}

		protected void OnValueSet(object oldValue, object newValue)
		{
		}


		protected override void ReactImpl()
		{
			if (Target != null && Property != null && Value != null)
			{
				Target.SetCurrentValue(Property, Value);
			}
			else
			{
				SourceNameResolver.ResolvedElementChanged += onResolvedElementChanged;
			}
		}

		private void onResolvedElementChanged(object s, NameResolvedEventArgs e)
		{
			SourceNameResolver.ResolvedElementChanged -= onResolvedElementChanged;
			ReactImpl();
		}

		//public static void ReceiveMarkupExtension2(object targetObject, XamlSetMarkupExtensionEventArgs eventArgs)
		//{
		//	var reactiveSetter = targetObject as ReactiveSetter;
		//	if (reactiveSetter == null)
		//		return;

		//	if (eventArgs == null)
		//		throw new ArgumentNullException(nameof(eventArgs));

		//	if (eventArgs.MarkupExtension == null)
		//		throw new ArgumentNullException(nameof(eventArgs.MarkupExtension));


		//	if (eventArgs.Member.Name == nameof(TargetName))
		//	{
		//		var xamlNameResolver = (IXamlNameResolver)eventArgs.ServiceProvider
		//			.GetService(typeof(IXamlNameResolver));

		//		var resolvedElement = xamlNameResolver.Resolve(reactiveSetter.TargetName);
		//		if (resolvedElement == null)
		//			throw new NullReferenceException(
		//				$"Cannot find name \'{reactiveSetter.TargetName}\' in the current context.");


		//		eventArgs.Handled = true;


		//	}
		//}


		public static void ReceiveMarkupExtension(object targetObject, XamlSetMarkupExtensionEventArgs eventArgs)
		{
			//ReceiveMarkupExtension2(targetObject, eventArgs);


			var reactiveSetter = targetObject as ReactiveSetter;
			if (reactiveSetter == null)
				return;

			if (eventArgs == null)
				throw new ArgumentNullException(nameof(eventArgs));

			if (eventArgs.MarkupExtension == null)
				throw new ArgumentNullException(nameof(eventArgs.MarkupExtension));

			//reactiveSetter.ServiceProvider = eventArgs.ServiceProvider;

			if (eventArgs.Member.Name == nameof(TargetObject))
			{
				// ReSharper disable once CanBeReplacedWithTryCastAndCheckForNull
				// ReSharper disable once UseNullPropagation
				if (eventArgs.MarkupExtension is Binding)
				{
					var binding = (Binding)eventArgs.MarkupExtension;
					if (binding.RelativeSource != null)
					{
						// TargetObject is being set to a Binding with a RelativeSource that 
						// relies on traversing the visual tree.
						if (binding.RelativeSource.Mode == RelativeSourceMode.FindAncestor)
						{
							if (!reactiveSetter.IsAssociated)
							{
								reactiveSetter._targetObject = targetObject;
								reactiveSetter._eventArgs = eventArgs;
								reactiveSetter._deferMarkupExtensionResolve = true;
								//TODO eventArgs.Handled = true;?
								return;
							}
							reactiveSetter._targetObject = null;
							reactiveSetter._eventArgs = null;
							reactiveSetter._deferMarkupExtensionResolve = false;

							var frameworkElement = reactiveSetter.AssociatedObject as FrameworkElement;
							if (frameworkElement == null)
								throw new NotSupportedException();

							var resolvedVisualAncestor = frameworkElement.FindParent(
								binding.RelativeSource.AncestorType,
								binding.RelativeSource.AncestorLevel);

							var adjustedBinding = new Binding
							{
								Source = resolvedVisualAncestor,
								Path = binding.Path
							};
							BindingOperations.SetBinding(reactiveSetter, TargetObjectProperty, adjustedBinding);
							eventArgs.Handled = true;
						}
					}
				}
				else if (eventArgs.MarkupExtension is StaticResourceExtension)
				{
					var staticResourceExtension = (StaticResourceExtension)eventArgs.MarkupExtension;
					var effectiveValue = staticResourceExtension.Reflect().Invoke<object>(
						ReflectionVisibility.InstanceNonPublic, 
						"ProvideValueInternal", eventArgs.ServiceProvider, true);

					var effectiveTargetObject = effectiveValue as DependencyObject;
					if (effectiveTargetObject == null)
						throw new NotSupportedException("The Provided Value from the StaticResource extension is null");

					reactiveSetter.TargetObject = effectiveTargetObject;
					eventArgs.Handled = true;
				}
				else if (eventArgs.MarkupExtension is DynamicResourceExtension)
				{
					var dynamicResourceExtension = (DynamicResourceExtension)eventArgs.MarkupExtension;
					var effectiveValue = dynamicResourceExtension.Reflect().Invoke<object>(
						ReflectionVisibility.InstanceNonPublic,
						"ProvideValueInternal", eventArgs.ServiceProvider, true);

					var effectiveTargetObject = effectiveValue as DependencyObject;
					if (effectiveTargetObject == null)
						throw new NotSupportedException("The Provided Value from the DynamicResource extension is null");

					reactiveSetter.TargetObject = effectiveTargetObject;
					eventArgs.Handled = true;
				}
				else
				{
					var effectiveValue = eventArgs.MarkupExtension.Reflect().Invoke<object>(
						ReflectionVisibility.InstanceNonPublic,
						"ProvideValueInternal", eventArgs.ServiceProvider, true);

					var effectiveTargetObject = effectiveValue as DependencyObject;
					if (effectiveTargetObject == null)
						throw new NotSupportedException($"The Provided Value from the MarkupExtension \'{eventArgs.MarkupExtension.GetType().Name}\' extension is null");

					reactiveSetter.TargetObject = effectiveTargetObject;
					eventArgs.Handled = true;
				}
			}
		}

		public static void ReceiveTypeConverter(object targetObject, XamlSetTypeConverterEventArgs eventArgs)
		{
			var setter = targetObject as ReactiveSetter;
			if (setter == null)
				throw new ArgumentNullException(nameof(targetObject));
			if (eventArgs == null)
				throw new ArgumentNullException(nameof(eventArgs));

			if (eventArgs.Member.Name == nameof(Property))
			{
				setter._unresolvedProperty = eventArgs.Value;
				setter._typeDescriptorContext = eventArgs.ServiceProvider;
				setter._typeConverterCultureInfo = eventArgs.CultureInfo;
				eventArgs.Handled = true;
			}
			//else if (eventArgs.Member.Name == nameof(TargetName))
			//{
			//	var rootObjectProvider = (IRootObjectProvider)eventArgs.ServiceProvider
			//		.GetService(typeof(IRootObjectProvider));

			//	var rootFrameworkElement = rootObjectProvider.RootObject as FrameworkElement;
			//	if (rootFrameworkElement == null)
			//		throw new Exception("cannot find root fe");

			//	var i = rootFrameworkElement.FindName(eventArgs.Value.ToString());
			//}
			else
			{
				if (eventArgs.Member.Name != nameof(Value))
					return;
				setter._unresolvedValue = eventArgs.Value;
				setter._typeDescriptorContext = eventArgs.ServiceProvider;
				setter._typeConverterCultureInfo = eventArgs.CultureInfo;
				eventArgs.Handled = true;
			}
		}


		void ISupportInitialize.BeginInit()
		{
		}

		void ISupportInitialize.EndInit()
		{
			if (_unresolvedProperty != null)
			{
				try
				{
					Property = typeof(DependencyPropertyConverter).Reflect().Invoke<DependencyProperty>(
						ReflectionVisibility.StaticNonPublic,
						"ResolveProperty", _typeDescriptorContext, TargetName, _unresolvedProperty);
				}
				finally
				{
					_unresolvedProperty = null;
				}
			}
			if (_unresolvedValue != null)
			{
				try
				{
					Value = typeof(SetterTriggerConditionValueConverter).Reflect().Invoke<object>(
						ReflectionVisibility.InstanceNonPublic,
						"ResolveValue", _typeDescriptorContext, Property, _typeConverterCultureInfo, _unresolvedValue);
				}
				finally
				{
					_unresolvedValue = null;
				}
			}
			_typeConverterCultureInfo = null;
		}
	}
	public partial class ReactiveSetter
	{
		private NameResolver SourceNameResolver { get; }
		protected bool IsSourceChangeRegistered { get; set; }
		private bool _deferTargetNameResolve;

		public static readonly DependencyProperty TargetNameProperty = DP.Register(
			new Meta<ReactiveSetter, string>(null, onTargetNameChanged));

		//TODO make this work!
		[DefaultValue(null)]
		[Ambient]
		public string TargetName
		{
			get { return (string)GetValue(TargetNameProperty); }
			set { SetValue(TargetNameProperty, value); }
		}

		public ReactiveSetter()
		{
			SourceNameResolver = new NameResolver();
		}


		protected override void OnAttached()
		{
			base.OnAttached();
			SourceNameResolver.NameScopeReferenceElement = (FrameworkElement)AssociatedObject;
			if (_deferTargetNameResolve)
				resolveEffectiveTarget();
		}

		protected override void OnDetaching()
		{
			base.OnDetaching();
			SourceNameResolver.NameScopeReferenceElement = null;
		}

		private static void onTargetNameChanged(ReactiveSetter i, DPChangedEventArgs<string> e)
		{
			i.SourceNameResolver.Name = e.NewValue;
			if (i.IsAssociated)
				i.resolveEffectiveTarget();
			else
			{
				i._deferTargetNameResolve = true;
			}
		}

		private void resolveEffectiveTarget()
		{
			if (!TargetName.IsNullOrWhiteSpace())
			{
				//if (ServiceProvider == null)
				//{
				//	_deferTargetNameCapture = true;
				//	return;
				//}
				//var target = resolveXamlName(TargetName) as DependencyObject;
				//if (target == null)
				//	throw new Exception($"\'{TargetName}\' not found.");

				//Target = SourceNameResolver.Object;
				//_deferTargetNameCapture = false;

			}
			else
			{
				//Target = TargetObject ?? AssociatedObject;
			}

			OnEffectiveResolvedTargetChanged();

			if (_deferMarkupExtensionResolve)
			{
				ReceiveMarkupExtension(_targetObject, _eventArgs);
			}
		}

	}

	//TODO delete?
	public class XamlElementNameConverter : TypeConverter
	{
		public Type TargetType => typeof(object);

		public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
		{
			if (sourceType == typeof(string) || TargetType.IsAssignableFrom(sourceType))
				return true;
			return false;
		}

		public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
		{
			if (destinationType == typeof(InstanceDescriptor) || destinationType == TargetType)
				return true;
			return false;
		}

		public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
		{
			return null;
		}
	}
}
/*	[XamlSetMarkupExtension(nameof(ReceiveMarkupExtension))]
	[XamlSetTypeConverter(nameof(ReceiveTypeConverter))]
	[ContentProperty(nameof(Value))]
	public class ReactiveSetter : AttachableReactionBase, ISupportInitialize
	{
		private object _value = DependencyProperty.UnsetValue;
		private DependencyProperty _property;
		private object _unresolvedProperty;
		private object _unresolvedValue;

		private bool _deferTargetNameCapture;

		private ITypeDescriptorContext _typeDescriptorContext;
		//protected ITypeDescriptorContext TypeDescriptorContext
		//{
		//	set
		//	{
		//		_backingFieldTypeDescriptorContext = value;
		//		onTypeDescriptorContextChanged();
		//	}
		//	get { return _backingFieldTypeDescriptorContext; }
		//}

		private IServiceProvider _backingFieldServiceProvider;
		protected IServiceProvider ServiceProvider
		{
			set
			{
				_backingFieldServiceProvider = value;
				onServiceProviderChanged();
			}
			get { return _backingFieldServiceProvider; }
		}


		private CultureInfo _typeConverterCultureInfo;

		private bool _deferMarkupExtensionResolve;
		private object _targetObject;
		private XamlSetMarkupExtensionEventArgs _eventArgs;



		private static readonly DependencyPropertyKey TargetPropertyKey = DP.RegisterReadOnly(
			new Meta<ReactiveSetter, DependencyObject>(null, onTargetChanged));
		public static readonly DependencyProperty TargetProperty = TargetPropertyKey.DependencyProperty;

		public static readonly DependencyProperty TargetObjectProperty = DP.Register(
			new Meta<ReactiveSetter, DependencyObject>(null, onTargetObjectChanged));

		public static readonly DependencyProperty TargetNameProperty = DP.Register(
			new Meta<ReactiveSetter, string>(null, onTargetNameChanged));



		public DependencyObject Target
		{
			get { return (DependencyObject)GetValue(TargetProperty); }
			protected set { SetValue(TargetPropertyKey, value); }
		}
		[DefaultValue(null)]
		[Ambient]
		public DependencyObject TargetObject
		{
			get { return (DependencyObject)GetValue(TargetObjectProperty); }
			set { SetValue(TargetObjectProperty, value); }
		}
		//TODO make this work!
		[DefaultValue(null)]
		[Ambient]
		public string TargetName
		{
			get { return (string)GetValue(TargetNameProperty); }
			set { SetValue(TargetNameProperty, value); }
		}



		//TODO check valid dp
		[Ambient]
		[DefaultValue(null)]
		[Localizability(LocalizationCategory.None, Modifiability = Modifiability.Unmodifiable, Readability = Readability.Unreadable)]
		public DependencyProperty Property
		{
			get { return _property; }
			set
			{
				var oldProperty = _property;
				_property = value;
				OnPropertyChanged(oldProperty, _property);
			}
		}

		[DependsOn("Property")]
		[DependsOn("TargetName")]
		[Localizability(LocalizationCategory.None, Readability = Readability.Unreadable)]
		[TypeConverter(typeof(SetterTriggerConditionValueConverter))]
		public object Value
		{
			get { return _value; }
			set
			{
				if (value == DependencyProperty.UnsetValue)
					throw new ArgumentException("ReactiveSetter's Value property cannot be set Unset.");
				if (value is Expression)
					throw new ArgumentException("Expressions cannot be used with ReactiveSetter's Value property.");
				_value = value;
			}
		}



		protected override void OnAttached()
		{
			base.OnAttached();
			if (_deferTargetNameCapture)
				resolveEffectiveTarget();
		}

		private static void onTargetChanged(ReactiveSetter i, DPChangedEventArgs<DependencyObject> e)
		{
			i.OnEffectiveResolvedTargetChanged();
		}
		private static void onTargetObjectChanged(ReactiveSetter i, DPChangedEventArgs<DependencyObject> e)
		{
			i.resolveEffectiveTarget();
		}

		private void resolveEffectiveTarget()
		{
			if (!TargetName.IsNullOrWhiteSpace())
			{
				if (ServiceProvider == null)
				{
					_deferTargetNameCapture = true;
					return;
				}
				var target = resolveXamlName(TargetName) as DependencyObject;
				//if (target == null)
				//	throw new Exception($"\'{TargetName}\' not found.");

				Target = target;
				_deferTargetNameCapture = false;

			}
			else
			{
				Target = TargetObject ?? AssociatedObject;
			}

			OnEffectiveResolvedTargetChanged();

			if (_deferMarkupExtensionResolve)
			{
				ReceiveMarkupExtension(_targetObject, _eventArgs);
			}
		}

		private static void onTargetNameChanged(ReactiveSetter i, DPChangedEventArgs<string> e)
		{
			i.resolveEffectiveTarget();
			//if (!e.NewValue.IsNullOrWhiteSpace())
			//{
			//	if (i.ServiceProvider == null)
			//	{
			//		i._deferTargetNameCapture = true;
			//		return;
			//	}
			//	else
			//	{

			//	}
			//}
		}

		protected void onServiceProviderChanged()
		{
			if (!_deferTargetNameCapture)
				return;
			if (ServiceProvider != null)
			{
				resolveEffectiveTarget();
				_deferTargetNameCapture = false;
			}
			else
			{
				_deferTargetNameCapture = true;
			}
			
		}

		protected object resolveXamlName(string name)
		{
			if (ServiceProvider == null)
				throw new NullReferenceException(nameof(ServiceProvider));

			var xamlNameResolverService = (IXamlNameResolver)ServiceProvider.GetService(typeof(IXamlNameResolver));
			var target = xamlNameResolverService.Resolve(TargetName);

			return target;
		}


		protected void OnEffectiveResolvedTargetChanged()
		{
		}
		protected void OnTargetChanged(DependencyObject oldTarget, DependencyObject newTarget)
		{
		}
		protected void OnPropertyChanged(DependencyProperty oldProperty, DependencyProperty newProperty)
		{
		}
		protected void OnValueSet(object oldValue, object newValue)
		{
		}


		protected override void ReactImpl()
		{
			if (Target != null && Property != null && Value != null)
			{
				//TODO should use SetCurrentValue?
				Target.SetCurrentValue(Property, Value);
			}
			else
			{

			}
		}

		public static void ReceiveMarkupExtension(object targetObject, XamlSetMarkupExtensionEventArgs eventArgs)
		{
			var reactiveSetter = targetObject as ReactiveSetter;
			if (reactiveSetter == null)
				return;

			if (eventArgs == null)
				throw new ArgumentNullException(nameof(eventArgs));

			if (eventArgs.MarkupExtension == null)
				throw new ArgumentNullException(nameof(eventArgs.MarkupExtension));

			reactiveSetter.ServiceProvider = eventArgs.ServiceProvider;

			if (eventArgs.Member.Name == nameof(TargetObject))
			{
				// ReSharper disable once CanBeReplacedWithTryCastAndCheckForNull
				// ReSharper disable once UseNullPropagation
				if (eventArgs.MarkupExtension is Binding)
				{
					var binding = (Binding)eventArgs.MarkupExtension;
					if (binding.RelativeSource != null)
					{
						// TargetObject is being set to a Binding with a RelativeSource that 
						// relies on traversing the visual tree.
						if (binding.RelativeSource.Mode == RelativeSourceMode.FindAncestor)
						{
							if (!reactiveSetter.IsAssociated)
							{
								reactiveSetter._targetObject = targetObject;
								reactiveSetter._eventArgs = eventArgs;
								reactiveSetter._deferMarkupExtensionResolve = true;
								//TODO eventArgs.Handled = true;?
								return;
							}
							reactiveSetter._targetObject = null;
							reactiveSetter._eventArgs = null;
							reactiveSetter._deferMarkupExtensionResolve = false;

							var frameworkElement = reactiveSetter.AssociatedObject as FrameworkElement;
							if (frameworkElement == null)
								throw new NotSupportedException();

							var resolvedVisualAncestor = frameworkElement.FindParent(
								binding.RelativeSource.AncestorType,
								binding.RelativeSource.AncestorLevel);

							var adjustedBinding = new Binding
							{
								Source = resolvedVisualAncestor,
								Path = binding.Path
							};
							BindingOperations.SetBinding(reactiveSetter, TargetObjectProperty, adjustedBinding);
							eventArgs.Handled = true;
						}
					}
				}
				else if (eventArgs.MarkupExtension is StaticResourceExtension)
				{
					var staticResourceExtension = (StaticResourceExtension)eventArgs.MarkupExtension;
					var effectiveValue = staticResourceExtension.InvokeInternalMethod<object>(
						"ProvideValueInternal", eventArgs.ServiceProvider, true);

					var effectiveTargetObject = effectiveValue as DependencyObject;
					if (effectiveTargetObject == null)
						throw new NotSupportedException("The Provided Value from the StaticResource extension is null");

					reactiveSetter.TargetObject = effectiveTargetObject;
					eventArgs.Handled = true;
				}
				else if (eventArgs.MarkupExtension is DynamicResourceExtension)
				{
					var dynamicResourceExtension = (DynamicResourceExtension)eventArgs.MarkupExtension;
					var effectiveValue = dynamicResourceExtension.InvokeInternalMethod<object>(
						"ProvideValueInternal", eventArgs.ServiceProvider, true);

					var effectiveTargetObject = effectiveValue as DependencyObject;
					if (effectiveTargetObject == null)
						throw new NotSupportedException("The Provided Value from the DynamicResource extension is null");

					reactiveSetter.TargetObject = effectiveTargetObject;
					eventArgs.Handled = true;
				}
				else
				{
					var effectiveValue = eventArgs.MarkupExtension.InvokeInternalMethod<object>(
						"ProvideValueInternal", eventArgs.ServiceProvider, true);

					var effectiveTargetObject = effectiveValue as DependencyObject;
					if (effectiveTargetObject == null)
						throw new NotSupportedException($"The Provided Value from the MarkupExtension \'{eventArgs.MarkupExtension.GetType().Name}\' extension is null");

					reactiveSetter.TargetObject = effectiveTargetObject;
					eventArgs.Handled = true;
				}
			}
		}

		public static void ReceiveTypeConverter(object targetObject, XamlSetTypeConverterEventArgs eventArgs)
		{
			var setter = targetObject as ReactiveSetter;
			if (setter == null)
				throw new ArgumentNullException(nameof(targetObject));
			if (eventArgs == null)
				throw new ArgumentNullException(nameof(eventArgs));

			if (eventArgs.Member.Name == nameof(Property))
			{
				setter._unresolvedProperty = eventArgs.Value;
				setter._typeDescriptorContext = eventArgs.ServiceProvider;
				setter._typeConverterCultureInfo = eventArgs.CultureInfo;
				eventArgs.Handled = true;
			}
			else
			{
				if (eventArgs.Member.Name != nameof(Value))
					return;
				setter._unresolvedValue = eventArgs.Value;
				setter._typeDescriptorContext = eventArgs.ServiceProvider;
				setter._typeConverterCultureInfo = eventArgs.CultureInfo;
				eventArgs.Handled = true;
			}
		}


		void ISupportInitialize.BeginInit()
		{
		}

		void ISupportInitialize.EndInit()
		{
			if (_unresolvedProperty != null)
			{
				try
				{
					Property = typeof(DependencyPropertyConverter).InvokeInternalStaticMethod<DependencyProperty>(
						"ResolveProperty", _typeDescriptorContext, TargetName, _unresolvedProperty);
				}
				finally
				{
					_unresolvedProperty = null;
				}
			}
			if (_unresolvedValue != null)
			{
				try
				{
					Value = typeof(SetterTriggerConditionValueConverter).InvokeInternalStaticMethod<object>(
						"ResolveValue", _typeDescriptorContext, Property, _typeConverterCultureInfo, _unresolvedValue);
				}
				finally
				{
					_unresolvedValue = null;
				}
			}
			_typeConverterCultureInfo = null;
		}
	}*/
