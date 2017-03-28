using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using EventManager;
using UnityEngine.UI;
using ModelManager;
using Http;
using Helper;

namespace ViewController
{
    public class PanelProfileChange : SingletonMonoBehaviour<PanelProfileChange>
    {
        #region SerializeField Method
        public RawImage _profImage;
        public RawImage _coverImage;

        [SerializeField]
        private Text _gender;

        [SerializeField]
        private Transform _nickName; //input field.

        [SerializeField]
		private Text _tall;
        
        [SerializeField]
        private Slider _tallSlider; //Slider.

        [SerializeField]
		private Text _weight;
        
        [SerializeField]
        private Slider _weightSlider; //Slider

        [SerializeField]
        private Text _bloodType;

        [SerializeField]
        private Text _pref;

        [SerializeField]
        private Text _city;

        [SerializeField]
        private Text _profile;

        [SerializeField]
        private Text _hairStyle;

        [SerializeField]
        private Text _bodyType;

        [SerializeField]
        private Text _glasses;

        [SerializeField]
        private Text _type;

        [SerializeField]
        private Text _personality;

        [SerializeField]
        private Text _holiday;

        [SerializeField]
        private Text _annualIncome;

        [SerializeField]
        private Text _education;

        [SerializeField]
        private Text _housemate;

        [SerializeField]
        private Text _sibling;

        [SerializeField]
        private Text _alcohol;

        [SerializeField]
        private Text _tobacco;

        [SerializeField]
        private Text _car;

        [SerializeField]
        private Text _pet;

        [SerializeField]
        private Text _hobby;

        [SerializeField]
        private Text _interest;

        [SerializeField]
        private Text _marital;

        [SerializeField]
        private Dropdown _dropdownYear;

        [SerializeField]
        private Dropdown _dropdownMonth;

        [SerializeField]
        private Dropdown _dropdownDays;

        [SerializeField]
        private GameObject _birthdayObj;

        [SerializeField]
        private GameObject _birthdayObjNative;

        public ProfTemplateInfiniteLimitScroll _profTemplatefInfiniteLimitScroll;

        public GameObject _profileInput;

        private bool _isProfileChangeState = false;
        #endregion
        
        
        #region Memeber Variable
        public List <string> _birthYear  = new List <string> ();
        public List <string> _birthMonth = new List <string> ();
        public List <string> _birthDays  = new List <string> ();
        #endregion
        
        #region ピッカー用の処理。
        /// <summary>
        /// The draw rect.
        /// </summary>
        private Rect _drawRect;

        /// <summary>
        /// Tos the screen rect.
        /// </summary>
        /// <returns>The screen rect.</returns>
        /// <param name="rect">Rect.</param>
        Rect toScreenRect(Rect rect) {
            Vector2 lt = new Vector2(rect.x, rect.y);
            Vector2 br = lt + new Vector2(rect.width, rect.height);

            lt = GUIUtility.GUIToScreenPoint(lt);
            br = GUIUtility.GUIToScreenPoint(br);

            return new Rect(lt.x, lt.y, br.x-lt.x, br.y-lt.y);
        }
        #endregion

        
        /// <summary>
        /// Init this instance.
        /// サーバー等から引いてきたデータを初期化セット
        /// </summary>
        public void Init ()
        {
            _isProfileChangeState = false;
            //外部パネルから使用するシリアライズ変数をセット。
            PanelProfileSetTemplate.Instance.SetInfiniteLimitScroll ();
            PanelProfileInput.Instance.SetProfInput ();

            //初期処理。
            PanelStartInputUserData.Instance._currentProfSettingState = CurrentProfSettingStateType.None;
            MypageEventManager.Instance._currentProfSettingState      = CurrentProfSettingStateType.None;

             //生年月日のセット処理用 ------------------------------------- ここから　
             #if UNITY_EDITOR
             _birthdayObj.SetActive (true);
            _birthdayObjNative.SetActive (false);
            _dropdownYear.ClearOptions ();
            _dropdownYear.options.Clear ();

            _dropdownMonth.ClearOptions ();
            _dropdownMonth.options.Clear ();

            _dropdownDays.ClearOptions ();
            _dropdownDays.options.Clear ();
                
            //Year「年」設定 //マジックナンバーなんとかする
            //_birthYear.Add ("");
            for (int i = (System.DateTime.Now.Year - 18); i >= 1930; i--) {
                _birthYear.Add (i.ToString());
            }
    
            //Month「月」設定
            //_birthMonth.Add ("");
            for (int i = 1; i <= 12; i++) {
                _birthMonth.Add (i.ToString());
            }
            //Day「日」設定
            //_birthDays.Add ("");
            for (int i = 1; i <= 31; i++) {
                _birthDays.Add (i.ToString());
            }

            if (_dropdownYear != null && _birthYear != null) {
                _dropdownYear.AddOptions (_birthYear);
            }

            if (_dropdownMonth != null && _birthMonth != null) {
                _dropdownMonth.AddOptions (_birthMonth);
            }

            if (_dropdownDays != null && _birthDays != null) {
                _dropdownDays.AddOptions (_birthDays);
            }

            if (GetUserApi._httpCatchData != null && string.IsNullOrEmpty (GetUserApi._httpCatchData.result.user.birth_date) == false) {
                string[] dateSplit = GetUserApi._httpCatchData.result.user.birth_date.Split ('-');
                _dropdownYear.captionText.text  = dateSplit[0];
                _dropdownMonth.captionText.text = dateSplit[1];
                _dropdownDays.captionText.text  = dateSplit[2];
            }
            
            
            #elif UNITY_IOS || UNITY_ANDROID
                _birthdayObj.SetActive (false);
                _birthdayObjNative.SetActive (true);
    
                float width  = Screen.width/2;
                float height = Screen.width / 14;
                _drawRect = new Rect((Screen.width - width)/2, height, width, height);
            #endif
            //生年月日のセット処理用 ------------------------------------- ここまで

            if (GetUserApi._httpCatchData.result.user.cover_image_url != null && string.IsNullOrEmpty (GetUserApi._httpCatchData.result.user.cover_image_url) == false) {
                if (GetUserApi._httpCatchData.result.user.cover_image_status == "1") {
                    _coverImage.texture = Resources.Load ("Texture/check_image_cover@2x") as Texture;
                } else {
                    StartCoroutine (WwwToRendering (GetUserApi._httpCatchData.result.user.cover_image_url, _coverImage));
                }
            }

            if (_profImage != null && string.IsNullOrEmpty (GetUserApi._httpCatchData.result.user.profile_image_url) == false) {
                if (GetUserApi._httpCatchData.result.user.profile_image_status == "1"){
                    _profImage.texture = Resources.Load ("Texture/check_image_user@2x") as Texture;
                } else {
                    StartCoroutine (WwwToRendering (GetUserApi._httpCatchData.result.user.profile_image_url , _profImage));
                }
            }
                
            SetData ();
        }

