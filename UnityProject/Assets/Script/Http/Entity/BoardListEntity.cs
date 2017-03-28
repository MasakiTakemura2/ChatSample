using System;
using System.Collections.Generic;

namespace Http
{
	public class BoardListEntity
	{

		[Serializable]
		public class Result
		{
			public Boards result;
		}

		[Serializable]
		public class Boards
		{
			public List<Board> boards;

		}

		[Serializable]
		public class Board
		{
            public string is_banner;
            public string url;
            public string image_url;

			public string id;
			public string board_category_id;
			public string user_id;
			public string status;
			public string title;
			public string body;
			public string message_count;
			public string views;
			public string write_datetime;
			public string lastup_datetime;
			public string disable;
			public string board_category_name;
            public string time_ago;
			public BoardCategory board_category;

			public UserDataEntity.Basic user;
		}

		[Serializable]
		public class BoardCategory 
		{
			public string id;
			public string name;
			public string seq;
			public string regist_datetime;
			public string lastup_datetime;
			public string disable;
		}

	
	}
}