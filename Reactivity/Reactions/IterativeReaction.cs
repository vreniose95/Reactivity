using System;
using System.Collections;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Markup;
using Core.Helpers;
using Core.Helpers.DependencyHelpers;
using Reactivity.Animation;
using Reactivity.Core;
using Reactivity.Iterative.Emitters;

namespace Reactivity.Reactions
{
	//TODO should all reactions and setter use SetCurrentValue instead of SetValue? CHANGE PROPERTIES SET THROUGH REFLECTION TO METHOD CALLS!
	[XamlSetMarkupExtension("ReceiveMarkupExtension")]
	[XamlSetTypeConverter("ReceiveTypeConverter")]
	[ContentProperty("AnimationTemplate")]
	public class IterativeReaction : AttachableReactionBase
	{
		public const string _defaultIterationTargetPropertyName = "Items";

		private string _iterationTargetPropertyName = _defaultIterationTargetPropertyName;
		private PropertyInfo _propertyInfo;
		private IEnumerable _resolvedIterationTarget;
		private bool _deferIterationTargetReflect;

		public string IterationTargetPropertyName
		{
			get { return _iterationTargetPropertyName; }
			set
			{
				if (_deferIterationTargetReflect)
				{
					AttemptResolveIterationTargetPropertyInfo();
				}

				var oldProperty = _iterationTargetPropertyName;
				_iterationTargetPropertyName = value;
				OnIterationTargetPropertyNameChanged(oldProperty, _iterationTargetPropertyName);
			}
		}

		public IEnumerable ResolvedIterationTarget
		{
			get { return _resolvedIterationTarget; }
			protected set
			{
				_resolvedIterationTarget = value;
				OnResolvedIterationTarget();
			}
		}


		public static readonly DependencyProperty AnimationTemplateProperty = DP.Register(
			new Meta<IterativeReaction, AnimationTemplate>(null, onAnimationTemplateChanged));

		public AnimationTemplate AnimationTemplate
		{
			get { return (AnimationTemplate)GetValue(AnimationTemplateProperty); }
			set { SetValue(AnimationTemplateProperty, value); }
		}

		protected void OnResolvedIterationTarget()
		{

		}


		protected override void OnAttached()
		{
			base.OnAttached();
			attachAssociatedChildElement();
			AttemptResolveIterationTargetPropertyInfo();
		}
		protected override void OnDetaching()
		{
			base.OnDetaching();
			if (AnimationTemplate != null)
			{
				AnimationTemplate.Detach();
			}
		}


		private static void onAnimationTemplateChanged(IterativeReaction i, DPChangedEventArgs<AnimationTemplate> e)
		{
			if (e.OldValue != null)
			{
				e.OldValue.Detach();
			}
			if (e.NewValue != null)
			{
				i.attachAssociatedChildElement();
			}
		}

		protected void attachAssociatedChildElement()
		{
			//TODO was IsAssociateD! change others??
			if (AnimationTemplate != null && !IsAssociated)
			{
				AnimationTemplate.Attach(AssociatedObject);
			}
		}

		protected void OnIterationTargetPropertyNameChanged(string oldName, string newName)
		{
			AttemptResolveIterationTargetPropertyInfo();
		}

		protected void AttemptResolveIterationTargetPropertyInfo()
		{
			_deferIterationTargetReflect = false;
			if (AssociatedObject == null || IterationTargetPropertyName == null || AnimationTemplate == null)
			{
				_deferIterationTargetReflect = true;
				return;
			}
			var associatedObjectType = AssociatedObject.GetType();
			var propertyInfo = associatedObjectType.GetProperty(IterationTargetPropertyName, BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);

			if (propertyInfo == null)
			{
				throw new MissingMemberException(associatedObjectType.Name, IterationTargetPropertyName);
			}
			_propertyInfo = propertyInfo;
		}

