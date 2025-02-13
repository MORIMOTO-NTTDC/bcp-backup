using System.Text.Json.Serialization;

public class AccountDetailRequest
{
    /// <summary>
    /// アカウントID
    /// </summary>
    [JsonPropertyName("accountId")]
    public string AccountId { get; set; }
}

public class AccountDetailResponse
{
    /// <summary>
    /// アカウントID
    /// </summary>
    [JsonPropertyName("accountId")]
    public string AccountId { get; set; }

    /// <summary>
    /// アップロード実行可否
    /// </summary>
    [JsonPropertyName("uploadEnable")]
    public string UploadEnable { get; set; }

    /// <summary>
    /// アップロード頻度
    /// </summary>
    [JsonPropertyName("uploadTiming")]
    public string UploadTiming { get; set; }

    /// <summary>
    /// バックアップ容量（GB）
    /// </summary>
    [JsonPropertyName("backupCapa")]
    public string BackupCapa { get; set; }

    /// <summary>
    /// 使用容量（GB）
    /// </summary>
    [JsonPropertyName("usedSize")]
    public string UsedSize { get; set; }

    /// <summary>
    /// ダウンロード実行可否
    /// </summary>
    [JsonPropertyName("downloadEnable")]
    public string DownloadEnable { get; set; }

    /// <summary>
    /// 最終同期日時（yyyy/MM/DD HH24:mm:ss形式）
    /// </summary>
    [JsonPropertyName("lastdate")]
    public string LastDate { get; set; }

    /// <summary>
    /// 同期ステータス
    /// </summary>
    [JsonPropertyName("status")]
    public string Status { get; set; }

    /// <summary>
    /// エラー情報
    /// </summary>
    [JsonPropertyName("errorInfo")]
    public string ErrorInfo { get; set; }

    /// <summary>
    /// S3バケット
    /// </summary>
    //[JsonPropertyName("s3Bucket")]
    [JsonPropertyName("s3backet")]
    public string S3Bucket { get; set; }

    /// <summary>
    /// 実行許可パスワード
    /// </summary>
    [JsonPropertyName("permitPassword")]
    public string PermitPassword { get; set; }

    /// <summary>
    /// 更新回数
    /// </summary>
    [JsonPropertyName("version")]
    public string Version { get; set; }

    /// <summary>
    /// S3エンドポイント
    /// </summary>
    //[JsonPropertyName("s3Endpoint")]
    [JsonPropertyName("s3Endpoint")]
    public string S3Endpoint { get; set; }
}
