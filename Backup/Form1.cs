using System;
using System.Windows.Forms;

namespace arcball
{
    public partial class Form1 : Form
    {

        private OGL glWindow = new OGL();

        public Form1()
        {
            InitializeComponent();

            this.glWindow.Parent = this;
            this.glWindow.Dock = DockStyle.Fill; // fill the parent form
            this.glWindow.MouseMove += new System.Windows.Forms.MouseEventHandler(this.glWindow.glOnMouseMove);

        }

        private void Form1_SizeChanged(object sender, EventArgs e)
        {
            this.glWindow.PlotGL();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            this.glWindow.PlotGL();
        }


    }
}