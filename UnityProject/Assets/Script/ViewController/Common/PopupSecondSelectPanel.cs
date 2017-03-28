using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.Events;
using Http;
using uTools;

namespace ViewController {
    public class PopupSecondSelectPanel : SingletonMonoBehaviour<PopupSecondSelectPanel>
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
        private GameObject _firstUserOnlyUI;

        [SerializeField]
        private GameObject _loadingOverlay;

        [SerializeField]
        private GameObject _popupOverlay;

        /// <summary>
        /// To user identifier.
        /// 通報用。
        /// </summary>
        public string _toUserId;

        /// <summary>
        /// Pops the message insert.
        /// </summary>
        /// <param name="s">S.</param>
        /// <param name="ok">Ok.</param>
        /// <param name="ng">Ng.</param>
        /// <param name="OkActEvent">Ok act event.</param>
        /// <param name="NgActEvent">Ng act event.</param>
        public void PopMessageInsert (string s = "", string ok = "", string ng = "", UnityAction OkActEvent = null, UnityAction NgActEvent = null, bool isReport = false)
        {
            if (isReport == true) 
            {
                _firstUserOnlyUI.SetActive (true);
            } 
            else {
                _firstUserOnlyUI.SetActive (false);
            }

            GameObject backSwipeObj = GameObject.FindGameObjectWithTag (CommonConstants.BACK_SWIPE);
            if (backSwipeObj != null) 
            {
                backSwipeObj.GetComponent<ScreenRaycaster> ().enabled = false;
            }

            _popupText.text = s;
            _popupOk.GetComponent<Text>().text = ok;
            _popupNg.GetComponent<Text>().text = ng;
            _popupOk.GetComponent<Button>().onClick.AddListener (OkActEvent);
            _popupNg.GetComponent<Button>().onClick.AddListener (NgActEvent);
        }
        
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
            {
                backSwipeObj.GetComponent<ScreenRaycaster> ().enabled = false;
            }

            if (_popupOverlay != null) 
                _popupOverlay.SetActive (true);

            //タイトル
            _popupTitle.gameObject.SetActive (true);
            _popupTitle.alignment = TextAnchor.MiddleCenter;
            _popupTitle.text     = title;
            
            //本文
            _popupText.alignment = TextAnchor.UpperLeft;
            _popupText.text      = body;

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

            if (_popupOverlay != null) 
                _popupOverlay.SetActive (false);

            GameObject backSwipeObj = GameObject.FindGameObjectWithTag (CommonConstants.BACK_SWIPE);
            if (backSwipeObj != null) 
            {
                backSwipeObj.GetComponent<ScreenRaycaster> ().enabled = true;
            }
            _popupOk.GetComponent<Button>().onClick.RemoveAllListeners();
            _popupNg.GetComponent<Button>().onClick.RemoveAllListeners();
        }



        /// <summary>
        /// Reports the confirm open.
        /// 通報ボタン
        /// </summary>
        /// <param name="target">Target.</param>
        public void ReportConfirmOpen (GameObject target)
        {
            Debug.Log (" ----------------AAAAAAAAAA ");

            if (string.IsNullOrEmpty (_toUserId) == true)
                return;

            StartCoroutine (ReportConfirmOpenCoroutine (target));
        }

        /// <summary>
        /// Reports the confirm open coroutine.
        /// </summary>
        private IEnumerator ReportConfirmOpenCoroutine ( GameObject target ) 
        {
            _loadingOverlay.SetActive (true);
            new GetUserApi (_toUserId);

            while (GetUserApi._success == false)
                yield return (GetUserApi._success == true);

            _loadingOverlay.SetActive (false);

            PopClean ();
            _firstUserOnlyUI.SetActive (false);
            string question = string.Format (LocalMsgConst.REPORT_QUESTION, GetUserApi._httpOtherUserCatchData.name);

            PopMessageInsert (
                question,
                LocalMsgConst.YES,
                LocalMsgConst.NO,
                ReportApiCall,
                ReportCancel
            );
            PanelPopupAnimate (target);            
        }

        /// <summary>
        /// Reports the API call.
        /// </summary>
        void ReportApiCall ()
        {
            PopupSecondSelectPanel.Instance.PopClean ();

            StartCoroutine (SendReportWait ());
            PanelPopupCloseAnimate (GameObject.FindGameObjectWithTag (CommonConstants.POPUP_SECOND_SELECT_TAG));
        }

        /// <summary>
        /// Sends the report wait.
        /// </summary>
        /// <returns>The report wait.</returns>
        private IEnumerator SendReportWait ()
        {
            _loadingOverlay.SetActive (true);
            new SendReportApi (GetUserApi._httpOtherUserCatchData.id);
            while (SendReportApi._success == false)
                yield return (SendReportApi._success == true);
            _loadingOverlay.SetActive (false);

            PopupPanel.Instance.PopMessageInsert (
                SendReportApi._httpCatchData.result.complete [0],
                LocalMsgConst.OK,
                ReportFinishClose
            );
            PanelPopupAnimate (GameObject.FindGameObjectWithTag (CommonConstants.POPUP_BASIC_TAG));
        }

        /// <summary>
        /// Reports the cancel.
        /// </summary>
        void ReportCancel ()
        {
            PopupSecondSelectPanel.Instance.PopClean ();

            PanelPopupCloseAnimate (GameObject.FindGameObjectWithTag (CommonConstants.POPUP_SECOND_SELECT_TAG));
        }

        /// <summary>
        /// Reports the finish close.
        /// </summary>
        void ReportFinishClose ()
        {
            PopupPanel.Instance.PopClean ();

            PanelPopupCloseAnimate (GameObject.FindGameObjectWithTag (CommonConstants.POPUP_BASIC_TAG));
        }

        /// <summary>
        /// Panels the popup animate.
        /// </summary>
        /// <param name="target">Target.</param>
        public void PanelPopupAnimate ( GameObject target )
        {
            _popupOverlay.SetActive (true);
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
            _popupOverlay.SetActive (false);
            target.GetComponent<uTweenScale> ().delay    = 0.01f;
            target.GetComponent<uTweenScale> ().duration = 0.25f;
            target.GetComponent<uTweenScale> ().from = new Vector3 (1f, 1f ,1f );
            target.GetComponent<uTweenScale> ().to = Vector3.zero;
            target.GetComponent<uTweenScale> ().ResetToBeginning ();
            target.GetComponent<uTweenScale> ().enabled = true;
        }
    }
}