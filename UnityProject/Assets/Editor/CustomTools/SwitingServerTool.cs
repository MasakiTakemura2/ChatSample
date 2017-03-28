using UnityEngine;
using System.Linq;
using UnityEditor;
using System;
using Helper;


/// <summary>
/// Switing server tool.
/// サーバー環境をEDITORからスイッチする。
/// </summary>
namespace Tools {
	public class SwitingServerTool
	{
		[MenuItem("Tools/Server/LOCAL")]
		public static void SwitchLocal()
		{

            string s = LocalFileConstants.GetLocalFileDir () + LocalFileConstants.COMMON_LOCAL_FILE_NAME;
            LocalFileHandler.Init ( LocalFileConstants.GetLocalFileDir() + LocalFileConstants.COMMON_LOCAL_FILE_NAME);
            LocalFileHandler.SetInt (LocalFileConstants.EDITOR_ONLY_DOMAIN_SWICHER, (int)SeverMachineType.LOCAL);
            LocalFileHandler.Flush ();
			Debug.Log ("サーバーをローカルマシンに設定しました");

		}

		[MenuItem("Tools/Server/DEVELOP")]
		public static void SwitchDevelp()
		{
            LocalFileHandler.Init (LocalFileConstants.GetLocalFileDir() + LocalFileConstants.COMMON_LOCAL_FILE_NAME);
            LocalFileHandler.SetInt (LocalFileConstants.EDITOR_ONLY_DOMAIN_SWICHER, (int)SeverMachineType.DEV);
            LocalFileHandler.Flush ();
			Debug.Log ("サーバーを開発環境マシンに設定しました");
		}

		[MenuItem("Tools/Server/STAGING")]
		public static void SwitchStaging()
		{
            LocalFileHandler.Init (LocalFileConstants.GetLocalFileDir() + LocalFileConstants.COMMON_LOCAL_FILE_NAME);
            LocalFileHandler.SetInt (LocalFileConstants.EDITOR_ONLY_DOMAIN_SWICHER, (int)SeverMachineType.STG);
            LocalFileHandler.Flush ();
			Debug.Log ("サーバーをステージング環境マシンに設定しました");
		}

		[MenuItem("Tools/Server/PRODUCTION")]
		public static void SwitchProduction()
		{
            LocalFileHandler.Init (LocalFileConstants.GetLocalFileDir() + LocalFileConstants.COMMON_LOCAL_FILE_NAME);
            LocalFileHandler.SetInt (LocalFileConstants.EDITOR_ONLY_DOMAIN_SWICHER, (int)SeverMachineType.PRODUCTION);
            LocalFileHandler.Flush ();
			Debug.Log ("サーバーを本番環境マシンに設定しました");
		}
	}
}