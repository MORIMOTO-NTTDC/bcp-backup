using CommonLibrary;
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Reflection;
using System.Reflection.Emit;
using System.Text.Json;
using System.Windows.Forms;
using BCP_Backup_Common.Services;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Button;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.ToolBar;
using static HttpClientService;
using System.Threading.Tasks;
using System.Windows.Forms.Design;
using System.Text.Json.Serialization;


namespace BCP_Backup_Client
{
    public partial class CL01_02 : Form
    {
        /// <summary>
        /// デフォルトコンストラクタ。
        /// </summary>
        public CL01_02()
        {
            InitializeComponent();
            this.ControlBox = false;
        }
        //ただ一つのフォームのインスタンスを保持するフィールド
        private static CL01_02 _instance;
        //ただ一つのフォームにアクセスするためのプロパティ
        public static CL01_02 Instance
        {
            get
            {
                //_instanceがnullまたは破棄されているときは、新しくインスタンスを作成する
                if (_instance == null || _instance.IsDisposed)
                {
                    _instance = new CL01_02();
                }
                return _instance;
            }
        }

        /// <summary>
        /// Loadイベント
        /// </summary>
        private void CL01_02_Load(object sender, EventArgs e)
        {
            EventLog.WriteEntry("BCP_Backup_Client", this.Name + ":" + "設定アプリ開始", EventLogEntryType.Information);

            // レジストリの値でアカウント認証を実行
            if (AccountLogin(false, string.Empty, string.Empty, string.Empty))
            {
                // 認証OKならログインフラグをON、フォームを閉じる（終了）
                Program.loginFlg = true;
                this.Close();
            }
            // 認証NGならレジストリのアカウントID、同期元ディレクトリに設定
            this.textAccountId.Text = Configuration.ReadFromRegistry(Constants.Settings.AccountId);
            this.textAuthKey.Text = Configuration.ReadFromRegistry(Constants.Settings.AuthKey);
            this.textLocalDir.Text = Configuration.ReadFromRegistry(Constants.Settings.LocalDir);
            this.Activate();
        }

        /// <summary>
        /// 同期元ディレクトリ参照ボタンClickイベント
        /// </summary>
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

