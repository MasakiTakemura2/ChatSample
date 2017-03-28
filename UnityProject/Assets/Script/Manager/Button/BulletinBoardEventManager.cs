using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using uTools;
using ViewController;
using Http;
using Helper;
using NendUnityPlugin.AD;
using NendUnityPlugin.Common;

namespace EventManager
{
    public class BulletinBoardEventManager : SingletonMonoBehaviour<BulletinBoardEventManager>
    {
        [System.Serializable]
        public class SearchCondition
        {
            public string _CategoryID = "";
            public string _lat = "";
            public string _lng = "";
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
    
        #region Serialize Valiable
        [SerializeField]
        private GameObject _panelEazyNotify;

        [SerializeField]
        private GameObject _panelBoardDetail;

        [SerializeField]
        private GameObject _panelSearchCondition;

        [SerializeField]
        private GameObject _panelBulletinBoardWrite;

        [SerializeField]
        private SwipeRecognizer _backSwipe;

        [SerializeField]
        private GameObject _panelProfile;

        [SerializeField]
        private Text _tmpText;

        [SerializeField]
        private GameObject _loadingOverlay;

        [SerializeField]
        private GameObject _popupOverLayForDetail;

        [SerializeField]
        private GameObject _popupSecondPanel;

        // 拡大画像表示用
        [SerializeField]
        private GameObject _scaleImagePanel;

        [SerializeField]
        private GameObject _popupMovie;
        
        public bool _listConnectUpdateDisable = false;

        // 絞り込み条件用のID
        public enum CurrentSettingState
        {
            Sex,
            BodyType,
            Sort,
            Radius
        }
        public CurrentSettingState _currentSettingState;

        [SerializeField]
        private GameObject _panelChat;

        // チャット画面へ
        public bool _toFromBoardOrProflilePanel = false;

        [SerializeField]
        private GameObject _popupOverlay;

        [SerializeField]
        private NendAdBanner _nendAdBanner;

        public string _CategoryID = "";
        public string _lat = "";
        public string _lng = "";
        public string _sex = "1";
        public string _ageFrom = "";
        public string _ageTo = "";
        public string _heightFrom = "";
        public string _heightTo = "";
        public string _bodyType = "";
        public string _isImage = "";
        public string _radius = "";
        public string _keyword = "";
        public string _nowShowPage = "0";

        // プロフィール表示用のid
        public UserDataEntity.Basic _profileUserData = null;

        public bool _isEventStart = false;

        /// <summary>
        /// The tex caches.
        /// 一度WWWで取り込んだ画像をキャッシュでデータを読み込みの高速化。
        /// </summary>
        public Dictionary<string, Texture> _profTexCaches = new Dictionary<string, Texture> ();
        #endregion


        #region ユーザーの画像アップロード前準備用のステート。
        /// <summary>
        /// The type of the chat or selef image.
        /// チャットのトークルームか？掲示板か？
        /// </summary>
        public ChatOrSelefImageType _chatOrSelefImageType;

        /// <summary>
        /// Chats the image set.
        /// チャットで画像を送る場合
        /// </summary>
        /// <returns>The image set.</returns>
        public void ChatImageSet ()
        {
            _chatOrSelefImageType = ChatOrSelefImageType.Chat;
        }

        /// <summary>
        /// Boards the image set.
        /// 掲示板の画像
        /// </summary>
        /// <returns>The image set.</returns>
        public void BoardImageSet ()
        {
            _chatOrSelefImageType = ChatOrSelefImageType.Self;
        }
        #endregion


        #region life Cycle
        /// <summary>
        /// Ons the application pause.
        /// </summary>
        /// <returns>The application pause.</returns>
        /// <param name="pauseStatus">Pause status.</param>
        void OnApplicationPause (bool pauseStatus)
        {
            if (pauseStatus == true) {

            } else {
                string catchData = null;
#if UNITY_ANDROID && !UNITY_EDITOR
    catchData  = GCMService.GetPushMessage();
#elif UNITY_IPHONE && !UNITY_EDITOR
    catchData = NativeRecieveManager.GetPushMessageIos ();
#endif
                if (string.IsNullOrEmpty(catchData) == false) 
                {
                    NotificationRecieveManager.NextSceneProccess (catchData);
                }
            }
        }

