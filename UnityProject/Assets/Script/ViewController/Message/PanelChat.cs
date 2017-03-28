using UnityEngine;
using System.Collections;
using EventManager;
using UnityEngine.UI;
using Http;
using UnityEngine.SceneManagement;
using uTools;


namespace ViewController
{
    /// <summary>
    /// Pane] chat.
    /// </summary>
    public class PanelChat : SingletonMonoBehaviour<PanelChat>
    {
        public InputField _messageSend;
        public RawImage _sendPicture;
        public Transform _sendPictureLargeOverlay;
        public RawImage _sendPictureLarge;
        public GameObject _loadingOverlay;
        
		public float _waitTimeForListReset = 0; // タップとリストの更新を区別するため
		public bool _nowTap = false;
		public bool _connectWait = false;	// 重複してとばさないようにフラグを持っておく（必要ないかもしれないが一応）

		public bool _listUpdateDisable = true;	// リストの更新Onoff


		public bool _sendImage = false;

        [SerializeField]
        private MessageInfiniteLimitScroll _messageInfiniteLimitScroll;

        [SerializeField]
        private Transform _scrollContent;
		[SerializeField]
		private GameObject _scrollBar;

        [SerializeField]
        private GameObject _profileButton;

        [SerializeField]
        private GameObject _favoriteButton;

        [SerializeField]
        private SwipeRecognizer _backSwipe;

        [SerializeField]
        private GameObject _headerTitle;

        [SerializeField]
        private GameObject _panelChat;
        
        [SerializeField]
        private GameObject _panelProfile;

        [SerializeField]
        private GameObject _popupOverlay;

		[SerializeField]
		private GameObject _limitReleaseButton;
        

		public bool _messageSceneProfilePanelShow = false;

        
		public string _toUser = ""; 			// 誰と話しているのか？
		public string _maxBeforeID = ""; 		// 現状ページの一番前
		public string _maxAfterID = "";			// 現状リストの一番後
		public string _maxBeforeIDBackup = ""; 	// 現状ページの一番前
		public string _maxAfterIDBackup = "";	// 現状リストの一番後

        
        #region Panel表示の初期化処理
		void Update()
		{
			// 更新しない場合チェック
			if(_scrollBar == null)
			{
				return;
			}
			if(_listUpdateDisable)
			{
				return;
			}
		}

		/// <summary>
        /// Init the specified id.
        /// </summary>
        /// <param name="toUserId">Identifier.</param>
        public void Init (string toUserId) 
        {
            if (PanelFooterButtonManager.Instance != null) 
                PanelFooterButtonManager.Instance.gameObject.SetActive (false);
			_maxAfterID = "";
			_maxBeforeID = "";
			_maxAfterIDBackup = "";
			_maxBeforeIDBackup = "";
			_toUser = toUserId;
            _sendPicture.texture = null;
			StartCoroutine (InitEnumerator(_toUser));
        }
        