        /// <summary>
        /// Errors the popup close.
        /// </summary>
        void ErrorPopupClose() {
            MypageEventManager.Instance.PanelPopupCloseAnimate (PopupPanel.Instance.gameObject);
        }

        #region 外部用メソッド
        /// <summary>
        /// Sets the data.
        /// マスターデータから動的に値が変更する所用
        /// </summary>
        public void SetData ()
        {
            string prefId      = MypageEventManager.Instance._prefId;
            string cityId      = MypageEventManager.Instance._cityId;
            string bloodTypeId = MypageEventManager.Instance._bloodType;
            string birthDate   = "";

            #if UNITY_EDITOR
			//生年月日
			if (_dropdownYear.value != 0 && _dropdownMonth.value != 0 && _dropdownDays.value != 0) 
			{
			    birthDate = _birthYear [_dropdownYear.value] + "-" + _birthMonth [_dropdownMonth.value] + "-" + _birthDays [_dropdownDays.value];
			} else if (_dropdownYear.GetComponentInChildren<Text>().text != null && 
           		_dropdownMonth.GetComponentInChildren<Text>().text != null && 
           		_dropdownDays.GetComponentInChildren<Text>().text != null
			){
    			birthDate = _dropdownYear.GetComponentInChildren<Text>().text + "-" + _dropdownMonth.GetComponentInChildren<Text>().text + "-" + _dropdownDays.GetComponentInChildren<Text>().text;
			}

#elif !UNITY_EDITOR && UNITY_IOS || UNITY_ANDROID
            //IOSとアンドロイドの場合の処理。
            if (string.IsNullOrEmpty (MypageEventManager.Instance._birthDate) == false ) {
                birthDate = MypageEventManager.Instance._birthDate;
                string[] bdArray = MypageEventManager.Instance._birthDate.Split('-');
                _birthdayObjNative.transform.GetChild (0).gameObject.SetActive (false);
                _birthdayObjNative.transform.GetChild (1).GetComponent<Text> ().text = string.Format ("{0}年{1}月{2}日", bdArray[0], bdArray[1], bdArray[2]);
            }
#endif
            

            //プロフ更新APIに送信するデータをセット。
            //データをセットする箇所
            MypageEventManager.Instance._userDataBasic.user_key = AppStartLoadBalanceManager._userKey;
            //性別は規定、変えれない。
            MypageEventManager.Instance._userDataBasic.sex_cd   = AppStartLoadBalanceManager._gender;

            MypageEventManager.Instance._userDataBasic.name       = _nickName.GetComponent<InputField> ().text;
            MypageEventManager.Instance._userDataBasic.birth_date = birthDate;
            MypageEventManager.Instance._userDataBasic.height     = _tall.text;
            MypageEventManager.Instance._userDataBasic.weight     = _weight.text;
            MypageEventManager.Instance._userDataBasic.pref       = prefId;

            if (MypageEventManager.Instance._prefChange == true) {
                MypageEventManager.Instance._userDataBasic.city_id = "";
            } else {
                MypageEventManager.Instance._userDataBasic.city_id = cityId;
            }
            
            MypageEventManager.Instance._userDataBasic.blood_type = bloodTypeId;
            MypageEventManager.Instance._userDataBasic.profile    = MypageEventManager.Instance._profile;
            MypageEventManager.Instance._userDataBasic.hair_style = MypageEventManager.Instance._hairStyle;
            MypageEventManager.Instance._userDataBasic.body_type  = MypageEventManager.Instance._bodyType;
            MypageEventManager.Instance._userDataBasic.glasses    = MypageEventManager.Instance._glasses;
            MypageEventManager.Instance._userDataBasic.type       = MypageEventManager.Instance._type;        //TODO: 複数選択可・対応
            MypageEventManager.Instance._userDataBasic.personality = MypageEventManager.Instance._personality;//TODO: 複数選択可・対応
            MypageEventManager.Instance._userDataBasic.holiday     = MypageEventManager.Instance._holiday;
            MypageEventManager.Instance._userDataBasic.annual_income = MypageEventManager.Instance._annualIncome;
            MypageEventManager.Instance._userDataBasic.education = MypageEventManager.Instance._education;
            MypageEventManager.Instance._userDataBasic.housemate = MypageEventManager.Instance._housemate;
            MypageEventManager.Instance._userDataBasic.hair_style = MypageEventManager.Instance._hairStyle;
            MypageEventManager.Instance._userDataBasic.sibling = MypageEventManager.Instance._sibling;
            MypageEventManager.Instance._userDataBasic.alcohol = MypageEventManager.Instance._alcohol;
            MypageEventManager.Instance._userDataBasic.tobacco = MypageEventManager.Instance._tobacco;
            MypageEventManager.Instance._userDataBasic.car = MypageEventManager.Instance._car;
            MypageEventManager.Instance._userDataBasic.pet = MypageEventManager.Instance._pet;
            MypageEventManager.Instance._userDataBasic.hobby = MypageEventManager.Instance._hobby;
            MypageEventManager.Instance._userDataBasic.interest = MypageEventManager.Instance._interest;
            MypageEventManager.Instance._userDataBasic.marital = MypageEventManager.Instance._marital;

            //選択、入力した項目をヴューにセットするデータ。
            if (string.IsNullOrEmpty (prefId)  == false) 
                _pref.text      = CommonModelHandle.GetPrefDataById (prefId).name;

            if (MypageEventManager.Instance._prefChange == true) {
                _city.text = "";
                MypageEventManager.Instance._prefChange = false;
            } else if (string.IsNullOrEmpty (cityId)  == false) {
                _city.text = CommonModelHandle.GetCityDataById (cityId).name;
            }

            if (MypageEventManager.Instance._hairStyle != null && MypageEventManager.Instance._hairStyle.Count > 0) 
                _hairStyle.text = MypageEventManager.Instance._hairStyle[0];
                
             if (MypageEventManager.Instance._bodyType != null && MypageEventManager.Instance._bodyType.Count > 0) 
                _bodyType.text  = MypageEventManager.Instance._bodyType[0];
                
             if (MypageEventManager.Instance._glasses != null && MypageEventManager.Instance._glasses.Count > 0) 
                _glasses.text   = MypageEventManager.Instance._glasses[0];
                
             if (MypageEventManager.Instance._type != null && MypageEventManager.Instance._type.Count > 0) 
                _type.text      = MypageEventManager.Instance._type[0];//TODO: 複数選択可・対応
                
             if (MypageEventManager.Instance._personality != null && MypageEventManager.Instance._personality.Count > 0) 
                _personality.text  = MypageEventManager.Instance._personality[0];//TODO: 複数選択可・対応
                
            if (MypageEventManager.Instance._holiday != null && MypageEventManager.Instance._holiday.Count > 0) 
                _holiday.text      = MypageEventManager.Instance._holiday[0];
                
            if (MypageEventManager.Instance._annualIncome != null && MypageEventManager.Instance._annualIncome.Count > 0) 
                _annualIncome.text = MypageEventManager.Instance._annualIncome[0];
                
            if (MypageEventManager.Instance._education != null && MypageEventManager.Instance._education.Count > 0) 
                _education.text = MypageEventManager.Instance._education[0];
                
            if (MypageEventManager.Instance._housemate != null && MypageEventManager.Instance._housemate.Count > 0) 
                _housemate.text = MypageEventManager.Instance._housemate[0];
                
            if (MypageEventManager.Instance._sibling != null && MypageEventManager.Instance._sibling.Count > 0) 
                _sibling.text   = MypageEventManager.Instance._sibling[0];
                
            if (MypageEventManager.Instance._alcohol != null && MypageEventManager.Instance._alcohol.Count > 0) 
                _alcohol.text   = MypageEventManager.Instance._alcohol[0];
                
            if (MypageEventManager.Instance._tobacco != null && MypageEventManager.Instance._tobacco.Count > 0) 
                _tobacco.text   = MypageEventManager.Instance._tobacco[0];
                
            if (MypageEventManager.Instance._car != null && MypageEventManager.Instance._car.Count > 0) 
                _car.text       = MypageEventManager.Instance._car[0];
                
            if (MypageEventManager.Instance._pet != null && MypageEventManager.Instance._pet.Count > 0)
                _pet.text       = MypageEventManager.Instance._pet[0];
                
            if (MypageEventManager.Instance._hobby != null && MypageEventManager.Instance._hobby.Count > 0)
                _hobby.text     = MypageEventManager.Instance._hobby[0];
                
            if (MypageEventManager.Instance._interest != null && MypageEventManager.Instance._interest.Count > 0)
                _interest.text  = MypageEventManager.Instance._interest[0];
                
            if (MypageEventManager.Instance._marital != null && MypageEventManager.Instance._marital.Count > 0)
                _marital.text   = MypageEventManager.Instance._marital[0];
            
            _nickName.GetComponent<InputField> ().text = MypageEventManager.Instance._nickName;

            if (MypageEventManager.Instance._cpsTypeSliderWeight == CurrentProfSettingStateType.None) {
                _weight.text = MypageEventManager.Instance._weight;
                _weightSlider.value = float.Parse(MypageEventManager.Instance._weight);
            }

            if (MypageEventManager.Instance._cpsTypeSliderHeight == CurrentProfSettingStateType.None) {
                _tall.text = MypageEventManager.Instance._tall;
                _tallSlider.value = float.Parse(MypageEventManager.Instance._tall);            
            }

            
            if (AppStartLoadBalanceManager._gender  == ((int)GenderType.FeMale).ToString())
			{
                _gender.text = LocalMsgConst.GENDER_FEMALE;
            } else if (AppStartLoadBalanceManager._gender  == ((int)GenderType.Male).ToString()) {
                _gender.text = LocalMsgConst.GENDER_MALE;
            }
            
            if (MypageEventManager.Instance._profile.Length == 0) {
                _profile.text = "未入力";
            } else if (MypageEventManager.Instance._profile.Length >= 20){
                _profile.text = MypageEventManager.Instance._profile.Substring(0,20) + "…";
            } else {
                _profile.text = MypageEventManager.Instance._profile.Substring(0, MypageEventManager.Instance._profile.Length) + "…";
            }
            _profileInput.GetComponent<InputField> ().text = MypageEventManager.Instance._profile;
            if (string.IsNullOrEmpty(bloodTypeId) == false) {
                string bloodTypeMsg = CommonModelHandle.GetByIdBaseData(bloodTypeId, CurrentProfSettingStateType.BloodType).name;
               _bloodType.text = bloodTypeMsg + LocalMsgConst.BLOOD_TYPE_JA;
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
                if (gesture.Direction == FingerGestures.SwipeDirection.Right) {
                    if (MypageEventManager.Instance != null) {
                        if (_isProfileChangeState == true)
                        {
                            MypageEventManager.Instance.PanelPopupAnimate (PopupSecondSelectPanel.Instance.gameObject);
            
                            PopupSecondSelectPanel.Instance.PopClean ();
                            PopupSecondSelectPanel.Instance.PopMessageInsert (
                                LocalMsgConst.PROFILE_CHANGE_ANNOUNCE,
                                LocalMsgConst.YES,
                                LocalMsgConst.NO,
                                SaveEvent,
                                SaveCancelEvent
                            );
                            _isProfileChangeState = false;
                            return;
                        }
                    }
                    
                    if (MypageEventManager.Instance != null) 
                        MypageEventManager.Instance.ProfileChangeClose (this.gameObject);
                }
            }
        }
        
