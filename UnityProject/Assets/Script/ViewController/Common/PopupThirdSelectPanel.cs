using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.Events;
using Http;
using uTools;

namespace ViewController {
    public class PopupThirdSelectPanel : SingletonMonoBehaviour<PopupThirdSelectPanel>
    {
        [SerializeField]
        public Text _popupTitle;
    
        [SerializeField]
        public Text _popupText;

        [SerializeField]
        public GameObject _popupOk;

        [SerializeField]
        private GameObject _popupNg;

        [SerializeField]
        private GameObject _popupOverlay;

        [SerializeField]
        private GameObject _fromText;

        /// <summary>
        /// To user identifier.
        /// 通報用。
        /// </summary>
        public string _toUserId;

        
        /// <summary>
        /// Pops the message insert.
        /// </summary>
        /// <param name="body">S.</param>
        /// <param name="ok">Ok.</param>
        /// <param name="ng">Ng.</param>
        /// <param name="OkActEvent">Ok act event.</param>
        /// <param name="NgActEvent">Ng act event.</param>
        public void PushPopMessageInsert (string title = "", string body = "", string ok = "", string ng = "", UnityAction OkActEvent = null, UnityAction NgActEvent = null)
        {
            GameObject backSwipeObj = GameObject.FindGameObjectWithTag (CommonConstants.BACK_SWIPE);
            if (backSwipeObj != null) 
                backSwipeObj.GetComponent<ScreenRaycaster> ().enabled = false;

            if (this.transform.GetChild (0).gameObject != null) 
                this.transform.GetChild (0).gameObject.SetActive (true);

            ////タイトル
            if (string.IsNullOrEmpty (title) == false) 
            {
                _fromText.SetActive (true);
                _popupTitle.gameObject.SetActive (true);
                _popupTitle.text     = title;
                //文字の表示位置を変える処理。
                //_popupTitle.alignment = TextAnchor.MiddleCenter;
            }  else {
                _fromText.SetActive (false);
                _popupTitle.gameObject.SetActive (false);
            }
                ////本文
                //文字の表示位置を変える処理。
                _popupText.gameObject.SetActive (true);
                _popupText.text      = body;
                //文字の表示位置を変える処理。
                //_popupText.alignment = TextAnchor.UpperLeft;
    
                _popupOk.GetComponent<Text>().text = ok;
                _popupNg.GetComponent<Text>().text = ng;
                _popupOk.GetComponent<Button>().onClick.AddListener (OkActEvent);
                _popupNg.GetComponent<Button>().onClick.AddListener (NgActEvent);
            
        }
        

        /// <summary>
        /// Pops the message insert.
        /// </summary>
        /// <param name="s">S.</param>
        /// <param name="ok">Ok.</param>
        /// <param name="ng">Ng.</param>
        /// <param name="OkActEvent">Ok act event.</param>
        /// <param name="NgActEvent">Ng act event.</param>
        public void PopClean (UnityAction OkActEvent = null, UnityAction NgActEvent = null)
        {
            _popupTitle.gameObject.SetActive (false);

            if (this.transform.GetChild (0).gameObject != null) 
                this.transform.GetChild (0).gameObject.SetActive (false);

            GameObject backSwipeObj = GameObject.FindGameObjectWithTag (CommonConstants.BACK_SWIPE);
            if (backSwipeObj != null) 
            {
                backSwipeObj.GetComponent<ScreenRaycaster> ().enabled = true;
            }
            _popupOk.GetComponent<Button>().onClick.RemoveAllListeners();
            _popupNg.GetComponent<Button>().onClick.RemoveAllListeners();
        }


        /// <summary>
        /// Panels the popup animate.
        /// </summary>
        /// <param name="target">Target.</param>
        public void PanelPopupAnimate ( GameObject target )
        {
            this.transform.GetChild (0).gameObject.SetActive (true);

            target.GetComponent<uTweenScale> ().from = Vector3.zero;
            target.GetComponent<uTweenScale> ().to = new Vector3 (1, 1 ,1 );
            target.GetComponent<uTweenScale> ().delay    = 0.01f;
            target.GetComponent<uTweenScale> ().duration = 0.25f;
            target.GetComponent<uTweenScale> ().ResetToBeginning ();
            target.GetComponent<uTweenScale> ().enabled = true;
        }

        /// <summary>
        /// Panels the popup close animate.
        /// </summary>
        /// <param name="target">Target.</param>
        public void PanelPopupCloseAnimate ( GameObject target )
        {
            this.transform.GetChild (0).gameObject.SetActive (false);

            target.GetComponent<uTweenScale> ().delay    = 0.01f;
            target.GetComponent<uTweenScale> ().duration = 0.25f;
            target.GetComponent<uTweenScale> ().from = new Vector3 (1f, 1f ,1f );
            target.GetComponent<uTweenScale> ().to = Vector3.zero;
            target.GetComponent<uTweenScale> ().ResetToBeginning ();
            target.GetComponent<uTweenScale> ().enabled = true;
        }
    }
}