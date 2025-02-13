using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Windows.Forms;
using CommonLibrary;
using static HttpClientService;

namespace BCP_Backup_Boot
{
    internal static class Program
    {
        private static readonly string deployDir = System.Configuration.ConfigurationManager.AppSettings["deployDir"];

        private static readonly string clientDir = System.Configuration.ConfigurationManager.AppSettings["clientDir"];
        private static readonly string clientPath = Path.Combine(clientDir, Constants.ClientExeName);

        private static readonly string serviceDir = System.Configuration.ConfigurationManager.AppSettings["serviceDir"];

        /// <summary>
        /// アプリケーションのメイン エントリ ポイントです。
        /// </summary>
        [STAThread]
        static void Main()
        {
            bool clientStartFlg = true;
            try
            {
                // 管理者権限で実行されているか確認
                if (!Configuration.IsAdministrator())
                {
                    MessageBox.Show(CommonLibrary.Message.Get(MessageKey.e00057));

                    // アプリケーションを終了し、終了コードとして95を渡す
                    Environment.Exit(95);
                }

                // 現在のプロセス名を取得
                string currentProcessName = Process.GetCurrentProcess().ProcessName;

                // 同じ名前のプロセスが他に存在するか確認
                var bootProcess
                    = Process.GetProcessesByName(currentProcessName).FirstOrDefault(p => p.Id != Process.GetCurrentProcess().Id);
                if (bootProcess != null)
                {
                    MessageBox.Show(CommonLibrary.Message.Get(MessageKey.i00023, new string[] { "BCP起動アプリ" }));
                    //Application.Exit();
                    // アプリケーションを終了し、終了コードとして90を渡す
                    Environment.Exit(90);
                }

                // 設定アプリが起動しているか確認
                var clientProcess = Process.GetProcessesByName(Constants.ClientProcessName).FirstOrDefault();
                if (clientProcess != null)
                {
                    MessageBox.Show(CommonLibrary.Message.Get(MessageKey.i00023, new string[] { "BCP設定アプリ" }));
                    //Application.Exit();
                    // アプリケーションを終了し、終了コードとして91を渡す
                    Environment.Exit(91);
                }

                var apiService = new APIService(new HttpClientService());

                //１．バージョンチェック
                var isLatestVersion = VersionCheck(apiService).Result;

                //２．最新ソフトダウンロード
                if (!isLatestVersion)
                {

                    // TODO 動きはあっているのか？
                    // 処理中（アップロード、ダウンロード中）の場合は処理を中断する
                    if (Configuration.ReadFromRegistry(Constants.Settings.ServiceStatus) == ServiceStatus.Processing)
                    {
                        MessageBox.Show(CommonLibrary.Message.Get(CommonLibrary.MessageKey.i00022));
                        return;
                    }
                    // バックアップサービス停止
                    if (!WindowsService.StopService(Constants.ServiceName))
                    {
                        MessageBox.Show(CommonLibrary.Message.Get(CommonLibrary.MessageKey.e00054,
                            new string[] { "バックアップサービス停止" }));
                        return;
                    }
                    clientStartFlg = DownloadLatestVersion(apiService).Result;

                    // バックアップサービス開始はBCP設定アプリ起動時 
                }
                if (clientStartFlg)
                {
                    //３．設定アプリ起動
                    var startInfo = new ProcessStartInfo
                    {
                        FileName = Path.GetFullPath(clientPath),  // 絶対パスにする必要がある
                        WorkingDirectory = Path.GetFullPath(clientDir),  // 絶対パスにする必要がある
                        CreateNoWindow = true,      // コンソール・ウィンドウを開かない
                        UseShellExecute = false     // シェル機能を使用しない
                    };
                    Process.Start(startInfo);
                }
                else
                {
                    MessageBox.Show(CommonLibrary.Message.Get(CommonLibrary.MessageKey.e00056,
                        new string[] { "BCP起動アプリ" }));
                }
            }
            catch (Exception e)
            {
                // イベントログのソースを設定
                if (!EventLog.SourceExists(BootConstants.eventSourceName))
                {
                    EventLog.CreateEventSource(BootConstants.eventSourceName, "Application");
                }
                EventLog.WriteEntry(BootConstants.eventSourceName, $"BCP起動アプリにエラーが発生しました: {e}", EventLogEntryType.Error);

                MessageBox.Show(CommonLibrary.Message.Get(CommonLibrary.MessageKey.e00001));
            }
        }

