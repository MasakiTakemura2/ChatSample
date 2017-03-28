 using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using uTools;
using ViewController;
using Http;


namespace EventManager
{
    public class StartEventManager : SingletonMonoBehaviour<StartEventManager>
    {
        #region serialize valiable
        [SerializeField]
        private GameObject _panelGenderSelect;

        [SerializeField]
        private GameObject _panelChatDescripton;

        [SerializeField]
        private GameObject _maleButton;

        [SerializeField]
        private GameObject _femaleButton;

        [SerializeField]
        private GameObject _loadingOverlay;

        [SerializeField]
        private GameObject _popupOverlay;

        [SerializeField]
        private GameObject _panelProfileSetTemplate;

        [SerializeField]
        private ProfTemplateInfiniteLimitScroll _profTemplatefInfiniteLimitScroll;

        [SerializeField]
        private SwipeRecognizer _backSwipe;

        [SerializeField]
        private Transform _scrollContent;

        [SerializeField]
        private GameObject _genderError;
        
        [SerializeField]
        private GameObject _nameErrorText;
        
        [SerializeField]
        private GameObject _prefErrorText;
        
        [SerializeField]
        private GameObject _cityErrorText;

        [SerializeField]
        private GameObject _birthErrorText;

        [SerializeField]
        private GameObject _webviewTerms;

        public CurrentProfSettingStateType _currentProfSettingState;
        public UserDataEntity.Basic _userData;
        #endregion

        #region Member variable
        public GenderType _selfGender;
        public string _nickname;
        public string _prefId;
        public string _cityId;
        public string _gender;
        public string _birthday;
        private bool _isGenderSelect = false;
        #endregion
        
        
        #region life cycle
        void OnApplicationQuit () {
        }
        #endregion


        #region Button Methods

        /// <summary>
        /// Webviews the terms open.
        /// </summary>
        public void WebviewTermsOpen ()
        {
            HeaderPanel.Instance.BackButtonSwitch (false); 
            HeaderPanel.Instance.BackButtonSwitch (true, WebviewTermsCloseEvent); 
            _webviewTerms.SetActive (true);
            PanelAnimate (_webviewTerms);
        }

        /// <summary>
        /// Webviews the terms open.
        /// </summary>
        public void WebviewTermsClose ()
        {
            HeaderPanel.Instance.BackButtonSwitch (false); 
            BackButton (_webviewTerms);
            _webviewTerms.SetActive (false);
        }

        /// <summary>
        /// Webviews the terms close event.
        /// </summary>
        void WebviewTermsCloseEvent () {
            WebviewTermsClose ();
        }

        /// <summary>
        /// The term ok next panel.
        /// </summary>
        private GameObject _termOkNextPanel;

        /// <summary>
        /// Genders the next selected next.
        /// </summary>
        public void GenderNextSelectedNext ( GameObject nextPanel ) {
            //nextPanel.name
            //男女の選択が出来ていない場合。
            if (_isGenderSelect == false) {
                _genderError.SetActive (true);
                return;
            }

            //利用規約の了承を得るポップアップ
            PopupSecondSelectPanel.Instance.PopClean ();
            PopupSecondSelectPanel.Instance.PopMessageInsert (
                LocalMsgConst.CONFIRM_TEMRS_ALLOW_DENNY_TEXT,
                LocalMsgConst.YES,
                LocalMsgConst.NO,
                TermOkEvent,
                TermNoEvent
            );

            _termOkNextPanel = nextPanel; //後で使用する変数をセット。

            GameObject obj = GameObject.FindGameObjectWithTag (CommonConstants.POPUP_SECOND_SELECT_TAG);
            PanelPopupAnimate (obj);

        }

        /// <summary>
        /// Gender this instance.
        /// </summary>
        private void TermOk(GameObject nextPanel) {

#if UNITY_IPHONE
    StartCoroutine (MypageSceneJump());
    return;
#else
    //必要情報の入力が必要な場合。
    if (PanelStartInputUserData.Instance != null && nextPanel.name == PanelStartInputUserData.Instance.gameObject.name)
    {
       PanelStartInputUserData.Instance.Init ();
    }

    PanelAnimate ( nextPanel );
#endif

        }

