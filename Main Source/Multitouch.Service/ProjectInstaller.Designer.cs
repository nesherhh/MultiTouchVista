namespace Multitouch.Service
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
			this.multitouchServiceProcessInstaller = new System.ServiceProcess.ServiceProcessInstaller();
			this.multitouchServiceInstaller = new System.ServiceProcess.ServiceInstaller();
			// 
			// multitouchServiceProcessInstaller
			// 
			this.multitouchServiceProcessInstaller.Account = System.ServiceProcess.ServiceAccount.LocalService;
			this.multitouchServiceProcessInstaller.Password = null;
			this.multitouchServiceProcessInstaller.Username = null;
			// 
			// multitouchServiceInstaller
			// 
			this.multitouchServiceInstaller.Description = "Provides Multitouch input";
			this.multitouchServiceInstaller.DisplayName = "Multitouch input";
			this.multitouchServiceInstaller.ServiceName = "MultitouchService";
			// 
			// ProjectInstaller
			// 
			this.Installers.AddRange(new System.Configuration.Install.Installer[] {
            this.multitouchServiceProcessInstaller,
            this.multitouchServiceInstaller});

		}

		#endregion

		private System.ServiceProcess.ServiceProcessInstaller multitouchServiceProcessInstaller;
		private System.ServiceProcess.ServiceInstaller multitouchServiceInstaller;
	}
}