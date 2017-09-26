using System.Windows;
using System.Windows.Markup;
using Reactivity.Collections;
using Reactivity.Core;
using Reactivity.Triggers;

namespace Reactivity.Storyboarding
{
	//TODO is reinheriting IAttachedObject necessary if already inheriting attachablebase?
	[ContentProperty("Reactions")]
	[RuntimeNameProperty("Name")]
	public class ReactiveStoryboard : AttachableBase, IAttachedObject, IReactionHostTarget
	{
		private static readonly DependencyPropertyKey DynamicTriggersPropertyKey = DependencyProperty.RegisterReadOnly("DynamicTriggers",
			typeof(ReactiveTriggerCollection), typeof(ReactiveStoryboard), new FrameworkPropertyMetadata());
		public static readonly DependencyProperty DynamicTriggersProperty = DynamicTriggersPropertyKey.DependencyProperty;

		private static readonly DependencyPropertyKey ReactionsPropertyKey = DependencyProperty.RegisterReadOnly("Reactions",
			typeof(ReactionCollection), typeof(ReactiveStoryboard), new FrameworkPropertyMetadata());
		public static readonly DependencyProperty ReactionsProperty = ReactionsPropertyKey.DependencyProperty;

		public ReactiveTriggerCollection DynamicTriggers => (ReactiveTriggerCollection)GetValue(DynamicTriggersProperty);

		public ReactionCollection Reactions => (ReactionCollection)GetValue(ReactionsProperty);

		public string Name { get; set; }


		public ReactiveStoryboard()
		{
			SetValue(DynamicTriggersPropertyKey, new ReactiveTriggerCollection());
			SetValue(ReactionsPropertyKey, new ReactionCollection());
		}


		protected override void OnAttached()
		{
			base.OnAttached();
			DynamicTriggers.RegisterHost(this);
			DynamicTriggers.Attach(AssociatedObject);
			
			Reactions.Attach(AssociatedObject);
		}

		protected override void OnDetaching()
		{
			base.OnDetaching();
			DynamicTriggers.UnregisterHost();
			DynamicTriggers.Detach();

			Reactions.Detach();
		}

		void IReactionHostTarget.Execute(HostedAttachableBase source)
		{
			var sourceTrigger = source as ReactiveTriggerBase;
			if (sourceTrigger != null)
				OnDynamicTriggerFired(sourceTrigger);
		}

		protected virtual void OnDynamicTriggerFired(ReactiveTriggerBase sourceTrigger)
		{
			Reactions.React();
		}
	}
}