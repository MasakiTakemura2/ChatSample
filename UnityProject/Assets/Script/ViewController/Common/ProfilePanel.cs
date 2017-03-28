using UnityEngine;
using System.Collections;
using EventManager;
using UnityEngine.UI;
using uTools;
using Http;
using ModelManager;
using Helper;

namespace ViewController
{
    public class ProfilePanel : SingletonMonoBehaviour<ProfilePanel> 
	{
		[SerializeField]
        private GameObject _popupOverlay;

        [SerializeField]
        private GameObject _loadingOverlay;

        [SerializeField]
        private SwipeRecognizer _backSwipe;

        [SerializeField]
        private GameObject _panelChat;
        
        // 更新の為引っ張っておくオブジェクト
        [SerializeField]
        private RawImage _profImage;
        [SerializeField]
        private RawImage _coverImage;
        [SerializeField]
        private Text _name;
        [SerializeField]
        private Text _sex_cd;
        [SerializeField]
        private Text _blood_type;
        [SerializeField]
        private Text _prefAndCity;
        [SerializeField]
        private Text _birth_date;
        [SerializeField]
        private Text _height;
        [SerializeField]
        private Text _weight;
        [SerializeField]
        private Text _profile;
        [SerializeField]
        private Text _body_type;
        [SerializeField]
        private Text _hair_style;
        [SerializeField]
        private Text _glasses;
        [SerializeField]
        private Text _type;
        [SerializeField]
        private Text _personality;
        [SerializeField]
        private Text _holiday;
        [SerializeField]
        private Text _annual_income;
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
        private Text _headerTitle;

        [SerializeField]
        private Transform _blockButton;
        
        [SerializeField]
        private Transform _favoriteButton;        

		[SerializeField]
		private Transform _LimitMessageButton;

        public string _toUserId;
        

        [SerializeField]
        private GameObject _pforLarge;

        [SerializeField]
        private GameObject _popupMovie;

        #region 初期化
        /// <summary>
        /// Init the specified userId.
        /// </summary>
        /// <param name="userId">User identifier.</param>
        public void Init ( string toUserId ) 
		{
            if (_popupOverlay.activeSelf == true)
                _popupOverlay.SetActive (false);

            this.transform.GetChild (1).GetComponent<ScrollRect> ().content.localPosition = Vector3.zero;

            _toUserId = toUserId;
            new GetUserApi (toUserId);
            StartCoroutine (GetProfileApi (toUserId));
        }

