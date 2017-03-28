using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Http {
    public class SetDeviceToken
    {
        #region member variable
        public static bool _success = false;
        public static EazyReturnDataEntity.Result _httpCatchData;
        #endregion

        #region Construct
        public SetDeviceToken (string userKey, string deviceToken) 
        {
            //Ready Proccesing
            _success = false;

            //post parameter Set
            var postDatas = new Dictionary<string, string>();
            postDatas.Add (HttpConstants.UIID_NAME, NativeRecieveManager.GetUiid (DomainData._bundle));
            postDatas.Add (HttpConstants.DEVICE_TOKEN, deviceToken);
            postDatas.Add (HttpConstants.API_VERSION_NAME, DeviceService.GetAppVersion ());
            postDatas.Add (HttpConstants.USER_KEY, userKey);

Debug.Log (HttpConstants.API_VERSION_NAME + " == " + DeviceService.GetAppVersion ());
Debug.Log (HttpConstants.USER_KEY         + " == " + userKey);
Debug.Log(HttpConstants.UIID_NAME         + " == " + NativeRecieveManager.GetUiid (DomainData._bundle));
Debug.Log (HttpConstants.DEVICE_TOKEN     + " ==  " + deviceToken);


            //TODO: 「_bundle」<- TESTです。DomainData._bundle ※適宜変更
            Request (postDatas);
        }
        #endregion

        #region Request Send Prossesing
        /// <summary>
        /// Request this instance.
        /// </summary>
        private void Request (Dictionary<string, string> postDatas)
        {
            string url = DomainData.GetApiUrl(DomainData.SET_DEVICE_TOKEN);
            HttpHandler.Send<EazyReturnDataEntity.Result> (url, postDatas, CallBack);   
        }

        /// <summary>
        /// Calls the back.
        /// </summary>
        /// <param name="result">Result.</param>
        private void CallBack (EazyReturnDataEntity.Result result) 
        {
            _success = (result != null);
Debug.Log (result.result + " Set Device Token Set Device Token  Success  ");
            if (_success == true)
                _httpCatchData = result;
        }
        #endregion
    }
}