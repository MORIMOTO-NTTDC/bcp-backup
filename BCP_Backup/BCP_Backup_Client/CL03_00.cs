using System;
using System.Windows.Forms;
using CommonLibrary;
using System.Diagnostics;

namespace BCP_Backup_Client
{
    public partial class CL03_00 : Form
    {
        public CL03_00()
        {
            InitializeComponent();
        }

        private void CL03_00_Load(object sender, EventArgs e)
        {
            this.Visible = false;
        }

        private void 状況確認ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            /* 二重起動防止 */
            if (Program.applicationContext.MainForm == null || Program.applicationContext.MainForm.IsDisposed)
            {
                if (Configuration.ExistsInRegistry(Constants.Settings.AccountId))
                {
                    CL03_03 formCL03_03 = new CL03_03(); //インスタンスを作成
                    Program.applicationContext.MainForm = formCL03_03;
                    Program.applicationContext.MainForm.Show();  //CL03_03を表示する
                }
                else
                {
#if !CLIENT_DEBUG
                    // バックアップサービス停止
                    if (!WindowsService.StopService(Constants.ServiceName))
                    {
                        //イベントログに出力する。
                        EventLog.WriteEntry("BCP_Backup_Client", CommonLibrary.Message.Get(CommonLibrary.MessageKey.e00054,
                            new string[] { "バックアップサービス停止" }), EventLogEntryType.Error);
                    }
#endif
                    MessageBox.Show(CommonLibrary.Message.Get(CommonLibrary.MessageKey.e00053) + "アプリを終了します");
                    EventLog.WriteEntry("BCP_Backup_Client", this.Name + ":" + "設定アプリ終了（アカウント無し）", EventLogEntryType.Warning);
                    notifyIcon1.Visible = false;    //アイコンをトレイから取り除く
                    notifyIcon1.Dispose();
                    Application.Exit();             //アプリケーションの終了
                }
            }
            else
            {
                if (!Program.applicationContext.MainForm.Visible)
                {
                    Program.applicationContext.MainForm.Show();
                }
                else
                {
                    Program.applicationContext.MainForm.Activate();
                }
            }
        }

