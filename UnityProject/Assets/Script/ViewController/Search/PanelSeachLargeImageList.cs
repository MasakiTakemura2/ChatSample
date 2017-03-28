using UnityEngine;
using System.Collections;
using Http;
using Helper;
using UnityEngine.UI;
using EventManager;
using uTools;

namespace ViewController
{
	public class PanelSeachLargeImageList : SingletonMonoBehaviour<PanelSeachLargeImageList>
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
		private GameObject _loadingDownObject;
		[SerializeField]
		private GameObject _loadingUpObject;



		public bool _disablePageForNetworkCall = false;
		public float _waitTimeForListReset = 0; // タップとリストの更新を区別するため
		public bool _nowTap = false;

		//画面初期化処理
		void Start ()
		{
			
			if(_InfiniteLimitScrollBar != null)
			{
				_InfiniteLimitScrollBar.GetComponent<Scrollbar> ().value = 0.995f;
			}
		}

		public void Initialize(int pageChange)
		{
			//HeaderPanel.Instance.GetComponent<Text> ().text = PanelStateManager.GetHeaderStringByKey("SearchPanel");

			StartCoroutine (Init (pageChange));
		}
		IEnumerator Init(int pageChange)
		{

string _commonFileName = LocalFileConstants.GetLocalFileDir () + LocalFileConstants.COMMON_LOCAL_FILE_NAME;

LocalFileHandler.Init (_commonFileName);

//ファイルが作成されるまでポーリングして処理待ち
while (System.IO.File.Exists (_commonFileName) == false)
    yield return (System.IO.File.Exists (_commonFileName) == true);

_loadingOverlay.SetActive (true);
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
			int page_next = int.Parse (EventManager.SearchEventManager.Instance._nowShowPage) + pageChange;
			new UserListApi (
				AppStartLoadBalanceManager._userKey,
				SearchEventManager.Instance._lat,
				SearchEventManager.Instance._lng,
				SearchEventManager.Instance._order,
				SearchEventManager.Instance._sex,
				SearchEventManager.Instance._ageFrom,
				SearchEventManager.Instance._ageTo,
				SearchEventManager.Instance._heightFrom,
				SearchEventManager.Instance._heightTo,
				SearchEventManager.Instance._bodyType,
				SearchEventManager.Instance._isImage,
				SearchEventManager.Instance._radius,
				SearchEventManager.Instance._keyword,
				page_next.ToString()//EventManager.SearchEventManager.Instance._nowShowPage
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
					_InfiniteLimitScroll.GetComponent<SearchListLargeImageInfiniteLimitScroll> ().Init ();

					// 更新後のリストの位置にバーをあわせる
					if (pageChange >= 0)
					{
						_InfiniteLimitScrollBar.GetComponent<Scrollbar> ().value = 0.995f;
					} else {
						_InfiniteLimitScrollBar.GetComponent<Scrollbar> ().value = 0.005f;
					}


					int page = int.Parse(SearchEventManager.Instance._nowShowPage) + pageChange;
                    if (page % 4 == 0){
                        System.GC.Collect ();
                        Resources.UnloadUnusedAssets ();
                    }
					SearchEventManager.Instance._nowShowPage = page.ToString();
				} else {
					// リストのないページカウントになってるので　一旦通信とリストの更新をさせないようにする
					_disablePageForNetworkCall = true;
					_InfiniteLimitScrollBar.GetComponent<Scrollbar> ().value = 0.995f;
					Debug.Log ("存在しないページにアクセスしようとしたのでやめる");

					//_InfiniteLimitScroll.GetComponent<SearchListLargeImageInfiniteLimitScroll> ().SetPanelMax(0);
					//_InfiniteLimitScroll.GetComponent<SearchListLargeImageInfiniteLimitScroll> ().SetPanelInstantateCount(0);

				}
			} else {
				_disablePageForNetworkCall = true;
				_InfiniteLimitScrollBar.GetComponent<Scrollbar> ().value = 0.995f;
				Debug.Log ("存在しないページにアクセスしようとしたのでやめる");

				//_InfiniteLimitScroll.GetComponent<SearchListLargeImageInfiniteLimitScroll> ().SetPanelMax(0);
				//_InfiniteLimitScroll.GetComponent<SearchListLargeImageInfiniteLimitScroll> ().SetPanelInstantateCount(0);

			}

