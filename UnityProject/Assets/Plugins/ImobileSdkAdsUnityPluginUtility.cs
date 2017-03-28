using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace ImobileSdkAdsUnityPluginUtility {

	internal static class IMobileSdkAdsViewUtility
	{
		#if UNITY_IPHONE && !UNITY_EDITOR
		[DllImport("__Internal")]
		private static extern int getScreenWidth_(bool isPortrait);
		[DllImport("__Internal")]
		private static extern int getScreenHeight_(bool isPortrait);
		#endif

		/// <summary>
		/// 広告の表示領域を返します
		/// </summary>
		/// <returns>広告の表示領域</returns>
		/// <param name="left">Left</param>
		/// <param name="top">Top</param>
		/// <param name="adType">AdType</param>
		/// <param name="iconParams">IMobileIconParams</param>
		/// <param name="sizeAdjust">sizeAdjust</param>
		internal static Rect getAdRect(int left, int top, IMobileSdkAdsUnityPlugin.AdType adType, IMobileIconParams iconParams, bool sizeAdjust)
		{
			iconParams = iconParams ?? new IMobileIconParams ();
			CGSize adSize = getAdSize (adType, iconParams, sizeAdjust);
			return new Rect (left, top, adSize.width, adSize.height);
		}

		/// <summary>
		/// 広告の表示領域を返します
		/// </summary>
		/// <returns>広告の表示領域</returns>
		/// <param name="alignPosition">AdAlignPosition</param>
		/// <param name="valignPosition">AdValignPosition</param>
		/// <param name="adType">AdType</param>
		/// <param name="iconParams">IMobileIconParams</param>
		/// <param name="sizeAdjust">sizeAdjust</param>
		internal static Rect getAdRect(IMobileSdkAdsUnityPlugin.AdAlignPosition alignPosition, IMobileSdkAdsUnityPlugin.AdValignPosition valignPosition, IMobileSdkAdsUnityPlugin.AdType adType, IMobileIconParams iconParams, bool sizeAdjust)
		{
			// スクリーンの論理サイズを取得
			int screenWidth = 0;
			int screenHeight = 0;

			#if UNITY_IPHONE && !UNITY_EDITOR
			screenWidth = getScreenWidth_(isScreenPortrait());
			screenHeight = getScreenHeight_(isScreenPortrait());
			#elif UNITY_ANDROID && !UNITY_EDITOR
			screenWidth = getDensitySize (Screen.width);
			screenHeight = getDensitySize (Screen.height);
			#endif

			// 広告の表示位置を計算
			int left = 0;
			int top = 0;

			// 広告サイズを取得
			iconParams = iconParams ?? new IMobileIconParams ();
			CGSize adSize = getAdSize (adType, iconParams, sizeAdjust);

			// x座標の取得
			switch (alignPosition) {
				case IMobileSdkAdsUnityPlugin.AdAlignPosition.LEFT:
					left = 0;
					break;
				case IMobileSdkAdsUnityPlugin.AdAlignPosition.CENTER:
					left = (screenWidth - adSize.width) / 2;
					break;
				case IMobileSdkAdsUnityPlugin.AdAlignPosition.RIGHT:
					left = screenWidth - adSize.width;
					break;
			}

			// y座標の取得
			switch (valignPosition) {
				case IMobileSdkAdsUnityPlugin.AdValignPosition.TOP:
					top = 0;
					break;
				case IMobileSdkAdsUnityPlugin.AdValignPosition.MIDDLE:
					top = (screenHeight / 2) - (adSize.height / 2);
					break;
				case IMobileSdkAdsUnityPlugin.AdValignPosition.BOTTOM:
                #if UNITY_IOS
                top = screenHeight - adSize.height - 56;

                if (SystemInfo.deviceModel.Contains("iPad")) {
                    // ipadでの処理
                    top = screenHeight - adSize.height - 82; // ゴリ押しの修正
                }

                #else
                top = screenHeight - adSize.height - 48;
                #endif
					
					break;
			}

			return new Rect (left, top, adSize.width, adSize.height);
		}

		private static CGSize getAdSize(IMobileSdkAdsUnityPlugin.AdType adType, IMobileIconParams iconParams, bool sizeAdjust)
		{
			CGSize adSize = new CGSize();

			switch (adType) {
			case IMobileSdkAdsUnityPlugin.AdType.BANNER:
				adSize = getAdjustedAdSize(new CGSize(320, 50), sizeAdjust, IMobileSdkAdsUnityPlugin.AdType.BANNER);
				break;
			case IMobileSdkAdsUnityPlugin.AdType.BIG_BANNER:
				adSize = getAdjustedAdSize(new CGSize(320, 100), sizeAdjust, IMobileSdkAdsUnityPlugin.AdType.BIG_BANNER);
				break;
			case IMobileSdkAdsUnityPlugin.AdType.TABLET_BANNER:
				adSize = new CGSize(468, 60);
				break;
			case IMobileSdkAdsUnityPlugin.AdType.TABLET_BIG_BANNER:
				adSize = new CGSize(728, 90);
				break;
			case IMobileSdkAdsUnityPlugin.AdType.MEDIUM_RECTANGLE:
				adSize = getAdjustedAdSize(new CGSize(300, 250), sizeAdjust, IMobileSdkAdsUnityPlugin.AdType.MEDIUM_RECTANGLE);
				break;
			case IMobileSdkAdsUnityPlugin.AdType.BIG_RECTANGLE:
				adSize = new CGSize(336, 280);
				break;
			case IMobileSdkAdsUnityPlugin.AdType.SKYSCRAPER:
				adSize = new CGSize(120, 600);
				break;
			case IMobileSdkAdsUnityPlugin.AdType.WIDE_SKYSCRAPER:
				adSize = new CGSize(160, 600);
				break;
			case IMobileSdkAdsUnityPlugin.AdType.SQUARE:
				adSize = new CGSize(250, 250);
				break;
			case IMobileSdkAdsUnityPlugin.AdType.SMALL_SQUARE:
				adSize = new CGSize(200, 200);
				break;
			case IMobileSdkAdsUnityPlugin.AdType.HALFPAGE:
				adSize = new CGSize(300, 600);
				break;
			case IMobileSdkAdsUnityPlugin.AdType.ICON:

				#if !UNITY_EDITOR
				int iconDefaultWidth = 57;			// アイコン１つあたりのデフォルトサイズ
				int iconMinimumWidth = 47;			// アイコン1つあたりの最小サイズ
				int iconDefaultMargin = 18;			// デフォルトアイコン間隔
				int iconMinimumMargin = 4;			// 最小アイコン間隔
				int iconAdTitleReserveSpace = 3;    // タイトル表示エリアの予備マージン

				// アイコン広告の表示サイズをアイコンパラメータから計算して求める
				int iconAdWidth = 0;
				int iconAdHeight = 0;

				// スクリーンの論理サイズを取得
				int screenWidth = 0;
				int screenHeight = 0;
				#endif

				#if UNITY_IPHONE && !UNITY_EDITOR
				screenWidth = getScreenWidth_(isScreenPortrait());
				screenHeight = getScreenHeight_(isScreenPortrait());

				// 幅を計算する
				if (iconParams.iconViewLayoutWidth > 0) {
					iconAdWidth = iconParams.iconViewLayoutWidth;
				} else {
					int iconWidth = (iconParams.iconSize > 0) ? Math.Max(iconParams.iconSize, iconMinimumWidth) : iconDefaultWidth;
					iconAdWidth =  (iconWidth + iconDefaultMargin) * 1;
				}
				
				// 高さを計算する
				// アイコン一つあたりの幅を計算
				int iconImageWidth = 0;
				if (iconParams.iconSize > 0) {
					iconImageWidth  = Math.Max(iconParams.iconSize, iconMinimumWidth);
				} else {
					if (iconAdWidth >= (iconDefaultWidth + iconDefaultMargin) * 1) {
						iconImageWidth = iconDefaultWidth;
					} else {
						iconImageWidth = iconMinimumWidth;
					}
				}
				
				// アイコンの表示間隔を取得(アイコン広告の表示サイス - アイコン一つあたりの幅 * アイコンの数) / アイコンの数 の結果と最低マージンとの比較で大きい方をマージンとして設定
				int iconImageMargin = Math.Max((int)Math.Ceiling((double)(iconAdWidth - iconImageWidth * 1) / 1) , iconMinimumMargin);

				#elif UNITY_ANDROID && !UNITY_EDITOR
				screenWidth = getDensitySize (Screen.width);
				screenHeight = getDensitySize (Screen.height);

				// 幅を計算する
				if (iconParams.iconViewLayoutWidth > 0) {
					iconAdWidth = iconParams.iconViewLayoutWidth;
				} else {
					if (iconParams.iconNumber < 4) {
						int iconWidth = (iconParams.iconSize > 0) ? Math.Max(iconParams.iconSize, iconMinimumWidth) : iconDefaultWidth;
						iconAdWidth =  (iconWidth + iconDefaultMargin) * iconParams.iconNumber;
					} else {
						iconAdWidth =  Math.Min(screenWidth, screenHeight);
					}
				}
				
				// 高さを計算する
				// アイコン一つあたりの幅を計算
				int iconImageWidth = 0;
				if (iconParams.iconSize > 0) {
					iconImageWidth  = Math.Max(iconParams.iconSize, iconMinimumWidth);
				} else {
					if (iconAdWidth >= (iconDefaultWidth + iconDefaultMargin) * iconParams.iconNumber) {
						iconImageWidth = iconDefaultWidth;
					} else {
						iconImageWidth = iconMinimumWidth;
					}
				}
				
				// アイコンの表示間隔を取得(アイコン広告の表示サイス - アイコン一つあたりの幅 * アイコンの数) / アイコンの数 の結果と最低マージンとの比較で大きい方をマージンとして設定
				int iconImageMargin = Math.Max((int)Math.Ceiling((double)(iconAdWidth - iconImageWidth * iconParams.iconNumber) / iconParams.iconNumber) , iconMinimumMargin);
				#endif


				#if !UNITY_EDITOR
				// 高さの取得
				if (!iconParams.iconTitleEnable) {
					iconAdHeight = iconImageWidth;
				} else {
					int iconTitleOffset = (iconParams.iconTitleOffset > 0) ? iconParams.iconTitleOffset : 4;
					int iconTitleFontSize = (iconParams.iconTitleFontSize > 0) ? Math.Max(iconParams.iconTitleFontSize, 8) : 10;
					int iconTitleShadowDy = (iconParams.iconTitleShadowEnable) ? iconParams.iconTitleShadowDy : 0;
					
					// タイトルが一行に収まるか計算
					if ((iconImageWidth + iconImageMargin) >= (iconDefaultWidth + iconDefaultMargin / 2)) {
						iconAdHeight = iconImageWidth + iconTitleOffset + iconTitleFontSize + iconTitleShadowDy + iconAdTitleReserveSpace;
					} else {
						iconAdHeight = iconImageWidth + iconTitleOffset + iconTitleFontSize * 2 + iconTitleShadowDy + iconAdTitleReserveSpace;
					}
				}
				adSize = new CGSize(iconAdWidth, iconAdHeight);
				#endif
				break;
			}
			return adSize;
		}

		private static CGSize getAdjustedAdSize(CGSize originalSize, bool sizeAdjust, IMobileSdkAdsUnityPlugin.AdType adType)
		{
			if (!sizeAdjust) {
				return originalSize;
			}

			int screenWidth = 0;
			int screenHeight = 0;
			
			#if UNITY_IPHONE && !UNITY_EDITOR
			screenWidth = getScreenWidth_(isScreenPortrait());
			screenHeight = getScreenHeight_(isScreenPortrait());
			#elif UNITY_ANDROID && !UNITY_EDITOR
			screenWidth = getDensitySize (Screen.width);
			screenHeight = getDensitySize (Screen.height);
			#endif

			screenWidth = Math.Min (screenWidth, screenHeight);
			int adjustedWidth = (adType == IMobileSdkAdsUnityPlugin.AdType.MEDIUM_RECTANGLE) ? screenWidth - 20 : screenWidth; 
			int adjustedHeight = (int)Math.Round(originalSize.height * (double)adjustedWidth / (double)originalSize.width);

			return new CGSize (adjustedWidth, adjustedHeight);
		}

		private static bool isScreenPortrait()
		{
			if (IMobileSdkAdsUnityPlugin.inlinieAdOrientation == IMobileSdkAdsUnityPlugin.ImobileSdkAdsInlineAdOrientation.AUTO) {
				switch (Screen.orientation) {
				case ScreenOrientation.Portrait:
				case ScreenOrientation.PortraitUpsideDown:
					return true;
				default:
					return false;
				}
			} else {
				if (IMobileSdkAdsUnityPlugin.inlinieAdOrientation == IMobileSdkAdsUnityPlugin.ImobileSdkAdsInlineAdOrientation.PORTRAIT) {
					return true;
				} else {
					return false;
				}
			}
		}
	
		private static int getDensitySize(int size)
		{
			#if UNITY_ANDROID && !UNITY_EDITOR
	        AndroidJavaObject displayMetrics = new AndroidJavaObject ("android.util.DisplayMetrics");
	        AndroidJavaClass unityPlayerClass = new AndroidJavaClass ("com.unity3d.player.UnityPlayer");
	        AndroidJavaObject activityObject = unityPlayerClass.GetStatic<AndroidJavaObject> ("currentActivity");
	        AndroidJavaObject windowManagerObject = activityObject.Call<AndroidJavaObject> ("getWindowManager");
	        AndroidJavaObject displayObject = windowManagerObject.Call<AndroidJavaObject> ("getDefaultDisplay");
	        displayObject.Call ("getMetrics", displayMetrics);
	        float density = displayMetrics.Get<float> ("density");
	        return (int)(((size / density) + 0.5f));
			#else
			return 0;
			#endif
		}
	}


	internal struct CGSize
	{
		public int width;
		public int height;
		
		public CGSize(int w, int h) {
			width = w;
			height = h;
		}
	}

	internal static class IMobileAdViewIdManager
	{
		private static int adViewIdCounter = 100000;	
		
		internal static int CreateId()
		{
			return adViewIdCounter++;
		}
	}

	internal static class IMobileSpotInfoManager
	{
		private static Dictionary<string, List<string>> spotInfoDictionary = new Dictionary<string, List<string>>();
		
		private enum SpotInfo{
			PartnerId,
			MediaId
		}
		
		internal static void SetSpotInfo(string spotId, string partnerId, string mediaId){
			List<string> spotInfo = new List<string> {partnerId, mediaId};
			if (spotInfoDictionary.ContainsKey (spotId)) {
				spotInfoDictionary [spotId] = spotInfo;
			} else {
				spotInfoDictionary.Add(spotId, spotInfo);
			}
		}
		
		internal static string GetPartnerId(string spotId){
			return spotInfoDictionary[spotId][(int)SpotInfo.PartnerId];
		}
		
		internal static string GetMediaId(string spotId){
			return spotInfoDictionary[spotId][(int)SpotInfo.MediaId];
		}
	}

    internal static class IMobileNativeAdRecieverManager
	{
        internal static Dictionary<int, GameObject> recieverDictionary = new Dictionary<int, GameObject>();

		internal static string CreateNativeAdReciever(int adViewId, Action<IMobileNativeAdObject[]> callBack, bool dontDestroyReciever){
			GameObject recieverGameObject = new GameObject();
			recieverGameObject.name = "IMobileNativeAdID" + adViewId.ToString();
			IMobileNativeAdReciever nativeAdReciever = recieverGameObject.AddComponent<IMobileNativeAdReciever>();
			nativeAdReciever.NativeAdObjectCallBack = callBack;
            nativeAdReciever.adViewId = adViewId;
			if (dontDestroyReciever) {
                nativeAdReciever.isDontDestroy = true;
				UnityEngine.GameObject.DontDestroyOnLoad (recieverGameObject);
            }
            recieverDictionary.Add(adViewId, recieverGameObject);
			return recieverGameObject.name;
		}

		internal static bool ReplaceRecieverCallBack(int adViewId, Action<IMobileNativeAdObject[]> callBack) {
			if (recieverDictionary.ContainsKey(adViewId) && recieverDictionary[adViewId] != null) {
				recieverDictionary[adViewId].GetComponent<IMobileNativeAdReciever> ().NativeAdObjectCallBack = callBack;
				return true;
			}
			return false;
		}

        internal static bool RemoveRecieverFromDictionary(int adViewId)
        {
            if (recieverDictionary.ContainsKey(adViewId))
            {
                recieverDictionary.Remove(adViewId);
				return true;
            }
            return false;
        }

		internal static bool DestroyReciever (int adViewId) {
            if (recieverDictionary.ContainsKey(adViewId) && recieverDictionary[adViewId] != null)
            {
                    UnityEngine.GameObject.Destroy(recieverDictionary[adViewId]);
                    recieverDictionary.Remove(adViewId);
                    return true;
            }
            else if (recieverDictionary.ContainsKey(adViewId) && recieverDictionary[adViewId] == null)
            {
                recieverDictionary.Remove(adViewId);
                return true;
            }
			return false;
		}
	}
}
