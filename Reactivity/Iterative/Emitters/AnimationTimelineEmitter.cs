using System;
using System.Windows;
using System.Windows.Media.Animation;
using Core.Extensions;
using Core.Helpers.CLREventHelpers;
using Core.Helpers.DependencyHelpers;
using Reactivity.Animation;
using Reactivity.Iterative.Targeting.Core;

namespace Reactivity.Iterative.Emitters
{
	//[ContentProperty("Emitter")]
	//TODO binding support
	//public class AnimationTimelineEmitter : DependencyObject
	//{

	//	public static readonly DependencyProperty SubElementSelectorProperty = DP.Register(
	//		new Meta<AnimationTimelineEmitter, ElementSelectorBase>());

	//	public static readonly DependencyProperty PropertyProperty = DP.Register(
	//		new Meta<AnimationTimelineEmitter, DependencyProperty>());

	//	public static readonly DependencyProperty ApplyFromBeforeOffsetProperty = DP.Register(
	//		new Meta<AnimationTimelineEmitter, bool>());

	//	public static readonly DependencyProperty FromProperty = DP.Register(
	//		new Meta<AnimationTimelineEmitter, AnimationValue<double?>>(new LiteralDoubleAnimationValue(null),
	//			FromPropertyChangedCallback), IsValidAnimationValue);

	//	public static readonly DependencyProperty ToProperty = DP.Register(
	//		new Meta<AnimationTimelineEmitter, AnimationValue<double?>>(new LiteralDoubleAnimationValue(null),
	//			ToPropertyChangedCallback), IsValidAnimationValue);

	//	public static readonly DependencyProperty DurationProperty = DP.Register(
	//		new Meta<AnimationTimelineEmitter, Duration>());

	//	public static readonly DependencyProperty BeginTimeProperty = DP.Register(
	//		new Meta<AnimationTimelineEmitter, AnimationTimeSpan>());

	//	public static readonly DependencyProperty OffsetProperty = DP.Register(
	//		new Meta<AnimationTimelineEmitter, AnimationTimeSpan>());

	//	public static readonly DependencyProperty EasingFunctionProperty = DP.Register(
	//		new Meta<AnimationTimelineEmitter, IEasingFunction>());

	//	public static readonly DependencyProperty IterativeOffsetModeProperty = DP.Register(
	//		new Meta<AnimationTimelineEmitter, IterativeOffsetMode>());

	//	public static readonly DependencyProperty OffsetBeginTimeProperty = DP.Register(
	//		new Meta<AnimationTimelineEmitter, bool>());

	//	public static readonly DependencyProperty MaximumEffectiveOffsetProperty = DP.Register(
	//		new Meta<AnimationTimelineEmitter, AnimationTimeSpan?>(null));

	//	public static readonly DependencyProperty SpeedRatioProperty = DP.Register(
	//		new Meta<AnimationTimelineEmitter, double>(1));


	//	public event ParameterizedEventHandler<AnimationValue<double?>> FromPropertyChanged;
	//	public event ParameterizedEventHandler<AnimationValue<double?>> ToPropertyChanged;


	//	private static void FromPropertyChangedCallback(AnimationTimelineEmitter i, DPChangedEventArgs<AnimationValue<double?>> e)
	//	{
	//		i.FromPropertyChanged.Raise(e.NewValue);
	//	}
	//	private static void ToPropertyChangedCallback(AnimationTimelineEmitter i, DPChangedEventArgs<AnimationValue<double?>> e)
	//	{
	//		i.ToPropertyChanged.Raise(e.NewValue);
	//	}

	//	private static bool IsValidAnimationValue(AnimationValue<double?> value)
	//	{
	//		return value != null;
	//	}


