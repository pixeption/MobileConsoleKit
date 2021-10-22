using System;

namespace MobileConsole
{
	[AttributeUsage(AttributeTargets.Method)]
	public class ButtonAttribute : Attribute
	{
		public string icon;
		public string category;
	}
}