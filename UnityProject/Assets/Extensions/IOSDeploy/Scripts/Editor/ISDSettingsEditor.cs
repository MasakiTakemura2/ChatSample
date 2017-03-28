using UnityEngine;
using System.Collections;
using UnityEditor;
using System.Reflection;

[CustomEditor(typeof(ISDSettings))]
public class ISDSettingsEditor : Editor
{
	GUIContent SdkVersion   = new GUIContent("Plugin Version [?]", "This is the Plugin version.  If you have problems or compliments please include this so that we know exactly which version to look out for.");
	GUIContent SupportEmail = new GUIContent("Support [?]", "If you have any technical questions, feel free to drop us an e-mail.");

	/// <summary>
	/// Raises the inspector GU event.
	/// </summary>
	public override void OnInspectorGUI () {

		GUI.changed = false;


		EditorGUILayout.LabelField("iOS Deploy Settings", EditorStyles.boldLabel);
		EditorGUILayout.Space();

		Frameworks();
		EditorGUILayout.Space();

		LinkerFlags();
		EditorGUILayout.Space();

		CompilerFlags();
		EditorGUILayout.Space();

		UrlScheme ();
		EditorGUILayout.Space();

		RegionSettings ();
		EditorGUILayout.Space();
        
		AppUsesEncryption ();
		EditorGUILayout.Space();

        EnableBitcode ();
        EditorGUILayout.Space();

		AboutGUI();

		if(GUI.changed) {
			DirtyEditor();
		}

	}


	private string _newFramework = string.Empty;
	private void Frameworks()
	{
        ISDSettings.Instance.IsFrameworksSettingOpen = EditorGUILayout.Foldout(ISDSettings.Instance.IsFrameworksSettingOpen, "Frameworks");

		if(ISDSettings.Instance.IsFrameworksSettingOpen) {
			if (ISDSettings.Instance.frameworks.Count == 0) {
				EditorGUILayout.HelpBox("No Frameworks added", MessageType.None);
			}

			foreach(string framework in ISDSettings.Instance.frameworks) {
				EditorGUILayout.BeginVertical (GUI.skin.box);

				EditorGUILayout.BeginHorizontal();
				EditorGUILayout.SelectableLabel(framework, GUILayout.Height(18));
				EditorGUILayout.Space();

				bool pressed  = GUILayout.Button("x",  EditorStyles.miniButton, GUILayout.Width(20));
				if(pressed) {
					ISDSettings.Instance.frameworks.Remove(framework);
					return;
				}

				EditorGUILayout.EndHorizontal();


				EditorGUILayout.EndVertical ();
			}
		
			EditorGUILayout.Space();
			EditorGUILayout.BeginHorizontal();

			EditorGUILayout.LabelField("Add New Framework");
            _newFramework = EditorGUILayout.TextField(_newFramework, GUILayout.Width(200));
			EditorGUILayout.EndHorizontal();

			EditorGUILayout.BeginHorizontal();
			
			EditorGUILayout.Space();
			
			if(GUILayout.Button("Add",  GUILayout.Width(100))) {
                if (!ISDSettings.Instance.frameworks.Contains(_newFramework) && _newFramework.Length > 0) {
                    ISDSettings.Instance.frameworks.Add(_newFramework);
                    _newFramework = string.Empty;
				}
				
			}
			EditorGUILayout.EndHorizontal();
		}
	}

	private string NewLinkerFlag = string.Empty;
	private void LinkerFlags()
	{
		ISDSettings.Instance.IsLinkerSettingOpen = EditorGUILayout.Foldout(ISDSettings.Instance.IsLinkerSettingOpen, "Linker Flags");
		
		if(ISDSettings.Instance.IsLinkerSettingOpen) {
			if (ISDSettings.Instance.frameworks.Count == 0) {
				EditorGUILayout.HelpBox("No Linker Flags added", MessageType.None);
			}
			

			foreach(string flasg in ISDSettings.Instance.linkFlags) {
				EditorGUILayout.BeginVertical (GUI.skin.box);
				
				EditorGUILayout.BeginHorizontal();
				EditorGUILayout.SelectableLabel(flasg, GUILayout.Height(18));
				EditorGUILayout.Space();
				
				bool pressed  = GUILayout.Button("x",  EditorStyles.miniButton, GUILayout.Width(20));
				if(pressed) {
					ISDSettings.Instance.linkFlags.Remove(flasg);
					return;
				}
				
				EditorGUILayout.EndHorizontal();
				
				EditorGUILayout.EndVertical ();
			}

			EditorGUILayout.Space();
			EditorGUILayout.BeginHorizontal();
			
			EditorGUILayout.LabelField("Add New Flag");
			NewLinkerFlag = EditorGUILayout.TextField(NewLinkerFlag, GUILayout.Width(200));
			EditorGUILayout.EndHorizontal();

			EditorGUILayout.BeginHorizontal();
			
			EditorGUILayout.Space();
			
			if(GUILayout.Button("Add",  GUILayout.Width(100))) {
				if(!ISDSettings.Instance.linkFlags.Contains(NewLinkerFlag) && NewLinkerFlag.Length > 0) {
					ISDSettings.Instance.linkFlags.Add(NewLinkerFlag);
					NewLinkerFlag = string.Empty;
				}
				
			}
			EditorGUILayout.EndHorizontal();
		}
	}