        /// <summary>
        /// IEs the numerator.
        /// </summary>
        /// <returns>The numerator.</returns>
        private IEnumerator InitEnumerator(string toUserId ) 
        {
            if (MessageEventManager.Instance != null) 
            {
                if (MessageEventManager.Instance._msgReads.Contains (toUserId) == false)
                {
                    MessageEventManager.Instance._msgReads.Add (toUserId);
                }
            }

            _loadingOverlay.SetActive (true);
			new MessageListApi (toUserId, _maxBeforeID, _maxAfterID);
			while (MessageListApi._success == false) 
                yield return (MessageListApi._success == true);

            _loadingOverlay.SetActive (false);
            
			GetUserApi._httpOtherUserCatchData = MessageListApi._httpCatchData.result.to_user;
            _headerTitle.GetComponent<Text> ().text = GetUserApi._httpOtherUserCatchData.name;

   			//if (SceneManager.GetActiveScene().name == CommonConstants.MESSAGE_SCENE)
			//{
            	_profileButton.SetActive (true);
                _favoriteButton.SetActive (true);
                
                string isFavorite = GetUserApi._httpOtherUserCatchData.is_favorite;

                if (isFavorite == "true")
				{
                	_favoriteButton.transform.GetChild (0).gameObject.SetActive (true);
                } else if (isFavorite == "false") {
                    _favoriteButton.transform.GetChild (0).gameObject.SetActive (false);
                }


			string islimitRelease = MessageListApi._httpCatchData.result.to_user.is_limit_release;
			if (islimitRelease == "true")
			{
				_limitReleaseButton.GetComponent<Button> ().enabled = false;
				_limitReleaseButton.GetComponent<Image> ().color = new Color (147/255.0f,147/255.0f,147/255.0f,255);
			} else if (islimitRelease == "false") {
				_limitReleaseButton.GetComponent<Image> ().color = new Color (247/255.0f,117/255.0f,133/255.0f,255);
				_limitReleaseButton.GetComponent<Button> ().enabled = true;
			}


			if (MessageListApi._httpCatchData.result.messages != null)
			{
				if (MessageListApi._httpCatchData.result.messages.Count > 0)
				{
					_messageInfiniteLimitScroll.Init ();               
					// 正常に通信が終わったのなら更新用に前と後ろを覚える
					_maxAfterID = MessageListApi._httpCatchData.result.messages[MessageListApi._httpCatchData.result.messages.Count-1].id;
					_maxBeforeID = MessageListApi._httpCatchData.result.messages[0].id;
					if(MessageListApi._httpCatchData.result.messages.Count >= MessageInfiniteLimitScroll.PANEL_TOTAL)
					{
						_maxBeforeID = MessageListApi._httpCatchData.result.messages[MessageListApi._httpCatchData.result.messages.Count - MessageInfiniteLimitScroll.PANEL_TOTAL].id;
					}

					//Debug.Log ("after = " + _maxAfterID);
					//Debug.Log ("before = " + _maxBeforeID);
					//Debug.Log ("beforemessage = " + MessageListApi._httpCatchData.result.messages[0].message);
					//Debug.Log ("aftermessage = " + MessageListApi._httpCatchData.result.messages[MessageListApi._httpCatchData.result.messages.Count-1].message);

				} else {
					// リストのないページカウントになってるので　一旦通信とリストの更新をさせないようにする
					//_disablePageForNetworkCall = true;
					if(_scrollBar != null)
					{
						_scrollBar.GetComponent<Scrollbar> ().value = 0.005f;
					}
					_maxAfterID = _maxAfterIDBackup;
					_maxBeforeID = _maxBeforeIDBackup;

					Debug.Log ("存在しないページにアクセスしようとしたのでやめる");
				}
			} else {
				//_disablePageForNetworkCall = true;
				if (_scrollBar != null) 
				{
					//_scrollBar.GetComponent<Scrollbar> ().value = 0.005f;
				}
				_maxAfterID = _maxAfterIDBackup;
				_maxBeforeID = _maxBeforeIDBackup;

				Debug.Log ("存在しないページにアクセスしようとしたのでやめる");
			}         
        }

        #endregion

