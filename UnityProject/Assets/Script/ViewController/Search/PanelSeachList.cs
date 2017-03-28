using UnityEngine;
using System.Collections;
using Http;
using Helper;
using UnityEngine.UI;

using uTools;


namespace ViewController
{
	public class PanelSeachList : SingletonMonoBehaviour<PanelSeachList>
	{
		[SerializeField]
		public GameObject _InfiniteLimitScroll;
		[SerializeField]
		private GameObject _InfiniteLimitScrollBar;

		[SerializeField]
		private GameObject _loadingOverlay;

		[SerializeField]
		private GameObject _popupOverlay;

		public bool _disablePageForNetworkCall = false;
		public float _waitTimeForListReset = 0;
		public bool _nowTap = false;


		public void Initialize(int pageChange)
		{
			//HeaderPanel.Instance.GetComponent<Text> ().text = PanelStateManager.GetHeaderStringByKey("SearchPanel");
			EventManager.SearchEventManager.Instance._nowShowPage = "1";
			StartCoroutine (Init (pageChange));
		}
		IEnumerator Init(int pageChange)
		{
			_loadingOverlay.SetActive (true);
         
			// 通信レスポンス待ってから
			UserListApi._success = false;
			int page_next = int.Parse (EventManager.SearchEventManager.Instance._nowShowPage) + pageChange;

            //経度、緯度セット
            string lat = "";
            string lng = "";
            
            if (string.IsNullOrEmpty(GetUserApi._httpCatchData.result.user.lat) == false )
                lat = GetUserApi._httpCatchData.result.user.lat;
            
            if (string.IsNullOrEmpty(GetUserApi._httpCatchData.result.user.lng) == false) 
               lng = GetUserApi._httpCatchData.result.user.lng;

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
					_InfiniteLimitScroll.GetComponent<SearchListInfiniteLimitScroll> ().Init ();

					// 更新後のリストの位置にバーをあわせる
					if (pageChange >= 0)
					{
						_InfiniteLimitScrollBar.GetComponent<Scrollbar> ().value = 0.995f;
					} else {
						_InfiniteLimitScrollBar.GetComponent<Scrollbar> ().value = 0.005f;
					}

					int page = int.Parse(EventManager.SearchEventManager.Instance._nowShowPage) + pageChange;
                    if (page % 4 == 0) 
                    {
                        System.GC.Collect ();
                        Resources.UnloadUnusedAssets ();
                    }
					EventManager.SearchEventManager.Instance._nowShowPage = page.ToString();

				} else {
					// リストのないページカウントになってるので　一旦通信とリストの更新をさせないようにする
					_disablePageForNetworkCall = true;
					_InfiniteLimitScrollBar.GetComponent<Scrollbar> ().value = 0.005f;
					Debug.Log ("存在しないページにアクセスしようとしたのでやめる");

				}
			} else {
				_disablePageForNetworkCall = true;
				_InfiniteLimitScrollBar.GetComponent<Scrollbar> ().value = 0.005f;
				Debug.Log ("存在しないページにアクセスしようとしたのでやめる");
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

			// 通信レスポンス待ってから
			UserListApi._success = false;

            ////経度、緯度セット
            string lat = "";
            string lng = "";
            
            //if (string.IsNullOrEmpty(GetUserApi._httpCatchData.result.user.lat) == false )
            //    lat = GetUserApi._httpCatchData.result.user.lat;
            
            //if (string.IsNullOrEmpty(GetUserApi._httpCatchData.result.user.lng) == false) 
            //   lng = GetUserApi._httpCatchData.result.user.lng;
            

			new UserListApi (
				AppStartLoadBalanceManager._userKey,
				lat,
				lng,
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
				yield return (UserListApi._success == true);

			if (UserListApi._httpCatchData.result.users != null)
			{
				if (UserListApi._httpCatchData.result.users.Count > 0)
				{
					_InfiniteLimitScroll.GetComponent<SearchListInfiniteLimitScroll> ().m_currentItemNo = 0;

					_InfiniteLimitScroll.GetComponent<SearchListInfiniteLimitScroll> ().Init ();

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
				case EventManager.SearchEventManager.StatePanel.Notify:

				break;
				default:
					return;
				break;
			}
			if(EventManager.SearchEventManager.Instance._listConnectUpdateDisable)
			{
				return;
			}


			if (Input.GetMouseButtonUp (0))
			{
				/*
				if(_InfiniteLimitScroll.transform.childCount > 2)
				{
					Transform t = _InfiniteLimitScroll.transform.GetChild (1);
					t.GetChild(t.transform.childCount-2).gameObject.SetActive(false);
					Transform t2 = _InfiniteLimitScroll.transform.GetChild (_InfiniteLimitScroll.transform.childCount-1);
					t2.GetChild(t2.transform.childCount-1).gameObject.SetActive(false);
				}

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
					//_nowShowPage--;
					StartCoroutine (Init (-1));
					//_InfiniteLimitScrollBar.GetComponent<Scrollbar> ().value = 0.005f;
					//_disablePageForNetworkCall = false;
					return;
				}



				// バーの監視　下に下りきったらページを変える
				if (_InfiniteLimitScrollBar.GetComponent<Scrollbar> ().value <= 0.0f)
				{
					if (_disablePageForNetworkCall)
					{
						//Debug.Log ("存在しないページにアクセスしようとしたのでやめる");
						//_InfiniteLimitScrollBar.GetComponent<Scrollbar> ().value = 0.05f;
						//return;
					}
					//_nowShowPage++;
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
						} else {
							Transform t = _InfiniteLimitScroll.transform.GetChild (_InfiniteLimitScroll.transform.childCount - 1);
							t.GetChild (t.transform.childCount - 1).gameObject.SetActive (true);
						}
					}
				}
			}
			*/
		}

        /// <summary>
        /// Searchs the list object reset.
        /// メモリヒープ懸念のため、Gameobjectを強制的に破棄。
        /// </summary>
        public void SearchListGameObjectReset () 
        {
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
