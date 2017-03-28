using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System;
using EventManager;


public class SearchListDataSetItem : UIBehaviour 
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
	/// ページ表示時の読み込み
    public void UpdateItem (int count, string id = "", string itemName = "")
    {
        this.gameObject.name = id;


        //一回クリーン。
        _checkMarck.SetActive (false);

		switch(SearchEventManager.Instance._currentSettingState)
        {	
			case SearchEventManager.CurrentSettingState.Sort:
			if(id == ViewController.PanelSearchListChange.Instance._orderAPIThrow) 
			{
				_checkMarck.SetActive (true);
			}
			break;

			case SearchEventManager.CurrentSettingState.Sex:
				if(id == ViewController.PanelSearchListChange.Instance._sexAPIThrow) 
				{
					_checkMarck.SetActive (true);
				}
			break;

			case SearchEventManager.CurrentSettingState.BodyType:
				if(id == ViewController.PanelSearchListChange.Instance._bodyTypeAPIThrow) 
				{
					_checkMarck.SetActive (true);
				}
            break;

			case SearchEventManager.CurrentSettingState.Radius:
			if(id == ViewController.PanelSearchListChange.Instance._radiusAPIThrow) 
			{
				_checkMarck.SetActive (true);
			}
			break;
        }

        if (_itemName != null)
		{
            _itemName.text = itemName; 
		}


	}


}
