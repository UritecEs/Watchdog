using System;
using WatchDog.TrayIconTest;
using System.Windows.Forms;
using System.Diagnostics;
using NLog;

namespace WatchDog
{
	/// <summary>
	/// The Watchdog Application
	/// </summary>
	/// 
	class Program
	{
		[STAThread]
		static void Main(string[] args)
		{
			ExceptionsManager.Logger = null;
			ExceptionsManager.TrayIcon = null;

			// Run only one instance
			if (Process.GetProcessesByName(Process.GetCurrentProcess().ProcessName).Length > 1)
			{
				LogManager.GetLogger("WatchdogServer").Debug("There's another instance already running. Don't start new one.");
				LogManager.Flush();
				return;
			}

			Application.EnableVisualStyles();
			Application.SetCompatibleTextRenderingDefault(false);
			Application.Run(new TrayIcon());
		}
	}
}
