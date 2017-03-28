using UnityEngine;
using System.Collections;

namespace Helper {
    public class PlatformHelper {
       
        private static int _platformId;

        public static int GetPlatformNumber ()
        {
           #if UNITY_IPHONE
            	_platformId = (int)PlatformType.IOS;
           #elif UNITY_ANDROID
            	_platformId = (int)PlatformType.Android;
           #else
				_platformId = (int)PlatformType.UNKNOWN;
           #endif
                
           return _platformId;
        }
    }
}