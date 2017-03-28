using UnityEngine;
using System.Collections;
using EventManager;
using UnityEngine.UI;
using uTools;
using Http;


namespace ViewController
{
    public class PanelBoardDetail : SingletonMonoBehaviour<PanelBoardDetail>
    {
        [SerializeField]
        private GameObject _panelBoardDetail;

		[SerializeField]
		private GameObject _LoadingOverlay;

		// プロフィール詳細
		[SerializeField]
		private Text _boardHeaderName;

		// 掲示板のボディ部分
		[SerializeField]
		private Text _boardBodyTitle;

		[SerializeField]
		private Text _boardBodyContent;

		[SerializeField]
		private Text _boardCategory;

		[SerializeField]
		private RawImage _profImage;

		[SerializeField]
		private RawImage _thumnail1 = null;

		[SerializeField]
		private RawImage _thumnail2 = null;

		[SerializeField]
		private RawImage _thumnail3 = null;

        [SerializeField]
        private Transform _favoriteButton;

		[SerializeField]
		private Transform _limitReleaseButton;

		public Transform GetLimitReleaseButton()
		{
			return _limitReleaseButton;
		}

        /// <summary>
        /// The BBD ID
        /// </summary>
		public string _boadID = "";


     
        /// <summary>
        /// Init this instance.
        /// </summary>
		public void Init()
		{
			StartCoroutine (InitializeThread());
		}

        /// <summary>
        /// Initializes the thread.
        /// </summary>
        /// <returns>The thread.</returns>
		private IEnumerator InitializeThread()
		{
			_profImage.enabled = false;
			_thumnail1.enabled = false;
			_thumnail2.enabled = false;
			_thumnail3.enabled = false;
            
			// ユーザープロフィール更新
			_boardHeaderName.text ="";
			// 掲示板詳細画面更新
			_boardBodyTitle.text ="";
			_boardBodyContent.text ="";

			_LoadingOverlay.SetActive (true);
			new ReadBoardApi (AppStartLoadBalanceManager._userKey, _boadID);
			while (ReadBoardApi._success == false) 
			{
				yield return (ReadBoardApi._success == true);
			}
			_LoadingOverlay.SetActive (false);


             string isFavorite = ReadBoardApi._httpCatchData.result.board.user.is_favorite;
            if (isFavorite == "true")
			{
                 _favoriteButton.GetChild (0).gameObject.SetActive (true);
            } else if (isFavorite == "false") {
                 _favoriteButton.GetChild (0).gameObject.SetActive (false);
            }

			string islimitrelease = ReadBoardApi._httpCatchData.result.board.user.is_limit_release;
			if (islimitrelease == "true")
			{
				_limitReleaseButton.GetComponent<Button> ().enabled = false;
				_limitReleaseButton.GetComponent<Image> ().color = new Color (147/255.0f,147/255.0f,147/255.0f,255);
			} else if (islimitrelease == "false") {
				_limitReleaseButton.GetComponent<Image> ().color = new Color (247/255.0f,117/255.0f,133/255.0f,255);
				_limitReleaseButton.GetComponent<Button> ().enabled = true;
			}


			// 掲示板詳細画面更新
			_boardHeaderName.text  = ReadBoardApi._httpCatchData.result.board.user.name;
			_boardBodyTitle.text   = ReadBoardApi._httpCatchData.result.board.title;
			_boardBodyContent.text = ReadBoardApi._httpCatchData.result.board.body;
			_boardCategory.text    = ReadBoardApi._httpCatchData.result.board.board_category.name;

			// プロフィール画面へ飛ぶ用にデータをわたす
			BulletinBoardEventManager.Instance._profileUserData = ReadBoardApi._httpCatchData.result.board.user;

            
			// ヘッダー画像データ設定
			Debug.Log("url " + ReadBoardApi._httpCatchData.result.board.user.profile_image_url);

			StartCoroutine(WwwToRendering(ReadBoardApi._httpCatchData.result.board.user.profile_image_url, _profImage));
			if (ReadBoardApi._httpCatchData.result.board.user.profile_image_url != "") {
				_profImage.enabled = true;
			} else {
				_profImage.texture = Resources.Load ("Texture/noimage_user") as Texture;
			}

			//サムネイル３つ
			if(ReadBoardApi._httpCatchData.result.board.images != null)
			{
				if(ReadBoardApi._httpCatchData.result.board.images.Count > 0)
				{
					for(int index=0; index<ReadBoardApi._httpCatchData.result.board.images.Count; ++index)
					{
						switch(index)
						{
							case 0:
							if(ReadBoardApi._httpCatchData.result.board.images [index].url != null)
							{
								StartCoroutine (WwwToRendering (ReadBoardApi._httpCatchData.result.board.images [index].url, _thumnail1));
								_thumnail1.enabled = true;
								_thumnail1.transform.parent.gameObject.SetActive (true);
							}
							break;

							case 1:
							if (ReadBoardApi._httpCatchData.result.board.images [index].url != null)
							{
								StartCoroutine (WwwToRendering (ReadBoardApi._httpCatchData.result.board.images [index].url, _thumnail2));
								_thumnail2.enabled = true;
                                _thumnail2.transform.parent.gameObject.SetActive (true);
							}

							break;

							case 2:
							if (ReadBoardApi._httpCatchData.result.board.images [index].url != null) 
							{
								StartCoroutine (WwwToRendering (ReadBoardApi._httpCatchData.result.board.images [index].url, _thumnail3));
								_thumnail3.enabled = true;
                                _thumnail3.transform.parent.gameObject.SetActive (true);
							}
							break;

						}
					}
				}
			}

			// ボタン制御用におくる
			GetUserApi._httpOtherUserCatchData = ReadBoardApi._httpCatchData.result.board.user; 

		}
		private IEnumerator WwwToRendering (string url, RawImage targetObj)
		{
            targetObj.texture = null;
            targetObj.gameObject.SetActive (false);
			if (string.IsNullOrEmpty (url) == true) yield break;

			using (WWW www = new WWW (url))
			{
				while (www == null)
					yield return (www != null);

				while (www.isDone == false)
					yield return (www.isDone);

				//non texture file
				if (string.IsNullOrEmpty (www.error) == false) 
				{
					Debug.LogError (www.error);
					Debug.Log (url);
					yield break;
				}
				while (targetObj == null)
					yield return (targetObj != null);

                targetObj.gameObject.SetActive (true);
				targetObj.texture = www.texture;

			}
		}


