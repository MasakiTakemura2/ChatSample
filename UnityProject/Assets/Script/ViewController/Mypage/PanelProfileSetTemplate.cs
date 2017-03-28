using UnityEngine;
using System.Collections;
using EventManager;

namespace ViewController 
{
    /// <summary>
    /// Panel profile set template.
    /// </summary>
    public class PanelProfileSetTemplate : SingletonMonoBehaviour<PanelProfileSetTemplate>
    {
        [SerializeField]
        private ProfTemplateInfiniteLimitScroll _profTemplateInfiniteLimitScroll;

        #region Finger Event
        private CurrentProfSettingStateType _profType;
        /// <summary>
        /// Backs the swipe.
        /// </summary>
        void OnSwipe (SwipeGesture gesture) {
            if (gesture.Selection) {
                if (gesture.Direction == FingerGestures.SwipeDirection.Left)
                {
                    //Debug.Log ("Left Left Left Left Left Left ");
                }
                else if (gesture.Direction == FingerGestures.SwipeDirection.Right)
                {
                    if (StartEventManager.Instance != null)
                    {
                        switch (StartEventManager.instance._currentProfSettingState) {
                        case CurrentProfSettingStateType.Pref:
                            StartEventManager.Instance.PlaceOfOriginClose (this.gameObject);
                            break;
                        case CurrentProfSettingStateType.City:
                            StartEventManager.Instance.PlaceOfOriginDetailClose (this.gameObject);
                            break;
                        }
                    }

                    if (MypageEventManager.Instance != null)
                    {
                        switch (MypageEventManager.instance._currentProfSettingState) {
                        case CurrentProfSettingStateType.Pref:
                            PanelProfileChange.Instance.PanelEditClose ();
                            break;
                        case CurrentProfSettingStateType.City:
                            PanelProfileChange.Instance.PanelEditClose ();
                            break;
                        case CurrentProfSettingStateType.BloodType:
                            PanelProfileChange.Instance.PanelEditClose ();
                            break;
                        case CurrentProfSettingStateType.HairStyle:
                            PanelProfileChange.Instance.PanelEditClose ();
                            break;
                        case CurrentProfSettingStateType.Glasses:
                            PanelProfileChange.Instance.PanelEditClose ();
                            break;
                        case CurrentProfSettingStateType.Type:
                            PanelProfileChange.Instance.PanelEditClose ();
                            break;
                        case CurrentProfSettingStateType.Personality:
                            PanelProfileChange.Instance.PanelEditClose ();
                            break;
                        case CurrentProfSettingStateType.Holiday:
                            PanelProfileChange.Instance.PanelEditClose ();
                            break;
                        case CurrentProfSettingStateType.AnnualIncome:
                            PanelProfileChange.Instance.PanelEditClose ();
                            break;
                        case CurrentProfSettingStateType.Education:
                            PanelProfileChange.Instance.PanelEditClose ();
                            break;
                        case CurrentProfSettingStateType.Housemate:
                            PanelProfileChange.Instance.PanelEditClose ();
                            break;
                        case CurrentProfSettingStateType.Sibling:
                            PanelProfileChange.Instance.PanelEditClose ();
                            break;
                        case CurrentProfSettingStateType.Alcohol:
                            PanelProfileChange.Instance.PanelEditClose ();
                            break;
                        case CurrentProfSettingStateType.Tobacco:
                            PanelProfileChange.Instance.PanelEditClose ();
                            break;
                        case CurrentProfSettingStateType.Car:
                            PanelProfileChange.Instance.PanelEditClose ();
                            break;
                        case CurrentProfSettingStateType.Pet:
                            PanelProfileChange.Instance.PanelEditClose ();
                            break;
                        case CurrentProfSettingStateType.Hobby:
                            PanelProfileChange.Instance.PanelEditClose ();
                            break;
                        case CurrentProfSettingStateType.Interest:
                            PanelProfileChange.Instance.PanelEditClose ();
                            break;
                        case CurrentProfSettingStateType.Marital:
                            PanelProfileChange.Instance.PanelEditClose ();
                            break;
                        }
                    }
                }
            }
        }
        #endregion

        #region 外部用メソッド。
        public void SetInfiniteLimitScroll()
        {
            if (PanelProfileChange.Instance != null) {
                PanelProfileChange.Instance._profTemplatefInfiniteLimitScroll = _profTemplateInfiniteLimitScroll;
            }
        }
        #endregion
    }
}