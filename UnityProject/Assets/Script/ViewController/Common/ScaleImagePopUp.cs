using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class ScaleImagePopUp : MonoBehaviour 
{
	[SerializeField]
	private RawImage _scaleImage;

	public void SetRawImage(string url)
	{
		if (url != "") 
		{
			StartCoroutine (WwwToRendering (url, _scaleImage));
		} else {
			_scaleImage.texture = null;
		}
	}

	private IEnumerator WwwToRendering (string url, RawImage targetObj)
	{
        targetObj.texture = null;
        targetObj.gameObject.SetActive (false);
		if (string.IsNullOrEmpty (url) == true) yield break;

		using (WWW www = new WWW (url))
		{
			while (www == null)
				yield return (www != null);

			while (www.isDone == false)
				yield return (www.isDone);

			//non texture file
			if (string.IsNullOrEmpty (www.error) == false) 
			{
				Debug.LogError (www.error);
				Debug.Log (url);
				yield break;
			}
			while (targetObj == null)
				yield return (targetObj != null);

            targetObj.gameObject.SetActive (true);
			targetObj.texture = www.texture;

		}
	}
}
