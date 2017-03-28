#if UNITY_4_2 || UNITY_4_1 || UNITY_4_0_1 || UNITY_4_0 || UNITY_3_5 || UNITY_3_4 || UNITY_3_3 || UNITY_3_2 || UNITY_3_1 || UNITY_3_0_0 || UNITY_3_0 || UNITY_2_6_1 || UNITY_2_6
#define TWODEE_NOT_SUPPORTED
#endif
#if UNITY_4_5 || UNITY_4_3 || UNITY_4_2 || UNITY_4_1 || UNITY_4_0_1 || UNITY_4_0 || UNITY_3_5 || UNITY_3_4 || UNITY_3_3 || UNITY_3_2 || UNITY_3_1 || UNITY_3_0_0 || UNITY_3_0 || UNITY_2_6_1 || UNITY_2_6
#define GUI_NOT_SUPPORTED
#endif

using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using ImobileSdkAdsUnityPluginUtility;
#if !GUI_NOT_SUPPORTED
using UnityEngine.UI;
#endif

public class IMobileSdkAdsUnityPlugin : MonoBehaviour {
	
	/// <summary>
	/// 端末の向き
	/// </summary>
	public enum ImobileSdkAdsAdOrientation : int {
		IMOBILESDKADS_AD_ORIENTATION_AUTO,
		IMOBILESDKADS_AD_ORIENTATION_PORTRAIT,
		IMOBILESDKADS_AD_ORIENTATION_LANDSCAPE,
	}

	public enum ImobileSdkAdsInlineAdOrientation : int {
		AUTO,
		PORTRAIT,
		LANDSCAPE,
	}

	public static ImobileSdkAdsInlineAdOrientation inlinieAdOrientation = ImobileSdkAdsInlineAdOrientation.AUTO;

	/// <summary>
	/// 水平方向の広告表示位置
	/// </summary>
	public enum AdAlignPosition{
		LEFT,
		CENTER,
		RIGHT
	}
	
	/// <summary>
	/// 垂直方向の広告表示位置
	/// </summary>
	public enum AdValignPosition{
		BOTTOM,
		MIDDLE,
		TOP
	}

	/// <summary>
	/// 広告の種類
	/// </summary>
	public enum AdType{
		ICON,
		BANNER,
		BIG_BANNER,
		TABLET_BANNER,
		TABLET_BIG_BANNER,
		MEDIUM_RECTANGLE,
		BIG_RECTANGLE,
		SKYSCRAPER,
		WIDE_SKYSCRAPER,
		SQUARE,
		SMALL_SQUARE,
		HALFPAGE,
		NATIVE
    }
	
	#region Unity Pugin init
	#if UNITY_IPHONE && !UNITY_EDITOR
	[DllImport("__Internal")]
	private static extern void imobileAddObserver_(string gameObjectName);
	[DllImport("__Internal")]
	private static extern void imobileRemoveObserver_(string gameObjectName);
	[DllImport("__Internal")]
	private static extern void imobileRegisterWithPublisherID_(string publisherid, string mediaid, string spotid); 
	[DllImport("__Internal")]
	private static extern void imobileStart_();
	[DllImport("__Internal")]
	private static extern void imobileStop_();
	[DllImport("__Internal")]
	private static extern bool imobileStartBySpotID_(string spotid);
	[DllImport("__Internal")]
	private static extern bool imobileStopBySpotID_(string spotid);
	[DllImport("__Internal")]
	private static extern bool imobileShowBySpotID_(string spotid);
	[DllImport("__Internal")]
	private static extern bool imobileGetNativeAdDataAndNativeAdParams_(string paramStr);
	[DllImport("__Internal")]
	private static extern bool imobileShowBySpotIDWithPosition_(string paramStr);
	[DllImport("__Internal")]
	private static extern void imobileSetAdOrientation_(ImobileSdkAdsAdOrientation orientation);
	[DllImport("__Internal")]
	private static extern void imobileSetVisibility_(int adViewId, bool visible);
	[DllImport("__Internal")]
	private static extern void imobileSetLegacyIosSdkMode_(bool isLegacyMode);
	[DllImport("__Internal")]
	private static extern void imobileSendClick_(int adViewId, int nativeAdObjectIndex);
	[DllImport("__Internal")]
	private static extern void imobileDestroyNativeAd_(int adViewId);
	[DllImport("__Internal")]
	private static extern void setTestMode_(bool testMode);
#elif UNITY_ANDROID && !UNITY_EDITOR
    private static AndroidJavaClass imobileSdkAdsAndroidPlugin = new AndroidJavaClass("jp.co.imobile.sdkads.android.unity.Plugin");
	public static AndroidJavaClass getAndroidClass() { return imobileSdkAdsAndroidPlugin; }
#endif