		// すでに通信しなくてもデータあるからこれで初期化してって場合  常に通信かかっても更新しなおしたほうがいいのかもしれない・・・
		public void InitFromAlreadyData (UserDataEntity.Basic user)
        {
            string isFavorite = user.is_favorite;
            string isBlock = user.is_block;
        
            if (isBlock == "true") {
                _blockButton.GetChild (0).gameObject.SetActive (true);
            } else if (isBlock == "false") {
                _blockButton.GetChild (0).gameObject.SetActive (false); 
            }
             
            if (isFavorite == "true") {
                _favoriteButton.GetChild (0).gameObject.SetActive (true);
            } else if (isFavorite == "false") {
                _favoriteButton.GetChild (0).gameObject.SetActive (false);
            }

            string islimitrelease = user.is_limit_release;
            if (islimitrelease == "true") {
                _LimitMessageButton.GetComponent<Button> ().enabled = false;
                _LimitMessageButton.GetComponent<Image> ().color = new Color (147 / 255.0f, 147 / 255.0f, 147 / 255.0f, 255);
            } else if (islimitrelease == "false") {
                _LimitMessageButton.GetComponent<Image> ().color = new Color (247 / 255.0f, 117 / 255.0f, 133 / 255.0f, 255);
                _LimitMessageButton.GetComponent<Button> ().enabled = true;
            }
        
            if (user != null) {
                Debug.Log ("user情報がヌルです");
            }

            if (string.IsNullOrEmpty (user.profile_image_url) == false && _profImage != null) {
                StartCoroutine (WwwToRendering (user.profile_image_url, _profImage));
            } else {
                _profImage.texture = Resources.Load ("Texture/noimage_user") as Texture;
            }

            if (string.IsNullOrEmpty (user.cover_image_url) == false && _coverImage != null) {
                StartCoroutine (WwwToRendering (user.cover_image_url, _coverImage));
            } else {
                _coverImage.texture = Resources.Load ("Texture/noimage_cover") as Texture;
            }

            if (_name != null)
                _name.text = user.name;

            if (_sex_cd != null) {
                 
                if (user.sex_cd == ((int)GenderType.FeMale).ToString ()) {
                    _sex_cd.text = LocalMsgConst.GENDER_FEMALE;
                } else if (user.sex_cd == ((int)GenderType.Male).ToString ()) {
                    _sex_cd.text = LocalMsgConst.GENDER_MALE;
                }
            }

            if (_blood_type != null) {
                string bloodText = user.blood_type;

                if (bloodText == "0") {
                    _blood_type.text = "秘密";
                } else {
                    string bloodTypeMsg = CommonModelHandle.GetByIdBaseData (user.blood_type, CurrentProfSettingStateType.BloodType).name;
                    _blood_type.text = bloodTypeMsg + LocalMsgConst.BLOOD_TYPE_JA;
                }
            }


			if (_prefAndCity != null){
				string prefName = CommonModelHandle.GetPrefDataById (user.pref).name;
				string cityName = CommonModelHandle.GetCityDataById (user.city_id).name;

				_prefAndCity.text = prefName + "" + cityName;
			}

            if (_birth_date != null) {
                if (user.age == "0") {
                    _birth_date.text = "秘密";
                } else {
                    _birth_date.text = user.age + "歳";
                }
            }
                
            if (_height != null) {
                if (user.height == "0") {
                    _height.text = "秘密";
                } else {
                    _height.text = user.height + "cm";
                }
            }

            if (_weight != null) {
                if (user.weight == "0") {
                    _weight.text = "秘密";
                } else {
                    _weight.text = user.weight + "kg";    
                }
            } 
				

			if (_profile != null)
				_profile.text = user.profile;

			if (_body_type != null)
				_body_type.text = user.body_type[0];

			if (_hair_style != null)
				_hair_style.text = user.hair_style[0];

			if (_glasses != null)
				_glasses.text = user.glasses[0];

			if (_type != null)
				_type.text = user.type[0];

			if (_personality != null)
				_personality.text = user.personality[0];

			if (_holiday != null)
				_holiday.text = user.holiday[0];

			if (_annual_income)
				_annual_income.text = user.annual_income[0];

			if (_education != null)
				_education.text = user.education[0];

			if (_housemate != null)
				_housemate.text = user.housemate[0];

			if (_sibling != null)
				_sibling.text = user.sibling[0];

			if (_alcohol != null)
				_alcohol.text = user.alcohol[0];

			if (_tobacco != null)
				_tobacco.text = user.tobacco[0];

			if (_car != null)
				_car.text = user.car[0];

			if (_pet != null)
				_pet.text = user.pet[0];

			if (_hobby != null)
				_hobby.text = user.hobby[0];

			if (_interest != null)
				_interest.text = user.interest[0];

			if (_marital != null)
				_marital.text = user.marital[0];


			_headerTitle.GetComponent<Text> ().text = GetUserApi._httpOtherUserCatchData.name;
		}