		protected void AttemptResolveIterationTarget()
		{
			var reflectedValue = _propertyInfo.GetValue(AssociatedObject);

			var enumerableTarget = reflectedValue as IEnumerable;
			if (enumerableTarget == null)
				throw new NotSupportedException($"Target reflected value must be IEnumerable. Type \'{reflectedValue.GetType()}\' is not valid.");

			ResolvedIterationTarget = enumerableTarget;
		}

		protected override void ReactImpl()
		{
			if (_propertyInfo == null)
				AttemptResolveIterationTargetPropertyInfo();

			if (ResolvedIterationTarget == null)
				AttemptResolveIterationTarget();

			if (ResolvedIterationTarget == null)
				throw new NotSupportedException("Target Enumerable could not be resolved.");

			if (AnimationTemplate.Emitters == null || AnimationTemplate == null)
				return;

			if (!AnimationTemplate.Emitters.Any())
				return;

			//if (AnimationTemplate.Emitter.Property == null)
			//	throw new NullReferenceException("Emitter Property is null");


			var itemIndex = 0;
			var _ResolvedListTarget = (IList)ResolvedIterationTarget;
			var total = _ResolvedListTarget.Count;

			foreach (var item in ResolvedIterationTarget)
			{
				//try
				//{

				//TODO use selectors for this to use on other types instead of itemscontrol
				var associatedItemsControl = AssociatedObject as ItemsControl;
				if (associatedItemsControl == null)
					throw new NotSupportedException(
						$"IterativeReactionBase Associated Item \'{AssociatedObject.GetType().Name}\' must be of type \'ItemsControl\'");

				var container = associatedItemsControl.ItemContainerGenerator.ContainerFromItem(item);

				var targetAnimatable = AnimationTemplate.SelectorExpression.ResolveSelectorTree(container, associatedItemsControl);

				foreach (var emitter in AnimationTemplate.Emitters)
				{
					if (emitter.Property == null)
						throw new NullReferenceException("Emitter Property is null");

					var hasSubExpression = emitter.SubSelectorExpression != null;
					var subTargetAnimatable = targetAnimatable;

					if (hasSubExpression)
					{
						subTargetAnimatable = emitter.SubSelectorExpression.ResolveSelectorTree((DependencyObject)targetAnimatable, associatedItemsControl);
					}
					var targetFreezable = subTargetAnimatable as Freezable;
					if (targetFreezable != null)
					{
						if (targetFreezable.IsSealed || targetFreezable.IsFrozen)
						{
							//var cloned = targetFreezable.Clone();
							var cloned = targetFreezable.CloneCurrentValue();
							object newValueReference = cloned;

							if (hasSubExpression)
							{
								emitter.SubSelectorExpression.UpdateTargetedParentReference((DependencyObject)targetAnimatable, associatedItemsControl,
									 ref newValueReference);

								subTargetAnimatable = emitter.SubSelectorExpression.ResolveSelectorTree((DependencyObject)targetAnimatable, associatedItemsControl);

							}
							else
							{
								AnimationTemplate.SelectorExpression.UpdateTargetedParentReference(
									container, associatedItemsControl, ref newValueReference);


								//TODO remove this and find a way to have UpdateTargetedParentReference provide the actual reference to the updated value instance
								subTargetAnimatable = AnimationTemplate.SelectorExpression.ResolveSelectorTree(
									container, associatedItemsControl);
							}

						}
					}


					var targetDependendencyObject = subTargetAnimatable as DependencyObject;
					if (targetDependendencyObject == null)
						throw new NotSupportedException(
							$"Element targeted by the AnimationSelector type \'{subTargetAnimatable.GetType().Name}\' must be \'DependencyObject\'");

					var currentValue = targetDependendencyObject.GetValue(emitter.Property); // as double?;

					//var currentValue2 = targetDependendencyObject.ReadLocalValue(AnimationTemplate.Emitter.Property) as double?;

					var animationEmitterBaseCore = (IAnimationEmitterBaseCore)emitter;

					if (currentValue == null)
					{
						throw new NotSupportedException("Current value must not null and of type \'double\'");
					}
					if (emitter.ApplyFromBeforeOffset)
					{
						if (animationEmitterBaseCore.From.IsUnset)
						{
							animationEmitterBaseCore.FromPropertyChanged += (@newAnimationValue) =>
																															{
																																var effectiveCurrentValue =
																																	animationEmitterBaseCore.From.GetEffectiveValue(currentValue);
																																if (effectiveCurrentValue == null)
																																	return;

																																targetDependendencyObject.SetCurrentValue(
																																	emitter.Property, effectiveCurrentValue);
																															};
						}
						else
						{
							var effectiveCurrentValue = animationEmitterBaseCore.From.GetEffectiveValue(currentValue);
							if (effectiveCurrentValue == null)
								break;

							targetDependendencyObject.SetCurrentValue(emitter.Property, effectiveCurrentValue);
							//targetDependendencyObject.SetCurrentValue(AnimationTemplate.Emitter.Property, AnimationTemplate.Emitter.From);
						}
					}

					var animationTimeline = animationEmitterBaseCore.Emit(itemIndex, total, currentValue);

					subTargetAnimatable.BeginAnimation(emitter.Property, animationTimeline);
				}
				//}
				//catch (ReactionExecutionException ex)
				//{

				//}
				itemIndex++;
			}
		}

