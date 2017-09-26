using System;
using System.Linq;
using System.Windows;
using System.Windows.Markup;
using System.Windows.Media.Animation;
using Core.Extensions;
using Core.Helpers.CLREventHelpers;
using Core.Helpers.DependencyHelpers;
using Core.Markup.TypeConverterInjection;
using Reactivity.Animation;
using Reactivity.Animation.Stylization;
using Reactivity.Core;
using Reactivity.Iterative.Targeting.Core;

namespace Reactivity.Iterative.Emitters
{
	[XamlSetTypeConverter(nameof(ReceiveTypeConverter))]
	public abstract class AnimationEmitterBase : AttachableBase, IAnimationEmitterBaseCore
	{
		public static readonly DependencyProperty AnimationStyleProperty = DP.Register(
			new Meta<AnimationEmitterBase, AnimationStyle>(null, onAnimationStyleChanged));

		public static readonly DependencyProperty SubSelectorExpressionProperty = DP.Register(
			new Meta<AnimationEmitterBase, SelectorExpressionTree>());

		public static readonly DependencyProperty PropertyProperty = DP.Register(
			new Meta<AnimationEmitterBase, DependencyProperty>());

		public static readonly DependencyProperty ApplyFromBeforeOffsetProperty = DP.Register(
			new Meta<AnimationEmitterBase, bool>());

		public static readonly DependencyProperty DurationProperty = DP.Register(
			new Meta<AnimationEmitterBase, Duration>());

		public static readonly DependencyProperty BeginTimeProperty = DP.Register(
			new Meta<AnimationEmitterBase, TimeSpan>());

		public static readonly DependencyProperty EasingFunctionProperty = DP.Register(
			new Meta<AnimationEmitterBase, IEasingFunction>());

		public static readonly DependencyProperty OffsetBeginTimeProperty = DP.Register(
			new Meta<AnimationEmitterBase, bool>());

		public static readonly DependencyProperty SpeedRatioProperty = DP.Register(
			new Meta<AnimationEmitterBase, double>(1));

		public static readonly DependencyProperty IterativeOffsetProperty = DP.Register(
			new Meta<AnimationEmitterBase, IterativeOffset>(new IterativeOffset()));


		public AnimationStyle AnimationStyle
		{
			get { return (AnimationStyle)GetValue(AnimationStyleProperty); }
			set { SetValue(AnimationStyleProperty, value); }
		}
		public SelectorExpressionTree SubSelectorExpression
		{
			get { return (SelectorExpressionTree)GetValue(SubSelectorExpressionProperty); }
			set { SetValue(SubSelectorExpressionProperty, value); }
		}
		public DependencyProperty Property
		{
			get { return (DependencyProperty)GetValue(PropertyProperty); }
			set { SetValue(PropertyProperty, value); }
		}
		public bool ApplyFromBeforeOffset
		{
			get { return (bool)GetValue(ApplyFromBeforeOffsetProperty); }
			set { SetValue(ApplyFromBeforeOffsetProperty, value); }
		}
		//[InjectTypeConverter(typeof(CustomDurationConverter))]
		public Duration Duration
		{
			get { return (Duration)GetValue(DurationProperty); }
			set { SetValue(DurationProperty, value); }
		}
		//[InjectTypeConverter(typeof(CustomTimeSpanConverter))]
		public TimeSpan BeginTime
		{
			get { return (TimeSpan)GetValue(BeginTimeProperty); }
			set { SetValue(BeginTimeProperty, value); }
		}
		public IEasingFunction EasingFunction
		{
			get { return (IEasingFunction)GetValue(EasingFunctionProperty); }
			set { SetValue(EasingFunctionProperty, value); }
		}
		public bool OffsetBeginTime
		{
			get { return (bool)GetValue(OffsetBeginTimeProperty); }
			set { SetValue(OffsetBeginTimeProperty, value); }
		}
		public double SpeedRatio
		{
			get { return (double)GetValue(SpeedRatioProperty); }
			set { SetValue(SpeedRatioProperty, value); }
		}
		public IterativeOffset IterativeOffset
		{
			get { return (IterativeOffset)GetValue(IterativeOffsetProperty); }
			set { SetValue(IterativeOffsetProperty, value); }
		}



		private static void onAnimationStyleChanged(AnimationEmitterBase i, DPChangedEventArgs<AnimationStyle> e)
		{
			if (e.NewValue == null)
			{
				return;
			}
			i.applyAnimationStyle(e.NewValue);
		}

		private void applyAnimationStyle(AnimationStyle animationStyle)
		{
			foreach (var setter in animationStyle.Setters.OfType<Setter>())
			{
				SetValue(setter.Property, setter.Value);
			}
		}


		IAnimationValueCore IAnimationEmitterBaseCore.From => FromBase;

