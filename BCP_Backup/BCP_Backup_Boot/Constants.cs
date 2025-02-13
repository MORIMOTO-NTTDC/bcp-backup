namespace CommonLibrary
{
    /// <summary>
    /// 定数を管理するクラスです。
    /// </summary>
    public static class Constants
    {
        // レジストリキーのパス
        public const string RegistryKeyPath = @"Software\bcpSoft";

        // BCP Backup Clientの実行ファイル名
        public const string ClientExeName = "BCP_Backup_Client.exe";

        // BCP Backup Serviceのサービス名
        public const string ServiceName = "BCP_Backup_Service";

        // 設定キー
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

            // バージョン
            public const string Version = "version";

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
        }
    }

    /// <summary>
    /// サービスステータスを表すクラスです。<br />
    /// <br />
    /// 停止中<br />
    /// 　↓　　サービス開始<br />
    /// 待機中<br />
    /// 　↓　　1分待機<br />
    /// 処理中<br />
    /// 　↓　　処理完了<br />
    /// 待機中 →　1分待機で処理中に戻る。<br />
    /// 　↓　　サービス停止<br />
    /// 停止中
    /// </summary>
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
    }
}
