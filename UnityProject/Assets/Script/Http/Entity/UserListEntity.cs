using System;
using System.Collections.Generic;

namespace Http
{
	public class UserListEntity
	{

		[Serializable]
		public class Result
		{
			public Data result;
		}

		[Serializable]
		public class Data
		{
			public List<UserDataEntity.Basic> users;
		}
	}
}