using UnityEngine;
using System.Collections;

public static class HttpConstants
{
    #region 必ず取得する値。
    public const string API_KEY_NAME             = "api_key";
    //public const string API_KEY_VALUE            = "b9b001e7f126cee5633c5f8d360df06c";
    public const string API_KEY_VALUE            = "54712f9c1e0753a0a286d332d278ef51";
    public const string API_REQUEST_FORMAT_NAME  = "request_format";
    public const string API_REQUEST_FORMAT_VALUE = "json";
    public const string API_VERSION_NAME         = "version";
    public const string API_AES_POST_KEY         = "json_data";
    #endregion

    #region API user_device でPOST送信する際の名前
    public const string UIID_NAME         = "uiid";
    public const string UDID_NAME         = "udid";
    public const string IDFV_NAME         = "idfv";
    public const string PLATFORM_ID_NAME  = "platform";
    public const string BUNDLE            = "bundle";
	public const string DEVICE_MODEL      = "device_model";
	public const string REGIST_DATE 	  = "regist_date";
    public const string USER_KEY          = "user_key";
    public const string TO_USER_ID        = "to_user_id";
    public const string MESSAGE           = "message";
    public const string BEFORE_MESSAGE_ID = "before_message_id";
    public const string AFTER_MESSAGE_ID  = "after_message_id";
    public const string MESSAGE_ID        = "message_id";
    public const string IS_SUPER          = "is_super";
    public const string LATITUDE          = "lat";
    public const string LONGITUDE         = "lng";
    public const string IS_PUBLIC_PROFILE = "is_public_profile";
    public const string PROFILE_IMAGE     = "profile_image";
    public const string COVER_IMAGE       = "cover_image";
    public const string HISTORY_TYPE      = "history_type";
    public const string MAIL_ADDRESS      = "mail_address";
    public const string PASSWORD          = "password";
    public const string INQUIRY           = "inquiry";
    #endregion
    
    #region ユーザープロフィール更新用
    public const string NAME         = "name";
    public const string BLOOD_TYPE   = "blood_type";
    public const string PREF         = "pref";
    public const string CITY_ID      = "city_id";
    public const string HEIGHT       = "height";
    public const string WEIGHT       = "weight";
    public const string BIRTH_DATE   = "birth_date";
    public const string PROFILE      =  "profile";
    public const string BODY_TYPES   = "body_type[]";
    public const string HAUR_STYLES  = "hair_style[]";
    public const string GLASSESS     = "glasses[]";
    public const string HOLIDAYS     = "holiday[]";
    public const string ANNUAL_INCOMES = "annual_income[]";
    public const string EDUCATIONS     = "education[]";
    public const string HOUSEMATES     = "housemate[]";
    public const string SIBLINGS       = "sibling[]";
    public const string ALCOHOLS       = "alcohol[]";
    public const string TOBACCOS       = "tobacco[]";
    public const string CARS           = "car[]";
    public const string PETS           = "pet[]";
    public const string HOBBYS         = "hobby[]";
    public const string INTERESTS      = "interest[]";
    public const string TYPES          = "type[]";
    public const string PERSONALITYS   = "personality[]";
    public const string MARTIALS       = "marital[]";
    #endregion

    #region API user_device_token でPOST送信する際の名前
    public const string DEVICE_TOKEN = "device_token";
    #endregion


	#region API メッセージ一覧 送信する際の名前
	public const string DISPLAY_TYPE = "display_type";
	#endregion

	#region API board_list でPOST送信する際の名前
	public const string KEYWORD = "keyword";
	#endregion

	#region API board_list でPOST送信する際の名前
	public const string ORDER = "order";
	public const string SEX_CD = "sex_cd";
    #endregion

    #region API すれ違い設定更新で使用する名前
    public const string IS_PASSING      = "is_passing";
    public const string IS_NOTIFICATION = "is_notification";
    public const string IS_SEND_MESSAGE = "is_send_message";
    #endregion

    #region API プッシュの許可設定
    public const string IS_DISPLAY_MESSAGE = "is_display_message";
    #endregion

    #region API GPSの許可設定用
    public const string IS_PUBLIC_GPS = "is_public_gps";
    #endregion
    
	#region API board_list でPOST送信する際の名前
	public const string BOARD_CATEGORY_ID	= "board_category_id";
	public const string LAT	= "lat";
	public const string LNG	= "lng";
	public const string AGE_FROM	= "age_from";
	public const string AGE_TO	= "age_to";
	public const string HEIGHT_FROM	= "height_from";
	public const string HEIGHT_TO	= "height_to";
	public const string BODY_TYPE	= "body_type";
	public const string IS_IMAGE = "is_image";
	public const string RADIUS	= "radius";
	public const string PAGE_NUBER	= "page";
	#endregion

	#region API write_board でPOST送信する際の名前
	public const string TITLE = "title";
	public const string BODY = "body";
	public const string IMAGES = "image[]";
    public const string MOVIES = "movie[]";
    public const string IMAGE = "image";
    public const string MOVIE = "movie";
	#endregion

	#region API read_board でPOST送信する際の名前
	public const string BOARD_ID = "board_id";
	#endregion

    #region 課金APIでポストする用に導入。
    public const string RECEIPT           = "receipt";
    public const string AUTO_RENEWBLE     =  "auto_renewable";
    #endregion
}