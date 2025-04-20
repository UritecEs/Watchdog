using System;
using System.Windows.Forms;
using Utilities.Controls;

namespace WatchDog
{
    public partial class LogForm : Form
    {
		public EventHandler HideEvent;
        public LoggingView LoggingView { get { return loggingView;  } }

        public LogForm()
        {
            InitializeComponent();
        }

        private void LogFormFormClosing(object sender, FormClosingEventArgs e)
        {
            if (e.CloseReason == CloseReason.UserClosing)
            {
                e.Cancel = true;
                Hide();
				HideEvent?.Invoke(this, null);
            }
        }

        private void LogFormResize(object sender, EventArgs e)
        {

            if (WindowState == FormWindowState.Minimized)
            {
                this.Hide();
				HideEvent?.Invoke(this, null);
            }
        }
    
    }
}
