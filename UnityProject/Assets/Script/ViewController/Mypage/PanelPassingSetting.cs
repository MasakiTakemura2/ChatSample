using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using EventManager;
using UnityEngine.UI;
using uTools;
using ModelManager;
using Http;

namespace ViewController
{
    /// <summary>
    /// Panel passing setting.
    /// </summary>
    public class PanelPassingSetting : SingletonMonoBehaviour<PanelPassingSetting>
    {
        #region SerializeField Variable
        [SerializeField]
        private GameObject _panelPassingSetting;

        [SerializeField]
        private Dropdown _passing;
        
        [SerializeField]
        private Dropdown _notification;

        [SerializeField]
        private Dropdown _gender;

        [SerializeField]
        private Dropdown _ageTo;

        [SerializeField]
        private Dropdown _ageFrom;
        
        [SerializeField]
        private Dropdown _heightTo;
        
        [SerializeField]
        private Dropdown _heightFrom;

        [SerializeField]
        private Dropdown _isImageListObject;

        [SerializeField]
        private Dropdown _bodyType;
        
        [SerializeField]
        private Dropdown _radius;
        
        [SerializeField]
        private InputField _keyword;

        [SerializeField]
        private Dropdown _isSendMessage;

        [SerializeField]
        private Text _message;
        
        [SerializeField]
        private GameObject _passingNative;
        
        [SerializeField]
        private GameObject _notificationNative;

        [SerializeField]
        private GameObject _genderNative;

        [SerializeField]
        private GameObject _ageToNative;

        [SerializeField]
        private GameObject _ageFromNative;
        
        [SerializeField]
        private GameObject _heightToNative;
        
        [SerializeField]
        private GameObject _heightFromNative;

        [SerializeField]
        private GameObject _isImageListObjectNative;

        [SerializeField]
        private GameObject _bodyTypeNative;
        
        [SerializeField]
        private GameObject _radiusNative;

        [SerializeField]
        private GameObject _isSendMessageNative;
        
        [SerializeField]
        private GameObject _panelProfileInput;
        #endregion

        public bool _isPanelPassingOpen = false;
        public SetPassingConfigPostEntity.Post _setpassingConfigPost;
        public CurrentProfSettingStateType _currentProfSettingState;

        #region ドロップダウンのデータをキャッチする用のリスト
        List<string> _genderList          = new List<string> ();
        List<string> _bodyTypeList        = new List<string> ();
        List<string> _ageFromList         = new List<string> ();
        List<string> _ageToList           = new List<string> ();
        List<string> _heightFromList      = new List<string> ();
        List<string> _heightToList        = new List<string> ();
        List<string> _radiusList          = new List<string> ();
        #endregion


        //APIにて送信して保存する設定するデータ定義。
        public string _isPassingPost;      //すれ違い通信の可否。
        public string _isNotificationPost; //プッシュ通知の可否。
        public string _sexCdPost;      //男女。
        public string _ageFromPost;   //年齢から〜。
        public string _ageToPost;     //年齢〜まで。
        public string _heightFromPost;//身長から〜。
        public string _heightToPost;  //身長まで。
        public string _bodyTypePost;  //体型。
        public string _isImagePost;   //画像の有無。
        public string _radiusPost;     //自分の位置からの距離。
        public string _keywordPost;    //キーワード。
        public string _isSendMessagePost; //すれ違い時に送信するか否か。
        public string _messagePost;    //すれ違い時に送信するメッセージ。

        private int _bodyTypeValueSet;
        private int _ageFromValueSet;
        private int _ageToValueSet;
        private int _heightFromValueSet;
        private int _heightToValueSet;
        private int _radiusValueSet;


        /// <summary>
        /// Passing setting.
        /// ピッカーセットするリストをEnumに。
        /// </summary>
        private enum PassingSettingType
        {
            IS_PASSING,
            IS_SEND_PUSH,
            GENDER,
            AGE_FROM,
            AGE_TO,
            HEIGHT_FROM,
            HEIGHT_TO,
            BODY_TYPE,
            IS_IMAGE,
            RADIUS,
            IS_SEND_MESSAGE
        }


