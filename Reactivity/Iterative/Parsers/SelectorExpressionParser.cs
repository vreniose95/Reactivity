using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Reactivity.Iterative.Targeting.Core;
using Reactivity.Iterative.Targeting.Macros;
using Reactivity.Iterative.Targeting.Selectors;
using Reactivity.Parsers;

namespace Reactivity.Iterative.Parsers
{
	public class SelectorExpressionParser
	{
		protected static List<SelectorMacroMapping> MacroMappings = new List<SelectorMacroMapping>
		{
			new SelectorMacroMapping("VisualChild", new []{typeof(int)}, typeof(VisualChildSelector), (values, types) =>
			{
				if (values.Length != 1)
					throw new ArgumentException("incorrect number of parameters.");

				var converter = TypeDescriptor.GetConverter(typeof (int));
				var convertedValue = converter.ConvertFrom(values[0]) as int?;
				if(!convertedValue.HasValue)
					throw new ArgumentException("converter failed to convert a the parameter to int.");

				var visualChildSelector = new VisualChildSelector {ChildIndex = convertedValue.Value};
				return visualChildSelector;
			}),
			new SelectorMacroMapping("FindName", new []{typeof(string)}, typeof(VisualChildSelector), (values, types) =>
			{
				if (values.Length != 1)
					throw new ArgumentException("incorrect number of parameters.");

				var converter = TypeDescriptor.GetConverter(typeof (string));
				var convertedValue = converter.ConvertFrom(values[0]) as string;
				if(convertedValue.IsNullOrWhiteSpace())
					throw new ArgumentException("target name cannot be null or whitespace.");

				var visualChildSelector = new FindNameInDataTemplateSelector { TargetName = convertedValue };
				return visualChildSelector;
			}),
			new SelectorMacroMapping("FindNameInItemsControl", new []{typeof(string)}, typeof(FindNameInItemsControlSelector), (values, types) =>
			{
				if (values.Length != 1)
					throw new ArgumentException("incorrect number of parameters.");

				var converter = TypeDescriptor.GetConverter(typeof (string));
				var convertedValue = converter.ConvertFrom(values[0]) as string;
				if(convertedValue.IsNullOrWhiteSpace())
					throw new ArgumentException("target name cannot be null or whitespace.");

				var visualChildSelector = new FindNameInItemsControlSelector { TargetName = convertedValue };
				return visualChildSelector;
			}),
			new SelectorMacroMapping("FindNameInListView", new []{typeof(string)}, typeof(FindNameInListViewSelector), (values, types) =>
			{
				if (values.Length != 1)
					throw new ArgumentException("incorrect number of parameters.");

				var converter = TypeDescriptor.GetConverter(typeof (string));
				var convertedValue = converter.ConvertFrom(values[0]) as string;
				if(convertedValue.IsNullOrWhiteSpace())
					throw new ArgumentException("target name cannot be null or whitespace.");

				var visualChildSelector = new FindNameInListViewSelector { TargetName = convertedValue };
				return visualChildSelector;
			}),
			new SelectorMacroMapping("FindNameInControlTemplate", new []{typeof(string)}, typeof(FindNameInItemsControlSelector), (values, types) =>
			{
				if (values.Length != 1)
					throw new ArgumentException("incorrect number of parameters.");

				var converter = TypeDescriptor.GetConverter(typeof (string));
				var convertedValue = converter.ConvertFrom(values[0]) as string;
				if(convertedValue.IsNullOrWhiteSpace())
					throw new ArgumentException("target name cannot be null or whitespace.");

				var visualChildSelector = new FindNameInControlTemplateSelector { TargetName = convertedValue };
				return visualChildSelector;
			}),new SelectorMacroMapping("this", new Type[] {}, typeof(ThisSelector), (values, types) =>
			{
				//if (values.Length != 0)
				//	throw new ArgumentException("incorrect number of parameters.");
				
				var visualChildSelector = new ThisSelector();
				return visualChildSelector;
			})
		};

