using UnityEngine;
using System.Collections;
using EventManager;
using UnityEngine.UI;
using Http;

namespace ViewController
{
    public class OtherSetting : SingletonMonoBehaviour<OtherSetting>
    {
        [SerializeField]
        private Transform _setOnGps;
        
        [SerializeField]
        private Transform _setOnPublicOpen;

        [SerializeField]
        private Transform _setPublicNotification;
        
        [SerializeField]
        private GameObject _loadingOverlay;

        [SerializeField]
        private GameObject _webviewTerm;

        [SerializeField]
        private GameObject _webviewPrivachiy;
        
        [SerializeField]
        private GameObject _webviewTrans;

        [SerializeField]
        private GameObject _panelPassingSettingButton;

        #region Finger Event
        /// <summary>
        /// Backs the swipe.
        /// </summary>
        void OnSwipe (SwipeGesture gesture)
        {
            if (gesture.Selection) {
                if (gesture.Direction == FingerGestures.SwipeDirection.Left) {
                    //Debug.Log ("Left Left Left Left Left Left ");
                } else if (gesture.Direction == FingerGestures.SwipeDirection.Right) {
                    Debug.Log ("Right Right Right Right Right Right Right ");

                    if (MypageEventManager.Instance != null)
                        MypageEventManager.Instance.OtherSettingClose (this.gameObject);
                }
            }
        }
        #endregion

        /// <summary>
        /// Init this instance.
        /// 初期表示用処理。
        /// </summary>
        public void Init () 
        {
            var user = GetUserApi._httpCatchData.result.user;
            //GPSのオンオフ
            if (user.is_public_gps == "1") {
                _setOnGps.GetChild (0).gameObject.SetActive (true);
            } else if (user.is_public_gps == "0") {
               _setOnGps.GetChild (0).gameObject.SetActive (false);
            }

            //プロフィールのオンオフ
            if (user.is_public_profile == "1") {
                _setOnPublicOpen.GetChild (0).gameObject.SetActive (true);
            } else if (user.is_public_profile == "0") {
                _setOnPublicOpen.GetChild (0).gameObject.SetActive (false);
            }

            //プッシュのオンオフ
            if (user.is_notification == "1") {
                _setPublicNotification.GetChild (0).gameObject.SetActive (true);
            } else if (user.is_notification == "0") {
               _setPublicNotification.GetChild (0).gameObject.SetActive (false);
            }

            //レビュー時、すれ違い機能は表示しないようにしておく。リスクヘッジ。
            if (GetUserApi._httpCatchData.result.review == "true") {
                _panelPassingSettingButton.SetActive (false);
            } else if (GetUserApi._httpCatchData.result.review == "false") {
                _panelPassingSettingButton.SetActive (true);
            }
        }
        
        /// <summary>
        /// Gps the set.
        /// </summary>
        /// <returns>The set.</returns>
        public void GpsSetButton () {
           var user = GetUserApi._httpCatchData.result.user;
            if (user.is_public_gps == "1") {
                StartCoroutine (SetPublicGps ("0"));
            } else {
                StartCoroutine (SetPublicGps ("1"));
            }
        }
        
        /// <summary>
        /// Sets the public profile.
        /// プロフィールを公開、非公開にする。
        /// </summary>
        /// <returns>The public profile.</returns>
        public void SetPublicProfileButton () {
           var user = GetUserApi._httpCatchData.result.user;
            if (user.is_public_profile == "1") {
                StartCoroutine (SetPublicProfile ("0"));
            } else {
                StartCoroutine (SetPublicProfile ("1"));
            }
        }
        
        /// <summary>
        /// Sets the public profile button.
        /// </summary>
        /// <returns>The public profile button.</returns>
        public void SetPublicNotificationButton () {
           var user = GetUserApi._httpCatchData.result.user;
            if (user.is_notification == "1") {
                StartCoroutine (SetPublicNotification ("0"));
            } else {
                StartCoroutine (SetPublicNotification ("1"));
            }
        }
        

