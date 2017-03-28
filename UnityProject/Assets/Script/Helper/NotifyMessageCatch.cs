using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using uTools;
using ViewController;

namespace Helper
{
    public class NotifyMessageCatch : SingletonMonoBehaviour<NotifyMessageCatch>
    {
        [SerializeField]
        private Transform _mainCanvas;

        private string _viewName;
        private string _id;

        /// <summary>
        /// Message the specified getMsg.
        /// </summary>
        /// <param name="getMsg">Get message.</param>
        public void Message( string getMsg )
        {
            Debug.Log ("どこのタイミング取得出来るか？ check !! start");
            
            //メッセージ系列の場合。
            //[chender:メッセージだお、ハロー(*^^*)-message_detail-3316 ] 展開？？？ 
            
            //お知らせの場合。
            //[出会い応援運営事務局です！-information_detail-info_9 ] 展開？？？ 
            
            //上記のようなデータ取得で処理を書く。ノーティフィケーションアニメーションポップアップダイアログシステム。
            Debug.Log ("[" + getMsg + "] 展開？？？ ");
            Debug.Log ("どこのタイミング取得出来るか？ check !! end");

            string title     = "";
            string body      = "";
            string viewName = "";
            string id        = "";

            //フォアグラウンドで受け取るメッセージを分解する処理。
            if (string.IsNullOrEmpty (getMsg) == false) 
            {
                if (0 < getMsg.IndexOf (":"))
                {
                    if (0 < getMsg.IndexOf ("-"))
                    {
                        string[] stArrayData = getMsg.Split('-');
                        string[] stTitleBody = stArrayData[0].Split(':');
    
                        title = stTitleBody[0];
                        body  = stTitleBody[1];
                        if (1 == getMsg.IndexOf ("-")) {
                            viewName = stArrayData[1];
                        } else {
                            viewName = stArrayData[1];
                            id       = stArrayData[2];                  
                        }
                    //user id と view nameが存在しない場合。
                    } else {
                        string[] stTitleBody = getMsg.Split(':');
                        title = stTitleBody[0];
                        body  = stTitleBody[1];
                    }
                }
                else
                {
                    //お知らせ等はタイトル無し。の場合の処理。
                    
                    if (0 < getMsg.IndexOf ("-"))
                    {
                        string [] stArrayData = getMsg.Split ('-');
                        title = "";
                        body  = stArrayData[0];

                        if (1 == getMsg.IndexOf ("-")) {
                            viewName = stArrayData[1];
                        } else {
                            viewName = stArrayData[1];
                            id       = stArrayData[2];                  
                        }
                    //user id と view nameが存在しない場合
                    } else {
                        body  = getMsg;
                    }
                }
             
                _viewName = viewName;
                _id       = id;
            }

            GameObject obj = GameObject.FindGameObjectWithTag (CommonConstants.POPUP_THIRD_SELECT_TAG);
            PopupThirdSelectPanel.Instance.PanelPopupCloseAnimate (obj);
            PopupThirdSelectPanel.Instance.PopClean ();

            //説明用ポップアップ。
            PopupThirdSelectPanel.Instance.PushPopMessageInsert (
                title,
                body,
                LocalMsgConst.PUSH_POP_OK,
                LocalMsgConst.PUSH_POP_NG,
                PopupConfirm,
                PopupCancel
            );

            PopupThirdSelectPanel.Instance.PanelPopupAnimate (obj);
        }
        
        /// <summary>
        /// Popups the confirm.
        /// </summary>
        void PopupConfirm () {
            NotificationRecieveManager.NextSceneProccess (_viewName + " " + _id);   
        }
        
        /// <summary>
        /// Popups the cancel.
        /// </summary>
        void PopupCancel () {
            PopupThirdSelectPanel.Instance.PopClean ();
            GameObject obj = GameObject.FindGameObjectWithTag (CommonConstants.POPUP_THIRD_SELECT_TAG);
            PopupThirdSelectPanel.Instance.PanelPopupCloseAnimate (obj);
        }
        
        
        /// <summary>
        /// Res the panel animate.
        /// </summary>
        /// <param name="target">Target.</param>
        private void PanelPopupAnimate ( GameObject target )
        {
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
            target.GetComponent<uTweenScale> ().delay    = 0.001f;
            target.GetComponent<uTweenScale> ().duration = 0.15f;
            target.GetComponent<uTweenScale> ().from = new Vector3 (1f, 1f ,1f );
            target.GetComponent<uTweenScale> ().to = Vector3.zero;
            target.GetComponent<uTweenScale> ().ResetToBeginning ();
            target.GetComponent<uTweenScale> ().enabled = true;
        }
        
    }
}