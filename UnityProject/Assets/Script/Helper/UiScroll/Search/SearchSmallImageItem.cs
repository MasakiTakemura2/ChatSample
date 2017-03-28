using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Http;
using ModelManager;

public class SearchSmallImageItem : UIBehaviour 
{
    [SerializeField]
    private RawImage _userPict;

	[SerializeField]
	private RawImage _userPict2;

	[SerializeField]
	private RawImage _userPict3;

	[SerializeField]
	private RawImage _userPict4;
    
    [SerializeField]
    private Text _userPrefAndAge_1;
    
    [SerializeField]
    private Text _userPrefAndAge_2;
    
    [SerializeField]
    private Text _userPrefAndAge_3;

    [SerializeField]
    private Text _userPrefAndAge_4;

    /// <summary>
    /// Updates the item.
    /// </summary>
    /// <returns>The item.</returns>
    /// <param name="">.</param>
	/// public void UpdateItem (int count, MessageUserListEntity.UserList user)
	public void UpdateItem (int count, UserDataEntity.Basic user_1, UserDataEntity.Basic user_2, UserDataEntity.Basic user_3, UserDataEntity.Basic user_4,  bool userenable, bool userenable2, bool userenable3, bool userenable4)
    {
		// ユーザーid記憶
		_userPict.gameObject.name = user_1.id;
		_userPict2.gameObject.name = user_2.id;
		_userPict3.gameObject.name = user_3.id;
		_userPict4.gameObject.name = user_4.id;


		if (_userPict != null && string.IsNullOrEmpty (user_1.profile_image_url) == false)
		{
			StartCoroutine (WwwToRendering (user_1.profile_image_url, _userPict));
		}

		if (_userPict != null && string.IsNullOrEmpty (user_2.profile_image_url) == false)
		{
			StartCoroutine (WwwToRendering (user_2.profile_image_url, _userPict2));
		}

		if (_userPict != null && string.IsNullOrEmpty (user_3.profile_image_url) == false)
		{
			StartCoroutine (WwwToRendering (user_3.profile_image_url, _userPict3));
		}

		if (_userPict != null && string.IsNullOrEmpty (user_4.profile_image_url) == false)
		{
			StartCoroutine (WwwToRendering (user_4.profile_image_url, _userPict4));
		}

        if (_userPrefAndAge_1 != null ) {
            _userPrefAndAge_1.text = CommonModelHandle.GetPrefDataById (user_1.pref).name + " (" + user_1.age + LocalMsgConst.AGE_TEXT + ") ";
        }
        
        if (_userPrefAndAge_2 != null ) {
            _userPrefAndAge_2.text = CommonModelHandle.GetPrefDataById (user_2.pref).name + " (" + user_2.age + LocalMsgConst.AGE_TEXT + ") ";
        }
        
        if (_userPrefAndAge_3 != null ) {
            _userPrefAndAge_3.text = CommonModelHandle.GetPrefDataById (user_3.pref).name + " (" + user_3.age + LocalMsgConst.AGE_TEXT + ") ";
        }
        
        if (_userPrefAndAge_4 != null ) {
            _userPrefAndAge_4.text = CommonModelHandle.GetPrefDataById (user_4.pref).name + " (" + user_3.age + LocalMsgConst.AGE_TEXT + ") ";
        }
        
        
        

		_userPict.enabled = userenable;
		_userPict2.enabled = userenable2;
		_userPict3.enabled = userenable3;
		_userPict4.enabled = userenable4;

	}
    
    /// <summary>
    /// Wwws to rendering.
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
