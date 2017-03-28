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
using System.Text.RegularExpressions;

public class MessageInfiniteLimitScroll : UIBehaviour, IInfiniteScrollSetup
{
    #region SerializeField

	[SerializeField]
	private RectTransform _userMessageBase;
	public RectTransform GetUserMessage()
	{
		return _userMessageBase;
	}

    [SerializeField]
    private RectTransform _toUserMessageBase;
	public RectTransform GetToUserMessage()
	{
		return _toUserMessageBase;
	}


    [SerializeField]
    private RectTransform _userPictureBase;
	public RectTransform GetUserPictureMessage()
	{
		return _userPictureBase;
	}

    
    [SerializeField]
    private RectTransform _toPictureBase;
	public RectTransform GetUserToPictureMessage()
	{
		return _toPictureBase;
	}
    
    [SerializeField]
    private RectTransform _borderBase;

    [SerializeField]
    private GameObject _loadingOverlay;

    [SerializeField]
    private GameObject _panelTutorialFavorite;

	// １ページ分のパネル表示個数
	[SerializeField, Range(0, 40)]
	public int m_instantateItemCount = 30;		
   
    [SerializeField, Range(1, 999)]
    private int max = 50;

	// 通信から受け取ったデータリストをその場に保存用　通信ごとに違うパネルが存在すれば追加していく　条件検索の際は無条件に一旦初期化
	public List<MessageListEntity.MessgeData> _connectedGetDataList = new List<MessageListEntity.MessgeData>();

	// 更新後のバーの位置確認の為 何個追加されたかそのつど覚えておく
	public int _listAddCount = 0;

	// 一回の通信で表示分は３０個までに固定決め (仕様変更によりもう必要ないかも・・・）
	public static  int PANEL_TOTAL = 30; 

    [SerializeField]
    private Text _headerTitle;

    public static int _lastTimeStamp;
	public string _displayType = "";

    #endregion


    #region Member Valiable
    public Direction direction;
    public OnItemPositionChange onUpdateItem = new OnItemPositionChange ();

	[NonSerialized]
    public List<RectTransform>	m_itemList = new List<RectTransform> ();
    
    public float m_diffPreFramePosition = 0;
	public float m_diffPreFramePositionUp = 0;
	public float m_diffPreFramePositionDown = 0;


    protected int m_currentItemNo = 0;
    protected bool _isUpdateTrigger = false;

    public int msgCount = 0;
    private RectTransform itemObj;
    public float addPos;
    public float addPosCache;

	public List<Vector2> _backupMessagePosition = new List<Vector2>();
	public List<float> _backupDeltaSize = new List<float>();
	//public float _scrollDeltaSize = 0;

	protected float _lastBarPosiiton;	// 表示位置確認用

	//protected float _lastUpPanelPosition;
	//protected float _lastDownPanelPosition;


    public enum Direction 
	{
		Vertical,
		Horizontal,
	}

    public MessageType _messageType;
    public enum MessageType 
	{
        Message = 1,
        Image   = 2,
        Movie   = 3,
        Random  = 4,
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

    public float AnchoredPosition
    {
        get
		{
            return -_RectTransform.anchoredPosition.y;
        }
    }

    private float _itemScale = -1;
    public float itemScale
	{
        get
		{
            if (_userMessageBase != null && _itemScale == -1)
			{
                _itemScale = _userMessageBase.sizeDelta.y;
            }
            return _itemScale;
        }
        private set
		{
            _itemScale = value;
        }
    }
    #endregion





	public void Init ()
    {
		_isUpdateTrigger = false;

        if (_panelTutorialFavorite != null) 
		{
            Helper.LocalFileHandler.Init ( LocalFileConstants.GetLocalFileDir() + LocalFileConstants.COMMON_LOCAL_FILE_NAME);
            if (Helper.LocalFileHandler.GetBool (LocalFileConstants.TUTORIAL_FAVORITE_KEY) == false)
            {
                _panelTutorialFavorite.SetActive (true);
            } else {
                _panelTutorialFavorite.SetActive (false);
            }
        }

        _lastTimeStamp = 0;

        var scrollRect = GetComponentInParent<ScrollRect>();
        scrollRect.horizontal = direction == Direction.Horizontal;
        scrollRect.vertical   = direction == Direction.Vertical;
        scrollRect.content    = _RectTransform;

        m_diffPreFramePosition = 0;
        m_currentItemNo = 0;
        m_itemList  = new List<RectTransform> ();
        msgCount    = 0;
        addPos      = 0f;
        addPosCache = 0f;
		_backupMessagePosition.Clear ();
		_backupDeltaSize.Clear ();
		_connectedGetDataList.Clear ();
		max = 0;
		m_instantateItemCount = 0;

		StartCoroutine (Set());
    }


