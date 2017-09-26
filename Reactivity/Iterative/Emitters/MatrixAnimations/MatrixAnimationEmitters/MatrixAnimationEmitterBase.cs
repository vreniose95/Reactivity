using System.Windows;
using System.Windows.Markup;
using System.Windows.Media.Animation;
using Core.Extensions;
using Core.Helpers.CLREventHelpers;
using Core.Helpers.DependencyHelpers;
using Reactivity.Animation;
using Reactivity.Iterative.Emitters.MatrixAnimations.MatrixComposition;

namespace Reactivity.Iterative.Emitters.MatrixAnimations.MatrixAnimationEmitters
{
	public abstract class MatrixAnimationEmitterBase : AnimationEmitterBase
	{
		public static readonly DependencyProperty MatrixCompositionProperty = DP.Register(
			new Meta<MatrixAnimationEmitterBase, MatrixCompositionBase>(new LinearMatrixComposition()));
		public MatrixCompositionBase MatrixComposition
		{
			get { return (MatrixCompositionBase)GetValue(MatrixCompositionProperty); }
			set { SetValue(MatrixCompositionProperty, value); }
		}
	}
	[XamlSetTypeConverter(nameof(ReceiveTypeConverter))]
	public abstract class MatrixAnimationEmitterBase<T> : MatrixAnimationEmitterBase where T : struct
	{
		public static readonly DependencyProperty FromProperty = DP.Register(
			new Meta<MatrixAnimationEmitterBase<T>, AnimationValue<T>>(new LiteralAnimationValue<T>(null),
				FromPropertyChangedCallback), IsValidAnimationValue);

		public static readonly DependencyProperty ToProperty = DP.Register(
			new Meta<MatrixAnimationEmitterBase<T>, AnimationValue<T>>(new LiteralAnimationValue<T>(null),
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


		private static void FromPropertyChangedCallback(MatrixAnimationEmitterBase<T> i, DPChangedEventArgs<AnimationValue<T>> e)
		{
			i.FromPropertyChanged.Raise(e.NewValue);
		}
		private static void ToPropertyChangedCallback(MatrixAnimationEmitterBase<T> i, DPChangedEventArgs<AnimationValue<T>> e)
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
