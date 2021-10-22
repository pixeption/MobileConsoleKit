using System.Collections.Generic;

namespace MobileConsole
{
	public static class LogChannelInfoCache 
	{
		public delegate void CallbackNewChannelsAdded(int[] channelHashes);
		internal static event CallbackNewChannelsAdded OnNewChannelsAdded;

		static Dictionary<int, string> _channels = new Dictionary<int, string>();
		static Dictionary<string, LogChannelInfo> _logChannelInfos = new Dictionary<string, LogChannelInfo>();

		public static LogChannelInfo GetOrCreateChannelInfo(string strChannels)
		{
			if (!_logChannelInfos.ContainsKey(strChannels))
			{
				string[] channelNames = strChannels.Split(',');
				int[] channelHashes = new int[channelNames.Length];
				for (int i = 0; i < channelNames.Length; i++)
				{
					channelHashes[i] = channelNames[i].GetHashCode();
					
					if (!_channels.ContainsKey(channelHashes[i]))
					{
						_channels[channelHashes[i]] = channelNames[i];
					}
				}

				LogChannelInfo channelInfo = new LogChannelInfo();
				channelInfo.name = strChannels;
				channelInfo.nameWithColor = GetNameWithColors(channelNames);
				channelInfo.hashes = channelHashes;
				_logChannelInfos.Add(strChannels, channelInfo);

				if (OnNewChannelsAdded != null)
				{
					OnNewChannelsAdded(channelHashes);
				}
			}

			return _logChannelInfos[strChannels];
		}

		public static Dictionary<int, string> GetChannels()
		{
			return _channels;
		}

		public static string GetChannelName(int hash)
		{
			if (_channels.ContainsKey(hash))
			{
				return _channels[hash];
			}

			return string.Empty;
		}

		static string GetNameWithColors(string[] channels)
		{
			for (int i = 0; i < channels.Length; i++)
			{
				channels[i] = TextColors.GetUniqueColorWithTag(channels[i]);
			}

			return string.Format(LogConsoleSettings.Instance.channelFormat, string.Join(",", channels));
		}
	}
}