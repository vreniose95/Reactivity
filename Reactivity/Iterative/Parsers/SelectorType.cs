namespace Reactivity.Iterative.Parsers
{
	internal enum SelectorType
	{
		Unknown,
		RootExecution,
		MacroCall,
		ReflectedMethodCall,
		ArrayElement,
		Property
	}
}