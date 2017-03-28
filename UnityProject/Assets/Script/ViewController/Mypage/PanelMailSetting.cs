﻿using UnityEngine;
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
    public class PanelMailSetting : SingletonMonoBehaviour<PanelMailSetting>
    {
        [SerializeField]
        private GameObject _loadingOverlay;
        
        [SerializeField]
        private GameObject _popupOverlay;

        [SerializeField]
        private InputField _mailAddress;
        
        [SerializeField]
        private GameObject _mailAddressText;
        
        [SerializeField]
        private InputField _password;

        [SerializeField]
        private GameObject _passwordText;

        private string _setMail;
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
        
        
        /// <summary>
        /// Init this instance.
        ///初期画面表示用
        /// </summary>
        public void Init () {
            Debug.Log (" ここメール画面初期化通っているか？ ");
            if (string.IsNullOrEmpty (_mailAddress.text) == true) {
                _mailAddress.text = "";
            }
            
            if (string.IsNullOrEmpty(GetUserApi._httpCatchData.result.user.mail_address) == false) {
                _mailAddress.text = GetUserApi._httpCatchData.result.user.mail_address;
            }
            

            if (string.IsNullOrEmpty (_password.text) == true) {
                _password.text = "";
            }
        }

        #region メールアドレス登録用の関数群
        ///TODO: pop up 共通化にする　- ここから
        /// <summary>
        /// MailSets the confirm button.
        /// </summary>
        public void MailSet()
        {
            bool isError = false;
            if (string.IsNullOrEmpty (_mailAddress.text) == true) {
                isError = true;
                _mailAddressText.SetActive (true);
            } else {
               _mailAddressText.SetActive (false);
            }

            if (string.IsNullOrEmpty (_password.text) == true) {
                isError = true;
                _passwordText.SetActive (true);
            } else {
                _passwordText.SetActive (false);
            }
            
            if (isError == true)
                return;

            _setMail = _mailAddress.text;
            _setPass = _password.text;

            new SetMailAddressApi (_setMail, _setPass);
            StartCoroutine (SendMailSetWait());
        }
        
        /// <summary>
        /// Sends the report wait.
        /// </summary>
        /// <returns>The report wait.</returns>
        private IEnumerator SendMailSetWait ()
        {
            _loadingOverlay.SetActive (true);
            while (SetMailAddressApi._success == false)
                yield return (SetMailAddressApi._success == true);
           _loadingOverlay.SetActive (false);

            PopupPanel.Instance.PopMessageInsert (
                SetMailAddressApi._httpCatchData.result.complete[0],
                LocalMsgConst.OK,
                MailSetFinishClose
            );
            PanelPopupAnimate (GameObject.FindGameObjectWithTag(CommonConstants.POPUP_BASIC_TAG));
        }
        
        /// <summary>
        /// MailSets the finish close.
        /// </summary>
        void MailSetFinishClose () 
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