using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

namespace Http
{
    public class SetPassingConfigPostEntity {
		[Serializable]
        public class Post
		{
            public string isPassing;      //すれ違い通信の可否。
            public string isNotification; //プッシュ通知の可否。
            public string sexCd;      //男女。
            public string ageFrom;   //年齢から〜。
            public string ageTo;     //年齢〜まで。
            public string heightFrom;//身長から〜。
            public string heightTo;  //身長まで。
            public string bodyType;  //体型。
            public string isImage;   //画像の有無。
            public string radius;     //自分の位置からの距離。
            public string keyword;    //キーワード。
            public string isSendMessage; //すれ違い時に送信するか否か。
            public string message;    //すれ違い時に送信するメッセージ。
		}
	}
}
