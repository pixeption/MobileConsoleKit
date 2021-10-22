using UnityEngine;

namespace MobileConsole.UI
{
	public class ViewBuilder
	{
		public enum UpdateUIType
		{
			DataChanged,
			CellVisibleChanged,
			TreeChanged
		}

		const string IconCategory = "category";
		const string IconCategoryClosed = "category_close";
		const string IconAction = "action";

		public delegate void Callback();
		public event Callback OnShow;
		public event Callback OnHide;

		public delegate void UpdateUICallback(UpdateUIType type);
		public UpdateUICallback OnRequireUpdateUI;

		protected RootNode _rootNode = new RootNode();

		public int level;
		public string title;
		public Callback actionButtonCallback;
		public string actionButtonIcon;
		public bool closeAllSubViewOnAction = true;
		public bool saveScrollViewPosition = true;
		public Vector2 scrollViewPosition = new Vector2(0f, 1f);
		public string filterString = string.Empty;

		public virtual void OnPrepareToShow()
		{
			if (OnShow != null)
			{
				OnShow();
			}
		}

		public virtual void OnPrepareToHide()
		{
			if (OnHide != null)
			{
				OnHide();
			}
		}

		public RootNode GetRootNode()
		{
			return _rootNode;
		}

		protected CategoryNodeView CreateCategories(string path, Node parentNode = null)
		{
			if (parentNode == null)
				parentNode = _rootNode;

			string[] paths = path.Trim('/').Split('/');
			if (paths == null || paths.Length == 0)
			{
				return CreateCategory(path, IconCategory, IconCategoryClosed, string.Empty, parentNode);
			}
			else
			{
				Node currentNode = parentNode;
				foreach (var name in paths)
				{
					currentNode = CreateCategory(name, IconCategory, IconCategoryClosed, string.Empty, currentNode);
				}

				if (currentNode == _rootNode)
					return null;

				return (CategoryNodeView)currentNode;
			}
		}

		protected CategoryNodeView CreateCategories(string[] paths, Node parentNode = null)
		{
			if (paths == null || paths.Length == 0)
				return null;

			if (parentNode == null)
				parentNode = _rootNode;

			Node currentNode = parentNode;
			foreach (var name in paths)
			{
				currentNode = CreateCategory(name, IconCategory, IconCategoryClosed, string.Empty, currentNode);
			}

			return (CategoryNodeView)currentNode;
		}

		protected CategoryNodeView CreateCategory(string name)
		{
			return CreateCategory(name, IconCategory, IconCategoryClosed, string.Empty, _rootNode);
		}

		protected CategoryNodeView CreateCategory(string name, string icon)
		{
			return CreateCategory(name, icon, string.Empty, string.Empty, _rootNode);
		}

		protected CategoryNodeView CreateCategory(string name, string icon, string iconClose, string id, Node parentNode)
		{
			CategoryNodeView node = (CategoryNodeView)parentNode.FindChildByName(name);
			if (node == null)
			{
				node = new CategoryNodeView();
				node.name = name;
				node.icon = icon;
				node.iconClose = iconClose;
				node.id = id;
				node.expandable = true;
				node.callback += OnCategoryToggled;
				parentNode.AddNode(node);
			}
			return node;
		}

		protected GenericNodeView AddButton(string name, GenericNodeView.Callback callback)
		{
			return AddButton(name, string.Empty, callback, string.Empty, _rootNode);
		}

		protected GenericNodeView AddButton(string name, string icon, GenericNodeView.Callback callback)
		{
			return AddButton(name, icon, callback, string.Empty, _rootNode);
		}

		protected GenericNodeView AddButton(string name, GenericNodeView.Callback callback, Node parentNode)
		{
			return AddButton(name, string.Empty, callback, string.Empty, parentNode);
		}

		protected GenericNodeView AddButton(string name, string icon, GenericNodeView.Callback callback, Node parentNode)
		{
			return AddButton(name, icon, callback, string.Empty, parentNode);
		}

		protected GenericNodeView AddButton(string name, string icon, GenericNodeView.Callback callback, string id, Node parentNode)
		{
			if (parentNode == null)
				parentNode = _rootNode;

			GenericNodeView node = new GenericNodeView();
			node.id = id;
			node.name = name;
			node.icon = icon;
			node.callback = callback;
			parentNode.AddNode(node);
			return node;
		}

		protected void AddCommandFields(Command command, Node parentNode)
		{
			if (parentNode == null)
				parentNode = _rootNode;

			foreach (var variableInfo in command.info.variableInfos)
			{
				FieldNodeView node = new FieldNodeView(command, variableInfo);
				node.name = variableInfo.fieldInfo.Name.GetReadableName();
				parentNode.AddNode(node);
			}
		}

		protected void AddCommandField(Command command, string fieldName, string displayName)
		{
			AddCommandField(command, fieldName, displayName, string.Empty, _rootNode);
		}

		protected void AddCommandField(Command command, string fieldName, string displayName, Node parentNode)
		{
			AddCommandField(command, fieldName, displayName, string.Empty, parentNode);
		}

		protected void AddCommandField(Command command, string fieldName, string displayName, string id, Node parentNode)
		{
			if (parentNode == null)
				parentNode = _rootNode;

			foreach (var variableInfo in command.info.variableInfos)
			{
				if (variableInfo.fieldInfo.Name == fieldName)
				{
					FieldNodeView node = new FieldNodeView(command, variableInfo);
					node.name = displayName;
					node.id = id;
					parentNode.AddNode(node);
					break;
				}
			}
		}

