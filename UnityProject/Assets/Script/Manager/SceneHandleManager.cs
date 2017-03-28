using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

/// <summary>
/// Scene manager.
/// </summary>
public class SceneHandleManager : SingletonMonoBehaviour <SceneHandleManager>
{
    private static int _sceneChangeCount;

    /// <summary>
    /// Nexts the scene redirect.
    /// </summary>
    /// <param name="sceneName">Scene name.</param>
	public static void NextSceneRedirect ( string sceneName ) 
    {
        _sceneChangeCount++;

//        if (_sceneChangeCount % 5 == 0)
//        {
            Resources.UnloadUnusedAssets();
            System.GC.Collect ();
            _sceneChangeCount = 0;
//        }

        Helper.LocalFileHandler.Init ( LocalFileConstants.GetLocalFileDir() + LocalFileConstants.COMMON_LOCAL_FILE_NAME);
        if (SceneManager.GetActiveScene().name == CommonConstants.MESSAGE_SCENE || SceneManager.GetActiveScene().name == CommonConstants.PURCHASE_SCENE) {
            Helper.LocalFileHandler.SetString (LocalFileConstants.FROM_MYPAGE_SCENE, "");
            Helper.LocalFileHandler.Flush ();
        }
        
        //SceneManager.UnloadScene (SceneManager.GetActiveScene().buildIndex);
        SceneManager.LoadScene (sceneName, LoadSceneMode.Single);
    }
}
