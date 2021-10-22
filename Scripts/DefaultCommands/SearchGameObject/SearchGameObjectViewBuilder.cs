using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using MobileConsole.UI;
using UnityEngine;

namespace MobileConsole
{
	class SearchData : Command
	{
		public enum SearchType { Name, Tag, Component }
		public bool ignoreCase = true;
		public bool showHierarchy = true;
		public SearchType searchBy = SearchType.Name;
		public string keyword = string.Empty;
	}

	public class SearchGameObjectViewBuilder : ViewBuilder
	{
		const float Float_DeactiveGOAlpha = 0.3f;
		const float Float_ActiveGOAlpha = 0.8f;
		static Color Color_FoundGOColor = new Color(0.33f, 0.74f, 1.0f, 0.8f);
		static Color Color_NormalGOColor = new Color(1.0f, 1.0f, 1.0f, 0.8f);
		const string StartSign = "<!-";
		const string EndSign = "-!>";
		const string SearchTextMarkSign_Template = "<!-{0}-!>";
		const string SearchTextHighlightStartTag = "<color=#54BDFFCC>";
		const string SearchTextHighlightEndTag = "</color>";
		const string SearchText_Regex_Case_Insensitive = "(?i){0}";
		const string SearchText_Regex_Case_Sensitive = "(?-i){0}";

		SearchData _searchData;
		List<GameObject> _results;
		GameObjectDetailViewBuilder _detailViewBuilder;
		Node _resultNode;

		public SearchGameObjectViewBuilder()
		{
			_searchData = new SearchData();
			_searchData.CacheVariableInfos<SearchData>();
			_detailViewBuilder = new GameObjectDetailViewBuilder();

			DrawSearchControl();
		}

		public override void OnPrepareToShow()
		{
			base.OnPrepareToShow();
			_searchData.LoadSavedValue();
		}

		void DrawSearchControl()
		{
			Node searchGroup = CreateCategories("Actions");
			AddCommandFields(_searchData, searchGroup);
			AddButton("Search", "search", Search, searchGroup);
		}

		void DrawNoResultFound()
		{
			string resultMessage = string.Format("No result for search by {0} - {1}", _searchData.searchBy.ToString(), _searchData.keyword);
			AddResizableText(resultMessage, _resultNode);
		}

		void Search(GenericNodeView nodeView)
		{
			if (string.IsNullOrEmpty(_searchData.keyword))
				return;

			if (_resultNode != null)
				_resultNode.RemoveFromParent();
			_resultNode = CreateCategory("Result - " + DateTime.Now.ToString("[hh:mm:ss.ff]"));

			SearchGameObjects();
			// Add no result cell
			if (_results == null || _results.Count == 0)
			{
				DrawNoResultFound();
			}
			else
			{
				if (_searchData.showHierarchy)
				{
					DrawResultTree();
				}
				else
				{
					DrawResultList();
				}
			}
	
			// Update new data as the node tree has changed
			Rebuild();
		}

		void OnSelectGameObject(GenericNodeView nodeView)
		{
			// Magic here, need to cast data to GameObject before checking null, otherwise it won't work
			GameObject selectGO = (GameObject)nodeView.data;
			if (selectGO != null)
			{
				_detailViewBuilder.SetGameObject(selectGO);
				LogConsole.PushSubView(_detailViewBuilder);
			}
			else
			{
				// Check if all game object inside result is destroyed
				foreach (GenericNodeView node in GetAllChildrenOfNode(_resultNode))
				{
					GameObject go = (GameObject)node.data;
					if (go == null)
					{
						node.overrideDisplayText = "<s>" + node.overrideDisplayText;
					}
				}

				RefreshUI();
			}
		}

		void SearchGameObjects()
		{
			if (_searchData.searchBy == SearchData.SearchType.Name)
			{
				StringComparison comparison = _searchData.ignoreCase ? StringComparison.OrdinalIgnoreCase : StringComparison.CurrentCulture;
				var allGameObjects = Resources.FindObjectsOfTypeAll(typeof(GameObject));

				_results = new List<GameObject>();
				foreach (var gameObject in allGameObjects)
				{
					if (gameObject.name.IndexOf(_searchData.keyword, comparison) >= 0)
					{
						_results.Add((GameObject)gameObject);
					}
				}
			}
			else if (_searchData.searchBy == SearchData.SearchType.Tag)
			{
				try { _results = GameObject.FindGameObjectsWithTag(_searchData.keyword).ToList(); }
				catch { }
			}
			else if (_searchData.searchBy == SearchData.SearchType.Component)
			{
				// Get all types
				Type[] types = TypeCollector.GetClassesOfBase(typeof(Component));
				List<Type> foundTypes = new List<Type>();
				StringComparison comparison = _searchData.ignoreCase ? StringComparison.OrdinalIgnoreCase : StringComparison.CurrentCulture;
				foreach (var type in types)
				{
					if (type.Name.IndexOf(_searchData.keyword, comparison) >= 0)
					{
						foundTypes.Add(type);
					}
				}

				var allGameObjects = Resources.FindObjectsOfTypeAll(typeof(GameObject));
				_results = new List<GameObject>();
				foreach (GameObject gameObject in allGameObjects)
				{
					bool isFound = false;
					foreach (var type in foundTypes)
					{
						if (gameObject.GetComponent(type) != null)
						{
							isFound = true;
							break;
						}
					}

					if (isFound)
						_results.Add(gameObject);
				}
			}
		}

