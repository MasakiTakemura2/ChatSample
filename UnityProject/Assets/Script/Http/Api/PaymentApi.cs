using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Http {
    /// <summary>
    /// Payment API.
    /// </summary>
    public class PaymentApi
    {
        #region member variable
        public static bool _success = false;
        public static EazyReturnDataEntity.PaymentResult _httpCatchData;
        #endregion

        #region Construct
        public PaymentApi (string receipt) 
        {
            //Ready Proccesing
            _success = false;

            //post parameter Set
            var postDatas = new Dictionary<string, string>();
            postDatas.Add (HttpConstants.USER_KEY, AppStartLoadBalanceManager._userKey);
            postDatas.Add (HttpConstants.RECEIPT, receipt);
            postDatas.Add (HttpConstants.AUTO_RENEWBLE, "1");            
            postDatas.Add (HttpConstants.API_VERSION_NAME, DeviceService.GetAppVersion());
            postDatas.Add (HttpConstants.PLATFORM_ID_NAME, DeviceService.GetPlatformId ());
            

            Request (postDatas);
        }
        #endregion

        #region Request Send Prossesing
        /// <summary>
        /// Request this instance.
        /// </summary>
        private void Request (Dictionary<string,string> postDatas)
        {
            string url = DomainData.GetApiUrl(DomainData.PAYMENT);
            HttpHandler.Send<EazyReturnDataEntity.PaymentResult> (url, postDatas, CallBack,errorCallBack);
        }

        /// <summary>
        /// Calls the back.
        /// </summary>
        /// <param name="result">Result.</param>
        private void CallBack (EazyReturnDataEntity.PaymentResult result) 
        {
            _success = (result != null);

            if (_success == true) {
                _httpCatchData = result;
            }
        }

        /// <summary>
		/// Errors the call back.
		/// </summary>
		private static void errorCallBack (Http.ErrorEntity.Error  error) {
			_success = true;			
			if (_success == true) {
				_httpCatchData = null;
			}
		}
        #endregion
    }
}