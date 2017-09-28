using System.Windows;
using System.Windows.Markup;
using System.Windows.Media.Animation;
using Ccr.PresentationCore.Helpers.DependencyHelpers;
using Core.Extensions;
using Core.Helpers.CLREventHelpers;
using Core.Helpers.DependencyHelpers;
using Reactivity.Animation;
using Reactivity.Iterative.Emitters.MatrixAnimations.MatrixComposition;

namespace Reactivity.Iterative.Emitters.MatrixAnimations.MatrixAnimationEmitters
{
	public abstract class MatrixAnimationEmitterBase 
		: AnimationEmitterBase
	{
		public static readonly DependencyProperty MatrixCompositionProperty = DP.Register(
			new Meta<MatrixAnimationEmitterBase, MatrixCompositionBase>(new LinearMatrixComposition()));

		public MatrixCompositionBase MatrixComposition
		{
			get => (MatrixCompositionBase)GetValue(MatrixCompositionProperty);
			set => SetValue(MatrixCompositionProperty, value);
		}
	}
	[XamlSetTypeConverter(nameof(ReceiveTypeConverter))]
	public abstract class MatrixAnimationEmitterBase<TValue>
		: MatrixAnimationEmitterBase
			where TValue
				: struct
	{
		public static readonly DependencyProperty FromProperty = DP.Register(
			new Meta<MatrixAnimationEmitterBase<TValue>, AnimatedValueBase<TValue>>(
				new LiteralAnimatedValue<TValue>(null),
				FromPropertyChangedCallback), IsValidAnimationValue);

		public static readonly DependencyProperty ToProperty = DP.Register(
			new Meta<MatrixAnimationEmitterBase<TValue, AnimatedValueBase<TValue>>(
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
		


		private static void FromPropertyChangedCallback(
			MatrixAnimationEmitterBase<TValue> @this, 
			DPChangedEventArgs<AnimatedValueBase<TValue>> args)
		{
			@this.FromPropertyChanged?.Invoke(args.NewValue);
		}
		private static void ToPropertyChangedCallback(
			MatrixAnimationEmitterBase<TValue> @this, 
			DPChangedEventArgs<AnimatedValueBase<TValue>> args)
		{
			@this.ToPropertyChanged?.Invoke(args.NewValue);
		}


		public event AnimatedValueBaseChangedHandler<TValue> FromPropertyChanged;
		public event AnimatedValueBaseChangedHandler<TValue> ToPropertyChanged;


		protected sealed override void HookFromPropertyChangedBase(
			AnimatedValueBaseChangedHandler<TValue> from)
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