	public static void sendClick(int adViewId, int nativeAdObjectIndex){
		#if UNITY_IPHONE && !UNITY_EDITOR
		if(Application.platform == RuntimePlatform.IPhonePlayer) {
			imobileSendClick_(adViewId, nativeAdObjectIndex);
		}
		#endif
	}
	public static void destroyNativeAd(int adViewId){
		#if UNITY_IPHONE && !UNITY_EDITOR
		if(Application.platform == RuntimePlatform.IPhonePlayer) {
			imobileDestroyNativeAd_(adViewId);
		}
		#endif
	}
	#endregion

	#region Unity Pugin Function
	/// <summary>
	/// 広告表示の状態通知イベントを受け取るオブジェクトを登録します
	/// </summary>
	/// <param name="gameObjectName">登録するゲームオブジェクト名</param>
	public static void addObserver(string gameObjectName){
		#if UNITY_IPHONE && !UNITY_EDITOR
		if(Application.platform == RuntimePlatform.IPhonePlayer) {
			imobileAddObserver_(gameObjectName);
		}
		#elif UNITY_ANDROID && !UNITY_EDITOR
		if(Application.platform == RuntimePlatform.Android) {
			imobileSdkAdsAndroidPlugin.CallStatic("addObserver",gameObjectName);
		}
		#endif
	}
	
	/// <summary>
	/// 広告表示の状態通知イベントを受け取るオブジェクトを解除します
	/// </summary>
	/// <param name="gameObjectName">解除するゲームオブジェクト名</param>
	public static void removeObserver(string gameObjectName){
		#if UNITY_IPHONE && !UNITY_EDITOR
		if(Application.platform == RuntimePlatform.IPhonePlayer) {
			imobileRemoveObserver_(gameObjectName);
		}
		#elif UNITY_ANDROID && !UNITY_EDITOR
		if(Application.platform == RuntimePlatform.Android) {
			imobileSdkAdsAndroidPlugin.CallStatic("removeObserver",gameObjectName);
		}
		#endif
	}

	/// <summary>
	/// Androidのイベント通知時のValue値を旧方式(JSON方式)に変更します 
	/// </summary>
	/// <param name="isLegacyObserverValueForAndroid">true : ValueをJSON形式で返します false:ValueをCSV形式で返します(デフォルト)</param>
	public static void setLegacyObserverValueForAndroid(bool isLegacyObserverValueForAndroid){
		#if UNITY_ANDROID && !UNITY_EDITOR
		if(Application.platform == RuntimePlatform.Android) {
			imobileSdkAdsAndroidPlugin.CallStatic("setLegacyObserverReturnValue", isLegacyObserverValueForAndroid);
		}
		#endif
	}


