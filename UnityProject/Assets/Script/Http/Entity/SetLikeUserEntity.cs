using System;
using System.Collections.Generic;

namespace Http
{
    public class SetLikeUserEntity
    {
        [Serializable]
        public class Result
        {
            public Match result;
        }

        [Serializable]
        public class Match
        {
            public string is_like;
            public string is_matching;
            public string is_like_limit;
            public string release_time;
            public string limit_count;
            public string point;
            public UserDataEntity.Basic matching_user;
		}
    }
}