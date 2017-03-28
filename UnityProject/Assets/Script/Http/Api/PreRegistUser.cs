using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Helper;
using System;


namespace Http
{
    public class PreRegistUser
    {
        #region member variable
        public static bool _success = false;
        public static UserDataEntity.Result _httpCatchData;
        #endregion

        #region Construct
        public PreRegistUser () 
        {
            //Ready Proccesing
            _success = false;
            _httpCatchData = null;
            Dictionary<string, string> postDatas = new Dictionary<string, string> ();

#if !UNITY_EDITOR && UNITY_IOS
            postDatas.Add (HttpConstants.UIID_NAME, NativeRecieveManager.GetUiid (DomainData._bundle));

            if (DomainData._bundle == DomainData._parentBundle) {
                postDatas.Add (HttpConstants.UDID_NAME, NativeRecieveManager.GetUdid ()); 
                postDatas.Add (HttpConstants.IDFV_NAME, NativeRecieveManager.GetIdfv ());
            } else {
                postDatas.Add (HttpConstants.IDFV_NAME, NativeRecieveManager.GetUdid ());
                postDatas.Add (HttpConstants.UDID_NAME, NativeRecieveManager.GetIdfv ());                
            }
#else
            postDatas.Add (HttpConstants.UIID_NAME, NativeRecieveManager.GetUiid (DomainData._bundle));
            postDatas.Add (HttpConstants.UDID_NAME, NativeRecieveManager.GetUdid ()); 
            postDatas.Add (HttpConstants.IDFV_NAME, NativeRecieveManager.GetIdfv ());            
#endif
            

            postDatas.Add (HttpConstants.PLATFORM_ID_NAME, DeviceService.GetPlatformId ());
            postDatas.Add (HttpConstants.API_VERSION_NAME, DeviceService.GetAppVersion ());
            postDatas.Add (HttpConstants.BUNDLE, DomainData._bundle);

            Request (postDatas);
        }
        #endregion

        #region Request Send Prossesing
        /// <summary>
        /// Request this instance.
        /// </summary>
        private void Request (Dictionary<string, string> postDatas)
        {
            string url = DomainData.GetApiUrl(DomainData.PRE_REGIST_USER);
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
                _httpCatchData = result;
            }
        }
        #endregion
    }
}
        