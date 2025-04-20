using System;
using System.IO;
using System.Windows.Forms;

namespace WatchDog
{
    public partial class EditForm : Form
    {
        public EventHandler ActivateEvent;
        public EditForm()
        {
            InitializeComponent();
        }

        private void ButtonSelectFileClick(object sender, EventArgs e)
        {

            var openFileDialog1 = new OpenFileDialog
            {
                Filter = "executable files |*.exe;*.com;*.bat|All files|*.*",

                RestoreDirectory = true
            };


            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    var filenamePath = openFileDialog1.FileName;

                    if (File.Exists(filenamePath))
                    {
                        textBoxApplicationPath.Text = filenamePath;
                        textBoxProcessName.Text          = System.IO.Path.GetFileNameWithoutExtension(filenamePath);//  FileUtils.GetBaseName(filenamePath)
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error: Could not read file from disk. Original error: " + ex.Message);
                }
            }
        }

        private void MainFormFormClosing(object sender, FormClosingEventArgs e)
        {
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
