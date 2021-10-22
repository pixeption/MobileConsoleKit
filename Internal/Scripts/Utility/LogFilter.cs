using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine;

namespace MobileConsole
{
	public class LogFilter
	{
		public delegate void CallbackChannelConfigChanged();
		internal static event CallbackChannelConfigChanged OnChannelConfigChanged;
		const string Key_IsCollapse = "MobileConsole.LogFilter_isCollapse";
		const string Key_LogTypeEnablePrefix = "MobileConsole.LogFilter_";
		
		bool _isCollapse = false;
		public bool IsCollapse
		{
			get { return _isCollapse; }
			set 
			{ 
				_isCollapse = value;
				PlayerPrefs.SetInt(Key_IsCollapse, _isCollapse ? 1 : 0);
				PlayerPrefs.Save();
			}
		}

		public bool enabled = false;
		public string filterString;
		Dictionary<LogType, bool> _logTypeConfigs = new Dictionary<LogType, bool>();
		Dictionary<int, LogInfo> _logHashes = new Dictionary<int, LogInfo>();
		Dictionary<int, bool> _channelConfigs = new Dictionary<int, bool>();
		bool _channelEnabledByDefault;
		bool _showNonChannelLogs;
		Result _cachedResult;
		IStringSearch _stringSearch;

		LogFilter()
		{
			_logTypeConfigs[LogType.Log] = PlayerPrefs.GetInt(GetLogTypePlayerPrefsKey(LogType.Log), 1) == 1 ? true : false;
			_logTypeConfigs[LogType.Warning] = PlayerPrefs.GetInt(GetLogTypePlayerPrefsKey(LogType.Warning), 1) == 1 ? true : false;
			_logTypeConfigs[LogType.Error] = PlayerPrefs.GetInt(GetLogTypePlayerPrefsKey(LogType.Error), 1) == 1 ? true : false;
			_isCollapse = PlayerPrefs.GetInt(Key_IsCollapse, 0) == 1 ? true : false;
			
			if (LogConsoleSettings.Instance.useRegexSearch)
			{
				_stringSearch = new RegexSearch(LogConsoleSettings.Instance.regexSearchOptions);
			}
			else
			{
				_stringSearch = new SimpleStringSearch();
			}
		}

		static string GetLogTypePlayerPrefsKey(LogType logType)
		{
			return Key_LogTypeEnablePrefix + logType.ToString();
		}

		public void SetLogEnable(LogType logType, bool enabled)
		{
			_logTypeConfigs[logType] = enabled;
			PlayerPrefs.SetInt(GetLogTypePlayerPrefsKey(logType), enabled ? 1 : 0);
		}

		public bool IsLogTypeEnable(LogType logType)
		{
			if (_logTypeConfigs.ContainsKey(logType))
			{
				return _logTypeConfigs[logType];
			}

			return false;
		}

		public void ClearLogHashes()
		{
			_logHashes.Clear();
		}

		public void RegisterChannelListener()
		{
			_channelEnabledByDefault = true;
			_showNonChannelLogs = true;
			LogChannelInfoCache.OnNewChannelsAdded += OnNewChannelsAdded;
		}

		void OnNewChannelsAdded(int[] channelHashes)
		{
			foreach (var channelHash in channelHashes)
			{
				_channelConfigs[channelHash] = _channelEnabledByDefault;
			}

			NotifyChannelConfigChanged();
		}

		public void SetChannelEnabled(int hash, bool enabled)
		{
			_channelConfigs[hash] = enabled;
			NotifyChannelConfigChanged();
		}

		public void SetAllChannelEnable(bool enabled)
		{
			foreach (var hash in _channelConfigs.Keys.ToList())
			{
				SetChannelEnabled(hash, enabled);
			}

			_showNonChannelLogs = enabled;

			NotifyChannelConfigChanged();
		}

		public bool IsChannelEnabled(int hash)
		{
			if (_channelConfigs.ContainsKey(hash))
			{
				return _channelConfigs[hash];
			}

			return false;
		}

