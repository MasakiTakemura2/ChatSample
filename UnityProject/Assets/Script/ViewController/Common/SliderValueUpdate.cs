using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using EventManager;


public class SliderValueUpdate : UIBehaviour ,IBeginDragHandler, IEndDragHandler
{
    [SerializeField]
    private GameObject _controlObject;

    [SerializeField]
    private Text _valueText;
    
    [SerializeField]
    private CurrentProfSettingStateType _cpsType;
    
    public void OnBeginDrag(PointerEventData eventData)
    {
        eventData.selectedObject = this.gameObject;
        GameObject.FindGameObjectWithTag (CommonConstants.BACK_SWIPE).GetComponent<ScreenRaycaster> ().enabled = false;
    }
    
    public void OnEndDrag (PointerEventData eventData) {
        eventData.selectedObject = this.gameObject;
        GameObject.FindGameObjectWithTag (CommonConstants.BACK_SWIPE).GetComponent<ScreenRaycaster> ().enabled = true;
    }
    
    void Update () 
    {
        if(_controlObject != null)
        {
            if(_controlObject.GetComponent<Slider>() != null)
            {
                if(_valueText != null)
                {
                     if (_cpsType == CurrentProfSettingStateType.Height) {
                        MypageEventManager.Instance._cpsTypeSliderHeight = _cpsType;
                     } else if (_cpsType == CurrentProfSettingStateType.Weight) {
                        MypageEventManager.Instance._cpsTypeSliderWeight = _cpsType; 
                     }
                     
                    _valueText.text = _controlObject.GetComponent<Slider> ().value.ToString();
                }
            }
        }
    }
}
