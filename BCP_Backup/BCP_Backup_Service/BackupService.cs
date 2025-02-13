using Amazon;
using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.Http.Headers;
using System.ServiceProcess;
using System.Timers;
using CommonLibrary;
using System.Security.Cryptography;
using System.Threading.Tasks;
using System.Net.Http;
using static HttpClientService;
using BCP_Backup_Common.Services;
using System.Globalization;
using System.Reflection;

namespace BCP_Backup_Service
{
    public partial class BackupService : ServiceBase
    {
        private readonly Timer timer = new Timer();
        private readonly S3SyncService s3SyncService;
        private readonly APIService apiService;
        private readonly CommonBizService bizService;
        private readonly string interval = System.Configuration.ConfigurationManager.AppSettings["interval"];

        public BackupService()
        {
            InitializeComponent();

            var bucketName = Configuration.ReadFromRegistry(Constants.Settings.S3Bucket);
            var accessKeyId = Configuration.ReadFromRegistry(Constants.Settings.AccessKeyId);
            var secretAccessKey = Configuration.ReadFromRegistry(Constants.Settings.SecretAccessKey);
            var sessionToken = Configuration.ReadFromRegistry(Constants.Settings.SessionToken);

            // TODO　リージョン固定？
            this.s3SyncService = new S3SyncService(bucketName, Amazon.RegionEndpoint.APNortheast1, accessKeyId, secretAccessKey, sessionToken);
            this.apiService = new APIService(new HttpClientService());
            this.bizService = new CommonBizService(s3SyncService, apiService);
        }

        public void StartDebug(string[] args) => OnStart(args);
        public void StopDebug() => OnStop();

        protected override void OnStart(string[] args)
        {
            try
            {
                // 実行ファイルのディレクトリを取得
                string exeDirectory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

                // カレントディレクトリを実行ファイルのディレクトリに設定
                Directory.SetCurrentDirectory(exeDirectory);

                // イベントログのソースを設定
                if (!EventLog.SourceExists("BCP_Backup_Service"))
                {
                    EventLog.CreateEventSource("BCP_Backup_Service", "Application");
                }

                // サービス開始時のログ
                EventLog.WriteEntry("BCP_Backup_Service", "サービスが開始されました。", EventLogEntryType.Information);

                // サービスステータスを待機中に更新
                Configuration.WriteToRegistry(Constants.Settings.ServiceStatus, ServiceStatus.Waiting);
                timer.Interval = double.Parse(this.interval);
                timer.Elapsed += new ElapsedEventHandler(this.OnTimer);
                timer.Start();
            }
            catch (Exception ex)
            {
                EventLog.WriteEntry("BCP_Backup_Service", $"サービスの開始中にエラーが発生しました: {ex.Message}", EventLogEntryType.Error);
                throw;
            }
        }

        protected override void OnStop()
        {
            timer.Stop();
            // サービスステータスを停止中に更新
            Configuration.WriteToRegistry(Constants.Settings.ServiceStatus, ServiceStatus.Stopping);

            // サービス停止時のログ
            EventLog.WriteEntry("BCP_Backup_Service", "サービスが停止されました。。。。", EventLogEntryType.Information);
        }

        public async void OnTimer(object sender, ElapsedEventArgs args)
        {
            // タイマを一時停止
            timer.Stop();

            // サービスステータスを処理中に更新
            // Configuration.WriteToRegistry(Constants.Settings.ServiceStatus, ServiceStatus.Processing);

            var bucketName = Configuration.ReadFromRegistry(Constants.Settings.S3Bucket);

            try
            {

                //１．アカウント設定情報取得
                var response = await GetAccountInfoAsync();

                // NULLの場合はエラーとして処理を終了
                if (response == null)
                {
                    return;
                }

                //２．ダウンロード（リストア）
                //２－１．応答結果の「ダウンロード実行可否」がON（true）の場合

                if (bool.Parse(response.DownloadEnable))
                {
                    // サービスステータスを処理中に更新
                    Configuration.WriteToRegistry(Constants.Settings.ServiceStatus, ServiceStatus.Processing);

                    // ダウンロード（リストア）時のログ
                    EventLog.WriteEntry("BCP_Backup_Service", bucketName + "からダウンロード（リストア）", EventLogEntryType.Information);

                    //・【CL02_03：ダウンロード】を実行する
                    await this.bizService.Restore();
                    return;
                }

                //３．アップロード（バックアップ）
                //３－１．応答結果の「アップロード実行可否」がON（true）でかつ、応答結果の「最終同期日時」が空文字、又は応答結果の「最終同期日時」（"yyyy/MM/DD HH24:mm:ss"形式）＋応答結果の「アップロード頻度」がシステム日時より小さい場合
                if (bool.Parse(response.UploadEnable) &&
                    (string.IsNullOrEmpty(response.LastDate) || DateTime.ParseExact(response.LastDate, "yyyy/MM/dd HH:mm:ss", CultureInfo.InvariantCulture).AddMinutes(int.Parse(response.UploadTiming)) < DateTime.Now))
                {
                    // サービスステータスを処理中に更新
                    Configuration.WriteToRegistry(Constants.Settings.ServiceStatus, ServiceStatus.Processing);

                    // アップロード（バックアップ）時のログ
                    EventLog.WriteEntry("BCP_Backup_Service", bucketName + "へアップロード（バックアップ）", EventLogEntryType.Information);

                    //・【CL99_01：AWS S3 同期】を実行する
                    await this.bizService.Backup();
                }
            }
            catch (Exception ex)
            {
                EventLog.WriteEntry("BCP_Backup_Service", $"アップロード／ダウンロード中にエラーが発生しました: {ex.ToString()}", EventLogEntryType.Error);
            }
            finally
            {
                // 最新のアカウント設定情報取得
                var response = await GetAccountInfoAsync();

                // サービスステータスを待機中に更新
                Configuration.WriteToRegistry(Constants.Settings.ServiceStatus, ServiceStatus.Waiting);

                // タイマを再開
                timer.Start();
            }
        }