        /// <summary>
        /// Saves the event.
        /// </summary>
        /// <returns>The event.</returns>
        void SaveEvent() {
            StartCoroutine (SaveEventCoroutine ());
        }

        /// <summary>
        /// Saves the event coroutine.
        /// </summary>
        /// <returns>The event coroutine.</returns>
        private IEnumerator SaveEventCoroutine() 
        {
            PanelProfileChange.Instance.SetData ();
            yield return StartCoroutine (ProfileUpdateApiCoroutine ());

            PopupSecondSelectPanel.Instance.PopClean ();
            MypageEventManager.Instance.PanelPopupCloseAnimate (PopupSecondSelectPanel.Instance.gameObject);
        }
        
        /// <summary>
        /// Saves the cancel event.
        /// </summary>
        /// <returns>The cancel event.</returns>
        void SaveCancelEvent () {
            PopupSecondSelectPanel.Instance.PopClean ();
            MypageEventManager.Instance.PanelPopupCloseAnimate (PopupSecondSelectPanel.Instance.gameObject);
            //MypageEventManager.Instance.ProfileChangeClose (this.gameObject);
        }
        #endregion


        #region ボタンMethod群
        /// <summary>
        /// Panels the edit open.
        /// </summary>
        /// <param name="animObj">Animation object.</param>
        public void PanelEditOpen (string state)
        {
            CurrentProfSettingStateType sType = (CurrentProfSettingStateType)System.Enum.Parse(typeof(CurrentProfSettingStateType), state);
            #if !UNITY_EDITOR && (UNITY_IOS || UNITY_ANDROID)
                NativePickerSet (sType);
            #elif UNITY_EDITOR
            HeaderPanel.Instance.BackButtonSwitch (false);
            HeaderPanel.Instance.BackButtonSwitch (true, EditCloseEvent);
            GameObject.FindGameObjectWithTag (CommonConstants.BACK_SWIPE).GetComponent<SwipeRecognizer>().EventMessageTarget = PanelProfileSetTemplate.Instance.gameObject;
            this.GetComponent<BoxCollider2D> ().enabled = false;
            
            MypageEventManager.Instance._currentProfSettingState = sType;
            _profTemplatefInfiniteLimitScroll.Init ();

            MypageEventManager.Instance.PanelAnimate (PanelProfileSetTemplate.Instance.gameObject);
            #endif
        }

