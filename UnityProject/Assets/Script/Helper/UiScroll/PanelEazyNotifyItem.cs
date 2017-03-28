using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Http;
using ModelManager;
using ViewController;

public class PanelEazyNotifyItem : UIBehaviour 
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
    
    
    public GameObject _delObj;

    /// <summary>
    /// Updates the item.
    /// </summary>
    /// <returns>The item.</returns>
    /// <param name="">.</param>
	/// public void UpdateItem (int count, MessageUserListEntity.UserList user)
    public void UpdateItem (int count, MessageUserListEntity.UserList user, bool isInfo = false)
    {
        if (isInfo == true) {
            this.gameObject.name = "info_" + user.user.id + "-" + user.time_ago;
        } else {
            this.gameObject.name = user.user.id;
        }

        _delObj.name = user.user.id;
        
        if (PanelTalkList.Instance._displayState == PanelTalkList.DisplayState.ListShow) {
            _delObj.GetComponent<Toggle> ().isOn = false;
            this.GetComponent<Button> ().enabled = true;
        } else {
            if (PanelTalkList.Instance._msgDeleteList.Count > 0) {
                if (PanelTalkList.Instance._msgDeleteList.Contains (_delObj.name) == false) {
                    _delObj.GetComponent<Toggle> ().isOn = false;
                } else if (PanelTalkList.Instance._msgDeleteList.Contains (_delObj.name) == true) {
                    _delObj.GetComponent<Toggle> ().isOn = true;
                }
            }
        }

        if (_userPict != null && string.IsNullOrEmpty (user.user.profile_image_url) == false)
            StartCoroutine (WwwToRendering (user.user.profile_image_url, _userPict));

        if (_userName != null) {
            string userNameStr = user.user.name.Replace ("\n", "");

            if (userNameStr.Length > 15) {
                _userName.text =  userNameStr.Substring (0, 15) + "...";
            } else {
                _userName.text = userNameStr + "( " +user.user.age + LocalMsgConst.AGE_TEXT + " )";
            }
        }

        if (_userPlace != null)
            _userPlace.text = CommonModelHandle.GetPrefDataById(user.user.pref).name + " " + CommonModelHandle.GetCityDataById(user.user.city_id).name;

        if (_eazyTitle != null) {
            if (user.message.Length > 20) {
                _eazyTitle.text = user.message.Substring (0, 20) + "…";
            } else {
                _eazyTitle.text = user.message;
            }
        }

        if (_dateTime != null)
            _dateTime.text = user.time_ago;

        if (_read != null) {
            if (EventManager.MessageEventManager.Instance._msgReads.Contains (user.user.id)) {
                _read.text = "";
            } else {
                _read.text = user.status;
            }
        }
	}
    
    /// <summary>
    /// Mses the delete set.
    /// </summary>
    /// <returns>The delete set.</returns>
    public void MsgDeleteSet( GameObject id )
    {
        if (id.GetComponent<Toggle> ().isOn == false) {
            PanelTalkList.Instance._msgDeleteList.Remove (id.name);
        } else if (id.GetComponent<Toggle> ().isOn == true) {
            if (PanelTalkList.Instance._msgDeleteList.Contains(id.name) == false) 
                PanelTalkList.Instance._msgDeleteList.Add (id.name); 
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
