using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

namespace Http
{
    public class UserDataEntity {

        [Serializable]
        public class Result {
            public User result;
        }

        [Serializable]
        public class User {
            public Basic user;
            public List<string> complete;
            public string review;
            public string maintenance;
            public string reject;
            public string is_auto_renewable;
            public string force_update;
            public AppReview appli_review;
        }

        [Serializable]
        public class AppReview {
            public string url;
            public string text;
        }

		[Serializable]
        public class Basic
		{
            public string is_banner;
            public string url;
            public string image_url;

            public string id;
            public string user_key;
            public string admin_id;
            public string appli_id;
            public string uiid;
            public string udid;
            public string idfv;
            public string name;
            public string ad_cd;
            public string status;
            public string platform;
            public string pref;
            public string city_id;
            public string location_pref;
            public string location_city_id;
            public string sex_cd;
            public string birth_date;
            public string blood_type;
            public string height;
            public string weight;
            public string profile;
            public string is_public_profile;
            public string is_public_gps;
            public string is_notification;
            public string current_point;
            public string is_payment;
            public string is_uninstall;
            public string pay_amount;
            public string pay_count;
            public string receive_report_count;
            public string send_report_count;
            public string remark;
            public string version;
            public string user_agent;
            public string model;
            public string ip_address;
            public string login_count;
            public string firstpayment_datetime;
            public string lastpayment_datetime;
            public string pre_regist_datetime;
            public string user_regist_datetime;
            public string lastaccess_datetime;
            public string resign_datetime;
            public string regist_datetime;
            public string lastup_datetime;
            public string disable;
            public string is_favorite;
            public string is_limit_release;
            public string is_block;
            public string profile_image_url;
            public string cover_image_url;
            public string profile_image_status;
            public string cover_image_status;
            public List<string> body_type;
            public List<string> hair_style;
            public List<string> glasses;
			public List<string> type;
            public List<string> personality;
            public List<string> holiday;
            public List<string> annual_income;
            public List<string> education;
            public List<string> housemate;
            public List<string> sibling;
            public List<string> alcohol;
            public List<string> tobacco;
            public List<string> car;
            public List<string> pet;
            public List<string> hobby;
            public List<string> interest;
            public List<string> marital;
			public string age;
            public string time_ago;
            public string lat;
            public string lng;
            public string mail_address;
            public string distance;
            public MyPassingSetting passing_config;
		}
        
        [Serializable]
        public class MyPassingSetting
        {
            public string is_passing;
            public string is_notification;
            public string is_send_message;
            public string message;
            public string sex_cd;
            public string age_from;
            public string age_to;
            public string height_from;
            public string height_to;
            public string body_type;
            public string is_image;
            public string radius;
            public string keyword;
        }
	}
}