        /// <summary>
        /// パラメータのアカウントID等でWebAPIのアカウント認証（API01_03)を呼び出す。
        /// 画面表示有無がfalseの場合、レジストリのアカウントID等を使用する。
        /// 認証OKの場合、BCPバックアップサービスを開始する。
        /// </summary>
        /// <param name="shown">画面表示有無（true:表示あり）</param>
        /// <param name="accountId">アカウントID</param>
        /// <param name="authKey">認証キー</param>
        /// <param name="localDir">同期元ディレクトリ</param>
        /// <returns>認証結果（true:認証OK）</returns>
        /// </summary>
        private bool AccountLogin(bool shown, string accountId, string authKey, string localDir)
        {
            string regAccountId = string.Empty;
            string regAuthKey = string.Empty;
            // 画面表示なしの場合
            if (!shown)
            {
                // レジストリよりアカウントID、認証キーを取得
                regAccountId = Configuration.ReadFromRegistry(Constants.Settings.AccountId);
                regAuthKey = Configuration.ReadFromRegistry(Constants.Settings.AuthKey);
                if (regAccountId == null || regAccountId == string.Empty || regAuthKey == null || regAuthKey == string.Empty)
                {
                    // アカウントIDか認証キーが無ければ認証NGとして本処理を中断する
                    return false;
                }
                // BCPバックアップサービスが待機、処理中の場合は認証OKとして本処理を中断する
                if (Configuration.ReadFromRegistry(Constants.Settings.ServiceStatus) == ServiceStatus.Waiting ||
                    Configuration.ReadFromRegistry(Constants.Settings.ServiceStatus) == ServiceStatus.Processing)
                {
                    return true;
                }
            }

            // アカウント認証（/api01_auth.do）用のJOSN形式のリクエストパラメータを生成
            AuthRequest authRequest = new AuthRequest();
            if (shown)
            {
                // 画面表示ありの場合、パラメータの値を設定
                authRequest.AccountId = accountId;
                authRequest.AuthKey = authKey;
                authRequest.LocalDir = localDir;
            }
            else
            {
                // 画面表示なしの場合、レジストリの値を設定
                authRequest.AccountId = regAccountId;
                authRequest.AuthKey = regAuthKey;
                authRequest.LocalDir = Configuration.ReadFromRegistry(Constants.Settings.LocalDir);
            }
            // 設定アプリ（自分自身）のバージョン情報をBCP端末ソフトバージョンとして取得し設定
            System.Reflection.Assembly assembly = Assembly.GetExecutingAssembly();
            System.Reflection.AssemblyName asmName = assembly.GetName();
            System.Version verSoft = asmName.Version;
            authRequest.SoftVersion = verSoft.ToString();

            Debug.WriteLine(JsonUtilty.ObjectToJson(authRequest));

            toolStripStatusLabel1.Text = "アカウント認証中....";

            // 管理Webサーバにリクエスト送信
            HttpStatusCode res = Program.webApiService.Post(ApiEndpoints.Auth, string.Empty, authRequest);
            // HTTPステータスが200
            if (res.Equals(HttpStatusCode.OK))
            {
                Debug.WriteLine(Program.webApiService.GetResultBodyStr());

                AuthResponse authResponse = null;
                try
                {
                    authResponse = JsonSerializer.Deserialize<AuthResponse>(Program.webApiService.GetResultBodyStr());
                }
                catch (Exception ex)
                {
                    //例外をイベントログに出力する。
                    EventLog.WriteEntry("BCP_Backup_Client", this.Name + ":" + ex.Message + " responce:" + Program.webApiService.GetResultBodyStr(), EventLogEntryType.Error);
                    // ステータスに共通エラーメッセージを表示
                    toolStripStatusLabel1.Text = CommonLibrary.Message.Get(CommonLibrary.MessageKey.e00001);
                    return false;
                }

                if (shown)
                {
                    // 画面表示ありの場合、アカウント情報（名称）を確認メッセージに表示する
                    string[] p = new string[] { authRequest.AccountId, authResponse.AccountName };
                    DialogResult result = MessageBox.Show(CommonLibrary.Message.Get(CommonLibrary.MessageKey.i00020, p), lblTitle.Text, MessageBoxButtons.OKCancel);
                    if (result == System.Windows.Forms.DialogResult.Cancel)
                    {
                        // キャンセル押下の場合、認証NGとしてアカウント認証画面に戻る。
                        toolStripStatusLabel1.Text = string.Empty;
                        return false;
                    }
                    // 同期ステータスが未実行以外、確認メッセージに表示する
                    if (!authResponse.Status.Equals(CommonLibrary.Constants.SyncStatus.NotExecuted)) {
                        result = MessageBox.Show(CommonLibrary.Message.Get(CommonLibrary.MessageKey.i00026, p), lblTitle.Text, MessageBoxButtons.OKCancel);
                        if (result == System.Windows.Forms.DialogResult.Cancel)
                        {
                            // キャンセル押下の場合、認証NGとしてアカウント認証画面に戻る。
                            toolStripStatusLabel1.Text = string.Empty;
                            return false;
                        }
                    }
                }

                // アカウント設定情報取得
                bool ret = WebApiService.GetAccountDetail(this.Name, authRequest.AccountId, authResponse.SessionToken);
                if (ret)
                {
                    // リクエストパラメータ値とレスポンスデータ（サーバ応答結果）をレジストリに保存
                    Configuration.WriteToRegistry(Constants.Settings.AccountId, authRequest.AccountId);
                    Configuration.WriteToRegistry(Constants.Settings.AuthKey, authRequest.AuthKey);
                    Configuration.WriteToRegistry(Constants.Settings.LocalDir, authRequest.LocalDir);

                    Configuration.WriteToRegistry(Constants.Settings.AccountName, string.IsNullOrEmpty(authResponse.AccountName) ? "" : authResponse.AccountName);
                    Configuration.WriteToRegistry(Constants.Settings.AccessKeyId, string.IsNullOrEmpty(authResponse.AccessKeyId) ? "" : authResponse.AccessKeyId);
                    Configuration.WriteToRegistry(Constants.Settings.SecretAccessKey, string.IsNullOrEmpty(authResponse.SecretAccessKey) ? "" : authResponse.SecretAccessKey);
                    Configuration.WriteToRegistry(Constants.Settings.SessionToken, string.IsNullOrEmpty(authResponse.SessionToken) ? "" : authResponse.SessionToken);
#if !CLIENT_DEBUG
                    // バックアップサービス開始
                    if (!WindowsService.StartService(Constants.ServiceName))
                    {
                        // バックアップサービス開始に失敗した場合の処理
                        MessageBox.Show(CommonLibrary.Message.Get(CommonLibrary.MessageKey.e00054,
                            new string[] { "バックアップサービス開始" }));
                        //イベントログに出力する。
                        EventLog.WriteEntry("BCP_Backup_Client", this.Name + ":" + CommonLibrary.Message.Get(CommonLibrary.MessageKey.e00054,
                            new string[] { "バックアップサービス開始" }), EventLogEntryType.Error);
                        return false;
                    }
#endif
                    // 認証OKを返却
                    return true;
                }
                else
                {
                    // ステータスに共通エラーメッセージを表示
                    toolStripStatusLabel1.Text = CommonLibrary.Message.Get(CommonLibrary.MessageKey.e00001);
                }
            }
            else if (res.Equals(HttpStatusCode.NotFound))
            {
                // HTTPステータスが404（アカウントID誤り等）の場合、ステータスにエラーメッセージを表示
                toolStripStatusLabel1.Text = CommonLibrary.Message.Get(CommonLibrary.MessageKey.e00050);
                //イベントログに出力する。
                EventLog.WriteEntry("BCP_Backup_Client", this.Name + ":" + toolStripStatusLabel1.Text, EventLogEntryType.Error);
            }
            else if (res.Equals(HttpStatusCode.PreconditionFailed))
            {
                // HTTPステータスが412（Cognito認証情報誤り等）の場合、ステータスにエラーメッセージを表示
                toolStripStatusLabel1.Text = CommonLibrary.Message.Get(CommonLibrary.MessageKey.e00051);
                //イベントログに出力する。
                EventLog.WriteEntry("BCP_Backup_Client", this.Name + ":" + toolStripStatusLabel1.Text, EventLogEntryType.Error);
            }
            else if ((int)res == 480)
            {
                // HTTPステータスが480（Cognito認証エラー）の場合、ステータスにエラーメッセージを表示
                toolStripStatusLabel1.Text = CommonLibrary.Message.Get(CommonLibrary.MessageKey.e00052);
                //イベントログに出力する。
                EventLog.WriteEntry("BCP_Backup_Client", this.Name + ":" + toolStripStatusLabel1.Text, EventLogEntryType.Error);
            }
            else
            {
                // HTTPステータスが上記以外の場合、ステータスに共通エラーメッセージを表示
                toolStripStatusLabel1.Text = CommonLibrary.Message.Get(CommonLibrary.MessageKey.e00001);
                //イベントログに出力する。
                EventLog.WriteEntry("BCP_Backup_Client", this.Name + ":" + toolStripStatusLabel1.Text + " HTTPステータス:" + res.ToString(), EventLogEntryType.Error);
            }
            // 認証NGを返却
            return false;
        }

