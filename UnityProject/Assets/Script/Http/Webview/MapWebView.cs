using UnityEngine;
using System.Collections;

/// <summary>
/// https://ghweb.info/post-3762.html#point1
/// Webview.
/// GameObjectに付いている前提
/// </summary>
public class MapWebView : SingletonMonoBehaviour<MapWebView>
{
    [SerializeField]
    private GameObject _loadingOverlay;
   
    private UniWebView _webView;
    private GameObject WebviewObject;

    /// <summary>
    /// Creates the web view.
    /// </summary>
    /// <returns>The web view.</returns>
    public void Init () 
    {
        _loadingOverlay.SetActive (true);
                
        LoadingTimer.Instance.IsTimerStop (true);

        this.gameObject.AddComponent<UniWebView> ();
        _webView = this.gameObject.GetComponent<UniWebView> ();

        string url= DomainData.GetWebviewDataURL(DomainData.WEBVIEW_MAP) + AppStartLoadBalanceManager._userKey;

        // 条件しばり追加
        string querystring = "";
        if(EventManager.SearchEventManager.Instance._lat != "")
        {
            querystring += "&lat=" + EventManager.SearchEventManager.Instance._lat;
        }
        if(EventManager.SearchEventManager.Instance._lng != "")
        {
            querystring += "&lng=" + EventManager.SearchEventManager.Instance._lng;
        }
        if(EventManager.SearchEventManager.Instance._sex != "")
        {
            querystring += "&sex_cd=" + EventManager.SearchEventManager.Instance._sex;
        }
        if(EventManager.SearchEventManager.Instance._ageFrom != "")
        {
            querystring += "&age_from=" + EventManager.SearchEventManager.Instance._ageFrom;
        }
        if(EventManager.SearchEventManager.Instance._ageTo != "")
        {
            querystring += "&age_to=" + EventManager.SearchEventManager.Instance._ageTo;
        }
        if(EventManager.SearchEventManager.Instance._heightFrom != "")
        {
            querystring += "&height_from=" + EventManager.SearchEventManager.Instance._heightFrom;
        }
        if(EventManager.SearchEventManager.Instance._heightTo != "")
        {
            querystring += "&height_to=" + EventManager.SearchEventManager.Instance._heightTo;
        }
        if(EventManager.SearchEventManager.Instance._bodyType != "")
        {
            querystring += "&body_type=" + WWW.EscapeURL(EventManager.SearchEventManager.Instance._bodyType);
        }
        if(EventManager.SearchEventManager.Instance._isImage != "")
        {
            querystring += "&is_image=" + EventManager.SearchEventManager.Instance._isImage;
        }
        if(EventManager.SearchEventManager.Instance._keyword != "")
        {
            querystring += "&keyword=" + EventManager.SearchEventManager.Instance._keyword;
        }
      
        _webView.OnLoadComplete += OnLoadComplete;
      
        url += querystring;
Debug.Log (url + " U R L ");
        _webView.url = url;
        
            
        _webView.backButtonEnable = false;
        _webView.autoShowWhenLoadComplete = true;
        _webView.SetTransparentBackground(true);
        _webView.AddUrlScheme ("profile");
        // 設定サイズを取得
        //WebviewLayout.InsetsParameter wInsets = WebviewLayout.GetInsetsWebview (0.1785f, 0.0f, 0.097f);
        WebviewLayout.InsetsParameter wInsets = WebviewLayout.GetInsetsWebview (0.19f, 0.0f, 0.2f);
        _webView.insets = new UniWebViewEdgeInsets(wInsets.top, wInsets.side, wInsets.bottom, wInsets.side);
        //_webView.SetShowSpinnerWhenLoading (true);
        _webView.OnReceivedMessage += OnReceivedMessage;
        _webView.Load ();
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

    /// <summary>
    /// Views the hide.
    /// 外部で使用する用。
    /// </summary>
    public void ViewHide () 
    {
        if (_webView != null) 
            _webView.Hide ();
    }

    public void OnGoBack ()
    {
        _webView.GoBack ();
    }


    void OnLoadComplete(UniWebView webView, bool success, string errorMessage)
    {
        if (success == true) {
            _loadingOverlay.SetActive (false);
            LoadingTimer.Instance.IsTimerStop (false);
            if (this.transform.childCount > 1)
            {
                Destroy (this.transform.GetChild (0));
            }
            _webView.Show ();
        }
        // 通信エラー等制御
        else if (success == false)
		{
            _webView.LoadHTMLString("<html><body><font color='#ffffff'></font></body></html>", "");
        }
    }

	// 地図のタップをしたらここにくる　スキームprofile:// とコールバックの指定を忘れずに
	public void OnReceivedMessage (UniWebView webView, UniWebViewMessage message)
	{
		if(ViewController.ProfilePanel.Instance != null)
		{
            ViewHide ();
			EventManager.SearchEventManager.Instance.SetGpsObjectActive(false);
			EventManager.SearchEventManager.Instance.ProfileOpenFromGPS(message.path);
		}
	}
}
