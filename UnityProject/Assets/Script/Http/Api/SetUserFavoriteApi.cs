using UnityEngine;
using System.Collections;
using System.Collections.Generic;

using EventManager;
using UnityEngine.UI;
using Http;
using UnityEngine.SceneManagement;
using uTools;
using ViewController;


namespace Http {
    /// <summary>
    /// Message list API.
    /// </summary>
    public class SetUserFavoriteApi
    {
        #region member variable
        public static bool _success = false;
        public static EazyReturnDataEntity.ResultFavorite _httpCatchData;
        #endregion

        #region Construct
        public SetUserFavoriteApi (string toUserId = "1") 
        {
            //Ready Proccesing
            _success = false;

            //post parameter Set
            var postDatas = new Dictionary<string, string>();
            postDatas.Add (HttpConstants.USER_KEY, AppStartLoadBalanceManager._userKey);
            postDatas.Add (HttpConstants.API_VERSION_NAME, DeviceService.GetAppVersion());
            postDatas.Add (HttpConstants.TO_USER_ID, toUserId);
            
            Request (postDatas);
        }
        #endregion

        #region Request Send Prossesing
        /// <summary>
        /// Request this instance.
        /// </summary>
        private void Request (Dictionary<string,string> postDatas)
        {
            string url = DomainData.GetApiUrl(DomainData.SET_USER_FAVORITE);
			HttpHandler.Send<EazyReturnDataEntity.ResultFavorite> (url, postDatas, CallBack, CallBackError);
        }

        /// <summary>
        /// Calls the back.
        /// </summary>
        /// <param name="result">Result.</param>
        private void CallBack (EazyReturnDataEntity.ResultFavorite result) 
        {
            _success = (result != null);
			Debug.Log (result.result.is_favorite + " == trueならお気に入り登録完了。");

            if (_success == true) {
                _httpCatchData = result;
            }
        }


		private void CallBackError (Http.ErrorEntity.Error result) 
		{
			// ポイントないから課金へとべ
			if(LocalMsgConst.POINT_SHORTAGE == result.error[0])
			{
				PopupPanel.Instance.PopMessageInsert
				(
					result.error[0],
					LocalMsgConst.OK,
					JumpPurchaseScene
				);

				PanelPopupAnimate (GameObject.FindGameObjectWithTag(CommonConstants.POPUP_BASIC_TAG));
			}
		}

		private void JumpPurchaseScene()
		{
			EventManager.PanelFooterButtonManager.Instance.Purchase ();
		}
		private void PanelPopupAnimate ( GameObject target )
		{
			//ポップ用の背景セット
			target.GetComponent<uTweenScale> ().from = Vector3.zero;
			target.GetComponent<uTweenScale> ().to = new Vector3 (1, 1 ,1 );
			target.GetComponent<uTweenScale> ().delay    = 0.001f;
			target.GetComponent<uTweenScale> ().duration = 0.25f;
			target.GetComponent<uTweenScale> ().ResetToBeginning ();
			target.GetComponent<uTweenScale> ().enabled = true;
		}


        #endregion
    }
}