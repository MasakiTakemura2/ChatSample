using UnityEngine;
using System;
using System.Reflection;

using System.Text;
using LitJson;

using System.Collections.Generic;
using System.Security.Cryptography;
//susing Common;

namespace Helper {

    /*
     * StringLibクラス
     * 文字列を扱う共通処理用クラス
     *
     * 外部インターフェース
     *  スタティックメソッド
     *  - StringToBytes(string s)
     *    文字列sをバイト列に変換する
     *  - JsonToObject<T>(string s)
     *    文字列s(JSON)をT型のオブジェクトにdeserializeし作成されたオブジェクトを返す
     *  - ObjectToJson(object o)
     *    引数で受け取ったオブジェクトをJSONにserializeする
     */

	public class StringLib {
		public static byte[] StringToBytes(string s){
			if(s.Length == 0){
				return new byte[]{0};
			}
			return Encoding.UTF8.GetBytes(s);
		}
		
		//スクリプト上で日本語がベタ書きされている箇所があるのでここのクラスではなくても良いですが、洗い出して書き変える作業が必要。

		public static T JsonToObject<T>(string s){
			try {
				return JsonMapper.ToObject<T>(s);
			} catch(JsonException e){
                // Error POPUP
				Debug.Log(e.Message);
			}
			return default(T);
		}
		
		public static string ObjectToJson(object o){
			if(o == null){
				return "{}";
			}
			if(o is string){
				return o.ToString();
			}

			try {
				return JsonMapper.ToJson(o);
			} catch(JsonException e){
				// Error POPUP
				Debug.Log(e.Message);
			}
			return "";
		}

		public static string HashingMD5(string input, string secret){
			HashAlgorithm md5 = new MD5CryptoServiceProvider ();
			byte[] data = md5.ComputeHash (Encoding.UTF8.GetBytes(input + secret));

			return ByteToHexString (data);
		}

		public static string HashingHMACSHA1(string input, string secret){
			HMACSHA256 hmac_sha256 = new HMACSHA256 (Encoding.UTF8.GetBytes(secret));
			byte[] data = hmac_sha256.ComputeHash (Encoding.UTF8.GetBytes (input));
			//HMACSHA1 hmac_sha1 = new HMACSHA1 (Encoding.ASCII.GetBytes(secret));
			//byte[] data = hmac_sha1.ComputeHash (Encoding.ASCII.GetBytes (input));

			return ByteToHexString (data);
		}

		public static bool Contains(string target_word, string search_word){
			if(target_word.Trim () == "" || search_word.Trim () == ""){
				return false;
			}
			return target_word.Trim ().Contains(search_word);
		}

		public class EmptyObject
        {
			public EmptyObject(){
			}
		}

        #region プライベート関数
		private static string ByteToHexString(byte[] data){
			StringBuilder strBuilder = new StringBuilder ();
			for(int i = 0; i < data.Length; i++){
				strBuilder.Append (data [i].ToString ("x2"));
			}
			return strBuilder.ToString ();
		}
        #endregion
	}
}
