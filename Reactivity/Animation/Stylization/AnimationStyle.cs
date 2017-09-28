using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Markup;
using System.Windows.Threading;
using Reactivity.Iterative.Emitters;

namespace Reactivity.Animation.Stylization
{
	[Localizability(LocalizationCategory.Ignore)]
	[DictionaryKeyProperty("TargetType")]
	[ContentProperty("Setters")]
	public class AnimationStyle : DispatcherObject, INameScope, IQueryAmbient
	{
		private Type _targetType = typeof(AnimationEmitterBase);

		private AnimationStyle _basedOn = null;

		private SetterBaseCollection _setters;


		[Ambient]
		[Localizability(LocalizationCategory.NeverLocalize)]
		public Type TargetType
		{
			get { return _targetType; }
			set
			{
				if (value == null)
					throw new ArgumentNullException(nameof(value));

				if (!typeof(AnimationEmitterBase).IsAssignableFrom(value))
					throw new ArgumentException("Must be derived from \'AnimationEmitterBase\'");

				_targetType = value;
			}
		}

		[DefaultValue(null)]
		[Ambient]
		public AnimationStyle BasedOn
		{
			get { return _basedOn; }
			set
			{
				if (value == this)
					throw new ArgumentException("AnimationStyle cannot be BasedOn itself due to circular dependencies.");

				_basedOn = value;
			}
		}


		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public SetterBaseCollection Setters
		{
			get
			{
				if (_setters == null)
				{
					_setters = new SetterBaseCollection();
				}
				return _setters;
			}
		}

		bool IQueryAmbient.IsAmbientPropertyAvailable(string propertyName)
		{
			switch (propertyName)
			{
				case "BasedOn":
					if (_basedOn == null)
					{
						return false;
					}
					break;
			}
			return true;
		}

		#region INameScope 
		/// <summary>
		/// Registers the name - Context combination 
		/// </summary>
		/// <param name="name">Name to register</param>
		/// <param name="scopedElement">Element where name is defined</param>
		public void RegisterName(string name, object scopedElement)
		{
			// Verify Context Access 
			VerifyAccess();

			_nameScope.RegisterName(name, scopedElement);
		}

		/// <summary>
		/// Unregisters the name - element combination 
		/// </summary>
		/// <param name="name">Name of the element</param> 
		public void UnregisterName(string name)
		{
			// Verify Context Access 
			VerifyAccess();

			_nameScope.UnregisterName(name);
		}

		/// <summary> 
		/// Find the element given name 
		/// </summary>
		/// <param name="name">Name of the element</param> 
		object INameScope.FindName(string name)
		{
			// Verify Context Access
			VerifyAccess();

			return _nameScope.FindName(name);
		}

		private NameScope _nameScope = new NameScope();
		#endregion IIdScope
	}
}
