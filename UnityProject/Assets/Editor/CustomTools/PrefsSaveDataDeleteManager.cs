using UnityEngine;
using System.Collections;
using UnityEditor;
using System.IO;

namespace Tools
{
	public class PrefsSaveDataDeleteManager 
	{
		private static readonly string tutorialFileName       = Application.persistentDataPath + "/../Library/U2FsdGVkX18jRSO8DDX3cHVObPTckRzH7UVhTZfOqVXcnr04TG8JBU02IbVAQ2f4=.dat";
		private static readonly string tutorialSecureFileName = Application.persistentDataPath + "/../Library/U2FsdGVkX18jRSO8DDX3cHVObPTckRzH7UVhTZfOqVXcnr04TG8JBU02IbVAQ2f4.dat";

		[MenuItem("Tools/TutorialDataDelete")]
		public static void TutorialDataOnlyDelete () 
		{
			File.Delete (tutorialFileName);
			File.Delete (tutorialSecureFileName);
			Debug.Log (" シーン毎のチュートリアル削除 ");
		}

//		[MenuItem("Tools/PrefsDataDeleteAll")]
//		public static void PrefsDataDeleteAll () 
//		{
//			Debug.Log ("prefs data all delete");
//		}
	}
}

