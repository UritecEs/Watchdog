﻿using System;
using System.Drawing;
using System.Windows.Forms;
using Utilities;
using WatchdogLib;

namespace WatchDog
{
	public class MainFormVm
	{
		private readonly MainForm _mainForm;
		private readonly ApplicationWatcher _applicationWatcher;
		private readonly Configuration _configuration;
		private ApplicationHandlerConfig _selectedItem;
		private int _selectedItemNo;
		private readonly ConfigurationSerializer<Configuration> _serializer;

		public MainFormVm(MainForm mainForm, ApplicationWatcher applicationWatcher, Configuration configuration, Utilities.ConfigurationSerializer<Configuration> serializer)
		{
			_mainForm = mainForm;
			_applicationWatcher = applicationWatcher;
			_configuration = configuration;
			_serializer = serializer;
			_selectedItem = null;
			// todo load item
			_mainForm.listBoxMonitoredApplications.SelectedIndexChanged += ListBoxMonitoredApplicationsSelectedIndexChanged;

			_mainForm.buttonAddProcess.Click += ButtonAddProcessClick;
			_mainForm.buttonDeleteProcess.Click += ButtonDeleteProcessOnClick;
			_mainForm.buttonEditProcess.Click += ButtonEditProcessClick;
			//_mainForm.buttonRebootSettings.Click += ButtonRebootSettingsClick;

			_mainForm.btnExitApp.Click += ButtonFinalizarClick;
			_mainForm.btnPause.Click += ButtonPararClick;
			_mainForm.btnContinue.Click += ButtonContinuarClick;

			foreach (var applicationHandlerConfig in configuration.ApplicationHandlers)
			{
				_mainForm.listBoxMonitoredApplications.Items.Add(applicationHandlerConfig.ApplicationName);
			}
			SelectMenuItemInList(0);

			applicationWatcher.HeartbeatEvent += ApplicationWatcherOnHeartbeat;
		}

		private void ApplicationWatcherOnHeartbeat(object sender, EventArgs e)
		{
			// si no tenemos handle creado entonces no intentaremos interactuar con los controles
			if (!_mainForm.IsHandleCreated)
				return;

			_mainForm.Invoke(new MethodInvoker(delegate
			{
				var i = _mainForm.listBoxMonitoredApplications.SelectedIndex;
				if (i < 0)
					return;

				var selectedApplication = _configuration.ApplicationHandlers[i];

				var app = (ApplicationHandler)sender;
				if (app.Id != selectedApplication.Id)
					return;

				try
				{
					_mainForm.lblLastHeartbeat.Text = DateTime.Now.ToString("HH:mm:ss");
				}
				catch (Exception)
				{
					//si la ventana no se ha mostrado entonces lanza excepción 
					// No se puede llamar a Invoke o a BeginInvoke en un control hasta que se haya creado el identificador de ventana.
				}
			}));
		}

		private void ButtonRebootSettingsClick(object sender, EventArgs e)
		{
			var rebootForm = new RebootForm();
			//var rebootFormVm = new RebootFormVm(editForm,applicationHandlerConfig, _configuration);
			rebootForm.ShowDialog(_mainForm);
		}


		private void ButtonAddProcessClick(object sender, EventArgs e)
		{
			var applicationHandlerConfig = new ApplicationHandlerConfig();
			var editForm = new EditForm();
			var editFormVm = new EditFormVm(editForm, applicationHandlerConfig);
			var result = editForm.ShowDialog(_mainForm);
			if (result != DialogResult.OK)
				return;

			_configuration.ApplicationHandlers.Add(applicationHandlerConfig);
			_serializer.Serialize(_configuration);
			_mainForm.listBoxMonitoredApplications.Items.Add(applicationHandlerConfig.ApplicationName);
			SelectMenuItemInList(_mainForm.listBoxMonitoredApplications.Items.Count - 1);
			SetForm(applicationHandlerConfig);
			_applicationWatcher.Add(applicationHandlerConfig);
		}

