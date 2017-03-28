using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using uTools;
using ViewController;
using UnityEngine.UI;
using Helper;
using Http;
using NendUnityPlugin.AD;
using NendUnityPlugin.Common;


namespace EventManager
{
    public class MatchingEventManager : SingletonMonoBehaviour<MatchingEventManager>
    {
        #region Seriarize Variable
        [SerializeField]
        private Transform _panelProfile;

        [SerializeField]
        private Transform _panelPopupLikeLimit;

        [SerializeField]
        private Transform _panelPopupSuperLikeLimit;

        [SerializeField]
        private Transform _tinderMain;

        [SerializeField]
        private GameObject _popupOverlay;

        [SerializeField]
        private GameObject _fingerGesture;

        [SerializeField]
        private GameObject _loadingOverlay;

        public void SetFingerGestureEnable (bool active)
        {
            _fingerGesture.GetComponent<FingerGestures> ().enabled = active;
        }

        [SerializeField]
        private Text _headerTitle;

        [SerializeField]
        private GameObject _tutorial;


       [SerializeField]
        private NendAdBanner _nendAdBanner;
        #endregion



        public bool _isStart = false;


        #region Button Click Scripting
        /// <summary>
        /// Ons the application pause.
        /// </summary>
        /// <returns>The application pause.</returns>
        /// <param name="pauseStatus">Pause status.</param>
        void OnApplicationPause (bool pauseStatus)
        {
            if (pauseStatus == true) {

            } else {
                string jsonCatch = null;
#if UNITY_ANDROID && !UNITY_EDITOR
      jsonCatch  = GCMService.GetPushMessage();
#elif UNITY_IPHONE && !UNITY_EDITOR
      jsonCatch = NativeRecieveManager.GetPushMessageIos ();
#endif

                if (string.IsNullOrEmpty (jsonCatch) == false) {
                    NotificationRecieveManager.NextSceneProccess (jsonCatch);
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

                //ここでユーザーキーを取得。
                AppStartLoadBalanceManager._userKey = LocalFileHandler.GetString (LocalFileConstants.USER_KEY);

#if UNITY_ANDROID
    //ステータスバーを表示 //Android用
    ApplicationChrome.statusBarState = ApplicationChrome.States.VisibleOverContent;
#endif
            }

            //ユーザーデータの取得。
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

            //審査レビューじゃない場合のみ広告表示。
            if (GetUserApi._httpCatchData.result.review == "false") {
#if UNITY_IPHONE || UNITY_ANDROID && !UNITY_EDITOR
                if (CommonConstants.IS_PREMIUM == false) 
                {
                    // 広告の取得、表示処理                
                    if (CommonConstants.IS_AD_TEST == true) {
                        //インタースティシャル広告
                        IMobileSdkAdsUnityPlugin.show (CommonConstants.IMOBILE_INTERSTATIAL_SPOT_TEST_ID);
                    } else {
                        //インタースティシャル広告
                        IMobileSdkAdsUnityPlugin.show (CommonConstants.IMOBILE_INTERSTATIAL_SPOT_ID);
                    }
                } else {
                    IMobileSdkAdsUnityPlugin.stop ();
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


            //マスターデータの取得
            if (InitDataApi._httpCatchData == null) {
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

            LocalFileHandler.Init (LocalFileConstants.GetLocalFileDir () + LocalFileConstants.COMMON_LOCAL_FILE_NAME);
            if (LocalFileHandler.GetBool (LocalFileConstants.TUTORIAL_MATCHING_KEY) == false) {
                _tutorial.SetActive (true);
            } else {
                _tutorial.SetActive (false);
            }

            _isStart = true;
            _loadingOverlay.SetActive (false);
            yield break;
        }


        /// <summary>
        /// Pops up panel close.
        /// </summary>
        public void PopUpPanelOpen (GameObject target)
        {
            PanelPopupAnimate (target);
        }

        /// <summary>
        /// Pops up panel close.
        /// </summary>
        public void PopUpPanelClose (GameObject target)
        {
            PanelPopupCloseAnimate (target);
        }

        public void PopUpPanelSuperLike (GameObject target)
        {
#if UNITY_EDITOR

#endif
            _tinderMain.GetComponent<TinderGesture> ().PopupSuperLikeButton ();
			PanelPopupCloseAnimate (target);
		}


        /// <summary>
        /// Profiles the open.
        /// 第２引数がユーザー特定する用。
        /// </summary>
        /// <param name="animObj">Animation object.</param>
        /// <param name="obj">Object.</param>
        public void ProfileOpen (GameObject animObj, GameObject obj)
		{
            HeaderPanel.Instance.BackButtonSwitch (true, ProfileBackButton);
            string userId = obj.name;
			_panelProfile.GetComponent<ProfilePanel> ().Init (userId);
            PanelProfileAnimate (animObj);
        }
        /// <summary>
        /// Profiles the open.
        /// </summary>
        /// <param name="animObj">Animation object.</param>
        public void ProfileClose (GameObject animObj) {
            if (_tinderMain.GetComponent<ScreenRaycaster> () != null)
                _tinderMain.GetComponent<ScreenRaycaster> ().enabled = true;

            HeaderPanel.Instance.BackButtonSwitch (false, ProfileBackButton);
            PanelProfileAnimate (animObj, true);
        }

        /// <summary>
        /// Profiles the back button.
        /// Eventセット用
        /// </summary>
        void ProfileBackButton () {
            ProfileClose (_panelProfile.gameObject);
        }
        #endregion

 
        #region private method
        
        /// <summary>
        /// Res the panel animate.
        /// </summary>
        /// <param name="target">Target.</param>
        public void PanelPopupAnimateMatch ( GameObject target )
        {
            _popupOverlay.SetActive (true);
            TinderMainPictureCollider();
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
        public void PanelPopupAnimate ( GameObject target )
        {
            _popupOverlay.SetActive (true);
            TinderMainPictureCollider();
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
            TinderMainPictureCollider(true);
            target.GetComponent<uTweenScale> ().delay    = 0.01f;
            target.GetComponent<uTweenScale> ().duration = 0.25f;
            target.GetComponent<uTweenScale> ().from = new Vector3 (1f, 1f ,1f );
            target.GetComponent<uTweenScale> ().to = Vector3.zero;
            target.GetComponent<uTweenScale> ().ResetToBeginning ();
            target.GetComponent<uTweenScale> ().enabled = true;
        }

        /// <summary>
        /// Res the panel animate.
        /// </summary>
        /// <param name="target">Target.</param>
        private void PanelProfileAnimate ( GameObject target, bool isTo = false )
        {
            TinderMainPictureCollider (isTo);
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



        /// <summary>
        /// Backs the button.
        /// </summary>
        /// <param name="fromObj">From object.</param>
        /// <param name="toObj">To object.</param>
        public void PanelAnimate (GameObject fromObj) 
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
        /// Backs the button.
        /// </summary>
        /// <param name="fromObj">From object.</param>
        /// <param name="toObj">To object.</param>
        public void BackButton (GameObject fromObj) 
        {
            HeaderPanel.Instance.BackButtonSwitch (false);
            PanelChat.Instance.ResetScrollItem ();
            _headerTitle.GetComponent<Text>().text = LocalMsgConst.TITLE_MATCHING;

            if (_tinderMain.GetComponent <ScreenRaycaster> () != null)
                _tinderMain.GetComponent <ScreenRaycaster> ().enabled = true;
                

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

            BackButton (PanelChat.Instance.gameObject);
            PanelChat.Instance.GetComponent<BoxCollider2D> ().enabled = true;

            PanelChat.Instance.GetComponent<PanelChat> ()._listUpdateDisable = true;

            if (_tinderMain.GetComponent <ScreenRaycaster> () != null)
                _tinderMain.GetComponent <ScreenRaycaster> ().enabled = true;

            if (ProfilePanel.Instance.transform.localScale.x != 0) 
            {
                PanelPopupCloseAnimate (_panelProfile.gameObject);
                ProfilePanel.Instance.GetComponent<BoxCollider2D> ().enabled = true;
            }
            
            TinderGesture.Instance._isEventPopUp = false;
            TinderGesture.Instance.GetComponent<ScreenRaycaster> ().enabled = true;
        }

        /// <summary>
        /// Tinders the main picture collider.
        /// </summary>
        /// <param name="isOnOff">If set to <c>true</c> is on off.</param>
        private void TinderMainPictureCollider(bool isOnOff = false) {
           int index = _tinderMain.childCount - 1;
            _tinderMain.GetChild (index).GetComponent<BoxCollider2D> ().enabled = isOnOff;
        }
        #endregion
    }
}