        void OnSwipe (SwipeGesture gesture) 
		{
            if (gesture.Selection) 
			{
                if (gesture.Direction == FingerGestures.SwipeDirection.Left) 
				{
                    //Debug.Log ("Left Left Left Left Left Left ");
                } else if (gesture.Direction == FingerGestures.SwipeDirection.Right) {
                  
					if (BulletinBoardEventManager.Instance != null) 
					{
						BulletinBoardEventManager.Instance.BoardDetailClose (_panelBoardDetail);
					}
                }
            }
        }


		#region 掲示板に配置されているボタン制御群
	
		public void OnBoardDetailLimitReleaseButton()
		{
			PanelChat.Instance._toUser = ReadBoardApi._httpCatchData.result.board.user.id;
			PanelChat.Instance.LimitMessageConfirmOpen ();
		}


		// 通報ボタン
		public void ReportConfirmOpen(GameObject target)
		{
			string popText = string.Format(LocalMsgConst.REPORT_QUESTION, GetUserApi._httpOtherUserCatchData.name);
	    	PopupSecondSelectPanel.Instance.PopMessageInsert(
				popText,
	       		LocalMsgConst.YES,
	        	LocalMsgConst.NO,
	        	ReportApiCall,
	        	ReportCancel
	    	);
	    	PanelPopupAnimate (target);
		}

		void ReportApiCall()
		{
			PopupSecondSelectPanel.Instance.PopClean();

			StartCoroutine (SendReportWait ());
			PanelPopupCloseAnimate (GameObject.FindGameObjectWithTag(CommonConstants.POPUP_SECOND_SELECT_TAG));
		}

		private IEnumerator SendReportWait ()
		{
			_LoadingOverlay.SetActive (true);
			new SendReportApi (GetUserApi._httpOtherUserCatchData.id);
			while (SendReportApi._success == false)
			{
				yield return (SendReportApi._success == true);
			}
			_LoadingOverlay.SetActive (false);

			PopupPanel.Instance.PopMessageInsert (
				SendReportApi._httpCatchData.result.complete[0],
				LocalMsgConst.OK,
				ReportFinishClose
			);
			PanelPopupAnimate (GameObject.FindGameObjectWithTag(CommonConstants.POPUP_BASIC_TAG));
		}

