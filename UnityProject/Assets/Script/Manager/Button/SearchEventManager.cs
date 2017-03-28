using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using uTools;
using ViewController;
using Helper;
using Http;
using NendUnityPlugin.AD;
using NendUnityPlugin.Common;

/// <summary>
/// Search event manager.
/// ここのイベント処理、リファクタリングの必要有り。
/// </summary>
namespace EventManager
{
    /// <summary>
    /// Search event manager.
    /// </summary>
    public class SearchEventManager : SingletonMonoBehaviour<SearchEventManager>
    {
        [System.Serializable]
        public class SearchCondition
        {
            public string _lat = "";
            public string _lng = "";
            public string _order = "1";
            public string _sex = "";
            public string _ageFrom = "";
            public string _ageTo = "";
            public string _heightFrom = "";
            public string _heightTo = "";
            public string _bodyType = "";
            public string _isImage = "";
            public string _radius = "";
            public string _keyword = "";
            public string _nowShowPage = "1";
        }

        #region Serialize Field
        [SerializeField]
        private Transform _panelSubHeader;

        [SerializeField]
        private GameObject _panelGpsSearch;
		public void SetGpsObjectActive(bool activeflag)
		{
			_panelGpsSearch.SetActive(activeflag);
		}

        [SerializeField]
        private GameObject _panelImageList;

        [SerializeField]
        private GameObject _panelEazyNotify;

        [SerializeField]
        private GameObject _panelSearchCondition;

        [SerializeField]
        private GameObject _panelVer1;

        [SerializeField]
        private GameObject _panelVer2;
        
        [SerializeField]
        private GameObject _nextButton;

        [SerializeField]
        private Text _tmpText; //タイトル（仮）

        [SerializeField]
        private GameObject _panelProfile;

		[SerializeField]
		private GameObject _loadingOverLay;

        [SerializeField]
        private SwipeRecognizer _backSwipe;

		[SerializeField]
		private MessageInfiniteLimitScroll _messageInfiniteLimitScroll;

		[SerializeField]
		private GameObject _panelChat;

		[SerializeField]
        private GameObject _popupOverlay;

        [SerializeField]
        private GameObject _webViewTerms;

        [SerializeField]
        private NendAdBanner _nendAdBanner;

        private string _locationStatus;
        
		public bool _listConnectUpdateDisable = false;

        /// <summary>
        /// The tex caches.
        /// 一度WWWで取り込んだ画像をキャッシュでデータを読み込みの高速化。
        /// </summary>
        public Dictionary<string, Texture> _profTexCaches = new Dictionary<string, Texture> ();

		public enum CurrentSettingState
		{
			Sort,
			Sex,
			BodyType,
			Radius,
			Pref,
			City,
			BloodType,
			Profile,
			HairStyle,
			//BodyType,
			Glasses,
			Type,
			Personality,
			Holiday,
			AnnualIncome,
			Education,
			Housemate,
			Sibling,
			Alcohol,
			Tobacco,
			Car,
			Pet,
			Hobby,
			Interest,
			Marital,
		}
        

		public CurrentSettingState _currentSettingState;

		// 検索条件指定用　共通項目 ここの指定がシーン来た時の初期値
		public string _lat = "";
		public string _lng = "";
		public string _order = "1";
		public string _sex = "";
		public string _ageFrom = "";
		public string _ageTo = "";
		public string _heightFrom = "";
		public string _heightTo = "";
		public string _bodyType = "";
		public string _isImage = "";
		public string _radius = "";
		public string _keyword = "";
		public string _nowShowPage = "1";
        #endregion


        #region life cycle
        /// <summary>
        /// Ons the application quit.
        /// </summary>
        /// <returns>The application quit.</returns>
        void OnApplicationQuit () {
        }
        /// <summary>
        /// Ons the application pause.
        /// </summary>
        /// <returns>The application pause.</returns>
        /// <param name="pauseStatus">Pause status.</param>
        void OnApplicationPause (bool pauseStatus)
        {
            if (pauseStatus == true) {
                Debug.Log(" バックグラウンドから落ちた時 ");
            } else {
                string catchData = null;
#if UNITY_ANDROID && !UNITY_EDITOR
    catchData  = GCMService.GetPushMessage();
#elif UNITY_IPHONE && !UNITY_EDITOR
    catchData = NativeRecieveManager.GetPushMessageIos ();
#endif

    if (string.IsNullOrEmpty(catchData) == false ) {
    NotificationRecieveManager.NextSceneProccess (catchData);
}
            }
        }
        #endregion

