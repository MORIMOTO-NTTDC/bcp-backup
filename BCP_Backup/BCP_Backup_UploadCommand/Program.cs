using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Threading.Tasks;
using BCP_Backup_Common.Services;
using CommonLibrary;
using static HttpClientService;

// コンソール・アプリケーション名
//  bcpbackup.exe

// 前提条件
// ・BCP管理ソフトがインストールされていること
// ・BCP管理ソフトが起動されアカウント認証されていること
namespace BCP_Backup_UploadCommand
{
    internal class Program
    {
        private static readonly S3SyncService s3SyncService;
        private static readonly APIService apiService;
        private static readonly CommonBizService bizService;

        static Program()
        {
            var bucketName = Configuration.ReadFromRegistry(Constants.Settings.S3Bucket);
            var accessKeyId = Configuration.ReadFromRegistry(Constants.Settings.AccessKeyId);
            var secretAccessKey = Configuration.ReadFromRegistry(Constants.Settings.SecretAccessKey);
            var sessionToken = Configuration.ReadFromRegistry(Constants.Settings.SessionToken);

            apiService = new APIService(new HttpClientService());
            s3SyncService = new S3SyncService(bucketName, Amazon.RegionEndpoint.APNortheast1, accessKeyId, secretAccessKey, sessionToken);
            bizService = new CommonBizService(s3SyncService, apiService);
        }

        static async Task Main(string[] args)
        {
            // 実行権限が管理者権限であるか確認
            LogWithTimestamp("管理者権限確認");
            if (!IsAdministrator())
            {
                LogWithTimestamp("  ... NG");
                // 終了コードとして“7”を返して終了。
                Environment.Exit(7);
            }
            LogWithTimestamp("  ... OK");

            // ① レジストリキーにアカウントIDが無い場合、終了コードとして“1”を返して終了。
            LogWithTimestamp("アカウントID確認");
            if (!Configuration.ExistsInRegistry(Constants.Settings.AccountId))
            {
                LogWithTimestamp("  ... NG");
                Environment.Exit(1);
            }
            // レジストリにアカウントIDが空の場合、終了コードとして“1”を返して終了。
            if (Configuration.ReadFromRegistry(Constants.Settings.AccountId) == string.Empty)
            {
                LogWithTimestamp("  ... NG");
                Environment.Exit(1);
            }
            LogWithTimestamp("  ... OK");

            // ② バックアップサービス等でアップロード、ダウンロードが処理中の場合、
            // 　終了コードとして“2”を返して終了。
            LogWithTimestamp("アップロード、ダウンロード処理中確認");
            var beforeUploadServiceStatus = Configuration.ReadFromRegistry(Constants.Settings.ServiceStatus);
            if (beforeUploadServiceStatus == ServiceStatus.Processing)
            {
                LogWithTimestamp("  ... NG");
                Environment.Exit(2);
            }
            LogWithTimestamp("  ... OK");

            // ③ レジストリに設定された同期元ローカルディレクトリが存在しない場合、
            // 　終了コードとして“3”を返して終了。
            LogWithTimestamp("同期元ローカルディレクトリ確認");
            if (!Directory.Exists(Configuration.ReadFromRegistry(Constants.Settings.LocalDir)))
            {
                LogWithTimestamp("  ... NG");
                Environment.Exit(3);
            }
            LogWithTimestamp("  ... OK");

            // ④ 同期元ローカルディレクトリのディスク使用容量が当該アカウントのバックアップ
            // 　容量より大きい場合、終了コードとして“4”を返して終了。
            LogWithTimestamp("ディスク使用容量確認");
            int usedSize = GetLocalDirUsedSize(Configuration.ReadFromRegistry(Constants.Settings.LocalDir));
            // 取得した使用容量とレジストリ「バックアップ容量」と比較する。
            if (usedSize > int.Parse(Configuration.ReadFromRegistry(Constants.Settings.BackupCapa)))
            {
                // 使用容量がレジストリ「バックアップ容量」より大きい場合
                LogWithTimestamp("  ... NG");
                Environment.Exit(4);
            }
            LogWithTimestamp("  ... OK");

            // ⑤ BCPバックアップサービスを停止する。
            if (beforeUploadServiceStatus == ServiceStatus.Waiting) {
                LogWithTimestamp("BCPバックアップサービスを停止");
#if !DEBUG
                if (!WindowsService.StopService(Constants.ServiceName))
                {
                    // 終了コードとして“5”を返して終了。
                    Environment.Exit(5);
                }
#endif
            }
            var endcode = 0;
            try
            {
                // サービスステータスを処理中に更新
                Configuration.WriteToRegistry(Constants.Settings.ServiceStatus, ServiceStatus.Processing);


                //【CL99_01：AWS S3 同期】バックアップを実行する
                // ⑦ BCPバックアップサービスを開始する。
                // ⑥ S3へアップロードを実行し、当該アカウントの同期結果を管理Webサーバへ登録する。
                LogWithTimestamp("アップロード開始");
#if DEBUG
                await Task.Delay(1000 * 5);
#endif
#if !DEBUG
                await bizService.Backup();
#endif
                LogWithTimestamp("アップロード終了");
            }

            // ⑧ アップロードが正常な場合は終了コードとして“0”を、異常な場合は“9”を返して終了。
            catch (Exception ex)
            {
                LogWithTimestamp($"バックアップ処理異常終了:{ex}");
                endcode = 9;
                //Environment.Exit(9);
            }
            finally
            {
                LogWithTimestamp("アカウント設定情報取得");
                try
                {
                    // アカウント設定情報取得
                    await GetAccountInfoAsync();
                }
                catch (Exception exx)
                {
                    LogWithTimestamp($"  ... NG:{exx}");
                }

                if (beforeUploadServiceStatus == ServiceStatus.Waiting)
                {
                    // サービスステータスを待機中に更新
                    Configuration.WriteToRegistry(Constants.Settings.ServiceStatus, ServiceStatus.Waiting);
                    LogWithTimestamp("BCPバックアップサービスを開始");
#if !DEBUG
                    // バックアップサービス開始
                    if (!WindowsService.StartService(Constants.ServiceName))
                    {
                        // 終了コードとして“6”を返して終了。
                        endcode = 6;
                        //Environment.Exit(6);
                    }
#endif
                } else
                {
                    Configuration.WriteToRegistry(Constants.Settings.ServiceStatus, ServiceStatus.Stopping);
                }
            }

            Environment.Exit(endcode);
        }