			_loadingOverlay.SetActive (false);
		}

		// 条件検索
		public void InitializeFromCondiiton()
		{
			StartCoroutine (InitFromCondition ());
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
				AppStartLoadBalanceManager._userKey,
				SearchEventManager.Instance._lat,
				SearchEventManager.Instance._lng,
				SearchEventManager.Instance._order,
				SearchEventManager.Instance._sex,
				SearchEventManager.Instance._ageFrom,
				SearchEventManager.Instance._ageTo,
				SearchEventManager.Instance._heightFrom,
				SearchEventManager.Instance._heightTo,
				SearchEventManager.Instance._bodyType,
				SearchEventManager.Instance._isImage,
				SearchEventManager.Instance._radius,
				SearchEventManager.Instance._keyword,
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

Debug.Log (" 検索条件を保存。 ");

			if (UserListApi._httpCatchData.result.users != null)
			{
				if (UserListApi._httpCatchData.result.users.Count > 0)
				{
					_InfiniteLimitScroll.GetComponent<SearchListLargeImageInfiniteLimitScroll> ().m_currentItemNo = 0;


					_InfiniteLimitScroll.GetComponent<SearchListLargeImageInfiniteLimitScroll> ().Init ();

					EventManager.SearchEventManager.Instance._nowShowPage = "1";
					_InfiniteLimitScrollBar.GetComponent<Scrollbar> ().value = 0.995f;
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
					if (!EventManager.SearchEventManager.Instance._isTwoColumn) 
					{
						return;
					}
				break;

				default:
					return;
				break;
			}

			if(EventManager.SearchEventManager.Instance._listConnectUpdateDisable)
			{
				return;
			}


			if(Input.GetMouseButtonUp (0)) 
			{
				/*
				_loadingUpObject.SetActive(false);
				_loadingDownObject.SetActive(false);


				if (_waitTimeForListReset < 0.4f) 
				{
					_nowTap = false;
					_waitTimeForListReset = 0;
					return;
				}
				_nowTap = false;
				_waitTimeForListReset = 0;
				*/

				// バーの監視　上に上りきったらページを変える
				if (_InfiniteLimitScrollBar.GetComponent<Scrollbar> ().value >= 1.0f) 
				{
					if (int.Parse(EventManager.SearchEventManager.Instance._nowShowPage) - 1 < 1)
					{
						// ページ１以下は存在しないはず
						return;
					}
					StartCoroutine (Init (-1));
					//_InfiniteLimitScrollBar.GetComponent<Scrollbar> ().value = 0.005f;
					//_InfiniteLimitScrollBar.GetComponent<Scrollbar> ().value = 0.05f;

					return;
				}



				// バーの監視　下に下りきったらページを変える
				if (_InfiniteLimitScrollBar.GetComponent<Scrollbar> ().value <= 0.0f)
				{
					StartCoroutine (Init (1));
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
							_loadingUpObject.SetActive(true);

						} else {
							Transform t = _InfiniteLimitScroll.transform.GetChild (_InfiniteLimitScroll.transform.childCount - 1);
							t.GetChild (t.transform.childCount - 1).gameObject.SetActive (true);
							_loadingDownObject.SetActive(true);

						}
					}
				}
			}
			*/
	
		}

        /// <summary>
        /// Gameobjects the reset.
        /// メモリヒープ懸念のため、Gameobjectを強制的に破棄。
        /// </summary>
        public void GameobjectReset() {
            if (_InfiniteLimitScroll.transform.childCount > 1 ) {
                for (int i = 0; i < _InfiniteLimitScroll.transform.childCount; i++) {
                    if (i > 0) {
                        Destroy (_InfiniteLimitScroll.transform.GetChild (i).gameObject);
                    }
                }
            }
        }
	}
}
