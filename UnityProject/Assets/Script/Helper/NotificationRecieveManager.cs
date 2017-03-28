using UnityEngine;
using System.Collections;

namespace Helper {
    /// <summary>
    /// Notification recieve manager.
    /// IOSやアンドロイドで
    /// </summary>
    public class NotificationRecieveManager : SingletonMonoBehaviour<NotificationRecieveManager> 
    {

        public static bool _isCatch = false;

        /// <summary>
        /// Notification recive.
        /// </summary>
        public class NotificationRecive
        {
            public string view_name; //遷移先
            public string id;        //user_id
        }
        
        /// <summary>
        /// Nexts the scene proccess.
        /// </summary>
        /// <returns>The scene proccess.</returns>
        public static void NextSceneProccess (string catchData = "")
        {
            _isCatch = false;

            if (string.IsNullOrEmpty (catchData) == false)
            {
                NotificationRecive data = new NotificationRecive ();
                string[] splitData = catchData.Split (' ');
                data.view_name = splitData [0];
                if (splitData.Length > 1)
                    data.id = splitData [1];

#if UNITY_ANDROID
        //ステータスバーを表示 //Android用
        ApplicationChrome.statusBarState = ApplicationChrome.States.VisibleOverContent;
#endif
                

                switch (data.view_name)
                {
                    //メッセージリスト
                    case CommonConstants.VIEW_NAME_MESSAGE: 
                        SceneHandleManager.NextSceneRedirect (CommonConstants.MESSAGE_SCENE);
                        _isCatch = true;
                        break;
                    //メッセージ詳細
                    case CommonConstants.VIEW_MESSAGE_DETAIL: 
                        SceneHandleManager.NextSceneRedirect (CommonConstants.MESSAGE_SCENE);
                        _isCatch = true;
                        break;
                    //ユーザーのプロファイル表示
                    case CommonConstants.VIEW_PROFILE: 
                        SceneHandleManager.NextSceneRedirect (CommonConstants.MESSAGE_SCENE);
                        _isCatch = true;
                        break;
                    case CommonConstants.VIEW_BOARD: //掲示板リストに飛ばす
                        SceneHandleManager.NextSceneRedirect (CommonConstants.BULLETIN_BOARD_SCENE);
                        //_isCatch = true;
                        break;
                    //マイページの履歴を表示する処理。
                    case CommonConstants.VIEW_HISTORY:
                        SceneHandleManager.NextSceneRedirect (CommonConstants.MYPAGE_SCENE);
                        _isCatch = true;
                        break;
                    //お知らせ詳細用プッシュが飛んできた場合。
                    case CommonConstants.VIEW_INFO_DETAIL:
                        SceneHandleManager.NextSceneRedirect (CommonConstants.MESSAGE_SCENE);
                        _isCatch = true;
                    break;
                   default:
                       SceneHandleManager.NextSceneRedirect (CommonConstants.MYPAGE_SCENE);
                   break;
                }

                AppStartLoadBalanceManager._toPushCatchUserId = data.id;
                AppStartLoadBalanceManager._toScenePanel      = data.view_name;
                return;
            }
            else
            {
                //キャッチしたデータが無ければマイページのシーンにジャンプ
                SceneHandleManager.NextSceneRedirect (CommonConstants.MYPAGE_SCENE);
                return;
            }
        }
    }
}