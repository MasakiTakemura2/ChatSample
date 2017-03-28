using UnityEngine;
using System.Collections.Generic;
using Helper;

namespace Http
{
	public class ReadBoardApi
	{

		#region member variable
		public static bool _success = false;
		public static ReadBoardEntity.Result _httpCatchData;
		#endregion


		#region Construct
		public ReadBoardApi (string userKey, string boardID)
		{
			_success = false;
			Dictionary<string,string> postDatas = new Dictionary<string,string> ();

			postDatas.Add (HttpConstants.USER_KEY, userKey);
			postDatas.Add (HttpConstants.BOARD_ID, boardID);
			postDatas.Add (HttpConstants.API_VERSION_NAME, DeviceService.GetAppVersion ());

			Request (postDatas);
		}
		#endregion

		#region Request Send Processing
		private void Request (Dictionary<string,string> postDatas)
		{
			string url = DomainData.GetApiUrl (DomainData.READ_BOARD);
			HttpHandler.Send<ReadBoardEntity.Result> (url, postDatas, CallBack);
		}

		private void CallBack (ReadBoardEntity.Result result)
		{
			_success = (result != null);
			if (_success == true)
			{
				_httpCatchData = result;
			}
		}
		#endregion
	}

}
