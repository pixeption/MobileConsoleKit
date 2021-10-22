using UnityEngine;

namespace MobileConsole.UI
{
	public static class NodeColor
	{
		public static void AdjustIconColor(Node node)
		{
			foreach (NodeView child in node.children)
			{
				if (child == null || !child.expandable)
					continue;

				Color color = TextColors.GetUniqueColor(child.name, 0.4f, 1.0f);
				_SetNodeIconColorRecursive(child as GenericNodeView, color);
			}
		}

		static void _SetNodeIconColorRecursive(GenericNodeView node, Color color)
		{
			if (node == null)
				return;

			node.iconColor = color;
			foreach (var child in node.children)
			{
				_SetNodeIconColorRecursive(child as GenericNodeView, color);
			}
		}
	}
}