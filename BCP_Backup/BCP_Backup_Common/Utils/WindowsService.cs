using System;
using System.ServiceProcess;

public static class WindowsService
{
    /// <summary>
    /// 指定されたサービスを起動します。
    /// </summary>
    /// <param name="serviceName">サービスの名前</param>
    /// <returns>起動に成功した場合は true、それ以外の場合は false</returns>
    public static bool StartService(string serviceName)
    {
        using (ServiceController service = new ServiceController(serviceName))
        {
            try
            {
                // すでにサービスが起動中の場合は何もしない
                if (service.Status == ServiceControllerStatus.Running)
                {
                    return true;
                }
                else if (service.Status == ServiceControllerStatus.Stopped)
                {
                    service.Start();
                    service.WaitForStatus(ServiceControllerStatus.Running, TimeSpan.FromSeconds(30));
                    return true;
                }
                else if (service.Status == ServiceControllerStatus.StartPending)
                {
                    Console.WriteLine($"サービス {serviceName} は既に起動中です。");
                    service.WaitForStatus(ServiceControllerStatus.Running, TimeSpan.FromSeconds(30));
                    return true;
                }
                else
                {
                    Console.WriteLine($"サービス {serviceName} は現在の状態: {service.Status} で起動できません。");
                    return false;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"サービス {serviceName} の起動中にエラーが発生しました: {ex.Message}");
                return false;
            }
        }
    }

    /// <summary>
    /// 指定されたサービスを停止します。
    /// </summary>
    /// <param name="serviceName">サービスの名前</param>
    /// <returns>停止に成功した場合は true、それ以外の場合は false</returns>
    public static bool StopService(string serviceName)
    {
        using (ServiceController service = new ServiceController(serviceName))
        {
            try
            {
                // すでにサービスが停止中の場合は何もしない
                if (service.Status == ServiceControllerStatus.Stopped)
                {
                    return true;
                }
                else if(service.Status == ServiceControllerStatus.Running)
                {
                    service.Stop();
                    service.WaitForStatus(ServiceControllerStatus.Stopped, TimeSpan.FromSeconds(30));
                    return true;
                }
                else if (service.Status == ServiceControllerStatus.StopPending)
                {
                    Console.WriteLine($"サービス {serviceName} は既に停止中です。");
                    service.WaitForStatus(ServiceControllerStatus.Stopped, TimeSpan.FromSeconds(30));
                    return true;
                }
                else
                {
                    Console.WriteLine($"サービス {serviceName} は現在の状態: {service.Status} で停止できません。");
                    return false;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"サービス {serviceName} の停止中にエラーが発生しました: {ex.Message}");
                return false;
            }
        }
    }

    /// <summary>
    /// 指定されたサービスの状態を取得します。
    /// </summary>
    /// <param name="serviceName">サービスの名前</param>
    /// <returns>サービスの状態</returns>
    public static ServiceControllerStatus GetServiceStatus(string serviceName)
    {
        using (ServiceController service = new ServiceController(serviceName))
        {
            return service.Status;
        }
    }
}
