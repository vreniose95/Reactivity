using static Reactivity.React;

namespace Reactivity.Core
{
	//TODO theres logic here for deferring reaction execution until Associated, but that condition might also be handled in the implementation classes?
	public abstract class AttachableReactionBase : AttachableBase, IReaction
	{
		protected bool _deferReactionUntilAssociation;

		protected override void OnAttached()
		{
			base.OnAttached();
			if (_deferReactionUntilAssociation)
			{
				React();
				_deferReactionUntilAssociation = false;
			}
		}

		public void React()
		{
			if (!IsAssociated)
			{
				_deferReactionUntilAssociation = true;
				return;
			}
			if (!GetDisable(AssociatedObject))
			{
				ReactImpl();
			}
			else
			{
				
			}
		}

		protected abstract void ReactImpl();

		void IReaction.React() => React();
	}
}
