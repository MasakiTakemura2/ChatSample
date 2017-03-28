using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using System.Linq;
using UnityEngine.EventSystems;
using Http;

public class PanelInformationInfiniteScroll : UIBehaviour, IInfiniteScrollSetup
{
	[SerializeField]
	private RectTransform m_ItemBase;

	[SerializeField, Range(0, 30)]
	int m_instantateItemCount = 9;
    
    [SerializeField, Range (1, 999)]
    private int max = 30;

    [SerializeField]
    private GameObject _loadingOverlay;

	public Direction direction;

	public OnItemPositionChange onUpdateItem = new OnItemPositionChange ();

	[System.NonSerialized]
	public List<RectTransform>	m_itemList = new List<RectTransform> ();

	protected float m_diffPreFramePosition = 0;

	protected int m_currentItemNo = 0;
    
	public string _displayType = "";
	public string _message = "";
    public MessageListEntity.MessgeData _showPanelUserEntity = null;


	public enum Direction
	{
		Vertical,
		Horizontal,
	}
	// cache component

	private RectTransform m_rectTransform;
	protected RectTransform _RectTransform 
	{
		get 
		{
			if (m_rectTransform == null)
				m_rectTransform = GetComponent<RectTransform> ();
			return m_rectTransform;
		}
	}

	private float AnchoredPosition
	{
		get{
            return -_RectTransform.anchoredPosition.y;
                    
		}
	}

	private float m_itemScale = -1;
	public float ItemScale
	{
		get
		{
			if (m_ItemBase != null && m_itemScale == -1) 
			{
                m_itemScale = m_ItemBase.sizeDelta.y;
			}
			return m_itemScale;
		}
	}
    private int iCount = 0;

    /// <summary>
    /// Init this instance.
    /// </summary>
	public void Init (string id)
	{
        // create items      
        var scrollRect = GetComponentInParent<ScrollRect>();
        scrollRect.horizontal = direction == Direction.Horizontal;
        scrollRect.vertical   = direction == Direction.Vertical;
        scrollRect.content    = _RectTransform;

        m_ItemBase.gameObject.SetActive (false);
        m_diffPreFramePosition = 0;
        m_currentItemNo = 0;
		iCount = 0;
        GetComponent<RectTransform> ().sizeDelta = Vector2.zero;

		// 子リスト一旦全削除
		if(transform.childCount > 1)
		{
			for(int i=1; i<transform.childCount; i++)
			{
				Destroy(transform.GetChild (i).gameObject);
			}
		}


        m_itemList = new List<RectTransform> ();
        StartCoroutine (Set(id));
    }

    private string _timeAgo;
    /// <summary>
    /// Set this instance.
    /// </summary>
	private IEnumerator Set(string id)
	{
		var controllers = GetComponents<MonoBehaviour>().Where(item => item is IInfiniteScrollSetup ).Select(item => item as IInfiniteScrollSetup ).ToList();

		// create items
		var scrollRect      = GetComponentInParent<ScrollRect>();
		scrollRect.vertical = direction == Direction.Vertical;
		scrollRect.content  = _RectTransform;

        _loadingOverlay.SetActive (true);
        if (EventManager.MessageEventManager.Instance._isFromPush == false) {
            EventManager.MessageEventManager.Instance._isFromPush = false;
            string[] split = id.Split('-');

            id = split[0];

            if (split.Length > 0) 
                _timeAgo = split [1];
        }

        new MessageListApi (id);
        while (MessageListApi._success == false)
            yield return (MessageListApi._success == true);

        _loadingOverlay.SetActive (false);

		m_ItemBase.gameObject.SetActive (false);
        max = 1;//MessageListApi._httpCatchData.result.users.Count;
        if (max == 0)
		{
            Debug.Log ("MessageListApi._httpCatchData.result.users.Count ==^=^====^=^==^=^=====> " + max);
            yield break;
        } else {
            Debug.Log (max + " <<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<< ");
        }
        
        foreach ( var user in MessageListApi._httpCatchData.result.messages) 
        {
            var item = Instantiate (m_ItemBase) as RectTransform;
			_showPanelUserEntity = user;

            item.SetParent (transform, false);
            item.name = user.receive_user_id;
            item.anchoredPosition = new Vector2 (0, -ItemScale * (iCount));
            m_itemList.Add (item);
            item.gameObject.SetActive (true);

            foreach (var controller in controllers) 
			{
                controller.OnUpdateItem (iCount, item.gameObject);
            }
            iCount++;

			break;
        }

		foreach( var controller in controllers )
		{
			controller.OnPostSetupItems();
		}

        yield break;
	}

