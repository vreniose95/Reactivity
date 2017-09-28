using System;
using System.Windows;
using System.Windows.Data;
using System.Windows.Markup;
using System.Windows.Media.Animation;
using Ccr.Core.Extensions;
using Ccr.PresentationCore.Helpers.DependencyHelpers;
using Core.Extensions;
using Core.Helpers;
using Core.Helpers.DependencyHelpers;
using Reactivity.Animation;

namespace Reactivity.Iterative.Emitters
{
	[XamlSetMarkupExtension(nameof(ReceiveMarkupExtension))]
	[XamlSetTypeConverter(nameof(ReceiveTypeConverter))]
	public class DoublePlanarAnimationEmitter
		: AnimationEmitterBase<double>
	{
		private bool _deferMarkupExtensionResolve;
		private object _targetObject;
		private XamlSetMarkupExtensionEventArgs _eventArgs;


		public static readonly DependencyProperty PlanarGroupCountProperty = DP.Register(
			new Meta<DoublePlanarAnimationEmitter, int>(4));

		public static readonly DependencyProperty PlanarAxisIterativeOffsetProperty = DP.Register(
			new Meta<DoublePlanarAnimationEmitter, IterativeOffset>(new IterativeOffset()));


		public int PlanarGroupCount
		{
			get => (int)GetValue(PlanarGroupCountProperty);
			set => SetValue(PlanarGroupCountProperty, value);
		}
		public IterativeOffset PlanarAxisIterativeOffset
		{
			get => (IterativeOffset)GetValue(PlanarAxisIterativeOffsetProperty);
			set => SetValue(PlanarAxisIterativeOffsetProperty, value);
		}


		public override AnimationTimeline Emit(int index, int totalSteps, double? currentValue)
		{
			var planarVerticalSteps = getEffectiveVerticalAxisSteps(totalSteps);
			var planarHorizontalSteps = getEffectiveHorizontalAxisSteps(totalSteps);

			var horizontalStepOffset = IterativeOffset.GetStepOffset(planarHorizontalSteps);
			var verticalStepOffset = PlanarAxisIterativeOffset.GetStepOffset(planarVerticalSteps);


			var position = getPositionInPlanarMatrix(totalSteps, index);

			//if (OffsetBeginTime)
			//{
			//	//currentOffset += stepOffset;
			//}

			var effectiveFromValue = From.GetEffectiveValue(currentValue);
			var effectiveToValue = To.GetEffectiveValue(currentValue);


			var horizontalAxisSteps = getEffectiveHorizontalAxisSteps(totalSteps);
			var verticalAxisSteps = getEffectiveVerticalAxisSteps(totalSteps);

			var horizontalAxisProgression = position.X.Map(0d, horizontalAxisSteps, 0d, 1d);
			var verticalAxisProgression = position.Y.Map(0d, verticalAxisSteps, 0d, 1d);

			var horizontalOffset = position.X * horizontalAxisSteps;
			var verticalOffset = position.Y * verticalAxisSteps;

			var currentHorizontalOffset = horizontalStepOffset.MultipliedBy((int)position.X);
			var currentVerticalOffset = verticalStepOffset.MultipliedBy((int)position.Y);

			var currentOffset = BeginTime + currentHorizontalOffset + currentVerticalOffset;


			return new DoubleAnimation
			{
				Duration = Duration,
				From = effectiveFromValue,
				To = effectiveToValue,
				BeginTime = currentOffset,
				EasingFunction = EasingFunction,
				SpeedRatio = SpeedRatio
			};
		}

		protected int getEffectiveHorizontalAxisSteps(
			int totalSteps)
		{
			return totalSteps < PlanarGroupCount
				? totalSteps
				: PlanarGroupCount;
		}

		protected int getEffectiveVerticalAxisSteps(
			int totalSteps)
		{
			var rowSteps = (int)Math.Ceiling(
				(double)totalSteps / PlanarGroupCount);
			return rowSteps;
		}

		protected Vector getPositionInPlanarMatrix(
			int totalSteps,
			int currentStep)
		{
			var horizontalAxisSteps = getEffectiveHorizontalAxisSteps(totalSteps);
			var verticalAxisSteps = getEffectiveVerticalAxisSteps(totalSteps);

			var verticalRowNumber = (int)Math.Floor(
				(double)currentStep / horizontalAxisSteps);
			var horizontalColumnNumber = currentStep - 
				verticalRowNumber * PlanarGroupCount;

			return new Vector(
				verticalRowNumber, 
				horizontalColumnNumber);
		}


		protected override void OnAttached()
		{
			base.OnAttached();

			if (_deferMarkupExtensionResolve)
				ReceiveMarkupExtension(_targetObject, _eventArgs);
		}


		public static void ReceiveMarkupExtension(
			object targetObject, 
			XamlSetMarkupExtensionEventArgs eventArgs)
		{
			targetObject.IsNotNull(nameof(targetObject));
			eventArgs.IsNotNull(nameof(eventArgs));

			var planarAnimationEmitter = targetObject as DoublePlanarAnimationEmitter;
			if (planarAnimationEmitter == null)
				return;
			
			switch (eventArgs.Member.Name)
			{
				case nameof(PlanarGroupCount):
					if (!planarAnimationEmitter.IsAssociated)
					{
						planarAnimationEmitter._targetObject = targetObject;
						planarAnimationEmitter._eventArgs = eventArgs;
						planarAnimationEmitter._deferMarkupExtensionResolve = true;
						return;
					}
					planarAnimationEmitter._targetObject = null;
					planarAnimationEmitter._eventArgs = null;
					planarAnimationEmitter._deferMarkupExtensionResolve = false;

					var markupExtension = eventArgs.MarkupExtension;
					if (markupExtension is Binding)
					{
						var binding = markupExtension as Binding;
						var relativeSource = binding.RelativeSource;
						if (relativeSource != null)
						{
							if (relativeSource.Mode == RelativeSourceMode.Self)
							{
								var frameworkElement = planarAnimationEmitter.AssociatedObject as FrameworkElement;
								if (frameworkElement == null)
									throw new NotSupportedException();

								var adjustedBinding = new Binding
								{
									Source = frameworkElement,
									Path = binding.Path
								};
								BindingOperations.SetBinding(planarAnimationEmitter, PlanarGroupCountProperty, adjustedBinding);
								eventArgs.Handled = true;
							}
							else if (relativeSource.Mode == RelativeSourceMode.FindAncestor)
							{
								var frameworkElement = planarAnimationEmitter.AssociatedObject as FrameworkElement;
								if (frameworkElement == null)
									throw new NotSupportedException();

								DependencyObject resolvedVisualAncestor = null;

								if (frameworkElement.GetType() == relativeSource.AncestorType)
								{
									if (relativeSource.AncestorLevel >= 1)
									{
										resolvedVisualAncestor = frameworkElement;
									}
									else
									{
										relativeSource.AncestorLevel = relativeSource.AncestorLevel - 1;
									}
								}
								if (resolvedVisualAncestor == null)
								{
									resolvedVisualAncestor = frameworkElement.FindParent(relativeSource.AncestorType, relativeSource.AncestorLevel);
								}

								var adjustedBinding = new Binding
								{
									Source = resolvedVisualAncestor,
									Path = binding.Path
								};
								BindingOperations.SetBinding(planarAnimationEmitter, PlanarGroupCountProperty, adjustedBinding);
								eventArgs.Handled = true;
							}
						}
					}
					else
					{
						return;
						//	throw new NotImplementedException();
					}


					break;

				default:
					return;
			}
		}
	}
}