        /// <summary>
        /// Init this instance.
        /// </summary>
        public void Init ()
        {
            _bodyTypeValueSet   = 0;
            _ageToValueSet      = 0;
            _ageFromValueSet    = 0;
            _heightFromValueSet = 0;
            _heightToValueSet   = 0;
            _radiusValueSet     = 0;

            MypageEventManager.Instance._currentProfSettingState = CurrentProfSettingStateType.None;

            if (GetUserApi._httpCatchData != null)
            {
                if (GetUserApi._httpCatchData.result.user.passing_config == null)
                {
                   //取得したデータからデータ・セットするドロップデータの中身を一旦クリーンにする。
                    _gender.options.Clear ();
                    _gender.ClearOptions ();

                    _ageTo.options.Clear ();
                    _ageTo.ClearOptions ();

                    _ageFrom.options.Clear ();
                    _ageFrom.ClearOptions ();

                    _heightTo.options.Clear ();
                    _heightTo.ClearOptions ();

                    _heightFrom.options.Clear ();
                    _heightFrom.ClearOptions ();

                    _bodyType.options.Clear ();
                    _bodyType.ClearOptions ();

                    _radius.options.Clear ();
                    _radius.ClearOptions (); 

                    _genderList.Add ("指定しない");

                    foreach (var d in InitDataApi._httpCatchData.result.sex_cd) {
                        _genderList.Add (d.name);
                    }

                    _gender.AddOptions (_genderList);

                    string gender = "1";
                    if (AppStartLoadBalanceManager._gender == ((int)GenderType.Male).ToString ()) {
                        gender = ((int)GenderType.Male).ToString ();
                    } else if (AppStartLoadBalanceManager._gender == ((int)GenderType.FeMale).ToString ()) {
                        gender = ((int)GenderType.FeMale).ToString ();
                    }

                    var bodyType = CommonModelHandle.GetNameMaster (gender, CurrentProfSettingStateType.BodyType);

                    _bodyTypeList.Add ("指定しない");

                    foreach (var d in bodyType) {
                        _bodyTypeList.Add (d.name);
                    }

                    _bodyType.AddOptions (_bodyTypeList);

                    _ageFromList = new List<string> ();
                    _ageFromList.Add ("18");


                    for (int i = 18; i <= 100; i++) {
                        _ageFromList.Add (i.ToString ());
                    }

                    _ageFrom.AddOptions (_ageFromList);


                    _ageToList = new List<string> ();
                    _ageToList.Add ("100");
                            
                    for (int i = 18; i <= 100; i++) {
                        _ageToList.Add (i.ToString ());
                    }

                    _ageTo.AddOptions (_ageToList);

                    _heightFromList = new List<string> ();

                    _heightFromList.Add ("110"); //初期値    

                    for (int i = 110; i <= 220; i++) {
                        _heightFromList.Add (i.ToString ());
                    }

                    _heightFrom.AddOptions (_heightFromList);

                    _heightToList = new List<string> ();
                    _heightToList.Add ("220");

                    for (int i = 110; i <= 220; i++) {
                        _heightToList.Add (i.ToString ());
                    }
                    _heightTo.AddOptions (_heightToList);

                    //距離 - ここから
                    _radiusList = new List<string> ();
                    _radiusList.Add ("指定しない");

                    foreach (var d in InitDataApi._httpCatchData.result.radius) {
                        _radiusList.Add (d.name);
                    }
                    _radius.AddOptions(_radiusList);
                    //距離 - ここまで

                } else {
                    Debug.Log ("Passing Configのデータが存在する場合の処理。");
                    UserDataEntity.MyPassingSetting passingConfig = GetUserApi._httpCatchData.result.user.passing_config;

                    if (string.IsNullOrEmpty (passingConfig.is_passing) == false) {
                        //取得したデータからデータ・セットするドロップデータの中身を一旦クリーンにする。
                        _gender.options.Clear ();
                        _gender.ClearOptions ();

                        _ageTo.options.Clear ();
                        _ageTo.ClearOptions ();

                        _ageFrom.options.Clear ();
                        _ageFrom.ClearOptions ();

                        _heightTo.options.Clear ();
                        _heightTo.ClearOptions ();

                        _heightFrom.options.Clear ();
                        _heightFrom.ClearOptions ();

                        _bodyType.options.Clear ();
                        _bodyType.ClearOptions ();

                        _radius.options.Clear ();
                        _radius.ClearOptions ();

                        if (passingConfig.is_passing == "0") {
                            _passing.captionText.text = "すれ違い通信しない";
                            _passing.value = 1;
                        } else if (passingConfig.is_passing == "1") {
                            _passing.captionText.text = "すれ違い通信する";
                            _passing.value = 0;
                        }

                        if (passingConfig.is_notification == "0") {
                            _notification.captionText.text = "Off";
                            _notification.value = 1;
                        } else if (passingConfig.is_notification == "1") {
                            _notification.captionText.text = "On";
                            _notification.value = 0;
                        }

                        //性別 - 選択 ここから ----------------------------------------------------------------- 
                        _genderList = new List<string> ();

                        _genderList.Add ("指定しない");
                        foreach (var d in InitDataApi._httpCatchData.result.sex_cd) {
                            _genderList.Add (d.name);
                        }


                        _gender.AddOptions (_genderList);

                        if (string.IsNullOrEmpty (passingConfig.sex_cd) == false) {
                            Debug.Log (passingConfig.sex_cd + " 性別　どっち？ ");
                            if (passingConfig.sex_cd == ((int)GenderType.Male).ToString ()) {
                                _gender.captionText.text = LocalMsgConst.GENDER_MALE;
                            } else if (passingConfig.sex_cd == ((int)GenderType.FeMale).ToString ()) {
                                _gender.captionText.text = LocalMsgConst.GENDER_FEMALE;
                            }

                        } else {
                            _genderList.Add ("指定しない");
                        }

//                        if (string.IsNullOrEmpty (passingConfig.sex_cd) == false) {
//                            int initGenderSet = int.Parse (passingConfig.sex_cd);
//                            _gender.value = initGenderSet;
//                        }
                        //性別 - 選択 ここまで ----------------------------------------------------------------- 

                        string gender = "1";
                        if (AppStartLoadBalanceManager._gender == ((int)GenderType.Male).ToString ()) {
                            gender = ((int)GenderType.Male).ToString ();
                        } else if (AppStartLoadBalanceManager._gender == ((int)GenderType.FeMale).ToString ()) {
                            gender = ((int)GenderType.FeMale).ToString ();
                        }

                        //体型から ----------------------------------------------------------------- 
                        var bodyType = CommonModelHandle.GetNameMaster (gender, CurrentProfSettingStateType.BodyType);
                        if (string.IsNullOrEmpty (passingConfig.body_type) == false) {
                            _bodyTypeList.Add (passingConfig.body_type);
                            int c = 0;

                            foreach (var d in bodyType) {
                                c++;
                                if (d.name == passingConfig.body_type) {
                                    _bodyTypeValueSet = c;
                                    break;
                                }
                            }
                        } else {
                            _bodyTypeList.Add ("指定しない");
                        }

                        foreach (var d in bodyType) {
                            _bodyTypeList.Add (d.name);
                        }
                        _bodyTypeList.Add ("指定しない");

                        _bodyType.AddOptions (_bodyTypeList);
                        _bodyType.value = _bodyTypeValueSet;
                        //体型まで ----------------------------------------------------------------- 




                        //年齢 - 何歳から ここから ----------------------------------------------------------------- 
                        if (string.IsNullOrEmpty (passingConfig.age_from) == false) {

                            _ageFromList.Add (passingConfig.age_from);
                            int c = 0;
                            for (int i = 18; i <= 100; i++) {
                                c++;
                                if (i.ToString () == passingConfig.age_from)
                                {
                                    _ageFromValueSet = c;
                                    break;
                                }
                            }
                        } else {
                            _ageFromList.Add ("18");
                        }

                        for (int i = 18; i <= 100; i++) {
                            _ageFromList.Add (i.ToString ());
                        }

                        _ageFrom.AddOptions (_ageFromList);
                        _ageFrom.value = _ageFromValueSet;
                        //年齢 - 何歳から ここまで ----------------------------------------------------------------- 



                        //年齢 - 何歳まで ここから ----------------------------------------------------------------- 
                        _ageToList = new List<string> ();
                        if (string.IsNullOrEmpty (passingConfig.age_to) == false) {
                            _ageToList.Add (passingConfig.age_to);

                            int c = 0;
                            for (int i = 18; i <= 100; i++) {
                                c++;
                                if (i.ToString () == passingConfig.age_to)
                                {
                                    _ageToValueSet = c;
                                    break;
                                }
                            }
                        } else {
                            _ageToList.Add ("100");
                        }
                            
                        for (int i = 18; i <= 100; i++) {
                            _ageToList.Add (i.ToString ());
                        }

                        _ageTo.AddOptions (_ageToList);
                        _ageTo.value = _ageToValueSet;

                        //年齢 - 何歳まで ここまで ----------------------------------------------------------------- 


                        //身長から - ここから ----------------------------------------------------------------- 
                        _heightFromList = new List<string> ();

                        //身長のデータが存在する場合の処理
                        if (string.IsNullOrEmpty (passingConfig.height_from) == false) {
                            _heightFromList.Add (passingConfig.height_from);
                            int c = 0;
                            for (int i = 110; i <= 220; i++) {
                                c++;
                                if (i.ToString () == passingConfig.height_from) {
                                    _heightFromValueSet = c;
                                    break;
                                }
                            }
                        } else {
                            _heightFromList.Add ("110"); //初期値    
                        }

                        for (int i = 110; i <= 220; i++) {
                            _heightFromList.Add (i.ToString ());
                        }
                        _heightFrom.AddOptions (_heightFromList);
                        _heightFrom.value = _heightFromValueSet;
                        //身長から - ここまで ----------------------------------------------------------------- 



                         //身長まで - ここから
                        _heightToList = new List<string> ();
                        if (string.IsNullOrEmpty (passingConfig.height_to) == false) {
                            _heightToList.Add (passingConfig.height_to);
                            int c = 0;

                            for (int i = 110; i <= 220; i++) {
                                c++;
                                if (i.ToString () == passingConfig.height_to) {
                                    _heightToValueSet = c;
                                    break;
                                }
                            }
                        } else {
                            _heightToList.Add ("220");    
                        }

                        for (int i = 110; i <= 220; i++) {
                            _heightToList.Add (i.ToString ());
                        }
                        _heightTo.AddOptions (_heightToList);
                        _heightTo.value = _heightToValueSet;
                        //身長まで - ここまで


                        //距離 - ここから
                        _radiusList = new List<string> ();
                        //データが存在する場合。
                        if (string.IsNullOrEmpty (passingConfig.radius) == false) 
                        {
                            int c = 1;
                            foreach (var d in InitDataApi._httpCatchData.result.radius) {
                                if (d.id == passingConfig.radius) {
                                    _radiusList.Add (d.name);
                                    _radiusValueSet =  c;
                                    break;
                                }
                                c++;
                            }
                        } else {
                            _radiusList.Add ("指定しない");    
                        }

                        foreach (var d in InitDataApi._httpCatchData.result.radius) {
                            _radiusList.Add (d.name);
                        }
                        _radius.AddOptions(_radiusList);
                        _radius.value = _radiusValueSet;
                        //距離 - ここまで


                         //キーワード
                         if (string.IsNullOrEmpty (passingConfig.keyword) == false) 
                         {
                             _keyword.text                 = passingConfig.keyword;
                             _setpassingConfigPost.keyword = passingConfig.keyword;
                         }
                         
                         if (passingConfig.is_send_message == "0") {
                            _isSendMessage.captionText.text = "送信しない";
                            _isSendMessage.value = 1;
                         } else if (passingConfig.is_send_message == "1") {
                            _isSendMessage.captionText.text = "送信する";
                            _isSendMessage.value = 0;
                         }

                         _message.text                 = passingConfig.message;
                         _setpassingConfigPost.message = passingConfig.message;
                    }                
                }
             }
        }

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



