using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EventManager;
using Helper;

namespace ViewController {
    public class MaioMovieSdkEvent : SingletonMonoBehaviour<MaioMovieSdkEvent>
    {
        public enum WhereFromAds {
            None,
            MessageList,
            BoardDetailChatOpen,
            BoardToProfileToMessageSend,
            ProfileToChat,
            PanelChat,
            MatchBackloadImage,
            MatchLikeLimit,
            MatchSupreLike
        }
        
        /// <summary>
        /// Onclose the specified zoneId.
        /// </summary>
        /// <returns>The onclose.</returns>
        /// <param name="zoneId">Zone identifier.</param>
        public static void OnClosed(string zoneId)
        {
            if (AppStartLoadBalanceManager._whereFromAds != WhereFromAds.None) {
                if (AppStartLoadBalanceManager._whereFromAds == WhereFromAds.MessageList) {
                    MessageEventManager.Instance.OnClosedAd (zoneId);
                } else if (AppStartLoadBalanceManager._whereFromAds == WhereFromAds.PanelChat) {
                    PanelChat.Instance.OnClosed (zoneId);
                } else if (AppStartLoadBalanceManager._whereFromAds == WhereFromAds.ProfileToChat) {
                    ProfilePanel.Instance.OnClosedAd (zoneId);
                } else if (AppStartLoadBalanceManager._whereFromAds == WhereFromAds.MatchLikeLimit) {
                    PanelPopupLikeLimit.Instance.OnClosedAd (zoneId);
                } else if (AppStartLoadBalanceManager._whereFromAds == WhereFromAds.MatchBackloadImage) {
                    TinderGesture.Instance.OnClosedAdToBackLoadImage (zoneId);
                } else if (AppStartLoadBalanceManager._whereFromAds == WhereFromAds.MatchSupreLike) {
                    TinderGesture.Instance.OnCloseAdToSuperLike (zoneId);
                } else if (AppStartLoadBalanceManager._whereFromAds == WhereFromAds.BoardToProfileToMessageSend) {
                    BulletinBoardEventManager.Instance.ProfileToChatOpenMethod (zoneId);
                } else if (AppStartLoadBalanceManager._whereFromAds == WhereFromAds.BoardDetailChatOpen) {
                    BulletinBoardEventManager.Instance.BoardDetailToChatMethod (zoneId);
                }
                
            }
            AppStartLoadBalanceManager._whereFromAds = WhereFromAds.None;
        }
       
        /// <summary>
        /// Ons the finished ad event handler.
        /// </summary>
        /// <param name="zoneId">Zone identifier.</param>
        /// <param name="playtime">Playtime.</param>
        /// <param name="skipped">If set to <c>true</c> skipped.</param>
        /// <param name="rewardParam">Reward parameter.</param>
        public static void OnFinishedAd(string zoneId, int playtime, bool skipped, string rewardParam) {
            //スキップしても動作はするように設置。
            if (skipped == true) {
                OnClosed (zoneId);
            }

            AppStartLoadBalanceManager._whereFromAds = WhereFromAds.None;
        }
    }
}

