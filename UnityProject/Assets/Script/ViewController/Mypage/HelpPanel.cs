using UnityEngine;
using System.Collections;
using Http;
using Helper;
using UnityEngine.UI;
using EventManager;

namespace ViewController
{
	public class HelpPanel : SingletonMonoBehaviour<HelpPanel>
	{
		[SerializeField]
		public GameObject _helpListInfiniteLimitScroll;
        
		public void Initialize()
		{
			StartCoroutine (Init());
		}
      
		// 初期化 changepaeはページが進むか戻るか
		private IEnumerator Init ()
        {
            MypageEventManager.Instance.LoadingSwitch (true);

            // 通信レスポンス待ってから
            new HelpListApi ();
            while (HelpListApi._success == false) {
                yield return (HelpListApi._success == true);
            }

            if (HelpListApi._httpCatchData.result.helps != null) {
                if (HelpListApi._httpCatchData.result.helps.Count > 0) {
                    _helpListInfiniteLimitScroll.GetComponent<HelpListInfiniteLimitScroll> ().Init ();
                }
            }

            MypageEventManager.Instance.LoadingSwitch (false);
        }         
	}
}
