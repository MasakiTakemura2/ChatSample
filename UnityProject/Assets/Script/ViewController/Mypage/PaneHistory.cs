using UnityEngine;
using System.Collections;
using Http;
using UnityEngine.UI;
using EventManager;

namespace ViewController
{
    public class PaneHistory : SingletonMonoBehaviour<PaneHistory>
    {
        [SerializeField]
        private HistoryListInfiniteLimitScroll _historyListInfiniteLimitScroll;

        [SerializeField]
        private GameObject _historyListInfiniteLimitScrollBar;

        [SerializeField]
        private Transform _footPrintButton;
        
        [SerializeField]
        private Transform _passingButton;
        
        [SerializeField]
        private Transform _favoriteButton;

        [SerializeField]
        private GameObject _loadingOverlay;

        public int _nowShowPage = 0;
        private bool _isFirstOpen = false;
        public HistoryType _historyType = HistoryType.FootPrint;

        public void Init (int pageChange = 0) {
            if (_isFirstOpen == false) {
                HistoryButton (1);
            }
            StartCoroutine (StartHistory(pageChange));
        }
        
        public bool _isUpdate = false;

    	/// <summary>
        /// Start this instance.
        /// </summary>
    	private IEnumerator StartHistory(int pageChange = 0)
        {
            //最初の表示だけ。
            int page_next = _nowShowPage + pageChange;

            _loadingOverlay.SetActive (true);

            new HistoryUserListApi (((int)_historyType).ToString(), page_next.ToString());
            while (HistoryUserListApi._success == false) 
                yield return (HistoryUserListApi._success == true);

            if (HistoryUserListApi._httpCatchData.result.users != null)
            {
                if (HistoryUserListApi._httpCatchData.result.users.Count > 0)
                {
                    _historyListInfiniteLimitScroll.Init ();

                    // 更新後のリストの位置にバーをあわせる
                    if (pageChange >= 0)
                    {
                        _historyListInfiniteLimitScrollBar.GetComponent<Scrollbar> ().value = 0.995f;
                    } else {
                        _historyListInfiniteLimitScrollBar.GetComponent<Scrollbar> ().value = 0.005f;
                    }

                    int page = _nowShowPage + pageChange;
                    if (page % 4 == 0) 
                    {
                        System.GC.Collect ();
                        Resources.UnloadUnusedAssets ();
                    }

                    _nowShowPage = page;

                } else {
                    // リスト全消し
                    if (page_next == 0) {
                        if(_historyListInfiniteLimitScroll.transform.childCount > 1)
                        {
                            for(int i=1; i < _historyListInfiniteLimitScroll.transform.childCount; i++)
                            {
                                Destroy(_historyListInfiniteLimitScroll.transform.GetChild (i).gameObject);
                            }
                        }
                        Debug.Log ("存在しないページにアクセスしようとしたのでやめる");
                    } else {
                        _historyListInfiniteLimitScrollBar.GetComponent<Scrollbar> ().value = 0.005f;
                        // リストのないページカウントになってるので　一旦通信とリストの更新をさせないようにする
                    }
                }
            } else {
                // リスト全消し
                if (page_next == 0) {
                    if(_historyListInfiniteLimitScroll.transform.childCount > 1)
                    {
                        for(int i=1; i < _historyListInfiniteLimitScroll.transform.childCount; i++)
                        {
                            Destroy(_historyListInfiniteLimitScroll.transform.GetChild (i).gameObject);
                        }
                    }
                    Debug.Log ("存在しないページにアクセスしようとしたのでやめる");
                } else {
                    _historyListInfiniteLimitScrollBar.GetComponent<Scrollbar> ().value = 0.005f;
                    // リストのないページカウントになってるので　一旦通信とリストの更新をさせないようにする
                }
                Debug.Log ("存在しないページにアクセスしようとしたのでやめる");
            }

            _loadingOverlay.SetActive (false);
             
            //トリガーフラグ更新。
            _isUpdate    = true;
            _isFirstOpen = true;
            yield break;
    	}
        
        /// <summary>
        /// Update this instance.
        /// </summary>
        void Update()
        {
            if (_isUpdate == false) return;

            if (Input.GetMouseButtonUp (0))
            {
                // バーの監視　上に上りきったらページを変える
                if (_historyListInfiniteLimitScrollBar.GetComponent<Scrollbar> ().value >= 1.0f) 
                {
                    if (_nowShowPage - 1 < 1)
                    {
                        // ページ１以下は存在しないはず
                        return;
                    }
                    StartCoroutine (StartHistory (-1));
                    return;
                }

                // バーの監視　下に下りきったらページを変える
                if (_historyListInfiniteLimitScrollBar.GetComponent<Scrollbar> ().value <= 0.0f)
                {
                    StartCoroutine (StartHistory (1));
                    return;
                }
            }
        }
        
        #region Finger Event
        /// <summary>
        /// Backs the swipe.
        /// </summary>
        void OnSwipe (SwipeGesture gesture) {
            if (gesture.Selection) {
                if (gesture.Direction == FingerGestures.SwipeDirection.Left) {
                    //Debug.Log ("Left Left Left Left Left Left ");
                } else if (gesture.Direction == FingerGestures.SwipeDirection.Right) {
                    Debug.Log ("Right Right Right Right Right Right Right ");

                    if (MypageEventManager.Instance != null) 
                        MypageEventManager.Instance.HistoryListClose (this.gameObject);
                }
            }
        }
        #endregion
        

        #region ボタンメソッド
        /// <summary>
        /// Foots the print button.
        /// </summary>
        /// <returns>The print button.</returns>
        public void HistoryButton (int s)
        {
            switch (s) {
            case (int)HistoryType.FootPrint:
                _historyType = HistoryType.FootPrint;
                _footPrintButton.GetChild (0).gameObject.SetActive (false);
                _footPrintButton.GetChild (1).gameObject.SetActive (true);
                
                _passingButton.GetChild (0).gameObject.SetActive (true);
                _passingButton.GetChild (1).gameObject.SetActive (false);
                
                _favoriteButton.GetChild (0).gameObject.SetActive (true);
                _favoriteButton.GetChild (1).gameObject.SetActive (false);
                _nowShowPage = 0;

                if (_isFirstOpen == true) {
                    Init ();
                }
                
                break;
            case (int)HistoryType.Passing:
                _historyType = HistoryType.Passing;
                _footPrintButton.GetChild (0).gameObject.SetActive (true);
                _footPrintButton.GetChild (1).gameObject.SetActive (false);
                
                _passingButton.GetChild (0).gameObject.SetActive (false);
                _passingButton.GetChild (1).gameObject.SetActive (true);
                
                _favoriteButton.GetChild (0).gameObject.SetActive (true);
                _favoriteButton.GetChild (1).gameObject.SetActive (false);
                _nowShowPage = 0;
                Init ();
                break;
            case (int)HistoryType.Favorite:
                _historyType = HistoryType.Favorite;
                _footPrintButton.GetChild (0).gameObject.SetActive (true);
                _footPrintButton.GetChild (1).gameObject.SetActive (false);
                
                _passingButton.GetChild (0).gameObject.SetActive (true);
                _passingButton.GetChild (1).gameObject.SetActive (false);
                
                _favoriteButton.GetChild (0).gameObject.SetActive (false);
                _favoriteButton.GetChild (1).gameObject.SetActive (true);
                _nowShowPage = 0;
                Init ();
                break;
            }
        }
        #endregion
    }
}