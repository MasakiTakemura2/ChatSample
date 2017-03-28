using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using Helper;
using EventManager;
using System.IO;
using uTools;
using Http;
using ViewController;


namespace Helper
{
    public class TinderGesture : SingletonMonoBehaviour<TinderGesture>
    {

        #region SerializeField Variable
        [SerializeField]
        private Transform _imageParent;

        [SerializeField]
        private Transform _panelPopupLikeLimit;
        
        [SerializeField]
        private Text _panelPopupLikeLimitText;

        [SerializeField]
        private Transform _panelPopupSuperLikeLimit;

        [SerializeField]
        private Text _panelPopupSuperLikeLimitText;

        [SerializeField]
        private Transform _panelPopupRewind;
        
        [SerializeField]
        private Text _panelPopupRewindText;

        [SerializeField]
        private Transform _panelProfile;

        [SerializeField]
        private GameObject _loadingOverLay;

        [SerializeField]
        private Transform _panelPopupReWind;
        #endregion


        #region Public Variable
        [SerializeField]
        public GameObject _controlObject;

        [SerializeField]
        public GameObject _nextControlObject;

        public GameObject _nope;
        public GameObject _like;
        public GameObject _superLike;
        public int _tinCounter = 0;
        public int _tinMax = 0;
        public float _springFrequency;
        public Vector2 _springDirection;
        public Vector2 _tapReleasePosition;
        public Vector2 _nowPosition;

        public Vector2 _releaseDirection;
        public Vector2 _baseControlObjectPosition;  // ドラッグ離した時のオブジェクトの基準にする中心位置
        public Vector3 _dragStartPosition;
        public const int BASEPOSITION_ADJUST = 100; // ドラッグの傾き中心点調整に定義

        public float _time = 1;
        public Vector2 _rotateBasePosition;
        public List<UserDataEntity.Basic> _usersCache;
        public int garbagePointer = 10;
        #endregion


        #region Private Variable
        private Vector3 _cacheTransform = Vector3.zero;
        private bool _isTin = false;
        private LikeOrNope LNState = LikeOrNope.None;
        private enum LikeOrNope
        {
            None,
            Nope,
            Like,
            SuperLike,
            Back
        }

        public bool _isEventPopUp = false;
        private string _nowPage = "0";
        private bool _isGesture = false;
        private string commonFileName = "";
        private int _backloadCount = 0;
        #endregion


        #region Unity Api
        IEnumerator Start ()
        {
            while (MatchingEventManager.Instance._isStart == false) {
                yield return (MatchingEventManager.Instance._isStart == true);
            }

            if (GetUserApi._httpCatchData.result.review == "true" || CommonConstants.IS_PREMIUM == true)
            {
                _panelPopupSuperLikeLimitText.text = "";
                _panelPopupRewindText.text         = "";
                _panelPopupLikeLimitText.text      = "";
                
            }

            _usersCache = new List<UserDataEntity.Basic> ();
            _loadingOverLay.SetActive (true);

            _springDirection    = new Vector2 (0, 0);
            _tapReleasePosition = new Vector2 (0, 0);
            _nowPosition        = new Vector2 (0, 0);
            _rotateBasePosition = new Vector2 (0, 300);
            _cacheTransform     = _controlObject.transform.localPosition;

            //ローカルファイル用の初期化
            commonFileName = LocalFileConstants.GetLocalFileDir () + LocalFileConstants.COMMON_LOCAL_FILE_NAME;
            LocalFileHandler.Init (commonFileName);
            _backloadCount = 0;

            yield return StartCoroutine (CallRandomUserListApi (_nowPage));
            _loadingOverLay.SetActive (false);
        }

