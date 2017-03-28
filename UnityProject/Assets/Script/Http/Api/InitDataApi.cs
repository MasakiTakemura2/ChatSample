using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Http {
    public class InitDataApi
    {
        #region member variable
        public static bool _success = false;
        public static InitDataEntity.Result _httpCatchData;
        #endregion

        #region Construct
        public InitDataApi () 
        {
            //Ready Proccesing
            _success = false;

            //post parameter Set
            var postDatas = new Dictionary<string, string>();
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
            string url = DomainData.GetApiUrl(DomainData.GET_INIT_DATA);
            HttpHandler.Send<InitDataEntity.Result> (url, postDatas, CallBack);
        }

        /// <summary>
        /// Calls the back.
        /// </summary>
        /// <param name="result">Result.</param>
        private void CallBack (InitDataEntity.Result result) 
        {
            _success = (result != null);
//Debug.Log (result.result.pref.prefsData.Count + " ====== ");

            if (_success == true) {
                _httpCatchData = result;
            }
        }
        #endregion
    }
}