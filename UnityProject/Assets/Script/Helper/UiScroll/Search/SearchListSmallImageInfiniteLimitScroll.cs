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


public class SearchListSmallImageInfiniteLimitScroll : UIBehaviour, IInfiniteScrollSetup
{
	[SerializeField]
	private RectTransform m_ItemBase;

	[SerializeField, Range (0, 30)]
	int m_instantateItemCount = 9;
	public void SetPanelInstantateCount(int itemcount)
	{
		m_instantateItemCount = itemcount;
	}

	[SerializeField, Range (1, 999)]
	private int max = 30;
	public void SetPanelMax(int maxCount)
	{
		max = maxCount;
	}




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

		max = 0;
		m_instantateItemCount = 0;


		// 一旦リスト全消し
		if(transform.childCount > 1)
		{
			for(int i=1; i<transform.childCount; i++)
			{
				Destroy(transform.GetChild (i).gameObject);
			}
		}

		// 表示するリストの情報追加 idが一致しないものは新規として追加する
		StartCoroutine (Set ());
	}

    private List<UserDataEntity.Basic> users;

	/// <summary>
	/// Set this instance.
	/// </summary>
	public IEnumerator Set ()
	{
		var controllers = GetComponents<MonoBehaviour> ().Where (item => item is IInfiniteScrollSetup).Select (item => item as IInfiniteScrollSetup).ToList ();

		while (UserListApi._success == false)
			yield return (UserListApi._success == true);
		
        users = new List<UserDataEntity.Basic> ();
        users = GetUserListAdjust ();
Debug.Log (users.Count + " user count display ");
		max = users.Count / 4;
        
		m_instantateItemCount = max;

		if(m_instantateItemCount > max)
		{
			m_instantateItemCount = max;
		}
		// 端数で一列にみたない場合
		if((users.Count / 4) == 0)
		{
			max = 2;
			m_instantateItemCount = 1;
		}
	
		// 4枚ずつだから...
		//todo for (int i = m_currentItemNo; i < m_currentItemNo + m_instantateItemCount; i++)
		for (int i = 0; i < max; i++)
		{
			if(i >= max)
			{
				continue;
			}

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

		if (UserListApi._success == false)
		{
			return;
		}

		if(max == 0)
		{
			return;
		}
		if(m_instantateItemCount >= max)
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
		var infiniteScroll = GetComponent<SearchListSmallImageInfiniteLimitScroll> ();
		infiniteScroll.onUpdateItem.AddListener (OnUpdateItem);
		GetComponentInParent<ScrollRect> ().movementType = ScrollRect.MovementType.Elastic;

		var rectTransform = GetComponent<RectTransform> ();
		var delta = rectTransform.sizeDelta;
		delta.y = infiniteScroll.ItemScale * max;

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
			if (obj.GetComponentInChildren<SearchSmallImageItem> () != null)
			{
				var item = obj.GetComponentInChildren<SearchSmallImageItem> ();

				bool userenable1 = true;
				bool userenable2 = true;
				bool userenable3 = true;
				bool userenable4 = true;
                UserDataEntity.Basic item1 = null;
                UserDataEntity.Basic item2 = null;
                UserDataEntity.Basic item3 = null;
                UserDataEntity.Basic item4 = null;


				if (itemCount*4 < users.Count)
				{
					item1 = users.ElementAt (itemCount*4);
				} else {
					userenable1 = false;
				}
				if (itemCount*4 + 1 < users.Count)
				{
					item2 = users.ElementAt (itemCount*4 + 1);
				} else {
					userenable2 = false;
				}
				if (itemCount*4 + 2 < users.Count)
				{
					item3 = users.ElementAt (itemCount*4 + 2);
				} else {
					userenable3 = false;
				}
				if (itemCount*4 + 3 < users.Count)
				{
					item4 = users.ElementAt (itemCount*4 + 3);
				} else {
					userenable4 = false;
				}

				item.UpdateItem (itemCount, item1, item2, item3, item4, userenable1, userenable2, userenable3, userenable4);
			}
		}
	}
    
    /// <summary>
    /// Gets the user list adjust.
    /// </summary>
    /// <returns>The user list adjust.</returns>
    private List<UserDataEntity.Basic> GetUserListAdjust () {
        var res     = new List<UserDataEntity.Basic> ();
        var p_users = UserListApi._httpCatchData.result.users;
        
        foreach ( var u in p_users) 
        {
            if (u.is_banner == "1") {
                continue;
            } else {
                res.Add (u);
            }
        }
        return res;
    }
    
}
