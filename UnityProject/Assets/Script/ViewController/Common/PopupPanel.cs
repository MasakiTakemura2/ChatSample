using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.Events;


namespace ViewController {
    public class PopupPanel : SingletonMonoBehaviour<PopupPanel>
    {
        [SerializeField]
        public Text _popupText;

        [SerializeField]
        public GameObject _popupOk;

        /// <summary>
        /// Pops the message insert.
        /// </summary>
        /// <param name="s">S.</param>
        /// <param name="actEvent">Act event.</param>
        public void PopMessageInsert(string s = "", string ok = "",  UnityAction OkActEvent = null)
        {
            GameObject backSwipeObj = GameObject.FindGameObjectWithTag (CommonConstants.BACK_SWIPE);
            if (backSwipeObj != null) 
            {
                backSwipeObj.GetComponent<ScreenRaycaster> ().enabled = false;
            }
        
           _popupText.text =  s;
           _popupOk.GetComponent<Text>().text = ok;
           _popupOk.GetComponent<Button>().onClick.AddListener (OkActEvent);
        }

        /// <summary>
        /// Pops the message clean.
        /// </summary>
        /// <param name="s">S.</param>
        /// <param name="OkActEvent">Ok act event.</param>
        public void PopClean(UnityAction OkActEvent = null)
        {
            GameObject backSwipeObj = GameObject.FindGameObjectWithTag (CommonConstants.BACK_SWIPE);
            if (backSwipeObj != null) 
            {
                backSwipeObj.GetComponent<ScreenRaycaster> ().enabled = true;
            }
           _popupOk.GetComponent<Button>().onClick.RemoveAllListeners ();
        }
    }
}