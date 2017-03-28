using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Http;
using EventManager;

namespace ModelManager
{
    public class CommonModelHandle
    {
        /// <summary>
        /// Gets the preference data.
        /// 都道府県取得
        /// </summary>
        /// <returns>The preference data.</returns>
        public static Dictionary<int, InitDataEntity.PrefsData> GetPrefData () 
        {
            Dictionary<int, InitDataEntity.PrefsData> strList = new Dictionary<int, InitDataEntity.PrefsData> ();
            if (InitDataApi._httpCatchData != null)
            {
                int c = 0;
                foreach (var item in InitDataApi._httpCatchData.result.pref)
                {
                    strList.Add (c, new InitDataEntity.PrefsData(){
                        id   = item.id, 
                        name = item.name, 
                        });
                    c++;
                }
            }
            return strList;
         }

         /// <summary>
         /// Gets the preference data by preg identifier.
         /// Pref都道府県データ単体で取得
         /// </summary>
         /// <returns>The preference data by preg identifier.</returns>
         /// <param name="id">Identifier.</param>
         public static InitDataEntity.PrefsData GetPrefDataById (string id)
         {
              InitDataEntity.PrefsData prefData = new InitDataEntity.PrefsData();
              foreach (var item in InitDataApi._httpCatchData.result.pref) {
                  if (item.id == id)
                  {
                      prefData = item;
                      break;
                  }
              }
              return prefData;
         }

         /// <summary>
         /// Gets the city data.
         /// 都道府県データから市区町村データを取得。
         /// </summary>
         /// <returns>The city data.</returns>
         /// <param name="prefId">Preference identifier.</param>
         public static Dictionary<int, InitDataEntity.CityData> GetCityData (string prefId)
         {
            Dictionary<int, InitDataEntity.CityData> strList = new Dictionary<int, InitDataEntity.CityData> ();
            if (prefId != null)
            {
                int c = 0;
                foreach (var item in InitDataApi._httpCatchData.result.city)
                {
                    if (item.pref == prefId) {
                        strList.Add (c, new InitDataEntity.CityData(){
                            id   = item.id,
                            pref = item.pref,
                            name = item.name,
                        });
                        c++;
                    }
                }         
            } else {
                Debug.Log ("都道府県選択してないです。warning pop up ?");
            }
            return strList;
         }

        /// <summary>
        /// Gets the preference data by preference identifier.
        /// 市区町村データを単体で取得。
        /// </summary>
        /// <returns>The preference data by preference identifier.</returns>
        /// <param name="id">Identifier.</param>
        public static InitDataEntity.CityData GetCityDataById (string id)
        {
            InitDataEntity.CityData cityData = new InitDataEntity.CityData();
            foreach (var item in InitDataApi._httpCatchData.result.city) {
                if (item.id == id)
                {
                    cityData = item;
                    break;
                }
            }

            return cityData;
        }

		/// <summary>
		/// Gets the name of the city data by.
		/// </summary>
		/// <returns>The city data by name.</returns>
		/// <param name="name">Name.</param>
		public static InitDataEntity.CityData GetCityDataByName (string name)
		{
			InitDataEntity.CityData cityData = new InitDataEntity.CityData();
			foreach (var item in InitDataApi._httpCatchData.result.city) {
				if (item.name == name)
				{
					cityData = item;
					break;
				}
			}

			return cityData;
		}

        /// <summary>
        /// Gets the by name base data.
        /// </summary>
        /// <returns>The by name base data.</returns>
        /// <param name="name">Name.</param>
        /// <param name="type">Type.</param>
        public static InitDataEntity.FixedPropertyIdBase GetByNameBaseData (string name, CurrentProfSettingStateType type)
        {
            List<InitDataEntity.FixedPropertyIdBase> tmpSets = new List<InitDataEntity.FixedPropertyIdBase> ();
            InitDataEntity.FixedPropertyIdBase data = new InitDataEntity.FixedPropertyIdBase ();

            if (type == CurrentProfSettingStateType.BloodType) {
                tmpSets = InitDataApi._httpCatchData.result.blood_type;
            } else if (type == CurrentProfSettingStateType.Radius) {
                tmpSets = InitDataApi._httpCatchData.result.radius;
            } else if (type == CurrentProfSettingStateType.Gender) {
                tmpSets = InitDataApi._httpCatchData.result.sex_cd;
            } else if (type == CurrentProfSettingStateType.BoardCategory) {
                tmpSets = InitDataApi._httpCatchData.result.board_category;
            } else if (type == CurrentProfSettingStateType.UserSort) {
                tmpSets = InitDataApi._httpCatchData.result.user_sort;
            }

            foreach (var item in tmpSets)
            {
                if (item.name == name) {
                    data = item;
                    break;
                }
            }
            return data;
        }


