using System.Text.Json.Serialization;

public class AccountUpdateRequest
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
    public bool UploadEnable { get; set; }

    /// <summary>
    /// アップロード頻度
    /// </summary>
    [JsonPropertyName("uploadTiming")]
    public int UploadTiming { get; set; }

    /// <summary>
    /// BCP端末同期ディレクトリ
    /// </summary>
    [JsonPropertyName("localDir")]
    public string LocalDir { get; set; }

    /// <summary>
    /// 更新回数
    /// </summary>
    [JsonPropertyName("version")]
    public int Version { get; set; }
}

public class AccountUpdateResponse
{

}
