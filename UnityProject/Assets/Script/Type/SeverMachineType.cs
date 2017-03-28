using UnityEngine;
using System;

/// <summary>
/// サーバータイプの選択
/// </summary>
public enum SeverMachineType
{
    /// <summary>
    /// 開発用マシーン
    /// </summary>
    DEV,

    /// <summary>
    /// ローカルマシン
    /// </summary>
    LOCAL,

    /// <summary>
    /// ステージング
    /// </summary>
    STG,

    /// <summary>
    /// 本番用サーバ
    /// </summary>
    PRODUCTION,
}
