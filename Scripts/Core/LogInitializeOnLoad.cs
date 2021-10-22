#if DebugLog
using System;
using System.IO;
using UnityEngine;
using UnityEngine.Profiling;
using UnityEngine.SceneManagement;

namespace MobileConsole
{

	static class LogInitializeOnLoad
	{
		[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
		static void OnInitializeOnLoad()
		{
			// Init share log
			EventBridge.OnShareLog += ShareLog;
			EventBridge.OnShareAllLog += ShareAllLogs;
			EventBridge.OnShareFiles += ShareFiles;
			EventBridge.AppVersionCode = NativeHelper.GetAppVersionCode();
			EventBridge.AppBundleIdentifier = BundleIdentifier.GetAppIdentifier();
			EventBridge.OnRequestAllocatedMemory += AllocatedMemory;
			EventBridge.OnRequestReservedMemory += ReservedMemory;
			EventBridge.OnRequestMonoUsedMemory += MonoUsedMemory;

			// Init log receiver
			SceneManager.sceneLoaded += OnSceneLoaded;
			LogReceiver.Init();
		}

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
		static void LoadConsoleScene()
		{
            if (LogConsoleSettings.Instance.autoStartup)
            {
                SceneManager.LoadSceneAsync("LogConsole", LoadSceneMode.Additive);
            }
		}

		static void OnSceneLoaded(Scene scene, LoadSceneMode mode)
		{
			if (scene.name == "LogConsole")
			{
				SceneManager.sceneLoaded -= OnSceneLoaded;
				SceneManager.UnloadSceneAsync(scene);
			}
		}

		static void ShareLog(string log)
		{
#if UNITY_EDITOR || UNITY_STANDALONE
			TextEditor textEditor = new TextEditor();
			textEditor.text = log;
			textEditor.SelectAll();
			textEditor.Copy();
			Debug.Log("Log has been coppied to Clipboard");
#elif UNITY_IOS || UNITY_ANDROID
			NativeShare.Share(log);
#endif
		}

		static void ShareAllLogs(string logs)
		{
			// Android intent limit size is 1MB (https://stackoverflow.com/questions/39098590)
			// Thus we save it as a file then share the file instead
			string fileName = string.Format("{0}_{1}({2})_{3}.log",
				Application.productName,
				Application.version,
				EventBridge.AppVersionCode,
				DateTime.Now.ToString("MMMM-dd_HH-mm-ss-ff"));
			string folderPath = Path.Combine(Application.persistentDataPath, "MCK_Logs");
			string filePath = Path.Combine(folderPath, fileName);

			// Make sure the directory is created
			Directory.CreateDirectory(folderPath);
			File.WriteAllText(filePath, logs);

#if UNITY_EDITOR || UNITY_STANDALONE
			if (LogConsoleSettings.Instance.useShareLogViaMail)
				ShareLogToEmailClient(logs);
#elif UNITY_IOS || UNITY_ANDROID
			NativeShare.ShareMultiple("", new string[] {filePath});
#endif

#if UNITY_EDITOR
			Debug.LogFormat("Logs are saved at: {0}\n You can quickly open the log folder by <b>Tools > Mobile Console > Open Log Folder </b>", filePath);
#endif
		}

        static void ShareFiles(string[] filePaths)
        {
            if (filePaths == null || filePaths.Length == 0)
                return;

            NativeShare.ShareMultiple("", filePaths);
        }

		static void ShareLogToEmailClient(string logs)
		{
			string recipients = string.Empty;
			if (LogConsoleSettings.Instance.mailRecipients != null)
			{
				recipients = string.Join(",", LogConsoleSettings.Instance.mailRecipients);
			}

			string fullUrl = string.Format(LogConsoleSettings.mailTemplate,
									EscapeUrl(recipients),
									EscapeUrl(LogConsoleSettings.Instance.mailSubject),
									EscapeUrl(logs));
			
			Application.OpenURL(fullUrl);
		}

		static string EscapeUrl(string content)
		{
#if UNITY_2018_3_OR_NEWER
			return UnityEngine.Networking.UnityWebRequest.EscapeURL(content).Replace("+", "%20");
#else
			return WWW.EscapeURL(content).Replace("+", "%20");
#endif		
		}

		static float AllocatedMemory()
		{
#if UNITY_5_6_OR_NEWER
			return Profiler.GetTotalAllocatedMemoryLong() / 1048576f;
#else
			return Profiler.GetTotalAllocatedMemory() / 1048576f;
#endif
		}

		static float ReservedMemory()
		{
#if UNITY_5_6_OR_NEWER
			return Profiler.GetTotalReservedMemoryLong() / 1048576f;
#else
			return Profiler.GetTotalReservedMemory() / 1048576f;
#endif
		}

		static float MonoUsedMemory()
		{
#if UNITY_5_6_OR_NEWER
			return Profiler.GetMonoUsedSizeLong() / 1048576f;
#else
			return Profiler.GetMonoUsedSize() / 1048576f;
#endif
		}
	}
}
#endif