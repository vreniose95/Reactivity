using System;
using System.Linq;
using System.Windows;
using System.Windows.Markup;
using System.Windows.Media.Animation;
using Ccr.PresentationCore.Helpers.DependencyHelpers;
using Ccr.Xaml.Markup.TypeConverterInjection;
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
	public abstract class AnimationEmitterBase 
		: AttachableBase, 
			IAnimationTimelineEmitter
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
			get => (AnimationStyle)GetValue(AnimationStyleProperty);
			set => SetValue(AnimationStyleProperty, value);
		}
		public SelectorExpressionTree SubSelectorExpression
		{
			get => (SelectorExpressionTree)GetValue(SubSelectorExpressionProperty);
			set => SetValue(SubSelectorExpressionProperty, value);
		}
		public DependencyProperty Property
		{
			get => (DependencyProperty)GetValue(PropertyProperty);
			set => SetValue(PropertyProperty, value);
		}
		public bool ApplyFromBeforeOffset
		{
			get => (bool)GetValue(ApplyFromBeforeOffsetProperty);
			set => SetValue(ApplyFromBeforeOffsetProperty, value);
		}
		//[InjectTypeConverter(typeof(CustomDurationConverter))]
		public Duration Duration
		{
			get => (Duration)GetValue(DurationProperty);
			set => SetValue(DurationProperty, value);
		}
		//[InjectTypeConverter(typeof(CustomTimeSpanConverter))]
		public TimeSpan BeginTime
		{
			get => (TimeSpan)GetValue(BeginTimeProperty);
			set => SetValue(BeginTimeProperty, value);
		}
		public IEasingFunction EasingFunction
		{
			get => (IEasingFunction)GetValue(EasingFunctionProperty);
			set => SetValue(EasingFunctionProperty, value);
		}
		public bool OffsetBeginTime
		{
			get => (bool)GetValue(OffsetBeginTimeProperty);
			set => SetValue(OffsetBeginTimeProperty, value);
		}
		public double SpeedRatio
		{
			get => (double)GetValue(SpeedRatioProperty);
			set => SetValue(SpeedRatioProperty, value);
		}
		public IterativeOffset IterativeOffset
		{
			get => (IterativeOffset)GetValue(IterativeOffsetProperty);
			set => SetValue(IterativeOffsetProperty, value);
		}



		private static void onAnimationStyleChanged(
			AnimationEmitterBase @this, 
			DPChangedEventArgs<AnimationStyle> args)
		{
			if (args.NewValue == null)
				return;
			
			@this.applyAnimationStyle(args.NewValue);
		}

		private void applyAnimationStyle(
			AnimationStyle animationStyle)
		{
			foreach (var setter in animationStyle.Setters.OfType<Setter>())
			{
				SetValue(setter.Property, setter.Value);
			}
		}


		IAnimatedValue IAnimationTimelineEmitter.From
		{
			get => FromBase;
		}

		IAnimatedValue IAnimationTimelineEmitter.To
		{
			get => ToBase;
		}


		protected abstract IAnimatedValue FromBase { get; }

		protected abstract IAnimatedValue ToBase { get; }



		event IAnimatedValueChangedHandler IAnimationTimelineEmitter.FromPropertyChanged
		{
			add => HookFromPropertyChangedBase(value);
			remove => UnhookFromPropertyChangedBase(value);
		}

		event IAnimatedValueChangedHandler IAnimationTimelineEmitter.ToPropertyChanged
		{
			add => HookToPropertyChangedBase(value);
			remove => UnhookToPropertyChangedBase(value);
		}


		protected abstract void HookFromPropertyChangedBase(IAnimatedValueChangedHandler from);

		protected abstract void UnhookFromPropertyChangedBase(IAnimatedValueChangedHandler from);


		protected abstract void HookToPropertyChangedBase(IAnimatedValueChangedHandler to);

		protected abstract void UnhookToPropertyChangedBase(IAnimatedValueChangedHandler to);


		AnimationTimeline IAnimationTimelineEmitter.Emit(
			int index,
			int totalSteps,
			object currentValue)
		{
			return EmitBase(index, totalSteps, currentValue);
		}

		protected abstract AnimationTimeline EmitBase(
			int index,
			int totalSteps,
			object currentValue);


		public static void ReceiveTypeConverter(
			object targetObject,
			XamlSetTypeConverterEventArgs eventArgs)
		{
			TypeConverterInjectionCore.HandlePropertySet(
				targetObject,
				eventArgs);
		}
	}

	[XamlSetTypeConverter(nameof(ReceiveTypeConverter))]
	public abstract class AnimationEmitterBase<TValue>
		: AnimationEmitterBase 
			where TValue
			: struct
	{
		public static readonly DependencyProperty FromProperty = DP.Register(
			new Meta<AnimationEmitterBase<TValue>, AnimatedValueBase<TValue>>(
				new LiteralAnimatedValue<TValue>(null),
				FromPropertyChangedCallback), IsValidAnimationValue);

		public static readonly DependencyProperty ToProperty = DP.Register(
			new Meta<AnimationEmitterBase<TValue>, AnimatedValueBase<TValue>>(
				new LiteralAnimatedValue<TValue>(null),
				ToPropertyChangedCallback), IsValidAnimationValue);


		public AnimatedValueBase<TValue> From
		{
			get => (AnimatedValueBase<TValue>)GetValue(FromProperty);
			set => SetValue(FromProperty, value);
		}
		public AnimatedValueBase<TValue> To
		{
			get => (AnimatedValueBase<TValue>)GetValue(ToProperty);
			set => SetValue(ToProperty, value);
		}


		protected sealed override IAnimatedValue FromBase
		{
			get => From;
		}
		protected sealed override IAnimatedValue ToBase
		{
			get => To;
		}
		
		public event AnimatedValueBaseChangedHandler<TValue> FromPropertyChanged;
		public event AnimatedValueBaseChangedHandler<TValue> ToPropertyChanged;


		private static void FromPropertyChangedCallback(
			AnimationEmitterBase<TValue> @this, 
			DPChangedEventArgs<AnimatedValueBase<TValue>> args)
		{
			@this.FromPropertyChanged?.Invoke(args.NewValue);
		}
		private static void ToPropertyChangedCallback(
			AnimationEmitterBase<TValue> @this,
			DPChangedEventArgs<AnimatedValueBase<TValue>> args)
		{
			@this.ToPropertyChanged?.Invoke(args.NewValue);
		}


		protected sealed override void HookFromPropertyChangedBase(
			ParameterizedEventHandler<IAnimationValueCore> from)
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
