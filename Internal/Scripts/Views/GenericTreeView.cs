using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace MobileConsole.UI
{
	public class GenericTreeView : BaseView, IRecycleScrollViewDelegate
	{
		[SerializeField]
		protected AssetConfig _config;

		[SerializeField]
		protected UIBridge _title;

		[SerializeField]
		protected GameObject _filterGroup;

		[SerializeField]
		protected UIBridge _inputFilter;

		[SerializeField]
		protected GameObject _backButton;

		[SerializeField]
		protected GameObject _actionButton;

		[SerializeField]
		protected RecycleScrollView _scrollView;

		[SerializeField]
		protected Transform _scrollViewContent;

		Dictionary<string, ScrollViewCell> _resizableCells = new Dictionary<string, ScrollViewCell>();

		List<Node> _filterNodes = new List<Node>();
		ViewBuilder _viewBuilder;
		protected string _filterString;
		bool _isInitialized = false;
		bool _isPreparingView = false;

		public void Show(ViewBuilder builder)
		{
			if (builder == null)
			{
				throw new System.Exception("View Builder is null");
			}

			base.Show();

			if (!_isInitialized)
			{
				_isInitialized = true;
				_scrollView.AddCellTemplates(_config.cellTemplates);
				_scrollView.SetDelegate(this);
			}

			ClearOldBuilder();
			SetupNewBuilder(builder);
		}

		public override void Hide()
		{
			base.Hide();
			ClearOldBuilder();
		}

		void ClearOldBuilder()
		{
			if (_viewBuilder != null)
			{
				_viewBuilder.OnRequireUpdateUI = null;
				_viewBuilder.scrollViewPosition = _scrollView.ScrollPosition;
				_viewBuilder.filterString = _filterString;
				_viewBuilder.OnPrepareToHide();
			}

			_viewBuilder = null;
		}

		void SetupNewBuilder(ViewBuilder builder)
		{
			// Prevent input filter callback while we set its value
			_isPreparingView = true;

            _viewBuilder = builder;
            _viewBuilder.OnRequireUpdateUI = OnUpdateView;
            _viewBuilder.OnPrepareToShow();

			// Update title
			bool hasTitle = !string.IsNullOrEmpty(_viewBuilder.title);
			_title.gameObject.SetActive(hasTitle);
			if (hasTitle)
			{
				_title.text = _viewBuilder.title;
			}

			// Update filter string
			_filterString = _viewBuilder.filterString;
			_inputFilter.input = _filterString;
			_filterGroup.SetActive(!hasTitle);

			// Update buttons
			_backButton.SetActive(_viewBuilder.level > 0);
			_actionButton.SetActive(_viewBuilder.actionButtonCallback != null);
			AssetConfig.SpriteInfo spriteInfo = _config.GetSpriteInfo(_viewBuilder.actionButtonIcon);
			if (spriteInfo != null)
			{
				UnityEngine.UI.Image buttonImage = _actionButton.GetComponentInChildren<UnityEngine.UI.Image>();
				buttonImage.sprite = spriteInfo.sprite;
				buttonImage.color = spriteInfo.color;
			}

			// Rebuild the whole tree
            _viewBuilder.Rebuild();

			// Update scroll view
            if (!_viewBuilder.saveScrollViewPosition)
                _scrollView.MoveViewToTop();
            else
                _scrollView.ScrollPosition = _viewBuilder.scrollViewPosition;

			_isPreparingView = false;
		}

		void OnUpdateView(ViewBuilder.UpdateUIType updateType)
		{
			switch (updateType)
			{
				case ViewBuilder.UpdateUIType.DataChanged:
				{
					_scrollView.ReloadData();
					break;
				}
				case ViewBuilder.UpdateUIType.CellVisibleChanged:
				case ViewBuilder.UpdateUIType.TreeChanged:
				{
					UpdateTree();
					break;
				}
				default:
					break;
			}			
		}

		void UpdateTree()
		{
			RootNode rootNode = _viewBuilder.GetRootNode();

			_filterNodes.Clear();
			if (string.IsNullOrEmpty(_filterString))
			{
				_filterNodes.AddRange(rootNode.FlattenedVisibleChilds());
			}
			else
			{
				HashSet<Node> passFilterNodes = new HashSet<Node>();
				foreach (var node in rootNode.FlattenChilds())
				{
					if (node.name.IndexOf(_filterString, StringComparison.OrdinalIgnoreCase) >= 0)
					{
						if (passFilterNodes.Contains(node))
							continue;

						foreach (var n in node.GetBranch())
						{
							if (n.isVisible)
								passFilterNodes.Add(n);
						}

						foreach (var n in node.GetAllChildren())
						{
							if (n.isVisible)
								passFilterNodes.Add(n);
						}
					}
				}

				foreach (var node in passFilterNodes)
				{
					if (node.level == -1)
					{
						continue;
					}

					_filterNodes.Add(node);
				}
			}

			_scrollView.ReloadData();
		}

		public void OnFilterValueChanged(string filterString)
		{
			// Don't process if view is being prepared
			if (_isPreparingView)
				return;
			
			_filterString = filterString;
			UpdateTree();
		}

		public void OnClosed()
		{
			LogConsole.CloseAllSubView();
		}

		public void OnBack()
		{
			LogConsole.PopSubView();
		}

		public void OnAction()
		{
			if (_viewBuilder.actionButtonCallback != null)
			{
				try
				{
					_viewBuilder.actionButtonCallback();
                    if (_viewBuilder.closeAllSubViewOnAction)
                    {
                        LogConsole.CloseAllSubView();
                    }
				}
				catch (Exception e)
				{
					Debug.LogException(e);
				}
			}
		}

		public void OnExpandAll()
		{
			RootNode rootNode = _viewBuilder.GetRootNode();
			rootNode.ExpandAllChild(true);
			rootNode.RebuildFlattenVisibleChilds();
			OnUpdateView(ViewBuilder.UpdateUIType.CellVisibleChanged);
		}

		public void OnCollapseAll()
		{
			RootNode rootNode = _viewBuilder.GetRootNode();
			rootNode.ExpandAllChild(false);
			rootNode.RebuildFlattenVisibleChilds();
			OnUpdateView(ViewBuilder.UpdateUIType.CellVisibleChanged);
		}

		public ScrollViewCell ScrollCellCreated(int cellIndex)
		{
			NodeView nodeView = (NodeView)_filterNodes[cellIndex];
			return nodeView.CreateCell(_scrollView, _config, cellIndex);
		}

		public void ScrollCellSelected(int cellIndex)
		{
			NodeView nodeView = (NodeView)_filterNodes[cellIndex];
			nodeView.CellSelected();
		}

		public void ScrollCellWillDisplay(ScrollViewCell cell)
		{
		}

		public float ScrollCellSize(int cellIndex)
		{
			NodeView nodeView = (NodeView)_filterNodes[cellIndex];
			if (nodeView.resizable)
			{
				string cellIdentifier = nodeView.CellIdentifier();
				if (!_resizableCells.ContainsKey(cellIdentifier))
				{
					ScrollViewCell cellTemplate = _config.cellTemplates.Find(c => c.identifier == cellIdentifier);
					ScrollViewCell cell = Instantiate(cellTemplate, _scrollViewContent);
					_resizableCells[cellIdentifier] = cell;

					LayoutRebuilder.ForceRebuildLayoutImmediate(cell.rectTransform);
					cell.gameObject.SetActive(false);
				}

				return Mathf.Max(nodeView.CellSize(_resizableCells[cellIdentifier]), 120);
			}
			else
			{
				return nodeView.CellSize();
			}
		}

		public int ScrollCellCount()
		{
			return _filterNodes.Count;
		}
	}
}