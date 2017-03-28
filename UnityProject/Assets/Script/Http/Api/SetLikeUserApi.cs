using UnityEngine;
using System.Collections.Generic;
using Helper;

namespace Http
{
	public class SetLikeUserApi
	{

		#region member variable
		public static bool _success = false;
		public static SetLikeUserEntity.Result _httpCatchData; // ユーザーリストとエンティティ共用
		#endregion


		#region Construct

        /// <summary>
        /// Initializes a new instance of the <see cref="Http.SetLikeUserApi"/> class.
        /// </summary>
        /// <param name="toUserID">To user I.</param>
        /// <param name="isSuper">Is super.</param>
		public SetLikeUserApi(string toUserID, string isSuper)
		{
			_success = false;
			Dictionary<string,string> postDatas = new Dictionary<string,string> ();

			postDatas.Add (HttpConstants.USER_KEY,AppStartLoadBalanceManager._userKey);
			postDatas.Add (HttpConstants.TO_USER_ID, toUserID);
			postDatas.Add (HttpConstants.IS_SUPER, isSuper);

			postDatas.Add (HttpConstants.API_VERSION_NAME,DeviceService.GetAppVersion());

			Request (postDatas);
			
		}
		#endregion

		#region Request Send Processing

        /// <summary>
        /// Request the specified postDatas.
        /// </summary>
        /// <param name="postDatas">Post datas.</param>
		private void Request (Dictionary<string,string> postDatas)
		{
			string url = DomainData.GetApiUrl (DomainData.SET_USER_LIKE);
			HttpHandler.Send<SetLikeUserEntity.Result> (url,postDatas,CallBack);
		}	


        /// <summary>
        /// Calls the back.
        /// </summary>
        /// <param name="result">Result.</param>
		private void CallBack (SetLikeUserEntity.Result result)
		{
	
			_success = (result!=null);

			if (_success == true) 
			{
				_httpCatchData = result;
			}         
		}

		#endregion
	}

}
