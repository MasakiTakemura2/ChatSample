using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using BestHTTP;
using Helper;
using UnityEngine.UI;
using uTools;

namespace Http {
	public class HttpHandler : SingletonMonoBehaviour<HttpHandler> {

        #region member variable
        public static bool _reqSuccess = false;
        private static readonly int _connectTimeout = 20000;
        private static readonly int _reqTimeOut     = 60;
        #endregion

        #region Http Request Prosseing
        /// <summary>
        /// Send the specified url, reqPostDatas, callback and errorCallBack.
        /// 通常のリクエスト用。
        /// </summary>
        /// <param name="url">URL.</param>
        /// <param name="reqPostDatas">Req post datas.</param>
        /// <param name="callback">Callback.</param>
        /// <param name="errorCallBack">Error call back.</param>
        /// <typeparam name="T">The 1st type parameter.</typeparam>
		public static T Send<T> ( string url, Dictionary<string, string> reqPostDatas, Action<T> callback, Action<Http.ErrorEntity.Error> errorCallBack = null)
		{
            // ネットワークの状況により処理を変更
            if(Application.internetReachability==NetworkReachability.NotReachable) {
                //ネットワークに接続不可な場合の処理
                PopUpWindow (LocalMsgConst.NETWORK_ERROR_TEXT);
                return default(T);
            }

            _reqSuccess   = false;
            HTTPRequest request = new HTTPRequest(new Uri(url), HTTPMethods.Post, (req, resp) =>
			{
				switch (req.State) {
				// The request finished without any problem.
				case HTTPRequestStates.Finished:
                    if (resp.DataAsText.Contains("error") == true) {
                    var err = StringLib.JsonToObject<ErrorEntity.Result> (resp.DataAsText);
                        //サーバー系のエラーキャッチ
                        if (err.result.error != null) {
                            PopUpWindow (err.result.error[0]);

								if(errorCallBack != null)
								{
									errorCallBack(err.result);
								}
                            break;
                        }
                    }

				    Debug.Log("Request Finished Successfully!\n" + resp.DataAsText);

                    //if (err.result.error != null) {
                    //    Debug.Log (err.result.error);
                    //    break;
                    //}

                    callback (StringLib.JsonToObject<T>(resp.DataAsText));
                    
                    _reqSuccess = true;
                    break;
				// The request finished with an unexpected error.
				// The request's Exception property may contain more information about the error.
				case HTTPRequestStates.Error:
					Debug.LogError("Request Finished with Error! " +
						(req.Exception != null ?
							(req.Exception.Message + "\n" +
								req.Exception.StackTrace) :
							"No Exception"));
				break;
				// The request aborted, initiated by the user.
				case HTTPRequestStates.Aborted:
                     Debug.LogWarning("Request Aborted!"); break;
				// Ceonnecting to the server timed out.
				case HTTPRequestStates.ConnectionTimedOut:
                    Debug.LogError("Connection Timed Out!"); break;
				// The request didn't finished in the given time.
				case HTTPRequestStates.TimedOut:
                   Debug.LogError("Processing the request Timed Out!"); break;
				}
            });
            //HTTPMethods.Post
			// Very little time, for testing purposes:
            Debug.Log (url);

            if(reqPostDatas.Count > 0) {
                foreach ( var param in reqPostDatas) {

Debug.Log ("key: " + param.Key+" val: "+param.Value) ;

                    request.AddField(param.Key, param.Value);
                }
            }
            request.ConnectTimeout = TimeSpan.FromMilliseconds(_connectTimeout);
            request.Timeout = TimeSpan.FromSeconds(_reqTimeOut);
			request.DisableCache = true;
			request.Send();
            return default(T);
         }
         
         
        /// <summary>
        /// Send the specified url, reqPostDatas, callback, errorCallBack and isWWW.
        /// </summary>
        /// <param name="url">URL.</param>
        /// <param name="reqPostDatas">Req post datas.</param>
        /// <param name="callback">Callback.</param>
        /// <param name="errorCallBack">Error call back.</param>
        /// <param name="isWWW">Is www.</param>
        /// <typeparam name="T">The 1st type parameter.</typeparam>
        public static T Send<T> ( string url, Dictionary<string, string> reqPostDatas , bool isWWW, Action<T> callback, Action<Http.ErrorEntity.Error> errorCallBack = null)
        {
            // ネットワークの状況により処理を変更
            if(Application.internetReachability==NetworkReachability.NotReachable) {
                //ネットワークに接続不可な場合の処理
                PopUpWindow (LocalMsgConst.NETWORK_ERROR_TEXT);
                return default(T);
            }

            _reqSuccess   = false;
            HTTPRequest request = new HTTPRequest(new Uri(url), HTTPMethods.Post, (req, resp) =>
            {
                switch (req.State) {
                // The request finished without any problem.
                case HTTPRequestStates.Finished:
                    if (resp.DataAsText.Contains("error") == true) {
                    var err = StringLib.JsonToObject<ErrorEntity.Result> (resp.DataAsText);
                        //サーバー系のエラーキャッチ
                        if (err.result.error != null) {
                            PopUpWindow (err.result.error[0]);

                                if(errorCallBack != null)
                                {
                                    errorCallBack(err.result);
                                }
                            break;
                        }
                    }

                    Debug.Log("Request Finished Successfully!\n" + resp.DataAsText);

                    callback (StringLib.JsonToObject<T>(resp.DataAsText));
                    
                    _reqSuccess = true;
                    break;
                // The request finished with an unexpected error.
                // The request's Exception property may contain more information about the error.
                case HTTPRequestStates.Error:
                    Debug.LogError("Request Finished with Error! " +
                        (req.Exception != null ?
                            (req.Exception.Message + "\n" +
                                req.Exception.StackTrace) :
                            "No Exception"));
                break;
                // The request aborted, initiated by the user.
                case HTTPRequestStates.Aborted:
                     Debug.LogWarning("Request Aborted!"); break;
                // Ceonnecting to the server timed out.
                case HTTPRequestStates.ConnectionTimedOut:
                    Debug.LogError("Connection Timed Out!"); break;
                // The request didn't finished in the given time.
                case HTTPRequestStates.TimedOut:
                   Debug.LogError("Processing the request Timed Out!"); break;
                }
            });
            //HTTPMethods.Post
            // Very little time, for testing purposes:
            
            Debug.Log (url);
            WWWForm wf = new WWWForm ();
            if(reqPostDatas.Count > 0) {
                foreach ( var param in reqPostDatas) {
                    wf.AddField (param.Key, param.Value);
                }
            }
            
            request.SetFields (wf);
            request.ConnectTimeout = TimeSpan.FromMilliseconds(_connectTimeout);
            request.Timeout = TimeSpan.FromSeconds(_reqTimeOut);
            request.DisableCache = true;
            request.Send();
            return default(T);
         }
        