        /// <summary>
        /// Gpses the set.
        /// </summary>
        /// <returns>The set.</returns>
        /// <param name="stateData">State data.</param>
        private IEnumerator SetPublicGps (string stateData) {
           _loadingOverlay.SetActive (true);
           new SetPublicGpsApi (stateData);
            while (SetPublicGpsApi._success == false)
                yield return (SetPublicGpsApi._success == true);

            new GetUserApi ();
            while (GetUserApi._success == false)
                yield return (GetUserApi._success == true);
            _loadingOverlay.SetActive (false);
            
            var user = GetUserApi._httpCatchData.result.user;
            
            if (user.is_public_gps == "1") 
            {
                _setOnGps.GetChild (0).gameObject.SetActive (true);
            } else if (user.is_public_gps == "0") {
                _setOnGps.GetChild (0).gameObject.SetActive (false);
            }
        }
        
        /// <summary>
        /// Sets the public profile.
        /// </summary>
        /// <returns>The public profile.</returns>
        /// <param name="stateData">Stete data.</param>
        private IEnumerator SetPublicProfile (string stateData) 
        {
            new SetPublicProfileApi (stateData);
            _loadingOverlay.SetActive (true);
            while (SetPublicProfileApi._success == false)
                yield return (SetPublicProfileApi._success == true);

            new GetUserApi ();
            while (GetUserApi._success == false)
                yield return (GetUserApi._success == true);
            _loadingOverlay.SetActive (false);
            
            var user = GetUserApi._httpCatchData.result.user;
            if (user.is_public_profile == "1") 
            {
                _setOnPublicOpen.GetChild (0).gameObject.SetActive (true);
            } else {
                _setOnPublicOpen.GetChild (0).gameObject.SetActive (false);
            }
        }
        
        
        /// <summary>
        /// Sets the public notification.
        /// </summary>
        /// <returns>The public notification.</returns>
        /// <param name="stateData">State data.</param>
        private IEnumerator SetPublicNotification (string stateData)
        {
           _loadingOverlay.SetActive (true);
           new SetNotificationConfigApi (stateData);
            while (SetNotificationConfigApi._success == false)
                yield return (SetNotificationConfigApi._success == true);

            new GetUserApi ();
            while (GetUserApi._success == false)
                yield return (GetUserApi._success == true);
            _loadingOverlay.SetActive (false);
            
            var user = GetUserApi._httpCatchData.result.user;

            if (user.is_notification == "1") 
            {
                _setPublicNotification.GetChild (0).gameObject.SetActive (true);
            } else if (user.is_notification == "0") {
                _setPublicNotification.GetChild (0).gameObject.SetActive (false);
            }
        }

        #region Mail設定画面。Open, Close
        /// <summary>
        /// Mails the setting open.
        /// </summary>
        /// <param name="animObj">Animation object.</param>
        public void MailSettingOpen ()
        {
            PanelMailSetting.Instance.Init ();
            HeaderPanel.Instance.BackButtonSwitch (false);
            HeaderPanel.Instance.BackButtonSwitch (true, MailSettingEvent);
            OtherSetting.Instance.gameObject.GetComponent<BoxCollider2D> ().enabled = false;
            GameObject.FindGameObjectWithTag (CommonConstants.BACK_SWIPE).GetComponent<SwipeRecognizer>().EventMessageTarget = PanelMailSetting.Instance.gameObject;
            MypageEventManager.Instance.PanelAnimate (PanelMailSetting.Instance.gameObject);
        }

        /// <summary>
        /// Mails the setting close.
        /// </summary>
        /// <param name="animObj">Animation object.</param>
        public void MailSettingClose (GameObject animObj)
        {
            HeaderPanel.Instance.BackButtonSwitch (false);
            HeaderPanel.Instance.BackButtonSwitch (true, MypageEventManager.Instance.OtherSettingCloseEvent);
            OtherSetting.Instance.gameObject.GetComponent<BoxCollider2D> ().enabled = true;
            MypageEventManager.Instance.BackButton (animObj);
        }

        /// <summary>
        /// Mails the setting event.
        /// </summary>
        void MailSettingEvent ()
        {
            MailSettingClose (PanelMailSetting.Instance.gameObject);
        }
        #endregion



