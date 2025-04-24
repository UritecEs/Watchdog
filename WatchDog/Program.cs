using System;
using WatchDog.TrayIconTest;
using System.Windows.Forms;
using System.Diagnostics;

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
			if (Process.GetProcessesByName(Process.GetCurrentProcess().ProcessName).Length <= 1)
			{
				Application.EnableVisualStyles();
				Application.SetCompatibleTextRenderingDefault(false);
				Application.Run(new TrayIcon());
			}
		}
	}
}
