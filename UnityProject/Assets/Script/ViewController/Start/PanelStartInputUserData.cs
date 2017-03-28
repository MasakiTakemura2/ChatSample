using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using EventManager;
using ModelManager;
using Http;
using uTools;

namespace ViewController
{
    public class PanelStartInputUserData : SingletonMonoBehaviour<PanelStartInputUserData>
    {
        #region SerializeField
        [SerializeField]
        private Text _pref;

        [SerializeField]
        private Text _city;

        [SerializeField]
        private Dropdown _dropdownYear;

        [SerializeField]
        private Dropdown _dropdownMonth;

        [SerializeField]
        private Dropdown _dropdownDays;

        [SerializeField]
        private InputField _nickName;

        [SerializeField]
        private ProfTemplateInfiniteLimitScroll _profTemplatefInfiniteLimitScroll;

        [SerializeField]
        private GameObject _panelProfileSetTemplate;

        [SerializeField]
        private SwipeRecognizer _backSwipe;

        [SerializeField]
        private GameObject _loadingOverlay;

        [SerializeField]
        private GameObject _nameErrorText;

        [SerializeField]
        private GameObject _prefErrorText;

        [SerializeField]
        private GameObject _cityErrorText;

        [SerializeField]
        private GameObject _birthErrorText;

        [SerializeField]
        private GameObject _birthdayObj;

        [SerializeField]
        private GameObject _birthdayObjNative;

        [SerializeField]
        private GameObject _internalPopupOverLay;


        public UserDataEntity.Basic _userData;
        public CurrentProfSettingStateType _currentProfSettingState;
        public string _nickname;
        public string _prefId;
        public string _cityId;
        public string _birthday;

        #endregion

        #region Memeber Variable
        public List<string> _birthYear = new List<string> ();
        public List<string> _birthMonth = new List<string> ();
        public List<string> _birthDays = new List<string> ();
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
        Rect toScreenRect (Rect rect)
        {
            Vector2 lt = new Vector2 (rect.x, rect.y);
            Vector2 br = lt + new Vector2 (rect.width, rect.height);

            lt = GUIUtility.GUIToScreenPoint (lt);
            br = GUIUtility.GUIToScreenPoint (br);

            return new Rect (lt.x, lt.y, br.x - lt.x, br.y - lt.y);
        }
        #endregion

