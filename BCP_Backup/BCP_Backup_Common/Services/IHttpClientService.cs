using System;
using System.Threading.Tasks;

public interface IHttpClientService
{
    string GetLoginInfo(string url);
    Task<string> PostData(string url, string idToken, object data);
    Task<string> PostFile(string url, string idToken, object data, string path);
}
