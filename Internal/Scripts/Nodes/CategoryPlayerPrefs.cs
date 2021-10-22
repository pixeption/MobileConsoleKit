using UnityEngine;

namespace MobileConsole.UI
{
	public static class CategoryPlayerPrefs
	{
		const string KeyCategoryFormat = "MobileConsole.{0}.{1}";
		public static void LoadCategoryStates(RootNode rootNode, string prefix)
		{
			foreach (NodeView child in rootNode.children)
			{
				_LoadCategoryStates(child, prefix);
				child.UpdateChildVisibilityRecursive();
			}
		}

		static void _LoadCategoryStates(NodeView node, string prefix, string path = "")
		{
			if (node == null || !node.expandable)
			{
				return;
			}

			string fullPath = string.IsNullOrEmpty(path) ? node.name : string.Format("{0}.{1}", path, node.name);
			string key = string.Format(KeyCategoryFormat, prefix, fullPath);
			node.isExpanded = PlayerPrefs.GetInt(key, 1) == 1 ? true : false;

			foreach (NodeView child in node.children)
			{
				_LoadCategoryStates(child, prefix, fullPath);
			}
		}

		public static void SaveCategoryState(NodeView node, string prefix)
		{
			string key = string.Format(KeyCategoryFormat, prefix, GetCategoryFullPath(node));
			PlayerPrefs.SetInt(key, node.isExpanded ? 1 : 0);
		}

		static string GetCategoryFullPath(NodeView node)
		{
			string fullPath = string.Empty;
			NodeView iterNode = node;
			while (iterNode != null)
			{
				fullPath = string.Format("{0}.{1}", iterNode.name, fullPath);
				iterNode = iterNode.parent as NodeView;
			}

			return fullPath.Trim('.');
		}
	}
}