	/// <summary>
	/// 全画面広告のスポットを登録します
	/// </summary>
	/// <param name="partnerid">パートナーID</param>
	/// <param name="mediaid">メディアID</param>
	/// <param name="spotid">スポットID</param>
	public static void register(string partnerid, string mediaid, string spotid){

		IMobileSpotInfoManager.SetSpotInfo(spotid, partnerid, mediaid);

		#if UNITY_IPHONE && !UNITY_EDITOR
		if(Application.platform == RuntimePlatform.IPhonePlayer) {
			imobileRegisterWithPublisherID_(partnerid, mediaid, spotid);
			imobileStartBySpotID_(spotid);
		}
		#elif UNITY_ANDROID && !UNITY_EDITOR
		if(Application.platform == RuntimePlatform.Android) {
			imobileSdkAdsAndroidPlugin.CallStatic("registerFullScreen", partnerid, mediaid, spotid);
			imobileSdkAdsAndroidPlugin.CallStatic("start", spotid);
		}
		#endif
	}

	/// <summary>
	/// 全画面広告のスポットを登録します
	/// </summary>
	/// <param name="partnerid">Partnerid.</param>
	/// <param name="mediaid">Mediaid.</param>
	/// <param name="spotid">Spotid.</param>
	public static void registerFullScreen(string partnerid, string mediaid, string spotid){
		register (partnerid, mediaid, spotid);
	}

	/// <summary>
	/// インライン広告のスポットを登録します
	/// </summary>
	/// <param name="partnerid">Partnerid.</param>
	/// <param name="mediaid">Mediaid.</param>
	/// <param name="spotid">Spotid.</param>
	public static void registerInline(string partnerid, string mediaid, string spotid){
		IMobileSpotInfoManager.SetSpotInfo(spotid, partnerid, mediaid);
	}
	
	/// <summary>
	/// 登録済みの全ての広告のスポット情報の取得を開始します
	/// </summary>
	public static void start(){
	}
	
	/// <summary>
	/// 登録済みの全ての広告のスポット情報の取得を停止します
	/// </summary>
	public static void stop(){
		#if UNITY_IPHONE && !UNITY_EDITOR
		if(Application.platform == RuntimePlatform.IPhonePlayer) {
			imobileStop_();
		}
		#elif UNITY_ANDROID && !UNITY_EDITOR
		if(Application.platform == RuntimePlatform.Android) {
			imobileSdkAdsAndroidPlugin.CallStatic("stop");
		}
		#endif
	}
	
	/// <summary>
	/// 広告のスポット情報の取得を開始します
	/// </summary>
	/// <param name="spotid">スポットID</param>
	public static void start(string spotid){
		#if !UNITY_EDITOR
		string partnerid = IMobileSpotInfoManager.GetPartnerId(spotid);
		string mediaid = IMobileSpotInfoManager.GetMediaId(spotid);
		#endif
		#if UNITY_IPHONE && !UNITY_EDITOR
		if(Application.platform == RuntimePlatform.IPhonePlayer) {
			imobileRegisterWithPublisherID_(partnerid, mediaid, spotid);
			imobileStartBySpotID_(spotid);
		}
		#elif UNITY_ANDROID && !UNITY_EDITOR
		if(Application.platform == RuntimePlatform.Android) {
			imobileSdkAdsAndroidPlugin.CallStatic("registerInLine", partnerid, mediaid, spotid);
			imobileSdkAdsAndroidPlugin.CallStatic("start", spotid);
		}
		#endif
	}
	
	/// <summary>
	/// 広告のスポット情報の取得を停止します
	/// </summary>
	/// <param name="spotid">スポットID</param>
    public static void stop(string spotid){
		#if UNITY_IPHONE && !UNITY_EDITOR
		if(Application.platform == RuntimePlatform.IPhonePlayer) {
			imobileStopBySpotID_(spotid); 
		}
		#elif UNITY_ANDROID && !UNITY_EDITOR
		if(Application.platform == RuntimePlatform.Android) {
			imobileSdkAdsAndroidPlugin.CallStatic("stop", spotid);
		}
		#endif
	}
	
