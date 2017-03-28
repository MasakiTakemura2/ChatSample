using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using uTools;
using ViewController;
using Http;
using Helper;
using System;
using NendUnityPlugin.AD;
using NendUnityPlugin.Common;

namespace EventManager
{
    public class MypageEventManager : SingletonMonoBehaviour<MypageEventManager>
    {
        #region Serialize Valiable
        [SerializeField]
        private Transform _canvas;

        [SerializeField]
        private GameObject _panelMypageMain;

        [SerializeField]
        private GameObject _panelProfileInput;

        [SerializeField]
        private GameObject _exampleProfileBody;

        [SerializeField]
        private SwipeRecognizer _backSwipe;

        [SerializeField]
        private Text _tmpText;

        [SerializeField]
        private GameObject _panelProfileSetTemplate;

        [SerializeField]
        private ProfTemplateInfiniteLimitScroll _profTemplatefInfiniteLimitScroll;

        [SerializeField]
        private GameObject _panelChat;

        [SerializeField]
        private Transform _scrollContent;

        [SerializeField]
        private GameObject _paneHistory;
        
        [SerializeField]
        private GameObject _loadingOverlay;
        
        [SerializeField]
        private GameObject _popupOverlay;

        [SerializeField]
        private GameObject _panelProfile;

        [SerializeField]
        private GameObject _panelRandMessage;

        [SerializeField]
        private GameObject _panelCampaign;

		[SerializeField]
		private GameObject _gestureObject;

        [SerializeField]
        private GameObject _webViewTerms;

        [SerializeField]
        private NendAdBanner _nendAdBanner;
        

        #endregion

		private static int _imobileBannerViewId = 0;
        public static bool _isUnserBanner = false;

        public string _prefId     = "";
        public string _cityId     = "";
        public string _tall       = "";
        public string _weight     = "";
        public string _bloodType  = "";
        public string _nickName   = "";
        public string _birthDate  = "";
        public string _profile    = "";
        
        public List<string> _bodyType      = new List<string>();
        public List<string> _hairStyle     = new List<string>();
        public List<string> _glasses       = new List<string>();
        public List<string> _type          = new List<string>();//TODO: 複数選択可・対応
        public List<string> _personality   = new List<string>();//TODO: 複数選択可・対応
        public List<string> _holiday       = new List<string>();
        public List<string> _annualIncome  = new List<string>();
        public List<string> _education     = new List<string>();
        public List<string> _housemate     = new List<string>();
        public List<string> _sibling       = new List<string>();
        public List<string> _alcohol       = new List<string>();
        public List<string> _tobacco       = new List<string>();
        public List<string> _car           = new List<string>();
        public List<string> _pet           = new List<string>();
        public List<string> _hobby         = new List<string>();
        public List<string> _interest      = new List<string>();
        public List<string> _marital       = new List<string>();

        //API更新する用の登録用データ
        public UserDataEntity.Basic _userDataBasic;
        public CurrentProfSettingStateType _currentProfSettingState;
        public ChatOrSelefImageType _chatOrSelefImageType;
        public bool _prefChange = false;

        public CurrentProfSettingStateType _cpsTypeSliderHeight = CurrentProfSettingStateType.None;
        public CurrentProfSettingStateType _cpsTypeSliderWeight = CurrentProfSettingStateType.None;
        
        #region life Cycle
        private float _time = 0;
       
        /// <summary>
        /// Ons the application pause.
        /// </summary>
        /// <returns>The application pause.</returns>
        /// <param name="pauseStatus">Pause status.</param>
        void OnApplicationPause (bool pauseStatus)
        {
            if (pauseStatus == true) {
                Debug.Log(" バックグラウンドから落ちた時");
            } else {
                string catchData = null;
#if UNITY_ANDROID && !UNITY_EDITOR
    catchData  = GCMService.GetPushMessage();
#elif UNITY_IPHONE && !UNITY_EDITOR
    catchData = NativeRecieveManager.GetPushMessageIos ();
#endif
                //プッシュキャッチした時の処理用。
                if (string.IsNullOrEmpty(catchData) == false) {
                    NotificationRecieveManager.NextSceneProccess (catchData);
                    NotificationRecieveManager._isCatch = true;
                }
            }
        }
        
