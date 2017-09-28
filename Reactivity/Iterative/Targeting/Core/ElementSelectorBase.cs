using System.Windows;
using System.Windows.Markup;
using Ccr.PresentationCore.Helpers.DependencyHelpers;
using Reactivity.Core;

namespace Reactivity.Iterative.Targeting.Core
{
	//TODO should this be freezable?
	[ContentProperty(nameof(NextSelector))]
	public abstract class ElementSelectorBase
		: AttachableBase
	{
		protected bool _isAssociationDeferred;

		public static readonly DependencyProperty NextSelectorProperty = DP.Register(
			new Meta<ElementSelectorBase, ElementSelectorBase>(null, onNextSelectorChanged));

		public ElementSelectorBase NextSelector
		{
			get => (ElementSelectorBase)GetValue(NextSelectorProperty);
			set => SetValue(NextSelectorProperty, value);
		}

		private static void onNextSelectorChanged(
			ElementSelectorBase @this,
			DPChangedEventArgs<ElementSelectorBase> args)
		{
			args.NewValue?.Attach(@this);
		}

		public object Resolve(
			object parent, 
			ref SelectorTreeResolutionContext context)
		{
			var resolvedObject = ResolveImpl(parent, ref context);
			context.AddFrame(this, resolvedObject);

			return resolvedObject;
		}

		protected abstract object ResolveImpl(
			object parent, 
			ref SelectorTreeResolutionContext context);
	}
}
