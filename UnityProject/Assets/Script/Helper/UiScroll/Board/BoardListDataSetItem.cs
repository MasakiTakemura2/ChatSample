using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System;
using EventManager;


public class BoardListDataSetItem : UIBehaviour 
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

		switch (BulletinBoardEventManager.Instance._currentSettingState)
        {	
			case BulletinBoardEventManager.CurrentSettingState.Sort:
			if (id == ViewController.PanelBulletinBoardChange.Instance._categoryAPIThrow) 
			{
				_checkMarck.SetActive (true);
			}
			break;
			case BulletinBoardEventManager.CurrentSettingState.Radius:
			if (id == ViewController.PanelBulletinBoardChange.Instance._radiusAPIThrow) 
			{
				_checkMarck.SetActive (true);
			}
			break;

			case BulletinBoardEventManager.CurrentSettingState.Sex:
			if (id == ViewController.PanelBulletinBoardChange.Instance._sexAPIThrow) 
			{
				_checkMarck.SetActive (true);
			}
			break;

			case BulletinBoardEventManager.CurrentSettingState.BodyType:
			if (id == ViewController.PanelBulletinBoardChange.Instance._bodyTypeAPIThrow) 
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
