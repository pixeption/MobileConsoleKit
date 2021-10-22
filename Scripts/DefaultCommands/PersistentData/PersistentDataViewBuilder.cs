using System;
using System.IO;
using MobileConsole.UI;
using UnityEngine;

namespace MobileConsole
{
	public class PersistentDataViewBuilder : ViewBuilder
	{
		const string ClassName = "PersistentData";
		TextInspectorViewBuilder _textInspectorViewBuilder = new TextInspectorViewBuilder();
		ImageInspectorViewBuilder _imageInspectorViewBuilder = new ImageInspectorViewBuilder();
		NodeView _resultNode;
		string[] _filePaths;

		public PersistentDataViewBuilder()
		{
			Node actionGroup = CreateCategories("Actions");
			AddButton("Refresh", "action", RefreshLocalFiles, actionGroup);
			AddButton("Delete All Files", "delete", DeleteAllFiles, actionGroup);
			AddButton("Share All Files", "share", ShareAllFiles, actionGroup);
		}

		public override void OnPrepareToShow()
		{
			base.OnPrepareToShow();
			RefreshLocalFiles(null);
		}

		void RefreshLocalFiles(GenericNodeView nodeView)
		{
			if (_resultNode != null)
				_resultNode.RemoveFromParent();
			_resultNode = CreateCategory("Persistent Data");
			_resultNode.overrideDisplayText = "Persistent Data - " + DateTime.Now.ToString("[hh:mm:ss.ff]");

			_filePaths = Directory.GetFiles(Application.persistentDataPath, "*", SearchOption.AllDirectories);
			DrawHierarchy(_filePaths);
			CategoryPlayerPrefs.LoadCategoryStates(_rootNode, ClassName);

			Rebuild();
		}

		void DrawHierarchy(string[] filePaths)
		{
			foreach (var path in filePaths)
			{
				string relativePath = path.Replace(Application.persistentDataPath, "").Trim(Path.DirectorySeparatorChar);
				string directoryPath = Path.GetDirectoryName(relativePath);

				#if UNITY_EDITOR_WIN || UNITY_STANDALONE_WIN
				directoryPath = directoryPath.Replace(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);
				#endif

				Node categoryNode = _resultNode;
				if (!string.IsNullOrEmpty(directoryPath))
					categoryNode = CreateCategories(directoryPath, _resultNode);

				FileInfo fileInfo = new FileInfo(path);
				string nodeName = string.Format("{0} <alpha=#66>({1:0.#} KB)", Path.GetFileName(path), fileInfo.Length / 1024.0f);
				Node fileNode = AddButton(nodeName, InspectFile, categoryNode);
				fileNode.data = fileInfo;
			}
		}

		void InspectFile(GenericNodeView nodeView)
		{
			try
			{
				FileInfo fileInfo = (FileInfo)nodeView.data;
				if (fileInfo.Extension == ".png" || fileInfo.Extension == ".jpg")
				{
					_imageInspectorViewBuilder.SetContent(fileInfo);
					LogConsole.PushSubView(_imageInspectorViewBuilder);
				}
				else
				{
					_textInspectorViewBuilder.SetContent(fileInfo);
					LogConsole.PushSubView(_textInspectorViewBuilder);
				}
			}
			catch (Exception e)
			{
				Debug.LogError(e.Message);
			}
		}

		void DeleteAllFiles(GenericNodeView nodeView)
		{
			if (_filePaths.Length == 0)
				return;

			DirectoryInfo dir = new DirectoryInfo(Application.persistentDataPath);

			foreach (var file in dir.GetFiles())
			{
				file.Delete();
			}

			foreach (var directory in dir.GetDirectories())
			{
				directory.Delete(true);
			}

			RefreshLocalFiles(null);
		}

		void ShareAllFiles(GenericNodeView nodeView)
		{
			if (_filePaths == null)
			{
				_filePaths = Directory.GetFiles(Application.persistentDataPath, "*", SearchOption.AllDirectories);	
			}

			if (_filePaths.Length > 0)
			{
				NativeShare.ShareMultiple("", _filePaths);
			}
		}

		protected override void OnCategoryToggled(GenericNodeView nodeView)
		{
			base.OnCategoryToggled(nodeView);
			CategoryPlayerPrefs.SaveCategoryState(nodeView, ClassName);
		}
	}
}