		public static void ReceiveMarkupExtension(object targetObject, XamlSetMarkupExtensionEventArgs eventArgs)
		{
			if (targetObject == null)
				throw new ArgumentNullException(nameof(targetObject));
			if (eventArgs == null)
				throw new ArgumentNullException(nameof(eventArgs));
			var setter = targetObject as IterativeReaction;
			if (setter == null || eventArgs.Member.Name != nameof(IterationTargetPropertyName))
				return;
			var markupExtension = eventArgs.MarkupExtension;
			if (markupExtension is StaticResourceExtension)
			{
				var resourceExtension = markupExtension as StaticResourceExtension;
				setter.IterationTargetPropertyName = resourceExtension.InvokeInternalMethod<string>(
					"ProvideValueInternal", eventArgs.ServiceProvider, true);
				eventArgs.Handled = true;
			}
			else
			{
				if (!(markupExtension is DynamicResourceExtension) && !(markupExtension is BindingBase))
					return;
				throw new NotImplementedException();
				//setter.IterationTargetPropertyName = markupExtension;
				//eventArgs.Handled = true;
			}
		}

		public static void ReceiveTypeConverter(object targetObject, XamlSetTypeConverterEventArgs eventArgs)
		{
			var setter = targetObject as IterativeReaction;
			if (setter == null)
				throw new ArgumentNullException(nameof(targetObject));
			if (eventArgs == null)
				throw new ArgumentNullException(nameof(eventArgs));
			if (eventArgs.Member.Name == nameof(IterationTargetPropertyName))
			{
				//setter._unresolvedProperty = eventArgs.Value;
				//setter._serviceProvider = eventArgs.ServiceProvider;
				//setter._typeConverterCultureInfo = eventArgs.CultureInfo;
				eventArgs.Handled = true;
			}
			else
			{
				throw new NotImplementedException();
				//if (eventArgs.Member.Name != nameof(Value))
				//	return;
				//setter._unresolvedValue = eventArgs.Value;
				//setter._serviceProvider = eventArgs.ServiceProvider;
				//setter._typeConverterCultureInfo = eventArgs.CultureInfo;
				//eventArgs.Handled = true;
			}
		}

	}

	//TODO should all reactions and setter use SetCurrentValue instead of SetValue? CHANGE PROPERTIES SET THROUGH REFLECTION TO METHOD CALLS!
	//[XamlSetMarkupExtension("ReceiveMarkupExtension")]
	//[XamlSetTypeConverter("ReceiveTypeConverter")]
	//[ContentProperty("AnimationTemplate")]
	//public class FlexibleIterativeReaction : AttachableReactionBase
	//{
	//	public const string _defaultIterationTargetPropertyName = "Items";

