using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using EventManager;
using Http;
using Helper;

namespace ViewController
{
    /// <summary>
    /// Panel popup like limit.
    /// </summary>
    public class PanelPopupLikeLimit : SingletonMonoBehaviour<PanelPopupLikeLimit> 
    {
        [SerializeField]
        private Text _timer;

        [SerializeField]
        private Text _description;

        [SerializeField]
        private Text _limitButton;

        [SerializeField]
        private GameObject _loadingOverlay;

        private DateTime _startTime;
        private DateTime _elapsed;

        void Update () 
        {   
            DateTime now = DateTime.Now;
            if (now != this._elapsed)
            {
                TimeSpan deltaTime = now - _startTime;
                _timer.text = deltaTime.Hours.ToString("D2").Replace("-","") + ":" + deltaTime.Minutes.ToString("D2").Replace("-","") + ":" + deltaTime.Seconds.ToString("D2").Replace("-","");
                this._elapsed = System.DateTime.Now;
            }
        }
        
        /// <summary>
        /// Init the specified match.
        /// </summary>
        /// <param name="match">Match.</param>
        public void Init (SetLikeUserEntity.Match match)
        {
            MatchingEventManager.Instance.PanelPopupAnimate (this.gameObject);
            const string DEFAULT = "00 : 00 : 00";

            if (string.IsNullOrEmpty (match.release_time) == false) {
                match.release_time.Replace ("-", "/");
                DateTime dt = DateTime.Parse (match.release_time);
                _startTime    = dt;
                this._elapsed = DateTime.Now;
                _timer.text   = DEFAULT;
            }
            
            //string tmpText = string.Format (LocalMsgConst.POINT_LIMIT_TXT, match.point, match.limit_count);
            //_description.text = tmpText;
            _limitButton.text = match.limit_count + LocalMsgConst.LIKE_TEXT;
        }
        
        /// <summary>
        /// Revives the like.
        /// </summary>
        /// <returns>The like.</returns>
        public void ReviveLike() 
        {
#if UNITY_IPHONE || UNITY_ANDROID && !UNITY_EDITOR
                if (GetUserApi._httpCatchData.result.review == "false") {
                    //無料会員の場合。
                    if (CommonConstants.IS_PREMIUM == false) {
    
                        //動画広告が何もない場合はアイモバイル、インターステシャルを表示。
                        if (Maio.CanShow (CommonConstants.MAIO_ZONE_ID_2) == false) {
    
                            if (AppStartLoadBalanceManager.m_NendAdFullBoard != null){
                                AppStartLoadBalanceManager.m_NendAdFullBoard.Load ();
                                AppStartLoadBalanceManager.m_NendAdFullBoard.Show ();
                            }
                            //インタースティシャルのイベントが取れないのでバナー表示で機能が使えるように。
                            StartCoroutine (ReviveLikeWait ());
                        } else {
AppStartLoadBalanceManager._whereFromAds = MaioMovieSdkEvent.WhereFromAds.MatchLikeLimit;
                            //問答無用で動画広告を表示
                            Maio.Show (CommonConstants.MAIO_ZONE_ID_2);
                            return;
                        }
                    } else {
                        //有料会員の場合、広告表示は一切しない。
                        StartCoroutine (ReviveLikeWait ());
                    }
                } else {
                    StartCoroutine (ReviveLikeWait ());
                }
#endif

        }
        
        public void OnClosedAd (string zoneId) {
             StartCoroutine (ReviveLikeWait ());
        }
        
        
        /// <summary>
        /// Revives the like API.
        /// </summary>
        /// <returns>The like API.</returns>
        private IEnumerator ReviveLikeWait() 
        {
            _loadingOverlay.SetActive (true);
            new ReviveLikeApi ();
            while (ReviveLikeApi._success == false)
                yield return (ReviveLikeApi._success == true);
            _loadingOverlay.SetActive (false);
            Close ();
            PopupPanel.Instance.PopMessageInsert (
                ReviveLikeApi._httpCatchData.result.complete[0],
                LocalMsgConst.OK,
                ReviveLikeFinishClose
            );
            MatchingEventManager.Instance.PanelPopupAnimate (GameObject.FindGameObjectWithTag(CommonConstants.POPUP_BASIC_TAG));
        }
        
        void ReviveLikeFinishClose () {
            TinderGesture.Instance._isEventPopUp = false;
            PopupPanel.Instance.PopClean (); 
            MatchingEventManager.Instance.PanelPopupCloseAnimate (GameObject.FindGameObjectWithTag(CommonConstants.POPUP_BASIC_TAG));
        }
        
        /// <summary>
        /// Close this instance.
        /// </summary>
        public void Close () {
            TinderGesture.Instance._isEventPopUp = false;
            MatchingEventManager.Instance.PopUpPanelClose (this.gameObject);  
        }
    }
}