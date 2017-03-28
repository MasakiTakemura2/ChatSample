using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Http;
using ModelManager;

public class SearchLargeImageItem : UIBehaviour 
{
	[SerializeField]
	private RawImage _userPict;
	[SerializeField]
	private Text _userName;

	[SerializeField]
	private RawImage _userPict2;
	[SerializeField]
	private Text _userName2;

     /// <summary>
     /// Updates the item.
     /// </summary>
     /// <returns>The item.</returns>
     /// <param name="count">Count.</param>
     /// <param name="imageurl1">Imageurl1.</param>
     /// <param name="imageurl2">Imageurl2.</param>
     /// <param name="name1">Name1.</param>
    /// <param name="name2">Name2.</param>
    /// <param name="userid1">Userid1.</param>
    /// <param name="userid2">Userid2.</param>
	public void UpdateItem (int count, string imageurl1, string imageurl2, string name1, string name2, string userid1, string userid2, bool enable1, bool enable2)
	{
		_userPict.gameObject.name = userid1;
		_userPict2.gameObject.name = userid2;

		if (imageurl1 != "") 
		{	
			StartCoroutine (WwwToRendering (imageurl1, _userPict));
		}
		_userName.text = name1;

		if (imageurl2 != "")
		{	
			StartCoroutine (WwwToRendering (imageurl2, _userPict2));
		}
		_userName2.text = name2;


		// 端分の非アクティブ化
		if(!enable1) 
		{
			_userPict.transform.GetChild (0).gameObject.SetActive (false);
		}
		if(!enable2) 
		{
			_userPict2.transform.GetChild (0).gameObject.SetActive (false);
		}
		_userPict.enabled = enable1;
		_userPict2.enabled = enable2;

	}
    
    private IEnumerator WwwToRendering (string url, RawImage targetObj)
    {
        targetObj.texture = null;
        targetObj.gameObject.SetActive (false);
        if (string.IsNullOrEmpty (url) == true) yield break;
        using (WWW www = new WWW (url)) {
            while (www == null)
                yield return (www != null);

            while (www.isDone == false)
                yield return (www.isDone);

            //non texture file
            if (string.IsNullOrEmpty (www.error) == false) {
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
