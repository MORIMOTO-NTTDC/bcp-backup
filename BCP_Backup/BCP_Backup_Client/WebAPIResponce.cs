using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace BCP_Backup_Client
{
    internal class WebAPIResponce
    {
        // 端末ソフトバージョンチェック /api01_soft_check.do
        public class API01_01
        {
        }
        // BCP管理ソフトダウンロード /api01_soft_download.do
        public class API01_02
        {
        }
        // アカウント認証 /api01_auth.do
        public class API01_03
        {
            public string accountName { get; set; } // アカウント名
            public string accessKeyId { get; set; } // アクセスキーID
            public string secretAccessKey { get; set; } // シークレットアクセスキー
            public string sessionToken { get; set; }	// セッショントークン
        }
        // アカウント設定情報取得 /api01_detail.do
        public class API01_04
        {
            public string accountId { get; set; }   // アカウントID
            public string uploadEnable { get; set; }    // アップロード実行可否
            public string uploadTiming { get; set; }    // アップロード頻度
            public string backupCapa { get; set; }  // バックアップ容量（GB）
            public string usedSize { get; set; }    // 使用容量（GB）
            public string downloadEnable { get; set; }  // ダウンロード実行可否
            public string lastdate { get; set; }    // 最終同期日時（yyyy/MM/DD HH24:mm:ss形式）
            public string status { get; set; }  // 同期ステータス
            public string errorInfo { get; set; }   // エラー情報
            public string s3backet { get; set; }    // S3バケット
            public string permitPassword { get; set; }  // 実行許可パスワード
            public string version { get; set; }	// 更新回数
        }
        // アカウント設定情報更新	/api01_update.do
        public class API01_05
        {
        }
        // 容量オーバー通知	/api01_capa_over.do
        public class API01_06
        {
        }
        // 同期結果登録	/api01_sync.do
        public class API01_07
        {
        }
    }
}