		#region 条件検索の各項目処理
		// 検索条件を決定して　apiをとばす
		public void SendSearchApiButton()
		{
			ConditionClose(_panelSearchCondition);
            PanelSearchListChange.Instance.SetData ();
			switch (_statePanel)
			{
				case StatePanel.Image:
                    MapWebView.Instance.ViewHide ();

					if (_isTwoColumn) 
					{
						_panelVer2.GetComponent<PanelSeachLargeImageList> ().InitializeFromCondiiton ();
					} else {
						_panelVer1.GetComponent<PanelSeachSmallImageList> ().InitializeFromCondition ();
					}
                    HeaderPanel.Instance.UiButtonSwitch (false);
                    HeaderPanel.Instance.UiButtonSwitch (true, PictLayoutChangeEvent);
                    HeaderPanel.Instance.SetUIButton (_isTwoColumn);
				break;

				case StatePanel.Notify:
                    MapWebView.Instance.ViewHide ();
					PanelSeachList.Instance.InitializeFromCondition ();
				break;
            
                case StatePanel.Gps:
                    _panelGpsSearch.transform.GetChild (0).GetChild(0).GetComponent<MapWebView>().Init();
                break;
			}
		}

        /// <summary>
        /// Conditions the open.
        /// ユーザー検索の絞込み画面オープン。
        /// </summary>
        /// <returns>The open.</returns>
        /// <param name="animObj">Animation object.</param>
		public void ConditionOpen (GameObject animObj)
		{
            HeaderPanel.Instance.BackButtonSwitch (false);
			HeaderPanel.Instance.BackButtonSwitch (true, ConditionCloseEvent);
			_backSwipe.EventMessageTarget = _panelSearchCondition;
			PanelAnimate (animObj);

			_listConnectUpdateDisable = true;
		}

        /// <summary>
        /// Conditions the close.
        /// 絞込閉じる
        /// </summary>
        /// <param name="animObj">Animation object.</param>
		public void ConditionClose (GameObject animObj)
		{
			HeaderPanel.Instance.BackButtonSwitch (false);
            HeaderPanel.Instance.UiButtonSwitch (false);
            HeaderPanel.Instance.UiButtonSwitch (true, PictLayoutChangeEvent);
			BackButton (animObj);
			_listConnectUpdateDisable = false;
		}
        /// <summary>
        /// Conditions the close event.
        /// 絞込イベント設定用
        /// </summary>
		void ConditionCloseEvent ()
		{
			ConditionClose (_panelSearchCondition);
		}
		#endregion


      
        #region member variable
        //private bool _isBackButtonTap = false;
		public bool _isTwoColumn = false;//true;
        //ステートチェック。
		public StatePanel _statePanel = StatePanel.Image;
        public enum StatePanel
        {
            Gps,
            Image,
            Notify,
            Condition
        }
        #endregion

        #region Life Cycle
        /// <summary>
        /// Start this instance.
        /// </summary>
        IEnumerator Start ()
		{      
            _tmpText.text = LocalMsgConst.TITLE_SEARCH;
            _loadingOverLay.SetActive (true);
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
                _loadingOverLay.SetActive (false);
                yield break;
            }

            AppStartLoadBalanceManager._gender = GetUserApi._httpCatchData.result.user.sex_cd;

            ///メンテナンスの場合、処理を止める。
            if (AppliEventController.Instance.MaintenanceCheck () == true) {
                _loadingOverLay.SetActive (false);
                yield break;
            }

            ///強制アップデートの場合、処理を止める。
            if (AppliEventController.Instance.ForceUpdateCheck () == true) {
                _loadingOverLay.SetActive (false);
                yield break;
            }
            
            ///アプリポップアップレビューの立ち上げ処理。
            AppliEventController.Instance.AppliReview ();

            //マスターデータ取得。
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

