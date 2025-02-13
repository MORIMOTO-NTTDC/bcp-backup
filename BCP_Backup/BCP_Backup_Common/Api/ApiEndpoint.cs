using System;

public enum ApiEndpoint
{
    /// <summary>
    /// 端末ソフトバージョンチェック
    /// </summary>
    SoftVersionCheck,

    /// <summary>
    /// BCP管理ソフトダウンロード
    /// </summary>
    SoftDownload,

    /// <summary>
    /// アカウント認証
    /// </summary>
    Auth,

    /// <summary>
    /// アカウント設定情報取得
    /// </summary>
    AccountDetail,

    /// <summary>
    /// アカウント設定情報更新
    /// </summary>
    AccountUpdate,

    /// <summary>
    /// 容量オーバー通知
    /// </summary>
    CapacityOverNotification,

    /// <summary>
    /// 同期結果登録
    /// </summary>
    SyncResult,

    /// <summary>
    /// アカウント登録
    /// </summary>
    AccountRegistration
}

public static class ApiEndpointExtensions
{
    /// <summary>
    /// ApiEndpointの値に対応するエンドポイントURLを取得します。
    /// </summary>
    /// <param name="endpoint">ApiEndpointの値</param>
    /// <returns>エンドポイントURL</returns>
    public static string GetEndpoint(this ApiEndpoint endpoint)
    {
        switch (endpoint)
        {
            case ApiEndpoint.SoftVersionCheck:
                return ApiEndpoints.SoftVersionCheck;
            case ApiEndpoint.SoftDownload:
                return ApiEndpoints.SoftDownload;
            case ApiEndpoint.Auth:
                return ApiEndpoints.Auth;
            case ApiEndpoint.AccountDetail:
                return ApiEndpoints.AccountDetail;
            case ApiEndpoint.AccountUpdate:
                return ApiEndpoints.AccountUpdate;
            case ApiEndpoint.CapacityOverNotification:
                return ApiEndpoints.CapacityOverNotification;
            case ApiEndpoint.SyncResult:
                return ApiEndpoints.SyncResult;
            case ApiEndpoint.AccountRegistration:
                return ApiEndpoints.AccountRegistration;
            default:
                throw new ArgumentOutOfRangeException(nameof(endpoint), endpoint, null);
        }
    }
}