        // フリックするユーザーデータの読み込み後　初期化処理
        public IEnumerator CallRandomUserListApi (string pageNo)
        {
            _isGesture = false;
            _loadingOverLay.SetActive (true);
            RandomUserListApi._success = false;

            new RandomUserListApi (pageNo);
            while (RandomUserListApi._success == false) {
                yield return (RandomUserListApi._success == true);
            }
            int page = int.Parse (pageNo);
            _usersCache = new List<UserDataEntity.Basic> ();
            foreach (var user in RandomUserListApi._httpCatchData.result.users) {
                _usersCache.Add (user);
            }

            _tinMax = _usersCache.Count;
            if (_tinMax == 0) {
                // データがないなら表示しない
                _controlObject.SetActive (false);
                _nextControlObject.SetActive (false);
                yield break;
            }

            //最初に画像二枚読み込み
            _tinCounter = 0;
            if (_tinMax > 1 && _tinMax > _tinCounter) {
                if (_nextControlObject != null) {
                    _nextControlObject.GetComponent<TinderImage> ().Init (_usersCache [_tinCounter + 1]);
                }

                if (_controlObject != null) {
                    _controlObject.GetComponent<TinderImage> ().Init (_usersCache [_tinCounter]);
                }

                _controlObject.transform.SetAsLastSibling ();
                _controlObject.GetComponent<BoxCollider2D> ().enabled = true;

            } else if (_tinMax == 1) {
                _controlObject.GetComponent<TinderImage> ().Init (_usersCache [_tinCounter]);
                _controlObject.GetComponent<BoxCollider2D> ().enabled = false;
                _controlObject.transform.SetAsLastSibling ();
            } else if (_tinMax == 0) {
                _controlObject.SetActive (true);
            }

            Vector2 pos;
            pos.x = _controlObject.transform.localPosition.x;
            pos.y = _controlObject.transform.localPosition.y - BASEPOSITION_ADJUST; // 補正のためすこしだけ位置をずらす
            _baseControlObjectPosition = pos;

            //はじめての「_tinCounter」のカウント。
            _tinCounter = 1;
            _isGesture = true;
            _loadingOverLay.SetActive (false);
        }


        private const float WAIT_TIME = 10.0f;
        void FixedUpdate ()
        {
            // ダイアログ開いているときはジェスチャー無効にしたい
            if (_loadingOverLay.activeSelf || _panelPopupLikeLimit.transform.localScale.x > 0 || _panelPopupSuperLikeLimit.transform.localScale.x > 0) {
                MatchingEventManager.Instance.SetFingerGestureEnable (false);
            } else {
                MatchingEventManager.Instance.SetFingerGestureEnable (true);
            }

            //アニメーションの慣性の処理。
            _springFrequency += 0.14f;
            _time -= 0.06f;


            // ドラッグ中の状態表示
            if (_controlObject != null && _like != null && _nope != null && _superLike != null && _isGesture == true && _isEventPopUp == false) {
                if (_nowPosition.y > 220f) {
                    LNState = LikeOrNope.SuperLike;
                    _like.SetActive (false);
                    _nope.SetActive (false);
                    _superLike.SetActive (true);
                } else if (_nowPosition.x < 0 && _controlObject != null) {
                    //_controlObject.transform.localRotation = Quaternion.Euler (new Vector3(0,0, -3.5f));
                    LNState = LikeOrNope.Nope;
                    _like.SetActive (false);
                    _nope.SetActive (true);
                    _superLike.SetActive (false);
                } else if (_nowPosition.x > 0 && _controlObject != null) {
                    //_controlObject.transform.localRotation = Quaternion.Euler (new Vector3(0,0, 3.5f));
                    LNState = LikeOrNope.Like;
                    _nope.SetActive (false);
                    _like.SetActive (true);
                    _superLike.SetActive (false);
                }

                if (_isTin == true || LNState != LikeOrNope.None && LNState != LikeOrNope.SuperLike && (_nowPosition.x < 60 && _nowPosition.x > -60)) {
                    LNState = LikeOrNope.None;
                    _nope.SetActive (false);
                    _like.SetActive (false);
                    _superLike.SetActive (false);

                    /*
		          	_controlObject.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
		          	_controlObject.transform.localRotation = Quaternion.Euler (Vector3.zero);
					*/
                    if (_isTin == true) {
                        //_controlObject.transform.localPosition = _cacheTransform;
                        _isTin = false;
                    }
                }
            }
        }
        #endregion

        #region 「前に戻る。」ボタン。
        public void BackUserPictButton ()
        {
            if (_backloadCount == 0) {
                string alertTin = LocalMsgConst.ALERT_TINDER;
                PopupPanel.Instance.PopMessageInsert (
                   alertTin,
                   LocalMsgConst.OK,
                    AlertOK
                );
                MatchingEventManager.Instance.PanelPopupAnimate (PopupPanel.Instance.gameObject);
                return;
            }

            MatchingEventManager.Instance.PopUpPanelOpen (_panelPopupRewind.gameObject);
        }

