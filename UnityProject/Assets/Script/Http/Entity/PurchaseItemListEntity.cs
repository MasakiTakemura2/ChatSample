using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Http
{
    /// <summary>
    /// Purchase item list entity.
    /// </summary>
    public class PurchaseItemListEntity
    {
        [Serializable]
        public class Result {
           public PurchaseList result;
        }

        [Serializable]
        public class PurchaseList {
            public List<PurchaseItem> purchase_items;
        }

        [Serializable]
        public class PurchaseItem {
            public string id;
            public string point_group_id;
            public string name;
            public string description;
            public string platform;
            public string method;
            public string amount;
            public string point;
            public string service_point;
            public string product_id;
            public string seq;
            public string regist_datetime;
            public string lastup_datetime;
            public string disable;
        }
    }
}