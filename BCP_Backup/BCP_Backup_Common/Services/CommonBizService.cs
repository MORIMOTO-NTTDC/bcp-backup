using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.Eventing.Reader;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Amazon.S3;
using CommonLibrary;
using YamlDotNet.Core.Tokens;
using static HttpClientService;

namespace BCP_Backup_Common.Services
{
    public class CommonBizService
    {
        private static readonly string clientDir = System.Configuration.ConfigurationManager.AppSettings["clientDir"];
        private static readonly string clientPath = Path.Combine(clientDir, Constants.ClientExeName);

        private static readonly string restore_display_name = "リストア";
        private static readonly string backup_display_name = "バックアップ";

        public CommonBizService(S3SyncService s3SyncService, APIService apiService) 
        {
            this.s3SyncService = s3SyncService;
            this.apiService = apiService;
        }

        private readonly S3SyncService s3SyncService;
        private readonly APIService apiService;

        public async Task Restore()
        {
            //１．ダウンロード
            //１－０．ディレクトリ存在チェック
            if (!Directory.Exists(Configuration.ReadFromRegistry(Constants.Settings.LocalDir)))
            {
                //エラーメッセージ「e00055」をログに出力する。
                var logInfo = Message.Get(MessageKey.e00055, new string[] { "同期元ディレクトリ", "ダウンロード" });
                EventLog.WriteEntry("BCP_Backup_Common", logInfo + " LocalDir:" + Configuration.ReadFromRegistry(Constants.Settings.LocalDir), EventLogEntryType.Error);

                // 同期不可で同期結果登録
                await SyncResultAsync(Constants.SyncStatus.SyncUnavailable, 0, logInfo);
                return;
            }

            string localDir;

            // 認証実施フラグ（認証できない場合は無限ループを抜けるため）
            var doesAuth = false;

            while (true)
            {

                //１－１．【CL99_01：AWS S3 同期】のパラメータを設定する。
                //No.パラメータ名 設定値
                //1   アクセスキーID CL01_02でメモリに保存したアクセスキーID
                //2   シークレットアクセスキー CL01_02でメモリに保存したシークレットアクセスキー
                //3   セッショントークン CL01_02でメモリに保存したセッショントークン
                //4   コピー元 レジストリ「S3バケット」とレジストリ「アカウントID」から生成したURI
                //5   コピー先 レジストリ「BCP端末同期ディレクトリ」
                var accessKeyId = Configuration.ReadFromRegistry(Constants.Settings.AccessKeyId);
                var secretAccessKey = Configuration.ReadFromRegistry(Constants.Settings.SecretAccessKey);
                var sessionToken = Configuration.ReadFromRegistry(Constants.Settings.SessionToken);
                var accountId = Configuration.ReadFromRegistry(Constants.Settings.AccountId);
                localDir = Configuration.ReadFromRegistry(Constants.Settings.LocalDir);

                this.s3SyncService.UpdateS3Client(accessKeyId, secretAccessKey, sessionToken);

                // S3処理計測
                var s3Stopwatch = new System.Diagnostics.Stopwatch();

                try
                {

                    // ・開始ログをイベントログへ出力する。
                    EventLog.WriteEntry("BCP_Backup_Common", Message.Get(MessageKey.i00024, new String[] { restore_display_name }), EventLogEntryType.Information);

                    // 処理計測開始
                    s3Stopwatch.Start();

                    //１－２．【CL99_01：AWS S3 同期】の処理を呼び出す。
                    await s3SyncService.RestoreFolderAsync(accountId, localDir);

                    // 正常終了した場合はループを抜ける
                    break;
                }
                catch (AmazonS3Exception e)
                {
                    // トークンが無効や期限切れ（provided token）の場合かつ認証が未実施の場合
                    if (e.Message.ToLower().Contains("provided token") & !doesAuth)
                    {
                        // 認証を実施してリトライする
                        var authResponse = await AuthAsync();
                        doesAuth = true;
                        continue;
                    }
                    else {
                        // ダウンロード不可で同期結果登録
                        await SyncResultAsync(Constants.SyncStatus.DownloadFailed, GetUsedSize(localDir), Configuration.ReadFromRegistry(Constants.Settings.ErrorInfo));
                        throw e;
                    }
                }
                catch (Exception e)
                {
                    // ダウンロード不可で同期結果登録
                    await SyncResultAsync(Constants.SyncStatus.DownloadFailed, GetUsedSize(localDir), Configuration.ReadFromRegistry(Constants.Settings.ErrorInfo));
                    throw e;
                }
                finally
                {
                    // 処理計測終了
                    s3Stopwatch.Stop();

                    // ・終了ログをイベントログへ出力する。
                    EventLog.WriteEntry("BCP_Backup_Common", Message.Get(MessageKey.i00025, new object[] { restore_display_name, s3Stopwatch.ElapsedMilliseconds }), EventLogEntryType.Information);

                }
            }

            //１－３．レジストリ「BCP端末同期ディレクトリ」のディスク使用容量をGB（端数切り上げ）で取得する。
            //２．ダウンロード成功で同期結果登録
            await SyncResultAsync(Constants.SyncStatus.DownloadSuccess, GetUsedSize(localDir));
        }

