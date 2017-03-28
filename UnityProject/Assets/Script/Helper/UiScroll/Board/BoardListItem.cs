using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System;
using EventManager;
using Http;
using ViewController;
using BestHTTP;

//NEND 用のプラグイン。
using NendUnityPlugin.AD.Native; 

public class BoardListItem : UIBehaviour 
{
    [SerializeField]
    private Text _name;

    [SerializeField]
	private GameObject _userPict;

	[SerializeField]
	private Text _category;

	[SerializeField]
	private Text body;

	[SerializeField]
	private Text _dateTime;

	[SerializeField]
	private RawImage _profImage;

    [SerializeField]
    private GameObject _addView;
    
    [SerializeField]
    private RawImage _addRawImage;

	[SerializeField] private GameObject _bodyBg;
	[SerializeField] private GameObject _categoryBg;
	[SerializeField] private GameObject _prLongBg;

	[SerializeField] private Text _prShortText;
	[SerializeField] private Text _prLongText;


    
	// どのパネル押したのか知るため
	public string _id = "";

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
		SelectTap ();
	}


    /// <summary>
    /// Updates the item.
    /// </summary>
    /// <returns>The item.</returns>
    /// <param name="count">Count.</param>
    /// <param name="board">Board.</param>
	public void UpdateItem (int count, BoardListEntity.Board board = null)
    {        
        if (board == null) return;

		this.transform.GetComponent<Button> ().onClick.AddListener(NomalClick);
		_categoryBg.SetActive (true);
		_bodyBg.SetActive (true);
		_prLongBg.SetActive (false);

		//広告表示の場合の処理。
        if (board.is_banner == "1") {
			if (CommonConstants.IS_AD_NEND == true) {
				_categoryBg.SetActive (false);
				_bodyBg.SetActive (false);
				_prLongBg.SetActive (true);
				AppStartLoadBalanceManager.m_NendAdClient.LoadNativeAd ((INativeAd ad, int code, string message) => {
					
					if (null != ad) {
						_prNendUrl = ad.PromotionUrl;
						this.transform.GetComponent<Button> ().onClick.RemoveAllListeners ();
						this.transform.GetComponent<Button> ().onClick.AddListener(NendAdClick);

						// 広告明示のテキストを取得します
						_prShortText.text = ad.GetAdvertisingExplicitlyText (AdvertisingExplicitly.AD);
						_prShortText.color = new Color (128/255.0f,128/255.0f,128/255.0f,255);

						// 広告見出しを取得します
						_name.text = "<size=35>" + ad.ShortText + "</size>";

						_prLongText.text = ad.LongText;

						// 広告画像のTextureをダウンロードします
						new HTTPRequest(new Uri(ad.AdImageUrl), (request, response) => {
							if (_profImage != null && response != null) {
								_profImage.texture = response.DataAsTexture2D;
							}
						}).Send();

						// インプレッションの計測とクリック処理の登録を行います
						ad.Activate (this.gameObject, _profImage.gameObject);
					} else {
						Debug.Log ("Failed to load ad. code ="+code+", message = " + message);
					}
				});

			} else {
				_addView.SetActive (true);
				_addView.name = board.url;
				new HTTPRequest(new Uri(board.image_url), (request, response) =>
					_addRawImage.texture = response.DataAsTexture2D).Send();				
			}
            return;
        }

		_id = board.id;
        gameObject.name = board.id;
		_name.text      = board.title;
		_category.text  = board.board_category_name;
        string bodyStr  = board.body.Replace ("\n", "");


        if (bodyStr.Length > 12) {
            body.text = bodyStr.Substring (0, 12) + "...";
        } else {
            body.text = bodyStr;
        }

		_dateTime.text  = board.time_ago;
      
        if (string.IsNullOrEmpty (board.user.profile_image_url) == false ) 
        {
            if (BulletinBoardEventManager.Instance._profTexCaches.ContainsKey (board.user.profile_image_url) == true) 
            {
                _profImage.gameObject.SetActive (true);
                _profImage.texture = BulletinBoardEventManager.Instance._profTexCaches[board.user.profile_image_url];
                return;
            } 
            else 
            {
                StartCoroutine( WwwToRendering(board.user.profile_image_url, _profImage));    
            }
        }
	}

	public void SelectTap()
	{
		BulletinBoardEventManager.Instance.BoardDetailOpen (PanelBoardDetail.Instance.gameObject);

		PanelBoardDetail.Instance._boadID = _id;
		PanelBoardDetail.Instance.Init();
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
    /// <param name="userId">User identifier.</param>
	private IEnumerator WwwToRendering (string url, RawImage targetObj)
    {
        targetObj.texture = null;
        targetObj.gameObject.SetActive (false);
        if (string.IsNullOrEmpty (url) == true)
            yield break;

        if (BulletinBoardEventManager.Instance._profTexCaches.ContainsKey (url) == false)
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

                //Debug.Log ("掲示板プロフ読み込み： " + url);

            }

            if (BulletinBoardEventManager.Instance._profTexCaches.ContainsKey (url) == false) 
            {
                BulletinBoardEventManager.Instance._profTexCaches.Add (url, targetObj.texture);
            }
        }
	}
}
