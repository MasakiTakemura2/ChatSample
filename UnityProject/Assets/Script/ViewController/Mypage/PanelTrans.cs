﻿using UnityEngine;
using System.Collections;
using EventManager;


namespace ViewController 
{
    public class PanelTrans : SingletonMonoBehaviour<PanelTrans>
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
                        OtherSetting.Instance.WebviewTransClose (this.gameObject);
                    }
                }
            }
        }
    }
}