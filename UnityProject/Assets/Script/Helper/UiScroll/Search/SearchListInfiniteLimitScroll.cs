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


public class SearchListInfiniteLimitScroll : UIBehaviour, IInfiniteScrollSetup
{
	[SerializeField]
	private RectTransform m_ItemBase;

	[SerializeField, Range (0, 30)]
	int m_instantateItemCount = 9;

	[SerializeField, Range (1, 999)]
	private int max = 30;

	// 更新後のバーの位置確認の為 何個追加されたかそのつど覚えておく
	public int _listAddCount = 0;


	public Direction direction;

	public OnItemPositionChange onUpdateItem = new OnItemPositionChange ();

	[NonSerialized]
	public List<RectTransform>	m_itemList = new List<RectTransform> ();


	protected float m_diffPreFramePosition = 0;
	public int m_currentItemNo = 0;


	public enum Direction
	{
		Vertical,
		Horizontal,
	}



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

		while (UserListApi._success == false)
		{
			yield return (UserListApi._success == true);
		}

		max = UserListApi._httpCatchData.result.users.Count;

		//m_instantateItemCount = 10;
		m_instantateItemCount = max;

		if(m_instantateItemCount > max)
		{
			m_instantateItemCount = max;
		}

		//for (int i = 0; i < m_instantateItemCount; i++)
		for (int i = 0; i < max; i++)
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
		if(UserListApi._success == false)
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
		var infiniteScroll = GetComponent<SearchListInfiniteLimitScroll> ();
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

	public void OnUpdateItem (int itemCount, GameObject obj)
	{
		if (itemCount < 0 || itemCount >= max)
		{
			obj.SetActive (false);
		} else {
			obj.SetActive (true);
			if (obj.GetComponentInChildren<EazyProfileItem> () != null)
			{
				var item = obj.GetComponentInChildren<EazyProfileItem> ();
				UserDataEntity.Basic searchItem = UserListApi._httpCatchData.result.users.ElementAt (itemCount);
				item.UpdateItem (itemCount, searchItem);
			}
		}
	}
}
