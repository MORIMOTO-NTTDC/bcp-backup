using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using CommonLibrary;
using YamlDotNet.Core.Tokens;

public class HttpClientService : IHttpClientService
{
    private readonly HttpClient _httpClient;
    private const string JsonMediaType = "application/json";

    public HttpClientService()
    {
        _httpClient = new HttpClient();
    }

    public string GetLoginInfo(string url)
    {
        var response = _httpClient.GetAsync(url).Result;
        return HandleResponse(response);
    }

    private string HandleResponse(HttpResponseMessage response)
    {
        if (response.IsSuccessStatusCode)
        {
            var content = response.Content.ReadAsStringAsync().Result;
            return content;
        }
        else
        {
            var errorResponse = response.Content.ReadAsStringAsync().Result;

            // エラーメッセージが取得できない場合はステータスコードのみを返す
            if (string.IsNullOrEmpty(errorResponse))
            {
                throw new HttpRequestErrorException(response.ReasonPhrase, response.StatusCode);
            }

            // エラーメッセージが取得できる場合はエラーメッセージを返す
            try
            {
                var error = JsonSerializer.Deserialize<ErrorResponse>(errorResponse);
                throw new HttpRequestErrorException(error.Messages, response.StatusCode);
            }

            // エラーメッセージのデシリアライズに失敗した場合はステータスコードのみを返す
            catch (JsonException)
            {
                throw new HttpRequestErrorException(errorResponse, response.StatusCode);
            }
        }
    }

    public async Task<string> PostData(string url, string idToken, object data)
    {
        // IDトークンをAuthorizationヘッダーに追加
        if (string.IsNullOrEmpty(idToken))
        {
            _httpClient.DefaultRequestHeaders.Authorization = null;
        }
        else
        {
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", idToken);
        }

        var jsonData = JsonUtilty.ObjectToJson(data);
        var content = new StringContent(jsonData, System.Text.Encoding.UTF8, JsonMediaType);
        var response = await _httpClient.PostAsync(url, content);
        return HandleResponse(response);
    }

    public async Task<string> PostFile(string url, string idToken, object data, string path)
    {
        // IDトークンをAuthorizationヘッダーに追加
        if (string.IsNullOrEmpty(idToken))
        {
            _httpClient.DefaultRequestHeaders.Authorization = null;
        }
        else
        {
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", idToken);
        }

        var jsonData = JsonSerializer.Serialize(data);
        var content = new StringContent(jsonData, System.Text.Encoding.UTF8, JsonMediaType);
        var response = await _httpClient.PostAsync(url, content);

        if (response.IsSuccessStatusCode)
        {
            using (var stream = response.Content.ReadAsStreamAsync().Result)
            {
                using (var fileStream = System.IO.File.Create(path))
                {
                    stream.CopyTo(fileStream);
                }
            }

        }
        else
        {
            // 200以外のエラー処理はPostDataと共通
            return HandleResponse(response);
        }
        return null;
    }

    public class ErrorResponse
    {
        [JsonPropertyName("error_code")]
        public string ErrorCode { get; set; }

        [JsonPropertyName("messages")]
        public string Messages { get; set; }
    }

    public class HttpRequestErrorException : Exception
    {
        public HttpRequestErrorException(string message, System.Net.HttpStatusCode statusCode) : base(message)
        {
            StatusCode = statusCode;
        }

        public System.Net.HttpStatusCode StatusCode { get; }
    }
}
