using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Http;
using ViewController;
using uTools;

namespace Helper
{
    /// <summary>
    /// Appli event controller.
    /// </summary>
    public class AppliEventController : SingletonMonoBehaviour<AppliEventController>
    {
         [SerializeField]
         private GameObject _popupOverlay;

        [SerializeField]
        private Transform _mainCanvas;

        /// <summary>
        /// Maintenances the check.
        /// メンテナンスの時、強制ポップアップ。
        /// </summary>
        public bool MaintenanceCheck ()
        {
            bool isMaintenance = false;

            if ( (GetUserApi._httpCatchData != null  && GetUserApi._httpCatchData.result.maintenance == "true")) 
            {
                    PopupPanel.Instance.PopMessageInsert (
                    LocalMsgConst.MAINTENANCE,
                    LocalMsgConst.OK,
                    MaintenanceEvent
                );

                GameObject obj = GameObject.FindGameObjectWithTag (CommonConstants.POPUP_BASIC_TAG);
                PanelPopupAnimate (obj);
                isMaintenance = true;
            } 

            return isMaintenance;
        }

        /// <summary>
        /// Forces the update check.
        /// 強制アップデートの処理。配信元スキームに飛ばす。
        /// </summary>
        public bool ForceUpdateCheck() 
        {
            bool isForceUpdate = false;
            //TODO: テスト。
            if (GetUserApi._httpCatchData != null && GetUserApi._httpCatchData.result.force_update == "true")
            {
                    PopupPanel.Instance.PopMessageInsert (
                    LocalMsgConst.FORCE_UPDATE,
                    LocalMsgConst.OK,
                    ForceUpdate
                );

                GameObject obj = GameObject.FindGameObjectWithTag (CommonConstants.POPUP_BASIC_TAG);
                PanelPopupAnimate (obj);
                isForceUpdate = true;
            }

            return isForceUpdate;
        }


        /// <summary>
        /// Maintenances the event.
        /// ポップアップのOKボタンのイベント用
        /// </summary>
        void MaintenanceEvent () {
            StartCoroutine (Quit());
        }

        /// <summary>
        /// Quit this instance.
        /// </summary>
        private IEnumerator Quit() {
            #if UNITY_EDITOR
              UnityEditor.EditorApplication.isPlaying = false;
            #else
                Application.Quit ();
            #endif
            yield break; 
        }

        /// <summary>
        /// Forces the update.
        /// ポップアップのOKボタンのイベント用
        /// </summary>
        void ForceUpdate () {
            Application.OpenURL ("http://google.co.jp");
            return;
        }
        
        /// <summary>
        /// Applis the review.
        /// </summary>
        /// <returns>The review.</returns>
        public void AppliReview(){
            StartCoroutine (AppliReviewCoroutine ());
        }
        
        /// <summary>
        /// Applis the review coroutine.
        /// </summary>
        /// <returns>The review coroutine.</returns>
        private IEnumerator AppliReviewCoroutine ()
        {
            //まず、「アプリレビュー」条件に該当するか？
            while (GetUserApi._httpCatchData == null)
                yield return (GetUserApi._httpCatchData != null);
                
            //ポップアップに表示するテキストと、アプリレビューのスキームリンクが返り値で返ってきているか？
            if (string.IsNullOrEmpty (GetUserApi._httpCatchData.result.appli_review.url) == true || string.IsNullOrEmpty (GetUserApi._httpCatchData.result.appli_review.text) == true) {
                yield break;
            }

            //ローカルファイル - Init
            string commonFileName = LocalFileConstants.GetLocalFileDir () + LocalFileConstants.COMMON_LOCAL_FILE_NAME;
            
            LocalFileHandler.Init (commonFileName);

            //ファイルが作成されるまでポーリングして処理待ち
            while (System.IO.File.Exists (commonFileName) == false)
                yield return (System.IO.File.Exists (commonFileName) == true);

            //ここでアプリレビュー用の処理を実行して良いか確認。
            if (string.IsNullOrEmpty (LocalFileHandler.GetString (LocalFileConstants.APPLI_REVIEW_POPUP_SHOW)) == true)
            {
                if (_mainCanvas != null) {
                    GameObject go = Instantiate (Resources.Load (CommonConstants.APPLI_REVIEW_POPUP_PANEL)) as GameObject;
    
                    while (go == null)
                        yield return (go != null);

                    go.transform.SetParent (_mainCanvas, false);

                    //表示を最前面に
                    go.transform.SetAsLastSibling();
                    go.name = CommonConstants.APPLI_REVIEW_POPUP_PANEL;
                    go.GetComponent<PopupAppliReView> ().Init (GetUserApi._httpCatchData.result.appli_review.url, GetUserApi._httpCatchData.result.appli_review.text);
                    PanelPopupAnimate (go);                            
                }
            } else {
                //次回表示をオンにしているため、何の処理もしない
                
                yield break;
            }

            yield break;
        }
        
        
        /// <summary>
        /// Users the status problem.
        /// ユーザーのステータスが「垢バンか退会処理」の場合シーンを強制移動。
        /// </summary>
        /// <returns>The status problem.</returns>
        public void UserStatusProblem() {
            var user = GetUserApi._httpCatchData.result.user;
            Debug.Log (user.status + " ==================== " + ((int)UserStatusType.STATUS_RESIGN).ToString()) ;

            if (user.status == ((int)UserStatusType.STATUS_BAN).ToString() || user.status == ((int)UserStatusType.STATUS_RESIGN).ToString()) {
                SceneHandleManager.NextSceneRedirect (CommonConstants.PROBLEM_SCENE);
            }
        }
        

        /// <summary>
        /// Res the panel animate.
        /// </summary>
        /// <param name="target">Target.</param>
        private void PanelPopupAnimate ( GameObject target )
        {
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
            _popupOverlay.SetActive (false);
            target.GetComponent<uTweenScale> ().delay    = 0.001f;
            target.GetComponent<uTweenScale> ().duration = 0.15f;
            target.GetComponent<uTweenScale> ().from = new Vector3 (1f, 1f ,1f );
            target.GetComponent<uTweenScale> ().to = Vector3.zero;
            target.GetComponent<uTweenScale> ().ResetToBeginning ();
            target.GetComponent<uTweenScale> ().enabled = true;
        }
        
        /// <summary>
        /// Popups the overlay switch.
        /// </summary>
        /// <returns>The overlay switch.</returns>
        public void PopupOverlaySwitch (bool isOn)
        {
            _popupOverlay.SetActive (isOn);
        }
    }
}