            HeaderPanel.Instance.UiButtonSwitch (true, PictLayoutChangeEvent);

			#if UNITY_EDITOR
				yield return StartCoroutine (CallInitApi());
			#endif
                     
            SearchListButton ();
            _statePanel = StatePanel.Notify;
            

            _loadingOverLay.SetActive (false);
            yield break;
        }

        /// <summary>
        /// Calls the init API.
        /// </summary>
        /// <returns>The init API.</returns>
		private IEnumerator CallInitApi()
		{
			_loadingOverLay.SetActive (true);
			new InitDataApi();
			while (InitDataApi._success == false) 
			{
				yield return (InitDataApi._success == true);
			}
			_loadingOverLay.SetActive (false);
		}      
        #endregion

        #region Button Method
        /// <summary>
        /// Gps the reload button.
        ///  </summary>
        public void PictLayoutChange ()
        {
			if (_isTwoColumn)
			{
				Debug.Log("レイアウト切り替え　false");
				_isTwoColumn = false;
			} else {
				Debug.Log("レイアウト切り替え　true");
				_isTwoColumn = true;
			}

            if (_isTwoColumn) 
			{
               _nextButton.SetActive (false);
                _panelVer1.SetActive (false);
                _panelVer2.SetActive (true);

				_panelVer2.GetComponent<PanelSeachLargeImageList> ().InitializeFromCondiiton ();

				Debug.Log("レイアウト切り替え　2枚ずつ");

            } else {
                _nextButton.SetActive (true);
                _panelVer1.SetActive (true);
                _panelVer2.SetActive (false);                

				_panelVer1.GetComponent<PanelSeachSmallImageList> ().InitializeFromCondition ();

				Debug.Log("レイアウト切り替え　4枚ずつ");
            }

			// ボタンの画像切替
			HeaderPanel.Instance.SetUIButton (_isTwoColumn);

        }

        /// <summary>
        /// Picts the layout change event.
        /// </summary>
        public void PictLayoutChangeEvent()
		{
            PictLayoutChange ();
        }

        /// <summary>
        /// Tab1 this instance.
        /// (Temporary Method Name)
        /// </summary>
        public void GpsButton () 
        {
            if (_statePanel == StatePanel.Gps) {
                return;
            }


            if (AppStartLoadBalanceManager._isBaseProfile == false){
                PanelFooterButtonManager.Instance.NoRegistBaseProfile ();
                return;
            }
            if (string.IsNullOrEmpty (GetUserApi._httpCatchData.result.user.lat) == true ||
                string.IsNullOrEmpty (GetUserApi._httpCatchData.result.user.lng) == true
            ) {
                //緯度、経度どちらかのデータがない場合、表示させないように制御。
                PopupPanel.Instance.PopMessageInsert (
                    LocalMsgConst.ERROR_NOT_MAP_SHOW,
                    LocalMsgConst.OK,
                    ErrorNotGpsShow
                );
                GameObject notShowPopObj = GameObject.FindGameObjectWithTag (CommonConstants.POPUP_BASIC_TAG);
                PanelPopupAnimate (notShowPopObj);
                return;
            }

            TabOnOffSwitcher (1);

#if UNITY_IOS && !UNITY_EDITOR
Debug.Log (" Test Test Test Test Test Test Test Test Test Test Test Test Test Test Test ");
//    _locationStatus = NativeRecieveManager.GetLocationStatus ();
Debug.Log (" Test Test Test Test Test Test Test Test Test Test Test Test Test Test Test ");

    //GPSマップ表示の可否
    //説明用ポップアップ。
    PopupSecondSelectPanel.Instance.PopMessageInsert (
        LocalMsgConst.CHECKIN_CONFIRM,
        LocalMsgConst.APPROVAL,
        LocalMsgConst.DENIAL,
        LocatePermissionAllow,
        LocatePermissionDeny
    );
GameObject obj = GameObject.FindGameObjectWithTag (CommonConstants.POPUP_SECOND_SELECT_TAG);
PanelPopupAnimate (obj);
#else

            if (_panelGpsSearch != null) {
                PanelAnimate (_panelGpsSearch);
                _panelGpsSearch.transform.GetChild(0).GetChild(0).GetComponent<MapWebView>().Init ();
            }

            switch (_statePanel)
            {
                case StatePanel.Image:
                BackButton (_panelImageList);
                break;  
                case StatePanel.Condition:
                BackButton (_panelSearchCondition);
                break;
            case StatePanel.Notify:
                BackButton (_panelEazyNotify);
                break;
            }
            _statePanel = StatePanel.Gps;
            HeaderPanel.Instance.UiButtonSwitch (false, PictLayoutChangeEvent);
#endif

        }
        