        /// <summary>
        /// Terms the ok event.
        /// </summary>
        void TermOkEvent () {
            GameObject obj = GameObject.FindGameObjectWithTag (CommonConstants.POPUP_SECOND_SELECT_TAG);  
            PanelPopupCloseAnimate (obj);
            TermOk (_termOkNextPanel);
        }

        /// <summary>
        /// Terms the no event.
        /// </summary>
        void TermNoEvent () {
            GameObject obj = GameObject.FindGameObjectWithTag (CommonConstants.POPUP_SECOND_SELECT_TAG);  
            PanelPopupCloseAnimate (obj);
        }

        /// <summary>
        /// Nexts the button.
        /// </summary>
        public void NextButton (GameObject nextPanel)
        {
            //必要情報の入力が必要な場合。
            if (PanelStartInputUserData.Instance != null && nextPanel.name == PanelStartInputUserData.Instance.gameObject.name)
            {
               PanelStartInputUserData.Instance.Init ();
            }

            PanelAnimate ( nextPanel );
        }

        /// <summary>
        /// Scenes the jump.
        /// </summary>
        /// <returns>The jump.</returns>
        private IEnumerator MypageSceneJump () 
        {
            _userData.user_key = AppStartLoadBalanceManager._userKey;
            _userData.sex_cd   = AppStartLoadBalanceManager._gender;

            _loadingOverlay.SetActive (true);
            new ProfileUpdateApi (_userData);
            while (ProfileUpdateApi._success == false)
                yield return (ProfileUpdateApi._success == true);
            _loadingOverlay.SetActive (false);                

            SceneHandleManager.NextSceneRedirect (CommonConstants.MYPAGE_SCENE);
            yield break;
        }
        
        /// <summary>
        /// Nexts the rand post panel.
        /// </summary>
        /// <returns>The rand post panel.</returns>
        /// <param name="nextPanel">Next panel.</param>
        public void NextRandPostPanel(GameObject nextPanel ) 
        {
            PanelRandMessage.Instance.Init ();
            PanelAnimate ( nextPanel );
        }

        /// <summary>
        /// Genders the select.
        /// </summary>
        /// <param name="obj">Object.</param>
        public void GenderSelect (GameObject obj)
        {
            _isGenderSelect = true;
            if (_maleButton.name == obj.name) {
                _selfGender = GenderType.Male;
                AppStartLoadBalanceManager._gender = ((int)GenderType.Male).ToString();
                obj.transform.transform.GetChild (0).gameObject.SetActive(true);
                _femaleButton.transform.GetChild (0).gameObject.SetActive (false);
                _userData.sex_cd = AppStartLoadBalanceManager._gender;
            } else if (_femaleButton.name == obj.name) {
                _selfGender = GenderType.FeMale;
                AppStartLoadBalanceManager._gender = ((int)GenderType.FeMale).ToString();
                obj.transform.transform.GetChild (0).gameObject.SetActive(true);
                _maleButton.transform.GetChild (0).gameObject.SetActive (false);                
                _userData.sex_cd = AppStartLoadBalanceManager._gender;
            }
        }

        /// <summary>
        /// Rands the messege post.
        /// </summary>
        public void RandMessegePost( GameObject nextPanel) {
            PanelAnimate (nextPanel);
        }

        /// <summary>
        /// Gets the profile item.
        /// 都道府県選択項目を選択した時の処理。
        /// </summary>
        /// <param name="Object">Object.</param>
        public void GetProfileItem (GameObject Object)
        {

Debug.Log ("取得したIDをセット前" + Object.name + " == " + Instance._currentProfSettingState);

            string id = Object.name;
            switch (_currentProfSettingState) {
            case CurrentProfSettingStateType.Pref:
                _prefId = id;//データセット
                PlaceOfOriginClose (_panelProfileSetTemplate); //パネル閉じる
                break;
            case CurrentProfSettingStateType.City:
                _cityId = id; //データセット
                PlaceOfOriginDetailClose (_panelProfileSetTemplate); //パネル閉じる
                break;
            }

            //パラメータ反映用の処理。
            PanelStartInputUserData.Instance.SetData ();
        }

