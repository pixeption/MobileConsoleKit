using System;

namespace MobileConsole
{
	[AttributeUsage(AttributeTargets.Field)]
	public class VariableAttribute : Attribute
	{
		public string OnValueChanged;
	}
}