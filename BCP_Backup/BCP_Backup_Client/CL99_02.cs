using CommonLibrary;
using System;
using System.Security.Cryptography;
using System.Text;
using System.Windows.Forms;
using System.Windows.Input;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace BCP_Backup_Client
{
    public partial class CL99_02 : Form
    {
        public int nextForm = 0;
        public CL99_02()
        {
            InitializeComponent();
            this.statusStrip1.Text = "";
            this.ControlBox = false;
        }

        //ただ一つのフォームのインスタンスを保持するフィールド
        private static CL99_02 _instance;
        //ただ一つのフォームにアクセスするためのプロパティ
        public static CL99_02 Instance
        {
            get
            {
                //_instanceがnullまたは破棄されているときは、
                //新しくインスタンスを作成する
                if (_instance == null || _instance.IsDisposed)
                {
                    _instance = new CL99_02();
                }
                return _instance;
            }
        }

        private void PermitPasswordCheck()
        {
            // 必須入力チェック
            this.toolStripStatusLabel1.Text = "";
            if (!ValidateUtility.IsRequire(textPassword.Text))
            {
                string[] p = new string[] { "許可パスワード" };
                this.toolStripStatusLabel1.Text = CommonLibrary.Message.Get(CommonLibrary.MessageKey.e00010, p);
                this.textPassword.BackColor = System.Drawing.Color.LightPink;
                this.textPassword.SelectAll();
                this.textPassword.Focus();
                return;
            }
            // 英数チェック
            if (!ValidateUtility.IsAlphaNumeric(textPassword.Text))
            {
                string[] p = new string[] { "許可パスワード" };
                this.toolStripStatusLabel1.Text = CommonLibrary.Message.Get(CommonLibrary.MessageKey.e00013, p);
                this.textPassword.BackColor = System.Drawing.Color.LightPink;
                this.textPassword.SelectAll();
                this.textPassword.Focus();
                return;
            }

            // SHA256 ハッシュ値を計算する
            byte[] beforeByteArray = Encoding.UTF8.GetBytes(textPassword.Text);
            SHA256 sha256 = SHA256.Create();
            byte[] afterByteArray = sha256.ComputeHash(beforeByteArray);
            sha256.Clear();
            // バイト配列を16進数文字列に変換
            StringBuilder sb2 = new StringBuilder();
            foreach (byte b in afterByteArray)
            {
                sb2.Append(b.ToString("x2"));
            }
            //            MessageBox.Show(sb2.ToString());

            // 実行許可パスワードはレジストリから取得
            var strPassPhase = Configuration.ReadFromRegistry(Constants.Settings.PermitPassword);

            // パスワードのチェック
            if (!sb2.ToString().Equals(strPassPhase))
            {
                string[] p = new string[] { "許可パスワード", "パスワード" };
                this.toolStripStatusLabel1.Text = CommonLibrary.Message.Get(CommonLibrary.MessageKey.e00039, p);
                this.textPassword.BackColor = System.Drawing.Color.LightPink;
                this.textPassword.SelectAll();
                this.textPassword.Focus();
                return;
            }
            this.textPassword.BackColor = System.Drawing.Color.White;
            this.toolStripStatusLabel1.Text = "";

            // 画面遷移
            switch (nextForm)
            {
                case 1: // アカウント設定変更
                    CL03_01 formCL03_01 = new CL03_01(); //インスタンスを作成
                    Program.applicationContext.MainForm = formCL03_01;
                    formCL03_01.Show();  //CL03_01を表示する
                    break;
                case 2: // 即時ダウンロード
                    CL03_04 formCL03_04 = new CL03_04(); //インスタンスを作成
                    Program.applicationContext.MainForm = formCL03_04;
                    formCL03_04.Show();  //CL03_04を表示する
                    break;
                default:
                    break;
            }
            this.Close();
        }
        private void BtnPwdCheck_Click(object sender, EventArgs e)
        {
            PermitPasswordCheck();
        }

        private void BtnCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void CL99_02_Load(object sender, EventArgs e)
        {
            textPassword.Text = string.Empty;
//            textPassword.Text = "abcd1234";
            
            toolStripStatusLabel1.Text = string.Empty;
        }

        private void TextPassword_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                PermitPasswordCheck();
            }
        }
    }
}