        /// <summary>
        /// Backs the user pict button.
        /// </summary>
        public void PopupBackUserPictButton ()
        {
#if UNITY_IPHONE || UNITY_ANDROID && !UNITY_EDITOR
            if (GetUserApi._httpCatchData.result.review == "false") {
                //無料会員か、有料会員の状態チェック
                if (CommonConstants.IS_PREMIUM == false) {
                    //動画広告が何もない場合はアイモバイル、インターステシャルを表示。
                    if (Maio.CanShow (CommonConstants.MAIO_ZONE_ID_2) == false) {
                        if (AppStartLoadBalanceManager.m_NendAdFullBoard != null){
                            AppStartLoadBalanceManager.m_NendAdFullBoard.Load ();
                            AppStartLoadBalanceManager.m_NendAdFullBoard.Show ();
                        }
                        //インタースティシャルのイベントが取れないのでバナー表示で機能が使えるように。
                        StartCoroutine (BackLoadImage ());
                    } else {
AppStartLoadBalanceManager._whereFromAds = MaioMovieSdkEvent.WhereFromAds.MatchBackloadImage;
                        //問答無用で動画広告を表示
                        Maio.Show (CommonConstants.MAIO_ZONE_ID_2);
                        return;
                    }
                } else { 
                    MatchingEventManager.Instance.PopUpPanelClose (_panelPopupRewind.gameObject);
                    StartCoroutine (BackLoadImage ());     
                }
            } else {
                MatchingEventManager.Instance.PopUpPanelClose (_panelPopupRewind.gameObject);
                StartCoroutine (BackLoadImage ());
            }
#endif

        }

        /// <summary>
        /// Ons the finished ad to back load image.
        /// </summary>
        /// <param name="zoneId">Zone identifier.</param>
        /// <param name="playtime">Playtime.</param>
        /// <param name="skipped">If set to <c>true</c> skipped.</param>
        /// <param name="rewardParam">Reward parameter.</param>
        public void OnClosedAdToBackLoadImage (string zoneId)
        {
            MatchingEventManager.Instance.PopUpPanelClose (_panelPopupRewind.gameObject);
            StartCoroutine (BackLoadImage ());
        }

        void AlertOK ()
        {
            if (_controlObject != null) {
                _controlObject.GetComponent<BoxCollider2D> ().enabled = false;
            }

            PopupPanel.Instance.PopClean ();
            MatchingEventManager.Instance.PanelPopupCloseAnimate (PopupPanel.Instance.gameObject);
        }

        public IEnumerator BackLoadImage ()
        {

#if UNITY_EDITOR
MatchingEventManager.Instance.PopUpPanelClose (_panelPopupRewind.gameObject);
#endif
            
		 	//pt 減算通知用リクエスト
		  	_loadingOverLay.SetActive (true);
		  	new RewindUserApi ();
			while (RewindUserApi._success == false) 
			{
				yield return (RewindUserApi._success == true);
			}
		  	_loadingOverLay.SetActive (false);

			// 前の写真に戻る 下でカウントがかかるので２つ分で確認する
		  	if (_tinCounter - 2 >= 0 && _tinMax >= _tinCounter)
		  	{
		     	if (_tinCounter >= 2)
		     	{
		        	_tinCounter--;
		         	_tinCounter--;
		     	}

		      	// 下に控える画像の読み込み
		      	GameObject go = Instantiate (Resources.Load (CommonConstants.TINDER_PANEL)) as GameObject;

		      	if(_tinCounter < _tinMax)
		      	{
		        	go.GetComponent<TinderImage>().Init (_usersCache[_tinCounter]);
		      	}

				if (_nextControlObject != null)
				{
					Destroy (_nextControlObject);
					while (_nextControlObject != null) 
					{
						yield return (_nextControlObject == null);
					}
				}
		      	go.transform.SetParent (_imageParent.transform, false);
		      	_nextControlObject = go;

		      	// 描画順から一番したに持って行きたいから
		      	if (_controlObject != null)
		      	{
		         	_controlObject.transform.SetAsLastSibling();
		        	StartCoroutine (TinAction (LikeOrNope.Back, true));
		      	}else{
		            if (_controlObject == null)
					{
		               _controlObject = _nextControlObject;
		            }

		            if (_tinCounter < _tinMax)
		            {                  
		              	// 下に控える画像の読み込み
		              	GameObject backupgo = Instantiate (Resources.Load (CommonConstants.TINDER_PANEL)) as GameObject;
		              	backupgo.GetComponent<TinderImage> ().Init (_usersCache[_tinCounter+1]);

		              	backupgo.transform.SetParent (_imageParent.transform, false);
		              
		              	_nextControlObject = backupgo;
		              	SetObj (_controlObject);
		              	_nextControlObject.GetComponent<BoxCollider2D> ().enabled = false;
		              	_controlObject.GetComponent<BoxCollider2D> ().enabled = true;
			        }else{
		              	_controlObject = _nextControlObject;
		              	_nextControlObject = null;

		              	if(_controlObject != null)
		              	{
		                	SetObj (_controlObject);
		                  	_controlObject.GetComponent<BoxCollider2D> ().enabled = true;
		              	}
		          	}
		      	}

		  	}else{
		     	_backloadCount = 0;
		      	string alertTin = LocalMsgConst.ALERT_TINDER;
		      	PopupPanel.Instance.PopMessageInsert(
		        	alertTin,
		          	LocalMsgConst.OK,
		          	AlertOK
		      	);

		      	MatchingEventManager.Instance.PanelPopupAnimate (PopupPanel.Instance.gameObject);
		     	yield break;
		  	}

		  	yield break;
		}
		#endregion

