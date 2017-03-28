using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Http {
    /// <summary>
    /// Rewind user API.
    /// </summary>
    public class ReadMessageApi
    {
        #region member variable
        public static bool _success = false;
        public static EazyReturnDataEntity.Result _httpCatchData;
        #endregion

        #region Construct
        public ReadMessageApi (string messageId) 
        {
            //Ready Proccesing
            _success = false;

            //post parameter Set
            var postDatas = new Dictionary<string, string>();
            postDatas.Add (HttpConstants.USER_KEY, AppStartLoadBalanceManager._userKey);
            postDatas.Add (HttpConstants.MESSAGE_ID, messageId);
            postDatas.Add (HttpConstants.API_VERSION_NAME, DeviceService.GetAppVersion());
            Request (postDatas);
        }
        #endregion

        #region Request Send Prossesing
        /// <summary>
        /// Request this instance.
        /// </summary>
        private void Request (Dictionary<string, string> postDatas)
        {
            string url = DomainData.GetApiUrl(DomainData.READ_MESSAGE);
            HttpHandler.Send<EazyReturnDataEntity.Result> (url, postDatas, CallBack);
        }

        /// <summary>
        /// Calls the back.
        /// </summary>
        /// <param name="result">Result.</param>
        private void CallBack (EazyReturnDataEntity.Result result) 
        {
            _success = (result != null);

            if (_success == true) {
                _httpCatchData = result;
            }
        }
        #endregion
    }
}