        /// <summary>
        /// Start this instance.
        /// </summary>
        IEnumerator Start ()
        {
            _coverOrProf = CoverOrProfType.None;
            _time = 0;
            _tmpText.text = LocalMsgConst.TITLE_MYPAGE;
            _loadingOverlay.SetActive (true);
            //アンドロイドでバックグラウンドプッシュ?スプラッシュから?何かのタイミングで
            //static変数のデータが消え去ってしまうみたいなのでリカバー
            if (string.IsNullOrEmpty (AppStartLoadBalanceManager._userKey) == true) {
                //サーバーにイベント通知用Api(インストール時に一回のみ) ----------------------
                string _commonFileName = LocalFileConstants.GetLocalFileDir () + LocalFileConstants.COMMON_LOCAL_FILE_NAME;

                LocalFileHandler.Init (_commonFileName);

                //ファイルが作成されるまでポーリングして処理待ち
                while (System.IO.File.Exists (_commonFileName) == false)
                    yield return (System.IO.File.Exists (_commonFileName) == true);

                //ここでユーザーキーを取得。
                AppStartLoadBalanceManager._userKey = LocalFileHandler.GetString (LocalFileConstants.USER_KEY);

#if UNITY_ANDROID
    //ステータスバーを表示 //Android用
    ApplicationChrome.statusBarState = ApplicationChrome.States.VisibleOverContent;
#endif
            }

            //ユーザーの取得。
            new GetUserApi ();
            while (GetUserApi._success == false)
                yield return (GetUserApi._success == true);

    if (GetUserApi._httpCatchData.result.is_auto_renewable == "true") {
        CommonConstants.IS_PREMIUM = true;
    } else {
        CommonConstants.IS_PREMIUM = false;
    }
PremiumPushPanel.Instance.Init ();

            //性別が取得出来ていない場合の処理。
            if (GetUserApi._httpCatchData.result.user.sex_cd == "0") {
                PanelGenderSelectCommon.Instance.Init (); //性別選択のUIを表示。
                _loadingOverlay.SetActive (false);
                yield break;
            }

            AppStartLoadBalanceManager._gender = GetUserApi._httpCatchData.result.user.sex_cd;

            ///メンテナンスの場合、処理を止める。
            if (AppliEventController.Instance.MaintenanceCheck () == true) {
                _loadingOverlay.SetActive (false);
                yield break;
            }

            ///ユーザーのステータスをチェックする処理。
            AppliEventController.Instance.UserStatusProblem ();

            ///強制アップデートの場合、処理を止める。
            if (AppliEventController.Instance.ForceUpdateCheck () == true) {
                _loadingOverlay.SetActive (false);
                yield break;
            }
            
            ///アプリポップアップレビューの立ち上げ処理。
            AppliEventController.Instance.AppliReview ();
            
            //マスターデータの取得。
            if (InitDataApi._httpCatchData == null)
            {
                new InitDataApi ();
                while (InitDataApi._success == false)
                    yield return (InitDataApi._success == true);
            }

            //基本プロフィールを作成しているかどうかの判定。念の為、もう一回判定。
            if (string.IsNullOrEmpty (GetUserApi._httpCatchData.result.user.name) == false &&
                string.IsNullOrEmpty (GetUserApi._httpCatchData.result.user.pref) == false &&
                string.IsNullOrEmpty (GetUserApi._httpCatchData.result.user.city_id) == false &&
                string.IsNullOrEmpty (GetUserApi._httpCatchData.result.user.birth_date) == false) {
                //基本プロフィールを作成している。
                AppStartLoadBalanceManager._isBaseProfile = true;
            } else {
                //まだ、基本プロフィールを作成していない。
                AppStartLoadBalanceManager._isBaseProfile = false;
            }

            //こっからプッシュ飛んできた時の処理。   
            if (NotificationRecieveManager._isCatch == true) {
                switch (AppStartLoadBalanceManager._toScenePanel) {
                case CommonConstants.VIEW_HISTORY:
                    HeaderPanel.Instance.BackButtonSwitch (false);
                    HeaderPanel.Instance.BackButtonSwitch (true, HistoryListCloseEvent);
                    _tmpText.text = LocalMsgConst.TITLE_HISTORY;
                    PaneHistory.Instance.Init ();
                    PanelAnimate (_paneHistory);
                    _paneHistory.GetComponent<BoxCollider2D> ().enabled = false;
                    break;
                }
                NotificationRecieveManager._isCatch = false;
                yield break;
            }

            //ヘッダーパネル初期化
            PanelStateManager.InitPanelSet ();

            //データの初期化等
            DataInit ();

            //シーンの最初に表示する箇所、初期化。
            PanelMypageMain.Instance.Init ();


//審査レビューじゃない場合のみ広告表示。
if (GetUserApi._httpCatchData.result.review == "false") {

#if UNITY_IPHONE || UNITY_ANDROID && !UNITY_EDITOR
//有料会員か？
if (CommonConstants.IS_PREMIUM == false) {
// 広告の取得、表示処理

string spotId = "";
                if (CommonConstants.IS_AD_TEST == true) {
                    spotId = CommonConstants.IMOBILE_BANNER_SPOT_TEST_ID;
                    //インタースティシャル広告
						IMobileSdkAdsUnityPlugin.show(CommonConstants.IMOBILE_INTERSTATIAL_SPOT_TEST_ID); 
                } else {
                    spotId = CommonConstants.IMOBILE_BANNER_SPOT_ID;
                    //インタースティシャル広告
						IMobileSdkAdsUnityPlugin.show(CommonConstants.IMOBILE_INTERSTATIAL_SPOT_ID);
                }

                if (AppStartLoadBalanceManager.m_NendAdFullBoard != null){
                    AppStartLoadBalanceManager.m_NendAdFullBoard.Load ();
                    AppStartLoadBalanceManager.m_NendAdFullBoard.Show ();
                }
if (_isUnserBanner == false) {
    _imobileBannerViewId = IMobileSdkAdsUnityPlugin.show (
        spotId,
        IMobileSdkAdsUnityPlugin.AdType.BANNER,
        IMobileSdkAdsUnityPlugin.AdAlignPosition.CENTER,
        IMobileSdkAdsUnityPlugin.AdValignPosition.BOTTOM,
        true
    );
    _isUnserBanner = true;
}

    //_nendAdBanner.Show ();

} else {
					
Debug.Log (" i-mobile LOG: => Start This is Premium");

string spotId = "";
                if (CommonConstants.IS_AD_TEST == true) {
                    spotId = CommonConstants.IMOBILE_BANNER_SPOT_TEST_ID;
                    //インタースティシャル広告
					IMobileSdkAdsUnityPlugin.stop(CommonConstants.IMOBILE_INTERSTATIAL_SPOT_TEST_ID); 
                } else {
                    spotId = CommonConstants.IMOBILE_BANNER_SPOT_ID;
                    //インタースティシャル広告
					IMobileSdkAdsUnityPlugin.stop(CommonConstants.IMOBILE_INTERSTATIAL_SPOT_ID);
                }

    //バナー広告隠す処理。
	IMobileSdkAdsUnityPlugin.setVisibility (_imobileBannerViewId, false);

    //有料会員のため、広告ストップ
    IMobileSdkAdsUnityPlugin.stop (spotId);

    _nendAdBanner.Pause ();
    _nendAdBanner.Hide ();
    Destroy (_nendAdBanner.gameObject);
}

#endif
}

            _loadingOverlay.SetActive (false);
           
            yield break;
        }

