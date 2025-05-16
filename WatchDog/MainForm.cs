using System;
using System.Drawing;
using System.IO;
using System.Timers;
using System.Windows.Forms;
using WatchdogLib;

namespace WatchDog
{
	public partial class MainForm : Form
	{
		private System.Timers.Timer _timer;

		/// <summary>
		/// Initial seconds at Watchdog start before starting monitoring of apps
		/// </summary>
		private int WatchdogDelay = 5;

		public ApplicationWatcher ApplicationWatcher { get; set; }
		//public EventHandler ActivateEvent;
		public EventHandler HideEvent;

		public MainForm()
		{
			InitializeComponent();

			// Start countdown to begin monitoring
			_timer = new System.Timers.Timer(1000);
			_timer.Elapsed += OnTimedEvent;
			_timer.Enabled = true;

			btnPause.Click += BtnPause_Click;
			grpMonitoring.BackColor = Color.LightGray;
		}

		private void BtnPause_Click(object sender, EventArgs e)
		{
			// If we click pause before monitoring has started, leave it in manual mode
			btnPause.Click -= BtnPause_Click;
			_timer.Stop();
			Invoke(new MethodInvoker(delegate
			{
				btnContinue.Text = "Continue";
			}));
		}

		private void MainFormFormClosing(object sender, FormClosingEventArgs e)
		{
			if (e.CloseReason == CloseReason.UserClosing)
			{
				e.Cancel = true;
				Hide();
				HideEvent?.Invoke(this, null);
			}
		}

		private void OnTimedEvent(object sender, ElapsedEventArgs e)
		{
			WatchdogDelay -= 1;
			// When countdown reaches 0, start monitoring
			if (WatchdogDelay <= 0)
			{
				_timer.Stop();
				btnPause.Click -= BtnPause_Click;
				try
				{
					Invoke(new MethodInvoker(delegate
					{
						btnContinue.Text = "Continue";
						btnContinue.PerformClick();
					}));
					return;
				}
				catch (Exception)
				{
					ApplicationWatcher.Continuar();
					btnContinue.Enabled = false;
					grpMonitoring.BackColor = SystemColors.Control;
				}
			}
			if (IsHandleCreated)
			{
				Invoke(new MethodInvoker(delegate
				{
					btnContinue.Text = "Start in " + WatchdogDelay;
				}));
			} else
			{
				btnContinue.Text = "Start in " + WatchdogDelay;
			}
		}

		//private void ButtonActivateClick(object sender, EventArgs e)
		//{
		//    if (ActivateEvent != null) ActivateEvent(this,null);
		//}

		//private void ButtonDeactivateClick(object sender, EventArgs e)
		//{
		//    if (DeactivateEvent != null) DeactivateEvent(this, null);
		//}

		//private void buttonAddProcess_Click(object sender, EventArgs e)
		//{
		//    if (AddEvent != null) DeactivateEvent(this, null);
		//}
	}
}
