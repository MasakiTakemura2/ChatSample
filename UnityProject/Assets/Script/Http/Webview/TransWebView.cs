using UnityEngine;
using System.Collections;

/// <summary>
/// https://ghweb.info/post-3762.html#point1
/// Webview.
/// GameObjectに付いている前提
/// </summary>
public class TransWebView : SingletonMonoBehaviour<TransWebView>
{
    private UniWebView _webView;

    /// <summary>
    /// Awake this instance.
    /// </summary>
    void Awake ()
    {
        _webView = gameObject.AddComponent<UniWebView> ();

        //TODO: 予めロードしているデータのデータが存在しない場合は通信してサーバーから再取得する必要がある
        //UserDeviceEntity.UserDevice uDevice = Extention.CommonPlayerPrefs.CommonLoad<UserDeviceEntity.UserDevice> (ApiConstants.API_USER_DEVICE);

        _webView.CleanCache ();
        _webView.SetShowSpinnerWhenLoading (true);
		_webView.backButtonEnable = true;
        _webView.autoShowWhenLoadComplete = true;

        // 設定サイズを取得
        //WebviewLayout.InsetsParameter wInsets = WebviewLayout.GetInsetsWebview (0.1785f, 0.0f, 0.097f);
		WebviewLayout.InsetsParameter wInsets = WebviewLayout.GetInsetsWebview (0.13f, 0.0f, 0.2f);
		_webView.insets = new UniWebViewEdgeInsets(wInsets.top, wInsets.side, wInsets.bottom, wInsets.side);
        
        _webView.SetShowSpinnerWhenLoading (true);
        _webView.SetTransparentBackground(true);
        
    }

    void OnEnable () 
    {
        string url= DomainData.GetWebviewDataURL(DomainData.WEBVIEW_TRANS) + "?user_key=" + AppStartLoadBalanceManager._userKey;
Debug.Log (url + " 特定商取引法。 ");
        _webView.OnLoadComplete += OnLoadComplete;
		_webView.OnReceivedMessage += OnReceivedMessage;

        //string loadUrl = url.Replace ("{0}", uDevice.user_id);
        _webView.url = url;
        _webView.Load ();

    }

    void OnDisable () 
    {
        _webView.Hide ();
    }

    void OnApplicationPause(bool pauseStatus)
	{
        if (_webView != null) {
            if (pauseStatus) {
                _webView.Hide ();
            } else {
                _webView.Show();
            }
        }
    }

    public void OnGoBack ()
    {
        _webView.GoBack ();
    }

    void OnLoadComplete(UniWebView webView, bool success, string errorMessage)
    {
        // 通信エラー等制御
        if (success == false)
		{
            _webView.LoadHTMLString("<html><body><font color='#ffffff'></font></body></html>", "");
        }
    }

	// 地図のタップをしたらここにくる　スキームprofile:// とコールバックの指定を忘れずに
	public void OnReceivedMessage (UniWebView webView, UniWebViewMessage message)
	{
		if(ViewController.ProfilePanel.Instance != null)
		{
			EventManager.SearchEventManager.Instance.ImageButton ();
			EventManager.SearchEventManager.Instance.ProfileOpen(ViewController.ProfilePanel.Instance.gameObject);
		}
	}
}