		private void ButtonEditProcessClick(object sender, EventArgs e)
		{
			var i = _mainForm.listBoxMonitoredApplications.SelectedIndex;
			if (i < 0)
			{
				MessageBox.Show("Please, select an element to edit.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
				return;
			}
			var applicationHandlerConfig = _configuration.ApplicationHandlers[i];

			var editForm = new EditForm();
			var editFormVm = new EditFormVm(editForm, applicationHandlerConfig);
			var result = editForm.ShowDialog(_mainForm);
			if (result != DialogResult.OK)
				return;

			_serializer.Serialize(_configuration);
			SetForm(applicationHandlerConfig);
			_mainForm.listBoxMonitoredApplications.Items[i] = _selectedItem.ApplicationName;
			_applicationWatcher.Update(applicationHandlerConfig);
		}

		private void ButtonDeleteProcessOnClick(object sender, EventArgs eventArgs)
		{
			var i = _mainForm.listBoxMonitoredApplications.SelectedIndex;
			if (i < 0)
			{
				MessageBox.Show("Please, select an element to remove.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
				return;
			}

			_applicationWatcher.Remove(_configuration.ApplicationHandlers[i]);

			_mainForm.listBoxMonitoredApplications.Items.RemoveAt(i);
			_configuration.ApplicationHandlers.RemoveAt(i);
			_serializer.Serialize(_configuration);

			if (_configuration.ApplicationHandlers.Count > 0)
			{
				i = Math.Max(0, i - 1);
				SelectMenuItemInList(i);
				SetForm(_configuration.ApplicationHandlers[i]);
			}
			else
			{
				_mainForm.textBoxProcessName.Text = string.Empty;
				_mainForm.textBoxApplicationPath.Text = string.Empty;
			}

		}

		/// <summary>
		/// Finalizar la aplicación
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="eventArgs"></param>
		private void ButtonFinalizarClick(object sender, EventArgs eventArgs)
		{
			Application.Exit();
		}

		/// <summary>
		/// Parar monitornización de todas las apps
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="eventArgs"></param>
		private void ButtonPararClick(object sender, EventArgs eventArgs)
		{
			_applicationWatcher.Pausar();
			_mainForm.btnContinue.Enabled = true;
			_mainForm.btnPause.Enabled = false;
			_mainForm.grpMonitoring.BackColor = Color.LightGray;
		}

		/// <summary>
		/// Continuar la monitorización de todas las apps
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="eventArgs"></param>
		private void ButtonContinuarClick(object sender, EventArgs eventArgs)
		{
			_applicationWatcher.Continuar();
			_mainForm.btnContinue.Enabled = false;
			_mainForm.btnPause.Enabled = true;
			_mainForm.grpMonitoring.BackColor = SystemColors.Control;
		}

		private void UpdateHandler()
		{
			// throw new NotImplementedException();
		}



		private void ButtonAcceptClick(object sender, EventArgs e)
		{
			AcceptChanges();
		}


		private void AcceptChanges()
		{
			//FillHandlerConfig(_selectedItem);
			var i = _mainForm.listBoxMonitoredApplications.SelectedIndex;
			_mainForm.listBoxMonitoredApplications.Items[i] = _selectedItem.ApplicationName;
		}



		private void SelectMenuItemInList(int i)
		{
			if (i < _mainForm.listBoxMonitoredApplications.Items.Count)
			{
				_mainForm.listBoxMonitoredApplications.SelectedIndex = i;
			}
		}

		private void SetSelectedItem(int i)
		{
			// Check old state
			//if (i != _selectedItemNo)
			//{
			//    // check if settings have changed, if so, ask for Accept
			//    //_selectedItem = _configuration.ApplicationHandlers[_selectedItemNo];
			//    if (!SettingsSame(_selectedItem))
			//    {
			//        var result = MessageBox.Show("This Application has not been applied. Do you want to apply and store it? If not, these updates will be lost",
			//            "Update Monitored Application",
			//            MessageBoxButtons.YesNo,
			//            MessageBoxIcon.Question);
			//        if (result == DialogResult.Yes)
			//        {
			//            AcceptChanges();
			//        }
			//    }
			//}


			if (i < _configuration.ApplicationHandlers.Count)
			{
				_selectedItem = _configuration.ApplicationHandlers[i];
				_selectedItemNo = i;

				SetForm(_selectedItem);
			}
		}



		//private void SetDefaultSettings()
		//{
		//    _mainForm.textBoxUnresponsiveInterval.Text = "30";
		//    _mainForm.textBoxHeartbeatInterval.Text    = "15";
		//    _mainForm.textBoxHeartbeatInterval.Text    = "15";
		//    _mainForm.textBoxMaxProcesses.Text         = "1";
		//    _mainForm.textBoxMinProcesses.Text         = "1";
		//    _mainForm.textBoxProcessName.Text          = "";
		//    _mainForm.textBoxApplicationPath.Text      = "";
		//    _mainForm.checkBoxUseHeartbeat.Checked     = false;
		//    _mainForm.checkBoxGrantKillRequest.Checked = true;
		//    _mainForm.textBoxStartupMonitorDelay.Text  = "30";
		//    applicationHandlerConfig.Active 
		//}

		/// <summary>
		/// Refresh the data in the form to show the selected application
		/// </summary>
		/// <param name="applicationHandlerConfig"></param>
		private void SetForm(ApplicationHandlerConfig applicationHandlerConfig)
		{
			_mainForm.textBoxProcessName.Text = applicationHandlerConfig.ApplicationName;
			_mainForm.textBoxApplicationPath.Text = applicationHandlerConfig.ApplicationPath;

			_mainForm.listBoxMonitoredApplications.SelectedItem = applicationHandlerConfig.ApplicationName;
			_mainForm.grpAplicacion.BackColor = applicationHandlerConfig.Active ? SystemColors.Control : Color.LightGray;

			if (applicationHandlerConfig.UseHeartbeat && applicationHandlerConfig.Active)
			{
				_mainForm.lblHearbeat.Visible = true;
				_mainForm.lblLastHeartbeat.Visible = true;
				_mainForm.lblLastHeartbeat.Text = "";
			}
			else
			{
				_mainForm.lblHearbeat.Visible = false;
				_mainForm.lblLastHeartbeat.Visible = false;
			}
		}

		private bool SettingsSame(ApplicationHandlerConfig applicationHandlerConfig)
		{
			return _mainForm.textBoxProcessName.Text == applicationHandlerConfig.ApplicationName &&
				   _mainForm.textBoxApplicationPath.Text == applicationHandlerConfig.ApplicationPath;
		}

		private void ListBoxMonitoredApplicationsSelectedIndexChanged(object sender, EventArgs e)
		{
			var index = _mainForm.listBoxMonitoredApplications.SelectedIndex;
			if (index < 0) return;
			SetSelectedItem(index);
		}


	}
}
