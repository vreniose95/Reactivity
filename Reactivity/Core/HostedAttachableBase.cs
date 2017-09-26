using System;
using System.Diagnostics;

namespace Reactivity.Core
{
	public abstract class HostedAttachableBase : AttachableBase, IHostedObject
	{
		public bool IsHosted => HostObject != null;

		public IReactionHostTarget HostObject { get; protected set; }

		IReactionHostTarget IHostedObject.HostObject => HostObject;

		public event EventHandler HostObjectChanged;
		
		//[TraceAspect]
		public void RegisterHost(IReactionHostTarget reactionHostTarget)
		{ 
			//Logger.EnterMethod();
			if (reactionHostTarget == HostObject)
			{
				Debug.Print("IReactionHostTarget object already registered.");
				return;
			}
			if (IsHosted)
				throw new InvalidOperationException("HostedAttachableBase cannot register multiple hosts.")
				{
					Data = {
						["FailedObjectHosting"] = reactionHostTarget.ToString(),
						["ExistingHostObject"] = HostObject.ToString()
					}
				};
			HostObject = reactionHostTarget;
			HostObjectChanged?.Invoke(this, new EventArgs());
			OnHostRegistered();
		}
		
		public void UnregisterHost()
		{
			OnHostUnregistering();
			HostObject = null;
		}
		
		public virtual void OnHostRegistered() { }
		
		public virtual void OnHostUnregistering() { }
	}
}