        private async static Task<Boolean> VersionCheck(APIService apiService)
        {
            //１－１．リクエストパラメータ①を設定し、「API01_01」WebAPIを呼び出す。
            var request = new SoftVersionCheckRequest
            {

                // TODO バージョンチェックはこれでいいのか？複数ファイルで構成されているはず
                //BCP端末ソフトバージョン BCP管理ソフトの実行ファイルに設定されているバージョン	
                SoftVersion = GetFileVersion(clientPath)
            };

            try
            {
                await apiService.SoftVersionCheckAsync(request);
            }
            catch (HttpRequestErrorException ex)
            {
                //（２） HTTPステータスが401の場合
                //「２．最新ソフトダウンロード」の処理へ
                if (ex.StatusCode == HttpStatusCode.Unauthorized)
                {
                    return false;
                }

                //（３） HTTPステータスが上記以外の場合
                else
                {
                    //エラーメッセージ「e00001」を表示し、BCP管理ソフトを終了する。
                    MessageBox.Show(CommonLibrary.Message.Get(MessageKey.e00001));

                    //TODO BCP管理ソフトを終了するはこれであっている？
                    //Application.Exit();
                    // アプリケーションを終了し、終了コードとして92を渡す
                    Environment.Exit(92);
                }

            }

            //１－２．応答結果を判定する。
            //（１） HTTPステータスが200（正常）の場合
            //【CL01_02：アカウント認証】の処理へ
            return true;
        }

        private static string GetFileVersion(string filePath)
        {
            if (!File.Exists(filePath))
            {
                return string.Empty;
            }

            return FileVersionInfo.GetVersionInfo(filePath).FileVersion;
        }

        private static async Task<Boolean> DownloadLatestVersion(APIService apiService)
        {
            var currentDirectory = Directory.GetCurrentDirectory();
            var zipPath = Path.Combine(currentDirectory, Path.GetRandomFileName() + ".zip");

            //１－１．リクエストパラメータ②を設定し、「API01_02」WebAPIを呼び出す。
            var request = new SoftDownloadRequest
            {
                AccountInfo = Configuration.ReadFromRegistry(Constants.Settings.AccountId)
            };

            try
            {
                await apiService.SoftDownloadAsync(request, zipPath);
            }
            catch (HttpRequestErrorException ex)
            {
                //（２） HTTPステータスが404の場合
                //エラーメッセージ「e00003」を表示し、BCP管理ソフトを終了する。
                if (ex.StatusCode == HttpStatusCode.NotFound)
                {
                    MessageBox.Show(CommonLibrary.Message.Get(MessageKey.e00003));
                    //Application.Exit();
                    // アプリケーションを終了し、終了コードとして93を渡す
                    Environment.Exit(93);
                }
                //（３） HTTPステータスが上記以外の場合
                else
                {
                    if (!EventLog.SourceExists(BootConstants.eventSourceName))
                    {
                        EventLog.CreateEventSource(BootConstants.eventSourceName, "Application");
                    }
                    EventLog.WriteEntry(BootConstants.eventSourceName, $"最新ソフトのダウンロード中にエラーが発生しました: {ex}", EventLogEntryType.Error);

                    //エラーメッセージ「e00001」を表示し、BCP管理ソフトを終了する。
                    MessageBox.Show(CommonLibrary.Message.Get(MessageKey.e00001));
                    //Application.Exit();
                    // アプリケーションを終了し、終了コードとして94を渡す
                    Environment.Exit(94);
                }
                return false;
            }

            EventLog.WriteEntry(BootConstants.eventSourceName, $"最新ソフトのアップデート {zipPath}", EventLogEntryType.Information);
            bool result = true;
            try
            {
                // 実行ファイルを強制削除する
                //ForcedDeleteFile(clientPath);
                //var servicePath = Path.Combine(serviceDir, Constants.ServiceName + ".exe");
                //ForcedDeleteFile(servicePath);
                // 解凍する
                //ExtractToDirectoryOverwrite(zipPath, deployDir);

                // ディレクトリを強制削除する
                ForcedDeleteDirectory(clientDir);
                ForcedDeleteDirectory(serviceDir);
                // Zipを解凍する
                ZipFile.ExtractToDirectory(zipPath, deployDir);
            }
            catch (Exception e) {
                if (!EventLog.SourceExists(BootConstants.eventSourceName))
                {
                    EventLog.CreateEventSource(BootConstants.eventSourceName, "Application");
                }
                EventLog.WriteEntry(BootConstants.eventSourceName, $"最新ソフトのアップデート中にエラーが発生しました: {e}", EventLogEntryType.Error);
                //エラーメッセージ「e00001」を表示し、BCP管理ソフトを終了する。
                MessageBox.Show(CommonLibrary.Message.Get(MessageKey.e00001));
                result = false;
            }
            finally {
                // zipは削除する
                File.Delete(zipPath);
            }

            //サーバ応答結果②のBCP管理ソフトで再起動する。
            return result;
        }

