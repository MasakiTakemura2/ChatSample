using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Http;
using ModelManager;

public class PanelInformationNotifyItem : UIBehaviour 
{
    [SerializeField]
    private RawImage _profImage;

    [SerializeField]
    private Text _message;

	[SerializeField]
	private RectTransform _hukidasi;

    [SerializeField]
    private Text _read;

    [SerializeField]
    private Text _registTime;
    

    [SerializeField]
    private Text _headerTitle;    
    

    /// <summary>
    /// Updates the item.
    /// </summary>
    /// <returns>The item.</returns>
    /// <param name="">.</param>
	/// public void UpdateItem (int count, MessageUserListEntity.UserList user)
    public void UpdateItem (int countobj, MessageListEntity.MessgeData msgData, string timeAgo="")
    {

//        if (msgData.image. != null && _profImage != null)
//		{
//            StartCoroutine (WwwToRendering (msgData.profile_image_url, _profImage));
//        }
    
		if (_read != null)
		{
			_read.text = msgData.status;
		}

		if (_registTime != null) 
		{
			//_registTime.text = msgData.time_ago;
            _registTime.text = timeAgo;

		}

		if (_message != null) 
		{
			_message.text = msgData.message;
		}

		if (_headerTitle != null)
		{
            _headerTitle.text = msgData.send_user_name;
		}      
	}
  
    /// <summary>
    /// Wwws to rendering.
    /// TODO: 共通化にしていく。
    /// </summary>
    /// <returns>The to rendering.</returns>
    /// <param name="url">URL.</param>
    /// <param name="targetObj">Target object.</param>
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
