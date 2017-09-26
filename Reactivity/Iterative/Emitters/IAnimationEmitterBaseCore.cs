using System.Windows.Media.Animation;
using Core.Helpers.CLREventHelpers;
using Reactivity.Animation;

namespace Reactivity.Iterative.Emitters
{
	public interface IAnimationEmitterBaseCore
	{
		IAnimationValueCore From { get; } 

		IAnimationValueCore To { get; } 


		event ParameterizedEventHandler<IAnimationValueCore> FromPropertyChanged;

		event ParameterizedEventHandler<IAnimationValueCore> ToPropertyChanged;


		AnimationTimeline Emit(int index, int totalSteps, object currentValue);

	}
}
