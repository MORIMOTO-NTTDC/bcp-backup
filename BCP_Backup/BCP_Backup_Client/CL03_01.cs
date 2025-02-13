using CommonLibrary;
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Text.Json;
using System.Windows.Forms;

namespace BCP_Backup_Client
{
    public partial class CL03_01 : Form
    {
        bool bActivated = false;
        // 更新回数
        private int accountUpdateCount = 0;

        public CL03_01()
        {
            InitializeComponent();
            this.ControlBox = false;
            this.toolStripStatusLabel1.Text = "";
            bActivated = false;
        }
        //ただ一つのフォームのインスタンスを保持するフィールド
        private static CL03_01 _instance;
        //ただ一つのフォームにアクセスするためのプロパティ
        public static CL03_01 Instance
        {
            get
            {
                //_instanceがnullまたは破棄されているときは、
                //新しくインスタンスを作成する
                if (_instance == null || _instance.IsDisposed)
                {
                    _instance = new CL03_01();
                }
                return _instance;
            }
        }

        private void CL03_01_Load(object sender, EventArgs e)
        {
            string strConst = Configuration.ReadFromRegistry(Constants.Settings.AccountId);
            this.lblAccountId.Text = strConst ?? "";

            strConst = Configuration.ReadFromRegistry(Constants.Settings.UploadEnable);
            this.cboxUploadEnable.Checked = (strConst == string.Empty) ? false : bool.Parse(strConst);

            strConst = Configuration.ReadFromRegistry(Constants.Settings.UploadTiming);

            RadioButton[] rbList = pnlUploadTiming.Controls.OfType<RadioButton>().ToArray();
            foreach (var cb in rbList)
            {
                if (cb.Text == strConst) { cb.Checked = true; break; }
            }

            strConst = Configuration.ReadFromRegistry(Constants.Settings.LocalDir);
            this.textLocalDir.Text = strConst ?? "";

            // 画面ロード時にレジストリから更新回数を取得
            strConst = Configuration.ReadFromRegistry(Constants.Settings.UpdateCount);
            accountUpdateCount = int.Parse(strConst ?? "0");

            //Activatedイベントハンドラの追加
            this.Activated += new EventHandler(CL03_01_Activated);
        }

        private void CL03_01_Activated(object sender, EventArgs e)
        {
            //CL03_01_Activatedが二度と呼び出されないようにする
            this.Activated -= new EventHandler(CL03_01_Activated);
            this.bActivated = true;
#if DEBUG
            Console.WriteLine("CL03_01が表示されました");
#endif
        }

        private void CboxUploadEnable_CheckedChanged(object sender, EventArgs e)
        {
            System.Windows.Forms.CheckBox chkBox = (System.Windows.Forms.CheckBox)sender;

            if (chkBox.Checked)
            {
                chkBox.BackColor = System.Drawing.Color.Blue;
                chkBox.ForeColor = System.Drawing.Color.White;
                chkBox.Text = "ON";
            }
            else
            {
                if (this.bActivated == false)
                {
                    chkBox.BackColor = System.Drawing.Color.Gray;
                    chkBox.ForeColor = System.Drawing.Color.White;
                    chkBox.Text = "OFF";
                }
                else
                {
                    DialogResult result = MessageBox.Show(CommonLibrary.Message.Get(CommonLibrary.MessageKey.i00013), lblTitle.Text, MessageBoxButtons.OKCancel);
                    if (result == System.Windows.Forms.DialogResult.Cancel)
                    {
                        chkBox.Checked = true;
                        return;
                    }
                    else
                    {
                        chkBox.BackColor = System.Drawing.Color.Gray;
                        chkBox.ForeColor = System.Drawing.Color.White;
                        chkBox.Text = "OFF";
                    }
                }
            }
        }