        /// <summary>
        /// Start this instance.
        /// </summary>
        IEnumerator Start ()
        {
            PanelStateManager.InitPanelSet ();
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

            //ユーザー情報を取得
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
            if (AppliEventController.Instance.MaintenanceCheck () == true)
            {
                _loadingOverlay.SetActive (false);
                yield break;
            }

            ///ユーザーのステータスをチェックする処理。
            AppliEventController.Instance.UserStatusProblem ();

            ///強制アップデートの場合、処理を止める。
            if (AppliEventController.Instance.ForceUpdateCheck () == true) 
            {
                _loadingOverlay.SetActive (false);
                yield break;
            }

            ///アプリポップアップレビューの立ち上げ処理。
            AppliEventController.Instance.AppliReview ();

            //マスター情報を取得。
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

            _tmpText.text = LocalMsgConst.TITLE_BBS;

            _loadingOverlay.SetActive (false);

            if (_isEventStart == false)
                _isEventStart = true;
            yield break;
        }

        #endregion


        #region ボタンメソッド群
        public void BackButton (GameObject fromObj)
        {
            if (_toFromBoardOrProflilePanel) {
                HeaderPanel.Instance.BackButtonSwitch (false);
                HeaderPanel.Instance.BackButtonSwitch (true, ProfileCloseEvent);

            } else {
                HeaderPanel.Instance.BackButtonSwitch (false);
                HeaderPanel.Instance.BackButtonSwitch (true, BoardDetailCloseEvent);
            }

            fromObj.GetComponent<uTweenPosition> ().delay = 0.001f;
            fromObj.GetComponent<uTweenPosition> ().duration = 0.25f;
            fromObj.GetComponent<uTweenPosition> ().to = fromObj.transform.GetComponent<uTweenPosition> ().from;
            fromObj.GetComponent<uTweenPosition> ().from = Vector3.zero;
            fromObj.GetComponent<uTweenPosition> ().ResetToBeginning ();
            fromObj.GetComponent<uTweenPosition> ().enabled = true;
        }

        #endregion



        #region 掲示板検索条件のOpen Close 処理
        public void ConditionOpen (GameObject animObj)
        {
        #if !UNITY_EDITOR && (UNITY_ANDROID || UNITY_IOS)
            PanelBulletinBoardChange.Instance.NativePickerInit ();
        #else
            PanelBulletinBoardChange.Instance.Init ();
            
        #endif
            
            HeaderPanel.Instance.BackButtonSwitch (true, ConditionCloseEvent);
            _backSwipe.EventMessageTarget = _panelSearchCondition;
            PanelAnimate (animObj);
            _listConnectUpdateDisable = true;
            
        }

        public void ConditionClose (GameObject animObj)
        {
            BackButton (animObj);
            HeaderPanel.Instance.BackButtonSwitch (false, ConditionCloseEvent);
            _listConnectUpdateDisable = false;
        }

        void ConditionCloseEvent ()
        {
            ConditionClose (_panelSearchCondition);
        }
        #endregion



        #region 新規掲示板のOpen Close 処理
        public void NewBoardOpen (GameObject animObj)
        {
            PanelBulletinBoardWrite.Instance.Init ();
            HeaderPanel.Instance.BackButtonSwitch (false);
            HeaderPanel.Instance.BackButtonSwitch (true, NewBoardCloseEvent);

            _backSwipe.EventMessageTarget = _panelBulletinBoardWrite;
            PanelAnimate (animObj);
        }

        public void NewBoardClose (GameObject animObj)
        {
            BackButton (animObj);
            HeaderPanel.Instance.BackButtonSwitch (false, NewBoardCloseEvent);

        }

