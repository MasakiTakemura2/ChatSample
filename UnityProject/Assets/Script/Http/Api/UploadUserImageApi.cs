using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Http {
    /// <summary>
    /// Message list API.
    /// </summary>
    public class UploadUserImageApi
    {
        #region member variable
        public static bool _success = false;
        public static UserDataEntity.Result _httpCatchData;
        #endregion

        #region Construct
        public UploadUserImageApi (Texture2D coverImage = null, Texture2D profImage = null) 
        {
            //Ready Proccesing
            _success = false;

            //post parameter Set
            var postDatas       = new Dictionary<string, string>();
            var postBinaryDatas = new Dictionary<string,Texture2D> ();
            
            postDatas.Add (HttpConstants.USER_KEY, AppStartLoadBalanceManager._userKey);
            postDatas.Add (HttpConstants.API_VERSION_NAME, DeviceService.GetAppVersion());
            
            if (profImage != null) {
                //TextureScale.Point(profImage, (profImage.width / 2), (profImage.height / 2));
                postBinaryDatas.Add (HttpConstants.PROFILE_IMAGE, profImage);
            }

            if (coverImage != null) {
                //TextureScale.Point(coverImage, (coverImage.width / 2), (coverImage.height / 2));
                postBinaryDatas.Add (HttpConstants.COVER_IMAGE, coverImage);
            }
            
            Request (postDatas, postBinaryDatas);
        }
        #endregion

        #region Request Send Prossesing
        /// <summary>
        /// Request this instance.
        /// </summary>
        
        private void Request (Dictionary<string,string> postDatas, Dictionary<string,Texture2D> postBinaryDatas)
        {
            string url = DomainData.GetApiUrl(DomainData.UPLOAD_USER_IMAGE);
            Debug.Log (url);
            HttpHandler.Send<UserDataEntity.Result> (url, postDatas, postBinaryDatas, CallBack);
            
        }

        /// <summary>
        /// Calls the back.
        /// </summary>
        /// <param name="result">Result.</param>
        private void CallBack (UserDataEntity.Result result) 
        {
            _success = (result != null);

Debug.Log (result.result + " === trueならメッセージ画像のサーバー送信完了。");

            if (_success == true) {
                _httpCatchData = result;
            }
        }
        #endregion
    }
}