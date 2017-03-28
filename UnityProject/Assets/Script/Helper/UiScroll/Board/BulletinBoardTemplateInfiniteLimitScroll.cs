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


public class BulletinBoardTemplateInfiniteLimitScroll : UIBehaviour, IInfiniteScrollSetup
{
	[SerializeField]
	private RectTransform m_ItemBase;

	[SerializeField, Range(0, 30)]
	int m_instantateItemCount = 9;
   
    [SerializeField, Range(1, 999)]
    private int max = 30;

    public Direction direction;
    public OnItemPositionChange onUpdateItem = new OnItemPositionChange ();

	[NonSerialized]
    public List<RectTransform>	m_itemList = new List<RectTransform> ();

    protected float m_diffPreFramePosition = 0;
    protected int m_currentItemNo = 0;
    protected bool _isUpdateTrigger;

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
        get{
            return  (direction == Direction.Vertical ) ? 
                    -_RectTransform.anchoredPosition.y:
                    _RectTransform.anchoredPosition.x;
        }
    }

    private float m_itemScale = -1;
    public float ItemScale
	{
        get 
		{
            if (m_ItemBase != null && m_itemScale == -1)
			{
            	m_itemScale = (direction == Direction.Vertical ) ? 
                m_ItemBase.sizeDelta.y : 
                m_ItemBase.sizeDelta.x ;
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
		var scrollRect = GetComponentInParent<ScrollRect>();
		scrollRect.horizontal = direction == Direction.Horizontal;
		scrollRect.vertical   = direction == Direction.Vertical;
		scrollRect.content    = _RectTransform;

		m_ItemBase.gameObject.SetActive (false);
		m_diffPreFramePosition = 0;
		m_currentItemNo = 0;
		m_itemList = new List<RectTransform> ();
		StartCoroutine (Set());
    }

    /// <summary>
    /// Set this instance.
    /// </summary>
    private IEnumerator Set ()
    {
        var controllers = GetComponents<MonoBehaviour>()
                .Where(item  => item is IInfiniteScrollSetup )
                .Select(item => item as IInfiniteScrollSetup )
                .ToList();


		switch (BulletinBoardEventManager.Instance._currentSettingState) 
		{
			case BulletinBoardEventManager.CurrentSettingState.Sort:
				max = InitDataApi._httpCatchData.result.user_sort.Count;
			break;
			case BulletinBoardEventManager.CurrentSettingState.Radius:
				max = InitDataApi._httpCatchData.result.radius.Count;
			break;

			case BulletinBoardEventManager.CurrentSettingState.Sex:
				max = InitDataApi._httpCatchData.result.sex_cd.Count;
			break;

			case BulletinBoardEventManager.CurrentSettingState.BodyType:
				max = BoardListModelManager.Instance.GetNameMaster (
					AppStartLoadBalanceManager._gender,
				BulletinBoardEventManager.CurrentSettingState.BodyType
				).Count;	
			break;

		}


        for (int i=0; i < m_instantateItemCount; i++)
		{
            var item = GameObject.Instantiate (m_ItemBase) as RectTransform;
            item.SetParent (transform, false);
            item.name = i.ToString ();
            item.anchoredPosition = (direction == Direction.Vertical ) ? new Vector2 (0, -ItemScale * (i)) : new Vector2 (ItemScale * (i), 0) ;
            m_itemList.Add (item);

            item.gameObject.SetActive (true);

            foreach( var controller in controllers )
			{
                controller.OnUpdateItem(i, item.gameObject);
            }
        }
	

        foreach(var controller in controllers) 
		{
            controller.OnPostSetupItems();
        }

        _isUpdateTrigger = true;
        
		yield break;
    }

    void Update ()
    {
		if (_isUpdateTrigger == false) 
		{
			return;
		}

        while (AnchoredPosition - m_diffPreFramePosition  < -ItemScale * 2 )
        {
            m_diffPreFramePosition -= ItemScale;

            var item = m_itemList [0];
            m_itemList.RemoveAt (0);
            m_itemList.Add (item);

            var pos = ItemScale * m_instantateItemCount + ItemScale * m_currentItemNo;
            item.anchoredPosition = (direction == Direction.Vertical ) ? new Vector2 (0, -pos) : new Vector2 (pos, 0);

            onUpdateItem.Invoke (m_currentItemNo + m_instantateItemCount, item.gameObject);

            m_currentItemNo ++;
        }

        while (AnchoredPosition- m_diffPreFramePosition > 0 )
		{
            m_diffPreFramePosition += ItemScale;

            var itemListLastCount = m_instantateItemCount - 1; 
            var item = m_itemList [itemListLastCount];
            m_itemList.RemoveAt (itemListLastCount);
            m_itemList.Insert (0, item);

            m_currentItemNo --;

            var pos = ItemScale * m_currentItemNo;
            item.anchoredPosition = (direction == Direction.Vertical ) ? new Vector2 (0, -pos): new Vector2 (pos, 0);
            onUpdateItem.Invoke (m_currentItemNo, item.gameObject);
        }      
    }

	[Serializable]
	public class OnItemPositionChange : UnityEngine.Events.UnityEvent<int, GameObject>{}

    /// <summary>
    /// Raises the post setup items event.
    /// </summary>
    public void OnPostSetupItems ()
    {
		var infiniteScroll = GetComponent<BulletinBoardTemplateInfiniteLimitScroll> ();
        infiniteScroll.onUpdateItem.AddListener (OnUpdateItem);
        GetComponentInParent<ScrollRect> ().movementType = ScrollRect.MovementType.Elastic;
      
        var rectTransform = GetComponent<RectTransform> ();
        var delta = rectTransform.sizeDelta;
        delta.y   = infiniteScroll.ItemScale * (max);
        
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

			if (obj.GetComponentInChildren<BoardListDataSetItem> () != null)
            {
				switch(BulletinBoardEventManager.Instance._currentSettingState)
                {
					case BulletinBoardEventManager.CurrentSettingState.Sort:

					var sort = InitDataApi._httpCatchData.result.user_sort;
					id    = sort [itemCount].id;
					value = sort [itemCount].name;

					break;

					case BulletinBoardEventManager.CurrentSettingState.Radius:

					var radius = InitDataApi._httpCatchData.result.radius;
					id    = radius [itemCount].id;
					value = radius [itemCount].name;

					break;




					case BulletinBoardEventManager.CurrentSettingState.Sex:

						var sex = InitDataApi._httpCatchData.result.sex_cd;
						id    = sex [itemCount].id;
						value = sex [itemCount].name;

					break;


					case BulletinBoardEventManager.CurrentSettingState.BodyType:
						var bodyType = BoardListModelManager.Instance.GetNameMaster
						(
							AppStartLoadBalanceManager._gender,
							BulletinBoardEventManager.CurrentSettingState.BodyType
						);
						id    = bodyType [itemCount].name;
						value = bodyType [itemCount].name;
					break;


			
                }

				var item = obj.GetComponentInChildren<BoardListDataSetItem> ();
                item.UpdateItem (itemCount, id, value);
            }
        }
    }

}
