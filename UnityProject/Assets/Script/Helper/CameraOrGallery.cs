using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using uTools;
using System.Runtime.InteropServices;
using System.IO;
using ViewController;
using EventManager;


namespace Helper {
    /// <summary>
    /// Camera helper.
    /// [IOS, ANDROID] Native Bind
    /// </summary>
    public class CameraOrGallery : SingletonMonoBehaviour<CameraOrGallery>
    {

#if UNITY_IOS && !UNITY_EDITOR
    [DllImport ("__Internal")]
    private static extern void _cameraOpeniOSDevice();
    
    [DllImport ("__Internal")]
    private static extern void _galleryOpeniOSDevice();
#endif
        #region Serialize Valiable
        [SerializeField]
        private GameObject _cameraOrGalleryPanel;

        [SerializeField]
        private GameObject _tmpImage_1;

        [SerializeField]
        private GameObject _tmpImage_2;

        [SerializeField]
        private GameObject _tmpImage_3;

       
		[SerializeField]
		private GameObject _popupOverlay;


		private GameObject _pictSetTargetObj;
		private string _sendFilePath = "";

        #endregion


        #region Member variable
        public string _postLocalFilePath;
        private WWW www;
        #endregion


        #region publc method Button Open [IOS]
        /// <summary>
        /// Cameras the or gallery pop open.
        /// </summary>
        public void CameraOrGalleryPopOpen (GameObject target ) {
            if (target.GetComponent<uTweenPosition> ().from == Vector3.zero) {
                target.GetComponent<uTweenPosition> ().from = target.GetComponent<uTweenPosition> ().to;
                target.GetComponent<uTweenPosition> ().to = Vector3.zero;
                target.GetComponent<uTweenPosition> ().ResetToBeginning ();
                target.GetComponent<uTweenPosition> ().enabled = true;
            }
        }

        /// <summary>
        /// Cameras the or gallery pop open.
        /// </summary>
        public void CameraOrGalleryPopCancel (GameObject target)
        {
            if (target.GetComponent<uTweenPosition> ().to == Vector3.zero) {
                target.GetComponent<uTweenPosition> ().to = target.GetComponent<uTweenPosition> ().from;
                target.GetComponent<uTweenPosition> ().from = Vector3.zero;
                target.GetComponent<uTweenPosition> ().ResetToBeginning ();
                target.GetComponent<uTweenPosition> ().enabled = true;
            }

        }

        /// <summary>
        /// Opens the camera.
        /// </summary>
        public void OpenCamera() {
            CameraOrGalleryPopCancel (_cameraOrGalleryPanel);
            #if UNITY_IOS && !UNITY_EDITOR
            _cameraOpeniOSDevice();
            #elif UNITY_EDITOR
                string path = UnityEditor.EditorUtility.OpenFilePanel ("ファイルを選択します", "", "png,jpg");
                Debug.Log (path + "を表示");
                _postLocalFilePath = path;
                StartCoroutine (SendView (path));
            #endif
            return;
        }

        /// <summary>
        /// Opens the gallery.
        /// </summary>
        public void OpenGallery () {
            CameraOrGalleryPopCancel (_cameraOrGalleryPanel);
            #if UNITY_IOS && !UNITY_EDITOR
            _galleryOpeniOSDevice();
            #elif UNITY_EDITOR
                string path = UnityEditor.EditorUtility.OpenFilePanel ("ファイルを選択します", "", "png, jpg, jpeg");
                Debug.Log (path + "を表示");
                _postLocalFilePath = path;

                StartCoroutine (SendView (path));
            #endif
            return;
        }

        /// <summary>
        /// Opens the camera or gallery set.
        /// </summary>
        /// <param name="pictObj">Pict object.</param>
        public void OpenCameraOrGallerySet (GameObject pictObj)
        {
            _pictSetTargetObj = pictObj;
        }

        /// <summary>
        /// /[ Bind ]
        /// Catchs the ios send message.
        ///カメラか、フォトギャラリーを選択、撮影して画像に落とし込み
        /// </summary>
        public void PictCatch(string _mstSelectedImage)
        {
Debug.Log (_mstSelectedImage + " C#の受け皿の処理。ファイルパス取得できている");

            if (File.Exists (_mstSelectedImage) == true) {
                _postLocalFilePath = _mstSelectedImage;
                switch (_pictSetTargetObj.name) {
                case "1":
                    _tmpImage_1.SetActive (false);
                    break;
                case "2":
                    _tmpImage_2.SetActive (false);
                    break;
                case "3":
                    _tmpImage_3.SetActive (false);
                  break;
                }
                Debug.Log ("  "  + _mstSelectedImage);
                if (_pictSetTargetObj.activeSelf == false)
                    _pictSetTargetObj.SetActive (true);

                StartCoroutine (SendView (_mstSelectedImage));
            } else {
                Debug.LogError ("ファイルが取得出来ていません。");
            }
        }

