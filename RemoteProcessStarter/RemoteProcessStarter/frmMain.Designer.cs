namespace RemoteProcessStarter
{
    partial class frmMain
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmMain));
            this.btnStartProcess = new System.Windows.Forms.Button();
            this.lblComputer = new System.Windows.Forms.Label();
            this.lblUsername = new System.Windows.Forms.Label();
            this.lblPassword = new System.Windows.Forms.Label();
            this.lblProcessPath = new System.Windows.Forms.Label();
            this.txtUsername = new System.Windows.Forms.TextBox();
            this.txtPassword = new System.Windows.Forms.TextBox();
            this.txtProcessPath = new System.Windows.Forms.TextBox();
            this.txtArguments = new System.Windows.Forms.TextBox();
            this.lblArguments = new System.Windows.Forms.Label();
            this.upDownTimeout = new System.Windows.Forms.NumericUpDown();
            this.lblTimeoutUnit = new System.Windows.Forms.Label();
            this.pnlTimeout = new System.Windows.Forms.Panel();
            this.chkTimeout = new System.Windows.Forms.CheckBox();
            this.txtResult = new System.Windows.Forms.TextBox();
            this.lblResult = new System.Windows.Forms.Label();
            this.statusBar = new System.Windows.Forms.StatusStrip();
            this.sLblStatus = new System.Windows.Forms.ToolStripStatusLabel();
            this.chkSessionID = new System.Windows.Forms.CheckBox();
            this.upDownSessionID = new System.Windows.Forms.NumericUpDown();
            this.txtRunAs = new System.Windows.Forms.TextBox();
            this.lblRunAs = new System.Windows.Forms.Label();
            this.optRunAsSameUser = new System.Windows.Forms.RadioButton();
            this.optRunAsDifferentUser = new System.Windows.Forms.RadioButton();
            this.cboComputer = new System.Windows.Forms.ComboBox();
            this.btnReloadComputers = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.upDownTimeout)).BeginInit();
            this.pnlTimeout.SuspendLayout();
            this.statusBar.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.upDownSessionID)).BeginInit();
            this.SuspendLayout();
            // 
            // btnStartProcess
            // 
            this.btnStartProcess.Location = new System.Drawing.Point(348, 209);
            this.btnStartProcess.Name = "btnStartProcess";
            this.btnStartProcess.Size = new System.Drawing.Size(94, 30);
            this.btnStartProcess.TabIndex = 13;
            this.btnStartProcess.Text = "Start process";
            this.btnStartProcess.UseVisualStyleBackColor = true;
            this.btnStartProcess.Click += new System.EventHandler(this.btnStartProcess_Click);
            // 
            // lblComputer
            // 
            this.lblComputer.AutoSize = true;
            this.lblComputer.Location = new System.Drawing.Point(12, 18);
            this.lblComputer.Name = "lblComputer";
            this.lblComputer.Size = new System.Drawing.Size(101, 13);
            this.lblComputer.TabIndex = 2;
            this.lblComputer.Text = "Computer name/IP:";
            // 
            // lblUsername
            // 
            this.lblUsername.AutoSize = true;
            this.lblUsername.Location = new System.Drawing.Point(12, 46);
            this.lblUsername.Name = "lblUsername";
            this.lblUsername.Size = new System.Drawing.Size(59, 13);
            this.lblUsername.TabIndex = 3;
            this.lblUsername.Text = "Username:";
            // 
            // lblPassword
            // 
            this.lblPassword.AutoSize = true;
            this.lblPassword.Location = new System.Drawing.Point(259, 46);
            this.lblPassword.Name = "lblPassword";
            this.lblPassword.Size = new System.Drawing.Size(57, 13);
            this.lblPassword.TabIndex = 4;
            this.lblPassword.Text = "Password:";
            // 
            // lblProcessPath
            // 
            this.lblProcessPath.AutoSize = true;
            this.lblProcessPath.Location = new System.Drawing.Point(12, 98);
            this.lblProcessPath.Name = "lblProcessPath";
            this.lblProcessPath.Size = new System.Drawing.Size(73, 13);
            this.lblProcessPath.TabIndex = 5;
            this.lblProcessPath.Text = "Process path:";
            // 
            // txtUsername
            // 
            this.txtUsername.Location = new System.Drawing.Point(117, 43);
            this.txtUsername.Name = "txtUsername";
            this.txtUsername.Size = new System.Drawing.Size(121, 20);
            this.txtUsername.TabIndex = 2;
            // 
            // txtPassword
            // 
            this.txtPassword.Location = new System.Drawing.Point(321, 43);
            this.txtPassword.Name = "txtPassword";
            this.txtPassword.Size = new System.Drawing.Size(121, 20);
            this.txtPassword.TabIndex = 3;
            this.txtPassword.UseSystemPasswordChar = true;
            // 
            // txtProcessPath
            // 
            this.txtProcessPath.Location = new System.Drawing.Point(117, 95);
            this.txtProcessPath.Name = "txtProcessPath";
            this.txtProcessPath.Size = new System.Drawing.Size(325, 20);
            this.txtProcessPath.TabIndex = 7;
            // 
            // txtArguments
            // 
            this.txtArguments.Location = new System.Drawing.Point(117, 121);
            this.txtArguments.Name = "txtArguments";
            this.txtArguments.Size = new System.Drawing.Size(325, 20);
            this.txtArguments.TabIndex = 8;
            // 
            // lblArguments
            // 
            this.lblArguments.AutoSize = true;
            this.lblArguments.Location = new System.Drawing.Point(12, 124);
            this.lblArguments.Name = "lblArguments";
            this.lblArguments.Size = new System.Drawing.Size(63, 13);
            this.lblArguments.TabIndex = 9;
            this.lblArguments.Text = "Arguments:";
            // 
            // upDownTimeout
            // 
            this.upDownTimeout.Location = new System.Drawing.Point(3, 3);
            this.upDownTimeout.Maximum = new decimal(new int[] {
            120,
            0,
            0,
            0});
            this.upDownTimeout.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.upDownTimeout.Name = "upDownTimeout";
            this.upDownTimeout.Size = new System.Drawing.Size(120, 20);
            this.upDownTimeout.TabIndex = 0;
            this.upDownTimeout.Value = new decimal(new int[] {
            10,
            0,
            0,
            0});
            // 
            // lblTimeoutUnit
            // 
            this.lblTimeoutUnit.AutoSize = true;
            this.lblTimeoutUnit.Location = new System.Drawing.Point(129, 5);
            this.lblTimeoutUnit.Name = "lblTimeoutUnit";
            this.lblTimeoutUnit.Size = new System.Drawing.Size(50, 13);
            this.lblTimeoutUnit.TabIndex = 12;
            this.lblTimeoutUnit.Text = "seconds.";
            // 
            // pnlTimeout
            // 
            this.pnlTimeout.Controls.Add(this.upDownTimeout);
            this.pnlTimeout.Controls.Add(this.lblTimeoutUnit);
            this.pnlTimeout.Enabled = false;
            this.pnlTimeout.Location = new System.Drawing.Point(117, 147);
            this.pnlTimeout.Name = "pnlTimeout";
            this.pnlTimeout.Size = new System.Drawing.Size(183, 28);
            this.pnlTimeout.TabIndex = 10;
            // 
            // chkTimeout
            // 
            this.chkTimeout.AutoSize = true;
            this.chkTimeout.Location = new System.Drawing.Point(15, 153);
            this.chkTimeout.Name = "chkTimeout";
            this.chkTimeout.Size = new System.Drawing.Size(68, 17);
            this.chkTimeout.TabIndex = 9;
            this.chkTimeout.Text = "Timeout:";
            this.chkTimeout.UseVisualStyleBackColor = true;
            this.chkTimeout.CheckedChanged += new System.EventHandler(this.chkTimeout_CheckedChanged);
            // 
            // txtResult
            // 
            this.txtResult.BackColor = System.Drawing.SystemColors.ControlLight;
            this.txtResult.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.txtResult.Location = new System.Drawing.Point(0, 245);
            this.txtResult.Multiline = true;
            this.txtResult.Name = "txtResult";
            this.txtResult.ReadOnly = true;
            this.txtResult.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txtResult.Size = new System.Drawing.Size(454, 84);
            this.txtResult.TabIndex = 14;
            this.txtResult.TabStop = false;
            // 
            // lblResult
            // 
            this.lblResult.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.lblResult.AutoSize = true;
            this.lblResult.Location = new System.Drawing.Point(12, 225);
            this.lblResult.Name = "lblResult";
            this.lblResult.Size = new System.Drawing.Size(41, 13);
            this.lblResult.TabIndex = 10;
            this.lblResult.Text = "Result:";
            // 
            // statusBar
            // 
            this.statusBar.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.sLblStatus});
            this.statusBar.Location = new System.Drawing.Point(0, 329);
            this.statusBar.Name = "statusBar";
            this.statusBar.Size = new System.Drawing.Size(454, 22);
            this.statusBar.SizingGrip = false;
            this.statusBar.TabIndex = 11;
            // 
            // sLblStatus
            // 
            this.sLblStatus.Name = "sLblStatus";
            this.sLblStatus.Size = new System.Drawing.Size(39, 17);
            this.sLblStatus.Text = "Ready";
            // 
            // chkSessionID
            // 
            this.chkSessionID.AutoSize = true;
            this.chkSessionID.Location = new System.Drawing.Point(15, 182);
            this.chkSessionID.Name = "chkSessionID";
            this.chkSessionID.Size = new System.Drawing.Size(80, 17);
            this.chkSessionID.TabIndex = 11;
            this.chkSessionID.Text = "Session ID:";
            this.chkSessionID.UseVisualStyleBackColor = true;
            this.chkSessionID.CheckedChanged += new System.EventHandler(this.chkSessionID_CheckedChanged);
            // 
            // upDownSessionID
            // 
            this.upDownSessionID.Enabled = false;
            this.upDownSessionID.Location = new System.Drawing.Point(120, 181);
            this.upDownSessionID.Name = "upDownSessionID";
            this.upDownSessionID.Size = new System.Drawing.Size(120, 20);
            this.upDownSessionID.TabIndex = 12;
            // 
            // txtRunAs
            // 
            this.txtRunAs.Location = new System.Drawing.Point(321, 69);
            this.txtRunAs.Name = "txtRunAs";
            this.txtRunAs.Size = new System.Drawing.Size(121, 20);
            this.txtRunAs.TabIndex = 6;
            this.txtRunAs.Visible = false;
            // 
            // lblRunAs
            // 
            this.lblRunAs.AutoSize = true;
            this.lblRunAs.Location = new System.Drawing.Point(12, 72);
            this.lblRunAs.Name = "lblRunAs";
            this.lblRunAs.Size = new System.Drawing.Size(44, 13);
            this.lblRunAs.TabIndex = 13;
            this.lblRunAs.Text = "Run as:";
            // 
            // optRunAsSameUser
            // 
            this.optRunAsSameUser.AutoSize = true;
            this.optRunAsSameUser.Checked = true;
            this.optRunAsSameUser.Location = new System.Drawing.Point(117, 70);
            this.optRunAsSameUser.Name = "optRunAsSameUser";
            this.optRunAsSameUser.Size = new System.Drawing.Size(75, 17);
            this.optRunAsSameUser.TabIndex = 4;
            this.optRunAsSameUser.TabStop = true;
            this.optRunAsSameUser.Text = "Same user";
            this.optRunAsSameUser.UseVisualStyleBackColor = true;
            // 
            // optRunAsDifferentUser
            // 
            this.optRunAsDifferentUser.AutoSize = true;
            this.optRunAsDifferentUser.Location = new System.Drawing.Point(219, 70);
            this.optRunAsDifferentUser.Name = "optRunAsDifferentUser";
            this.optRunAsDifferentUser.Size = new System.Drawing.Size(96, 17);
            this.optRunAsDifferentUser.TabIndex = 5;
            this.optRunAsDifferentUser.Text = "Different user:";
            this.optRunAsDifferentUser.UseVisualStyleBackColor = true;
            this.optRunAsDifferentUser.CheckedChanged += new System.EventHandler(this.optRunAsDifferentUser_CheckedChanged);
            // 
            // cboComputer
            // 
            this.cboComputer.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.Append;
            this.cboComputer.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.ListItems;
            this.cboComputer.FormattingEnabled = true;
            this.cboComputer.Location = new System.Drawing.Point(117, 14);
            this.cboComputer.Name = "cboComputer";
            this.cboComputer.Size = new System.Drawing.Size(291, 21);
            this.cboComputer.TabIndex = 0;
            // 
            // btnReloadComputers
            // 
            this.btnReloadComputers.Image = global::RemoteProcessStarter.Properties.Resources.reload;
            this.btnReloadComputers.Location = new System.Drawing.Point(414, 12);
            this.btnReloadComputers.Name = "btnReloadComputers";
            this.btnReloadComputers.Size = new System.Drawing.Size(28, 25);
            this.btnReloadComputers.TabIndex = 1;
            this.btnReloadComputers.TabStop = false;
            this.btnReloadComputers.UseVisualStyleBackColor = true;
            this.btnReloadComputers.Click += new System.EventHandler(this.btnReloadComputers_Click);
            // 
            // frmMain
            // 
            this.AcceptButton = this.btnStartProcess;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(454, 351);
            this.Controls.Add(this.btnReloadComputers);
            this.Controls.Add(this.cboComputer);
            this.Controls.Add(this.optRunAsDifferentUser);
            this.Controls.Add(this.optRunAsSameUser);
            this.Controls.Add(this.lblRunAs);
            this.Controls.Add(this.txtRunAs);
            this.Controls.Add(this.upDownSessionID);
            this.Controls.Add(this.chkSessionID);
            this.Controls.Add(this.lblResult);
            this.Controls.Add(this.txtResult);
            this.Controls.Add(this.chkTimeout);
            this.Controls.Add(this.pnlTimeout);
            this.Controls.Add(this.txtArguments);
            this.Controls.Add(this.lblArguments);
            this.Controls.Add(this.txtProcessPath);
            this.Controls.Add(this.txtPassword);
            this.Controls.Add(this.txtUsername);
            this.Controls.Add(this.lblProcessPath);
            this.Controls.Add(this.lblPassword);
            this.Controls.Add(this.lblUsername);
            this.Controls.Add(this.lblComputer);
            this.Controls.Add(this.btnStartProcess);
            this.Controls.Add(this.statusBar);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.Name = "frmMain";
            this.Opacity = 0D;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Remote Process Starter";
            this.Load += new System.EventHandler(this.frmMain_Load);
            this.Shown += new System.EventHandler(this.frmMain_Shown);
            ((System.ComponentModel.ISupportInitialize)(this.upDownTimeout)).EndInit();
            this.pnlTimeout.ResumeLayout(false);
            this.pnlTimeout.PerformLayout();
            this.statusBar.ResumeLayout(false);
            this.statusBar.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.upDownSessionID)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnStartProcess;
        private System.Windows.Forms.Label lblComputer;
        private System.Windows.Forms.Label lblUsername;
        private System.Windows.Forms.Label lblPassword;
        private System.Windows.Forms.Label lblProcessPath;
        private System.Windows.Forms.TextBox txtUsername;
        private System.Windows.Forms.TextBox txtPassword;
        private System.Windows.Forms.TextBox txtProcessPath;
        private System.Windows.Forms.TextBox txtArguments;
        private System.Windows.Forms.Label lblArguments;
        private System.Windows.Forms.NumericUpDown upDownTimeout;
        private System.Windows.Forms.Label lblTimeoutUnit;
        private System.Windows.Forms.Panel pnlTimeout;
        private System.Windows.Forms.CheckBox chkTimeout;
        private System.Windows.Forms.TextBox txtResult;
        private System.Windows.Forms.Label lblResult;
        private System.Windows.Forms.StatusStrip statusBar;
        private System.Windows.Forms.ToolStripStatusLabel sLblStatus;
        private System.Windows.Forms.CheckBox chkSessionID;
        private System.Windows.Forms.NumericUpDown upDownSessionID;
        private System.Windows.Forms.TextBox txtRunAs;
        private System.Windows.Forms.Label lblRunAs;
        private System.Windows.Forms.RadioButton optRunAsSameUser;
        private System.Windows.Forms.RadioButton optRunAsDifferentUser;
        private System.Windows.Forms.ComboBox cboComputer;
        private System.Windows.Forms.Button btnReloadComputers;
    }
}