	private string NewCompilerFlag = string.Empty;
	private void CompilerFlags()
	{
		ISDSettings.Instance.IsCompilerSettingsOpen = EditorGUILayout.Foldout(ISDSettings.Instance.IsCompilerSettingsOpen, "Compiler Flags");
		
		if(ISDSettings.Instance.IsCompilerSettingsOpen) {
			if (ISDSettings.Instance.frameworks.Count == 0) {
				EditorGUILayout.HelpBox("No Linker Flags added", MessageType.None);
			}

			foreach(string flasg in ISDSettings.Instance.compileFlags) {
				EditorGUILayout.BeginVertical (GUI.skin.box);
				
				EditorGUILayout.BeginHorizontal();
				EditorGUILayout.SelectableLabel(flasg, GUILayout.Height(18));
				
				EditorGUILayout.Space();
				
				bool pressed  = GUILayout.Button("x",  EditorStyles.miniButton, GUILayout.Width(20));
				if(pressed) {
					ISDSettings.Instance.compileFlags.Remove(flasg);
					return;
				}
				
				EditorGUILayout.EndHorizontal();
				EditorGUILayout.EndVertical ();
			}

			EditorGUILayout.Space();
			EditorGUILayout.BeginHorizontal();
			
			EditorGUILayout.LabelField("Add New Flag");
			NewCompilerFlag = EditorGUILayout.TextField(NewCompilerFlag, GUILayout.Width(200));
			EditorGUILayout.EndHorizontal();

			EditorGUILayout.BeginHorizontal();
			
			EditorGUILayout.Space();
			
			if(GUILayout.Button("Add",  GUILayout.Width(100))) {
				if(!ISDSettings.Instance.compileFlags.Contains(NewCompilerFlag) && NewCompilerFlag.Length > 0) {
					ISDSettings.Instance.compileFlags.Add(NewCompilerFlag);
					NewCompilerFlag = string.Empty;
				}
				
			}
			EditorGUILayout.EndHorizontal();
		}
	}


	private string NewUrlScheme = string.Empty;
	private void UrlScheme()
	{
		ISDSettings.Instance.IsSchemeSettingOpen = EditorGUILayout.Foldout(ISDSettings.Instance.IsSchemeSettingOpen, "URL Scheme");

		if(ISDSettings.Instance.IsSchemeSettingOpen) {
			if (ISDSettings.Instance.urlSchemes.Count == 0) {
				EditorGUILayout.HelpBox("No URL Scheme added", MessageType.None);
			}

			foreach(string flasg in ISDSettings.Instance.urlSchemes) {
				EditorGUILayout.BeginVertical (GUI.skin.box);

				EditorGUILayout.BeginHorizontal();
				EditorGUILayout.SelectableLabel(flasg, GUILayout.Height(18));
				EditorGUILayout.Space();

				bool pressed  = GUILayout.Button("x",  EditorStyles.miniButton, GUILayout.Width(20));
				if(pressed) {
					ISDSettings.Instance.urlSchemes.Remove(flasg);
					return;
				}

				EditorGUILayout.EndHorizontal();

				EditorGUILayout.EndVertical ();
			}

			EditorGUILayout.Space();
			EditorGUILayout.BeginHorizontal();

			EditorGUILayout.LabelField("Add New URL Type");
			NewUrlScheme = EditorGUILayout.TextField(NewUrlScheme, GUILayout.Width(200));
			EditorGUILayout.EndHorizontal();

			EditorGUILayout.BeginHorizontal();

			EditorGUILayout.Space();

			if(GUILayout.Button("Add",  GUILayout.Width(100))) {
				if(!ISDSettings.Instance.urlSchemes.Contains(NewUrlScheme) && NewUrlScheme.Length > 0) {
					ISDSettings.Instance.urlSchemes.Add(NewUrlScheme);
					NewUrlScheme = string.Empty;
				}

			}
			EditorGUILayout.EndHorizontal();
		}
	}