        /// <summary>
        /// Init this instance.
        /// </summary>
        public void Init ()
        {
            StartCoroutine (InitWait ());
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
        /// Gets the date time by age.
        /// </summary>
        /// <returns>The date time by age.</returns>
        private System.DateTime GetDateTimeByAge (int age = 0)
        {
            // 現在の日付を取得する
            System.DateTime dtToday = System.DateTime.Today;
            System.DateTime tmp = dtToday.AddYears (-age);

            return tmp;
        }

        /// <summary>
        /// Inits the wait.
        /// </summary>
        /// <returns>The wait.</returns>
        private IEnumerator InitWait ()
        {
            _currentProfSettingState = CurrentProfSettingStateType.None;
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
            _birthYear.Add ("");
            for (int i = (System.DateTime.Now.Year - 18); i >= 1930; i--) {
                _birthYear.Add (i.ToString ());
            }

            //Month「月」設定
            _birthMonth.Add ("");
            for (int i = 1; i <= 12; i++) {
                _birthMonth.Add (i.ToString ());
            }
            //Day「日」設定
            _birthDays.Add ("");
            for (int i = 1; i <= 31; i++) {
                _birthDays.Add (i.ToString ());
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
#elif UNITY_IOS || UNITY_ANDROID
			_birthdayObj.SetActive (false);
			_birthdayObjNative.SetActive (true);

			float width  = Screen.width/2;
			float height = Screen.width / 14;
			_drawRect = new Rect((Screen.width - width)/2, height, width, height);
#endif
            yield break;
        }

        /// <summary>
        /// Sets the data.
        /// 登録する用にセット。
        /// </summary>
        /// <returns>The data.</returns>
        public void SetData ()
        {
            string prefId = "";
            string cityId = "";

            prefId = _prefId;
            cityId = _cityId;
            _userData.user_key = AppStartLoadBalanceManager._userKey;
            _userData.pref = prefId;
            _userData.city_id = cityId;
            _userData.name = _nickName.GetComponent<InputField> ().text;

#if UNITY_EDITOR
            string birthDate = "";
            if (
                string.IsNullOrEmpty (_dropdownYear.GetComponentInChildren<Text> ().text) == false &&
                string.IsNullOrEmpty (_dropdownMonth.GetComponentInChildren<Text> ().text) == false &&
                string.IsNullOrEmpty (_dropdownDays.GetComponentInChildren<Text> ().text) == false
            ) {
                birthDate = _dropdownYear.GetComponentInChildren<Text> ().text + "-" + _dropdownMonth.GetComponentInChildren<Text> ().text + "-" + _dropdownDays.GetComponentInChildren<Text> ().text;
            }
            _userData.birth_date = birthDate;
#endif
            _userData.sex_cd = AppStartLoadBalanceManager._gender;

            if (string.IsNullOrEmpty (CommonModelHandle.GetPrefDataById (prefId).name) == false)
                _pref.text = CommonModelHandle.GetPrefDataById (prefId).name;

            if (string.IsNullOrEmpty (CommonModelHandle.GetCityDataById (cityId).name) == false)
                _city.text = CommonModelHandle.GetCityDataById (cityId).name;
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
                PlaceOfOriginClose (_panelProfileSetTemplate); //パネル閉じる
                break;
            }

            //パラメータ反映用の処理。
            SetData ();
        }


        /// <summary>
        /// Starts the button.
        /// </summary>
        /// <returns>The button.</returns>
        public void StartButton ()
        {
            SetData ();
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

            if (string.IsNullOrEmpty (_userData.birth_date) == true) {
                _birthErrorText.SetActive (true);
                _birthErrorText.GetComponent<Text> ().text = "生年月日に誤りがあります。";
                isError = true;
			} else {
				_birthErrorText.SetActive (false);
			}

            //年齢入力チェック
            bool isNgAge = AgeCheck (GetDateTimeByAge (18), _userData.birth_date);

            if (isNgAge == true) {
                _birthErrorText.SetActive (true);
                _birthErrorText.GetComponent<Text> ().text = "生年月日に誤りがあります。";
                isError = true;
            }


			Debug.Log (_userData.name + ": _userData.name");
			Debug.Log (_userData.pref + ": _userData.pref");
			Debug.Log (_userData.city_id + ": _userData.city_id");
			Debug.Log (_userData.birth_date + ": _userData.birth_date");

			if (isError == true)
				return;

			new ProfileUpdateApi (_userData);
			StartCoroutine (ProfileUpdateApiWait(this.gameObject));
		}
        
        

		/// <summary>
		/// Profiles the update API wait.
		/// </summary>
		/// <returns>The update API wait.</returns>
		private IEnumerator ProfileUpdateApiWait (GameObject animObj)
		{
			_loadingOverlay.SetActive (true);

			while (ProfileUpdateApi._success == false)
				yield return (ProfileUpdateApi._success == true);

			new GetUserApi ();
			while (GetUserApi._success == false)
				yield return (GetUserApi._success == true);

			_loadingOverlay.SetActive (false);

			if (MypageEventManager.Instance != null) {
				MypageEventManager.Instance._userDataBasic =  GetUserApi._httpCatchData.result.user;
			}

			//基本プロフィール作成OK♪
			AppStartLoadBalanceManager._isBaseProfile = true;
			AppStartLoadBalanceManager._gender = GetUserApi._httpCatchData.result.user.sex_cd;
			_currentProfSettingState = CurrentProfSettingStateType.None;

			if (PanelChatDescripton.Instance != null ) {
				//レビュー状態の時はランダムメッセージを送らないようにする
				if (PreRegistUser._httpCatchData.result.review == "false") {
					PanelAnimate ( PanelChatDescripton.Instance.gameObject );
				} else {
					StarFinished ();
				}
			} else {
				BackButton (animObj);
			}

			if (MypageEventManager.Instance != null) {
				//リロード
				SceneHandleManager.NextSceneRedirect (CommonConstants.MYPAGE_SCENE);
			}
		}

		/// <summary>
		/// Splashs the finished.
		/// </summary>
		public void StarFinished() {
			SceneHandleManager.NextSceneRedirect (CommonConstants.MYPAGE_SCENE);
		}

		#region 生年月日ボタンを押した時の処理。(ドラムロールの処理) ※（IOS, ANDROID用）ネイティブピッカー
		/// <summary>
		/// Births the set.
		/// </summary>
		/// <returns>The set.</returns>
		public void BirthSet()
		{
			System.DateTime defaultSetDate =  GetDateTimeByAge (18);

			int y = int.Parse(defaultSetDate.Year.ToString());
			int m = int.Parse (defaultSetDate.Month.ToString());
			int d = int.Parse (defaultSetDate.Day.ToString());

			NativePicker.Instance.ShowDatePicker(toScreenRect(_drawRect), NativePicker.DateTimeForDate(y, m, d),(long val) =>
			{
				_birthdayObjNative.transform.GetChild(0).GetComponent<Text>().text = "";
				_userData.birth_date = NativePicker.ConvertToDateTime (val).ToString ("yyyy-MM-dd");
				_birthdayObjNative.transform.GetChild(1).GetComponent<Text>().text = NativePicker.ConvertToDateTime(val).ToString("yyyy年MM月dd日");
			}, () => {
				_birthdayObjNative.transform.GetChild(0).GetComponent<Text>().text = "生年月日を選択してください";
				_birthdayObjNative.transform.GetChild(1).GetComponent<Text>().text = "";
			});
			return;
		}
		#endregion




		#region 都道府県設定（出身地設定）, 市区町村設定（出身地詳細）
		/// <summary>
		/// Places the of origin.
		/// 都道府県設定（出身地設定）, 市区町村設定（出身地詳細）パネルのオープン
		/// </summary>
		public void PlaceOfOriginOpen (bool isPref)
		{

#if !UNITY_EDITOR && (UNITY_ANDROID || UNITY_IOS)
			//#if !UNITY_EDITOR && (UNITY_IOS)
			//Native Picker方式に変更。
			if (InitDataApi._httpCatchData != null) 
			{
			List<string> tmpDataList = new List<string> ();
			string [] itemList = {""};

#if UNITY_IOS
            tmpDataList.Add("");
#endif

			//都道府県、選択。
			if (isPref == true) {

			System.DateTime defaultSetDate =  GetDateTimeByAge (18);

			foreach (var tmpItem in InitDataApi._httpCatchData.result.pref) {
			    tmpDataList.Add (tmpItem.name);
			}
			itemList = tmpDataList.ToArray ();
            
			//市区町村、選択。
			} else if (isPref == false && string.IsNullOrEmpty (_prefId) == false) {
			    foreach (var tmpItem in CommonModelHandle.GetCityData (_prefId)) {
			        tmpDataList.Add (tmpItem.Value.name);
			    }
			    itemList = tmpDataList.ToArray ();
			} else {
    			Debug.Log (" 例外の処理。 ");
    			return;
			}

			//ここでイベントを「OKかキャンセル」のイベントをキャッチ。
			NativePicker.Instance.ShowCustomPicker(toScreenRect(_drawRect), itemList, (long val) => 
			{
    			if (isPref == true)
    			{
        			_currentProfSettingState = CurrentProfSettingStateType.Pref;
        			_cityId = "";
        			_city.text = "";
        			_userData.city_id = "";
        			_pref.text = itemList[val];
#if UNITY_IOS
_prefId = ((int)val).ToString();
#else
_prefId = ((int)val + 1).ToString();
#endif
        			
        			_userData.pref = _prefId;
    			} else if (isPref== false) {
        			_currentProfSettingState = CurrentProfSettingStateType.City;
        			_city.text = itemList [val];
        			var city = CommonModelHandle.GetCityDataByName (_city.text);
        			_cityId = city.id;
        			_userData.city_id = _cityId;
    			}
			}, () => {
    			if (isPref == true) {
        			_pref.text = "指定しない";
        			_prefId = "";
        			_userData.pref = _prefId;
    			} else if (isPref == false) {
        			if (string.IsNullOrEmpty (_prefId) == false) {
            			_city.text = "";
            			_cityId    = "";
            			_userData.city_id = _cityId;
        			} else {
        			    Debug.Log ("都道府県を先に選択してください的なポップアップが親切。");
        			}
			    }
			});
			}
#else

            HeaderPanel.Instance.BackButtonSwitch (false);
			HeaderPanel.Instance.BackButtonSwitch (true, PlaceOfOriginCloseEvent);
			if (isPref == true) {
				_currentProfSettingState = CurrentProfSettingStateType.Pref;
				_cityId = "";
				_city.text = "";
				_userData.city_id = "";
			} else if (isPref== false) {
				_currentProfSettingState = CurrentProfSettingStateType.City;
			}

			_profTemplatefInfiniteLimitScroll.Init ();
			_backSwipe.EventMessageTarget = _panelProfileSetTemplate;

			PanelAnimate (_panelProfileSetTemplate);
			#endif
		}

		/// <summary>
		/// Places the of origin close.
		/// 都道府県設定（出身地設定）, 市区町村設定（出身地詳細）のクローズ
		/// </summary>
		/// <param name="animObj">Animation object.</param>
		public void PlaceOfOriginClose (GameObject animObj)
		{
			HeaderPanel.Instance.BackButtonSwitch (false);
			BackButton (_panelProfileSetTemplate);
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


		#region internal method
		/// <summary>
		/// Backs the button.
		/// </summary>
		/// <param name="fromObj">From object.</param>
		/// <param name="toObj">To object.</param>
		private void BackButton (GameObject fromObj) 
		{
			if (fromObj.GetComponent<uTweenPosition> ().to.x == 0)
			{
				fromObj.GetComponent<uTweenPosition> ().delay    = 0.001f;
				fromObj.GetComponent<uTweenPosition> ().duration = 0.25f;
				fromObj.GetComponent<uTweenPosition> ().to      = fromObj.transform.GetComponent<uTweenPosition> ().from;
				fromObj.GetComponent<uTweenPosition> ().from    = Vector3.zero;
				fromObj.GetComponent<uTweenPosition> ().ResetToBeginning ();
				fromObj.GetComponent<uTweenPosition> ().enabled = true;
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

			target.GetComponent<uTweenPosition> ().to = Vector3.zero;
			target.GetComponent<uTweenPosition> ().delay = 0.1f;
			target.GetComponent<uTweenPosition> ().duration = 0.2f;
			target.GetComponent<uTweenPosition> ().ResetToBeginning ();
			target.GetComponent<uTweenPosition> ().enabled = true;
		}
		#endregion


		#region 利用規約 open, close
		/// <summary>
		/// Webviews the term open.
		/// </summary>
		/// <returns>The term open.</returns>
		public void WebviewTermOpen ()
		{
			GameObject obj = null; //初期化。
			HeaderPanel.Instance.BackButtonSwitch (false);
			HeaderPanel.Instance.BackButtonSwitch (true, WebviewTermCloseEvent);

			if (StartEventManager.Instance != null) {
				obj = StartEventManager.Instance.WebViewTermsSwitch (true);
			} else if (MessageEventManager.Instance != null) {
				obj = MessageEventManager.Instance.WebViewTermsSwitch (true);
			}  else if (MypageEventManager.Instance != null) {
				obj =  MypageEventManager.Instance.WebViewTermsSwitch (true);
			} else if (SearchEventManager.Instance != null) {
				obj =  SearchEventManager.Instance.WebViewTermsSwitch (true);
			} else if (Helper.PurchaseManager.Instance != null) {
				obj = Helper.PurchaseManager.Instance.WebViewTermsSwitch (true);
			}

			if (obj != null) {
				PanelAnimate (obj);
			}
		}

		/// <summary>
		/// Webviews the term close.
		/// </summary>
		/// <returns>The term close.</returns>
		public void WebviewTermClose ()
		{
			GameObject obj = null; //初期化。
			HeaderPanel.Instance.BackButtonSwitch (false);
			if (StartEventManager.Instance != null) {
				obj = StartEventManager.Instance.WebViewTermsSwitch (false);
			} else if (MessageEventManager.Instance != null) {
				obj = MessageEventManager.Instance.WebViewTermsSwitch (false);
			} else if (MypageEventManager.Instance != null) {
				obj =  MypageEventManager.Instance.WebViewTermsSwitch (false);
			} else if (SearchEventManager.Instance != null) {
				obj =  SearchEventManager.Instance.WebViewTermsSwitch (false);
			} else if (Helper.PurchaseManager.Instance != null) {
				obj = Helper.PurchaseManager.Instance.WebViewTermsSwitch (false);
			}

			if (obj != null) {
				BackButton (obj);
			}
		}

		/// <summary>
		/// Webviews the term close event.
		/// </summary>
		/// <returns>The term close event.</returns>
		void WebviewTermCloseEvent ()
		{
			WebviewTermClose ();
		}
		#endregion


	}
}