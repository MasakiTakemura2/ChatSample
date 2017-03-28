using UnityEngine;
using System.Collections;
using Http;
using UnityEngine.UI;

namespace ViewController
{
    /// <summary>
    /// Status red panel.
    /// </summary>
    public class StatusRedPanel : SingletonMonoBehaviour<StatusRedPanel>
    {
        [SerializeField]
        private Text _userName;
    
        [SerializeField]
        private Text _description;

        [SerializeField]
        private GameObject _resignButton;

        [SerializeField]
        private GameObject _loadingOverlay;

    	/// <summary>
        /// Start this instance.
        /// </summary>
    	IEnumerator Start ()
        {
            string userStatusBan    = ((int)UserStatusType.STATUS_BAN).ToString();
            string userStatusResign = ((int)UserStatusType.STATUS_RESIGN).ToString();
            _userName.text = GetUserApi._httpCatchData.result.user.name + "様";

            if (GetUserApi._httpCatchData.result.user.status == userStatusBan)
            {
                string tmp = string.Format (LocalMsgConst.USER_NO_ASSCESS_TEXT, "<color=#ff0000ff>" + DomainData._infoAddress + "</color>" ,GetUserApi._httpCatchData.result.user.id);
                _description.text = tmp;
                _resignButton.SetActive (false);
            }
            else if (GetUserApi._httpCatchData.result.user.status == userStatusResign)
            {
                _description.text = LocalMsgConst.RESIGN_TEXT;
                _resignButton.SetActive (true);
            }
    
            yield break;
    	}
        
        #region 退会解除処理。
        /// <summary>
        /// Resigns the release button.
        /// </summary>
        /// <returns>The release button.</returns>
        public void ResignReleaseBtn ()
        {
            PopupSecondSelectPanel.Instance.PopClean ();
            PopupSecondSelectPanel.Instance.PopMessageInsert (
                LocalMsgConst.RESIGN_RELASE_TEXT,
                LocalMsgConst.YES,
                LocalMsgConst.NO,
                ResignRelaseProcess,
                ResignRelasePopupCancel
            );
            PopupSecondSelectPanel.Instance.PanelPopupAnimate (PopupSecondSelectPanel.Instance.gameObject);
            
        }

        /// <summary>
        /// Resigns the relase process.
        /// </summary>
        /// <returns>The relase process.</returns>
        void ResignRelaseProcess ()
        {
            StartCoroutine (ResignApiEnumurator());
        }
        
        /// <summary>
        /// Resigns the relase popup cancel.
        /// </summary>
        /// <returns>The relase popup cancel.</returns>
        void ResignRelasePopupCancel ()
        {
            PopupSecondSelectPanel.Instance.PopClean ();
            PopupSecondSelectPanel.Instance.PanelPopupCloseAnimate (PopupSecondSelectPanel.Instance.gameObject);
        }

        /// <summary>
        /// Resigns the API enumurator.
        /// </summary>
        /// <returns>The API enumurator.</returns>
        private IEnumerator ResignApiEnumurator () 
        {
            _loadingOverlay.SetActive (true);

            //ここが退会解除処理。
            new ReviveUserApi ();
            while (ReviveUserApi._success == false)
                yield return (ReviveUserApi._success == true);
            _loadingOverlay.SetActive (false);
            ResignRelasePopupCancel ();

            SceneHandleManager.NextSceneRedirect (CommonConstants.MYPAGE_SCENE);
            yield break;
        }
        #endregion
    }
}