		public void SetChannelEnabledByDefault(bool enabled)
		{
			_channelEnabledByDefault = enabled;
		}

		public bool IsChannelEnabledByDefault()
		{
			return _channelEnabledByDefault;
		}

		public int GetNumEnabledChannel()
		{
			int count = 0;
			foreach (var channel in _channelConfigs)
			{
				if (channel.Value)
				{
					count += 1;
				}
			}

			return count;
		}

		public void SetShowNonChannel(bool enabled)
		{
			_showNonChannelLogs = enabled;
			NotifyChannelConfigChanged();
		}

		public bool IsShowNonChannel()
		{
			return _showNonChannelLogs;
		}

		void NotifyChannelConfigChanged()
		{
			if (OnChannelConfigChanged != null)
			{
				OnChannelConfigChanged();
			}
		}

		public Result FilterLog(LogInfo logInfo)
		{
			// Checking for channel
			if (logInfo.channelInfo == null)
			{
				if (!_showNonChannelLogs)
				{
					_cachedResult.isPassFilter = false;
					return _cachedResult;
				}
			}
			else
			{
				bool isChannelEnable = false;
				foreach (var channelHash in logInfo.channelInfo.hashes)
				{
					if (_channelConfigs.ContainsKey(channelHash) && _channelConfigs[channelHash] == true)
					{
						isChannelEnable = true;
						break;
					}
				}

				if (!isChannelEnable)
				{
					_cachedResult.isPassFilter = false;
					return _cachedResult;
				}
			}

			// Checking for log type
			if (this._logTypeConfigs.ContainsKey(logInfo.type) && this._logTypeConfigs[logInfo.type] == false)
			{
				_cachedResult.isPassFilter = false;
				return _cachedResult;
			}

			// Checking for filter string
			if (!string.IsNullOrEmpty(this.filterString))
			{
				if (!_stringSearch.IsMatch(logInfo.message, this.filterString))
				{
					if (logInfo.channelInfo == null || !_stringSearch.IsMatch(logInfo.channelInfo.name, this.filterString))
					{
						_cachedResult.isPassFilter = false;
						return _cachedResult;
					}
				}
			}

			if (_isCollapse)
			{
				if (_logHashes.ContainsKey(logInfo.hash))
				{
					_cachedResult.isPassFilter = false;
					_cachedResult.hasCollapsedInstance = true;
					_cachedResult.collapsedLogInfo = _logHashes[logInfo.hash];
					_cachedResult.collapsedLogInfo.numInstance += 1;
				}
				else
				{
					_cachedResult.isPassFilter = true;
					_cachedResult.hasCollapsedInstance = false;
					logInfo.numInstance = 1;
					_logHashes.Add(logInfo.hash, logInfo);
				}
			}
			else
			{
				_cachedResult.isPassFilter = true;
				_cachedResult.hasCollapsedInstance = false;
			}
			
			return _cachedResult;
		}

		static LogFilter _instance;
		public static LogFilter Instance
		{
			get
			{
				if (_instance != null)
				{
					return _instance;
				}
				
				_instance = new LogFilter();
				return _instance;
			}
		}

		public struct Result
		{
			public bool isPassFilter;
			public bool hasCollapsedInstance;
			public LogInfo collapsedLogInfo;
		}

		internal interface IStringSearch
		{
			bool IsMatch(string input, string pattern);
		}

		internal class SimpleStringSearch : IStringSearch
		{
			public bool IsMatch(string input, string pattern)
			{
				return input.IndexOf(pattern, StringComparison.OrdinalIgnoreCase) >= 0;
			}
		}

		internal class RegexSearch : IStringSearch
		{
			RegexOptions _options;
			public RegexSearch(RegexOptions options)
			{
				_options = options;
			}

			public bool IsMatch(string input, string pattern)
			{
				try 
				{
					return Regex.IsMatch(input, pattern, _options);
				}
				catch
				{
					return false;
				}
			}
		}
	}
}