	/// <summary>
	/// 広告を表示します
	/// </summary>
	/// <param name="spotid">スポットID</param>
    public static void show(string spotid){
		#if UNITY_IPHONE && !UNITY_EDITOR
		if(Application.platform == RuntimePlatform.IPhonePlayer) {
			imobileShowBySpotID_(spotid);		
		}
		#elif UNITY_ANDROID && !UNITY_EDITOR
		if(Application.platform == RuntimePlatform.Android) {
			imobileSdkAdsAndroidPlugin.CallStatic("showAd", spotid);
		}
		#endif
	}

	/// <summary>
	/// 広告を表示します
	/// </summary>
	/// <param name="spotid">スポットID</param>
	/// <param name="adType">AdType</param>
	/// <param name="alignPosition">AdAlignPosition</param>
	/// <param name="valignPosition">AdValignPosition</param>
	public static int show(string spotid, AdType adType, AdAlignPosition alignPosition, AdValignPosition valignPosition) {
		return show (spotid, adType, alignPosition, valignPosition, false);
	}

	/// <summary>
	/// 広告を表示します
	/// </summary>
	/// <param name="spotid">スポットID</param>
	/// <param name="adType">AdType</param>
	/// <param name="alignPosition">AdAlignPosition</param>
	/// <param name="valignPosition">AdValignPosition</param>
	/// <param name="sizeAdjust">sizeAdjust</param> 
	public static int show(string spotid, AdType adType, AdAlignPosition alignPosition, AdValignPosition valignPosition, bool sizeAdjust) {
		Rect adRect = IMobileSdkAdsViewUtility.getAdRect (alignPosition, valignPosition, adType, null, sizeAdjust);
		return show (spotid, adType, adRect, null, sizeAdjust, null);
	}
	
	/// <summary>
	/// 広告を表示します
	/// </summary>
	/// <param name="spotid">スポットID</param>
	/// <param name="adType">AdType</param>
	/// <param name="alignPosition">AdAlignPosition</param>
	/// <param name="valignPosition">AdValignPosition</param>
	/// <param name="iconParams">IMobileIconParams</param>
	public static int show(string spotid, AdType adType, AdAlignPosition alignPosition, AdValignPosition valignPosition, IMobileIconParams iconParams) {
		Rect adRect = IMobileSdkAdsViewUtility.getAdRect (alignPosition, valignPosition, adType, iconParams, false);
		return show (spotid, adType, adRect, iconParams, false, null);
	}
	
	/// <summary>
	/// 広告を表示します
	/// </summary>
	/// <param name="spotid">スポットID</param>
	/// <param name="adType">AdType</param>
	/// <param name="left">水平方向の広告表示座標</param>
	/// <param name="top">垂直方向の広告表示座標</param>
	public static int show(string spotid, AdType adType, int left, int top){
		return show(spotid, adType, left, top, false);
	}

	/// <summary>
	/// 広告を表示します
	/// </summary>
	/// <param name="spotid">スポットID</param>
	/// <param name="adType">AdType</param>
	/// <param name="left">水平方向の広告表示座標</param>
	/// <param name="top">垂直方向の広告表示座標</param>
	/// <param name="sizeAdjust">sizeAdjust</param> 
	public static int show(string spotid, AdType adType, int left, int top, bool sizeAdjust){
		Rect adRect = IMobileSdkAdsViewUtility.getAdRect (left, top, adType, null, sizeAdjust);
		return show (spotid, adType, adRect, null, sizeAdjust, null);
	}

	/// <summary>
	/// 広告を表示します
	/// </summary>
	/// <param name="spotid">スポットID</param>
	/// <param name="adType">AdType</param>
	/// <param name="left">水平方向の広告表示座標</param>
	/// <param name="top">垂直方向の広告表示座標</param>
	/// <param name="iconParams">IMobileIconParams</param>
    public static int show(string spotid, AdType adType, int left, int top, IMobileIconParams iconParams){
		Rect adRect = IMobileSdkAdsViewUtility.getAdRect (left, top, adType, iconParams, false);
		return show (spotid, adType, adRect, iconParams, false, null);
	}