        #region Native Picker用の処理
        /// <summary>
        /// Natives the init.
        /// </summary>
        /// <returns>The init.</returns>
        public void NativeInit() 
        {
            MypageEventManager.Instance._currentProfSettingState = CurrentProfSettingStateType.None;

            //Editor用で使用しているオブジェクトを非アクティブに
            _passing.gameObject.SetActive (false);
            _notification.gameObject.SetActive (false);
            _gender.gameObject.SetActive (false);
            _ageTo.gameObject.SetActive (false);
            _ageFrom.gameObject.SetActive (false);
            _heightTo.gameObject.SetActive (false);
            _heightFrom.gameObject.SetActive (false);
            _isImageListObject.gameObject.SetActive (false);
            _bodyType.gameObject.SetActive (false);
            _radius.gameObject.SetActive (false);
            _isSendMessage.gameObject.SetActive (false);

            //ピッカー用のオブジェクトをアクティブに
            _passingNative.SetActive (true);
            _notificationNative.SetActive (true);
            _genderNative.SetActive (true);
            _ageToNative.SetActive (true);
            _ageFromNative.SetActive (true);
            _heightToNative.SetActive (true);
            _heightFromNative.SetActive (true);
            _isImageListObjectNative.SetActive (true);
            _bodyTypeNative.SetActive (true);
            _radiusNative.SetActive (true);
            _isSendMessageNative.SetActive (true);

            if (GetUserApi._httpCatchData.result.user.passing_config != null)
            {
                var passingConfig = GetUserApi._httpCatchData.result.user.passing_config;

                _passingNative.SetActive (true);
                _notificationNative.SetActive (true);
                _genderNative.SetActive (true);
                _ageToNative.SetActive (true);
                _ageFromNative.SetActive (true);
                _heightToNative.SetActive (true);
                _heightFromNative.SetActive (true);
                _isImageListObjectNative.SetActive (true);
                
                _bodyTypeNative.SetActive (true);
                _radiusNative.SetActive (true);
                _isSendMessageNative.SetActive (true);

                //すれ違いするか？しないか？
                if (string.IsNullOrEmpty (passingConfig.is_passing) == false)
                {
                    if (passingConfig.is_passing == "0") {
                        _passingNative.transform.GetChild (0).gameObject.SetActive(false);
                        _passingNative.transform.GetChild (1).gameObject.SetActive(true);
                        _passingNative.transform.GetChild (1).GetComponent<Text> ().text = "すれ違い通信しない";
                    } else if (passingConfig.is_passing == "1") {
                        _passingNative.transform.GetChild (0).gameObject.SetActive(false);
                        _passingNative.transform.GetChild (1).gameObject.SetActive(true);
                        _passingNative.transform.GetChild (1).GetComponent<Text> ().text = "すれ違い通信する";
                    }
                    _setpassingConfigPost.isPassing = passingConfig.is_passing;
                }

                //すれ違い時のプッシュ通知を受け取るか？受け取らないか？
                if (string.IsNullOrEmpty (passingConfig.is_notification) == false)
                {
                    if (passingConfig.is_notification == "0") {
                        _notificationNative.transform.GetChild (0).gameObject.SetActive(false);
                        _notificationNative.transform.GetChild (1).gameObject.SetActive(true);
                        _notificationNative.transform.GetChild (1).gameObject.GetComponent<Text>().text = "通知しない";
                    } else if (passingConfig.is_notification == "1") {
                        _notificationNative.transform.GetChild (0).gameObject.SetActive(false);
                        _notificationNative.transform.GetChild (1).gameObject.SetActive(true);
                        _notificationNative.transform.GetChild (1).gameObject.GetComponent<Text>().text = "通知する";
                    }
                    _setpassingConfigPost.isNotification = passingConfig.is_notification;
                }

                //すれ違いたい相手の性別を判定。
                if (string.IsNullOrEmpty (passingConfig.sex_cd) == false) {
                    if (passingConfig.sex_cd == ((int)GenderType.Male).ToString ()) {
                        _genderNative.transform.GetChild (0).gameObject.SetActive(false);
                        _genderNative.transform.GetChild (1).gameObject.SetActive(true);
                        _genderNative.transform.GetChild (1).gameObject.GetComponent<Text>().text = LocalMsgConst.GENDER_MALE;
                    } else if (passingConfig.sex_cd == ((int)GenderType.FeMale).ToString ()) {
                        _genderNative.transform.GetChild (0).gameObject.SetActive(false);
                        _genderNative.transform.GetChild (1).gameObject.SetActive(true);
                        _genderNative.transform.GetChild (1).gameObject.GetComponent<Text>().text = LocalMsgConst.GENDER_FEMALE;
					} else {
						_genderNative.transform.GetChild (0).gameObject.SetActive(false);
						_genderNative.transform.GetChild (1).gameObject.SetActive(true);
						_genderNative.transform.GetChild (1).gameObject.GetComponent<Text>().text = "指定しない";
					}
                    _setpassingConfigPost.sexCd = passingConfig.sex_cd;
                }

                //すれ違い時の条件（年齢）から〜
                if (string.IsNullOrEmpty (passingConfig.age_from) == false) {
                    _ageFromNative.transform.GetChild (0).gameObject.SetActive(false);
                    _ageFromNative.transform.GetChild (1).gameObject.SetActive(true);
                    _ageFromNative.transform.GetChild (1).gameObject.GetComponent<Text>().text = passingConfig.age_from;

                    _setpassingConfigPost.ageFrom = passingConfig.age_from;
                }

                //すれ違い時の条件（年齢）まで〜
                if (string.IsNullOrEmpty (passingConfig.age_to) == false) {
                    _ageToNative.transform.GetChild (0).gameObject.SetActive(false);
                    _ageToNative.transform.GetChild (1).gameObject.SetActive(true);
                    _ageToNative.transform.GetChild (1).gameObject.GetComponent<Text>().text = passingConfig.age_to;
                    _setpassingConfigPost.ageTo = passingConfig.age_to;
                }
                
                //すれ違いの通信の条件、（身長）から〜
                if (string.IsNullOrEmpty (passingConfig.height_from) == false) {
                    _heightFromNative.transform.GetChild (0).gameObject.SetActive(false);
                    _heightFromNative.transform.GetChild (1).gameObject.SetActive(true);
                    _heightFromNative.transform.GetChild (1).gameObject.GetComponent<Text>().text = passingConfig.height_from;
                    _setpassingConfigPost.heightFrom = passingConfig.height_from;
                }
                
                //すれ違いの通信の条件、（身長）まで〜
                if (string.IsNullOrEmpty (passingConfig.height_to) == false) {
                    _heightToNative.transform.GetChild (0).gameObject.SetActive(false);
                    _heightToNative.transform.GetChild (1).gameObject.SetActive(true);
                    _heightToNative.transform.GetChild (1).gameObject.GetComponent<Text>().text = passingConfig.height_to;
                    _setpassingConfigPost.heightTo = passingConfig.height_to;
                }
                
                //すれ違い通信の条件 (画像ありか？画像なしか？)
                if (string.IsNullOrEmpty (passingConfig.is_image) == false)
                {
                    if (passingConfig.is_image == "0"){
                        _isImageListObjectNative.transform.GetChild (0).gameObject.SetActive(false);
                        _isImageListObjectNative.transform.GetChild (1).gameObject.SetActive(true);
                        _isImageListObjectNative.transform.GetChild (1).GetComponent<Text> ().text = "画像なし";
                    } else if (passingConfig.is_image == "1") {
                        _isImageListObjectNative.transform.GetChild (0).gameObject.SetActive(false);
                        _isImageListObjectNative.transform.GetChild (1).gameObject.SetActive(true);
                        _isImageListObjectNative.transform.GetChild (1).GetComponent<Text> ().text = "画像あり";
					} else {
						_isImageListObjectNative.transform.GetChild (0).gameObject.SetActive(false);
						_isImageListObjectNative.transform.GetChild (1).gameObject.SetActive(true);
						_isImageListObjectNative.transform.GetChild (1).GetComponent<Text> ().text = "指定しない";
					}
                    _setpassingConfigPost.isImage = passingConfig.is_image;
                }

                //すれ違い通信の条件 (体型)
                if (string.IsNullOrEmpty (passingConfig.body_type) == false)
                {
                    _bodyTypeNative.transform.GetChild (0).gameObject.SetActive(false);
                    _bodyTypeNative.transform.GetChild (1).gameObject.SetActive(true);
                    _bodyTypeNative.transform.GetChild (1).GetComponent<Text> ().text = passingConfig.body_type;
                    _setpassingConfigPost.bodyType = passingConfig.body_type;
                }

                //すれ違い通信の条件 （距離）
                if (string.IsNullOrEmpty (passingConfig.radius) == false) {
                    var radius = CommonModelHandle.GetByIdBaseData (passingConfig.radius, CurrentProfSettingStateType.Radius);
                    
                    _radiusNative.transform.GetChild (0).gameObject.SetActive(false);
                    _radiusNative.transform.GetChild (1).gameObject.SetActive(true);
                    _radiusNative.transform.GetChild (1).GetComponent<Text> ().text = radius.name;
                    _setpassingConfigPost.radius = passingConfig.radius;
                }

                //メッセージを送信するか？否か？！
                if (string.IsNullOrEmpty (passingConfig.is_send_message) == false) {
                    if (passingConfig.is_send_message == "0") {
                        _isSendMessageNative.transform.GetChild (0).gameObject.SetActive(false);
                        _isSendMessageNative.transform.GetChild (1).gameObject.SetActive(true);
                        _isSendMessageNative.transform.GetChild (1).GetComponent<Text> ().text = "送信しない";
                    } else if (passingConfig.is_send_message == "1") {
                        _isSendMessageNative.transform.GetChild (0).gameObject.SetActive(false);
                        _isSendMessageNative.transform.GetChild (1).gameObject.SetActive(true);
                        _isSendMessageNative.transform.GetChild (1).GetComponent<Text> ().text = "送信する";
                    }
                    _setpassingConfigPost.isSendMessage = passingConfig.is_send_message;
                }
                
                 //すれ違いの時に引っかかる、キーワード
                 if (string.IsNullOrEmpty (passingConfig.keyword) == false) 
                 {
                     _keyword.text                 = passingConfig.keyword;
                     _setpassingConfigPost.keyword = passingConfig.keyword;
                 }
                 
                 //すれ違いの際に送信するメッセージ。
                 //if (string.IsNullOrEmpty (passingConfig.is_send_message) == false) 
                 //{
                     if (passingConfig.is_send_message == "1")
                     {
                         _message.text                 = passingConfig.message;
                         _setpassingConfigPost.message = passingConfig.message;
                     } else {
                        _message.text                  = "";
                     }
                 //}
            }
        }