        #region お問い合わせ (Open, Close)
        /// <summary>
        /// Contacts the open.
        /// </summary>
        /// <returns>The open.</returns>
        /// <param name="animObj">Animation object.</param>
        public void ContactOpen ( GameObject animObj ) 
        {
            string title = PanelStateManager.GetHeaderStringByKey ("PanelContact");
            HeaderPanel.Instance.BackButtonSwitch (false);
            HeaderPanel.Instance.BackButtonSwitch (true, ContactCloseEvent, title);
            this.GetComponent<BoxCollider2D> ().enabled = false;
            MypageEventManager.Instance.PanelAnimate (animObj);            
        }
        
        /// <summary>
        /// Contacts the close.
        /// </summary>
        /// <returns>The close.</returns>
        /// <param name="animObj">Animation object.</param>
        public void ContactClose ( GameObject animObj )
        {
            string title = PanelStateManager.GetHeaderStringByKey ("OtherSetting");

            HeaderPanel.Instance.BackButtonSwitch (false);
            HeaderPanel.Instance.BackButtonSwitch (true, BackButtonEvent, title);
            this.GetComponent<BoxCollider2D> ().enabled = true;
            MypageEventManager.Instance.BackButton (animObj);
        }
        
        /// <summary>
        /// Webviews the term close event.
        /// </summary>
        /// <returns>The term close event.</returns>
        public void ContactCloseEvent () 
        {
            ContactClose (PanelContact.Instance.gameObject);
        }
        #endregion


        #region 利用規約 利用規約 利用規約
        /// <summary>
        /// Webviews the term open.
        /// </summary>
        /// <returns>The term open.</returns>
        public void WebviewTermOpen ( GameObject animObj )
        {
            string title = PanelStateManager.GetHeaderStringByKey ("PanelTerms");
            HeaderPanel.Instance.BackButtonSwitch (false);
            HeaderPanel.Instance.BackButtonSwitch (true, WebviewTermCloseEvent, title);

            this.GetComponent<BoxCollider2D> ().enabled = false;
            _webviewTerm.SetActive (true);
            MypageEventManager.Instance.PanelAnimate (animObj);
        }

        /// <summary>
        /// Webviews the term close.
        /// </summary>
        /// <returns>The term close.</returns>
        public void WebviewTermClose ( GameObject animObj )
        {
            string title = PanelStateManager.GetHeaderStringByKey ("OtherSetting");
            HeaderPanel.Instance.BackButtonSwitch (false);
            HeaderPanel.Instance.BackButtonSwitch (true, BackButtonEvent, title);
            _webviewTerm.SetActive (false);
            this.GetComponent<BoxCollider2D> ().enabled = true;
            MypageEventManager.Instance.BackButton (animObj);
        }
        
        /// <summary>
        /// Webviews the term close event.
        /// </summary>
        /// <returns>The term close event.</returns>
        void WebviewTermCloseEvent () 
        {
            WebviewTermClose (_webviewTerm);
        }
        #endregion
        
        #region プライバシーポリシーWEBVIEW
        /// <summary>
        /// Webviews the term open.
        /// </summary>
        /// <returns>The term open.</returns>
        public void WebviewPrivachyOpen ( GameObject animObj )
        {
            string title = PanelStateManager.GetHeaderStringByKey ("PanelPrivachy");

            HeaderPanel.Instance.BackButtonSwitch (false);
            HeaderPanel.Instance.BackButtonSwitch (true, WebviewPrivachyCloseEvent, title);

            this.GetComponent<BoxCollider2D> ().enabled = false;
            _webviewPrivachiy.SetActive (true);
            MypageEventManager.Instance.PanelAnimate (animObj);
        }

        /// <summary>
        /// Webviews the term close.
        /// </summary>
        /// <returns>The term close.</returns>
        public void WebviewPrivachyClose ( GameObject animObj )
        {
            string title = PanelStateManager.GetHeaderStringByKey ("OtherSetting");

            HeaderPanel.Instance.BackButtonSwitch (false);
            HeaderPanel.Instance.BackButtonSwitch (true, BackButtonEvent, title);
            _webviewPrivachiy.SetActive (false);
            this.GetComponent<BoxCollider2D> ().enabled = true;
            MypageEventManager.Instance.BackButton (animObj);
        }
        