	private string NewRegionSettings = string.Empty;
	private void RegionSettings()
	{
		ISDSettings.Instance.IsRegionSettingOpen = EditorGUILayout.Foldout(ISDSettings.Instance.IsRegionSettingOpen, "Region Settings");

		if(ISDSettings.Instance.IsRegionSettingOpen) {
			if (ISDSettings.Instance.regionSettings.Count == 0) {
				EditorGUILayout.HelpBox("No Region added", MessageType.None);
			}

			foreach(string flasg in ISDSettings.Instance.regionSettings) {
				EditorGUILayout.BeginVertical (GUI.skin.box);

				EditorGUILayout.BeginHorizontal();
				EditorGUILayout.SelectableLabel(flasg, GUILayout.Height(18));
				EditorGUILayout.Space();

				bool pressed  = GUILayout.Button("x",  EditorStyles.miniButton, GUILayout.Width(20));
				if(pressed) {
					ISDSettings.Instance.regionSettings.Remove(flasg);
					return;
				}

				EditorGUILayout.EndHorizontal();

				EditorGUILayout.EndVertical ();
			}

			EditorGUILayout.Space();
			EditorGUILayout.BeginHorizontal();

			EditorGUILayout.LabelField("Add New Region");
			NewRegionSettings = EditorGUILayout.TextField(NewRegionSettings, GUILayout.Width(200));
			EditorGUILayout.EndHorizontal();

			EditorGUILayout.BeginHorizontal();

			EditorGUILayout.Space();

			if(GUILayout.Button("Add",  GUILayout.Width(100))) {
				if(!ISDSettings.Instance.regionSettings.Contains(NewRegionSettings) && NewRegionSettings.Length > 0) {
					ISDSettings.Instance.regionSettings.Add(NewRegionSettings);
					NewRegionSettings = string.Empty;
				}

			}
			EditorGUILayout.EndHorizontal();
		}
	}


//	private string NewAppTransportSecurity = string.Empty;
//	private void AppTransportSecurity()
//	{
//		ISDSettings.Instance.IsAppTransportSecurityOpen = EditorGUILayout.Foldout(ISDSettings.Instance.IsAppTransportSecurityOpen, "App Transport Security");
//
//		if(ISDSettings.Instance.IsAppTransportSecurityOpen) {
//			if (ISDSettings.Instance.appTransportSecurity.Count == 0) {
//				EditorGUILayout.HelpBox("No App Domain added", MessageType.None);
//			}
//
//			foreach(string flasg in ISDSettings.Instance.appTransportSecurity) {
//				EditorGUILayout.BeginVertical (GUI.skin.box);
//
//				EditorGUILayout.BeginHorizontal();
//				EditorGUILayout.SelectableLabel(flasg, GUILayout.Height(18));
//				EditorGUILayout.Space();
//
//				bool pressed  = GUILayout.Button("x",  EditorStyles.miniButton, GUILayout.Width(20));
//				if(pressed) {
//					ISDSettings.Instance.appTransportSecurity.Remove(flasg);
//					return;
//				}
//
//				EditorGUILayout.EndHorizontal();
//
//				EditorGUILayout.EndVertical ();
//			}
//
//			EditorGUILayout.Space();
//			EditorGUILayout.BeginHorizontal();
//
//			EditorGUILayout.LabelField("Add New Domain");
//			NewAppTransportSecurity = EditorGUILayout.TextField(NewAppTransportSecurity, GUILayout.Width(200));
//			EditorGUILayout.EndHorizontal();
//
//			EditorGUILayout.BeginHorizontal();
//
//			EditorGUILayout.Space();
//
//			if(GUILayout.Button("Add",  GUILayout.Width(100))) {
//				if(!ISDSettings.Instance.appTransportSecurity.Contains(NewAppTransportSecurity) && NewAppTransportSecurity.Length > 0) {
//					ISDSettings.Instance.appTransportSecurity.Add(NewAppTransportSecurity);
//					NewAppTransportSecurity = string.Empty;
//				}
//
//			}
//			EditorGUILayout.EndHorizontal();
//		}
//	}