        /// <summary>
        /// Gets the profile API.
        /// </summary>
        /// <returns>The profile API.</returns>
        /// <param name="toUserId">To user identifier.</param>
        private IEnumerator GetProfileApi (string toUserId)
        {
            _profImage.texture = Resources.Load ("Texture/noimage_user") as Texture;
            _coverImage.texture = Resources.Load ("Texture/noimage_cover") as Texture;


            _loadingOverlay.SetActive (true);
            while (GetUserApi._success == false)
                yield return (GetUserApi._success == true);

            string isFavorite = GetUserApi._httpOtherUserCatchData.is_favorite;
            string isBlock = GetUserApi._httpOtherUserCatchData.is_block;

            if (isBlock == "true") {
                _blockButton.GetChild (0).gameObject.SetActive (true);
            } else if (isBlock == "false") {
                _blockButton.GetChild (0).gameObject.SetActive (false); 
            }
             
            if (isFavorite == "true") {
                _favoriteButton.GetChild (0).gameObject.SetActive (true);
            } else if (isFavorite == "false") {
                _favoriteButton.GetChild (0).gameObject.SetActive (false);
            }

            string islimitrelease = GetUserApi._httpOtherUserCatchData.is_limit_release;
            if (islimitrelease == "true") {
                _LimitMessageButton.GetComponent<Button> ().enabled = false;
                _LimitMessageButton.GetComponent<Image> ().color = new Color (147 / 255.0f, 147 / 255.0f, 147 / 255.0f, 255);
            } else if (islimitrelease == "false") {
                _LimitMessageButton.GetComponent<Image> ().color = new Color (247 / 255.0f, 117 / 255.0f, 133 / 255.0f, 255);
                _LimitMessageButton.GetComponent<Button> ().enabled = true;
            }
            
            _loadingOverlay.SetActive (false);

            var user = GetUserApi._httpOtherUserCatchData;

            if (string.IsNullOrEmpty (user.profile_image_url) == false && _profImage != null) {
                StartCoroutine (WwwToRendering (user.profile_image_url, _profImage));
            } else {
                _profImage.texture = Resources.Load ("Texture/noimage_user") as Texture;
            }

            if (string.IsNullOrEmpty (user.cover_image_url) == false && _coverImage != null) {
                StartCoroutine (WwwToRendering (user.cover_image_url, _coverImage));
            } else {
                _coverImage.texture = Resources.Load ("Texture/noimage_cover") as Texture;
            }
                
            if (_name != null)
                _name.text = user.name;
                
            if (_sex_cd != null) {
                if (user.sex_cd == ((int)GenderType.FeMale).ToString ()) {
                    _sex_cd.text = LocalMsgConst.GENDER_FEMALE;
                } else if (user.sex_cd == ((int)GenderType.Male).ToString ()) {
                    _sex_cd.text = LocalMsgConst.GENDER_MALE;
                }
            }
                
            if (_blood_type != null) {
                string bloodText = user.blood_type;
                if (bloodText == "0") {
                    _blood_type.text = "秘密";
                } else {
                    string bloodTypeMsg = CommonModelHandle.GetByIdBaseData (user.blood_type, CurrentProfSettingStateType.BloodType).name;
                    _blood_type.text = bloodTypeMsg + LocalMsgConst.BLOOD_TYPE_JA;
                }
            }


            if (_prefAndCity != null) {
                string prefName = CommonModelHandle.GetPrefDataById (user.pref).name;
                string cityName = CommonModelHandle.GetCityDataById (user.city_id).name;
                 
                _prefAndCity.text = prefName + "" + cityName;
            }

            if (_birth_date != null) {
                if (user.age == "0") {
                    _birth_date.text = "秘密";
                } else {
                    _birth_date.text = user.age + "歳";
                }
            }
                
            if (_height != null) {
                if (user.height == "0") {
                    _height.text = "秘密";
                } else {
                    _height.text = user.height + "cm";
                }
            }

            if (_weight != null) {
                if (user.weight == "0") {
                    _weight.text = "秘密";
                } else {
                    _weight.text = user.weight + "kg";    
                }
            } 

            if (_profile != null)
                _profile.text = user.profile;

            if (_body_type != null &&  user.body_type != null && user.body_type.Count > 0)
                _body_type.text = user.body_type[0];

            if (_hair_style != null && user.hair_style != null && user.hair_style.Count > 0)
                _hair_style.text = user.hair_style[0];

            if (_glasses != null && user.glasses !=null && user.glasses.Count > 0)
                _glasses.text = user.glasses[0];

            if (_type != null && user.type != null && user.type.Count > 0)
                _type.text = user.type[0];//TODO: 複数選択可・対応

            if (_personality != null && user.personality != null && user.personality.Count > 0)
                _personality.text = user.personality[0];//TODO: 複数選択可・対応

            if (_holiday != null && user.holiday != null && user.holiday.Count > 0)
                _holiday.text = user.holiday[0];

            if (_annual_income &&  user.annual_income != null && user.annual_income.Count > 0)
                _annual_income.text = user.annual_income[0];

            if (_education != null && user.education != null && user.education.Count > 0)
                _education.text = user.education[0];
                
            if (_housemate != null &&  user.housemate != null && user.housemate.Count > 0)
                _housemate.text = user.housemate[0];

            if (_sibling != null && user.sibling != null && user.sibling.Count > 0)
                _sibling.text = user.sibling[0];

            if (_alcohol != null && user.alcohol != null && user.alcohol.Count > 0)
                _alcohol.text = user.alcohol[0];

            if (_tobacco != null && user.tobacco != null && user.tobacco.Count > 0)
                _tobacco.text = user.tobacco[0];

            if (_car != null && user.car != null && user.car.Count > 0)
                _car.text = user.car[0];

            if (_pet != null && user.pet != null && user.pet.Count > 0)
                _pet.text = user.pet[0];

            if (_hobby != null && user.hobby != null && user.hobby.Count > 0)
                _hobby.text = user.hobby[0];

            if (_interest != null && user.interest != null && user.interest.Count > 0)
                _interest.text = user.interest[0];

            if (_marital != null &&  user.marital != null && user.marital.Count > 0)
                _marital.text = user.marital[0];

			_headerTitle.GetComponent<Text> ().text = GetUserApi._httpOtherUserCatchData.name;
        }
        #endregion


        #region Popup Event
        //---------------- ここから通報用のポップアップ ---------------- 
        ///TODO: pop up 共通化にする　- ここから
        /// <summary>
        /// Reports the confirm button.
        /// </summary>
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
        
        /// <summary>
        /// Reports the API call.
        /// </summary>
        void ReportApiCall()
		{
			PopupSecondSelectPanel.Instance.PopClean(ReportApiCall, ReportCancel);

			new SendReportApi (_toUserId);
            StartCoroutine (SendReportWait ());
            PanelPopupCloseAnimate (GameObject.FindGameObjectWithTag(CommonConstants.POPUP_SECOND_SELECT_TAG));
        }