        /// <summary>
        /// Webviews the term close event.
        /// </summary>
        /// <returns>The term close event.</returns>
        public void WebviewPrivachyCloseEvent () 
        {
            WebviewPrivachyClose (_webviewPrivachiy);
        }
        #endregion
        
        
        #region 特定商取引WEBVIEW
        /// <summary>
        /// Webviews the term open.
        /// </summary>
        /// <returns>The term open.</returns>
        public void WebviewTransOpen ( GameObject animObj )
        {
            string title = PanelStateManager.GetHeaderStringByKey ("PanelTrans");
            HeaderPanel.Instance.BackButtonSwitch (false);
            HeaderPanel.Instance.BackButtonSwitch (true, WebviewTransCloseEvent, title);

            this.GetComponent<BoxCollider2D> ().enabled = false;
            _webviewTrans.SetActive (true);
            MypageEventManager.Instance.PanelAnimate (animObj);
        }

        /// <summary>
        /// Webviews the term close.
        /// </summary>
        /// <returns>The term close.</returns>
        public void WebviewTransClose ( GameObject animObj )
        {
            string title = PanelStateManager.GetHeaderStringByKey ("OtherSetting");

            HeaderPanel.Instance.BackButtonSwitch (false);
            HeaderPanel.Instance.BackButtonSwitch (true, BackButtonEvent, title);
            
            _webviewTrans.SetActive (false);
            this.GetComponent<BoxCollider2D> ().enabled = true;
            MypageEventManager.Instance.BackButton (animObj);
        }
        
        /// <summary>
        /// Webviews the term close event.
        /// </summary>
        /// <returns>The term close event.</returns>
        void WebviewTransCloseEvent () 
        {
            WebviewTransClose (_webviewTrans);
        }
        #endregion


        #region ヘルプ画面制御
        /// <summary>
        /// Helps the open.
        /// </summary>
        /// <returns>The open.</returns>
        /// <param name="animObj">Animation object.</param>
        public void HelpOpen ( GameObject animObj )
        {
            string title = PanelStateManager.GetHeaderStringByKey ("HelpPanel");
            HeaderPanel.Instance.BackButtonSwitch (false);
            HeaderPanel.Instance.BackButtonSwitch (true, HelpEvent, title);
            HelpPanel.Instance.gameObject.SetActive (true);
            HelpPanel.Instance.Initialize ();
            MypageEventManager.Instance.PanelAnimate (animObj);

            GameObject.FindGameObjectWithTag (CommonConstants.BACK_SWIPE).GetComponent<ScreenRaycaster>().enabled = false;
        }

        /// <summary>
        /// Helps the close.
        /// </summary>
        /// <returns>The close.</returns>
        /// <param name="animObj">Animation object.</param>
        public void HelpClose ( GameObject animObj )
        {
            string title = PanelStateManager.GetHeaderStringByKey ("OtherSetting");
            HeaderPanel.Instance.BackButtonSwitch (false);
            HeaderPanel.Instance.BackButtonSwitch (true, MypageEventManager.Instance.OtherSettingCloseEvent, title);

            HelpPanel.Instance.gameObject.SetActive (false);
            MypageEventManager.Instance.BackButton (animObj);

            GameObject.FindGameObjectWithTag (CommonConstants.BACK_SWIPE).GetComponent<ScreenRaycaster>().enabled = true;
        }
        
        /// <summary>
        /// Helps the detail open.
        /// </summary>
        /// <returns>The detail open.</returns>
        /// <param name="panelObject">Panel object.</param>
        void HelpEvent ()
        {
            HelpClose (HelpPanel.Instance.gameObject);
        }

