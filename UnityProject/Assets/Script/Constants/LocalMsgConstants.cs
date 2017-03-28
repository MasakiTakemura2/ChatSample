using UnityEngine;
using System.Collections;

public class LocalMsgConst 
{
    #region タイトルで使用する文言
    public const string TITLE_TALKLIST   = "トークリスト";
    public const string TITLE_FAVORITE   = "お気に入り";
    public const string TITLE_INFOMATION = "お知らせ";
    public const string TITLE_PROFILE    = "プロフィール";
    public const string TITLE_SEARCH     = "検索";
    public const string TITLE_BBS        = "掲示板";
    public const string TITLE_PURCHASE   = "ポイント購入";
    public const string TITLE_MYPAGE     = "マイページ";
    public const string TITLE_MATCHING   = "マッチング";
    public const string TITLE_HISTORY    = "履歴";
    
    public const string TITLE_PASSING_SETTING = "すれ違い通信設定";
    #endregion

    #region ポップアップで使用するローカル文言
    public const string REPORT_QUESTION         = "{0}さんを通報しますか？";
    public const string FAVORITE_ON_CONFIRM     = "{0}さんをお気に入り登録をしますか？";
    public const string USER_BLOCK_ON_QUESTION  = "{0}さんをブロックしますか？";
    public const string FAVORITE_OFF_CONFIRM    = "{0}さんをお気に入り解除しますか？";
    public const string USER_BLOCK_OFF_QUESTION = "{0}さんをブロック解除しますか？";

	public const string LIST_SEARCH_ZERO = "検索結果が０件の為　更新を中止します";
	public const string CONFIRM_IMAGE_SEND_MESSAGE = "画像を送信しますか？";
    public const string ALERT_TINDER               = "表示できる画像がありません";
	public const string SAVE_PROFILE_CONFIG = "プロフィールを設定しました";
	public const string POINT_SHORTAGE = "ポイントが足りません";
    public const string SELF_SHARE_INFO_CONFIRM = "当アプリではお客様の位置、写真、ニックネーム等の情報を他のユーザーと共有します。\n※各情報は非公開にも設定出来ます。";
    public const string CHECKIN_CONFIRM = "<size=55>位置情報を更新/共有し、マップにチェックインしますか？</size>\n「許可しない」をタップした場合はマップ上非公開（チェックアウト状態）になります。取得した位置情報は他のユーザーと共有し、掲示板、ユーザー検索で使用されます。";
    public const string BASIC_PROFILE_VALID_CONFIRM = "この機能を使うには基本プロフィールを作成してください。";
    public const string CHECKOUT_FIN    = "<size=60>チェックアウトしました。</size>\n位置情報がマップ上に公開されないように切り替わっています。";
    public const string LESS_THAN_20    = "20歳未満の可能性があるため選択できません。";
    public const string POINT_LIMIT_TXT = "{0}pt消費で{1}いいねを追加することが出来ます。";
    public const string BLOCK_USER      = "ブロックしているユーザーはチャットが出来ません。";
    public const string BBS_TITLE_VALIDATE = "掲示板のタイトルは20文字までになります";
    public const string BBS_NO_INPUT_VALIDATE = "タイトルと本文を入力してください";
    public const string ERROR_NOT_MAP_SHOW = "このアプリでの位置情報サービスが許可されていません。";
    public const string CONFIRM_TEMRS_ALLOW_DENNY_TEXT =  "不適切な（画像、文言）等を生成するユーザーに対しては24時間以内に監視員によるアカウント停止及びコンテンツ削除処理を行います。通報・ブロックによる報告は24時間以内に監視員にて対応を行います。\n\n利用規約に同意";
    
    public const string USER_NO_ASSCESS_TEXT = "只今、お客様のご利用を制限させて頂いております。\n利用制限についてのお問い合わせにつきましては\nIDをメールタイトルに記載して頂きました後\n{0}までお問い合わせください。\n\nお客様のIDは「{1}」です。";
    public const string RESIGN_TEXT = "ご利用になられておりましたアカウントは\n退会処理となっております。\n再度ご利用頂く場合は、解除ボタンを押して下さい。";
    public const string RESIGN_CHECK_TEXT = "{0}様\n当アプリの会員登録を解除します。解除を中止する場合はこのままマイページにお戻りください。\n\n※退会後、当アプリを再度利用をご希望される場合は退会後の表示画面に従い手続きを完了させて下さい。";
    public const string RESIGN_RELASE_TEXT = "退会を解除しますか？";
	public const string REALLY_RESIGN_RELASE_TEXT = "本当に退会しても宜しいでしょうか？";

    public const string OK  = "Ok";
    public const string NG  = "No";
    public const string PUSH_POP_OK = "今すぐ確認";
    public const string PUSH_POP_NG = "あとでみる";
    public const string YES = "はい";
    public const string NO  = "いいえ";
    public const string LIKE_TEXT  = "いいね!";
    public const string MAKE_BASIC_PROF = "プロフ作成";
    public const string CANCEL          = "キャンセル";
    public const string APPROVAL   = "承認";
    public const string DENIAL     = "拒否";
    public const string OK_PERMISSION = "許可する";
    public const string NG_PERMISSION = "許可しない";

    public const string POST_FIX_MESSAGE = "投稿が完了しました";
    //public const string POINT_USE_IMAGE_CONFIRM = "ポイントを消費して画像を見ますか？";
    public const string POINT_USE_IMAGE_CONFIRM = "動画を閲覧して画像を見ますか？";
    public const string MESSAGE_DEL_FIX         = "削除が完了しました";
    public const string MESSAGE_LIST_DELETE     = "メッセージを削除しても宜しいですか？";
    public const string PROFILE_CHANGE_ANNOUNCE = "プロフィールの変更があります、変更を保存しますか？";
    #endregion

    #region その他で使用する文言
    public const string ANOUYMUS_NAME  = "匿名";
    public const string USE_POINT_TEXT = "所持ポイント:";
    public const string PT_TEXT        = "pt";
    public const string AGE_TEXT       = "歳";
    public const string BLOOD_TYPE_JA  = "型";
    public const string GENDER_MALE    = "男性";
    public const string GENDER_FEMALE  = "女性";
    public const string NOT_OPEN_FUNCTION  = "未開放";
    public const string OPEN_FUNCTION      = "送り放題機能開放中";
    public const string ME_TEXT            = "あなた";

    public const string NETWORK_ERROR_TEXT = "ネットワークに接続が出来ません";
    public const string MAINTENANCE        = "現在、メンテナンスを行なっています。\nしばらくしてアプリを起動してください。";
    public const string FORCE_UPDATE       = "アプリの新しいアップデートがあります。";
    
    public const string BIRTHDAY_SELECT = "生年月日を選択してください";
    public const string YMD_FORMAT_1    = "yyyy-MM-dd";
    public const string YMD_FORMAT_2    = "yyyy年MM月dd日";

    public const string BIRTHDAY_ERROR  = "<color=red>生年月日に誤りがあります。</color>";    
    #endregion
}
