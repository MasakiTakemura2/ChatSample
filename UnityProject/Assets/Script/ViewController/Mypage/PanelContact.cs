using UnityEngine;
using System.Collections;
using EventManager;
using Http;
using UnityEngine.UI;
using uTools;

namespace ViewController
{
    /// <summary>
    /// Panel mail setting.
    /// </summary>
    public class PanelContact : SingletonMonoBehaviour<PanelContact>
    {
        [SerializeField]
        private GameObject _loadingOverlay;
        
        [SerializeField]
        private GameObject _popupOverlay;

        [SerializeField]
        private InputField _contactInput;
        
        [SerializeField]
        private GameObject _contactText;
        
        private string _setContact;
        private string _setPass;

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
                    
                    if (OtherSetting.Instance != null) 
                        OtherSetting.Instance.MailSettingClose (this.gameObject);
                }
            }
        }
        #endregion
        

        #region お問い合わせ送信処理
        ///TODO: pop up 共通化にする　- ここから
        /// <summary>
        /// MailSets the confirm button.
        /// </summary>
        public void ContactSend()
        {
            bool isError = false;
            if (string.IsNullOrEmpty (_contactInput.text) == true) {
                isError = true;
                _contactText.SetActive (true);
            } else {
               _contactText.SetActive (false);
            }
            
            if (isError == true)
                return;

            _setContact = _contactInput.text;

            new SendInquiryApi (_setContact);
            StartCoroutine (SendApiSetWait());
        }
        
        /// <summary>
        /// Sends the report wait.
        /// </summary>
        /// <returns>The report wait.</returns>
        private IEnumerator SendApiSetWait ()
        {
            _loadingOverlay.SetActive (true);
            while (SendInquiryApi._success == false)
                yield return (SendInquiryApi._success == true);
           _loadingOverlay.SetActive (false);

           //input data を 初期化
            _contactInput.text = "";
            PopupPanel.Instance.PopMessageInsert (
                SendInquiryApi._httpCatchData.result.complete[0],
                LocalMsgConst.OK,
                ContactSetFinishClose
            );
            PanelPopupAnimate (GameObject.FindGameObjectWithTag(CommonConstants.POPUP_BASIC_TAG));
        }
        
        /// <summary>
        /// MailSets the finish close.
        /// </summary>
        void ContactSetFinishClose () 
        {
            PanelPopupCloseAnimate (GameObject.FindGameObjectWithTag(CommonConstants.POPUP_BASIC_TAG));
        }
        //---------------- ここまで通報用のポップアップ ---------------- 
        #endregion
        
        #region private Method
        /// <summary>
        /// Res the panel animate.
        /// </summary>
        /// <param name="target">Target.</param>
        private void PanelPopupAnimate ( GameObject target )
        {
            //ポップ用の背景セット
            _popupOverlay.SetActive (true);

            target.GetComponent<uTweenScale> ().from = Vector3.zero;
            target.GetComponent<uTweenScale> ().to   = new Vector3 (1, 1 ,1 );
            target.GetComponent<uTweenScale> ().delay    = 0.001f;
            target.GetComponent<uTweenScale> ().duration = 0.25f;
            target.GetComponent<uTweenScale> ().ResetToBeginning ();
            target.GetComponent<uTweenScale> ().enabled = true;
        }

        /// <summary>
        /// Res the panel animate.
        /// </summary>
        /// <param name="target">Target.</param>
        private void PanelPopupCloseAnimate ( GameObject target )
        {
            //ポップ用の背景外す
            _popupOverlay.SetActive (false);

            target.GetComponent<uTweenScale> ().delay    = 0.001f;
            target.GetComponent<uTweenScale> ().duration = 0.15f;
            target.GetComponent<uTweenScale> ().from = new Vector3 (1f, 1f ,1f );
            target.GetComponent<uTweenScale> ().to = Vector3.zero;
            target.GetComponent<uTweenScale> ().ResetToBeginning ();
            target.GetComponent<uTweenScale> ().enabled = true;
        }
        #endregion
    }
}