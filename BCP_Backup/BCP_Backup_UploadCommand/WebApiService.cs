using CommonLibrary;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace BCP_Backup_Client
{
    internal class WebApiService
    {
        public static bool GetAccountDetail(string className, string accountId, String sessionToken)
        {
            var request = new AccountDetailRequest();
            request.AccountId = accountId;

            // 管理Webサーバにリクエスト送信
            HttpStatusCode res = Program.webApiService.Post(ApiEndpoints.AccountDetail, sessionToken, request);
            // HTTPステータスが200（正常）の場合
            if (res.Equals(HttpStatusCode.OK))
            {
                Debug.WriteLine(Program.webApiService.GetResultBodyStr());

                try
                {
                    AccountDetailResponse response = JsonSerializer.Deserialize<AccountDetailResponse>(Program.webApiService.GetResultBodyStr());

                    // 応答結果をレジストリに保存後、trueを返却
                    Configuration.WriteToRegistry(Constants.Settings.UploadEnable, string.IsNullOrEmpty(response.UploadEnable) ? "" : response.UploadEnable);
                    Configuration.WriteToRegistry(Constants.Settings.UploadTiming, string.IsNullOrEmpty(response.UploadTiming) ? "" : response.UploadTiming);
                    Configuration.WriteToRegistry(Constants.Settings.BackupCapa, string.IsNullOrEmpty(response.BackupCapa) ? "" : response.BackupCapa);
                    Configuration.WriteToRegistry(Constants.Settings.UsedSize, string.IsNullOrEmpty(response.UsedSize) ? "" : response.UsedSize);
                    Configuration.WriteToRegistry(Constants.Settings.DownloadEnable, string.IsNullOrEmpty(response.DownloadEnable) ? "" : response.DownloadEnable);
                    Configuration.WriteToRegistry(Constants.Settings.LastDate, string.IsNullOrEmpty(response.LastDate) ? "" : response.LastDate);
                    Configuration.WriteToRegistry(Constants.Settings.Status, string.IsNullOrEmpty(response.Status) ? "" : response.Status);
                    Configuration.WriteToRegistry(Constants.Settings.ErrorInfo, string.IsNullOrEmpty(response.ErrorInfo) ? "" : response.ErrorInfo);
                    Configuration.WriteToRegistry(Constants.Settings.S3Bucket, string.IsNullOrEmpty(response.S3Bucket) ? "" : response.S3Bucket);
                    Configuration.WriteToRegistry(Constants.Settings.PermitPassword, string.IsNullOrEmpty(response.PermitPassword) ? "" : response.PermitPassword);
                    Configuration.WriteToRegistry(Constants.Settings.UpdateCount, string.IsNullOrEmpty(response.Version) ? "" : response.Version);
                    return true;
                }
                catch (Exception ex)
                {
                    // 例外をイベントログに出力する。
                    EventLog.WriteEntry("BCP_Backup_Client", className + ":" + ex.Message + " responce:" + Program.webApiService.GetResultBodyStr(), EventLogEntryType.Error);
                }
            }
            // HTTPステータスが上記以外の場合、falseを返却
            return false;
        }
    }
}
