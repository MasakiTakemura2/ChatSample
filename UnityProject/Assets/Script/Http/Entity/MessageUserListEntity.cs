using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Http
{
    public class MessageUserListEntity
    {
        [Serializable]
        public class Result {
           public Users result;
        }

        [Serializable]
        public class Users {
            public List<UserList> users;
        }

        [Serializable]
        public class UserList {
            public string id;
            public string send_user_id;
            public string send_user_name;
            public string receive_user_id;
            public string type;
            public string status;
            public string message;
            public string regist_datetime;
            public string lastup_datetime;
            public string disable;
            public string time_ago;
            public string profile_image_url;
            public UserDataEntity.Basic user;
        }
    }
}