        private async Task<AuthResponse> AuthAsync()
        {
            var idToken = Configuration.ReadFromRegistry(Constants.Settings.SessionToken);

            var request = new AuthRequest()
            {
                AccountId = Configuration.ReadFromRegistry(Constants.Settings.AccountId),
                AuthKey = Configuration.ReadFromRegistry(Constants.Settings.AuthKey),
                SoftVersion = GetFileVersion(clientPath),
                LocalDir = Configuration.ReadFromRegistry(Constants.Settings.LocalDir)
            };
            //EventLog.WriteEntry("BCP_Backup_Common", $"Auth -----> {clientPath}:{GetFileVersion(clientPath)}", EventLogEntryType.Warning);

            AuthResponse response;

            // WebAPIを呼び出し、応答結果を判定する。
            try
            {
                response = await apiService.AuthAsync(idToken, request);

                //２－２．応答結果を判定する。
                //（１） HTTPステータスが200（正常）の場合
                //処理を終了する。
            }
            catch (HttpRequestErrorException ex)
            {

                // ②HTTPステータスが404の場合
                if (ex.StatusCode == HttpStatusCode.NotFound)
                {
                    // ・エラーメッセージ「e00050」をイベントログへ出力する。
                    EventLog.WriteEntry("BCP_Backup_Common", Message.Get(MessageKey.e00050), EventLogEntryType.Error);
                }
                // ③HTTPステータスが412の場合
                if (ex.StatusCode == HttpStatusCode.PreconditionFailed)
                {
                    // ・エラーメッセージ「e00051」をイベントログへ出力する。
                    EventLog.WriteEntry("BCP_Backup_Common", Message.Get(MessageKey.e00051), EventLogEntryType.Error);
                }
                // ④HTTPステータスが480の場合
                if (ex.StatusCode == (HttpStatusCode) 480)
                {
                    // ・エラーメッセージ「e00052」をイベントログへ出力する。
                    EventLog.WriteEntry("BCP_Backup_Common", Message.Get(MessageKey.e00052), EventLogEntryType.Error);
                }

                // ⑤HTTPステータスが上記以外の場合
                else
                {
                    // エラーメッセージ「e00001」をイベントログへ出力する。
                    EventLog.WriteEntry("BCP_Backup_Common", Message.Get(MessageKey.e00001), EventLogEntryType.Error);
                }

                throw ex;
            }

            Configuration.WriteToRegistry(Constants.Settings.AccountName, string.IsNullOrEmpty(response.AccountName) ? "" : response.AccountName);
            Configuration.WriteToRegistry(Constants.Settings.AccessKeyId, string.IsNullOrEmpty(response.AccessKeyId) ? "" : response.AccessKeyId);
            Configuration.WriteToRegistry(Constants.Settings.SecretAccessKey, string.IsNullOrEmpty(response.SecretAccessKey) ? "" : response.SecretAccessKey);
            Configuration.WriteToRegistry(Constants.Settings.SessionToken, string.IsNullOrEmpty(response.SessionToken) ? "" : response.SessionToken);

            //（１） HTTPステータスが200（正常）の場合
            //処理を終了する。
            return response;

        }
        private static string GetFileVersion(string filePath)
        {
            if (!File.Exists(filePath))
            {
                return string.Empty;
            }

            return FileVersionInfo.GetVersionInfo(filePath).FileVersion;
        }