        /// <summary>
        /// Locates the permission allow.
        ///イベント用
        /// </summary>
        /// <returns>The permission allow.</returns>
        void LocatePermissionAllow ()
        {
            PopupPanel.Instance.PopClean ();
            GameObject obj = GameObject.FindGameObjectWithTag (CommonConstants.POPUP_SECOND_SELECT_TAG);
            PanelPopupCloseAnimate (obj);
            
            if (_panelGpsSearch != null) {
                _panelGpsSearch.transform.GetChild(0).GetChild(0).GetComponent<MapWebView> ().Init ();
            }
    
            switch (_statePanel)
            {
                case StatePanel.Image:
                    BackButton (_panelImageList);
                break;  
                case StatePanel.Condition:
                    BackButton (_panelSearchCondition);
                break;
            case StatePanel.Notify:
                    BackButton (_panelEazyNotify);
                break;
            }
            _statePanel = StatePanel.Gps;
            HeaderPanel.Instance.UiButtonSwitch (false, PictLayoutChangeEvent);            
        }
        
        /// <summary>
        /// Locates the permission deny.
        /// イベント用
        /// </summary>
        /// <returns>The permission deny.</returns>
        void LocatePermissionDeny ()
        {
            GameObject obj = GameObject.FindGameObjectWithTag (CommonConstants.POPUP_SECOND_SELECT_TAG);
            PanelPopupCloseAnimate (obj);
            PopupPanel.Instance.PopClean ();
        }

        /// <summary>
        /// Errors the not gps show.
        /// 緯度、経度とれてない場合。
        /// </summary>
        void ErrorNotGpsShow ()
        {
            GameObject obj = GameObject.FindGameObjectWithTag (CommonConstants.POPUP_BASIC_TAG);
            PanelPopupCloseAnimate (obj);
            PopupPanel.Instance.PopClean ();
        }

        /// <summary>
        /// Tab2 this instance.
        /// (Temporary Method Name)
        /// </summary>
        public void ImageButton ()
        {
			if (_panelGpsSearch != null) {
                _panelGpsSearch.transform.GetChild(0).GetChild(0).GetComponent<MapWebView> ().ViewHide();
			}

            TabOnOffSwitcher (2);

            switch (_statePanel)
            {
	            case StatePanel.Condition:
    	            BackButton (_panelSearchCondition);
                break;
        	    case StatePanel.Notify:
            	    BackButton (_panelEazyNotify);
                break;
                case StatePanel.Gps:
                    BackButton (_panelGpsSearch);
                break;
            }

            PanelAnimate (_panelImageList);

            _statePanel = StatePanel.Image;
			HeaderPanel.Instance.UiButtonSwitch (false);
           	HeaderPanel.Instance.UiButtonSwitch (true, PictLayoutChangeEvent);

			// リストの初期化
			if (_isTwoColumn) 
			{
				_panelVer2.GetComponent<PanelSeachLargeImageList> ().InitializeFromCondiiton ();
			} else {
				_panelVer1.GetComponent<PanelSeachSmallImageList> ().InitializeFromCondition ();
			}
        }

        /// <summary>
        /// Tab3 this instance.
        /// (Temporary Method Name)
        /// </summary>
        public void SearchListButton ()
        {
            if (AppStartLoadBalanceManager._isBaseProfile == false){
                PanelFooterButtonManager.Instance.NoRegistBaseProfile ();
                return;
            }

			if (_panelGpsSearch != null) 
			{
                _panelGpsSearch.transform.GetChild(0).GetChild(0).GetComponent<MapWebView> ().ViewHide();
			}

            TabOnOffSwitcher (3);

            switch (_statePanel)
            {
            	case StatePanel.Condition:
                	BackButton (_panelSearchCondition);
                break;
            	case StatePanel.Image:
                	BackButton (_panelImageList);
                break;
                case StatePanel.Gps:
                    BackButton (_panelGpsSearch);
                break;
            }

            PanelAnimate (_panelEazyNotify);

            _statePanel = StatePanel.Notify;
            HeaderPanel.Instance.UiButtonSwitch (false);

			_panelEazyNotify.GetComponent<PanelSeachList> ().InitializeFromCondition ();
        }

