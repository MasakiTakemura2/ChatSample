using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using UnityEngine.Purchasing;
using UnityEngine.Events;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using ViewController;
using Http;
using uTools;

namespace Helper {
    /// <summary>
    /// Purchase manager.
    /// </summary>
    public class PurchaseManager : SingletonMonoBehaviour<PurchaseManager>//, IStoreListener
    {
        [SerializeField]
        private Text _tmpText;

        [SerializeField]
        private GameObject _panelPurchase;

        [SerializeField]
        private Text _point;

        [SerializeField]
        private GameObject _loadingOverlay;
        
        [SerializeField]
        private GameObject _popupOverlay;

        [SerializeField]
        private GameObject _panelTerms;

        [SerializeField]
        private GameObject _panelTrans;

        [SerializeField]
        private Transform _purchaseItemList;

        [SerializeField]
        private SwipeRecognizer _backSwipe;

        [SerializeField]
        private GameObject _guidePanel;

        [SerializeField]
        private GameObject _introduction;

        #region member variable
        // Unity IAP objects 
        //private static IStoreController m_Controller; //コントローラーにプロダクトが入る
        //private static IExtensionProvider m_Extentions;
        #endregion

        #region life cycle
        /// <summary>
        /// Ons the application pause.
        /// </summary>
        /// <returns>The application pause.</returns>
        /// <param name="pauseStatus">Pause status.</param>
        void OnApplicationPause (bool pauseStatus)
        {
            if (pauseStatus == true) {
                //Debug.Log(" バックグラウンドに入る処理。");
            } else {
                string catchData = null;
#if UNITY_ANDROID && !UNITY_EDITOR
    catchData  = GCMService.GetPushMessage();
#elif UNITY_IPHONE && !UNITY_EDITOR
    catchData = NativeRecieveManager.GetPushMessageIos ();
#endif
                if (string.IsNullOrEmpty(catchData) == false) {
                    NotificationRecieveManager.NextSceneProccess (catchData);
                    NotificationRecieveManager._isCatch = true;
                }
            }
        }

