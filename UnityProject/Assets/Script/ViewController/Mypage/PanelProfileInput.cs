using UnityEngine;
using System.Collections;
using EventManager;
using UnityEngine.UI;

namespace ViewController 
{
    public class PanelProfileInput : SingletonMonoBehaviour<PanelProfileInput>
    {
        [SerializeField]
        private GameObject _okButton;

        public InputField _message;
        public static string _postMessage;

        public void OnValueChanaged() {
//            _postMessage = _message.text;
//            Debug.Log (_message.text + " <>>><< ") ;
        }

        public void OnEnded() {
            _postMessage = _message.text;
            Debug.Log (_message.text + " <>>><< ") ;
        }

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
                    if (PanelProfileChange.Instance != null)
                    {
                        PanelProfileChange.Instance.PanelProfileInputClose ();
                    }
                }
            }
        }


        #region 外部用メソッド。
        /// <summary>
        /// Sets the infinite limit scroll.
        /// </summary>
        public void SetProfInput()
        {
            if (PanelProfileChange.Instance != null) {
                PanelProfileChange.Instance._profileInput = _message.gameObject;
            }
        }
        
        /// <summary>
        /// Oks the button switch.
        /// </summary>
        /// <returns>The button switch.</returns>
        /// <param name="isSwitch">Is switch.</param>
        public void OkButtonSwitch(bool isSwitch) {
            _okButton.SetActive (isSwitch);
        }
           
        #endregion

    }
}