using System.Windows;
using System.Windows.Markup;
using Reactivity.Iterative.Targeting.Core;

namespace Reactivity.Iterative.Targeting.Macros
{
	//TODO frame backrace mode... RelativeSelectorSource Findancestor AncestorBacktraceIndex/AncestorSelectorType
	[ContentProperty("NextSelector")]
	public class FindNameInDataTemplateSelector : ElementSelectorBase
	{
		public string TargetName { get; set; }

		protected override object ResolveImpl(object parent, ref SelectorTreeResolutionContext context)
		{
			var dataTemplate = parent as DataTemplate;
			if (dataTemplate == null)
				return null;
			//var templatedParent = param as FrameworkElement;
			//if (templatedParent == null)
			//	return null;
			var templatedParentFrame = context.GetBacktracedFrame(1);
			var templatedParent = templatedParentFrame.ResolvedValue as FrameworkElement;
			if (templatedParent == null)
				return null;
			var targetObject = dataTemplate.FindName(TargetName, templatedParent);
			if (targetObject == null)
				return null;
			return targetObject;
		}
	}
}