		#region Nope

		public void NopeButton ()
		{
		  	if (_tinCounter == _tinMax) 
			{
		      	string alertTin = LocalMsgConst.ALERT_TINDER;
		      	PopupPanel.Instance.PopMessageInsert(
		          	alertTin,
		          	LocalMsgConst.OK,
		        	AlertOK
		      	);

		      	MatchingEventManager.Instance.PanelPopupAnimate (PopupPanel.Instance.gameObject);
		      	return;
		  	}

		  	if(_controlObject != null)
		  	{
		      	StartCoroutine (TinAction (LikeOrNope.Nope, true));
		  	}
		}

		#endregion

		#region いいねボタン
		public void LikeButton ()
		{
		  	if (_tinCounter == _tinMax) 
			{
		      	string alertTin = LocalMsgConst.ALERT_TINDER;
		      	PopupPanel.Instance.PopMessageInsert(
		        	alertTin,
		        	LocalMsgConst.OK,
		         	AlertOK
		      	);

		      	MatchingEventManager.Instance.PanelPopupAnimate (PopupPanel.Instance.gameObject);
		      	return;
		  	}

		  	if (_controlObject != null)
		  	{
		      	StartCoroutine (TinAction (LikeOrNope.Like, true));
		  	}
		}
		#endregion

		#region スーパーいいねボタン
		public void SuperLike ()
		{
            _superLike.SetActive (false);

			if (_tinCounter == _tinMax) 
			{
		      	string alertTin = LocalMsgConst.ALERT_TINDER;
		      	PopupPanel.Instance.PopMessageInsert(
		          	alertTin,
		          	LocalMsgConst.OK,
		          	AlertOK
		      	);

		      	MatchingEventManager.Instance.PanelPopupAnimate (PopupPanel.Instance.gameObject);
		      	return;
		  	}

		  	if (_controlObject != null) 
			{
		      	StartCoroutine (TinAction (LikeOrNope.SuperLike, true));
		  	}
		}

        //        
        public void PopupSuperLikeButton ()
        {
#if UNITY_IPHONE || UNITY_ANDROID && !UNITY_EDITOR
                if (GetUserApi._httpCatchData.result.review == "false") {
                    if (CommonConstants.IS_PREMIUM == false) {
                        //動画広告が何もない場合はアイモバイル、インターステシャルを表示。
                        if (Maio.CanShow (CommonConstants.MAIO_ZONE_ID_2) == false) {
                            if (AppStartLoadBalanceManager.m_NendAdFullBoard != null){
                                AppStartLoadBalanceManager.m_NendAdFullBoard.Load ();
                                AppStartLoadBalanceManager.m_NendAdFullBoard.Show ();
                            }
                            //インタースティシャルのイベントが取れないのでバナー表示で機能が使えるように。
                            StartCoroutine (TinAction (LikeOrNope.SuperLike, true));
                        } else {
AppStartLoadBalanceManager._whereFromAds = MaioMovieSdkEvent.WhereFromAds.MatchSupreLike;
                            //問答無用で動画広告を表示
                            Maio.Show (CommonConstants.MAIO_ZONE_ID_2);
                            return;
                        }
                    } else { //有料会員の場合、広告をスルー
                        MatchingEventManager.Instance.PanelPopupCloseAnimate (_panelPopupSuperLikeLimit.gameObject);
                        StartCoroutine (TinAction (LikeOrNope.SuperLike, true, true));
                    }
                } else {
                    MatchingEventManager.Instance.PanelPopupCloseAnimate (_panelPopupSuperLikeLimit.gameObject);
                    StartCoroutine (TinAction (LikeOrNope.SuperLike, true, true));
                }
#endif
        }