        /// <summary>
        /// Bloods the type close.
        /// セットテンプレートの閉じる処理。
        /// </summary>
        /// <param name="animObj">Animation object.</param>
        public void PanelEditClose ()
        {
            HeaderPanel.Instance.BackButtonSwitch (false);
            HeaderPanel.Instance.BackButtonSwitch (true, MypageEventManager.Instance.ProfileChangeEvent);
            GameObject.FindGameObjectWithTag (CommonConstants.BACK_SWIPE).GetComponent<SwipeRecognizer>().EventMessageTarget = this.gameObject;
            this.GetComponent<BoxCollider2D> ().enabled = true;
            MypageEventManager.Instance.BackButton (PanelProfileSetTemplate.Instance.gameObject);
            //クローズしたらInstantiateオブジェクトを一旦リセット
            MypageEventManager.Instance.CleanTemplate ();
            StartCoroutine (MypageEventManager.Instance.BackAnimWait (PanelProfileSetTemplate.Instance.gameObject));
        }

        /// <summary>
        /// Edits the close event.
        /// </summary>
        public void EditCloseEvent () {
            PanelEditClose ();
        }

        /// <summary>
        /// Places the of origin.
        /// 自己紹介のオープン
        /// </summary>
        public void PanelProfileInputOpen ()
        {
            HeaderPanel.Instance.BackButtonSwitch (false);
            HeaderPanel.Instance.BackButtonSwitch (true,  PanelProfileInputCloseEvent);
            this.GetComponent<BoxCollider2D> ().enabled = false;
            MypageEventManager.Instance._currentProfSettingState = CurrentProfSettingStateType.Profile;

            GameObject.FindGameObjectWithTag (CommonConstants.BACK_SWIPE).GetComponent<SwipeRecognizer>().EventMessageTarget =  PanelProfileInput.Instance.gameObject;
            MypageEventManager.Instance.PanelAnimate (PanelProfileInput.Instance.gameObject);
        }