        /// <summary>
        ///     ログ出力（タイムスタンプ付き）
        /// </summary>
        /// <param name="message">メッセージ</param>
        private static void LogWithTimestamp(string message)
        {
            string timestamp = DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss.fff");
            Console.WriteLine($"[{timestamp}] {message}");
        }

        /// <summary>
        ///     管理者権限で実行されているか確認します。
        /// </summary>
        /// <returns>確認結果</returns>
        private static bool IsAdministrator()
        {
            var identity = System.Security.Principal.WindowsIdentity.GetCurrent();
            var principal = new System.Security.Principal.WindowsPrincipal(identity);
            return principal.IsInRole(System.Security.Principal.WindowsBuiltInRole.Administrator);
        }

        /// <summary>
        /// フォルダ内のサブフォルダすべてのファイルサイズを取得します。
        /// <param name="dirInfo">フォルダ情報</param>
        /// <returns>ファイルサイズ（Byte）</returns>
        /// </summary>
        private static double GetDirectorySize(DirectoryInfo dirInfo)
        {
            double DirectorySize = 0;
            //フォルダ内の全ファイルのサイズを加算
            foreach (FileInfo fi in dirInfo.GetFiles())
                DirectorySize += fi.Length;
            //サブフォルダのサイズを合算
            foreach (DirectoryInfo di in dirInfo.GetDirectories())
                DirectorySize += GetDirectorySize(di);
            return DirectorySize;
        }

        /// <summary>
        /// ディレクトリのディスク使用容量をGB単位で取得します。
        /// </summary>
        /// <param name="dir">ディレクトリのパス</param>
        /// <returns>ディスク使用容量（GB）</returns>
        private static int GetLocalDirUsedSize(string Path)
        {
            double FolderSize = GetDirectorySize(new DirectoryInfo(Path));
            int diskUsage = (int)Math.Ceiling((double)FolderSize / (1024 * 1024 * 1024));
            return diskUsage;
        }

        private static async Task<AccountDetailResponse> GetAccountInfoAsync()
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

                    //レジストリを削除
                    Configuration.DeleteAllRegistry();
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

            //s3SyncService.SetBucketName(response.S3Bucket);

            return response;
        }

    }
}