		protected CheckboxNodeView AddCheckbox(string name, bool isOn, CheckboxNodeView.Callback callback, Node parentNode)
		{
			return AddCheckbox(name, isOn, callback, string.Empty, parentNode);
		}

		protected CheckboxNodeView AddCheckbox(string name, bool isOn, CheckboxNodeView.Callback callback, string id, Node parentNode)
		{
			if (parentNode == null)
				parentNode = _rootNode;

			CheckboxNodeView node = new CheckboxNodeView();
			node.name = name;
			node.isOn = isOn;
			node.id = id;
			node.callback = callback;
			parentNode.AddNode(node);
			return node;
		}

		protected InputNodeView AddInput(string name, string value, InputNodeView.Callback callback)
		{
			return AddInput(name, value, false, callback, string.Empty, _rootNode);
		}

		protected InputNodeView AddInput(string name, string value, InputNodeView.Callback callback, Node parentNode)
		{
			return AddInput(name, value, false, callback, string.Empty, parentNode);
		}

		protected InputNodeView AddInput(string name, string value, bool isNumeric, InputNodeView.Callback callback, string id, Node parentNode)
		{
			if (parentNode == null)
				parentNode = _rootNode;

			InputNodeView node = new InputNodeView();
			node.name = name;
			node.value = value;
			node.isNumeric = isNumeric;
			node.callback = callback;
			node.id = id;
			parentNode.AddNode(node);
			return node;
		}

		protected DropdownNodeView AddDropdown(string name, int index, string[] options, DropdownNodeView.Callback callback, Node parentNode)
		{
			return AddDropdown(name, index, options, callback, string.Empty, parentNode);
		}

		protected DropdownNodeView AddDropdown(string name, int index, string[] options, DropdownNodeView.Callback callback, string id, Node parentNode)
		{
			if (parentNode == null)
				parentNode = _rootNode;

			DropdownNodeView node = new DropdownNodeView();
			node.name = name;
			node.callback = callback;
			node.id = id;
			node.options = options;
			node.index = index;
			parentNode.AddNode(node);
			return node;
		}

		protected SliderNodeView AddSlider(string name, float value, float min, float max, SliderNodeView.Callback callback)
		{
			return AddSlider(name, value, min, max, false, callback, string.Empty, _rootNode);
		}

		protected SliderNodeView AddSlider(string name, float value, float min, float max, SliderNodeView.Callback callback, Node parentNode)
		{
			return AddSlider(name, value, min, max, false, callback, string.Empty, parentNode);
		}

		protected SliderNodeView AddSlider(string name, float value, float min, float max, bool wholeNumbers, SliderNodeView.Callback callback, string id, Node parentNode)
		{
			if (parentNode == null)
				parentNode = _rootNode;

			SliderNodeView node = new SliderNodeView();
			node.name = name;
			node.callback = callback;
			node.id = id;
			node.min = min;
			node.max = max;
			node.value = value;
			node.wholeNumbers = wholeNumbers;
			parentNode.AddNode(node);
			return node;
		}

		protected GenericNodeView AddResizableText(string text)
		{
			return AddResizableText(text, string.Empty, string.Empty, _rootNode);
		}

		protected GenericNodeView AddResizableText(string text, string icon)
		{
			return AddResizableText(text, icon, string.Empty, _rootNode);
		}

		protected GenericNodeView AddResizableText(string text, Node parentNode)
		{
			return AddResizableText(text, string.Empty, string.Empty, parentNode);
		}

		protected GenericNodeView AddResizableText(string text, string icon, Node parentNode)
		{
			return AddResizableText(text, icon, string.Empty, parentNode);
		}

		protected GenericNodeView AddResizableText(string text, string icon, string id, Node parentNode)
		{
			if (parentNode == null)
				parentNode = _rootNode;
				
			GenericNodeView node = new GenericNodeView();
			node.resizable = true;
			node.name = text;
			node.icon = icon;
			parentNode.AddNode(node);
			return node;
		}

		protected Node FindNodeById(string id)
		{
			foreach (var node in _rootNode.FlattenChilds())
			{
				if (node.id == id)
				{
					return node;
				}
			}

			return null;
		}

		protected void ClearNodes()
		{
			_rootNode.children.Clear();
		}

		public void Rebuild()
		{
			_rootNode.RebuildFlattenChilds();
			_rootNode.RebuildFlattenVisibleChilds();
			NotifyRequireUpdateUI(UpdateUIType.TreeChanged);
		}

		protected void RefreshUI()
		{
			NotifyRequireUpdateUI(UpdateUIType.DataChanged);
		}

		protected virtual void OnCategoryToggled(GenericNodeView nodeView)
		{
			nodeView.ToggleExpand();
			_rootNode.RebuildFlattenVisibleChilds();
			NotifyRequireUpdateUI(UpdateUIType.CellVisibleChanged);
		}

		void NotifyRequireUpdateUI(UpdateUIType updateType)
		{
			if (OnRequireUpdateUI != null)
			{
				OnRequireUpdateUI(updateType);
			}
		}

		protected static int DefaultCompareNode(Node nodeA, Node nodeB)
		{
			if (nodeA.children.Count == 0 || nodeB.children.Count == 0)
			{
				if (nodeA.children.Count == 0 && nodeB.children.Count == 0)
					return nodeA.name.CompareTo(nodeB.name);
				else
					return nodeA.children.Count == 0 ? -1 : 1;
			}
			else
			{
				return nodeA.name.CompareTo(nodeB.name);
			}
		}
	}
}