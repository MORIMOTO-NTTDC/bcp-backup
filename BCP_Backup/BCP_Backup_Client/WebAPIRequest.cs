using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace BCP_Backup_Client
{
    public class WebAPIRequest
    {
        // 端末ソフトバージョンチェック /api01_soft_check.do
        public class API01_01
        {
            [JsonPropertyName("softVersion")]
            public string SoftVersion { get; set; }
        }
        // BCP管理ソフトダウンロード /api01_soft_download.do
        public class API01_02
        {
            [JsonPropertyName("accountId")]
            public string AccountId { get; set; }
        }
        // アカウント認証 /api01_auth.do
        public class API01_03
        {
            [JsonPropertyName("accountId")]
            public string AccountId { get; set; }

            [JsonPropertyName("softVersion")]
            public string SoftVersion { get; set; }

            [JsonPropertyName("localDir")]
            public string LocalDir { get; set; }
        }
        // アカウント設定情報取得 /api01_detail.do
        public class API01_04
        {
            [JsonPropertyName("accountId")]
            public string AccountId { get; set; }
        }
        // アカウント設定情報更新	/api01_update.do
        public class API01_05
        {
            [JsonPropertyName("accountId")]
            public string AccountId { get; set; }

            [JsonPropertyName("uploadEnable")]
            public string UploadEnable { get; set; }

            [JsonPropertyName("uploadTiming")]
            public string UploadTiming { get; set; }

            [JsonPropertyName("localDir")]
            public string LocalDir { get; set; }

            [JsonPropertyName("version")]
            public string Version { get; set; }
        }
        // 容量オーバー通知	/api01_capa_over.do
        public class API01_06
        {
            [JsonPropertyName("accountId")]
            public string AccountId { get; set; }

            [JsonPropertyName("usedSize")]
            public string UsedSize { get; set; }
        }
        // 同期結果登録	/api01_sync.do
        public class API01_07
        {
            [JsonPropertyName("accountId")]
            public string AccountId { get; set; }

            [JsonPropertyName("lastdate")]
            public string Lastdate { get; set; }

            [JsonPropertyName("status")]
            public string Status { get; set; }

            [JsonPropertyName("errorInfo")]
            public string ErrorInfo { get; set; }

            [JsonPropertyName("usedSize")]
            public string UsedSize { get; set; }
        }
    }
}
