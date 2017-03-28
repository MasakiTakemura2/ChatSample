using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using NendUnityPlugin.AD;
using NendUnityPlugin.Common;

namespace EventManager {
	public class NendAdBannerOrigenalManager : SingletonMonoBehaviour<NendAdBannerOrigenalManager>
	{
		[SerializeField]
		private NendAdBanner _nendAdBanner;

	    public void Init () {
		    if (_nendAdBanner != null) {
				_nendAdBanner.AdLoaded           += OnFinishLoadBannerAd;
				_nendAdBanner.AdReceived         += OnReceiveBannerAd;
				_nendAdBanner.AdFailedToReceive  += OnFailToReceiveBannerAd;
				_nendAdBanner.AdClicked          += OnClickBannerAd;
				_nendAdBanner.AdBacked           += OnDismissScreen;
				_nendAdBanner.InformationClicked += OnClickInformation;
			}
		}

	public void OnFinishLoadBannerAd (object sender, EventArgs args)
	{
		UnityEngine.Debug.Log ("広告のロードが完了しました。");
	}

	public void OnClickBannerAd (object sender, EventArgs args)
	{
		UnityEngine.Debug.Log ("広告がクリックされました。");
	}

	public void OnReceiveBannerAd (object sender, EventArgs args)
	{
		UnityEngine.Debug.Log ("広告の受信に成功しました。");
	}

	public void OnFailToReceiveBannerAd (object sender, NendAdErrorEventArgs args)
	{
		UnityEngine.Debug.Log ("広告の受信に失敗しました。エラーメッセージ: " + args.Message);
	}

	public void OnDismissScreen (object sender, EventArgs args)
	{
		UnityEngine.Debug.Log ("広告が画面上に復帰しました。");
	}

	public void OnClickInformation(object sender, EventArgs args)
	{
		UnityEngine.Debug.Log ("インフォメーションボタンがクリックされオプトアウトページに遷移しました。");
	}

	}
}