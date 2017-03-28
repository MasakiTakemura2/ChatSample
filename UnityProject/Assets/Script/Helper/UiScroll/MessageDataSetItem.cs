using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System;
using EventManager;
using Http;

using System.Linq;

public class MessageDataSetItem : UIBehaviour
{
    [SerializeField]
    private RawImage _profImage;

    [SerializeField]
    private RawImage _uploadImage;

    [SerializeField]
    private Text _message;
	public Text GetMessage()
	{
		return _message;
	}

    [SerializeField]
    private Text _read;

    [SerializeField]
    private Text _registTime;

    [SerializeField]
    private GameObject _mosicField;

    private bool _isProfImage = false;
    private Texture _copyProfImage;
    

    /// <summary>
    /// Sets the data.
    /// </summary>
    /// <returns>The data.</returns>
    /// <param name="msg">Message.</param>
    /// <param name="tex2D">Tex2 d.</param>
    public void SetData (string msg, Texture2D tex2D = null)
	{
        if (tex2D != null && _uploadImage != null)
           _uploadImage.texture = tex2D;

//        if (_read != null)
//            _read.text = "未読";

        if (_registTime != null)
            _registTime.text = (System.DateTime.Now).ToString("HH:mm");

        if (_message != null)
            _message.text = msg;
    }

    /// <summary>
    /// Updates the item.
    /// </summary>
    /// <param name="count">Count.</param>
    /// <param name="id">Identifier.</param>
    /// <param name="itemName">Item name.</param>
    public void UpdateItem (int itemcount, MessageListEntity.MessgeData mData, string profUrl)
    {
        if (profUrl != null && _profImage != null) 
		{
            if (_isProfImage == false)
            {
                StartCoroutine (WwwToRendering (profUrl, _profImage, mData, true));
            } else {
                _profImage.texture = _copyProfImage;
            }

        }

        if (mData.image != null && _uploadImage != null)
		{
            _uploadImage.name = mData.id;
            StartCoroutine (WwwToRendering (mData.image.thumbnail_url, _uploadImage, mData));
        }

		if (_read != null)
		{
			_read.text = mData.status;
		}

		if (_registTime != null) 
		{
            _registTime.text = mData.regist_datetime;
		}

		if (_message != null)
		{
            //_message.text = System.Text.RegularExpressions.Regex.Replace (mData.message, @"[ ]|[　]", "");
            _message.text = mData.message;
		}      
    }

    /// <summary>
    /// Wwws to rendering.
    /// TODO: 共通化にしていく。
    /// </summary>
    /// <returns>The to rendering.</returns>
    /// <param name="url">URL.</param>
    /// <param name="targetObj">Target object.</param>
    private IEnumerator WwwToRendering (string url, RawImage targetObj, MessageListEntity.MessgeData mData, bool isProf = false)
    {
        targetObj.texture = null;
        targetObj.gameObject.SetActive (false);
        if (string.IsNullOrEmpty (url) == true)
            yield break;
        using (WWW www = new WWW (url)) {
            while (www == null)
                yield return (www != null);

            while (www.isDone == false)
                yield return (www.isDone);

            //non texture file
            if (string.IsNullOrEmpty (www.error) == false) {
                Debug.LogError (www.error);
                Debug.Log (url);
                yield break;
            }
            while (targetObj == null)
                yield return (targetObj != null);


            if (_mosicField != null && mData.is_mask == "true") {
                _mosicField.name = mData.id;
                //モザイクだめみたい。。。
                //Material mat = Resources.Load ("MosaicField") as Material;
                _mosicField.GetComponent<Image> ().color = Color.gray;
                _mosicField.SetActive (true);
            } else if ( _mosicField != null && mData.is_mask == "false" ){
                targetObj.material = null;
                _mosicField.SetActive (false);
            } 
            if (isProf == true) {
                targetObj.gameObject.SetActive (true);
                targetObj.texture = www.texture;
                _copyProfImage = www.texture;
                _isProfImage = true;
                yield break;
            }
                

            targetObj.gameObject.SetActive (true);
            targetObj.texture = www.texture;
        }
    }
}