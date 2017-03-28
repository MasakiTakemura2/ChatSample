using UnityEngine;
using System.Collections.Generic;
using Helper;

namespace Http
{
	public class HistoryUserListApi
	{

		#region member variable
		public static bool _success = false;
		public static HistoryUserListEntity.Result _httpCatchData;
		#endregion


		#region Construct
        /// <summary>
        /// Initializes a new instance of the <see cref="T:Http.HistoryUserListApi"/> class.
        /// </summary>
        /// <param name="historyType">History type.</param>
        /// <param name="page">Page.</param>
		public HistoryUserListApi(string historyType, string page){
			//Ready processing 
			_success = false;
			Dictionary<string,string> postDatas = new Dictionary<string,string> ();
			postDatas.Add (HttpConstants.USER_KEY, AppStartLoadBalanceManager._userKey);
            postDatas.Add (HttpConstants.API_VERSION_NAME, DeviceService.GetAppVersion());
            postDatas.Add (HttpConstants.HISTORY_TYPE, historyType);
            postDatas.Add (HttpConstants.PAGE_NUBER, page);
            
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
			string url = DomainData.GetApiUrl (DomainData.HISTORY_USER_LIST);
			HttpHandler.Send<HistoryUserListEntity.Result> (url,postDatas,CallBack);
		}	

        /// <summary>
        /// Calls the back.
        /// </summary>
        /// <returns>The back.</returns>
        /// <param name="result">Result.</param>
		private void CallBack (HistoryUserListEntity.Result result)
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