    public IEnumerator Set ()
    {
        var controllers = GetComponents<MonoBehaviour> ().Where (item => item is IInfiniteScrollSetup).Select (item => item as IInfiniteScrollSetup).ToList ();
        while (MessageListApi._success == false) {
            yield return (MessageListApi._success == true);
        }

        _loadingOverlay.SetActive (true);


        //_headerTitle.text = MessageListApi._httpCatchData.result.to_user.name;
      
        // 初期化 一旦全消し
        if (transform.childCount > 1) {
            for (int i = 5; i < transform.childCount; i++) {
                Destroy (transform.GetChild (i).gameObject);
            }
        }
    
        for (int index = 0; index < MessageListApi._httpCatchData.result.messages.Count; ++index) {
            // 存在していないメッセージだけ加える
            bool check = false;
            for (int indexbackup = 0; indexbackup < _connectedGetDataList.Count; ++indexbackup) {
                if (_connectedGetDataList [indexbackup].id == MessageListApi._httpCatchData.result.messages [index].id) {
                    check = true;
                    break;
                }
            }
            if (!check) {
                _connectedGetDataList.Add (MessageListApi._httpCatchData.result.messages [index]);
            }
        }


        // メッセージ個数　
        max = _connectedGetDataList.Count;
        m_instantateItemCount = 10; 
        if (m_instantateItemCount > max) {
            m_instantateItemCount = max;
        }


        // 全体の位置調整の為予め計算
        //for(int index = 0;  index<max - m_instantateItemCount; ++index)
        for (int index = 0; index < max; ++index) {
            MessageListEntity.MessgeData msg = _connectedGetDataList [index];

            itemObj = null;

            if (msg.send_user_id == MessageListApi._httpCatchData.result.from_user.id) {
                //自分メッセージ or Image
                if (msg.type == ((int)MessageType.Message).ToString () || msg.type == ((int)MessageType.Random).ToString ()) {
                    itemObj = Instantiate (_userMessageBase) as RectTransform;
                } else if (msg.type == ((int)MessageType.Image).ToString () || msg.type == ((int)MessageType.Movie).ToString ()) {
                    itemObj = Instantiate (_userPictureBase) as RectTransform;
                }
            } else if (msg.send_user_id == MessageListApi._httpCatchData.result.to_user.id) {
                //相手メッセージ or Image
                if (msg.type == ((int)MessageType.Message).ToString () || msg.type == ((int)MessageType.Random).ToString ()) {
                    itemObj = Instantiate (_toUserMessageBase) as RectTransform;
                } else if (msg.type == ((int)MessageType.Image).ToString ()  || msg.type == ((int)MessageType.Movie).ToString ()) {
                    itemObj = Instantiate (_toPictureBase) as RectTransform;
                }
            }

            // 画像か文章ながいなら縦幅の大きさの調整
            if (msg.type == ((int)MessageType.Image).ToString ()  || msg.type == ((int)MessageType.Movie).ToString ()) {
                _backupMessagePosition.Add (new Vector2 (0, -addPosCache));
                Debug.Log ("imagesize = " + itemObj.sizeDelta.y.ToString ());
                addPos = itemObj.sizeDelta.y;
                addPosCache += addPos;
                _backupDeltaSize.Add (addPos);
            
            } else {
                

                //string s = System.Text.RegularExpressions.Regex.Replace (msg.message, @"[ ]|[　]", "");
                string s = msg.message;
                Debug.Log (s);
                Debug.Log (s.Length + "<- 文字数 ベンチベンチベンチベンチベンチベンチベンチベンチベンチベンチベンチベンチ　");

//[\\x01-\\x7E]                
//ASCII文字 !"#$%&'()*+,-./0123456789:;<=>?@ABCDEFGHIJKLMNOPQRSTUVWXYZ[\]^_`abcdefghijklmnopqrstuvwxyz{|}~

//[\\xA1-\\xDF]|
//検索したい文字
//あいうえお１２３４５漢字


                Regex re = new Regex ("[\\x01-\\x7E]|[\\xA1-\\xDF]|[\\uFF61-\\uFF9F]|[\\*]");
                Regex reSpace = new Regex ("[\n]");
                MatchCollection spaceMatch = reSpace.Matches (s);
Debug.Log (spaceMatch.Count + " 改行の正規表現にひかかった数。 ");
                Match m = re.Match (s);
                MatchCollection matches = re.Matches (s);
                
                //spaceMatch

                //if (s.Length > 500) {
                    while (m.Success == true) {
                        //一致した対象が見つかったときキャプチャした部分文字列を表示
                        if (m.Value == "*") {
                            Debug.Log (m.Value + " アスタリスクやで！ ");
                        } else {
                            Debug.Log (m.Value);
                        }
                        //次に一致する対象を検索
                        m = m.NextMatch ();
                    }
                //}


                string searchWord = "\n";   // 改行文字を探せ
                #if UNITY_IOS
                const int LineTextMax = 18; //文字幅による改行カウントの文字数
                #elif UNITY_ANDROID
                const int LineTextMax = 21; //文字幅による改行カウントの文字数
                #else 
                const int LineTextMax = 18; //文字幅による改行カウントの文字数
                #endif
                int count = 0;

                int foundIndex = s.IndexOf (searchWord);

                // 改行文字が１つはある場合
                if (foundIndex >= 0) {
                    while (foundIndex >= 0) {
                        //見つかった位置を表示する
//						Debug.Log (foundIndex);

                        //次の検索開始位置
                        int nextIndex = foundIndex + searchWord.Length;
                        //if (nextIndex < s.Length) 
                        //{
                        //次の位置を探す
                        int indexbackup = foundIndex;
                        foundIndex = s.IndexOf (searchWord, nextIndex);

                        if (foundIndex >= 0) {                  
                            // １行分の文字幅数をこえているなら改行文字がなくても改行とみなす
                            if (foundIndex - nextIndex > LineTextMax) {
                                count += (foundIndex - nextIndex) / LineTextMax;
                            }
                        } else {
                            if (s.Length - indexbackup > LineTextMax) {
                                count += (s.Length - indexbackup) / LineTextMax;                        
                            }
                        }
                        count++;
                    }
                } else {
                    if (s.Length > LineTextMax) {
                        count += s.Length / LineTextMax;
                    }
                }
            
Debug.Log (count + " << 改行 / 総合文字数で 開業するラインを決めるカウント"  );

                _backupMessagePosition.Add (new Vector2 (0, -addPosCache));
                if (matches.Count > 0) {

                    float mc = matches.Count;
Debug.Log (mc + " <= match count 正規表現とマッチしている => ");


                    addPos = (46 * count) - mc;
                    addPos = addPos + spaceMatch.Count;
                } else {
                    addPos = (46 * count);
                }
                float diff = addPos - s.Length;

                Debug.Log (diff + " - addpos:" + addPos + " - s.Length:" + s.Length);

                if (diff >= 2500f) {
                    addPos -= 550f;
                }
                else if (diff >= 2000f) {
                    if (matches.Count > 200) 
                        addPos -= 400f;
                } else if (diff >= 1500f) {
                    if (matches.Count > 150) 
                        addPos -= 200f;
                } else if (diff >= 1000f) {
                    if (matches.Count > 100) {
                       addPos -= 175f;
                    } else {
                        addPos -= 50f;
                    }
                } else if (diff <= 0f) {
                    addPos += 100f;
                    addPos += Mathf.Abs (diff);
                }

Debug.Log (addPos + " addPos <<<<<<<<<<<< ");

#if UNITY_EDITOR
				if (index != 0) 
				{
					if (count > 4)
					{
						addPos += 180;
					} else {
                        addPos += 200;
					}
				} else {
					if(count > 4)
					{
                        addPos += 180;
					}else{
                        addPos += 200;
					}
				}
#elif !UNITY_EDITOR && UNITY_ANDROID

                if (index != 0) 
                {
                    if (count > 4)
                    {
                addPos += 180;
                    } else {
                        addPos += 220;
                    }
                } else {
                    if(count > 4)
                    {
                addPos += 180;
                    }else{
                        addPos += 220;
                    }
                }
#else
                if (index != 0) 
                {
                    if (count > 4)
                    {
                addPos += 180;
                    } else {
                        addPos += 220;
                    }
                } else {
                    if(count > 4)
                    {
                addPos +=  180;
                    }else{
                        addPos += 220;
                    }
                }
#endif
Debug.Log (addPos + "エンドエンドエンドエンドエンドエンドエンドエンドエンドエンドエンドエンド");
				//addPos = (msg.message.Length * 2) + (LineTextMax*count) + 170;
                addPosCache += addPos;
                _backupDeltaSize.Add (addPos);
            }

            if (itemObj != null) 
                Destroy (itemObj.gameObject);
        }


		// 履歴の最新部分数件に焦点をあわせる
		msgCount = max - m_instantateItemCount;
		for(int index = max - m_instantateItemCount; index<max; ++index)
		{
			if(index >= max)
			{
				break;
			}
			MessageListEntity.MessgeData msg = _connectedGetDataList [index];

            itemObj = new RectTransform ();
            if (msg.send_user_id == MessageListApi._httpCatchData.result.from_user.id)
            {
                //自分メッセージ or Image
                if (msg.type == ((int)MessageType.Message).ToString () || msg.type == ((int)MessageType.Random).ToString ())
				{
                   	itemObj = Instantiate (_userMessageBase) as RectTransform;
                }else if (msg.type == ((int)MessageType.Image).ToString () || msg.type == ((int)MessageType.Movie).ToString ()){
                    itemObj =Instantiate (_userPictureBase) as RectTransform;
                }
            } else if (msg.send_user_id == MessageListApi._httpCatchData.result.to_user.id) {
                //相手メッセージ or Image
                if (msg.type == ((int)MessageType.Message).ToString () || msg.type == ((int)MessageType.Random).ToString ()) 
				{
                    itemObj = Instantiate (_toUserMessageBase) as RectTransform;
                } else if (msg.type == ((int)MessageType.Image).ToString () || msg.type == ((int)MessageType.Movie).ToString ()) {
                    itemObj = Instantiate (_toPictureBase) as RectTransform;
                }
            }

            if (itemObj != null) 
            {
                itemObj.SetParent (transform, false);
                m_itemList.Add (itemObj);

				itemObj.anchoredPosition = _backupMessagePosition [index];
                itemObj.name = msgCount.ToString ();

				// 初期化関数呼び出し
				foreach( var controller in controllers )
				{
					controller.OnUpdateItem(index, itemObj.gameObject);
				}
				msgCount++;
            } else {
                Debug.Log ("gameobjectがヌル");
            }

		}
		foreach(var controller in controllers)
        {
            controller.OnPostSetupItems();
        }


		// 位置調節
		GetComponentInParent<ScrollRect> ().verticalNormalizedPosition = 0;
		if(m_instantateItemCount == 1)
		{
			// １個しかないならとりあえず一番上にあわせればよい
			//GetComponentInParent<ScrollRect> ().verticalNormalizedPosition = 1;
		}

		//_lastBarPosiiton = 1.0f / (float)max;
		m_currentItemNo = max-1;
		/*
		for(int i=max-1; i>=0; i--)
		{
			if( (AnchoredPosition >= (-_backupMessagePosition[i].y))  && (AnchoredPosition < (-_backupMessagePosition[i].y + _backupDeltaSize[i])))
			{
				Debug.Log ("current " + (i).ToString());
				m_currentItemNo = i;
				break;
			}
		}
		*/
		_lastBarPosiiton = 1.0f / (float)max;

		m_diffPreFramePosition = AnchoredPosition;

		m_diffPreFramePositionUp = -_backupMessagePosition[m_currentItemNo].y;
		m_diffPreFramePositionDown = -_backupMessagePosition[m_currentItemNo].y + _backupDeltaSize[m_currentItemNo];

		//_scrollDeltaSize = _backupDeltaSize [max - 1]; 
		//_lastUpPanelPosition = AnchoredPosition - m_itemList [0].sizeDelta.y;
		//_lastDownPanelPosition = AnchoredPosition + m_itemList [m_itemList.Count-1].sizeDelta.y;
			

		_isUpdateTrigger = true;
		_loadingOverlay.SetActive (false);
        
		yield break;
    }

