using System.Text;
using UnityEngine;

namespace MobileConsole
{
	public static class AppAndDeviceInfo
	{
		public static string FullInfos()
		{
			StringBuilder sb = new StringBuilder();
			sb.AppendLine("--- App Info ---");
			sb.AppendLine("App name: " + Application.productName);
			sb.AppendLine("Bundle identifier: " + EventBridge.AppBundleIdentifier);
			sb.AppendFormat("App version: {0} ({1})\n", Application.version, EventBridge.AppVersionCode);
			sb.AppendLine("Unity version: " + Application.unityVersion);

			// Device info
			sb.AppendLine();
			sb.AppendLine("--- Device Info ---");
			sb.AppendLine("Device name: " + SystemInfo.deviceName);
			sb.AppendLine("Device model: " + SystemInfo.deviceModel);
			sb.AppendLine("Operation system: " + SystemInfo.operatingSystem);
			sb.AppendLine("System language: " + Application.systemLanguage.ToString());
			sb.AppendLine("Device orientation: " + Input.deviceOrientation.ToString());
			if (Application.internetReachability == NetworkReachability.NotReachable)
			{
				sb.AppendLine("Connectivity: None");
			}
			else if (Application.internetReachability == NetworkReachability.ReachableViaCarrierDataNetwork)
			{
				sb.AppendLine("Connectivity: Carrier Data Network (Cellular)");
			}
			else if (Application.internetReachability == NetworkReachability.ReachableViaLocalAreaNetwork)
			{
				sb.AppendLine("Connectivity: Local (LAN/Wifi)");
			}

			sb.AppendLine();
			sb.AppendLine("--- CPU Info ---");
			sb.AppendFormat("CPU type: {0} ({1} core(s))\n", SystemInfo.processorType, SystemInfo.processorCount);
			if (Application.platform != RuntimePlatform.IPhonePlayer && Application.platform != RuntimePlatform.WebGLPlayer)
			{
				sb.AppendFormat("CPU speed: {0} MHz\n", SystemInfo.processorFrequency);
			}
			sb.AppendFormat("System memory size: {0} MB\n", SystemInfo.systemMemorySize);
			sb.AppendFormat("Allocated memory: {0} MB\n", EventBridge.RequestAllocatedMemory().ToString("n2"));
			sb.AppendFormat("Reserved memory: {0} MB\n", EventBridge.RequestReservedMemory().ToString("n2"));
			sb.AppendFormat("Mono used memory: {0} MB\n", EventBridge.RequestMonoUsedMemory().ToString("n2"));
			
			sb.AppendLine();
			sb.AppendLine("--- GPU Info ---");
			sb.AppendLine("GPU: " + SystemInfo.graphicsDeviceName);
			sb.AppendFormat("Graphic memory size: {0} MB\n", SystemInfo.graphicsMemorySize);
			sb.AppendFormat("Screen size: {0}x{1}@{2}Hz\n", Screen.currentResolution.width, Screen.currentResolution.height, Screen.currentResolution.refreshRate);
			sb.AppendLine("Screen dpi: " + Screen.dpi);

			return sb.ToString();
		}
	}
}