        /// <summary>
        /// Sends the report wait.
        /// </summary>
        /// <returns>The report wait.</returns>
        private IEnumerator SendReportWait ()
        {
            _loadingOverlay.SetActive (true);
            while (SendReportApi._success == false)
                yield return (SendReportApi._success == true);
           _loadingOverlay.SetActive (false);


            PopupPanel.Instance.PopMessageInsert (
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
            PopupSecondSelectPanel.Instance.PopClean(ReportApiCall, ReportCancel);

            PanelPopupCloseAnimate (GameObject.FindGameObjectWithTag(CommonConstants.POPUP_SECOND_SELECT_TAG));
        }
        
        /// <summary>
        /// Reports the finish close.
        /// </summary>
        void ReportFinishClose () 
        {
			PopupPanel.Instance.PopClean();
			PanelPopupCloseAnimate (GameObject.FindGameObjectWithTag(CommonConstants.POPUP_BASIC_TAG));
        }
        //---------------- ここまで通報用のポップアップ ---------------- 
        
        //---------------- ここから - お気に入り有無のポップアップ ---------------- 
        /// <summary>
        /// Favorites the confirm open.
        /// </summary>
        /// <param name="target">Target.</param>
        public void FavoriteConfirmOpen(GameObject target) 
		{
            string popText = "";
            if (_favoriteButton.GetChild(0).gameObject.activeSelf == true)
			{
                 popText = string.Format (LocalMsgConst.FAVORITE_OFF_CONFIRM, GetUserApi._httpOtherUserCatchData.name);
            } else if (_favoriteButton.GetChild(0).gameObject.activeSelf == false) {
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
        void FavoriteApiCall () 
		{
			PopupSecondSelectPanel.Instance.PopClean(FavoriteApiCall, FavoriteCancel);

            new SetUserFavoriteApi (_toUserId);
            PanelPopupCloseAnimate (GameObject.FindGameObjectWithTag(CommonConstants.POPUP_SECOND_SELECT_TAG));
            StartCoroutine (SetUserFavoriteApiWait ());
        }

        /// <summary>
        /// Sets the user favorite API wait.
        /// </summary>
        /// <returns>The user favorite API wait.</returns>
        private IEnumerator SetUserFavoriteApiWait () {
            _loadingOverlay.SetActive (true);
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

             if (isFavorite == "true") {
                 _favoriteButton.GetChild (0).gameObject.SetActive (true);
             } else if (isFavorite == "false") {
                 _favoriteButton.GetChild (0).gameObject.SetActive (false);
             }
            
        }

        /// <summary>
        /// Reports the cancel.
        /// </summary>
        void FavoriteCancel ()
		{
            PopupSecondSelectPanel.Instance.PopClean(FavoriteApiCall,FavoriteCancel);

            PanelPopupCloseAnimate (GameObject.FindGameObjectWithTag(CommonConstants.POPUP_SECOND_SELECT_TAG));
        }

        /// <summary>
        /// Reports the finish close.
        /// </summary>
        void FavoriteFinishClose () 
		{
			PopupPanel.Instance.PopClean();
            PanelPopupCloseAnimate (GameObject.FindGameObjectWithTag(CommonConstants.POPUP_BASIC_TAG));
        }
        //---------------- ここまで - お気に入り有無のポップアップ ---------------- 
        
        
        //---------------- ここからユーザーブロックポップアップ ---------------- 
        public void UserBlockConfirmOpen ( GameObject target ) 
        {
             string popText = "";
             if (_blockButton.GetChild(0).gameObject.activeSelf == true) {
                 popText = string.Format (LocalMsgConst.USER_BLOCK_OFF_QUESTION, GetUserApi._httpOtherUserCatchData.name);
             } else if (_blockButton.GetChild(0).gameObject.activeSelf == false) {
                 popText = string.Format (LocalMsgConst.USER_BLOCK_ON_QUESTION, GetUserApi._httpOtherUserCatchData.name);
             }

             PopupSecondSelectPanel.Instance.PopMessageInsert(
                popText,
                LocalMsgConst.YES,
                LocalMsgConst.NO,
                UserBlockCall,
                UserBlockCancel
            );
            PanelPopupAnimate (target);
        }

        /// <summary>
        /// Users the block call.
        /// </summary>
        /// <returns>The block call.</returns>
        void UserBlockCall () {
            PopupSecondSelectPanel.Instance.PopClean();
            new SetUserBlockApi (_toUserId);
            PanelPopupCloseAnimate (GameObject.FindGameObjectWithTag(CommonConstants.POPUP_SECOND_SELECT_TAG));
            StartCoroutine (UserBlockCallWait ());
        }
        
        /// <summary>
        /// Sets the user favorite API wait.
        /// </summary>
        /// <returns>The user favorite API wait.</returns>
        private IEnumerator UserBlockCallWait () {
            _loadingOverlay.SetActive (true);
            while (SetUserBlockApi._success == false)
                yield return (SetUserBlockApi._success == true);
            _loadingOverlay.SetActive (false);
            
           PopupPanel.Instance.PopMessageInsert(
                SetUserBlockApi._httpCatchData.result.complete[0],
                LocalMsgConst.OK,
                UserBlockFinishClose
            );
            PanelPopupAnimate (GameObject.FindGameObjectWithTag(CommonConstants.POPUP_BASIC_TAG));
            string isBlock    = SetUserBlockApi._httpCatchData.result.is_block;
    
            if (isBlock == "true")  {
                _blockButton.GetChild(0).gameObject.SetActive (true);
            } else if (isBlock == "false") {
                _blockButton.GetChild(0).gameObject.SetActive (false); 
            }
        }

        /// <summary>
        /// Users the block cancel.
        /// </summary>
        /// <returns>The block cancel.</returns>
        void UserBlockCancel () {
            PopupSecondSelectPanel.Instance.PopClean();
            PanelPopupCloseAnimate (GameObject.FindGameObjectWithTag(CommonConstants.POPUP_SECOND_SELECT_TAG));
        }
        
        /// <summary>
        /// Reports the finish close.
        /// </summary>
        void UserBlockFinishClose () {
            PopupPanel.Instance.PopClean ();
            PanelPopupCloseAnimate (GameObject.FindGameObjectWithTag(CommonConstants.POPUP_BASIC_TAG));
        }
        //---------------- ここまでユーザーブロックポップアップ ---------------- 
        

		//---------------- ここから - 送り放題のポップアップ ---------------- 
		public void LimitMessageConfirmOpen() 
		{
			// メッセージ呼び出し
			new  LimitReleaseConfirmMessage(_toUserId);
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
            PopupSecondSelectPanel.Instance.PopClean();
			new LimitReleaseApi (_toUserId);
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
				LimitReleaseApi._httpCatchData.result.complete[0],
				LocalMsgConst.OK,
				FavoriteFinishClose
			);
			PanelPopupAnimate (GameObject.FindGameObjectWithTag(CommonConstants.POPUP_BASIC_TAG));

			string islimitRelease = LimitReleaseApi._httpCatchData.result.is_limit_release;

			if (islimitRelease == "true") 
			{
				_LimitMessageButton.GetComponent<Button> ().enabled = false;
				_LimitMessageButton.GetComponent<Image> ().color = new Color (147/255.0f,147/255.0f,147/255.0f,255);

				if(PanelBoardDetail.Instance != null)
				{
					PanelBoardDetail.Instance.GetLimitReleaseButton().GetComponent<Button> ().enabled = false;
					PanelBoardDetail.Instance.GetLimitReleaseButton().GetComponent<Image> ().color = new Color (147/255.0f,147/255.0f,147/255.0f,255);
				}
			} else if (islimitRelease == "false") {
				_LimitMessageButton.GetComponent<Image> ().color = new Color (247/255.0f,117/255.0f,133/255.0f,255);
				_LimitMessageButton.GetComponent<Button> ().enabled = true;
			}

		}

		void LimitReleaseCancel () 
		{
			PopupSecondSelectPanel.Instance.PopClean();
			PanelPopupCloseAnimate (GameObject.FindGameObjectWithTag(CommonConstants.POPUP_SECOND_SELECT_TAG));
		}

		void LimitReleaseFinishClose () 
		{
			PanelPopupCloseAnimate (GameObject.FindGameObjectWithTag(CommonConstants.POPUP_BASIC_TAG));
		}
		//---------------- ここまで - 送り放題のポップアップ ---------------- 


		#endregion



        #region チャットをオープン
        /// <summary>
        /// Profiles to chat open.
        /// </summary>
        /// <returns>The to chat open.</returns>
        public void ProfileToChatOpen (GameObject animObj)
        {
#if UNITY_IPHONE || UNITY_ANDROID && !UNITY_EDITOR
                if (GetUserApi._httpCatchData.result.review == "false") {
                    if (CommonConstants.IS_PREMIUM == false ) {
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
AppStartLoadBalanceManager._whereFromAds = MaioMovieSdkEvent.WhereFromAds.ProfileToChat;
                                //問答無用で動画広告を表示
                                Maio.Show (CommonConstants.MAIO_ZONE_ID_1);
                                return;                        
                            }
                        }
                    } else {
                        //有料会員の場合は何もしない。
                    }
                }
#endif
        
        
            if (_blockButton.transform.GetChild (0).gameObject.activeSelf == true)
            {
                PopupPanel.Instance.PopClean ();
                PopupPanel.Instance.PopMessageInsert(
                    LocalMsgConst.BLOCK_USER,
                    LocalMsgConst.OK,
                    ChatBlockEvent
                );
                PanelPopupAnimate (GameObject.FindGameObjectWithTag (CommonConstants.POPUP_BASIC_TAG));
                return;
            }

			_panelChat.GetComponent<PanelChat> ()._maxAfterID = "";
			_panelChat.GetComponent<PanelChat> ()._maxBeforeID = "";
			_panelChat.GetComponent<PanelChat> ()._maxAfterIDBackup = "";
			_panelChat.GetComponent<PanelChat> ()._maxBeforeIDBackup = "";
			_panelChat.GetComponent<PanelChat> ()._listUpdateDisable = false; //解除処理
            PanelChat.Instance.Init (_toUserId);
            

			PanelPopupCloseAnimate (this.gameObject);
            _backSwipe.EventMessageTarget = _panelChat;
            PanelAnimate (animObj);
            _panelChat.GetComponent<BoxCollider2D> ().enabled = true;


			if (MypageEventManager.Instance != null || SearchEventManager.Instance != null ) 
            {
                HeaderPanel.Instance.BackButtonSwitch (false);
                HeaderPanel.Instance.BackButtonSwitch (true, ProfileToChatCloseEvent);
				_backSwipe.EventMessageTarget = _panelChat.gameObject;
            }

			if(MessageEventManager.Instance != null)
			{
				HeaderPanel.Instance.BackButtonSwitch (false);
				//HeaderPanel.Instance.BackButtonSwitch (true, MessageEventManager.Instance.ChatBackButton);
				HeaderPanel.Instance.BackButtonSwitch (true, ProfileToChatCloseEvent);

				_backSwipe.EventMessageTarget = _panelChat.gameObject;
			}
			if(BulletinBoardEventManager.Instance != null)
			{
				HeaderPanel.Instance.BackButtonSwitch (false);
				HeaderPanel.Instance.BackButtonSwitch (true, BulletinBoardEventManager.Instance.ProfileToChatCloseEvent);
				_backSwipe.EventMessageTarget = _panelChat.gameObject;
			}

			if (MatchingEventManager.Instance != null)
			{
				HeaderPanel.Instance.BackButtonSwitch (false);
				HeaderPanel.Instance.BackButtonSwitch (true, MatchingEventManager.Instance.ChatBackButton);
				_backSwipe.EventMessageTarget = _panelChat.gameObject;
			} 
        }
        
