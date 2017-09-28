using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Markup;
using System.Windows.Media.Animation;
using Ccr.PresentationCore.Helpers.DependencyHelpers;
using Core.Helpers;
using Core.Helpers.DependencyHelpers;
using Reactivity.Iterative.Targeting.Selectors;

namespace Reactivity.Iterative.Targeting.Core
{
	[TypeConverter(typeof(SelectorExpressionTreeConverter))]
	[ContentProperty("Selector")]
	public class SelectorExpressionTree : DependencyObject
	{
		public static readonly DependencyProperty SelectorProperty = DP.Register(
				new Meta<SelectorExpressionTree, ElementSelectorBase>());
		public ElementSelectorBase Selector
		{
			get { return (ElementSelectorBase)GetValue(SelectorProperty); }
			set { SetValue(SelectorProperty, value); }
		}

		public void AppendSelectorToTree(ElementSelectorBase selector)
		{
			if (Selector == null)
			{
				Selector = selector;
				return;
			}
			var currentSelector = Selector;
			while (currentSelector.NextSelector != null)
			{
				currentSelector = currentSelector.NextSelector;
			}
			currentSelector.NextSelector = selector;
		}
		//TODO implement other selectors like array accessors?

		internal IAnimatable ResolveSelectorTree(DependencyObject dependencyObject, DependencyObject rootExecutionContext)
		{
			var selectorTreeResolutionContext = new SelectorTreeResolutionContext(rootExecutionContext);
			var resolvedObject = Selector.Resolve(dependencyObject, ref selectorTreeResolutionContext);
			var currentSelector = Selector.NextSelector;
			while (currentSelector != null)
			{
				resolvedObject = currentSelector.Resolve(resolvedObject, ref selectorTreeResolutionContext);
				currentSelector = currentSelector.NextSelector;
			}
			return (IAnimatable)resolvedObject;
		}