		void DrawResultList()
		{
			StringBuilder sb = new StringBuilder();
			foreach (var gameObject in _results)
			{
				if (string.IsNullOrEmpty(gameObject.scene.name))
					continue;

				sb.Length = 0;
				List<GameObject> objectTree = GetGameObjectTree(gameObject.transform);
				for (int i = 0; i < objectTree.Count; i++)
				{
					sb.Append(objectTree[i].name);
					if (i != objectTree.Count - 1)
						sb.Append("/");
				}

				string foundGOSearchText = GetFoundSearchText(sb.ToString(), _searchData);
				GenericNodeView node = AddButton(gameObject.name, "cube", OnSelectGameObject, _resultNode);
				node.overrideDisplayText = foundGOSearchText;
				node.data = gameObject;
				node.iconColor = Color_FoundGOColor;
				node.iconColor.a = gameObject.activeInHierarchy ? Float_ActiveGOAlpha : Float_DeactiveGOAlpha;
				node.resizable = true;
			}
		}

		void DrawResultTree()
		{
			foreach (var gameObject in _results)
			{
				Node currentNode = _resultNode;
				List<GameObject> objectTree = GetGameObjectTree(gameObject.transform);
				for (int i = 0; i < objectTree.Count; i++)
				{
					// Ignore all prefab/asset here
					if (string.IsNullOrEmpty(objectTree[i].scene.name))
						continue;
					
					GenericNodeView node = (GenericNodeView)currentNode.children.Find(n => (GameObject)n.data == objectTree[i]);
					if (node == null)
					{
						bool isActive = objectTree[i].activeInHierarchy;
						bool isTarget = _results.Contains(objectTree[i]);
						bool isRoot = (i == 0);
						string foundGOSearchText = string.Format("{0}{1}",
															objectTree[i].name,
															isRoot ? string.Format(" ({0})", objectTree[i].scene.name) : string.Empty);

						foundGOSearchText = GetFoundSearchText(foundGOSearchText, _searchData);
						node = AddButton(objectTree[i].name, "cube", OnSelectGameObject, objectTree[i].GetInstanceID().ToString(), currentNode);
						node.overrideDisplayText = foundGOSearchText;
						node.data = objectTree[i];

						// Adjust icon color
						node.iconColor = isTarget ? Color_FoundGOColor : Color_NormalGOColor;
						node.iconColor.a = isActive ? Float_ActiveGOAlpha : Float_DeactiveGOAlpha;
					}
					currentNode = node;
				}
			}
		}

		static List<GameObject> GetGameObjectTree(Transform transform)
		{
			List<GameObject> gameObjects = new List<GameObject>();
			gameObjects.Add(transform.gameObject);
			while (transform.parent != null)
			{
				transform = transform.parent;
				gameObjects.Insert(0, transform.gameObject);
			}

			return gameObjects;
		}

		static List<Node> GetAllChildrenOfNode(Node node, List<Node> list = null)
		{
			if (list == null)
			{
				list = new List<Node>();
			}

			foreach (var child in node.children)
			{
				list.Add(child);
				GetAllChildrenOfNode(child, list);
			}

			return list;
		}

		static string GetFoundSearchText(string text, SearchData searchData)
		{
			if (searchData.searchBy != SearchData.SearchType.Name)
				return text;

			string searchPattern = searchData.ignoreCase ? SearchText_Regex_Case_Insensitive : SearchText_Regex_Case_Sensitive;
			MatchCollection matches = Regex.Matches(text, string.Format(searchPattern, searchData.keyword));
			foreach (Match match in matches)
			{
				text = text.Replace(match.Value, string.Format(SearchTextMarkSign_Template, match.Value));
			}

			text = text.Replace(StartSign, SearchTextHighlightStartTag)
										.Replace(EndSign, SearchTextHighlightEndTag);
			
			return text;
		}
	}
}