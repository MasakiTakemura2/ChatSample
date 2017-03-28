using System;
using UnityEngine;
using System.Collections;


namespace ViewController {
    public class PanelTutorialMatching : SingletonMonoBehaviour<PanelTutorialMatching>
    {

        [SerializeField]
        private ScreenRaycaster _tinScreenRay;

        void OnEnable () {
            _tinScreenRay.enabled = false;
        }
         
    
        /// <summary>
        /// Tutorials the end button.
        /// </summary>
        /// <returns>The end button.</returns>
        public void TutorialEndButton () {
            //チュートリアルの状態を保存。
            Helper.LocalFileHandler.Init ( LocalFileConstants.GetLocalFileDir() + LocalFileConstants.COMMON_LOCAL_FILE_NAME);
Debug.Log (Helper.LocalFileHandler.GetBool (LocalFileConstants.TUTORIAL_MATCHING_KEY) + " TUTORIAL_MATCHING_KEY ") ;
            if (Helper.LocalFileHandler.GetBool (LocalFileConstants.TUTORIAL_MATCHING_KEY) == false) 
            {
                Helper.LocalFileHandler.SetBool (LocalFileConstants.TUTORIAL_MATCHING_KEY, true);
                Helper.LocalFileHandler.Flush ();
            }
            this.gameObject.SetActive (false);
            _tinScreenRay.enabled = true;
        }
    }
}