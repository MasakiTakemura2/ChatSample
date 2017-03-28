using UnityEngine;
using System.Collections.Generic;
using Helper;

namespace Http
{
	public class HelpListApi
	{

		#region member variable
		public static bool _success = false;
		public static HelpListEntity.Result _httpCatchData;
		#endregion


		#region Construct

		public HelpListApi()
		{
			_success = false;
			Dictionary<string,string> postDatas = new Dictionary<string,string> ();

			postDatas.Add (HttpConstants.USER_KEY, AppStartLoadBalanceManager._userKey);
			postDatas.Add (HttpConstants.API_VERSION_NAME, DeviceService.GetAppVersion());

			Request (postDatas);
		}
		#endregion

		#region Request Send Processing

		private void Request (Dictionary<string,string> postDatas)
		{
			string url = DomainData.GetApiUrl (DomainData.HELP_LIST);
			HttpHandler.Send<HelpListEntity.Result> (url, postDatas, CallBack);
		}	


		private void CallBack (HelpListEntity.Result result)
		{
			_success = (result!=null);

			if (_success == true) 
			{
				_httpCatchData = result;
			} else {
				Debug.Log ("Entity null....");
			}

		}

		#endregion
	}

}
