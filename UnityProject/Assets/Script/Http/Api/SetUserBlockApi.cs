﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Http {
    /// <summary>
    /// Message list API.
    /// </summary>
    public class SetUserBlockApi
    {
        #region member variable
        public static bool _success = false;
        public static EazyReturnDataEntity.ResultUserBlock _httpCatchData;
        #endregion

        #region Construct
        public SetUserBlockApi (string toUserId = "1") 
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
            string url = DomainData.GetApiUrl(DomainData.SET_USER_BLOCK);
            HttpHandler.Send<EazyReturnDataEntity.ResultUserBlock> (url, postDatas, CallBack);
        }

        /// <summary>
        /// Calls the back.
        /// </summary>
        /// <param name="result">Result.</param>
        private void CallBack (EazyReturnDataEntity.ResultUserBlock result) 
        {
            _success = (result != null);

            if (_success == true) {
                _httpCatchData = result;
            }
        }
        #endregion
    }
}