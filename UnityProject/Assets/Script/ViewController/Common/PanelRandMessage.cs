using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using Http;
using ModelManager;
using EventManager;

namespace ViewController
{
    public class PanelRandMessage : SingletonMonoBehaviour<PanelRandMessage> 
    {
        [SerializeField]
        private Dropdown _genderDropdown;

        [SerializeField]
        private InputField _message;

        [SerializeField]
        private GameObject _popUpPanel;
        
        [SerializeField]
        private GameObject _loadingOverlay;

        public string _gender = "";
        List<string> _genderList = new List<string>();
         
        /// <summary>
        /// Init this instance.
        /// </summary>
        public void Init () 
        {
            _genderList = new List<string>();

            _genderDropdown.ClearOptions ();
            _genderDropdown.options.Clear ();

            if (GetUserApi._httpCatchData != null && GetUserApi._httpCatchData.result.user.sex_cd != "")
            {
                if (GetUserApi._httpCatchData.result.user.sex_cd == ((int)GenderType.FeMale).ToString ()) 
                {
                    _genderList.Add ("男性");
                    _genderList.Add ("女性");
                    _genderDropdown.value = 0;
                    _gender = "2";
                
                }
                else if (GetUserApi._httpCatchData.result.user.sex_cd == ((int)GenderType.Male).ToString ())
                {
                    _genderList.Add ("女性");
                    _genderList.Add ("男性");
                    _genderDropdown.value = 1;
                    _gender = "1";
                }
            }
            
            
//            foreach ( var sexCd in InitDataApi._httpCatchData.result.sex_cd) {
//                _genderList.Add (sexCd.name);
//            }

            //フィールド
            _genderDropdown.AddOptions (_genderList);

            if (GetUserApi._httpCatchData != null && GetUserApi._httpCatchData.result.user.profile != "") {
                _message.text = GetUserApi._httpCatchData.result.user.profile;
            } else {
                _message.text = "はじめまして、よろしくね♪";
            }

        }
        
        /// <summary>
        /// Genders the changed.
        /// </summary>
        /// <param name="result">Result.</param>
        public void GenderChanged(int result)
        {
            string name = _genderList[_genderDropdown.value];
            Debug.Log ( name  + " どっちに送られてます？ ");
            var d = CommonModelHandle.GetByNameBaseData (name, CurrentProfSettingStateType.Gender);
            _gender = d.id;
        }

        /// <summary>
        /// Sends the post.
        /// </summary>
        /// <returns>The post.</returns>
        public void SendRandPost ()
        {
           if (string.IsNullOrEmpty(_message.text) == false) 
           {
               StartCoroutine (SendRandPostWait());
           } else {
                StartEventManager.Instance.NextButton (_popUpPanel);
           }
        }

        /// <summary>
        /// Sends the rand post.
        /// </summary>
        /// <returns>The rand post.</returns>
        private IEnumerator SendRandPostWait () 
        {
            _loadingOverlay.SetActive (true);
            new SendRandomMessageApi (_message.text, _gender);
            while (SendRandomMessageApi._success == false)
                yield return (SendRandomMessageApi._success == true);
            _loadingOverlay.SetActive (false);
        
            PopupPanel.Instance.PopMessageInsert (
                SendRandomMessageApi._httpCatchData.result.complete[0],
                LocalMsgConst.OK,
                SendRandPostFinish
            );
            
            if (StartEventManager.Instance != null) 
                 StartEventManager.Instance.PanelPopupAnimate (GameObject.FindGameObjectWithTag(CommonConstants.POPUP_BASIC_TAG));

            if (MypageEventManager.Instance != null)
                MypageEventManager.Instance.PanelPopupAnimate (GameObject.FindGameObjectWithTag (CommonConstants.POPUP_BASIC_TAG));
        }
        
        /// <summary>
        /// Sends the rand post finish.
        /// </summary>
        /// <returns>The rand post finish.</returns>
        void SendRandPostFinish () {
            PopupPanel.Instance.PopClean (SendRandPostFinish);

            if (MypageEventManager.Instance != null) {
                MypageEventManager.Instance.PanelPopupCloseAnimate (GameObject.FindGameObjectWithTag(CommonConstants.POPUP_BASIC_TAG));
                MypageEventManager.Instance.PanelRandMessageClose (this.gameObject);
            }

            if (StartEventManager.Instance != null) {
                StartEventManager.Instance.PanelPopupCloseAnimate (GameObject.FindGameObjectWithTag(CommonConstants.POPUP_BASIC_TAG));
                StartEventManager.Instance.NextButton (_popUpPanel);
            }
        }
        
        
        #region BackSwipe Finger Event
        /// <summary>
        /// Backs the swipe.
        /// </summary>
        void OnSwipe (SwipeGesture gesture)
        {
            if (gesture.Selection) {
                if (gesture.Direction == FingerGestures.SwipeDirection.Left) {
                    //Debug.Log ("Left Left Left Left Left Left ");
                } else if (gesture.Direction == FingerGestures.SwipeDirection.Right) {
                    if (MypageEventManager.Instance != null)
                        MypageEventManager.Instance.PanelRandMessageClose (this.gameObject);
                }
            }
        }
        #endregion

        
    }
}