using System.Windows.Media.Animation;
using Reactivity.Animation;

namespace Reactivity.Iterative.Emitters
{
	public interface IAnimationTimelineEmitter
	{
		IAnimatedValue From { get; }

		IAnimatedValue To { get; } 


		event IAnimatedValueChangedHandler FromPropertyChanged;

		event IAnimatedValueChangedHandler ToPropertyChanged;


		AnimationTimeline Emit(
			int index,
			int totalSteps,
			object currentValue);

	}
}