        /// <summary>
        /// Initialize this instance.
        /// </summary>
		public IEnumerator Initialize () 
		{
			_tmpText.text = LocalMsgConst.TITLE_MYPAGE;
			PanelStateManager.InitPanelSet ();
			//ローカルファイルからマスターデータを取得してくる。 - ここから
			_loadingOverlay.SetActive (true);

			new GetUserApi ();
			while (GetUserApi._success == false)
				yield return (GetUserApi._success == true);

			AppStartLoadBalanceManager._userKey = GetUserApi._httpCatchData.result.user.user_key;

			AppStartLoadBalanceManager._gender  = GetUserApi._httpCatchData.result.user.sex_cd;


			//シーンの最初に表示する箇所、初期化。
			PanelMypageMain.Instance.Init ();

			_loadingOverlay.SetActive (false);
			//ローカルファイルからマスターデータを取得してくる。 - ここまで

			//TODO: プロフィールアップデートAPIが終わった後でユーザーデータ取得してきて照らし合わせる処理を書く。

			yield break;
		}
        #endregion



        #region 新着メッセージ
        /// <summary>
        /// News the message.
        /// </summary>
        /// <returns>The message.</returns>
        public void NewMessage ()
        {
            string commonFileName = LocalFileConstants.GetLocalFileDir () + LocalFileConstants.COMMON_LOCAL_FILE_NAME;
            LocalFileHandler.Init (commonFileName);
            LocalFileHandler.SetString ( LocalFileConstants.FROM_MYPAGE_SCENE, CommonConstants.MYPAGE_SCENE);
            LocalFileHandler.Flush ();
            SceneHandleManager.NextSceneRedirect (CommonConstants.MESSAGE_SCENE);
        }
        #endregion


        #region プロフィール変更画面。Open, Close
        /// <summary>
        /// Profiles the change open.
        /// </summary>
        /// <param name="animObj">Animation object.</param>
        public void ProfileChangeOpen ()
        {
            if (AppStartLoadBalanceManager._isBaseProfile == false) {
                HeaderPanel.Instance.BackButtonSwitch (false);
                HeaderPanel.Instance.BackButtonSwitch (true, PanelFooterButtonManager.Instance.BasicProfileClose);
                PanelAnimate (PanelStartInputUserData.Instance.gameObject);
                PanelStartInputUserData.Instance.Init ();
                return;
            }
            StartCoroutine (ProfileChangeOpenInumerator ());
        }

        /// <summary>
        /// Profiles the change open inumerator.
        /// </summary>
        /// <returns>The change open inumerator.</returns>
        private IEnumerator ProfileChangeOpenInumerator() 
        {
            GameObject go = Instantiate (Resources.Load (CommonConstants.PROFILE_CHANGE_PANEL)) as GameObject;
           
            while (go == null)
                yield return (go != null);

            go.transform.SetParent (_canvas, false);
            go.transform.SetSiblingIndex (1);
            go.name = CommonConstants.PROFILE_CHANGE_PANEL;

            string title = PanelStateManager.GetHeaderStringByKey ("PanelProfileChange");
            HeaderPanel.Instance.BackButtonSwitch (false);
            HeaderPanel.Instance.BackButtonSwitch (true, ProfileChangeEvent, title);
            DataInit ();
            go.GetComponent<PanelProfileChange>().Init ();
            _backSwipe.EventMessageTarget = go;
            PanelAnimate (go);

        }

        /// <summary>
        /// Profiles the change close.
        /// </summary>
        /// <param name="animObj">Animation object.</param>
        public void ProfileChangeClose (GameObject animObj)
        {
            string title = PanelStateManager.GetHeaderStringByKey ("PanelMypageMain");
            HeaderPanel.Instance.BackButtonSwitch (false, ProfileChangeEvent, title);
            PanelMypageMain.Instance.Init (true);

            StartCoroutine (BackAnimateToDestory (animObj));
             
        }

        /// <summary>
        /// Backs the animate to destory.
        /// </summary>
        /// <returns>The animate to destory.</returns>
        private IEnumerator BackAnimateToDestory(GameObject go)
        {
            BackButton (go);
            while (go.transform.localPosition.x <= 1000f) 
                yield return (go.transform.localPosition.x >= 1000f);
            Destroy (go);

            _cpsTypeSliderWeight = CurrentProfSettingStateType.None;
            _cpsTypeSliderHeight = CurrentProfSettingStateType.None;
        }

