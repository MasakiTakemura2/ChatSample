using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System;
using EventManager;


public class ProfileDataSetItem : UIBehaviour 
{
    [SerializeField]
    private Text _itemName;

    [SerializeField]
    private GameObject _checkMarck;

    /// <summary>
    /// Updates the item.
    /// </summary>
    /// <param name="count">Count.</param>
    /// <param name="id">Identifier.</param>
    /// <param name="itemName">Item name.</param>
    public void UpdateItem (int count, string id = "", string itemName = "")
    {
        this.gameObject.name = id;

        //一回クリーン。
        _checkMarck.SetActive (false);
        if (StartEventManager.Instance != null) {
            switch (StartEventManager.Instance._currentProfSettingState){
            case CurrentProfSettingStateType.Pref:
                if (id == StartEventManager.Instance._prefId)
                    _checkMarck.SetActive (true);
                break;
            case CurrentProfSettingStateType.City:
                if (id == StartEventManager.Instance._cityId) 
                    _checkMarck.SetActive (true);
                break;
            }
        }

        if (MypageEventManager.Instance != null) {
            switch (MypageEventManager.Instance._currentProfSettingState)
            {
            case CurrentProfSettingStateType.Pref:
                if (id == MypageEventManager.Instance._prefId)
                    _checkMarck.SetActive (true);
                break;
            case CurrentProfSettingStateType.City:
                if (id == MypageEventManager.Instance._cityId) 
                    _checkMarck.SetActive (true);
                break;
            case CurrentProfSettingStateType.BloodType:
                if (id == MypageEventManager.Instance._bloodType)
                    _checkMarck.SetActive (true);
                break;
            case CurrentProfSettingStateType.HairStyle:
                if (id == MypageEventManager.Instance._hairStyle[0])
                    _checkMarck.SetActive (true);
                break;
            case CurrentProfSettingStateType.BodyType:
                if (id == MypageEventManager.Instance._bodyType[0])
                    _checkMarck.SetActive (true);
                break;
            case CurrentProfSettingStateType.Glasses:
                if (id == MypageEventManager.Instance._glasses[0])
                    _checkMarck.SetActive (true);
                    break;
            case CurrentProfSettingStateType.Type://TODO: 複数選択可・対応
                if (id == MypageEventManager.Instance._type[0])
                    _checkMarck.SetActive (true);
                break;
            case CurrentProfSettingStateType.Personality://TODO: 複数選択可・対応
                if (id == MypageEventManager.Instance._personality[0])
                    _checkMarck.SetActive (true);
                break;
            case CurrentProfSettingStateType.Holiday:
                if (id == MypageEventManager.Instance._holiday[0])
                    _checkMarck.SetActive (true);
                break;
            case CurrentProfSettingStateType.AnnualIncome:
                if (id == MypageEventManager.Instance._annualIncome[0])
                    _checkMarck.SetActive (true);
                break;
            case CurrentProfSettingStateType.Education:
                if (id == MypageEventManager.Instance._education[0])
                    _checkMarck.SetActive (true);
                break;
            case CurrentProfSettingStateType.Housemate:
                if (id == MypageEventManager.Instance._housemate[0])
                    _checkMarck.SetActive (true);
                break;
            case CurrentProfSettingStateType.Sibling:
                if (id == MypageEventManager.Instance._sibling[0])
                    _checkMarck.SetActive (true);
                break;
            case CurrentProfSettingStateType.Alcohol:
                if (id == MypageEventManager.Instance._alcohol[0])
                    _checkMarck.SetActive (true);
                break;
            case CurrentProfSettingStateType.Tobacco:
                if (id == MypageEventManager.Instance._tobacco[0])
                    _checkMarck.SetActive (true);
                break;
            case CurrentProfSettingStateType.Car:
                if (id == MypageEventManager.Instance._car[0])
                    _checkMarck.SetActive (true);
                break;
            case CurrentProfSettingStateType.Pet:
                if (id == MypageEventManager.Instance._pet[0])
                    _checkMarck.SetActive (true);
                break;
            case CurrentProfSettingStateType.Hobby:
                if (id == MypageEventManager.Instance._hobby[0])
                    _checkMarck.SetActive (true);
                break;
            case CurrentProfSettingStateType.Interest:
                if (id == MypageEventManager.Instance._interest[0])
                    _checkMarck.SetActive (true);
                break;
            case CurrentProfSettingStateType.Marital:
                if (id == MypageEventManager.Instance._marital[0])
                    _checkMarck.SetActive (true);
                break;
            case CurrentProfSettingStateType.Gender:
                if (id == ViewController.PanelPassingSetting.Instance._sexCdPost)
                    _checkMarck.SetActive (true);
                break;
            }
        }

        if (_itemName != null)
            _itemName.text = itemName; //profileItem.name;
	}
}