        /// <summary>
        /// Natives the picker button.
        /// コンポーネントに貼り付けるようのスクリプト
        /// </summary>
        /// <returns>The picker button.</returns>
        /// <param name="state">State.</param>
        public void NativePickerButton(string state)
        {
            PassingSettingType psType = (PassingSettingType)System.Enum.Parse(typeof(PassingSettingType), state);
            
            string [] itemList = {""};
            List<string> list = new List<string> ();

#if UNITY_IOS
list.Add("");
#endif
            
            switch (psType) {
                case PassingSettingType.IS_PASSING:
                    list.Add ("すれ違い通信しない");
                    list.Add ("すれ違い通信する");
                break;
                case PassingSettingType.IS_SEND_PUSH:
                    list.Add ("通知しない");
                    list.Add ("通知する");
                break;
                case PassingSettingType.GENDER:
                    list.Add ("指定しない");
                    list.Add ("女性");
				    list.Add ("男性");
                break;
                case PassingSettingType.AGE_FROM:
                    for (int i = 18; i <= 100; i++) {
                        list.Add (i.ToString());
                    }
                break;
                case PassingSettingType.AGE_TO:
                    for (int i = 18; i <= 100; i++) {
                        list.Add (i.ToString());
                    }
                break;
                case PassingSettingType.HEIGHT_FROM:
                    for (int i = 110; i <= 220; i++) {
                        list.Add (i.ToString ());
                    }
                break;
                case PassingSettingType.HEIGHT_TO:
                    for (int i = 110; i <= 220; i++) {
                        list.Add (i.ToString ());
                    }
                break;
                case PassingSettingType.BODY_TYPE:
                    var bodyType = CommonModelHandle.GetNameMaster (AppStartLoadBalanceManager._gender, CurrentProfSettingStateType.BodyType);
				    list.Add ("指定しない");
                    foreach (var b in bodyType) {
                        list.Add (b.name);
                    }
                break;
                case PassingSettingType.IS_IMAGE:
				    list.Add ("指定しない");
                    list.Add ("画像なし");
                    list.Add ("画像あり");
                break;
                case PassingSettingType.RADIUS:
				    list.Add ("指定しない");
                    foreach (var radius in InitDataApi._httpCatchData.result.radius) {
                        list.Add (radius.name);
                    }
                break;
                case PassingSettingType.IS_SEND_MESSAGE:
                    list.Add ("送信しない");
                    list.Add ("送信する");
                break;
            }
            
            if (list.Count > 0) {
                itemList = list.ToArray ();

                NativePicker.Instance.ShowCustomPicker (toScreenRect (_drawRect), itemList, 0, (long val) => {
                    for (int i = 0; i < list.Count; i++ ) {
                       if ((int)val == i) {
                            DisplayDataSet (psType, i, list[i]);
                            break;
                       }
                    }
                    Debug.Log ("ピッカーの値。 " + val);
                }, () => {
                 Debug.Log ("ピッカーをキャンセルにした場合。");
                });
            }
        }
        