        /// <summary>
        /// Profiles the change event.
        /// イベント用に設定。
        /// </summary>
        public void ProfileChangeEvent()
        {
            if (PanelProfileChange.Instance != null)  {
                ProfileChangeClose (PanelProfileChange.Instance.gameObject);
            }

            
        }
        #endregion
        
        


        
        
        #region キャンペーンWEBVIEW
        /// <summary>
        /// Webviews the campaign open.
        /// </summary>
        /// <returns>The campaign open.</returns>
        /// <param name="animObj">Animation object.</param>
        public void WebviewCampaignOpen ( GameObject animObj )
        {
            HeaderPanel.Instance.BackButtonSwitch (false);
            string title = PanelStateManager.GetHeaderStringByKey ("PanelCampaign");
            HeaderPanel.Instance.BackButtonSwitch (true, WebviewTransCloseEvent, title);

            _panelCampaign.SetActive (true);
            PanelAnimate (animObj);
        }

        /// <summary>
        /// Webviews the campaign close.
        /// </summary>
        /// <returns>The campaign close.</returns>
        /// <param name="animObj">Animation object.</param>
        public void WebviewCampaignClose ( GameObject animObj )
        {
            string title = PanelStateManager.GetHeaderStringByKey ("PanelMypageMain");
            HeaderPanel.Instance.BackButtonSwitch (false, null, title);
            
            _panelCampaign.SetActive (false);
            BackButton (animObj);
        }
        
        void WebviewCampaignCloseEvent () {
            WebviewCampaignClose (_panelCampaign);
        }
        
        /// <summary>
        /// Webviews the trans close event.
        /// </summary>
        /// <returns>The trans close event.</returns>
        public void WebviewTransCloseEvent () 
        {
            WebviewCampaignClose (_panelCampaign);
        }
        #endregion
        

        #region 履歴、あしあと、お気に入り。Open, Close
        /// <summary>
        /// Histories the list open.
        /// </summary>
        /// <returns>The list open.</returns>
        /// <param name="animObj">Animation object.</param>
        public void HistoryListOpen (GameObject animObj) {
            HeaderPanel.Instance.BackButtonSwitch (false);
            string title = PanelStateManager.GetHeaderStringByKey ("PaneHistory");
            HeaderPanel.Instance.BackButtonSwitch (true, HistoryListCloseEvent, title);
            _backSwipe.EventMessageTarget = _paneHistory;
            PaneHistory.Instance.Init ();
            PanelAnimate (animObj);
        }
        
        /// <summary>
        /// Histories the list close.
        /// </summary>
        /// <returns>The list close.</returns>
        /// <param name="animObj">Animation object.</param>
        public void HistoryListClose (GameObject animObj) {
            string title = PanelStateManager.GetHeaderStringByKey ("PanelMypageMain");
            HeaderPanel.Instance.BackButtonSwitch (false, HistoryListCloseEvent, title);
            BackButton (animObj);
        }
        
        /// <summary>
        /// Histories the list close event.
        /// イベント設定用に
        /// </summary>
        /// <returns>The list close event.</returns>
        void HistoryListCloseEvent () {
            HistoryListClose (_paneHistory);
#if UNITY_IPHONE || UNITY_ANDROID && !UNITY_EDITOR
            if (GetUserApi._httpCatchData.result.review == "false" && CommonConstants.IS_PREMIUM == false) {
                IMobileSdkAdsUnityPlugin.show (CommonConstants.IMOBILE_FULL_SPOT_ID);
            }
#endif

        }
        #endregion


        #region その他セッティング画面。 Open, Close
        /// <summary>
        /// Others the setting.
        /// </summary>
        /// <returns>The setting.</returns>
        public void OtherSettingOpen ()
        {
            string title = PanelStateManager.GetHeaderStringByKey ("OtherSetting");
            HeaderPanel.Instance.BackButtonSwitch (false);
            HeaderPanel.Instance.BackButtonSwitch (true, OtherSettingCloseEvent, title);

            StartCoroutine (OtherSettingOpenInumerator ());
        }

        /// <summary>
        /// Others the setting open inumerator.
        /// </summary>
        /// <returns>The setting open inumerator.</returns>
        private IEnumerator OtherSettingOpenInumerator() 
        {
            GameObject go = Instantiate (Resources.Load (CommonConstants.OTHER_SETTING_PANEL)) as GameObject;
           
            while (go == null)
                yield return (go != null);

            go.transform.SetParent (_canvas, false);
            go.transform.SetSiblingIndex (1);
            go.name = CommonConstants.OTHER_SETTING_PANEL;

            DataInit ();

            go.GetComponent<OtherSetting>().Init ();
            _backSwipe.EventMessageTarget = go;
            PanelAnimate (go);
        }

        /// <summary>
        /// Others the setting close.
        /// </summary>
        /// <returns>The setting close.</returns>
        public void OtherSettingClose (GameObject animObj)
        {
            string title = PanelStateManager.GetHeaderStringByKey ("PanelMypageMain");
            HeaderPanel.Instance.BackButtonSwitch (false, OtherSettingCloseEvent, title);
            StartCoroutine (BackAnimateToDestory (animObj));
        }
        
        /// <summary>
        /// Others the setting close event.
        /// </summary>
        /// <returns>The setting close event.</returns>
        public void OtherSettingCloseEvent () {
            OtherSettingClose(OtherSetting.Instance.gameObject);
        }
        #endregion



