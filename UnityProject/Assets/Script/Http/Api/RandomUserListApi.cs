using UnityEngine;
using System.Collections.Generic;
using Helper;

namespace Http
{
	public class RandomUserListApi
	{

		#region member variable
		public static bool _success = false;
		public static UserListEntity.Result _httpCatchData; // ユーザーリストとエンティティ共用
		#endregion


		#region Construct

		public RandomUserListApi(string page)
		{
			_success = false;
			Dictionary<string,string> postDatas = new Dictionary<string,string> ();

			postDatas.Add (HttpConstants.USER_KEY,AppStartLoadBalanceManager._userKey);
			postDatas.Add (HttpConstants.PAGE_NUBER, page);
			postDatas.Add (HttpConstants.API_VERSION_NAME,DeviceService.GetAppVersion());

			Request (postDatas);
			
		}
		#endregion

		#region Request Send Processing

		private void Request (Dictionary<string,string> postDatas)
		{
			string url = DomainData.GetApiUrl (DomainData.RANDOM_USER_LIST);
			HttpHandler.Send<UserListEntity.Result> (url,postDatas,CallBack);
		}	


		private void CallBack (UserListEntity.Result result)
		{
	
			_success = (result!=null);

			if (_success == true) 
			{
				_httpCatchData = result;
			} else {
				Debug.Log ("userlistEntity null....");
			}

		}

		#endregion
	}

}
