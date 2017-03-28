using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using Http;
using EventManager;
using uTools;

namespace ViewController
{
    /// <summary>
    /// Popup matching.
    /// </summary>
    public class PopupMatching : SingletonMonoBehaviour<PopupMatching>
    {
        [SerializeField]
        private RawImage _userProf;

        [SerializeField]
        private RawImage _toUserProf;
       
        [SerializeField]
        private Text _userName;

        [SerializeField]
        private Text _toUserName;

        [SerializeField]
        private SwipeRecognizer _backSwipe;

        [SerializeField]
        private ScreenRaycaster _tinderFingerGestureRay;

        public string _toUserId;

        /// <summary>
        /// Init this Instance.
        /// </summary>
        public void Init (UserDataEntity.Basic user, UserDataEntity.Basic toUser)
        {
            _toUserId = toUser.id;
            MatchingEventManager.Instance.PanelPopupAnimateMatch (this.gameObject);

            _userName.text   = LocalMsgConst.ME_TEXT;
            _toUserName.text = toUser.name;

            if (string.IsNullOrEmpty (user.profile_image_url) == false) {
                StartCoroutine (WwwToRendering (user.profile_image_url, _userProf));
            }

            if (string.IsNullOrEmpty (toUser.profile_image_url)  == false) {
                StartCoroutine (WwwToRendering (toUser.profile_image_url, _toUserProf)); 
            }
        }

        /// <summary>
        /// Tos the message room.
        /// </summary>
        /// <returns>The message room.</returns>
        /// <param name="toUserId">To user identifier.</param>
        public void ToMessageRoom ( GameObject animObj) 
        {
            if (string.IsNullOrEmpty (_toUserId) == false) 
            {
                _tinderFingerGestureRay.enabled = false;

                ClosePopup (); //ポップアップを閉じる。
                HeaderPanel.Instance.BackButtonSwitch (false);
                HeaderPanel.Instance.BackButtonSwitch (true, MatchingEventManager.Instance.ChatBackButton);
                _backSwipe.EventMessageTarget = PanelChat.Instance.gameObject;
                
                if (PanelChat.Instance != null)
                {
                    GameObject panelChat = PanelChat.Instance.gameObject;
                    if (panelChat.GetComponent<uTweenPosition> ().from.x == 0) 
                    {
                        panelChat.GetComponent<uTweenPosition> ().from = panelChat.GetComponent<uTweenPosition> ().to;
                    }

                    panelChat.GetComponent<uTweenPosition> ().to = Vector3.zero;
                    panelChat.GetComponent<uTweenPosition> ().ResetToBeginning ();
                    panelChat.GetComponent<uTweenPosition> ().enabled = true;

                    //初期化処理
                    panelChat.GetComponent<PanelChat> ()._maxAfterID = "";
                    panelChat.GetComponent<PanelChat> ()._maxBeforeID = "";
                    panelChat.GetComponent<PanelChat> ()._maxAfterIDBackup = "";
                    panelChat.GetComponent<PanelChat> ()._maxBeforeIDBackup = "";
                    panelChat.GetComponent<PanelChat> ()._listUpdateDisable = false;

                    panelChat.GetComponent<PanelChat> ().Init (_toUserId);
                }
            }
        }
        /// <summary>
        /// Lates to message.
        /// </summary>
        /// <returns>The to message.</returns>
        public void ClosePopup () {
            Helper.TinderGesture.Instance._isEventPopUp = false;
            MatchingEventManager.Instance.PopUpPanelClose (this.gameObject);
        }
        

        /// <summary>
        /// Wwws to rendering.
        /// </summary>
        /// <returns>The to rendering.</returns>
        /// <param name="url">URL.</param>
        /// <param name="targetObj">Target object.</param>
        private IEnumerator WwwToRendering (string url, RawImage targetObj)
        {
            targetObj.texture = null;
            targetObj.gameObject.SetActive (false);
            if (string.IsNullOrEmpty (url) == true) 
                yield break;

            using (WWW www = new WWW (url))
            {
                while (www == null)
                    yield return (www != null);

                while (www.isDone == false)
                    yield return (www.isDone);

                //non texture file
                if (string.IsNullOrEmpty (www.error) == false) 
                {
                    Debug.LogError (www.error);
                    Debug.Log (url);
                    yield break;
                }

                while (targetObj == null)
                    yield return (targetObj != null);
                targetObj.gameObject.SetActive (true);
                targetObj.texture = www.texture;

            }
        }
    }
}