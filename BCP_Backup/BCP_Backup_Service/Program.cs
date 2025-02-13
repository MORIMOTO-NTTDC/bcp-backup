using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace BCP_Backup_Service
{
    internal static class Program
    {
        /// <summary>
        /// アプリケーションのメイン エントリ ポイントです。
        /// </summary>
        static void Main()
        {
            var service = new BackupService();

            // デバッグ用参考URL
            // https://qiita.com/okbear/items/dc0aa0a03c02cec462e6
            if (System.Environment.UserInteractive)
            {
                var wh = new EventWaitHandle(false, EventResetMode.AutoReset, "StopService");
                service.StartDebug(null);
                wh.WaitOne();
                service.StopDebug();
            }
            else
            {
                ServiceBase.Run(service);
            }
        }
    }
}
