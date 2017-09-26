using System;

namespace Reactivity.Core
{
	public interface IHostedObject
	{
		bool IsHosted { get; }

		IReactionHostTarget HostObject { get; }

		event EventHandler HostObjectChanged;

		void RegisterHost(IReactionHostTarget reactionHostTarget);

    void UnregisterHost();

		void OnHostRegistered();

		void OnHostUnregistering();
	}
}
