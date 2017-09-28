using System.Windows.Media.Animation;
using Core.Extensions;

namespace Reactivity.Iterative.Emitters
{
	public class DoubleAnimationEmitter
		: AnimationEmitterBase<double>
	{
		//protected override void OnAttached()
		//{
		//	base.OnAttached();
		//	if (Emitter != null)
		//	{
		//		Emitter.Attach(this);
		//	}
		//}
		//protected override void OnDetaching()
		//{
		//	base.OnDetaching();
		//	if (Emitter != null)
		//	{
		//		Emitter.Detach();
		//	}
		//}

		public override AnimationTimeline Emit(
			int index, int totalSteps, double? currentValue)
		{
			var stepOffset = IterativeOffset.GetStepOffset(totalSteps);
			var currentOffset = BeginTime + stepOffset.MultipliedBy(index);

			if (OffsetBeginTime)
			{
				currentOffset += stepOffset;
			}

			var effectiveFromValue = From.GetEffectiveValue(currentValue);
			var effectiveToValue = To.GetEffectiveValue(currentValue);

			return new DoubleAnimation
			{
				Duration = Duration,
				From = effectiveFromValue,
				To = effectiveToValue,
				BeginTime = currentOffset,
				EasingFunction = EasingFunction,
				SpeedRatio = SpeedRatio
			};
		}
	}
}
