using UnityEngine;
using System;

/// <summary>
/// シングルトンでアクセスできるScriptableObject
/// </summary>
public abstract class SingletonScriptableObject<T> : ScriptableObject where T : SingletonScriptableObject<T>
{
	/// <summary>
	/// アセットスクリプト
	/// </summary>
    protected static T instance = null;
	
	/// <summary>
	/// アセットスクリプトの取得
	/// </summary>
    public static T Instance
	{
		get
		{
			if (instance == null)
			{
				Type type = typeof(T);
                instance = Resources.Load("_Asset/" + type.Name, type) as T;
			}
			return instance;
		}      
		set
		{
			if (instance == null )
			{
				Type type = typeof(T);
                instance = Resources.Load("_Asset/" + type.Name, type) as T;
			}
			instance = value;
		}
	}
}
