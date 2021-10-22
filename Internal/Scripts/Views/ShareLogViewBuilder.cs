using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;

namespace MobileConsole.UI
{
	public class ShareLogViewBuilder : ViewBuilder
	{
		public class ShareLogConfig : Command
		{
			public bool infoEnable = true;
			public bool infoStacktraceEnable = true;
			public bool warningEnable = true;
			public bool warningStacktraceEnable = true;
			public bool errorEnable = true;
			public bool errorStacktraceEnable = true;
			public bool useCurrentFilter = false;
			public bool applicationAndDeviceInfos = true;
		}

		Dictionary<LogType, bool> logEnables = new Dictionary<LogType, bool>() {
			{LogType.Log, true},
			{LogType.Warning, true},
			{LogType.Error, true}
		};

		Dictionary<LogType, bool> stacktraceEnables = new Dictionary<LogType, bool>() {
			{LogType.Log, true},
			{LogType.Warning, true},
			{LogType.Error, true}
		};

		List<LogInfo> _filteredLogs;
		ShareLogConfig _shareConfig = new ShareLogConfig();

		public ShareLogViewBuilder(List<LogInfo> filteredLogs)
		{
			_filteredLogs = filteredLogs;

			_shareConfig.CacheVariableInfos<ShareLogConfig>();
			_shareConfig.LoadSavedValue();

			Node currentNode;
			currentNode = CreateCategories("Log Info");
			AddCommandField(_shareConfig, "infoEnable", "Enable", currentNode);
			AddCommandField(_shareConfig, "infoStacktraceEnable", "Stacktrace Enable", currentNode);

			currentNode = CreateCategories("Log Warning");
			AddCommandField(_shareConfig, "warningEnable", "Enable", currentNode);
			AddCommandField(_shareConfig, "warningStacktraceEnable", "Stacktrace Enable", currentNode);

			currentNode = CreateCategories("Log Error");
			AddCommandField(_shareConfig, "errorEnable", "Enable", currentNode);
			AddCommandField(_shareConfig, "errorStacktraceEnable", "Stacktrace Enable", currentNode);

			AddCommandField(_shareConfig, "useCurrentFilter", "Use Current Filter");
			AddCommandField(_shareConfig, "applicationAndDeviceInfos", "Application and Device infos");

			title = "Share Log Setting";
			actionButtonIcon = "share";
			actionButtonCallback += OnShareLog;
		}

		public void OnShareLog()
		{
			logEnables[LogType.Log] = _shareConfig.infoEnable;
			logEnables[LogType.Warning] = _shareConfig.warningEnable;
			logEnables[LogType.Error] = _shareConfig.errorEnable;
			stacktraceEnables[LogType.Log] = _shareConfig.infoStacktraceEnable;
			stacktraceEnables[LogType.Warning] = _shareConfig.warningStacktraceEnable;
			stacktraceEnables[LogType.Error] = _shareConfig.errorStacktraceEnable;

			StringBuilder sb = new StringBuilder();

			// Append application & device info
			if (_shareConfig.applicationAndDeviceInfos)
			{
				sb.AppendLine(AppAndDeviceInfo.FullInfos());
				sb.AppendLine("------------------");
				sb.AppendLine();
				sb.AppendLine();
			}

			// Append log infos
			List<LogInfo> logInfos = _shareConfig.useCurrentFilter ? _filteredLogs : LogReceiver.LogInfos;
			foreach (var logInfo in logInfos)
			{
				if (logEnables[logInfo.type])
				{
					sb.Append(string.Format("[{0}] ", logInfo.type.ToString()));
					sb.Append(logInfo.time);
					if (logInfo.channelInfo != null)
					{
						sb.AppendFormat(LogConsoleSettings.Instance.channelFormat, logInfo.channelInfo.name);
					}
					sb.AppendLine(logInfo.message);
					if (stacktraceEnables[logInfo.type])
					{
						sb.AppendLine("------------------");
						sb.AppendLine(logInfo.stackTrace);
						sb.AppendLine();
					}
				}
			}

            EventBridge.NotifyShareAllLog(sb.ToString());
        }
	}
}