        #region ランダムチャットOpen, Close
        /// <summary>
        /// Panels the rand message open.
        /// </summary>
        /// <returns>The rand message open.</returns>
        public void PanelRandMessageOpen ( GameObject animObj )
        {
//            if (AppStartLoadBalanceManager._isBaseProfile == false) {
//                PanelFooterButtonManager.Instance.NoRegistBaseProfile ();
//                return;
//            }

            HeaderPanel.Instance.BackButtonSwitch (false);
            string title = PanelStateManager.GetHeaderStringByKey ("PanelRandMessage");
            HeaderPanel.Instance.BackButtonSwitch (true, PanelRandMessageCloseEvent, title);
            _backSwipe.EventMessageTarget = _panelRandMessage;
            PanelRandMessage.Instance.Init ();
            PanelAnimate (animObj);            
        }
        
        /// <summary>
        /// Panels the rand message close.
        /// </summary>
        /// <returns>The rand message close.</returns>
        public void PanelRandMessageClose ( GameObject animObj ) 
        {
             string title =  PanelStateManager.GetHeaderStringByKey ("PanelMypageMain");
             HeaderPanel.Instance.BackButtonSwitch (false, null, title);
             
             BackButton (animObj);
        }
        
        /// <summary>
        /// Panels the rand message close event.
        /// </summary>
        /// <returns>The rand message close event.</returns>
        void PanelRandMessageCloseEvent () {
            PanelRandMessageClose (_panelRandMessage);
        }
        #endregion



//TODO: ここから下がネスト構造。
        #region カバー写真とプロフ写真のアップロード機能
        public CoverOrProfType _coverOrProf;
        
        public void ChatImageSet() {
            _chatOrSelefImageType = ChatOrSelefImageType.Chat;
        }
        
        /// <summary>
        /// Covers the or prof set.
        /// </summary>
        /// <returns>The or prof set.</returns>
        /// <param name="coverOrProf">Cover or prof.</param>
        public void CoverOrProfSet(string coverOrProf)
        {
            _chatOrSelefImageType = ChatOrSelefImageType.Self;

            if (coverOrProf == CoverOrProfType.Cover.ToString ()) {
                _coverOrProf = CoverOrProfType.Cover;
            } else if (coverOrProf == CoverOrProfType.Prof.ToString ()) {
                _coverOrProf = CoverOrProfType.Prof;
            }
        }

        /// <summary>
        /// Uploads the image.
        /// </summary>
        /// <returns>The image.</returns>
        public void UploadImageButton () 
        {
            if (_coverOrProf == CoverOrProfType.Cover) 
			{         
                Texture2D tex2D = PanelProfileChange.Instance._coverImage.mainTexture as Texture2D;
                new UploadUserImageApi (tex2D, null);
            } else if (_coverOrProf == CoverOrProfType.Prof) {

               Texture2D tex2D = PanelProfileChange.Instance._profImage.mainTexture as Texture2D;
                new UploadUserImageApi (null, tex2D);
            }
            StartCoroutine (UploadImageButtonWait());
        }

        /// <summary>
        /// Uploads the image button wait.
        /// </summary>
        /// <returns>The image button wait.</returns>
        private IEnumerator UploadImageButtonWait () {
            _loadingOverlay.SetActive (true);
            while (UploadUserImageApi._success == false)
                yield return (UploadUserImageApi._success == true);

            if (_coverOrProf == CoverOrProfType.Cover) {
                 PanelProfileChange.Instance._coverImage.texture = Resources.Load ("Texture/check_image_cover@2x") as Texture;
                 PanelMypageMain.Instance._coverImage.texture    = Resources.Load ("Texture/check_image_cover@2x") as Texture;
            } else if (_coverOrProf == CoverOrProfType.Prof) {
                PanelProfileChange.Instance._profImage.texture = Resources.Load ("Texture/check_image_user@2x") as Texture;
                PanelMypageMain.Instance._profImage.texture    = Resources.Load ("Texture/check_image_user@2x") as Texture;
            }

            _loadingOverlay.SetActive (false);

            Resources.UnloadUnusedAssets ();
            System.GC.Collect();
        }
        #endregion




        
        
        
//        #region 機種変更用パネル設定画面。Open, Close
//        /// <summary>
//        /// Mails the setting open.
//        /// </summary>
//        /// <param name="animObj">Animation object.</param>
//        public void ModelChangeOpen (GameObject animObj)
//        {
//            HeaderPanel.Instance.BackButtonSwitch (false);
//            string title =  PanelStateManager.GetHeaderStringByKey ("PanelDeviceChange");
//            HeaderPanel.Instance.BackButtonSwitch (true, ModelChangeCloseEvent, title);
//            OtherSetting.Instance.gameObject.GetComponent<BoxCollider2D> ().enabled = false;
//            _backSwipe.EventMessageTarget = _panelDeviceChange;
//            PanelAnimate (animObj);
//        }
//
//        /// <summary>
//        /// Mails the setting close.
//        /// </summary>
//        /// <param name="animObj">Animation object.</param>
//        public void ModelChangeClose (GameObject animObj)
//        {
//            HeaderPanel.Instance.BackButtonSwitch (false);
//            string title =  PanelStateManager.GetHeaderStringByKey ("OtherSetting");
//            HeaderPanel.Instance.BackButtonSwitch (true, OtherSettingCloseEvent, title);
//            OtherSetting.Instance.gameObject.GetComponent<BoxCollider2D> ().enabled = true;
//            BackButton (animObj);
//        }
//
//        /// <summary>
//        /// Mails the setting event.
//        /// </summary>
//        void ModelChangeCloseEvent ()
//        {
//            ModelChangeClose (_panelDeviceChange);
//        }
//        #endregion