        void ChatBlockEvent () {
            PopupPanel.Instance.PopClean ();
            PanelPopupCloseAnimate (GameObject.FindGameObjectWithTag (CommonConstants.POPUP_BASIC_TAG));
        }
        
       
            

        /// <summary>
        /// Profiles to chat close.
        /// </summary>
        /// <returns>The to chat close.</returns>
        public void ProfileToChatClose (GameObject animObj)
        {
			Init (_toUserId);
            PanelChat.Instance.ResetScrollItem ();
            PanelPopupAnimate (this.gameObject);
            _backSwipe.EventMessageTarget = this.gameObject;
            BackButton (animObj);
            
            if (MypageEventManager.Instance != null || SearchEventManager.Instance != null || MatchingEventManager.Instance != null) 
            {
                HeaderPanel.Instance.BackButtonSwitch (false);
                HeaderPanel.Instance.BackButtonSwitch (true, ProfileToChatBack);
            }

			if(BulletinBoardEventManager.Instance != null)
			{
				HeaderPanel.Instance.BackButtonSwitch (false);
				HeaderPanel.Instance.BackButtonSwitch (true, BulletinBoardEventManager.Instance.ProfileCloseEvent);
			}

			if(MessageEventManager.Instance != null)
			{
                //_panelEazyNotifyInfiniteScroll.Init (""); 
				HeaderPanel.Instance.BackButtonSwitch (false);
				HeaderPanel.Instance.BackButtonSwitch (true, MessageEventManager.Instance.ProfileCloseEvent);
			}
         
            _popupOverlay.SetActive (false);
        }

