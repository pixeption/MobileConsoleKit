using System.IO;
using System.Text;
using MobileConsole.UI;
using UnityEngine;

namespace MobileConsole
{
	class ImageInspectorViewBuilder : ViewBuilder
	{
		FileInfo _fileInfo;

		public ImageInspectorViewBuilder()
		{
			actionButtonIcon = "share";
			actionButtonCallback = ShareContent;
			closeAllSubViewOnAction = false;
		}

		public void SetContent(FileInfo fileInfo)
		{
			_fileInfo = fileInfo;
			title = _fileInfo.Name;

			ClearNodes();

			Node infoGroup = CreateCategories("Info");
			Node contentGroup = CreateCategories("Content");

			try
			{
				byte[] bytes = File.ReadAllBytes(_fileInfo.FullName);
				Texture2D texture = new Texture2D(2, 2, TextureFormat.ARGB32, false);
				texture.LoadImage(bytes);
				Sprite sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));

				AddResizableText(GetFileInfo(texture), infoGroup);
				AddImageNodeView(sprite, contentGroup);
			}
			catch
			{
				AddResizableText("There must be something wrong!");
			}
		}

		void AddImageNodeView(Sprite sprite, Node parrent)
		{
			ImageNodeView nodeView = new ImageNodeView();
			nodeView.data = sprite;
			nodeView.resizable = true;
			parrent.AddNode(nodeView);
		}

		string GetFileInfo(Texture2D texture)
		{
			StringBuilder sb = new StringBuilder();
			sb.AppendFormat("<b>Name</b>: {0}\n\n", title);
			sb.AppendFormat("<b>Full Path</b>: {0}\n\n", _fileInfo.FullName);
			sb.AppendFormat("<b>File Size</b>: {0:0.#} KB\n\n", _fileInfo.Length / 1024.0f);
			sb.AppendFormat("<b>Texture Size</b>: {0}x{1}\n\n", texture.width, texture.height);
			sb.AppendFormat("<b>Created</b>: {0}\n\n", _fileInfo.CreationTime.ToString("MM/dd/yyyy HH:mm:ss"));
			sb.AppendFormat("<b>Modified</b>: {0}\n\n", _fileInfo.LastWriteTime.ToString("MM/dd/yyyy HH:mm:ss"));
			sb.AppendFormat("<b>Last Accessed</b>: {0}", _fileInfo.LastAccessTime.ToString("MM/dd/yyyy HH:mm:ss"));
			return sb.ToString();
		}

		void ShareContent()
		{
			NativeShare.Share("", _fileInfo.FullName);
		}
	}
}