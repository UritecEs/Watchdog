using System;
using WatchDog.TrayIconTest;
using System.Windows.Forms;

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
			Application.EnableVisualStyles();
			Application.SetCompatibleTextRenderingDefault(false);
			Application.Run(new TrayIcon());
		}
	}
}
