using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using EventManager;
using ViewController;
using Http;
using Helper;
using uTools;
using LitJson;
#if UNITY_IPHONE && !UNITY_EDITOR
using NotificationServices = UnityEngine.iOS.NotificationServices;
using NotificationType = UnityEngine.iOS.NotificationType;
#endif

//NEND 用のプラグイン。
using NendUnityPlugin.AD.Native; 
using NendUnityPlugin.AD.FullBoard;
using NendUnityPlugin.Common;
using NendUnityPlugin.AD;


public class AppStartLoadBalanceManager : SingletonMonoBehaviour<AppStartLoadBalanceManager>
{
    //TODO: 全シーン使い回す用-ハッシュテーブルに持っていいかも…
    /// <summary>
    /// The user key.
    /// ユーザーのユニークキー
    /// </summary>
    public static string _userKey; //
    /// <summary>
    /// The gender.
    /// 女1:男2
    /// </summary>
    public static string _gender;  
    
    /// <summary>
    /// To push catch user identifier.
    /// push 時の誰から来たかのuser Id /必ずリセットすること。
    /// </summary>
    public static string _toPushCatchUserId;
    
    /// <summary>
    /// To scene panel.
    ///  必ずリセットする事。
    /// </summary>
    public static string _toScenePanel;
    
    /// <summary>
    /// The badge.
    /// メッセージのバッジ
    /// </summary>
    public static string _msgBadge;

    /// <summary>
    /// The is base profile.
    /// ユーザーのニックネーム、生年月日、お住まい都道府県、市区町村
    /// </summary>
    public static bool _isBaseProfile = false;

    /// <summary>
    /// The where from ads.
    /// </summary>
    public static MaioMovieSdkEvent.WhereFromAds _whereFromAds = MaioMovieSdkEvent.WhereFromAds.None;

    private string _deviceToken;

    [SerializeField]
    private GameObject _loadAnimation;

    [SerializeField]
    private GameObject _startAnimation;

    [SerializeField]
    private GameObject _popupOverlay;

    [SerializeField]
    private GameObject _loginBonus;
    
    [SerializeField]
    private NendAdBanner m_NendAdBanner;

    private string _commonFileName = "";

	public static INativeAdClient m_NendAdClient;
    public static NendAdFullBoard m_NendAdFullBoard;
    
    

    private bool _tokenSent;

    /// <summary>
    /// Notification recive.
    /// </summary>
    public class NotificationRecive
    {
        public string view_name; //遷移先
        public string id;        //user_id
    }
    
    
    void OnApplicationPause (bool pauseStatus)
    {
        if (pauseStatus == true) {
            //NativePicker.Instance.
            //ホームボタンを押してアプリがバックグランドに移行した時
#if !UNITY_EDITOR && UNITY_ANDROID
    AndroidJavaClass gps = new AndroidJavaClass ("com.tiam.gcmplugin.GcmRegistrar");
    gps.CallStatic("stopGPS");
    Debug.Log ("バックグランドに移行したよ = エラー出ないか？確認。");

#endif
        }
    }

    void Update ()
    {

#if UNITY_IPHONE && !UNITY_EDITOR
        
        if (_tokenSent == false)
        {
            byte[] deviceTokenBit = NotificationServices.deviceToken;

Debug.Log ("[ " + deviceTokenBit + " ] <- deviceTokenBitが取得されているか？ ");
            
            if (deviceTokenBit != null && string.IsNullOrEmpty (_userKey) == false)
            {
                _deviceToken = System.BitConverter.ToString(deviceTokenBit).Replace('-','%');
                _deviceToken = _deviceToken.Replace ("%", "");
Debug.Log (" this is a device token " +_deviceToken);
                new SetDeviceToken (_userKey, _deviceToken);
                _tokenSent = true;
            }
        }
#endif

    }

