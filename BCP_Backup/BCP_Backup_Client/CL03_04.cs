using BCP_Backup_Common.Services;
using CommonLibrary;
using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace BCP_Backup_Client
{
    public partial class CL03_04 : Form
    {
        public CL03_04()
        {
            InitializeComponent();
            this.ControlBox = false;
        }

        //ただ一つのフォームのインスタンスを保持するフィールド
        private static CL03_04 _instance;
        //ただ一つのフォームにアクセスするためのプロパティ
        public static CL03_04 Instance
        {
            get
            {
                //_instanceがnullまたは破棄されているときは、
                //新しくインスタンスを作成する
                if (_instance == null || _instance.IsDisposed)
                {
                    _instance = new CL03_04();
                }
                return _instance;
            }
        }
        private async void btnDownload_Click(object sender, EventArgs e)
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
                var logInfo = CommonLibrary.Message.Get(MessageKey.e00055, new string[] { "同期元ディレクトリ", "即時ダウンロード" });
                MessageBox.Show(logInfo + "\\n LocalDir:" + Configuration.ReadFromRegistry(Constants.Settings.LocalDir));
                return;
            }
            DialogResult result2 = MessageBox.Show(CommonLibrary.Message.Get(CommonLibrary.MessageKey.i00005, new string[] { "即時ダウンロードを実行" }), lblTitle.Text, MessageBoxButtons.OKCancel);
            if (result2 == System.Windows.Forms.DialogResult.Cancel)
            {
                return;
            }

            bool syncResult = true;
            // ダウンロードボタン等を無効にする
            btnCancel.Enabled = false;
            lblLocalDir.Enabled = false;
            label2.Enabled = false;
            label4.Enabled = false;

            // 処理中画像を表示
            picSpinner.Visible = true;
            toolStripStatusLabel1.Text = "即時ダウンロード中....";
#if !CLIENT_DEBUG
            // バックアップサービス停止
            if (!WindowsService.StopService(Constants.ServiceName))
            {
                MessageBox.Show(CommonLibrary.Message.Get(CommonLibrary.MessageKey.e00054,
                    new string[] { "バックアップサービス停止" }));
                //イベントログに出力する。
                EventLog.WriteEntry("BCP_Backup_Client", this.Name + ":" + CommonLibrary.Message.Get(CommonLibrary.MessageKey.e00054,
                    new string[] { "バックアップサービス停止" }), EventLogEntryType.Error);

                // ダウンロードボタンを有効にする
                btnDownload.Enabled = true;
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
                EventLog.WriteEntry("BCP_Backup_Client", this.Name + ":" + bucketName + "から即時ダウンロード（リストア）", EventLogEntryType.Information);

                //【CL99_01：AWS S3 同期】リストアを実行する
                await bizService.Restore();
            }
            catch (Exception ex)
            {
                syncResult = false;
                EventLog.WriteEntry("BCP_Backup_Client", this.Name + ":" + $"即時ダウンロード中にエラーが発生しました: {ex.ToString()}", EventLogEntryType.Error);

                MessageBox.Show(CommonLibrary.Message.Get(CommonLibrary.MessageKey.e00056, new string[] { "即時ダウンロード中" }));
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
            picSpinner.Visible = false;
            toolStripStatusLabel1.Text = string.Empty;
            // ダウンロードが正常の場合
            if (syncResult)
            {
                MessageBox.Show(CommonLibrary.Message.Get(CommonLibrary.MessageKey.i00006, new string[] { "即時ダウンロード" }));
            }
            // ダウンロードボタン等を有効にする
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

        private void CL03_04_Load(object sender, EventArgs e)
        {
            string strConst = Configuration.ReadFromRegistry(Constants.Settings.LocalDir);
            this.lblLocalDir.Text = strConst ?? "";
        }
    }
}
