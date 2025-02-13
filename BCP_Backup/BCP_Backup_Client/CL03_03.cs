using System;
using System.Drawing;
using System.IO;
using System.Reflection;
using System.Windows.Forms;
using CommonLibrary;

namespace BCP_Backup_Client
{
    public partial class CL03_03 : Form
    {
        public CL03_03()
        {
            InitializeComponent();
            this.ControlBox = false;
        }
        //ただ一つのフォームのインスタンスを保持するフィールド
        private static CL03_03 _instance;
        //ただ一つのフォームにアクセスするためのプロパティ
        public static CL03_03 Instance
        {
            get
            {
                //_instanceがnullまたは破棄されているときは、
                //新しくインスタンスを作成する
                if (_instance == null || _instance.IsDisposed)
                {
                    _instance = new CL03_03();
                }
                return _instance;
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void CL03_03_Load(object sender, EventArgs e)
        {
            string strConst = Configuration.ReadFromRegistry(Constants.Settings.AccountId);
            this.lblAccountId.Text = strConst ?? "";

            this.lblStatusOK.Visible = false;
            this.lblStatusNG.Visible = false;
            this.lblErrorInfo.Visible = false;
            strConst = Configuration.ReadFromRegistry(Constants.Settings.Status);
            switch (strConst ?? "")
            {
                case Constants.SyncStatus.UploadSuccess:
                    this.lblStatusOK.Text = "アップロード成功";
                    this.lblStatusOK.Visible = true;
                    this.pnlInfo.BackColor = Color.RoyalBlue;
                    break;
                case Constants.SyncStatus.DownloadSuccess:
                    this.lblStatusOK.Text = "ダウンロード成功";
                    this.lblStatusOK.Visible = true;
                    this.pnlInfo.BackColor = Color.RoyalBlue;
                    break;
                case Constants.SyncStatus.UploadFailed:
                    this.lblStatusNG.Text = "アップロード失敗";
                    this.lblStatusNG.Visible = true;
                    this.lblErrorInfo.Visible = true;
                    this.pnlInfo.BackColor = Color.Crimson;
                    break;
                case Constants.SyncStatus.DownloadFailed:
                    this.lblStatusNG.Text = "ダウンロード失敗";
                    this.lblStatusNG.Visible = true;
                    this.lblErrorInfo.Visible = true;
                    this.pnlInfo.BackColor = Color.Crimson;
                    break;
                case Constants.SyncStatus.SyncUnavailable:
                    this.lblStatusNG.Text = "同期不可";
                    this.lblStatusNG.Visible = true;
                    this.lblErrorInfo.Visible = true;
                    this.pnlInfo.BackColor = Color.Crimson;
                    break;
                default:
                    this.lblStatusOK.Text = "未実行";
                    this.lblStatusOK.Visible = true;
                    this.pnlInfo.BackColor = Color.DarkGray;
                    break;
            }
            strConst = Configuration.ReadFromRegistry(Constants.Settings.ErrorInfo);
            this.lblErrorInfo.Text = strConst ?? "";

            strConst = Configuration.ReadFromRegistry(Constants.Settings.LastDate);
            this.lblLastDate.Text = strConst ?? "";

            strConst = Configuration.ReadFromRegistry(Constants.Settings.UsedSize);
            double nUsedSize = (strConst == null) ? 0 : double.Parse(strConst);

            strConst = Configuration.ReadFromRegistry(Constants.Settings.BackupCapa);
            double nBackupCapa = (strConst == null) ? 0 : double.Parse(strConst);
            if (nBackupCapa != 0)
            {
                int nRate = (int)((nUsedSize / nBackupCapa) * 100);
                this.lblRate.Text = nRate.ToString() + "%使用";
                this.prgBarRate.Value = (nRate > 100)? 100 : nRate;
            }
            else
            {
                this.lblRate.Text = "0%";
                this.prgBarRate.Value = 0;
            }
            this.lblUsage.Text = nUsedSize.ToString() + "GB / " + nBackupCapa.ToString() + "GB";

            strConst = Configuration.ReadFromRegistry(Constants.Settings.UploadEnable);
            this.lblUploadEnable.Text = ((strConst ?? "").ToLower() == "true") ? "ON" : "OFF";
            if (this.lblUploadEnable.Text == "ON")
            {
                this.lblUploadTiming.Visible = true;
                this.picUploadTiming.Visible = true;

                strConst = Configuration.ReadFromRegistry(Constants.Settings.UploadTiming);
                this.lblUploadTiming.Text = (strConst ?? "‐") + "分";
            }
            else
            {
                this.lblUploadTiming.Visible = false;
                this.picUploadTiming.Visible = false;
            }
            strConst = Configuration.ReadFromRegistry(Constants.Settings.LocalDir);
            this.lblLocalDir.Text = strConst ?? "";
            // ディレクトリ存在チェック
            if (!Directory.Exists(this.lblLocalDir.Text))
            {
                this.lblLocalDir.ForeColor = Color.Red;
                //this.toolStripStatusLabel1.Text = "同期元ディレクトリが存在しません";
            }
            // 自分自身のバージョン情報を取得
            System.Reflection.Assembly assembly = Assembly.GetExecutingAssembly();
            System.Reflection.AssemblyName asmName = assembly.GetName();
            System.Version verSoft = asmName.Version;
            this.lblSoftVersion.Text = "ver." + verSoft.ToString();

            strConst = Configuration.ReadFromRegistry(Constants.Settings.ServiceStatus);
            switch (strConst ?? "")
            {
                case ServiceStatus.Waiting:
                    this.toolStripStatusLabel1.Text = "バックアップサービス：待機中";
                    break;
                case ServiceStatus.Processing:
                    this.toolStripStatusLabel1.Text = "バックアップサービス：処理中（アップロード、ダウンロード中）";
                    break;
                case ServiceStatus.Stopping:
                    this.toolStripStatusLabel1.Text = "バックアップサービス：停止中";
                    break;
            }

        }
    }
}
