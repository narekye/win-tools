using Meshimer.Common;

namespace Meshimer.Scrapper.Service
{
    partial class ProjectInstaller
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

        #region Component Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.MeshimerProcessInstaller = new System.ServiceProcess.ServiceProcessInstaller();
            this.MeshimerInstaller = new System.ServiceProcess.ServiceInstaller();
            // 
            // MeshimerProcessInstaller
            // 
            this.MeshimerProcessInstaller.Account = System.ServiceProcess.ServiceAccount.LocalSystem;
            this.MeshimerProcessInstaller.Password = null;
            this.MeshimerProcessInstaller.Username = null;
            // 
            // ProjectInstaller
            // 
            this.Installers.AddRange(new System.Configuration.Install.Installer[] {
            this.MeshimerProcessInstaller,
            this.MeshimerInstaller});

        }

        #endregion

        private System.ServiceProcess.ServiceProcessInstaller MeshimerProcessInstaller;
        private System.ServiceProcess.ServiceInstaller MeshimerInstaller;
    }
}