        /// <summary>
        /// Tab4 this instance.
        /// (Temporary Method Name)
        /// </summary>
        public void ConditionButton () 
        {
            if (AppStartLoadBalanceManager._isBaseProfile == false){
                PanelFooterButtonManager.Instance.NoRegistBaseProfile ();
                return;
            }

            _backSwipe.EventMessageTarget = _panelSearchCondition;
            
			switch (_statePanel)
            {
            	case StatePanel.Image:
               		TabOnOffSwitcher (2);
               		_statePanel = StatePanel.Image;
               		BackButton (_panelImageList);
                break;
            	
				case StatePanel.Notify:
                	TabOnOffSwitcher (3);
               		_statePanel = StatePanel.Notify;
                	BackButton (_panelEazyNotify);
                break;

                case StatePanel.Gps:
                    TabOnOffSwitcher (1);
                    _statePanel = StatePanel.Gps;
                    BackButton (_panelGpsSearch);                
                break;
            }

			if(_listConnectUpdateDisable == true)
			{
				return;
			}

            if (_panelGpsSearch != null) {
                _panelGpsSearch.transform.GetChild (0).GetChild (0).GetComponent<MapWebView> ().ViewHide ();
            }

#if !UNITY_EDITOR && (UNITY_ANDROID || UNITY_IOS)
PanelSearchListChange.Instance.NativePickerInit ();
#else
PanelSearchListChange.Instance.Init ();
#endif

            PanelAnimate (_panelSearchCondition);

            HeaderPanel.Instance.UiButtonSwitch (false);
			HeaderPanel.Instance.BackButtonSwitch (true, ConditionCloseEvent);
        }


        /// <summary>
        /// Profiles the open.
        /// </summary>
        /// <param name="buttonObj">Button object.</param>
        public void ProfileOpen (GameObject buttonObj)
        {
            if (AppStartLoadBalanceManager._isBaseProfile == false){
                PopupSecondSelectPanel.Instance._toUserId = buttonObj.name;
                PanelFooterButtonManager.Instance.NoRegistBaseProfile (true);
                return;
            }

			HeaderPanel.Instance.UiButtonSwitch (false);

            HeaderPanel.Instance.BackButtonSwitch (true, ProfileBackButton);
            _backSwipe.EventMessageTarget = _panelProfile;
			_panelProfile.GetComponent<ProfilePanel> ().Init (buttonObj.name);
			PanelProfileAnimate (_panelProfile);
        }


        /// <summary>
        /// Profiles the open from GP.
        /// </summary>
        /// <param name="userID">User I.</param>
		public void ProfileOpenFromGPS (string userID)
		{
			HeaderPanel.Instance.BackButtonSwitch (false);
			HeaderPanel.Instance.BackButtonSwitch (true, ProfileBackButtonGps);

			_backSwipe.EventMessageTarget = _panelProfile;
			_panelProfile.GetComponent<ProfilePanel> ().Init (userID);
			PanelProfileAnimate (_panelProfile);
		}

        /// <summary>
        /// Profiles the back button.
        /// Eventセット用
        /// </summary>
        public void ProfileBackButtonGps ()
        {
            HeaderPanel.Instance.BackButtonSwitch (false, null, LocalMsgConst.TITLE_SEARCH);
            ProfileClose (_panelProfile.gameObject);

            SetGpsObjectActive(true);
            _panelGpsSearch.transform.GetChild(0).GetChild(0).GetComponent<MapWebView>().Init ();
        }

