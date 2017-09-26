using System;
using System.Windows;

namespace Reactivity.Core
{
	//TODO (HERE AND HOST BASE, also on custom collection implementations!) should call changed event on detatched? double firing for one change? Seperate event for detached events?
	//TODO custom eventhandler type for passing new and old associated objects Eventargs
	//TODO Pick a name, attach... or associated!
	public abstract class AttachableBase : DependencyObject, IAttachedObject
	{
		public bool IsAssociated => AssociatedObject != null;
		
		public DependencyObject AssociatedObject { get; protected set; }

		DependencyObject IAttachedObject.AssociatedObject => AssociatedObject;

		public event EventHandler AssociatedObjectChanged;
		
		public void Attach(DependencyObject dependencyObject)
		{
			if (dependencyObject == AssociatedObject)
				return;
			if (AssociatedObject != null)
				throw new InvalidOperationException("AttachableBase cannot host objects multiple times.")
				{
					Data = {
						["FailedObjectAssociation"] = dependencyObject.ToString(),
						["ExistingAssociatedObject"] = AssociatedObject.ToString() 
					}
				};
			AssociatedObject = dependencyObject;
			AssociatedObjectChanged?.Invoke(this, new EventArgs());
			OnAttached();
		}

		public void Detach()
		{
			OnDetaching();
			AssociatedObject = null;
		}
		
		protected virtual void OnAttached() { }
		
		protected virtual void OnDetaching() { }
	}
}
