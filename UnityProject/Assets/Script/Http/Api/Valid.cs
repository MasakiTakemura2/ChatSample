using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Http {
    public class Valid
    {
        #region member variable
        public static bool _success = false;
        public static EazyReturnDataEntity.ResultUrl _httpCatchData;
        #endregion

        #region Construct
        /// <summary>
        /// Initializes a new instance of the <see cref="Http.Valid"/> class.
        /// サーバーでキャッチしてサーバーで広告や、データ解析系の処理を調整する用のApi。
        /// </summary>
        public Valid (string userKey) 
        {
            //Ready Proccesing
            _success = false;

            //post parameter Set
            var postDatas = new Dictionary<string, string>();
            postDatas.Add (HttpConstants.USER_KEY, userKey);
            postDatas.Add (HttpConstants.PLATFORM_ID_NAME, DeviceService.GetPlatformId ());
            postDatas.Add (HttpConstants.API_VERSION_NAME, DeviceService.GetAppVersion ());
            //TODO: 「_bundle」<- TESTです。DomainData._bundle ※適宜変更
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
            string url = DomainData.GetApiUrl(DomainData.VALID);
            HttpHandler.Send<EazyReturnDataEntity.ResultUrl> (url, postDatas, CallBack);
        }

        /// <summary>
        /// Calls the back.
        /// </summary>
        /// <param name="result">Result.</param>
        private void CallBack (EazyReturnDataEntity.ResultUrl result) 
        {
            _success = (result != null);

            if (_success == true)
                _httpCatchData = result;
        }
        #endregion
    }
}