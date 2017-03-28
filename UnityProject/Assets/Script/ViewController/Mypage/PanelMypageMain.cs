using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using uTools;
using Http;

namespace ViewController
{
    /// <summary>
    /// Panel mypage main.
    /// </summary>
    public class PanelMypageMain : SingletonMonoBehaviour<PanelMypageMain>
    {
        [SerializeField]
        public RawImage _coverImage;
        
        [SerializeField]
        public RawImage _profImage;

        [SerializeField]
        private Text _nameAndAge;
        
        [SerializeField]
        private Text _myPoint;

        [SerializeField]
        private Transform _newMsgBadge;

        [SerializeField]
        private GameObject _randMsgBtn;

        [SerializeField]
        private Text _premiumText;
        
        [SerializeField]
        private GameObject _premiumButton;

        /// <summary>
        /// Init the specified isFromProfileSetting.
        /// </summary>
        /// <param name="isFromProfileSetting">If set to <c>true</c> is from profile setting.</param>
		public void Init (bool isFromProfileSetting = false) 
		{
            // CommonPurchasePanel.Instance.Init ();
            StartCoroutine (InitApiReload (isFromProfileSetting));
        }
        
        /// <summary>
        /// Inits the API reload.
        /// </summary>
        /// <returns>The API reload.</returns>
        private IEnumerator InitApiReload (bool isFromProfileSetting = false)
        {
            //StartCoroutine (WwwToRendering ("file:///storage/emulated/0/Lin3/lin3SelectedImage.jpg", _testRawImage));

            //プロフィールを変更パネルから戻ってきた時のみデータ更新。
            if (isFromProfileSetting == true) {
                new GetUserApi ();
                while (GetUserApi._success == false)
                    yield return (GetUserApi._success == true);
            }

            var user = GetUserApi._httpCatchData.result.user;

if (CommonConstants.IS_PREMIUM == true) {
    _premiumText.text = "有料会員";
    _premiumButton.GetComponent<Image> ().color         = new Color (255/255,233/255,1/255,255/255);
    _premiumButton.GetComponent<uTweenAlpha> ().enabled = false;
} else {
    _premiumText.text = "無料会員";
}

            if (_coverImage != null && string.IsNullOrEmpty (user.cover_image_url) == false) {
                if (user.cover_image_status == "1") {
                    _coverImage.texture = Resources.Load ("Texture/check_image_cover@2x") as Texture;
                } else {
                    yield return StartCoroutine (WwwToRendering (user.cover_image_url, _coverImage));
                }
            }

            if (_profImage != null && string.IsNullOrEmpty (user.profile_image_url) == false) {
                if (user.profile_image_status == "1") {
                    _profImage.texture = Resources.Load ("Texture/check_image_user@2x") as Texture;
                } else {
                    yield return StartCoroutine (WwwToRendering (user.profile_image_url, _profImage));
                }
            }


            if (_nameAndAge != null) {
                string ageStr = "";
                if (string.IsNullOrEmpty (user.age) == false) {
                    int age;
                    // 年齢
                    System.Text.RegularExpressions.Regex re = new System.Text.RegularExpressions.Regex ("-");
                    string pDate = re.Replace (user.birth_date, "/");

                    System.DateTime dt;
                    if (System.DateTime.TryParse(pDate, out dt))
                    {
                        System.DateTime birthDay = System.DateTime.Parse (pDate); // 誕生日を取得
                        System.DateTime today = System.DateTime.Today;
                         
                        age = today.Year - birthDay.Year;
                        age -= birthDay > today.AddYears (-age) ? 1 : 0; // 誕生日が来てない場合は1歳引く
                        ageStr = age.ToString ();
                    }

                } else {
                    ageStr = user.age;
                }

                if (string.IsNullOrEmpty (ageStr) == false && string.IsNullOrEmpty (user.name) == false) {
                    _nameAndAge.text = user.name + "  <size='40'>(" + ageStr + LocalMsgConst.AGE_TEXT + ")</size>";    
                } else if (string.IsNullOrEmpty (ageStr) == true && string.IsNullOrEmpty (user.name) == false) {
                    _nameAndAge.text = user.name;
                } else if (string.IsNullOrEmpty (ageStr) == false && string.IsNullOrEmpty (user.name) == true) {
                    _nameAndAge.text = LocalMsgConst.ANOUYMUS_NAME + "  <size='40'>(" + ageStr + LocalMsgConst.AGE_TEXT + ")</size>";
                } else {
                    _nameAndAge.text = "";
                }
             }

            if (_myPoint != null){
                int poInt = int.Parse (user.current_point);
                string poText = string.Format ("{0:#,0}", poInt);
                _myPoint.text = poText + " " + LocalMsgConst.PT_TEXT;
            }

            //メッセージボタン部分にバッジを仕込み
            if (string.IsNullOrEmpty (AppStartLoadBalanceManager._msgBadge) == false) 
            {
                int badgeCount = int.Parse (AppStartLoadBalanceManager._msgBadge);
                if (badgeCount > 0) {
                    _newMsgBadge.gameObject.SetActive(true);
                } else {
                    _newMsgBadge.gameObject.SetActive(false);
                }
            }

           if (GetUserApi._httpCatchData != null) {
               if (GetUserApi._httpCatchData.result.review == "true") {
                   _randMsgBtn.SetActive (false);
               } else if (GetUserApi._httpCatchData.result.review == "false"){
                    _randMsgBtn.SetActive (true);
               }
           } else {
                _randMsgBtn.SetActive (true);
           }

            yield break;
        }

        private IEnumerator WwwToRendering (string url, RawImage targetObj)
        {
            targetObj.texture = null;
            targetObj.gameObject.SetActive (false);
            if (string.IsNullOrEmpty (url) == true) yield break;
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

                targetObj.gameObject.SetActive (true);
                targetObj.texture = www.texture;
            }
        }
        
        
        /// <summary>
        /// Premiums the service introduction button.
        /// </summary>
        public void PremiumServiceIntroductionButton() {
            CommonPurchasePanel.Instance.OpenPopupAnimate (CommonPurchasePanel.Instance.gameObject);
        }
        
    }
}