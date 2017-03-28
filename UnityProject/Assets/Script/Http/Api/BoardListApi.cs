using UnityEngine;
using System.Collections.Generic;
using Helper;

namespace Http
{
	public class BoardListApi
	{

		#region member variable
		public static bool _success = false;
		public static BoardListEntity.Result _httpCatchData;
		#endregion


		#region Construct

		public BoardListApi(
			string userKey,
			string boardCategoryID,
			string lat,
			string lng,
			string sex_cd,
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
			//Ready processing 
			_success = false;
			Dictionary<string,string> postDatas = new Dictionary<string,string> ();
            
            lat = "";
            lng = "";
			postDatas.Add (HttpConstants.USER_KEY,userKey);
			postDatas.Add (HttpConstants.BOARD_CATEGORY_ID, boardCategoryID);
			postDatas.Add (HttpConstants.LAT, lat);
			postDatas.Add (HttpConstants.LNG, lng);
			postDatas.Add (HttpConstants.SEX_CD, sex_cd);
			postDatas.Add (HttpConstants.AGE_FROM, agefrom);
			postDatas.Add (HttpConstants.AGE_TO, ageto);
			postDatas.Add (HttpConstants.HEIGHT_FROM, heightfrom);
			postDatas.Add (HttpConstants.HEIGHT_TO, heightto);
			postDatas.Add (HttpConstants.BODY_TYPE, bodytype);
			postDatas.Add (HttpConstants.IS_IMAGE, isimage);
			postDatas.Add (HttpConstants.RADIUS, radius);
			postDatas.Add (HttpConstants.PAGE_NUBER, page);
			postDatas.Add (HttpConstants.KEYWORD, keyword);
			postDatas.Add (HttpConstants.API_VERSION_NAME,DeviceService.GetAppVersion());

			Request (postDatas);
			
		}
		#endregion

		#region Request Send Processing

		private void Request (Dictionary<string,string> postDatas)
		{
			string url = DomainData.GetApiUrl (DomainData.BOARD_LIST);
			HttpHandler.Send<BoardListEntity.Result> (url,postDatas,CallBack);
		}	


		private void CallBack (BoardListEntity.Result result)
		{
			_success = (result!=null);

			if (_success == true) 
			{
				_httpCatchData = result;
                Debug.Log  (_httpCatchData.result.boards.Count + " c o u n t 注目、注目") ;
			}
		}

		#endregion
	}

}