        /// <summary>
        /// Profiles to chat close event.
        /// Mypage専用イベント
        /// </summary>
        /// <returns>The to chat close event.</returns>
        public void ProfileToChatCloseEvent ()
        {
            if (PanelFooterButtonManager.Instance != null)
                PanelFooterButtonManager.Instance.gameObject.SetActive (true);
            ProfileToChatClose (_panelChat);

#if UNITY_IPHONE || UNITY_ANDROID && !UNITY_EDITOR
            if (GetUserApi._httpCatchData.result.review == "false" && CommonConstants.IS_PREMIUM == false) {
                IMobileSdkAdsUnityPlugin.show (CommonConstants.IMOBILE_FULL_SPOT_ID);
            }
#endif
        }

        /// <summary>
        /// Profiles to chat back.
        ///プロフィールマイページ, Search専用イベント。
        /// </summary>
        /// <returns>The to chat back.</returns>
        void ProfileToChatBack ()
		{
            if (SearchEventManager.Instance != null) 
            {
				if (_headerTitle != null)
				{
					_headerTitle.text = LocalMsgConst.TITLE_SEARCH;
				}
                SearchEventManager.Instance.SearchProfileClose (this.gameObject);

			} else if (MypageEventManager.instance != null) {
               
				_headerTitle.text = LocalMsgConst.TITLE_MYPAGE;
				if (_headerTitle != null)
				{
					_headerTitle.text = LocalMsgConst.TITLE_MYPAGE;
				}
                MypageEventManager.Instance.HistoryToProfileClose (this.gameObject);
			}
        }
        #endregion
        
