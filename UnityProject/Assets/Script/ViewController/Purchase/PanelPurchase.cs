using UnityEngine;
using System.Collections;

namespace ViewController
{
    /// <summary>
    /// Panel purchase.
    /// </summary>
    public class PanelPurchase : SingletonMonoBehaviour<PanelPurchase>
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
                    Helper.LocalFileHandler.Init ( LocalFileConstants.GetLocalFileDir() + LocalFileConstants.COMMON_LOCAL_FILE_NAME);
                    string fromScene = Helper.LocalFileHandler.GetString (LocalFileConstants.FROM_MYPAGE_SCENE);
                    if (string.IsNullOrEmpty (fromScene) == false && fromScene == CommonConstants.MYPAGE_SCENE) {
                        Helper.LocalFileHandler.SetString (LocalFileConstants.FROM_MYPAGE_SCENE, "");
                        Helper.LocalFileHandler.Flush ();
                        SceneHandleManager.NextSceneRedirect (CommonConstants.MYPAGE_SCENE);
                    }
                }
            }
        }
    }
}