        /// <summary>
        /// Displaies the data set.
        /// </summary>
        /// <returns>The data set.</returns>
        /// <param name="psType">State type.</param>
        /// <param name="key">Key.</param>
        /// <param name="value">Value.</param>
        private void DisplayDataSet (PassingSettingType psType, int key, string value) {
			#if UNITY_IOS
			key = key-1;
			#endif
            switch (psType) {
            case PassingSettingType.IS_PASSING:
                _setpassingConfigPost.isPassing = key.ToString ();
                _passingNative.transform.GetChild (0).gameObject.SetActive (false);
                _passingNative.transform.GetChild (1).GetComponent<Text> ().text = value;
                break;
            case PassingSettingType.IS_SEND_PUSH:
                _setpassingConfigPost.isNotification = key.ToString ();
                _notificationNative.transform.GetChild (0).gameObject.SetActive (false);
                _notificationNative.transform.GetChild (1).GetComponent<Text> ().text = value;
                break;
            case PassingSettingType.GENDER:
				if (key == 0) {
					_setpassingConfigPost.sexCd = "0";
					_genderNative.transform.GetChild (0).gameObject.SetActive (false);
					_genderNative.transform.GetChild (1).GetComponent<Text> ().text = value;					
				} else {
					var gender = CommonModelHandle.GetByNameBaseData (value, CurrentProfSettingStateType.Gender);
					_setpassingConfigPost.sexCd = gender.id;
					_genderNative.transform.GetChild (0).gameObject.SetActive (false);
					_genderNative.transform.GetChild (1).GetComponent<Text> ().text = value;
				}
					
                break;
            case PassingSettingType.AGE_FROM:
                _setpassingConfigPost.ageFrom = value;
                _ageFromNative.transform.GetChild (0).gameObject.SetActive (false);
                _ageFromNative.transform.GetChild (1).GetComponent<Text> ().text = value;
                break;
            case PassingSettingType.AGE_TO:
                _setpassingConfigPost.ageTo = value;
                _ageToNative.transform.GetChild (0).gameObject.SetActive (false);
                _ageToNative.transform.GetChild (1).GetComponent<Text> ().text = value;
                break;
            case PassingSettingType.HEIGHT_FROM:
                _setpassingConfigPost.heightFrom = value;
                _heightFromNative.transform.GetChild (0).gameObject.SetActive (false);
                _heightFromNative.transform.GetChild (1).GetComponent<Text> ().text = value;
                break;
            case PassingSettingType.HEIGHT_TO:
                _setpassingConfigPost.heightTo = value;
                _heightToNative.transform.GetChild (0).gameObject.SetActive (false);
                _heightToNative.transform.GetChild (1).GetComponent<Text> ().text = value;
                break;
            case PassingSettingType.BODY_TYPE:
				if (key == 0) {
					_setpassingConfigPost.bodyType = "";
					_bodyTypeNative.transform.GetChild (0).gameObject.SetActive (false);
					_bodyTypeNative.transform.GetChild (1).GetComponent<Text> ().text = "指定しない";
				} else {
					_setpassingConfigPost.bodyType = value;
					_bodyTypeNative.transform.GetChild (0).gameObject.SetActive (false);
					_bodyTypeNative.transform.GetChild (1).GetComponent<Text> ().text = value;	
				}
                break;
            case PassingSettingType.IS_IMAGE:
				if (key == 0) {
					_isImageListObjectNative.transform.GetChild (0).gameObject.SetActive (false);
					_isImageListObjectNative.transform.GetChild (1).GetComponent<Text> ().text = "指定しない";
				} else {
					_setpassingConfigPost.isImage = (key-1).ToString ();
					_isImageListObjectNative.transform.GetChild (0).gameObject.SetActive (false);
					_isImageListObjectNative.transform.GetChild (1).GetComponent<Text> ().text = value;
				}

                break;
             case PassingSettingType.RADIUS:
				if (key == 0) {
					_setpassingConfigPost.radius = "";
					_radiusNative.transform.GetChild (0).gameObject.SetActive (false);
					_radiusNative.transform.GetChild (1).GetComponent<Text> ().text = "指定しない";					
				} else {
					var r = CommonModelHandle.GetByNameBaseData (value, CurrentProfSettingStateType.Radius);
					if (r != null) {
						_setpassingConfigPost.radius = r.id;
						_radiusNative.transform.GetChild (0).gameObject.SetActive (false);
						_radiusNative.transform.GetChild (1).GetComponent<Text> ().text = value;	
					} else {
						_setpassingConfigPost.radius = "";
						_radiusNative.transform.GetChild (0).gameObject.SetActive (false);
						_radiusNative.transform.GetChild (1).GetComponent<Text> ().text = "指定しない";
					}
				}
	
                break;
             case PassingSettingType.IS_SEND_MESSAGE:
                _setpassingConfigPost.isSendMessage = key.ToString ();
                _isSendMessageNative.transform.GetChild (0).gameObject.SetActive (false);
                _isSendMessageNative.transform.GetChild (1).GetComponent<Text> ().text = value;
                break;
            }
        }
        
