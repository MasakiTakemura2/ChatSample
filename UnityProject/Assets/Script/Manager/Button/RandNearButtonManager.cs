using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using uTools;
using System;

namespace EventManager {
    public class RandNearButtonManager : SingletonMonoBehaviour<RandNearButtonManager>
    {
        #region Seriarize Variable
        [SerializeField]
        private GraphicRaycaster _raycaster;

        [SerializeField]
        private GameObject _maskBackground;

        [SerializeField]
        private GameObject _panelRippleAnimate;

        [SerializeField]
        private GameObject _panelTutorial;

        [SerializeField]
        private GameObject _panelRandMessage;

        [SerializeField]
        private Transform _genderSelected;
        #endregion

        #region Member Valiable
        private enum Gender {
            Male,
            Female,
            Both
        }
        #endregion

        #region Button Click Scripting
        /// <summary>
        /// Randoms the message panel.
        /// </summary>
        public void RandomMessagePanel () {

            _panelRippleAnimate.SetActive (false);

            //TODO: サーバーからチュートリアル判定なり
            //if (tutorial_flag == 1) {
                _raycaster.blockingObjects = GraphicRaycaster.BlockingObjects.All;
                _panelTutorial.SetActive (true);
                _maskBackground.SetActive (true);
            //}

            _panelRandMessage.SetActive (true);
        }

        /// <summary>
        /// Offs the tutorial.
        /// </summary>
        public void OffTutorialPanel () {
            _raycaster.blockingObjects = GraphicRaycaster.BlockingObjects.None;
            _panelTutorial.SetActive (false);
            _maskBackground.SetActive (false);
        }

        /// <summary>
        /// Genders the select.
        /// </summary>
        /// <param name="obj">Object.</param>
        public void GenderSelect (GameObject obj) {
            for (int i=0; i < _genderSelected.childCount; i++) {
                if (_genderSelected.GetChild (i).name == obj.name) {
                    _genderSelected.GetChild (i).gameObject.SetActive (true);
                } else {
                    _genderSelected.GetChild (i).gameObject.SetActive (false);
                }
            }
        }

        /// <summary>
        /// Messages the send.
        /// </summary>
        public void MessageSend () {
            Debug.Log ("メッセージ送信ボタンを押した場合の処理。");
        }

        #endregion

        #region private method

        #endregion
    }
}