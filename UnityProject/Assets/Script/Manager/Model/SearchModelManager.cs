using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Http;
using EventManager;

namespace ModelManager
{
	public class SearchModelManager : SingletonMonoBehaviour<SearchModelManager>
    {
        /// <summary>
        /// Gets the preference data.
        /// 都道府県取得
        /// </summary>
        /// <returns>The preference data.</returns>
        public Dictionary<int, InitDataEntity.PrefsData> GetPrefData () 
        {
            Dictionary<int, InitDataEntity.PrefsData> strList = new Dictionary<int, InitDataEntity.PrefsData> ();
            if (InitDataApi._httpCatchData != null)
            {
                int c = 0;
                foreach (var item in InitDataApi._httpCatchData.result.pref)
                {
                    strList.Add (c, new InitDataEntity.PrefsData()
						{
                        	id = item.id, 
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
         public InitDataEntity.PrefsData GetPrefDataByPrefId (string id)
         {
			InitDataEntity.PrefsData prefData = new InitDataEntity.PrefsData();
            foreach (var item in InitDataApi._httpCatchData.result.pref) 
			{
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
         public Dictionary<int, InitDataEntity.CityData> GetCityData (string prefId)
         {
            Dictionary<int, InitDataEntity.CityData> strList = new Dictionary<int, InitDataEntity.CityData> ();
            if (prefId != null)
            {
                int c = 0;
                foreach (var item in InitDataApi._httpCatchData.result.city)
                {
                    if (item.pref == prefId)
					{
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
        public InitDataEntity.CityData GetPrefDataByCityId (string id)
        {
            InitDataEntity.CityData cityData = new InitDataEntity.CityData();
            foreach (var item in InitDataApi._httpCatchData.result.city)
			{
                if (item.id == id)
                {
                    cityData = item;
                    break;
                }
            }

            return cityData;
        }



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
			}

			foreach (var item in tmpSets)
			{
				if (item.id == id) {
					data = item;
					break;
				}
			}
			Debug.Log(data.name + " data.name data.name data.name ");
			return data;
		}


		public static InitDataEntity.FixedPropertyIdBase GetByNameBaseData (string name, CurrentProfSettingStateType type)
		{
			List<InitDataEntity.FixedPropertyIdBase> tmpSets = new List<InitDataEntity.FixedPropertyIdBase> ();
			InitDataEntity.FixedPropertyIdBase data = new InitDataEntity.FixedPropertyIdBase ();

			if (type == CurrentProfSettingStateType.BloodType)
			{
				tmpSets = InitDataApi._httpCatchData.result.blood_type;
			} else if (type == CurrentProfSettingStateType.Radius) {
				tmpSets = InitDataApi._httpCatchData.result.radius;
			} else if (type == CurrentProfSettingStateType.Gender) {
				tmpSets = InitDataApi._httpCatchData.result.sex_cd;
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
        /// Gets the by data.
        /// </summary>
        /// <returns>The by data.</returns>
        /// <param name="id">Identifier.</param>
        public InitDataEntity.FixedPropertyIdBase GetByIdBaseData( string id , SearchEventManager.CurrentSettingState type )
        {
            InitDataEntity.FixedPropertyIdBase data = new InitDataEntity.FixedPropertyIdBase ();


			if (type == SearchEventManager.CurrentSettingState.Sex)
			{
				foreach (var item in InitDataApi._httpCatchData.result.sex_cd)
				{
					if (item.id == id)
					{
						data = item;
						break;
					}
				}
			}

			if (type == SearchEventManager.CurrentSettingState.Sort)
			{
				foreach (var item in InitDataApi._httpCatchData.result.user_sort)
				{
					if (item.id == id)
					{
						data = item;
						break;
					}
				}
			}

			if (type == SearchEventManager.CurrentSettingState.Radius)
			{
				foreach (var item in InitDataApi._httpCatchData.result.radius)
				{
					if (item.id == id)
					{
						data = item;
						break;
					}
				}
			}

            return data;
        }


        /// <summary>
        /// Gets the name master.
        /// マスターデータ取得する用。
        /// </summary>
        /// <returns>The name master.</returns>
        /// <param name="name">Name.</param>
        /// <param name="type">Type.</param>
        public InitDataEntity.NameOrSexCd GetNameMasterByName(string name, SearchEventManager.CurrentSettingState type)
        {
            InitDataEntity.NameOrSexCd data = new InitDataEntity.NameOrSexCd ();

            if (type == SearchEventManager.CurrentSettingState.HairStyle)
            {
                foreach (var item in InitDataApi._httpCatchData.result.hair_style.list)
                {
                    if (item.name == name)
					{
                        data = item;
                        break;
                    }
                }
            }

			if (type == SearchEventManager.CurrentSettingState.BodyType)
			{
				foreach (var item in InitDataApi._httpCatchData.result.body_type.list)
				{
					if (item.name == name)
					{
						data = item;
						break;
					}
				}
			}


				
            return data;
        }

		//性別の項目名取得
		/*
		public InitDataEntity.FixedPropertyIdBase GetNameMasterByName(string name, SearchEventManager.CurrentSettingState type)
		{

			InitDataEntity.FixedPropertyIdBase data = new InitDataEntity.FixedPropertyIdBase ();

			if (type == SearchEventManager.CurrentSettingState.Sex)
			{
				foreach (var item in InitDataApi._httpCatchData.result.sex_cd)
				{
					if (item.name == name)
					{
						data = item;
						break;
					}
				}
			}

			return data;
		}
		*/


        /// <summary>
        /// Gets the name master.
        /// 男女別にリスト取得するデータ群の取得用。
        /// </summary>
        /// <returns>The name master.</returns>
        /// <param name="gender">Gender.</param>
        /// <param name="type">Type.</param>
		public List<InitDataEntity.NameOrSexCd> GetNameMaster (string gender, SearchEventManager.CurrentSettingState type)
        {
            List<InitDataEntity.NameOrSexCd> tmpList = new List<InitDataEntity.NameOrSexCd> ();
            List<InitDataEntity.NameOrSexCd> returnData = new List<InitDataEntity.NameOrSexCd> ();

            switch (type)
			{
            case SearchEventManager.CurrentSettingState.BodyType:
                tmpList = InitDataApi._httpCatchData.result.body_type.list;
                break;
            case SearchEventManager.CurrentSettingState.HairStyle:
                tmpList = InitDataApi._httpCatchData.result.hair_style.list;
               break;
            case SearchEventManager.CurrentSettingState.Glasses:
                tmpList = InitDataApi._httpCatchData.result.glasses.list;
                break;
            case SearchEventManager.CurrentSettingState.Type:
                tmpList = InitDataApi._httpCatchData.result.type.list;
                break;
            case SearchEventManager.CurrentSettingState.Personality:
                tmpList = InitDataApi._httpCatchData.result.personality.list;
                break;
            case SearchEventManager.CurrentSettingState.Holiday:
                tmpList = InitDataApi._httpCatchData.result.holiday.list;
                break;
            case SearchEventManager.CurrentSettingState.AnnualIncome:
                tmpList = InitDataApi._httpCatchData.result.annual_income.list;
                break;
            case SearchEventManager.CurrentSettingState.Education:
                tmpList = InitDataApi._httpCatchData.result.education.list;
                break;
            case SearchEventManager.CurrentSettingState.Housemate:
                tmpList = InitDataApi._httpCatchData.result.housemate.list;
                break;
            case SearchEventManager.CurrentSettingState.Sibling:
                tmpList = InitDataApi._httpCatchData.result.sibling.list;
                break;
            case SearchEventManager.CurrentSettingState.Alcohol:
                tmpList = InitDataApi._httpCatchData.result.alcohol.list;
                break;
            case SearchEventManager.CurrentSettingState.Tobacco:
                tmpList = InitDataApi._httpCatchData.result.tobacco.list;
                break;
            case SearchEventManager.CurrentSettingState.Car:
                tmpList = InitDataApi._httpCatchData.result.car.list;
                break;
            case SearchEventManager.CurrentSettingState.Pet:
                tmpList = InitDataApi._httpCatchData.result.pet.list;
                break;
            case SearchEventManager.CurrentSettingState.Hobby:
                tmpList = InitDataApi._httpCatchData.result.hobby.list;
                break;
            case SearchEventManager.CurrentSettingState.Interest:
                tmpList = InitDataApi._httpCatchData.result.interest.list;
                break;
            case SearchEventManager.CurrentSettingState.Marital:
                tmpList = InitDataApi._httpCatchData.result.marital.list;
                break;
                
            }


            foreach (var item in tmpList) 
			{
                if (item.sex_cd == gender) 
				{
                    returnData.Add (item);
                }
            }
            
            return returnData;
        }
     }
}