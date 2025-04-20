using System;
using System.IO;
using System.Windows.Forms;

namespace WatchDog
{
    public partial class MainForm : Form
    {
		//public EventHandler ActivateEvent;
		public EventHandler HideEvent;
        public MainForm()
        {
            InitializeComponent();
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