        #region Finger Event
        /// <summary>
        /// Backs the swipe.
        /// </summary>
        void OnSwipe (SwipeGesture gesture)
        {
            if (gesture.Selection) {
                if (gesture.Direction == FingerGestures.SwipeDirection.Left) {
                    
                } else if (gesture.Direction == FingerGestures.SwipeDirection.Right) {
                    if (MessageEventManager.Instance != null) {

                        if (MessageEventManager.Instance._isFromPush == true) {
                            MessageEventManager.Instance.HeaderTab1 ();
                            MessageEventManager.Instance._isFromPush = false;
                            MessageEventManager.Instance.BackButton (this.gameObject);
                            return;
                        }
                        else if (_messageSceneProfilePanelShow) 
						{
							_panelProfile.GetComponent<ProfilePanel> ().ProfileToChatClose (this.gameObject);
						}else{
                            MessageEventManager.Instance.HeaderTab1 ();
							MessageEventManager.Instance.BackButton (this.gameObject);
						}
					}
                    
                    if (MatchingEventManager.Instance != null)
                    {
                        MatchingEventManager.Instance.BackButton (this.gameObject);
                    }
                    
					if (BulletinBoardEventManager.Instance != null)
					{
						BulletinBoardEventManager.Instance.BackButton(this.gameObject);
					}

					if (MypageEventManager.Instance != null || SearchEventManager.Instance != null) 
					{
						ProfilePanel.Instance.ProfileToChatClose (this.gameObject);
					}

                    if (PanelFooterButtonManager.Instance != null) 
                        PanelFooterButtonManager.Instance.gameObject.SetActive (true);

					_listUpdateDisable = true;
                }
            }
        }
        #endregion
        
        
        #region サムネイルを大きく表示する用
        /// <summary>
        /// Thumbnails the large.
        /// </summary>
        /// <returns>The large.</returns>
        /// <param name="obj">Object.</param>
        public void ThumbnailLargeOpen (GameObject obj)
        {
            _sendPictureLarge.gameObject.SetActive (true);
            this.GetComponent<BoxCollider2D> ().enabled = false;
            _sendPictureLargeOverlay.GetComponent<BoxCollider2D> ().enabled = true;
            PanelPopupAnimate (_sendPictureLargeOverlay.gameObject, false);
            _backSwipe.GetComponent<ScreenRaycaster> ().enabled = false;
            //_backSwipe.EventMessageTarget = _sendPictureLargeOverlay.gameObject;
            //メッセージのIDで取得
            var rawImage = _sendPictureLarge;

            string id = obj.name;
            foreach ( var msg in MessageListApi._httpCatchData.result.messages)
			{
                if (msg.id == id) 
				{
                    if (string.IsNullOrEmpty (msg.image.url) == false)
					{
                        StartCoroutine ( WwwToRendering (msg.image.url, rawImage));
                        break;
                    }
                }
            }
            HeaderPanel.Instance.BackButtonSwitch (false);
            HeaderPanel.Instance.BackButtonSwitch (true, ThumbnailLargeCloseEvent);
        }
        
        /// <summary>
        /// Thumbnails the large.
        /// </summary>
        /// <returns>The large.</returns>
        /// <param name="obj">Object.</param>
        public void ThumbnailLargeClose (GameObject obj)
        {
            this.GetComponent<BoxCollider2D> ().enabled = true;
            _sendPictureLargeOverlay.GetComponent<BoxCollider2D> ().enabled = false;
            _backSwipe.GetComponent<ScreenRaycaster> ().enabled = true;
            PanelPopupCloseAnimate (_sendPictureLargeOverlay.gameObject);
            //_backSwipe.EventMessageTarget = _panelChat;
            //メッセージのIDで取得
            var rawImage = _sendPictureLarge;
            rawImage.texture = null;
            _sendPictureLarge.gameObject.SetActive (false);

            HeaderPanel.Instance.BackButtonSwitch (false);
            HeaderPanel.Instance.BackButtonSwitch (true, ChatBackButton);
        }

        /// <summary>
        /// Thumbnails the large close event.
        /// </summary>
        /// <returns>The large close event.</returns>
        void ThumbnailLargeCloseEvent ()
		{
            ThumbnailLargeClose (_sendPictureLarge.gameObject);
        }
        
        /// <summary>
        /// Thumbnails the large close back.
        /// </summary>
        /// <returns>The large close back.</returns>
        void ThumbnailLargeCloseBack () 
        {
			if (MessageEventManager.Instance != null) 
			{
				MessageEventManager.Instance.BackButton (_panelChat);
			} else if ( MatchingEventManager.Instance != null ) {
                MatchingEventManager.Instance.BackButton (_panelChat);
            }
        
			if (MypageEventManager.Instance != null || SearchEventManager.Instance != null)
			{
				ProfilePanel.Instance.ProfileToChatClose (_panelChat);
			}
        }

        /// <summary>
        /// Wwws to rendering.
        /// TODO: 共通化にしていく。
        /// </summary>
        /// <returns>The to rendering.</returns>
        /// <param name="url">URL.</param>
        /// <param name="targetObj">Target object.</param>
        private IEnumerator WwwToRendering (string url, RawImage targetObj)
        {
            targetObj.texture = null;
            _loadingOverlay.SetActive (true);
            if (string.IsNullOrEmpty (url) == true) yield break;
            using (WWW www = new WWW (url)) {
                while (www == null)
                    yield return (www != null);
        
                while (www.isDone == false)
                    yield return (www.isDone);
        
                //non texture file
                if (string.IsNullOrEmpty (www.error) == false) {
                    Debug.LogError (www.error);
                    Debug.Log (url);
                    yield break;
                }
                while (targetObj == null)
                    yield return (targetObj != null);
        
                targetObj.texture = www.textureNonReadable;
                targetObj.transform.localScale  = new Vector3(1f, 1f, 1f);
                _loadingOverlay.SetActive (false);
            }
        }
        
