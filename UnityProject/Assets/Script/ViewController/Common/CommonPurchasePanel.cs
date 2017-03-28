using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Purchasing;
using UnityEngine.Events;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using uTools;
using BestHTTP;
using Http;
using UnityEngine.Purchasing.Security;
using UnityEngine.Purchasing.Extension;

using NendUnityPlugin.AD;
using NendUnityPlugin.Common;

namespace ViewController
{
	public class CommonPurchasePanel : SingletonMonoBehaviour<CommonPurchasePanel>
	{

		private static UnityIAPStoreListener m_singletonListener;        

		[SerializeField]
		private GameObject _loadingOverlay;

		[SerializeField]
		private GameObject _premiumRegistButtonLimited;

		[SerializeField]
		private GameObject _premiumRegistButton;

		[SerializeField]
		private GameObject _maskBg;

		[SerializeField]
		private GameObject _popupPayment;

		[SerializeField]
		private GameObject _popupContact;

		[SerializeField]
		private InputField _contactInput;

		[SerializeField]
		private string _localReceiptData;

		[SerializeField]
		private GameObject _restoreButton;
        
        [SerializeField]
        private NendAdBanner _nendAdBanner;


		/// <summary>
		/// Start this instance.
		/// </summary>
		void Start ()
		{
			Debug.Log("IAP ===>>>  GameObject Start!!!");
			_maskBg.SetActive (true);
			StartCoroutine (PaymentInit ());            
		}

		/// <summary>
		/// Payments the init.
		/// </summary>
		/// <returns>The init.</returns>
		private IEnumerator PaymentInit ()
		{
			_loadingOverlay.SetActive (true);

			PurchaseItemListApi.Init ();
			while (PurchaseItemListApi._success == false)
				yield return (PurchaseItemListApi._success == true);

			string productId = "";
			foreach (var item in PurchaseItemListApi._httpCatchData.result.purchase_items) {
				productId = item.product_id;
			}
			_premiumRegistButton.name = productId;
			_premiumRegistButtonLimited.name = productId;

			var builder = ConfigurationBuilder.Instance (StandardPurchasingModule.Instance ());
			_localReceiptData = "";                

			//IOSの場合。
			if (Application.platform == RuntimePlatform.IPhonePlayer) {
				_restoreButton.SetActive (true);                   
			} else{
				_restoreButton.SetActive (false);
			}

			if (m_singletonListener == null) {
				Debug.Log ("IAP ===>>> IAP Instance NULL . We are going to create instance. It must be singleton instance.");
				m_singletonListener = new UnityIAPStoreListener ();
				m_singletonListener.InitializeIAP ();
			}

			m_singletonListener.SetReceiptValidator(receiptValidation);

			m_singletonListener.SetOverlayGameObject (_loadingOverlay);                   

			_loadingOverlay.SetActive (false);
		}

		/// <summary>
		/// This function is called when the MonoBehaviour will be destroyed.
		/// </summary>
		void OnDestroy()
		{
			if(m_singletonListener!=null){
				m_singletonListener.removeReceiptValidator();
			}
		}

		private void receiptValidation(string item){

			if (item != null) {
				Debug.Log("IAP ===>>> delegate ===> going to call API");
				StartCoroutine (PaymentApiCall (item));
			}            
		}


		/// <summary>
		/// Paymets the API call.
		/// </summary>
		/// <returns>The API call.</returns>
		private IEnumerator PaymentApiCall (string receipt)
		{   
			Debug.Log ("IAP ===>>> Going to call API for receipt validation !!");

			new PaymentApi (receipt);
			while (PaymentApi._success == false)
				yield return (PaymentApi._success == true);

			if (PaymentApi._httpCatchData != null && PaymentApi._httpCatchData.result.payment == "true") {
				//Get User 更新用
				new GetUserApi ();
				while (GetUserApi._success == false)
					yield return (GetUserApi._success == true);

				PanelPopupAnimate (GameObject.FindGameObjectWithTag(CommonConstants.POPUP_BASIC_TAG));
				PopupPanel.Instance.PopMessageInsert (
					PaymentApi._httpCatchData.result.complete [0],
					LocalMsgConst.OK,
					PaymentApiFinish
				);

				m_singletonListener.FinishPendingPurchase();                

			}
			else{                
				m_singletonListener.FinishPendingPurchase();                
			}

			_loadingOverlay.SetActive (false);

		}

