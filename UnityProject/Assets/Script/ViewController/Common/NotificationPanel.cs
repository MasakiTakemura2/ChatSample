using UnityEngine;
using System.Collections;
using Helper;
using UnityEngine.UI;
using uTools;

namespace ViewController
{
	/// <summary>
	/// Notification panel.
	/// </summary>
	public class NotificationPanel : SingletonMonoBehaviour<NotificationPanel>
	{
		[SerializeField]
		private Text _title;

		[SerializeField]
		private Text _body;

		private string _viewName;
		private string _id;

		bool _stoper = false;

		float _time = 0;
		const float WAIT_TIME = 2.0f;

		void Start () 
		{
			_time = 0;
		}

		/// <summary>
		/// Update this instance.
		/// </summary>
		void Update() {
			if(this.gameObject.activeSelf && _stoper == false)
			{
				_time += Time.deltaTime;
				if(_time >= WAIT_TIME)
				{
					this.GetComponent<uTweenAlpha>().enabled = true;
					if (this.GetComponent<Image>().color.a == 0) {
						Destroy (gameObject);	
					}
					_time = 0;
				}
			}
		}

		/// <summary>
		/// Init this instance.
		/// </summary>
		public void Init(string title, string body, string viewName = "", string id ="")
		{
			_viewName = viewName;
			_id = id;
			_title.text = title;
			_body.text = body;
		}

		/// <summary>
		/// View this instance.
		/// </summary>
		public void ViewTouch()
		{
			NotificationRecieveManager.NextSceneProccess (_viewName + " " + _id);
		}
	}
}