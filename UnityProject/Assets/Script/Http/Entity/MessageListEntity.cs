using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Http
{
    public class MessageListEntity
    {
        [Serializable]
        public class Result {
           public Message result;
        }

        [Serializable]
        public class Message {
            public List<MessgeData> messages;
            public UserDataEntity.Basic from_user;
            public UserDataEntity.Basic to_user;
        }

        [Serializable]
        public class MessgeData {
            public string id;
            public string send_user_id;
            public string send_user_name;
            public string receive_user_id;
            public string type;
            public string status;
            public string message;
            public string regist_datetime;
            public string time_ago;
            public string lastup_datetime;
            public string disable;
            public string send;
            public string special_type;
            public string is_mask;
            public string receive;
            public string regist_date;
            public string regist_time;
            public Image image;
        }
        
        [Serializable]
        public class Image {
            public string url;
            public string thumbnail_url;
        }
    }
}