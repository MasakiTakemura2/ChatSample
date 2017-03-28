using UnityEngine;

/// <summary>
/// Debug.
/// UnityEngine.Debugをオーバーライド
/// 但し、UnityEngine.Debug.Log()などフル指定はオーバーライドされない
/// </summary>

#if !UNITY_EDITOR
public static class Debug
{
	// falseで強制全消し
    static bool IsEnable(){ return true; }
	//static bool IsEnable(){ return UnityEngine.Debug.isDebugBuild; }

    static public void Break(){
        if( IsEnable() )    UnityEngine.Debug.Break();
    }


    #region DrawLine
	public static void DrawLine(Vector3 start, Vector3 end)
	{
		if (IsEnable())
			UnityEngine.Debug.DrawLine(start, end);
	}
	public static void DrawLine(Vector3 start, Vector3 end, Color color)
	{
		if (IsEnable())
			UnityEngine.Debug.DrawLine(start, end, color);
	}
	public static void DrawLine(Vector3 start, Vector3 end, Color color, float duration)
	{
		if (IsEnable())
			UnityEngine.Debug.DrawLine(start, end, color, duration);
	}
	public static void DrawLine(Vector3 start, Vector3 end, Color color, float duration, bool depthTest)
	{
		if (IsEnable())
			UnityEngine.Debug.DrawLine(start, end, color, duration, depthTest);
	}
	#endregion DrawLine



	#region DrawRay
	public static void DrawRay(Vector3 start, Vector3 dir)
	{
		if (IsEnable())
			UnityEngine.Debug.DrawRay(start, dir);
	}
	public static void DrawRay(Vector3 start, Vector3 dir, Color color)
	{
		if (IsEnable())
			UnityEngine.Debug.DrawRay(start, dir, color);
	}
	public static void DrawRay(Vector3 start, Vector3 dir, Color color, float duration)
	{
		if (IsEnable())
			UnityEngine.Debug.DrawRay(start, dir, color, duration);
	}
	public static void DrawRay(Vector3 start, Vector3 dir, Color color, float duration, bool depthTest)
	{
		if (IsEnable())
			UnityEngine.Debug.DrawRay(start, dir, color, duration, depthTest);
	}
	#endregion DrawRay


	#region log
	static public void Log(object message)
	{
		if (IsEnable())
				UnityEngine.Debug.Log(message);
	}
	static public void Log(object message, Object context)
	{
		if (IsEnable())
			UnityEngine.Debug.Log(message, context);
	}



	static public void LogWarning(object message)
	{
		if (IsEnable())
			UnityEngine.Debug.LogWarning(message);
	}
	static public void LogWarning(object message, Object context)
	{
		if (IsEnable())
			UnityEngine.Debug.LogWarning(message, context);
	}



	static public void LogError(object message)
	{
		if (IsEnable())
			UnityEngine.Debug.LogError(message);
	}
	static public void LogError(object message, Object context)
	{
		if (IsEnable())
			UnityEngine.Debug.LogError(message, context);
	}

	static public void LogException(object message)
	{
		if (IsEnable())
			UnityEngine.Debug.LogError(message);
	}
	static public void LogException(object message, Object context)
	{
		if (IsEnable())
			UnityEngine.Debug.LogError(message, context);
	}

    public static void Assert (bool condition, string format, params object[] args)
    {
        if (IsEnable ())
            UnityEngine.Debug.Assert (condition, string.Format (format, args) );
    }


    public static void Assert (bool condition)
    {
        if (IsEnable ())
            UnityEngine.Debug.Assert (condition);
    }

    public static void Assert (bool condition, Object context)
    {
        if (IsEnable ())
            UnityEngine.Debug.Assert(condition, context);
    }


    public static void Assert (bool condition, object message)
    {
        if (IsEnable ())
            UnityEngine.Debug.Assert(condition, message);
    }


    public static void Assert (bool condition, string message)
    {
        if (IsEnable ())
            UnityEngine.Debug.Assert(condition, message);
    }


    public static void Assert (bool condition, object message, Object context)
    {
        if (IsEnable ())
            UnityEngine.Debug.Assert(condition, message, context);
    }


    public static void Assert (bool condition, string message, Object context)
    {
        if (IsEnable ())
            UnityEngine.Debug.Assert(condition, message, context);
    }


    public static void AssertFormat (bool condition, string format, params object[] args)
    {
        if (IsEnable ())
            UnityEngine.Debug.Assert(condition, string.Format (format, args));
    }


    public static void AssertFormat (bool condition, Object context, string format, params object[] args)
    {
        if (IsEnable ())
            UnityEngine.Debug.Assert(condition, string.Format (format ,args));
    }

	#endregion log
}
#endif
