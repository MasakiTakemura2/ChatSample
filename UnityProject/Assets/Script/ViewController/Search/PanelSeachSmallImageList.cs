using UnityEngine;
using System.Collections;
using Http;
using Helper;
using UnityEngine.UI;
using EventManager;
using uTools;


namespace ViewController
{
	public class PanelSeachSmallImageList : SingletonMonoBehaviour<PanelSeachSmallImageList>
	{
		[SerializeField]
		public GameObject _InfiniteLimitScroll;
        
		[SerializeField]
		private GameObject _InfiniteLimitScrollBar;

		[SerializeField]
		private GameObject _loadingOverlay;
        
		[SerializeField]
		private GameObject _popupOverlay;
        
        [SerializeField]
        private GameObject _nextButton;

        private const int _onePagelimitNumber = 20;

        private bool _isFirst = false;


		public bool _disablePageForNetworkCall = false;
		public float _waitTimeForListReset = 0;
		public bool _nowTap = false;

		//画面初期化処理
		IEnumerator Start ()
		{
            if(_InfiniteLimitScrollBar != null)
            {
                _InfiniteLimitScrollBar.GetComponent<Scrollbar> ().value = 0.995f;
            }

            if (EventManager.SearchEventManager.Instance._statePanel == EventManager.SearchEventManager.StatePanel.Image )
            {
    			// 代表して通信させる
    			StartCoroutine (Init (0));
            }
            yield break;
		}

		public void Initialize(int pageChange)
		{
			//HeaderPanel.Instance.GetComponent<Text> ().text = PanelStateManager.GetHeaderStringByKey("SearchPanel");

			StartCoroutine (Init (pageChange));
		}
		IEnumerator Init(int pageChange)
		{
			_loadingOverlay.SetActive (true);

			// 通信レスポンス待ってから
			UserListApi._success = false;
			int page_next = int.Parse (EventManager.SearchEventManager.Instance._nowShowPage) + pageChange;

			new UserListApi (
				AppStartLoadBalanceManager._userKey,
				EventManager.SearchEventManager.Instance._lat,
				EventManager.SearchEventManager.Instance._lng,
				EventManager.SearchEventManager.Instance._order,
				EventManager.SearchEventManager.Instance._sex,
				EventManager.SearchEventManager.Instance._ageFrom,
				EventManager.SearchEventManager.Instance._ageTo,
				EventManager.SearchEventManager.Instance._heightFrom,
				EventManager.SearchEventManager.Instance._heightTo,
				EventManager.SearchEventManager.Instance._bodyType,
				EventManager.SearchEventManager.Instance._isImage,
				EventManager.SearchEventManager.Instance._radius,
				EventManager.SearchEventManager.Instance._keyword,
				page_next.ToString()
			);
			while (UserListApi._success == false)
			{
				yield return (UserListApi._success == true);
			}

			if (UserListApi._httpCatchData.result.users != null)
			{
				if (UserListApi._httpCatchData.result.users.Count > 0)
				{
                    if (UserListApi._httpCatchData.result.users.Count >= 20)
                        _nextButton.SetActive (true);

					_InfiniteLimitScroll.GetComponent<SearchListSmallImageInfiniteLimitScroll> ().Init ();

					// 更新後のリストの位置にバーをあわせる
					_InfiniteLimitScrollBar.GetComponent<Scrollbar> ().value = 0.995f;

					int page = int.Parse(EventManager.SearchEventManager.Instance._nowShowPage) + pageChange;
                    if (page % 4 == 0){
                        System.GC.Collect ();
                        Resources.UnloadUnusedAssets ();
                    }
					EventManager.SearchEventManager.Instance._nowShowPage = page.ToString();
				} else {
                   _nextButton.SetActive (false);
					// リストのないページカウントになってるので　一旦通信とリストの更新をさせないようにする
					_disablePageForNetworkCall = true;
					_InfiniteLimitScrollBar.GetComponent<Scrollbar> ().value = 0.995f;
					Debug.Log ("存在しないページにアクセスしようとしたのでやめる");               
					//_InfiniteLimitScroll.GetComponent<SearchListSmallImageInfiniteLimitScroll> ().SetPanelMax (0);
					//_InfiniteLimitScroll.GetComponent<SearchListSmallImageInfiniteLimitScroll> ().SetPanelInstantateCount (0);               
				}
			} else {
                _nextButton.SetActive (false);
				_disablePageForNetworkCall = true;
				_InfiniteLimitScrollBar.GetComponent<Scrollbar> ().value = 0.995f;
				Debug.Log ("存在しないページにアクセスしようとしたのでやめる");

				//_InfiniteLimitScroll.GetComponent<SearchListSmallImageInfiniteLimitScroll> ().SetPanelMax (0);
				//_InfiniteLimitScroll.GetComponent<SearchListSmallImageInfiniteLimitScroll> ().SetPanelInstantateCount (0);

			}

			_loadingOverlay.SetActive (false);
		}