        void NewBoardCloseEvent ()
        {
            NewBoardClose (_panelBulletinBoardWrite);
        }
        #endregion



        #region プロフィールの処理
        public void ProfileOpen (GameObject animObj)
        {
            HeaderPanel.Instance.BackButtonSwitch (false, BoardDetailCloseEvent);
            HeaderPanel.Instance.BackButtonSwitch (true, ProfileCloseEvent);
            _backSwipe.EventMessageTarget = _panelProfile;

            // 指定ユーザーのプロフィールをひらいた時にボタン制御に必要なデータをセットする
            /*
			_panelProfile.GetComponent<ProfilePanel> ().InitFromAlreadyData (_profileUserData);
			GetUserApi._httpCatchData = new UserDataEntity.Result();
			GetUserApi._httpCatchData.result = new UserDataEntity.User ();
			GetUserApi._httpCatchData.result.user = _profileUserData; 
			*/
            _panelProfile.GetComponent<ProfilePanel> ().Init (_profileUserData.id);
            _panelProfile.GetComponent<ProfilePanel> ()._toUserId = _profileUserData.id;
            _panelBoardDetail.GetComponent<BoxCollider2D> ().enabled = false;

            PanelBoardListAnimate (animObj);
        }

        public void ProfileClose (GameObject animObj)
        {
            _panelBoardDetail.GetComponent<PanelBoardDetail> ().Init ();

            HeaderPanel.Instance.BackButtonSwitch (false, ProfileCloseEvent);
            HeaderPanel.Instance.BackButtonSwitch (true, BoardDetailCloseEvent, PanelStateManager.GetHeaderStringByKey ("BulletInBoardPanel"));
            _backSwipe.EventMessageTarget = _panelBoardDetail;

            PanelBoardListAnimate (animObj, true);

            _panelBoardDetail.GetComponent<BoxCollider2D> ().enabled = true;
            _panelChat.GetComponent<PanelChat> ()._listUpdateDisable = true; //無効化処理
        }

        public void ProfileCloseEvent ()
        {
            if (PanelFooterButtonManager.Instance != null) 
               PanelFooterButtonManager.Instance.gameObject.SetActive (true);
            ProfileClose (_panelProfile);
        }
        #endregion



        #region チャット画面処理

        // チャット画面へ移動のボタン処理
        public void ProfileToChatOpen (GameObject animObj)
        {

#if UNITY_IPHONE || UNITY_ANDROID && !UNITY_EDITOR
            if (GetUserApi._httpCatchData.result.review == "false") {
                //動画広告が何もない場合はアイモバイル、インターステシャルを表示。
                if (Maio.CanShow (CommonConstants.MAIO_ZONE_ID_1) == false) {
                    if (AppStartLoadBalanceManager.m_NendAdFullBoard != null){
                        AppStartLoadBalanceManager.m_NendAdFullBoard.Load ();
                        AppStartLoadBalanceManager.m_NendAdFullBoard.Show ();
                    }
                    ProfileToChatOpenMethod ("");

                } else {
                    //サーバーにイベント通知用Api(インストール時に一回のみ) ----------------------
                    string commonFileName = LocalFileConstants.GetLocalFileDir () + LocalFileConstants.COMMON_LOCAL_FILE_NAME;

                    LocalFileHandler.Init (commonFileName);
                    string isMovieFlag = LocalFileHandler.GetString (LocalFileConstants.MOVIE_POPUP_SHOW);

                    if (string.IsNullOrEmpty (isMovieFlag) == true) {
AppStartLoadBalanceManager._whereFromAds = MaioMovieSdkEvent.WhereFromAds.BoardToProfileToMessageSend;
                        PanelPopupAnimate (_popupMovie);
                        return;
                    } else {
AppStartLoadBalanceManager._whereFromAds = MaioMovieSdkEvent.WhereFromAds.BoardToProfileToMessageSend;
                        //問答無用で動画広告を表示
                        Maio.Show (CommonConstants.MAIO_ZONE_ID_1);
                        return;
                    }
                }
            }

#else
ProfileToChatOpenMethod ("");
#endif

        }
        
