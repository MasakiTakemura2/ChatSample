using UnityEngine;
using UnityEditor;
using UnityEditor.Callbacks;
using System.IO;
using System.Collections;
using System.Diagnostics;
using UnityEditor.iOS.Xcode;

namespace UnityEditor.IOSDeployEditor
{
	public class IOSDeployPostProcess  {

		#if UNITY_IPHONE
		[PostProcessBuild(100)]
		public static void OnPostprocessBuild(BuildTarget target, string pathToBuiltProject) {

			#if UNITY_IPHONE &&  UNITY_EDITOR_WIN
			UnityEngine.Debug.LogWarning("ISD Postprocess is not available for Windows");
			#endif
			
			
			#if UNITY_IPHONE && UNITY_EDITOR_OSX
			
			Process myCustomProcess = new Process();		
			myCustomProcess.StartInfo.FileName = "python";
			
			string frameworks 		= string.Join(" ", ISDSettings.Instance.frameworks.ToArray());
			string linkFlags 		= string.Join(" ", ISDSettings.Instance.linkFlags.ToArray());
			string compileFlags 	= string.Join(" ", ISDSettings.Instance.compileFlags.ToArray());
			string copyFWPath = Application.dataPath + "/Plugins/IOS/";
			
			UnityEngine.Debug.Log("Post process Xcode Project Path : " + pathToBuiltProject);
			UnityEngine.Debug.Log("Post process Copy Framework : " + copyFWPath);


			UnityEngine.Debug.Log ("Post process Info.plist");
			string plistFilePath = Path.Combine(pathToBuiltProject, "Info.plist");
			PListParser sParser = new PListParser(plistFilePath);

			UnityEngine.Debug.Log ("Set UrlScheme");
			sParser.UpdateSchemesSettings(ISDSettings.Instance.urlSchemes.ToArray());

			UnityEngine.Debug.Log ("Set RegionSettings");
			sParser.UpdateRegionSettings(ISDSettings.Instance.regionSettings.ToArray());

//			UnityEngine.Debug.Log ("Set AppTransportSecurity");
//			sParser.UpdateDomainSecuritySettings(ISDSettings.Instance.appTransportSecurity.ToArray());

			UnityEngine.Debug.Log ("Set ITSAppUsesNonExemptEncryption");
			sParser.UpdateAppUsesEncryptionSettings(ISDSettings.Instance.appUsesEncryption.ToArray());

UnityEngine.Debug.Log (ISDSettings.Instance.enableBitcode + " ISDSettings.Instance.enableBitcode ") ;

            if (ISDSettings.Instance.enableBitcode != null) 
            {
                UnityEngine.Debug.Log ("Set Enable Bitcode");
            
                PBXProject bpxProject = new PBXProject ();
                string projectPath = pathToBuiltProject + "/Unity-iPhone.xcodeproj/project.pbxproj";
    
                bpxProject.ReadFromString (File.ReadAllText (projectPath));
                string targetGuidByName = bpxProject.TargetGuidByName ("Unity-iPhone");
                bpxProject.SetBuildProperty(targetGuidByName, "ENABLE_BITCODE", ISDSettings.Instance.enableBitcode);
                File.WriteAllText(projectPath, bpxProject.WriteToString());

    			UnityEngine.Debug.Log ("Post process Wirite Info.plist");
    			sParser.WriteToFile();
            }

			UnityEngine.Debug.Log ("ISD Executing post process Start");
			//myCustomProcess.StartInfo.Arguments = string.Format("Assets/Extensions/IOSDeploy/Scripts/Editor/post_process.py \"{0}\" \"{1}\" \"{2}\" \"{3}\"", new object[] { pathToBuiltProject, frameworks, compileFlags, linkFlags });
			myCustomProcess.StartInfo.Arguments = string.Format("Assets/Extensions/IOSDeploy/Scripts/Editor/post_process.py \"{0}\" \"{1}\" \"{2}\" \"{3}\" \"{4}\"", new object[] { pathToBuiltProject, frameworks, compileFlags, linkFlags, copyFWPath });
			UnityEngine.Debug.Log("ISD Executing : python " + myCustomProcess.StartInfo.Arguments);
			myCustomProcess.StartInfo.UseShellExecute = false;
			myCustomProcess.StartInfo.RedirectStandardOutput = true;
			myCustomProcess.Start(); 
			myCustomProcess.WaitForExit();
			
			UnityEngine.Debug.Log("ISD Executing post process Done.");
			
			#endif

		}
		#endif
	}
}
