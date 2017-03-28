using UnityEngine;
using System.Collections;
using EventManager;


namespace ViewController 
{
    public class PanelCampaign : SingletonMonoBehaviour<PanelCampaign>
    {
        /// <summary>
        /// Backs the swipe.
        /// </summary>
        void OnSwipe (SwipeGesture gesture) {
            if (gesture.Selection) {
                if (gesture.Direction == FingerGestures.SwipeDirection.Left)
                {
                    //Debug.Log ("Left Left Left Left Left Left ");
                }
                else if (gesture.Direction == FingerGestures.SwipeDirection.Right)
                {
                    if (OtherSetting.Instance != null)
                    {
                        OtherSetting.Instance.WebviewTermClose (this.gameObject);
                    }
                }
            }
        }
    }
}