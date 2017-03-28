﻿namespace NendUnityPlugin.Platform
{
	internal class NendAdNativeInterfaceFactory
	{
		internal static NendAdBannerInterface CreateBannerAdInterface ()
		{
			#if UNITY_IPHONE && !UNITY_EDITOR
			return new NendUnityPlugin.Platform.iOS.IOSBannerInterface();
			#elif UNITY_ANDROID && !UNITY_EDITOR
			return new NendUnityPlugin.Platform.Android.AndroidBannerInterface();
			#else
			return new BannerStub ();
			#endif
		}
			
		#if UNITY_ANDROID
		internal static NendAdIconInterface CreateIconAdInterface ()
		{		    
            #if !UNITY_EDITOR
			return new NendUnityPlugin.Platform.Android.AndroidIconInterface();
     	    #else
			return new IconStub ();
		    #endif
		}
		#endif

		internal static NendAdInterstitialInterface CreateInterstitialAdInterface ()
		{
			#if UNITY_IPHONE && !UNITY_EDITOR
			return new NendUnityPlugin.Platform.iOS.IOSInterstitialInterface();
			#elif UNITY_ANDROID && !UNITY_EDITOR
			return new NendUnityPlugin.Platform.Android.AndroidInterstitialInterface();
			#else
			return new InterstitialStub ();
			#endif
		}

		private class BannerStub : NendAdBannerInterface
		{
			public void TryCreateBanner (string paramString)
			{
				UnityEngine.Debug.Log ("TryCreateBanner: " + paramString);
			}

			public void ShowBanner (string gameObject)
			{
				UnityEngine.Debug.Log ("ShowBanner: " + gameObject);
			}

			public void HideBanner (string gameObject)
			{
				UnityEngine.Debug.Log ("HideBanner: " + gameObject);
			}

			public void ResumeBanner (string gameObject)
			{
				UnityEngine.Debug.Log ("ResumeBanner: " + gameObject);
			}

			public void PauseBanner (string gameObject)
			{
				UnityEngine.Debug.Log ("PauseBanner: " + gameObject);
			}

			public void DestroyBanner (string gameObject)
			{
				UnityEngine.Debug.Log ("DestroyBanner: " + gameObject);
			}

			public void LayoutBanner (string gameObject, string paramString)
			{
				UnityEngine.Debug.Log ("LayoutBanner: " + gameObject + ", " + paramString);
			}
		}

		#if UNITY_ANDROID
		private class IconStub : NendAdIconInterface
		{
			public void TryCreateIcons (string paramString)
			{
				UnityEngine.Debug.Log ("TryCreateIcons: " + paramString);
			}

			public void ShowIcons (string gameObject)
			{
				UnityEngine.Debug.Log ("ShowIcons: " + gameObject);
			}

			public void HideIcons (string gameObject)
			{
				UnityEngine.Debug.Log ("HideIcons: " + gameObject);
			}

			public void ResumeIcons (string gameObject)
			{
				UnityEngine.Debug.Log ("ResumeIcons: " + gameObject);
			}

			public void PauseIcons (string gameObject)
			{
				UnityEngine.Debug.Log ("PauseIcons: " + gameObject);
			}

			public void DestroyIcons (string gameObject)
			{
				UnityEngine.Debug.Log ("DestroyIcons: " + gameObject);
			}

			public void LayoutIcons (string gameObject, string paramString)
			{
				UnityEngine.Debug.Log ("LayoutIcons: " + gameObject + ", " + paramString);
			}
		}
		#endif

		private class InterstitialStub : NendAdInterstitialInterface
		{
			public void LoadInterstitialAd (string apiKey, string spotId, bool isOutputLog)
			{
				UnityEngine.Debug.Log ("LoadInterstitialAd: " + apiKey + ", " + spotId + ", " + isOutputLog);
			}

			public void ShowInterstitialAd (string spotId)
			{
				UnityEngine.Debug.Log ("ShowInterstitialAd: " + spotId);
			}

			public void DismissInterstitialAd ()
			{
				UnityEngine.Debug.Log ("DismissInterstitialAd");
			}

			public void SetAutoReloadEnabled (bool enabled)
			{
				UnityEngine.Debug.Log ("SetAutoReloadEnabled: " + enabled);
			}
		}
	}
}