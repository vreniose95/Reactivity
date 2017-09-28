using System;
using System.Windows;
using System.Windows.Markup;
using Core.Extensions;
using Core.Helpers.DependencyHelpers;
using Core.Markup.TypeConverterInjection;
using Reactivity.Iterative.Emitters;

namespace Reactivity.Animation
{
	[XamlSetMarkupExtension(nameof(ReceiveMarkupExtension))]
	[XamlSetTypeConverter(nameof(ReceiveTypeConverter))]
	public class IterativeOffset : Freezable
	{
		public static readonly DependencyProperty OffsetProperty = DP.Register(
			new Meta<IterativeOffset, TimeSpan>());

		public static readonly DependencyProperty IterativeOffsetModeProperty = DP.Register(
			new Meta<IterativeOffset, IterativeOffsetMode>());

		public static readonly DependencyProperty MaximumEffectiveOffsetProperty = DP.Register(
			new Meta<IterativeOffset, TimeSpan?>(null));


		//[InjectTypeConverter(typeof(CustomTimeSpanConverter))]
		public TimeSpan Offset
		{
			get { return (TimeSpan)GetValue(OffsetProperty); }
			set { SetValue(OffsetProperty, value); }
		}
		public IterativeOffsetMode IterativeOffsetMode
		{
			get { return (IterativeOffsetMode)GetValue(IterativeOffsetModeProperty); }
			set { SetValue(IterativeOffsetModeProperty, value); }
		}
		//[InjectTypeConverter(typeof(CustomTimeSpanConverter))]
		public TimeSpan? MaximumEffectiveOffset
		{
			get { return (TimeSpan?)GetValue(MaximumEffectiveOffsetProperty); }
			set { SetValue(MaximumEffectiveOffsetProperty, value); }
		}


		public TimeSpan GetStepOffset(int totalSteps)
		{
			var effectiveOffset = IterativeOffsetMode == IterativeOffsetMode.Step ? Offset : Offset.DividedBy(totalSteps);

			if (MaximumEffectiveOffset.HasValue)
			{
				if (effectiveOffset > MaximumEffectiveOffset.Value)
				{
					effectiveOffset = MaximumEffectiveOffset.Value;
				}
			}
			return effectiveOffset;
		}


		public static void ReceiveMarkupExtension(object targetObject, XamlSetMarkupExtensionEventArgs eventArgs)
		{
			//if (eventArgs.Value.GetType().Name == "TypeConverterMarkupExtension")
			//{
			//	eventArgs.Value.SetFieldValue("_converter", new CustomTimeSpanConverter());
			//}

			TypeConverterInjectionCore.HandleMarkupExtensionTypeConverterInject(targetObject, eventArgs);

			//if (eventArgs.Handled)
			//	return;
		}

		public static void ReceiveTypeConverter(object targetObject, XamlSetTypeConverterEventArgs eventArgs)
		{
			TypeConverterInjectionCore.HandlePropertySet(targetObject, eventArgs);
		}

		protected override Freezable CreateInstanceCore()
		{
			return new IterativeOffset();
		}
	}
}
