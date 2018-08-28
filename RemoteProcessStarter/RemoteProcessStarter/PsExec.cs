using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace RemoteProcessStarter
{
    static class PsExec
    {
        /// <summary>
        /// Starts a process on another computer on the same network.
        /// </summary>
        /// <param name="computerName">The name of the remote computer or its IP address. Starting it with '\\' is optional.</param>
        /// <param name="username">The username for the user logged in user account on the remote computer.</param>
        /// <param name="password">The password for the user logged in user account on the remote computer.</param>
        /// <param name="processPath">The path of the target process (relative to the remote computer).</param>
        /// <param name="arguments">(Optional) The arguments to be passed to the process, separated by a space character.</param>
        /// <param name="timeout">(Optional) The number of milliseconds the application should wait for the command to be executed.</param>
        /// <returns>The result of executing the command. Could be the standard output and/or the standard error or a timeout indication.</returns>
        public static string StartRemoteProcess(string computerName, string username, string password, 
                                                string processPath, string arguments = "", 
                                                string sessionId = "", int? timeout = null)
        {
            if (!(computerName.StartsWith(@"\\"))) computerName = @"\\" + computerName;

            // -i	        Run the program so that it interacts with the desktop of the specified session 
            //              on the remote system. If no session is specified the process runs in the console session.
            // -d	        Don't wait for process to terminate (non-interactive).
            // -accepteula  Suppress the display of the license dialog.
            // More:    https://docs.microsoft.com/en-us/sysinternals/downloads/psexec
            string args = $@"{computerName} -accepteula -i {sessionId} -d -u ""{username}"" -p ""{password}"" ""{processPath}"" {arguments}";
            var pi = new ProcessStartInfo("PsExec.exe", args);
            pi.CreateNoWindow = true;
            pi.WindowStyle = ProcessWindowStyle.Hidden;
            pi.UseShellExecute = false;
            pi.RedirectStandardOutput = true;
            pi.RedirectStandardError = true;
            using (Process p = Process.Start(pi))
            {
                var resultLines = new List<string>();
                // Creating an event handler to receive the StdOutput and StdError.
                var handler = new DataReceivedEventHandler(delegate (object o, DataReceivedEventArgs e)
                {
                    // No need to fill the log with empty or redundant lines.
                    if (string.IsNullOrWhiteSpace(e.Data)) return;
                    if (e.Data.StartsWith("PsExec") ||
                        e.Data.StartsWith("Copyright") ||
                        e.Data.StartsWith("Sysinternals")) return;

                    resultLines.Add(e.Data);
                });

                p.ErrorDataReceived += handler;
                p.OutputDataReceived += handler;
                p.BeginErrorReadLine();
                p.BeginOutputReadLine();

                if (timeout.HasValue)
                    p.WaitForExit(timeout.Value);
                else
                    p.WaitForExit();

                // Signal a timeout indication if the timeout value has been reached.
                if (!p.HasExited)
                    resultLines.Add("TIMEOUT!");

                return string.Join("\r\n", resultLines);
            }
        }

        /// <summary>
        /// Starts a process on another computer on the same network.
        /// </summary>
        /// <param name="computerName">The name of the remote computer or its IP address. Starting it with '\\' is optional.</param>
        /// <param name="username">The username for the user logged in user account on the remote computer.</param>
        /// <param name="password">The password for the user logged in user account on the remote computer.</param>
        /// <param name="useAlternateOutput">If true, the stdout and stderr will be redirected to a temp file and then reloaded after the process exits.</param>
        /// <param name="processPath">The path of the target process (relative to the remote computer).</param>
        /// <param name="arguments">(Optional) The arguments to be passed to the process, separated by a space character.</param>
        /// <param name="timeout">(Optional) The number of milliseconds the application should wait for the command to be executed.</param>
        /// <returns>The result of executing the command. Could be the standard output and/or the standard error or a timeout indication.</returns>
        public static string StartRemoteProcess(string computerName, string username, string password,
                                                bool useAlternateOutput, string processPath,
                                                string arguments = "", string sessionId = "", int? timeout = null)
        {
            if (!useAlternateOutput)
                return StartRemoteProcess(computerName, username, password, processPath,
                                          arguments, sessionId, timeout);

            if (!(computerName.StartsWith(@"\\"))) computerName = @"\\" + computerName;

            // -i	        Run the program so that it interacts with the desktop of the specified session 
            //              on the remote system. If no session is specified the process runs in the console session.
            // -d	        Don't wait for process to terminate (non-interactive).
            // -accepteula  Suppress the display of the license dialog.
            // More:    https://docs.microsoft.com/en-us/sysinternals/downloads/psexec
            string tempFile = Path.GetTempFileName();
            string args = $@"/c PsExec.exe {computerName} -accepteula -i {sessionId} -d -u ""{username}"" -p ""{password}"" ""{processPath}"" {arguments}";
            var pi = new ProcessStartInfo("cmd.exe", $"{args} 1> {tempFile} 2>&1");
            pi.CreateNoWindow = true;
            pi.WindowStyle = ProcessWindowStyle.Hidden;
            using (Process p = Process.Start(pi))
            {
                if (timeout.HasValue)
                    p.WaitForExit(timeout.Value);
                else
                    p.WaitForExit();

                List<string> resultLines = File.ReadAllLines(tempFile).
                                                Where((l) => !string.IsNullOrWhiteSpace(l) &&
                                                             !l.StartsWith("PsExec") &&
                                                             !l.StartsWith("Copyright") &&
                                                             !l.StartsWith("Sysinternals")).ToList();
                File.Delete(tempFile);

                // Signal a timeout indication if the timeout value has been reached.
                if (!p.HasExited)
                    resultLines.Add("TIMEOUT!");

                return string.Join("\r\n", resultLines);
            }
        }

        public static string GetSessionId(string computerName, string logiUser, string loginPass, 
                                          string targetUser, int? timeout = null)
        {
            if (!(computerName.StartsWith(@"\\"))) computerName = @"\\" + computerName;

            string tempFile = Path.GetTempFileName();
            string args = $@"/c PsExec.exe {computerName} -accepteula -u ""{logiUser}"" -p ""{loginPass}"" query session";
            var pi = new ProcessStartInfo("cmd.exe", $"{args} 1> {tempFile} 2>&1");
            pi.CreateNoWindow = true;
            pi.WindowStyle = ProcessWindowStyle.Hidden;
            using (Process p = Process.Start(pi))
            {
                if (timeout.HasValue)
                    p.WaitForExit(timeout.Value);
                else
                    p.WaitForExit();

                if (!p.HasExited)
                    return string.Empty;

                string result = File.ReadAllText(tempFile);
                File.Delete(tempFile);

                targetUser = Regex.Split(targetUser, @"[\\\/]").Last();
                Match match = Regex.Match(result, $@"{targetUser}\s+(\d+)", RegexOptions.IgnoreCase);
                return match.Groups[1].Value;
            }
        }
    }
}
