using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using System.Linq;
using UnityEngine.EventSystems;
using Http;
using ModelManager;
using EventManager;


public class HistoryListInfiniteLimitScroll : UIBehaviour, IInfiniteScrollSetup
{
	[SerializeField]
	private RectTransform m_ItemBase;

	[SerializeField, Range (0, 30)]
	int m_instantateItemCount = 9;

	[SerializeField, Range (1, 999)]
	private int max = 30;

	public Direction direction;

	public OnItemPositionChange onUpdateItem = new OnItemPositionChange ();

	[NonSerialized]
	public List<RectTransform>	m_itemList = new List<RectTransform> ();


	protected float m_diffPreFramePosition = 0;
	protected int m_currentItemNo = 0;


	public enum Direction
	{
		Vertical,
		Horizontal,
	}

	protected bool _isHttp = false;

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
		get 
		{
			return  (direction == Direction.Vertical) ? -_RectTransform.anchoredPosition.y :_RectTransform.anchoredPosition.x;
		}
	}

	private float m_itemScale = -1;

	public float ItemScale 
	{
		get 
		{
			if (m_ItemBase != null && m_itemScale == -1) 
			{
				m_itemScale = (direction == Direction.Vertical) ? m_ItemBase.sizeDelta.y : m_ItemBase.sizeDelta.x;
			}
			return m_itemScale;
		}
	}

    /// <summary>
    /// Init this instance.
    /// </summary>
	public void Init ()
	{
		// create items      
		var scrollRect = GetComponentInParent<ScrollRect> ();
		scrollRect.horizontal = direction == Direction.Horizontal;
		scrollRect.vertical = direction == Direction.Vertical;
		scrollRect.content = _RectTransform;

		m_ItemBase.gameObject.SetActive (false);
		m_itemList = new List<RectTransform> ();

		m_diffPreFramePosition = 0;
		m_currentItemNo = 0;

		// 一旦リスト全消し
		if(transform.childCount > 1)
		{
			for(int i=1; i<transform.childCount; i++)
			{
				Destroy(transform.GetChild (i).gameObject);
			}
		}
		StartCoroutine (Set ());
	}

	/// <summary>
	/// Set this instance.
	/// </summary>
	public IEnumerator Set ()
	{
		var controllers = GetComponents<MonoBehaviour> ().Where (item => item is IInfiniteScrollSetup).Select (item => item as IInfiniteScrollSetup).ToList ();

		max = HistoryUserListApi._httpCatchData.result.users.Count;
		_isHttp = true;
        
        m_instantateItemCount = max;

        if(m_instantateItemCount > max)
        {
            m_instantateItemCount = max;    
        }

		for (int i = 0; i < m_instantateItemCount; i++)
		{
			var item = Instantiate (m_ItemBase) as RectTransform;
			item.SetParent (transform, false);
			item.name = i.ToString ();
			item.anchoredPosition = (direction == Direction.Vertical) ? new Vector2 (0, -ItemScale * (i)) : new Vector2 (ItemScale * (i), 0);
			m_itemList.Add (item);
			item.gameObject.SetActive (true);

			foreach (var controller in controllers) 
			{
				controller.OnUpdateItem (i, item.gameObject);
			}
		}

		foreach (var controller in controllers) 
		{
			controller.OnPostSetupItems ();
		}
		yield break;
	}

	void Update ()
	{
		if (_isHttp == false) 
		{
			return;
		}
	}

	[Serializable]
	public class OnItemPositionChange : UnityEngine.Events.UnityEvent<int, GameObject>
	{

	}

	/// <summary>
	/// Raises the post setup items event.
	/// </summary>
	public void OnPostSetupItems ()
	{
		var infiniteScroll = GetComponent<HistoryListInfiniteLimitScroll> ();
		infiniteScroll.onUpdateItem.AddListener (OnUpdateItem);
		GetComponentInParent<ScrollRect> ().movementType = ScrollRect.MovementType.Elastic;

		var rectTransform = GetComponent<RectTransform> ();
		var delta = rectTransform.sizeDelta;
		delta.y = infiniteScroll.ItemScale * (max);

		rectTransform.sizeDelta = delta;
	}

	/// <summary>
	/// Raises the update item event.
	/// </summary>
	/// <param name="itemCount">Item count.</param>
	/// <param name="obj">Object.</param>
	protected string id;
	protected string value;

	public void OnUpdateItem (int itemCount, GameObject obj)
	{
		if (itemCount < 0 || itemCount >= max) 
		{
			obj.SetActive (false);
		} else {
			obj.SetActive (true);
			if (obj.GetComponentInChildren<HistoryListItem> () != null)
			{
				var item = obj.GetComponentInChildren<HistoryListItem> ();
                if (HistoryUserListApi._httpCatchData != null) {
                    var users = HistoryUserListApi._httpCatchData.result.users;
                    //Image Loading from URL not yet
    				item.UpdateItem (itemCount, users[itemCount]);
                }
			}
		}

	}
}
