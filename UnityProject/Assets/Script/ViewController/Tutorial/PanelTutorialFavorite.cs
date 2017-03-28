using UnityEngine;
using System.Collections;

namespace ViewController {
    public class PanelTutorialFavorite : SingletonMonoBehaviour<PanelTutorialFavorite>
    {
        [SerializeField]
        private GameObject _text1;
        
        [SerializeField]
        private GameObject _text2;

        [SerializeField]
        private GameObject _popup;
        
        /// <summary>
        /// Steps the first.
        /// </summary>
        /// <returns>The first.</returns>
        public void StepFirst () 
        {
            _text1.SetActive (false);
            _text2.SetActive (true);
        }
        
        public void StepSecond () 
        {
            _text1.SetActive (false);
            _text2.SetActive (false);
            _popup.SetActive (true);
        }
        

        /// <summary>
        /// 最後お気に入りポップアップが出てきてチュートリアル完了
        /// Tutorials the end button.
        /// </summary>
        /// <returns>The end button.</returns>
        public void TutorialEndButton ( bool isOk) {
            //「OK」の場合は、本当に登録する
            if (isOk == true && PanelChat.Instance != null) 
            {
                PanelChat.Instance.FavoriteApiCall ();
            }
        
            //チュートリアルの状態を保存。
            Helper.LocalFileHandler.Init ( LocalFileConstants.GetLocalFileDir() + LocalFileConstants.COMMON_LOCAL_FILE_NAME);

Debug.Log (Helper.LocalFileHandler.GetBool (LocalFileConstants.TUTORIAL_FAVORITE_KEY) + " TUTORIAL_MESSAGE_KEY ") ;

            if (Helper.LocalFileHandler.GetBool (LocalFileConstants.TUTORIAL_FAVORITE_KEY) == false) 
            {
                Helper.LocalFileHandler.SetBool (LocalFileConstants.TUTORIAL_FAVORITE_KEY, true);
                Helper.LocalFileHandler.Flush ();
            }
            this.gameObject.SetActive (false);
        }
    }
}