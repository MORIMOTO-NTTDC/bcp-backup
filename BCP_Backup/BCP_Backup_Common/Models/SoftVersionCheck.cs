using System.Text.Json.Serialization;

public class SoftVersionCheckRequest
{
    /// <summary>
    /// BCP端末ソフトバージョン
    /// </summary>
    [JsonPropertyName("softVersion")]
    public string SoftVersion { get; set; }
}

public class SoftVersionCheckResponse
{
}