        /// <summary>
        /// Places the of origin detail close.
        /// 自己紹介のクローズ
        /// </summary>
        public void PanelProfileInputClose ()
        {
            //プロフィールの変更がある事を通知する用
            _isProfileChangeState = true; 

            HeaderPanel.Instance.BackButtonSwitch (false);
            HeaderPanel.Instance.BackButtonSwitch (true , MypageEventManager.Instance.ProfileChangeEvent);
            this.GetComponent<BoxCollider2D> ().enabled = true;

            MypageEventManager.Instance.BackButton (PanelProfileInput.Instance.gameObject);
            StartCoroutine (MypageEventManager.Instance.BackAnimWait (PanelProfileInput.Instance.gameObject));
        }

        /// <summary>
        /// Places the of origin detail close event.
        /// Unity イベント用に設定
        /// </summary>
        void PanelProfileInputCloseEvent ()
        {
            PanelProfileInputClose ();
        }

        /// <summary>
        /// Images the cover click.
        /// </summary>
        public void ImageCoverButton () 
        {
            MypageEventManager.Instance.CoverOrProfSet ("Cover");
            CameraOrGallery.Instance.OpenCameraOrGallerySet (_coverImage.gameObject);
            OpenGalleryPlugin.Instance.OnClickOpenGalleryCameraSet (_coverImage.gameObject);
            CameraOrGallery.Instance.CameraOrGalleryPopOpen (CameraOrGallery.Instance.gameObject);
        }

