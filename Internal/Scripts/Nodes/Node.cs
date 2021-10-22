using System;
using System.Collections.Generic;

namespace MobileConsole.UI
{
	public class Node
	{
		public string name;
		public string id;
		public object data;
		public bool isExpanded = true;
		public bool expandable = false;
		public bool isVisible = true;
		public int level = 0;
		public Node parent;
		public List<Node> children = new List<Node>();

		public void AddNode(Node node)
		{
			if (node == null)
				throw new System.Exception("Node can't be null!");

			node.parent = this;
			children.Add(node);
			node.level = level + 1;
		}

		public void RemoveFromParent()
		{
			if (parent != null)
			{
				parent.children.Remove(this);
				parent = null;
			}

			_ClearAll(this);
		}
		
		// We only need to set parent = null, cut the circular references and let GC does its job
		void _ClearAll(Node node)
		{
			foreach (var child in node.children)
			{
				child.parent = null;
				_ClearAll(child);
			}
		}

		public bool IsVisible()
		{
			if (parent == null)
			{
				return true;
			}
			else
			{
				return _IsVisible(this);
			}
		}

		bool _IsVisible(Node node)
		{
			bool isParentExpanded = true;
			if (node.parent != null)
			{
				isParentExpanded = _IsVisible(node.parent);
			}

			return node.isVisible && isParentExpanded;
		}

		public List<Node> GetBranch()
		{
			List<Node> branch = new List<Node>();
			_BuildBranch(branch, this);

			return branch;
		}

		void _BuildBranch(List<Node> branch, Node node)
		{
			branch.Insert(0, node);
			if (node.parent != null)
			{
				_BuildBranch(branch, node.parent);
			}
		}

		public List<Node> GetAllChildren()
		{
			List<Node> allChildren = new List<Node>();
			_GetAllChildren(allChildren, this);

			return allChildren;
		}

		void _GetAllChildren(List<Node> allChildren, Node node)
		{
			foreach (var child in node.children)
			{
				allChildren.Add(child);
				_GetAllChildren(allChildren, child);
			}
		}

		public void Sort(Comparison<Node> comparison, bool recursive = true)
		{
			if (recursive)
			{
				_SortRecursive(this, comparison);
			}
			else
			{
				children.Sort(comparison);
			}
		}

		public void _SortRecursive(Node node, Comparison<Node> comparison)
		{
			if (node.children.Count == 0)
				return;
			
			node.children.Sort(comparison);

			foreach (var child in node.children)
			{
				_SortRecursive(child, comparison);
			}
		}

		public bool HasChild()
		{
			return children.Count > 0;
		}

		public void ToggleExpand()
		{
			isExpanded = !isExpanded;
			UpdateChildVisibilityRecursive();
		}

		public void ExpandAllChild(bool expand = true)
		{
			_ExpandAllChild(this, expand);
			UpdateChildVisibilityRecursive();
		}

		void _ExpandAllChild(Node node, bool expand = false)
		{
			foreach (Node child in node.children)
			{
				if (child.expandable)
				{
					child.isExpanded = expand;
					_ExpandAllChild(child, expand);
				}
			}
		}

		public void UpdateChildVisibilityRecursive()
		{
			_UpdateChildVisibilityRecursive(this);
		}

		void _UpdateChildVisibilityRecursive(Node parent)
		{
			foreach (var child in parent.children)
			{
				child.isVisible = parent.isExpanded && parent.isVisible;
				_UpdateChildVisibilityRecursive(child);
			}
		}

		public Node FindChildByName(string name)
		{
			return children.Find(node => node.name == name);
		}

		public Node FindChildById(string id)
		{
			return children.Find(node => node.id == id);
		}
	}
}