	//	public ElementSelectorBase SubElementSelector
	//	{
	//		get { return (ElementSelectorBase)GetValue(SubElementSelectorProperty); }
	//		set { SetValue(SubElementSelectorProperty, value); }
	//	}
	//	public DependencyProperty Property
	//	{
	//		get { return (DependencyProperty)GetValue(PropertyProperty); }
	//		set { SetValue(PropertyProperty, value); }
	//	}
	//	public bool ApplyFromBeforeOffset
	//	{
	//		get { return (bool)GetValue(ApplyFromBeforeOffsetProperty); }
	//		set { SetValue(ApplyFromBeforeOffsetProperty, value); }
	//	}
	//	public AnimationValue<double?> From
	//	{
	//		get { return (AnimationValue<double?>)GetValue(FromProperty); }
	//		set { SetValue(FromProperty, value); }
	//	}
	//	public AnimationValue<double?> To
	//	{
	//		get { return (AnimationValue<double?>)GetValue(ToProperty); }
	//		set { SetValue(ToProperty, value); }
	//	}
	//	public Duration Duration
	//	{
	//		get { return (Duration)GetValue(DurationProperty); }
	//		set { SetValue(DurationProperty, value); }
	//	}
	//	public AnimationTimeSpan BeginTime
	//	{
	//		get { return (AnimationTimeSpan)GetValue(BeginTimeProperty); }
	//		set { SetValue(BeginTimeProperty, value); }
	//	}
	//	public AnimationTimeSpan Offset
	//	{
	//		get { return (AnimationTimeSpan)GetValue(OffsetProperty); }
	//		set { SetValue(OffsetProperty, value); }
	//	}
	//	public IEasingFunction EasingFunction
	//	{
	//		get { return (IEasingFunction)GetValue(EasingFunctionProperty); }
	//		set { SetValue(EasingFunctionProperty, value); }
	//	}
	//	public IterativeOffsetMode IterativeOffsetMode
	//	{
	//		get { return (IterativeOffsetMode)GetValue(IterativeOffsetModeProperty); }
	//		set { SetValue(IterativeOffsetModeProperty, value); }
	//	}
	//	public bool OffsetBeginTime
	//	{
	//		get { return (bool)GetValue(OffsetBeginTimeProperty); }
	//		set { SetValue(OffsetBeginTimeProperty, value); }
	//	}
	//	public AnimationTimeSpan? MaximumEffectiveOffset
	//	{
	//		get { return (AnimationTimeSpan?)GetValue(MaximumEffectiveOffsetProperty); }
	//		set { SetValue(MaximumEffectiveOffsetProperty, value); }
	//	}
	//	public double SpeedRatio
	//	{
	//		get { return (double)GetValue(SpeedRatioProperty); }
	//		set { SetValue(SpeedRatioProperty, value); }
	//	}


	//	public virtual AnimationTimeline Emit(int index, int totalSteps, double currentValue)
	//	{
	//		var stepOffset = getStepOffset(totalSteps);
	//		var currentOffset = BeginTime + stepOffset.MultipliedBy(index);

	//		if (OffsetBeginTime)
	//		{
	//			currentOffset += stepOffset;
	//		}

	//		var effectiveFromValue = From.GetEffectiveValue(currentValue);
	//		var effectiveToValue = To.GetEffectiveValue(currentValue);

	//		return new DoubleAnimation
	//		{
	//			Duration = Duration,
	//			From = effectiveFromValue,
	//			To = effectiveToValue,
	//			BeginTime = currentOffset.TimeSpan,
	//			EasingFunction = EasingFunction,
	//			SpeedRatio = SpeedRatio,
	//		};
	//	}

	//	private AnimationTimeSpan getStepOffset(int totalSteps)
	//	{
	//		var effectiveOffset = IterativeOffsetMode == IterativeOffsetMode.Step ? Offset : Offset.DividedBy(totalSteps);

	//		if (MaximumEffectiveOffset.HasValue)
	//		{
	//			if (effectiveOffset > MaximumEffectiveOffset.Value)
	//			{
	//				effectiveOffset = MaximumEffectiveOffset.Value;
	//			}
	//		}
	//		return effectiveOffset;
	//	}
	//}

