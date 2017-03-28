using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Http {
    /// <summary>
    /// Message list API.
    /// </summary>
    public class SendRandomMessageApi
    {
        #region member variable
        public static bool _success = false;
        public static EazyReturnDataEntity.ResultComplete _httpCatchData;
        #endregion

        #region Construct
        public SendRandomMessageApi (string message, string sex_cd = "", string radius = "") 
        {
            //Ready Proccesing
            _success = false;

            //post parameter Set
            var postDatas = new Dictionary<string, string>();
            postDatas.Add (HttpConstants.USER_KEY, AppStartLoadBalanceManager._userKey);
            postDatas.Add (HttpConstants.API_VERSION_NAME, DeviceService.GetAppVersion());
            postDatas.Add (HttpConstants.SEX_CD, sex_cd);
            postDatas.Add (HttpConstants.RADIUS, radius);
            postDatas.Add (HttpConstants.MESSAGE, message);
            Request (postDatas);
        }
        #endregion

        #region Request Send Prossesing
        /// <summary>
        /// Request this instance.
        /// </summary>
        private void Request (Dictionary<string,string> postDatas)
        {
            string url = DomainData.GetApiUrl(DomainData.SEND_RANDOM_MESSAGE);
            HttpHandler.Send<EazyReturnDataEntity.ResultComplete> (url, postDatas, CallBack);
        }

        /// <summary>
        /// Calls the back.
        /// </summary>
        /// <param name="result">Result.</param>
        private void CallBack (EazyReturnDataEntity.ResultComplete result) 
        {
            _success = (result != null);

            if (_success == true) {
                _httpCatchData = result;
            }
        }
        #endregion
    }
}