	//	private string _iterationTargetPropertyName = _defaultIterationTargetPropertyName;
	//	private PropertyInfo _propertyInfo;
	//	private IEnumerable _resolvedIterationTarget;
	//	private bool _deferIterationTargetReflect;

	//	public string IterationTargetPropertyName
	//	{
	//		get { return _iterationTargetPropertyName; }
	//		set
	//		{
	//			if (_deferIterationTargetReflect)
	//			{
	//				AttemptResolveIterationTargetPropertyInfo();
	//			}

	//			var oldProperty = _iterationTargetPropertyName;
	//			_iterationTargetPropertyName = value;
	//			OnIterationTargetPropertyNameChanged(oldProperty, _iterationTargetPropertyName);
	//		}
	//	}

	//	public IEnumerable ResolvedIterationTarget
	//	{
	//		get { return _resolvedIterationTarget; }
	//		protected set
	//		{
	//			_resolvedIterationTarget = value;
	//			OnResolvedIterationTarget();
	//		}
	//	}

	//	public static readonly DependencyProperty AnimationTemplateProperty = DP.Register(
	//		new Meta<FlexibleIterativeReaction, AnimationTemplate>(null, onAnimationTemplateChanged));

	//	public AnimationTemplate AnimationTemplate
	//	{
	//		get { return (AnimationTemplate)GetValue(AnimationTemplateProperty); }
	//		set { SetValue(AnimationTemplateProperty, value); }
	//	}

	//	protected void OnResolvedIterationTarget()
	//	{

	//	}

	//	protected override void OnAttached()
	//	{
	//		base.OnAttached();
	//		AttemptResolveIterationTargetPropertyInfo();
	//	}

	//	private static void onAnimationTemplateChanged(FlexibleIterativeReaction i, DPChangedEventArgs<AnimationTemplate> e)
	//	{
	//	}

	//	protected void OnIterationTargetPropertyNameChanged(string oldName, string newName)
	//	{
	//		AttemptResolveIterationTargetPropertyInfo();
	//	}

	//	protected void AttemptResolveIterationTargetPropertyInfo()
	//	{
	//		_deferIterationTargetReflect = false;
	//		if (AssociatedObject == null || IterationTargetPropertyName == null || AnimationTemplate == null)
	//		{
	//			_deferIterationTargetReflect = true;
	//			return;
	//		}
	//		var associatedObjectType = AssociatedObject.GetType();
	//		var propertyInfo = associatedObjectType.GetProperty(IterationTargetPropertyName, BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);

	//		if (propertyInfo == null)
	//		{
	//			throw new MissingMemberException(associatedObjectType.Name, IterationTargetPropertyName);
	//		}
	//		_propertyInfo = propertyInfo;
	//	}

	//	protected void AttemptResolveIterationTarget()
	//	{
	//		var reflectedValue = _propertyInfo.GetValue(AssociatedObject);

	//		var enumerableTarget = reflectedValue as IEnumerable;
	//		if (enumerableTarget == null)
	//			throw new NotSupportedException($"Target reflected value must be IEnumerable. Type \'{reflectedValue.GetType()}\' is not valid.");

	//		ResolvedIterationTarget = enumerableTarget;
	//	}

	//	protected override void ReactImpl()
	//	{
	//		if (_propertyInfo == null)
	//			AttemptResolveIterationTargetPropertyInfo();

	//		if (ResolvedIterationTarget == null)
	//			AttemptResolveIterationTarget();

	//		if (ResolvedIterationTarget == null)
	//			throw new NotSupportedException("Target Enumerable could not be resolved.");

	//		if (AnimationTemplate.Emitter == null || AnimationTemplate == null)
	//			return;

	//		if (AnimationTemplate.Emitter.Property == null)
	//			throw new NullReferenceException("Emitter Property is null");