        /// ------------------------------------------------------------------------------------------
        /// <summary>
        /// Images the prof click.
        /// </summary>
        public void ImageProfButton () 
        {
            MypageEventManager.Instance.CoverOrProfSet ("Prof");
            CameraOrGallery.Instance.OpenCameraOrGallerySet (_profImage.gameObject);
            OpenGalleryPlugin.Instance.OnClickOpenGalleryCameraSet (_profImage.gameObject);
            CameraOrGallery.Instance.CameraOrGalleryPopOpen (CameraOrGallery.Instance.gameObject);
        }
        /// ------------------------------------------------------------------------------------------
        #endregion



        #region 自己紹介の例文のOpen, Close
        public void ExampleProfileBodyOpen () 
        {
            //PanelPopupAnimate (ExampleProfileBody);
        }

        /// <summary>
        /// Examples the profile body open.
        /// </summary>
        public void ExampleProfileBodyClose () 
        {
           //PanelPopupCloseAnimate (_exampleProfileBody);
        }
        #endregion


        /// <summary>
        /// Wwws to rendering.
        /// </summary>
        /// <returns>The to rendering.</returns>
        /// <param name="url">URL.</param>
        /// <param name="targetObj">Target object.</param>
        private IEnumerator WwwToRendering (string url, RawImage targetObj)
        {
            targetObj.texture = null;
            targetObj.gameObject.SetActive (false);
            if (string.IsNullOrEmpty (url) == true) yield break;
            using (WWW www = new WWW (url)) {
                while (www == null)
                    yield return (www != null);

                while (www.isDone == false)
                    yield return (www.isDone);
    
                //non texture file
                if (string.IsNullOrEmpty (www.error) == false)
                    yield break;

                while (targetObj == null)
                    yield return (targetObj != null);

                targetObj.gameObject.SetActive (true);
                targetObj.texture = www.texture;
            }
        }

        /// <summary>
        /// Settings the save.
        /// プロフィール設定画面 - Api送信。
        /// </summary>
        public void ProfSettingSaveButton ()
        {
            SetData ();
            
            //年齢入力チェック
            bool isNgAge = AgeCheck (GetDateTimeByAge (18), MypageEventManager.Instance._birthDate);

            if (isNgAge == true)
            {
                // 設定しました　ポップアップ
                PopupPanel.Instance.PopMessageInsert (
                    "<color=red>生年月日に誤りがあります。</color>",
                    LocalMsgConst.OK,
                    SaveProfileConfig
                );
                MypageEventManager.Instance.PanelPopupAnimate (GameObject.FindGameObjectWithTag(CommonConstants.POPUP_BASIC_TAG));
                return;
            }
            StartCoroutine (ProfileUpdateApiCoroutine ());
        }
        
        /// <summary>
        /// Ages the check.
        /// </summary>
        /// <returns><c>true</c> if check was aged <c>false</c> otherwise.<//>
        /// <param name="limitDate">Limit date.</param>
        /// <param name="setBirthday">Set birthday.</param>
        private bool AgeCheck (System.DateTime limitDate, string setBirthday)
        {
            bool ngAge = false;
            if (string.IsNullOrEmpty (setBirthday) == false) {
                string [] setBirthdaySplit = setBirthday.Split ('-');

                // 任意の時刻
                System.DateTime setBirthdayTime = new System.DateTime (int.Parse (setBirthdaySplit [0]), int.Parse (setBirthdaySplit [1]), int.Parse (setBirthdaySplit [2]), 0, 0, 0);
                if (setBirthdayTime > limitDate) {
                    ngAge = true;
                }
            }
            return ngAge;
        }

        /// <summary>
        /// Profiles the update API.
        /// </summary>
        /// <returns>The update API.</returns>
        private IEnumerator ProfileUpdateApiCoroutine ()
        {
            //ローディングアニメーションOn
            MypageEventManager.Instance.LoadingSwitch (true);
           
            new ProfileUpdateApi (MypageEventManager.Instance._userDataBasic);
            //APIのポーリング
            while (ProfileUpdateApi._success == false)
                yield return (ProfileUpdateApi._success == true);

            MypageEventManager.Instance.DataInit ();
            SetData ();
            //ローディングアニメーションOff

            yield return StartCoroutine(MypageEventManager.Instance.Initialize());

            // 設定しました　ポップアップ
            PopupPanel.Instance.PopMessageInsert (
                LocalMsgConst.SAVE_PROFILE_CONFIG,
                LocalMsgConst.OK,
                SaveProfileConfig
            );
            MypageEventManager.Instance.PanelPopupAnimate (GameObject.FindGameObjectWithTag(CommonConstants.POPUP_BASIC_TAG));
            MypageEventManager.Instance.LoadingSwitch (false);
        }

