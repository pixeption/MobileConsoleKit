using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;

namespace MobileConsole
{
	public static class LogReceiver
	{
		public delegate void LogCallback(LogInfo logInfo);
		public static event LogCallback OnLogReceived;
		static Pool<LogInfo> _logInfoPool;
		static List<LogInfo> _logInfos = new List<LogInfo>();
		static int _limitCharacterView = 200;

        internal static List<LogInfo> LogInfos
		{
			get { return _logInfos; }
		}

        public static void Init()
        {
            _logInfoPool = new Pool<LogInfo>(1000);
            Application.logMessageReceived += LogMessageReceived;
			LogFilter.Instance.RegisterChannelListener();
        }

		static void LogMessageReceived(string message, string stackTrace, LogType type)
		{
			LogInfo logInfo = _logInfoPool.Get();
			Match match = Regex.Match(message, LogConsoleSettings.Instance.channelRegex);

			logInfo.channelInfo = match.Success ? LogChannelInfoCache.GetOrCreateChannelInfo(match.Groups[1].Value) : null;
			logInfo.message = match.Success ? message.Substring(match.Length) : message;
			logInfo.shortMessage = (logInfo.message.Length <= _limitCharacterView) ? logInfo.message : logInfo.message.Substring(0, _limitCharacterView);
			logInfo.stackTrace = stackTrace;
			logInfo.type = ConvertLogType(type);
			logInfo.time = System.DateTime.Now.ToString(LogConsoleSettings.Instance.timeFormat);
			logInfo.numInstance = 0;
			logInfo.hash = (logInfo.message + logInfo.stackTrace).GetHashCode();
			
			_logInfos.Add(logInfo);

			if (OnLogReceived != null)
			{
				OnLogReceived(logInfo);
			}
		}

        internal static void Clear()
		{
			if (_logInfoPool != null)
			{
				_logInfoPool.Return(_logInfos);
				_logInfos.Clear();
			}
		}

		static LogType ConvertLogType(LogType logType)
		{
			switch (logType)
			{
				case LogType.Error:
				case LogType.Exception:
				case LogType.Assert:
					return LogType.Error;
				default:
					return logType;
			}
		}
	}
}