        /// <summary>
        /// Ons the close ad to super like.
        /// </summary>
        /// <param name="zoneId">Zone identifier.</param>
        public void OnCloseAdToSuperLike (string zoneId) 
        {
            MatchingEventManager.Instance.PanelPopupCloseAnimate (_panelPopupSuperLikeLimit.gameObject);
            StartCoroutine (TinAction (LikeOrNope.SuperLike, true, true));
        }
		#endregion


		#region オブジェクトのドラッグ制御
		void OnDrag (DragGesture gesture)
		{
		  	if (gesture.Selection && _isGesture == true && _isEventPopUp == false)
		  	{
		      	if (_controlObject != null)
		      	{
					if (gesture.State == GestureRecognitionState.Ended) 
					{
						_dragStartPosition = new Vector3 (gesture.Position.x - (Screen.width * 0.5f), gesture.Position.y - (Screen.height * 0.5f), 0);
					}

					//上を中心として回転処理
					Vector2 basepos = _rotateBasePosition;
					basepos.y += 600;
					Vector2 pos = basepos - _nowPosition;
					float rad = Mathf.Atan2 (pos.y, pos.x);
					rad = rad * Mathf.Rad2Deg;
					_controlObject.transform.localRotation = Quaternion.identity;
					_controlObject.transform.Rotate(new Vector3(0,0,1), rad - 90);
					
					// 位置反映
					_controlObject.transform.localPosition = new Vector3 (gesture.Position.x - (Screen.width * 0.5f), gesture.Position.y - (Screen.height * 0.5f), 0);
			
					// 現在位置
					_nowPosition = new Vector2 (gesture.Position.x - (Screen.width * 0.5f), gesture.Position.y - (Screen.height * 0.5f));
					

			        // 指を離した時
		          	if (gesture.State == GestureRecognitionState.Ended)
		          	{
						//離して飛んで行く方向記憶
						_releaseDirection = _nowPosition - _baseControlObjectPosition;

		              	_time = 1;
		              	_tapReleasePosition = _nowPosition;
		             	_springDirection    = gesture.DeltaMove;

		              	if (_tapReleasePosition.y > 220f && LNState == LikeOrNope.SuperLike)
						{
		                  	//Super Like
		                  	StartCoroutine (TinAction (LNState)) ;
		              	} else if (_tapReleasePosition.x > 40 && LNState == LikeOrNope.Like) {
		                  	//Like
		                  	StartCoroutine (TinAction (LNState)) ;
		              	} else if (_tapReleasePosition.x < -40 && LNState == LikeOrNope.Nope) {
		                	//Nope
		                	StartCoroutine (TinAction(LNState));
						}else if(LNState == LikeOrNope.None) {
							StartCoroutine (TinAction(LNState));
						}
		          	}
		      	}
		  	}
		}
		#endregion
	      

