using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using ViewController;
using uTools;
using Http;

namespace EventManager
{
    public class PanelFooterButtonManager : SingletonMonoBehaviour<PanelFooterButtonManager>
    {
        #region SerializeField Method
        [SerializeField]
        private Transform _footerParent;

        [SerializeField]
        private GameObject _popupOverlay;
        #endregion

        #region Life Cycle
        IEnumerator Start ()
        {
            //リセット
            for (int i = 0; i < _footerParent.childCount; i++) {
                _footerParent.GetChild (i).GetChild (1).gameObject.SetActive (false);
            }

            //メッセージフッターのみバッジを仕込み
            if (string.IsNullOrEmpty (AppStartLoadBalanceManager._msgBadge) == false) 
            {
                int badgeCount = int.Parse (AppStartLoadBalanceManager._msgBadge);

                if (badgeCount > 0) {
                    _footerParent.GetChild (1).GetChild(2).gameObject.SetActive(true);
                } else {
                    _footerParent.GetChild (1).GetChild(2).gameObject.SetActive(false);
                }
            }

            //アクティブのものだけ。
            switch (SceneManager.GetActiveScene().name) {
                case CommonConstants.MATCHING_SCENE:
                    _footerParent.GetChild (0).GetChild (0).gameObject.SetActive (false);
                    _footerParent.GetChild (0).GetChild (1).gameObject.SetActive (true);
                break;

                case CommonConstants.MESSAGE_SCENE:
                    _footerParent.GetChild (1).GetChild (0).gameObject.SetActive (false);
                    _footerParent.GetChild (1).GetChild (1).gameObject.SetActive (true);
                break;

                case CommonConstants.SEARCH_SCENE:
                    _footerParent.GetChild (2).GetChild (0).gameObject.SetActive (false);
                    _footerParent.GetChild (2).GetChild (1).gameObject.SetActive (true);
                break;

                case CommonConstants.BULLETIN_BOARD_SCENE:
                    _footerParent.GetChild (3).GetChild (0).gameObject.SetActive (false);
                    _footerParent.GetChild (3).GetChild (1).gameObject.SetActive (true);
                break;

                case CommonConstants.PURCHASE_SCENE:
                    _footerParent.GetChild (4).GetChild (0).gameObject.SetActive (false);
                    _footerParent.GetChild (4).GetChild (1).gameObject.SetActive (true);
                break;
            }
            yield break;
        }
        #endregion

        #region Button method
        /// <summary>
        /// Matching this instance.
        /// </summary>
        public void Matching ()
        {
            if (SceneManager.GetActiveScene ().name == CommonConstants.START_SCENE || SceneManager.GetActiveScene().name == CommonConstants.PROBLEM_SCENE) {
                return;
            }

            if (AppStartLoadBalanceManager._isBaseProfile == false) {
                NoRegistBaseProfile ();
                return;
            }
            if (SceneManager.GetActiveScene().name != CommonConstants.START_SCENE) 
                SceneHandleManager.NextSceneRedirect (CommonConstants.MATCHING_SCENE);
        }

        /// <summary>
        /// Message this instance.
        /// </summary>
        public void Message () 
        {
            if (SceneManager.GetActiveScene().name != CommonConstants.START_SCENE && SceneManager.GetActiveScene().name != CommonConstants.PROBLEM_SCENE) 
                SceneHandleManager.NextSceneRedirect (CommonConstants.MESSAGE_SCENE);
        }

        /// <summary>
        /// Bulletins the board.
        /// </summary>
        public void BulletinBoard () 
        {
            if (SceneManager.GetActiveScene ().name == CommonConstants.START_SCENE || SceneManager.GetActiveScene().name == CommonConstants.PROBLEM_SCENE) {
                return;
            }

            if (AppStartLoadBalanceManager._isBaseProfile == false) {
                NoRegistBaseProfile ();
                return;
            }
            if (SceneManager.GetActiveScene().name != CommonConstants.START_SCENE) 
                SceneHandleManager.NextSceneRedirect (CommonConstants.BULLETIN_BOARD_SCENE);
        }

        /// <summary>
        /// Search this instance.
        /// </summary>
        public void Search () 
        {
            if (SceneManager.GetActiveScene().name != CommonConstants.START_SCENE && SceneManager.GetActiveScene().name != CommonConstants.PROBLEM_SCENE) 
                SceneHandleManager.NextSceneRedirect (CommonConstants.SEARCH_SCENE);
        }