    /// <summary>
    /// Start this instance.
    /// </summary>
    IEnumerator Start ()
    {
#if UNITY_IPHONE && !UNITY_EDITOR
        
        NotificationServices.RegisterForNotifications(
                NotificationType.Alert | 
                NotificationType.Badge | 
                NotificationType.Sound);

        // notification clear
        NativeRecieveManager.RemotePushClear ();
#endif


        //ローディングアニメーション取得。
        _loadAnimation.SetActive (true);
        //アプリ全体で使用する変数をリセット。
        ResetStaticVariable ();

        //仮登録API ----------------------
        new PreRegistUser ();
        while (PreRegistUser._success == false)
            yield return (PreRegistUser._success == true);

        _userKey = PreRegistUser._httpCatchData.result.user.user_key;
        _gender  = PreRegistUser._httpCatchData.result.user.sex_cd;

        new GetUserApi ();
        while (GetUserApi._success == false)
            yield return (GetUserApi._success == true);

        //-------------------------------- 広告用設定　初期化処理　ここから --------------------------------
#if UNITY_IPHONE || UNITY_ANDROID && !UNITY_EDITOR
        if (GetUserApi._httpCatchData.result.review == "false")
        {
			string nendNativeAdApiKey_IOS = "";
			string nendNativeAdSpotId_IOS = "";

			string nendNativeAdApiKey_Android = "";
			string nendNativeAdSpotId_Android = "";

            if (CommonConstants.IS_AD_TEST == true) {
                //Imobile Test mode
                IMobileSdkAdsUnityPlugin.setTestMode (true);
        
                //Maio test mode
                Maio.SetAdTestMode (true);

                //バナー広告用 テスト
                 IMobileSdkAdsUnityPlugin.registerInline(
                     CommonConstants.IMOBILE_PARTNER_TEST_ID,
                     CommonConstants.IMOBILE_MDDIA_TEST_ID,
                     CommonConstants.IMOBILE_BANNER_SPOT_TEST_ID
                 ); 

                IMobileSdkAdsUnityPlugin.registerFullScreen(
                    CommonConstants.IMOBILE_PARTNER_TEST_ID,
                    CommonConstants.IMOBILE_MDDIA_TEST_ID,
                    CommonConstants.IMOBILE_INTERSTATIAL_SPOT_TEST_ID
                ); 
               
                //スポット情報の取得処理
                 //バナー広告用テスト
                IMobileSdkAdsUnityPlugin.start(CommonConstants.IMOBILE_BANNER_SPOT_TEST_ID);

                 //インタースティシャル広告テスト
                IMobileSdkAdsUnityPlugin.start(CommonConstants.IMOBILE_INTERSTATIAL_SPOT_TEST_ID);

				//NEND IOS用 テスト
				nendNativeAdApiKey_IOS = CommonConstants.NEND_NATIVEAD_API_TEST_ID;
				nendNativeAdSpotId_IOS = CommonConstants.NEND_NATIVEAD_SPOT_TEST_ID;

				//NEND Android用 テスト
				nendNativeAdApiKey_Android = CommonConstants.NEND_NATIVEAD_API_TEST_ID;
				nendNativeAdSpotId_Android = CommonConstants.NEND_NATIVEAD_SPOT_TEST_ID;
            } else {

                // //バナー広告用
                 IMobileSdkAdsUnityPlugin.registerInline(
                     CommonConstants.IMOBILE_PARTNER_ID,
                     CommonConstants.IMOBILE_MDDIA_ID,
                     CommonConstants.IMOBILE_BANNER_SPOT_ID
                 ); 

                //インタースティシャル用
                IMobileSdkAdsUnityPlugin.registerFullScreen(
                    CommonConstants.IMOBILE_PARTNER_ID,
                    CommonConstants.IMOBILE_MDDIA_ID,
                    CommonConstants.IMOBILE_INTERSTATIAL_SPOT_ID
                ); 
                
                //フル広告用
                IMobileSdkAdsUnityPlugin.registerFullScreen(
                    CommonConstants.IMOBILE_PARTNER_ID,
                    CommonConstants.IMOBILE_MDDIA_ID,
                    CommonConstants.IMOBILE_FULL_SPOT_ID
                ); 
                
                //スポット情報の取得処理
                //インタースティシャル
                IMobileSdkAdsUnityPlugin.start(CommonConstants.IMOBILE_INTERSTATIAL_SPOT_ID); 

                //バナー
                IMobileSdkAdsUnityPlugin.start(CommonConstants.IMOBILE_BANNER_SPOT_ID);
                
                //アイモバイル・フル広告用
                IMobileSdkAdsUnityPlugin.start(CommonConstants.IMOBILE_FULL_SPOT_ID); 

				//NEND Native AD IOS
				nendNativeAdApiKey_IOS = CommonConstants.NEND_NATIVEAD_API_IOS_ID;
				nendNativeAdSpotId_IOS = CommonConstants.NEND_NATIVEAD_SPOT_IOS_ID;

				//NEND Native AD ANDROID
				nendNativeAdApiKey_Android = CommonConstants.NEND_NATIVEAD_API_ANDROID_ID;
				nendNativeAdSpotId_Android = CommonConstants.NEND_NATIVEAD_SPOT_ANDROID_ID;
            }



			//Nend Native Ad セットアップ処理。
			#if UNITY_EDITOR
				// UnityEditorの場合は、広告枠のタイプを指定しテスト用の広告を使って表示の確認が行えます。
m_NendAdClient     = NativeAdClientFactory.NewClient (NativeAdClientFactory.NativeAdType.SmallSquare);
m_NendAdFullBoard  = NendAdFullBoard.NewFullBoardAd (CommonConstants.NEND_NATIVEFULL_SPOT_IOS_ID, CommonConstants.NEND_NATIVEFULL_API_IOS_ID);




			#elif UNITY_IPHONE
m_NendAdClient     = NativeAdClientFactory.NewClient (nendNativeAdSpotId_IOS, nendNativeAdApiKey_IOS);
m_NendAdFullBoard  = NendAdFullBoard.NewFullBoardAd (CommonConstants.NEND_NATIVEFULL_SPOT_IOS_ID, CommonConstants.NEND_NATIVEFULL_API_IOS_ID);
			#elif UNITY_ANDROID
m_NendAdClient = NativeAdClientFactory.NewClient (nendNativeAdSpotId_Android, nendNativeAdApiKey_Android);
//m_NendAdFullBoard  = NativeAdClientFactory.NewClient (nendNativeAdSpotId_Android, nendNativeAdApiKey_Android);
			#endif

            //バナー広告用。
            IMobileSdkAdsUnityPlugin.inlinieAdOrientation = IMobileSdkAdsUnityPlugin.ImobileSdkAdsInlineAdOrientation.PORTRAIT;
            
            
            //maio動画広告のクローズボタンが押された時のイベント処理。
            Maio.OnClosedAd += MaioMovieSdkEvent.OnClosed;
            
            //動画　広告用。
            Maio.Start (CommonConstants.MAIO_MEDIA_ID);

            NendAdBannerOrigenalManager.Instance.Init ();
    }

#endif
//-------------------------------- 広告用設定　初期化処理 ここまで--------------------------------

        //メンテナンスの場合、処理を止める。
        if (AppliEventController.Instance.MaintenanceCheck () == true) {
            _loadAnimation.SetActive (false);
            yield break;
        }

        //ユーザーのステータスをチェックする処理。
        AppliEventController.Instance.UserStatusProblem ();

        //強制アップデートの場合、処理を止める。
        if (AppliEventController.Instance.ForceUpdateCheck () == true) {
            _loadAnimation.SetActive (false);
            yield break;
        }

        //GPSを取得する処理。
        yield return StartCoroutine (GpsSet ());

#if UNITY_IPHONE && !UNITY_EDITOR
    NativeRecieveManager.GetPushMessageIos ();
#endif
        _tokenSent = false;
        _toPushCatchUserId = "";

#if UNITY_ANDROID
        //ステータスバーを表示 //Android用
        ApplicationChrome.statusBarState = ApplicationChrome.States.VisibleOverContent;
#endif


        //基本プロフィールを作成しているかどうかの判定。
        if (string.IsNullOrEmpty (PreRegistUser._httpCatchData.result.user.name) == false &&
            string.IsNullOrEmpty (PreRegistUser._httpCatchData.result.user.pref) == false &&
            string.IsNullOrEmpty (PreRegistUser._httpCatchData.result.user.city_id) == false &&
            string.IsNullOrEmpty (PreRegistUser._httpCatchData.result.user.birth_date) == false) {
            //基本プロフィールを作成している。
            _isBaseProfile = true;
        } else {
            //まだ、基本プロフィールを作成していない。
            _isBaseProfile = false;  
        }

        //サーバーにイベント通知用Api(インストール時に一回のみ) ----------------------
        _commonFileName = LocalFileConstants.GetLocalFileDir () + LocalFileConstants.COMMON_LOCAL_FILE_NAME;

        LocalFileHandler.Init (_commonFileName);

        //ファイルが作成されるまでポーリングして処理待ち
        while (System.IO.File.Exists (_commonFileName) == false)
            yield return (System.IO.File.Exists (_commonFileName) == true);

        //ここでユーザーキーを保存
        LocalFileHandler.SetString (LocalFileConstants.USER_KEY, _userKey);
        LocalFileHandler.Flush ();

        //レビュー時、すれ違い機能は表示しないようにしておく。リスクヘッジ。


#if UNITY_ANDROID && !UNITY_EDITOR
       _deviceToken = GCMService.GetRegistrationId ();

        //デバイストークンの登録API----------------------
        if (string.IsNullOrEmpty (_deviceToken) == false) {
            new SetDeviceToken (_userKey, _deviceToken);
            while (SetDeviceToken._success == false)
                yield return (SetDeviceToken._success == true);
        } else {
            GCMService.Registration ();
        }
#endif

        //初期マスターデータの取得
        //ファイルネーム比較してマスター更新分があったら再度Api飛ばしてローカルデータを更新する
        new InitDataApi ();
        while (InitDataApi._success == false)
            yield return (InitDataApi._success == true);

        //新着メッセージ未既読のAPI取得する
        new GetUnreadMessageCountApi ();
        while (GetUnreadMessageCountApi._success == false)
            yield return (GetUnreadMessageCountApi._success == true);
        _msgBadge = GetUnreadMessageCountApi._httpCatchData.result.count;

        var user = PreRegistUser._httpCatchData.result.user;

        if (string.IsNullOrEmpty (user.pref) == false && string.IsNullOrEmpty (user.city_id) == false &&
            user.pref != "0" && user.city_id != "0") {
            if (PreRegistUser._httpCatchData.result.complete.Count > 0) {
                //Mypageシーンに遷移 - 遷移する前にログインボーナス表示
                string loginComplete = PreRegistUser._httpCatchData.result.complete [0];
                if (string.IsNullOrEmpty (loginComplete) == false) {
                    StartEventManager.Instance.PanelPopupAnimate (_loginBonus);
                    _loginBonus.transform.GetChild (0).localScale = new Vector3 (1, 1, 1);
                    _loadAnimation.SetActive (false);
                    yield break;
                } else {
                    _loadAnimation.SetActive (false);
                    NextSceneProccess ();
                    yield break;
                }
            } else {
                _loadAnimation.SetActive (false);
                NextSceneProccess ();
                yield break;
            }
        } else {
            if (_gender != "0") {
                NextSceneProccess ();
                yield break;
            }

#if !UNITY_EDITOR && UNITY_IPHONE
        //初回時に通る処理。 => ポップアップ
        //説明用ポップアップ。
        PopupSecondSelectPanel.Instance.PopMessageInsert (
            LocalMsgConst.SELF_SHARE_INFO_CONFIRM,
            LocalMsgConst.APPROVAL,
            LocalMsgConst.DENIAL,
            PublicUserInfoAllow,
            PublicUserInfoDeny
        );
        GameObject obj = GameObject.FindGameObjectWithTag (CommonConstants.POPUP_SECOND_SELECT_TAG);
        StartEventManager.Instance.PanelPopupAnimate (obj);
#endif

        if (string.IsNullOrEmpty (LocalFileHandler.GetString (LocalFileConstants.VALID_URL_KEY)) == true) {
            
            if (GetUserApi._httpCatchData.result.review == "false")
            {
                new Valid (_userKey);
                while (Valid._success == false)
                    yield return (Valid._success == true);

                //start app installed. defaul browser open
                if (string.IsNullOrEmpty (Valid._httpCatchData.result.url) == false) {
                    //ローカルファイルにvalid_urlで保存 初回インストール時のみ通知するURLとする
                    LocalFileHandler.SetString (LocalFileConstants.VALID_URL_KEY, Valid._httpCatchData.result.url);
                    LocalFileHandler.Flush ();
                    var uri = new System.Uri (Valid._httpCatchData.result.url);
                    Application.OpenURL (uri.AbsoluteUri);
                }
            }
        }
        LocalFileHandler.HashAllClear ();


            _startAnimation.GetComponent<uTweenPosition> ().enabled = true;
            _loadAnimation.SetActive (false);
        }
    }
    
