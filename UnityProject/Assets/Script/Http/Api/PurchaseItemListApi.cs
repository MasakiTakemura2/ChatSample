using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Http {
    public class PurchaseItemListApi
    {
        #region member variable
        public static bool _success = false;
        public static PurchaseItemListEntity.Result _httpCatchData;
        #endregion

        #region Construct
        public static void Init () 
        {
            //Ready Proccesing
            _success = false;

            //post parameter Set
            var postDatas = new Dictionary<string, string>();
            postDatas.Add (HttpConstants.API_VERSION_NAME, DeviceService.GetAppVersion());
            postDatas.Add (HttpConstants.USER_KEY, AppStartLoadBalanceManager._userKey);
            Request (postDatas);
        }
        #endregion

        #region Request Send Prossesing
        /// <summary>
        /// Request this instance.
        /// </summary>
        private static void Request (Dictionary<string, string> postDatas)
        {
            string url = DomainData.GetApiUrl(DomainData.PURCHASE_ITEMLIST);
            HttpHandler.Send<PurchaseItemListEntity.Result> (url, postDatas, CallBack);
        }

        /// <summary>
        /// Calls the back.
        /// </summary>
        /// <param name="result">Result.</param>
        private static void CallBack (PurchaseItemListEntity.Result result) 
        {
            _success = (result != null);

            if (_success == true) {
                _httpCatchData = result;
            }
        }
        #endregion
    }
}