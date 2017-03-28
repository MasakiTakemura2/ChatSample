using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

namespace Http
{
    public class EazyReturnDataEntity
    {
        [Serializable]
        public class Result {
            public string result;
        }


        #region 返り値が Complete Messageのみ (共通で使用)
        [Serializable]
        public class ResultComplete {
            public Complete result;
        }
        #endregion



        #region Valid用
        [Serializable]
        public class ResultUrl {
            public Url result;
        }
        
        [Serializable]
        public class Url {
            public string url;
        }
        #endregion


        
        #region お気に入り用
        [Serializable]
        public class ResultFavorite {
            public IsFavorite result;
        }
        
        [Serializable]
        public class IsFavorite {
            public string is_favorite;
            public List<string> complete;
        }
        #endregion



        #region 通報用
        [Serializable]
        public class ResultReport {
            public Complete result;
        }
        #endregion
        
        
        #region ユーザーブロック用
        [Serializable]
        public class ResultUserBlock {
            public IsBlock result;
        }
         
        [Serializable]
        public class IsBlock {
            public string is_block;
            public List<string> complete;
        }
        #endregion
        
        #region 共通で使用。
        [Serializable]
        public class Complete {
            public List<string> complete;
        }
        #endregion

        #region 課金処理リクエストの返り値
        [Serializable]
        public class PaymentResult {
            public PaymentComplete result;
        }

        [Serializable]
        public class PaymentComplete 
        {
            public string payment;
            public string current_point;
            public List<string> complete;
        }
        #endregion
        

        #region メッセージのカウントを取得する時の返り値
        [Serializable]
        public class MsgBadgeResult 
        {
            public MsgBadgeCount result;
        }

        [Serializable]
        public class MsgBadgeCount  {
            public string count;
        }
        #endregion
        
        #region Like、Nppeで巻き戻しの返り値用
        [Serializable]
        public class ReWindResult 
        {
            public IsReWind result;
        }
        
        [Serializable]
        public class IsReWind  {
            public string is_rewind;
        }        
        #endregion

        #region 送り放題機能開放の返り値用。
		[Serializable]
		public class LimitMessageDecisionResult
		{
			public LimitReleaseResult result;
		}
        [Serializable]
        public class LimitReleaseResult 
		{
            public string is_limit_release;
			public List<string> complete; 
        }

		[Serializable]
		public class LimitMessageResult
		{
			public LimitReleaseMsg result;
		}
        [Serializable]
        public class LimitReleaseMsg
		{
             public List<string> message;
        }
        #endregion

	}
}