        /// <summary>
        /// [ Bind ]
        /// Auths the setting off.
        /// </summary>
        public void AuthSettingOff(string returnStr) {
            CameraOrGalleryPopCancel (_cameraOrGalleryPanel);
            ViewController.PermmitWarningPanel.Instance.Open ();
        }
        #endregion

        #region private method
        /// <summary>
        /// Sends the view.
        /// </summary>
        /// <returns>The view.</returns>
        /// <param name="filePath">File path.</param>
        private IEnumerator SendView (string filePath)
        {
Debug.Log (filePath + " ファイルパス取れているか？ OKボタン押したときのログ11111111111111 ");

            if (string.IsNullOrEmpty (filePath) == true) yield break;

			_sendFilePath = filePath;
            _pictSetTargetObj.GetComponent<RawImage> ().texture = MakeTex (filePath);
            //yield return new WaitForSeconds (2.0f);
			SendImageConfirmOpen (PopupSecondSelectPanel.Instance.gameObject);
        }
        
		private IEnumerator SendImageOKToButton(string filePath)
		{
Debug.Log (filePath + " ファイルパス取れているか？ OKボタン押したときのログ22222222222222 ");
			if (_pictSetTargetObj.activeSelf == false)
				_pictSetTargetObj.SetActive (true);
                
			//マイページ
			if (MypageEventManager.Instance != null) {
				if (MypageEventManager.Instance._chatOrSelefImageType == ChatOrSelefImageType.Self)
				{
					MypageEventManager.Instance.UploadImageButton ();
				} else if (MypageEventManager.Instance._chatOrSelefImageType == ChatOrSelefImageType.Chat) {
					
					if(PanelChat.Instance != null)
					{
						PanelChat.Instance._sendImage = true;
					}
					PanelChat.Instance.MessageSendButton ();
				}
				//掲示板
			} else if (BulletinBoardEventManager.Instance != null) {
				if (BulletinBoardEventManager.Instance._chatOrSelefImageType == ChatOrSelefImageType.Self) {
					//BulletinBoardEventManager.Instance.WriteButtonEvent ();
				} else if (BulletinBoardEventManager.Instance._chatOrSelefImageType == ChatOrSelefImageType.Chat) {
					if(PanelChat.Instance != null)
					{
						PanelChat.Instance._sendImage = true;
					}
					PanelChat.Instance.MessageSendButton ();
				}
            //メッセージ, 検索, マッチング
            } else if ( MessageEventManager.Instance != null || SearchEventManager.Instance != null || MatchingEventManager.Instance != null) {

                if(PanelChat.Instance != null)
                {
                    PanelChat.Instance._sendImage = true;
                }
                PanelChat.Instance.MessageSendButton ();
            }

            yield break;
		}

		// 送信確認ポップアップ
		public void SendImageConfirmOpen(GameObject target)
		{
			string popText = LocalMsgConst.CONFIRM_IMAGE_SEND_MESSAGE;

			PopupSecondSelectPanel.Instance.PopMessageInsert(
				popText,
				LocalMsgConst.YES,
				LocalMsgConst.NO,
				SendImageOK,
				SendImageCancel
			);
			PanelPopupAnimate (target);
		}
		void SendImageOK()
		{
            PopupSecondSelectPanel.Instance.PopClean();
			StartCoroutine (SendImageOKToButton(_sendFilePath));
			PanelPopupCloseAnimate (GameObject.FindGameObjectWithTag(CommonConstants.POPUP_SECOND_SELECT_TAG));
		}
		void SendImageCancel () 
		{
			PopupSecondSelectPanel.Instance.PopClean();
			PanelPopupCloseAnimate (GameObject.FindGameObjectWithTag(CommonConstants.POPUP_SECOND_SELECT_TAG));
		}

		private void PanelPopupAnimate ( GameObject target )
		{
			//ポップ用の背景セット
			_popupOverlay.SetActive (true);

			target.GetComponent<uTweenScale> ().from = Vector3.zero;
			target.GetComponent<uTweenScale> ().to = new Vector3 (1, 1 ,1 );
			target.GetComponent<uTweenScale> ().delay    = 0.001f;
			target.GetComponent<uTweenScale> ().duration = 0.25f;
			target.GetComponent<uTweenScale> ().ResetToBeginning ();
			target.GetComponent<uTweenScale> ().enabled = true;
		}
      
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
              
        /// <summary>
        /// Makes the tex.
        /// </summary>
        /// <returns>The tex.</returns>
        /// <param name="tex">Tex.</param>
        private Texture2D MakeTex (string filePath) 
        {
            byte[] bytReadBinary = System.IO.File.ReadAllBytes (filePath);
            Texture2D texNewImage = new Texture2D(1, 1);
            texNewImage.LoadImage(bytReadBinary);
            texNewImage.Apply(true, false);
            return texNewImage;
        }

        #endregion
    }
}