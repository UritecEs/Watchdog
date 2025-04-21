namespace WatchDog
{
	partial class MainForm
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
			this.listBoxMonitoredApplications = new System.Windows.Forms.ListBox();
			this.buttonAddProcess = new System.Windows.Forms.Button();
			this.label1 = new System.Windows.Forms.Label();
			this.label2 = new System.Windows.Forms.Label();
			this.textBoxApplicationPath = new System.Windows.Forms.TextBox();
			this.textBoxProcessName = new System.Windows.Forms.TextBox();
			this.grpAplicacion = new System.Windows.Forms.GroupBox();
			this.buttonEditProcess = new System.Windows.Forms.Button();
			this.buttonDeleteProcess = new System.Windows.Forms.Button();
			this.grpMonitoring = new System.Windows.Forms.GroupBox();
			this.btnExitApp = new System.Windows.Forms.Button();
			this.btnContinue = new System.Windows.Forms.Button();
			this.btnPause = new System.Windows.Forms.Button();
			this.lblHearbeat = new System.Windows.Forms.Label();
			this.lblLastHeartbeat = new System.Windows.Forms.Label();
			this.grpAplicacion.SuspendLayout();
			this.grpMonitoring.SuspendLayout();
			this.SuspendLayout();
			// 
			// listBoxMonitoredApplications
			// 
			this.listBoxMonitoredApplications.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.listBoxMonitoredApplications.FormattingEnabled = true;
			this.listBoxMonitoredApplications.Location = new System.Drawing.Point(12, 4);
			this.listBoxMonitoredApplications.Name = "listBoxMonitoredApplications";
			this.listBoxMonitoredApplications.Size = new System.Drawing.Size(481, 95);
			this.listBoxMonitoredApplications.TabIndex = 0;
			// 
			// buttonAddProcess
			// 
			this.buttonAddProcess.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.buttonAddProcess.Location = new System.Drawing.Point(12, 104);
			this.buttonAddProcess.Name = "buttonAddProcess";
			this.buttonAddProcess.Size = new System.Drawing.Size(75, 23);
			this.buttonAddProcess.TabIndex = 1;
			this.buttonAddProcess.Text = "Add Application";
			this.buttonAddProcess.UseVisualStyleBackColor = true;
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(6, 31);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(29, 13);
			this.label1.TabIndex = 0;
			this.label1.Text = "Path";
			// 
			// label2
			// 
			this.label2.AutoSize = true;
			this.label2.Location = new System.Drawing.Point(6, 61);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(35, 13);
			this.label2.TabIndex = 1;
			this.label2.Text = "Name";
			// 
			// textBoxApplicationPath
			// 
			this.textBoxApplicationPath.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.textBoxApplicationPath.Enabled = false;
			this.textBoxApplicationPath.Location = new System.Drawing.Point(62, 28);
			this.textBoxApplicationPath.Name = "textBoxApplicationPath";
			this.textBoxApplicationPath.Size = new System.Drawing.Size(372, 20);
			this.textBoxApplicationPath.TabIndex = 2;
			// 
			// textBoxProcessName
			// 
			this.textBoxProcessName.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.textBoxProcessName.Enabled = false;
			this.textBoxProcessName.Location = new System.Drawing.Point(62, 58);
			this.textBoxProcessName.Name = "textBoxProcessName";
			this.textBoxProcessName.Size = new System.Drawing.Size(175, 20);
			this.textBoxProcessName.TabIndex = 3;
			// 
			// grpAplicacion
			// 
			this.grpAplicacion.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.grpAplicacion.Controls.Add(this.lblLastHeartbeat);
			this.grpAplicacion.Controls.Add(this.lblHearbeat);
			this.grpAplicacion.Controls.Add(this.textBoxProcessName);
			this.grpAplicacion.Controls.Add(this.textBoxApplicationPath);
			this.grpAplicacion.Controls.Add(this.label2);
			this.grpAplicacion.Controls.Add(this.label1);
			this.grpAplicacion.Location = new System.Drawing.Point(12, 142);
			this.grpAplicacion.Name = "grpAplicacion";
			this.grpAplicacion.Size = new System.Drawing.Size(481, 88);
			this.grpAplicacion.TabIndex = 2;
			this.grpAplicacion.TabStop = false;
			this.grpAplicacion.Text = "Application";
			// 
			// buttonEditProcess
			// 
			this.buttonEditProcess.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.buttonEditProcess.Location = new System.Drawing.Point(93, 104);
			this.buttonEditProcess.Name = "buttonEditProcess";
			this.buttonEditProcess.Size = new System.Drawing.Size(75, 23);
			this.buttonEditProcess.TabIndex = 3;
			this.buttonEditProcess.Text = "Edit";
			this.buttonEditProcess.UseVisualStyleBackColor = true;
			// 
			// buttonDeleteProcess
			// 
			this.buttonDeleteProcess.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.buttonDeleteProcess.Location = new System.Drawing.Point(174, 104);
			this.buttonDeleteProcess.Name = "buttonDeleteProcess";
			this.buttonDeleteProcess.Size = new System.Drawing.Size(75, 23);
			this.buttonDeleteProcess.TabIndex = 4;
			this.buttonDeleteProcess.Text = "Delete";
			this.buttonDeleteProcess.UseVisualStyleBackColor = true;
			// 
			// grpMonitoring
			// 
			this.grpMonitoring.BackColor = System.Drawing.SystemColors.Control;
			this.grpMonitoring.Controls.Add(this.btnExitApp);
			this.grpMonitoring.Controls.Add(this.btnContinue);
			this.grpMonitoring.Controls.Add(this.btnPause);
			this.grpMonitoring.Location = new System.Drawing.Point(13, 243);
			this.grpMonitoring.Name = "grpMonitoring";
			this.grpMonitoring.Size = new System.Drawing.Size(479, 74);
			this.grpMonitoring.TabIndex = 5;
			this.grpMonitoring.TabStop = false;
			this.grpMonitoring.Text = "Monitoring";
			// 
			// btnExitApp
			// 
			this.btnExitApp.Location = new System.Drawing.Point(384, 29);
			this.btnExitApp.Name = "btnExitApp";
			this.btnExitApp.Size = new System.Drawing.Size(75, 23);
			this.btnExitApp.TabIndex = 3;
			this.btnExitApp.Text = "Exit app";
			this.btnExitApp.UseVisualStyleBackColor = true;
			// 
			// btnContinue
			// 
			this.btnContinue.Location = new System.Drawing.Point(104, 29);
			this.btnContinue.Name = "btnContinue";
			this.btnContinue.Size = new System.Drawing.Size(75, 23);
			this.btnContinue.TabIndex = 1;
			this.btnContinue.Text = "Continue";
			this.btnContinue.UseVisualStyleBackColor = true;
			// 
			// btnPause
			// 
			this.btnPause.Location = new System.Drawing.Point(8, 29);
			this.btnPause.Name = "btnPause";
			this.btnPause.Size = new System.Drawing.Size(75, 23);
			this.btnPause.TabIndex = 0;
			this.btnPause.Text = "Pause";
			this.btnPause.UseVisualStyleBackColor = true;
			// 
			// lblHearbeat
			// 
			this.lblHearbeat.AutoSize = true;
			this.lblHearbeat.Location = new System.Drawing.Point(254, 61);
			this.lblHearbeat.Name = "lblHearbeat";
			this.lblHearbeat.Size = new System.Drawing.Size(54, 13);
			this.lblHearbeat.TabIndex = 4;
			this.lblHearbeat.Text = "Heartbeat";
			// 
			// lblLastHeartbeat
			// 
			this.lblLastHeartbeat.AutoSize = true;
			this.lblLastHeartbeat.Location = new System.Drawing.Point(314, 61);
			this.lblLastHeartbeat.Name = "lblLastHeartbeat";
			this.lblLastHeartbeat.Size = new System.Drawing.Size(35, 13);
			this.lblLastHeartbeat.TabIndex = 5;
			this.lblLastHeartbeat.Text = "label3";
			// 
			// MainForm
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(505, 332);
			this.Controls.Add(this.grpMonitoring);
			this.Controls.Add(this.buttonDeleteProcess);
			this.Controls.Add(this.buttonEditProcess);
			this.Controls.Add(this.grpAplicacion);
			this.Controls.Add(this.buttonAddProcess);
			this.Controls.Add(this.listBoxMonitoredApplications);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.MaximizeBox = false;
			this.Name = "MainForm";
			this.Text = "Watchdog";
			this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.MainFormFormClosing);
			this.grpAplicacion.ResumeLayout(false);
			this.grpAplicacion.PerformLayout();
			this.grpMonitoring.ResumeLayout(false);
			this.ResumeLayout(false);

		}

		#endregion

		public System.Windows.Forms.ListBox listBoxMonitoredApplications;
		public System.Windows.Forms.Button buttonAddProcess;
		public System.Windows.Forms.Label label1;
		public System.Windows.Forms.Label label2;
		public System.Windows.Forms.TextBox textBoxApplicationPath;
		public System.Windows.Forms.TextBox textBoxProcessName;
        public System.Windows.Forms.GroupBox grpAplicacion;
		public System.Windows.Forms.Button buttonEditProcess;
		public System.Windows.Forms.Button buttonDeleteProcess;
		public System.Windows.Forms.GroupBox grpMonitoring;
		public System.Windows.Forms.Button btnExitApp;
		public System.Windows.Forms.Button btnContinue;
		public System.Windows.Forms.Button btnPause;
		public System.Windows.Forms.Label lblHearbeat;
		public System.Windows.Forms.Label lblLastHeartbeat;
	}
}