        void SaveProfileConfig () 
        {
            _isProfileChangeState = false;
            PopupPanel.Instance.PopClean();

            MypageEventManager.Instance.PanelPopupCloseAnimate (GameObject.FindGameObjectWithTag(CommonConstants.POPUP_BASIC_TAG));
        }

        /// <summary>
        /// Natives the picker set.
        /// </summary>
        /// <returns>The picker set.</returns>
        /// <param name="stateType">State type.</param>
        private void NativePickerSet(CurrentProfSettingStateType stateType)
        {
            string [] itemList = {""};
            List<string> list = new List<string> ();
            
            #if UNITY_IOS
            list.Add ("");
            #endif
            
            
            switch (stateType)
            {
            case CurrentProfSettingStateType.Pref:
                var pref = CommonModelHandle.GetPrefData ();
                foreach (var p in pref) {
                    list.Add (p.Value.name);
                }

                //都道府県選択のスキームに入るから市区町村のデータをリセット。
                MypageEventManager.Instance._cityId = "";
                _city.text = "";
                
                break;
            case CurrentProfSettingStateType.City:
                var city = CommonModelHandle.GetCityData (MypageEventManager.Instance._prefId);
                foreach (var c in city) {
                    list.Add (c.Value.name);
                }
                break;
            case CurrentProfSettingStateType.BloodType:
               foreach (var b in InitDataApi._httpCatchData.result.blood_type) {
                    list.Add (b.name);
               }
               break;
            case CurrentProfSettingStateType.Profile:
                
                break;
            case CurrentProfSettingStateType.HairStyle:
            case CurrentProfSettingStateType.BodyType:
            case CurrentProfSettingStateType.Glasses:
            case CurrentProfSettingStateType.Type:
            case CurrentProfSettingStateType.Personality:
            case CurrentProfSettingStateType.Holiday:
            case CurrentProfSettingStateType.AnnualIncome:
            case CurrentProfSettingStateType.Education:
            case CurrentProfSettingStateType.Housemate:
            case CurrentProfSettingStateType.Sibling:
            case CurrentProfSettingStateType.Alcohol:
            case CurrentProfSettingStateType.Tobacco:
            case CurrentProfSettingStateType.Car:
            case CurrentProfSettingStateType.Pet:
            case CurrentProfSettingStateType.Hobby:
            case CurrentProfSettingStateType.Interest:
            case CurrentProfSettingStateType.Marital:
                var item = CommonModelHandle.GetNameMaster (AppStartLoadBalanceManager._gender, stateType);
                foreach ( var i in item) {
                    list.Add (i.name);
                }
                break;
            }
            
            if (list.Count > 0) {
                itemList = list.ToArray ();

                NativePicker.Instance.ShowCustomPicker (toScreenRect (_drawRect), itemList, 0, (long val) => {
                    //プロフィールの変更がある事を通知する用
                    _isProfileChangeState = true; 
                    for (int i = 0; i < list.Count; i++ ) {
                       if ((int)val == i) {
                            DisplayDataSet (stateType, i, list[i]);
                            break;
                       }
                    }
                }, () => {
                 //ピッカーをキャンセルにした場合。");
                });
            }
        }
        
