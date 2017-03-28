using UnityEngine;
using System.Collections;
using System.Text.RegularExpressions;

/// <summary>
/// https://ghweb.info/post-3762.html#point1
/// Webview.
/// GameObjectに付いている前提
/// </summary>
public class IntroductionWebView : SingletonMonoBehaviour<IntroductionWebView>
{
    [SerializeField]
    private GameObject _loadingOverlay;
   
    private UniWebView _webView;

    /// <summary>
    /// The on scheme.
    /// 連打対策のためのフラグ。
    /// </summary>
    private bool _onSchemeFlag;

    /// <summary>
    /// Gps Map the webview load.
    /// Map用のWEBViewデータを取得する
    /// </summary>
    public void Init ()
    {
        _loadingOverlay.SetActive (true);
        LoadingTimer.Instance.IsTimerStop (true);
        _webView = this.gameObject.AddComponent<UniWebView> ();
        _webView.backButtonEnable = false;
        _webView.autoShowWhenLoadComplete = true;

        // 設定サイズを取得
        WebviewLayout.InsetsParameter wInsets = WebviewLayout.GetInsetsWebview (0.19f, 0.0f, 0.2f);
        _webView.insets = new UniWebViewEdgeInsets(wInsets.top, wInsets.side, wInsets.bottom, wInsets.side);
        _webView.SetTransparentBackground(true);
        _webView.AddUrlScheme ("chat");

        string url= DomainData.GetWebviewDataURL(DomainData.WEBVIEW_INTRODUCTION);
        string querystring = "";

        querystring = "?user_key=" + AppStartLoadBalanceManager._userKey;

        _webView.url = url += querystring;
        _webView.Load ();
        _webView.OnLoadComplete += OnLoadComplete;
        _webView.OnReceivedMessage += OnReceivedMessage;
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
            _webView.Show ();
        }
        // 通信エラー等制御
        else if (success == false)
        {
            _webView.LoadHTMLString("<html><body><font color='#ffffff'></font></body></html>", "");
        }
    }

    /// <summary>
    /// Raises the received message event.
    /// </summary>
    /// <param name="webView">Web view.</param>
    /// <param name="message">Message.</param>
    public void OnReceivedMessage (UniWebView webView, UniWebViewMessage message)
    {
        if (!_onSchemeFlag)
        {
            _onSchemeFlag  = true;
            string loadUrl = message.args ["url"];

            //ブラウザ起動
            loadUrl = new AES256Cipher().AES_decrypt (loadUrl, HttpConstants.API_KEY_VALUE);

#if !UNITY_EDITOR && UNITY_ANDROID
            AndroidOpenUrl(loadUrl);
#else
Application.OpenURL(loadUrl);
#endif

            _onSchemeFlag = false;
        }
    }

    /// <summary>
    /// Androids the open URL.
    /// </summary>
    /// <param name="loadUrl">Load URL.</param>
    private static void AndroidOpenUrl( string loadUrl )
    {
        AndroidJavaClass classPlayer = new AndroidJavaClass("com.tiam.gcmplugin.MainActivity"); 
        AndroidJavaObject objActivity = classPlayer.GetStatic<AndroidJavaObject>("currentActivity"); 
        AndroidJavaClass classUri = new AndroidJavaClass("android.net.Uri");
        AndroidJavaObject objURL = classUri.CallStatic<AndroidJavaObject>( "parse", loadUrl );
        AndroidJavaObject objIntent = new AndroidJavaObject("android.content.Intent", "android.intent.action.VIEW", objURL ); 

        AndroidJavaObject launchIntent = null;
        try
        {
            launchIntent = objActivity.Call<AndroidJavaObject>( "startActivity", objIntent );
        }
        catch (System.Exception ex)
        {
            Debug.Log (" Catch " + ex.Message);
            if (launchIntent == null)
            {
                string expression = "package=(.*);";
                string output = "";
                Regex rex   = new Regex (expression);
                Match match = rex.Match(loadUrl);

                if (match.Success == true)
                {
                    foreach (var m in match.Groups)
                    {
                        output = m.ToString ();
                        break;
                    }

                    output = output.Replace ("package=", "");
                    output = output.Replace (";", "");
                    output = output.Replace ("end", "");
                }

                loadUrl = "https://play.google.com/store/apps/details?id=" + output;

                Application.OpenURL(loadUrl);
            }

        }
    }
}
