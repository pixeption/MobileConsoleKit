using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace MobileConsole.UI
{
	public class RootNode : Node
	{
		List<Node> _flattenChilds = new List<Node>();
		List<Node> _flattenVisibleChilds = new List<Node>();

		public RootNode()
		{
			level = -1;
		}

		public ReadOnlyCollection<Node> FlattenChilds()
		{
			return _flattenChilds.AsReadOnly();
		}

		public ReadOnlyCollection<Node> FlattenedVisibleChilds()
		{
			return _flattenVisibleChilds.AsReadOnly();
		}

		public void RebuildFlattenChilds()
		{
			_flattenChilds.Clear();
			_RebuildFlattenChilds(this, true);
		}

		void _RebuildFlattenChilds(Node node, bool ignoreSelf = false)
		{
			if (!ignoreSelf)
			{
				_flattenChilds.Add(node);
			}

			foreach (var child in node.children)
			{
				_RebuildFlattenChilds(child);
			}
		}

		public void RebuildFlattenVisibleChilds()
		{
			_flattenVisibleChilds.Clear();
			foreach (var child in _flattenChilds)
			{
				if (child.isVisible)
				{
					_flattenVisibleChilds.Add(child);
				}
			}
		}

		void _RebuildFlattenVisibleChilds(Node node, bool ignoreSelf = false)
		{
			if (!ignoreSelf)
			{
				_flattenVisibleChilds.Add(node);
			}

			if (node.isExpanded)
			{
				foreach (var child in node.children)
				{
					_RebuildFlattenVisibleChilds(child);
				}
			}
		}

		public int VisibleCellCount()
		{
			return _flattenVisibleChilds.Count;
		}

		public Node VisibleNodeAtIndex(int index)
		{
			if (index < 0 || index >= _flattenVisibleChilds.Count)
			{
				return null;
			}

			return _flattenVisibleChilds[index];
		}
	}
}