		public void InitializeFromCondition()
		{
			StartCoroutine (InitFromCondition());
		}
		IEnumerator InitFromCondition()
		{
			_loadingOverlay.SetActive (true);
string _commonFileName = LocalFileConstants.GetLocalFileDir () + LocalFileConstants.COMMON_LOCAL_FILE_NAME;

LocalFileHandler.Init (_commonFileName);

//ファイルが作成されるまでポーリングして処理待ち
while (System.IO.File.Exists (_commonFileName) == false)
    yield return (System.IO.File.Exists (_commonFileName) == true);

SearchEventManager.SearchCondition SearchCondition = LocalFileHandler.Load<SearchEventManager.SearchCondition>(LocalFileConstants.SEARCH_CONDITION_KEY);

//条件検索のデータがローカルファイルに残っている場合。
if (SearchCondition != null) {
    SearchEventManager.Instance._lat        = SearchCondition._lat;
    SearchEventManager.Instance._lng        = SearchCondition._lng;
    SearchEventManager.Instance._order      = SearchCondition._order;
    SearchEventManager.Instance._sex        = SearchCondition._sex;
    SearchEventManager.Instance._ageFrom    = SearchCondition._ageFrom;
    SearchEventManager.Instance._ageTo      = SearchCondition._ageTo;
    SearchEventManager.Instance._heightFrom = SearchCondition._heightFrom;
    SearchEventManager.Instance._heightTo   = SearchCondition._heightTo;
    SearchEventManager.Instance._bodyType   = SearchCondition._bodyType;
    SearchEventManager.Instance._isImage    = SearchCondition._isImage;
    SearchEventManager.Instance._radius     = SearchCondition._radius;
    SearchEventManager.Instance._keyword    = SearchCondition._keyword;
}

			// 通信レスポンス待ってから
			UserListApi._success = false;

			new UserListApi (
				// todo とりあえず試すため全部の設定 あとできちんと値を設定して絞り込むようにしてください
				AppStartLoadBalanceManager._userKey,
				EventManager.SearchEventManager.Instance._lat,
				EventManager.SearchEventManager.Instance._lng,
				EventManager.SearchEventManager.Instance._order,
				EventManager.SearchEventManager.Instance._sex,
				EventManager.SearchEventManager.Instance._ageFrom,
				EventManager.SearchEventManager.Instance._ageTo,
				EventManager.SearchEventManager.Instance._heightFrom,
				EventManager.SearchEventManager.Instance._heightTo,
				EventManager.SearchEventManager.Instance._bodyType,
				EventManager.SearchEventManager.Instance._isImage,
				EventManager.SearchEventManager.Instance._radius,
				EventManager.SearchEventManager.Instance._keyword,
				"1"
			);
			while (UserListApi._success == false)
			{
				yield return (UserListApi._success == true);
			}
         
SearchEventManager.SearchCondition condition = new SearchEventManager.SearchCondition ();
condition._lat        = SearchEventManager.Instance._lat;
condition._lng        = SearchEventManager.Instance._lng;
condition._order      = SearchEventManager.Instance._order;
condition._sex        = SearchEventManager.Instance._sex;
condition._ageFrom    = SearchEventManager.Instance._ageFrom;
condition._ageTo      = SearchEventManager.Instance._ageTo;
condition._heightFrom = SearchEventManager.Instance._heightFrom;
condition._heightTo   = SearchEventManager.Instance._heightTo;
condition._bodyType   = SearchEventManager.Instance._bodyType;
condition._isImage    = SearchEventManager.Instance._isImage;
condition._radius     = SearchEventManager.Instance._radius;
condition._keyword    = SearchEventManager.Instance._keyword;

LocalFileHandler.Save<SearchEventManager.SearchCondition>(LocalFileConstants.SEARCH_CONDITION_KEY, condition);
LocalFileHandler.Flush ();


			if (UserListApi._httpCatchData.result.users != null)
			{
				if (UserListApi._httpCatchData.result.users.Count > 0)
				{
					_InfiniteLimitScroll.GetComponent<SearchListSmallImageInfiniteLimitScroll> ().m_currentItemNo = 0;

					_InfiniteLimitScroll.GetComponent<SearchListSmallImageInfiniteLimitScroll> ().Init ();

					EventManager.SearchEventManager.Instance._nowShowPage = "1";
					_InfiniteLimitScrollBar.GetComponent<Scrollbar> ().value = 0.9995f;
                    if (UserListApi._httpCatchData.result.users.Count >= 20) {
                        _nextButton.SetActive (true);
                    }
				} else {
					// リストのないページカウントになってるので　一旦通信とリストの更新をさせないようにする
					_disablePageForNetworkCall = true;
					_InfiniteLimitScrollBar.GetComponent<Scrollbar> ().value = 0.005f;
					Debug.Log ("存在しないページにアクセスしようとしたのでやめる");


					PopupPanel.Instance.PopMessageInsert(
						LocalMsgConst.LIST_SEARCH_ZERO,
						LocalMsgConst.OK,
						PanelListZeroFinishClose
					);
					PanelPopupAnimate (GameObject.FindGameObjectWithTag(CommonConstants.POPUP_BASIC_TAG));
				}
			} else {
				_disablePageForNetworkCall = true;
				_InfiniteLimitScrollBar.GetComponent<Scrollbar> ().value = 0.005f;
				Debug.Log ("存在しないページにアクセスしようとしたのでやめる");
			}

			_loadingOverlay.SetActive (false);
		}


