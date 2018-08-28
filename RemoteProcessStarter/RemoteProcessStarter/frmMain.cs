using System;
using System.Collections.Generic;
using System.Management;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Text.RegularExpressions;
using System.DirectoryServices;

namespace RemoteProcessStarter
{
    public partial class frmMain : Form
    {
        public frmMain()
        {
            InitializeComponent();
        }

        private void frmMain_Load(object sender, EventArgs e)
        {
            MinimumSize = Size;
            MaximumSize = Size;
        }

        private async void frmMain_Shown(object sender, EventArgs e)
        {
            this.Visible = false;

            await UpdateComputersList();

            this.Visible = true;
            this.ShowInTaskbar = true;
            this.Opacity = 100d;
        }

        private async Task UpdateComputersList()
        {
            btnReloadComputers.Enabled = false;
            cboComputer.Cursor = Cursors.WaitCursor;

            List<string> computerNames;

            try
            {
                computerNames = await Task.Run(() => GetComputersList());
            }
            finally
            {

                btnReloadComputers.Enabled = true;
                cboComputer.Cursor = Cursors.Default;
            }

            cboComputer.Items.Clear();
            cboComputer.Items.AddRange(computerNames.ToArray());
        }

        private List<string> GetComputersList(bool ignoreThisMachine = true)
        {
            var computerNames = new List<string>();

            DirectoryEntry root = new DirectoryEntry("WinNT:");
            foreach (DirectoryEntry computers in root.Children)
            {
                foreach (DirectoryEntry computer in computers.Children)
                {
                    if (computer.Name != "Schema" && computer.SchemaClassName == "Computer")
                    {
                        if (!ignoreThisMachine || computer.Name != Environment.MachineName)
                            computerNames.Add(computer.Name);
                    }
                }
            }

            return computerNames;
        }

        private async void btnReloadComputers_Click(object sender, EventArgs e)
        {
            cboComputer.Focus();
            await UpdateComputersList();
        }

        private void optRunAsDifferentUser_CheckedChanged(object sender, EventArgs e)
        {
            txtRunAs.Visible = optRunAsDifferentUser.Checked;
        }

        private void chkTimeout_CheckedChanged(object sender, EventArgs e)
        {
            pnlTimeout.Enabled = chkTimeout.Checked;
        }

        private void chkSessionID_CheckedChanged(object sender, EventArgs e)
        {
            upDownSessionID.Enabled = chkSessionID.Checked;
        }

        private bool ValidInputs()
        {
            if (HelperMethods.IsTextBoxEmpty(cboComputer, "Please enter the computer name.")) return false;
            if (HelperMethods.IsTextBoxEmpty(txtUsername, "Please enter the account username.")) return false;
            //if (HelperMethods.IsTextBoxEmpty(txtPassword, "Please enter the account password.", true)) return false;
            if (optRunAsDifferentUser.Checked &&
                HelperMethods.IsTextBoxEmpty(txtRunAs, "Please enter the target username.")) return false;
            if (HelperMethods.IsTextBoxEmpty(txtProcessPath, "Please enter the process path.")) return false;

            return true;
        }

        private async void btnStartProcess_Click(object sender, EventArgs e)
        {
            if (!ValidInputs()) return;

            btnStartProcess.Enabled = false;
            this.Cursor = Cursors.WaitCursor;
            txtResult.Clear();
            sLblStatus.Text = "Running..";

            string computer = cboComputer.Text.Trim();
            string username = txtUsername.Text.Trim();
            string pass = txtPassword.Text;
            string processPath = txtProcessPath.Text.Trim();
            string args = txtArguments.Text.Trim();
            int? timeout = chkTimeout.Checked ? (int)upDownTimeout.Value * 1000 : new int?();
            string sessionId = chkSessionID.Checked ? upDownSessionID.Value.ToString() : string.Empty;
            try
            {
                if (string.IsNullOrEmpty(sessionId))
                {
                    string targetUser = optRunAsDifferentUser.Checked ? txtRunAs.Text.Trim() : username;
                    sessionId = await Task.Run(() => PsExec.GetSessionId(computer, username, pass, targetUser, timeout));
                }
                string result = 
                    await Task.Run(() => PsExec.StartRemoteProcess(computer, username, pass, processPath, 
                                                                   args, sessionId, timeout));
                txtResult.Text = result;
                Match match = Regex.Match(result, @"Process ID\s+(\d+)", RegexOptions.IgnoreCase);
                if (match.Success)
                {
                    txtResult.AppendText("\r\n" + "Making sure the process is running on the remote computer...");
                    string processId = match.Groups[1].Value;
                    try
                    {
                        bool found = await Task.Run(() => WMI.RemoteProcessExists(computer, username, pass, processId));
                        if (found)
                            txtResult.AppendText("\r\n" + "The process is currently running on the remote computer.");
                        else
                            txtResult.AppendText("\r\n" + "The process is not running on the remote computer.");
                    }
                    catch (ManagementException ex)
                    {

                        MessageBox.Show("An error occurred while trying to find the process on the remote computer:" + 
                                        Environment.NewLine + ex.Message, "Error!",
                                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
            finally
            {
                btnStartProcess.Enabled = true;
                this.Cursor = Cursors.Default;
                sLblStatus.Text = "Completed";
            }

            txtResult.SelectionStart = txtResult.TextLength;
            txtResult.ScrollToCaret();
        }
    }
}
