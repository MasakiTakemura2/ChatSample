using UnityEngine;
using System.Collections;

public class WebviewLayout
{
	#if UNITY_IPHONE && !UNITY_EDITOR
		private static float screenWidth = UniWebViewHelper.screenWidth;
		private static float screenHeight = UniWebViewHelper.screenHeight;
	#elif UNITY_ANDROID && !UNITY_EDITOR
		private static float screenWidth = DeviceService.ScreenWidth ();
		private static float screenHeight = DeviceService.ScreenHeight ();
	#else
		private static float screenWidth = DeviceService.ScreenWidth ();
		private static float screenHeight = DeviceService.ScreenHeight ();
	#endif

	// iphone5を基準とする
	private static float baseWidth = 640.0f;
	private static float baseHeight = 1136.0f;

	private static int baseAspectWidth = 9;
	private static int baseAspectHeight = 16;

	public class InsetsParameter
	{
		public int top;
		public int side;
		public int bottom;

		public int left;
		public int right;
	}

	public static InsetsParameter GetInsetsWebview(float marginTop = 0.0f, float marginSide = 0.0f, float marginBottom = 0.0f)
	{
		InsetsParameter _wInsets = new InsetsParameter();

		float aspect = 1.0f;

		// 画面固定のためサイドを計算
		float blackSide = screenWidth - (baseAspectWidth * (screenHeight / baseAspectHeight));
		float scWidth = screenWidth;
		if (blackSide <= 0)
		{
			blackSide = 0;
			aspect = screenHeight / ((screenWidth / baseAspectWidth) * baseAspectHeight);
		} else {
			scWidth = baseAspectWidth * (screenHeight / baseAspectHeight);
			blackSide = blackSide / 2;
		}

		float setWidth = scWidth * marginSide * aspect;
		float setTop = screenHeight * marginTop * aspect;
		float setBottom = screenHeight * marginBottom * aspect;

//		Debug.Log ("wInsets screenWidth:" + screenWidth);
//		Debug.Log ("wInsets screenHeight:" + screenHeight);
//		Debug.Log ("wInsets aspect:" + aspect.ToString("F3"));
//		Debug.Log ("wInsets top:" + (int)setTop);
//		Debug.Log ("wInsets side:" + (int)setWidth);
//		Debug.Log ("wInsets Bottom:" + (int)setBottom);

		_wInsets.top = Mathf.RoundToInt(setTop);
		_wInsets.side = Mathf.RoundToInt(setWidth + blackSide);
		_wInsets.bottom = Mathf.RoundToInt(setBottom);

		return _wInsets;
	}

	public static InsetsParameter GetInsetsGachaBanner() 
	{
		// バナー基本サイズ
		float gBannerWidth = 574.0f;
		float gBannerHeight = 452.0f;
		// 両サイドに5%の余白
		float marginSide = 0.05f;
		// 上部に13%の余白
		float marginTop = 0.13F;

		InsetsParameter _wInsets = new InsetsParameter();

		float aspect = 1.0f;

		// 画面固定のためサイドを計算
		float blackSide = screenWidth - (baseAspectWidth * (screenHeight / baseAspectHeight));
		float scWidth = screenWidth;
		if (blackSide <= 0) {
			blackSide = 0;
			aspect = screenHeight / ((screenWidth / baseAspectWidth) * baseAspectHeight);
		} else {
			scWidth = baseAspectWidth * (screenHeight / baseAspectHeight);
			blackSide = blackSide / 2;
		}

		float setWidth = scWidth * marginSide * aspect;
		float setTop = screenHeight * marginTop * aspect;

		float imgWidth = scWidth - (setWidth * 2);
		float imgHight = gBannerHeight * (imgWidth / gBannerWidth);

		float setBottom = (screenHeight - setTop - imgHight) * aspect;

//		Debug.Log ("wInsets screenWidth:" + screenWidth);
//		Debug.Log ("wInsets screenHeight:" + screenHeight);
//		Debug.Log ("wInsets aspect:" + aspect.ToString("F3"));
//		Debug.Log ("wInsets imgWidth:" + (int)imgWidth);
//		Debug.Log ("wInsets imgHight:" + (int)imgHight);
//		Debug.Log ("wInsets top:" + (int)setTop);
//		Debug.Log ("wInsets side:" + (int)setWidth);
//		Debug.Log ("wInsets Bottom:" + (int)setBottom);

		_wInsets.top = Mathf.RoundToInt(setTop);
		_wInsets.side = Mathf.RoundToInt(setWidth + blackSide);
		_wInsets.bottom = Mathf.RoundToInt(setBottom);

		return _wInsets;
	}

	public static InsetsParameter GetInsetsQuestBanner() 
	{
		// バナー基本サイズ
		float gBannerWidth = 240.0f;
		float gBannerHeight = 80.0f;
		// 両サイドに5%の余白
		float marginLeft = 0.6f;
		float marginRight = 0.0f;
		// 上部に13%の余白
		float marginTop = 0.09f;

		InsetsParameter _wInsets = new InsetsParameter();

		float aspect = 1.0f;

		// 画面固定のためサイドを計算
		float blackSide = screenWidth - (baseAspectWidth * (screenHeight / baseAspectHeight));
		float scWidth = screenWidth;
		if (blackSide <= 0) {
			blackSide = 0;
			aspect = screenHeight / ((screenWidth / baseAspectWidth) * baseAspectHeight);
		} else {
			scWidth = baseAspectWidth * (screenHeight / baseAspectHeight);
			blackSide = blackSide / 2;
		}

		float setLeft = scWidth * marginLeft * aspect;
		float setRight = scWidth * marginRight * aspect;
		float setTop = screenHeight * marginTop * aspect;

		float imgWidth = scWidth - (setLeft + setRight);
		float imgHight = gBannerHeight * (imgWidth / gBannerWidth);

		float setBottom = (screenHeight - setTop - imgHight) * aspect;

//		Debug.Log ("wInsets screenWidth:" + screenWidth);
//		Debug.Log ("wInsets screenHeight:" + screenHeight);
//		Debug.Log ("wInsets aspect:" + aspect.ToString("F3"));
//		Debug.Log ("wInsets blackSide:" + blackSide);
//		Debug.Log ("wInsets imgWidth:" + (int)imgWidth);
//		Debug.Log ("wInsets imgHight:" + (int)imgHight);
//		Debug.Log ("wInsets top:" + (int)setTop);
//		Debug.Log ("wInsets left:" + (int)setLeft);
//		Debug.Log ("wInsets right:" + (int)setRight);
//		Debug.Log ("wInsets Bottom:" + (int)setBottom);

		_wInsets.top = Mathf.RoundToInt(setTop);
		_wInsets.left = Mathf.RoundToInt(setLeft + blackSide);
		_wInsets.right = Mathf.RoundToInt(setRight + blackSide);
		_wInsets.bottom = Mathf.RoundToInt(setBottom);

		return _wInsets;
	}
}