        #endregion
        
        #region 外部用メソッド
        /// <summary>
        /// Resets the scroll item.
        /// </summary>
        /// <returns>The scroll item.</returns>
        public void ResetScrollItem() 
		{
            for (int i = 0; i < _scrollContent.childCount; i++) 
			{
                if (i > 4)
                    Destroy (_scrollContent.GetChild (i).gameObject);
            }
        }
        #endregion
        
        #region プロフィールオープン,クローズ、クローズイベント
        /// <summary>
        /// Profiles the open.
        /// </summary>
        /// <param name="animObj">Animation object.</param>
        /// <param name="obj">Object.</param>
        public void ProfileOpen (GameObject animObj)
		{
            if (PanelFooterButtonManager.Instance != null) 
               PanelFooterButtonManager.Instance.gameObject.SetActive (true);

			if (AppStartLoadBalanceManager._isBaseProfile == false) {
				PanelFooterButtonManager.Instance.NoRegistBaseProfile ();
				return;
			}

            if (MessageEventManager.Instance != null || MatchingEventManager.Instance != null) 
			{
				ProfilePanel.Instance.Init (_toUser);
				HeaderPanel.Instance.BackButtonSwitch (false);
				HeaderPanel.Instance.BackButtonSwitch (true, ProfileCloseEvent);
			}else{

				if(BulletinBoardEventManager.Instance != null)
				{
					ProfilePanel.Instance.Init (_toUser);
					//HeaderPanel.Instance.BackButtonSwitch (false);
					HeaderPanel.Instance.BackButtonSwitch (true, BulletinBoardEventManager.Instance.ProfileCloseEvent);
					_backSwipe.EventMessageTarget = _panelChat.gameObject;
				}else{
					_panelProfile.GetComponent<ProfilePanel> ().ProfileToChatClose (this.gameObject);
				}
			}
           

			_backSwipe.EventMessageTarget = _panelProfile.gameObject;
			_headerTitle.GetComponent<Text> ().text = LocalMsgConst.TITLE_PROFILE;
            //user idは、流れで取ってくる。
            PanelProfileAnimate (animObj);
           
            _panelChat.GetComponent<BoxCollider2D> ().enabled    = false;
            _panelProfile.GetComponent<BoxCollider2D> ().enabled = true;


			_messageSceneProfilePanelShow = true;
        }

        /// <summary>
        /// Profiles the close.
        /// </summary>
        /// <returns>The close.</returns>
        /// <param name="animObj">Animation object.</param>
        public void ProfileClose (GameObject animObj)
		{
            if (PanelFooterButtonManager.Instance != null) 
               PanelFooterButtonManager.Instance.gameObject.SetActive (false);

            _backSwipe.EventMessageTarget = _panelChat.gameObject;

            if (MessageListApi._httpCatchData != null) 
                _headerTitle.GetComponent<Text> ().text = MessageListApi._httpCatchData.result.to_user.name;
          
            _panelChat.GetComponent<BoxCollider2D> ().enabled = true;
            PanelProfileAnimate (animObj, true);

			_messageSceneProfilePanelShow = false;
            if (MessageEventManager.Instance != null && MessageEventManager.Instance._isFromPush == true) {
                MessageEventManager.Instance.HeaderTab1 ();
                MessageEventManager.Instance._isFromPush = false;
            }
        }
        
        
        /// <summary>
        /// Profiles the close event.
        /// </summary>
        /// <returns>The close event.</returns>
		public void ProfileCloseEvent ()
		{      
			HeaderPanel.Instance.BackButtonSwitch (false, ProfileCloseEvent);
			HeaderPanel.Instance.BackButtonSwitch (true, ChatBackButton);         

           	if (MessageEventManager.Instance != null) 
			{
                HeaderPanel.Instance.BackButtonSwitch (false);
    			HeaderPanel.Instance.BackButtonSwitch (true, MessageEventManager.Instance.ChatBackButton);
           	}
           
           	if (MatchingEventManager.Instance != null) 
			{
                HeaderPanel.Instance.BackButtonSwitch (false);
                HeaderPanel.Instance.BackButtonSwitch (true, MatchingEventManager.Instance.ChatBackButton);
           	}

			ProfileClose(_panelProfile);
		}
        
