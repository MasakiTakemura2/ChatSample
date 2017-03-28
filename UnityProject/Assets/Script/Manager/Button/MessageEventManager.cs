using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using uTools;
using UnityEngine.UI;
using ViewController;
using Http;
using Helper;
using NendUnityPlugin.AD;
using NendUnityPlugin.Common;


namespace EventManager {
	public class MessageEventManager : SingletonMonoBehaviour<MessageEventManager>
    {
        #region Seriarize Variable
        [SerializeField]
        private Transform _messageTabParent;

        [SerializeField]
        private Transform _panelEazyNotify;

        [SerializeField]
        private Transform _panelChat;
	   
        [SerializeField]
        private Transform _panelProfile;

        [SerializeField]
        private Transform _headerTitle;

        [SerializeField]
        private SwipeRecognizer _backSwipe;

        [SerializeField]
        private GameObject _popupOverlay;
        
        [SerializeField]
        private GameObject _loadingOverlay;
        
        [SerializeField]
        private PanelEazyNotifyInfiniteScroll _panelEazyNotifyInfiniteScroll;

		[SerializeField]
		private MessageInfiniteLimitScroll _messageInfiniteLimitScroll;

        [SerializeField]
        private GameObject _panelTutorialMessge;
        
        [SerializeField]
        private GameObject _panelTutorialFavorite;

		[SerializeField]
		private Transform _talkListPanel;
        
		[SerializeField]
		private Transform _informationPanel;

		[SerializeField]
		private Transform _informationPanelScroll;

        [SerializeField]
        private GameObject _webViewTerms;
        
        [SerializeField]
        private GameObject _editButton;

        [SerializeField]
        private GameObject _popupMovie;

        [SerializeField]
        private NendAdBanner _nendAdBanner;

        #endregion

        private string _userId;

        public bool _isFromPush = false;
        public List<string> _msgReads = new List<string> ();

        public PanelState _panelState;

        public enum PanelState
        {
            Talklist,
            Favorite,
            Info
        }        
        
