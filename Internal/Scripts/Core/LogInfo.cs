using UnityEngine;

namespace MobileConsole
{
	public class LogInfo
	{
		public int hash;
		public LogType type;
		public string shortMessage;
		public string message;
		public string stackTrace;
		public LogChannelInfo channelInfo;
        public string time;
		public int numInstance;
	}
}