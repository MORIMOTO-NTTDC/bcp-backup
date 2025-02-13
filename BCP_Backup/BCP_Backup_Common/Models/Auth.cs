using System.Text.Json.Serialization;

public class AuthRequest
{
    /// <summary>
    /// アカウントID
    /// </summary>
    [JsonPropertyName("accountId")]
    public string AccountId { get; set; }

    /// <summary>
    /// 認証キー
    /// </summary>
    [JsonPropertyName("authKey")]
    public string AuthKey { get; set; }

    /// <summary>
    /// BCP端末ソフトバージョン
    /// </summary>
    [JsonPropertyName("softVersion")]
    public string SoftVersion { get; set; }

    /// <summary>
    /// BCP端末同期ディレクトリ
    /// </summary>
    [JsonPropertyName("localDir")]
    public string LocalDir { get; set; }

}

public class AuthResponse
{

    /// <summary>
    /// アカウント名
    /// </summary>
    [JsonPropertyName("accountName")]
    public string AccountName { get; set; }

    /// <summary>
    /// アクセスキーID
    /// </summary>
    [JsonPropertyName("accessKeyId")]
    public string AccessKeyId { get; set; }

    /// <summary>
    /// シークレットアクセスキー
    /// </summary>
    [JsonPropertyName("secretAccessKey")]
    public string SecretAccessKey { get; set; }

    /// <summary>
    /// セッショントークン
    /// </summary>
    [JsonPropertyName("sessionToken")]
    public string SessionToken { get; set; }

    /// <summary>
    /// 同期ステータス
    /// </summary>
    [JsonPropertyName("status")]
    public string Status { get; set; }
}
