using UnityEngine;
using System.IO;
using UnionAssets.FLE;
using System.Collections.Generic;


#if UNITY_EDITOR
using UnityEditor;
[InitializeOnLoad]
#endif

public class ISDSettings : ScriptableObject
{

	public const string VERSION_NUMBER = "1.03 by Fujisaki Coustom";

	public bool IsFrameworksSettingOpen;
	public bool IsLinkerSettingOpen;
	public bool IsCompilerSettingsOpen = true;
	public bool IsSchemeSettingOpen;
	public bool IsRegionSettingOpen;
	public bool IsAppTransportSecurityOpen;
	public bool IsAppUsesEncryptionOpen;
    public bool IsEnableBitcodeOpen;


	public List<string> frameworks = new List<string>();
	public List<string> compileFlags = new List<string>();
	public List<string> linkFlags = new List<string>();
	public List<string> urlSchemes = new List<string>();
	public List<string> regionSettings = new List<string>();
	public List<string> appTransportSecurity = new List<string>();
	public List<string> appUsesEncryption = new List<string>();
    //public string appUsesEncryption = string.Empty;
    public string enableBitcode = string.Empty;


	private const string ISDAssetPath = "Extensions/IOSDeploy/Resources";
	private const string ISDAssetName = "ISDSettingsResource";
	private const string ISDAssetExtension = ".asset";

	private static ISDSettings instance;

	public static ISDSettings Instance
	{
		get
		{
			if(instance == null)
			{
				instance = Resources.Load(ISDAssetName) as ISDSettings;
				if(instance == null)
				{
					instance = CreateInstance<ISDSettings>();
					#if UNITY_EDITOR


					//FileStaticAPI.CreateFolder(ISDAssetPath);
					
					string fullPath = Path.Combine(Path.Combine("Assets", ISDAssetPath), ISDAssetName + ISDAssetExtension );
					
					AssetDatabase.CreateAsset(instance, fullPath);
					#endif

				}
			}
			return instance;
		}
	}


}