        /// <summary>
        /// Chats the back button.
        /// </summary>
        /// <returns>The back button.</returns>
		public void ChatBackButton () 
		{
			BackButton (_panelChat.gameObject);
			_panelChat.GetComponent<BoxCollider2D> ().enabled        = true;         
			_panelChat.GetComponent<PanelChat> ()._listUpdateDisable = true;
                     
			if (_panelProfile.transform.localScale.x != 0) 
			{
				PanelPopupCloseAnimate (_panelProfile.gameObject);
				_panelProfile.GetComponent<BoxCollider2D> ().enabled = true;
			}
		}

        /// <summary>
        /// Backs the button.
        /// </summary>
        /// <returns>The button.</returns>
        /// <param name="fromObj">From object.</param>
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
        #endregion
        
        
        //TODO: pop up 共通化にする　- ここから
        /// <summary>
        /// Reports the confirm button.
        /// </summary>
        public void ReportConfirmOpen()
		{
            string tmpText  = LocalMsgConst.REPORT_QUESTION;
            string text = string.Format(tmpText, MessageListApi._httpCatchData.result.to_user.name);
            
            PopupSecondSelectPanel.Instance.PopMessageInsert(
                text,
                LocalMsgConst.YES,
                LocalMsgConst.NO,
                ReportApiCall,
                ReportCancel
            );
            PanelPopupAnimate (PopupSecondSelectPanel.Instance.gameObject);
        }
        
        /// <summary>
        /// Reports the API call.
        /// </summary>
        void ReportApiCall() 
		{
            PopupSecondSelectPanel.Instance.PopClean ();
            PanelPopupCloseAnimate (GameObject.FindGameObjectWithTag(CommonConstants.POPUP_SECOND_SELECT_TAG));
            new SendReportApi (MessageListApi._httpCatchData.result.to_user.id);
            StartCoroutine (ReportApiCallWait ()) ;
        }
        /// <summary>
        /// Reports the API call wait.
        /// </summary>
        /// <returns>The API call wait.</returns>
        private IEnumerator ReportApiCallWait ()
        {
            _loadingOverlay.SetActive (true);
        
            while (SendReportApi._success == false)
                yield return (SendReportApi._success == true);
        
            _loadingOverlay.SetActive (false);
            PopupPanel.Instance.PopMessageInsert(
                SendReportApi._httpCatchData.result.complete[0],
                LocalMsgConst.OK,
                ReportFinishClose
            );
            PanelPopupAnimate (GameObject.FindGameObjectWithTag(CommonConstants.POPUP_BASIC_TAG));
        }
        
        /// <summary>
        /// Reports the cancel.
        /// </summary>
        void ReportCancel ()
		{
            PopupSecondSelectPanel.Instance.PopClean ();
            PanelPopupCloseAnimate (GameObject.FindGameObjectWithTag(CommonConstants.POPUP_SECOND_SELECT_TAG));
        }
        
        /// <summary>
        /// Reports the finish close.
        /// </summary>
        void ReportFinishClose ()
		{
            PopupPanel.Instance.PopClean ();
            PanelPopupCloseAnimate (GameObject.FindGameObjectWithTag(CommonConstants.POPUP_BASIC_TAG));
        }
        
        
        //---------------- ここからお気に入り有無のポップアップ ---------------- 
        
        /// <summary>
        /// Favorites the confirm open.
        /// </summary>
        /// <param name="target">Target.</param>
        public void FavoriteConfirmOpen(GameObject target)
        {
        
            string popText = "";
            if (_favoriteButton.transform.GetChild(0).gameObject.activeSelf == true)
			{
            	popText = string.Format (LocalMsgConst.FAVORITE_OFF_CONFIRM, GetUserApi._httpOtherUserCatchData.name);
            } else if (_favoriteButton.transform.GetChild(0).gameObject.activeSelf == false) {
            	popText = string.Format (LocalMsgConst.FAVORITE_ON_CONFIRM, GetUserApi._httpOtherUserCatchData.name);
            }

            PopupSecondSelectPanel.Instance.PopMessageInsert(
                popText,
                LocalMsgConst.YES,
                LocalMsgConst.NO,
                FavoriteApiCall,
                FavoriteCancel
            );
            PanelPopupAnimate (target);
        }

