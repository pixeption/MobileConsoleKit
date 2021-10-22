using System;
using System.Collections.Generic;
using System.Text;
using MobileConsole.UI;
using UnityEngine;

namespace MobileConsole
{
	public class PlayerPrefsViewBuilder : ViewBuilder
	{
		enum ValueType
		{
			None,
			Int,
			Float,
			String
		}

		class KeyValue
		{
			public string key;
			public object value;
			public ValueType valueType = ValueType.None;
		}

		List<KeyValue> _keyValues;
		Node _resultNode;

		public PlayerPrefsViewBuilder()
		{
			actionButtonIcon = "share";
			actionButtonCallback = Share;

			Node actionGroup = CreateCategory("Actions");
			AddButton("Refresh", "action", RefreshData, actionGroup);
			AddButton("Save/Flush", "action", SaveAll, actionGroup);
			AddButton("Delete All Keys", "delete", DeleteAllKeys, actionGroup);
		}

		public override void OnPrepareToShow()
		{
#if !(UNITY_EDITOR_WIN || UNITY_STANDALONE_WIN) || !NET_STANDARD_2_0
			FetchData();
			DrawResults();
#else
			DrawUnsupportDotNetStandard20();
#endif

			base.OnPrepareToShow();
		}

		void FetchData()
		{
			if (_keyValues == null)
				_keyValues = new List<KeyValue>();
			else
				_keyValues.Clear();

			string[] allKeys = NativeHelper.GetAllPlayerPrefsKeys();
			foreach (var key in allKeys)
			{
				// Ignore all mobileconsole key
				if (key.StartsWith("MobileConsole."))
					continue;

				KeyValue kv = new KeyValue();
				_keyValues.Add(kv);
				kv.key = key;

				kv.value = PlayerPrefs.GetString(kv.key, string.Empty);
				if (!string.IsNullOrEmpty((string)kv.value))
				{
					kv.valueType = ValueType.String;
					continue;
				}

				kv.value = PlayerPrefs.GetInt(kv.key, int.MinValue);
				if ((int)kv.value != int.MinValue)
				{
					kv.valueType = ValueType.Int;
					continue;
				}

				kv.value = PlayerPrefs.GetFloat(kv.key, float.MinValue);
				if ((float)kv.value != float.MinValue)
				{
					kv.valueType = ValueType.Float;
					continue;
				}
			}

			_keyValues.Sort((kv1, kv2) => kv1.key.CompareTo(kv2.key));
		}

		void DrawResults()
		{
			if (_resultNode != null)
				_resultNode.RemoveFromParent();
			_resultNode = CreateCategory("Player Prefs - " + DateTime.Now.ToString("[hh:mm:ss.ff]"));

			foreach (var kv in _keyValues)
			{
				InputNodeView node = AddInput(kv.key, kv.value.ToString(), OnValueChanged, _resultNode);
				node.data = kv;
			}
		}

		void OnValueChanged(InputNodeView nodeView)
		{
			KeyValue kv = (KeyValue)nodeView.data;
			try
			{
				if (kv.valueType == ValueType.String)
				{
					PlayerPrefs.SetString(kv.key, nodeView.value);
				}
				else if (kv.valueType == ValueType.Int)
				{
					int value = Convert.ToInt32(nodeView.value);
					PlayerPrefs.SetInt(kv.key, value);
				}
				else if (kv.valueType == ValueType.Float)
				{
					float value = (float)Convert.ToDouble(nodeView.value);
					PlayerPrefs.SetFloat(kv.key, value);
				}

				PlayerPrefs.Save();
			}
			catch
			{
				nodeView.value = kv.value.ToString();
				RefreshUI();
			}
		}

		void RefreshData(GenericNodeView nodeView)
		{
			FetchData();
			DrawResults();
			Rebuild();
		}

		void SaveAll(GenericNodeView nodeView)
		{
			PlayerPrefs.Save();
			FetchData();
			DrawResults();
			Rebuild();
		}

		void DeleteAllKeys(GenericNodeView nodeView)
		{
			PlayerPrefs.DeleteAll();
			
			// Re-save command info
			foreach (var command in LogConsole.GetCommands())
			{
				command.SaveAllFieldInfos();
			}
			PlayerPrefs.Save();

			FetchData();
			DrawResults();
			Rebuild();
		}

		void Share()
		{
			StringBuilder sb = new StringBuilder();
			sb.AppendLine(AppAndDeviceInfo.FullInfos());
			sb.AppendLine("------------");
			sb.AppendLine();

			foreach (var kv in _keyValues)
			{
				sb.AppendFormat("{0}: {1}\n", kv.key, kv.value.ToString());
			}

			NativeShare.Share(sb.ToString());
		}

		void DrawUnsupportDotNetStandard20()
		{
			if (_resultNode != null)
				_resultNode.RemoveFromParent();
			_resultNode = CreateCategory("Player Prefs - " + DateTime.Now.ToString("[hh:mm:ss.ff]"));

			AddResizableText("Feature is unsupported on Window with <b>.Net Standard 2.0</b>. If you really want to use this feature, go to <b>Build Setting/API Compatibility Level</b> and switch to <b>.Net 4x</b> ", _resultNode);
		}
	}
}