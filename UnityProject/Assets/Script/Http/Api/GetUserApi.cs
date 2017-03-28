using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Http {
    public class GetUserApi
    {
        #region member variable
        public static bool _success = false;
        public static UserDataEntity.Result _httpCatchData;
        public static UserDataEntity.Basic _httpOtherUserCatchData;
        private string _toUserId;
        #endregion

        #region Construct
        public GetUserApi ( string toUserId = "" ) 
        {
            //Ready Proccesing
            _success = false;
            _toUserId = toUserId;

            //post parameter Set
            var postDatas = new Dictionary<string, string>();
            postDatas.Add (HttpConstants.USER_KEY, AppStartLoadBalanceManager._userKey);
            postDatas.Add (HttpConstants.TO_USER_ID, toUserId);
            postDatas.Add (HttpConstants.API_VERSION_NAME, DeviceService.GetAppVersion());
            postDatas.Add (HttpConstants.PLATFORM_ID_NAME, DeviceService.GetPlatformId ());
            Request (postDatas);
        }
        #endregion

        #region Request Send Prossesing
        /// <summary>
        /// Request this instance.
        /// </summary>
        private void Request (Dictionary<string, string> postDatas)
        {
            string url = DomainData.GetApiUrl(DomainData.GET_USER);
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
                if (_toUserId != "" ) {
                    _httpOtherUserCatchData = result.result.user;
                } else {
                    _httpCatchData = result;
                }
                
            }
        }
        #endregion
    }
}