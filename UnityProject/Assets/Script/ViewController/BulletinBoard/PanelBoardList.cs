using UnityEngine;
using System.Collections;
using Http;
using Helper;
using UnityEngine.UI;
using uTools;
using EventManager;

namespace ViewController
{
	public class PanelBoardList : SingletonMonoBehaviour<PanelBoardList>
	{
		[SerializeField]
		public GameObject _boardListInfiniteLimitScroll;

		[SerializeField]
		private GameObject _boardListInfiniteLimitScrollBar;

		[SerializeField]
		private GameObject _loadingOverlay;
		[SerializeField]
		private GameObject _popupOverlay;
        
		//[SerializeField]
		//private RectTransform _upbarLoadingImage;
		//[SerializeField]
		//private RectTransform _downbarLoadingImage;
		//private float _updownLoadingImageBaseY;
              
		public bool _disablePageForNetworkCall = false;
		public float _waitTimeForListReset = 0; // タップとリストの更新を区別するため
		public bool _nowTap = false;
		public bool _connectWait = false;	// 重複してとばさないようにフラグを持っておく　（必要ないかもしれないが一応）


		//掲示板画面初期化処理
		IEnumerator Start ()
		{
            //Event Start 処理（ライフサイクル処理）を待つ。
            while (BulletinBoardEventManager.Instance._isEventStart == false)
                yield return (BulletinBoardEventManager.Instance._isEventStart == true);
            Initialize (0);
            yield break;
		}
		public void Initialize(int ChangePage)
		{			
			StartCoroutine (Init (ChangePage));
		}
		// 初期化　changepaeはページが進むか戻るか
		IEnumerator Init(int ChangePage)
		{
			//HeaderPanel.Instance.GetComponent<Text> ().text = PanelStateManager.GetHeaderStringByKey("BulletInBoardPanel");

			_loadingOverlay.SetActive(true);

			// 通信レスポンス待ってから
			int page_next = int.Parse(EventManager.BulletinBoardEventManager.Instance._nowShowPage) + ChangePage;
string _commonFileName = LocalFileConstants.GetLocalFileDir () + LocalFileConstants.COMMON_LOCAL_FILE_NAME;

LocalFileHandler.Init (_commonFileName);

//ファイルが作成されるまでポーリングして処理待ち
while (System.IO.File.Exists (_commonFileName) == false)
    yield return (System.IO.File.Exists (_commonFileName) == true);

BulletinBoardEventManager.SearchCondition SearchCondition = LocalFileHandler.Load<BulletinBoardEventManager.SearchCondition>(LocalFileConstants.BBS_SEARCH_CONDITION_KEY);

            //条件検索のデータがローカルファイルに残っている場合。
            if (SearchCondition != null) {
                if (string.IsNullOrEmpty (SearchCondition._lat) == false)
                    BulletinBoardEventManager.Instance._lat = SearchCondition._lat;
                    
                if (string.IsNullOrEmpty (SearchCondition._lng) == false) 
                    BulletinBoardEventManager.Instance._lng = SearchCondition._lng;
                    
                if (string.IsNullOrEmpty (SearchCondition._CategoryID) == false)
                    BulletinBoardEventManager.Instance._CategoryID = SearchCondition._CategoryID;
                    
                if (string.IsNullOrEmpty(SearchCondition._sex) == false)
                    BulletinBoardEventManager.Instance._sex = SearchCondition._sex;
                    
                if (string.IsNullOrEmpty(SearchCondition._ageFrom) == false)
                    BulletinBoardEventManager.Instance._ageFrom = SearchCondition._ageFrom;
                    
                if (string.IsNullOrEmpty(SearchCondition._ageTo) == false) 
                    BulletinBoardEventManager.Instance._ageTo = SearchCondition._ageTo;
                    
                if (string.IsNullOrEmpty (SearchCondition._heightFrom) == false)
                    BulletinBoardEventManager.Instance._heightFrom = SearchCondition._heightFrom;
                    
                if (string.IsNullOrEmpty (SearchCondition._heightTo) == false)
                    BulletinBoardEventManager.Instance._heightTo   = SearchCondition._heightTo;
                
                if (string.IsNullOrEmpty (SearchCondition._bodyType) == false)    
                    BulletinBoardEventManager.Instance._bodyType   = SearchCondition._bodyType;
                    
                if (string.IsNullOrEmpty (SearchCondition._isImage) == false)
                    BulletinBoardEventManager.Instance._isImage    = SearchCondition._isImage;
                    
                if (string.IsNullOrEmpty (SearchCondition._radius) == false)
                   BulletinBoardEventManager.Instance._radius     = SearchCondition._radius;
                
                if (string.IsNullOrEmpty (SearchCondition._keyword) == false)
                    BulletinBoardEventManager.Instance._keyword    = SearchCondition._keyword;
            }


			new BoardListApi
			(
				AppStartLoadBalanceManager._userKey,
				BulletinBoardEventManager.Instance._CategoryID,
				BulletinBoardEventManager.Instance._lat,
				BulletinBoardEventManager.Instance._lng,
				BulletinBoardEventManager.Instance._sex,
				BulletinBoardEventManager.Instance._ageFrom,
				BulletinBoardEventManager.Instance._ageTo,
				BulletinBoardEventManager.Instance._heightFrom,
				BulletinBoardEventManager.Instance._heightTo,
				BulletinBoardEventManager.Instance._bodyType,
				BulletinBoardEventManager.Instance._isImage,
				BulletinBoardEventManager.Instance._radius,
				BulletinBoardEventManager.Instance._keyword,
				page_next.ToString()//EventManager.BulletinBoardEventManager.Instance._nowShowPage
			);
			while (BoardListApi._success == false) 
			{
				yield return (BoardListApi._success == true);
			}

			if (BoardListApi._httpCatchData.result.boards != null)
			{
				if (BoardListApi._httpCatchData.result.boards.Count > 0)
				{
					_boardListInfiniteLimitScroll.GetComponent<BoardListInfiniteLimitScroll> ().Init ();

					// 更新後のリストの位置にバーをあわせる
                    if (ChangePage >= 0)
                    {
                        _boardListInfiniteLimitScrollBar.GetComponent<Scrollbar> ().value = 0.995f;
                    } else {
                        _boardListInfiniteLimitScrollBar.GetComponent<Scrollbar> ().value = 0.005f;
                    }

					int page = int.Parse (EventManager.BulletinBoardEventManager.Instance._nowShowPage) + ChangePage;

                    if (page % 2 == 0) 
                    {
                        System.GC.Collect ();
                        Resources.UnloadUnusedAssets ();
                    }

					EventManager.BulletinBoardEventManager.Instance._nowShowPage = page.ToString();
				} else {
                    // リストのないページカウントになってるので　一旦通信とリストの更新をさせないようにする
                    _disablePageForNetworkCall = true;
                    _boardListInfiniteLimitScrollBar.GetComponent<Scrollbar> ().value = 0.995f;
                    Debug.Log ("存在しないページにアクセスしようとしたのでやめる");
				}
			} else {
                _disablePageForNetworkCall = true;
                _boardListInfiniteLimitScrollBar.GetComponent<Scrollbar> ().value = 0.995f;
                Debug.Log ("存在しないページにアクセスしようとしたのでやめる");
			}

			_loadingOverlay.SetActive(false);
		}