        private void アカウント設定ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            /* 二重起動防止 */
            if (Program.applicationContext.MainForm == null || Program.applicationContext.MainForm.IsDisposed)
            {
                // アカウントが存在する
                if (Configuration.ExistsInRegistry(Constants.Settings.AccountId))
                {
                    CL99_02 formCL99_02 = new CL99_02(); //インスタンスを作成
                    formCL99_02.nextForm = 1;
                    Program.applicationContext.MainForm = formCL99_02;
                    Program.applicationContext.MainForm.Show();  //CL99_02を表示する
                }
                else
                {
#if !CLIENT_DEBUG
                    // バックアップサービス停止
                    if (!WindowsService.StopService(Constants.ServiceName))
                    {
                        //イベントログに出力する。
                        EventLog.WriteEntry("BCP_Backup_Client", CommonLibrary.Message.Get(CommonLibrary.MessageKey.e00054,
                            new string[] { "バックアップサービス停止" }), EventLogEntryType.Error);
                    }
#endif
                    MessageBox.Show(CommonLibrary.Message.Get(CommonLibrary.MessageKey.e00053) + "アプリを終了します");
                    EventLog.WriteEntry("BCP_Backup_Client", this.Name + ":" + "設定アプリ終了（アカウント無し）", EventLogEntryType.Warning);
                    notifyIcon1.Visible = false;    //アイコンをトレイから取り除く
                    notifyIcon1.Dispose();
                    Application.Exit();             //アプリケーションの終了
                }
            }
            else
            {
                if (!Program.applicationContext.MainForm.Visible)
                {
                    Program.applicationContext.MainForm.Show();
                }
                else
                {
                    Program.applicationContext.MainForm.Activate();
                }
            }
        }
        private void 即時アップロードToolStripMenuItem_Click(object sender, EventArgs e)
        {
            /* 二重起動防止 */
            if (Program.applicationContext.MainForm == null || Program.applicationContext.MainForm.IsDisposed)
            {
                if (Configuration.ExistsInRegistry(Constants.Settings.AccountId))
                {
                    CL03_02 formCL03_02 = new CL03_02(); //インスタンスを作成
                    Program.applicationContext.MainForm = formCL03_02;
                    Program.applicationContext.MainForm.Show();  //CL03_02を表示する
                }
                else
                {
#if !CLIENT_DEBUG
                    // バックアップサービス停止
                    if (!WindowsService.StopService(Constants.ServiceName))
                    {
                        //イベントログに出力する。
                        EventLog.WriteEntry("BCP_Backup_Client", CommonLibrary.Message.Get(CommonLibrary.MessageKey.e00054,
                            new string[] { "バックアップサービス停止" }), EventLogEntryType.Error);
                    }
#endif
                    MessageBox.Show(CommonLibrary.Message.Get(CommonLibrary.MessageKey.e00053) + "アプリを終了します");
                    EventLog.WriteEntry("BCP_Backup_Client", this.Name + ":" + "設定アプリ終了（アカウント無し）", EventLogEntryType.Warning);
                    notifyIcon1.Visible = false;    //アイコンをトレイから取り除く
                    notifyIcon1.Dispose();
                    Application.Exit();             //アプリケーションの終了
                }
            }
            else
            {
                if (!Program.applicationContext.MainForm.Visible)
                {
                    Program.applicationContext.MainForm.Show();
                }
                else
                {
                    Program.applicationContext.MainForm.Activate();
                }
            }
        }

        private void 即時ダウンロードToolStripMenuItem_Click(object sender, EventArgs e)
        {
            /* 二重起動防止 */
            if (Program.applicationContext.MainForm == null || Program.applicationContext.MainForm.IsDisposed)
            {
                if (Configuration.ExistsInRegistry(Constants.Settings.AccountId))
                {
                    CL99_02 formCL99_02 = new CL99_02(); //インスタンスを作成
                    formCL99_02.nextForm = 2;
                    Program.applicationContext.MainForm = formCL99_02;
                    Program.applicationContext.MainForm.Show();  //CL99_02を表示する
                }
                else
                {
#if !CLIENT_DEBUG
                    // バックアップサービス停止
                    if (!WindowsService.StopService(Constants.ServiceName))
                    {
                        //イベントログに出力する。
                        EventLog.WriteEntry("BCP_Backup_Client", CommonLibrary.Message.Get(CommonLibrary.MessageKey.e00054,
                            new string[] { "バックアップサービス停止" }), EventLogEntryType.Error);
                    }
#endif
                    MessageBox.Show(CommonLibrary.Message.Get(CommonLibrary.MessageKey.e00053) + "アプリを終了します");
                    EventLog.WriteEntry("BCP_Backup_Client", this.Name + ":" + "設定アプリ終了（アカウント無し）", EventLogEntryType.Warning);
                    notifyIcon1.Visible = false;    //アイコンをトレイから取り除く
                    notifyIcon1.Dispose();
                    Application.Exit();             //アプリケーションの終了
                }
            }
            else
            {
                if (!Program.applicationContext.MainForm.Visible)
                {
                    Program.applicationContext.MainForm.Show();
                }
                else
                {
                    Program.applicationContext.MainForm.Activate();
                }
            }
        }

        private void 終了ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            /* 二重起動防止 */
            if (Program.applicationContext.MainForm == null || Program.applicationContext.MainForm.IsDisposed)
            {
                const string strTitle = "終了確認";
                const string strMsg = "終了した場合は、ファイルのアップロードができなくなりますが、本当によろしいですか？";
                DialogResult result = MessageBox.Show(strMsg, strTitle, MessageBoxButtons.OKCancel);
                if(result == System.Windows.Forms.DialogResult.Cancel)
                {
                    return;
                }
                // バックアップサービス停止
                if (!WindowsService.StopService(Constants.ServiceName))
                {
                    //イベントログに出力する。
                    EventLog.WriteEntry("BCP_Backup_Client", CommonLibrary.Message.Get(CommonLibrary.MessageKey.e00054,
                        new string[] { "バックアップサービス停止" }), EventLogEntryType.Error);
                }

                EventLog.WriteEntry("BCP_Backup_Client", this.Name + ":" + "設定アプリ終了", EventLogEntryType.Information);
                notifyIcon1.Visible = false;    //アイコンをトレイから取り除く
                notifyIcon1.Dispose();
                Application.Exit();             //アプリケーションの終了
            }
            else
            {
                if (!Program.applicationContext.MainForm.Visible)
                {
                    Program.applicationContext.MainForm.Show();
                }
                else
                {
                    Program.applicationContext.MainForm.Activate();
                }
            }
        }

        private void CL03_00_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (e.CloseReason != System.Windows.Forms.CloseReason.ApplicationExitCall)
            {
                // 終了命令のキャンセルは行わないため下記コメント
                //e.Cancel = true;
                //this.Visible = false;
            }
        }
    }

}
