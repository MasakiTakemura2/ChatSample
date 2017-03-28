using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using Http;

/// <summary>
/// Tinder image.
/// </summary>
public class TinderImage : MonoBehaviour
{
    [SerializeField]
    private RawImage _rawImage;

    [SerializeField]
    private Text _name;
    
    [SerializeField]
    private Text _target;

    [SerializeField]
    private Image _nope;

    [SerializeField]
    private Image _like;
    
    [SerializeField]
    private Image _superLike;
    
    /// <summary>
    /// Init the specified user.
    /// </summary>
    /// <param name="user">User.</param>
    public void Init (UserDataEntity.Basic user)
    {
        if (_rawImage != null && string.IsNullOrEmpty(user.profile_image_url) == false ) {
             StartCoroutine(WwwToRendering( user.profile_image_url, _rawImage));
         }

         string userName   = user.name;
         string age    = user.age + LocalMsgConst.AGE_TEXT;
         string gender = "";
         
         if (user.sex_cd == ((int)GenderType.Male).ToString())
         {
             gender = LocalMsgConst.GENDER_MALE;
         } else {
             gender = LocalMsgConst.GENDER_FEMALE;
         }

         _name.text   = string.Format ("{0}  {1} ({2})",gender, userName,age) ;
         _target.text = ModelManager.CommonModelHandle.GetPrefDataById(user.pref).name;
         this.gameObject.name = user.id;

        //user.name;
    }
    
      /// <summary>
      /// Wwws to rendering.
      /// </summary>
      /// <returns>The to rendering.</returns>
      /// <param name="url">URL.</param>
      /// <param name="targetObj">Target object.</param>
      private IEnumerator WwwToRendering (string url, RawImage targetObj)
      {
          targetObj.texture = null;
          targetObj.gameObject.SetActive (false);
          if (string.IsNullOrEmpty (url) == true) {
              yield break;
          }

          using (WWW www = new WWW (url)) {
              while (www == null)
                  yield return (www != null);

              while (www.isDone == false)
                  yield return (www.isDone);

              //non texture file
              if (string.IsNullOrEmpty (www.error) == false) {
                  Debug.LogError (www.error);
                  Debug.Log (url);
                  yield break;
              }
              while (targetObj == null)
                  yield return (targetObj != null);
              targetObj.gameObject.SetActive (true);
              targetObj.texture = www.texture;
              targetObj.enabled = true;
          }
      }
    
}