		public void ProfileClose (GameObject animObj)
        {
			switch (_statePanel)
			{
				case StatePanel.Image:
					HeaderPanel.Instance.UiButtonSwitch (true, PictLayoutChangeEvent);
				break;
			}

            if (_statePanel == StatePanel.Gps) {
                _panelGpsSearch.transform.GetChild(0).GetChild(0).GetComponent<MapWebView>().Init ();
            }
			
            HeaderPanel.Instance.BackButtonSwitch (false, null, LocalMsgConst.TITLE_SEARCH);
            PanelProfileAnimate (animObj, true);
        }

		public void ProfileToChatOpen (GameObject animObj)
		{
			_panelChat.GetComponent<PanelChat> ()._maxAfterID = "";
			_panelChat.GetComponent<PanelChat> ()._maxBeforeID = "";
			_panelChat.GetComponent<PanelChat> ()._maxAfterIDBackup = "";
			_panelChat.GetComponent<PanelChat> ()._maxBeforeIDBackup = "";
			_panelChat.GetComponent<PanelChat> ()._listUpdateDisable = false; //解除処理

            PanelChat.Instance.Init (ProfilePanel.Instance._toUserId);
			//PanelChat.Instance._toUser = ProfilePanel.Instance._toUserId;
			PanelPopupCloseAnimate (_panelProfile);
			HeaderPanel.Instance.BackButtonSwitch (false, ProfileBackButton);
			//HeaderPanel.Instance.BackButtonSwitch (false, HistoryToProfileCloseEvent);
			HeaderPanel.Instance.BackButtonSwitch (true, ProfileToChatCloseEvent);
			_backSwipe.EventMessageTarget = _panelChat;
			PanelAnimate(animObj);

		}

		public void ProfileToChatClose (GameObject animObj)
		{
			PanelChat.Instance.ResetScrollItem ();
			PanelPopupAnimate (_panelProfile);
			HeaderPanel.Instance.BackButtonSwitch (false, ProfileToChatCloseEvent);
			HeaderPanel.Instance.BackButtonSwitch (true, ProfileBackButton); 

			_backSwipe.EventMessageTarget = _panelProfile;
			BackButton (animObj);
		}

		void ProfileToChatCloseEvent ()
		{
			ProfileToChatClose (_panelChat);

			_panelChat.GetComponent<PanelChat> ()._maxAfterID = "";
			_panelChat.GetComponent<PanelChat> ()._maxBeforeID = "";
			_panelChat.GetComponent<PanelChat> ()._maxAfterIDBackup = "";
			_panelChat.GetComponent<PanelChat> ()._maxBeforeIDBackup = "";
			_panelChat.GetComponent<PanelChat> ()._listUpdateDisable = true; //解除処理
		}


		// チャット画面での通報ボタン
		public void ReportConfirmOpen(GameObject target)
		{
			string question = string.Format(LocalMsgConst.REPORT_QUESTION, GetUserApi._httpOtherUserCatchData.name);
			PopupSecondSelectPanel.Instance.PopMessageInsert(
				question,
				LocalMsgConst.YES,
				LocalMsgConst.NO,
				ReportApiCall,
				ReportCancel
			);
			PanelPopupAnimate (target);
		}
		void ReportApiCall()
		{
            PopupSecondSelectPanel.Instance.PopClean();
			new SendReportApi (GetUserApi._httpOtherUserCatchData.id);
			StartCoroutine (SendReportWait ());
			PanelPopupCloseAnimate (GameObject.FindGameObjectWithTag(CommonConstants.POPUP_SECOND_SELECT_TAG));
			
		}
		private IEnumerator SendReportWait ()
		{
			_loadingOverLay.SetActive (true);
			while (SendReportApi._success == false)
				yield return (SendReportApi._success == true);
			_loadingOverLay.SetActive (false);

			PopupPanel.Instance.PopMessageInsert (
				SendReportApi._httpCatchData.result.complete[0],
				LocalMsgConst.OK,
				ReportFinishClose
			);
			PanelPopupAnimate (GameObject.FindGameObjectWithTag(CommonConstants.POPUP_BASIC_TAG));

		}
      
		void ReportCancel () 
		{
			PopupSecondSelectPanel.Instance.PopClean();
			PanelPopupCloseAnimate (GameObject.FindGameObjectWithTag(CommonConstants.POPUP_SECOND_SELECT_TAG));
		}
      
