using System;
using System.Collections.Generic;
using System.IO.Compression;
using System.Text;

namespace NetworkLib
{
    class Installer
    {

        public static void Unzip(string zipPath, string extractPath)
        {
            try
            {
                ZipFile.ExtractToDirectory(zipPath, extractPath);
            }
            catch (Exception ex)
            {
                throw (ex);
            }

        }

        public static void InstallService(String ServiceName, String ServicePath)
        {
            try
            {
                if (ServiceControl.IsServiceRunning(ServiceName))
                {
                    ServiceControl.StopService(ServiceName);
                }
                if (ServiceControl.IsServiceInstalled(ServiceName))
                {
                    ServiceControl.UinstallService(ServiceName);
                }
                ServiceControl.InstallService(ServiceName, ServicePath);
            }
            catch (Exception ex)
            {
                throw (ex);
            }
        }
        public static void CopyTo()
        {

        }
    }
}