	//		var itemIndex = 0;
	//		var _ResolvedListTarget = (IList)ResolvedIterationTarget;
	//		var total = _ResolvedListTarget.Count;

	//		foreach (var item in ResolvedIterationTarget)
	//		{
	//			var dependencyObject = item as DependencyObject;
	//			if (dependencyObject == null)
	//				throw new NotSupportedException(
	//					$"Type \'{item.GetType()}\' not supported. Item must be of type \'DependencyObject\'");

	//			var targetAnimatable = AnimationTemplate.SelectorExpression.ResolveSelectorTree(dependencyObject, AssociatedObject);


	//			var targetFreezable = targetAnimatable as Freezable;
	//			if (targetFreezable != null)
	//			{
	//				if (targetFreezable.IsSealed || targetFreezable.IsFrozen)
	//				{
	//					//var cloned = targetFreezable.Clone();
	//					var cloned = targetFreezable.CloneCurrentValue();
	//					object newValueReference = cloned;

	//					AnimationTemplate.SelectorExpression.UpdateTargetedParentReference(
	//						dependencyObject, AssociatedObject, ref newValueReference);

	//					//TODO remove this and find a way to have UpdateTargetedParentReference provide the actual reference to the updated value instance
	//					targetAnimatable = AnimationTemplate.SelectorExpression.ResolveSelectorTree(
	//						dependencyObject, AssociatedObject);
	//				}
	//			}


	//			var targetDependendencyObject = targetAnimatable as DependencyObject;
	//			if (targetDependendencyObject == null)
	//				throw new NotSupportedException(
	//					$"Element targeted by the AnimationSelector type \'{targetAnimatable.GetType().Name}\' must be \'DependencyObject\'");

	//			var currentValue = targetDependendencyObject.GetValue(AnimationTemplate.Emitter.Property) as double?;

	//			//var currentValue2 = targetDependendencyObject.ReadLocalValue(AnimationTemplate.Emitter.Property) as double?;


	//			if (!currentValue.HasValue)
	//			{
	//				throw new NotSupportedException("Current value must not null and of type \'double\'");
	//			}
	//			if (AnimationTemplate.Emitter.ApplyFromBeforeOffset)
	//			{
	//				if (AnimationTemplate.Emitter.From.IsUnset)
	//				{
	//					AnimationTemplate.Emitter.FromPropertyChanged += (@newAnimationValue) =>
	//					{
	//						var effectiveCurrentValue = AnimationTemplate.Emitter.From.GetEffectiveValue(currentValue);
	//						if (!effectiveCurrentValue.HasValue)
	//							return;

	//						targetDependendencyObject.SetCurrentValue(AnimationTemplate.Emitter.Property, effectiveCurrentValue);
	//					};
	//				}
	//				else
	//				{
	//					var effectiveCurrentValue = AnimationTemplate.Emitter.From.GetEffectiveValue(currentValue);
	//					if (!effectiveCurrentValue.HasValue)
	//						break;

	//					targetDependendencyObject.SetCurrentValue(AnimationTemplate.Emitter.Property, effectiveCurrentValue);
	//					//targetDependendencyObject.SetCurrentValue(AnimationTemplate.Emitter.Property, AnimationTemplate.Emitter.From);
	//				}
	//			}

	//			var animationTimeline = AnimationTemplate.Emitter.Emit(itemIndex, total, currentValue.Value);

	//			targetAnimatable.BeginAnimation(AnimationTemplate.Emitter.Property, animationTimeline);
	//			//}
	//			//catch (ReactionExecutionException ex)
	//			//{

	//			//}
	//			itemIndex++;
	//		}
	//	}