        #endregion

        /// <summary>
        /// Sets the post data.
        /// Apiでセットするポストデータ
        /// </summary>
        /// <returns>The post data.</returns>
        public void SetPostData ()
        {
Debug.Log (" ここからデータセット================================================ ");
            if (_passing.value == 1) {
                _setpassingConfigPost.isPassing = "0";
            } else if (_passing.value == 0) {
                _setpassingConfigPost.isPassing = "1";
            }

Debug.Log (_setpassingConfigPost.isPassing + " _passing.value " + _passing.value);

            if (_notification.value == 1) {
                _setpassingConfigPost.isNotification = "0";
            } else if (_notification.value == 0) {
                _setpassingConfigPost.isNotification = "1";
            }
           
Debug.Log (_setpassingConfigPost + " _notification.value ");

            if (_genderList.Count > 0) {
                if (_gender.value != 0) {
                    string n = _genderList[_gender.value];
                
                    var d = CommonModelHandle.GetByNameBaseData (n, CurrentProfSettingStateType.Gender);
                    _setpassingConfigPost.sexCd = d.id;
                } else {
                    _setpassingConfigPost.sexCd = "1";
                }

Debug.Log (_setpassingConfigPost.sexCd + " 性別のデータ。  ");
            } else {
                _setpassingConfigPost.sexCd = "";
            }

            if (_ageFromList.Count > 0) {
                _setpassingConfigPost.ageFrom = _ageFromList [_ageFrom.value];
Debug.Log (_setpassingConfigPost.ageFrom + " 年齢から ");
            } else {
                _setpassingConfigPost.ageFrom = "";
            }
                

            if (_ageToList.Count > 0) {
                _setpassingConfigPost.ageTo = _ageToList [_ageTo.value];
Debug.Log (_setpassingConfigPost.ageTo + " 年齢まで ");
            } else {
                _setpassingConfigPost.ageTo = "";
            }

            if (_heightFromList.Count > 0) {
                _setpassingConfigPost.heightFrom = _heightFromList [_heightFrom.value];
Debug.Log (_setpassingConfigPost.heightFrom + "身長から");

            } else {
                _setpassingConfigPost.heightFrom  = "";
            }

            if (_heightToList.Count > 0) {
                _setpassingConfigPost.heightTo = _heightToList [_heightTo.value];
                Debug.Log (_setpassingConfigPost.heightTo + " 身長まで ");
            } else {
                _setpassingConfigPost.heightTo = "";
            }

            //体型
            if (_bodyTypeList.Count > 0) {
                if (_bodyTypeList [_bodyType.value] == "指定しない") {
                    _setpassingConfigPost.bodyType = "";
                } else {
                    _setpassingConfigPost.bodyType = _bodyTypeList [_bodyType.value];    
                }
Debug.Log (_setpassingConfigPost.heightTo + " 体型 ");
            } else {
                _setpassingConfigPost.bodyType = "";
            }

            //距離
            if (_radiusList.Count > 0) {
                if (_radius.value != 0) {
                    string n = _radiusList [_radius.value];
                    var d = CommonModelHandle.GetByNameBaseData (n, CurrentProfSettingStateType.Radius);
                    _setpassingConfigPost.radius = d.id;
                } else {
                    _setpassingConfigPost.radius = "";
                }
            }


            _setpassingConfigPost.isImage = _isImageListObject.value.ToString ();
            
Debug.Log (_setpassingConfigPost.isImage + " _isImage.value ");

            if (_isSendMessage.value == 1) 
            {
                _setpassingConfigPost.isSendMessage = "0";
            } else if (_isSendMessage.value == 0) {
                _setpassingConfigPost.isSendMessage = "1";
            }

Debug.Log (_setpassingConfigPost.isSendMessage + " _passing.value " + _isSendMessage.value);

           _setpassingConfigPost.keyword = _keyword.text;
Debug.Log (_keyword.text + " _keyword.text ");

print(" ここまでデータのセット================================================ ");
            
        }