    /// <summary>
    /// Publics the allow deny.
    /// イベント用,承認した場合
    /// </summary>
    /// <returns>The allow deny.</returns>
    void PublicUserInfoAllow () 
    {
        _loadAnimation.SetActive (false);
        PopupSecondSelectPanel.Instance.PopClean ();
        GameObject obj = GameObject.FindGameObjectWithTag (CommonConstants.POPUP_SECOND_SELECT_TAG);
        StartEventManager.Instance.PanelPopupCloseAnimate (obj);
    }
    
    /// <summary>
    /// Publics the user info deny.
    /// イベント用、拒否した場合再度ポップアップ立ち上げ。
    /// </summary>
    /// <returns>The user info deny.</returns>
    void PublicUserInfoDeny () {
        GameObject obj = GameObject.FindGameObjectWithTag (CommonConstants.POPUP_SECOND_SELECT_TAG);
        StartEventManager.Instance.PanelPopupCloseAnimate (obj);
        PopupSecondSelectPanel.Instance.PopClean ();

        //説明用ポップアップ。
        PopupSecondSelectPanel.Instance.PopMessageInsert (
            LocalMsgConst.SELF_SHARE_INFO_CONFIRM,
            LocalMsgConst.APPROVAL,
            LocalMsgConst.DENIAL,
            PublicUserInfoAllow,
            PublicUserInfoDeny
        );
            
        StartEventManager.Instance.PanelPopupAnimate (obj);
    }