		#region private Methods
		private IEnumerator TinAction(LikeOrNope lNState, bool isButtonAnimation = false, bool isSuperLikePop = false)
		{
		  	if (_controlObject != null && _tinMax >= _tinCounter && _isEventPopUp == false)
		  	{
		      	_controlObject.GetComponent<BoxCollider2D> ().enabled = false;
		      	
				// ボタン操作の動き
				if (isButtonAnimation == true)
				{
					if (lNState == LikeOrNope.Nope)
					{
						_nope.SetActive (true);

						if (_controlObject != null)
						{
							_controlObject.GetComponent<uTweenRotation> ().to = new Vector3 (0, 0, 90f);
							_controlObject.GetComponent<uTweenRotation> ().enabled = true;
							_controlObject.GetComponent<RectTransform> ().pivot = Vector2.zero;
	              
							while (_controlObject.GetComponent<RectTransform> ().localEulerAngles.z < 90)
							{
								if (_controlObject == null)
								{
									yield break;
								}

								yield return (_controlObject.GetComponent<RectTransform> ().localEulerAngles.z >= 90);
							}
						}

					} else if (lNState == LikeOrNope.Like) {
	              	
						_like.SetActive (true);
						if (_tinCounter < _usersCache.Count)
						{
							yield return StartCoroutine (CallSetUserLike (_usersCache [_tinCounter - 1].id, "0"));
						}


						if (_controlObject != null)
						{
							_controlObject.GetComponent<uTweenRotation> ().to = new Vector3 (0, 0, -90f);
							_controlObject.GetComponent<uTweenRotation> ().enabled = true;

							/*
							while (_controlObject.GetComponent<RectTransform> ().localEulerAngles.z > -90)
							{
								if (_controlObject == null)
								{
									yield break;
								}
								yield return (_controlObject.GetComponent<RectTransform> ().localEulerAngles.z <= -90);
							}
							*/

						}

					
	          	
					} else if (lNState == LikeOrNope.SuperLike) {
                        
                        
						_superLike.SetActive (true);
if (isSuperLikePop == false) {
    if (LocalFileHandler.GetBool (LocalFileConstants.FIRST_SUPER_LIKE) == false)
    {
        MatchingEventManager.Instance.PopUpPanelOpen (_panelPopupSuperLikeLimit.gameObject);
        //LocalFileHandler.SetBool (LocalFileConstants.FIRST_SUPER_LIKE, true);
        yield break;
    }
} else {
    LocalFileHandler.SetBool (LocalFileConstants.FIRST_SUPER_LIKE, false);
     if (_controlObject != null)
         _controlObject.GetComponent<BoxCollider2D> ().enabled = true;
}

						if (_tinCounter < _usersCache.Count)
						{
							//pt 減算通知用リクエスト
							yield return StartCoroutine (CallSetUserLike (_usersCache [_tinCounter - 1].id, "1"));
						}

						if (_controlObject != null)
						{
							_controlObject.GetComponent<uTweenRotation> ().to = new Vector3 (0, 90, 90f);
							_controlObject.GetComponent<uTweenRotation> ().enabled = true;

							while (_controlObject.GetComponent<RectTransform> ().localEulerAngles.z < 40) 
							{
								if (_controlObject == null)
								{
									yield break;
								}
								yield return (_controlObject.GetComponent<RectTransform> ().localEulerAngles.z >= 40);
							}
						}


					} else if (lNState == LikeOrNope.Back) {
	              	
						if (_controlObject != null)
						{
							_controlObject.GetComponent<uTweenRotation> ().to = new Vector3 (0, 0, 90f);
							_controlObject.GetComponent<uTweenRotation> ().enabled = true;
							_controlObject.GetComponent<RectTransform> ().pivot = Vector2.zero;

							while (_controlObject.GetComponent<RectTransform> ().localEulerAngles.z < 90)
							{
								if (_controlObject == null)
								{
									yield break;
								}
								yield return (_controlObject.GetComponent<RectTransform> ().localEulerAngles.z >= 90);
							}

						}
					}
	      	
				}
					
				Debug.Log (" ボタンアニメーション以外。 ここから");

				// タップ操作で引っ張ってからの続き
				if (lNState == LikeOrNope.Nope)
				{
					if (isButtonAnimation)
					{
						_controlObject.GetComponent<uTweenPosition> ().from = _controlObject.transform.localPosition;
						_controlObject.GetComponent<uTweenPosition> ().to = new Vector3 (-750f, 0f, 0f);
						_controlObject.GetComponent<uTweenPosition> ().enabled = true;
					} else {

						while (_controlObject.transform.localPosition.x >= -375)
						{
							if (_controlObject == null)
							{
								yield break;
							}
				
							Vector3 pos = _controlObject.transform.localPosition;
							pos.x += _releaseDirection.x;
							pos.y += _releaseDirection.y;
							_controlObject.transform.localPosition = pos;

							yield return (_controlObject.transform.localPosition.x >= -750);
						}
					}

					if (_controlObject != null)
					{
						Destroy (_controlObject);
						while (_controlObject != null)
						{
							yield return (_controlObject == null);
						}
					}

				} else if (lNState == LikeOrNope.Like) {

					if (isButtonAnimation == false)
					{
						yield return StartCoroutine (CallSetUserLike (_usersCache [_tinCounter - 1].id, "0"));
					}

					if (isButtonAnimation)
					{
						_controlObject.GetComponent<uTweenPosition> ().from = _controlObject.transform.localPosition;
						_controlObject.GetComponent<uTweenPosition> ().to = new Vector3 (750f, 0f, 0f);
						_controlObject.GetComponent<uTweenPosition> ().enabled = true;

						while (_controlObject.transform.localPosition.x < 375)
						{
							if (_controlObject == null)
							{
								yield break;
							}

							yield return (_controlObject.transform.localPosition.x >= 375);
						}
					} else {

						while (_controlObject.transform.localPosition.x <= 375)
						{
							if (_controlObject == null)
							{
								yield break;
							}

							Vector3 pos = _controlObject.transform.localPosition;
							pos.x += _releaseDirection.x;
							pos.y += _releaseDirection.y;
							_controlObject.transform.localPosition = pos;

							yield return (_controlObject.transform.localPosition.x >= 375);
						}
					}

					if (_controlObject != null) 
					{
						Destroy (_controlObject);
						while (_controlObject != null)
						{
							yield return (_controlObject == null);
						}
					}

				} else if (lNState == LikeOrNope.SuperLike) {

if (isSuperLikePop == false) {
    if (LocalFileHandler.GetBool (LocalFileConstants.FIRST_SUPER_LIKE) == false)
    {
        MatchingEventManager.Instance.PopUpPanelOpen (_panelPopupSuperLikeLimit.gameObject);
        //LocalFileHandler.SetBool (LocalFileConstants.FIRST_SUPER_LIKE, true);
        yield break;
    }
} else {
    LocalFileHandler.SetBool (LocalFileConstants.FIRST_SUPER_LIKE, false);

    if (_controlObject != null)
        _controlObject.GetComponent<BoxCollider2D> ().enabled = true;
}

					if (isButtonAnimation == false && _tinMax > _tinCounter) 
					{

						if (_tinMax == _tinCounter)
						{
							Debug.Log ("スーパーいいね前の、_tinCounterの減算している。");
							_tinCounter--;
						}

						Debug.Log (_usersCache [_tinCounter].id + " < スーパーいいねした時のユーザーのID、確認用 tincounter:" + _tinCounter);
						yield return StartCoroutine (CallSetUserLike (_usersCache [_tinCounter - 1].id, "1"));
					}

					if (isButtonAnimation)
					{
						_controlObject.GetComponent<uTweenPosition> ().from = _controlObject.transform.localPosition;
						_controlObject.GetComponent<uTweenPosition> ().to = new Vector3 (0f, 1000f, 0f);
						_controlObject.GetComponent<uTweenPosition> ().enabled = true;
					} else {

						while (_controlObject.transform.localPosition.y <= 500)
						{
							if (_controlObject == null) {
								yield break;
							}

							Vector3 pos = _controlObject.transform.localPosition;
							pos.x += _releaseDirection.x;
							pos.y += _releaseDirection.y;
							_controlObject.transform.localPosition = pos;

							yield return (_controlObject.transform.localPosition.y >= 500);
						}
					}

					if (_controlObject != null)
					{
						Destroy (_controlObject);
						while (_controlObject != null)
						{
							yield return (_controlObject == null);
						}
					}

				} else if (lNState == LikeOrNope.Back) {

					if (isButtonAnimation)
					{
						_controlObject.GetComponent<uTweenPosition> ().from = _controlObject.transform.localPosition;
						_controlObject.GetComponent<uTweenPosition> ().to = new Vector3 (-750f, 0f, 0f);
						_controlObject.GetComponent<uTweenPosition> ().enabled = true;
					} else {
						
						while (_controlObject.transform.localPosition.x >= -375) {
							if (_controlObject == null) {
								yield break;
							}
							yield return (_controlObject.transform.localPosition.x >= -375);
						}
					}

					if (_controlObject != null)
					{
						Destroy (_controlObject);
						while (_controlObject != null)
						{
							yield return (_controlObject == null);
						}
					}

				} else if (lNState == LikeOrNope.None) {
			
					if (_controlObject == null) 
					{
						//yield break;
					}

					// 少しだけビヨンビヨンする
					float length = BASEPOSITION_ADJUST;
					_controlObject.transform.rotation = Quaternion.identity;
					while (length > 1)
					{
						Vector2 nowpos, basepos;
						nowpos.x = _controlObject.transform.localPosition.x;
						nowpos.y = _controlObject.transform.localPosition.y;
						basepos.x = _baseControlObjectPosition.x;
						basepos.y = _baseControlObjectPosition.y + BASEPOSITION_ADJUST;
						Vector2 lengthvector = basepos - nowpos;
						length = lengthvector.magnitude;
						lengthvector *= 0.5f;
				
						Vector3 pos = _controlObject.transform.localPosition;
						pos.x += lengthvector.x;
						pos.y += lengthvector.y;
						_controlObject.transform.localPosition = pos;
				
						yield return (length < 1);
					}
			
					_tapReleasePosition = Vector2.zero;
					_nowPosition = Vector2.zero;
					_springDirection = Vector2.zero;
					_isTin = true;

					_controlObject.GetComponent<RectTransform> ().anchoredPosition = Vector2.zero;
					_controlObject.transform.localRotation = Quaternion.Euler (Vector3.zero);
					_controlObject.GetComponent<BoxCollider2D> ().enabled = true;
					yield break;
				}


		      	if (_tinCounter < _tinMax)
				{
		         	_backloadCount++;
		         	_tinCounter++;
		      	}

		      	if(_tinCounter == _tinMax)
				{
		          	string alertTin = LocalMsgConst.ALERT_TINDER;
		          	PopupPanel.Instance.PopMessageInsert(
		              	alertTin,
		              	LocalMsgConst.OK,
		              	AlertOK
		          	);

		          	MatchingEventManager.Instance.PanelPopupAnimate (PopupPanel.Instance.gameObject);
		          	yield break;
		      	}

		      	if (_tinCounter < _tinMax)
		      	{
		          	// 下に控える画像の読み込み
		          	GameObject go = Instantiate (Resources.Load (CommonConstants.TINDER_PANEL)) as GameObject;
		          	go.GetComponent<TinderImage> ().Init (_usersCache[_tinCounter]);
		          	go.transform.SetParent (_imageParent.transform, false);
		          	_controlObject = _nextControlObject;

		          	_nextControlObject = go;
		           	SetObj (_controlObject);

		          	_nextControlObject.GetComponent<BoxCollider2D> ().enabled = false;
		          	_controlObject.GetComponent<BoxCollider2D> ().enabled     = true;
		      	} else {
		          	_controlObject = _nextControlObject;

		          	if(_controlObject != null)
		          	{
		              	SetObj (_controlObject);
		              	_controlObject.GetComponent<BoxCollider2D> ().enabled = true;
		          	}
		      	}

		      	// 描画順から一番したに持って行きたいから
		      	if (_controlObject != null)
				{
		         	_controlObject.transform.SetAsLastSibling ();
		      	}

		      	_tapReleasePosition = Vector2.zero;
		      	_nowPosition        = Vector2.zero;
		      	_springDirection    = Vector2.zero;
		      	_isTin = true;

		      	//追加がある場合
		      	if (lNState != LikeOrNope.Back)
		      	{
		          	if (_tinCounter == _tinMax-5)
		          	{
		              	int nPage = int.Parse (_nowPage) + 1;
		              	_nowPage = nPage.ToString ();
		              	yield return ( StartCoroutine (AddCacheData( nPage.ToString())));
		              	yield break;
		          	}
		      	}
			} else {
		      	Debug.Log ("ここ異常系");
		  	}

			yield break;
		}

