using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using EventManager;
using uTools;
using UnityEngine.UI;
using Http;

namespace ViewController 
{        
    /// <summary>
    /// Panel talk list.
    /// </summary>
    public class PanelTalkList : SingletonMonoBehaviour<PanelTalkList>
    {
        [SerializeField]
        private Transform _itemParent;

        [SerializeField]
        private Transform _editButton;

        [SerializeField]
        private GameObject _deleteAllButton;

        [SerializeField]
        private GameObject _loadingOverlay;

        public DisplayState _displayState = DisplayState.ListShow;

        public List<string> _msgDeleteList = new List<string> ();
        
        public enum DisplayState
        {
            ListShow,
            DeleteMode
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
                    Helper.LocalFileHandler.Init ( LocalFileConstants.GetLocalFileDir() + LocalFileConstants.COMMON_LOCAL_FILE_NAME);
                    string fromScene = Helper.LocalFileHandler.GetString (LocalFileConstants.FROM_MYPAGE_SCENE);
                    if (string.IsNullOrEmpty (fromScene) == false && fromScene == CommonConstants.MYPAGE_SCENE) {
                        Helper.LocalFileHandler.SetString (LocalFileConstants.FROM_MYPAGE_SCENE, "");
                        Helper.LocalFileHandler.Flush ();
                        SceneHandleManager.NextSceneRedirect (CommonConstants.MYPAGE_SCENE);
                    }
                }
            }
        }
        
        /// <summary>
        /// Edit this instance.
        /// 削除したいリストをアニメーションでインターフェース着火
        /// </summary>
        public void  EditOpenButton() 
        {
            if (MessageEventManager.Instance._panelState == MessageEventManager.PanelState.Info) {
                _displayState = DisplayState.DeleteMode;
            }

            for (int i = 0; i < _itemParent.childCount; i++)
            {
                if (_itemParent.GetChild(i).gameObject.activeSelf == true)
                {
                    _itemParent.GetChild(i).GetComponent<uTweenPosition> ().delay = 0.001f;
                    _itemParent.GetChild(i).GetComponent<uTweenPosition> ().duration = 0.25f;

                    if (_displayState == DisplayState.ListShow) {
                        _itemParent.GetChild (i).GetComponent<Button> ().enabled = false;
                        _itemParent.GetChild(i).GetComponent<uTweenPosition> ().from = Vector3.zero;
                        _itemParent.GetChild(i).GetComponent<uTweenPosition> ().to = new Vector3 (250f, _itemParent.GetChild(i).localPosition.y, 0);
                        _itemParent.GetChild (i).GetComponent<PanelEazyNotifyItem> ()._delObj.GetComponent<Toggle> ().isOn = false;
                    } else {
                        _itemParent.GetChild (i).GetComponent<Button> ().enabled = true;
                        _itemParent.GetChild(i).GetComponent<uTweenPosition> ().from = new Vector3 (250f, _itemParent.GetChild(i).localPosition.y, 0);
                        _itemParent.GetChild(i).GetComponent<uTweenPosition> ().to = new Vector3 (0, _itemParent.GetChild(i).localPosition.y, 0);
                    }
                    //_itemParent.GetChild(i).GetComponent<uTweenPosition> ().ResetToBeginning ();
                    _itemParent.GetChild(i).GetComponent<uTweenPosition> ().enabled = true;
                }
            }

            if (_displayState == DisplayState.ListShow)
            {
                _displayState = DisplayState.DeleteMode;
                _deleteAllButton.SetActive (true);
                _editButton.GetChild (0).gameObject.SetActive (false);
                _editButton.GetChild (1).gameObject.SetActive (true);
            } else {
                _displayState = DisplayState.ListShow;
                _deleteAllButton.SetActive (false);
               
                _editButton.GetChild (0).gameObject.SetActive (true);
                _editButton.GetChild (1).gameObject.SetActive (false);
            }
        }
        
        /// <summary>
        /// Messages the delete confirm.
        /// </summary>
        /// <returns>The delete confirm.</returns>
        public void MessageDeleteConfirm() {
            if (_msgDeleteList.Count > 0) {
                MessageEventManager.Instance.PanelPopupAnimate (PopupSecondSelectPanel.Instance.gameObject);
                PopupSecondSelectPanel.Instance.PopClean ();
                PopupSecondSelectPanel.Instance.PopMessageInsert (
                   LocalMsgConst.MESSAGE_LIST_DELETE,
                   LocalMsgConst.YES,
                   LocalMsgConst.NO,
                   MessageDeleteApi,
                   MessageDeleteCancel
                );
            }
        }

        /// <summary>
        /// Messages the delete API.
        /// </summary>
        /// <returns>The delete API.</returns>
        public void MessageDeleteApi () {
            StartCoroutine (MessageDeleteApiIterator ());
        }

        /// <summary>
        /// Messages the delete API iterator.
        /// </summary>
        /// <returns>The delete API iterator.</returns>
        private IEnumerator MessageDeleteApiIterator () 
        {
            //削除するリスト
            string [] messageIds = _msgDeleteList.ToArray ();
            string splitData = string.Join(",", messageIds);
            PopupSecondSelectPanel.Instance.PopClean ();
            MessageEventManager.Instance.PanelPopupCloseAnimate (PopupSecondSelectPanel.Instance.gameObject);
            _loadingOverlay.SetActive (true);

            new DeleteMessageApi (messageIds);
            while (DeleteMessageApi._success == false)
                yield return (DeleteMessageApi._success == true);

            _loadingOverlay.SetActive (false);
            if (DeleteMessageApi._httpCatchData.result == "true")
            {
                PopupPanel.Instance.PopClean ();
                PopupPanel.Instance.PopMessageInsert (LocalMsgConst.MESSAGE_DEL_FIX,
                    LocalMsgConst.OK,
                    MessageDeleteApiFixed
                );
                MessageEventManager.Instance.PanelPopupAnimate (PopupPanel.Instance.gameObject);
            }

            yield break;
        }
        
        void MessageDeleteApiFixed () {
            PopupPanel.Instance.PopClean ();
            MessageEventManager.Instance.PanelPopupCloseAnimate (PopupPanel.Instance.gameObject);
            MessageDeleteCancel ();
            MessageEventManager.Instance.HeaderTab1 ();
        }
        
        void MessageDeleteCancel () {
            PopupSecondSelectPanel.Instance.PopClean ();
            MessageEventManager.Instance.PanelPopupCloseAnimate (PopupSecondSelectPanel.Instance.gameObject);
            _msgDeleteList.Clear ();
            _displayState = DisplayState.DeleteMode;
            EditOpenButton ();
        }
    }
}