		// 条件しばりから　リスト更新に飛んできた場合　ページを１にするのと検索条件０でしたの場合ポップ表示
		public void InitializeFromCondition()
		{
			StartCoroutine (InitFromCondition());
		}
		IEnumerator InitFromCondition()
		{
			_loadingOverlay.SetActive(true);

            string lat = "";
            string lng = "";
#if !UNITY_EDITOR
            if (string.IsNullOrEmpty(GetUserApi._httpCatchData.result.user.lat) == false )
                lat = GetUserApi._httpCatchData.result.user.lat;
            
            if (string.IsNullOrEmpty(GetUserApi._httpCatchData.result.user.lng) == false) 
               lng = GetUserApi._httpCatchData.result.user.lng;
#endif


			// 通信レスポンス待ってから
			new BoardListApi
			(
				AppStartLoadBalanceManager._userKey,
				EventManager.BulletinBoardEventManager.Instance._CategoryID,
				lat,
				lng,
				EventManager.BulletinBoardEventManager.Instance._sex,
				EventManager.BulletinBoardEventManager.Instance._ageFrom,
				EventManager.BulletinBoardEventManager.Instance._ageTo,
				EventManager.BulletinBoardEventManager.Instance._heightFrom,
				EventManager.BulletinBoardEventManager.Instance._heightTo,
				EventManager.BulletinBoardEventManager.Instance._bodyType,
				EventManager.BulletinBoardEventManager.Instance._isImage,
				EventManager.BulletinBoardEventManager.Instance._radius,
				EventManager.BulletinBoardEventManager.Instance._keyword,
				"1"
			);
			while (BoardListApi._success == false) 
			{
				yield return (BoardListApi._success == true);
			}

string _commonFileName = LocalFileConstants.GetLocalFileDir () + LocalFileConstants.COMMON_LOCAL_FILE_NAME;

LocalFileHandler.Init (_commonFileName);

BulletinBoardEventManager.SearchCondition condition = new BulletinBoardEventManager.SearchCondition ();
condition._lat        = BulletinBoardEventManager.Instance._lat;
condition._lng        = BulletinBoardEventManager.Instance._lng;
condition._CategoryID = BulletinBoardEventManager.Instance._CategoryID;
condition._sex        = BulletinBoardEventManager.Instance._sex;
condition._ageFrom    = BulletinBoardEventManager.Instance._ageFrom;
condition._ageTo      = BulletinBoardEventManager.Instance._ageTo;
condition._heightFrom = BulletinBoardEventManager.Instance._heightFrom;
condition._heightTo   = BulletinBoardEventManager.Instance._heightTo;
condition._bodyType   = BulletinBoardEventManager.Instance._bodyType;
condition._isImage    = BulletinBoardEventManager.Instance._isImage;
condition._radius     = BulletinBoardEventManager.Instance._radius;
condition._keyword    = BulletinBoardEventManager.Instance._keyword;

LocalFileHandler.Save<BulletinBoardEventManager.SearchCondition>(LocalFileConstants.BBS_SEARCH_CONDITION_KEY, condition);
LocalFileHandler.Flush ();

			if (BoardListApi._httpCatchData.result.boards != null)
			{
				if (BoardListApi._httpCatchData.result.boards.Count > 0)
				{
					// 条件検索の場合は無条件に初期化               
					_boardListInfiniteLimitScroll.GetComponent<BoardListInfiniteLimitScroll> ().m_currentItemNo = 0;

					_boardListInfiniteLimitScroll.GetComponent<BoardListInfiniteLimitScroll> ().Init ();

					_boardListInfiniteLimitScrollBar.GetComponent<Scrollbar> ().value = 0.995f;
					EventManager.BulletinBoardEventManager.Instance._nowShowPage = "1";
				} else {
					// リストのないページカウントになってるので　一旦通信とリストの更新をさせないようにする
					_boardListInfiniteLimitScrollBar.GetComponent<Scrollbar> ().value = 0.005f;
					Debug.Log ("存在しないページにアクセスしようとしたのでやめる");

					PopupPanel.Instance.PopMessageInsert(
						LocalMsgConst.LIST_SEARCH_ZERO,
						LocalMsgConst.OK,
						BoardPanelListZeroFinishClose
					);
					PanelPopupAnimate (GameObject.FindGameObjectWithTag(CommonConstants.POPUP_BASIC_TAG));

				}
			} else {

				_boardListInfiniteLimitScrollBar.GetComponent<Scrollbar> ().value = 0.005f;
				Debug.Log ("存在しないページにアクセスしようとしたのでやめる");

				PopupPanel.Instance.PopMessageInsert(
					LocalMsgConst.LIST_SEARCH_ZERO,
					LocalMsgConst.OK,
					BoardPanelListZeroFinishClose
				);
			}

			_loadingOverlay.SetActive(false);
		}

		void BoardPanelListZeroFinishClose () 
		{
            PopupPanel.Instance.PopClean ();
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
            if (EventManager.BulletinBoardEventManager.Instance._listConnectUpdateDisable) {
                return;
            }

            if (Input.GetMouseButtonUp (0))
            {
                // バーの監視　上に上りきったらページを変える
                if (_boardListInfiniteLimitScrollBar.GetComponent<Scrollbar> ().value >= 1.0f) {
                    if (EventManager.BulletinBoardEventManager.Instance != null) {
                        if (int.Parse (EventManager.BulletinBoardEventManager.Instance._nowShowPage) - 1 < 1) {
                            
                            // ページ１以下は存在しないはず
                            return;
                        }
                        StartCoroutine (Init (-1));

                        return;
                    }
                }

                // バーの監視　下に下りきったらページを変える
                if (_boardListInfiniteLimitScrollBar.GetComponent<Scrollbar> ().value <= 0.0f)
                {
                    Debug.Log (" Bar下りきりました！！！！！！！！！！！！！ ");
                    StartCoroutine (Init (1));
                    return;
                }
            }
        }
	}
}
