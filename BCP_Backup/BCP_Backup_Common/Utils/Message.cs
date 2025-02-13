using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Remoting.Messaging;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace CommonLibrary
{
    public enum MessageKey
    {
        i00001,
        i00002,
        i00003,
        i00004,
        i00005,
        i00006,
        i00011,
        i00012,
        i00013,
        i00014,
        i00015,
        i00016,
        i00020,
        i00021,
        i00022,
        i00023,
        i00024,
        i00025,
        i00026,
        e00001,
        e00002,
        e00003,
        e00004,
        e00005,
        e00010,
        e00011,
        e00012,
        e00013,
        e00014,
        e00015,
        e00016,
        e00017,
        e00018,
        e00019,
        e00020,
        e00021,
        e00030,
        e00031,
        e00032,
        e00033,
        e00034,
        e00035,
        e00036,
        e00037,
        e00038,
        e00039,
        e00040,
        e00050,
        e00051,
        e00052,
        e00053,
        e00054,
        e00055,
        e00056,
        e00057,
    }

    public class Message
    {
        private static readonly Dictionary<MessageKey, string> messages = new Dictionary<MessageKey, string>
    {
        { MessageKey.i00001, "登録しました。" },
        { MessageKey.i00002, "更新しました。" },
        { MessageKey.i00003, "削除しました。" },
        { MessageKey.i00004, "{0}を変更しました。" },
        { MessageKey.i00005, "{0}します。よろしいですか？" },
        { MessageKey.i00006, "{0}しました。" },
        { MessageKey.i00011, "ログアウトしました。ログイン画面に遷移します。" },
        { MessageKey.i00012, "パスワード初期化しました。初期化後のパスワードはメールで確認してください" },
        { MessageKey.i00013, "アップロード実行をOFFにします。\n自動でファイルのアップロードが行われなくなります。\n本当によろしいですか？" },
        { MessageKey.i00014, "ダウンロード実行をONにします。\n端末側のフォルダの中身が、前回バックアップ時に置き換わります。\n本当によろしいですか？" },
        { MessageKey.i00015, "アップロード実行をOFFにされました。\n自動でファイルのアップロードが行われなくなります。\n本当によろしいですか？" },
        { MessageKey.i00016, "ダウンロード実行をONにされました。\n端末側のフォルダの中身が、前回バックアップ時に置き換わります。\n本当によろしいですか？" },
        { MessageKey.i00020, "下記のアカウントでログインします。よろしいですか？\n\nアカウントID：{0}\nアカウント名：{1}" },
        { MessageKey.i00021, "終了した場合は、ファイルのアップロードができなくなりますが、本当によろしいですか？" },
        { MessageKey.i00022, "バックアップサービスにて同期中のため、時間をおいて再度お試しください。" },
        { MessageKey.i00023, "{0} がすでに起動しています。終了します。" },
        { MessageKey.i00024, "{0}を開始します。" },
        { MessageKey.i00025, "{0}を終了します。処理時間：{1:N0}ms" },
        { MessageKey.i00026, "既に下記のアカウントでは、アップロードによるバックアップが行われています。\nログインすると同期元ディレクトリでバックアップされますが、ログインしてよろしいですか？\n\nアカウントID：{0}\nアカウント名：{1}" },
        { MessageKey.e00001, "予期しないエラーが発生しました。ヘルプデスクへお問い合わせください。" },
        { MessageKey.e00002, "ログインが必要です。ログイン画面へ遷移します。" },
        { MessageKey.e00003, "実行権限がありません。" },
        { MessageKey.e00004, "不正なリクエストです。" },
        { MessageKey.e00005, "入力エラーがあります。" },
        { MessageKey.e00010, "{0}は入力必須です。" },
        { MessageKey.e00011, "{0}には数字を入力してください。" },
        { MessageKey.e00012, "{0}には数値を入力してください。" },
        { MessageKey.e00013, "{0}には半角英数字で入力してください。" },
        { MessageKey.e00014, "{0}には半角英数字（記号含む）で入力してください。" },
        { MessageKey.e00015, "{0}に入力禁止文字\"{1}\"が含まれています。" },
        { MessageKey.e00016, "{0}には、{1}種類以上の文字種別を含めてください。" },
        { MessageKey.e00017, "{0}には{1}文字以上で入力してください。" },
        { MessageKey.e00018, "{0}にはｅメールアドレスの形式で入力してください。" },
        { MessageKey.e00019, "{0}には正しい日付を入力してください。" },
        { MessageKey.e00020, "{0}には{1}から{2}までの範囲で入力してください。" },
        { MessageKey.e00021, "{0}には{1}以降の日付を入力してください。" },
        { MessageKey.e00030, "ユーザID、メールアドレスまたはパスワードが間違っています。" },
        { MessageKey.e00031, "入力したメールアドレスは登録されていないため、パスワードを初期化できません。" },
        { MessageKey.e00032, "{0}と{1}の値が異なっています。同じ値を入力してください。" },
        { MessageKey.e00033, "{0}には{1}と異なる値を入力してください。" },
        { MessageKey.e00034, "{0}は見つかりませんでした。" },
        { MessageKey.e00035, "この{0}はすでに使用されています。別の{0}を入力してください。" },
        { MessageKey.e00036, "別のユーザ等により、すでに削除されているため、{0}できません。" },
        { MessageKey.e00037, "別のユーザ等により、すでに更新されているため、{0}できません。" },
        { MessageKey.e00038, "{0}には{1}を入力してください。" },
        { MessageKey.e00039, "{0}が間違っています。正しい{1}を入力してください。" },
        { MessageKey.e00040, "バックアップ容量は使用容量より大きい値（GB）を選択してください。" },
        { MessageKey.e00050, "申込番号か認証キーが間違っています。有効な申込番号と認証キーを入力してください。" },
        { MessageKey.e00051, "Cognito認証情報（ユーザID、またはパスワード）が間違っています。" },
        { MessageKey.e00052, "Cognito認証情報でエラーが発生しました。" },
        { MessageKey.e00053, "アカウントが存在しません。" },
        { MessageKey.e00054, "{0}に失敗しました。時間をおいて再度お試しください。" },
        { MessageKey.e00055, "{0}が存在しないため{1}を中止しました。" },
        { MessageKey.e00056, "{0}にエラーが発生しました。詳細はイベントログを確認してください。" },
        { MessageKey.e00057, "管理者権限がありません。管理者として実行してください。" },
    };

        public static string Get(MessageKey key, params object[] args)
        {
            if (messages.TryGetValue(key, out var message))
            {
                return string.Format(message, args);
            }
            return "メッセージが見つかりません。";
        }
    }
}
