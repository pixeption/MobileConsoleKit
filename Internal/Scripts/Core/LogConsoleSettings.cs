using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;

namespace MobileConsole
{
	[CreateAssetMenu(menuName = "Mobile Console/Log Console Settings", fileName = "LogConsoleSettings")]
	public class LogConsoleSettings : ScriptableObject
	{
        [Header("Startup")]
        public bool autoStartup = true;
		public bool hideInHierarchy = true;
		public bool useLogButton = true;


		[Header("Log")]
		public bool showTimestamp = true;
		public string timeFormat = "[hh:mm:ss.ff]  ";
		public bool showLogChannel = true;
		public string channelFormat = "[{0}]";
		public char channelSeparator = ',';
		[System.NonSerialized]
        public string fullChannelFormat;
		[System.NonSerialized]
        public string channelRegex;

		[Header("FPS")]
		public bool showFPS = true;

		[Header("Search")]
		public bool useRegexSearch = true;

		[EnumFlags]
		public RegexOptions regexSearchOptions = RegexOptions.IgnoreCase;

		[Header("Share Log - Window and Mac")]
		public bool useShareLogViaMail = true;
		public string mailSubject;
		public string[] mailRecipients;
		public const string mailTemplate = @"mailto:?to={0}&subject={1}&body={2}";

		[Header("UI")]
		[SerializeField]
		float _treeViewOffset = 60;

		[Range(0.25f, 1.0f)]
		public float windowWidth = 1.0f;
		
		[Range(0.25f, 1.0f)]
		public float windowHeight = 1.0f;

		[Range(0.0f, 1.0f)]
		public float windowPosX = 0.0f;

		[Range(0.0f, 1.0f)]
		public float windowPosY = 0.0f;
		
		[Range(0.1f, 1.0f)]
		[SerializeField]
		float _backgroundTransparency = 1.0f;
		public float backgroundTransparency
		{
			get { return _backgroundTransparency; }
			set {
				_backgroundTransparency = value;
				UpdateCellAlpha();
			}
		}

		[Header("Color")]
		[SerializeField]
        Color _oddCellColor = new Color(0.149f, 0.149f, 0.149f);
		Color _finalOddCelColor;
		
		[SerializeField]
        Color _evenCellColor = new Color(0.180f, 0.180f, 0.180f);
		Color _finalEvenCelColor;

		[Header("Views")]
		public bool useCategoryColor = true;

		void Init()
		{
			fullChannelFormat = channelFormat + " " + "{1}";

			// If channelFormat = "[{0}]" then channelRegex = "^\[(.*?)\] ";
			channelRegex = string.Format(@"^\" + channelFormat.Trim() + " ?", @"(.*?)\");
			
			// Update cell color
			_finalEvenCelColor = _evenCellColor;
			_finalOddCelColor = _oddCellColor;
			UpdateCellAlpha();
		}
		
		public static Color GetCellColor(int index)
		{
			return index % 2 == 0 ? Instance._finalEvenCelColor : Instance._finalOddCelColor;  
		}

		void UpdateCellAlpha()
		{
			_finalEvenCelColor.a = _backgroundTransparency;
			_finalOddCelColor.a = _backgroundTransparency;
		}

		public static float GetTreeViewOffsetByLevel(int level)
		{
			return Instance._treeViewOffset * level;
		}

		static LogConsoleSettings _instance;
		public static LogConsoleSettings Instance
		{
			get
			{
				if (_instance)
				{
					return _instance;
				}
				_instance = Resources.Load<LogConsoleSettings>("LogConsoleSettings");
				if (_instance == null)
				{
					_instance = ScriptableObject.CreateInstance<LogConsoleSettings>();
				}

				_instance.Init();
				return _instance;
			}
		}
	}
}