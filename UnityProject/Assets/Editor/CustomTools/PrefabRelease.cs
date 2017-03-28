using System.Linq;
using UnityEditor;
using UnityEngine;


/*
【Unity】プレハブとインスタンスのヒモ付を解除するエディタ拡張
*/
namespace Tools {
    public class PrefabRelease
    {
        [MenuItem("Tools/PrefabDisconnect")]
        public static void Disconnect()
        {
            foreach (var n in Selection.gameObjects)
            {
                PrefabUtility.DisconnectPrefabInstance(n);
            }
        }
    }
}