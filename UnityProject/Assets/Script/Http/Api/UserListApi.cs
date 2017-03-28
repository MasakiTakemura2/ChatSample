using UnityEngine;
using System.Collections.Generic;
using Helper;

namespace Http
{
	public class UserListApi
	{

		#region member variable
		public static bool _success = false;
		public static UserListEntity.Result _httpCatchData;
		#endregion


		#region Construct

		public UserListApi(
			string userKey,
			string lat,
			string lng,
			string order,
			string sex,
			string agefrom,
			string ageto,
			string heightfrom,
			string heightto,
			string bodytype,
			string isimage,
			string radius,
			string keyword,
			string page
		){
			_success = false;
			Dictionary<string,string> postDatas = new Dictionary<string,string> ();

			postDatas.Add (HttpConstants.USER_KEY,userKey);
			postDatas.Add (HttpConstants.LAT, lat);
			postDatas.Add (HttpConstants.LNG, lng);
         
            if (string.IsNullOrEmpty (order)  == true) {
                order = "1";
            }

            if (string.IsNullOrEmpty (sex) == true) {
                sex = "0";
            }

            if (heightfrom == "指定しない" || heightfrom =="指定なし") {
                heightfrom = "";
            }

            if (heightto == "指定しない" || heightto =="指定なし") {
                heightto = "";
            }

			postDatas.Add (HttpConstants.ORDER, order);
			postDatas.Add (HttpConstants.SEX_CD, sex);
			postDatas.Add (HttpConstants.AGE_FROM, agefrom);         
			postDatas.Add (HttpConstants.AGE_TO, ageto);

			postDatas.Add (HttpConstants.HEIGHT_FROM, heightfrom);
			postDatas.Add (HttpConstants.HEIGHT_TO, heightto);
			postDatas.Add (HttpConstants.BODY_TYPE, bodytype);

            //指定しない場合。
            if (string.IsNullOrEmpty (isimage) == true) {
                isimage = "";
            //画像なしの場合。
            } else if (isimage == "2") {
                isimage = "0";
            }

			postDatas.Add (HttpConstants.IS_IMAGE, isimage);
			postDatas.Add (HttpConstants.RADIUS, radius);
			postDatas.Add (HttpConstants.KEYWORD, keyword);
			postDatas.Add (HttpConstants.PAGE_NUBER, page);
			postDatas.Add (HttpConstants.API_VERSION_NAME,DeviceService.GetAppVersion());

			Request (postDatas);
			
		}
		#endregion

		#region Request Send Processing

		private void Request (Dictionary<string,string> postDatas)
		{
			string url = DomainData.GetApiUrl (DomainData.USER_LIST);
			HttpHandler.Send<UserListEntity.Result> (url,postDatas,CallBack);
		}	


		private void CallBack (UserListEntity.Result result)
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
