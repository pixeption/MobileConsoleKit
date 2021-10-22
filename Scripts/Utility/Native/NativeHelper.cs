using System.Runtime.InteropServices;
using UnityEngine;

#if UNITY_EDITOR_WIN || UNITY_STANDALONE_WIN
#if !NET_STANDARD_2_0
using Microsoft.Win32;
#else
#endif
#endif

namespace MobileConsole
{
    public static class NativeHelper
    {
#if UNITY_IOS
		[DllImport("__Internal")]
		static extern string _NHGetVersionCode();
#endif
        public static string GetAppVersionCode()
        {
#if UNITY_EDITOR
			return "Editor";
#elif UNITY_ANDROID
			AndroidJavaClass contextCls = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
			AndroidJavaObject context = contextCls.GetStatic<AndroidJavaObject>("currentActivity");
			AndroidJavaObject packageMngr = context.Call<AndroidJavaObject>("getPackageManager");
			string packageName = context.Call<string>("getPackageName");
			AndroidJavaObject packageInfo = packageMngr.Call<AndroidJavaObject>("getPackageInfo", packageName, 0);
			return packageInfo.Get<int>("versionCode").ToString();
#elif UNITY_IOS
            return _NHGetVersionCode();
#else
            return "NaN";
#endif
        }


#if UNITY_EDITOR_OSX || UNITY_STANDALONE_OSX
		[DllImport("NativeHelper")]
		static extern string _NHGetAllPlayerPrefsKeys(string identifier);
#elif UNITY_IOS
		[DllImport("__Internal")]
		static extern string _NHGetAllPlayerPrefsKeys(string identifier);
#endif

        public static string[] GetAllPlayerPrefsKeys()
        {
#if UNITY_EDITOR_WIN || UNITY_STANDALONE_WIN
#if !NET_STANDARD_2_0
			RegistryKey registryKey = Registry.CurrentUser.OpenSubKey(GetPlayerPrefsIdentifier());
			if (registryKey == null)
				return new string[0];
			
			string[] keys = registryKey.GetValueNames();
			string[] actualKeys = new string[keys.Length];
			
			for (int i = 0; i < keys.Length; i++)
			{
				int lastDashIndex = keys[i].LastIndexOf('_');
				actualKeys[i] = keys[i].Remove(lastDashIndex);
			}

			return actualKeys;
#else
			return new string[0];
#endif

#elif UNITY_IOS || UNITY_STANDALONE_OSX || UNITY_EDITOR_OSX
			string joinedKeys = _NHGetAllPlayerPrefsKeys(GetPlayerPrefsIdentifier());
			string[] keys = joinedKeys.Split(',');
			return keys;

#elif UNITY_ANDROID
			AndroidJavaClass unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
			AndroidJavaObject activity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");
			AndroidJavaObject context = activity.Call<AndroidJavaObject>("getApplicationContext");

			AndroidJavaClass nativeHelper = new AndroidJavaClass("com.pixeption.mck.NativeHelper");
			return nativeHelper.CallStatic<string[]>("GetPlayerPrefsKeys", context);
#else
            return new string[0];
#endif
        }

        static string GetPlayerPrefsIdentifier()
        {
#if UNITY_STANDALONE_OSX || UNITY_EDITOR_OSX
			return string.Format("{0}.{1}.{2}", "unity", Application.companyName, Application.productName);
#elif UNITY_IOS
			return EventBridge.AppBundleIdentifier;
#elif UNITY_EDITOR_WIN
			return string.Format(@"Software\Unity\UnityEditor\{0}\{1}", Application.companyName, Application.productName);
#elif UNITY_STANDALONE_WIN
			return string.Format(@"Software\{0}\{1}", Application.companyName, Application.productName);
#else
            return string.Empty;
#endif
        }
    }
}