        #region 都道府県、市区町村,プロフィール、血液型……………データが多数の場合選択された後の処理
        /// <summary>
        /// Gets the profile item.
        /// プロフィールの選択項目を選択した時の処理。
        /// </summary>
        /// <param name="Object">Object.</param>
        public void GetProfileItem (GameObject Object)
        {
            string  id =  Object.name;
            switch (_currentProfSettingState)
            {
            case CurrentProfSettingStateType.Pref:
                _prefId = id;//データセット
                _prefChange = true;
                _cityId = ""; //市区町村データ初期化。
                if (PanelProfileChange.Instance != null) {
                    PanelProfileChange.Instance.PanelEditClose ();
                } else if (PanelStartInputUserData.Instance != null) {
                    PanelStartInputUserData.Instance.PlaceOfOriginClose (PanelProfileSetTemplate.Instance.gameObject);
                }
                break;
            case CurrentProfSettingStateType.City:
                _cityId = id; //データセット
                if (PanelProfileChange.Instance != null) {
                    PanelProfileChange.Instance.PanelEditClose ();
                } else if (PanelStartInputUserData.Instance != null) {
                    PanelStartInputUserData.Instance.PlaceOfOriginClose (PanelProfileSetTemplate.Instance.gameObject);
                }
                break;
            case CurrentProfSettingStateType.BloodType:
                _bloodType = id;
                PanelProfileChange.Instance.PanelEditClose ();
                break;
            case CurrentProfSettingStateType.HairStyle:
                _hairStyle[0] = id;
                PanelProfileChange.Instance.PanelEditClose ();
                break;
            case CurrentProfSettingStateType.Profile:
                _profile = Object.GetComponent<Text> ().text;
                PanelProfileChange.Instance.PanelProfileInputClose ();
                break;
            case CurrentProfSettingStateType.BodyType:
                _bodyType[0]  = id;
                PanelProfileChange.Instance.PanelEditClose ();
                break;
            case CurrentProfSettingStateType.Glasses:
                _glasses[0] = id;
                PanelProfileChange.Instance.PanelEditClose ();
                break;
            case CurrentProfSettingStateType.Type:
                _type[0] = id;
                PanelProfileChange.Instance.PanelEditClose ();
                break;
            case CurrentProfSettingStateType.Personality:
                _personality[0] = id;
                PanelProfileChange.Instance.PanelEditClose ();
                break;
            case CurrentProfSettingStateType.Holiday:
                _holiday[0] = id;
                PanelProfileChange.Instance.PanelEditClose ();
                break;
            case CurrentProfSettingStateType.AnnualIncome:
                _annualIncome[0] = id;
                PanelProfileChange.Instance.PanelEditClose ();
                break;
            case CurrentProfSettingStateType.Education:
                _education[0] = id;
                PanelProfileChange.Instance.PanelEditClose ();
                break;
            case CurrentProfSettingStateType.Housemate:
                _housemate[0] = id;
                PanelProfileChange.Instance.PanelEditClose ();
                break;
            case CurrentProfSettingStateType.Sibling:
                _sibling[0] = id;
                PanelProfileChange.Instance.PanelEditClose ();
                break;
            case CurrentProfSettingStateType.Alcohol:
                _alcohol[0] = id;
                PanelProfileChange.Instance.PanelEditClose ();
                break;
            case CurrentProfSettingStateType.Tobacco:
                _tobacco[0] = id;
                PanelProfileChange.Instance.PanelEditClose ();
                break;
            case CurrentProfSettingStateType.Car:
                _car[0] = id;
                PanelProfileChange.Instance.PanelEditClose ();
                break;
            case CurrentProfSettingStateType.Pet:
                _pet[0] = id;
                PanelProfileChange.Instance.PanelEditClose ();
                break;
            case CurrentProfSettingStateType.Hobby:
                _hobby[0] = id;
                PanelProfileChange.Instance.PanelEditClose ();
                break;
            case CurrentProfSettingStateType.Interest:
                _interest[0] = id;
                PanelProfileChange.Instance.PanelEditClose ();
                break;
            case CurrentProfSettingStateType.Marital:
                _marital[0] = id;
                PanelProfileChange.Instance.PanelEditClose ();
                break;
            }

            if (PanelProfileChange.Instance != null) {
                PanelProfileChange.Instance.SetData (); //パラメータ反映
            } else if (PanelStartInputUserData.Instance != null) {
                PanelStartInputUserData.Instance.SetData ();
            }
        }
        #endregion
        
        
        #region 履歴系からプロファイル.[Open, Close].
        /// <summary>
        /// Histories to profile open.
        /// </summary>
        /// <returns>The to profile open.</returns>
        /// <param name="id">Identifier.</param>
        public void HistoryToProfileOpen (GameObject obj)
		{
            string id = obj.name; //user id
            ProfilePanel.Instance.Init (id);
            HeaderPanel.Instance.BackButtonSwitch (false);
            HeaderPanel.Instance.BackButtonSwitch (true, HistoryToProfileCloseEvent);
            _backSwipe.EventMessageTarget = _panelProfile;
            _paneHistory.GetComponent<BoxCollider2D> ().enabled = false;
            PanelPopupAnimate (_panelProfile);
        }

