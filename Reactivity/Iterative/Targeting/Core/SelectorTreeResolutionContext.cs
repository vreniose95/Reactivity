using System.Collections.Generic;
using System.Linq;
using System.Windows;

namespace Reactivity.Iterative.Targeting.Core
{
	public class SelectorTreeResolutionContext
	{
		protected List<SelectorResolutionFrame> SelectorResolutionFrames { get; }
		
		public DependencyObject RootExecutionContext { get; }

		public SelectorTreeResolutionContext(DependencyObject _rootExecutionContext)
		{
			RootExecutionContext = _rootExecutionContext;
			SelectorResolutionFrames = new List<SelectorResolutionFrame>();
		}
		//TODO support frame removal? or make selector trees freezablee so they cant be changed at runtime
		public void AddFrame(ElementSelectorBase selectorInstance, object resolvedValue)
		{
		SelectorResolutionFrames.Insert(0, new SelectorResolutionFrame(selectorInstance, resolvedValue));
		}

		public SelectorResolutionFrame GetBacktracedFrame(int reverseIndex)
		{
			if (reverseIndex > SelectorResolutionFrames.Count - 1)
				return null;
			return SelectorResolutionFrames[reverseIndex];
		}

		public SelectorResolutionFrame GetLastFrame()
		{
			if (!SelectorResolutionFrames.Any())
				return null;
			return SelectorResolutionFrames[0];
		}

	}
}
