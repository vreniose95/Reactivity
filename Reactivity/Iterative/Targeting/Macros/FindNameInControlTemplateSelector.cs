using System;
using System.Windows.Controls;
using System.Windows.Markup;
using Reactivity.Iterative.Targeting.Core;

namespace Reactivity.Iterative.Targeting.Macros
{
	[ContentProperty("NextSelector")]
	public class FindNameInControlTemplateSelector : ElementSelectorBase
	{
		public string TargetName { get; set; }

		protected override object ResolveImpl(object parent, ref SelectorTreeResolutionContext context)
		{
			var control = parent as Control;
			if (control == null)
				throw new NotSupportedException("Can only execute FindNameInControlTemplateSelector on a Control.");

			//var frameworkElement = parent as FrameworkElement;
			//if(frameworkElement == null)
			//	throw new NotSupportedException("Parent selector must be FrameworkElement.");

			//frameworkElement.ApplyTemplate();
			control.ApplyTemplate();

			var targetElement = control.Template.FindName(TargetName, control);
			return targetElement;
		}
	}
}