		void ReportFinishClose () 
		{
			PopupPanel.Instance.PopClean();
			PanelPopupCloseAnimate (GameObject.FindGameObjectWithTag(CommonConstants.POPUP_BASIC_TAG));
		}
        
		public void SearchProfileClose (GameObject animObj) {
			HeaderPanel.Instance.BackButtonSwitch (false);         
			_backSwipe.EventMessageTarget = _panelProfile;         
			PanelPopupCloseAnimate (animObj);
		}

        /// <summary>
        /// Panels the popup animate.
        /// </summary>
        /// <param name="target">Target.</param>
		private void PanelPopupAnimate ( GameObject target )
		{
            _popupOverlay.SetActive (true);
			target.GetComponent<uTweenScale> ().from = Vector3.zero;
			target.GetComponent<uTweenScale> ().to = new Vector3 (1, 1 ,1 );
			target.GetComponent<uTweenScale> ().delay    = 0.01f;
			target.GetComponent<uTweenScale> ().duration = 0.25f;
			target.GetComponent<uTweenScale> ().ResetToBeginning ();
			target.GetComponent<uTweenScale> ().enabled = true;
		}
        /// <summary>
        /// Panels the popup close animate.
        /// </summary>
        /// <param name="target">Target.</param>
		private void PanelPopupCloseAnimate ( GameObject target )
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
        /// Profiles the back button.
        /// Eventセット用
        /// </summary>
        public void ProfileBackButton ()
        {
            HeaderPanel.Instance.BackButtonSwitch (false, null, LocalMsgConst.TITLE_SEARCH);

            ProfileClose (_panelProfile.gameObject);
#if UNITY_IPHONE || UNITY_ANDROID && !UNITY_EDITOR
            if (GetUserApi._httpCatchData.result.review == "false" && CommonConstants.IS_PREMIUM == false) {
                IMobileSdkAdsUnityPlugin.show (CommonConstants.IMOBILE_FULL_SPOT_ID);
            }
#endif

        }



		public void FavoriteConfirmOpen(GameObject target)
		{
			string tmpText = string.Format (LocalMsgConst.FAVORITE_ON_CONFIRM, GetUserApi._httpOtherUserCatchData.name);
			PopupSecondSelectPanel.Instance.PopMessageInsert(
				tmpText,
				LocalMsgConst.YES,
				LocalMsgConst.NO,
				FavoriteApiCall,
				FavoriteCancel
			);
			PanelPopupAnimate (target);
		}

		void FavoriteApiCall () 
		{
			new SetUserFavoriteApi ();
			PanelPopupCloseAnimate (GameObject.FindGameObjectWithTag(CommonConstants.POPUP_SECOND_SELECT_TAG));
			StartCoroutine (SetUserFavoriteApiWait ());

			PopupSecondSelectPanel.Instance.PopClean();
		}

		private IEnumerator SetUserFavoriteApiWait ()
		{
			_loadingOverLay.SetActive (true);
			while (SetUserFavoriteApi._success == false)
				yield return (SetUserFavoriteApi._success == true);
			_loadingOverLay.SetActive (false);

			PopupPanel.Instance.PopMessageInsert(
				SetUserFavoriteApi._httpCatchData.result.complete[0],
				LocalMsgConst.OK,
				FavoriteFinishClose
			);
			PanelPopupAnimate (GameObject.FindGameObjectWithTag(CommonConstants.POPUP_BASIC_TAG));
		}

		void FavoriteCancel () 
		{
			PopupSecondSelectPanel.Instance.PopClean();
			PanelPopupCloseAnimate (GameObject.FindGameObjectWithTag(CommonConstants.POPUP_SECOND_SELECT_TAG));
		}

		void FavoriteFinishClose () 
		{
			PopupPanel.Instance.PopClean();
			PanelPopupCloseAnimate (GameObject.FindGameObjectWithTag(CommonConstants.POPUP_BASIC_TAG));
		}


