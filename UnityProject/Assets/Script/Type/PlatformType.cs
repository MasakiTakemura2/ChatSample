using UnityEngine;
using System;

/// <summary>
/// 言語種別
/// </summary>
public enum PlatformType
{
    /// <summary>
    /// 不明な言語はとりあえずIOS
    /// </summary>
    UNKNOWN = 2,

    /// <summary>
    /// UnityのEditor
    /// </summary>
    Editor = 1,

    /// <summary>
    /// IOS
    /// </summary>
    IOS = 2,

    /// <summary>
    /// アンドロイド
	/// Google
    /// </summary>
    Android = 3,
}