        private static void ExtractToDirectoryOverwrite(string zipPath, string extractPath)
        {
            using (ZipArchive archive = ZipFile.OpenRead(zipPath))
            {
                foreach (ZipArchiveEntry entry in archive.Entries)
                {
                    string destinationPath = Path.Combine(extractPath, entry.FullName);

                    // ディレクトリの場合はスキップ
                    if (string.IsNullOrEmpty(entry.Name))
                    {
                        Directory.CreateDirectory(destinationPath);
                        continue;
                    }

                    // 既存のファイルを上書き
                    entry.ExtractToFile(destinationPath, true);
                }
            }
        }
        /// <summary>
        /// フォルダを強制的に削除（ReadOnlyでも削除）
        /// </summary>
        /// <param name="dirPath">削除するフォルダ</param>
        private static void ForcedDeleteDirectory(string dirPath)
        {
            //DirectoryInfoオブジェクトの作成
            DirectoryInfo di = new DirectoryInfo(dirPath);
            if (di.Exists)
            {
                //フォルダ以下のすべてのファイル、フォルダの属性を削除
                RemoveReadonlyAttribute(di);

                //フォルダを根こそぎ削除
                di.Delete(true);
            }
        }

        private static void RemoveReadonlyAttribute(DirectoryInfo dirInfo)
        {
            //基のフォルダの属性を変更
            if ((dirInfo.Attributes & FileAttributes.ReadOnly) ==
                FileAttributes.ReadOnly)
                dirInfo.Attributes = FileAttributes.Normal;
            //フォルダ内のすべてのファイルの属性を変更
            foreach (FileInfo fi in dirInfo.GetFiles())
                if ((fi.Attributes & FileAttributes.ReadOnly) ==
                    FileAttributes.ReadOnly)
                    fi.Attributes = FileAttributes.Normal;
            //サブフォルダの属性を回帰的に変更
            foreach (DirectoryInfo di in dirInfo.GetDirectories())
                RemoveReadonlyAttribute(di);
        }
        /// <summary>
        /// ファイルを強制的に削除（ReadOnlyでも削除）
        /// </summary>
        /// <param name="filePath">削除するファイル</param>
        private static void ForcedDeleteFile(string filePath)
        {
            FileInfo fi = new FileInfo(filePath);
            if (fi.Exists)
            {
                if ((fi.Attributes & FileAttributes.ReadOnly) == FileAttributes.ReadOnly)
                {
                    fi.Attributes = FileAttributes.Normal;
                }
                fi.Delete();
            }
        }
    }
}
