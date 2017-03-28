using UnityEngine;
using System.Collections;
using UnityEngine.UI;

namespace ViewController 
{
    public class GuidePanel : SingletonMonoBehaviour<GuidePanel>
    {
        [SerializeField]
        private GameObject _iosDescription;

        [SerializeField]
        private GameObject _androidDescription;

        [SerializeField]
        private Text _descriptionInAppliName;

        /// <summary>
        /// Init this instance.
        /// </summary>
        public void Init() 
        {
            #if !UNITY_EDITOR && UNITY_ANDROID
               _androidDescription.SetActive (true);
               _iosDescription.SetActive (false);
            #elif !UNITY_EDITOR && UNITY_IOS
               _androidDescription.SetActive (false);
               _iosDescription.SetActive (true);
            #else
               _androidDescription.SetActive (false);
               _iosDescription.SetActive (true);
            #endif
        }
    }
}