        /// <summary>
        /// Favorites the API call.
        /// </summary>
        public void FavoriteApiCall ()
		{
           	PopupPanel.Instance.PopClean();
			PopupSecondSelectPanel.Instance.PopClean(FavoriteApiCall, FavoriteCancel);

           	PanelPopupCloseAnimate (GameObject.FindGameObjectWithTag(CommonConstants.POPUP_SECOND_SELECT_TAG));           
           	StartCoroutine (FavoriteApiCallWait ());
        }
        
        /// <summary>
        /// Favorites the API call wait.
        /// </summary>
        /// <returns>The API call wait.</returns>
        private IEnumerator FavoriteApiCallWait()
		{
			_loadingOverlay.SetActive (true);
            new SetUserFavoriteApi (_toUser);
            while (SetUserFavoriteApi._success == false)
                yield return (SetUserFavoriteApi._success == true);
            _loadingOverlay.SetActive (false);
            
            PopupPanel.Instance.PopMessageInsert(
                SetUserFavoriteApi._httpCatchData.result.complete[0],
                LocalMsgConst.OK,
				FavoriteFinishClose
            );
            PanelPopupAnimate (GameObject.FindGameObjectWithTag(CommonConstants.POPUP_BASIC_TAG)); 

			string isFavorite = SetUserFavoriteApi._httpCatchData.result.is_favorite;
            if (isFavorite == "true")
			{
            	_favoriteButton.transform.GetChild (0).gameObject.SetActive (true);
            } else if (isFavorite == "false") {
            	_favoriteButton.transform.GetChild (0).gameObject.SetActive (false);
            }
        }
        
        
        /// <summary>
        /// Reports the cancel.
        /// </summary>
        void FavoriteCancel () 
		{
            PopupPanel.Instance.PopClean ();
			PopupSecondSelectPanel.Instance.PopClean(FavoriteApiCall, FavoriteCancel);
            
			PanelPopupCloseAnimate (GameObject.FindGameObjectWithTag(CommonConstants.POPUP_SECOND_SELECT_TAG));
        }
        
        /// <summary>
        /// Reports the finish close.
        /// </summary>
        void FavoriteFinishClose ()
		{
			PopupPanel.Instance.PopClean ();
			PopupSecondSelectPanel.Instance.PopClean(FavoriteApiCall, FavoriteCancel);

			PanelPopupCloseAnimate (GameObject.FindGameObjectWithTag(CommonConstants.POPUP_BASIC_TAG));
        }
        //---------------- ここまで - お気に入り有無のポップアップ ---------------- 
        
        

		//---------------- ここから - 送り放題のポップアップ ---------------- 
		public void LimitMessageConfirmOpen() 
		{
			// メッセージ呼び出し
			new LimitReleaseConfirmMessage(_toUser);
			StartCoroutine (GetLimitReleaseConfirmMessage());
		}
		IEnumerator GetLimitReleaseConfirmMessage()
		{
			_loadingOverlay.SetActive (true);
			while (LimitReleaseConfirmMessage._success == false)
				yield return (LimitReleaseConfirmMessage._success == true);
			_loadingOverlay.SetActive (false);

			PopupSecondSelectPanel.Instance.PopMessageInsert(
				LimitReleaseConfirmMessage._httpCatchData.result.message[0],
				LocalMsgConst.YES,
				LocalMsgConst.NO,
				LimitReleaseApiCall,
				LimitReleaseCancel
			);
			PanelPopupAnimate (PopupSecondSelectPanel.Instance.gameObject);
		}

		void LimitReleaseApiCall () 
		{
			PopupPanel.Instance.PopClean ();
			PopupSecondSelectPanel.Instance.PopClean(LimitReleaseApiCall, LimitReleaseCancel);

			new LimitReleaseApi (_toUser);
			PanelPopupCloseAnimate (GameObject.FindGameObjectWithTag(CommonConstants.POPUP_SECOND_SELECT_TAG));
			StartCoroutine (SetUserLimitReleaseApiWait ());
		}

