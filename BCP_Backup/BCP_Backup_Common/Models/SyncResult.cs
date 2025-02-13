using System;
using System.Text.Json.Serialization;

public class SyncResultRequest
{
    /// <summary>
    /// アカウントID
    /// </summary>
    [JsonPropertyName("accountId")]
    public string AccountId { get; set; }

    /// <summary>
    /// 最終同期日時
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
    /// BCP端末同期ディレクトリ使用容量
    /// </summary>
    [JsonPropertyName("usedSize")]
    public int UsedSize { get; set; }
}

public class SyncResultResponse
{
}
