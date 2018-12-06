using System;
using System.Collections.Generic;
using System.DirectoryServices;
using System.Management;
using System.Linq;
using System.Diagnostics;
using System.Windows;
using System.Text.RegularExpressions;

/// <summary>
/// Author: Narek Yegoryan
/// </summary>
namespace ProcessController.Lib
{
    /// <summary>
    /// Core Library
    /// </summary>
    public class Library
    {
        #region Fields

        private const string Name = "Name";
        private const string DriverLetter = "C:";
        private const string FreeSpace = "FreeSpace";
        private const string Size = "Size";
        public const string DefaultMachine = "localhost";
        private string _currentDomain;

        #endregion

        #region Ctor

        public Library()
        {
            _currentDomain = System.Net.NetworkInformation.IPGlobalProperties.GetIPGlobalProperties().DomainName;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Get's a string which will indicate available space from total size
        /// </summary>
        /// <param name="machine"></param>
        /// <param name="username"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        public string GetDiskAvailableSpace(string machine, string username = null, string password = null)
        {
            if (string.IsNullOrWhiteSpace(machine))
                throw new ArgumentException(nameof(machine));

            var result = string.Empty;
            var options = new ConnectionOptions { Username = username, Password = password };
            var scope = new ManagementScope(@"\\" + machine + @"\root\cimv2", options);
            var queryString = "select Name, Size, FreeSpace from Win32_LogicalDisk where DriveType=3";
            var query = new ObjectQuery(queryString);

            var worker = new ManagementObjectSearcher(scope, query);

            try
            {
                var results = worker.Get();

                foreach (ManagementObject item in results)
                {
                    if (item[Name] as string == "C:")
                        result = string.Format("{0} {1} free of {2}", item[Name], FormatBytes(Convert.ToInt64(item[FreeSpace]), true), FormatBytes(Convert.ToInt64(item[Size]), true));
                }
            }
            catch (Exception e)
            {
                result = $"Cannot get {e.Message}";
            }

            return result;
        }

        /// <summary>
        /// Format's passed bytes into readable format
        /// </summary>
        /// <param name="bytes"></param>
        /// <param name="useUnit"></param>
        /// <returns></returns>
        public static string FormatBytes(long bytes, bool useUnit = false)
        {
            string[] Suffix = { " B", " kB", " MB", " GB", " TB" };
            double dblSByte = bytes;
            int i;
            for (i = 0; i < Suffix.Length && bytes >= 1024; i++, bytes /= 1024)
            {
                dblSByte = bytes / 1024.0;
            }
            return $"{dblSByte:0.##}{(useUnit ? Suffix[i] : null)}";
        }

        /// <summary>
        /// Gets all processes, if this action called from local machine it will retrive data for it also.
        /// Please use kindly
        /// </summary>
        /// <param name="machineName"></param>
        /// <returns></returns>
        public List<Process> GetProcesses(string machineName)
        {
            List<Process> processes = new List<Process>();

            if (machineName == DefaultMachine)
                processes = Process.GetProcesses().ToList();
            else
                processes = Process.GetProcesses(machineName).ToList();

            return processes;
        }

        /// <summary>
        /// Get's all processes from remote computer
        /// </summary>
        /// <param name="machineName">Machine name</param>
        /// <param name="userName">Login detail for user</param>
        /// <param name="password">Secret detail for user</param>
        /// <param name="checkDomain">Set this flag if you will use domain based connections</param>
        /// <returns></returns>
        public List<ProcessModel> GetProcessesFromRemoteMachine(string machineName, string userName, string password, bool checkDomain = false)
        {
            var connOptions = new ConnectionOptions();
            connOptions.Impersonation = ImpersonationLevel.Impersonate;

            connOptions.EnablePrivileges = true;

            if (checkDomain)
                connOptions.Username = _currentDomain + "\\" + userName;
            else
                connOptions.Username = machineName + "\\" + userName;

            connOptions.Password = password;

            var myScope = new ManagementScope(@"\\" + machineName + @"\ROOT\CIMV2", connOptions);
            var result = new List<ProcessModel>();

            try
            {
                myScope.Connect();
                var objSearcher = new ManagementObjectSearcher("SELECT * FROM Win32_Process");

                objSearcher.Scope = myScope;
                string[] sep = { "\n", "\t" };


                foreach (ManagementObject obj in objSearcher.Get())
                {
                    string caption = obj.GetText(TextFormat.Mof);
                    string[] split = caption.Split(sep, StringSplitOptions.RemoveEmptyEntries);

                    var model = new ProcessModel();

                    // Iterate through the splitter
                    for (int i = 0; i < split.Length; i++)
                    {
                        if (split[i].Split('=').Length > 1)
                        {
                            string[] procDetails = split[i].Split('=');
                            procDetails[1] = procDetails[1].Replace(@"""", "");
                            procDetails[1] = procDetails[1].Replace(';', ' ');
                            switch (procDetails[0].Trim().ToLower())
                            {
                                case "caption":
                                    model.Caption = procDetails[1];
                                    break;
                                case "csname":
                                    model.CsName = procDetails[1];
                                    break;
                                case "description":
                                    model.Description = procDetails[1];
                                    break;
                                case "name":
                                    model.Name = procDetails[1];
                                    break;
                                case "priority":
                                    model.Priority = procDetails[1];
                                    break;
                                case "processid":
                                    model.ProcessId = procDetails[1];
                                    break;
                                case "sessionid":
                                    model.SessionId = procDetails[1];
                                    break;
                            }
                        }
                    }

                    result.Add(model);
                }
            }
            catch (Exception ex)
            {

                throw;
            }

            return result;
        }

        /// <summary>
        /// Start's new process on {machine}, username and password is optional
        /// </summary>
        /// <param name="machine"></param>
        /// <param name="process"></param>
        /// <param name="username"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        public bool StartNewProcess(string machine, string process, string username = null, string password = null)
        {
            bool result = false;

            var options = new ConnectionOptions { Username = username, Password = password };
            var scope = new ManagementScope(@"\\" + machine + @"\root\cimv2", options);

            try
            {
                scope.Connect();

                using (var management = new ManagementClass(scope, new ManagementPath("Win32_Process"), new ObjectGetOptions()))
                {
                    management.InvokeMethod("Create", new object[] { process });
                }
                result = true;
            }
            catch
            {
                result = false;
            }

            return result;
        }

        /// <summary>
        /// Terminates specified process
        /// </summary>
        /// <param name="machine"></param>
        /// <param name="processId"></param>
        /// <param name="username"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        public bool TerminateProcess(string machine, string processId, string username = null, string password = null)
        {
            bool result = false;

            var options = new ConnectionOptions { Username = username, Password = password };
            var scope = new ManagementScope(@"\\" + machine + @"\root\cimv2", options);

            try
            {
                scope.Connect();

                var query = new SelectQuery("select * from Win32_process");

                using (var management = new ManagementObjectSearcher(scope, query))
                {
                    foreach (ManagementObject process in management.Get())
                    {
                        string caption = process.GetText(TextFormat.Mof).Trim();

                        if (caption.Contains(processId.Trim()))
                        {
                            process.InvokeMethod("Terminate", null);
                        }
                    }
                }
                result = true;
            }
            catch
            {
                throw;
            }

            return result;
        }

        /// <summary>
        /// Get's all computers in current domain
        /// </summary>
        /// <returns></returns>
        public List<string> GetCurrentDomainComputers()
        {
            List<string> ComputerNames = new List<string>();

            DirectoryEntry entry = new DirectoryEntry($"LDAP://{_currentDomain}");
            DirectorySearcher mySearcher = new DirectorySearcher(entry);
            mySearcher.Filter = ("(objectClass=computer)");
            mySearcher.SizeLimit = int.MaxValue;
            mySearcher.PageSize = int.MaxValue;
            try
            {
                foreach (SearchResult resEnt in mySearcher.FindAll())
                {
                    string ComputerName = resEnt.GetDirectoryEntry().Name;
                    if (ComputerName.StartsWith("CN="))
                        ComputerName = ComputerName.Remove(0, "CN=".Length);
                    ComputerNames.Add(ComputerName);
                }
            }
            catch
            {
                throw;
            }


            mySearcher.Dispose();
            entry.Dispose();

            return ComputerNames;
        }

        /// <summary>
        /// Get all logged in user
        /// </summary>
        /// <param name="machineName"></param>
        /// <returns></returns>
        public string WhoisLoggedIn(string machineName)
        {
            ConnectionOption­s myConnectionOptions = new ConnectionOptions
            {
                Impersonation = ImpersonationLevel.Impersonate
            };

            ManagementObject­Searcher myObjectSearcher;
            ManagementObject­Collection myCollection;

            try
            {
                ManagementScope objwmiservice = new ManagementScope(("\\\\" + (machineName + "\\root\\cimv2")), myConnectionOptions);
                objwmiservice.Connect();
                myObjectSearcher = new ManagementObject­Searcher(objwmiservice.Path.ToStri­ng(), "Select UserName from Win32_ComputerSystem");
                myObjectSearcher.Options.Timeout = new TimeSpan(0, 0, 0, 0, 7000);
                myCollection = myObjectSearcher.Get();

                foreach (ManagementObjec­t myObject in myCollection)
                {
                    if (!(myObject.GetPropertyValue("Username") == null))
                    {
                        string Userx = myObject.GetPropertyValue("Username").ToString();
                        int posx = Userx.LastIndexOf("\\");
                        if ((posx > 0))
                        {
                            Userx = Userx.Substring((posx + 1));
                            return Userx.ToUpper();
                        }
                    }
                }
                return "<Nobody>";
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        /// <summary>
        /// Enumerates over the list passed to method, and if found at least one computer will return computer.
        /// </summary>
        /// <param name="machines"></param>
        /// <param name="username"></param>
        /// <returns></returns>
        public string CheckIfUserIsLoggedIntoMachine(IEnumerable<string> machines, string username)
        {
            foreach (var item in machines)
            {
                string loggedInUser = WhoisLoggedIn(item);
                if (username.ToLower() == loggedInUser.ToLower()) return item;
            }
            return "No match";
        }

        /// <summary>
        /// Pings to computer to check computer is turned on or not.
        /// </summary>
        /// <param name="computer">Computer to ping</param>
        /// <param name="timeout">Timeout</param>
        /// <returns></returns>
        public bool Ping(string computer, int timeout)
        {
            if (timeout == 0)
                timeout = TimeSpan.FromMinutes(1).Milliseconds;
            System.Net.NetworkInformation.Ping ping = new System.Net.NetworkInformation.Ping();
            bool pingable = false;
            try
            {
                pingable = ping.Send(computer, timeout).Status == System.Net.NetworkInformation.IPStatus.Success;
            }
            catch (Exception ex)
            {
            }
            return pingable;
        }

        #endregion
    }
}