        #region 動画広告用の処理。
        /// <summary>
        /// Movies the popup look button.
        /// 動画をみて、チャットルームを開く
        /// </summary>
        public void MoviePopupLookButton () {
AppStartLoadBalanceManager._whereFromAds = MaioMovieSdkEvent.WhereFromAds.ProfileToChat;
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
        /// Ons the closed ad.
        /// </summary>
        /// <param name="zoneId">Zone identifier.</param>
        public void OnClosedAd (string zoneId)
        {
            if (_blockButton.transform.GetChild (0).gameObject.activeSelf == true)
            {
                PopupPanel.Instance.PopClean ();
                PopupPanel.Instance.PopMessageInsert(
                    LocalMsgConst.BLOCK_USER,
                    LocalMsgConst.OK,
                    ChatBlockEvent
                );
                PanelPopupAnimate (GameObject.FindGameObjectWithTag (CommonConstants.POPUP_BASIC_TAG));
                return;
            }

            _panelChat.GetComponent<PanelChat> ()._maxAfterID = "";
            _panelChat.GetComponent<PanelChat> ()._maxBeforeID = "";
            _panelChat.GetComponent<PanelChat> ()._maxAfterIDBackup = "";
            _panelChat.GetComponent<PanelChat> ()._maxBeforeIDBackup = "";
            _panelChat.GetComponent<PanelChat> ()._listUpdateDisable = false; //解除処理
            PanelChat.Instance.Init (_toUserId);
            

            PanelPopupCloseAnimate (this.gameObject);
            _backSwipe.EventMessageTarget = _panelChat;
            PanelAnimate (_panelChat);
            _panelChat.GetComponent<BoxCollider2D> ().enabled = true;


            if (MypageEventManager.Instance != null || SearchEventManager.Instance != null ) 
            {
                HeaderPanel.Instance.BackButtonSwitch (false);
                HeaderPanel.Instance.BackButtonSwitch (true, ProfileToChatCloseEvent);
                _backSwipe.EventMessageTarget = _panelChat.gameObject;
            }

            if(MessageEventManager.Instance != null)
            {
                HeaderPanel.Instance.BackButtonSwitch (false);
                //HeaderPanel.Instance.BackButtonSwitch (true, MessageEventManager.Instance.ChatBackButton);
                HeaderPanel.Instance.BackButtonSwitch (true, ProfileToChatCloseEvent);

                _backSwipe.EventMessageTarget = _panelChat.gameObject;
            }
            if(BulletinBoardEventManager.Instance != null)
            {
                HeaderPanel.Instance.BackButtonSwitch (false);
                HeaderPanel.Instance.BackButtonSwitch (true, BulletinBoardEventManager.Instance.ProfileToChatCloseEvent);
                _backSwipe.EventMessageTarget = _panelChat.gameObject;
            }

            if (MatchingEventManager.Instance != null)
            {
                HeaderPanel.Instance.BackButtonSwitch (false);
                HeaderPanel.Instance.BackButtonSwitch (true, MatchingEventManager.Instance.ChatBackButton);
                _backSwipe.EventMessageTarget = _panelChat.gameObject;
            } 
        }
        #endregion


        #region プロフィール画像を大きいサイズで見る
        /// <summary>
        /// Larges the prof image.
        /// </summary>
        public void LargeProfImageOpen ()
        {
            HeaderPanel.Instance.BackButtonSwitch (false);
            HeaderPanel.Instance.BackButtonSwitch (true, LargeProfImageCloseEvent);

            _backSwipe.GetComponent<ScreenRaycaster> ().enabled = false;
            PanelPopupAnimate (_pforLarge, true);
            RawImage rawImage = _pforLarge.transform.GetChild (0).GetComponent<RawImage>();

            if (string.IsNullOrEmpty (GetUserApi._httpOtherUserCatchData.profile_image_url) == false)
            {
                StartCoroutine (WwwToRendering (GetUserApi._httpOtherUserCatchData.profile_image_url ,rawImage));
            }
            return;
        }

        /// <summary>
        /// Larges the prof image close.
        /// </summary>
        public void LargeProfImageClose () 
        {
            _backSwipe.GetComponent<ScreenRaycaster> ().enabled = false;
            PanelPopupCloseAnimate(_pforLarge, true);
            HeaderPanel.Instance.BackButtonSwitch (false);
            HeaderPanel.Instance.BackButtonSwitch (true, PanelChat.Instance.ProfileCloseEvent);
            return;
        }

        /// <summary>
        /// Larges the prof image close.
        /// </summary>
        void LargeProfImageCloseEvent()
        {
            LargeProfImageClose();
            return;
        }
        #endregion


        #region Finger Event
        /// <summary>
        /// Backs the swipe.
        /// </summary>
        void OnSwipe (SwipeGesture gesture)
		{
            if (gesture.Selection)
			{
                if (gesture.Direction == FingerGestures.SwipeDirection.Left) 
				{
                    //Debug.Log ("Left Left Left Left Left Left ");
                } else if (gesture.Direction == FingerGestures.SwipeDirection.Right) {

					if (MypageEventManager.Instance != null) 
					{
						MypageEventManager.Instance.HistoryToProfileClose (this.gameObject);
					}
                     
					if (MatchingEventManager.Instance != null) 
					{ 
						MatchingEventManager.Instance.ProfileClose (this.gameObject);
					}

					if (MessageEventManager.Instance != null) 
					{
						PanelChat.Instance.ProfileClose (this.gameObject);
					}

					if (SearchEventManager.Instance != null)
					{
						SearchEventManager.Instance.ProfileClose (this.gameObject);
					}

					if (BulletinBoardEventManager.Instance != null)
					{
						BulletinBoardEventManager.Instance.ProfileClose (this.gameObject);
					}
                }
            }
        }
        #endregion

        #region private Method
        /// <summary>
        /// Res the panel animate.
        /// </summary>
        /// <param name="target">Target.</param>
        private void PanelPopupAnimate ( GameObject target, bool isLarge = false)
        {
            if (isLarge == false) 
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
        private void PanelPopupCloseAnimate (GameObject target, bool isLarge = false)
        {
            if (isLarge == false )
               _popupOverlay.SetActive (false);

            target.GetComponent<uTweenScale> ().delay    = 0.001f;
            target.GetComponent<uTweenScale> ().duration = 0.15f;
            target.GetComponent<uTweenScale> ().from = new Vector3 (1f, 1f ,1f );
            target.GetComponent<uTweenScale> ().to = Vector3.zero;
            target.GetComponent<uTweenScale> ().ResetToBeginning ();
            target.GetComponent<uTweenScale> ().enabled = true;
        }
        
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
    
            using (WWW www = new WWW (url))
            {
                while (www == null)
                    yield return (www != null);
    
                while (www.isDone == false)
                    yield return (www.isDone);
    
                //non texture file
                if (string.IsNullOrEmpty (www.error) == false) 
                {
                    Debug.LogError (www.error);
                    Debug.Log (url);
                    yield break;
                }
                while (targetObj == null)
                    yield return (targetObj != null);

                targetObj.gameObject.SetActive (true);
                targetObj.texture = www.texture;
            }
        }
        
        /// <summary>
        /// Res the panel animate.
        /// </summary>
        /// <param name="target">Target.</param>
        private void PanelAnimate ( GameObject target )
        {
            target.GetComponent<uTweenPosition> ().from = new Vector2 (2500f, 0);
            target.GetComponent<uTweenPosition> ().delay    = 0.001f;
            target.GetComponent<uTweenPosition> ().duration = 0.25f;
            target.GetComponent<uTweenPosition> ().to = Vector3.zero;
            target.GetComponent<uTweenPosition> ().ResetToBeginning ();
            target.GetComponent<uTweenPosition> ().enabled = true;
        }
 
        /// <summary>
        /// Backs the button.
        /// </summary>
        /// <param name="fromObj">From object.</param>
        /// <param name="toObj">To object.</param>
        private void BackButton (GameObject fromObj) 
        {
            fromObj.GetComponent<uTweenPosition> ().delay    = 0.001f;
            fromObj.GetComponent<uTweenPosition> ().duration = 0.25f;
            fromObj.GetComponent<uTweenPosition> ().to      = new Vector2 (2500f,0);
            fromObj.GetComponent<uTweenPosition> ().from    = Vector3.zero;
            fromObj.GetComponent<uTweenPosition> ().ResetToBeginning ();
            fromObj.GetComponent<uTweenPosition> ().enabled = true;
        }
       
        
        #endregion

    }
}