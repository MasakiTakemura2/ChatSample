using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using System.Linq;
using UnityEngine.EventSystems;
using Http;
using uTools;

public class PanelEazyNotifyInfiniteScroll : UIBehaviour, IInfiniteScrollSetup
{
	[SerializeField]
	private RectTransform m_ItemBase;

	[SerializeField, Range(0, 30)]
	int m_instantateItemCount = 10;
    
    [SerializeField, Range (1, 999)]
    private int max = 30;

    [SerializeField]
    private GameObject _loadingOverlay;

	public Direction direction;

	public OnItemPositionChange onUpdateItem = new OnItemPositionChange ();

	[System.NonSerialized]
	public List<RectTransform>	m_itemList = new List<RectTransform> ();

    [SerializeField]
    private Scrollbar _scrollBar;


	protected float m_diffPreFramePosition = 0;

	protected int m_currentItemNo = 0;
    
    protected bool _isUpdateTrigger;
    

	private string _displayType = "";
	public string GetDisplayType()
	{
		return _displayType;
	}

	public enum Direction
	{
		Vertical,
		Horizontal,
	}
	// cache component

	private RectTransform m_rectTransform;
	protected RectTransform _RectTransform {
		get {
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
	public float ItemScale {
		get {
			if (m_ItemBase != null && m_itemScale == -1) {
                m_itemScale = m_ItemBase.sizeDelta.y;
			}
			return m_itemScale;
		}
	}
    private int iCount = 0;

    /// <summary>
    /// Init this instance.
    /// </summary>
	public void Init (string displayType)
	{
		_displayType = displayType;


        // create items      
        var scrollRect = GetComponentInParent<ScrollRect>();
        
        scrollRect.horizontal = direction == Direction.Horizontal;
        scrollRect.vertical   = direction == Direction.Vertical;
        scrollRect.content    = _RectTransform;

        m_ItemBase.gameObject.SetActive (false);
        m_diffPreFramePosition = 0;
        m_currentItemNo = 0;
        _scrollBar.value = 1f;
		iCount = 0;
        m_instantateItemCount = 10;

		// 子リスト一旦全削除
		if(transform.childCount > 1)
		{
			for(int i=1; i<transform.childCount; i++)
			{
				Destroy(transform.GetChild (i).gameObject);
			}
		}

        m_itemList = new List<RectTransform> ();
        StartCoroutine (Set());
    }
    
    /// <summary>
    /// Set this instance.
    /// </summary>
	private IEnumerator Set ()
    {
        var controllers = GetComponents<MonoBehaviour> ()
				.Where (item => item is IInfiniteScrollSetup)
				.Select (item => item as IInfiniteScrollSetup)
				.ToList ();
        _loadingOverlay.SetActive (true);


        new MessageUserListApi (_displayType);
        while (MessageUserListApi._success == false)
            yield return (MessageUserListApi._success == true);

        _loadingOverlay.SetActive (false);

        // create items
        var scrollRect = GetComponentInParent<ScrollRect> ();
        scrollRect.vertical = direction == Direction.Vertical;
        scrollRect.content = _RectTransform;

        m_ItemBase.gameObject.SetActive (false);
        max = MessageUserListApi._httpCatchData.result.users.Count;

        if (max == 0) {
            Debug.Log ("MessageUserListApi._httpCatchData.result.users.Count ==^=^====^=^==^=^=====> " + max);
            yield break;
        } else {
            Debug.Log (max + " <<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<< ");
        }
        
        foreach (var user in MessageUserListApi._httpCatchData.result.users) {
            //「1」の場合は不適切な発言等による削除
            //if (user.disable == "0") 
            {
                if (m_instantateItemCount == iCount)
                    break;

                var item = Instantiate (m_ItemBase) as RectTransform;

                item.SetParent (transform, false);
                item.anchoredPosition = new Vector2 (0, -ItemScale * (iCount));
                m_itemList.Add (item);
                item.gameObject.SetActive (true);

                foreach (var controller in controllers) {
                    controller.OnUpdateItem (iCount, item.gameObject);
                }
                iCount++;
            }
        }

        if (max <= m_instantateItemCount) {
            m_instantateItemCount = max;
        }
         

		foreach( var controller in controllers ) {
			controller.OnPostSetupItems();
		}

        _isUpdateTrigger = true;
        yield break;
	}

	void Update ()
    {
        if (max <= m_instantateItemCount) {
            return;
        }

        if (_isUpdateTrigger == false) {
            return;
        }

        while (AnchoredPosition - m_diffPreFramePosition < -ItemScale * 2) {
            if (m_instantateItemCount < iCount)
                return;

            m_diffPreFramePosition -= ItemScale;
            if (m_itemList.Count > 0) {
                var item = m_itemList [0];
                m_itemList.RemoveAt (0);
                m_itemList.Add (item);

                var pos = ItemScale * m_instantateItemCount + ItemScale * m_currentItemNo;
                item.anchoredPosition = new Vector2 (0, -pos);

                onUpdateItem.Invoke (m_currentItemNo + m_instantateItemCount, item.gameObject);

                m_currentItemNo++;
            }


			
        }
        while (AnchoredPosition - m_diffPreFramePosition > 0) {
            if (m_instantateItemCount > iCount)
                return;
            m_diffPreFramePosition += ItemScale;


            if (m_itemList.Count > 0) {
                var itemListLastCount = m_instantateItemCount - 1; 
                var item = m_itemList [itemListLastCount];
                m_itemList.RemoveAt (itemListLastCount);
                m_itemList.Insert (0, item);

                m_currentItemNo --;

                var pos = ItemScale * m_currentItemNo;
                item.anchoredPosition = new Vector2 (0, -pos);
                onUpdateItem.Invoke (m_currentItemNo, item.gameObject);
            }
		}

	}

	[System.Serializable]
	public class OnItemPositionChange : UnityEngine.Events.UnityEvent<int, GameObject>{}
    
    
    /// <summary>
    /// Raises the post setup items event.
    /// </summary>
    public void OnPostSetupItems ()
    {
        var infiniteScroll = GetComponent<PanelEazyNotifyInfiniteScroll> ();
        infiniteScroll.onUpdateItem.AddListener (OnUpdateItem);
        GetComponentInParent<ScrollRect> ().movementType = ScrollRect.MovementType.Elastic;

        var rectTransform = GetComponent<RectTransform> ();
        var delta = rectTransform.sizeDelta;
        delta.y = infiniteScroll.ItemScale * (max);

        rectTransform.sizeDelta = delta;
    }

    /// <summary>
    /// Ons the update item.
    /// </summary>
    /// <returns>The update item.</returns>
    /// <param name="itemCount">Item count.</param>
    /// <param name="obj">Object.</param>
    public void OnUpdateItem (int itemCount, GameObject obj)
    {
        if (itemCount < 0 || itemCount >= max) {
            obj.SetActive (false);
        } else {
            obj.SetActive (true);
if (ViewController.PanelTalkList.Instance._displayState == ViewController.PanelTalkList.DisplayState.DeleteMode) {
    obj.transform.GetComponent<Button> ().enabled = false;
    obj.transform.GetComponent<uTweenPosition> ().delay    = 0.001f;
    obj.transform.GetComponent<uTweenPosition> ().duration = 0.25f;
    obj.transform.GetComponent<uTweenPosition> ().from = Vector3.zero;
    obj.transform.GetComponent<uTweenPosition> ().to = new Vector3 (250f, obj.transform.localPosition.y, 0);
    obj.transform.GetComponent<uTweenPosition> ().enabled = true;
}
            if (obj.GetComponentInChildren<PanelEazyNotifyItem> () != null) {
                var item = obj.GetComponentInChildren<PanelEazyNotifyItem> ();
                //BoardListEntity.Board boardItem = BoardListApi._httpCatchData.result.boards.ElementAt (itemCount);
                //Image Loading from URL not yet
                List<MessageUserListEntity.UserList> data = MessageUserListApi._httpCatchData.result.users;
                if (_displayType == "2") {
                    item.UpdateItem (itemCount, data[itemCount], true);
                } else {
                    item.UpdateItem (itemCount, data[itemCount]);
                }

            }
        }
    }
}
