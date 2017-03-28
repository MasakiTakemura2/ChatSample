using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using EventManager;
using UnityEngine.UI;
using Http;
using ModelManager;

namespace ViewController
{
    public class PanelBulletinBoardWrite : SingletonMonoBehaviour<PanelBulletinBoardWrite>
    {
        [SerializeField]
        private InputField titleTextInputField;
        
        [SerializeField]
        private InputField bodyTextInputField;

		[SerializeField]
		private Text titleText;

		[SerializeField]
		private Text bodyText;

		[SerializeField]
		private GameObject imageGameObject1;

		[SerializeField]
		private GameObject imageGameObject2;

		[SerializeField]
		private GameObject imageGameObject3;

        [SerializeField]
        private GameObject _loadingOverlay;

        [SerializeField]
        private Dropdown _boardCategory;
        
        private List<string> _boradCategoryList = new List<string>();

        public string _category;
        


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
                        BulletinBoardEventManager.Instance.NewBoardClose (this.gameObject);
                    }
					Debug.Log ("Right Right Right Right Right Right Right ");

                }
            }
        }
     //ポップアップは一度だけ。
     private bool _isBulletinBoardLimitChar = false;

     /// <summary>
     /// Fixeds the update.
     /// </summary>
     void FixedUpdate () 
     {
         if (titleTextInputField != null) {
             if (titleTextInputField.text.Length >= 20) 
             {
                 if (_isBulletinBoardLimitChar == false) {
                    PopupPanel.Instance.PopClean ();
                    PopupPanel.Instance.PopMessageInsert(
                        LocalMsgConst.BBS_TITLE_VALIDATE,
                        LocalMsgConst.OK,
                        PopCloseEvent
                    );

                    BulletinBoardEventManager.Instance.PanelPopupAnimate (GameObject.FindGameObjectWithTag (CommonConstants.POPUP_BASIC_TAG));
                    _isBulletinBoardLimitChar = true;
                }
                return;
             }
         }
     }

     /// <summary>
     /// Pops the close event.
     /// </summary>
     void PopCloseEvent () 
     {
         PopupPanel.Instance.PopClean ();
         BulletinBoardEventManager.Instance.PanelPopupCloseAnimate (GameObject.FindGameObjectWithTag (CommonConstants.POPUP_BASIC_TAG));
     }

        #region 初期表示処理。
        public void Init () 
        {
        
            StartCoroutine (InitWait ());
        }
        private IEnumerator InitWait () 
        {
            _loadingOverlay.SetActive (true);

			while (InitDataApi._httpCatchData == null) 
			{
				yield return (InitDataApi._httpCatchData != null);
			}

            //フィールドセット処理。
            if (InitDataApi._httpCatchData.result.board_category.Count > 0 )
			{
				_boradCategoryList.Clear ();
                foreach (var category in InitDataApi._httpCatchData.result.board_category)
				{
					//Debug.Log (category.id);
					//Debug.Log (category.name);
                    _boradCategoryList.Add (category.name);
                }
                
				_boardCategory.ClearOptions ();
                _boardCategory.options.Clear ();
                _boardCategory.AddOptions (_boradCategoryList);
            } 
            _loadingOverlay.SetActive (false);

			// カテゴリー初期化
			string name = _boradCategoryList[_boardCategory.value];
			var d = CommonModelHandle.GetByNameBaseData (name, CurrentProfSettingStateType.BoardCategory);
			_category = d.id;
        }
       
        /// <summary>
        /// Categories the changed.
        /// </summary>
        /// <param name="result">Result.</param>
        public void CategoryChanged(int result)
        {
             string name = _boradCategoryList[_boardCategory.value];
             var d = CommonModelHandle.GetByNameBaseData (name, CurrentProfSettingStateType.BoardCategory);
             _category = d.id;
        }
		#endregion
        

		#region get WriteBoard Info
        /// <summary>
        /// Resets the new bbs.
        /// </summary>
        public void ResetNewBbs() {
            titleTextInputField.text = "";
            bodyTextInputField.text  = "";
            titleText.text = "";
            bodyText.text  = "";
            //TODO: これもリセット対象？
            //imageGameObject1.GetComponent<RawImage>().texture = 
        }
        
        /// <summary>
        /// Gets the title.
        /// </summary>
        /// <returns>The title.</returns>
		public string getTitle()
		{
			if (titleText != null && titleText.text.Length > 0) 
			{
				return titleText.text;
			}
			return null;
		}

        /// <summary>
        /// Gets the body.
        /// </summary>
        /// <returns>The body.</returns>
		public string getBody()
		{
			if (bodyText != null && bodyText.text.Length > 0) 
			{
				return bodyText.text;
			}
			return null;
		}

        public Texture2D getImage1()
		{
			if (imageGameObject1 != null)
			{
                if (imageGameObject1.activeSelf == false) 
                {
                    return null;
                }

				Texture texture = imageGameObject1.GetComponent<RawImage> ().mainTexture;
                
				if (texture != null) 
				{
					Texture2D currentTexture = texture as Texture2D;
					return currentTexture;
				}
			}
			return  null;
		}
		public Texture2D getImage2()
		{
			if (imageGameObject2 != null) 
			{
                if (imageGameObject2.activeSelf == false)
                {
                    return null;
                }
            
				Texture texure = imageGameObject2.GetComponent<RawImage> ().mainTexture;
				if (texure != null) 
				{
					Texture2D currentTexture = texure as Texture2D;
					return currentTexture;
				}
			}
			return  null;
		}

		public Texture2D getImage3()
		{
            if (imageGameObject3 != null)
			{
                if (imageGameObject3.activeSelf == false) 
				{
                    return null;
                }

				Texture texure = imageGameObject3.GetComponent<RawImage> ().mainTexture;
				if (texure != null) 
				{
					Texture2D currentTexture = texure as Texture2D;
					return currentTexture;
				}
			}
			return  null;
		}
		#endregion
    }
}