	//public class HierarchicalAnimationTimelineEmitter : AnimationTimelineEmitter
	//{
	//	public static readonly DependencyProperty MatrixChunkSizeProperty = DP.Register(
	//		new Meta<AnimationTimelineEmitter, int>(5));
	//	public int MatrixChunkSize
	//	{
	//		get { return (int)GetValue(MatrixChunkSizeProperty); }
	//		set { SetValue(MatrixChunkSizeProperty, value); }
	//	}
	//	public static readonly DependencyProperty Axis2DurationProperty = DP.Register(
	//		new Meta<HierarchicalAnimationTimelineEmitter, Duration>());
	//	public Duration Axis2Duration
	//	{
	//		get { return (Duration)GetValue(Axis2DurationProperty); }
	//		set { SetValue(Axis2DurationProperty, value); }
	//	}
	//	public static readonly DependencyProperty Axis2OffsetProperty = DP.Register(
	//		new Meta<HierarchicalAnimationTimelineEmitter, AnimationTimeSpan>());
	//	public AnimationTimeSpan Axis2Offset
	//	{
	//		get { return (AnimationTimeSpan)GetValue(Axis2OffsetProperty); }
	//		set { SetValue(Axis2OffsetProperty, value); }
	//	}
	//	public static readonly DependencyProperty Axis2IterativeOffsetModeProperty = DP.Register(
	//		new Meta<HierarchicalAnimationTimelineEmitter, IterativeOffsetMode>());
	//	public IterativeOffsetMode Axis2IterativeOffsetMode
	//	{
	//		get { return (IterativeOffsetMode)GetValue(Axis2IterativeOffsetModeProperty); }
	//		set { SetValue(Axis2IterativeOffsetModeProperty, value); }
	//	}
	//	public static readonly DependencyProperty Axis2MaximumEffectiveOffsetProperty = DP.Register(
	//		new Meta<HierarchicalAnimationTimelineEmitter, AnimationTimeSpan?>());
	//	public AnimationTimeSpan? Axis2MaximumEffectiveOffset
	//	{
	//		get { return (AnimationTimeSpan?)GetValue(Axis2MaximumEffectiveOffsetProperty); }
	//		set { SetValue(Axis2MaximumEffectiveOffsetProperty, value); }
	//	}


	//	public override AnimationTimeline Emit(int index, int totalSteps, double currentValue)
	//	{
	//		var axis1Steps = MatrixChunkSize;
	//		var axis2Steps = (int)Math.Floor((double)totalSteps / axis1Steps);

	//		var axis1StepOffset = getAxis1StepOffset(axis1Steps);
	//		var axis2StepOffset = getAxis2StepOffset(axis2Steps);

	//		var currentAxis1Index = index % MatrixChunkSize;
	//		var currentAxis2Index = (int)Math.Floor((double)index / currentAxis1Index);

	//		var currentOffset = BeginTime + axis1StepOffset.MultipliedBy(currentAxis1Index)
	//			+ axis2StepOffset.MultipliedBy(currentAxis2Index);

	//		if (OffsetBeginTime)
	//		{
	//			currentOffset += axis1StepOffset;
	//		}

	//		var effectiveFromValue = From.GetEffectiveValue(currentValue);
	//		var effectiveToValue = To.GetEffectiveValue(currentValue);

	//		return new DoubleAnimation
	//		{
	//			Duration = Duration,
	//			From = effectiveFromValue,
	//			To = effectiveToValue,
	//			BeginTime = currentOffset.TimeSpan,
	//			EasingFunction = EasingFunction,
	//			SpeedRatio = SpeedRatio,
	//		};
	//	}

	//	private AnimationTimeSpan getAxis1StepOffset(int totalSteps)
	//	{
	//		var effectiveOffset = IterativeOffsetMode == IterativeOffsetMode.Step ? Offset : Offset.DividedBy(totalSteps);

	//		if (MaximumEffectiveOffset.HasValue)
	//		{
	//			if (effectiveOffset > MaximumEffectiveOffset.Value)
	//			{
	//				effectiveOffset = MaximumEffectiveOffset.Value;
	//			}
	//		}
	//		return effectiveOffset;
	//	}
	//	private AnimationTimeSpan getAxis2StepOffset(int totalSteps)
	//	{
	//		var effectiveOffset = Axis2IterativeOffsetMode == IterativeOffsetMode.Step ? Axis2Offset : Axis2Offset.DividedBy(totalSteps);

	//		if (Axis2MaximumEffectiveOffset.HasValue)
	//		{
	//			if (effectiveOffset > Axis2MaximumEffectiveOffset.Value)
	//			{
	//				effectiveOffset = Axis2MaximumEffectiveOffset.Value;
	//			}
	//		}
	//		return effectiveOffset;
	//	}
	//}
}
