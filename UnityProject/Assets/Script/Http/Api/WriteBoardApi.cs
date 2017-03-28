using UnityEngine;
using System.Collections.Generic;
using Helper;

namespace Http
{
public class WriteBoardApi{

		#region member variable
		public static bool _success = false;
		//public static EazyReturnDataEntity.Result _httpCatchData;
		public static string _httpCatchData = "";
		#endregion


		#region Construct

		public WriteBoardApi(string userKey,string boardCategoryID,string title,string body,Texture2D image1 = null,Texture2D image2  = null,Texture2D image3 = null)
		{
			_success = false;
			Dictionary<string,string> postDatas = new Dictionary<string,string> ();
			Dictionary<string,Texture2D> postBinaryDatas = new Dictionary<string,Texture2D> ();

			postDatas.Add (HttpConstants.USER_KEY, userKey);
			postDatas.Add (HttpConstants.BOARD_CATEGORY_ID, boardCategoryID);
			postDatas.Add (HttpConstants.TITLE, title);
			postDatas.Add (HttpConstants.BODY, body);
			postDatas.Add (HttpConstants.API_VERSION_NAME, DeviceService.GetAppVersion());

			if (image1 != null)
			{
				postBinaryDatas.Add ("image[0]", image1);
			}
			if (image2 != null)
			{
				postBinaryDatas.Add ("image[1]", image2);
			}
			if (image3 != null) 
			{
				postBinaryDatas.Add ("image[2]", image3);
			}
		
			Request (postDatas,postBinaryDatas);
		}
		#endregion

		#region Request Send Processing

		private void Request (Dictionary<string,string> postDatas,Dictionary<string,Texture2D> postBinaryDatas)
		{
			string url = DomainData.GetApiUrl (DomainData.WRITE_BOARD);
			HttpHandler.SendWriteBoard(url,postDatas,postBinaryDatas,CallBack);
		}	


		private void CallBack (string result)
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
