using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

namespace Http
{
    public class InitDataEntity
    {
        [Serializable]
        public class Result {
            public InitDataAll result;
        }

        [Serializable]
        public class InitDataAll {
            public List<PrefsData> pref;
            public List<CityData> city;

            //都道府県データのマスターVersion管理用。
            public string city_version;

			//public ConstansData radius;
			//public ConstansData user_sort;
            public ConstansData body_type;
            public ConstansData hair_style;
            public ConstansData glasses;
            public ConstansData type;
            public ConstansData personality;
            public ConstansData holiday;
            public ConstansData annual_income;
            public ConstansData education;
            public ConstansData housemate;
            public ConstansData sibling;
            public ConstansData alcohol;
            public ConstansData tobacco;
            public ConstansData car;
            public ConstansData pet;
            public ConstansData hobby;
            public ConstansData interest;
            public ConstansData marital;

            public List<FixedPropertyIdBase> sex_cd;
            public List<FixedPropertyIdBase> blood_type;
            public List<FixedPropertyIdBase> age_aetas;
            public List<FixedPropertyIdBase> board_category;
            public List<FixedPropertyIdBase> radius;
            public List<FixedPropertyIdBase> user_sort;
        }

		[Serializable]
        public class PrefsData
		{      
            public string id;
            public string name;
		}

        [Serializable]
        public class CityData
        {      
            public string id;
            public string pref;
            public string name;
        }

        [Serializable]
        public class ConstansData
        {
            public List<NameOrSexCd> list;
            public string title;
        }


        [Serializable]
        public class NameOrSexCd
        {
            public string name;
            public string sex_cd;
        }

        [Serializable]
        public class FixedPropertyIdBase
        {
            public string id;
            public string name;
        }

        [SerializeField]
        public class PrefAndCity {
            public List<PrefsData> pref;
            public List<CityData> city;
        }
	}
}
