using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Unicode;
using System.Threading.Tasks;

namespace BCP_Backup_Client
{
    public class WebAPIHttpClient
    {
        /// <summary>
        /// 通信先のベースURL
        /// </summary>
        private readonly string baseUrl;
        /// <summary>
        /// C#側のHttpクライアント
        /// </summary>
        private readonly HttpClient httpClient;
        /// <summary>
        /// レスポンスデータ（文字列）
        /// </summary>
        private string resBodyStr;

        /// <summary>
        /// デフォルトコンストラクタ。外部からは呼び出せません。
        /// </summary>
        private WebAPIHttpClient()
        {
        }

        /// <summary>
        /// 引数付きのコンストラクタ。こちらを使用します。
        /// 引数には正しいURLが入っていることが前提です。
        /// </summary>
        /// <param name="baseUrl">ベースのURL</param>
        public WebAPIHttpClient(string baseUrl)
        {
            this.baseUrl = baseUrl;
            // 通信するメソッドでその都度HttpClientをnewすると毎回ソケットを開いてリソースを消費するため、
            // メンバ変数で使い回す手法を取っています。
            this.httpClient = new HttpClient();
        }

        /// <summary>
        /// ボディに文字列のキーをJSONで持ってPOSTを送受信する。
        /// 正常時のレスポンスのBODYはresBodyStrに設定（GetResultBodyStrメソッドで取得）
        /// </summary>
        /// <param name="urlPath">ベースURL以降のURLパス</param>
        /// <param name="reqJsonParam">JSON形式のリクエストパラメータ</param>
        /// <returns>HTTPステータス</returns>
        public HttpStatusCode Post(string urlPath, string reqJsonParam)
        {
            String reqEndPoint = this.baseUrl + urlPath;
            var request = this.CreateRequest(HttpMethod.Post, reqEndPoint);
            var content = new StringContent(reqJsonParam, Encoding.UTF8, @"application/json");
            request.Content = content;

            // HTTP通信実行。メンバ変数でhttpClientを持っているので、using(～)で囲いません。
            // 囲うと通信後にオブジェクトが破棄されます。
            // 引数にrequestを取る場合はGetAsyncやPostAsyncでなくSendAsyncメソッドになります。
            // 戻り値はTask<HttpResponseMessage>で、変数名.ResultとするとSystem.Net.Http.HttpResponseMessageクラスが取れます。
            HttpStatusCode resStatusCoode = HttpStatusCode.NotFound;
            resBodyStr = string.Empty;
            Task<HttpResponseMessage> response;
            try
            {
                response = httpClient.SendAsync(request);
                resBodyStr = response.Result.Content.ReadAsStringAsync().Result;
                resStatusCoode = response.Result.StatusCode;
            }
            catch (HttpRequestException ex)
            {
                // UNDONE: 通信失敗のエラー処理
                // ログ出力
                Debug.WriteLine(ex);
                resBodyStr = "Error: " + ex.HResult.ToString("X") + " Message: " + ex.Message;
                return HttpStatusCode.ServiceUnavailable;
            }
            catch (Exception e)
            {
                Debug.WriteLine(e);
                resBodyStr = "Error: " + e.HResult.ToString("X") + " Message: " + e.Message;
                return HttpStatusCode.ServiceUnavailable;
            }
            return resStatusCoode;
        }

        /// <summary>
        /// Post正常時のレスポンスを取得
        /// </summary>
        /// <returns>レスポンスのボディ</returns>
        public string GetResultBodyStr()
        {
            return resBodyStr;
        }

        /// <summary>
        /// HTTPリクエストメッセージを生成する内部メソッドです。
        /// </summary>
        /// <param name="httpMethod">HTTPメソッドのオブジェクト</param>
        /// <param name="requestEndPoint">通信先のURL</param>
        /// <returns>HttpRequestMessage</returns>
        private HttpRequestMessage CreateRequest(HttpMethod httpMethod, string requestEndPoint)
        {
            var request = new HttpRequestMessage(httpMethod, requestEndPoint);
            return this.AddHeaders(request);
        }

        /// <summary>
        /// HTTPリクエストにヘッダーを追加する内部メソッドです。
        /// </summary>
        /// <param name="request">リクエスト</param>
        /// <returns>HttpRequestMessage</returns>
        private HttpRequestMessage AddHeaders(HttpRequestMessage request)
        {
            request.Headers.Add("Accept", "application/json");
            request.Headers.Add("Accept-Charset", "utf-8");
            // 同じようにして、例えば認証通過後のトークンが "Authorization: Bearer {トークンの文字列}"
            // のように必要なら適宜追加していきます。
            return request;
        }
    }
}