        /// <summary>
        /// Starts the button.
        /// </summary>
        /// <returns>The button.</returns>
        public void StartButton(GameObject animObj) 
        {
            PanelStartInputUserData.Instance.SetData ();
            bool isError = false;
            //ニックネームチェック
            if (string.IsNullOrEmpty (_userData.name) == true) {
                _nameErrorText.SetActive (true);
                isError = true;
            } else {
                _nameErrorText.SetActive (false);
            }
            
            //都道府県チェック
            if (string.IsNullOrEmpty (_userData.pref) == true) {
                _prefErrorText.SetActive (true);
                isError = true;
            } else {
                _prefErrorText.SetActive (false);
            }

            //市区町村チェック
            if (string.IsNullOrEmpty (_userData.city_id) == true) {
                _cityErrorText.SetActive (true);
                isError = true;
            } else {
                _cityErrorText.SetActive (false);
            }

            //年齢入力チェック
            if (string.IsNullOrEmpty (_userData.birth_date) == true) {
                _birthErrorText.SetActive(true);
                isError = true;
            } else {
                _birthErrorText.SetActive (false);
            }
            
            Debug.Log (_userData.name + ": _userData.name");
            Debug.Log (_userData.pref + ": _userData.pref");
            Debug.Log (_userData.city_id + ": _userData.city_id");
            Debug.Log (_userData.birth_date + ": _userData.birth_date");
            
            if (isError == true)
                return;

            new ProfileUpdateApi (_userData);
            StartCoroutine (ProfileUpdateApiWait(animObj));
        }
        
        /// <summary>
        /// Profiles the update API wait.
        /// </summary>
        /// <returns>The update API wait.</returns>
        private IEnumerator ProfileUpdateApiWait (GameObject animObj) {
            _loadingOverlay.SetActive (false);
            while (ProfileUpdateApi._success == false)
                yield return (ProfileUpdateApi._success == true);
            _loadingOverlay.SetActive (true);

            //基本プロフィール作成OK ♪
            AppStartLoadBalanceManager._isBaseProfile = true;
            
            //レビュー状態の時はランダムメッセージを送らないようにする
            if (PreRegistUser._httpCatchData.result.review == "false") {
                PanelAnimate ( animObj );
            } else {
                StarFinished ();
            }
        }

        /// <summary>
        /// Splashs the finished.
        /// </summary>
        public void StarFinished() {
            SceneHandleManager.NextSceneRedirect (CommonConstants.MYPAGE_SCENE);
        }
        #endregion

        /// <summary>
        /// Waits the next scene.
        /// </summary>
        /// <returns>The next scene.</returns>
        private IEnumerator WaitNextScene()
        {
            yield break;
        }
        
        #region 都道府県設定（出身地設定）
        /// <summary>
        /// Places the of origin.
        /// 都道府県パネルのオープン
        /// </summary>
        public void PlaceOfOriginOpen (GameObject animObj)
        {
            HeaderPanel.Instance.BackButtonSwitch (true, PlaceOfOriginCloseEvent);
            _currentProfSettingState = CurrentProfSettingStateType.Pref;
            _profTemplatefInfiniteLimitScroll.Init ();
            _backSwipe.EventMessageTarget = _panelProfileSetTemplate;
            PanelAnimate (animObj);
        }

        /// <summary>
        /// Places the of origin close.
        /// 都道府県パネルのクローズ
        /// </summary>
        /// <param name="animObj">Animation object.</param>
        public void PlaceOfOriginClose (GameObject animObj)
        {
            HeaderPanel.Instance.BackButtonSwitch (false, PlaceOfOriginCloseEvent);
            BackButton (animObj);
            //クローズしたらInstantiateオブジェクトを一旦リセット
            CleanTemplate();
        }

