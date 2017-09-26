using System;
using System.Reflection;
using System.Windows;
using System.Windows.Data;
using System.Windows.Markup;
using Core.Extensions;
using Core.Helpers;
using Core.Helpers.DependencyHelpers;
using Core.Reflection;
using Core.Reflection.Access;
using Reactivity.Core;

namespace Reactivity.Reactions
{
	[XamlSetMarkupExtension(nameof(ReceiveMarkupExtension))]
	//[XamlSetTypeConverter(nameof(ReceiveTypeConverter))]
	public class MethodCallReaction : AttachableReactionBase//, ISupportInitialize
	{
		private bool _deferMarkupExtensionResolve;
		private object _targetObject;
		private XamlSetMarkupExtensionEventArgs _eventArgs;

		#region Target
		private NameResolver _targetNameResolver { get; }
		protected bool IsTargetChangeRegistered { get; set; }

		public DependencyObject Target
		{
			get
			{
				if (TargetObject != null)
					return TargetObject;

				if (IsTargetNameSet)
					return _targetNameResolver.Object;

				return AssociatedObject;
			}
		}

		public static readonly DependencyProperty TargetObjectProperty = DP.Register(
			new Meta<MethodCallReaction, DependencyObject>(null, onTargetObjectChanged));

		[Ambient]
		public DependencyObject TargetObject
		{
			get { return (DependencyObject)GetValue(TargetObjectProperty); }
			set { SetValue(TargetObjectProperty, value); }
		}

		private static void onTargetObjectChanged(MethodCallReaction i, DPChangedEventArgs<DependencyObject> e)
		{
			i.ResolveMethodTarget();
		}


		public static readonly DependencyProperty TargetNameProperty = DP.Register(
			new Meta<MethodCallReaction, string>(null, onTargetNameChanged));

		[Ambient]
		public string TargetName
		{
			get { return (string)GetValue(TargetNameProperty); }
			set { SetValue(TargetNameProperty, value); }
		}

