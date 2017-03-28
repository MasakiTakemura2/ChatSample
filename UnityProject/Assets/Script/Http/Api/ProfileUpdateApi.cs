using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Http {
    public class ProfileUpdateApi
    {
        #region member variable
        public static bool _success = false;
        public static UserDataEntity.Result _httpCatchData;
        #endregion

        #region Construct
        public ProfileUpdateApi (UserDataEntity.Basic userBasicData) 
        {
            //Ready Proccesing
            _success = false;

            //post parameter Set
            var postDatas = new Dictionary<string, string>();
            postDatas.Add (HttpConstants.USER_KEY, userBasicData.user_key);
            postDatas.Add (HttpConstants.NAME    , userBasicData.name);
            postDatas.Add (HttpConstants.SEX_CD, userBasicData.sex_cd);
            postDatas.Add (HttpConstants.BLOOD_TYPE   , userBasicData.blood_type);
            postDatas.Add (HttpConstants.PREF         , userBasicData.pref);
            postDatas.Add (HttpConstants.CITY_ID      , userBasicData.city_id);
            postDatas.Add (HttpConstants.HEIGHT      , userBasicData.height);
            postDatas.Add (HttpConstants.WEIGHT      , userBasicData.weight);
            postDatas.Add (HttpConstants.BIRTH_DATE   , userBasicData.birth_date);
            postDatas.Add (HttpConstants.PROFILE      , userBasicData.profile);
            if (userBasicData.body_type != null && userBasicData.body_type.Count > 0) 
                postDatas.Add (HttpConstants.BODY_TYPES   , userBasicData.body_type[0]);
                
            if (userBasicData.hair_style != null && userBasicData.hair_style.Count > 0) 
                postDatas.Add (HttpConstants.HAUR_STYLES   , userBasicData.hair_style[0]);
                
            if ( userBasicData.glasses != null && userBasicData.glasses.Count > 0) 
                postDatas.Add (HttpConstants.GLASSESS      , userBasicData.glasses[0]);
                
            if ( userBasicData.holiday != null && userBasicData.holiday.Count > 0) 
                postDatas.Add (HttpConstants.HOLIDAYS      , userBasicData.holiday[0]);

            if ( userBasicData.annual_income != null && userBasicData.annual_income.Count > 0)
                postDatas.Add (HttpConstants.ANNUAL_INCOMES, userBasicData.annual_income[0]);
            
            if ( userBasicData.education != null && userBasicData.education.Count > 0)
                postDatas.Add (HttpConstants.EDUCATIONS    , userBasicData.education[0]);
                
            if ( userBasicData.housemate != null && userBasicData.housemate.Count > 0) 
                postDatas.Add (HttpConstants.HOUSEMATES    , userBasicData.housemate[0]);
                
            if ( userBasicData.sibling != null && userBasicData.sibling.Count > 0) 
                postDatas.Add (HttpConstants.SIBLINGS      , userBasicData.sibling[0]);
                
            if ( userBasicData.alcohol != null && userBasicData.alcohol.Count > 0)
                postDatas.Add (HttpConstants.ALCOHOLS      , userBasicData.alcohol[0]);
            
            if ( userBasicData.tobacco != null &&  userBasicData.tobacco.Count > 0) 
                postDatas.Add (HttpConstants.TOBACCOS      , userBasicData.tobacco[0]);
                
            if ( userBasicData.car != null && userBasicData.car.Count > 0) 
                postDatas.Add (HttpConstants.CARS          , userBasicData.car[0]);
            
            if ( userBasicData.pet != null && userBasicData.pet.Count > 0)
                postDatas.Add (HttpConstants.PETS          , userBasicData.pet[0]);
                
            if ( userBasicData.hobby != null && userBasicData.hobby.Count > 0)
               postDatas.Add (HttpConstants.HOBBYS        , userBasicData.hobby[0]);
               
            if ( userBasicData.interest != null && userBasicData.interest.Count > 0)
                postDatas.Add (HttpConstants.INTERESTS     , userBasicData.interest[0]);
                
            if ( userBasicData.marital != null && userBasicData.marital.Count > 0)
                postDatas.Add (HttpConstants.MARTIALS     , userBasicData.marital[0]);
            
            //TODO: foreachが入るはず。
            if ( userBasicData.type != null && userBasicData.type.Count > 0) 
            {
               //TODO: 複数選択可
               postDatas.Add (HttpConstants.TYPES         , userBasicData.type[0]);
            }
                
                
            if ( userBasicData.personality != null && userBasicData.personality.Count > 0) {
                //TODO: 複数選択可
                postDatas.Add (HttpConstants.PERSONALITYS  , userBasicData.personality[0]);
            }
                
                
            postDatas.Add (HttpConstants.API_VERSION_NAME, DeviceService.GetAppVersion());

            Request (postDatas);
        }
        #endregion

        #region Request Send Prossesing
        /// <summary>
        /// Request this instance.
        /// </summary>
        private void Request (Dictionary<string, string> postDatas)
        {
            string url = DomainData.GetApiUrl(DomainData.UPDATE_PROFILE);
            Debug.Log (url);
            HttpHandler.Send<UserDataEntity.Result> (url, postDatas, CallBack);
        }

        /// <summary>
        /// Calls the back.
        /// </summary>
        /// <param name="result">Result.</param>
        private void CallBack (UserDataEntity.Result result) 
        {
            _success = (result != null);
            //こっちのデータも更新しておく。
            GetUserApi._httpCatchData = result; 
            if (_success == true) {
                _httpCatchData = result;
            }
        }
        #endregion
    }
}