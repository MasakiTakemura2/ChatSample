using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.Events;
using Http;
using uTools;
using Helper;

namespace ViewController
{
    public class PopupAppliReView : SingletonMonoBehaviour<PopupAppliReView>
    {
        [SerializeField]
        private Toggle _isNotShowToggle;

        [SerializeField]
        private Text _popupText;

        private string _storeUrl;
        
        
        /// <summary>
        /// Init this instance.
        /// </summary>
        public void Init ( string url, string text ) 
        {
            _storeUrl = url;
            _popupText.text = text;

            string commonFileName = LocalFileConstants.GetLocalFileDir () + LocalFileConstants.COMMON_LOCAL_FILE_NAME;
            //ローカルファイルに保存用初期化。
            LocalFileHandler.Init (commonFileName);
        }
        
        /// <summary>
        /// Gos the appli review.
        /// </summary>
        /// <returns>The appli review.</returns>
        public void GoAppliReview () 
        {

#if (UNITY_IOS || UNITY_ANDORID) && !UNITY_EDITOR
    Debug.Log ("ここでチェック" +  _storeUrl);
    Application.OpenURL (_storeUrl);
#else
    Application.OpenURL (_storeUrl);
#endif
            //ここでアプリレビューキーを保存
            //LocalFileHandler.SetString (LocalFileConstants.APPLI_REVIEW_POPUP_SHOW, "1");
            //LocalFileHandler.Flush ();
            PopupClose ();
        }
        
        /// <summary>
        /// Popups the close.
        /// レビューをしない時の処理。
        /// </summary>
        /// <returns>The close.</returns>
        public void PopupClose()
        {
            //if (_isNotShowToggle.isOn == true)
            //{
                //ここでアプリレビューキーを保存
                LocalFileHandler.SetString (LocalFileConstants.APPLI_REVIEW_POPUP_SHOW, "1");
                LocalFileHandler.Flush ();
            //}
            StartCoroutine (PopupCloseCoroutine());
        }
        
        /// <summary>
        /// Popups the close coroutine.
        /// </summary>
        /// <returns>The close coroutine.</returns>
        private IEnumerator PopupCloseCoroutine() {
           
            PanelPopupCloseAnimate (this.gameObject);

            float lx = this.transform.localScale.x;

            AppliEventController.Instance.PopupOverlaySwitch (false);

            while (lx != 0)
                yield return (lx == 0);

            Destroy (this.gameObject);
            yield break;
        }
        
        /// <summary>
        /// Nos the next display.
        /// </summary>
        /// <returns>The next display.</returns>
        public void NoNextDisplay () {
            if (_isNotShowToggle.isOn == true) {
                _isNotShowToggle.isOn = false;
            } else if (_isNotShowToggle.isOn == false) {
                _isNotShowToggle.isOn = true;
            }
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