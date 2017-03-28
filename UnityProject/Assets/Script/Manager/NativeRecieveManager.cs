using System;
using UnityEngine;
using System.Runtime.InteropServices;

public class NativeRecieveManager
{
#if UNITY_IPHONE && !UNITY_EDITOR
    [DllImport("__Internal")]
    private static extern string getIdfv ();

    [DllImport("__Internal")]
    private static extern string getUiid ();

    [DllImport("__Internal")]
    private static extern string getUdid ();

    [DllImport("__Internal")]
    private static extern void remotePushClear ();

    [DllImport("__Internal")]
    private static extern string getLocationStatus ();

    [DllImport("__Internal")]
    private static extern string getPushMessage();

    public static string GetIdfv () {
        string keyChainData = getkeyChain();
        
        if(keyChainData == ""){
            setkeyChain(getIdfv ());
        }
    
        return getkeyChain();
    }

    public static string GetUiid (String bundle) {
        return getUiid ();
    }

    public static string GetUdid () {
        return getUdid ();
    }

    public static void RemotePushClear () {
        remotePushClear ();
    }

    public static string GetLocationStatus () {
        return getLocationStatus ();
    }

    public static string GetPushMessageIos () {
        return getPushMessage();
    }

    [DllImport("__Internal")]
    private static extern string getkeyChain_ ();
    
    [DllImport ("__Internal")]
    private static extern void setkeyChain_ (string info);

    public static string getkeyChain () {
        return getkeyChain_ ();
    }

    public static void setkeyChain (string info) {
        // Call plugin only when running on real device
        setkeyChain_ (info);
    }



#elif UNITY_ANDROID && !UNITY_EDITOR

    //アンドロイIDの取得
    public static string GetUdid ()
    {
        AndroidJavaClass unityPlayer = new AndroidJavaClass ("com.unity3d.player.UnityPlayer");
        AndroidJavaObject currentActivity = unityPlayer.GetStatic<AndroidJavaObject> ("currentActivity");
        AndroidJavaObject contentResolver = currentActivity.Call<AndroidJavaObject> ("getContentResolver");
        AndroidJavaClass secure = new AndroidJavaClass ("android.provider.Settings$Secure");
        string uiid = secure.CallStatic<string> ("getString", contentResolver, "android_id");

        return CalcMd5(uiid);
    }

    public static string GetUiid (String bundle)
    {
        AndroidJavaClass unityPlayer = new AndroidJavaClass ("com.unity3d.player.UnityPlayer");
        AndroidJavaObject currentActivity = unityPlayer.GetStatic<AndroidJavaObject> ("currentActivity");
        AndroidJavaObject contentResolver = currentActivity.Call<AndroidJavaObject> ("getContentResolver");
        AndroidJavaClass secure = new AndroidJavaClass ("android.provider.Settings$Secure");
        string uiid = secure.CallStatic<string> ("getString", contentResolver, "android_id");

        return CalcMd5(uiid);
    }

    //仮で割り当てて置く
    public static string GetIdfv ()
    {
        string uiid = SystemInfo.deviceUniqueIdentifier;
        return uiid;
    }

    private static string GetInstallDate(AndroidJavaObject currentActivity, String bundle)
    {
        int installDate = 0;

        AndroidJavaObject getPackageManager = currentActivity.Call<AndroidJavaObject> ("getPackageManager");
        try {
            AndroidJavaObject packageInfo = getPackageManager.Call<AndroidJavaObject>( "getPackageInfo", bundle, 128 );
            installDate = (int)packageInfo.Get<long>("firstInstallTime");
        } catch {
            Debug.LogError ("Error install date not get");
            return "0";
        } finally {

        }

        return installDate.ToString ();
    }

#elif UNITY_EDITOR

    //Unity Editor で世界で唯一のIDを作成する関数
    //      public static string GetUiid ()
    //      {
    //        string uiid = Guid.NewGuid().ToString("N");
    //        return uiid;
    //      }
    //使ってるPCのIDを取得出来る事が可能。

    public static string GetUdid ()
    {
        string uiid = SystemInfo.deviceUniqueIdentifier;
        //uiid = "F7DF4B5B-7DF3-6746-B21E-33F345F36VVC";
        //uiid = "F7DF4VVV-7DF3-6746-B21E-33F345F36VVV";
        //uiid = "F7DF4VVV-7DF3-6746-B21E-33F345F36WWW";
        uiid = "F7DF4VVV-7DF3-6746-B21E-33F345F36XXX";
        return uiid;
    }

    public static string GetUiid (String bundle)
    {
        string uiid = "";
        uiid = SystemInfo.deviceUniqueIdentifier;
        //uiid = "F7DF4B5B-7DF3-6746-B21E-33F345F36VVC";
        //uiid = "F7DF4VVV-7DF3-6746-B21E-33F345F36VVV";
        //uiid = "F7DF4VVV-7DF3-6746-B21E-33F345F36WWW";
        uiid = "F7DF4VVV-7DF3-6746-B21E-33F345F36XXX";
        return uiid;

    }

    //仮で割り当てて置く
    public static string GetIdfv ()
    {
        //return SystemInfo.deviceUniqueIdentifier;
        //return "F7DF4B5B-7DF3-6746-B21E-33F345F36VVC";
        //return "F7DF4VVV-7DF3-6746-B21E-33F345F36VVV";
        //return "F7DF4VVV-7DF3-6746-B21E-33F345F36WWW";
        return "F7DF4VVV-7DF3-6746-B21E-33F345F36XXX";
    }

    #endif

    private static string CalcMd5( string srcStr )
    {
        System.Security.Cryptography.MD5 md5 = System.Security.Cryptography.MD5.Create();

        // md5ハッシュ値を求める
        byte[] srcBytes = System.Text.Encoding.UTF8.GetBytes(srcStr);
        byte[] destBytes = md5.ComputeHash(srcBytes);

        // 求めたmd5値を文字列に変換する
        System.Text.StringBuilder destStrBuilder;
        destStrBuilder = new System.Text.StringBuilder();
        foreach (byte curByte in destBytes) {
            destStrBuilder.Append(curByte.ToString("x2"));
        }

        // 変換後の文字列を返す
        return destStrBuilder.ToString();
    }
}