        IEnumerator Start ()
        {
            _loadingOverlay.SetActive (true);

            if (string.IsNullOrEmpty (AppStartLoadBalanceManager._userKey) == true)
            {
                //サーバーにイベント通知用Api(インストール時に一回のみ) ----------------------
                string _commonFileName = LocalFileConstants.GetLocalFileDir () + LocalFileConstants.COMMON_LOCAL_FILE_NAME;

                LocalFileHandler.Init (_commonFileName);

                //ファイルが作成されるまでポーリングして処理待ち
                while (System.IO.File.Exists (_commonFileName) == false)
                    yield return (System.IO.File.Exists (_commonFileName) == true);

                //ここでユーザーキーを取得
                AppStartLoadBalanceManager._userKey = LocalFileHandler.GetString (LocalFileConstants.USER_KEY);

#if UNITY_ANDROID
    //ステータスバーを表示 //Android用
    ApplicationChrome.statusBarState = ApplicationChrome.States.VisibleOverContent;
#endif
            }

            while (string.IsNullOrEmpty (AppStartLoadBalanceManager._userKey) == true)
                yield return (string.IsNullOrEmpty (AppStartLoadBalanceManager._userKey) == false);

            //ユーザーデータ取得。
            new GetUserApi ();
            while (GetUserApi._success == false)
                yield return (GetUserApi._success == true);

            AppStartLoadBalanceManager._gender = GetUserApi._httpCatchData.result.user.sex_cd;

            //メンテナンスの場合、処理を止める。
            if (AppliEventController.Instance.MaintenanceCheck () == true) {
                _loadingOverlay.SetActive (false);
                yield break;
            }

			//ユーザーのステータスをチェックする処理。
			AppliEventController.Instance.UserStatusProblem ();

            //強制アップデートの場合、処理を止める。
            if (AppliEventController.Instance.ForceUpdateCheck () == true) {
                _loadingOverlay.SetActive (false);
                yield break;
            }
            if (GetUserApi._httpCatchData.result.reject == "true") {
                WebviewIntroductionOpen ();
                yield break;
            }

            //マスターデータ取得
            if (InitDataApi._httpCatchData == null)
            {
                new InitDataApi ();
                while (InitDataApi._success == false)
                    yield return (InitDataApi._success == true);
            }

            //基本プロフィールを作成しているかどうかの判定。念の為、もう一回判定。
            if (string.IsNullOrEmpty (GetUserApi._httpCatchData.result.user.name) == false &&
                string.IsNullOrEmpty (GetUserApi._httpCatchData.result.user.pref) == false &&
                string.IsNullOrEmpty (GetUserApi._httpCatchData.result.user.city_id) == false &&
                string.IsNullOrEmpty (GetUserApi._httpCatchData.result.user.birth_date) == false) {
                //基本プロフィールを作成している。
                AppStartLoadBalanceManager._isBaseProfile = true;
            } else {
                //まだ、基本プロフィールを作成していない。
                AppStartLoadBalanceManager._isBaseProfile = false;
            }


            Helper.LocalFileHandler.Init (LocalFileConstants.GetLocalFileDir () + LocalFileConstants.COMMON_LOCAL_FILE_NAME);

            string fromScene = Helper.LocalFileHandler.GetString (LocalFileConstants.FROM_MYPAGE_SCENE);
            
            if (string.IsNullOrEmpty (fromScene) == false && fromScene == CommonConstants.MYPAGE_SCENE) {
                _backSwipe.EventMessageTarget = _panelPurchase.gameObject;
                HeaderPanel.Instance.BackButtonSwitch (true, BackMypageScene);
            }
        
        
            if (SceneManager.GetActiveScene ().name == CommonConstants.PURCHASE_SCENE)
            {
                while (string.IsNullOrEmpty (AppStartLoadBalanceManager._userKey) == true)
                    yield return (string.IsNullOrEmpty (AppStartLoadBalanceManager._userKey) == false);

                new PurchaseItemListApi ();
                while (PurchaseItemListApi._success == false)
                    yield return (PurchaseItemListApi._success == true);
                  

                //if (m_Controller == null) {
                //    //基本は定形 （購入の内部もカスタマイズしたりも出来る）
                //    var builder = ConfigurationBuilder.Instance (StandardPurchasingModule.Instance ());
                //    var items = PurchaseItemListApi._httpCatchData.result.purchase_items;

                //    foreach (var item in items) {
                //        builder.AddProduct (item.product_id,
                //            ProductType.Consumable,
                //            new IDs {
                //                { item.product_id, AppleAppStore.Name },
                //                { item.product_id, GooglePlay.Name }
                //            }
                //        );
                //    }

                //    UnityPurchasing.Initialize (this, builder);
                //    //見本　↓↓↓↓↓↓↓↓↓↓↓↓
                //    //builder.AddProduct ("chat.test.0113.120",  ProductType.Consumable, new IDs {{"chat.test.0113.120", AppleAppStore.Name},{"chat.test.0113.120", GooglePlay.Name}});
                //    //builder.AddProduct ("chat.test.0113.360",  ProductType.Consumable, new IDs {{"chat.test.0113.360", AppleAppStore.Name},{"chat.test.0113.360", GooglePlay.Name}});
                //}
                _loadingOverlay.SetActive (false);
            }        

            if (SceneManager.GetActiveScene ().name == CommonConstants.PURCHASE_SCENE) {
                if (_tmpText != null)
                    _tmpText.text = LocalMsgConst.TITLE_PURCHASE;

                new GetUserApi ();
                while (GetUserApi._success == true)
                    yield return (GetUserApi._success == false);

                if (GetUserApi._httpCatchData != null) {
                    int poInt = int.Parse (GetUserApi._httpCatchData.result.user.current_point);
                    string poText = string.Format ("{0:#,0}", poInt);
                    _point.text = String.Format (LocalMsgConst.USE_POINT_TEXT + " {0:#,0} ", "<size=50>" + poText + "</size>") + LocalMsgConst.PT_TEXT;
                }
                    

                yield return StartCoroutine (PurchaseItemSet ());
            }
            yield break;
        }
        
        /// <summary>
        /// Backs the mypage scene.
        /// </summary>
        /// <returns>The mypage scene.</returns>
        void BackMypageScene () {
            HeaderPanel.Instance.BackButtonSwitch (false);
            SceneHandleManager.NextSceneRedirect (CommonConstants.MYPAGE_SCENE);
            Helper.LocalFileHandler.SetString (LocalFileConstants.FROM_MYPAGE_SCENE, "");
            Helper.LocalFileHandler.Flush ();
        }
        
        #endregion

        #region 課金アイテムオブジェクトセット処理
        /// <summary>
        /// Purchases the item set.
        /// </summary>
        /// <returns>The item set.</returns>
        private IEnumerator PurchaseItemSet ()
        {
            int count = 0;
            foreach (var item in PurchaseItemListApi._httpCatchData.result.purchase_items) {
                _purchaseItemList.GetChild (count).gameObject.SetActive (true);
                _purchaseItemList.GetChild (count).GetComponent<PurchaseItem> ().Init (item);
                count++;
            }

            //不要なオブジェクトを削除。
            for (int i = count; i < _purchaseItemList.childCount; i++) 
            {
                _purchaseItemList.GetChild (i).gameObject.SetActive (false);
            }

            yield break;
        }
        #endregion


