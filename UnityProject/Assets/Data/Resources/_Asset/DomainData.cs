using UnityEngine;
using System;
using System.Collections.Generic;
using Helper;

/// <summary>
/// 文言を管理するasset
/// </summary>
public class DomainData : SingletonScriptableObject<DomainData> 
{
    #region inner class
    /// <summary>
    /// API データを管理するEntity
    /// </summary>
    [Serializable]
    public class ApiKeyValue
    {
        /// <summary>
        /// API の識別キー
        /// </summary> 
        public string key = "";

        /// <summary>
        /// API 名
        /// </summary>
        public string api = "";
    }
    #endregion


    #region data
	public static string _env = "product";
	//public static string _env = "staging";
    //public static string _env = "develop";
    public static string _infoAddress = "info@cafeau1ait.xyz";
    public static string _parentBundle = "com.lino1017";
	#if UNITY_IPHONE
	public static int _subPlatformId = (int)SubPlatformType.Apple;

    public static string _bundle = "com.lino1017";
	#elif UNITY_ANDROID
	// Google
	public static int _subPlatformId = (int)SubPlatformType.Google;
    //パッケージネーム（書き換え必須）
    public static string _bundle = "com.lino1017";

	// Rakuten
	//public static int _subPlatformId = (int)SubPlatformType.Rakuten;
	//public static string _bundle = "com.tiam.dissworld.R";

	// Amazon
	//public static int _subPlatformId = (int)SubPlatformType.Amazon;
	//public static string _bundle = "com.tiam.dissworld.A";

	#else
	public static int _subPlatformId = (int)PlatformType.Apple;
	public static string _bundle = "com.lino1017";
	#endif

    private static string HTTP   = "http";
	private static string HTTPS  = "https";

    /// <summary>
    /// 使用するドメイン
    /// </summary>

    [SerializeField]
    //Vbox -> 個人個人で違うと思うので適宜変えてください。
    public string LOCAL_DOMAIN   = "192.168.33.10/chat/server/api/"; // 開発用 (ローカルマシン)

    [SerializeField]
    private string LOCAL_WEBVIEW = "192.168.33.10/chat/server/webview"; // 開発用 WEBVIEW (ローカルマシン)

    [SerializeField]
    public string DEV_DOMAIN     = "54.248.81.53/chat/api/"; // 開発用 ( AWSマシン )

    [SerializeField]
    private string DEV_WEBVIEW = "54.248.81.53/chat/webview/"; // 開発用 WEBVIEW ( AWSマシン )

    [SerializeField]
    public string STG_DOMAIN     = "cafeau1ait.xyz/chat/api/"; // STG用　( AWSマシン )

    [SerializeField]
    public string STG_WEBVIEW    = "cafeau1ait.xyz/chat/webview/"; // STG用 WEBVIEW( AWSマシン )

    [SerializeField]
    public string PRODUCT_DOMAIN = "cafeau1ait.xyz/api/"; // 本番用 ( AWSマシン )

    [SerializeField]
    private string PRODUCT_WEBVIEW = "cafeau1ait.xyz/webview/"; // 本番用 WEBVIEW ( AWSマシン )

    [SerializeField]
	public string DEV_CONTENTS_DOMAIN     = ""; // AWS CloudFront or S3 domain用

    [SerializeField]
	public string PRODUCT_CONTENTS_DOMAIN = ""; // AWS CloudFront or S3 domain用

    /// <summary>
    /// APIリスト
    /// </summary>
    public List<ApiKeyValue> _uriList = new List<ApiKeyValue>();
    #endregion

    public DomainData ()
    {
        var uri = new ApiKeyValue ();
        uri.key = "API_DATA_REQUEST_URI";
        uri.api = "request_api/index";

        _uriList.Add (uri);
        uri = null;
    }

    #region request uri
    //ec2から受け取るメインのAPI
    public const string API_DATA_REQUEST_URI     = "API_DATA_REQUEST_URI";

    //s3から受け取るURI
    public const string API_CONTENTS_REQUEST_URI = "API_CONTENTS_REQUEST_URI";//今後増える

    //仮登録用のAPI ※(URI取得用)
    public const string PRE_REGIST_USER = "PRE_REGIST_USER";

    //デバイストークンセット
    public const string SET_DEVICE_TOKEN = "SET_DEVICE_TOKEN";

    /// サーバーでキャッチしてサーバーで広告や、データ解析系の処理を調整する用のApi。
    public const string VALID = "VALID";

    //ユーザデータをすべて返すAPI ※(URI取得用)
    public const string GET_USER         = "GET_USER";

    //初期データを返すAPI ※(URI取得用)
    public const string GET_INIT_DATA    = "GET_INIT_DATA";

    //プロフィール更新用
    public const string UPDATE_PROFILE   = "UPDATE_PROFILE";

	//プロフィール検索
	public const string USER_LIST = "USER_LIST";

	//Tuhin Work
	//掲示板一覧
	public const string BOARD_LIST = "BOARD_LIST";

	//掲示板書き込み
	public const string WRITE_BOARD = "WRITE_BOARD";

