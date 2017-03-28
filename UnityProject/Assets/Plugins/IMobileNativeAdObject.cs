#if UNITY_4_2 || UNITY_4_1 || UNITY_4_0_1 || UNITY_4_0 || UNITY_3_5 || UNITY_3_4 || UNITY_3_3 || UNITY_3_2 || UNITY_3_1 || UNITY_3_0_0 || UNITY_3_0 || UNITY_2_6_1 || UNITY_2_6
#define TWODEE_NOT_SUPPORTED
#endif
#if UNITY_4_5 || UNITY_4_3 || UNITY_4_2 || UNITY_4_1 || UNITY_4_0_1 || UNITY_4_0 || UNITY_3_5 || UNITY_3_4 || UNITY_3_3 || UNITY_3_2 || UNITY_3_1 || UNITY_3_0_0 || UNITY_3_0 || UNITY_2_6_1 || UNITY_2_6
#define GUI_NOT_SUPPORTED
#endif

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if !GUI_NOT_SUPPORTED
using UnityEngine.UI;
using UnityEngine.EventSystems;
#endif

public class IMobileNativeAdObject
{
	public string SpotId { get; set; }
	public string AdTitle { get; set; }
	public string AdDescription { get; set; }
	public string AdSponserd { get; set; }
	public Texture2D AdImage { get; set; }
	public int NativeAdObjectIndex { get; set; }
	public float RayDistance { get; set; }
	public List<string> ClickTargetGameObjectNameList = new List<string>();
	public string RecieverGameObjectName { get; set; }
	public int adViewId { get; set; }

	public static void CheckClickListener(IMobileNativeAdObject clickedNativeAdObject)
	{
		if (clickedNativeAdObject != null)
		{
			clickedNativeAdObject.sendClickEvent();
		}
	}


	/// <summary>
	/// (GameObject用) 広告をクリックできるよう設定します
	/// </summary>
	/// <param name="targetGameObject">クリックの対象とするGameObject</param>
	/// <param name="value">RayCastを使用する際、Rayが届くユニット数</param>
	public void setClickListener(GameObject targetGameObject, float targetRayDistance)
	{
		ClickTargetGameObjectNameList.Add(targetGameObject.name);
		RayDistance = targetRayDistance;
	}

	/// <summary>
	/// (uGUI用) 広告をクリックできるよう設定します
	/// </summary>
	/// <param name="targetGameObject">クリックの対象とするGameObject</param>
	public void setClickListener(GameObject targetGameObject)
	{
#if GUI_NOT_SUPPORTED
#warning uGUI is not applicable for Unity under Version 4.6. 
#else
		if (targetGameObject.GetComponent<Button>() == null)
		{
			targetGameObject.AddComponent<Button>();
		}
		Button button = targetGameObject.GetComponent<Button>();
		if (button.onClick.GetPersistentEventCount() != 0)
		{
			button.onClick.RemoveAllListeners();
		}
		button.onClick.AddListener(() =>
		{
			sendClick(RecieverGameObjectName, adViewId, NativeAdObjectIndex);
		});
#endif
	}

	private void sendClickEvent()
	{
		if (ClickTargetGameObjectNameList.Count == 0) return;
		if (Input.GetMouseButtonUp(0))
		{
			Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
			RaycastHit hit = new RaycastHit();
			if (Physics.Raycast(ray, out hit, RayDistance))
			{
				if (hit.collider != null && ClickTargetGameObjectNameList.Contains(hit.collider.gameObject.name))
				{
					sendClick(RecieverGameObjectName, adViewId, NativeAdObjectIndex);
				}
			}
#if !TWODEE_NOT_SUPPORTED
			else
			{
				Vector2 tapPoint = Camera.main.ScreenToWorldPoint(Input.mousePosition);
				Collider2D collider = Physics2D.OverlapPoint(tapPoint);
				if (collider != null && ClickTargetGameObjectNameList.Contains(collider.transform.gameObject.name))
				{
					sendClick(RecieverGameObjectName, adViewId, NativeAdObjectIndex);
				}
			}
#endif
		}
	}

	public static void sendClick(string recieverGameObjectName, int adViewId, int nativeAdObjectIndex)
	{
#if UNITY_IPHONE && !UNITY_EDITOR
		if(Application.platform == RuntimePlatform.IPhonePlayer) {
			IMobileSdkAdsUnityPlugin.sendClick(adViewId, nativeAdObjectIndex);
		}
#elif UNITY_ANDROID && !UNITY_EDITOR
		if(Application.platform == RuntimePlatform.Android) {
			IMobileSdkAdsUnityPlugin.getAndroidClass().CallStatic("onClickNativeAd", recieverGameObjectName, nativeAdObjectIndex );
		}
#endif
	}


}

