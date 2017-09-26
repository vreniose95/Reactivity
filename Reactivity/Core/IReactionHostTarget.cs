namespace Reactivity.Core
{
	public interface IReactionHostTarget
	{
		void Execute(HostedAttachableBase source);
	}
}
