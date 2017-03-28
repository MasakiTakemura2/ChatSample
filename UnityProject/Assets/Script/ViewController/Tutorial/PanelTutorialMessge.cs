using UnityEngine;
using System.Collections;

namespace ViewController {
    public class PanelTutorialMessge : SingletonMonoBehaviour<PanelTutorialMessge>
    {
        [SerializeField]
        private GameObject _item1;

        [SerializeField]
        private GameObject _text1;
        
        [SerializeField]
        private GameObject _item2;

        [SerializeField]
        private GameObject _text2;
        
    
        /// <summary>
        /// Steps the first.
        /// </summary>
        /// <returns>The first.</returns>
        public void StepFirst () 
        {
            _item1.SetActive (false);
            _text1.SetActive (false);

            _item2.SetActive (true);
            _text2.SetActive (true);
        }
    
    
        /// <summary>
        /// Tutorials the end button.
        /// </summary>
        /// <returns>The end button.</returns>
        public void TutorialEndButton () {
            //チュートリアルの状態を保存。
            Helper.LocalFileHandler.Init ( LocalFileConstants.GetLocalFileDir() + LocalFileConstants.COMMON_LOCAL_FILE_NAME);

Debug.Log (Helper.LocalFileHandler.GetBool (LocalFileConstants.TUTORIAL_MESSAGE_KEY) + " TUTORIAL_MESSAGE_KEY ") ;

            if (Helper.LocalFileHandler.GetBool (LocalFileConstants.TUTORIAL_MESSAGE_KEY) == false) 
            {
                Helper.LocalFileHandler.SetBool (LocalFileConstants.TUTORIAL_MESSAGE_KEY, true);
                Helper.LocalFileHandler.Flush ();
            }
            this.gameObject.SetActive (false);
        }
    }
}