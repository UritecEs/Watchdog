﻿using System;
using System.IO;
using System.Windows.Forms;
using WatchdogLib;

namespace WatchDog
{
	public class EditFormVm
	{
		private readonly EditForm _editApplicationsForm;
		private ApplicationHandlerConfig _applicationHandlerConfig;

		public EditFormVm(EditForm editApplicationsForm, ApplicationHandlerConfig applicationHandlerConfig)
		{
			_editApplicationsForm = editApplicationsForm;

			_applicationHandlerConfig = applicationHandlerConfig;
			SetForm();

			_editApplicationsForm.buttonActivate.Click += ButtonActivateClick;
			_editApplicationsForm.buttonDeactivate.Click += ButtonDeactivateClick;

			_editApplicationsForm.buttonAcceptChanges.Click += ButtonAcceptClick;
		}


		private void SetActive(bool isActive)
		{
			_editApplicationsForm.buttonActivate.Enabled = !isActive;
			_editApplicationsForm.buttonDeactivate.Enabled = isActive;
			_applicationHandlerConfig.Active = isActive;
		}


		private void ButtonAcceptClick(object sender, EventArgs e)
		{
			if (!ValidateExe())
				return;

			AcceptChanges();
			_editApplicationsForm.Close();
		}


		private void AcceptChanges()
		{
			FillHandlerConfig();
		}

		/// <summary>
		/// Validates that the selected application is correct
		/// </summary>
		/// <returns>true if it's OK</returns>
		private bool ValidateExe()
		{
			string ruta = _editApplicationsForm.textBoxApplicationPath.Text.Trim('\"');
			if (string.IsNullOrWhiteSpace(ruta))
			{
				MessageBox.Show("You must select the program to watch");
				return false;
			}

			if (!File.Exists(ruta))
			{
				MessageBox.Show("The selected file doesn't exist");
				return false;
			}
			return true;
		}

		private void ButtonDeactivateClick(object sender, EventArgs e)
		{
			SetActive(false);
		}

		private void ButtonActivateClick(object sender, EventArgs e)
		{
			SetActive(true);
		}

		private void FillHandlerConfig()
		{
			_applicationHandlerConfig.NonResponsiveInterval = int.Parse(_editApplicationsForm.textBoxUnresponsiveInterval.Text);
			_applicationHandlerConfig.HeartbeatInterval = uint.Parse(_editApplicationsForm.textBoxHeartbeatInterval.Text);
			//_applicationHandlerConfig.MaxProcesses                    = int.Parse(_editApplicationsForm.textBoxMaxProcesses.Text)         ;
			//_applicationHandlerConfig.MinProcesses                    = int.Parse(_editApplicationsForm.textBoxMinProcesses.Text)         ;
			_applicationHandlerConfig.ApplicationPath = _editApplicationsForm.textBoxApplicationPath.Text.Trim('\"');
			if (string.IsNullOrWhiteSpace(_editApplicationsForm.textBoxProcessName.Text))
				_applicationHandlerConfig.ApplicationName = Path.GetFileNameWithoutExtension(_applicationHandlerConfig.ApplicationPath);
			else
				_applicationHandlerConfig.ApplicationName = _editApplicationsForm.textBoxProcessName.Text;
			_applicationHandlerConfig.UseHeartbeat = _editApplicationsForm.checkBoxUseHeartbeat.Checked;
			_applicationHandlerConfig.GrantKillRequest = _editApplicationsForm.checkBoxGrantKillRequest.Checked;
			_applicationHandlerConfig.StartupMonitorDelay = uint.Parse(_editApplicationsForm.textBoxStartupMonitorDelay.Text);
			_applicationHandlerConfig.Active = _editApplicationsForm.buttonDeactivate.Enabled;
		}

		//private void SetDefaultSettings()
		//{
		//    _editApplicationsForm.textBoxUnresponsiveInterval.Text = "30";
		//    _editApplicationsForm.textBoxHeartbeatInterval.Text    = "15";
		//    _editApplicationsForm.textBoxHeartbeatInterval.Text    = "15";
		//    _editApplicationsForm.textBoxMaxProcesses.Text         = "1";
		//    _editApplicationsForm.textBoxMinProcesses.Text         = "1";
		//    _editApplicationsForm.textBoxProcessName.Text          = "";
		//    _editApplicationsForm.textBoxApplicationPath.Text      = "";
		//    _editApplicationsForm.checkBoxUseHeartbeat.Checked     = false;
		//    _editApplicationsForm.checkBoxGrantKillRequest.Checked = true;
		//    _editApplicationsForm.textBoxStartupMonitorDelay.Text  = "30";
		//    _applicationHandlerConfig.Active 
		//}

		private void SetForm()
		{
			_editApplicationsForm.textBoxUnresponsiveInterval.Text = _applicationHandlerConfig.NonResponsiveInterval.ToString();
			_editApplicationsForm.textBoxHeartbeatInterval.Text = _applicationHandlerConfig.HeartbeatInterval.ToString();
			//_editApplicationsForm.textBoxMaxProcesses.Text         = _applicationHandlerConfig.MaxProcesses.ToString();
			//_editApplicationsForm.textBoxMinProcesses.Text         = _applicationHandlerConfig.MinProcesses.ToString();

			_editApplicationsForm.textBoxProcessName.Text = _applicationHandlerConfig.ApplicationName;
			_editApplicationsForm.textBoxApplicationPath.Text = _applicationHandlerConfig.ApplicationPath;
			_editApplicationsForm.checkBoxUseHeartbeat.Checked = _applicationHandlerConfig.UseHeartbeat;
			_editApplicationsForm.checkBoxGrantKillRequest.Checked = _applicationHandlerConfig.GrantKillRequest;
			_editApplicationsForm.textBoxStartupMonitorDelay.Text = _applicationHandlerConfig.StartupMonitorDelay.ToString();

			SetActive(_applicationHandlerConfig.Active);
		}

		private bool SettingsSame(ApplicationHandlerConfig applicationHandlerConfig)
		{
			return _editApplicationsForm.textBoxUnresponsiveInterval.Text == applicationHandlerConfig.NonResponsiveInterval.ToString() &&
				   _editApplicationsForm.textBoxHeartbeatInterval.Text == applicationHandlerConfig.HeartbeatInterval.ToString() &&
				   //_editApplicationsForm.textBoxMaxProcesses.Text          == applicationHandlerConfig.MaxProcesses.ToString()         &&
				   //_editApplicationsForm.textBoxMinProcesses.Text          == applicationHandlerConfig.MinProcesses.ToString()         &&
				   _editApplicationsForm.textBoxProcessName.Text == applicationHandlerConfig.ApplicationName &&
				   _editApplicationsForm.textBoxApplicationPath.Text == applicationHandlerConfig.ApplicationPath &&
				   _editApplicationsForm.checkBoxUseHeartbeat.Checked == applicationHandlerConfig.UseHeartbeat &&
				   _editApplicationsForm.checkBoxGrantKillRequest.Checked == applicationHandlerConfig.GrantKillRequest &&
				   _editApplicationsForm.textBoxStartupMonitorDelay.Text == applicationHandlerConfig.StartupMonitorDelay.ToString() &&
				   _editApplicationsForm.buttonDeactivate.Enabled == applicationHandlerConfig.Active;

		}

	}
}