		/// <summary>
		/// Payments the API finish.
		/// </summary>
		public void PaymentApiFinish ()
		{
			if (GetUserApi._httpCatchData.result.is_auto_renewable == "true")
			{
				CommonConstants.IS_PREMIUM = true;
				//とりあえず、アイモバイルの広告全ストップ。
				IMobileSdkAdsUnityPlugin.stop ();
				string spotId = "";
				if (CommonConstants.IS_AD_TEST == true) {
					spotId = CommonConstants.IMOBILE_BANNER_SPOT_TEST_ID;
					//インタースティシャル広告
					IMobileSdkAdsUnityPlugin.stop(CommonConstants.IMOBILE_INTERSTATIAL_SPOT_TEST_ID); 
				} else {
					spotId = CommonConstants.IMOBILE_BANNER_SPOT_ID;
					//インタースティシャル広告
					IMobileSdkAdsUnityPlugin.stop(CommonConstants.IMOBILE_INTERSTATIAL_SPOT_ID);
				}
                
                _nendAdBanner.Pause ();
                _nendAdBanner.Hide ();

				IMobileSdkAdsUnityPlugin.stop (spotId);
				SceneHandleManager.NextSceneRedirect (CommonConstants.MYPAGE_SCENE);
			}

		}

		/// <summary>
		/// アイテムのリストア処理を行います。
		/// <return>NotInitialization/NetworkUnavailable/None/NotSupportedのいずれか</return>
		/// </summary>
		public void RestorePurchases ()
		{
			_loadingOverlay.SetActive (true);
			// iPhone/OSXでない場合
			if (Application.platform != RuntimePlatform.IPhonePlayer &&
				Application.platform != RuntimePlatform.OSXPlayer) {
				return;
			}

			if (m_singletonListener != null ) 
			{
				Debug.Log("IAP ===>>> Item going to restoring ......");
				m_singletonListener.RefreshReceipt();
			}                
		}


		/// <summary>
		/// Purchases the button.
		/// </summary>
		/// <param name="obj">Object.</param>
		public void PurchaseButton (GameObject obj)
		{
            Debug.Log ("purchase button tap");
			if (m_singletonListener != null ) 
			{
				var product = m_singletonListener.GetProductInfo(obj.name);	
				if (product != null && product.availableToPurchase) {                        
					_loadingOverlay.SetActive (true);
					AES256Cipher aesSet = new AES256Cipher ();
					string _uiid = NativeRecieveManager.GetUiid (DomainData._bundle);
					string payload = aesSet.AES_encrypt (_uiid + ":" + obj.name, HttpConstants.API_KEY_VALUE);

					m_singletonListener.InitiatePurchase (product, payload);
				}	
			}

		}

		/// <summary>
		/// Contacts the open button.
		/// </summary>
		public void ContactOpenButton ()
		{            
			_popupPayment.SetActive (false);
			_popupContact.SetActive (true);
		}

		/// <summary>
		/// Contacts the open button.
		/// </summary>
		public void ContactCloseButton ()
		{
			_popupPayment.SetActive (true);
			_popupContact.SetActive (false);
		}


		/// <summary>
		/// Cancels the button.
		/// </summary>
		public void CancelButton ()
		{
			_maskBg.SetActive (false);
			ClosePopupAnimate (this.gameObject);
		}


		/// <summary>
		/// Contacts the send button.
		/// </summary>
		public void ContactSendButton ()
		{
			StartCoroutine (SendApiSetWait ());
		}


		/// <summary>
		/// Sends the report wait.
		/// </summary>
		/// <returns>The report wait.</returns>
		private IEnumerator SendApiSetWait ()
		{
			_loadingOverlay.SetActive (true);
			new SendInquiryApi (_contactInput.text);
			while (SendInquiryApi._success == false)
				yield return (SendInquiryApi._success == true);
			_loadingOverlay.SetActive (false);

			//input data を 初期化
			_contactInput.text = "";
			PopupPanel.Instance.PopMessageInsert (
				SendInquiryApi._httpCatchData.result.complete [0],
				LocalMsgConst.OK,
				ContactSetFinishClose
			);
			OpenPopupAnimate (GameObject.FindGameObjectWithTag (CommonConstants.POPUP_BASIC_TAG));
		}

		/// <summary>
		/// イベント用
		/// </summary>
		private void ContactSetFinishClose ()
		{
			OpenPopupAnimate (GameObject.FindGameObjectWithTag (CommonConstants.POPUP_BASIC_TAG));
		}        


