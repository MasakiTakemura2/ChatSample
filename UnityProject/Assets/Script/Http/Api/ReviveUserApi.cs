using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Http {
    public class ReviveUserApi
    {
        #region member variable
        public static bool _success = false;
        #endregion

        #region Construct
        public ReviveUserApi ( ) 
        {
            //Ready Proccesing
            _success = false;

            //post parameter Set
            var postDatas = new Dictionary<string, string>();
            postDatas.Add (HttpConstants.USER_KEY, AppStartLoadBalanceManager._userKey);
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
            string url = DomainData.GetApiUrl(DomainData.REVIVE_USER);
            HttpHandler.Send<UserDataEntity.Result> (url, postDatas, CallBack);
        }

        /// <summary>
        /// Calls the back.
        /// </summary>
        /// <param name="result">Result.</param>
        private void CallBack (UserDataEntity.Result result) 
        {
            _success = (result != null);
            if (_success == true) {
                GetUserApi._httpCatchData = result;
            }
        }
        #endregion
    }
}