	    public IEnumerator AddCacheData (string page)
		{
	    	new RandomUserListApi (page);
			while (RandomUserListApi._success == false) 
			{
				yield return (RandomUserListApi._success == false);
			}

	        System.GC.Collect ();
	        Resources.UnloadUnusedAssets ();

	        if (RandomUserListApi._httpCatchData.result.users.Count > 0) 
			{
	        	foreach (var u in RandomUserListApi._httpCatchData.result.users)
				{
	            	_usersCache.Add (u);
	            }
	        }
	        _tinMax = _usersCache.Count;
		}

	    private void SetObj (GameObject controlObj)
	    {
	    	if (controlObj != null)
	        {
	        	for (int i = 0; i < controlObj.transform.childCount; i++)
	            {
	            	if (controlObj.transform.GetChild (i).name == LikeOrNope.Nope.ToString ()) 
					{
	                	_nope = controlObj.transform.GetChild (i).gameObject;
	                } else if (controlObj.transform.GetChild (i).name == LikeOrNope.Like.ToString ()) {
	                   	_like = controlObj.transform.GetChild (i).gameObject;
	               	} else if (controlObj.transform.GetChild (i).name == LikeOrNope.SuperLike.ToString ()) {
	                    _superLike = controlObj.transform.GetChild (i).gameObject;
	                }
	            }
	        }
	    }

	   	public IEnumerator CallSetUserLike(string toUserID, string isSuper)
	    {
	    	_loadingOverLay.SetActive (true);

	        SetLikeUserApi._success = false;
	        new SetLikeUserApi (toUserID, isSuper);
			while (SetLikeUserApi._success == false)
			{
				yield return (SetLikeUserApi._success == true);
			}

	        var res    = SetLikeUserApi._httpCatchData.result;
	        var meUser = GetUserApi._httpCatchData.result.user;

	        //マッチした場合、ポップアップ表示
	        if (res.is_matching == "true") 
			{
	        	_isEventPopUp = true;
	            PopupMatching.Instance.Init (meUser, res.matching_user);
	        } else if (res.is_like_limit == "true") {
	           _isEventPopUp = true;
	           
				//リミットが来ている場合の処理。
	            PanelPopupLikeLimit.Instance.Init (res);
	        }else{
	        	_isEventPopUp = false;
	        }

	        _loadingOverLay.SetActive (false);
	      }
	      #endregion
	}
}