	[System.Serializable]
	public class OnItemPositionChange : UnityEngine.Events.UnityEvent<int, GameObject>{}
    
    
    /// <summary>
    /// Raises the post setup items event.
    /// </summary>
    public void OnPostSetupItems ()
    {
        var infiniteScroll = GetComponent<PanelInformationInfiniteScroll> ();
        infiniteScroll.onUpdateItem.AddListener (OnUpdateItem);
        GetComponentInParent<ScrollRect> ().movementType = ScrollRect.MovementType.Elastic;

        var rectTransform = GetComponent<RectTransform> ();
        var delta = rectTransform.sizeDelta;

		// ※１個しかないだろうからここで表示領域の調整
        //検索する文字列
        string s = MessageListApi._httpCatchData.result.messages[0].message;
		string searchWord = "\n";	// 改行文字を探せ
		const int LineTextMax = 21; //文字幅による改行カウントの文字数
		const int LineSize = 23; 	// 一行分の縦サイズ
		int count = 0;

		int foundIndex = s.IndexOf(searchWord);
		int foundIndexBefore = s.IndexOf(searchWord);
		while (0 <= foundIndex)
		{
			//次の検索開始位置
			int nextIndex = foundIndex + searchWord.Length;
			if (nextIndex < s.Length)
			{
				//次の位置を探す
				foundIndex = s.IndexOf(searchWord, nextIndex);

				// １行分の文字幅数をこえているなら改行文字がなくても改行とみなす
				if(foundIndex - nextIndex > LineTextMax)
				{
					count += (foundIndex - nextIndex)/LineTextMax;
				}
				count++;
			}else{
				//最後まで検索したときは終わる
				break;
			}
		}
        //改行がない場合。半角スペースで改行取られるので半角スペース文をcountから引くとちょうど良い。
        if ( count == 0) {
            string[] l =  s.Split(' ');

            count = s.Length / LineTextMax;
//Debug.Log (LineTextMax + " aaaaaaaaaaaaaaaaaaaaaa " + msg.message.Length);
//Debug.Log (count  + " countcountcountcountcountcountcountcountcountcountcount ");
            count = count / 2;
            count = count - (l.Length - 2);
        }

		Vector2 size = this.GetComponent<RectTransform> ().sizeDelta;
		size.y = 300 + (LineSize * 2 * count) + (_message.Length * 2);

		this.GetComponent<RectTransform> ().sizeDelta = size;

		/*
		size = obj.GetComponent<RectTransform> ().sizeDelta;
		size.y += (data[itemCount].message.Length * 2) + (LineTextMax*2*count);
		obj.GetComponent<RectTransform> ().sizeDelta = size;
		*/
        //delta.y = infiniteScroll.ItemScale * (max);
        //rectTransform.sizeDelta = delta;
    }

    /// <summary>
    /// Ons the update item.
    /// </summary>
    /// <returns>The update item.</returns>
    /// <param name="itemCount">Item count.</param>
    /// <param name="obj">Object.</param>
    public void OnUpdateItem (int itemCount, GameObject obj)
    {
        if (itemCount < 0 || itemCount >= max) 
		{
            obj.SetActive (false);
        } else {
            obj.SetActive (true);
			if (obj.GetComponentInChildren<PanelInformationNotifyItem> () != null) 
			{
				var item = obj.GetComponentInChildren<PanelInformationNotifyItem> ();
                //List<MessageUserListEntity.UserList> data = MessageUserListApi._httpCatchData.result.users;

                item.UpdateItem (itemCount, _showPanelUserEntity, _timeAgo);
            }
        }
    }
}
