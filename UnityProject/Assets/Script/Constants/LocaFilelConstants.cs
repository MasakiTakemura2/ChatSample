using UnityEngine;
using System.Collections;

public class LocalFileConstants
{
    #region HTTPをONにEditor専用
    public const string EDITOR_ONLY_DOMAIN_SWICHER = "EDITOR_ONRY_DOMAIN_SWICHER";
    #endregion

    #region シーンまたぐ時に現在のシーンと次へのシーンデータをローカルに保存しておく用キー
    public const string SCENE_BEFORE_QUEUE_SAVE_KEY = "SCENE_BEFORE_QUEUE_SAVE_KEY";
    public const string SCENE_AFTER_QUEUE_SAVE_KEY  = "SCENE_AFTER_QUEUE_SAVE_KEY";
    #endregion

    #region 音量調整用に保存用キー
    public const string BGM_VOLUME_KEY = "BGM_VOLUME_KEY";
    public const string SE_VOLUME_KEY  = "SE_VOLUME_KEY";
    #endregion

    #region チュートリアル 保存用キー
    public const string TUTORIAL_MATCHING_KEY = "TUTORIAL_MATCHING_KEY";
    public const string TUTORIAL_MESSAGE_KEY  = "TUTORIAL_MESSAGE_KEY";
    public const string TUTORIAL_FAVORITE_KEY = "TUTORIAL_FAVORITE_KEY";
	#endregion

    #region Local File Name
    public const string COMMON_LOCAL_FILE_NAME = "U2FsdGVkX18AXay107XDtZb7bFhteu27XyI5gNgTPSI=.bat";
    #endregion
    
	#region その他
    public const string LOCAL_FILE_CYPHER_KEY = "TwIqA8M1";
	#endregion

    #region ローカル保存用の暗号化キー
    public const string VALID_URL_KEY           = "ValidUrl";                //Valid Url用
    public const string INIT_MASTER_DATA_KEY    = "initMasterData";         //マスターデータ保存用のキー
    public const string USER_KEY                = "USER_KEY_EDITOR_ONLY";   //ユーザーID保存用の鍵 (Unity EDITOR only)
    public const string GENDER_KEY              = "GENDER_KEY_EDITOR_ONLY"; //ユーザー性別保存用の鍵 (Unity EDITOR only)
    public const string FIRST_GPS_SET_KEY       = "FIRST_GPS_SET_KEY";      //初回起動時にGPSを取得出来た時のローカル保存用キー
    public const string FIRST_SUPER_LIKE        = "FIRST_SUPER_LIKE";
    public const string FIRST_REWIND            = "FIRST_REWIND";
    public const string FROM_MYPAGE_SCENE       = "FromMypageScene";
    public const string SEARCH_CONDITION_KEY    = "SEARCH_CONDITION_KEY"; // 検索条件を保存する用に
    public const string BBS_SEARCH_CONDITION_KEY = "BBS_SEARCH_CONDITION_KEY"; // 検索条件を保存する用に
    public const string APPLI_REVIEW_POPUP_SHOW = "APPLI_REVIEW_POPUP_SHOW";
    public const string MOVIE_POPUP_SHOW        = "MOVIE_POPUP_SHOW";                //MOVIE POPUP SHOW
    #endregion

    /// <summary>
    /// Gets the local file dir.
    /// Application.persistentDataPath + "/../Library/Data/";
    /// </summary>
    /// <returns>The local file dir.</returns>
    public static string GetLocalFileDir ()
    {
        return Application.persistentDataPath + "/../Library/Data/";
    }

    /// <summary>
    /// Gets the assets folder path.
    /// </summary>
    /// <returns>The assets folder path.</returns>
    public static string GetAssetsFolderPath ()
    {
        return Application.persistentDataPath + "/../Library/Assets/";
    }

    /// <summary>
    /// Gets down load path.
    /// </summary>
    /// <returns>The down load path.</returns>
    public static string GetDownLoadPath ()
    {
        string path = "";
        #if UNITY_ANDROID && !UNITY_EDITOR
            path = Application.persistentDataPath + "/../Library/";
        #elif UNITY_IPHONE  && !UNITY_EDITOR
            path = Application.persistentDataPath + "/../Library/Caches/";
        #else
            path = Application.persistentDataPath + "/../Library/";
        #endif

        return path;
    }

}