        /// <summary>
        /// Send the specified url, reqPostDatas, reqPostBinaryDatas and callback.
        /// </summary>
        /// <param name="url">URL.</param>
        /// <param name="reqPostDatas">Req post datas.</param>
        /// <param name="reqPostBinaryDatas">Req post binary datas.</param>
        /// <param name="callback">Callback.</param>
        /// <typeparam name="T">The 1st type parameter.</typeparam>
		public static T Send<T> ( string url, Dictionary<string, string> reqPostDatas, Dictionary<string,Texture2D> reqPostBinaryDatas,Action<T> callback)
		{
            // ネットワークの状況により処理を変更
            if(Application.internetReachability==NetworkReachability.NotReachable) {
                //ネットワークに接続不可な場合の処理
                PopUpWindow (LocalMsgConst.NETWORK_ERROR_TEXT);
                return default(T);
            }

			_reqSuccess   = false;

			var request = new HTTPRequest(new Uri(url), HTTPMethods.Post, (req, resp) =>
				{
					switch (req.State) {
					// The request finished without any problem.
					case HTTPRequestStates.Finished:
                        if (resp.DataAsText.Contains ("error") == true) {
                            var err = StringLib.JsonToObject<ErrorEntity.Result> (resp.DataAsText);
                            //サーバー系のエラーキャッチ
                            if (err.result.error != null) {
                                Debug.Log (err.result.error [0] + " Error Error Error  ");
                                PopUpWindow (err.result.error [0]);
                                break;
                            }
                        }

						Debug.Log("Request Finished Successfully!\n" + req.Response);
						callback (StringLib.JsonToObject<T>(resp.DataAsText));
						_reqSuccess = true;
						break;
						// The request finished with an unexpected error.
						// The request's Exception property may contain more information about the error.
					case HTTPRequestStates.Error:
						Debug.LogError("Request Finished with Error! " +
							(req.Exception != null ?
								(req.Exception.Message + "\n" +
									req.Exception.StackTrace) :
								"No Exception"));
						break;
						// The request aborted, initiated by the user.
					case HTTPRequestStates.Aborted:
						Debug.LogWarning("Request Aborted!"); break;
						// Ceonnecting to the server timed out.
					case HTTPRequestStates.ConnectionTimedOut:
						Debug.LogError("Connection Timed Out!"); break;
						// The request didn't finished in the given time.
					case HTTPRequestStates.TimedOut:
						Debug.LogError("Processing the request Timed Out!"); break;
					}
				});
			//HTTPMethods.Post
			// Very little time, for testing purposes:
			Debug.Log (url);


			if(reqPostDatas.Count > 0) {
				foreach ( var param in reqPostDatas) {
					Debug.Log (param.Key + " == " + param.Value);
					request.AddField(param.Key, param.Value);
					//					request.AddBinaryData ();
				}
			}


			if (reqPostBinaryDatas.Count > 0) 
			{

				foreach ( var param in reqPostBinaryDatas)
				{
					
					Texture2D expectedImage = param.Value;

					byte[] binaryData = expectedImage.EncodeToJPG();

                    request.AddBinaryData (param.Key, binaryData, param.Key, "image/jpeg");
				}
			}

			request.ConnectTimeout = TimeSpan.FromMilliseconds(_connectTimeout);
			request.Timeout = TimeSpan.FromSeconds(_reqTimeOut);
			request.DisableCache = true;
			request.Send();


			return default(T);
		}


		
        /// <summary>
        /// Sends the write board.
        /// 掲示板書き込み時にかえってくるのは成功かどうかだけなので
        /// </summary>
        /// <returns>The write board.</returns>
        /// <param name="url">URL.</param>
        /// <param name="reqPostDatas">Req post datas.</param>
        /// <param name="reqPostBinaryDatas">Req post binary datas.</param>
        /// <param name="callback">Callback.</param>
		public static string SendWriteBoard ( string url, Dictionary<string, string> reqPostDatas, Dictionary<string,Texture2D> reqPostBinaryDatas,Action<string> callback)
		{
			_reqSuccess   = false;

			var request = new HTTPRequest(new Uri(url), HTTPMethods.Post, (req, resp) =>
				{
					switch (req.State) 
					{
					// The request finished without any problem.
					case HTTPRequestStates.Finished:
						if (resp.DataAsText.Contains ("error") == true) 
						{
							var err = StringLib.JsonToObject<ErrorEntity.Result> (resp.DataAsText);
							//サーバー系のエラーキャッチ
							if (err.result.error != null) 
							{
								PopUpWindow (err.result.error [0]);
								break;
							}
						}

						callback (resp.DataAsText);
						_reqSuccess = true;
						break;
						// The request finished with an unexpected error.
						// The request's Exception property may contain more information about the error.
					case HTTPRequestStates.Error:
						Debug.LogError("Request Finished with Error! " +
							(req.Exception != null ?
								(req.Exception.Message + "\n" +
									req.Exception.StackTrace) :
								"No Exception"));
						break;
						// The request aborted, initiated by the user.
					case HTTPRequestStates.Aborted:
						Debug.LogWarning("Request Aborted!"); break;
						// Ceonnecting to the server timed out.
					case HTTPRequestStates.ConnectionTimedOut:
						Debug.LogError("Connection Timed Out!"); break;
						// The request didn't finished in the given time.
					case HTTPRequestStates.TimedOut:
						Debug.LogError("Processing the request Timed Out!"); break;
					}
				});

			if(reqPostDatas.Count > 0) {
				foreach ( var param in reqPostDatas) {
					request.AddField(param.Key, param.Value);
				}
			}

			if (reqPostBinaryDatas.Count > 0) {
				foreach ( var param in reqPostBinaryDatas) {

					Texture2D expectedImage = param.Value;
					byte[] binaryData = expectedImage.EncodeToJPG();

                    request.AddBinaryData (param.Key, binaryData, param.Key, "image/jpeg");
				}
			}

			request.ConnectTimeout = TimeSpan.FromMilliseconds(_connectTimeout);
			request.Timeout = TimeSpan.FromSeconds(_reqTimeOut);
			request.DisableCache = true;
			request.Send();
			return "";
		}
        
