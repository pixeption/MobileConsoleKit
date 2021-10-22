using UnityEngine;

namespace MobileConsole
{
	public static class BundleIdentifier
	{
		public static string GetAppIdentifier()
		{
#if UNITY_5_6_OR_NEWER
			return Application.identifier;
#else
			return Application.bundleIdentifier;
#endif
		}
	}
}