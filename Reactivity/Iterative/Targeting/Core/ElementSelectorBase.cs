using System.Windows;
using System.Windows.Markup;
using Core.Helpers.DependencyHelpers;
using Reactivity.Core;

namespace Reactivity.Iterative.Targeting.Core
{
	//TODO should this be freezable?
	[ContentProperty("NextSelector")]
	public abstract class ElementSelectorBase : AttachableBase
	{
		protected bool _isAssociationDeferred;

		public static readonly DependencyProperty NextSelectorProperty = DP.Register(
			new Meta<ElementSelectorBase, ElementSelectorBase>(null, onNextSelectorChanged));

		private static void onNextSelectorChanged(ElementSelectorBase i, DPChangedEventArgs<ElementSelectorBase> e)
		{
			e.NewValue?.Attach(i);
		}

		public ElementSelectorBase NextSelector
		{
			get { return (ElementSelectorBase)GetValue(NextSelectorProperty); }
			set { SetValue(NextSelectorProperty, value); }
		}

		public object Resolve(object parent, ref SelectorTreeResolutionContext context)
		{
			var resolvedObject = ResolveImpl(parent, ref context);
			context.AddFrame(this, resolvedObject);
			return resolvedObject;
		}

		protected abstract object ResolveImpl(object parent, ref SelectorTreeResolutionContext context);
	}
}