        /// <summary>
        /// Histories to profile close.
        /// </summary>
        /// <returns>The to profile close.</returns>
        public void HistoryToProfileClose (GameObject animObj) {
             HeaderPanel.Instance.BackButtonSwitch (false);
             HeaderPanel.Instance.BackButtonSwitch (true, HistoryListCloseEvent);
            _backSwipe.EventMessageTarget = _paneHistory;
            _paneHistory.GetComponent<BoxCollider2D> ().enabled = true;
            PanelPopupCloseAnimate (animObj);
        }
        
        /// <summary>
        /// Histories to profile close event.
        /// </summary>
        /// <returns>The to profile close event.</returns>
        void HistoryToProfileCloseEvent () {
            Debug.Log (_panelProfile.name +  " セットされるタイミングを知る。。。 ");
            HistoryToProfileClose (_panelProfile);
        }
        #endregion

        #region ボタンメソッド群
        /// <summary>
        /// Funcs the limit over.
        /// </summary>
        public void FuncLimitOver()
        {
            string commonFileName = LocalFileConstants.GetLocalFileDir () + LocalFileConstants.COMMON_LOCAL_FILE_NAME;
            LocalFileHandler.Init (commonFileName);
            LocalFileHandler.SetString ( LocalFileConstants.FROM_MYPAGE_SCENE, CommonConstants.MYPAGE_SCENE);
            LocalFileHandler.Flush ();
            SceneHandleManager.NextSceneRedirect (CommonConstants.PURCHASE_SCENE);            
        }

        /// <summary>
        /// Backs the button.
        /// </summary>
        /// <param name="fromObj">From object.</param>
        /// <param name="toObj">To object.</param>
        public void BackButton (GameObject fromObj) 
        {
            fromObj.GetComponent<uTweenPosition> ().delay    = 0.001f;
            fromObj.GetComponent<uTweenPosition> ().duration = 0.25f;
            fromObj.GetComponent<uTweenPosition> ().to      = new Vector2 (2500f,0);
            fromObj.GetComponent<uTweenPosition> ().from    = Vector3.zero;
            fromObj.GetComponent<uTweenPosition> ().ResetToBeginning ();
            fromObj.GetComponent<uTweenPosition> ().enabled = true;
        }



        /// <summary>
        /// Cleans the template.
        /// サーバーから引いてきた列挙しているデータをクリーンにする。
        /// </summary>
        public void CleanTemplate ()
        {
            _scrollContent.GetComponent<RectTransform> ().anchoredPosition = Vector2.zero;

            for (int i = 0; i < _scrollContent.childCount; i++) 
            {
                if (_scrollContent.GetChild (0).name  != _scrollContent.GetChild (i).name) {
                    Destroy (_scrollContent.GetChild (i).gameObject);
                }
            }
        }

        #endregion


        #region 内部メソッド
        /// <summary>
        /// Ages the check.
        /// 20歳未満を検知する用。タバコと酒を選択させないため trueの場合20歳未満
        /// </summary>
        /// <returns>The check.</returns>
        /// <param name="date">Date.</param>
        public bool AgeCheck (string date ){
            bool twentyCheck = false;
            if (string.IsNullOrEmpty (date) == true) {
                LessThanTwentyOpen (GameObject.FindGameObjectWithTag (CommonConstants.POPUP_BASIC_TAG));
                return true;
            }
            int age;
            // 年齢

            System.Text.RegularExpressions.Regex re = new System.Text.RegularExpressions.Regex("-");
            string pDate = re.Replace (date, "/");
Debug.Log (pDate + " pDatepDate ");
            DateTime birthDay = DateTime.Parse (pDate) ; // 誕生日を取得
            DateTime today    = DateTime.Today;
             
            age = today.Year - birthDay.Year;
            age -= birthDay > today.AddYears(-age) ? 1 : 0; // 誕生日が来てない場合は1歳引く
Debug.Log ("あなたの年齢は" + age);
            if (age < 20) {
                LessThanTwentyOpen (GameObject.FindGameObjectWithTag (CommonConstants.POPUP_BASIC_TAG));
                return true;
            }
            return twentyCheck;
        }
        
        /// <summary>
        /// Lesses the than twenty open.
        /// </summary>
        /// <returns>The than twenty open.</returns>
        /// <param name="target">Target.</param>
        public void LessThanTwentyOpen(GameObject target) {
             PopupPanel.Instance.PopMessageInsert(
                LocalMsgConst.LESS_THAN_20,
                LocalMsgConst.YES,
                LessThanTwentyCall
            );
            PanelPopupAnimate (target);
        }
        
        /// <summary>
        /// Lesses the than twenty call.
        /// </summary>
        /// <returns>The than twenty call.</returns>
        void LessThanTwentyCall () {
           PanelPopupCloseAnimate (GameObject.FindGameObjectWithTag(CommonConstants.POPUP_BASIC_TAG));
           PopupPanel.Instance.PopClean(LessThanTwentyCall);
        }

        /// <summary>
        /// Res the panel animate.
        /// </summary>
        /// <param name="target">Target.</param>
        public void PanelAnimate ( GameObject target )
        {
            Debug.Log (target.name + " テストでターゲットの名前を取得。 ");
            target.GetComponent<uTweenPosition> ().from = new Vector2 (2500f, 0);
            target.GetComponent<uTweenPosition> ().delay    = 0.001f;
            target.GetComponent<uTweenPosition> ().duration = 0.25f;
            target.GetComponent<uTweenPosition> ().to = Vector3.zero;
            target.GetComponent<uTweenPosition> ().ResetToBeginning ();
            target.GetComponent<uTweenPosition> ().enabled = true;
        }


