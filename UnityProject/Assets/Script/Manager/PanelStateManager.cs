using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Panel state manager.
/// </summary>
public class PanelStateManager
{
    /// <summary>
    /// The state of the panel.
    /// </summary>
    //public static PanelStateType _panelState;
    
    public static Dictionary<string, string> _panelHeaderNames = new Dictionary<string, string>();
    
    /// <summary>
    /// Inits the header string manager.
    /// const的にタイトルの名前を管理
    /// アンドロイドのバックボタンもこのデータを応用して対応出来るかも
    /// </summary>
    /// <returns>The header string manager.</returns>
    public static void InitPanelSet ()
    {
        _panelHeaderNames = new Dictionary<string, string>();   
         //※※※ 追加する時にキーが重複してないか確認してから追加してください。
        _panelHeaderNames.Add ("PanelMypageMain"        , "マイページ");
        _panelHeaderNames.Add ("PanelProfileChange"     , "プロフ変更");
        _panelHeaderNames.Add ("OtherSetting"           , "その他設定");
        _panelHeaderNames.Add ("PanelContact"           , "お問い合わせ");
        _panelHeaderNames.Add ("PanelDeviceChange"      , "機種変更");
        _panelHeaderNames.Add ("PanelProfileSetTemplate", "{0}");
        _panelHeaderNames.Add ("PanelProfileInput"      , "{0}");
        _panelHeaderNames.Add ("PaneHistory"            , "履歴");
        _panelHeaderNames.Add ("PanelChat"              , "{0}");
        _panelHeaderNames.Add ("PanelProfile"           , "{0}");
        _panelHeaderNames.Add ("PanelRandMessage"       , "ご近所チャット");
        _panelHeaderNames.Add ("PanelTerms"             , "ご利用規約");
        _panelHeaderNames.Add ("PanelPrivachy"          , "プライバシーポリシー");
        _panelHeaderNames.Add ("PanelTrans"             , "特定商取引");
        _panelHeaderNames.Add ("PanelCampaign"          , "キャンペーン");
        _panelHeaderNames.Add ("HelpPanel"              , "ヘルプ");
        _panelHeaderNames.Add ("HelpPanelDetail"        , "ヘルプ詳細");
		_panelHeaderNames.Add ("BulletInBoardPanel"     , "掲示板");
        _panelHeaderNames.Add ("BulletInBoardDetailPanel" , "掲示板詳細");
		_panelHeaderNames.Add ("SearchPanel"     , "検索");

    }

    /// <summary>
    /// Gets the header string by key.
    /// </summary>
    /// <returns>The header string by key.</returns>
    public static string GetHeaderStringByKey (string key)
    {
        string returnString = "";

        foreach (var name in _panelHeaderNames) {
            if (name.Key == key) {
                returnString = name.Value;
                break;
            }
        }

        return returnString;
    }
}