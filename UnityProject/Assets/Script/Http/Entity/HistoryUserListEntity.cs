using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Http
{
    public class HistoryUserListEntity
    {
        [Serializable]
        public class Result {
           public Users result;
        }

        [Serializable]
        public class Users {
            public List<UserDataEntity.Basic> users;
        }
    }
}