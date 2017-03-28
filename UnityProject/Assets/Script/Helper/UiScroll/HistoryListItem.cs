using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System;
using EventManager;
using Http;
using ModelManager;

/// <summary>
/// History list item.
/// </summary>
public class HistoryListItem : UIBehaviour
{
    [SerializeField]
    private RawImage _userPict;

    [SerializeField]
    private Text _userName;

    [SerializeField]
    private Text _userPlace;

    [SerializeField]
    private Text _eazyTitle;

    [SerializeField]
    private Text _dateTime;

    [SerializeField]
    private Text _read;

    /// <summary>
    /// Updates the item.
    /// </summary>
    /// <param name="count">Count.</param>
    /// <param name="id">Identifier.</param>
    /// <param name="itemName">Item name.</param>
	public void UpdateItem (int count, UserDataEntity.Basic user)
    {
        if ( user != null)
        {
            this.gameObject.name = user.id;

    		_userName.text  = user.name;
            string userNameStr = user.name.Replace ("\n", "");

            if (userNameStr.Length > 15) {
                _userName.text =  userNameStr.Substring (0, 15) + "...";
            } else {
                _userName.text = userNameStr  + "( " +user.age + LocalMsgConst.AGE_TEXT + " )";
            }

            string bodyStr = user.profile.Replace ("\n", "");

            if (bodyStr.Length > 15) {
                _eazyTitle.text = bodyStr.Substring (0, 15) + "...";
            } else {
                _eazyTitle.text = bodyStr;
            }
                  
    		_dateTime.text  = user.time_ago;
            _userPlace.text = CommonModelHandle.GetPrefDataById(user.pref).name + " " + CommonModelHandle.GetCityDataById(user.city_id).name;
            _read.text      = "";
    
            if (string.IsNullOrEmpty (user.profile_image_url) == false && _userPict != null) 
    		    StartCoroutine(WwwToRendering(user.profile_image_url, _userPict));
        }
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
