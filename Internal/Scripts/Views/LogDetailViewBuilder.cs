using System.Text;

namespace MobileConsole.UI
{
	public class LogDetailViewBuilder : ViewBuilder
	{
		LogInfo _logInfo;
		StringBuilder _cachedText = new StringBuilder(500);

		public LogDetailViewBuilder()
		{
			title = "Log Detail";
			actionButtonIcon = "share";
			actionButtonCallback = OnShareLog;
			saveScrollViewPosition = false;
		}

		public void SetLogInfo(LogInfo logInfo, string iconName)
		{
			_logInfo = logInfo;
			ClearNodes();

			// Build log message
			_cachedText.Length = 0;
			_cachedText.Append(_logInfo.time);
			if (logInfo.channelInfo != null)
			{
				_cachedText.Append(logInfo.channelInfo.nameWithColor);
			}
			_cachedText.AppendLine();
			_cachedText.Append(_logInfo.message);

			// Append log message
			AddResizableText(_cachedText.ToString(), iconName);
			
			// Append stacktraces
			string[] stacktraces = _logInfo.stackTrace.Split('\n');
			foreach (var stacktrace in stacktraces)
			{
				if (string.IsNullOrEmpty(stacktrace))
					continue;
					
				AddResizableText(stacktrace);
			}
		}

		public void OnShareLog()
		{
			StringBuilder sb = new StringBuilder();
			sb.AppendLine(AppAndDeviceInfo.FullInfos());
			sb.AppendLine();
			sb.AppendLine();
			sb.Append(_logInfo.time);
			if (_logInfo.channelInfo != null)
			{
				sb.AppendFormat(LogConsoleSettings.Instance.channelFormat, _logInfo.channelInfo.name);
			}
			sb.AppendLine(_logInfo.message);
			sb.AppendLine();
			sb.AppendLine(_logInfo.stackTrace);

			EventBridge.NotifyShareLog(sb.ToString());
		}
	}
}

