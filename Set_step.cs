using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace arcball
{
    public partial class Set_step : Form
    {
        public int step;
        private Form fm;
        public Set_step(Form fmain)
        {
            InitializeComponent();
            fm=fmain;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            step = int.Parse(textBox1.Text);
            _3Dshow childForm = new _3Dshow(step);
            childForm.MdiParent =fm;
            childForm.WindowState = FormWindowState.Maximized;//窗体最大化
            childForm.Show();
            childForm.Dock = DockStyle.Fill;
        }
    }
}
