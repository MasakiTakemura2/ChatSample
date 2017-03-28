using UnityEngine;
using System.Collections;
using ViewController;

/// <summary>
/// https://ghweb.info/post-3762.html#point1
/// Webview.
/// GameObjectに付いている前提
/// </summary>
public class CampaignWebView : SingletonMonoBehaviour<CampaignWebView>
{
    private UniWebView _webView;
    public static string _toUser;
    public static bool _isFromCampaign = false;

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

        _webView.AddUrlScheme ("campaign");
    }

    void OnEnable () 
    {
        string url= DomainData.GetWebviewDataURL(DomainData.WEBVIEW_CAMPAIGN) + AppStartLoadBalanceManager._userKey;
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
	
    /// <summary>
    /// Raises the received message event.
    /// WEBVIEWからネイティブアクセス処理。
    /// </summary>
    /// <param name="webView">Web view.</param>
    /// <param name="message">Message.</param>
	public void OnReceivedMessage (UniWebView webView, UniWebViewMessage message)
	{
        _toUser = "";
        if (message.path != null) {
            string key = message.path;
            if (message.path.Contains ("/")) {
                string[] profile = message.path.Split ('/');
                key = profile[0];
                _toUser      = profile[1];
            }
            switch (key) 
            {
            case "profile":
                _isFromCampaign = true;
                SceneHandleManager.NextSceneRedirect (CommonConstants.MESSAGE_SCENE);
                break;
            case "inquiry":
                HeaderPanel.Instance.BackButtonSwitch (false);
                HeaderPanel.Instance.BackButtonSwitch (true, InquiryBack);
                EventManager.MypageEventManager.Instance.BackButton (PanelCampaign.Instance.gameObject);
                PanelCampaign.Instance.gameObject.SetActive (false);
                OtherSetting.Instance.ContactOpen (PanelContact.Instance.gameObject);
                break;
            case "purchase":
                SceneHandleManager.NextSceneRedirect (CommonConstants.PURCHASE_SCENE);
                break;
            }
        }
	}
   
    void InquiryBack ()
    {
        EventManager.MypageEventManager.Instance.BackButton (PanelMypageMain.Instance.gameObject);
    }

}
