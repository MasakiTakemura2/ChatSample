using UnityEngine;
using System.Collections;

namespace Helper {
    public class AndroidHeaderAdjust : SingletonMonoBehaviour<AndroidHeaderAdjust> {

        // Use this for initialization
        void Start () { Debug.Log ("Use this for initialization"); }
    	//void Start () {
            //TODO: 後で調整する。
            //#if UNITY_ANDROID
            ////ステータスバーを表示 //Android用
            //ApplicationChrome.statusBarState = ApplicationChrome.States.VisibleOverContent;
            //this.GetComponent<RectTransform>().sizeDelta = new Vector2 (this.GetComponent<RectTransform>().sizeDelta.x, 65.5f);
            //this.GetComponent<RectTransform>().anchoredPosition = new Vector2 (this.GetComponent<RectTransform>().anchoredPosition.x, 32.5f);
            //#endif
    	//}
    }
}