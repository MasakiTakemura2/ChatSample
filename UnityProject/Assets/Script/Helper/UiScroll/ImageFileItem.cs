using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ImageFileItem : UIBehaviour 
{
    [SerializeField]
    private Image _userPict;

    [SerializeField]
    private Image _backGround;

    [SerializeField]
    private Text _userName;

    [SerializeField]
    private Text _userPlace;

    [SerializeField]
    private Text _eazyTitle;

    [SerializeField]
    private Text _dateTime;

    [SerializeField]
    private Text _read;

    private readonly Color[] colors = new Color[]
    {
        new Color(1, 1, 1, 1),
        new Color(0.2f, 0.2f, 0.2f, 0.15f),
        
    };

    public void UpdateItem (int count)
    {
        if (_userPict != null) 
            //_userPict.sprite = "";

        if (_userName != null)
        _userName.text = "ありさ" + (count + 1).ToString("00");

        if (_userPlace != null)
        _userPlace.text = "0.0" + (count + 1).ToString("0") + "km  東京都 渋谷区";

        if (_eazyTitle != null)
            _eazyTitle.text = "仕事終わった〜(*^^*)か...";

        if (_dateTime != null)
            _dateTime.text = (System.DateTime.Now).ToString("yyyy/MM/dd HH:mm:ss");

        if (_read != null)
            _read.text = "未読";

        //-- Example -- [nest vertical Item color change possible] 
        //if (_backGround != null)
            //_backGround.color = colors[Mathf.Abs(count) % colors.Length];
    }
}
