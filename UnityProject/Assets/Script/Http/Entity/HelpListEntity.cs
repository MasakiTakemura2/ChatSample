using System;
using System.Collections.Generic;

namespace Http
{
	public class HelpListEntity
	{
		[Serializable]
		public class Result
		{
			public Data result;
		}

		[Serializable]
		public class Data
		{
			public List<Helps> helps;
		}

		[Serializable]
		public class Helps
		{
			public string quesstion;
			public string answer;
		}
	}
}