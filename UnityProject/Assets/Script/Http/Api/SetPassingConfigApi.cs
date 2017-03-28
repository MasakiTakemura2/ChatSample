using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Http {
    public class SetPassingConfigApi
    {
        #region member variable
        public static bool _success = false;
        public static EazyReturnDataEntity.ResultComplete _httpCatchData;
        #endregion

        #region Construct
        public SetPassingConfigApi (SetPassingConfigPostEntity.Post post) 
        {
            //Ready Proccesing
            _success = false;

            //post parameter Set
            var postDatas = new Dictionary<string, string>();
            postDatas.Add (HttpConstants.USER_KEY, AppStartLoadBalanceManager._userKey);
            postDatas.Add (HttpConstants.IS_PASSING      , post.isPassing);
            postDatas.Add (HttpConstants.IS_NOTIFICATION , post.isNotification);
            postDatas.Add (HttpConstants.SEX_CD          , post.sexCd);
            postDatas.Add (HttpConstants.AGE_FROM        , post.ageFrom);
            postDatas.Add (HttpConstants.AGE_TO          , post.ageTo);
            postDatas.Add (HttpConstants.HEIGHT_FROM     , post.heightFrom);
            postDatas.Add (HttpConstants.HEIGHT_TO       , post.heightTo);
            postDatas.Add (HttpConstants.BODY_TYPE       , post.bodyType);
            postDatas.Add (HttpConstants.IS_IMAGE        , post.isImage);
            postDatas.Add (HttpConstants.RADIUS          , post.radius);
            postDatas.Add (HttpConstants.KEYWORD         , post.keyword);
            postDatas.Add (HttpConstants.IS_SEND_MESSAGE , post.isSendMessage);
            postDatas.Add (HttpConstants.MESSAGE         , post.message);
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
            string url = DomainData.GetApiUrl(DomainData.SET_PASSING_CONFIG);
            Debug.Log (url);
            HttpHandler.Send<EazyReturnDataEntity.ResultComplete> (url, postDatas, CallBack);
        }

        /// <summary>
        /// Calls the back.
        /// </summary>
        /// <param name="result">Result.</param>
        private void CallBack (EazyReturnDataEntity.ResultComplete result)
        {
            _success = (result != null);
            //こっちのデータも更新しておく。

            if (_success == true) {
                _httpCatchData = result;
            }
        }
        #endregion
    }
}