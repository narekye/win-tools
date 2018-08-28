using System.Management;

namespace RemoteProcessStarter
{
    static class WMI
    {
        public static bool RemoteProcessExists(string computerName, string username, string password, string processId)
        {
            ManagementScope managementScope;
            ConnectionOptions connOptions = new ConnectionOptions();

            connOptions.Impersonation = ImpersonationLevel.Impersonate;
            connOptions.EnablePrivileges = true;
            connOptions.Username = username;
            connOptions.Password = password;
            if (!(computerName.StartsWith(@"\\"))) computerName = @"\\" + computerName;
            managementScope = new ManagementScope(computerName + @"\ROOT\CIMV2", connOptions);

            managementScope.Connect();
            ManagementObjectSearcher objSearcher =
                new ManagementObjectSearcher($"SELECT Name FROM Win32_Process WHERE ProcessId = {processId}");
            ManagementOperationObserver opsObserver = new ManagementOperationObserver();
            objSearcher.Scope = managementScope;

            ManagementObjectCollection objects = objSearcher.Get();
            return (objects.Count > 0);
        }
    }
}