		void PanelListZeroFinishClose () 
		{
			PanelPopupCloseAnimate (GameObject.FindGameObjectWithTag(CommonConstants.POPUP_BASIC_TAG));
		}
		private void PanelPopupAnimate ( GameObject target )
		{
			//ポップ用の背景セット
			_popupOverlay.SetActive (true);

			target.GetComponent<uTweenScale> ().from = Vector3.zero;
			target.GetComponent<uTweenScale> ().to = new Vector3 (1, 1 ,1 );
			target.GetComponent<uTweenScale> ().delay    = 0.001f;
			target.GetComponent<uTweenScale> ().duration = 0.25f;
			target.GetComponent<uTweenScale> ().ResetToBeginning ();
			target.GetComponent<uTweenScale> ().enabled = true;
		}
		private void PanelPopupCloseAnimate ( GameObject target )
		{
			_popupOverlay.SetActive (false);
			target.GetComponent<uTweenScale> ().delay    = 0.01f;
			target.GetComponent<uTweenScale> ().duration = 0.25f;
			target.GetComponent<uTweenScale> ().from = new Vector3 (1f, 1f ,1f );
			target.GetComponent<uTweenScale> ().to = Vector3.zero;
			target.GetComponent<uTweenScale> ().ResetToBeginning ();
			target.GetComponent<uTweenScale> ().enabled = true;
		}



