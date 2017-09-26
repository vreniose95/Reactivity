using System;
using System.Reflection;
using Core.Helpers;

namespace Reactivity.Core
{
	internal class __SR
	{
		private const string assemblyPath = "Assembly: PresentationFramework, " +
		                                    "Version=4.0.0.0, " +
		                                    "Culture=neutral, " +
		                                    "PublicKeyToken=31bf3856ad364e35";
		private const string reflTypePath = "System.Windows.SR";
		private static readonly Type ReflType_SR;
			
		static __SR()
		{
			var assembly = Assembly.Load(assemblyPath);
			ReflType_SR = assembly.GetType(reflTypePath);
			//var srGetMethodRefl = srTypeRefl.GetMethod("Get");
		}
		internal static string Get(string id)
		{
			var value = ReflType_SR.InvokeInternalStaticMethod<string>("Get", id);
			return value;
		}

		internal static string Get(string id, params object[] args)
		{
			var value = ReflType_SR.InvokeInternalStaticMethod<string>("Get", id, args);
			return value;
		}
	}
}
/*/var assembly = Assembly.Load("Assembly: System, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089");
			//var srTypeReference = assembly.GetType("System.SR");
			var assembly = Assembly.Load("Assembly: PresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35");
			var srTypeRefl = assembly.GetType("System.Windows.SR");
			var srGetMethodRefl = srTypeRefl.GetMethod("Get");
			var message = srTypeRefl.InvokeInternalStaticMethod<string>("Get", "CannotConvertType", source.GetType().FullName, typeof(PropertyPath));
			t*/