        private async Task<SyncResultResponse> SyncResultAsync(string status, int usedSize, string errorInfo = "")
        {

            var idToken = Configuration.ReadFromRegistry(Constants.Settings.SessionToken);

            var request = new SyncResultRequest();
            request.AccountId = Configuration.ReadFromRegistry(Constants.Settings.AccountId); ;
            request.LastDate = DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss");
            request.Status = status;
            // 最大文字数が200文字のため、200文字を超える場合は切り捨てる
            //request.ErrorInfo = errorInfo.Length > 200 ? errorInfo.Substring(0, 200) : errorInfo;
            int offset = errorInfo.IndexOf("\r");
            string errorInfoString = errorInfo;
            if (offset > 0)
            {
                errorInfoString = errorInfo.Substring(0, offset);
            }
            request.ErrorInfo = errorInfoString.Length > 200 ? errorInfoString.Substring(0, 200) : errorInfoString;

            request.UsedSize = usedSize;

            SyncResultResponse response;

            //２－１．リクエストパラメータ①を設定し、「API01_07」WebAPIを呼び出す。
            try
            {
                response = await apiService.SyncResultAsync(idToken, request);

                //２－２．応答結果を判定する。
                //（１） HTTPステータスが200（正常）の場合
                //処理を終了する。
            }
            catch (HttpRequestErrorException ex)
            {

                //（２） HTTPステータスが404の場合
                if (ex.StatusCode == HttpStatusCode.NotFound)
                {
                    //エラーメッセージ「e00053」をログに出力する。
                    EventLog.WriteEntry("BCP_Backup_Common", Message.Get(MessageKey.e00053), EventLogEntryType.Error);
                }

                //（３） HTTPステータスが上記以外の場合
                else
                {
                    //エラーメッセージ「e00001」をログに出力する。
                    EventLog.WriteEntry("BCP_Backup_Common", Message.Get(MessageKey.e00001), EventLogEntryType.Error);
                }

                return null;

            }

            //（１） HTTPステータスが200（正常）の場合
            //処理を終了する。
            return response;
        }

