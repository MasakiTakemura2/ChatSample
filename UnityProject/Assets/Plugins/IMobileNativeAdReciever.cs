using UnityEngine;
using System;
using ImobileSdkAdsUnityPluginUtility;

public class IMobileNativeAdReciever : MonoBehaviour
{
    public bool isDontDestroy = false;
    public int adViewId;
    private bool hasReceivedNativeAd = false;
	public Action<IMobileNativeAdObject[]> NativeAdObjectCallBack;
	/// <summary>
	/// ネイティブ広告の受信完了
	/// </summary>
	/// <param name="value">広告情報</param>
	private void onNativeAdDataReciveCompleted(string nativeAdParams)
	{
        hasReceivedNativeAd = true;
		string[] recievedNativeAdArray = nativeAdParams.Split('\\');
		int request = recievedNativeAdArray.Length - 1;

		IMobileNativeAdObject[] nativeAdObjectArray = new IMobileNativeAdObject[request];

		for (int i = 1; i < request + 1; i++)
		{
			string[] nativeAdParamArray = recievedNativeAdArray[i].Split(':');

			Texture2D adImageTexture2D = new Texture2D(int.Parse(nativeAdParamArray[4]), int.Parse(nativeAdParamArray[5]));
			if (nativeAdParamArray[6] != "")
			{
				byte[] result = System.Convert.FromBase64String(nativeAdParamArray[6]);
				adImageTexture2D.LoadImage(result);
			}

			IMobileNativeAdObject nativeObject = new IMobileNativeAdObject();
			nativeObject.SpotId = recievedNativeAdArray[0];
			nativeObject.NativeAdObjectIndex = int.Parse(nativeAdParamArray[0]);
			nativeObject.AdTitle = nativeAdParamArray[1];
			nativeObject.AdDescription = nativeAdParamArray[2];
			nativeObject.AdSponserd = nativeAdParamArray[3];
			nativeObject.AdImage = adImageTexture2D;
			nativeObject.RecieverGameObjectName = this.gameObject.name;
			nativeObject.adViewId = adViewId;

			nativeAdObjectArray[i - 1] = nativeObject;

		}

		NativeAdObjectCallBack(nativeAdObjectArray);

	}
    
    void OnDisable()
    {
        if (!isDontDestroy)
        {
            IMobileNativeAdRecieverManager.RemoveRecieverFromDictionary(adViewId);
        }   
        if (hasReceivedNativeAd) {
			#if UNITY_IPHONE && !UNITY_EDITOR
			if(Application.platform == RuntimePlatform.IPhonePlayer) {
				IMobileSdkAdsUnityPlugin.destroyNativeAd(adViewId);
			}
			#elif UNITY_ANDROID && !UNITY_EDITOR
			if(Application.platform == RuntimePlatform.Android) {
				IMobileSdkAdsUnityPlugin.getAndroidClass().CallStatic("destroyNativeAd", this.gameObject.name);
			}
			#endif
		}
    }
}