	private static int show(string spotid, AdType adType, Rect adRect, IMobileIconParams iconParams, bool sizeAdjust, IMobileNativeAdParams adParams){

		int adViewId = IMobileAdViewIdManager.CreateId();

		#if !UNITY_EDITOR
		iconParams = iconParams ?? new IMobileIconParams();
		string partnerId = IMobileSpotInfoManager.GetPartnerId(spotid);
		string mediaId = IMobileSpotInfoManager.GetMediaId(spotid);
		bool isIcon = adType == AdType.ICON;
		#endif

		#if UNITY_IPHONE && !UNITY_EDITOR
		if(Application.platform == RuntimePlatform.IPhonePlayer) {

			string[] parameters = {
				partnerId,
				mediaId,
				spotid,
				adRect.x.ToString(),
				adRect.y.ToString(),
				adRect.width.ToString(),
				adRect.height.ToString(),
				sizeAdjust.ToString(),
				adViewId.ToString()
			};

			string paramStr =  string.Join(":", parameters);
			imobileShowBySpotIDWithPosition_(paramStr);
		}
		#elif UNITY_ANDROID && !UNITY_EDITOR
		if(Application.platform == RuntimePlatform.Android) {
			imobileSdkAdsAndroidPlugin.CallStatic("show", partnerId, mediaId, spotid, (int)adRect.x, (int)adRect.y,
													iconParams.iconNumber,
													iconParams.iconViewLayoutWidth,
													iconParams.iconSize,
													iconParams.iconTitleEnable,
													iconParams.iconTitleFontSize,
													iconParams.iconTitleFontColor, 
													iconParams.iconTitleOffset,
													iconParams.iconTitleShadowEnable,
													iconParams.iconTitleShadowColor,
													iconParams.iconTitleShadowDx,
													iconParams.iconTitleShadowDy,
													isIcon,
													sizeAdjust,
													adViewId
												);
		}
#endif


		return adViewId;
	}

	#region Native Ad Functions

	/// <summary>
	/// ネイティブ広告の取得を行います
	/// </summary>
	/// <param name="spotid">スポットID</param>
	/// <param name="callBack">広告取得完了に実行されます</param>
	/// <param name="adViewId">(省略可能)adViewIdを既に取得している場合、それを指定できます</param>
	public static int getNativeAdData(string spotid, Action<IMobileNativeAdObject[]> callBack, bool dontDestroyReciever = false)
	{
		IMobileNativeAdParams adParams = new IMobileNativeAdParams ();
		return getNativeAdData(spotid, adParams, callBack, dontDestroyReciever);
	}

	/// <summary>
	/// ネイティブ広告の取得を行います
	/// </summary>
	/// <param name="spotId">スポットID</param>
	/// <param name="adParams">ネイティブ広告取得時のパラメータ</param>
	/// <param name="callBack">広告取得完了に実行されます</param>
	/// <param name="dontDestroyReciever">シーン間で広告を引き継ぐ際にTrueを指定します</param>
	/// <param name="adViewId">(省略可能)adViewIdを既に取得している場合、それを指定できます</param>
	public static int getNativeAdData(string spotId, IMobileNativeAdParams adParams, Action<IMobileNativeAdObject[]> callBack, bool dontDestroyReciever = false)
	{
		int adViewId = IMobileAdViewIdManager.CreateId ();

		#if !UNITY_EDITOR
		string partnerId = IMobileSpotInfoManager.GetPartnerId(spotId);
		string mediaId = IMobileSpotInfoManager.GetMediaId(spotId);
		string recieverGameObjectName = IMobileNativeAdRecieverManager.CreateNativeAdReciever(adViewId,callBack,dontDestroyReciever);
		#endif

		#if UNITY_IPHONE && !UNITY_EDITOR
		if(Application.platform == RuntimePlatform.IPhonePlayer) {

			string[] parameters = {
				partnerId,
				mediaId,
				spotId,
				adParams.requestAdCount.ToString(),
				adParams.nativeImageGetFlag.ToString(),
				recieverGameObjectName,
				adViewId.ToString()
			};

			string paramStr =  string.Join(":", parameters);
			imobileGetNativeAdDataAndNativeAdParams_(paramStr);
		}
		#elif UNITY_ANDROID && !UNITY_EDITOR
		if(Application.platform == RuntimePlatform.Android) {
			imobileSdkAdsAndroidPlugin.CallStatic(
				"getNativeAdData", 
				partnerId,
				mediaId,
				spotId,
				adParams.requestAdCount,
				adParams.nativeImageGetFlag,
				recieverGameObjectName
			);
		}
		#endif
		return adViewId;
	}