		private static void onTargetNameChanged(MethodCallReaction i, DPChangedEventArgs<string> e)
		{
			i._targetNameResolver.Name = e.NewValue;
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

		protected bool IsTargetResolved => Target != null;
		#endregion

		#region Method
		private bool _deferMethodResolve;

		private MethodInfo _methodInfo;


		public static readonly DependencyProperty MethodNameProperty = DP.Register(
			new Meta<MethodCallReaction, string>(null, onMethodNameChanged));

		public string MethodName
		{
			get { return (string)GetValue(MethodNameProperty); }
			set { SetValue(MethodNameProperty, value); }
		}

		private static void onMethodNameChanged(MethodCallReaction i, DPChangedEventArgs<string> e)
		{
			i.ResolveMethodTarget();

			//i._methodInfo = resolveMethodInfo(i.Target, i.MethodName);
		}

		protected bool IsMethodNameSet
		{
			get
			{
				if (string.IsNullOrEmpty(MethodName))
					return ReadLocalValue(MethodNameProperty) != DependencyProperty.UnsetValue;
				return true;
			}
		}

		protected void ResolveMethodTarget()
		{
			if (!IsTargetResolved)
			{
				_deferMethodResolve = true;
				return;
			}
			_deferMethodResolve = false;
			_methodInfo = reflectMethodInfo(Target, MethodName);
		}

		private static MethodInfo reflectMethodInfo(object target, string methodName)
		{
			var targetType = target.GetType();
			var methodInfo = targetType.GetMethod(methodName,
				BindingFlags.Public | BindingFlags.Instance);

			if (methodInfo == null)
				throw new MissingMethodException(targetType.Name, methodName);

			return methodInfo;
		}

		protected bool IsMethodTargetResolved => _methodInfo != null;
		#endregion

		public MethodCallReaction()
		{
			_targetNameResolver = new NameResolver();
		}

		protected override void OnAttached()
		{
			base.OnAttached();
			_targetNameResolver.NameScopeReferenceElement = (FrameworkElement)AssociatedObject;
			if (_deferMethodResolve)
				ResolveMethodTarget();
		}

		protected override void OnDetaching()
		{
			base.OnDetaching();
			_targetNameResolver.NameScopeReferenceElement = null;
		}


		protected override void ReactImpl()
		{
			if (!IsMethodNameSet)
				return;

			if (IsTargetResolved)
			{
				if (IsMethodTargetResolved)
				{
					_methodInfo.Invoke(Target, new object[] { });
				}
				else
				{
					//_methodInfo = resolveMethodInfo(Target, MethodName);
					ResolveMethodTarget();
					if (IsMethodTargetResolved)
					{
						_methodInfo.Invoke(Target, new object[] { });
					}
					else
					{
						throw new NotSupportedException(
							$"Could not resolve Method name \'{MethodName}\' in object \'{Target}\'");
					}
				}
			}
			else
			{
				_targetNameResolver.ResolvedElementChanged += onResolvedElementChanged;
			}
		}

		private void onResolvedElementChanged(object s, NameResolvedEventArgs e)
		{
			_targetNameResolver.ResolvedElementChanged -= onResolvedElementChanged;
			ReactImpl();
		}


		public static void ReceiveMarkupExtension(object targetObject, XamlSetMarkupExtensionEventArgs eventArgs)
		{
			var methodCallReaction = targetObject as MethodCallReaction;
			if (methodCallReaction == null)
				return;

			eventArgs.ThrowIfNull();
			eventArgs.MarkupExtension.ThrowIfNull();
			
			//reactiveSetter.ServiceProvider = eventArgs.ServiceProvider;

			if (eventArgs.Member.Name == "TargetObject")
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
							if (!methodCallReaction.IsAssociated)
							{
								//reactiveSetter.TargetObject = targetObject as DependencyObject;
								methodCallReaction._eventArgs = eventArgs;
								methodCallReaction._deferMarkupExtensionResolve = true;
								//TODO eventArgs.Handled = true;?
								return;
							}
							methodCallReaction.TargetObject = null;
							methodCallReaction._eventArgs = null;
							methodCallReaction._deferMarkupExtensionResolve = false;

							var frameworkElement = methodCallReaction.AssociatedObject as FrameworkElement;
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
							BindingOperations.SetBinding(methodCallReaction, TargetObjectProperty, adjustedBinding);
							eventArgs.Handled = true;
						}
					}
				}
				else if (eventArgs.MarkupExtension is StaticResourceExtension)
				{
					var staticResourceExtension = (StaticResourceExtension)eventArgs.MarkupExtension;
					var effectiveTargetObject = staticResourceExtension.Reflect().Invoke<DependencyObject>(
						ReflectionVisibility.InstanceNonPublic,
						"ProvideValueInternal",
						eventArgs.ServiceProvider, true);
					
					if (effectiveTargetObject == null)
						throw new NullReferenceException(
							"The provided value from the StaticResourceExtension " +
							 $"\'{eventArgs.MarkupExtension.GetType().Name}\' is null.");

					methodCallReaction.TargetObject = effectiveTargetObject;
					eventArgs.Handled = true;
				}
				else if (eventArgs.MarkupExtension is DynamicResourceExtension)
				{
					var dynamicResourceExtension = (DynamicResourceExtension)eventArgs.MarkupExtension;

					var effectiveTargetObject = dynamicResourceExtension.Reflect().Invoke<DependencyObject>(
						ReflectionVisibility.InstanceNonPublic,
						"ProvideValueInternal",
						eventArgs.ServiceProvider, true);

					if (effectiveTargetObject == null)
						throw new NullReferenceException(
							"The provided value from the DynamicResourceExtension " +
							 $"\'{eventArgs.MarkupExtension.GetType().Name}\' is null.");

					methodCallReaction.TargetObject = effectiveTargetObject;
					eventArgs.Handled = true;
				}
				else
				{
					var effectiveTargetObject = eventArgs.MarkupExtension.Reflect().Invoke<DependencyObject>(
						ReflectionVisibility.InstanceNonPublic,
						"ProvideValueInternal", eventArgs.ServiceProvider, true);
					
					if (effectiveTargetObject == null)
						throw new NotSupportedException(
							"The provided value from the MarkupExtension " +
							 $"\'{eventArgs.MarkupExtension.GetType().Name}\' is null.");

					methodCallReaction.TargetObject = effectiveTargetObject;
					eventArgs.Handled = true;
				}
			}
		}

	}
}
