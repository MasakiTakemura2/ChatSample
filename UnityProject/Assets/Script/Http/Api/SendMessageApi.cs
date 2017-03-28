using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Http {
    /// <summary>
    /// Message list API.
    /// </summary>
    public class SendMessageApi
    {
        #region member variable
        public static bool _success = false;
        public static EazyReturnDataEntity.Result _httpCatchData;
        #endregion

        #region Construct
        public SendMessageApi (string toUserId = "1", string message = "", Texture2D sendImage = null) 
        {
            //Ready Proccesing
            _success = false;

            //post parameter Set
            var postDatas = new Dictionary<string, string>();
            Dictionary<string,Texture2D> postBinaryDatas = new Dictionary<string,Texture2D> ();
            
            postDatas.Add (HttpConstants.USER_KEY, AppStartLoadBalanceManager._userKey);
            postDatas.Add (HttpConstants.API_VERSION_NAME, DeviceService.GetAppVersion());
            postDatas.Add (HttpConstants.TO_USER_ID, toUserId);
            postDatas.Add (HttpConstants.MESSAGE, message);

            if (sendImage != null)
                postBinaryDatas.Add (HttpConstants.IMAGE, sendImage);

            //TODO: movie対応。
            postDatas.Add (HttpConstants.MOVIE, "");
            
            Request (postDatas, postBinaryDatas);
        }
        #endregion

        #region Request Send Prossesing
        /// <summary>
        /// Request this instance.
        /// </summary>
        
        private void Request (Dictionary<string,string> postDatas, Dictionary<string,Texture2D> postBinaryDatas)
        {
            string url = DomainData.GetApiUrl(DomainData.SEND_MESSAGE);
            Debug.Log (url);
            HttpHandler.Send<EazyReturnDataEntity.Result> (url, postDatas, postBinaryDatas, CallBack);
            
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