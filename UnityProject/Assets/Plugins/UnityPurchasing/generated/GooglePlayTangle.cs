#if UNITY_ANDROID || UNITY_IPHONE || UNITY_STANDALONE_OSX || UNITY_TVOS
// WARNING: Do not modify! Generated file.

namespace UnityEngine.Purchasing.Security {
    public class GooglePlayTangle
    {
        private static byte[] data = System.Convert.FromBase64String("hDyvMYWwpozXz0/snzh9VEwlL4r8E60swfJ+00vTCayb9e8YncWpP6OOwRAsF4Dz7NluHSecKUfnVyaJozZcqC+kVpxn5AaXO8ha6sgPNFnoO5UA6jctMg1C8skpq8V8SgpmEnhQ2wlFkoEb3mobbt3r9Ys7dYYcnvBAKehEHSRSS2oWZT8FqdOvqk8KMa5iJpT3LtKIviZkVZx7FAiT5Q/1HoIQ++QoDjPCdgVcYEhqQ2389Hd5dkb0d3x09Hd3dsOD/CINgSFG9HdURntwf1zwPvCBe3d3d3N2de56vAmIMjQ3Fej2snHbZIy8QcjWBDChDwl2y7OY1H1mBGjXVlcFOyC8Reqv6IpJchHVyi04Zz5o24dY/ROrGYbQ3YmRs3R1d3Z3");
        private static int[] order = new int[] { 11,2,7,4,13,13,12,8,10,10,11,12,13,13,14 };
        private static int key = 118;

        public static byte[] Data() {
            return Obfuscator.DeObfuscate(data, order, key);
        }
    }
}
#endif
