using System.Text.Json.Serialization;

public class CapacityOverNotificationRequest
{
    /// <summary>
    /// アカウントID
    /// </summary>
    [JsonPropertyName("accountId")]
    public string AccountId { get; set; }

    /// <summary>
    /// BCP端末同期ディレクトリ使用容量
    /// </summary>
    [JsonPropertyName("usedSize")]
    public long UsedSize { get; set; }
}

public class CapacityOverNotificationResponse
{
}