	//掲示板詳細
	public const string READ_BOARD = "READ_BOARD";

    //メッセージ一覧
    public const string MESSAGE_LIST = "MESSAGE_LIST";

    //受信ユーザー一覧
    public const string MESSAGE_USER_LIST = "MESSAGE_USER_LIST";

    //メッセージ送信。
    public const string SEND_MESSAGE = "SEND_MESSAGE";
    
    //ユーザーを通報するAPI
    public const string SEND_REPORT = "SEND_REPORT";

    //ユーザーをお気に入り登録する。
    public const string SET_USER_FAVORITE = "SET_USER_FAVORITE";
    
    //ユーザーのカバーイメージとプロフィールイメージをアップロードする用。
    public const string UPLOAD_USER_IMAGE = "UPLOAD_USER_IMAGE";
    
    //現在地の緯度、経度をサーバーに通知する用。
    public const string SET_GPS= "SET_GPS";
    
    //プロフィール公開設定
    public const string SET_PUBLIC_PROFILE= "SET_PUBLIC_PROFILE";
    
    //履歴 (あしあと、メッセージ履歴)
    public const string HISTORY_USER_LIST= "HISTORY_USER_LIST";

    //いいね
    public const string SET_USER_LIKE = "SET_USER_LIKE";

	//LikeNopeユーザー
	public const string RANDOM_USER_LIST = "RANDOM_USER_LIST";


    //ユーザーブロック用のAPI
    public const string SET_USER_BLOCK = "SET_USER_BLOCK";
    
    //メールアドレス登録
    public const string SET_MAIL_ADDRESS = "SET_MAIL_ADDRESS";

    //ユーザーの公開設定。
    public const string SEND_RANDOM_MESSAGE = "SEND_RANDOM_MESSAGE";

    //set passing config
    public const string SET_PASSING_CONFIG  = "SET_PASSING_CONFIG";

    //課金用
    public const string PAYMENT = "PAYMENT";

    //機種変更用
    public const string MODEL_CHANGE = "MODEL_CHANGE";

    //ヘルプ用表示データ。
    public const string HELP_LIST    = "HELP_LIST";

    //お問い合わせ
    public const string SEND_INQUIRY = "SEND_INQUIRY";
    
    //push通知通知設定
    public const string SET_NOTIFICATION_CONFIG = "SET_NOTIFICATION_CONFIG";

    //GPSのオンとオフ。
    public const string SET_PUBLIC_GPS = "SET_PUBLIC_GPS";
    
    //課金アイテムリスト
    public const string PURCHASE_ITEMLIST = "PURCHASE_ITEMLIST";
    
    //起動時とリストページのInitでリクエスト処理でバッジカウント取得
    public const string GET_UNREAD_MESSAGE_COUNT = "GET_UNREAD_MESSAGE_COUNT";
    
    //Tinder風UIの箇所で戻る場合に投げるリクエスト。
    public const string REWIND_USER = "REWIND_USER";

    //送り放題機能開放
    public const string LIMIT_RELEASE = "LIMIT_RELEASE";
    
    //送り放題機能開放確認メッセージ
    public const string LIMIT_RELASE_CONFIRM_MESSAGE = "LIMIT_RELASE_CONFIRM_MESSAGE";
    
    //WEBVIEWのURI - 利用規約
    public const string WEBVIEW_TERMS = "WEBVIEW_TERMS";

    //WEBVIEW MAP - 地図
    public const string WEBVIEW_MAP      = "WEBVIEW_MAP";


    public const string REVIVE_LIKE      = "REVIVE_LIKE";
    
    //WEBVIEW プライバシーポリシー
    public const string WEBVIEW_PRIVACY  = "WEBVIEW_PRIVACY";
    
    //WEBVIEW キャンペーン
    public const string WEBVIEW_CAMPAIGN = "WEBVIEW_CAMPAIGN";
    
    //WEBVIEW 特定商取引
    public const string WEBVIEW_TRANS    = "WEBVIEW_TRANS";

    //メッセージポイント消費するAPI
    public const string READ_MESSAGE    = "READ_MESSAGE";

    //誘導ページ用のWEBVIEW
    public const string WEBVIEW_INTRODUCTION = "WEBVIEW_INTRODUCTION";

    //メッセージ削除用
    public const string DELETE_MESSAGE = "DELETE_MESSAGE";

    //退会処理
    public const string RESIGN_USER = "RESIGN_USER";
    
    //退会解除処理
    public const string REVIVE_USER = "REVIVE_USER";
    
    #endregion