        /// <summary>
        /// Purchase this instance.
        /// </summary>
        public void Purchase()
        {
            if (SceneManager.GetActiveScene().name != CommonConstants.START_SCENE && SceneManager.GetActiveScene().name != CommonConstants.PROBLEM_SCENE) 
                SceneHandleManager.NextSceneRedirect (CommonConstants.PURCHASE_SCENE);
        }
        #endregion

        #region Other Function 
        //TODO: このリージョン内のクラスは別に写した方が良いかも

        /// <summary>
        /// Nos the base profile.
        /// </summary>
        public void NoRegistBaseProfile (bool isReport = false) {
            
            PopupSecondSelectPanel.Instance.PopClean ();
            //-----------------------------
            PopupSecondSelectPanel.Instance.PopMessageInsert (
                LocalMsgConst.BASIC_PROFILE_VALID_CONFIRM,
                LocalMsgConst.MAKE_BASIC_PROF,
                LocalMsgConst.CANCEL,
                LocateBasicProf,
                MakeBasicProfCancel,
                isReport
            );

            GameObject obj = GameObject.FindGameObjectWithTag (CommonConstants.POPUP_SECOND_SELECT_TAG);
            PanelPopupAnimate (obj);
        }

        /// <summary>
        /// Locates the basic prof.
        /// </summary>
        void LocateBasicProf () {
            HeaderPanel.Instance.BackButtonSwitch (false);
            HeaderPanel.Instance.BackButtonSwitch (true, BasicProfileClose);

            PopupSecondSelectPanel.Instance.PopClean ();
            GameObject obj = GameObject.FindGameObjectWithTag (CommonConstants.POPUP_SECOND_SELECT_TAG);
            PanelPopupCloseAnimate (obj);

            PanelStartInputUserData.Instance.Init ();
            PanelAnimate (PanelStartInputUserData.Instance.gameObject);
        }

        /// <summary>
        /// Makes the basic prof cancel.
        /// </summary>
        void MakeBasicProfCancel ()
        {
            PopupSecondSelectPanel.Instance.PopClean ();
            GameObject obj = GameObject.FindGameObjectWithTag (CommonConstants.POPUP_SECOND_SELECT_TAG);
            PanelPopupCloseAnimate (obj);
        }

        /// <summary>
        /// Basics the button.
        /// </summary>
        public void BasicProfileClose()
        {
            HeaderPanel.Instance.BackButtonSwitch (false);
            BackButton (PanelStartInputUserData.Instance.gameObject);
        }


        /// <summary>
        /// Backs the button.
        /// </summary>
        /// <param name="fromObj">From object.</param>
        /// <param name="toObj">To object.</param>
        public void BackButton (GameObject fromObj) 
        {
            fromObj.GetComponent<uTweenPosition> ().delay    = 0.001f;
            fromObj.GetComponent<uTweenPosition> ().duration = 0.25f;
            fromObj.GetComponent<uTweenPosition> ().to      = fromObj.transform.GetComponent<uTweenPosition> ().from;
            fromObj.GetComponent<uTweenPosition> ().from    = Vector3.zero;
            fromObj.GetComponent<uTweenPosition> ().ResetToBeginning ();
            fromObj.GetComponent<uTweenPosition> ().enabled = true;
        }

        /// <summary>
        /// Res the panel animate.
        /// </summary>
        /// <param name="target">Target.</param>
        public void PanelAnimate ( GameObject target )
        {
            if (target.GetComponent<uTweenPosition> ().from.x == 0) {
                target.GetComponent<uTweenPosition> ().from = target.GetComponent<uTweenPosition> ().to;
            }

            target.GetComponent<uTweenPosition> ().to = Vector3.zero;
            target.GetComponent<uTweenPosition> ().delay = 0.1f;
            target.GetComponent<uTweenPosition> ().duration = 0.2f;
            target.GetComponent<uTweenPosition> ().ResetToBeginning ();
            target.GetComponent<uTweenPosition> ().enabled = true;
        }

        /// <summary>
        /// Panels the popup animate.
        /// </summary>
        /// <param name="target">Target.</param>
        private void PanelPopupAnimate ( GameObject target )
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
        private void PanelPopupCloseAnimate ( GameObject target )
        {
            _popupOverlay.SetActive (false);
            target.GetComponent<uTweenScale> ().delay    = 0.01f;
            target.GetComponent<uTweenScale> ().duration = 0.25f;
            target.GetComponent<uTweenScale> ().from = new Vector3 (1f, 1f ,1f );
            target.GetComponent<uTweenScale> ().to = Vector3.zero;
            target.GetComponent<uTweenScale> ().ResetToBeginning ();
            target.GetComponent<uTweenScale> ().enabled = true;
        }

        #endregion
    }
}