		#region アニメーション処理
		/// <summary>
		/// Closes the popup animate.
		/// </summary>
		/// <param name="fromObj">From object.</param>
		public void OpenPopupAnimate (GameObject fromObj)
		{
			_maskBg.SetActive (true);
			fromObj.GetComponent<uTweenPosition> ().delay = 0.001f;
			fromObj.GetComponent<uTweenPosition> ().duration = 0.25f;
			fromObj.GetComponent<uTweenPosition> ().from = new Vector2 (0, -2500f);
			fromObj.GetComponent<uTweenPosition> ().to = new Vector2 (0, -30f);
			fromObj.GetComponent<uTweenPosition> ().ResetToBeginning ();
			fromObj.GetComponent<uTweenPosition> ().enabled = true;
		}

		/// <summary>
		/// Closes the popup animate.
		/// </summary>
		/// <param name="fromObj">From object.</param>
		public void ClosePopupAnimate (GameObject fromObj)
		{
			_maskBg.SetActive (false);
			fromObj.GetComponent<uTweenPosition> ().delay = 0.001f;
			fromObj.GetComponent<uTweenPosition> ().duration = 0.25f;
			fromObj.GetComponent<uTweenPosition> ().from = Vector3.zero;
			fromObj.GetComponent<uTweenPosition> ().to = new Vector2 (0, -2500f);
			fromObj.GetComponent<uTweenPosition> ().ResetToBeginning ();
			fromObj.GetComponent<uTweenPosition> ().enabled = true;
		}

		/// <summary>
		/// Res the panel animate.
		/// </summary>
		/// <param name="target">Target.</param>
		private void PanelPopupAnimate ( GameObject target )
		{
			target.GetComponent<uTweenScale> ().from     = Vector3.zero;
			target.GetComponent<uTweenScale> ().to       = new Vector3 (1, 1 ,1 );
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
			target.GetComponent<uTweenScale> ().delay    = 0.01f;
			target.GetComponent<uTweenScale> ().duration = 0.25f;
			target.GetComponent<uTweenScale> ().from = new Vector3 (1f, 1f ,1f );
			target.GetComponent<uTweenScale> ().to = Vector3.zero;
			target.GetComponent<uTweenScale> ().ResetToBeginning ();
			target.GetComponent<uTweenScale> ().enabled = true;
		}        
		#endregion       
	}

	public class UnityIAPStoreListener : IStoreListener{
		// Unity IAP objects 
		private IStoreController m_Controller; //コントローラーにプロダクトが入る
		private IExtensionProvider m_Extentions;
		public Dictionary<string,Product> _pendingItems;

		GameObject _overlayGameObject;

		public delegate void ReceiptValidation(string item);
		public ReceiptValidation receiptValidator = null;

		private Boolean isReceiptValidating = false;

		public void InitializeIAP(){

			//基本は定形 （購入の内部もカスタマイズしたりも出来る）			
			Debug.Log ("IAP ===>>> Initialize Starting ....");
			var builder = ConfigurationBuilder.Instance (StandardPurchasingModule.Instance ());
			var items = PurchaseItemListApi._httpCatchData.result.purchase_items;
            var item = items [items.Count - 1];
              Debug.Log("IAP Product ID >>>> " + item.product_id);
              builder.AddProduct ("subscription",
                  ProductType.Subscription,
                  new IDs {
                      { item.product_id, AppleAppStore.Name },
                      { item.product_id, GooglePlay.Name }
                  }
              );   
			//foreach (var item in items) {
                
			//	Debug.Log("IAP Product ID >>>> " + item.product_id);
			//	builder.AddProduct ("subscription",
			//		ProductType.Subscription,
			//		new IDs {
			//			{ item.product_id, AppleAppStore.Name },
			//			{ item.product_id, GooglePlay.Name }
			//		}
			//	);                
			//}

			_pendingItems = new Dictionary<string,Product> ();

			UnityPurchasing.Initialize (this, builder);       

		}

		/// <summary>
		/// Raises the initialized event.
		/// 初期化イベント
		/// </summary>
		/// <param name="controller">Controller.</param>
		/// <param name="extentions">Extentions.</param>
		public void OnInitialized (IStoreController controller, IExtensionProvider extentions)
		{
			Debug.Log ("IAP ===>>> Initialize Successful ....");
			m_Controller = controller; //コントローラーは購入時に使うので保持しておく
			m_Extentions = extentions;

		}