    void Update ()
    {
		if (_isUpdateTrigger == false) 
		{
			return;
		}

		if(MessageListApi._success == false) 
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

		// バー下移動中のパネル遷移
		if (GetComponentInParent<ScrollRect> ().verticalNormalizedPosition < _lastBarPosiiton) 
		{
			while (AnchoredPosition - m_diffPreFramePosition < 0)
			//while(AnchoredPosition < m_diffPreFramePositionDown)
			{
				//-itemScale * 2 )
				//if (m_instantateItemCount < msgCount)
				//    return;

				if (m_currentItemNo < 0)
				{
					//m_currentItemNo = 0;
					return;
				}
				if (m_currentItemNo >= max - 1)
				{
					//m_currentItemNo = max-2;
					return;
				}

				int addIndex;
				addIndex = max - (max - m_currentItemNo) + 1;
                
				if (m_itemList.Count <= 0)
				{
					return;
				}

				var item = m_itemList [0];
				m_itemList.RemoveAt (0);
                
                if (itemObj != null)
				    Destroy (item.gameObject);
            
				MessageListEntity.MessgeData msg = _connectedGetDataList [addIndex];

				itemObj = null;
				if (msg.send_user_id == MessageListApi._httpCatchData.result.from_user.id) 
				{
					//自分メッセージ or Image
					if (msg.type == ((int)MessageType.Message).ToString () || msg.type == ((int)MessageType.Random).ToString ()) 
					{
						itemObj = Instantiate (_userMessageBase) as RectTransform;
					} else if (msg.type == ((int)MessageType.Image).ToString () || msg.type == ((int)MessageType.Movie).ToString ()) {
						itemObj = Instantiate (_userPictureBase) as RectTransform;
					}
				} else if (msg.send_user_id == MessageListApi._httpCatchData.result.to_user.id) {
					//相手メッセージ or Image
					if (msg.type == ((int)MessageType.Message).ToString () || msg.type == ((int)MessageType.Random).ToString ()) 
					{
						itemObj = Instantiate (_toUserMessageBase) as RectTransform;
					} else if (msg.type == ((int)MessageType.Image).ToString () || msg.type == ((int)MessageType.Movie).ToString ()) {
						itemObj = Instantiate (_toPictureBase) as RectTransform;
					}
				}

				if (itemObj != null) 
				{
					itemObj.SetParent (transform, false);               
                    
					itemObj.anchoredPosition = _backupMessagePosition [addIndex];

					itemObj.name = _connectedGetDataList [addIndex].id;

					itemObj.gameObject.SetActive (true);
                  
					itemObj.GetComponent<MessageDataSetItem> ().UpdateItem (addIndex, _connectedGetDataList [addIndex], MessageListApi._httpCatchData.result.to_user.profile_image_url);
               
				} else {
					Debug.Log ("gameobjectがヌル");
				}
                
				m_itemList.Add (itemObj);
      
				float delta = _backupDeltaSize [addIndex];

				m_diffPreFramePosition -= delta;
				//_scrollDeltaSize = delta;

				//m_diffPreFramePositionDown -= delta;
				//m_diffPreFramePositionUp = -_backupMessagePosition [m_currentItemNo + 1].y;

				m_currentItemNo++;
			}         
		}

		// バー上移動中のパネル遷移
		if (GetComponentInParent<ScrollRect> ().verticalNormalizedPosition > _lastBarPosiiton)
		{
      
//Debug.Log ((AnchoredPosition - m_diffPreFramePosition) + " > 0 ");

			while(AnchoredPosition - m_diffPreFramePosition > 0) 
			//while(AnchoredPosition > m_diffPreFramePositionUp) 
			{
				if (m_currentItemNo < 1) 
				{
					//m_currentItemNo = 1;
					return;
				}
				if (m_currentItemNo >= max)
				{
					//m_currentItemNo = max-1;
					return;
				}
				if (m_currentItemNo - 1 - m_instantateItemCount + 2 < 0) 
				{
					return;
				}

				int itemListLastCount = m_itemList.Count - 1; 
				if (itemListLastCount >= m_itemList.Count)
				{
					return;
				}
				var item = m_itemList [itemListLastCount];

				m_itemList.RemoveAt (itemListLastCount);
                
				


				m_currentItemNo--;

				MessageListEntity.MessgeData msg = _connectedGetDataList [m_currentItemNo - m_instantateItemCount + 2];

				itemObj = null;
				if (msg.send_user_id == MessageListApi._httpCatchData.result.from_user.id)
				{
					//自分メッセージ or Image
					if (msg.type == ((int)MessageType.Message).ToString () ||  msg.type == ((int)MessageType.Random).ToString ()) 
					{
						itemObj = Instantiate (_userMessageBase) as RectTransform;

					} else if (msg.type == ((int)MessageType.Image).ToString () || msg.type == ((int)MessageType.Movie).ToString ()) {
						itemObj = Instantiate (_userPictureBase) as RectTransform;

					}
				} else if (msg.send_user_id == MessageListApi._httpCatchData.result.to_user.id) {
					//相手メッセージ or Image
					if (msg.type == ((int)MessageType.Message).ToString () ||  msg.type == ((int)MessageType.Random).ToString ()) 
					{
						itemObj = Instantiate (_toUserMessageBase) as RectTransform;
					} else if (msg.type == ((int)MessageType.Image).ToString () || msg.type == ((int)MessageType.Movie).ToString ()) {
						itemObj = Instantiate (_toPictureBase) as RectTransform;
					}
				}

				if (itemObj != null)
				{
                    if (this.transform.localPosition.y < Mathf.Abs (item.anchoredPosition.y))
                        Destroy (item.gameObject);

					itemObj.SetParent (transform, false);
					//m_itemList.Add (itemObj);

					// 画像か文章ながいなら縦幅の大きさの調整                              
					itemObj.anchoredPosition = _backupMessagePosition [m_currentItemNo - m_instantateItemCount + 2];


					itemObj.name = _connectedGetDataList [m_currentItemNo - m_instantateItemCount + 2].id;

					itemObj.gameObject.SetActive (true);
					// 初期化関数呼び出し
					//foreach( var controller in controllers )
					{
						//controller.OnUpdateItem(msgCount, itemObj.gameObject);
						itemObj.GetComponent<MessageDataSetItem> ().UpdateItem (m_currentItemNo - m_instantateItemCount + 2, _connectedGetDataList [m_currentItemNo - m_instantateItemCount + 2], MessageListApi._httpCatchData.result.to_user.profile_image_url);

					}
					//msgCount++;

				} else {
					Debug.Log ("gameobjectがヌル");
				}
				m_itemList.Insert (0, itemObj);

				float delta = _backupDeltaSize [m_currentItemNo + 1];         

				m_diffPreFramePosition += delta;

				//m_diffPreFramePositionUp += delta;
				//m_diffPreFramePositionDown = -_backupMessagePosition [m_currentItemNo+1].y;
			}  
		}
              
		// 現状のバー位置を記憶
		_lastBarPosiiton = GetComponentInParent<ScrollRect> ().verticalNormalizedPosition;

	}