		private Tokenizer _tokenizer;
		private string _expression;
		public SelectorExpressionParser(string expression)
		{
			_expression = expression;
		}
		public SelectorExpressionTree Parse()
		{
			var selectorExpressionTree = new SelectorExpressionTree();
			var selectorExpressionParts = _expression.Split('.');
			foreach (var selectorExpressionPart in selectorExpressionParts)
			{
				var currentSelectorType = SelectorType.Unknown;
				_tokenizer = new Tokenizer(selectorExpressionPart);

				_tokenizer.SkipWhiteSpace();

				char c;
				if (!_tokenizer.TryRead(out c))
					throw new FormatException("Could not read from tokenizer.");

				//if (c == '#')
				//{
				//	var frame = _tokenizer.GetFrame();

				//	if (_tokenizer.HasMore())
				//		throw new FormatException(
				//			_tokenizer.GetExceptionRangeText(frame) +
				//			$"Unexpected character(s) after root selector \'#\' character");


				//	selectorExpressionTree.AppendSelectorToTree(
				//		new RootExecutionContextSelector());
				//}
				if (c == '$')
				{
					currentSelectorType = SelectorType.MacroCall;
					var identifier = scanIdentifer();
					var frame = _tokenizer.GetFrame();
					char c2;
					if (!_tokenizer.TryRead(out c2))
						throw new FormatException(
							_tokenizer.GetExceptionRangeText(frame) +
							$"Could not read MacroCall parenthetis/parameter list");

					if (c2 != '(')
						throw new FormatException(
							_tokenizer.GetExceptionRangeText(frame) +
							$"Macro call expected a parameter list inside a matched parenthesis set. (Ex. \'...$testMacro(4)...\').");

					var unknownArguments = scanUntil(')');

					if (!_tokenizer.TryRead(out c2))
						throw new FormatException(
							_tokenizer.GetExceptionRangeText(frame) +
							$"Could not end MacroCall parenthetis/parameter list");

					if (c2 != ')')
						throw new FormatException(
							_tokenizer.GetExceptionRangeText(frame) +
							$"Macro call expected a \')\' character at the end of the argument list, (Ex. \'...$testMacro(4)...\').");

					_tokenizer.SkipWhiteSpace();
					frame = _tokenizer.GetFrame();

					if (_tokenizer.HasMore())
						throw new FormatException(
							_tokenizer.GetExceptionRangeText(frame) +
							$"Unexpected character(s) after argument list closing parenthesis character");
					//TODO will this throw on empty arg list $xxx()?
					var argumentArray = unknownArguments.Split(',').Select(i => i.Trim()).Cast<object>().ToArray();
					var macroName = identifier.LiteralValue;

					var associatedMacros = MacroMappings.Where(mapping => mapping.MacroName == macroName).ToArray();

					if (associatedMacros.Length == 0)
						throw new NotSupportedException($"Macro mapping for the name \'{macroName}\' could not be found in the mapping list.");

					if (associatedMacros.Length > 1)
						throw new NotSupportedException($"Multiple macro mappings for the name \'{macroName}\' were found in the mapping list.");

					var elementSelector = associatedMacros[0].SelectorFunc(argumentArray, new[] { typeof(int) });

					selectorExpressionTree.AppendSelectorToTree(elementSelector);
				}


				else if (c == '[')
				{
					currentSelectorType = SelectorType.ArrayElement;
					var index = scanNumericLiteral();

					var frame = _tokenizer.GetFrame();
					char c2;
					if (!_tokenizer.TryRead(out c2))
						throw new FormatException(
							_tokenizer.GetExceptionRangeText(frame) +
							$"Could not read ArrayElement index end bracket.");

					//if (c2 != ']')
					//	throw new FormatException(
					//		_tokenizer.GetExceptionRangeText(frame) +
					//		$"ArrayElement end bracket missing. Expected a zero-based integer index " +
					//		$"parameter inside a matched square bracket set. (Ex. \'.[7].\').");

					_tokenizer.SkipWhiteSpace();
					frame = _tokenizer.GetFrame();

					if (_tokenizer.HasMore())
						throw new FormatException(
							_tokenizer.GetExceptionRangeText(frame) +
							$"Unexpected character(s) after array element selector closing square bracket character.");

					var converter = TypeDescriptor.GetConverter(typeof(int));
					var convertedValue = converter.ConvertFrom(index.LiteralValue) as int?;
					if (!convertedValue.HasValue)
						throw new ArgumentException("converter failed to convert the array idex parameter to type int.");

					selectorExpressionTree.AppendSelectorToTree(
						new ArrayElementSelector { ArrayIndex = convertedValue.Value });
				}

				else if (c.IsLetter() || c == '_')
				{
					currentSelectorType = SelectorType.Property;

					_tokenizer.Step(-1);
					var identifier = scanIdentifer();

					_tokenizer.SkipWhiteSpace();
					var frame = _tokenizer.GetFrame();

					if (_tokenizer.Step(1) && _tokenizer.HasMore())
						throw new FormatException(
							_tokenizer.GetExceptionRangeText(frame) +
							$"Unexpected character(s) after property selector identifier.");

					selectorExpressionTree.AppendSelectorToTree(
						new PropertySelector { PropertyName = identifier.LiteralValue });
				}
				else
				{

				}


			}
			return selectorExpressionTree;
		}

		private IdentiferToken scanIdentifer()
		{
			var text = "";
			var isFirst = true;
			char c;
			while (_tokenizer.TryRead(out c))
			{
				if (isFirst)
				{
					isFirst = false;
					if (c.IsDigit())
						throw new FormatException(FSR.Parse.IdentifierMustStartWithLetter(c));
					if (c.IsLetter() || c == '_')
						text += c;
					else
						throw new FormatException(FSR.Parse.IdentifierMustStartWithLetter(c));
				}
				else
				{
					if (c.IsLetterOrDigit() || c == '_')
						text += c;
					else
						break;
					//return new IdentiferToken(text);
				}
			}
			if (string.IsNullOrWhiteSpace(text))
				throw new FormatException(FSR.Parse.CannotParseIdentifier());

			_tokenizer.Step(-1);

			return new IdentiferToken(text);
		}

