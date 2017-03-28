using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Http {
    /// <summary>
    /// Message list API.
    /// </summary>
    public class MessageUserListApi
    {
        #region member variable
        public static bool _success = false;
        public static MessageUserListEntity.Result _httpCatchData;
        #endregion
 
        #region Construct
        /// <summary>
        /// Initializes a new instance of the <see cref="T:Http.MessageUserListApi"/> class.
        /// </summary>
		public MessageUserListApi (string displayType) 
        {
            //Ready Proccesing
            _success = false;
            
			//post parameter Set
            var postDatas = new Dictionary<string, string>();
            postDatas.Add (HttpConstants.USER_KEY, AppStartLoadBalanceManager._userKey);
			postDatas.Add (HttpConstants.DISPLAY_TYPE, displayType);
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
            string url = DomainData.GetApiUrl(DomainData.MESSAGE_USER_LIST);
            HttpHandler.Send<MessageUserListEntity.Result> (url, postDatas, CallBack);
        }

        /// <summary>
        /// Calls the back.
        /// </summary>
        /// <param name="result">Result.</param>
        private void CallBack (MessageUserListEntity.Result result) 
        {
            _success = (result != null);
            if (_success == true) {
                _httpCatchData = result;
            }
        }
        #endregion
    }
}