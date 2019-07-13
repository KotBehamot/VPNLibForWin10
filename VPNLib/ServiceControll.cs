using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceProcess;
using System.Threading;
using System.Configuration;
using System.Configuration.Install;

namespace NetworkLib
{
    public static class ServiceControl 
    {


        public static ServiceController GetService(string serviceName)
        {
            ServiceController[] services = ServiceController.GetServices();
            return services.FirstOrDefault(o => o.ServiceName == serviceName);
        }

        public static bool IsServiceRunning(string serviceName)
        {
            ServiceControllerStatus status;
            uint counter = 0;
            do
            {
                ServiceController service = GetService(serviceName);
                if (service == null)
                {
                    return false;
                }

                Thread.Sleep(100);
                status = service.Status;
            } while (!(status == ServiceControllerStatus.Stopped ||
                       status == ServiceControllerStatus.Running) &&
                     (++counter < 30));
            return status == ServiceControllerStatus.Running;
        }

        public static bool IsServiceInstalled(string serviceName)
        {
            return GetService(serviceName) != null;
        }

        public static void StartService(string serviceName)
        {
            ServiceController controller = GetService(serviceName);
            if (controller == null)
            {
                return;
            }
            if (IsServiceRunning(serviceName))
            {
                return;
            }
            controller.Start();
            controller.WaitForStatus(ServiceControllerStatus.Running);
        }
        public static void StartService(string serviceName, string[] arg)
        {
            ServiceController controller = GetService(serviceName);
            if (controller == null)
            {
                return;
            }
            if (IsServiceRunning(serviceName))
            {
                return;
            }
            controller.Start(arg);
            controller.WaitForStatus(ServiceControllerStatus.Running);
        }

        public static void StopService(string serviceName)
        {
            ServiceController controller = GetService(serviceName);
            if (controller == null)
            {
                return;
            }

            controller.Stop();
            controller.WaitForStatus(ServiceControllerStatus.Stopped);
        }
        public static bool LongTermOperation(string serviceName)
        {
            ServiceController controller = GetService(serviceName);
            if (controller.Status == ServiceControllerStatus.ContinuePending || controller.Status == ServiceControllerStatus.StartPending || controller.Status == ServiceControllerStatus.StopPending)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        public static void UinstallService(string serviceName)
        {
            StopService(serviceName);
            ServiceInstaller ServiceInstallerObj = new ServiceInstaller();
            InstallContext Context = new InstallContext("<<log file path>>", null);
            ServiceInstallerObj.Context = Context;
            ServiceInstallerObj.ServiceName = serviceName;
            ServiceInstallerObj.Uninstall(null);
            ServiceInstallerObj.Install(null);


        }
        public static void InstallService(string serviceName, string path)
        {
            StopService(serviceName);
            string cmdpath = string.Format("/assemblypath={0}", path);
            string[] cmdline = { cmdpath };
            ServiceInstaller ServiceInstallerObj = new ServiceInstaller();
            InstallContext Context = new System.Configuration.Install.InstallContext("", cmdline);
            ServiceInstallerObj.Context = Context;
            ServiceInstallerObj.ServiceName = serviceName;
            ServiceInstallerObj.Install(null);


        }
    }

}
