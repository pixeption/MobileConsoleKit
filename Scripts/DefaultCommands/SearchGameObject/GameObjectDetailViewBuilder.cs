using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using MobileConsole.UI;
using UnityEngine;
using UnityEngine.EventSystems;

namespace MobileConsole
{
	public class GameObjectDetailViewBuilder : ViewBuilder
	{
		class _FieldInfo
		{
			public string name;
			public string type;
			public string value;
		}

		class _ComponentInfo
		{
			public string name;
			public List<_FieldInfo> fieldInfos = new List<_FieldInfo>();
		}

		class _MonoInfo
		{
			public string name;
			public List<_ComponentInfo> componentInfos = new List<_ComponentInfo>();
		}

		List<_MonoInfo> _monoInfos = new List<_MonoInfo>();
		StringBuilder _sb;
		GameObject _gameObject;
		bool _isShowBaseClass = true;

		static Type[] _blacklistTypes = new Type[] {
			typeof(MonoBehaviour),
			typeof(Component),
			typeof(UIBehaviour),
			typeof(Behaviour)
		};

		public GameObjectDetailViewBuilder()
		{
			saveScrollViewPosition = false;
			actionButtonIcon = "share";
			actionButtonCallback = Share;
			closeAllSubViewOnAction = false;

			_sb = new StringBuilder();
		}

		public void SetGameObject(GameObject gameObject)
		{
			_gameObject = gameObject;
			title = _gameObject.name;
			AnalyzeGameObject();
			RebuildView();
		}

		void AnalyzeGameObject()
		{
			_monoInfos.Clear();
			Component[] components = _gameObject.GetComponents<Component>();
			foreach (var component in components)
			{
				var componentType = component.GetType();

				_MonoInfo monoInfo = new _MonoInfo();
				_monoInfos.Add(monoInfo);

				monoInfo.name = componentType.Name;
				while (componentType != null && Array.IndexOf(_blacklistTypes, componentType) == -1)
				{
					_ComponentInfo componentInfo = GetComponentInfo(componentType, component);
					monoInfo.componentInfos.Add(componentInfo);
					componentType = componentType.BaseType;
				}
			}
		}

		void RebuildView()
		{
			ClearNodes();

			Node settingGroup = CreateCategories("Setting");
			AddCheckbox("Show base classes", _isShowBaseClass, OnShowBaseClassChanged, settingGroup);

			foreach (var monoInfo in _monoInfos)
			{
				Node group = CreateCategories(monoInfo.name);
				foreach (var componentInfo in monoInfo.componentInfos)
				{
					_sb.Length = 0;
					_sb.AppendFormat("<size=110%>{0}<size=85%>\n", TextColors.GetUniqueColorWithTag(componentInfo.name));
					foreach (var fieldInfo in componentInfo.fieldInfos)
					{
						_sb.AppendFormat("{0}<alpha=#66>({1})<alpha=#FF> - {2}\n\n", fieldInfo.name, fieldInfo.type, fieldInfo.value);
					}

					AddResizableText(_sb.ToString(), group);

					// if not showing base class, break after the first iteration
					if (!_isShowBaseClass)
					{
						break;
					}
				}
			}
		}

		_ComponentInfo GetComponentInfo(Type type, object component)
		{
			_ComponentInfo componentInfo = new _ComponentInfo();
			componentInfo.name = type.Name;

			var fieldInfos = type.GetFields(BindingFlags.DeclaredOnly | BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
			foreach (var fieldInfo in fieldInfos)
			{
				try
				{
					var value = fieldInfo.GetValue(component);
					_FieldInfo fi = new _FieldInfo();
					fi.name = fieldInfo.Name;
					fi.type = fieldInfo.FieldType.Name;
					fi.value = (value != null) ? value.ToString() : "null";
					componentInfo.fieldInfos.Add(fi);
				}
				catch { }
			}

			var propertyInfos = type.GetProperties(BindingFlags.DeclaredOnly | BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
			foreach (var property in propertyInfos)
			{
				// Query for deprecated value will yield exception
				try
				{
					var value = property.GetValue(component, null);
					_FieldInfo fi = new _FieldInfo();
					fi.name = property.Name;
					fi.type = property.PropertyType.Name;
					fi.value = (value != null) ? value.ToString() : "null";
					componentInfo.fieldInfos.Add(fi);
				}
				catch { }
			}

			return componentInfo;
		}

		void OnShowBaseClassChanged(CheckboxNodeView nodeView)
		{
			_isShowBaseClass = nodeView.isOn;
			RebuildView();
			Rebuild();
		}

		void Share()
		{
			_sb.Length = 0;
			// Get all visible cell
			foreach (var monoInfo in _monoInfos)
			{
				_sb.AppendFormat("========= {0} ========\n", monoInfo.name);
				foreach (var componentInfo in monoInfo.componentInfos)
				{
					_sb.AppendFormat("\t\t--- {0}\n", componentInfo.name);
					foreach (var fieldInfo in componentInfo.fieldInfos)
					{
						_sb.AppendFormat("\t\t\t\t{0}({1}) - {2}\n", fieldInfo.name, fieldInfo.type, fieldInfo.value);
					}
					_sb.AppendLine();

					// if not showing base class, break after the first iteration
					if (!_isShowBaseClass)
					{
						break;
					}
				}
				_sb.AppendLine();
				_sb.AppendLine();
				_sb.AppendLine();
			}

			NativeShare.Share(_sb.ToString());
		}
	}
}