        /// <summary>
        /// Gets the profile item.
        /// </summary>
        /// <param name="Object">Object.</param>
        public void GetProfileItem (GameObject Object)
        {
            _setpassingConfigPost.message = PanelProfileInput._postMessage;
            _message.text = "";
            _message.text = _setpassingConfigPost.message;

            TemplatePanelClose (PanelProfileInput.Instance.gameObject);
        }

        #region すれ違い設定Open、Close
        /// <summary>
        /// Panels the passing setting open.
        /// </summary>
        /// <returns>The passing setting open.</returns>
        /// <param name="animObj">Animation object.</param>
        public void PanelPassingSettingOpen ( ) {
            #if !UNITY_EDITOR && (UNITY_IOS || UNITY_ANDROID )
            NativeInit ();
            #else
            Init ();
            #endif

            HeaderPanel.Instance.BackButtonSwitch (false);
            HeaderPanel.Instance.BackButtonSwitch (true, PanelPassingSettingCloseEvent);

            OtherSetting.Instance.gameObject.GetComponent<BoxCollider2D> ().enabled = false;

            GameObject obj = GameObject.FindGameObjectWithTag (CommonConstants.BACK_SWIPE);
            obj.GetComponent<SwipeRecognizer>().EventMessageTarget = _panelPassingSetting;

            MypageEventManager.Instance.PanelAnimate (this.gameObject);
            _isPanelPassingOpen = true;

        }
        
