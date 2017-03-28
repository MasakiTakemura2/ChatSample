using UnityEngine;
using System;

/// <summary>
/// ユーザーの状態。
/// </summary>
public enum UserStatusType
{
        ///"仮登録"
        STATUS_PRE_REGIST = 1,

        ///"本登録"
        STATUS_REGIST     = 2,

        ///"ブラック"
        STATUS_BLACK      = 3,

        ///"重複"
        STATUS_DUPLICATE  = 4,

        ///"垢BAN",
        STATUS_BAN        = 5,

        ///"退会"
        STATUS_RESIGN     =  9
}