        #region アプリの紹介 - WEBVIEW
        /// <summary>
        /// Webviews the term open.
        /// </summary>
        /// <returns>The term open.</returns>
        public void WebviewIntroductionOpen ()
        {
            HeaderPanel.Instance.BackButtonSwitch (false);
            //HeaderPanel.Instance.BackButtonSwitch (true, WebviewIntroductionClose);
            if (_introduction != null) {
                string url= DomainData.GetWebviewDataURL(DomainData.WEBVIEW_INTRODUCTION);
                string querystring = "";
    
                querystring = "?user_key=" + AppStartLoadBalanceManager._userKey;
    
                url = url += querystring;
                var uri = new System.Uri (url);
                Application.OpenURL (uri.AbsoluteUri);
                //_introduction.SetActive (true);
                //_introduction.transform.GetChild (0).GetChild(0).GetComponent<IntroductionWebView> ().Init ();
                //PanelAnimate (_introduction);
            }
        }

        /// <summary>
        /// Webviews the term close.
        /// </summary>
        /// <returns>The term close.</returns>
        public void WebviewIntroductionClose ()
        {
            HeaderPanel.Instance.BackButtonSwitch (false);
            if (_introduction != null) 
            {
                _introduction.transform.GetChild (0).GetChild(0).GetComponent<IntroductionWebView> ().ViewHide ();
                BackButton (_introduction);
            }
        }

        /// <summary>
        /// Webviews the term close event.
        /// </summary>
        /// <returns>The term close event.</returns>
        void WebviewIntroductionCloseEvent ()
        {
            WebviewIntroductionClose ();
        }
        #endregion


        #region 利用規約 open, close
        /// <summary>
        /// Webviews the term open.
        /// </summary>
        /// <returns>The term open.</returns>
        public void WebviewTermOpen (GameObject animObj)
        {
            HeaderPanel.Instance.BackButtonSwitch (false);
            HeaderPanel.Instance.BackButtonSwitch (true, WebviewTermCloseEvent);
            _panelTerms.SetActive (true);
            PanelAnimate (animObj);
        }

        /// <summary>
        /// Webviews the term close.
        /// </summary>
        /// <returns>The term close.</returns>
        public void WebviewTermClose (GameObject animObj)
        {
            HeaderPanel.Instance.BackButtonSwitch (false);
            _panelTerms.SetActive (false);
            BackButton (animObj);
        }

        /// <summary>
        /// Webviews the term close event.
        /// </summary>
        /// <returns>The term close event.</returns>
        void WebviewTermCloseEvent ()
        {
            WebviewTermClose (_panelTerms);
        }
        #endregion

        #region 特定商取引 open, close
        /// <summary>
        /// Webviews the trans open.
        /// </summary>
        /// <returns>The trans open.</returns>
        /// <param name="animObj">Animation object.</param>
        public void WebviewTransOpen (GameObject animObj)
        {
            HeaderPanel.Instance.BackButtonSwitch (false);
            HeaderPanel.Instance.BackButtonSwitch (true, WebviewTransCloseEvent);
            _panelTrans.SetActive (true);
            PanelAnimate (animObj);
        }

        /// <summary>
        /// Webviews the trans close.
        /// </summary>
        /// <returns>The trans close.</returns>
        /// <param name="animObj">Animation object.</param>
        public void WebviewTransClose (GameObject animObj)
        {
            HeaderPanel.Instance.BackButtonSwitch (false);
            _panelTrans.SetActive (false);
            BackButton (animObj);
        }

        /// <summary>
        /// Webviews the trans close event.
        /// </summary>
        /// <returns>The trans close event.</returns>
        void WebviewTransCloseEvent ()
        {
            WebviewTransClose (_panelTrans);
        }
        #endregion

        ///// <summary>
        ///// Raises the initialized event.
        ///// 初期化イベント
        ///// </summary>
        ///// <param name="controller">Controller.</param>
        ///// <param name="extentions">Extentions.</param>
        //public void OnInitialized (IStoreController controller, IExtensionProvider extentions)
        //{
        //    m_Controller = controller; //コントローラーは購入時に使うので保持しておく
        //    m_Extentions = extentions;
        //}