        /// <summary>
        /// Panels the passing setting close.
        /// </summary>
        /// <returns>The passing setting close.</returns>
        public void PanelPassingSettingClose () {
            HeaderPanel.Instance.BackButtonSwitch (false);
            HeaderPanel.Instance.BackButtonSwitch (true, OtherSettingCloseEvent);
            OtherSetting.Instance.gameObject.GetComponent<BoxCollider2D> ().enabled = true;
            MypageEventManager.Instance.BackButton (this.gameObject);
            _isPanelPassingOpen = false;
        }
        
        /// <summary>
        /// Panels the passing setting close event.
        /// </summary>
        /// <returns>The passing setting close event.</returns>
        void PanelPassingSettingCloseEvent() {
            PanelPassingSettingClose ();
        }
        
        /// <summary>
        /// Others the setting close event.
        /// </summary>
        /// <returns>The setting close event.</returns>
        void OtherSettingCloseEvent () {
            
            MypageEventManager.Instance.OtherSettingClose(OtherSetting.Instance.gameObject);
        }
        #endregion
        
   
        #region すれ違い時に送るメッセージ
        /// <summary>
        /// Passings the message.
        /// </summary>
        /// <returns>The message.</returns>
        public void PassingMessage()
        {
            PanelProfileInput.Instance.OkButtonSwitch (false);
            PanelProfileInput.Instance._message.text = _setpassingConfigPost.message;
            this.GetComponent<BoxCollider2D> ().enabled = false;
            HeaderPanel.Instance.BackButtonSwitch (false);
            HeaderPanel.Instance.BackButtonSwitch (true, InputPanelCloseEvent);
            MypageEventManager.Instance.PanelAnimate (PanelProfileInput.Instance.gameObject);
        }
        #endregion
        
        
        #region 元に戻ってきた場合のイベント && 共通で使用するテンプレートオブジェクトクローズ処理
        /// <summary>
        /// Templates the panel close.
        /// </summary>
        /// <returns>The panel close.</returns>
        /// <param name="animObj">Animation object.</param>
        public void TemplatePanelClose( GameObject animObj ) 
        {
            MypageEventManager.Instance.CleanTemplate ();
            this.GetComponent<BoxCollider2D> ().enabled = true;
            HeaderPanel.Instance.BackButtonSwitch (false);
            HeaderPanel.Instance.BackButtonSwitch (true, ThisPanelBackEvent);
            MypageEventManager.Instance.BackButton (animObj);
        }
       
        /// <summary>
        /// Inputs the panel close event.
        /// </summary>
        /// <returns>The panel close event.</returns>
        void InputPanelCloseEvent () 
        {
            PanelProfileInput.Instance.OkButtonSwitch (true);
            _setpassingConfigPost.message = PanelProfileInput.Instance._message.text;
            _message.text = _setpassingConfigPost.message;
            TemplatePanelClose (PanelProfileInput.Instance.gameObject);
        }
        
        /// <summary>
        /// Thises the panel back event.
        /// </summary>
        /// <returns>The panel back event.</returns>
        void ThisPanelBackEvent()
        {
            HeaderPanel.Instance.BackButtonSwitch (false);
            HeaderPanel.Instance.BackButtonSwitch (true, BackOtherSettingEvent);
            GameObject obj = GameObject.FindGameObjectWithTag (CommonConstants.BACK_SWIPE);
            obj.GetComponent<SwipeRecognizer>().EventMessageTarget = _panelPassingSetting;
            MypageEventManager.Instance.BackButton (_panelPassingSetting);
        }
        
        void BackOtherSettingEvent () {
            HeaderPanel.Instance.BackButtonSwitch (false);
            MypageEventManager.Instance.BackButton (OtherSetting.Instance.gameObject);
        }
        #endregion
    
        #region すれ違い通信の設定[API通信 / 登録]ボタン。
        /// <summary>
        /// Passings the setting fixed button.
        /// </summary>
        /// <returns>The setting fixed button.</returns>
        public void PassingSettingFixedButton () {
            #if UNITY_EDITOR
            SetPostData ();
            #elif !UNITY_EDITOR && (UNITY_IOS || UNITY_ANDROID)
                _setpassingConfigPost.keyword = _keyword.text;
                
            #endif

            new SetPassingConfigApi (_setpassingConfigPost);
            StartCoroutine (SetPassingConfigApiWait ());
        }
        
        /// <summary>
        /// Sets the passing config API wait.
        /// </summary>
        /// <returns>The passing config API wait.</returns>
        private IEnumerator SetPassingConfigApiWait ()
        {
            MypageEventManager.Instance.LoadingSwitch (true);

            while (SetPassingConfigApi._success == false)
                yield return (SetPassingConfigApi._success == true);

            MypageEventManager.Instance.LoadingSwitch(false);
            
            PopupPanel.Instance.PopMessageInsert(
                SetPassingConfigApi._httpCatchData.result.complete[0],
                LocalMsgConst.OK,
                SetPassingConfigApiClose
            );
            MypageEventManager.Instance.PanelPopupAnimate (GameObject.FindGameObjectWithTag(CommonConstants.POPUP_BASIC_TAG));
        }
        
        /// <summary>
        /// Reports the cancel.
        /// </summary>
        void SetPassingConfigApiClose ()
        {
            PopupSecondSelectPanel.Instance.PopClean();
            MypageEventManager.Instance.PanelPopupCloseAnimate (GameObject.FindGameObjectWithTag(CommonConstants.POPUP_BASIC_TAG));
        }
        #endregion
         
    
        #region back swipe event
        /// <summary>
        /// Backs the swipe.
        /// </summary>
        void OnSwipe (SwipeGesture gesture)
        {
            if (gesture.Selection) {
                if (gesture.Direction == FingerGestures.SwipeDirection.Left)
                {
                    //Debug.Log ("Left Left Left Left Left Left ");
                }
                else if (gesture.Direction == FingerGestures.SwipeDirection.Right)
                {
                    PanelPassingSettingClose ();
                }
            }
        }
        #endregion
    }
}