using UnityEngine;
using System.Collections;
using Http;
using Helper;
using UnityEngine.UI;

namespace ViewController
{
	public class PanelHelpDetail : SingletonMonoBehaviour<PanelHelpDetail>
	{
		[SerializeField]
		public Text _title;

		[SerializeField]
		public Text _description;

		public void Init(string question, string answer)
		{
			_title.text = question;
			_description.text = answer;
		}
		

	}
}
