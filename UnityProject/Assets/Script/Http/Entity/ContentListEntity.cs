using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;


namespace Http
{
    public class ContentsEntity
    {
        /// <summary>
        /// StoryBoard Entity.
        /// 取得時の親クラス
        /// </summary>
        [Serializable]
        public class Contents
        {
            public string file_type_id;
			public string data_type_id;
            public string release_id;
            public string org_name;
            public string file_name;
            public string directory;
            public string file_path;
			public string file_size;
			public string file_hash;
        }
    }
}