        /// <summary>
        /// Gets the by identifier base data.
        /// </summary>
        /// <returns>The by identifier base data.</returns>
        /// <param name="id">Identifier.</param>
        /// <param name="type">Type.</param>
        public static InitDataEntity.FixedPropertyIdBase GetByIdBaseData (string id, CurrentProfSettingStateType type)
        {
            List<InitDataEntity.FixedPropertyIdBase> tmpSets = new List<InitDataEntity.FixedPropertyIdBase> ();
            InitDataEntity.FixedPropertyIdBase data = new InitDataEntity.FixedPropertyIdBase ();

            if (type == CurrentProfSettingStateType.BloodType) {
                tmpSets = InitDataApi._httpCatchData.result.blood_type;
            } else if (type == CurrentProfSettingStateType.Radius) {
                tmpSets = InitDataApi._httpCatchData.result.radius;
            } else if (type == CurrentProfSettingStateType.Gender) {
                tmpSets = InitDataApi._httpCatchData.result.sex_cd;
            }  else if (type == CurrentProfSettingStateType.BoardCategory) {
                tmpSets = InitDataApi._httpCatchData.result.board_category;
            } else if (type == CurrentProfSettingStateType.UserSort){
                tmpSets = InitDataApi._httpCatchData.result.user_sort;
            }

            foreach (var item in tmpSets)
            {
                if (item.id == id) {
                    data = item;
                    break;
                }
            }
            return data;
        }

        /// <summary>
        /// Gets the name master.
        /// 男女別にリスト取得するデータ群の取得用。
        /// </summary>
        /// <returns>The name master.</returns>
        /// <param name="gender">Gender.</param>
        /// <param name="type">Type.</param>
        public static List<InitDataEntity.NameOrSexCd> GetNameMaster (string gender, CurrentProfSettingStateType type)
        {
            List<InitDataEntity.NameOrSexCd> tmpList = new List<InitDataEntity.NameOrSexCd> ();
            List<InitDataEntity.NameOrSexCd> returnData = new List<InitDataEntity.NameOrSexCd> ();

            switch (type) {
            case CurrentProfSettingStateType.BodyType:
                tmpList = InitDataApi._httpCatchData.result.body_type.list;
                break;
            case CurrentProfSettingStateType.HairStyle:
                tmpList = InitDataApi._httpCatchData.result.hair_style.list;
               break;
            case CurrentProfSettingStateType.Glasses:
                tmpList = InitDataApi._httpCatchData.result.glasses.list;
                break;
            case CurrentProfSettingStateType.Type:
                tmpList = InitDataApi._httpCatchData.result.type.list;
                break;
            case CurrentProfSettingStateType.Personality:
                tmpList = InitDataApi._httpCatchData.result.personality.list;
                break;
            case CurrentProfSettingStateType.Holiday:
                tmpList = InitDataApi._httpCatchData.result.holiday.list;
                break;
            case CurrentProfSettingStateType.AnnualIncome:
                tmpList = InitDataApi._httpCatchData.result.annual_income.list;
                break;
            case CurrentProfSettingStateType.Education:
                tmpList = InitDataApi._httpCatchData.result.education.list;
                break;
            case CurrentProfSettingStateType.Housemate:
                tmpList = InitDataApi._httpCatchData.result.housemate.list;
                break;
            case CurrentProfSettingStateType.Sibling:
                tmpList = InitDataApi._httpCatchData.result.sibling.list;
                break;
            case CurrentProfSettingStateType.Alcohol:
                tmpList = InitDataApi._httpCatchData.result.alcohol.list;
                break;
            case CurrentProfSettingStateType.Tobacco:
                tmpList = InitDataApi._httpCatchData.result.tobacco.list;
                break;
            case CurrentProfSettingStateType.Car:
                tmpList = InitDataApi._httpCatchData.result.car.list;
                break;
            case CurrentProfSettingStateType.Pet:
                tmpList = InitDataApi._httpCatchData.result.pet.list;
                break;
            case CurrentProfSettingStateType.Hobby:
                tmpList = InitDataApi._httpCatchData.result.hobby.list;
                break;
            case CurrentProfSettingStateType.Interest:
                tmpList = InitDataApi._httpCatchData.result.interest.list;
                break;
            case CurrentProfSettingStateType.Marital:
                tmpList = InitDataApi._httpCatchData.result.marital.list;
                break;
            }

            foreach (var item in tmpList) {
                if (item.sex_cd == gender) {
                    returnData.Add (item);
                }
            }
            return returnData;
        }
        
        /// <summary>
        /// Gets the name master.
        /// 男女別じゃない、ID付きのマスターデータをリストで取得する用。
        /// </summary>
        /// <returns>The name master.</returns>
        /// <param name="type">Type.</param>
        public static List<InitDataEntity.FixedPropertyIdBase> GetNameMaster (CurrentProfSettingStateType type)
        {
            List<InitDataEntity.FixedPropertyIdBase> tmpList = new List<InitDataEntity.FixedPropertyIdBase> ();

            if (type == CurrentProfSettingStateType.BloodType) {
                tmpList = InitDataApi._httpCatchData.result.blood_type;
            } else if (type == CurrentProfSettingStateType.Radius) {
                tmpList = InitDataApi._httpCatchData.result.radius;
            } else if (type == CurrentProfSettingStateType.Gender) {
                tmpList = InitDataApi._httpCatchData.result.sex_cd;
            }  else if (type == CurrentProfSettingStateType.BoardCategory) {
                tmpList = InitDataApi._httpCatchData.result.board_category;
            } else if (type == CurrentProfSettingStateType.UserSort){
                tmpList = InitDataApi._httpCatchData.result.user_sort;
            }

            return tmpList;
        }
     }
}