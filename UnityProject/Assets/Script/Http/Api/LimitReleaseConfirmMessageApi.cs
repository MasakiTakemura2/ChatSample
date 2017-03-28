using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Http {
    /// <summary>
    /// Limit release confirm message.
    /// </summary>
    public class LimitReleaseConfirmMessage
    {
        #region member variable
        public static bool _success = false;
		public static EazyReturnDataEntity.LimitMessageResult _httpCatchData;
        #endregion

        #region Construct
        public LimitReleaseConfirmMessage ( string _toUserId ) 
        {
            //Ready Proccesing
            _success = false;

            //post parameter Set
            var postDatas = new Dictionary<string, string>();
            postDatas.Add (HttpConstants.USER_KEY, AppStartLoadBalanceManager._userKey);
            postDatas.Add (HttpConstants.TO_USER_ID, _toUserId);
            
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
            string url = DomainData.GetApiUrl(DomainData.LIMIT_RELASE_CONFIRM_MESSAGE);
			HttpHandler.Send<EazyReturnDataEntity.LimitMessageResult> (url, postDatas, CallBack);
        }

        /// <summary>
        /// Calls the back.
        /// </summary>
        /// <param name="result">Result.</param>
		private void CallBack (EazyReturnDataEntity.LimitMessageResult result) 
        {
            _success = (result != null);

            if (_success == true) {
                _httpCatchData = result;
            }
        }
        #endregion
    }
}