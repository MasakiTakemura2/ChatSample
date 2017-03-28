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
    /// Limit release API.
    /// </summary>
    public class LimitReleaseApi
    {
        #region member variable
        public static bool _success = false;
		public static EazyReturnDataEntity.LimitMessageDecisionResult _httpCatchData;
        #endregion

        #region Construct
        public LimitReleaseApi (string _toUSerId) 
        {
            //Ready Proccesing
            _success = false;

            //post parameter Set
            var postDatas = new Dictionary<string, string>();
            postDatas.Add (HttpConstants.USER_KEY, AppStartLoadBalanceManager._userKey);
            postDatas.Add (HttpConstants.TO_USER_ID, _toUSerId);
            postDatas.Add (HttpConstants.API_VERSION_NAME, DeviceService.GetAppVersion());

            Request (postDatas);
        }
        #endregion

        #region Request Send Prossesing
        /// <summary>
        /// Request this instance.
        /// </summary>
        private void Request (Dictionary<string,string> postDatas)
        {
            string url = DomainData.GetApiUrl(DomainData.LIMIT_RELEASE);
			HttpHandler.Send<EazyReturnDataEntity.LimitMessageDecisionResult> (url, postDatas, CallBack, CallBackError);
        }

        /// <summary>
        /// Calls the back.
        /// </summary>
        /// <param name="result">Result.</param>
		private void CallBack (EazyReturnDataEntity.LimitMessageDecisionResult result) 
        {
            _success = (result != null);
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