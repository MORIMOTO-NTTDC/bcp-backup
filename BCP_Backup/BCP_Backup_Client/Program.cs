using CommonLibrary;
using System;
using System.Diagnostics;
using System.Linq;
using System.Windows.Forms;

namespace BCP_Backup_Client
{
    internal static class Program
    {
        public static ApplicationContext applicationContext;
        public static HttpClientUtility webApiService;
        public static bool loginFlg = false;

        private static CL01_02 cl01_02; // 1. Add form instance
        private static CL03_00 cl03_00; // 1. Add form instance
        /// <summary>
        /// アプリケーションのメイン エントリ ポイントです。
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            applicationContext = new ApplicationContext();

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
                MessageBox.Show(CommonLibrary.Message.Get(MessageKey.i00023, new string[] { "BCP設定アプリ" }));
            }
            else
            {
                webApiService = new HttpClientUtility();

#if CLIENT_DEBUG
                //Configuration.WriteToRegistry(Constants.Settings.AccountId, "1311234567");
                //Configuration.WriteToRegistry(Constants.Settings.AccountId, "");
                //Configuration.WriteToRegistry(Constants.Settings.Status, "0");
                //Configuration.WriteToRegistry(Constants.Settings.LastDate, "");
                //Configuration.WriteToRegistry(Constants.Settings.ErrorInfo, "");
                //Configuration.WriteToRegistry(Constants.Settings.UsedSize, "0");
                //Configuration.WriteToRegistry(Constants.Settings.BackupCapa, "0");
                //Configuration.WriteToRegistry(Constants.Settings.UploadEnable, "true");
                //Configuration.WriteToRegistry(Constants.Settings.UploadTiming, "60");
                //Configuration.WriteToRegistry(Constants.Settings.LocalDir, "C:\\work");

                // "abcd1234" のSHA256ハッシュ値
                //string strPassPhase = "e9cee71ab932fde863338d08be4de9dfe39ea049bdafb342ce659ec5450b69ae";
                //Configuration.WriteToRegistry(Constants.Settings.PermitPassword, strPassPhase);

                // サービスステータスを待機中
                //Configuration.WriteToRegistry(Constants.Settings.ServiceStatus, ServiceStatus.Stopping);
#endif
                // １．アカウントン認証（CL01_02）
                cl01_02 = new CL01_02();
                Program.applicationContext.MainForm = cl01_02;
                Application.Run(cl01_02);

                // 認証OK（ログインフラグがON）？
                if (loginFlg == true)
                {
                    // ２．メニュー（CL03_00）をタスクトレイに常駐
                    cl03_00 = new CL03_00();
                    Application.Run();
                }
            }
            // ３．アプリケーションの終了
            Environment.Exit(0);
        }
    }
}