		/// <summary>
		/// Raises the initialize failed event.
		/// 初期化失敗 引数「error」に失敗の理由が入る
		/// </summary>
		/// <param name="error">Error.</param>
		public void OnInitializeFailed (InitializationFailureReason error)
		{			

			Debug.Log ("IAP ===>>> Initialize Failed ....");			
			switch (error) {
			case InitializationFailureReason.AppNotKnown:
				Debug.LogError ("Is your App correctly uploaded on the relevant publisher console?");
				break;
			case InitializationFailureReason.PurchasingUnavailable:
				// Ask the user if billing is disabled in device settings.
				Debug.Log ("Billing disabled!");
				break;
			case InitializationFailureReason.NoProductsAvailable:
				// Developer configuration error; check product metadata.
				Debug.Log ("No products available for purchase!");
				break;
			}
		}

		/// <summary>
		/// Proccesses the puchase.
		/// プラットフォームの購入が購入完了。
		/// </summary>
		/// <returns>The puchase.</returns>
		/// <param name="e">E.</param>
		public PurchaseProcessingResult ProcessPurchase (PurchaseEventArgs e)
		{	
			Debug.Log ("IAP ===>>> Process Purchase Block.");  

			Product currentItem = e.purchasedProduct;
			string currentTrasactionID = currentItem.transactionID;
			string productID = currentItem.definition.id;

			//Subscription items 
			if(currentItem.definition.id=="subscription"){
				if(currentItem != null && currentItem.hasReceipt){

					LitJson.JsonData jsonData =  LitJson.JsonMapper.ToObject(currentItem.receipt);
					string receipt = (string)jsonData["Payload"];

					if(receipt.Length>0){
						//API Call from here 
						StartReciptValidation(receipt);
					}
					else{
						RefreshReceipt();
					}

				}
				return PurchaseProcessingResult.Complete;
			}

			return PurchaseProcessingResult.Complete;
		}


		/// <summary>
		/// Raises the purchase faield event.
		/// 購入失敗
		/// </summary>
		/// <param name="i">The index.</param>
		/// <param name="p">P.</param>
		public void OnPurchaseFailed (Product item, PurchaseFailureReason r)
		{
			Debug.Log ("IAP ===>>> Purchase Failure Block.");               

			if (this._overlayGameObject != null) {
				this._overlayGameObject.SetActive (false);
			}		
		}


		public bool IsInitialized()
		{
			// Only say we are initialized if both the Purchasing references are set.
			// 二つのPurchasing の参照が設定されていれば、初期設定されていると言える
			return m_Controller != null && m_Extentions != null;
		}

		public Product GetProductInfo(string id){
			if(IsInitialized()){
				return m_Controller.products.WithStoreSpecificID(id);
			}
			return null;

		}

		public void InitiatePurchase(Product product, string payload){

			if(IsInitialized()){                                
				m_Controller.InitiatePurchase (product,payload);
			}			
		}

		public void FinishPendingPurchase(){

			this.isReceiptValidating = false;              
		}

		private void StartReciptValidation(string receipt){

			if(!this.isReceiptValidating){ 
				this.isReceiptValidating = true;                                                 
				if(receiptValidator!=null){ 
					if (_overlayGameObject != null) {
						_overlayGameObject.SetActive (true);
					}    
					this.isReceiptValidating = true;                                                                       
					receiptValidator(receipt);
				}
				else{
					this.isReceiptValidating = false; 
				}
			}            
		}

		public void RefreshReceipt(){			

			m_Extentions.GetExtension<IAppleExtensions> ().RefreshAppReceipt (receipt => {
				// This handler is invoked if the request is successful.
				// Receipt will be the latest app receipt.				
				Debug.Log ("IAP ===>>> Receipt Refresh OK!!!");  
				if(receipt!=null)
					StartReciptValidation(receipt);


			},
				() => {
					// This handler will be invoked if the request fails,
					// such as if the network is unavailable or the user
					// enters the wrong password.
					Debug.Log ("IAP ===>>> Receipt Refresh Failed!!!");					                    
				});

		}

		public void SetOverlayGameObject(GameObject overlayGameObject){

			this._overlayGameObject = overlayGameObject;
		}

		public void SetReceiptValidator(ReceiptValidation receiptValidator){
			Debug.Log("IAP ===>>> Recipt Validator set");   
			if(receiptValidator!=null){         
				this.receiptValidator = null;
			}
			this.receiptValidator = receiptValidator;
			this.isReceiptValidating = false;
		}

		public void removeReceiptValidator(){

			if(this.receiptValidator!=null){
				this.receiptValidator = null;
			}
		}

	}
}