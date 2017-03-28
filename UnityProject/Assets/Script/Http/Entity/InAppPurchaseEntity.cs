using UnityEngine;
using System.Collections;
using System;

namespace Http
{
	public class InAppPurchaseEntity {

		[Serializable]
		public class PaymentList
		{
			public string payment_id;
			public string platform_id;
			public string store_id;
			public string price;
			public string orb;
			public string payment_name;
		}
	}
}
	