        #region Life Cycle
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
                if (string.IsNullOrEmpty(catchData) == false) {
                    NotificationRecieveManager.NextSceneProccess (catchData);
                }
            }
        }

        /// <summary>
        /// Start this instance.
        /// </summary>
        IEnumerator Start ()
        {
            _loadingOverlay.SetActive (true);
            // アンドロイドでバックグラウンドプッシュ?スプラッシュから?何かのタイミングで
            //static変数のデータが消え去ってしまうみたいなのでリカバー
            if (string.IsNullOrEmpty (AppStartLoadBalanceManager._userKey) == true) {
                //サーバーにイベント通知用Api(インストール時に一回のみ) ----------------------
                string _commonFileName = LocalFileConstants.GetLocalFileDir () + LocalFileConstants.COMMON_LOCAL_FILE_NAME;

                LocalFileHandler.Init (_commonFileName);

                //ファイルが作成されるまでポーリングして処理待ち
                while (System.IO.File.Exists (_commonFileName) == false)
                    yield return (System.IO.File.Exists (_commonFileName) == true);

                //ここでユーザーキーを取得
                AppStartLoadBalanceManager._userKey = LocalFileHandler.GetString (LocalFileConstants.USER_KEY);

#if UNITY_ANDROID
    //ステータスバーを表示 //Android用
    ApplicationChrome.statusBarState = ApplicationChrome.States.VisibleOverContent;
#endif
            }

            //ユーザーデータ取得。
            new GetUserApi ();
            while (GetUserApi._success == false)
                yield return (GetUserApi._success == true);
                

if (GetUserApi._httpCatchData.result.review == "false") {
#if UNITY_IPHONE || UNITY_ANDROID && !UNITY_EDITOR
//有料会員か？
if (CommonConstants.IS_PREMIUM == false) {
    //_nendAdBanner.Show ();
} else {
    _nendAdBanner.Pause ();
    _nendAdBanner.Hide ();
    Destroy (_nendAdBanner.gameObject);
}
#endif
}

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
                _isFromPush = true;

                switch (AppStartLoadBalanceManager._toScenePanel) {
                case CommonConstants.VIEW_MESSAGE_DETAIL:
                    if (_panelChat != null) {
                        _panelChat.gameObject.name = AppStartLoadBalanceManager._toPushCatchUserId;
                        TapThisMessage (_panelChat.gameObject);
                    }

                    NotificationRecieveManager._isCatch = false;
                    _loadingOverlay.SetActive (false);
                    yield break;
                    break;

                case CommonConstants.VIEW_PROFILE:
                    HeaderPanel.Instance.BackButtonSwitch (false);
                    HeaderPanel.Instance.BackButtonSwitch (true, ChatBackButton);
                    ProfilePanel.Instance.Init (AppStartLoadBalanceManager._toPushCatchUserId);
                    PanelProfileAnimate (_panelProfile.gameObject);
                    NotificationRecieveManager._isCatch = false;
                    _loadingOverlay.SetActive (false);
                    yield break;
                    break;
                //お知らせ詳細用プッシュが飛んできた場合。
                case CommonConstants.VIEW_INFO_DETAIL:
                    HeaderPanel.Instance.BackButtonSwitch (false);
                    HeaderPanel.Instance.BackButtonSwitch (true, InformationBackButton);
                    _backSwipe.EventMessageTarget = _informationPanel.gameObject;

                    if (AppStartLoadBalanceManager._toPushCatchUserId != null)
                    {
                        string id = AppStartLoadBalanceManager._toPushCatchUserId;
                    }
    
                    if (_informationPanel != null)
                    {
                        if (_informationPanel.GetComponent<uTweenPosition> ().from.x == 0) 
                        {
                            _informationPanel.GetComponent<uTweenPosition> ().from = _informationPanel.GetComponent<uTweenPosition> ().to;
                        }
    
                        _informationPanel.GetComponent<uTweenPosition> ().to = Vector3.zero;
                        _informationPanel.GetComponent<uTweenPosition> ().ResetToBeginning ();
                        _informationPanel.GetComponent<uTweenPosition> ().enabled = true;
    
                        // 初期化処理
                        _informationPanelScroll.GetComponent<PanelInformationInfiniteScroll> ().Init (AppStartLoadBalanceManager._toPushCatchUserId);
                    }
                    yield break;
                    break;                    
                }
            //キャンペーンからユーザーのプロフィールに行く特
            }
            else if (CampaignWebView._isFromCampaign == true)
            {
                HeaderPanel.Instance.BackButtonSwitch (false);
                HeaderPanel.Instance.BackButtonSwitch (true, ChatBackButton);
                ProfilePanel.Instance.Init (CampaignWebView._toUser);
                PanelProfileAnimate (_panelProfile.gameObject);
                _loadingOverlay.SetActive (false);
                CampaignWebView._isFromCampaign = false;
                yield break;
            }

            LocalFileHandler.Init (LocalFileConstants.GetLocalFileDir () + LocalFileConstants.COMMON_LOCAL_FILE_NAME);

            string fromScene = LocalFileHandler.GetString (LocalFileConstants.FROM_MYPAGE_SCENE);
            
            if (string.IsNullOrEmpty (fromScene) == false && fromScene == CommonConstants.MYPAGE_SCENE) {
                _backSwipe.EventMessageTarget = _panelEazyNotify.gameObject;
                HeaderPanel.Instance.BackButtonSwitch (true, BackMypageScene);
            }
            
            if (Helper.LocalFileHandler.GetBool (LocalFileConstants.TUTORIAL_MESSAGE_KEY) == false) {
                _panelTutorialMessge.SetActive (true);
            } else {
                _panelTutorialMessge.SetActive (false);
            }

            HeaderTab1 ();
            

			//新着メッセージ未既読のAPI取得する
			new GetUnreadMessageCountApi ();
			while (GetUnreadMessageCountApi._success == false) 
				yield return (GetUnreadMessageCountApi._success == true);
            
			AppStartLoadBalanceManager._msgBadge = GetUnreadMessageCountApi._httpCatchData.result.count;

            _loadingOverlay.SetActive (false); 
            yield break;
        }
        
        /// <summary>
        /// Backs the mypage scene.
        /// </summary>
        /// <returns>The mypage scene.</returns>
        void BackMypageScene () {
            HeaderPanel.Instance.BackButtonSwitch (false);
            SceneHandleManager.NextSceneRedirect (CommonConstants.MYPAGE_SCENE);
            Helper.LocalFileHandler.SetString (LocalFileConstants.FROM_MYPAGE_SCENE, "");
            Helper.LocalFileHandler.Flush ();
        }
        
        #endregion

        #region Button Click Scripting
        /// <summary>
        /// Tab1 this instance.
        /// (Temporary Method Name)
        /// </summary>
        public void HeaderTab1 () 
        {
            _editButton.SetActive (true);
            
            TabOnOffSwitcher (1);
            _headerTitle.GetComponent<Text> ().text = LocalMsgConst.TITLE_TALKLIST;

			_panelEazyNotifyInfiniteScroll.Init ("");
            _panelState = PanelState.Talklist;
        }

        /// <summary>
        /// Tab2 this instance.
        /// (Temporary Method Name)
        /// </summary>
        public void HeaderTab2 () 
        {
            _editButton.SetActive (true);

            TabOnOffSwitcher (2);
            _headerTitle.GetComponent<Text> ().text = LocalMsgConst.TITLE_FAVORITE;

			_panelEazyNotifyInfiniteScroll.Init ("1"); 
            _panelState = PanelState.Favorite;
        }

        /// <summary>
        /// Tab3 this instance.
        /// (Temporary Method Name)
        /// </summary>
        public void HeaderTab3 () 
        {
            _editButton.SetActive (false);

            TabOnOffSwitcher (3);
            _headerTitle.GetComponent<Text> ().text = LocalMsgConst.TITLE_INFOMATION;

			_panelEazyNotifyInfiniteScroll.Init ("2"); 
            _panelState = PanelState.Info;
            PanelTalkList.Instance.EditOpenButton ();
        }

        /// <summary>
        /// Funcs the limit over.
        /// </summary>
        public void FuncLimitOver() {
            //TODO: とりあえず課金シーンに飛ばす、後で書き換え
            SceneHandleManager.NextSceneRedirect (CommonConstants.PURCHASE_SCENE);            
        }

        /// <summary>
        /// Taps the this message.
        /// 自分と相手とのメッセージのやり取り.
        /// </summary>
        /// <param name="obj">Object.</param>
        public void TapThisMessage (GameObject obj)
        {
            if (_panelEazyNotifyInfiniteScroll.GetDisplayType () == "2") {
                HeaderPanel.Instance.BackButtonSwitch (false);
                HeaderPanel.Instance.BackButtonSwitch (true, InformationBackButton);
                _backSwipe.EventMessageTarget = _informationPanel.gameObject;

                if (obj != null) {
                    string id = obj.name;
                }

                if (_informationPanel != null) {
                    if (_informationPanel.GetComponent<uTweenPosition> ().from.x == 0) {
                        _informationPanel.GetComponent<uTweenPosition> ().from = _informationPanel.GetComponent<uTweenPosition> ().to;
                    }

                    _informationPanel.GetComponent<uTweenPosition> ().to = Vector3.zero;
                    _informationPanel.GetComponent<uTweenPosition> ().ResetToBeginning ();
                    _informationPanel.GetComponent<uTweenPosition> ().enabled = true;

                    // 初期化処理
                    _informationPanelScroll.GetComponent<PanelInformationInfiniteScroll> ().Init (obj.name);
                }
            } else {

_userId = obj.name;

#if UNITY_IPHONE || UNITY_ANDROID && !UNITY_EDITOR
                if (GetUserApi._httpCatchData.result.review == "false") {
                    if (CommonConstants.IS_PREMIUM == false) {
                        //動画広告が何もない場合はアイモバイル、インターステシャルを表示。
                        if (Maio.CanShow (CommonConstants.MAIO_ZONE_ID_1) == false) {
                            if (AppStartLoadBalanceManager.m_NendAdFullBoard != null){
                                AppStartLoadBalanceManager.m_NendAdFullBoard.Load ();
                                AppStartLoadBalanceManager.m_NendAdFullBoard.Show ();
                            }
                        } else {
                            //サーバーにイベント通知用Api(インストール時に一回のみ) ----------------------
                            string commonFileName = LocalFileConstants.GetLocalFileDir () + LocalFileConstants.COMMON_LOCAL_FILE_NAME;
    
                            LocalFileHandler.Init (commonFileName);
                            string isMovieFlag = LocalFileHandler.GetString (LocalFileConstants.MOVIE_POPUP_SHOW);
    
                            if (string.IsNullOrEmpty (isMovieFlag) == true) {
                                PanelPopupAnimate (_popupMovie);
                                return;
                            } else {
                                //問答無用で動画広告を表示
AppStartLoadBalanceManager._whereFromAds = MaioMovieSdkEvent.WhereFromAds.MessageList;
                                Maio.Show (CommonConstants.MAIO_ZONE_ID_1);
                                return;                        
                            }
                        }
                    } else {
                        //
                        IMobileSdkAdsUnityPlugin.stop ();
                    }
                }
#endif
                //審査中のときは、広告を一切挟まないので下記、処理。
                HeaderPanel.Instance.BackButtonSwitch (false);
                HeaderPanel.Instance.BackButtonSwitch (true, ChatBackButton);
                _backSwipe.EventMessageTarget = _panelChat.gameObject;

                if (obj != null) {
                    string id = obj.name;
                }

                if (_panelChat != null) {
                    if (_panelChat.GetComponent<uTweenPosition> ().from.x == 0) {
                        _panelChat.GetComponent<uTweenPosition> ().from = _panelChat.GetComponent<uTweenPosition> ().to;
                    }

                    _panelChat.GetComponent<uTweenPosition> ().to = Vector3.zero;
                    _panelChat.GetComponent<uTweenPosition> ().ResetToBeginning ();
                    _panelChat.GetComponent<uTweenPosition> ().enabled = true;

                    // 初期化処理
                    _panelChat.GetComponent<PanelChat> ()._maxAfterID = "";
                    _panelChat.GetComponent<PanelChat> ()._maxBeforeID = "";
                    _panelChat.GetComponent<PanelChat> ()._maxAfterIDBackup = "";
                    _panelChat.GetComponent<PanelChat> ()._maxBeforeIDBackup = "";
                    _panelChat.GetComponent<PanelChat> ()._listUpdateDisable = false;
                    _panelChat.GetComponent<PanelChat> ().Init (obj.name);
                }
            }
        }
        

        #region 動画広告用の処理。
        /// <summary>
        /// Movies the popup look button.
        /// 動画をみて、チャットルームを開く
        /// </summary>
        public void MoviePopupLookButton () {
AppStartLoadBalanceManager._whereFromAds = MaioMovieSdkEvent.WhereFromAds.MessageList;
            Maio.Show (CommonConstants.MAIO_ZONE_ID_1);
            PanelPopupCloseAnimate (_popupMovie);
        }
        
        /// <summary>
        /// Movies the popup dont show button.
        /// この警告を２度と表示しない
        /// </summary>
        public void MoviePopupDontShowButton () {
            //サーバーにイベント通知用Api(インストール時に一回のみ) ----------------------
            string commonFileName = LocalFileConstants.GetLocalFileDir () + LocalFileConstants.COMMON_LOCAL_FILE_NAME;
    
            LocalFileHandler.Init (commonFileName);
            LocalFileHandler.SetString (LocalFileConstants.MOVIE_POPUP_SHOW, "1");
            LocalFileHandler.Flush ();
            PanelPopupCloseAnimate (_popupMovie);
        }
        
        /// <summary>
        /// Movies the popup do nothing button.
        /// また今度
        /// </summary>
        public void MoviePopupDoNothingButton (){
            PanelPopupCloseAnimate (_popupMovie);
        }
        
        
        public void OnClosedAd (string zoneId)
        {
            HeaderPanel.Instance.BackButtonSwitch (false);
            HeaderPanel.Instance.BackButtonSwitch (true, ChatBackButton);
            _backSwipe.EventMessageTarget = _panelChat.gameObject;

            string id = _userId;
            
            if (_panelChat != null) 
            {
                if (_panelChat.GetComponent<uTweenPosition> ().from.x == 0) {
                    _panelChat.GetComponent<uTweenPosition> ().from = _panelChat.GetComponent<uTweenPosition> ().to;
                }

                _panelChat.GetComponent<uTweenPosition> ().to = Vector3.zero;
                _panelChat.GetComponent<uTweenPosition> ().ResetToBeginning ();
                _panelChat.GetComponent<uTweenPosition> ().enabled = true;

                // 初期化処理
                _panelChat.GetComponent<PanelChat> ()._maxAfterID = "";
                _panelChat.GetComponent<PanelChat> ()._maxBeforeID = "";
                _panelChat.GetComponent<PanelChat> ()._maxAfterIDBackup = "";
                _panelChat.GetComponent<PanelChat> ()._maxBeforeIDBackup = "";
                _panelChat.GetComponent<PanelChat> ()._listUpdateDisable = false;
                _panelChat.GetComponent<PanelChat> ().Init (id);
            }
        }
        #endregion

        /// <summary>
        /// Backs the button.
        /// </summary>
        /// <param name="fromObj">From object.</param>
        /// <param name="toObj">To object.</param>
        public void BackButton (GameObject fromObj) 
        {
            HeaderPanel.Instance.BackButtonSwitch (false, ChatBackButton);
            PanelChat.Instance.ResetScrollItem ();
            _headerTitle.GetComponent<Text>().text = LocalMsgConst.TITLE_TALKLIST;

            if (fromObj != null){
                fromObj.GetComponent<uTweenPosition> ().to      = fromObj.transform.GetComponent<uTweenPosition> ().from;
                fromObj.GetComponent<uTweenPosition> ().from    = Vector3.zero;
                fromObj.GetComponent<uTweenPosition> ().ResetToBeginning ();
                fromObj.GetComponent<uTweenPosition> ().enabled = true;
            }
        }

        /// <summary>
        /// Chats the back button.
        /// Event設定用。
        /// </summary>
        public void ChatBackButton () 
        {
            if (PanelFooterButtonManager.Instance != null) 
               PanelFooterButtonManager.Instance.gameObject.SetActive (true);

            BackButton (_panelChat.gameObject);
            _panelChat.GetComponent<BoxCollider2D> ().enabled    = true;

			_panelChat.GetComponent<PanelChat> ()._listUpdateDisable = true;


            if (_panelProfile.localScale.x != 0) 
			{
                PanelPopupCloseAnimate (_panelProfile.gameObject);
                _panelProfile.GetComponent<BoxCollider2D> ().enabled = true;
            }

            //_panelEazyNotifyInfiniteScroll.Init ("");
            NativeReadCheck();
            HeaderTab1 ();
#if UNITY_IPHONE || UNITY_ANDROID && !UNITY_EDITOR
            if (GetUserApi._httpCatchData.result.review == "false" && CommonConstants.IS_PREMIUM == false) {
                IMobileSdkAdsUnityPlugin.show (CommonConstants.IMOBILE_FULL_SPOT_ID);
            }
#endif
        }

		public void InformationBackButton () 
		{
			BackButton (_informationPanel.gameObject);
			_informationPanel.GetComponent<BoxCollider2D> ().enabled    = true;
            HeaderTab3 ();
		}

        /// <summary>
        /// Uses the point image on.
        /// ポイントを消費して画像を見る。
        /// </summary>
        public void UsePointImageOn (GameObject _mosaicObject)
		{
           //TODO: 制御文
            //int userPoint = 0; //データゲット
           //if (userPoint == 0) {
               //ここにポイントが足りない旨のポップアップ
            //} else if (userPoint >= 10?) {
                //ここにポイント消費処理。
                _mosaicObject.SetActive (false);
            //}
        }


		public void ProfileClose (GameObject animObj)
		{
			//messagepa.GetComponent<PanelBoardDetail> ().Init ();

			HeaderPanel.Instance.BackButtonSwitch (false, ProfileCloseEvent);
			//HeaderPanel.Instance.BackButtonSwitch (true, BoardDetailCloseEvent, PanelStateManager.GetHeaderStringByKey("BulletInBoardPanel"));
			//_backSwipe.EventMessageTarget = _panelBoardDetail;

			PanelProfileAnimate (animObj, true);

			//_panelBoardDetail.GetComponent<BoxCollider2D> ().enabled = true;

			_panelChat.GetComponent<PanelChat> ()._listUpdateDisable = true; //無効化処理
		}

		public void ProfileCloseEvent () 
		{
            if (PanelFooterButtonManager.Instance != null) 
               PanelFooterButtonManager.Instance.gameObject.SetActive (true);

			ProfileClose (_panelProfile.gameObject);
		}
		private void PanelProfileAnimate ( GameObject target, bool isTo = false )
		{
			target.GetComponent<uTweenScale> ().delay    = 0.001f;
			target.GetComponent<uTweenScale> ().duration = 0.25f;

			if (isTo == true)
			{
				target.GetComponent<uTweenScale> ().from = new Vector3 (1f, 1f ,1f );
				target.GetComponent<uTweenScale> ().to = Vector3.zero;
			} else {
				target.GetComponent<uTweenScale> ().from = Vector3.zero;
				target.GetComponent<uTweenScale> ().to = new Vector3 (1f, 1f ,1f );
			}

			target.GetComponent<uTweenScale> ().ResetToBeginning ();
			target.GetComponent<uTweenScale> ().enabled = true;
		}

        #endregion
        
        


        #region private method
        /// <summary>
        /// Tabs the on off switcher.
        /// </summary>
        private void TabOnOffSwitcher (int tabNumber) 
		{
            int cNo = tabNumber - 1;
            if (_messageTabParent != null && cNo >= 0)
			{
                foreach (Transform tab in _messageTabParent)
				{
                    //tab.GetChild(0).gameObject.SetActive (false);
                    if (_messageTabParent.GetChild (cNo).name != tab.name)
					{
                        tab.GetChild(1).gameObject.SetActive (false);
                    } else {
                        tab.GetChild(1).gameObject.SetActive (true);
                    }
                }
                _messageTabParent.GetChild (cNo).GetChild (0).gameObject.SetActive (true);
            }
        }




        /// <summary>
        /// Res the panel animate.
        /// </summary>
        /// <param name="target">Target.</param>
        public void PanelPopupAnimate ( GameObject target )
        {
            //ポップ用の背景セット
            _popupOverlay.SetActive (true);

            target.GetComponent<uTweenScale> ().from = Vector3.zero;
            target.GetComponent<uTweenScale> ().to = new Vector3 (1, 1 ,1 );
            target.GetComponent<uTweenScale> ().delay    = 0.001f;
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
            //ポップ用の背景外す
            _popupOverlay.SetActive (false);

            target.GetComponent<uTweenScale> ().delay    = 0.001f;
            target.GetComponent<uTweenScale> ().duration = 0.15f;
            target.GetComponent<uTweenScale> ().from = new Vector3 (1f, 1f ,1f );
            target.GetComponent<uTweenScale> ().to = Vector3.zero;
            target.GetComponent<uTweenScale> ().ResetToBeginning ();
            target.GetComponent<uTweenScale> ().enabled = true;
        }
        #endregion


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
        /// Natives the read check.
        /// </summary>
        private void NativeReadCheck()
        {
            if (_msgReads != null) 
            {
                for (int i = 0; i < _panelEazyNotifyInfiniteScroll.transform.childCount; i++)
                {
                    if (_msgReads.Contains (_panelEazyNotifyInfiniteScroll.transform.GetChild (i).name)) {
                        _panelEazyNotifyInfiniteScroll.transform.GetChild (i).GetChild (0).GetComponent<Text>().text = "";
                    }
                }
            }
        }
    }
}