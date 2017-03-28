using System;
using UnityEngine;
using System.Collections;
using System.Runtime.InteropServices;

public class DeviceService
{
    public static string GetPlatformId ()
    {
        string platformId;
        #if UNITY_IPHONE && !UNITY_EDITOR
        platformId = "1";
        #elif UNITY_ANDROID && !UNITY_EDITOR
        platformId = "2";
        #elif UNITY_IPHONE && UNITY_EDITOR
        platformId = "1";
        #elif UNITY_ANDROID && UNITY_EDITOR
        platformId = "2";
        #else
        platformId = "0";
        #endif

        return platformId;
    }
	#if UNITY_IPHONE && !UNITY_EDITOR
//		[DllImport("__Internal")]
//		private static extern float getHeight ();
//		[DllImport("__Internal")]
//		private static extern float getWidth ();
//		[DllImport("__Internal")]
//		private static extern float getScale ();
		[DllImport("__Internal")]
		private static extern string getBuildVersion();
//		[DllImport("__Internal")]
//		private static extern void googleAdInstall();
//		[DllImport("__Internal")]
//		private static extern void googleAdPayment(string payment);
//
//		public static float ScreenHeight () {
//			return getHeight ();
//		}
//
//		public static float ScreenWidth () {
//			return getWidth ();
//		}
//
//		public static float Scale () {
//			return getScale ();
//		}
//
//		/// <summary>
//		/// iOS版でのバージョンを取得する
//		/// </summary>
		public static string GetAppVersion () {
			string versionName = getBuildVersion ();
			return versionName;
		}
//
//		public static void GoogleAdInstall () {
//			googleAdInstall();
//		}
//
//		public static void GoogleAdPayment (string payment) {
//			googleAdPayment(payment);
//		}
	#elif UNITY_ANDROID && !UNITY_EDITOR
		public static float ScreenHeight () {
			return (float)Screen.height;
		}

		public static float ScreenWidth () {
			return (float)Screen.width;
		}
//
//		public static float Scale () {
//			return 1.0f;
//		}
//
//		/// <summary>
//		/// バージョンネームを取得する
//		/// PlayerSettings上では[ Bundle Version ]の値
//		/// </summary>
		public static string GetAppVersion () {
			AndroidJavaObject pInfo = GetPackageInfo();
			string versionName = pInfo.Get<string>( "versionName" );
			return versionName;
		}

		/// <summary>
		/// Version情報を保持しているPackageInfoクラスを取得する
		/// </summary>
		private static AndroidJavaObject GetPackageInfo() {
			AndroidJavaClass unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
			AndroidJavaObject context = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity").Call<AndroidJavaObject>("getApplicationContext");
			AndroidJavaObject pManager = context.Call<AndroidJavaObject>("getPackageManager");
			AndroidJavaObject pInfo = pManager.Call<AndroidJavaObject>( "getPackageInfo", context.Call<string>("getPackageName"), pManager.GetStatic<int>("GET_ACTIVITIES") );

			return pInfo;
		}
//
//		public static void GoogleAdInstall () {
//			AndroidJavaClass unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
//			AndroidJavaObject context = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity").Call<AndroidJavaObject>("getApplicationContext");
//			AndroidJavaClass instance = new AndroidJavaClass ("com.google.ads.conversiontracking.AdWordsConversionReporter");
//			instance.CallStatic("reportWithConversionId", context, "956014568", "Rh7oCO-PzFsQ6L_uxwM", "0", false);
//		}
//
//		public static void GoogleAdPayment (string payment) {
//			AndroidJavaClass unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
//			AndroidJavaObject context = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity").Call<AndroidJavaObject>("getApplicationContext");
//			AndroidJavaClass instance = new AndroidJavaClass ("com.google.ads.conversiontracking.AdWordsConversionReporter");
//			instance.CallStatic("reportWithConversionId", context, "956014568", "H7iNCKS72VwQ6L_uxwM", payment, true);
//		}
	#else
		public static float ScreenHeight () {
			return (float)Screen.height;
		}

		public static float ScreenWidth () {
			return (float)Screen.width;
		}

		public static float Scale () {
			return 1.0f;
		}
		public static string GetAppVersion () {
			return UnityEditor.PlayerSettings.bundleVersion;
		}
	#endif
}