        public async Task Backup()
        {
            //１．容量チェック
            //１－０．ディレクトリ存在チェック
            if (!Directory.Exists(Configuration.ReadFromRegistry(Constants.Settings.LocalDir)))
            {
                // エラーメッセージ「e00055」をログに出力する。
                var logInfo = Message.Get(MessageKey.e00055, new string[] { "同期元ディレクトリ", "アップロード" });
                EventLog.WriteEntry("BCP_Backup_Common", logInfo + " LocalDir:" + Configuration.ReadFromRegistry(Constants.Settings.LocalDir), EventLogEntryType.Error);

                // 同期不可で同期結果登録
                await SyncResultAsync(Constants.SyncStatus.SyncUnavailable, 0, logInfo);
                return;
            }

            //１－１．レジストリ「BCP端末同期ディレクトリ」のディスク使用容量をGB（端数切り上げ）で取得する。
            var usedSize = GetUsedSize(Configuration.ReadFromRegistry(Constants.Settings.LocalDir));

            //１－２．取得した使用容量とレジストリ「バックアップ容量」と比較する。
            //（１） 使用容量がレジストリ「バックアップ容量」より大きい場合
            //２の処理へ
            if (usedSize > int.Parse(Configuration.ReadFromRegistry(Constants.Settings.BackupCapa)))
            {
                //２．容量オーバー通知
                await CapacityOverNotificationAsync(usedSize);
                return;
            }

            //（２） 上記以外の場合
            //３の処理へ
            //３．アップロード
            string localDir;

            // 認証実施フラグ（認証できない場合は無限ループを抜けるため）
            var doesAuth = false;

            while (true)
            {

                //３－１．【CL99_01：AWS S3 同期】のパラメータを設定する。

                //No.パラメータ名 設定値
                //1 アクセスキーID CL01_02でメモリに保存したアクセスキーID
                //2 シークレットアクセスキー CL01_02でメモリに保存したアクセスキーID
                //3 セッショントークン CL01_02でメモリに保存したアクセスキーID
                //4 コピー元 レジストリ「BCP端末同期ディレクトリ」
                //5 コピー先 レジストリ「S3バケット」とレジストリ「アカウントID」から生成したURI
                var accessKeyId = Configuration.ReadFromRegistry(Constants.Settings.AccessKeyId);
                var secretAccessKey = Configuration.ReadFromRegistry(Constants.Settings.SecretAccessKey);
                var sessionToken = Configuration.ReadFromRegistry(Constants.Settings.SessionToken);
                var accountId = Configuration.ReadFromRegistry(Constants.Settings.AccountId);
                localDir = Configuration.ReadFromRegistry(Constants.Settings.LocalDir);

                this.s3SyncService.UpdateS3Client(accessKeyId, secretAccessKey, sessionToken);

                // S3処理計測
                var s3Stopwatch = new System.Diagnostics.Stopwatch();

                try
                {

                    // ・開始ログをイベントログへ出力する。
                    EventLog.WriteEntry("BCP_Backup_Service", Message.Get(MessageKey.i00024, new String[1] { backup_display_name }), EventLogEntryType.Information);

                    // 処理計測開始
                    s3Stopwatch.Start();

                    //３－２．【CL99_01：AWS S3 同期】の処理を呼び出す。
                    await s3SyncService.SyncFolderAsync(localDir, accountId);

                    // 正常終了した場合はループを抜ける
                    break;

                }
                catch (AmazonS3Exception e)
                {
                    // トークンが無効や期限切れ（provided token）の場合かつ認証が未実施の場合
                    if (e.Message.ToLower().Contains("provided token") & !doesAuth)
                    {
                        // 認証を実施してリトライする
                        var authResponse = await AuthAsync();
                        doesAuth = true;
                        continue;
                    }
                    else
                    {
                        // アップロード不可で同期結果登録
                        await SyncResultAsync(Constants.SyncStatus.UploadFailed, usedSize, Configuration.ReadFromRegistry(Constants.Settings.ErrorInfo));

                        throw e;
                    }
                }
                catch (Exception e)
                {
                    // アップロード不可で同期結果登録
                    await SyncResultAsync(Constants.SyncStatus.UploadFailed, usedSize, Configuration.ReadFromRegistry(Constants.Settings.ErrorInfo));

                    throw e;
                }
                finally
                {
                    // 処理計測終了
                    s3Stopwatch.Stop();

                    // ・終了ログをイベントログへ出力する。
                    EventLog.WriteEntry("BCP_Backup_Service", Message.Get(MessageKey.i00025, new object[] { backup_display_name, s3Stopwatch.ElapsedMilliseconds }), EventLogEntryType.Information);
                }
            }

            //４．アップロード成功で同期結果登録
            await SyncResultAsync(Constants.SyncStatus.UploadSuccess, usedSize);

        }
        private async Task<CapacityOverNotificationResponse> CapacityOverNotificationAsync(int usageSize)
        {

            var idToken = Configuration.ReadFromRegistry(Constants.Settings.SessionToken);

            var request = new CapacityOverNotificationRequest();
            request.AccountId = Configuration.ReadFromRegistry(Constants.Settings.AccountId);
            request.UsedSize = usageSize;

            CapacityOverNotificationResponse response;

            try
            {
                //２－１．リクエストパラメータ①を設定し、「API01_06」WebAPIを呼び出す。
                response = await apiService.CapacityOverNotificationAsync(idToken, request);

                //２－２．応答結果を判定する。
                //（１） HTTPステータスが200（正常）の場合
                //処理を終了する。
            }
            catch (HttpRequestErrorException ex)
            {

                //（２） HTTPステータスが404の場合
                if (ex.StatusCode == HttpStatusCode.NotFound)
                {
                    //エラーメッセージ「e00053」をログに出力する。
                    EventLog.WriteEntry("BCP_Backup_Common", Message.Get(MessageKey.e00053), EventLogEntryType.Error);
                }

                //（３） HTTPステータスが上記以外の場合
                else
                {
                    //エラーメッセージ「e00001」をログに出力する。
                    EventLog.WriteEntry("BCP_Backup_Common", Message.Get(MessageKey.e00001), EventLogEntryType.Error);
                }

                return null;

            }

            //（１） HTTPステータスが200（正常）の場合
            //処理を終了する。
            return response;
        }

        /// <summary>
        /// ディレクトリのディスク使用容量を取得します。
        /// </summary>
        /// <param name="dir">ディレクトリのパス</param>
        /// <returns>ディスク使用容量（GB）</returns>
        private int GetUsedSize(string dir)
        {
            long totalSize = 0;
            DirectoryInfo directory = new DirectoryInfo(dir);
            FileInfo[] files = directory.GetFiles("*.*", SearchOption.AllDirectories);
            foreach (FileInfo file in files)
            {
                totalSize += file.Length;
            }
            int diskUsage = (int)Math.Ceiling((double)totalSize / (1024 * 1024 * 1024));
            return diskUsage;
        }
    }
}