    /// <summary>
    /// API の URL を返す
    /// </summary>
    /// <returns>API の URL</returns>
    /// <param name="apiKey">APIの識別キー</param>
    public static string GetApiUrl (string apiKey, bool secure = false)
    {
        ApiKeyValue api = GetAPIKeyValue(apiKey);

        string http = "";

        if (secure == true) {
            http = HTTPS;
        } else {
            http = HTTP;
        }

		if (api != null) {
			string url = "";

			if (_env == "product") {
                url = string.Format ("{0}://{1}{2}", http, Instance.PRODUCT_DOMAIN, api.api);
				return url;
			} else if (_env == "staging") {
                url = string.Format ("{0}://{1}{2}", http, Instance.STG_DOMAIN, api.api);
				return url;
			} else {
				#if UNITY_EDITOR            
                    LocalFileHandler.Init ( LocalFileConstants.GetLocalFileDir() + LocalFileConstants.COMMON_LOCAL_FILE_NAME);
                    int domainSwich = LocalFileHandler.GetInt (LocalFileConstants.EDITOR_ONLY_DOMAIN_SWICHER);

                    switch (domainSwich)
                    {
                        case (int)SeverMachineType.LOCAL:
                            url = string.Format("{0}://{1}{2}", http, Instance.LOCAL_DOMAIN, api.api);
                            break;
                        case (int)SeverMachineType.DEV:
                            url =  string.Format("{0}://{1}{2}", http, Instance.DEV_DOMAIN, api.api);
                            break;
                        case (int)SeverMachineType.STG:
                            url =  string.Format("{0}://{1}{2}", http, Instance.STG_DOMAIN, api.api);
                            break;
                        case (int)SeverMachineType.PRODUCTION:
                            url =  string.Format("{0}://{1}{2}", http, Instance.PRODUCT_DOMAIN, api.api);
                            break;
                        default:
                            url =  string.Format("{0}://{1}{2}", http, Instance.DEV_DOMAIN, api.api);
                            break;
                    }
                    return url;
                #else
                     return string.Format("{0}://{1}{2}", http, Instance.DEV_DOMAIN, api.api);
                #endif
            }
        }
        return null;
    }

    #region コンテンツURLの管理
    /// <summary>
    /// コンテンツAPI の URL を返す
    /// </summary>
    /// <returns>API の URL</returns>
    /// <param name="apiKey">APIの識別キー</param>
    public static string GetContentsDataURL(string apiKey, bool secure = false)
    {
        string http = "";
        string domain = "";

        if (secure == true) {
            http = HTTPS;
        } else {
            http = HTTP;
        }

		if (_env == "product") {
            domain = http + Instance.PRODUCT_CONTENTS_DOMAIN;
        } else {
            domain =  http + Instance.DEV_CONTENTS_DOMAIN;
        }
        return domain;
    }
    #endregion


    #region WEBVIEW用のURL
    /// <summary>
    ///  WEB VIEW URL を返す
    /// </summary>
    /// <returns>APIのURL</returns>
    /// <param name="apiKey">APIの識別キー</param>
    public static string GetWebviewDataURL (string apiKey, bool secure = false)
    {
        ApiKeyValue api = GetAPIKeyValue(apiKey);
        if (api != null)
        {
            string url = "";
            string http;
            if (secure == true) {
                http = HTTPS;
            } else {
                http = HTTP;
            }

			if (_env == "product") {
                url =  string.Format("{0}://{1}{2}", http, Instance.PRODUCT_WEBVIEW, api.api);
                return url;
			} else if (_env == "staging") {
                url = string.Format ("{0}://{1}{2}", Instance.STG_WEBVIEW, api.api);
				return url;
            } else {
                //return string.Format("http://{0}{1}", Instance.DEV_DOMAIN, api.api);
                #if UNITY_EDITOR
                    LocalFileHandler.Init ( LocalFileConstants.GetLocalFileDir() + LocalFileConstants.COMMON_LOCAL_FILE_NAME);
                    int domainSwich = LocalFileHandler.GetInt (LocalFileConstants.EDITOR_ONLY_DOMAIN_SWICHER);

                    switch (domainSwich)
                    {
                        case (int)SeverMachineType.LOCAL:
                            url = string.Format("{0}://{1}{2}", http, Instance.LOCAL_WEBVIEW, api.api);
                            break;
                        case (int)SeverMachineType.DEV:
                            url =  string.Format("{0}://{1}{2}", http, Instance.DEV_WEBVIEW, api.api);
                            break;
                        case (int)SeverMachineType.STG:
                            url =  string.Format("{0}://{1}{2}", http, Instance.STG_WEBVIEW, api.api);
                            break;
                        case (int)SeverMachineType.PRODUCTION:
                            url =  string.Format("{0}://{1}{2}", http, Instance.PRODUCT_WEBVIEW, api.api);
                            break;
                        default:
                            url =  string.Format("{0}://{1}{2}", http, Instance.DEV_WEBVIEW, api.api);         
                            break;
                    }

                return url;
                #else
                   return string.Format("{0}://{1}{2}", http, Instance.DEV_WEBVIEW, api.api);
                #endif
            }
        }
        return null;
    }
    #endregion


    #region クラス内部関数
    /// <summary>
    /// API データを取得する
    /// </summary>
    /// <returns>API データ</returns>
    /// <param name="key">API識別キー</param>
    public static ApiKeyValue GetAPIKeyValue(string key)
    {
        ApiKeyValue reval = null;

        foreach (var obj in Instance._uriList)
        {
            if (obj.key == key)
            {
              reval  = obj;
              break;
            }
        }
        return reval;
    }
    #endregion
}