        /// <summary>
        /// Displaies the data set.
        /// </summary>
        /// <returns>The data set.</returns>
        /// <param name="stateType">State type.</param>
        /// <param name="setData">Set data.</param>
        private void DisplayDataSet (CurrentProfSettingStateType stateType, int key, string value) {
            switch (stateType) {
            case CurrentProfSettingStateType.Pref:
                _pref.text =  value;
            #if UNITY_IOS
            MypageEventManager.Instance._prefId = (key).ToString ();
            #else
            MypageEventManager.Instance._prefId = (key + 1).ToString ();
            #endif
                
                break;
            case CurrentProfSettingStateType.City:
                var city = CommonModelHandle.GetCityDataByName (value);
                _city.text = value;
                MypageEventManager.Instance._cityId = city.id;
                break;
            case CurrentProfSettingStateType.BloodType:
                _bloodType.text = value + LocalMsgConst.BLOOD_TYPE_JA;
#if UNITY_IOS
MypageEventManager.Instance._bloodType = (key).ToString();
#else
MypageEventManager.Instance._bloodType = (key+1).ToString();
#endif
                break;
            case CurrentProfSettingStateType.HairStyle:
                _hairStyle.text = value;
                MypageEventManager.Instance._hairStyle[0] = value;
                break;
            case CurrentProfSettingStateType.BodyType:
                _bodyType.text = value;
                MypageEventManager.Instance._bodyType [0] = value;
                break;
            case CurrentProfSettingStateType.Glasses:
                _glasses.text = value;
                MypageEventManager.Instance._glasses [0] = value;
                break;
            case CurrentProfSettingStateType.Type:
                _type.text = value;
                MypageEventManager.Instance._type [0] = value;
                break;
            case CurrentProfSettingStateType.Personality:
                _personality.text = value;
                MypageEventManager.Instance._personality [0] = value;
                break;
            case CurrentProfSettingStateType.Holiday:
                _holiday.text = value;
                MypageEventManager.Instance._holiday [0] = value;
                break;
             case CurrentProfSettingStateType.AnnualIncome:
                _annualIncome.text = value;
                MypageEventManager.Instance._annualIncome [0] = value;
                break;
             case CurrentProfSettingStateType.Education:
                _education.text = value;
                MypageEventManager.Instance._education [0] = value;
                break;
             case CurrentProfSettingStateType.Housemate:
                _housemate.text = value;
                MypageEventManager.Instance._housemate [0] = value;
                break;
             case CurrentProfSettingStateType.Sibling:
                _sibling.text = value;
                MypageEventManager.Instance._sibling [0] = value;
                break;
             case CurrentProfSettingStateType.Alcohol:
                _alcohol.text = value;
                MypageEventManager.Instance._alcohol [0] = value;
                break;
             case CurrentProfSettingStateType.Tobacco:
                _tobacco.text = value;
                MypageEventManager.Instance._tobacco [0] = value;
                break;
             case CurrentProfSettingStateType.Car:
                _car.text = value;
                MypageEventManager.Instance._car[0] = value;
                break;
             case CurrentProfSettingStateType.Pet:
                _pet.text = value;
                MypageEventManager.Instance._pet [0] = value;
                break;
             case CurrentProfSettingStateType.Hobby:
                _hobby.text = value;
                MypageEventManager.Instance._hobby [0] = value;
                break;
             case CurrentProfSettingStateType.Interest:
                _interest.text = value;
                MypageEventManager.Instance._interest [0] = value;
                break;
            case CurrentProfSettingStateType.Marital:
                _marital.text = value;
                MypageEventManager.Instance._marital[0] = value;
                break;
            }
        }
        
        
        #region 生年月日ボタンを押した時の処理。(ドラムロールの処理) ※（IOS, ANDROID用）ネイティブピッカー
        /// <summary>
        /// Births the set.
        /// </summary>
        /// <returns>The set.</returns>
        public void BirthSet()
        {
            int y = 0;
            int m = 0;
            int d = 0;
            if (string.IsNullOrEmpty (MypageEventManager.Instance._birthDate) == true) {
                System.DateTime defaultSetDate =  GetDateTimeByAge (18);

                y = int.Parse(defaultSetDate.Year.ToString());
                m = int.Parse (defaultSetDate.Month.ToString());
                d = int.Parse (defaultSetDate.Day.ToString());
            } else {
                string[] bdArray = MypageEventManager.Instance._birthDate.Split('-');
                y = int.Parse (bdArray[0]);
                m = int.Parse (bdArray[1]);
                d = int.Parse (bdArray[2]);
            }

            NativePicker.Instance.ShowDatePicker(toScreenRect(_drawRect), NativePicker.DateTimeForDate(y, m, d), (long val) =>
            {
                //プロフィールの変更がある事を通知する用
                _isProfileChangeState = true; 

                _birthdayObjNative.transform.GetChild(0).GetComponent<Text>().text = "";
                MypageEventManager.Instance._birthDate = NativePicker.ConvertToDateTime (val).ToString ("yyyy-MM-dd");
                _birthdayObjNative.transform.GetChild(1).GetComponent<Text>().text = NativePicker.ConvertToDateTime(val).ToString("yyyy年MM月dd日");
            }, () => {
                _birthdayObjNative.transform.GetChild(0).GetComponent<Text>().text = "生年月日を選択してください";
                _birthdayObjNative.transform.GetChild(1).GetComponent<Text>().text = "";
            });
            return;
        }
        #endregion

        /// <summary>
        /// Gets the date time by age.
        /// </summary>
        /// <returns>The date time by age.</returns>
        private System.DateTime GetDateTimeByAge(int age = 0) 
        {
            // 現在の日付を取得する
            System.DateTime dtToday = System.DateTime.Today;
            System.DateTime tmp = dtToday.AddYears (-age);

            return tmp;
        }
        
    }
}