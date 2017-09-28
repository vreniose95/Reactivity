using System;
using System.Collections.Specialized;
using System.Windows;
using System.Windows.Markup;
using Core.Helpers.DependencyHelpers;
using Reactivity.Collections;
using Reactivity.Core;
using Reactivity.Iterative.Emitters;
using Reactivity.Iterative.Targeting.Core;

namespace Reactivity.Animation
{
	//TODO look into AcceptedMarkupExtensionExpressionTypeAttribute
	[ContentProperty("Emitters")]
	public class AnimationTemplate : AttachableBase
	{
		public static readonly DependencyProperty SelectorExpressionProperty = DP.Register(
			new Meta<AnimationTemplate, SelectorExpressionTree>());

		//public static readonly DependencyProperty EmitterProperty = DP.Register(
		//	new Meta<AnimationTemplate, AnimationEmitterBase>(null, onEmitterChanged));

		public static readonly DependencyProperty EmittersProperty = DP.Register(
			new Meta<AnimationTemplate, AnimationEmitterCollection>(null, onEmittersChanged));


		//TODO get rid of this 
		public SelectorExpressionTree SelectorExpression
		{
			get { return (SelectorExpressionTree)GetValue(SelectorExpressionProperty); }
			set { SetValue(SelectorExpressionProperty, value); }
		}
		//public AnimationEmitterBase Emitter
		//{
		//	get { return (AnimationEmitterBase)GetValue(EmitterProperty); }
		//	set { SetValue(EmitterProperty, value); }
		//}
		public AnimationEmitterCollection Emitters
		{
			get { return (AnimationEmitterCollection)GetValue(EmittersProperty); }
			set { SetValue(EmittersProperty, value); }
		}

		public AnimationTemplate()
		{
			Emitters = new AnimationEmitterCollection();
			//Emitters.CollectionChanged += onEmittersCollectionChanged;
		}



		protected override void OnAttached()
		{
			base.OnAttached();
			if (Emitters != null && IsAssociated)
			{
				Emitters.Attach(AssociatedObject);
			}
			//attachAssociatedChildElement();
		}
		protected override void OnDetaching()
		{
			base.OnDetaching();
			if (Emitters != null)
			{
				Emitters.Detach();
			}
		}
		//protected override void OnDetaching()
		//{
		//	base.OnDetaching();
		//	if (Emitter != null)
		//	{
		//		Emitter.Detach();
		//	}
		//}

		private static void onEmittersChanged(AnimationTemplate i, DPChangedEventArgs<AnimationEmitterCollection> e)
		{
			if (e.OldValue == e.NewValue)
				return;
			if (e.OldValue != null && e.OldValue.IsAssociated)
				e.OldValue.Detach();

			if (e.NewValue == null || i == null)
				return;

			if (e.NewValue.IsAssociated)
				throw new InvalidOperationException("ExceptionStringTable.CannotHostBehaviorCollectionMultipleTimesExceptionMessage");

			e.NewValue.Attach(i);
		}
		//private void onEmittersCollectionChanged(object s, NotifyCollectionChangedEventArgs e)
		//{
		//	foreach (var newItem in e.NewItems)
		//	{
		//		var attachable = newItem as AttachableBase;
		//		if (attachable == null)
		//			throw new NotSupportedException($"Type \'{newItem.GetType().Name}\' not supported. Must be \'AttachableBase\'");

		//		if (e.Action == NotifyCollectionChangedAction.Add)
		//		{
		//			if (attachable.IsAssociated)
		//				throw new InvalidOperationException("Attachable element is already hosted. It cannot be hosted twice.");

		//			attachable.Attach(this);
		//		}
		//		else if (e.Action == NotifyCollectionChangedAction.Remove)
		//		{
		//			attachable.Detach();
		//		}
		//		else
		//		{
		//			throw new NotSupportedException("Action not supported.");
		//		}
		//	}

		//}
		//TODO get rid of
		//private static void onEmitterChanged(AnimationTemplate i, DPChangedEventArgs<AnimationEmitterBase> e)
		//{
		//	if (e.OldValue != null)
		//	{
		//		e.OldValue.Detach();
		//	}
		//	if (e.NewValue != null)
		//	{
		//		i.attachAssociatedChildElement();
		//	}
		//}

		//TODO get rid of this?
		//protected void attachAssociatedChildElement()
		//{
		//	if (Emitter != null && IsAssociated)
		//	{
		//		Emitter.Attach(AssociatedObject);
		//	}
		//}



	}
}
/*		//internal IAnimatable ResolveSelectorTree(DependencyObject dependencyObject)
		//{
		//	var selectorTreeResolutionContext = new SelectorTreeResolutionContext();
		//	var resolvedObject = SelectorEpxression.Resolve(dependencyObject, ref selectorTreeResolutionContext);
		//	var currentSelector = TargetElementSelector.NextSelector;
		//	while (currentSelector != null)
		//	{
		//		resolvedObject = currentSelector.Resolve(resolvedObject	, ref selectorTreeResolutionContext);
		//		currentSelector = currentSelector.NextSelector;
		//	}
		//	return (IAnimatable) resolvedObject;
		//}*/