        /// <summary>
        /// Images the set.
        /// </summary>
        /// <param name="rawImage">Raw image.</param>
        public static void ImageSet ( string url, RawImage rawImage ) {
            var request = new HTTPRequest(new Uri(url), (req, resp) => rawImage.texture = resp.DataAsTexture2D).Send();
            request.DisableCache = true;
            request.Timeout      = TimeSpan.FromSeconds(_reqTimeOut);
        }

        #endregion
         
         
        #region ポップアップ用のアニメーション。
        private static void PopUpWindow ( string msg)
		{
            // ポイントないから課金へとべ
            if (LocalMsgConst.POINT_SHORTAGE == msg) {
                ViewController.PopupPanel.Instance.PopMessageInsert (
                    msg,
                    LocalMsgConst.OK,
                    JumpPurchaseScene
                );
            } else {
                ViewController.PopupPanel.Instance.PopMessageInsert (
                    msg,
                    LocalMsgConst.OK,
                    PopupClose
                );
            }

            PanelPopupAnimate (GameObject.FindGameObjectWithTag(CommonConstants.POPUP_BASIC_TAG)); 
        }
        
        static void PopupClose ()
		{
            ViewController.PopupPanel.Instance.PopClean();
            PanelPopupCloseAnimate (GameObject.FindGameObjectWithTag(CommonConstants.POPUP_BASIC_TAG));
        }
        
