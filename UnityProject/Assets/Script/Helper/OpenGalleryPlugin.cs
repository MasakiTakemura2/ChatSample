using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using ViewController;
using EventManager;
using uTools;


namespace Helper
{
    /// <summary>
    /// Camera helper.
    /// [IOS, ANDROID] Native Bind
    /// </summary>
    public class OpenGalleryPlugin : SingletonMonoBehaviour<OpenGalleryPlugin>
    {
        private GameObject _pictSetTargetObj;
        private Sprite sp;        

        [SerializeField]
        private GameObject _tmpImage_1;

        [SerializeField]
        private GameObject _tmpImage_2;

        [SerializeField]
        private GameObject _tmpImage_3;

        [SerializeField]
        private GameObject _popupOverlay;

        private string _sendFilePath = "";
        
        void cameraPermitOn (){

#if UNITY_ANDROID && !UNITY_EDITOR
            AndroidJavaClass unityPlayer   = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
            AndroidJavaClass galleryBinder = new AndroidJavaClass("com.gs.launchgallery.UnityBinder");
            galleryBinder.CallStatic("openCameraGallery", unityPlayer.GetStatic<AndroidJavaObject>("currentActivity"), true);
#endif
        }

        void cameraPermitOff (){
        
        }
        
        void cameraAfterConditionPermitOn (){
            
        }
        
        void cameraAfterConditionPermitOff (){}

        void galleryPermitOn ()
        {
#if UNITY_ANDROID && !UNITY_EDITOR
            AndroidJavaClass unityPlayer   = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
            AndroidJavaClass galleryBinder = new AndroidJavaClass("com.gs.launchgallery.UnityBinder");
            galleryBinder.CallStatic("openCameraGallery", unityPlayer.GetStatic<AndroidJavaObject>("currentActivity"), false);
#endif
        }

        void galleryPermitOff (){
        }


        /// <summary>
        /// Raises the click open gallery camera event.
        /// </summary>
        /// <param name="isCam">If set to <c>true</c> is cam.</param>
        public void OnClickOpenGalleryCamera (bool isCam)
        {
#if UNITY_ANDROID && !UNITY_EDITOR
            if (isCam == true) {
                if (UniAndroidPermission.IsPermitted (AndroidPermission.CAMERA) == false) {
                    UniAndroidPermission.RequestCameraPremission (AndroidPermission.CAMERA,cameraPermitOn, cameraPermitOff );
                } else {

            AndroidJavaClass unityPlayer   = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
            AndroidJavaClass galleryBinder = new AndroidJavaClass("com.gs.launchgallery.UnityBinder");
            galleryBinder.CallStatic("openCameraGallery", unityPlayer.GetStatic<AndroidJavaObject>("currentActivity"), isCam);

                }
            } else {
                if (UniAndroidPermission.IsPermitted (AndroidPermission.WRITE_EXTERNAL_STORAGE) == false) {
UniAndroidPermission.RequestCameraPremission (AndroidPermission.WRITE_EXTERNAL_STORAGE,galleryPermitOn, galleryPermitOff );
                } else {

            AndroidJavaClass unityPlayer   = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
            AndroidJavaClass galleryBinder = new AndroidJavaClass("com.gs.launchgallery.UnityBinder");
            galleryBinder.CallStatic("openCameraGallery", unityPlayer.GetStatic<AndroidJavaObject>("currentActivity"), isCam);

                }
            }
#endif
        }

        /// <summary>
        /// Raises the click open gallery camera event.
        /// </summary>
        /// <param name="pictObj">Pict object.</param>
        public void OnClickOpenGalleryCameraSet (GameObject pictObj)
        {

            
        
            Resources.UnloadUnusedAssets ();
            System.GC.Collect();

            _pictSetTargetObj = pictObj;
        }

        /// <summary>
        /// Raises the photo pick event.
        /// </summary>
        /// <param name="filePath">File path.</param>
        public void OnPhotoPick (string filePath)
        {
            Resources.UnloadUnusedAssets ();
            System.GC.Collect();
            CameraOrGallery.Instance._postLocalFilePath = filePath;
            StartCoroutine (LoadImageinImageView (filePath));
        }

        /// <summary>
        /// Loads the imagein image view.
        /// </summary>
        /// <returns>The imagein image view.</returns>
        /// <param name="filePath">File path.</param>
        IEnumerator LoadImageinImageView (string filePath)
        {
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

            if (_pictSetTargetObj.activeSelf == false)
                _pictSetTargetObj.SetActive (true);

            if (string.IsNullOrEmpty (filePath) == true) yield break;

            _sendFilePath = filePath;
            SendImageConfirmOpen (PopupSecondSelectPanel.Instance.gameObject);
        }


        /// <summary>
        /// Sends the image OK to button.
        /// </summary>
        /// <returns>The image OK to button.</returns>
        /// <param name="filePath">File path.</param>
        private IEnumerator SendImageOKToButton(string filePath)
        {
            if (_pictSetTargetObj.activeSelf == false)
                _pictSetTargetObj.SetActive (true);
            
            //マイページ
            if (MypageEventManager.Instance != null) {
                // pathが与えられたとして

 //TextureImporter ti = TextureImporter.GetAtPath( filePath ) as TextureImporter;
// ti.isReadable = true;
// AssetDatabase.ImportAsset(path, ImportAssetOptions.ForceUpdate);
                _pictSetTargetObj.GetComponent<RawImage> ().texture = MakeTex (filePath);
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
                _pictSetTargetObj.GetComponent<RawImage> ().texture = MakeTex (filePath);
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

                _pictSetTargetObj.GetComponent<RawImage> ().texture = MakeTex (filePath);
                PanelChat.Instance.MessageSendButton ();
            }

            Resources.UnloadUnusedAssets ();
            System.GC.Collect();
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
            Texture2D txtNewImage = new Texture2D(2, 2, TextureFormat.ARGB32, false);
            txtNewImage.LoadImage(bytReadBinary);
            txtNewImage.anisoLevel = 0;
            txtNewImage.Apply(true, false);
            return txtNewImage;
        }
    }
}