        /// <summary>
        /// Res the panel animate.
        /// </summary>
        /// <param name="target">Target.</param>
        public void PanelPopupAnimate ( GameObject target )
        {
            Debug.Log (target.name + " target name > > ") ;
            _popupOverlay.SetActive (true);
            target.GetComponent<uTweenScale> ().from = Vector3.zero;
            target.GetComponent<uTweenScale> ().to = new Vector3 (1, 1 ,1 );
            target.GetComponent<uTweenScale> ().delay    = 0.01f;
            target.GetComponent<uTweenScale> ().duration = 0.25f;
            target.GetComponent<uTweenScale> ().ResetToBeginning ();
            target.GetComponent<uTweenScale> ().enabled = true;
        }

        /// <summary>
        /// Res the panel animate.
        /// </summary>
        /// <param name="target">Target.</param>
        public void PanelPopupCloseAnimate ( GameObject target )
        {
            _popupOverlay.SetActive (false);
            target.GetComponent<uTweenScale> ().delay    = 0.01f;
            target.GetComponent<uTweenScale> ().duration = 0.25f;
            target.GetComponent<uTweenScale> ().from = new Vector3 (1f, 1f ,1f );
            target.GetComponent<uTweenScale> ().to = Vector3.zero;
            target.GetComponent<uTweenScale> ().ResetToBeginning ();
            target.GetComponent<uTweenScale> ().enabled = true;
        }
       
        /// <summary>
        /// Backs the animation wait.
        /// </summary>
        /// <returns>The animation wait.</returns>
        /// <param name="animObj">Animation object.</param>
        public IEnumerator BackAnimWait(GameObject animObj)
        {
            while (animObj.GetComponent<RectTransform> ().anchoredPosition.x != animObj.GetComponent<uTweenPosition> ().to.x) {
                yield return (animObj.GetComponent<RectTransform> ().anchoredPosition.x == animObj.GetComponent<uTweenPosition> ().to.x);
            }

            _backSwipe.EventMessageTarget = animObj;
            yield break;
        }

        /// <summary>
        /// Init this instance.
        /// </summary>
        public void DataInit ()
        {
            var user = GetUserApi._httpCatchData.result.user;
            _userDataBasic = user;
            _prefId = user.pref;
            _cityId = user.city_id;
            _tall = user.height;
            _weight = user.weight;
            _bloodType = user.blood_type;
            _nickName = user.name;
            _birthDate = user.birth_date;
            _profile = user.profile;

            if (user.body_type != null) {
                _bodyType = user.body_type;
            } else {
                _bodyType.Add ("");
            }

            if (user.hair_style != null) {
                _hairStyle = user.hair_style;
            } else {
                _hairStyle.Add ("");
            }

            if (user.glasses != null) {
                _glasses = user.glasses;
            } else {
                _glasses.Add ("");
            }

            if (user.type != null) {
                _type = user.type;       //TODO: 複数選択可・対応
            } else {
                _type.Add ("");
            }

            if (user.personality != null) {
                _personality = user.personality;//TODO: 複数選択可・対応
            } else {
                _personality.Add ("");
            }

            if (user.holiday != null) {
                _holiday = user.holiday;
            } else {
                _holiday.Add ("");
            }

            if (user.annual_income != null) {
                _annualIncome = user.annual_income;
            } else {
                _annualIncome.Add ("");
            }

            if (user.education != null) {
                _education = user.education;
            } else {
                _education.Add ("");
            }

            if (user.housemate != null) {
                _housemate = user.housemate;
            } else {
                _housemate.Add ("");
            }

            if (user.sibling != null) {
                _sibling = user.sibling;
            } else {
                _sibling.Add ("");
            }

            if (user.alcohol != null) {
                _alcohol = user.alcohol;
            } else {
                _alcohol.Add ("");
            }

            if (user.tobacco != null) {
                _tobacco = user.tobacco;
            } else {
                _tobacco.Add ("");
            }
            
            if (user.car != null) {
                _car = user.car;
            } else {
                _car.Add ("");
            }

            if (user.pet != null) {
                _pet           = user.pet;
            } else {
                _pet.Add ("");
            }
            
            if (user.hobby != null) {
               _hobby         = user.hobby;
            } else {
                _hobby.Add ("");
            }
            
            if (user.interest != null) {
               _interest      = user.interest;    
            } else {
                _interest.Add ("");
            }
            
            if (user.marital != null) {
               _marital       = user.marital;
            } else {
                _marital.Add ("");
            }
        }
        
        #endregion


        /// <summary>
        /// Loadings the switch.
        /// </summary>
        /// <param name="isOn">If set to <c>true</c> is on.</param>
        public void LoadingSwitch(bool isOn) {
            _loadingOverlay.SetActive (isOn);
        }


        /// <summary>
        /// Webs the view terms switch.
        /// </summary>
        /// <param name="isSWitch">If set to <c>true</c> is S witch.</param>
        public GameObject WebViewTermsSwitch (bool isSWitch)
        {
            _webViewTerms.SetActive (isSWitch);
            return _webViewTerms;
        }
        
        /// <summary>
        /// Popups the overlay switch.
        /// </summary>
        /// <returns>The overlay switch.</returns>
        /// <param name="isOn">Is on.</param>
        public void popupOverlaySwitch( bool isOn ) 
        {
            _popupOverlay.SetActive (isOn);
        }
        
    }
}