	[Serializable]
	public class OnItemPositionChange : UnityEngine.Events.UnityEvent<int, GameObject>{}
    
	public void OnPostSetupItems ()
    {
        var infiniteScroll = GetComponent<MessageInfiniteLimitScroll> ();
        infiniteScroll.onUpdateItem.AddListener (OnUpdateItem);
        GetComponentInParent<ScrollRect> ().movementType = ScrollRect.MovementType.Elastic;
      
        var rectTransform = this.GetComponent<RectTransform> ();
        var delta = rectTransform.sizeDelta;
		//delta.y = infiniteScroll._itemScale * max + addPosCache;
		delta.y = addPosCache + 100f;

        rectTransform.sizeDelta = delta;

		Debug.Log ("addposcache " + addPosCache.ToString() + " addpos " + addPos.ToString());
    }


    protected string _thisDelta;
    public void OnUpdateItem (int itemCount, GameObject obj)
    {
        if (itemCount < 0 || itemCount >= max)
		{
            obj.SetActive (false);
        } else {
            obj.SetActive (true);

            if (obj.GetComponentInChildren<MessageDataSetItem> () != null)
            {
                var item = obj.GetComponentInChildren<MessageDataSetItem> ();
				List<MessageListEntity.MessgeData> d = _connectedGetDataList;

				// 検証用にIDに変更表示 さして意味はない
				item.name = _connectedGetDataList [itemCount].id;
                
				item.UpdateItem (itemCount, d[itemCount], MessageListApi._httpCatchData.result.to_user.profile_image_url);                
            }
        }
    }
}
