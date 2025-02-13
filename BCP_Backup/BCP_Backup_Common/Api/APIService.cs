using System.Configuration;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

public class APIService
{
    private readonly IHttpClientService _httpClientService;
    // App.config‚©‚çAPI‚ÌURL‚ðŽæ“¾
    private readonly string apiUrl = ConfigurationManager.AppSettings["apiUrl"];

    public APIService(IHttpClientService httpClientService)
    {
        _httpClientService = httpClientService;
    }

    public async Task<AccountDetailResponse> GetAccountDetailAsync(string idToken, AccountDetailRequest request)
    {
        var response = await _httpClientService.PostData(apiUrl + ApiEndpoints.AccountDetail, idToken, request);
        return JsonSerializer.Deserialize<AccountDetailResponse>(response);
    }

    public async Task<SoftVersionCheckResponse> SoftVersionCheckAsync(SoftVersionCheckRequest request)
    {
        var response = await _httpClientService.PostData(apiUrl + ApiEndpoints.SoftVersionCheck, null, request);
        return new SoftVersionCheckResponse();
    }

    public async Task<AccountRegistrationResponse> AccountRegistrationAsync(string idToken, AccountRegistrationRequest request)
    {
        var response = await _httpClientService.PostData(apiUrl + ApiEndpoints.AccountRegistration, idToken, request);
        return JsonSerializer.Deserialize<AccountRegistrationResponse>(response);
    }

    public async Task<AccountUpdateResponse> AccountUpdateAsync(string idToken, AccountUpdateRequest request)
    {
        var response = await _httpClientService.PostData(apiUrl + ApiEndpoints.AccountUpdate, idToken, request);
        return JsonSerializer.Deserialize<AccountUpdateResponse>(response);
    }

    public async Task<AuthResponse> AuthAsync(string idToken, AuthRequest request)
    {
        var response = await _httpClientService.PostData(apiUrl + ApiEndpoints.Auth, idToken, request);
        return JsonSerializer.Deserialize<AuthResponse>(response);
    }

    public async Task<CapacityOverNotificationResponse> CapacityOverNotificationAsync(string idToken, CapacityOverNotificationRequest request)
    {
        var response = await _httpClientService.PostData(apiUrl + ApiEndpoints.CapacityOverNotification, idToken, request);
        return new CapacityOverNotificationResponse();
    }

    public async Task SoftDownloadAsync(SoftDownloadRequest request, string filePath)
    {
        var response = await _httpClientService.PostFile(apiUrl + ApiEndpoints.SoftDownload, null, request, filePath);
    }

    public async Task<SyncResultResponse> SyncResultAsync(string idToken, SyncResultRequest request)
    {
        await _httpClientService.PostData(apiUrl + ApiEndpoints.SyncResult, idToken, request);
        return new SyncResultResponse();
    }
}