		private string scanUntil(char stopChar, bool throwIfNotFound = true)
		{
			var frame = _tokenizer.GetFrame();
			var text = "";
			char c;
			while (_tokenizer.TryRead(out c))
			{
				if (c == stopChar)
				{
					_tokenizer.Step(-1);
					return text;
				}
				text += c;
			}
			if (throwIfNotFound)
				throw new FormatException(
					$"\"{_tokenizer.SourceText}\" (Index {frame.Index} - {_tokenizer.CurrentIndex}): " +
					$"StopChar \'{stopChar}\' was not found.");

			return text;
		}
		//TODO int/no decimal option?
		private NumberToken scanNumericLiteral()
		{
			var text = "";

			var hasDecimal = false;
			char c;
			while (_tokenizer.TryRead(out c))
			{
				if (c == '.')
				{
					if (hasDecimal)
						throw new FormatException("Numeric literal cannot contain two decimals.");
					hasDecimal = true;
					text += c;
				}
				else if (c.IsDigit())
				{
					text += c;
				}
				else if (c == ']')
				{
					break;
				}
				else if (c.IsWhiteSpace() || c == '\0')
				{
					break;
				}
				else
				{
					throw new FormatException("Invalid character in numeric literal.");
				}
			}
			return new NumberToken(text);
		}
	}
	//public class SelectorExpressionParser
	//{
	//	private readonly Tokenizer tokenizer;

	//	public SelectorExpressionParser(string expression)
	//	{
	//		tokenizer = new Tokenizer(expression);
	//	}
	//	public SelectorExpressionTree Parse()
	//	{
	//		var selectorExpressionTree = new SelectorExpressionTree();

	//		char c;
	//		if (!tokenizer.TryRead(out c))
	//			throw new FormatException("Could not read from tokenizer.");

	//		if (c == '$')
	//			throw new FormatException("Expected period accessor after target name.");

	//		if (c != '.')
	//			throw new FormatException("Expected period accessor after target name.");


	//		var targetProperty = scanIdentifer();
	//		builder.SetTargetProperty(targetProperty);
	//		tokenizer.SkipWhiteSpace();

	//		var beginTime = scanTimeSpanLiteral();
	//		builder.SetBeginTime(beginTime);
	//		tokenizer.SkipWhiteSpace();

	//		var progressionBlock = scanProgressionBlock();
	//		builder.SetProgressionBlock(progressionBlock);
	//		tokenizer.SkipWhiteSpace();

	//		var duration = scanTimeSpanLiteral();
	//		builder.SetDuration(duration);
	//		tokenizer.SkipWhiteSpace();

	//		var easingMode = scanIdentifer();
	//		tokenizer.SkipWhiteSpace();
	//		var easingFunctionType = scanIdentifer();

	//		NumberLiteralToken param1 = null;
	//		NumberLiteralToken param2 = null;

	//		tokenizer.SkipWhiteSpace();
	//		if (tokenizer.TryPeek(out c))
	//		{
	//			param1 = scanNumericLiteral();
	//			tokenizer.SkipWhiteSpace();
	//			if (tokenizer.TryPeek(out c))
	//			{
	//				param2 = scanNumericLiteral();
	//			}
	//		}
	//		return builder.CompositeAnimation;
	//	}

	//	private IdentiferToken scanIdentifer()
	//	{
	//		var text = "";
	//		var isFirst = true;
	//		char c;
	//		while (tokenizer.TryRead(out c))
	//		{
	//			if (isFirst)
	//			{
	//				isFirst = false;
	//				if (c.IsDigit())
	//					throw new FormatException(FSR.Parse.IdentifierMustStartWithLetter(c));
	//				if (c.IsLetter() || c == '_')
	//					text += c;
	//				else
	//					throw new FormatException(FSR.Parse.IdentifierMustStartWithLetter(c));
	//			}
	//			else
	//			{
	//				if (c.IsLetterOrDigit() || c == '_')
	//					text += c;
	//				else
	//					break;
	//				//return new IdentiferToken(text);
	//			}
	//		}
	//		if (string.IsNullOrWhiteSpace(text))
	//			throw new FormatException(FSR.Parse.CannotParseIdentifier());

	//		tokenizer.Step(-1);

	//		return new IdentiferToken(text);
	//	}

	//	private NumberToken scanNumericLiteral()
	//	{
	//		var text = "";

	//		var hasDecimal = false;
	//		char c;
	//		while (tokenizer.TryRead(out c))
	//		{
	//			if (c == '.')
	//			{
	//				if (hasDecimal)
	//					throw new FormatException("Numeric literal cannot contain two decimals.");
	//				hasDecimal = true;
	//				text += c;
	//			}
	//			else if (c.IsDigit())
	//			{
	//				text += c;
	//			}
	//			else if (c.IsWhiteSpace() || c == '\0')
	//			{
	//				break;
	//			}
	//			else
	//			{
	//				throw new FormatException("Invalid character in numeric literal.");
	//			}
	//		}
	//		return new NumberLiteralToken(text);
	//	}
	//}
}

