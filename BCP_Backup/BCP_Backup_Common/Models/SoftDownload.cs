using System.Text.Json.Serialization;

public class SoftDownloadRequest
{
    /// <summary>
    /// アカウント情報
    /// </summary>
    [JsonPropertyName("accountId")]
    public string AccountInfo { get; set; }
}

public class SoftDownloadResponse
{
}