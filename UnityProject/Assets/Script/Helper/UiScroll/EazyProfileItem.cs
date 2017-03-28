using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Http;
using ModelManager;
using EventManager;
using System;
using BestHTTP;


//NEND 用のプラグイン。
using NendUnityPlugin.AD.Native; 

public class EazyProfileItem : UIBehaviour 
{
    [SerializeField]
	private RawImage _userPict;

    [SerializeField]
    private Image _backGround;

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

    [SerializeField]
    private GameObject _addView;

    [SerializeField]
    private RawImage _addRawImage;

	[SerializeField]
	private GameObject _titleBg;

	[SerializeField]
	private GameObject _prTextBg;

	[SerializeField]
	private Text _prText;

	private string _prNendUrl = "";

    /// <summary>
    /// Nends the ad click.
    /// </summary>
	private void NendAdClick () {
		var uri = new Uri (_prNendUrl);
		Application.OpenURL (uri.AbsoluteUri);
	}

	/// <summary>
	/// Nomals the click.
	/// </summary>
	private void NomalClick () {
		SearchEventManager.Instance.ProfileOpen (_userPict.gameObject);
	}

    /// <summary>
    /// Updates the item.
    /// </summary>
    /// <returns>The item.</returns>
    /// <param name="">.</param>
	//public void UpdateItem (int count, MessageUserListEntity.UserList user)
	public void UpdateItem (int count, UserDataEntity.Basic user = null)
    {
		_userPict.gameObject.name = user.id;
		this.transform.GetComponent<Button> ().onClick.AddListener(NomalClick);

		_titleBg.SetActive (true);
		_prTextBg.SetActive(false);


        //リストの間に広告バナーを導入。
        if (user.is_banner == "1") {
			if (CommonConstants.IS_AD_NEND == true) {
				_titleBg.SetActive (false);
				_prTextBg.SetActive(true);

				AppStartLoadBalanceManager.m_NendAdClient.LoadNativeAd ((INativeAd ad, int code, string message) => {
					if (null != ad) {
						_prNendUrl = ad.PromotionUrl;
						this.transform.GetComponent<Button> ().onClick.RemoveAllListeners ();
						this.transform.GetComponent<Button> ().onClick.AddListener(NendAdClick);

						// 広告明示のテキストを取得します
						_read.text = ad.GetAdvertisingExplicitlyText (AdvertisingExplicitly.AD);
						_read.color = new Color (128/255.0f,128/255.0f,128/255.0f,255);

						// 広告見出しを取得します
						_userName.text = "<size=40>" + ad.ShortText + "</size>";

						_prText.text = ad.LongText;

						// 広告画像のTextureをダウンロードします
						new HTTPRequest(new Uri(ad.AdImageUrl), (request, response) => {
							if (_userPict != null && response != null) {
								_userPict.texture = response.DataAsTexture2D;
							}
						}).Send();

						// インプレッションの計測とクリック処理の登録を行います
						ad.Activate (this.gameObject, _userName.gameObject);
					} else {
						Debug.Log ("Failed to load ad. code ="+code+", message = " + message);
					}
				});

			}
			else
			{

				_addView.SetActive (true);
				_addView.name = user.url;

				new HTTPRequest(new Uri(user.image_url), (request, response) =>
					_addRawImage.texture = response.DataAsTexture2D).Send();
				return;
			}

            return;
        }

        if (_userName != null)
		{			
            string userNameStr = user.name.Replace ("\n", "");

            if (userNameStr.Length > 15) {
                _userName.text =  userNameStr.Substring (0, 15) + "...";
            } else {
                _userName.text = userNameStr;
            }
		}

        if (_userPlace != null)
		{
            string pref = "";
            string city = "";
            if  (string.IsNullOrEmpty (user.pref) == false)
               pref = CommonModelHandle.GetPrefDataById (user.pref).name;
            
            if  (string.IsNullOrEmpty (user.city_id) == false)
               city = CommonModelHandle.GetCityDataById (user.city_id).name;
            
            string place = string.Format ("{0} {1} {2}", user.distance, pref, city );
			_userPlace.text = place;
		}

        if (_eazyTitle != null)
		{
            string bodyStr = user.profile.Replace ("\n", "");

            if (bodyStr.Length > 15) {
                _eazyTitle.text = bodyStr.Substring (0, 15) + "...";
            } else {
                _eazyTitle.text = bodyStr;
            }
		}

		if (_dateTime != null) 
		{
			_dateTime.text = user.time_ago;
		}

		if (_read != null) 
		{
			_read.text = "";//"未読";
		}

        //プロフ表示。
        if (_userPict != null && string.IsNullOrEmpty (user.profile_image_url) == false) 
        {
            

            if (SearchEventManager.Instance._profTexCaches.ContainsKey (user.profile_image_url) == true) 
            {
                _userPict.gameObject.SetActive (true);
                _userPict.texture = SearchEventManager.Instance._profTexCaches[user.profile_image_url];
                return;
            } 
            else 
            {
                StartCoroutine(WwwToRendering(user.profile_image_url, _userPict));
            }
        }
	}
    
    /// <summary>
    /// Ads the click.
    /// </summary>
    /// <param name="adObj">Ad object.</param>
    public void AdClick (GameObject adObj) 
    {
        var uri = new Uri (adObj.name);
        Application.OpenURL (uri.AbsoluteUri);
    }

    /// <summary>
    /// Wwws to ad rendering.
    /// 広告表示用。
    /// </summary>
    /// <returns>The to ad rendering.</returns>
    /// <param name="url">URL.</param>
    /// <param name="targetObj">Target object.</param>
    private IEnumerator WwwToAdRendering (string url, RawImage targetObj)
    {
        targetObj.texture = null;
        targetObj.gameObject.SetActive (false);
        if (string.IsNullOrEmpty (url) == true)
            yield break;

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

                //Debug.Log ("掲示板プロフ読み込み： " + url);

            }
        yield break;
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
        if (string.IsNullOrEmpty (url) == true)
            yield break;

        if (SearchEventManager.Instance._profTexCaches.ContainsKey (url) == false)
        {
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
             
                if (SearchEventManager.Instance._profTexCaches.ContainsKey (url) == false) {
                    SearchEventManager.Instance._profTexCaches.Add (url, targetObj.texture);
                }
            }
        }
	}
}
