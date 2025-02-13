using BCP_Backup_Common.Services;
using CommonLibrary;
using System;
using System.Diagnostics;
using System.IO;
using System.Windows.Forms;
using System.Windows.Forms.Design;
using System.Threading.Tasks;

namespace BCP_Backup_Client
{
    public partial class CL03_02 : Form
    {
        public CL03_02()
        {
            InitializeComponent();
        }
        //ただ一つのフォームのインスタンスを保持するフィールド
        private static CL03_02 _instance;
        //ただ一つのフォームにアクセスするためのプロパティ
        public static CL03_02 Instance
        {
            get
            {
                //_instanceがnullまたは破棄されているときは、
                //新しくインスタンスを作成する
                if (_instance == null || _instance.IsDisposed)
                {
                    _instance = new CL03_02();
                }
                return _instance;
            }
        }

        private async void btnUpload_Click(object sender, EventArgs e)
        {
            // アカウントが存在しない
            if (!Configuration.ExistsInRegistry(Constants.Settings.AccountId))
            {
                MessageBox.Show(CommonLibrary.Message.Get(CommonLibrary.MessageKey.e00053));
                return;
            }
            // 処理中（アップロード、ダウンロード中）の場合は処理を中断する
            if (Configuration.ReadFromRegistry(Constants.Settings.ServiceStatus) == ServiceStatus.Processing)
            {
                MessageBox.Show(CommonLibrary.Message.Get(CommonLibrary.MessageKey.i00022));
                return;
            }
            // ディレクトリ存在チェック
            if (!Directory.Exists(Configuration.ReadFromRegistry(Constants.Settings.LocalDir)))
            {
                //エラーメッセージ「e00055」を出力する。
                var logInfo = CommonLibrary.Message.Get(MessageKey.e00055, new string[] { "同期元ディレクトリ", "即時アップロード" });
                MessageBox.Show(logInfo + "\n\n LocalDir:" + Configuration.ReadFromRegistry(Constants.Settings.LocalDir));
                return;
            }

            DialogResult result2 = MessageBox.Show(CommonLibrary.Message.Get(CommonLibrary.MessageKey.i00005, new string[] { "即時アップロードを実行" }), lblTitle.Text, MessageBoxButtons.OKCancel);
            if (result2 == System.Windows.Forms.DialogResult.Cancel)
            {
                return;
            }

            bool syncResult = true;
            // アップロードボタン等を無効にする
            btnUpload.Enabled = false;
            btnCancel.Enabled = false;
            lblLocalDir.Enabled = false;
            label2.Enabled = false;
            label4.Enabled = false;

            // 処理中画像を表示
            picSpinner.Visible = true;
            toolStripStatusLabel1.Text = "即時アップロード中....";
            /***
                        // レジストリ「BCP端末同期ディレクトリ」のディスク使用容量をGB（端数切り上げ）で取得する。
                        int usedSize = FolderUtility.GetLocalDirUsedSize(Configuration.ReadFromRegistry(Constants.Settings.LocalDir));
                        // 取得した使用容量とレジストリ「バックアップ容量」と比較する。
                        if (usedSize > int.Parse(Configuration.ReadFromRegistry(Constants.Settings.BackupCapa)))
                        {
                            // 使用容量がレジストリ「バックアップ容量」より大きい場合
                            MessageBox.Show(CommonLibrary.Message.Get(CommonLibrary.MessageKey.i00022));
                            return;
                        }
            ****/
#if !CLIENT_DEBUG
            // バックアップサービス停止
            if (!WindowsService.StopService(Constants.ServiceName))
            {
                MessageBox.Show(CommonLibrary.Message.Get(CommonLibrary.MessageKey.e00054,
                    new string[] { "バックアップサービス停止" }));
                //イベントログに出力する。
                EventLog.WriteEntry("BCP_Backup_Client", this.Name + ":" + CommonLibrary.Message.Get(CommonLibrary.MessageKey.e00054,
                    new string[] { "バックアップサービス停止" }), EventLogEntryType.Error);

                // アップロードボタンを有効にする
                btnUpload.Enabled = true;
                toolStripStatusLabel1.Text = string.Empty;
                return;
            }
            try
            {
                // サービスステータスを処理中に更新
                Configuration.WriteToRegistry(Constants.Settings.ServiceStatus, ServiceStatus.Processing);

                var bucketName = Configuration.ReadFromRegistry(Constants.Settings.S3Bucket);
                var accessKeyId = Configuration.ReadFromRegistry(Constants.Settings.AccessKeyId);
                var secretAccessKey = Configuration.ReadFromRegistry(Constants.Settings.SecretAccessKey);
                var sessionToken = Configuration.ReadFromRegistry(Constants.Settings.SessionToken);

                var s3SyncService = new S3SyncService(bucketName, Amazon.RegionEndpoint.APNortheast1, accessKeyId, secretAccessKey, sessionToken);
                var apiService = new APIService(new HttpClientService());
                var bizService = new CommonBizService(s3SyncService, apiService);

                //イベントログに出力する。
                EventLog.WriteEntry("BCP_Backup_Client", this.Name + ":" + bucketName + "へ即時アップロード（バックアップ）", EventLogEntryType.Information);

                //【CL99_01：AWS S3 同期】バックアップを実行する
                await bizService.Backup();
            }
            catch (Exception ex)
            {
                syncResult = false;
                EventLog.WriteEntry("BCP_Backup_Client", this.Name + ":" + $"即時アップロード中にエラーが発生しました: {ex.ToString()}", EventLogEntryType.Error);

                MessageBox.Show(CommonLibrary.Message.Get(CommonLibrary.MessageKey.e00056, new string[] { "即時アップロード中" }));
            }
            finally
            {
                // アカウント設定情報取得
                _ = WebApiService.GetAccountDetail(this.Name, Configuration.ReadFromRegistry(Constants.Settings.AccountId), Configuration.ReadFromRegistry(Constants.Settings.SessionToken));

                // サービスステータスを待機中に更新
                Configuration.WriteToRegistry(Constants.Settings.ServiceStatus, ServiceStatus.Waiting);

                // バックアップサービス開始
                if (!WindowsService.StartService(Constants.ServiceName))
                {
                    // バックアップサービス開始に失敗した場合の処理
                    MessageBox.Show(CommonLibrary.Message.Get(CommonLibrary.MessageKey.e00054,
                        new string[] { "バックアップサービス開始" }));
                    //イベントログに出力する。
                    EventLog.WriteEntry("BCP_Backup_Client", this.Name + ":" + CommonLibrary.Message.Get(CommonLibrary.MessageKey.e00054,
                        new string[] { "バックアップサービス開始" }), EventLogEntryType.Error);
                }
            }
#endif
#if CLIENT_DEBUG
            // System.Threading.Thread.Sleep(3000);
            await Task.Delay(3000);
#endif
            // 処理中画像を非表示
            toolStripStatusLabel1.Text = string.Empty;
            picSpinner.Visible = false;
            // アップロードが正常の場合
            if (syncResult)
            {
                MessageBox.Show(CommonLibrary.Message.Get(CommonLibrary.MessageKey.i00006, new string[] { "即時アップロード" }));
            }
            // アップロードボタン等を有効にする
            btnUpload.Enabled = true;
            btnCancel.Enabled = true;
            lblLocalDir.Enabled = true;
            label2.Enabled = true;
            label4.Enabled = true;
            this.Close();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void CL03_02_Load(object sender, EventArgs e)
        {
            string strConst = Configuration.ReadFromRegistry(Constants.Settings.LocalDir);
            this.lblLocalDir.Text = strConst ?? "";
        }
    }
}
