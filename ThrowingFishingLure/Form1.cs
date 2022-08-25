using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ThrowingFishingLure
{
    public partial class Form1 : Form
    {
        public SettingsWindow settingsWindow;
        public DisplayWindow displayWindow;

        public Form1()
        {
            InitializeComponent();
        }

        private void настройкиToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (settingsWindow == null)
            {
                settingsWindow = new SettingsWindow();
                settingsWindow.MdiParent = this;
                settingsWindow.FormClosed += new FormClosedEventHandler(SettingsWindow_FormClosed);
                settingsWindow.Show();
            }
            else
            {
                settingsWindow.Activate();
            }
        }

        private void SettingsWindow_FormClosed(object sender, FormClosedEventArgs e)
        {
            //throw new NotImplementedException();
            settingsWindow = null;
        }

        private void графикToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (displayWindow == null)
            {
                displayWindow = new DisplayWindow();
                displayWindow.MdiParent = this;
                displayWindow.FormClosed += new FormClosedEventHandler(DisplayWindow_FormClosed);
                displayWindow.Show();
            }
            else
            {
                displayWindow.Activate();
            }
        }
        public void DisplayWindow_FormClosed(object sender, FormClosedEventArgs e)
        {
            //throw new NotImplementedException();
            displayWindow = null;
        }

    }
}