		void ReportCancel () 
		{
			PopupSecondSelectPanel.Instance.PopClean();

			PanelPopupCloseAnimate (GameObject.FindGameObjectWithTag(CommonConstants.POPUP_SECOND_SELECT_TAG));
		}

		void ReportFinishClose () 
		{
			PopupPanel.Instance.PopClean ();
			PanelPopupCloseAnimate (GameObject.FindGameObjectWithTag(CommonConstants.POPUP_BASIC_TAG));
		}

		// お気に入りボタン
        public void FavoriteConfirmOpen(GameObject target)
        {
        	string popText = "";

            if (_favoriteButton.GetChild(0).gameObject.activeSelf == true)
			{
            	popText = string.Format (LocalMsgConst.FAVORITE_OFF_CONFIRM, GetUserApi._httpOtherUserCatchData.name);
            } else if (_favoriteButton.GetChild(0).gameObject.activeSelf == false) {
                popText = string.Format (LocalMsgConst.FAVORITE_ON_CONFIRM, GetUserApi._httpOtherUserCatchData.name);
            }

            PopupSecondSelectPanel.Instance.PopMessageInsert(
        		popText,
                LocalMsgConst.YES,
                LocalMsgConst.NO,
                FavoriteApiCall,
                FavoriteCancel
            );
            PanelPopupAnimate (target);
        }

		void FavoriteApiCall ()
		{
			PopupSecondSelectPanel.Instance.PopClean();

			PanelPopupCloseAnimate (GameObject.FindGameObjectWithTag(CommonConstants.POPUP_SECOND_SELECT_TAG));
			StartCoroutine (SetUserFavoriteApiWait ());

		}

		private IEnumerator SetUserFavoriteApiWait () 
		{
			_LoadingOverlay.SetActive (true);
			new SetUserFavoriteApi (ReadBoardApi._httpCatchData.result.board.user.id);
			while (SetUserFavoriteApi._success == false) 
			{
				yield return (SetUserFavoriteApi._success == true);
			}
			_LoadingOverlay.SetActive (false);


			PopupPanel.Instance.PopMessageInsert(
				SetUserFavoriteApi._httpCatchData.result.complete[0],
				LocalMsgConst.OK,
				FavoriteFinishClose
			);
			PanelPopupAnimate (GameObject.FindGameObjectWithTag(CommonConstants.POPUP_BASIC_TAG));
            

			string isFavorite = SetUserFavoriteApi._httpCatchData.result.is_favorite;
            if (isFavorite == "true") 
			{
                _favoriteButton.GetChild (0).gameObject.SetActive (true);
            } else if (isFavorite == "false") {
                _favoriteButton.GetChild (0).gameObject.SetActive (false);
            }
		}

		void FavoriteCancel () 
		{
			PopupSecondSelectPanel.Instance.PopClean();

			PanelPopupCloseAnimate (GameObject.FindGameObjectWithTag(CommonConstants.POPUP_SECOND_SELECT_TAG));
		}

		void FavoriteFinishClose () 
		{
			PopupPanel.Instance.PopClean ();
		    PanelPopupCloseAnimate (GameObject.FindGameObjectWithTag(CommonConstants.POPUP_BASIC_TAG));
		}

        
		private void PanelPopupAnimate ( GameObject target )
        {
            //ポップ用の背景セット
            BulletinBoardEventManager.Instance.BackgroundOverRaySwitch (true);

            target.GetComponent<uTweenScale> ().from = Vector3.zero;
            target.GetComponent<uTweenScale> ().to = new Vector3 (1, 1 ,1 );
            target.GetComponent<uTweenScale> ().delay    = 0.001f;
            target.GetComponent<uTweenScale> ().duration = 0.25f;
            target.GetComponent<uTweenScale> ().ResetToBeginning ();
            target.GetComponent<uTweenScale> ().enabled = true;
        }

        private void PanelPopupCloseAnimate ( GameObject target )
        {
            //ポップ用の背景外す
            BulletinBoardEventManager.Instance.BackgroundOverRaySwitch (false);

            target.GetComponent<uTweenScale> ().delay    = 0.001f;
            target.GetComponent<uTweenScale> ().duration = 0.15f;
            target.GetComponent<uTweenScale> ().from = new Vector3 (1f, 1f ,1f );
            target.GetComponent<uTweenScale> ().to = Vector3.zero;
            target.GetComponent<uTweenScale> ().ResetToBeginning ();
            target.GetComponent<uTweenScale> ().enabled = true;
        }

        #endregion

    }
}