		internal void UpdateTargetedParentReference(
			DependencyObject dependencyObject, 
			DependencyObject rootExecutionContext,
			ref object newValue)
		{
			var selectorTreeResolutionContext = new SelectorTreeResolutionContext(rootExecutionContext);
			var resolvedObject = Selector.Resolve(dependencyObject, ref selectorTreeResolutionContext);
			var currentSelector = Selector.NextSelector;
			while (currentSelector != null)
			{
				resolvedObject = currentSelector.Resolve(resolvedObject, ref selectorTreeResolutionContext);
				currentSelector = currentSelector.NextSelector;
			}
			var targetedRefExecutionFrame = selectorTreeResolutionContext.GetLastFrame();
			var parentExecutionFrame = selectorTreeResolutionContext.GetBacktracedFrame(1);

			//the itemscontrol
			var resolvedParentElement = parentExecutionFrame.ResolvedValue;

			var targetedElementPropertySelector = targetedRefExecutionFrame.SelectorInstance as PropertySelector;
			//var targetedElementArraySelector = targetedRefExecutionFrame.SelectorInstance as ArrayElementSelector;

			if (targetedElementPropertySelector == null)
				throw new NotSupportedException();

			var targetedElementPropertyName = targetedElementPropertySelector.PropertyName;

			resolvedParentElement.SetPropertyValue(targetedElementPropertyName, newValue);
		}
	}
}
/*

			private string _path;
		private PathParameterCollection _parameters;


		public SelectorExpressionTree()
		{
		}
		//public SelectorExpressionTree(object parameter) : this("(0)", parameter)
		//{
		//}
		//internal SelectorExpressionTree(string path, ITypeDescriptorContext typeDescriptorContext)
		//{
		//	_path = path;
			
		//	//PrepareSourceValueInfo(typeDescriptorContext);
		//	//	NormalizePath();
		//}

		private class PathParameterCollection : ObservableCollection<object>
		{
			public PathParameterCollection()
			{
			}

			public PathParameterCollection(object[] parameters)
			{
				IList<object> items = Items;
				foreach (object obj in parameters)
					items.Add(obj);
			}
		}

			//public SelectorExpressionTree(string path, params object[] pathParameters)
		//{
		//	if (System.Windows.Threading.Dispatcher.CurrentDispatcher == null)
		//		throw new InvalidOperationException();
		//	_path = path;
		//	if (pathParameters != null && pathParameters.Length != 0)
		//		SetPathParameterCollection(new PathParameterCollection(pathParameters));
		////	PrepareSourceValueInfo((ITypeDescriptorContext)null);
		//}


		//internal static bool IsStaticProperty(object accessor)
		//{
		//	DependencyProperty dp;
		//	PropertyInfo pi;
		//	PropertyDescriptor pd;
		//	DynamicObjectAccessor doa;
		//	PropertyPath.DowncastAccessor(accessor, out dp, out pi, out pd, out doa);
		//	if (!(pi != (PropertyInfo)null))
		//		return false;
		//	MethodInfo getMethod = pi.GetGetMethod();
		//	if (getMethod != (MethodInfo)null)
		//		return getMethod.IsStatic;
		//	return false;
		//}

		//internal static void DowncastAccessor(object accessor, out DependencyProperty dp, out PropertyInfo pi, out PropertyDescriptor pd, out DynamicObjectAccessor doa)
		//{
		//	if ((dp = accessor as DependencyProperty) != null)
		//	{
		//		pd = (PropertyDescriptor)null;
		//		pi = (PropertyInfo)null;
		//		doa = (DynamicObjectAccessor)null;
		//	}
		//	else if ((pi = accessor as PropertyInfo) != (PropertyInfo)null)
		//	{
		//		pd = (PropertyDescriptor)null;
		//		doa = (DynamicObjectAccessor)null;
		//	}
		//	else if ((pd = accessor as PropertyDescriptor) != null)
		//		doa = (DynamicObjectAccessor)null;
		//	else
		//		doa = accessor as DynamicObjectAccessor;
		//}

		//internal IDisposable SetContext(object rootItem)
		//{
		//	return this.SingleWorker.SetContext(rootItem);
		//}

		//internal object GetItem(int k)
		//{
		//	return this.SingleWorker.GetItem(k);
		//}

		//internal object GetAccessor(int k)
		//{
		//	return this._earlyBoundPathParts[k] ?? this.SingleWorker.GetAccessor(k);
		//}

		//internal object[] GetIndexerArguments(int k)
		//{
		//	return this.SingleWorker.GetIndexerArguments(k);
		//}

		//internal object GetValue()
		//{
		//	return this.SingleWorker.RawValue();
		//}

		//internal int ComputeUnresolvedAttachedPropertiesInPath()
		//{
		//	int num = 0;
		//	for (int index = this.Length - 1; index >= 0; --index)
		//	{
		//		if (this._earlyBoundPathParts[index] == null)
		//		{
		//			string name = this._arySVI[index].name;
		//			if (PropertyPath.IsPropertyReference(name) && name.IndexOf('.') >= 0)
		//				++num;
		//		}
		//	}
		//	return num;
		//}

		//internal object ResolvePropertyName(int level, object item, Type ownerType, object context)
		//{
		//	return this._earlyBoundPathParts[level] ?? this.ResolvePropertyName(this._arySVI[level].name, item, ownerType, context, false);
		//}

		//internal IndexerParameterInfo[] ResolveIndexerParams(int level, object context)
		//{
		//	return this._earlyBoundPathParts[level] as IndexerParameterInfo[] ?? this.ResolveIndexerParams(this._arySVI[level].paramList, context, false);
		//}

		//internal void ReplaceIndexerByProperty(int level, string name)
		//{
		//	this._arySVI[level].name = name;
		//	this._arySVI[level].propertyName = name;
		//	this._arySVI[level].type = SourceValueType.Property;
		//	this._earlyBoundPathParts[level] = (object)null;
		//}

		//private void PrepareSourceValueInfo(ITypeDescriptorContext typeDescriptorContext)
		//{
		//	PathParser pathParser = DataBindEngine.CurrentDataBindEngine.PathParser;
		//	this._arySVI = pathParser.Parse(this.Path);
		//	if (this._arySVI.Length == 0)
		//		throw new InvalidOperationException(System.Windows.SR.Get("PropertyPathSyntaxError", (object)(pathParser.Error ?? this.Path)));
		//	this.ResolvePathParts(typeDescriptorContext);
		//}

		//private void NormalizePath()
		//{
		//	StringBuilder stringBuilder = new StringBuilder();
		//	PropertyPath.PathParameterCollection parameters = new PropertyPath.PathParameterCollection();
		//	for (int index1 = 0; index1 < this._arySVI.Length; ++index1)
		//	{
		//		switch (this._arySVI[index1].drillIn)
		//		{
		//			case DrillIn.Never:
		//				if (this._arySVI[index1].type == SourceValueType.Property)
		//				{
		//					stringBuilder.Append('.');
		//					break;
		//				}
		//				break;
		//			case DrillIn.Always:
		//				stringBuilder.Append('/');
		//				break;
		//		}
		//		switch (this._arySVI[index1].type)
		//		{
		//			case SourceValueType.Property:
		//				if (this._earlyBoundPathParts[index1] != null)
		//				{
		//					stringBuilder.Append('(');
		//					stringBuilder.Append(parameters.Count.ToString((IFormatProvider)TypeConverterHelper.InvariantEnglishUS.NumberFormat));
		//					stringBuilder.Append(')');
		//					parameters.Add(this._earlyBoundPathParts[index1]);
		//					break;
		//				}
		//				stringBuilder.Append(this._arySVI[index1].name);
		//				break;
		//			case SourceValueType.Indexer:
		//				stringBuilder.Append('[');
		//				if (this._earlyBoundPathParts[index1] != null)
		//				{
		//					IndexerParameterInfo[] indexerParameterInfoArray = (IndexerParameterInfo[])this._earlyBoundPathParts[index1];
		//					int index2 = 0;
		//					while (true)
		//					{
		//						IndexerParameterInfo indexerParameterInfo = indexerParameterInfoArray[index2];
		//						if (indexerParameterInfo.type != (Type)null)
		//						{
		//							stringBuilder.Append('(');
		//							stringBuilder.Append(parameters.Count.ToString((IFormatProvider)TypeConverterHelper.InvariantEnglishUS.NumberFormat));
		//							stringBuilder.Append(')');
		//							parameters.Add(indexerParameterInfo.value);
		//						}
		//						else
		//							stringBuilder.Append(indexerParameterInfo.value);
		//						++index2;
		//						if (index2 < indexerParameterInfoArray.Length)
		//							stringBuilder.Append(',');
		//						else
		//							break;
		//					}
		//				}
		//				else
		//					stringBuilder.Append(this._arySVI[index1].name);
		//				stringBuilder.Append(']');
		//				break;
		//		}
		//	}
		//	if (parameters.Count <= 0)
		//		return;
		//	this._path = stringBuilder.ToString();
		//	this.SetPathParameterCollection(parameters);
		//}
		private void SetPathParameterCollection(PathParameterCollection parameters)
		{
			if (this._parameters != null)
				this._parameters.CollectionChanged -= new NotifyCollectionChangedEventHandler(this.ParameterCollectionChanged);
			this._parameters = parameters;
			if (this._parameters == null)
				return;
			this._parameters.CollectionChanged += new NotifyCollectionChangedEventHandler(this.ParameterCollectionChanged);
		}
		private void ParameterCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
    {
     //TODO this.PrepareSourceValueInfo((ITypeDescriptorContext) null);
    }

*/