	private string NewAppUsesEncryption = string.Empty;
	private void AppUsesEncryption()
	{
		ISDSettings.Instance.IsAppUsesEncryptionOpen = EditorGUILayout.Foldout(ISDSettings.Instance.IsAppUsesEncryptionOpen, "App Uses Encryption");

		if(ISDSettings.Instance.IsAppUsesEncryptionOpen) {
			if (ISDSettings.Instance.appUsesEncryption.Count == 0) {
				EditorGUILayout.HelpBox("No AppUsesEncryption added", MessageType.None);
			}

			foreach(string flasg in ISDSettings.Instance.appUsesEncryption) {
				EditorGUILayout.BeginVertical (GUI.skin.box);

				EditorGUILayout.BeginHorizontal();
				EditorGUILayout.SelectableLabel(flasg, GUILayout.Height(18));
				EditorGUILayout.Space();

				bool pressed  = GUILayout.Button("x",  EditorStyles.miniButton, GUILayout.Width(20));
				if(pressed) {
					ISDSettings.Instance.appUsesEncryption.Remove(flasg);
					return;
				}

				EditorGUILayout.EndHorizontal();

				EditorGUILayout.EndVertical ();
			}

			EditorGUILayout.Space();
			EditorGUILayout.BeginHorizontal();

			EditorGUILayout.LabelField("Add New AppUsesEncryption");
			NewAppUsesEncryption = EditorGUILayout.TextField(NewAppUsesEncryption, GUILayout.Width(200));
			EditorGUILayout.EndHorizontal();

			EditorGUILayout.BeginHorizontal();

			EditorGUILayout.Space();

			if(GUILayout.Button("Add",  GUILayout.Width(100))) {
				if(!ISDSettings.Instance.appUsesEncryption.Contains(NewAppUsesEncryption) && NewAppUsesEncryption.Length > 0) {
					ISDSettings.Instance.appUsesEncryption.Add(NewAppUsesEncryption);
					NewAppUsesEncryption = string.Empty;
				}
			}
			EditorGUILayout.EndHorizontal();
		}
	}

    private string NewEnableBitcode = string.Empty;
    private void EnableBitcode()
    {
        ISDSettings.Instance.IsEnableBitcodeOpen = EditorGUILayout.Foldout(ISDSettings.Instance.IsEnableBitcodeOpen, "Enable Bitcode : YES/NO");

        if(ISDSettings.Instance.IsEnableBitcodeOpen) {
            if (ISDSettings.Instance.enableBitcode == string.Empty) {
                EditorGUILayout.HelpBox("No EnableBitcode added", MessageType.None);
            }

            // One Entry
            if (ISDSettings.Instance.enableBitcode == string.Empty) {
                EditorGUILayout.Space ();
                EditorGUILayout.BeginHorizontal ();

                EditorGUILayout.LabelField ("Add New EnableBitcode");
                NewEnableBitcode = EditorGUILayout.TextField (NewEnableBitcode, GUILayout.Width (200));
                EditorGUILayout.EndHorizontal ();

                EditorGUILayout.BeginHorizontal ();

                EditorGUILayout.Space ();

                if (GUILayout.Button ("Add", GUILayout.Width (100))) {
                    if (!ISDSettings.Instance.enableBitcode.Contains (NewEnableBitcode) && NewEnableBitcode.Length > 0) {
                        ISDSettings.Instance.enableBitcode = NewEnableBitcode;
                        NewEnableBitcode = string.Empty;
                    }

                }
                EditorGUILayout.EndHorizontal ();
            } else {
                EditorGUILayout.BeginVertical (GUI.skin.box);

                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.SelectableLabel(ISDSettings.Instance.enableBitcode, GUILayout.Height(18));
                EditorGUILayout.Space();
                bool pressed  = GUILayout.Button("x",  EditorStyles.miniButton, GUILayout.Width(20));
                if(pressed) {
                    ISDSettings.Instance.enableBitcode = string.Empty;
                    return;
                }
                EditorGUILayout.EndHorizontal();
                EditorGUILayout.EndVertical ();
            }
        }
    }

	private void AboutGUI() {
		EditorGUILayout.HelpBox("About the Plugin", MessageType.None);
		EditorGUILayout.Space();
		
		SelectableLabelField(SdkVersion, ISDSettings.VERSION_NUMBER);
		SelectableLabelField(SupportEmail, "stans.assets@gmail.com");
	}
	
	private void SelectableLabelField(GUIContent label, string value) {
		EditorGUILayout.BeginHorizontal();
		EditorGUILayout.LabelField(label, GUILayout.Width(180), GUILayout.Height(16));
		EditorGUILayout.SelectableLabel(value, GUILayout.Height(16));
		EditorGUILayout.EndHorizontal();
	}

	private static void DirtyEditor() {
			#if UNITY_EDITOR
		EditorUtility.SetDirty(ISDSettings.Instance);
			#endif
	}
}
