using UnityEngine;
using System.Collections;
using Http;
using uTools;
using UnityEngine.SceneManagement;

namespace ViewController
{
    public class PanelGenderSelectCommon : SingletonMonoBehaviour<PanelGenderSelectCommon> 
    {
        [SerializeField]
        private GameObject _genderError;
    
        [SerializeField]
        private GameObject _maleButton;
    
        [SerializeField]
        private GameObject _femaleButton;

        [SerializeField]
        private GameObject _loadingOvelay;
        
        private bool _isGenderSelect = false;
        
        /// <summary>
        /// Init this instance.
        /// </summary>
        public void Init ()
        {
            this.GetComponent<uTweenPosition> ().enabled = true;
        }
        
        /// <summary>
        /// Sets the data.
        /// </summary>
        public void SetData(GameObject obj)
        { 
            _isGenderSelect = true;
            if (_maleButton.name == obj.name) {
                AppStartLoadBalanceManager._gender = ((int)GenderType.Male).ToString();
                obj.transform.transform.GetChild (0).gameObject.SetActive(true);
                _femaleButton.transform.GetChild (0).gameObject.SetActive (false);
            } else if (_femaleButton.name == obj.name) {
                AppStartLoadBalanceManager._gender = ((int)GenderType.FeMale).ToString();
                obj.transform.transform.GetChild (0).gameObject.SetActive(true);
                _maleButton.transform.GetChild (0).gameObject.SetActive (false);                
            }
        }
        
        /// <summary>
        /// Genders the set button.
        /// </summary>
        public void GenderSetButton()
        {
            //男女の選択が出来ていない場合。
            if (_isGenderSelect == false) {
                _genderError.SetActive (true);
                return;
            }

            StartCoroutine (ProfUpdate());
        }
        
        /// <summary>
        /// Profs the update.
        ///通信処理。
        /// </summary>
        /// <returns>The update.</returns>
        private IEnumerator ProfUpdate()
        {
            _loadingOvelay.SetActive (true);
        
            //ユーザーデータ取得処理
            new GetUserApi ();
            while (GetUserApi._success == false)   
                yield return (GetUserApi._success == true);
    
            GetUserApi._httpCatchData.result.user.sex_cd = AppStartLoadBalanceManager._gender;

            //ユーザーアップデート処理
            new ProfileUpdateApi (GetUserApi._httpCatchData.result.user);
            while (ProfileUpdateApi._success == false)
                yield return (ProfileUpdateApi._success == true);
                
            _loadingOvelay.SetActive (false);

            SceneHandleManager.NextSceneRedirect(SceneManager.GetActiveScene().name);
            yield break;
        }
    }
}