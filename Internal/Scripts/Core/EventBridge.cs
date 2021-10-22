namespace MobileConsole
{
	public static class EventBridge
	{
		public delegate void Callback();
		public delegate void CallbackBool(bool value);
		public delegate void CallbackFloat(float value);
        public delegate void CallbackString(string value);
        public delegate void CallbackStrings(string[] values);
        public delegate float CallbackReturnFloat();

        internal static event Callback OnWindowSizeChanged;
        internal static event Callback OnTimestampVisibilityChanged;
		internal static event Callback OnChannelVisibilityChanged;
        internal static event Callback OnFPSVisibilityChanged;
		public static event Callback OnBackgroundTransparencyChanged;
        public static event CallbackString OnShareLog;
		public static event CallbackString OnShareAllLog;
        public static event CallbackStrings OnShareFiles;
        public static event CallbackReturnFloat OnRequestAllocatedMemory;
		public static event CallbackReturnFloat OnRequestReservedMemory;
		public static event CallbackReturnFloat OnRequestMonoUsedMemory;
		public static string AppVersionCode;
		public static string AppBundleIdentifier;

        public static void NotifyWindowSizeChanged()
		{
			if (OnWindowSizeChanged != null)
			{
				OnWindowSizeChanged();
			}
		}

        public static void NotifyTimestampVisibilityChanged()
		{
			if (OnTimestampVisibilityChanged != null)
			{
				OnTimestampVisibilityChanged();
			}	
		}

		public static void NotifyChannelVisibilityChanged()
		{
			if (OnChannelVisibilityChanged != null)
			{
				OnChannelVisibilityChanged();
			}
		}

        public static void NotifyFPSVisibilityChanged()
		{
			if (OnFPSVisibilityChanged != null)
			{
				OnFPSVisibilityChanged();
			}
		}

		public static void NotifyBackgroundTransparencyChanged()
		{
			if (OnBackgroundTransparencyChanged != null)
			{
				OnBackgroundTransparencyChanged();
			}
		}

        public static void NotifyShareLog(string value)
        {
            if (OnShareLog != null)
            {
                OnShareLog(value);
            }
        }

		public static void NotifyShareAllLog(string value)
		{
			if (OnShareAllLog != null)
			{
				OnShareAllLog(value);
			}
		}

        public static void NotifyShareFiles(params string[] values)
        {
            if (OnShareFiles != null)
            {
                OnShareFiles(values);
            }
        }

		public static float RequestAllocatedMemory()
		{
			if (OnRequestAllocatedMemory != null)
			{
				return OnRequestAllocatedMemory();
			}

			return 0f;
		}

		public static float RequestReservedMemory()
		{
			if (OnRequestReservedMemory != null)
			{
				return OnRequestReservedMemory();
			}

			return 0f;
		}

		public static float RequestMonoUsedMemory()
		{
			if (OnRequestMonoUsedMemory != null)
			{
				return OnRequestMonoUsedMemory();
			}

			return 0f;
		}
	}
}