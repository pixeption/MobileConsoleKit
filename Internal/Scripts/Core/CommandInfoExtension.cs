using System;

namespace MobileConsole
{
	public static class CommandInfoExtension
	{
		public static void CopyData(this CommandInfo info, ExecutableCommandAttribute attribute, Type type)
		{
			string[] paths = null;
			if (attribute.name != null)
			{
				info.fullPath = attribute.name;
				paths = attribute.name.Trim('/').Split('/');
			}

			if (paths == null || paths.Length == 0)
			{
				info.categories = new string[0];
				info.name = type.Name.GetReadableName();
			}
			else
			{
				info.name = paths[paths.Length - 1];
				info.categories = new string[paths.Length - 1];
				if (paths.Length > 1)
				{
					Array.Copy(paths, info.categories, paths.Length - 1);
				}
			}

			info.description = attribute.description;
			info.order = attribute.order;
		}

		public static void CopyData(this CommandInfo info, SettingCommandAttribute attribute, Type type)
		{
			string[] paths = null;
			if (attribute.name != null)
			{
				info.fullPath = attribute.name;
				paths = attribute.name.Trim('/').Split('/');
			}

			if (paths == null || paths.Length == 0)
			{
				info.categories = new string[1] { type.Name.GetReadableName() };
			}
			else
			{
				info.categories = new string[paths.Length];
				Array.Copy(paths, info.categories, paths.Length);
			}

			info.description = attribute.description;
			info.order = attribute.order;
		}
	}
}