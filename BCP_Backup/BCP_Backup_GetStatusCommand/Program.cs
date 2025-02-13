using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Microsoft.Win32;

namespace BCP_Backup_GetStatusCommand
{
    // コンソール・アプリケーション名
    // bcpstatus.exe
    // 前提条件
    // 特になし
    // 出力ファイル名
    // bcpstatus.csv

    internal class Program
    {
        static void Main(string[] args)
        {
            // 実行権限が管理者権限であるか確認
            if (!IsAdministrator())
            {
                // 終了コードとして“7”を返して終了。
                Environment.Exit(7);
            }

            // ① レジストリにアカウントIDが無い場合、終了コードとして“1”を返して終了。
            string accountId = GetAccountIdFromRegistry();
            if (string.IsNullOrEmpty(accountId))
            {
                Environment.Exit(1);
            }

            // ② 直近のアップロード、ダウンロード結果を取得しCSV形式で出力ファイルへ書き込む。
            var status = GetLatestStatus();
            WriteStatusToCsv(status);

            // ③ 終了コードとして“0” 返して終了。
            Environment.Exit(0);
        }

        static string GetAccountIdFromRegistry()
        {
            using (RegistryKey key = Registry.LocalMachine.OpenSubKey(@"Software\bcpSoft"))
            {
                return key?.GetValue(Settings.AccountId) as string;
            }
        }

        static Status GetLatestStatus()
        {
            using (RegistryKey key = Registry.LocalMachine.OpenSubKey(@"Software\bcpSoft"))
            {
                // ダミーデータを返す
                var status = new Status
                {
                    LastSyncDate = key?.GetValue(Settings.LastDate) as string,
                    SyncStatus = SyncStatus.TranslateStatus(key?.GetValue(Settings.Status) as string),
                    ErrorDetails = key?.GetValue(Settings.ErrorInfo) as string,
                    UsedSpaceGB = key?.GetValue(Settings.UsedSize) as string,
                    BackupSpaceGB = key?.GetValue(Settings.BackupCapa) as string,
                    ProcessState = ServiceStatus.TranslateStatus(key?.GetValue(Settings.ServiceStatus) as string),
                };

                status.UsageRate = double.Parse(status.UsedSpaceGB) / double.Parse(status.BackupSpaceGB) * 100 + "%";

                return status;
            }
        }

        static void WriteStatusToCsv(Status status)
        {
            using (StreamWriter writer = new StreamWriter("bcpstatus.csv"))
            {
                writer.WriteLine("最終同期日時,同期ステータス,エラー内容,使用容量(GB),バックアップ容量(GB),使用率,処理状態");
                writer.WriteLine($"{status.LastSyncDate},{status.SyncStatus},{status.ErrorDetails},{status.UsedSpaceGB},{status.BackupSpaceGB},{status.UsageRate},{status.ProcessState}");
            }
        }
        /// <summary>
        ///     管理者権限で実行されているか確認します。
        /// </summary>
        /// <returns>確認結果</returns>
        private static bool IsAdministrator()
        {
            var identity = System.Security.Principal.WindowsIdentity.GetCurrent();
            var principal = new System.Security.Principal.WindowsPrincipal(identity);
            return principal.IsInRole(System.Security.Principal.WindowsBuiltInRole.Administrator);
        }

    }

    class Status
    {
        public string LastSyncDate { get; set; }
        public string SyncStatus { get; set; }
        public string ErrorDetails { get; set; }
        public string UsedSpaceGB { get; set; }
        public string BackupSpaceGB { get; set; }
        public string UsageRate { get; set; }
        public string ProcessState { get; set; }
    }

    public static class Settings
    {
        // アカウントID
        public const string AccountId = "ACCOUNT_ID";

        // アカウント名
        public const string AccountName = "ACCOUNT_NAME";

        // アップロード実行可否
        public const string UploadEnable = "UPLOAD_ENABLE";

        // アップロード頻度
        public const string UploadTiming = "UPLOAD_TIMING";

        // バックアップ容量（GB）
        public const string BackupCapa = "BACKUP_CAPA";

        // 使用容量（GB）
        public const string UsedSize = "USED_SIZE";

        // 最終同期日時
        public const string LastDate = "LASTDATE";

        // 同期ステータス
        public const string Status = "STATUS";

        // エラー情報
        public const string ErrorInfo = "ERROR_INFO";

        // S3バケット
        public const string S3Bucket = "S3BACKET";

        // BCP端末同期ディレクトリ
        public const string LocalDir = "LOCAL_DIR";

        // 実行許可パスワード
        public const string PermitPassword = "PERMIT_PASSWORD";

        // 更新回数
        public const string UpdateCount = "UPDATE_COUNT";

        // アクセスキーID
        public const string AccessKeyId = "accessKeyId";

        // シークレットアクセスキー
        public const string SecretAccessKey = "secretAccessKey";

        // セッショントークン
        public const string SessionToken = "sessionToken";

        // ダウンロード実行可否
        public const string DownloadEnable = "DOWNLOAD_ENABLE";

        // サービスステータス
        public const string ServiceStatus = "SERVICE_STATUS";

        // s3エンドポイント
        public const string S3Endpoint = "S3_ENDPOINT";
    }

    /// <summary>
    /// 同期ステータスを表すクラスです。
    /// </summary>
    public static class SyncStatus
    {
        /// <summary>
        /// 未実行
        /// </summary>
        public const string NotExecuted = "0";

        /// <summary>
        /// アップロード成功
        /// </summary>
        public const string UploadSuccess = "1";

        /// <summary>
        /// ダウンロード成功
        /// </summary>
        public const string DownloadSuccess = "2";

        /// <summary>
        /// アップロード失敗
        /// </summary>
        public const string UploadFailed = "3";

        /// <summary>
        /// ダウンロード失敗
        /// </summary>
        public const string DownloadFailed = "4";

        /// <summary>
        /// 同期不可
        /// </summary>
        public const string SyncUnavailable = "5";

        public static string TranslateStatus(string status)
        {
            switch (status)
            {
                case SyncStatus.NotExecuted:
                    return "未実行";
                case SyncStatus.UploadSuccess:
                    return "アップロード成功";
                case SyncStatus.DownloadSuccess:
                    return "ダウンロード成功";
                case SyncStatus.UploadFailed:
                    return "アップロード失敗";
                case SyncStatus.DownloadFailed:
                    return "ダウンロード失敗";
                case SyncStatus.SyncUnavailable:
                    return "同期不可";
                default:
                    return "不明なステータス";
            }
        }
    }

    public static class ServiceStatus
    {
        /// <summary>
        /// 待機中
        /// </summary>
        public const string Waiting = "0";

        /// <summary>
        /// 処理中（アップロード、ダウンロード中）
        /// </summary>
        public const string Processing = "1";

        /// <summary>
        /// 停止中
        /// </summary>
        public const string Stopping = "2";

        public static string TranslateStatus(string status)
        {
            switch (status)
            {
                case ServiceStatus.Waiting:
                    return "待機中";
                case ServiceStatus.Processing:
                    return "処理中";
                case ServiceStatus.Stopping:
                    return "停止中";
                default:
                    return "不明なステータス";
            }
        }
    }


}
