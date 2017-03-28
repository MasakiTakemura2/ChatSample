using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using UnityEngine.Events;
using UnityEngine.UI;
using uTools;

namespace ViewController
{
    public class HeaderPanel : SingletonMonoBehaviour<HeaderPanel>
    {
       [SerializeField]
        private GameObject _backButton;

        [SerializeField]
        private GameObject _uiChangeButton;

        [SerializeField]
        private Text _headerTitle;

        [SerializeField]
        private GameObject _sideMenuIcon;

        /// <summary>
        /// Start this instance.
        /// </summary>
        void Start ()
        {
            //試験的にMYPageシーンのみ
            if (SceneManager.GetActiveScene().name == CommonConstants.MYPAGE_SCENE) {
                //_sideMenuIcon.SetActive (true);
            }
        }

		/// <summary>
		/// Android Hardware Back Button Event 
		/// </summary>
		void Update(){

			#if UNITY_ANDROID
			if (Input.GetKeyDown (KeyCode.Escape)) {

				Debug.Log("Back Button Event Start");
				//Check is there any popup
				GameObject popupOverlayGameObject = GameObject.FindWithTag(CommonConstants.POPUP_OVERLAY);

				if(popupOverlayGameObject!=null && popupOverlayGameObject.activeSelf){
					Debug.Log("Back Button Popup OverLayOpen");
					return;
				}

				GameObject cameraOrGalleryGameObject = GameObject.FindWithTag(CommonConstants.CAMERA_OR_GALLERY);

				if(cameraOrGalleryGameObject!=null ){
					if (cameraOrGalleryGameObject.GetComponent<uTweenPosition>().to.y == 0) {
						Debug.Log("Back Button Camera Open");	  
						return;
					}
				}

				if (_backButton.activeSelf) {
					Debug.Log("Back Button View Back");	
					_backButton.GetComponent<Button> ().onClick.Invoke();

					return;
				}
				Debug.Log("Back Button Event Stop");
//				else {
//					Application.Quit(); 					
//					return;
//				}
			}
			#endif
				
		}

        /// <summary>
        /// Tos my page.
        /// </summary>
        public void ToMyPage() {
            if (SceneManager.GetActiveScene().name != CommonConstants.START_SCENE && SceneManager.GetActiveScene().name != CommonConstants.PROBLEM_SCENE)
                SceneHandleManager.NextSceneRedirect (CommonConstants.MYPAGE_SCENE);
        }

        /// <summary>
        /// Backs the button switch.
        /// </summary>
        public void BackButtonSwitch (bool isOn , UnityAction ActEvent = null, string title = null)
        {
            if (title != null && _headerTitle != null ) {
                _headerTitle.text = title;
            }

            if (isOn == true) {
                _backButton.GetComponent<Button>().onClick.AddListener (ActEvent);   
            } else if (isOn == false) {
                _backButton.GetComponent<Button> ().onClick.RemoveAllListeners ();
            }
 
            _backButton.SetActive (isOn);
        }

        /// <summary>
        /// Backs the button switch.
        /// </summary>
        public void UiButtonSwitch (bool isOn , UnityAction ActEvent = null)
        {
            if (isOn == true) 
			{
                _uiChangeButton.GetComponent<Button>().onClick.AddListener (ActEvent);    
            } else if (isOn == false) {
				_uiChangeButton.GetComponent<Button> ().onClick.RemoveAllListeners ();
            }

            _uiChangeButton.SetActive (isOn);
        }


        /// <summary>
        /// Sets the user interface button.
        /// </summary>
        /// <param name="twoImage">If set to <c>true</c> two image.</param>
		public void SetUIButton(bool twoImage)
		{
			if(EventManager.SearchEventManager.Instance != null)
			{
                if (_uiChangeButton.activeSelf == false)
                    _uiChangeButton.SetActive (true);

				if (twoImage)
				{
					_uiChangeButton.transform.GetChild (1).gameObject.SetActive (true);
					_uiChangeButton.transform.GetChild (0).gameObject.SetActive (false);

				} else {
					_uiChangeButton.transform.GetChild (0).gameObject.SetActive (true);
					_uiChangeButton.transform.GetChild (1).gameObject.SetActive (false);

				}
			}
		}
    }
}
