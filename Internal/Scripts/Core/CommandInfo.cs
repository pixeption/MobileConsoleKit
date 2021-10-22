using System.Reflection;

namespace MobileConsole
{
	public class CommandInfo
	{
		public VariableInfo[] variableInfos;
		public MethodInfo[] customButtonInfos;
		public string fullPath;
		public string[] categories;
		public string name;
		public string description;
		public int order;
		public bool shouldCloseAfterExecuted = true;

		public bool IsComplex()
		{
			return variableInfos != null && variableInfos.Length > 0;
		}
	}
}