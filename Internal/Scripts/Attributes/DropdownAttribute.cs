using System;

namespace MobileConsole
{
	[AttributeUsage(AttributeTargets.Field)]
	public class DropdownAttribute : Attribute
	{
		public string methodName;

		public DropdownAttribute(string methodName)
		{
			this.methodName = methodName;
		}
	}
}