        private void BtnLogin_Click(object sender, EventArgs e)
        {
            // 入力チェック
            this.toolStripStatusLabel1.Text = string.Empty;
            // 申込番号のチェック
            if (!ValidateUtility.IsRequire(this.textAccountId.Text))
            {
                string[] p = new string[] { "申込番号" };
                this.toolStripStatusLabel1.Text = CommonLibrary.Message.Get(CommonLibrary.MessageKey.e00010, p);
                this.textAccountId.BackColor = System.Drawing.Color.LightPink;
                this.textAccountId.Focus();
                return;
            }
            if (!ValidateUtility.IsNumeric(this.textAccountId.Text))
            {
                string[] p = new string[] { "申込番号" };
                this.toolStripStatusLabel1.Text = CommonLibrary.Message.Get(CommonLibrary.MessageKey.e00011, p);
                this.textAccountId.BackColor = System.Drawing.Color.LightPink;
                this.textAccountId.Focus();
                return;
            }
            else
            {
                this.textAccountId.BackColor = System.Drawing.Color.White;
            }
            // 認証キーのチェック
            if (!ValidateUtility.IsRequire(this.textAuthKey.Text))
            {
                string[] p = new string[] { "認証キー" };
                this.toolStripStatusLabel1.Text = CommonLibrary.Message.Get(CommonLibrary.MessageKey.e00010, p);
                this.textAuthKey.BackColor = System.Drawing.Color.LightPink;
                this.textAuthKey.Focus();
                return;
            }
            if (!ValidateUtility.IsAlphaNumeric(this.textAuthKey.Text))
            {
                string[] p = new string[] { "認証キー" };
                this.toolStripStatusLabel1.Text = CommonLibrary.Message.Get(CommonLibrary.MessageKey.e00013, p);
                this.textAuthKey.BackColor = System.Drawing.Color.LightPink;
                this.textAuthKey.Focus();
                return;
            }
            else
            {
                this.textAccountId.BackColor = System.Drawing.Color.White;
            }
            // 同期元ディレクトリのチェック
            if (!ValidateUtility.IsRequire(this.textLocalDir.Text))
            {
                this.toolStripStatusLabel1.Text = CommonLibrary.Message.Get(CommonLibrary.MessageKey.e00010, new string[] { "同期元ディレクトリ" });
                this.textLocalDir.BackColor = System.Drawing.Color.LightPink;
                this.textLocalDir.Focus();
                return;
            }
            if (!ValidateUtility.IsAlphaNumericSymbol(this.textLocalDir.Text))
            {
                this.toolStripStatusLabel1.Text = CommonLibrary.Message.Get(CommonLibrary.MessageKey.e00014, new string[] { "同期元ディレクトリ" });
                this.textLocalDir.BackColor = System.Drawing.Color.LightPink;
                this.textLocalDir.Focus();
                return;
            }
            if (!ValidateUtility.IsValidFileName(this.textLocalDir.Text))
            {
                this.toolStripStatusLabel1.Text = CommonLibrary.Message.Get(CommonLibrary.MessageKey.e00039, new string[] { "同期元ディレクトリ", "ディレクトリ" });
                this.textLocalDir.BackColor = System.Drawing.Color.LightPink;
                this.textLocalDir.Focus();
                return;
            }
            if (!Directory.Exists(this.textLocalDir.Text))
            {
                this.toolStripStatusLabel1.Text = CommonLibrary.Message.Get(CommonLibrary.MessageKey.e00039, new string[] { "同期元ディレクトリ", "ディレクトリ" });
                this.textLocalDir.BackColor = System.Drawing.Color.LightPink;
                this.textLocalDir.Focus();
                return;
            }
            else
            {
                this.textLocalDir.BackColor = System.Drawing.Color.White;
            }

            // ログインボタンを無効にする
            btnLogin.Enabled = false;
            // アカウント認証（/api01_auth.do）
            if (AccountLogin(true, this.textAccountId.Text, this.textAuthKey.Text, this.textLocalDir.Text))
            {
                Program.loginFlg = true;
                this.Close();
            }
            this.textAccountId.SelectAll();
            this.textAccountId.Focus();
            // ログインボタンを有効に戻す
            btnLogin.Enabled = true;
        }

        private void BtnCancel_Click(object sender, EventArgs e)
        {
            EventLog.WriteEntry("BCP_Backup_Client", this.Name + ":" + "設定アプリ終了（ログイン中止）", EventLogEntryType.Warning);
            Program.loginFlg = false;
            this.Close();
        }
    }
}
