#if UNITY_IOS
using System.Runtime.InteropServices;
#endif
using System;
using System.Text;
using UnityEngine;

namespace MobileConsole
{
    /// <summary>
    /// https://github.com/ChrisMaire/unity-native-sharing
    /// </summary>
    public static class NativeShare
    {
        /// <summary>
        /// Shares on file maximum
        /// </summary>
        /// <param name="body"></param>
        /// <param name="filePath">The path to the attached file</param>
        /// <param name="url"></param>
        /// <param name="subject"></param>
        /// <param name="mimeType"></param>
        /// <param name="chooser"></param>
        /// <param name="chooserText"></param>
        public static void Share(string body, string filePath = null, string url = null, string subject = "", string mimeType = "text/plain", bool chooser = true, string chooserText = "Select Sharing App")
        {
            string[] filePaths = (filePath == null) ? null : new string[] { filePath };
            ShareMultiple(body, filePaths, url, subject, mimeType, chooser, chooserText);
        }

        /// <summary>
        /// Shares multiple files at once
        /// </summary>
        /// <param name="body"></param>
        /// <param name="filePaths">The paths to the attached files</param>
        /// <param name="url"></param>
        /// <param name="subject"></param>
        /// <param name="mimeType"></param>
        /// <param name="chooser"></param>
        /// <param name="chooserText"></param>
        public static void ShareMultiple(string body, string[] filePaths = null, string url = null, string subject = "", string mimeType = "text/plain", bool chooser = true, string chooserText = "Select Sharing App")
        {
#if UNITY_EDITOR
			// Do nothing
#elif UNITY_ANDROID
			ShareAndroid(body, subject, url, filePaths, mimeType, chooser, chooserText);
#elif UNITY_IOS
            ShareIOS(body, subject, url, filePaths);
#endif
        }

#if UNITY_ANDROID
	public static void ShareAndroid(string body, string subject, string url, string[] filePaths, string mimeType, bool chooser, string chooserText)
	{
        using (AndroidJavaClass unity = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
        using (AndroidJavaObject currentActivity = unity.GetStatic<AndroidJavaObject>("currentActivity"))
        using (AndroidJavaClass nativeShareClass = new AndroidJavaClass("com.pixeption.mck.NativeShare"))
        {
            nativeShareClass.CallStatic("Share", currentActivity, body, subject, url, filePaths, mimeType, chooser, chooserText);
        }
	}
#endif

#if UNITY_IOS
        public struct ConfigStruct
        {
            public string title;
            public string message;
        }

        [DllImport("__Internal")] private static extern void showAlertMessage(ref ConfigStruct conf);

        public struct SocialSharingStruct
        {
            public string text;
            public string subject;
            public string filePaths;
        }

        [DllImport("__Internal")] private static extern void showSocialSharing(ref SocialSharingStruct conf);

        public static void ShareIOS(string title, string message)
        {
            ConfigStruct conf = new ConfigStruct();
            conf.title = title;
            conf.message = message;
            showAlertMessage(ref conf);
        }

        public static void ShareIOS(string body, string subject, string url, string[] filePaths)
        {
            SocialSharingStruct conf = new SocialSharingStruct();
            conf.text = body;
            string paths = (filePaths == null) ? string.Empty : string.Join(";", filePaths);
            if (string.IsNullOrEmpty(paths))
                paths = url;
            else if (!string.IsNullOrEmpty(url))
                paths += ";" + url;
            conf.filePaths = paths;
            conf.subject = subject;

            showSocialSharing(ref conf);
        }
#endif
    }
}