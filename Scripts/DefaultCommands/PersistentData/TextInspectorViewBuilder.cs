using System.IO;
using System.Text;
using MobileConsole.UI;
using UnityEngine;

namespace MobileConsole
{
	public class TextInspectorViewBuilder : ViewBuilder
	{
		const int MaxFileSize = 2048;
		const int numCharacterPerCell = 1000;
		const string MaxFileSizeWarning = "File size is greater than {0} KB, this may be not a text file.\n\nIf this is a text file, you can adjust the max file size variable (maxFileSize) in <b>TextInspectorViewBuilder.cs</b>";

		FileInfo _fileInfo;

		public TextInspectorViewBuilder()
		{
			actionButtonIcon = "share";
			actionButtonCallback = ShareContent;
			closeAllSubViewOnAction = false;
			saveScrollViewPosition = false;
		}

		public void SetContent(FileInfo fileInfo)
		{
			_fileInfo = fileInfo;
			title = _fileInfo.Name;
			
			ClearNodes();

			Node infoGroup = CreateCategories("Info");
			AddResizableText(GetFileInfo(), infoGroup);

			Node contentGroup = CreateCategories("Content");
			if (_fileInfo.Length <= MaxFileSize * 1024)
			{
				string content = File.ReadAllText(_fileInfo.FullName);
				int startIndex = 0;
				int subStrLength;
				while (startIndex < content.Length)
				{
					subStrLength = Mathf.Min(numCharacterPerCell, content.Length - startIndex);
					AddResizableText(content.Substring(startIndex, subStrLength), contentGroup);
					startIndex += subStrLength;
				}
			}
			else
			{
				AddResizableText(string.Format(MaxFileSizeWarning, MaxFileSize), contentGroup);
			}
		}

		string GetFileInfo()
		{
			StringBuilder sb = new StringBuilder();
			sb.AppendFormat("<b>Name</b>: {0}\n\n", title);
			sb.AppendFormat("<b>Full Path</b>: {0}\n\n", _fileInfo.FullName);
			sb.AppendFormat("<b>File Size</b>: {0:0.#} KB\n\n", _fileInfo.Length / 1024.0f);
			sb.AppendFormat("<b>Created</b>: {0}\n\n", _fileInfo.CreationTime.ToString("MM/dd/yyyy HH:mm:ss"));
			sb.AppendFormat("<b>Modified</b>: {0}\n\n", _fileInfo.LastWriteTime.ToString("MM/dd/yyyy HH:mm:ss"));
			sb.AppendFormat("<b>Last Accessed</b>: {0}\n", _fileInfo.LastAccessTime.ToString("MM/dd/yyyy HH:mm:ss"));
			sb.Append("----------");
			return sb.ToString();
		}

		void ShareContent()
		{
			NativeShare.Share("", _fileInfo.FullName);
		}
	}
}