		IAnimationValueCore IAnimationEmitterBaseCore.To => ToBase;


		protected abstract IAnimationValueCore FromBase { get; }

		protected abstract IAnimationValueCore ToBase { get; }


		event ParameterizedEventHandler<IAnimationValueCore> IAnimationEmitterBaseCore.FromPropertyChanged
		{
			add { HookFromPropertyChangedBase(value); }
			remove { UnhookFromPropertyChangedBase(value); }
		}

		event ParameterizedEventHandler<IAnimationValueCore> IAnimationEmitterBaseCore.ToPropertyChanged
		{
			add { HookToPropertyChangedBase(value); }
			remove { UnhookToPropertyChangedBase(value); }
		}


		protected abstract void HookFromPropertyChangedBase(ParameterizedEventHandler<IAnimationValueCore> from);

		protected abstract void UnhookFromPropertyChangedBase(ParameterizedEventHandler<IAnimationValueCore> from);


		protected abstract void HookToPropertyChangedBase(ParameterizedEventHandler<IAnimationValueCore> to);

		protected abstract void UnhookToPropertyChangedBase(ParameterizedEventHandler<IAnimationValueCore> to);


		AnimationTimeline IAnimationEmitterBaseCore.Emit(int index, int totalSteps, object currentValue)
			=> EmitBase(index, totalSteps, currentValue);

		protected abstract AnimationTimeline EmitBase(int index, int totalSteps, object currentValue);


		public static void ReceiveTypeConverter(object targetObject, XamlSetTypeConverterEventArgs eventArgs)
		{
			TypeConverterInjectionCore.HandlePropertySet(targetObject, eventArgs);
		}
	}

	[XamlSetTypeConverter(nameof(ReceiveTypeConverter))]
	public abstract class AnimationEmitterBase<T> : AnimationEmitterBase where T : struct
	{
		public static readonly DependencyProperty FromProperty = DP.Register(
			new Meta<AnimationEmitterBase<T>, AnimationValue<T>>(new LiteralAnimationValue<T>(null),
				FromPropertyChangedCallback), IsValidAnimationValue);

		public static readonly DependencyProperty ToProperty = DP.Register(
			new Meta<AnimationEmitterBase<T>, AnimationValue<T>>(new LiteralAnimationValue<T>(null),
				ToPropertyChangedCallback), IsValidAnimationValue);


		public AnimationValue<T> From
		{
			get { return (AnimationValue<T>)GetValue(FromProperty); }
			set { SetValue(FromProperty, value); }
		}
		public AnimationValue<T> To
		{
			get { return (AnimationValue<T>)GetValue(ToProperty); }
			set { SetValue(ToProperty, value); }
		}

		protected sealed override IAnimationValueCore FromBase => From;
		protected sealed override IAnimationValueCore ToBase => To;

		//protected sealed override ParameterizedEventHandler<IAnimationValueCore> ToPropertyChangedBase => From;
		//protected sealed override ParameterizedEventHandler<IAnimationValueCore> ToPropertyChangedBase => To;


		public event ParameterizedEventHandler<AnimationValue<T>> FromPropertyChanged;
		public event ParameterizedEventHandler<AnimationValue<T>> ToPropertyChanged;


		private static void FromPropertyChangedCallback(AnimationEmitterBase<T> i, DPChangedEventArgs<AnimationValue<T>> e)
		{
			i.FromPropertyChanged.Raise(e.NewValue);
		}
		private static void ToPropertyChangedCallback(AnimationEmitterBase<T> i, DPChangedEventArgs<AnimationValue<T>> e)
		{
			i.ToPropertyChanged.Raise(e.NewValue);
		}


		protected sealed override void HookFromPropertyChangedBase(ParameterizedEventHandler<IAnimationValueCore> from)
		{
			FromPropertyChanged += from;
		}
		protected sealed override void UnhookFromPropertyChangedBase(ParameterizedEventHandler<IAnimationValueCore> from)
		{
			FromPropertyChanged -= from;
		}

		protected sealed override void HookToPropertyChangedBase(ParameterizedEventHandler<IAnimationValueCore> to)
		{
			ToPropertyChanged += to;
		}
		protected sealed override void UnhookToPropertyChangedBase(ParameterizedEventHandler<IAnimationValueCore> to)
		{
			ToPropertyChanged -= to;
		}


		private static bool IsValidAnimationValue(AnimationValue<T> value)
		{
			return value != null;
		}

		protected sealed override AnimationTimeline EmitBase(int index, int totalSteps, object currentValue)
		{
			return Emit(index, totalSteps, (T?)currentValue);
		}

		public abstract AnimationTimeline Emit(int index, int totalSteps, T? currentValue);
	}
}
