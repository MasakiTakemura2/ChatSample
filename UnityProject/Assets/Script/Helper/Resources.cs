using UnityEngine;
using System;
using System.Collections.Generic;

/// <summary>
/// Resources.
/// UnityEngine.Resourcesをオーバーライド
/// 但し、UnityEngine.Debug.Resources()などフル指定はオーバーライドされない
/// </summary>

public static class Resources
{
	private static Dictionary<string, UnityEngine.Object> cache;

	public static UnityEngine.Object Load(string path)
	{
		if (cache == null) {
			cache = new Dictionary<string, UnityEngine.Object> ();
		}

		if (!cache.ContainsKey(path)) {
			cache [path] = UnityEngine.Resources.Load (path);
		}
		return cache[path];
    }

	public static UnityEngine.Object Load(string path, Type systemTypeInstance) {return UnityEngine.Resources.Load (path, systemTypeInstance);}
	public static T Load<T>(string path) where T : UnityEngine.Object {return UnityEngine.Resources.Load<T>(path);}
	public static T[] FindObjectsOfTypeAll<T> () where T : UnityEngine.Object {return UnityEngine.Resources.FindObjectsOfTypeAll<T> ();}
	public static UnityEngine.Object[] FindObjectsOfTypeAll (Type type) {return UnityEngine.Resources.FindObjectsOfTypeAll (type);}
	public static T GetBuiltinResource<T> (string path) where T : UnityEngine.Object {return UnityEngine.Resources.GetBuiltinResource<T> (path);}
	public static UnityEngine.Object GetBuiltinResource (Type type, string path) {return UnityEngine.Resources.GetBuiltinResource (type, path);}
	public static T[] LoadAll<T> (string path) where T : UnityEngine.Object {return UnityEngine.Resources.LoadAll<T> (path);}
	public static UnityEngine.Object[] LoadAll (string path) {return UnityEngine.Resources.LoadAll (path);}
	public static UnityEngine.Object[] LoadAll (string path, Type systemTypeInstance) {return UnityEngine.Resources.LoadAll (path, systemTypeInstance);}
	//public static T LoadAssetAtPath<T> (string assetPath) where T : UnityEngine.Object {return UnityEngine.Resources.LoadAssetAtPath<T> (assetPath);}
	//public static UnityEngine.Object LoadAssetAtPath (string assetPath, Type type) {return UnityEngine.Resources.LoadAssetAtPath (assetPath, type);}
	public static ResourceRequest LoadAsync (string path, Type type) {return UnityEngine.Resources.LoadAsync (path, type);}
	public static ResourceRequest LoadAsync<T> (string path) where T : UnityEngine.Object {return UnityEngine.Resources.LoadAsync<T> (path);}
	public static ResourceRequest LoadAsync (string path) {return UnityEngine.Resources.LoadAsync (path);}
	//public static void UnloadAsset (UnityEngine.Object assetToUnload) {return UnityEngine.Resources.UnloadAsset (assetToUnload);}
	public static AsyncOperation UnloadUnusedAssets () {return UnityEngine.Resources.UnloadUnusedAssets ();}
}
