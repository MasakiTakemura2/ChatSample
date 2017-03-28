using UnityEngine;
using System.Collections;
using Http;

public class launch : SingletonMonoBehaviour<launch> {

    // Use this for initialization
    IEnumerator Start ()
    {
        while (GetUserApi._httpCatchData == null)
            yield return (GetUserApi._httpCatchData != null);

if (GetUserApi._httpCatchData.result.review == "false") 
{
		#if !UNITY_EDITOR && UNITY_ANDROID
			Partytrack.start(7482, "97552baa0c63b7c42ab2b0ea69afab0d");
		#elif !UNITY_EDITOR && UNITY_IOS
			Partytrack.start(8804, "a87e358e406dac13b9a18257d4962962");
		#endif
}

        yield break;


	}
}
