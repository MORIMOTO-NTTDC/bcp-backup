using System.Text.Json.Serialization;

public class AccountRegistrationRequest
{
    /// <summary>
    /// 申込番号
    /// </summary>
    [JsonPropertyName("accountId")]
    public string AccountId { get; set; }

    /// <summary>
    /// 医療機関名
    /// </summary>
    [JsonPropertyName("accountName")]
    public string AccountName { get; set; }

    /// <summary>
    /// 住所
    /// </summary>
    [JsonPropertyName("accountAddress")]
    public string AccountAddress { get; set; }

    /// <summary>
    /// バックアップ容量（GB）
    /// </summary>
    [JsonPropertyName("backupCapa")]
    public int BackupCapa { get; set; }

    /// <summary>
    /// ベンダ番号
    /// </summary>
    [JsonPropertyName("vendorId")]
    public string VendorId { get; set; }

    /// <summary>
    /// 親ベンダ番号
    /// </summary>
    [JsonPropertyName("parentId")]
    public string ParentId { get; set; }
}

public class AccountRegistrationResponse
{
}