	//	public static void ReceiveMarkupExtension(object targetObject, XamlSetMarkupExtensionEventArgs eventArgs)
	//	{
	//		if (targetObject == null)
	//			throw new ArgumentNullException(nameof(targetObject));
	//		if (eventArgs == null)
	//			throw new ArgumentNullException(nameof(eventArgs));
	//		var setter = targetObject as FlexibleIterativeReaction;
	//		if (setter == null || eventArgs.Member.Name != nameof(IterationTargetPropertyName))
	//			return;
	//		var markupExtension = eventArgs.MarkupExtension;
	//		if (markupExtension is StaticResourceExtension)
	//		{
	//			var resourceExtension = markupExtension as StaticResourceExtension;
	//			setter.IterationTargetPropertyName = resourceExtension.InvokeInternalMethod<string>(
	//				"ProvideValueInternal", eventArgs.ServiceProvider, true);
	//			eventArgs.Handled = true;
	//		}
	//		else
	//		{
	//			if (!(markupExtension is DynamicResourceExtension) && !(markupExtension is BindingBase))
	//				return;
	//			throw new NotImplementedException();
	//			//setter.IterationTargetPropertyName = markupExtension;
	//			//eventArgs.Handled = true;
	//		}
	//	}

	//	public static void ReceiveTypeConverter(object targetObject, XamlSetTypeConverterEventArgs eventArgs)
	//	{
	//		var setter = targetObject as FlexibleIterativeReaction;
	//		if (setter == null)
	//			throw new ArgumentNullException(nameof(targetObject));
	//		if (eventArgs == null)
	//			throw new ArgumentNullException(nameof(eventArgs));
	//		if (eventArgs.Member.Name == nameof(IterationTargetPropertyName))
	//		{
	//			//setter._unresolvedProperty = eventArgs.Value;
	//			//setter._serviceProvider = eventArgs.ServiceProvider;
	//			//setter._typeConverterCultureInfo = eventArgs.CultureInfo;
	//			eventArgs.Handled = true;
	//		}
	//		else
	//		{
	//			throw new NotImplementedException();
	//			//if (eventArgs.Member.Name != nameof(Value))
	//			//	return;
	//			//setter._unresolvedValue = eventArgs.Value;
	//			//setter._serviceProvider = eventArgs.ServiceProvider;
	//			//setter._typeConverterCultureInfo = eventArgs.CultureInfo;
	//			//eventArgs.Handled = true;
	//		}
	//	}

	//}
}
#region Old Stuff					
//TODO if the target is not a DependencyObject, should reflection be used to set the value?
//TODO should still set if From property is null?
//targetDependendencyObject.ClearAnimationClock(targetProperty);
//targetDependendencyObject.SetValue(targetProperty, AnimationTemplate.Emitter.From);


/*
			//var BeginTime = TimeSpan.FromMilliseconds(600);
		//var targetProperty = UIElement.OpacityProperty;
		//double? from = 0d;
		//double? to = 1d;
		//TimeSpan OffsetTime = TimeSpan.FromMilliseconds(500);

		//var currentOffset = BeginTime;



		//if (AssociatedObject != null && AnimationTemplate != null && ResolvedIterationTarget != null)
		//{

		//}
		//else
		//{

		//}
		*/

/*

	void ISupportInitialize.BeginInit()
	{
	}

	void ISupportInitialize.EndInit()
	{
		//if (_unresolvedProperty != null)
		//{
		//	try
		//	{
		//		Property = typeof(DependencyPropertyConverter).InvokeInternalStaticMethod<DependencyProperty>(
		//			"ResolveProperty", _serviceProvider, TargetName, _unresolvedProperty);
		//	}
		//	finally
		//	{
		//		_unresolvedProperty = null;
		//	}
		//}
		//if (_unresolvedValue != null)
		//{
		//	try
		//	{
		//		Value = typeof(SetterTriggerConditionValueConverter).InvokeInternalStaticMethod<object>(
		//			"ResolveValue", _serviceProvider, Property, _typeConverterCultureInfo, _unresolvedValue);
		//	}
		//	finally
		//	{
		//		_unresolvedValue = null;
		//	}
		//}
		//_typeConverterCultureInfo = null;
	}*/
#endregion