        private void BtnLocalDir_Click(object sender, EventArgs e)
        {
            // 画面を無効にする
            this.Enabled = false;

            // 現在の同期元ディレクトリの値をデフォルトとしてフォルダ選択ダイアログを表示
            var dlg = new FolderSelectDialog();
            dlg.Path = textLocalDir.Text;
            if (dlg.ShowDialog() == DialogResult.OK)
            {
                // 選択されたフォルダ名を同期元ディレクトリに設定
                textLocalDir.Text = dlg.Path;
            }

            // 画面をアクティブにし有効にする
            Program.applicationContext.MainForm.Activate();
            this.Enabled = true;
        }

        private void BtnRegist_Click(object sender, EventArgs e)
        {
            // アカウントが存在しない
            if (!Configuration.ExistsInRegistry(Constants.Settings.AccountId))
            {
                MessageBox.Show(CommonLibrary.Message.Get(CommonLibrary.MessageKey.e00053));
                return;
            }
            // 入力チェック
            // 同期元ディレクトリのチェック
            this.toolStripStatusLabel1.Text = string.Empty;
            if (!ValidateUtility.IsRequire(this.textLocalDir.Text))
            {
                this.toolStripStatusLabel1.Text = CommonLibrary.Message.Get(CommonLibrary.MessageKey.e00010, new string[] { "同期元ディレクトリ" });
                this.textLocalDir.BackColor = System.Drawing.Color.LightPink;
                return;
            }
            if (!ValidateUtility.IsAlphaNumericSymbol(this.textLocalDir.Text))
            {
                this.toolStripStatusLabel1.Text = CommonLibrary.Message.Get(CommonLibrary.MessageKey.e00014, new string[] { "同期元ディレクトリ" });
                this.textLocalDir.BackColor = System.Drawing.Color.LightPink;
                return;
            }
            if (!ValidateUtility.IsValidFileName(this.textLocalDir.Text))
            {
                this.toolStripStatusLabel1.Text = CommonLibrary.Message.Get(CommonLibrary.MessageKey.e00039, new string[] { "同期元ディレクトリ", "ディレクトリ" });
                this.textLocalDir.BackColor = System.Drawing.Color.LightPink;
                return;
            }
            if (!Directory.Exists(this.textLocalDir.Text))
            {
                this.toolStripStatusLabel1.Text = CommonLibrary.Message.Get(CommonLibrary.MessageKey.e00039, new string[] { "同期元ディレクトリ", "ディレクトリ" });
                this.textLocalDir.BackColor = System.Drawing.Color.LightPink;
                return;
            }
            else
            {
                this.textLocalDir.BackColor = System.Drawing.Color.White;
            }

            if (!cboxUploadEnable.Checked)
            {
                DialogResult result = MessageBox.Show(CommonLibrary.Message.Get(CommonLibrary.MessageKey.i00015), lblTitle.Text, MessageBoxButtons.OKCancel);
                if (result == System.Windows.Forms.DialogResult.Cancel)
                {
                    return;
                }
            }
            DialogResult result2 = MessageBox.Show(CommonLibrary.Message.Get(CommonLibrary.MessageKey.i00005, new string[] { "このアカウントを更新" }), lblTitle.Text, MessageBoxButtons.OKCancel);
            if (result2 == System.Windows.Forms.DialogResult.Cancel)
            {
                return;
            }

            // 処理中（アップロード、ダウンロード中）の場合は処理を中断する
            if (Configuration.ReadFromRegistry(Constants.Settings.ServiceStatus) == ServiceStatus.Processing)
            {
                MessageBox.Show(CommonLibrary.Message.Get(CommonLibrary.MessageKey.i00022), lblTitle.Text);
                return;
            }
#if !CLIENT_DEBUG
            // バックアップサービス停止
            if (!WindowsService.StopService(Constants.ServiceName))
            {
                MessageBox.Show(CommonLibrary.Message.Get(CommonLibrary.MessageKey.e00054, 
                    new string[] { "バックアップサービス停止" }), lblTitle.Text);
                //イベントログに出力する。
                EventLog.WriteEntry("BCP_Backup_Client", this.Name + ":" + CommonLibrary.Message.Get(CommonLibrary.MessageKey.e00054,
                    new string[] { "バックアップサービス停止" }), EventLogEntryType.Error);
                return;
            }
#endif
            try
            {
                // アカウント設定情報更新（/api01_update.do）
                // JOSN形式のリクエストパラメータの生成
                AccountUpdateRequest accountUpdateRequest = new AccountUpdateRequest();
                accountUpdateRequest.AccountId = this.lblAccountId.Text;
                accountUpdateRequest.UploadEnable = this.cboxUploadEnable.Checked;
                // 指定したグループ内のラジオボタンでチェックされている物を取り出す
                var RBChecked = this.pnlUploadTiming.Controls.OfType<RadioButton>().SingleOrDefault(rb => rb.Checked == true);
                if (RBChecked != null)
                {
                    accountUpdateRequest.UploadTiming = int.Parse(RBChecked.Text);
                }
                accountUpdateRequest.LocalDir = this.textLocalDir.Text;
                // 更新回数は画面ロード時に取得したバージョンを設定
                accountUpdateRequest.Version = accountUpdateCount;

                Debug.WriteLine(JsonUtilty.ObjectToJson(accountUpdateRequest));

                // 更新ボタンを無効にする
                this.Enabled = false;
                toolStripStatusLabel1.Text = "更新中....";
                // サーバにリクエスト送信
                //イベントログに出力する。
                EventLog.WriteEntry("BCP_Backup_Client", this.Name + ":アカウント情報の変更", EventLogEntryType.Information);

                // セッショントークンをレジストリから取得しヘッダに設定
                var strToken = Configuration.ReadFromRegistry(Constants.Settings.SessionToken);

                HttpStatusCode res = Program.webApiService.Post(ApiEndpoints.AccountUpdate, strToken, accountUpdateRequest);

                // 更新ボタンを有効に戻す
                this.Enabled = true;
                toolStripStatusLabel1.Text = string.Empty;

                if (res.Equals(HttpStatusCode.OK))  // 200
                {
                    MessageBox.Show(CommonLibrary.Message.Get(CommonLibrary.MessageKey.i00002), lblTitle.Text);

                    Debug.WriteLine(Program.webApiService.GetResultBodyStr());
                    Configuration.WriteToRegistry(Constants.Settings.UploadEnable, accountUpdateRequest.UploadEnable.ToString().ToLower());
                    Configuration.WriteToRegistry(Constants.Settings.UploadTiming, accountUpdateRequest.UploadTiming.ToString());
                    Configuration.WriteToRegistry(Constants.Settings.LocalDir, accountUpdateRequest.LocalDir);
                    this.Close();
                }
                else if (res.Equals(HttpStatusCode.PreconditionFailed)) // 412
                {
                    this.toolStripStatusLabel1.Text = CommonLibrary.Message.Get(CommonLibrary.MessageKey.e00037, new string[] { "更新" });
                    //イベントログに出力する。
                    EventLog.WriteEntry("BCP_Backup_Client", this.Name + ":" + this.toolStripStatusLabel1.Text, EventLogEntryType.Error);
                }
                else
                {
                    this.toolStripStatusLabel1.Text = CommonLibrary.Message.Get(CommonLibrary.MessageKey.e00001);
                    //イベントログに出力する。
                    EventLog.WriteEntry("BCP_Backup_Client", this.Name + ":" + this.toolStripStatusLabel1.Text, EventLogEntryType.Error);
                }
            }
            finally
            {
#if !CLIENT_DEBUG
                // バックアップサービス開始
                if (!WindowsService.StartService(Constants.ServiceName))
                {
                    // バックアップサービス開始に失敗した場合の処理
                    MessageBox.Show(CommonLibrary.Message.Get(CommonLibrary.MessageKey.e00054,
                        new string[] { "バックアップサービス開始" }), lblTitle.Text);
                    //イベントログに出力する。
                    EventLog.WriteEntry("BCP_Backup_Client", this.Name + ":" + CommonLibrary.Message.Get(CommonLibrary.MessageKey.e00054,
                        new string[] { "バックアップサービス開始" }), EventLogEntryType.Error);
                }
#endif
            }
        }
        private void BtnCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
