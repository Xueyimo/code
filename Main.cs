using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace arcball
{
    public partial class Main : Form
    {
        public Main()
        {
            InitializeComponent();
        }

        private void dToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Set_step sp = new Set_step(this);
            sp.WindowState = FormWindowState.Maximized;
            sp.MdiParent = this;
            sp.Show();
        }

        private void menuStrip1_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {

        }
    }
}