        private static void JumpPurchaseScene()
        {
            EventManager.PanelFooterButtonManager.Instance.Purchase ();
        }
        
        
        /// <summary>
        /// Res the panel animate.
        /// </summary>
        /// <param name="target">Target.</param>
        private static void PanelPopupAnimate ( GameObject target )
        {
            //ポップ用の背景セット
            if (GameObject.FindGameObjectWithTag(CommonConstants.POPUP_OVERLAY) != null) 
                GameObject.FindGameObjectWithTag(CommonConstants.POPUP_OVERLAY).SetActive (true);

            if (GameObject.FindGameObjectWithTag(CommonConstants.LOADING_OVERLAY) != null && GameObject.FindGameObjectWithTag(CommonConstants.LOADING_OVERLAY).activeSelf == true)
                GameObject.FindGameObjectWithTag(CommonConstants.LOADING_OVERLAY).SetActive (false);

            target.GetComponent<uTweenScale> ().from = Vector3.zero;
            target.GetComponent<uTweenScale> ().to = new Vector3 (1, 1 ,1 );
            target.GetComponent<uTweenScale> ().delay    = 0.001f;
            target.GetComponent<uTweenScale> ().duration = 0.25f;
            target.GetComponent<uTweenScale> ().ResetToBeginning ();
            target.GetComponent<uTweenScale> ().enabled = true;
        }

        /// <summary>
        /// Res the panel animate.
        /// </summary>
        /// <param name="target">Target.</param>
        private static void PanelPopupCloseAnimate ( GameObject target )
        {
            //ポップ用の背景外す
            if (GameObject.FindGameObjectWithTag(CommonConstants.POPUP_OVERLAY) != null) 
                GameObject.FindGameObjectWithTag(CommonConstants.POPUP_OVERLAY).SetActive (false);

            if (GameObject.FindGameObjectWithTag(CommonConstants.LOADING_OVERLAY) != null && GameObject.FindGameObjectWithTag(CommonConstants.LOADING_OVERLAY).activeSelf == true)
                GameObject.FindGameObjectWithTag(CommonConstants.LOADING_OVERLAY).SetActive (false);

            target.GetComponent<uTweenScale> ().delay    = 0.001f;
            target.GetComponent<uTweenScale> ().duration = 0.15f;
            target.GetComponent<uTweenScale> ().from = new Vector3 (1f, 1f ,1f );
            target.GetComponent<uTweenScale> ().to = Vector3.zero;
            target.GetComponent<uTweenScale> ().ResetToBeginning ();
            target.GetComponent<uTweenScale> ().enabled = true;
        }
        #endregion
	}
}