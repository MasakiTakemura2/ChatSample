using System.Collections;
using System.Collections.Generic;
using EventManager;
using uTools;
using UnityEngine;

namespace ViewController {
    public class PremiumPushPanel : SingletonMonoBehaviour<PremiumPushPanel> {
        public void Init () {
			if (CommonConstants.IS_PREMIUM == false) {
				int s = Random.Range(1,3);

				if (s == 1) {
					MypageEventManager.Instance.PanelPopupAnimate (this.gameObject);
				}
			}
        }

        public void CloseButton () {
            MypageEventManager.Instance.PanelPopupCloseAnimate (this.gameObject);
			CommonPurchasePanel.Instance.OpenPopupAnimate (CommonPurchasePanel.Instance.gameObject);
        }
    }
}