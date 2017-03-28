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
using ViewController;


public class ProfTemplateInfiniteLimitScroll : UIBehaviour, IInfiniteScrollSetup
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

    public enum Direction {
		Vertical,
		Horizontal,
	}
    
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
            return  (direction == Direction.Vertical ) ? 
                    -_RectTransform.anchoredPosition.y:
                    _RectTransform.anchoredPosition.x;
        }
    }

    private float m_itemScale = -1;
    public float ItemScale {
        get {
            if (m_ItemBase != null && m_itemScale == -1) {
                    m_itemScale = (direction == Direction.Vertical ) ? 
                    m_ItemBase.sizeDelta.y : 
                    m_ItemBase.sizeDelta.x ;
            }
            return m_itemScale;
        }
    }

    /// <summary>
    /// Init this instance.
    /// </summary>e
    public void Init () {
      // create items      
          var scrollRect = GetComponentInParent<ScrollRect>();
          scrollRect.horizontal = direction == Direction.Horizontal;
          scrollRect.vertical   = direction == Direction.Vertical;
          scrollRect.content    = _RectTransform;

          this.transform.GetComponent<RectTransform> ().anchoredPosition = Vector2.zero;

          // 初期化 一旦全消し
          if (transform.childCount > 1) {
              for (int i = 1; i < transform.childCount; i++) {
                  Destroy (transform.GetChild (i).gameObject);
              }
          }

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

        if (MypageEventManager.Instance) {
            switch (MypageEventManager.Instance._currentProfSettingState) 
            {
            case CurrentProfSettingStateType.Pref:
                max = InitDataApi._httpCatchData.result.pref.Count;
                break;
            case CurrentProfSettingStateType.City:
                string prefId = MypageEventManager.Instance._prefId;
                max = CommonModelHandle.GetCityData (prefId).Count;
                break;
            case CurrentProfSettingStateType.BloodType:
                max = InitDataApi._httpCatchData.result.blood_type.Count;
                break;
            case CurrentProfSettingStateType.HairStyle:
                max = CommonModelHandle.GetNameMaster (
                    AppStartLoadBalanceManager._gender,
                    CurrentProfSettingStateType.HairStyle
                ).Count;
                break;
            case CurrentProfSettingStateType.BodyType:
                max = CommonModelHandle.GetNameMaster (
                    AppStartLoadBalanceManager._gender,
                    CurrentProfSettingStateType.BodyType
                ).Count;
                break;
            case CurrentProfSettingStateType.Glasses:
                max = CommonModelHandle.GetNameMaster (
                    AppStartLoadBalanceManager._gender,
                    CurrentProfSettingStateType.Glasses
                ).Count;
                break;
            case CurrentProfSettingStateType.Type:
                max = CommonModelHandle.GetNameMaster (
                    AppStartLoadBalanceManager._gender,
                    CurrentProfSettingStateType.Type
                ).Count;
                break;
            case CurrentProfSettingStateType.Personality:
                max = CommonModelHandle.GetNameMaster (
                    AppStartLoadBalanceManager._gender,
                    CurrentProfSettingStateType.Personality
                ).Count;
                break;
            case CurrentProfSettingStateType.Holiday:
                max = CommonModelHandle.GetNameMaster (
                    AppStartLoadBalanceManager._gender,
                    CurrentProfSettingStateType.Holiday
                ).Count;
                break;
            case CurrentProfSettingStateType.AnnualIncome:
                max = CommonModelHandle.GetNameMaster (
                    AppStartLoadBalanceManager._gender,
                    CurrentProfSettingStateType.AnnualIncome
                ).Count;
                break;
            case CurrentProfSettingStateType.Education:
                max = CommonModelHandle.GetNameMaster (
                    AppStartLoadBalanceManager._gender,
                    CurrentProfSettingStateType.Education
                ).Count;
                break;
            case CurrentProfSettingStateType.Housemate:
                max = CommonModelHandle.GetNameMaster (
                    AppStartLoadBalanceManager._gender,
                    CurrentProfSettingStateType.Housemate
                ).Count;
                break;
            case CurrentProfSettingStateType.Sibling:
                max = CommonModelHandle.GetNameMaster (
                    AppStartLoadBalanceManager._gender,
                    CurrentProfSettingStateType.Sibling
                ).Count;
                break;
            case CurrentProfSettingStateType.Alcohol:
                max = CommonModelHandle.GetNameMaster (
                    AppStartLoadBalanceManager._gender,
                    CurrentProfSettingStateType.Alcohol
                ).Count;
                break;
            case CurrentProfSettingStateType.Tobacco:
                max = CommonModelHandle.GetNameMaster (
                    AppStartLoadBalanceManager._gender,
                    CurrentProfSettingStateType.Tobacco
                ).Count;
                break;
            case CurrentProfSettingStateType.Car:
                max = CommonModelHandle.GetNameMaster (
                    AppStartLoadBalanceManager._gender,
                    CurrentProfSettingStateType.Car
                ).Count;
                break;
            case CurrentProfSettingStateType.Pet:
                max = CommonModelHandle.GetNameMaster (
                    AppStartLoadBalanceManager._gender,
                    CurrentProfSettingStateType.Pet
                ).Count;
                break;
            case CurrentProfSettingStateType.Hobby:
                max = CommonModelHandle.GetNameMaster (
                    AppStartLoadBalanceManager._gender,
                    CurrentProfSettingStateType.Hobby
                ).Count;
                break;
            case CurrentProfSettingStateType.Interest:
                max = CommonModelHandle.GetNameMaster (
                    AppStartLoadBalanceManager._gender,
                    CurrentProfSettingStateType.Interest
                ).Count;
                break;
            case CurrentProfSettingStateType.Marital:
                max = CommonModelHandle.GetNameMaster (
                    AppStartLoadBalanceManager._gender,
                    CurrentProfSettingStateType.Marital
                ).Count;
                break;
            case CurrentProfSettingStateType.Gender:
                max = InitDataApi._httpCatchData.result.sex_cd.Count;
                break;
            case CurrentProfSettingStateType.Radius:
                max = InitDataApi._httpCatchData.result.radius.Count;
                break;
            }
        }

        if (PanelStartInputUserData.Instance != null)  {
            switch (PanelStartInputUserData.Instance._currentProfSettingState) {
            case CurrentProfSettingStateType.Pref:
                max = InitDataApi._httpCatchData.result.pref.Count;
                break;
            case CurrentProfSettingStateType.City:
                string prefId = PanelStartInputUserData.Instance._prefId;
                max = CommonModelHandle.GetCityData (prefId).Count;
                break;
            }
        }

        for (int i=0; i < m_instantateItemCount; i++) {
            var item = Instantiate (m_ItemBase) as RectTransform;
            item.SetParent (transform, false);
            item.name = i.ToString ();
            item.anchoredPosition = 
                    (direction == Direction.Vertical ) ?
                    new Vector2 (0, -ItemScale * (i)) : 
                    new Vector2 (ItemScale * (i), 0) ;
            m_itemList.Add (item);

            item.gameObject.SetActive (true);

            foreach( var controller in controllers ){
                controller.OnUpdateItem(i, item.gameObject);
            }
        }

        foreach(var controller in controllers) {
            controller.OnPostSetupItems();
        }
        _isUpdateTrigger = true;
        yield break;
    }

    void Update ()
    {
        if (_isUpdateTrigger == false)
            return;
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

        while (AnchoredPosition- m_diffPreFramePosition > 0 ) {

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
        var infiniteScroll = GetComponent<ProfTemplateInfiniteLimitScroll> ();
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
        if (itemCount < 0 || itemCount >= max) {
            obj.SetActive (false);
        } else {
            obj.SetActive (true);

            if (obj.GetComponentInChildren<ProfileDataSetItem> () != null)
            {
                if (MypageEventManager.Instance != null) 
                {
                    switch (MypageEventManager.Instance._currentProfSettingState)
                    {
                    case CurrentProfSettingStateType.Pref:
                            var pref = CommonModelHandle.GetPrefData ();
                            id    = pref[itemCount].id;
                            value = pref[itemCount].name;
                        break;
                    case CurrentProfSettingStateType.City:
                            string prefId = MypageEventManager.Instance._prefId;
                            var city = CommonModelHandle.GetCityData (prefId);
    
                            if (city.Count > 0) 
                            {
                                id    = city[itemCount].id;
                                value = city[itemCount].name;
                            }
                        break;
                    case CurrentProfSettingStateType.BloodType:
                            var bloodType = InitDataApi._httpCatchData.result.blood_type;
                            id    = bloodType [itemCount].id;
                            value = bloodType [itemCount].name;
                        break;
                    case CurrentProfSettingStateType.HairStyle:
                            var hairStyle = CommonModelHandle.GetNameMaster (
                                            AppStartLoadBalanceManager._gender,
                                            CurrentProfSettingStateType.HairStyle
                                        );
                            id    = hairStyle [itemCount].name;
                            value = hairStyle [itemCount].name;
                        break;
                    case CurrentProfSettingStateType.BodyType:
                            var bodyType = CommonModelHandle.GetNameMaster (
                                    AppStartLoadBalanceManager._gender,
                                    CurrentProfSettingStateType.BodyType
                                );
                            id    = bodyType [itemCount].name;
                            value = bodyType [itemCount].name;
                        break;
                    case CurrentProfSettingStateType.Glasses:
                            var glasses = CommonModelHandle.GetNameMaster (
                                    AppStartLoadBalanceManager._gender,
                                    CurrentProfSettingStateType.Glasses
                                );
                            id    = glasses [itemCount].name;
                            value = glasses [itemCount].name;
                        break;
                    case CurrentProfSettingStateType.Type:
                            var type = CommonModelHandle.GetNameMaster (
                                AppStartLoadBalanceManager._gender,
                                CurrentProfSettingStateType.Type
                            );
                            id    = type [itemCount].name;
                            value = type [itemCount].name;
                        break;
                    case CurrentProfSettingStateType.Personality:
                            var personality = CommonModelHandle.GetNameMaster (
                            AppStartLoadBalanceManager._gender,
                            CurrentProfSettingStateType.Personality
                            );
                            id    = personality [itemCount].name;
                            value = personality [itemCount].name;
                        break;
                    case CurrentProfSettingStateType.Holiday:
                            var holiday = CommonModelHandle.GetNameMaster (
                            AppStartLoadBalanceManager._gender,
                            CurrentProfSettingStateType.Holiday
                            );
                            id    = holiday [itemCount].name;
                            value = holiday [itemCount].name;
                        break;
                    case CurrentProfSettingStateType.AnnualIncome:
                            var annualIncome = CommonModelHandle.GetNameMaster (
                            AppStartLoadBalanceManager._gender,
                            CurrentProfSettingStateType.AnnualIncome
                            );
                            id    = annualIncome [itemCount].name;
                            value = annualIncome [itemCount].name;
                        break;
                    case CurrentProfSettingStateType.Education:
                            var education = CommonModelHandle.GetNameMaster (
                            AppStartLoadBalanceManager._gender,
                            CurrentProfSettingStateType.Education
                            );
                            id    = education [itemCount].name;
                            value = education [itemCount].name;
                        break;
                    case CurrentProfSettingStateType.Housemate:
                            var housemate = CommonModelHandle.GetNameMaster (
                            AppStartLoadBalanceManager._gender,
                            CurrentProfSettingStateType.Housemate
                            );
                            id    = housemate [itemCount].name;
                            value = housemate [itemCount].name;
                        break;
                    case CurrentProfSettingStateType.Sibling:
                            var sibling = CommonModelHandle.GetNameMaster (
                            AppStartLoadBalanceManager._gender,
                            CurrentProfSettingStateType.Sibling
                            );
                            id    = sibling [itemCount].name;
                            value = sibling [itemCount].name;
                        break;
                    case CurrentProfSettingStateType.Alcohol:
                            var alcohol = CommonModelHandle.GetNameMaster (
                            AppStartLoadBalanceManager._gender,
                            CurrentProfSettingStateType.Alcohol
                            );
                            id    = alcohol [itemCount].name;
                            value = alcohol [itemCount].name;
                        break;
                    case CurrentProfSettingStateType.Tobacco:
                            var tobacco = CommonModelHandle.GetNameMaster (
                            AppStartLoadBalanceManager._gender,
                            CurrentProfSettingStateType.Tobacco
                            );
                            id    = tobacco [itemCount].name;
                            value = tobacco [itemCount].name;
                        break;
                    case CurrentProfSettingStateType.Car:
                        var car = CommonModelHandle.GetNameMaster (
                            AppStartLoadBalanceManager._gender,
                            CurrentProfSettingStateType.Car
                            );
                            id    = car [itemCount].name;
                            value = car [itemCount].name;
                        break;
                    case CurrentProfSettingStateType.Pet:
                            var pet = CommonModelHandle.GetNameMaster (
                            AppStartLoadBalanceManager._gender,
                            CurrentProfSettingStateType.Pet
                            );
                            id    = pet [itemCount].name;
                            value = pet [itemCount].name;
                        break;
                    case CurrentProfSettingStateType.Hobby:
                            var hobby = CommonModelHandle.GetNameMaster (
                            AppStartLoadBalanceManager._gender,
                            CurrentProfSettingStateType.Hobby
                            );
                            id    = hobby [itemCount].name;
                            value = hobby [itemCount].name;
                        break;
                    case CurrentProfSettingStateType.Interest:
                            var interest = CommonModelHandle.GetNameMaster (
                            AppStartLoadBalanceManager._gender,
                            CurrentProfSettingStateType.Interest
                            );
                            id    = interest [itemCount].name;
                            value = interest [itemCount].name;
                        break;
                    case CurrentProfSettingStateType.Marital:
                            var marital = CommonModelHandle.GetNameMaster (
                            AppStartLoadBalanceManager._gender,
                            CurrentProfSettingStateType.Marital
                            );
                            id    = marital [itemCount].name;
                            value = marital [itemCount].name;            
                        break;
                    case CurrentProfSettingStateType.Gender:
                           var gender = InitDataApi._httpCatchData.result.sex_cd;
                           id    = gender[itemCount].id;
                           value = gender[itemCount].name;
                        break;
                    case CurrentProfSettingStateType.Radius:
                           var radius = InitDataApi._httpCatchData.result.radius;
                           id    = radius[itemCount].id;
                           value = radius[itemCount].name;
                        break;
                    }
                }

                if (PanelStartInputUserData.Instance != null){
                    switch (PanelStartInputUserData.Instance._currentProfSettingState) {
                    case CurrentProfSettingStateType.Pref:
                        var pref = CommonModelHandle.GetPrefData ();
                        id    = pref [itemCount].id;
                        value = pref [itemCount].name;
                        break;
                    case CurrentProfSettingStateType.City:
                        string prefId = PanelStartInputUserData.Instance._prefId;
                        var city = CommonModelHandle.GetCityData (prefId);
    
                        if (city.Count > 0) {
                            id    = city [itemCount].id;
                            value = city [itemCount].name;
                        }
                        break;
                    }
                }
                var item = obj.GetComponentInChildren<ProfileDataSetItem> ();
                item.UpdateItem (itemCount, id, value);
            }
        }
    }
}