    /// <summary>
    /// Logins the bounus finish.
    /// イベント用
    /// </summary>
    /// <returns>The bounus finish.</returns>
    public void LoginBounusFinish ()
    {
        PopupPanel.Instance.PopClean ();
        NextSceneProccess ();
    }
    
    /// <summary>
    /// Gpses the set.
    /// </summary>
    /// <returns>The set.</returns>
    private IEnumerator GpsSet ()
    {
Debug.Log (" gps処理の関数、取得 ここから @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@");

        //_loadAnimation.SetActive (true);

#if !UNITY_EDITOR && UNITY_ANDROID
    AndroidJavaClass gps = new AndroidJavaClass ("com.tiam.gcmplugin.GcmRegistrar");
    string getCurrentLongitude = gps.CallStatic<string> ("getCurrentLongitude");
    string getCurrentLatitude  = gps.CallStatic<string> ("getCurrentLatitude");
    
    if (getCurrentLongitude == "Unknown" || getCurrentLatitude == "Unknown")
    {
        //StartCoroutine (GpsDefaultSet ());
        gps.CallStatic("stopGPS");
        yield break;
    } else {
        if (string.IsNullOrEmpty (getCurrentLongitude) == false && string.IsNullOrEmpty (getCurrentLatitude) == false) {
            new SetGpsApi (getCurrentLatitude, getCurrentLongitude);
            while (SetGpsApi._success == false)
                yield return (SetGpsApi._success == true);
    
            gps.CallStatic ("stopGPS");
            yield break;
        } else {
            //StartCoroutine (GpsDefaultSet ());
            gps.CallStatic("stopGPS");
            yield break;
        }
    }
#else
        if (!Input.location.isEnabledByUser) {
            Debug.Log ("GPS: 取れてへんよ");
            Input.location.Stop ();
            yield break;
        }
        Input.location.Start ();
        int maxWait = 40;
        while (Input.location.status == LocationServiceStatus.Initializing && maxWait > 0) {
            yield return new WaitForSeconds (1);
            maxWait--;
        }

        if (maxWait < 1) {
            Debug.Log ("Gps: Timed out");
            Input.location.Stop ();
            yield break;
        }

        if (Input.location.status == LocationServiceStatus.Failed) {
            Debug.Log ("Gps:  Unable to determine device location");
            Input.location.Stop ();
            yield break;
        } else {
#if UNITY_IPHONE && !UNITY_EDITOR

    NativeRecieveManager.GetLocationStatus ();

#endif
            string lat_local = Input.location.lastData.latitude.ToString ();
            string lng_local = Input.location.lastData.longitude.ToString ();

            if (string.IsNullOrEmpty (lat_local) == false || string.IsNullOrEmpty (lng_local) == false)
            {
                new SetGpsApi (lat_local, lng_local);
                while (SetGpsApi._success == false)
                    yield return (SetGpsApi._success == true);
            } else {
                if (lat_local != null && lng_local != null) 
                {
                    new SetGpsApi (lat_local, lng_local);
                    while (SetGpsApi._success == false)
                        yield return (SetGpsApi._success == true);

                } else {
                    Input.location.Stop ();
                    yield break;
                }
            }

            Debug.Log ("Location: " +
                  Input.location.lastData.latitude + " " +
                  Input.location.lastData.longitude + " " +
                  Input.location.lastData.altitude + " " +
                  Input.location.lastData.horizontalAccuracy + " " +
                  Input.location.lastData.timestamp);
        }

        Input.location.Stop ();
#endif
    }
		

    /// <summary>
    /// Pushs the catch proccess.
    /// </summary>
    /// <returns>The catch proccess.</returns>
    private void NextSceneProccess () 
    {
        string recieveData = null;
#if UNITY_ANDROID && !UNITY_EDITOR
    recieveData  = GCMService.GetPushMessage();
#elif UNITY_IPHONE && !UNITY_EDITOR
    recieveData = NativeRecieveManager.GetPushMessageIos ();
#endif

        if (string.IsNullOrEmpty(recieveData) == false ) {
           NotificationRecieveManager.NextSceneProccess (recieveData);
        } else {
            SceneHandleManager.NextSceneRedirect (CommonConstants.MYPAGE_SCENE);
        }
    }
    
    
    /// <summary>
    /// Resets the static variable.
    /// </summary>
    /// <returns>The static variable.</returns>
    private void ResetStaticVariable ()
    {
       _userKey = null;
       _gender = null;
       _toPushCatchUserId = null;
       _toScenePanel = null;
       _msgBadge = null;
    }
}