		private IEnumerator SetUserLimitReleaseApiWait ()
		{
			_loadingOverlay.SetActive (true);
			while (LimitReleaseApi._success == false)
				yield return (LimitReleaseApi._success == true);
			_loadingOverlay.SetActive (false);

		
			PopupPanel.Instance.PopMessageInsert
			(
				LimitReleaseApi._httpCatchData.result.complete [0],
				LocalMsgConst.OK,
				LimitReleaseFinishClose
			);
			PanelPopupAnimate (GameObject.FindGameObjectWithTag(CommonConstants.POPUP_BASIC_TAG));


			string islimitRelease = LimitReleaseApi._httpCatchData.result.is_limit_release;
			if (islimitRelease == "true")
			{
				_limitReleaseButton.GetComponent<Button> ().enabled = false;
				_limitReleaseButton.GetComponent<Image> ().color = new Color (147/255.0f,147/255.0f,147/255.0f,255);

				if(PanelBoardDetail.Instance != null)
				{
					PanelBoardDetail.Instance.GetLimitReleaseButton().GetComponent<Button> ().enabled = false;
					PanelBoardDetail.Instance.GetLimitReleaseButton().GetComponent<Image> ().color = new Color (147/255.0f,147/255.0f,147/255.0f,255);
				}
			} else if (islimitRelease == "false") {
				_limitReleaseButton.GetComponent<Image> ().color = new Color (247/255.0f,117/255.0f,133/255.0f,255);
				_limitReleaseButton.GetComponent<Button> ().enabled = true;
			}

		}

		void LimitReleaseCancel () 
		{
			PopupPanel.Instance.PopClean ();
			PopupSecondSelectPanel.Instance.PopClean(LimitReleaseApiCall,LimitReleaseCancel);

			PanelPopupCloseAnimate (GameObject.FindGameObjectWithTag(CommonConstants.POPUP_SECOND_SELECT_TAG));
		}

		void LimitReleaseFinishClose () 
		{
			PopupPanel.Instance.PopClean ();
			PopupSecondSelectPanel.Instance.PopClean(LimitReleaseApiCall,LimitReleaseCancel);

			PanelPopupCloseAnimate (GameObject.FindGameObjectWithTag(CommonConstants.POPUP_BASIC_TAG));
		}

		void MovePurchaseScene () 
		{
			PopupPanel.Instance.PopClean ();
			PopupSecondSelectPanel.Instance.PopClean(LimitReleaseApiCall,LimitReleaseCancel);

			PanelPopupCloseAnimate (GameObject.FindGameObjectWithTag(CommonConstants.POPUP_BASIC_TAG));

			PanelFooterButtonManager.Instance.Purchase ();
		}

		//---------------- ここまで - 送り放題のポップアップ ---------------- 





        //TODO: pop up 共通化にする　- ここまで
        
        
        #region メッセージ送信ロジック
        /// <summary>
        /// Messages the send button.
        /// </summary>
        /// <returns>The send button.</returns>
        public void MessageSendButton ()
        {
            Texture2D tex2D = _sendPicture.mainTexture as Texture2D;
            
            if (_sendPicture.texture == null)
			{
                tex2D = null;

                if (string.IsNullOrEmpty(_messageSend.text) == true) 
				{
                    Debug.Log ("何も入力されてない旨を伝えるポップアップが親切。");
                    return;
                }
            }
            
			// 画像かメッセージかの判断
			if (!_sendImage) 
			{
				Debug.Log ("send message only");
				tex2D = null;
			} else {

				Debug.Log ("send image");
			}

			new SendMessageApi (_toUser, _messageSend.text, tex2D);
            StartCoroutine (SendMessageApiWait (_messageSend.text, tex2D));

			// デフォルトで　メッセージ送信フラグへ戻しておく
			_sendImage = false;
        }
        