        private async Task<AccountDetailResponse> GetAccountInfoAsync()
        {
            var request = new AccountDetailRequest();

            var accountId = Configuration.ReadFromRegistry(Constants.Settings.AccountId);

            request.AccountId = accountId;


            var idToken = Configuration.ReadFromRegistry(Constants.Settings.SessionToken);

            AccountDetailResponse response;

            //１－１．リクエストパラメータ①を設定し、「API01_04」WebAPIを呼び出す。
            try
            {
                 response = await apiService.GetAccountDetailAsync(idToken, request);
            }
            catch (HttpRequestErrorException ex)
            {
                //（２） HTTPステータスが404の場合
                if (ex.StatusCode == HttpStatusCode.NotFound)
                {
                    //エラーメッセージ「e00053」をログに出力する。
                    EventLog.WriteEntry("BCP_Backup_Service", Message.Get(MessageKey.e00053), EventLogEntryType.Error);

                    OnStop();

                    //レジストリを削除
                    Configuration.DeleteAllRegistry();

                    // 強制終了
                    Environment.Exit(9);
                }
                
                //（３） HTTPステータスが上記以外の場合
                else
                {
                    //エラーメッセージ「e00001」をログに出力する。
                    EventLog.WriteEntry("BCP_Backup_Service", Message.Get(MessageKey.e00001), EventLogEntryType.Error);
                }

                return null;

            }

            //１－２．応答結果を判定する。
            //（１） HTTPステータスが200（正常）の場合
            //        応答結果をレジストリに保存し、２の処理へ

            //1   アカウントID accountId                   取得した値のままをレジストリに保持する。
            //2   アップロード実行可否 uploadEnable                    取得した値のままをレジストリに保持する。
            //3   アップロード頻度 uploadTiming                    取得した値のままをレジストリに保持する。
            //4   バックアップ容量 backupCapa                  取得した値のままをレジストリに保持する。
            //5   使用容量 usedSize                    取得した値のままをレジストリに保持する。
            //6   ダウンロード実行可否 downloadEnable                  取得した値のままをメモリに保持する。
            //7   最終同期日時 lastdate                    取得した値のままをレジストリに保持する。
            //8   同期ステータス status                  取得した値のままをレジストリに保持する。
            //9   エラー情報 errorInfo                   取得した値のままをレジストリに保持する。
            //10  S3バケット s3backet                    取得した値のままをレジストリに保持する。
            //11  実行許可パスワード permitPassword                  取得した値のままをレジストリに保持する。
            //12  更新回数 version                 取得した値のままをレジストリに保持する。
            Configuration.WriteToRegistry(Constants.Settings.AccountId, response.AccountId);
            Configuration.WriteToRegistry(Constants.Settings.UploadEnable, string.IsNullOrEmpty(response.UploadEnable) ? "" : response.UploadEnable);
            Configuration.WriteToRegistry(Constants.Settings.UploadTiming, string.IsNullOrEmpty(response.UploadTiming) ? "" : response.UploadTiming);
            Configuration.WriteToRegistry(Constants.Settings.BackupCapa, string.IsNullOrEmpty(response.BackupCapa) ? "" : response.BackupCapa);
            Configuration.WriteToRegistry(Constants.Settings.UsedSize, string.IsNullOrEmpty(response.UsedSize) ? "" : response.UsedSize);
            //TODO CRUDになし
            Configuration.WriteToRegistry(Constants.Settings.DownloadEnable, string.IsNullOrEmpty(response.DownloadEnable) ? "" : response.DownloadEnable);
            Configuration.WriteToRegistry(Constants.Settings.LastDate, string.IsNullOrEmpty(response.LastDate) ? "" : response.LastDate);
            Configuration.WriteToRegistry(Constants.Settings.Status, string.IsNullOrEmpty(response.Status) ? "" : response.Status);
            Configuration.WriteToRegistry(Constants.Settings.ErrorInfo, string.IsNullOrEmpty(response.ErrorInfo) ? "" : response.ErrorInfo);
            Configuration.WriteToRegistry(Constants.Settings.S3Bucket, string.IsNullOrEmpty(response.S3Bucket) ? "" : response.S3Bucket);
            Configuration.WriteToRegistry(Constants.Settings.PermitPassword, string.IsNullOrEmpty(response.PermitPassword) ? "" : response.PermitPassword);
            Configuration.WriteToRegistry(Constants.Settings.UpdateCount, string.IsNullOrEmpty(response.Version) ? "" : response.Version);
            Configuration.WriteToRegistry(Constants.Settings.S3Endpoint, string.IsNullOrEmpty(response.S3Endpoint) ? "" : response.S3Endpoint);

            this.s3SyncService.SetBucketName(response.S3Bucket);

            return response;
        }

    }
}
