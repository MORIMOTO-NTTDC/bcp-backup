using System.ComponentModel;
using System.Configuration.Install;
using System.ServiceProcess;

[RunInstaller(true)]
public class ProjectInstaller : Installer
{
    private ServiceProcessInstaller processInstaller;
    private ServiceInstaller serviceInstaller;

    public ProjectInstaller()
    {
        processInstaller = new ServiceProcessInstaller();
        serviceInstaller = new ServiceInstaller();

        // サービスのアカウントを設定
        processInstaller.Account = ServiceAccount.LocalSystem;

        // サービスの情報を設定
        serviceInstaller.ServiceName = "BCP_Backup_Service";
        serviceInstaller.DisplayName = "BCP Backup Service";
        serviceInstaller.StartType = ServiceStartMode.Manual;

        // インストーラーに追加
        Installers.Add(processInstaller);
        Installers.Add(serviceInstaller);
    }
}