        /// <summary>
        /// Places the of origin close event.
        /// Unity イベント用に設定。
        /// </summary>
        void PlaceOfOriginCloseEvent()
        {
            PlaceOfOriginClose (_panelProfileSetTemplate);
        }
        #endregion

        #region 市区町村設定（出身地詳細）
        /// <summary>
        /// Places the of origin.
        /// 市区町村パネルのオープン
        /// </summary>
        public void PlaceOfOriginDetailOpen (GameObject animObj)
        {
            HeaderPanel.Instance.BackButtonSwitch (true,  PlaceOfOriginDetailCloseEvent);
            _currentProfSettingState = CurrentProfSettingStateType.City;
            _profTemplatefInfiniteLimitScroll.Init ();
            _backSwipe.EventMessageTarget = _panelProfileSetTemplate;
            PanelAnimate (animObj);
        }

        /// <summary>
        /// Places the of origin detail close.
        /// 市区町村パネルのクローズ
        /// </summary>
        public void PlaceOfOriginDetailClose (GameObject animObj)
        {
            HeaderPanel.Instance.BackButtonSwitch (false, PlaceOfOriginDetailCloseEvent);
            BackButton (animObj);
            //クローズしたらInstantiateオブジェクトを一旦リセット
            CleanTemplate ();
        }

        /// <summary>
        /// Places the of origin detail close event.
        /// Unity イベント用に設定
        /// </summary>
        void PlaceOfOriginDetailCloseEvent ()
        {
            PlaceOfOriginDetailClose (_panelProfileSetTemplate);
        }
        #endregion
        
 
        /// <summary>
        /// Backs the button.
        /// </summary>
        /// <param name="fromObj">From object.</param>
        /// <param name="toObj">To object.</param>
        public void BackButton (GameObject fromObj) 
        {
            fromObj.GetComponent<uTweenPosition> ().delay    = 0.001f;
            fromObj.GetComponent<uTweenPosition> ().duration = 0.25f;
            fromObj.GetComponent<uTweenPosition> ().to      = fromObj.transform.GetComponent<uTweenPosition> ().from;
            fromObj.GetComponent<uTweenPosition> ().from    = Vector3.zero;
            fromObj.GetComponent<uTweenPosition> ().ResetToBeginning ();
            fromObj.GetComponent<uTweenPosition> ().enabled = true;
        }

        /// <summary>
        /// Res the panel animate.
        /// </summary>
        /// <param name="target">Target.</param>
        public void PanelAnimate ( GameObject target )
        {
            if (target.GetComponent<uTweenPosition> ().from.x == 0) {
                target.GetComponent<uTweenPosition> ().from = target.GetComponent<uTweenPosition> ().to;
            }

            target.GetComponent<uTweenPosition> ().to = Vector3.zero;
            target.GetComponent<uTweenPosition> ().delay = 0.1f;
            target.GetComponent<uTweenPosition> ().duration = 0.2f;
            target.GetComponent<uTweenPosition> ().ResetToBeginning ();
            target.GetComponent<uTweenPosition> ().enabled = true;
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
            target.GetComponent<uTweenScale> ().to   = new Vector3 (1, 1 ,1 );
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
        
        /// <summary>
        /// Cleans the template.
        /// サーバーから引いてきた列挙しているデータをクリーンにする。
        /// </summary>
        private void CleanTemplate ()
        {
            _scrollContent.GetComponent<RectTransform> ().anchoredPosition = Vector2.zero;

            for (int i = 0; i < _scrollContent.childCount; i++) 
            {
                if (_scrollContent.GetChild (0).name  != _scrollContent.GetChild (i).name) {
                    Destroy (_scrollContent.GetChild (i).gameObject);
                }
            }
        }

        /// <summary>
        /// Webs the view terms switch.
        /// </summary>
        /// <param name="isSWitch">If set to <c>true</c> is S witch.</param>
        public GameObject WebViewTermsSwitch (bool isSWitch)
        {
            _webviewTerms.SetActive (isSWitch);
            return _webviewTerms;
        }
    }
}