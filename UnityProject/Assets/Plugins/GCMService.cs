using UnityEngine;
using System.Collections;

public class GCMService : MonoBehaviour {

	public static string _registerId = "";

	public static string _message = "";

	// Use this for initialization
	static public void Registration () {
		#if UNITY_ANDROID
			Debug.Log ("GCM Start UnityGCMPlugin");
			AndroidJavaClass unityPlayer = new AndroidJavaClass("com.tiam.gcmplugin.MainActivity");
			//AndroidJavaObject activity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");
			//AndroidJavaObject appContext = activity.Call<AndroidJavaObject>("getApplicationContext");
            AndroidJavaObject appContext = unityPlayer.GetStatic<AndroidJavaObject>("currentContext");

			Debug.Log ("GCM get RegistrationId");
			AndroidJavaClass registrar = new AndroidJavaClass ("com.tiam.gcmplugin.GcmRegistrar");

			//registrar.CallStatic ("clearCache", new object[] { appContext });

			string registrationId = registrar.CallStatic<string> ("getRegistrationId", new object[] { appContext });

			if (!string.IsNullOrEmpty (registrationId)) {
				Debug.Log ("GCM RegistrationId:[" + registrationId + "]");
			} else {
				// Invoke background thread to get registration ID
				Debug.Log ("GCM id empty");
				registrar.CallStatic("registerInBackground", new object[]{ appContext });
			}
		#endif
	}

	static public string GetRegistrationId () {
		string registrationId = "";

		#if UNITY_ANDROID
			AndroidJavaClass unityPlayer = new AndroidJavaClass("com.tiam.gcmplugin.MainActivity");
            //AndroidJavaObject activity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");
            //AndroidJavaObject appContext = activity.Call<AndroidJavaObject>("getApplicationContext");
            AndroidJavaObject appContext = unityPlayer.GetStatic<AndroidJavaObject>("currentContext");

			Debug.Log ("GCM get RegistrationId");
			AndroidJavaClass registrar = new AndroidJavaClass ("com.tiam.gcmplugin.GcmRegistrar");

			registrationId = registrar.CallStatic<string> ("getRegistrationId", new object[] { appContext });
			if (string.IsNullOrEmpty (registrationId)) {
				registrationId = "";
			}
		#endif

		return registrationId;
	}

	// <summary>
	// Callback from background thread if register completed.
	// </summary>
	// <param name="registerId">Registration ID</param>
	public void OnRegister(string registerId) {
		Debug.Log ("GCM OnRegister RegisterId: " + registerId);
		GCMService._registerId = registerId;
	}


	/// <summary>
	/// Raises the message event.
	/// </summary>
	/// <param name="message">Message.</param>
	public void OnMessage(string message) {
		Debug.Log ("GCM OnMessage Message: " + message);
		GCMService._message = message;
	}


	static public string GetPushMessage () {

		string messageID = "";

		#if UNITY_ANDROID
		AndroidJavaClass unityPlayer = new AndroidJavaClass("com.tiam.gcmplugin.MainActivity");
            //AndroidJavaObject activity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");
            //AndroidJavaObject appContext = activity.Call<AndroidJavaObject>("getApplicationContext");
            AndroidJavaObject appContext = unityPlayer.GetStatic<AndroidJavaObject>("currentContext");

		Debug.Log ("GCM Message Retrive");
		AndroidJavaClass registrar = new AndroidJavaClass ("com.tiam.gcmplugin.GcmRegistrar");

		messageID = registrar.CallStatic<string> ("getPushMessage", new object[] { appContext });
        Debug.Log  (messageID + " <= messageID Check Here");
		if (string.IsNullOrEmpty (messageID)) {
			messageID = "";
		}
		#endif
		return messageID;
	}

}