	/// <summary>
	/// 対象adViewIDのレシーバーに設定されているcallBackを書き換えます。
	/// </summary>
	/// <param name="spotid">スポットID</param>
	public static bool replaceCallBack(int adViewId, Action<IMobileNativeAdObject[]> callBack) {
		return IMobileNativeAdRecieverManager.ReplaceRecieverCallBack (adViewId, callBack);
	}

	/// <summary>
	/// 対象adViewIDのレシーバーをDestroyします。
	/// </summary>
	/// <param name="adViewId">adViewId</param>
	public static bool destroyReciever(int adViewId) {
		return IMobileNativeAdRecieverManager.DestroyReciever (adViewId);
	}

	#endregion

	/// <summary>
	/// 広告の表示の向きを設定します
	/// (iPhoneのみ設定可能)
	/// </summary>
	/// <param name="orientation">ImobileSdkAdsAdOrientation</param>
	public static void setAdOrientation(ImobileSdkAdsAdOrientation orientation){
		#if UNITY_IPHONE && !UNITY_EDITOR
		if(Application.platform == RuntimePlatform.IPhonePlayer) {
			imobileSetAdOrientation_(orientation);
			return;
		}
		#elif UNITY_ANDROID && !UNITY_EDITOR
		if(Application.platform == RuntimePlatform.Android) {
			imobileSdkAdsAndroidPlugin.CallStatic("setAdOrientation", (int)orientation);
		}
		#endif
	}
	
	/// <summary>
	/// 広告の表示・非表示の切り替えを行います
	/// </summary>
	/// <param name="adViewId">showメソッドの戻り値として受け取るAdViewId</param>
	/// <param name="visible">表示するかどうか</param>
    public static void setVisibility(int adViewId, bool visible){
		#if UNITY_IPHONE && !UNITY_EDITOR
		if(Application.platform == RuntimePlatform.IPhonePlayer) {
			imobileSetVisibility_(adViewId, visible);
			return;
		}
		#elif UNITY_ANDROID && !UNITY_EDITOR
		if(Application.platform == RuntimePlatform.Android) {
			imobileSdkAdsAndroidPlugin.CallStatic("setVisibility", adViewId, visible);
		}
		#endif
	}
	
	/// <summary>
	/// テストモードを設定します
	/// </summary>
	/// <param name="testMode">テストモード</param>
	public static void setTestMode(bool testMode) {
		#if UNITY_IPHONE && !UNITY_EDITOR
		if(Application.platform == RuntimePlatform.IPhonePlayer) {
			setTestMode_(testMode);
			return;
		}
		#elif UNITY_ANDROID && !UNITY_EDITOR
		if(Application.platform == RuntimePlatform.Android) {
			imobileSdkAdsAndroidPlugin.CallStatic("setTestMode", testMode);
		}
		#endif
	}

	/// <summary>
	/// Xcode5でのビルドに対応させる場合に設定します
	/// </summary>
	/// <param name="isLegacyMode">Xcode5でのビルドに対応させるかどうか</param>
	public static void setLegacyIosMode(bool isLegacyMode) {
		#if UNITY_IPHONE && !UNITY_EDITOR
		if(Application.platform == RuntimePlatform.IPhonePlayer) {
			imobileSetLegacyIosSdkMode_(isLegacyMode);
			return;
		}
		#endif
	}


	#endregion

}