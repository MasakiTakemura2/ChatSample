using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Http {
    /// <summary>
    /// Message list API.
    /// </summary>
    public class MessageListApi
    {
        #region member variable
        public static bool _success = false;
        public static MessageListEntity.Result _httpCatchData;
        #endregion

        #region Construct
		public MessageListApi (string toUserId = "1", string beforeid="", string afterid="") 
        {
            //Ready Proccesing
            _success = false;

            //post parameter Set
            var postDatas = new Dictionary<string, string>();
            postDatas.Add (HttpConstants.USER_KEY, AppStartLoadBalanceManager._userKey);
            postDatas.Add (HttpConstants.TO_USER_ID, toUserId);
			postDatas.Add (HttpConstants.BEFORE_MESSAGE_ID , beforeid);
			postDatas.Add (HttpConstants.AFTER_MESSAGE_ID  , afterid);
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
            string url = DomainData.GetApiUrl(DomainData.MESSAGE_LIST);
            Debug.Log (url);
            HttpHandler.Send<MessageListEntity.Result> (url, postDatas, CallBack);
        }

        /// <summary>
        /// Calls the back.
        /// </summary>
        /// <param name="result">Result.</param>
        private void CallBack (MessageListEntity.Result result) 
        {
            _success = (result != null);
            if (_success == true) {
                _httpCatchData = result;
            }
        }
        #endregion
    }
}