        /// <summary>
        /// Movies the popup look button.
        /// 動画をみて、チャットルームを開く
        /// </summary>
        public void MoviePopupLookButton () {
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
        
        /// <summary>
        /// Profiles to chat open method.
        /// </summary>
        /// <param name="zoneId">Zone identifier.</param>
        public void ProfileToChatOpenMethod (string zoneId) {
            _panelChat.GetComponent<PanelChat> ()._maxAfterID = "";
            _panelChat.GetComponent<PanelChat> ()._maxBeforeID = "";
            _panelChat.GetComponent<PanelChat> ()._maxAfterIDBackup = "";
            _panelChat.GetComponent<PanelChat> ()._maxBeforeIDBackup = "";
            _panelChat.GetComponent<PanelChat> ()._listUpdateDisable = false; //解除処理
            PanelChat.Instance._toUser = _profileUserData.id;
            PanelChat.Instance.Init (_profileUserData.id);

            PanelPopupCloseAnimate (_panelProfile);
            HeaderPanel.Instance.BackButtonSwitch (false);
            HeaderPanel.Instance.BackButtonSwitch (true, ProfileToChatCloseEvent);
            _backSwipe.EventMessageTarget = _panelChat;
            PanelAnimate (PanelChat.Instance.gameObject);

            _toFromBoardOrProflilePanel = true;
        }


        /// <summary>
        /// Boards the detail to chat open.
        /// </summary>
        /// <param name="animObj">Animation object.</param>
        public void BoardDetailToChatOpen (GameObject animObj)
        {
#if UNITY_IPHONE || UNITY_ANDROID && !UNITY_EDITOR
            if (GetUserApi._httpCatchData.result.review == "false") {

                //動画広告が何もない場合はアイモバイル、インターステシャルを表示。
                if (Maio.CanShow (CommonConstants.MAIO_ZONE_ID_1) == false) {
                    if (AppStartLoadBalanceManager.m_NendAdFullBoard != null){
                        AppStartLoadBalanceManager.m_NendAdFullBoard.Load ();
                        AppStartLoadBalanceManager.m_NendAdFullBoard.Show ();
                    }
                    BoardDetailToChatMethod ("");
                } else {
                    //サーバーにイベント通知用Api(インストール時に一回のみ) ----------------------
                    string commonFileName = LocalFileConstants.GetLocalFileDir () + LocalFileConstants.COMMON_LOCAL_FILE_NAME;

                    LocalFileHandler.Init (commonFileName);
                    string isMovieFlag = LocalFileHandler.GetString (LocalFileConstants.MOVIE_POPUP_SHOW);

                    if (string.IsNullOrEmpty (isMovieFlag) == true) {
AppStartLoadBalanceManager._whereFromAds = MaioMovieSdkEvent.WhereFromAds.BoardDetailChatOpen;
                        PanelPopupAnimate (_popupMovie);
                        return;
                    } else {
AppStartLoadBalanceManager._whereFromAds = MaioMovieSdkEvent.WhereFromAds.BoardDetailChatOpen;
                        //問答無用で動画広告を表示
                        Maio.Show (CommonConstants.MAIO_ZONE_ID_1);
                        return;
                    }
                }
			} else {
				//レビューの場合「true」の場合は、強制的に
				BoardDetailToChatMethod (""); //chatを開く。
			}
#elif UNITY_EDITOR
BoardDetailToChatMethod ("");
#endif

        }
        
        /// <summary>
        /// Boards the detail to chat method.
        /// </summary>
        public void BoardDetailToChatMethod ( string zoneId ) {
            _panelChat.GetComponent<PanelChat> ()._maxAfterID = "";
            _panelChat.GetComponent<PanelChat> ()._maxBeforeID = "";
            _panelChat.GetComponent<PanelChat> ()._maxAfterIDBackup = "";
            _panelChat.GetComponent<PanelChat> ()._maxBeforeIDBackup = "";
            _panelChat.GetComponent<PanelChat> ()._listUpdateDisable = false; //解除処理

            
            
            PanelPopupCloseAnimate (_panelBoardDetail);
            HeaderPanel.Instance.BackButtonSwitch (false);
            HeaderPanel.Instance.BackButtonSwitch (true, ProfileToChatCloseEvent);
            _backSwipe.EventMessageTarget = _panelChat;
            PanelAnimate (PanelChat.Instance.gameObject);

            _panelBoardDetail.GetComponent<BoxCollider2D> ().enabled = false;
            _toFromBoardOrProflilePanel = false;

            PanelChat.Instance._toUser = _profileUserData.id;
            PanelChat.Instance.Init (_profileUserData.id);
        }

        // チャット画面　閉じるとき チャットから掲示版内のプロフィールへ処理遷移
        public void ProfileToChatClose (GameObject animObj)
        {

            // 詳細からきたかプロフィールからきたか？
            if (_toFromBoardOrProflilePanel) {
                PanelBoardDetail.Instance.Init ();

                // 冗長かもしれないが　常に更新するために通信とばしたほうがいい・・・？
                _panelProfile.GetComponent<ProfilePanel> ().Init (_profileUserData.id);

                PanelChat.Instance.ResetScrollItem ();
                PanelPopupAnimate (_panelProfile);
                HeaderPanel.Instance.BackButtonSwitch (false, ProfileToChatCloseEvent);
                HeaderPanel.Instance.BackButtonSwitch (true, ProfileCloseEvent);
                _backSwipe.EventMessageTarget = _panelProfile;
                BackButton (animObj);
            } else {
                _panelBoardDetail.GetComponent<PanelBoardDetail> ().Init ();

                PanelChat.Instance.ResetScrollItem ();
                PanelPopupAnimate (_panelBoardDetail);
                HeaderPanel.Instance.BackButtonSwitch (false);
                HeaderPanel.Instance.BackButtonSwitch (true, BoardDetailCloseEvent);
                _backSwipe.EventMessageTarget = _panelBoardDetail;
                BackButton (animObj);

                _panelBoardDetail.GetComponent<BoxCollider2D> ().enabled = true;
            }

            //無効化処理
            _panelChat.GetComponent<PanelChat> ()._listUpdateDisable = true;
        }

        /// <summary>
        /// Profiles to chat close event.
        /// </summary>
        public void ProfileToChatCloseEvent ()
        {
            if (PanelFooterButtonManager.Instance != null) 
                PanelFooterButtonManager.Instance.gameObject.SetActive (true);

            ProfileToChatClose (_panelChat);
        }
        #endregion


        #region 掲示板詳細画面のボタン群
        // 通報ボタン
        public void ReportConfirmOpen (GameObject target)
        {
            string question = string.Format (LocalMsgConst.REPORT_QUESTION, GetUserApi._httpOtherUserCatchData.name);
            PopupSecondSelectPanel.Instance.PopMessageInsert (
                question,
                LocalMsgConst.YES,
                LocalMsgConst.NO,
                ReportApiCall,
                ReportCancel
            );
            PanelPopupAnimate (target);
        }
        void ReportApiCall ()
        {
            PopupSecondSelectPanel.Instance.PopClean ();

            StartCoroutine (SendReportWait ());
            PanelPopupCloseAnimate (GameObject.FindGameObjectWithTag (CommonConstants.POPUP_SECOND_SELECT_TAG));
        }
        private IEnumerator SendReportWait ()
        {
            _loadingOverlay.SetActive (true);
            new SendReportApi (GetUserApi._httpOtherUserCatchData.id);
            while (SendReportApi._success == false)
                yield return (SendReportApi._success == true);
            _loadingOverlay.SetActive (false);

            PopupPanel.Instance.PopMessageInsert (
                SendReportApi._httpCatchData.result.complete [0],
                LocalMsgConst.OK,
                ReportFinishClose
            );
            PanelPopupAnimate (GameObject.FindGameObjectWithTag (CommonConstants.POPUP_BASIC_TAG));
        }
        void ReportCancel ()
        {
            PopupSecondSelectPanel.Instance.PopClean ();

            PanelPopupCloseAnimate (GameObject.FindGameObjectWithTag (CommonConstants.POPUP_SECOND_SELECT_TAG));
        }
        void ReportFinishClose ()
        {
            PopupPanel.Instance.PopClean ();

            PanelPopupCloseAnimate (GameObject.FindGameObjectWithTag (CommonConstants.POPUP_BASIC_TAG));
        }

        #endregion



        #region 掲示板詳細のOpen Close 処理

        public void BoardDetailOpen (GameObject animObj)
        {
            //string title = PanelStateManager.GetHeaderStringByKey ("BulletInBoardDetailPanel");
            string title = PanelStateManager.GetHeaderStringByKey ("BulletInBoardPanel");
            HeaderPanel.Instance.BackButtonSwitch (false);
            HeaderPanel.Instance.BackButtonSwitch (true, BoardDetailCloseEvent, title);
            _backSwipe.EventMessageTarget = _panelBoardDetail;

            PanelBoardListAnimate (animObj);
        }
        public void BoardDetailClose (GameObject animObj)
        {
            string title = PanelStateManager.GetHeaderStringByKey ("BulletInBoardPanel");
            HeaderPanel.Instance.BackButtonSwitch (false, null , title);
            PanelBoardListAnimate (animObj, true);
        }
        void BoardDetailCloseEvent ()
        {
            BoardDetailClose (_panelBoardDetail);
            _panelChat.GetComponent<PanelChat> ()._listUpdateDisable = true; //解除処理
        }

        private void PanelBoardListAnimate (GameObject target, bool isTo = false)
        {
            target.GetComponent<uTweenScale> ().delay = 0.001f;
            target.GetComponent<uTweenScale> ().duration = 0.25f;

            if (isTo == true) {
                target.GetComponent<uTweenScale> ().from = new Vector3 (1f, 1f, 1f);
                target.GetComponent<uTweenScale> ().to = Vector3.zero;
            } else {
                target.GetComponent<uTweenScale> ().from = Vector3.zero;
                target.GetComponent<uTweenScale> ().to = new Vector3 (1f, 1f, 1f);
            }

            target.GetComponent<uTweenScale> ().ResetToBeginning ();
            target.GetComponent<uTweenScale> ().enabled = true;
        }

        public void TapThisBoardDetail (GameObject obj)
        {
            if (obj != null) {
                string id = obj.name;
                Debug.Log (id + " id id id id id ");
            }

            if (_panelBoardDetail != null) {
                _backSwipe.EventMessageTarget = _panelBoardDetail;
                if (_panelBoardDetail.GetComponent<uTweenPosition> ().from.x == 0) {
                    _panelBoardDetail.GetComponent<uTweenPosition> ().from = _panelBoardDetail.GetComponent<uTweenPosition> ().to;
                }

                _panelBoardDetail.GetComponent<uTweenPosition> ().to = Vector3.zero;
                _panelBoardDetail.GetComponent<uTweenPosition> ().ResetToBeginning ();
                _panelBoardDetail.GetComponent<uTweenPosition> ().enabled = true;
            }
        }

        /// <summary>
        /// Scales the image button.
        ///画像拡大
        /// </summary>
        /// <param name="button">Button.</param>
        public void ScaleImageButton (GameObject button)
        {
            if (_popupSecondPanel.transform.localScale.x != 0) {
                return;
            }
            if (ReadBoardApi._httpCatchData.result.board.images == null) {
                return;
            }
            if (int.Parse (button.name) >= ReadBoardApi._httpCatchData.result.board.images.Count) {
                return;
            }

            _popupOverLayForDetail.SetActive (true);
            PanelPopupAnimate (_scaleImagePanel);
            switch (int.Parse (button.name)) {
            case 0:
                _scaleImagePanel.GetComponent<ScaleImagePopUp> ().SetRawImage (ReadBoardApi._httpCatchData.result.board.images [0].url);
                break;
            case 1:
                _scaleImagePanel.GetComponent<ScaleImagePopUp> ().SetRawImage (ReadBoardApi._httpCatchData.result.board.images [1].url);
                break;
            case 2:
                _scaleImagePanel.GetComponent<ScaleImagePopUp> ().SetRawImage (ReadBoardApi._httpCatchData.result.board.images [2].url);
                break;

            }
        }
        public void ScaleImageCloseButton (GameObject target)
        {
            PanelPopupCloseAnimate (_scaleImagePanel);
            _scaleImagePanel.GetComponent<ScaleImagePopUp> ().SetRawImage (null);
            _popupOverLayForDetail.SetActive (false);

        }

        #endregion



        #region 条件検索の各項目処理制御

        // 表示順制御ボタン
        public void SortChangeClose (GameObject animObj)
        {
            HeaderPanel.Instance.BackButtonSwitch (false, SortChangeEvent);
            BackButton (animObj);

        }

        void SortChangeEvent ()
        {
            SortChangeClose (_panelSearchCondition);
        }

        #endregion


        #region 内部メソッド
		private IEnumerator BackAnimWait(GameObject animObj)
		{
			while (animObj.GetComponent<RectTransform> ().anchoredPosition.x != animObj.GetComponent<uTweenPosition> ().to.x) 
			{
				yield return (animObj.GetComponent<RectTransform> ().anchoredPosition.x == animObj.GetComponent<uTweenPosition> ().to.x);
			}
			_backSwipe.EventMessageTarget = _panelSearchCondition;
			yield break;
		}

		private void PanelAnimate ( GameObject target )
		{
			if (target.GetComponent<uTweenPosition> ().from.x == 0)
			{
				target.GetComponent<uTweenPosition> ().from = target.GetComponent<uTweenPosition> ().to;
			}

			target.GetComponent<uTweenPosition> ().delay    = 0.001f;
			target.GetComponent<uTweenPosition> ().duration = 0.25f;
			target.GetComponent<uTweenPosition> ().to = Vector3.zero;
			target.GetComponent<uTweenPosition> ().ResetToBeginning ();
			target.GetComponent<uTweenPosition> ().enabled = true;
		}

		public void PanelPopupAnimate ( GameObject target )
		{
			Debug.Log (target.name + " target name > > ") ;
			target.GetComponent<uTweenScale> ().from = Vector3.zero;
			target.GetComponent<uTweenScale> ().to = new Vector3 (1, 1 ,1 );
			target.GetComponent<uTweenScale> ().delay    = 0.01f;
			target.GetComponent<uTweenScale> ().duration = 0.25f;
			target.GetComponent<uTweenScale> ().ResetToBeginning ();
			target.GetComponent<uTweenScale> ().enabled = true;
		}

		public void PanelPopupCloseAnimate ( GameObject target )
		{
			target.GetComponent<uTweenScale> ().delay    = 0.01f;
			target.GetComponent<uTweenScale> ().duration = 0.25f;
			target.GetComponent<uTweenScale> ().from = new Vector3 (1f, 1f ,1f );
			target.GetComponent<uTweenScale> ().to = Vector3.zero;
			target.GetComponent<uTweenScale> ().ResetToBeginning ();
			target.GetComponent<uTweenScale> ().enabled = true;
		}


        #endregion

		#region 掲示板書き込みボタン
        /// <summary>
        /// Writes the button event.
        /// </summary>
        /// <returns>The button event.</returns>
		public void WriteButtonEvent()
		{
			PanelBulletinBoardWrite  panelBulletinBoardWriteScript = _panelBulletinBoardWrite.GetComponent<PanelBulletinBoardWrite> ();

			string title = panelBulletinBoardWriteScript.getTitle ();
			string body = panelBulletinBoardWriteScript.getBody ();

			Texture2D image1 = panelBulletinBoardWriteScript.getImage1 ();
			Texture2D image2 = panelBulletinBoardWriteScript.getImage2 ();
			Texture2D image3 = panelBulletinBoardWriteScript.getImage3 ();
            
			if (title == null && body == null) 
			{
                PopupPanel.Instance.PopClean ();
                PopupPanel.Instance.PopMessageInsert(
                    LocalMsgConst.BBS_NO_INPUT_VALIDATE,
                    LocalMsgConst.OK,
                    WriteWarningPopClose
                );
                PanelPopupAnimate (GameObject.FindGameObjectWithTag (CommonConstants.POPUP_BASIC_TAG));
				return;
			}

			StartCoroutine (WriteBoardAPICall (PanelBulletinBoardWrite.Instance._category, title, body, image1, image2, image3));
		}
        
        /// <summary>
        /// Writes the warning pop close.
        /// </summary>
        /// <returns>The warning pop close.</returns>
        void WriteWarningPopClose () {
            PopupPanel.Instance.PopClean ();
            PanelPopupCloseAnimate (GameObject.FindGameObjectWithTag (CommonConstants.POPUP_BASIC_TAG));
        }

        /// <summary>
        /// Sends the search API button.
        /// 絞り込み条件を決定して　apiをとばす
        /// </summary>
        /// <returns>The search API button.</returns>
		public void SendSearchApiButton()
		{
            PanelBulletinBoardChange.Instance.SetData ();
			PanelBoardList.Instance.InitializeFromCondition ();
			ConditionClose(_panelSearchCondition);

			_listConnectUpdateDisable = false;
		}

        /// <summary>
        /// Writes the board APIC all.
        /// </summary>
        /// <returns>The board APIC all.</returns>
        /// <param name="boardCategory">Board category.</param>
        /// <param name="title">Title.</param>
        /// <param name="body">Body.</param>
        /// <param name="image1">Image1.</param>
        /// <param name="image2">Image2.</param>
        /// <param name="image3">Image3.</param>
		private IEnumerator WriteBoardAPICall(string boardCategory, string title,string body,Texture2D image1,Texture2D image2,Texture2D image3)
		{
			WriteBoardApi ob = new WriteBoardApi (AppStartLoadBalanceManager._userKey,boardCategory,title,body, image1, image2, image3);
			_loadingOverlay.SetActive (true);
            
			while (WriteBoardApi._success == false) 
				yield return (WriteBoardApi._success == true);
		
			_loadingOverlay.SetActive (false);

			NewBoardClose (_panelBulletinBoardWrite);
			
			
            PopupPanel.Instance.PopClean ();
            PopupPanel.Instance.PopMessageInsert(
                LocalMsgConst.POST_FIX_MESSAGE,
                LocalMsgConst.OK,
                WriteWarningPopClose
            );
            PanelPopupAnimate (GameObject.FindGameObjectWithTag (CommonConstants.POPUP_BASIC_TAG));
            PanelBulletinBoardWrite.Instance.ResetNewBbs ();
            _loadingOverlay.SetActive (false);
		}
        
        void WriteFixPopClose () {
            PopupPanel.Instance.PopClean ();
            PanelPopupCloseAnimate (GameObject.FindGameObjectWithTag (CommonConstants.POPUP_BASIC_TAG));
            PanelBoardList.Instance.Initialize (0);
            
        }
		#endregion

        #region その他
        /// <summary>
        /// Backgrounds the over ray switch.
        /// </summary>
        public void BackgroundOverRaySwitch (bool isOn) {
            _popupOverlay.SetActive (isOn);
        }
        #endregion
    }
}