        /// <summary>
        /// Helps the detail open.
        /// </summary>
        /// <returns>The detail open.</returns>
        /// <param name="panelObject">Panel object.</param>
        public void HelpDetailOpen ( GameObject panelObject )
        {
            HeaderPanel.Instance.BackButtonSwitch (false);
            string title = PanelStateManager.GetHeaderStringByKey ("HelpPanelDetail");
            HeaderPanel.Instance.BackButtonSwitch (true, HelpDetailEvent, title);
            PanelHelpDetail.Instance.gameObject.SetActive (true);
            PanelHelpDetail.Instance.Init(panelObject.transform.GetChild(1).GetComponent<Text>().text, panelObject.transform.GetChild(2).GetComponent<Text>().text);
            MypageEventManager.Instance.PanelAnimate (PanelHelpDetail.Instance.gameObject);
        }

        /// <summary>
        /// Helps the detail close.
        /// </summary>
        /// <returns>The detail close.</returns>
        /// <param name="animObj">Animation object.</param>
        public void HelpDetailClose ( GameObject animObj )
        {
            string title = PanelStateManager.GetHeaderStringByKey ("HelpPanel");
            HeaderPanel.Instance.BackButtonSwitch (false);
            HeaderPanel.Instance.BackButtonSwitch (true, HelpEvent, title);

            HelpPanel.Instance.gameObject.SetActive (true);
            MypageEventManager.Instance.BackButton (animObj);
        }

        /// <summary>
        /// Helps the detail event.
        /// </summary>
        /// <returns>The detail event.</returns>
        void HelpDetailEvent ()
        {
            HelpDetailClose (PanelHelpDetail.Instance.gameObject);
        }

        #endregion

        #region 退会処理
        public void ResignButton() {
            PopupSecondSelectPanel.Instance.PopClean ();
            string tmpMsg = string.Format (LocalMsgConst.RESIGN_CHECK_TEXT, GetUserApi._httpCatchData.result.user.name);
            
            PopupSecondSelectPanel.Instance.PopMessageInsert (
                tmpMsg,
                LocalMsgConst.YES,
                LocalMsgConst.NO,
				LastResignCheck,
                ResignPopupCancel
            );
            PopupSecondSelectPanel.Instance.PanelPopupAnimate (PopupSecondSelectPanel.Instance.gameObject);
        }
        
		/// <summary>
		/// Lasts the resign check.
		/// 退会処理を２重でチェック。
		/// </summary>
		void LastResignCheck() {
			PopupSecondSelectPanel.Instance.PanelPopupCloseAnimate (PopupSecondSelectPanel.Instance.gameObject);
			PopupSecondSelectPanel.Instance.PopClean ();

			PopupSecondSelectPanel.Instance.PopMessageInsert (
				LocalMsgConst.REALLY_RESIGN_RELASE_TEXT,
				LocalMsgConst.YES,
				LocalMsgConst.NO,
				ResignProcess,
				ResignPopupCancel
			);
			PopupSecondSelectPanel.Instance.PanelPopupAnimate (PopupSecondSelectPanel.Instance.gameObject);			
		}

        /// <summary>
        /// Resigns the process.
        /// </summary>
        /// <returns>The process.</returns>
        void ResignProcess () {
            StartCoroutine (ResignApiEnumurator());
            
        }
        
        /// <summary>
        /// Resigns the popup cancel.
        /// </summary>
        /// <returns>The popup cancel.</returns>
        void ResignPopupCancel () {
            PopupSecondSelectPanel.Instance.PopClean ();
            PopupSecondSelectPanel.Instance.PanelPopupCloseAnimate (PopupSecondSelectPanel.Instance.gameObject);
        }

        /// <summary>
        /// Resigns the API enumurator.
        /// </summary>
        /// <returns>The API enumurator.</returns>
        private IEnumerator ResignApiEnumurator ()
        {
            _loadingOverlay.SetActive (true);
            new ResignUserApi ();
            while (ResignUserApi._success == false)
                yield return (ResignUserApi._success == true);                
            _loadingOverlay.SetActive (false);

            ResignPopupCancel ();
            SceneHandleManager.NextSceneRedirect (CommonConstants.PROBLEM_SCENE);
            yield break;
        }

        #endregion

        void BackButtonEvent ()
        {
            MypageEventManager.Instance.OtherSettingClose (this.gameObject);
        }
    }
}