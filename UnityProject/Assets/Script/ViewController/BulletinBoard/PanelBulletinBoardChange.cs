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
    public class PanelBulletinBoardChange : SingletonMonoBehaviour<PanelBulletinBoardChange>
    {
        [SerializeField]
        private Dropdown _category;

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
        private GameObject _categoryNative;
        
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
        private GameObject _bodyTypeNative;

        [SerializeField]
        private GameObject _isImageNative;
        
        [SerializeField]
        private GameObject _radiusNative;
        
        
        public string _categoryAPIThrow = "";
        public string _sexAPIThrow  = "";
        public string _ageHighAPIThrow = "";
        public string _ageLowAPIThrow = "";
        public string _heightToAPIThrow = "";
        public string _heightFromAPIThrow = "";
        public string _bodyTypeAPIThrow = "";
        public string _isImageAPIThrow = "";
        public string _radiusAPIThrow = "";
        public string _keywordAPIThrow = "";



        #region Native Pickerで使用する内部変数、関数。
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
        
        /// <summary>
        /// Search condition type.
        /// </summary>
        public enum SearchConditionType
        {
            None,
            Gender,
            AgeHigh,
            AgeLow,
            TallHigh,
            TallLow,
            BodyType,
            IsImage,
            Radius,
            Category
        }
        #endregion

        
        #region Native Picker用(IOS, Android)の初期処理
        /// <summary>
        /// Natives the picker init.
        /// </summary>
        public void NativePickerInit ()
        {
            /// Editor用で使用しているGameobjectをActive falseに
            _category.gameObject.SetActive (false);
            _gender.gameObject.SetActive (false);
            _ageTo.gameObject.SetActive (false);
            _ageFrom.gameObject.SetActive (false);
            _heightTo.gameObject.SetActive (false) ;
            _heightFrom.gameObject.SetActive (false);
            _bodyType.gameObject.SetActive (false);
            _radius.gameObject.SetActive (false);
            _isImageListObject.gameObject.SetActive (false);


            //ピッカー用のオブジェクトをアクティブに
           _categoryNative.SetActive (true);
           _genderNative.SetActive (true);
           _ageToNative.SetActive (true);
           _ageFromNative.SetActive(true);
           _heightToNative.SetActive (true);
           _heightFromNative.SetActive (true);
           _bodyTypeNative.SetActive (true);
           _isImageNative.SetActive (true);
           _radiusNative.SetActive (true);
           StartCoroutine (NaitiveInitCoroutine ());
            
        }
        
        /// <summary>
        /// Naitives the init coroutine.
        /// </summary>
        /// <returns>The init coroutine.</returns>
        private IEnumerator NaitiveInitCoroutine () 
        {
            string _commonFileName = LocalFileConstants.GetLocalFileDir () + LocalFileConstants.COMMON_LOCAL_FILE_NAME;

            LocalFileHandler.Init (_commonFileName);

            /// ファイルが作成されるまでポーリングして処理待ち
            while (System.IO.File.Exists (_commonFileName) == false)
                yield return (System.IO.File.Exists (_commonFileName) == true);

            /// 検索条件をローカルに保存しているので、そこから引いてくる。
            BulletinBoardEventManager.SearchCondition SearchCondition = LocalFileHandler.Load<BulletinBoardEventManager.SearchCondition> (LocalFileConstants.BBS_SEARCH_CONDITION_KEY);

            
            
            /// ローカルに「掲示板カテゴリ」のデータが保存されている場合の処理。
            if (string.IsNullOrEmpty (SearchCondition._CategoryID) == false)
            {
                _categoryNative.transform.GetChild (0).gameObject.SetActive(false);
                _categoryNative.transform.GetChild (1).gameObject.SetActive(true);
                var category = CommonModelHandle.GetByIdBaseData (SearchCondition._CategoryID, CurrentProfSettingStateType.BoardCategory);

                if (category != null) {
                   _categoryNative.transform.GetChild (1).GetComponent<Text> ().text = category.name;
                   _categoryAPIThrow = category.id;
                } else {
                   _categoryNative.transform.GetChild (1).GetComponent<Text> ().text = "指定しない";
                   _categoryAPIThrow = "";
                }
            } else {
                _categoryNative.transform.GetChild (0).gameObject.SetActive(false);
                _categoryNative.transform.GetChild (1).gameObject.SetActive(true);
                _categoryNative.transform.GetChild (1).GetComponent<Text> ().text = "指定しない";
                _categoryAPIThrow = "";
            }
            /// 性別の条件の場合の処理
            if (string.IsNullOrEmpty (SearchCondition._sex) == false)
            {
                _genderNative.transform.GetChild (0).gameObject.SetActive(false);
                _genderNative.transform.GetChild (1).gameObject.SetActive(true);

                var genderText = CommonModelHandle.GetByIdBaseData (SearchCondition._sex, CurrentProfSettingStateType.Gender);
                
                if (genderText != null) {
                    _genderNative.transform.GetChild (1).GetComponent<Text> ().text = genderText.name;
                    _sexAPIThrow = genderText.id;
                } else {
                    _genderNative.transform.GetChild (1).GetComponent<Text> ().text = "指定しない";
                    _sexAPIThrow = "";
                }
            } else {
                _genderNative.transform.GetChild (0).gameObject.SetActive(false);
                _genderNative.transform.GetChild (1).gameObject.SetActive(true);
                _genderNative.transform.GetChild (1).GetComponent<Text> ().text = "指定しない";
                _sexAPIThrow = "";
            }
            
            /// 年齢のまでの場合の処理。
            if (string.IsNullOrEmpty (SearchCondition._ageTo) == false) {
                _ageToNative.transform.GetChild (0).gameObject.SetActive (false);
                _ageToNative.transform.GetChild (1).gameObject.SetActive (true);
                _ageToNative.transform.GetChild (1).GetComponent<Text> ().text = SearchCondition._ageTo;
                _ageHighAPIThrow = SearchCondition._ageTo;
            } else {
                _ageToNative.transform.GetChild (0).gameObject.SetActive (false);
                _ageToNative.transform.GetChild (1).gameObject.SetActive (true);
                _ageToNative.transform.GetChild (1).GetComponent<Text> ().text = "指定しない";
                _ageHighAPIThrow = "";
            }
            
            /// 年齢の〜からの場合の処理
            if (string.IsNullOrEmpty (SearchCondition._ageFrom) == false) {
                    _ageFromNative.transform.GetChild (0).gameObject.SetActive (false);
                    _ageFromNative.transform.GetChild (1).gameObject.SetActive (true);
                    _ageFromNative.transform.GetChild (1).GetComponent<Text> ().text = SearchCondition._ageFrom;
                _ageLowAPIThrow = SearchCondition._ageFrom;
            } else {
                _ageFromNative.transform.GetChild (0).gameObject.SetActive (false);
                _ageFromNative.transform.GetChild (1).gameObject.SetActive (true);
                _ageFromNative.transform.GetChild (1).GetComponent<Text> ().text = "";
                _ageLowAPIThrow = "";
            }

            /// 身長のまでの場合の処理。
            if (string.IsNullOrEmpty (SearchCondition._heightTo) == false) {
                _heightToNative.transform.GetChild (0).gameObject.SetActive (false);
                _heightToNative.transform.GetChild (1).gameObject.SetActive (true);
                _heightToNative.transform.GetChild (1).GetComponent<Text> ().text = SearchCondition._heightTo;
                _heightToAPIThrow = SearchCondition._heightTo;
            } else {
                _heightToNative.transform.GetChild (0).gameObject.SetActive (false);
                _heightToNative.transform.GetChild (1).gameObject.SetActive (true);
                _heightToNative.transform.GetChild (1).GetComponent<Text> ().text = "指定しない";
                _heightToAPIThrow = "";
            }

            /// 身長の〜からの場合の処理。
            if (string.IsNullOrEmpty (SearchCondition._heightFrom) == false) {
                _heightFromNative.transform.GetChild (0).gameObject.SetActive (false);
                _heightFromNative.transform.GetChild (1).gameObject.SetActive (true);
                _heightFromNative.transform.GetChild (1).GetComponent<Text> ().text = SearchCondition._heightFrom;
                _heightFromAPIThrow = SearchCondition._heightFrom;
            } else {
                _heightFromNative.transform.GetChild (0).gameObject.SetActive (false);
                _heightFromNative.transform.GetChild (1).gameObject.SetActive (true);
                _heightFromNative.transform.GetChild (1).GetComponent<Text> ().text = "指定しない";
                _heightFromAPIThrow = "";
            }
            
            /// 体型の条件の場合の初期化処理
            if (string.IsNullOrEmpty (SearchCondition._bodyType) == false) {
                _bodyTypeNative.transform.GetChild (0).gameObject.SetActive (false);
                _bodyTypeNative.transform.GetChild (1).gameObject.SetActive (true);
                _bodyTypeNative.transform.GetChild (1).GetComponent<Text> ().text = SearchCondition._bodyType;
                _bodyTypeAPIThrow = SearchCondition._bodyType;
            } else {
                _bodyTypeNative.transform.GetChild (0).gameObject.SetActive (false);
                _bodyTypeNative.transform.GetChild (1).gameObject.SetActive (true);
                _bodyTypeNative.transform.GetChild (1).GetComponent<Text> ().text = "指定しない";
                _bodyTypeAPIThrow = "";
            }

            ///// 画像あり、なしの条件の場合の処理
            if (string.IsNullOrEmpty (SearchCondition._isImage) == false) {
                _isImageNative.transform.GetChild(0).gameObject.SetActive (false);
                _isImageNative.transform.GetChild(1).gameObject.SetActive (true);

                if (SearchCondition._isImage == "0")
                {
                    _isImageNative.transform.GetChild(1).GetComponent<Text> ().text = "画像なし";
                } else if (SearchCondition._isImage == "1") {
                    _isImageNative.transform.GetChild(1).GetComponent<Text> ().text = "画像あり";
                } else {
                     _isImageNative.transform.GetChild(1).GetComponent<Text> ().text = "指定しない";
                }
                _isImageAPIThrow = SearchCondition._isImage;
            } else {
                _isImageNative.transform.GetChild(0).gameObject.SetActive (false);
                _isImageNative.transform.GetChild(1).GetComponent<Text> ().text = "指定しない";
                _isImageAPIThrow = "";
            }

            ///// 距離の条件の場合の初期化処理。
            if (string.IsNullOrEmpty (SearchCondition._radius) == false) {
                _radiusNative.transform.GetChild (0).gameObject.SetActive (false);
                _radiusNative.transform.GetChild (1).gameObject.SetActive (true);

                var radiusText = CommonModelHandle.GetByIdBaseData (SearchCondition._radius, CurrentProfSettingStateType.Radius);

                if (radiusText != null) {
                    _radiusNative.transform.GetChild (1).GetComponent<Text> ().text = radiusText.name;
                } else {
                    _radiusNative.transform.GetChild (1).GetComponent<Text> ().text = "指定しない";
                }
            } else {
                _radiusNative.transform.GetChild (0).gameObject.SetActive (false);
                _radiusNative.transform.GetChild (1).gameObject.SetActive (true);                
                _radiusNative.transform.GetChild (1).GetComponent<Text> ().text = "指定しない";
            }

            /// キーワード条件の場合の初期化処理
            if (string.IsNullOrEmpty (SearchCondition._keyword) == false)
            {
                _keyword.text = SearchCondition._keyword;
            }
            
            yield break;  
        }
        
        /// <summary>
        /// Natives the picker button.
        /// ネイティブピッカー用のボタン処理。
        /// </summary>
        /// <returns>The picker button.</returns>
        /// <param name="stateType">State type.</param>
        public void NativePickerButton (string state )
        {
        
            SearchConditionType stateType = (SearchConditionType)System.Enum.Parse(typeof(SearchConditionType), state);

            string [] itemList = {""};
            List<string> list = new List<string> ();
#if UNITY_IOS
list.Add("");
#endif
            switch (stateType)
            {
                case SearchConditionType.Category:
                    var bbsCategory = CommonModelHandle.GetNameMaster(CurrentProfSettingStateType.BoardCategory);
                    list.Add ("指定しない");
                    foreach (var o in bbsCategory) {
                        list.Add (o.name);
                    }
                break;

                case SearchConditionType.Gender:
                    list.Add ("指定しない");
                    list.Add ("女性");
                    list.Add ("男性");
                break;

                case SearchConditionType.AgeHigh:
                case SearchConditionType.AgeLow:
                    for (int i = 18; i <= 100; i++) {
                        list.Add (i.ToString());
                    }
                break;
                
                case SearchConditionType.TallHigh:
                case SearchConditionType.TallLow:
                    for (int i = 110; i <= 220; i++) {
                        list.Add (i.ToString ());
                    }
                break;
                
                case SearchConditionType.BodyType:
                    var bodyType = CommonModelHandle.GetNameMaster (AppStartLoadBalanceManager._gender, CurrentProfSettingStateType.BodyType);
                    list.Add ("指定しない");
                    foreach (var b in bodyType) {
                        list.Add (b.name);
                    }
                break;

                case SearchConditionType.IsImage:
                    list.Add ("指定しない");
                    list.Add ("画像なし");
                    list.Add ("画像あり");
                break;

                case SearchConditionType.Radius:
                    list.Add ("指定しない");
                    foreach (var radius in InitDataApi._httpCatchData.result.radius) {
                        list.Add (radius.name);
                    }
                break;
            }
            
            if (list.Count > 0) {
                itemList = list.ToArray ();

                NativePicker.Instance.ShowCustomPicker (toScreenRect (_drawRect), itemList, 0, (long val) => {
                    for (int i = 0; i < list.Count; i++ ) {
                       if ((int)val == i) {
                            DisplayDataSet (stateType, i, list[i]);
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
        /// <param name="stateType">State type.</param>
        /// <param name="key">Key.</param>
        /// <param name="value">Value.</param>
        private void DisplayDataSet (SearchConditionType stateType, int key, string value)
        {
#if UNITY_IOS
key = key-1;
#endif
            switch (stateType)
            {
                case SearchConditionType.Category:
                    if (key == 0) {
                        _categoryAPIThrow = "";
                        _bodyTypeNative.transform.GetChild (0).gameObject.SetActive (false);
                        _categoryNative.transform.GetChild (1).gameObject.GetComponent<Text> ().text = "指定しない";
                    } else {
                        var bbsCategory = CommonModelHandle.GetByNameBaseData (value, CurrentProfSettingStateType.BoardCategory);
                        _categoryAPIThrow = bbsCategory.id;
                        _bodyTypeNative.transform.GetChild (0).gameObject.SetActive (false);
                        _categoryNative.transform.GetChild (1).gameObject.GetComponent<Text> ().text = bbsCategory.name;
                    }
                break;

                case SearchConditionType.Gender:
                    if (key == 0) {
                        _sexAPIThrow = "";
                        _genderNative.transform.GetChild (1).GetComponent<Text> ().text = value;                    
                    } else {
                        var gender = CommonModelHandle.GetByNameBaseData (value, CurrentProfSettingStateType.Gender);
                        _sexAPIThrow = gender.id;
                        _genderNative.transform.GetChild (1).GetComponent<Text> ().text = value;
                    }
                break;

                case SearchConditionType.AgeHigh:
                    _ageHighAPIThrow = value;
                    _ageToNative.transform.GetChild (1).gameObject.GetComponent<Text> ().text = value;
                break;

                case SearchConditionType.AgeLow:
                    _ageLowAPIThrow = value;
                    _ageFromNative.transform.GetChild (1).gameObject.GetComponent<Text> ().text = value;
                break;

                case SearchConditionType.TallHigh:
                    _heightToAPIThrow = value;
                    _heightToNative.transform.GetChild (1).gameObject.GetComponent<Text> ().text = value;
                break;

                case SearchConditionType.TallLow:
                   _heightFromAPIThrow = value;
                   _heightFromNative.transform.GetChild (1).gameObject.GetComponent<Text> ().text = value;
                break;
                
                case SearchConditionType.BodyType:
                if (key == 0) {
                    _bodyTypeAPIThrow = "";
                    _bodyTypeNative.transform.GetChild (0).gameObject.SetActive (false);
                    _bodyTypeNative.transform.GetChild (1).GetComponent<Text> ().text = "指定しない";
                } else {
                    _bodyTypeAPIThrow = value;
                    _bodyTypeNative.transform.GetChild (0).gameObject.SetActive (false);
                    _bodyTypeNative.transform.GetChild (1).GetComponent<Text> ().text = value;  
                }
                break;

                case SearchConditionType.IsImage:
                    if (key == 0) {
                        _isImageAPIThrow = "";
                        _isImageNative.transform.GetChild (0).gameObject.SetActive (false);
                        _isImageNative.transform.GetChild (1).GetComponent<Text> ().text = "指定しない";
                    } else {
                        if ((key - 1) == 0) {
                            _isImageAPIThrow = "0";
                        } else {
                            _isImageAPIThrow = (key-1).ToString ();
                        }
                        
                        _isImageNative.transform.GetChild (0).gameObject.SetActive (false);
                        _isImageNative.transform.GetChild (1).GetComponent<Text> ().text = value;
                    }
                break;

                case SearchConditionType.Radius:
                    if (key == 0) {
                        _radiusAPIThrow = "";
                        _radiusNative.transform.GetChild (0).gameObject.SetActive (false);
                        _radiusNative.transform.GetChild (1).GetComponent<Text> ().text = "指定しない";                  
                    } else {
                        var r = CommonModelHandle.GetByNameBaseData (value, CurrentProfSettingStateType.Radius);
                        if (r != null) {
                            _radiusAPIThrow = r.id;
                            _radiusNative.transform.GetChild (0).gameObject.SetActive (false);
                            _radiusNative.transform.GetChild (1).GetComponent<Text> ().text = value;    
                        } else {
                            _radiusAPIThrow = "";
                            _radiusNative.transform.GetChild (0).gameObject.SetActive (false);
                            _radiusNative.transform.GetChild (1).GetComponent<Text> ().text = "指定しない";
                        }
                    }
                break;
            }
        }
        
        #endregion
        
        
        #region Editor用の初期処理
        /// <summary>
        /// Editor用
        /// Init this instance.
        /// サーバー等から引いてきたデータを初期化セット
        /// </summary>
        public void Init ()
        {
            //初期化処理。
            _category.options.Clear();
            _category.ClearOptions ();

            _gender.options.Clear();
            _gender.ClearOptions ();

            _ageTo.options.Clear();
            _ageTo.ClearOptions ();

            _ageFrom.options.Clear();
            _ageFrom.ClearOptions ();

            _heightTo.options.Clear();
            _heightTo.ClearOptions();

            _heightFrom.options.Clear();
            _heightFrom.ClearOptions();

            _bodyType.options.Clear();
            _bodyType.ClearOptions();

            _radius.options.Clear();
            _radius.ClearOptions ();

            StartCoroutine (InitCoroutine ());
        }
        
        List<string> _categoryList   = new List<string> ();
        List<string> _genderList     = new List<string> ();
        List<string> _bodyTypeList   = new List<string> ();
        List<string> _ageFromList    = new List<string> ();
        List<string> _ageToList      = new List<string> ();
        List<string> _heightFromList = new List<string> ();
        List<string> _heightToList   = new List<string> ();
        List<string> _radiusList     = new List<string> ();
        
        
        /// <summary>
        /// Inits the coroutine.
        /// </summary>
        /// <returns>The coroutine.</returns>
        private IEnumerator InitCoroutine ()
        {
            //ここからマスターデータを当てるDropdownに当てる処理 - ここから
            if (InitDataApi._httpCatchData == null) {
                new InitDataApi ();
                while (InitDataApi._success == true)
                    yield return (InitDataApi._success == false);
            }

Debug.Log (InitDataApi._httpCatchData.result.board_category.Count + "  ここを通っていますか？ ") ;
            //カテゴリ - 選択 ここから
            _categoryList = new List<string> ();
            _categoryList.Add ("指定しない");
            foreach ( var d in InitDataApi._httpCatchData.result.board_category ) {
                _categoryList.Add (d.name);
            }
            _category.AddOptions (_categoryList);
            //カテゴリ - 選択 ここまで
            
            
            //性別 - 選択 ここから
            _genderList = new List<string> ();
            
            _genderList.Add ("指定しない");
            foreach (var d in InitDataApi._httpCatchData.result.sex_cd) {
                _genderList.Add (d.name);
            }

            _gender.AddOptions (_genderList);
            //性別 - 選択 ここまで


            //体型 - 選択 ここから
            _bodyTypeList = new List<string> ();
            _bodyTypeList.Add ("指定しない");

            string gender = "1";

            if (AppStartLoadBalanceManager._gender == ((int)GenderType.Male).ToString()) {
                gender = ((int)GenderType.Male).ToString();
            } else if (AppStartLoadBalanceManager._gender == ((int)GenderType.FeMale).ToString()) {
                gender = ((int)GenderType.FeMale).ToString();
            }
            
            var bodyType = CommonModelHandle.GetNameMaster (gender, CurrentProfSettingStateType.BodyType);
            foreach (var d in bodyType) {
                _bodyTypeList.Add (d.name);
            }
            _bodyType.AddOptions(_bodyTypeList);
            //体型 - 選択 ここまで


            //年齢 - 何歳まで ここから
            _ageFromList = new List<string> ();
            _ageFromList.Add ("18");
            for (int i = 18; i <= 100; i++) {
                _ageFromList.Add (i.ToString ());
            }
            _ageFrom.AddOptions (_ageFromList);
            //年齢 - 何歳まで ここまで


            //年齢 - 何歳まで ここから
            _ageToList = new List<string> ();
            _ageToList.Add ("100");
            for (int i = 18; i <= 100; i++) {
                _ageToList.Add (i.ToString ());
            }

            _ageTo.AddOptions (_ageToList);
            //年齢 - 何歳まで ここまで


            //身長から - ここから
            _heightFromList = new List<string> ();
            _heightFromList.Add ("110");
            for (int i = 110; i <= 220; i++) {
                _heightFromList.Add (i.ToString ());
            }
            _heightFrom.AddOptions (_heightFromList);
            //身長から - ここまで

             //身長まで - ここから
            _heightToList = new List<string> ();
            _heightToList.Add ("220");
            for (int i = 110; i <= 220; i++) {
                _heightToList.Add (i.ToString ());
            }
            _heightTo.AddOptions (_heightToList);
            //身長まで - ここまで
   
            //距離 - ここから
            _radiusList = new List<string> ();
            _radiusList.Add ("指定しない");
            foreach (var d in InitDataApi._httpCatchData.result.radius) {
                _radiusList.Add (d.name);
            }
            _radius.AddOptions(_radiusList);
            //距離 - ここまで
            
            
            //ここまでマスターデータを当てるDropdownに当てる処理 - ここまで
        }
        #endregion
        
        
        /// <summary>
        /// Categories the changed.
        /// カテゴリのやつ。
        /// </summary>
        /// <returns>The changed.</returns>
        public void CategoryChanged () {
            if (_category.value != 0) {
                string n = _categoryList[_category.value];
                var d = CommonModelHandle.GetByNameBaseData (n, CurrentProfSettingStateType.BoardCategory);
                _categoryAPIThrow = d.id;
            }
            else if (_category.value == 0) 
            {
                _categoryAPIThrow = "";
            }
            
Debug.Log("サーバーに送るデータが取得出来ているか？ Category " + _categoryAPIThrow);
        }

        /// <summary>
        /// Genders the changed.
        /// 性別選択。
        /// </summary>
        /// <returns>The changed.</returns>
        public void GenderChanged ()
        {
            string n = _genderList[_gender.value];
            var d = CommonModelHandle.GetByNameBaseData (n, CurrentProfSettingStateType.Gender);
            _sexAPIThrow = d.id;

            if (_gender.value == 0)
                _sexAPIThrow = "";

Debug.Log("サーバーに送るデータが取得出来ているか？ Gender " + _sexAPIThrow);

        }
        
        /// <summary>
        /// Ages from changed.
        /// 年齢から
        /// </summary>
        /// <returns>The from changed.</returns>
        public void AgeFromChanged () {

            _ageLowAPIThrow = _ageFromList [_ageFrom.value];
            
Debug.Log("サーバーに送るデータが取得出来ているか？ AgeFrom " + _ageLowAPIThrow);
        }
        
        
        /// <summary>
        /// Ages to changed.
        /// 年齢から
        /// </summary>
        /// <returns>The to changed.</returns>
        public void AgeToChanged ()
        {
            _ageHighAPIThrow = _ageToList [_ageTo.value];
            
Debug.Log("サーバーに送るデータが取得出来ているか？ AgeTo " + _ageHighAPIThrow);
        }
        

        /// <summary>
        /// Heights from changed.
        /// 身長から
        /// </summary>
        /// <returns>The from changed.</returns>
        public void HeightFromChanged ()
        {
            _heightFromAPIThrow = _heightToList [_heightFrom.value];
            
Debug.Log("サーバーに送るデータが取得出来ているか？ HeightFrom " + _heightFromAPIThrow);
        }
        
        /// <summary>
        /// Heights to changed.
        /// 身長まで
        /// </summary>
        /// <returns>The to changed.</returns>
        public void HeightToChanged ()
        {
            _heightToAPIThrow = _heightToList [_heightTo.value];
            
Debug.Log("サーバーに送るデータが取得出来ているか？ HeightTo " + _heightToAPIThrow);
        }

        /// <summary>
        /// Bodies the type changed.
        /// 体型
        /// </summary>
        /// <returns>The type changed.</returns>
        public void BodyTypeChanged ()
        {
            if (_bodyType.value != 0) {
                string n = _bodyTypeList [_bodyType.value];
                _bodyTypeAPIThrow = n;
            } else if (_bodyType.value == 0) {
                _bodyTypeAPIThrow = "";
            }
Debug.Log("サーバーに送るデータが取得出来ているか？ Body Type " + _bodyTypeAPIThrow);
        }
        
        
        /// <summary>
        /// Radiuses the changed.
        /// 距離データ取得
        /// </summary>
        /// <returns>The changed.</returns>
        public void RadiusChanged ()
        {
            if (_radius.value != 0) {
                string n = _radiusList [_radius.value];
                var d = CommonModelHandle.GetByNameBaseData (n, CurrentProfSettingStateType.Radius);
                _radiusAPIThrow = d.id;
            } else if (_radius.value == 0) {
                _radiusAPIThrow = "";
            }

Debug.Log("サーバーに送るデータが取得出来ているか？ Radius " + _radiusAPIThrow);
        }
        
        
        /// <summary>
        /// Ises the image changed.
        /// 画像のありなし選択。
        /// </summary>
        /// <returns>The image changed.</returns>
        public void IsImageChanged() {
             _isImageAPIThrow = _isImageListObject.value.ToString ();
            if (_isImageAPIThrow == "2") {
                _isImageAPIThrow = "0";
            } else if (_isImageAPIThrow == "0")
                _isImageAPIThrow = "";
        }
        
        /// <summary>
        /// Sets the data.
        /// マスターデータから動的に値が変更する所用
        /// </summary>
        public void SetData ()
        {
            BulletinBoardEventManager.Instance._CategoryID = _categoryAPIThrow;
            BulletinBoardEventManager.Instance._sex        = _sexAPIThrow;
            BulletinBoardEventManager.Instance._ageTo      = _ageHighAPIThrow;
            BulletinBoardEventManager.Instance._ageFrom    = _ageLowAPIThrow;
            BulletinBoardEventManager.Instance._heightTo   = _heightToAPIThrow;
            BulletinBoardEventManager.Instance._heightFrom = _heightFromAPIThrow;
            BulletinBoardEventManager.Instance._bodyType   = _bodyTypeAPIThrow;
            BulletinBoardEventManager.Instance._isImage    = _isImageAPIThrow;
            BulletinBoardEventManager.Instance._radius     = _radiusAPIThrow;
            BulletinBoardEventManager.Instance._keyword = _keyword.text;
        }


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
                    Debug.Log ("Right Right Right Right Right Right Right ");

                    if (SearchEventManager.Instance != null)
                    {
                        SearchEventManager.Instance.BackButton (this.gameObject);
                    }

                    if (BulletinBoardEventManager.Instance != null) 
                    {
                        BulletinBoardEventManager.Instance.ConditionClose (this.gameObject);
                    }
                }
            }
        }
        #endregion
    }
}