		public void Update ()
		{
			// 各リスト重複して反応するのを防ぐため
			switch (EventManager.SearchEventManager.Instance._statePanel)
			{
				case EventManager.SearchEventManager.StatePanel.Image:
					if(EventManager.SearchEventManager.Instance._isTwoColumn) 
					{
                        System.GC.Collect ();
                        Resources.UnloadUnusedAssets ();
						return;
					}
				break;

				default:
					return;
			}
			if(EventManager.SearchEventManager.Instance._listConnectUpdateDisable)
			{
				return;
			}


			if (Input.GetMouseButtonUp (0)) 
			{
				//_loadingUpObject.SetActive(false);
				//_loadingDownObject.SetActive(false);


				/*
				if(_waitTimeForListReset < 0.4f)
				{
					_waitTimeForListReset = 0;
					_nowTap = false;
					return;
				}
				_waitTimeForListReset = 0;
				_nowTap = false;
				*/


				// バーの監視　上に上りきったらページを変える
				if (_InfiniteLimitScrollBar.GetComponent<Scrollbar> ().value >= 1.0f) 
				{
					if (int.Parse(EventManager.SearchEventManager.Instance._nowShowPage) - 1 < 1)
					{
                        
						// ページ１以下は存在しないはず
						return;
					}
					//_nowShowPage--;
                    _nextButton.SetActive (true);
					StartCoroutine (Init (-1));
					//_InfiniteLimitScrollBar.GetComponent<Scrollbar> ().value = 0.005f;
					//_InfiniteLimitScrollBar.GetComponent<Scrollbar> ().value = 0.995f;
					//_disablePageForNetworkCall = false;

					return;
				}

				// バーの監視　下に下りきったらページを変える
				if (_InfiniteLimitScrollBar.GetComponent<Scrollbar> ().value <= 0.0f)
				{
                    Debug.Log ("kokokokokok");
					StartCoroutine (Init (1));
                    
                    _nextButton.SetActive (false);

					//_InfiniteLimitScrollBar.GetComponent<Scrollbar> ().value = 0.995f;

					return;
				}


			}

			/*
			if (Input.GetMouseButtonDown (0)) 
			{
				_nowTap = true;
			}
			if(_nowTap)
			{
				_waitTimeForListReset += Time.deltaTime;

				// 上下のひっぱり時にバー表示
				if (_waitTimeForListReset > 0.4f) 
				{
					if (_InfiniteLimitScroll.transform.childCount > 2) 
					{
						

						if (_InfiniteLimitScrollBar.GetComponent<Scrollbar> ().value >= 1.0f)
						{
							Transform t = _InfiniteLimitScroll.transform.GetChild (1);
							t.GetChild (t.transform.childCount - 2).gameObject.SetActive (true);
							_loadingDownObject.SetActive(true);

						} else {
							Transform t = _InfiniteLimitScroll.transform.GetChild (_InfiniteLimitScroll.transform.childCount - 1);
							t.GetChild (t.transform.childCount - 1).gameObject.SetActive (true);
							_loadingUpObject.SetActive(true);

						}
					}
				}
			}
			*/
			
	
		}
        
        /// <summary>
        /// Nexts the page.
        /// </summary>
        /// <returns>The page.</returns>
        public void NextPageButton () {
            StartCoroutine (Init (1));
        }
        
        public void GameobjectReset() {
            if (_InfiniteLimitScroll.transform.childCount > 1 ) {
                for (int i = 0; i < _InfiniteLimitScroll.transform.childCount; i++) {
                    if (i != 0) {
                        Destroy (_InfiniteLimitScroll.transform.GetChild (i).gameObject);
                    }
                }
            }
        }
        
	}
}
