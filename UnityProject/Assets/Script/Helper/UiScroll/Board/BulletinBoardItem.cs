using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class BulletinBoardItem : UIBehaviour 
{
    [SerializeField]
    private Image _userPict;

    [SerializeField]
    private Image _backGround;

    [SerializeField]
    private Text _userName;

    [SerializeField]
    private Text _category;

    [SerializeField]
    private Text _eazyTitle;

    [SerializeField]
    private Text _dateTime;

    private readonly Color[] colors = new Color[]
    {
        new Color(1, 1, 1, 1),
        new Color(0.2f, 0.2f, 0.2f, 0.15f),
    };

    public void UpdateItem (int count)
    {
        //TODO: FROM SERVER DATA AWS S3 ?
        if (_userPict != null) 
            //_userPict.sprite = "";

        //TODO: FROM SERVER
        if (_userName != null)
            _userName.text = "掲示板タイトル" + (count + 1).ToString("00");

        //TODO: FROM SERVER 
        if (_category != null)
            _category.text = "カテゴリ カテゴリ";

        //TODO: FROM SERVER
        if (_eazyTitle != null)
            _eazyTitle.text = "詳細内容の抜粋…詳細内容の抜粋…";

        //TODO: FROM SERVER
        if (_dateTime != null)
            _dateTime.text = "2時間前";

        //-- Example -- [nest vertical Item color change possible] 
        //if (_backGround != null)
            //_backGround.color = colors[Mathf.Abs(count) % colors.Length];
    }
}