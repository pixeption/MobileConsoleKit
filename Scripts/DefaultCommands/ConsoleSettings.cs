using UnityEngine;

namespace MobileConsole
{
	[SettingCommand(name = "Console")]
	public class ConsoleSettings : Command
	{
		[Range(0.4f, 1.0f)]
		[RelativeSlider]
		[Variable(OnValueChanged = "UpdateWindowSize")]
		public float _windowWidth = 1.0f;

		[Range(0.4f, 1.0f)]
		[RelativeSlider]
		[Variable(OnValueChanged = "UpdateWindowSize")]
		public float _windowHeight = 1.0f;

		[Range(0.0f, 1.0f)]
		[RelativeSlider]
		[Variable(OnValueChanged = "UpdateWindowSize")]
		public float _windowPosX = 1.0f;

		[Range(0.0f, 1.0f)]
		[RelativeSlider]
		[Variable(OnValueChanged = "UpdateWindowSize")]
		public float _windowPosY = 1.0f;

		[Range(0.1f, 1.0f)]
		[Variable(OnValueChanged = "UpdateBackgroundTransparency")]
		public float _backgroundTransparency = 1.0f;

		[Dropdown("TimeScaleOptions")]
		[Variable(OnValueChanged = "UpdateTimeScale")]
		public float _timeScale = 1.0f;

		[Variable(OnValueChanged = "UpdateShowFPS")]
		public bool _showFPS;

		[Variable(OnValueChanged = "UpdateShowTimestamp")]
		public bool _showTimestamp;

		[Variable(OnValueChanged = "UpdateShowChannel")]
		public bool _showChannel;

		public override void InitDefaultVariableValue()
		{
			_showFPS = LogConsoleSettings.Instance.showFPS;
			_showTimestamp = LogConsoleSettings.Instance.showTimestamp;
			_showChannel = LogConsoleSettings.Instance.showLogChannel;
			_windowWidth = LogConsoleSettings.Instance.windowWidth;
			_windowHeight = LogConsoleSettings.Instance.windowHeight;
			_windowPosX = LogConsoleSettings.Instance.windowPosX;
			_windowPosY = LogConsoleSettings.Instance.windowPosY;
			_backgroundTransparency = LogConsoleSettings.Instance.backgroundTransparency;
		}

		void UpdateTimeScale()
		{
			Time.timeScale = _timeScale;
		}

		void UpdateShowFPS()
		{
			LogConsoleSettings.Instance.showFPS = _showFPS;
			EventBridge.NotifyFPSVisibilityChanged();
		}

		void UpdateShowTimestamp()
		{
			LogConsoleSettings.Instance.showTimestamp = _showTimestamp;
			EventBridge.NotifyTimestampVisibilityChanged();
		}

		void UpdateShowChannel()
		{
			LogConsoleSettings.Instance.showLogChannel = _showChannel;
			EventBridge.NotifyChannelVisibilityChanged();
		}

		void UpdateWindowSize()
		{
			LogConsoleSettings.Instance.windowWidth = _windowWidth;
			LogConsoleSettings.Instance.windowHeight = _windowHeight;
			LogConsoleSettings.Instance.windowPosY = _windowPosY;
			LogConsoleSettings.Instance.windowPosX = _windowPosX;

			EventBridge.NotifyWindowSizeChanged();
		}

		void UpdateBackgroundTransparency()
		{
			LogConsoleSettings.Instance.backgroundTransparency = _backgroundTransparency;
			EventBridge.NotifyBackgroundTransparencyChanged();
		}

		float[] TimeScaleOptions()
		{
			return new float[] 
			{
				0.25f,
				0.5f,
				1.0f,
				1.5f,
				2.0f,
				2.5f,
				3.0f,
				4.0f,
				6.0f
			};
		}
	}
}