        /// <summary>
        /// Backs the button.
        /// </summary>
        /// <param name="fromObj">From object.</param>
        /// <param name="toObj">To object.</param>
        public void BackButton (GameObject fromObj)
        {
            //todo HeaderPanel.Instance.BackButtonSwitch (false, ConditionCloseEvent);

            if (fromObj.GetComponent<uTweenPosition> ().to.x == 0)
            {
                fromObj.GetComponent<uTweenPosition> ().to = fromObj.transform.GetComponent<uTweenPosition> ().from;
                fromObj.GetComponent<uTweenPosition> ().delay      = 0.1f;
                fromObj.GetComponent<uTweenPosition> ().duration   = 0.2f;
                fromObj.GetComponent<uTweenPosition> ().from    = Vector3.zero;
                fromObj.GetComponent<uTweenPosition> ().ResetToBeginning ();
                fromObj.GetComponent<uTweenPosition> ().enabled = true;
            }

            if (fromObj.name == _panelSearchCondition.name){
                switch (_statePanel){
                case StatePanel.Image:
                    PanelAnimate (_panelImageList);
                    break;
                case StatePanel.Notify:
                    PanelAnimate (_panelEazyNotify);
                    break;
                case StatePanel.Gps:
                    PanelAnimate (_panelGpsSearch);
                    break;
                }
            }
        }
        #endregion

        #region private method
        /// <summary>
        /// Nons the used gameobject resetter.
        /// </summary>
        private void NonUsedGameobjectResetter( StatePanel statePanel ) 
        {        
            switch (statePanel){
            case StatePanel.Image:
                if  (PanelSeachSmallImageList.Instance != null) {
                    PanelSeachSmallImageList.Instance.GameobjectReset ();
                }
                
                if  (PanelSeachLargeImageList.Instance != null) {
                    PanelSeachLargeImageList.Instance.GameobjectReset ();
                }
                break;
            case StatePanel.Notify:
                PanelSeachList.Instance.SearchListGameObjectReset ();
                break;
            case StatePanel.Gps:
            case StatePanel.Condition:
            
                break;
            }
        }        

        
        /// <summary>
        /// Res the panel animate.
        /// </summary>
        /// <param name="target">Target.</param>
        private void PanelAnimate ( GameObject target )
        {
            if (target.GetComponent<uTweenPosition> ().from.x == 0) {
                target.GetComponent<uTweenPosition> ().from = target.GetComponent<uTweenPosition> ().to;
            }

            target.GetComponent<uTweenPosition> ().delay    = 0.1f;
            target.GetComponent<uTweenPosition> ().duration = 0.2f;
            target.GetComponent<uTweenPosition> ().to = Vector3.zero;
            target.GetComponent<uTweenPosition> ().ResetToBeginning ();
            target.GetComponent<uTweenPosition> ().enabled = true;
        }

        /// <summary>
        /// Tabs the on off switcher.
        /// </summary>
        private void TabOnOffSwitcher (int tabNumber)
		{
            int cNo = tabNumber - 1;
            System.GC.Collect();
            Resources.UnloadUnusedAssets ();

            NonUsedGameobjectResetter (_statePanel);

            if (_panelSubHeader != null && cNo >= 0)
			{
                foreach (Transform tab in _panelSubHeader) 
				{
                    if (_panelSubHeader.GetChild (cNo).name != tab.name)
					{
                        tab.GetChild(0).gameObject.SetActive (false);
                    } else {
                        tab.GetChild(0).gameObject.SetActive (true);
                    }
                }
                _panelSubHeader.GetChild (cNo).GetChild (0).gameObject.SetActive (true);
            }
        }

        /// <summary>
        /// Res the panel animate.
        /// </summary>
        /// <param name="target">Target.</param>
        private void PanelProfileAnimate ( GameObject target, bool isTo = false ) {
            target.GetComponent<uTweenScale> ().delay    = 0.001f;
            target.GetComponent<uTweenScale> ().duration = 0.25f;
            if (isTo == true) {
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
        
        #region TODO:とりあえず課金シーンに飛ばす、後で書き換え
        /// <summary>
        /// Funcs the limit over.
        /// </summary>
        public void FuncLimitOver() {
            //TODO: とりあえず課金シーンに飛ばす、後で書き換え
            SceneHandleManager.NextSceneRedirect (CommonConstants.PURCHASE_SCENE);            
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
    }
}