        ///// <summary>
        ///// Raises the initialize failed event.
        ///// 初期化失敗 引数「error」に失敗の理由が入る
        ///// </summary>
        ///// <param name="error">Error.</param>
        //public void OnInitializeFailed (InitializationFailureReason error)
        //{
        //    Debug.Log ("Billing failed to initialize!");
        //    switch (error) {
        //    case InitializationFailureReason.AppNotKnown:
        //        Debug.LogError ("Is your App correctly uploaded on the relevant publisher console?");
        //        break;
        //    case InitializationFailureReason.PurchasingUnavailable:
        //        // Ask the user if billing is disabled in device settings.
        //        Debug.Log ("Billing disabled!");
        //        break;
        //    case InitializationFailureReason.NoProductsAvailable:
        //        // Developer configuration error; check product metadata.
        //        Debug.Log ("No products available for purchase!");
        //        break;
        //    }
        //}


        #region 課金処理
        /// <summary>
        /// Purchases the button.
        /// </summary>
        /// <param name="obj">Object.</param>
    //    public void PurchaseButton (GameObject obj)
    //    {
    //        if (m_Controller != null ) 
    //        {
    //            _loadingOverlay.SetActive (true);
    //            var product = m_Controller.products.WithID (obj.name);
    
    //            if (product != null && product.availableToPurchase) {
    //                AES256Cipher aesSet = new AES256Cipher ();
    //                string _uiid   = NativeRecieveManager.GetUiid (DomainData._bundle);
    //                string payload = aesSet.AES_encrypt (_uiid + ":" + obj.name, HttpConstants.API_KEY_VALUE);
    //Debug.Log (payload + " payload ");
    
    //                m_Controller.InitiatePurchase (product, payload);
    //            }
    //        }
    //    }

//        /// <summary>
//        /// Proccesses the puchase.
//        /// 購入完了。
//        /// </summary>
//        /// <returns>The puchase.</returns>
//        /// <param name="e">E.</param>
//        public PurchaseProcessingResult ProcessPurchase (PurchaseEventArgs e)
//        {
////            Debug.Log ("------------------------------------------------------------------------------------------------------------------------------------");
////            Debug.Log ("Purchase OK: " + e.purchasedProduct.definition.id);
////            Debug.Log ("Receipt: " + e.purchasedProduct.receipt);
////            Debug.Log (" Test1 Test1 Test1 Test1 Test1 ");
//            LitJson.JsonData jsonData =  LitJson.JsonMapper.ToObject(e.purchasedProduct.receipt);
//Debug.Log ("jsonData data null ? : " +jsonData);
//            string postJson = (string)jsonData["Payload"];

//Debug.Log ("jsonData data[postJson null ? : " + postJson);

//#if !UNITY_EDITOR
//Debug.Log (e.purchasedProduct.metadata.localizedTitle + " localizedTitle ? ") ;
//Debug.Log (e.purchasedProduct.metadata.isoCurrencyCode + " isoCurrencyCode ? ") ;
//Debug.Log ((double)e.purchasedProduct.metadata.localizedPrice + " localizedPrice ? ");

//#endif
            
////Debug.Log ("AAAAAAA" + e.purchasedProduct.hasReceipt);
//            //ここでIDとレシートを自社サーバーに送信して記録する処理を入れる。
//            if (e.purchasedProduct.hasReceipt == true && postJson != null) {
////Debug.Log ("API スタートコルーチン前の処理が飛んでいるか？");
//                    StartCoroutine (PaymetApiCall (postJson, e));
//            }
            
//            return PurchaseProcessingResult.Complete; //アプリ側の課金処理完了
//        }
        
        
        

