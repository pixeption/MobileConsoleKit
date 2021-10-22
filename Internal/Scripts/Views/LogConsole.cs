using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using MobileConsole.UI;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace MobileConsole
{
	[RequireComponent(typeof(CanvasScaler))]
	public class LogConsole : MonoBehaviour
	{
		private delegate void Callback();
		private delegate void CallbackBool(bool value);
		private static event Callback OnLogConsoleToggled;
		private static event CallbackBool OnRequestLogConsoleVisibilityChange;

		public delegate void EventVisibilityChanged(bool isActive);
		public static event EventVisibilityChanged OnVisibilityChanged;

		private delegate void SubViewCallback(ViewBuilder viewBuilder);
		private static event SubViewCallback OnSubViewPushed;
		private static event Callback OnSubViewPoped;
		private static event Callback OnAllSubViewClosed;
        private static event Callback OnShareAllLogRequest;

		public delegate void EventCommandsCreated(ReadOnlyCollection<Command> commands);
		public static event EventCommandsCreated OnCommandsCreated;
		private static bool _hasCommandsCreated = false;
		private static List<Command> _commands = new List<Command>();

		[SerializeField]
		Vector2 _designLandscapeResolution;

		[SerializeField]
		GameObject _logPanel;

		[SerializeField]
		LogView _logView;

		[SerializeField]
		GenericTreeView _genericView;

		[SerializeField]
		GameObject _logButton;

		[SerializeField]
		GameObject _fpsText;

		SettingViewBuilder _settingViewBuilder;
		CommandViewBuilder _commandViewBuilder;
		ShareLogViewBuilder _shareLogViewBuilder;
		ChannelViewBuilder _channelViewBuilder;

		List<ViewBuilder> _stackViewBuilders = new List<ViewBuilder>();
		RectTransform _logPanelRectTransform;
		RectTransform _rootTransform;
		int _defaultDragThreshold;

		void Awake()
		{
			if (LogConsoleSettings.Instance.hideInHierarchy)
			{
				gameObject.hideFlags = HideFlags.HideInHierarchy;
			}

			DontDestroyOnLoad(gameObject);
			UpdateCanvasScaler();
			InitViewBuilders();
			InitCommands();
		}

		void Start()
		{
			// Need 1 frame for the canvas scaler has effect
			_logPanelRectTransform = _logPanel.GetComponent<RectTransform>();
			_rootTransform = GetComponent<RectTransform>();

			OnRequestLogConsoleVisibilityChange += RequestLogConsoleVisibilityChange;
			OnLogConsoleToggled += ToggleShowLogConsole;
			
			EventBridge.OnWindowSizeChanged += OnWindowSizeChanged;
			OnWindowSizeChanged();

			EventBridge.OnFPSVisibilityChanged += OnFPSVisibilityChanged;
			OnFPSVisibilityChanged();

			OnSubViewPushed = _PushSubView;
			OnSubViewPoped = _PopSubView;
			OnAllSubViewClosed = _CloseAllSubView;
            OnShareAllLogRequest = _OnShareAllLogRequest;
			
			_logButton.SetActive(LogConsoleSettings.Instance.useLogButton);
		}

		void UpdateCanvasScaler()
		{
			CanvasScaler canvasScaler = GetComponent<CanvasScaler>();
			if (Screen.width > Screen.height)
			{
				canvasScaler.referenceResolution = _designLandscapeResolution;
			}
			else
			{
				canvasScaler.referenceResolution = new Vector2(_designLandscapeResolution.y, _designLandscapeResolution.x);
			}
		}

		void InitViewBuilders()
		{
			_shareLogViewBuilder = new ShareLogViewBuilder(_logView.FilterLogInfos);
			_channelViewBuilder = new ChannelViewBuilder();
			_channelViewBuilder.OnHide += OnChannelViewClosed;
		}

		void InitCommands()
		{
			Type[] commandTypes = TypeCollector.GetClassesOfClass(typeof(Command));
			List<Command> executableCommands = new List<Command>();
			List<Command> settingCommands = new List<Command>();

			// Init executable command
			foreach (var commandType in commandTypes)
			{
				ExecutableCommandAttribute attribute = commandType.GetCustomAttribute<ExecutableCommandAttribute>();
				if (attribute != null)
				{
					Command command = (Command)Activator.CreateInstance(commandType);
					command.CacheVariableInfos(commandType);
					command.CacheCustomButtonInfos(commandType, typeof(ButtonAttribute));
					command.info.CopyData(attribute, commandType);
					
					try
					{
						command.InitDefaultVariableValue();
						command.LoadSavedValue();
                        command.OnVariableValueLoaded();
                    }
					catch
					{
						Debug.LogErrorFormat("There is something wrong with command: {0}", commandType.Name);
					}
					
					executableCommands.Add(command);
				}
			}
			_commandViewBuilder = new CommandViewBuilder(executableCommands);
			_commands.AddRange(executableCommands);

			// Init setting command
			foreach (var commandType in commandTypes)
			{
				SettingCommandAttribute attribute = commandType.GetCustomAttribute<SettingCommandAttribute>();
				if (attribute != null)
				{
					Command command = (Command)Activator.CreateInstance(commandType);
					command.CacheVariableInfos(commandType);
					command.info.CopyData(attribute, commandType);

					try
					{
						command.InitDefaultVariableValue();
						command.LoadSavedValue();
                        command.OnVariableValueLoaded();
                    }
					catch
					{
						Debug.LogErrorFormat("There is something wrong with command: {0}", commandType.Name);
					}

					settingCommands.Add(command);
				}
			}

			_settingViewBuilder = new SettingViewBuilder(settingCommands);
			_commands.AddRange(settingCommands);


			// Notify event
			_hasCommandsCreated = true;
			if (OnCommandsCreated != null)
			{
				OnCommandsCreated(_commands.AsReadOnly());
			}
		}

		[System.Obsolete("This is an obsolete method, please use HasCommandsCreated instead")]
		public static bool IsCommandsCreated()
		{
			return HasCommandsCreated();
		}

		public static bool HasCommandsCreated()
		{
            return _hasCommandsCreated;
		}

		public static ReadOnlyCollection<Command> GetCommands()
		{
			return _commands.AsReadOnly();
		}

		public static T GetCommand<T>() where T : Command
		{
			Type commandType = typeof(T);
            return (T)_commands.Find(cm => cm.GetType() == commandType);
		}

		public static void PushSubView(ViewBuilder viewBuilder)
		{
			if (OnSubViewPushed != null)
			{
				OnSubViewPushed(viewBuilder);
			}
		}

		public static void PopSubView()
		{
			if (OnSubViewPoped != null)
			{
				OnSubViewPoped();
			}
		}

		public static void CloseAllSubView()
		{
			if (OnAllSubViewClosed != null)
			{
				OnAllSubViewClosed();
			}
		}

		void _PushSubView(ViewBuilder viewBuilder)
		{
			if (viewBuilder == null)
				throw new System.Exception("Node View Builder is null");
			
			if (_stackViewBuilders.Count == 0)
			{
				_logView.Hide();
			}

			viewBuilder.level = _stackViewBuilders.Count;

			_stackViewBuilders.Add(viewBuilder);
			_genericView.Show(viewBuilder);
		}

		void _PopSubView()
		{
			if (_stackViewBuilders.Count == 0)
				return;

			_stackViewBuilders.Remove(_stackViewBuilders.Last());

			if (_stackViewBuilders.Count > 0)
			{
				_genericView.Show(_stackViewBuilders.Last());
			}
			else
			{
				_genericView.Hide();
				_logView.Show();
			}
		}

		void _CloseAllSubView()
		{
			_stackViewBuilders.Clear();
			_genericView.Hide();
			_logView.Show();
		}

        public static void ShareAllLogs()
		{
			if (OnShareAllLogRequest != null)
				OnShareAllLogRequest();
		}

		void _OnShareAllLogRequest()
		{
			_shareLogViewBuilder.OnShareLog();
		}

		public void OpenShareLogView()
		{
			_PushSubView(_shareLogViewBuilder);
		}

		public void OpenChannelView()
		{
			_PushSubView(_channelViewBuilder);
		}

		void OnChannelViewClosed()
		{
			if (_channelViewBuilder.IsFilterChanged())
			{
				_logView.UpdateFilteredLog();
			}
		}

		public void OpenSettingView()
		{
			_PushSubView(_settingViewBuilder);
		}

		public void OpenCommandView()
		{
			_PushSubView(_commandViewBuilder);
		}

		void RequestLogConsoleVisibilityChange(bool visibility)
		{
			if (visibility ^ _logPanel.activeSelf)
			{
				ToggleShowLogConsole();
			}
		}

		public void ToggleShowLogConsole()
		{
			_logPanel.SetActive(!_logPanel.activeSelf);

			// Adjust drag threshold on Android
			if (Application.platform == RuntimePlatform.Android)
			{
				if (_logPanel.activeSelf)
				{
					_defaultDragThreshold = EventSystem.current.pixelDragThreshold;
					EventSystem.current.pixelDragThreshold = Mathf.Max(_defaultDragThreshold, (int)(_defaultDragThreshold * Screen.dpi / 160f));
				}
				else
				{
					// Restore to game value
					EventSystem.current.pixelDragThreshold = _defaultDragThreshold;
				}
			}

			// Event when show/hide Log Console
            if (OnVisibilityChanged != null)
			{
                OnVisibilityChanged(_logPanel.activeSelf);
			}
		}

		void OnWindowSizeChanged()
		{
			float width = LogConsoleSettings.Instance.windowWidth;
			float height = LogConsoleSettings.Instance.windowHeight;
			float x = LogConsoleSettings.Instance.windowPosX;
			float y = LogConsoleSettings.Instance.windowPosY;

			Vector2 unfilledRegion = new Vector2((1.0f - width) * _rootTransform.sizeDelta.x, (1.0f - height) * _rootTransform.sizeDelta.y);
			_logPanelRectTransform.sizeDelta = -unfilledRegion;
			_logPanelRectTransform.anchoredPosition = new Vector2(Mathf.Lerp(-unfilledRegion.x / 2, unfilledRegion.x / 2, x),
																  Mathf.Lerp(-unfilledRegion.y / 2, unfilledRegion.y / 2, y));
		}

		void OnFPSVisibilityChanged()
		{
			_fpsText.SetActive(LogConsoleSettings.Instance.showFPS);
		}

		public static void Show(bool isShow = true)
		{
			if (OnRequestLogConsoleVisibilityChange != null)
			{
				OnRequestLogConsoleVisibilityChange(isShow);
			}
		}

		public static void Hide()
		{
			Show(false);
		}

		public static void ToggleShow()
		{
            if (OnLogConsoleToggled != null)
            {
                OnLogConsoleToggled();
            }
		}

#if ENABLE_LEGACY_INPUT_MANAGER
		// Handle Android back button
		void Update()
		{
			if (Application.platform == RuntimePlatform.IPhonePlayer)
				return;
			
            if (Input.GetKeyUp(KeyCode.Escape))
            {
				if (_stackViewBuilders.Count > 0)
				{
					_PopSubView();
				}
				else if (_logPanel.activeSelf)
				{
					ToggleShow();
				}
            }
		}
	}
#endif
}