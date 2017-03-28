using UnityEngine;
using System.Collections;

public class CommonConstants
{
    public static bool IS_PREMIUM = false;

    #region Scene Name 
    public const string START_SCENE = "01_Start";
    public const string TUTORIAL_SCENE = "01_Tutorial";
    public const string MESSAGE_SCENE = "02_Message";
    public const string MYPAGE_SCENE = "03_Mypage";
    public const string MATCHING_SCENE = "04_Matching";
    public const string SEARCH_SCENE = "05_Search";
    public const string BULLETIN_BOARD_SCENE = "06_BulletinBoard";
    public const string PURCHASE_SCENE = "07_Purchase";
    public const string PROBLEM_SCENE = "999_ProblemPanel";
    #endregion

    #region SDK Setting Parameter
    public const int PARTY_IOS_APP_ID = 4434;
    public const string PARTY_IOS_APP_KEY = "b99f82813babf5da301a71f8c67a432e";
    public const int PARTY_IOS_TURTORIAL_ID = 34102;
    public const int PARTY_IOS_MACARON_ID = 42083;
    public const int PARTY_ADR_APP_ID = 4435;
    public const string PARTY_ADR_APP_KEY = "c519a4e88a53f21372e85e8efd9f33e9";
    public const int PARTY_ADR_TURTORIAL_ID = 34103;
    public const int PARTY_ADR_MACARON_ID = 41991;
    #endregion

    #region For Resources Load
    public const string TINDER_PANEL = "Prefab/Parts/TinderImage";
    public const string WEBVIEW_EMPTY_PANEL = "Prefab/Parts/EmptyWebview";
    public const string TINDER_PANEL_IMG_TEST_1 = "Textures/Tmp/te2";
    public const string TINDER_PANEL_IMG_TEST_2 = "Textures/Tmp/te3";
    public const string PROFILE_CHANGE_PANEL = "Prefab/SceneUguiPanel/03_Mypage/PanelProfileChange";
    public const string APPLI_REVIEW_POPUP_PANEL = "Prefab/SceneUguiPanel/Common/PopupAppliReView";
    public const string OTHER_SETTING_PANEL = "Prefab/SceneUguiPanel/03_Mypage/OtherSetting";
    public const string NOTIFICATION_PANEL = "Prefab/SceneUguiPanel/Common/NotificationPanel";
    #endregion

    #region use FindWithTag > Tag Name
    public const string POPUP_BASIC_TAG = "PopupBasic";
    public const string POPUP_SECOND_SELECT_TAG = "PopupSecondSelect";
    public const string MOSAICFIELD_TAG = "MosaicField";
    public const string HEADER_BACK_BUTTON = "HeaderBackButton";
    public const string POPUP_OVERLAY = "PopUpOverlay";
    public const string LOADING_OVERLAY = "LoadingOverlay";
    public const string BACK_SWIPE = "BackSwipe";
    public const string CAMERA_OR_GALLERY = "CameraOrGallery";
    public const string POPUP_THIRD_SELECT_TAG = "PopupThirdSelect";
    #endregion

    //view_name
    #region カスタムプッシュで遷移先を決める定義データ
    public const string VIEW_NAME_MESSAGE = "message";        //：メッセージリスト
    public const string VIEW_MESSAGE_DETAIL = "message_detail"; //：メッセージ詳細(id：ユーザーID)
    public const string VIEW_PROFILE = "profile";        //：プロフィール詳細(id：ユーザーID)
    public const string VIEW_BOARD = "board";          //：掲示板
    public const string VIEW_HISTORY = "history";        //：履歴 
    public const string VIEW_INFO_DETAIL = "information_detail";//：お知らせ詳細(id：お知らせID)   
    #endregion

    #region 広告管理系

    //本番に上げるときはfalse
	public static bool IS_AD_TEST = false;

    //ImobileパートナーID - テスト
    public const string IMOBILE_PARTNER_TEST_ID = "34816";
    
    //ImobileメディアID - テスト 
    public const string IMOBILE_MDDIA_TEST_ID = "135002";
    
    //Imobile各スポットID - テスト 
    public const string IMOBILE_INTERSTATIAL_SPOT_TEST_ID = "342418"; 
    public const string IMOBILE_BANNER_SPOT_TEST_ID       = "342407";
    
	//NEND SDK API KEY - テスト用
	public const string NEND_NATIVEAD_API_TEST_ID    = "10d9088b5bd36cf43b295b0774e5dcf7d20a4071"; 

	//NEND SDK SPOT KEY - テスト用
	public const string NEND_NATIVEAD_SPOT_TEST_ID = "485500"; 


	//ImobileパートナーID
    public const string IMOBILE_PARTNER_ID = "10083";

    //ImobileメディアID
    public const string IMOBILE_MDDIA_ID = "378748";
   
    //Imobile各スポットID
    public const string IMOBILE_INTERSTATIAL_SPOT_ID = "1294262"; //本番
    public const string IMOBILE_BANNER_SPOT_ID       = "1294572"; //本番
    public const string IMOBILE_FULL_SPOT_ID         = "1331440"; //本番
    

    //「Maio」動画広告用のMEDIA_ID
    public const string MAIO_MEDIA_ID = "md1b0cfeae9e4f6bcc5006d5a20a02a71";
    
    //「Maio」動画広告用のZONE_ID = 使用するシーンによるID
    public const string MAIO_ZONE_ID_1 = "z8dcaf1997c2debdad1337c4d4e7917a6";
    public const string MAIO_ZONE_ID_2 = "z9cac4ae5f9d94106fabbecee1c443e56";
    
	//NEND SDKのIOS API KEY - 本番用
	public const string NEND_NATIVEAD_API_IOS_ID  = "28c5c96ea58ace6955cd940d3886eb68e6ccd9b0"; 

	//NEND SDKのIOS SPOT KEY - 本番用
	public const string NEND_NATIVEAD_SPOT_IOS_ID = "701275";

	//NEND SDKのIOS API KEY - 本番用
	public const string NEND_NATIVEAD_API_ANDROID_ID  = ""; 

	//NEND SDKのIOS SPOT KEY - 本番用
	public const string NEND_NATIVEAD_SPOT_ANDROID_ID = "";

    //NEND SDKのIOS API KEY - 本番用
    public const string NEND_NATIVEFULL_API_IOS_ID  = "76ad707c8e7b81983360e254db2d7475413aea26";
    
    //NEND SDKのIOS SPOT KEY - 本番用
    public const string NEND_NATIVEFULL_SPOT_IOS_ID = "707478";
    
    //NEND SDKのIOS API KEY - 本番用
    public const string NEND_NATIVEFULL_API_ANDROID_ID  = "";
    
    //NEND SDKのIOS SPOT KEY - 本番用
    public const string NEND_NATIVEFULL_SPOT_ANDROID_ID = "";


	public const bool IS_AD_NEND = true;
    #endregion
}