        /// <summary>
        /// Sends the message API wait.
        /// </summary>
        /// <returns>The message API wait.</returns>
        private IEnumerator SendMessageApiWait (string msg, Texture2D tex2D)
        {
            _loadingOverlay.SetActive (true);
            while (SendMessageApi._success == false)
                yield return (SendMessageApi._success == true);

            _loadingOverlay.SetActive (false);
            _messageSend.text    = "";
            //_sendPicture.texture = null;

			// 送信した時にそもそも一緒に通信飛ばすことにした
			Init(_toUser);
         
            Resources.UnloadUnusedAssets ();
            System.GC.Collect();
        }
        #endregion
        
        
        #region 内部関数
        /// <summary>
        /// Res the panel animate.
        /// </summary>
        /// <param name="target">Target.</param>
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
        
        

        /// <summary>
        /// Res the panel animate.
        /// </summary>
        /// <param name="target">Target.</param>
        private void PanelPopupAnimate (GameObject target, bool isBackground = true)
        {
            if (isBackground == true) 
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
        private void PanelPopupCloseAnimate ( GameObject target)
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


		public void EndEdit()
		{
			//Debug.Log("入力終了があったよ =" + _messageSend.text);  // 入力された文字を表示
			MessageSendButton ();
		}
		public void OnValueChange()
		{
			//Debug.Log("入力変更があったよ =" + _messageSend.text);  // 入力された文字を表示
		}


        private string _messageId;
        /// <summary>
        /// Mosaics the confirm image open.
        /// </summary>
        public void MosaicConfirmImageOpen ( GameObject messageId )
        {
            _messageId = messageId.name;
            PopupSecondSelectPanel.Instance.PopClean ();

            PopupSecondSelectPanel.Instance.PopMessageInsert(
                LocalMsgConst.POINT_USE_IMAGE_CONFIRM,
                LocalMsgConst.YES,
                LocalMsgConst.NO,
                ImageUsePointOk,
                ImageUsePointCancel
            );
            PanelPopupAnimate (GameObject.FindGameObjectWithTag(CommonConstants.POPUP_SECOND_SELECT_TAG));
        }
        /// <summary>
        /// Images the use point ok.
        /// </summary>
        void ImageUsePointOk (){
            PanelPopupCloseAnimate (GameObject.FindGameObjectWithTag(CommonConstants.POPUP_SECOND_SELECT_TAG));
#if UNITY_IPHONE || UNITY_ANDROID && !UNITY_EDITOR
                if (GetUserApi._httpCatchData.result.review == "false") {
                    if (CommonConstants.IS_PREMIUM == false) {
                        //動画広告が何もない場合はアイモバイル、インターステシャルを表示。
                        if (Maio.CanShow (CommonConstants.MAIO_ZONE_ID_1) == false) {
                        
                            if (AppStartLoadBalanceManager.m_NendAdFullBoard != null){
                                AppStartLoadBalanceManager.m_NendAdFullBoard.Load ();
                                AppStartLoadBalanceManager.m_NendAdFullBoard.Show ();
                            }
                            //インタースティシャルのイベントが取れないのでバナー表示で機能が使えるように。
                            StartCoroutine (ReadMessageApiWait ()); 
                        } else {
AppStartLoadBalanceManager._whereFromAds = MaioMovieSdkEvent.WhereFromAds.PanelChat;
                            //問答無用で動画広告を表示
                            Maio.Show (CommonConstants.MAIO_ZONE_ID_1);
                            return;
                        }
                    } else {
                        //有料会員の場合、何もしない。
                    }
                }
#endif
            
        }

         public void OnClosed (string zoneId) {
            StartCoroutine (ReadMessageApiWait ()); 
         }

        /// <summary>
        /// Reads the message API.
        /// </summary>
        /// <returns>The message API.</returns>
        private IEnumerator ReadMessageApiWait () {
            _loadingOverlay.SetActive (true);
            //画像閲覧でポイントを減算する処理。
            new ReadMessageApi (_messageId);
            while (ReadMessageApi._success == false)
                yield return (ReadMessageApi._success == true);
            _loadingOverlay.SetActive (false);

            //panel chat初期化処理
            Init (_toUser);
        }


        /// <summary>
        /// Images the use point cancel.
        /// </summary>
        void ImageUsePointCancel () {
            PopupSecondSelectPanel.Instance.PopClean ();
            PanelPopupCloseAnimate (GameObject.FindGameObjectWithTag(CommonConstants.POPUP_SECOND_SELECT_TAG));
        }
        #endregion
    }
}