        ///// <summary>
        ///// Raises the purchase faield event.
        ///// 購入失敗
        ///// </summary>
        ///// <param name="i">The index.</param>
        ///// <param name="p">P.</param>
        //public void OnPurchaseFailed (Product item, PurchaseFailureReason r)
        //{
        //    _loadingOverlay.SetActive (false);
        //    Debug.Log ("Purchase failed: " + item.definition.id);
        //    Debug.Log (r);
        //}

//        /// <summary>
//        /// Paymets the API call.
//        /// </summary>
//        /// <returns>The API call.</returns>
//        private IEnumerator PaymetApiCall (string receipt, PurchaseEventArgs e) 
//        {

//            new PaymentApi (receipt);
//            while (PaymentApi._success == false)
//                yield return (PaymentApi._success == true);

//            //Get User 更新用
//            new GetUserApi ();
//            while (GetUserApi._success == false)
//                yield return (GetUserApi._success == true);
                            
//            _loadingOverlay.SetActive (false);
            
//            PopupPanel.Instance.PopMessageInsert (
//                PaymentApi._httpCatchData.result.complete[0],
//                LocalMsgConst.OK,
//                PaymentApiFinish
//            );

//            //表示しているポイント書き換え
//            //_point.text = String.Format (LocalMsgConst.USE_POINT_TEXT + " {0:#,0}", "<size=50>" + PaymentApi._httpCatchData.result.current_point + "</size>") + LocalMsgConst.PT_TEXT;
//            //_point.text = String.Format (LocalMsgConst.USE_POINT_TEXT + " {0:#,0} ", "<size=50>" + GetUserApi._httpCatchData.result.user.current_point + "</size>") + LocalMsgConst.PT_TEXT;

//            int poInt = int.Parse (GetUserApi._httpCatchData.result.user.current_point);
//            string poText = string.Format ("{0:#,0}", poInt);
//            _point.text = String.Format (LocalMsgConst.USE_POINT_TEXT + " {0:#,0} ", "<size=50>" + poText + "</size>") + LocalMsgConst.PT_TEXT;

//#if !UNITY_EDITOR
//            if (PaymentApi._httpCatchData.result.payment == "true") {
//                Partytrack.sendPayment (e.purchasedProduct.metadata.localizedTitle, 1 ,e.purchasedProduct.metadata.isoCurrencyCode, (double)e.purchasedProduct.metadata.localizedPrice);
//            }
//#endif

//            PanelPopupAnimate (GameObject.FindGameObjectWithTag(CommonConstants.POPUP_BASIC_TAG));
//        }

        /// <summary>
        /// Payments the API finish.
        /// </summary>
        public void PaymentApiFinish () {
            PopupPanel.Instance.PopClean ();
            PanelPopupCloseAnimate (GameObject.FindGameObjectWithTag(CommonConstants.POPUP_BASIC_TAG));
        }
        #endregion


        #region Tweeen系の処理
         /// <summary>
        /// Backs the button.
        /// </summary>
        /// <param name="fromObj">From object.</param>
        /// <param name="toObj">To object.</param>
        private void BackButton (GameObject fromObj) 
        {
            fromObj.GetComponent<uTweenPosition> ().delay    = 0.001f;
            fromObj.GetComponent<uTweenPosition> ().duration = 0.25f;
            fromObj.GetComponent<uTweenPosition> ().to      = new Vector2 (2500f,0);
            fromObj.GetComponent<uTweenPosition> ().from    = Vector3.zero;
            fromObj.GetComponent<uTweenPosition> ().ResetToBeginning ();
            fromObj.GetComponent<uTweenPosition> ().enabled = true;
        }
        
        /// <summary>
        /// Res the panel animate.
        /// </summary>
        /// <param name="target">Target.</param>
        private void PanelAnimate ( GameObject target )
        {
            target.GetComponent<uTweenPosition> ().from = new Vector2 (2500f, 0);
            target.GetComponent<uTweenPosition> ().delay    = 0.001f;
            target.GetComponent<uTweenPosition> ().duration = 0.25f;
            target.GetComponent<uTweenPosition> ().to = Vector3.zero;
            target.GetComponent<uTweenPosition> ().ResetToBeginning ();
            target.GetComponent<uTweenPosition> ().enabled = true;
        }
        
        
        /// <summary>
        /// Res the panel animate.
        /// </summary>
        /// <param name="target">Target.</param>
        private void PanelPopupAnimate ( GameObject target )
        {
            _popupOverlay.SetActive (false);
            target.GetComponent<uTweenScale> ().from = Vector3.zero;
            target.GetComponent<uTweenScale> ().to = new Vector3 (1, 1 ,1 );
            target.GetComponent<uTweenScale> ().delay    = 0.01f;
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
            target.GetComponent<uTweenScale> ().delay    = 0.01f;
            target.GetComponent<uTweenScale> ().duration = 0.25f;
            target.GetComponent<uTweenScale> ().from = new Vector3 (1f, 1f ,1f );
            target.GetComponent<uTweenScale> ().to = Vector3.zero;
            target.GetComponent<uTweenScale> ().ResetToBeginning ();
            target.GetComponent<uTweenScale> ().enabled = true;
        }
        #endregion

        /// <summary>
        /// Loadings the switch.
        /// </summary>
        /// <param name="isOn">If set to <c>true</c> is on.</param>
        public void LoadingSwitch(bool isOn) {
            _loadingOverlay.SetActive (isOn);
        }


        /// <summary>
        /// Webs the view terms switch.
        /// </summary>
        /// <param name="isSWitch">If set to <c>true</c> is S witch.</param>
        public GameObject WebViewTermsSwitch (bool isSWitch)
        {
            _panelTerms.SetActive (isSWitch);
            return _panelTerms;
        }
    }
}