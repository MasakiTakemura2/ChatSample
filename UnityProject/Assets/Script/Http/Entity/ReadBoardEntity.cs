using System;
using System.Collections.Generic;

namespace Http
{
	public class ReadBoardEntity
	{

		[Serializable]
		public class Result
		{
			public Boards result;
		}

		[Serializable]
		public class Boards
		{
			public Board board;
		}

		[Serializable]
		public class Board
		{
			public string id;
			public string user_id;
			public string status;
			public string title;
			public string body;
			public string message_count;
			public string views;
			public string write_datetime;
            public string time_ago;
			public string lastup_datetime;
			public string disable;
			public BoardCategory board_category;
			public List<Images> images;
			public UserDataEntity.Basic user;
		}

		[Serializable]
		public class Images
		{
			public string url;
			public string thumbnail_url;
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