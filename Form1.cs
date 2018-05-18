using System;
using System.Collections;
using System.Windows.Forms;
using System.Drawing;
using CsGL.OpenGL;

namespace arcball
{
    public partial class Form1 : Form
    {
        Vector3f p_tag = new Vector3f();//选取的点
        Delaunay_2D de = new Delaunay_2D();
        well2d w1 = new well2d();

        public Form1()
        {
            InitializeComponent();
            ogl1.MouseMove += new System.Windows.Forms.MouseEventHandler(ogl1.glOnMouseMove);
            this.KeyDown += Form1_KeyDown;
            this.MouseWheel += Form1_MouseWheel;
        }

        void Form1_MouseDown(object sender, MouseEventArgs e)
        {
            MessageBox.Show("here");
            Point tempAux = new Point(e.X, e.Y);
            arcball arcBall=new arcball (640.0f, 480.0f);
            arcBall.mapToSphere(tempAux,p_tag);
            textBox5.Text = p_tag.x.ToString();
            textBox1.Text = p_tag.y.ToString();
            textBox7.Text = p_tag.z.ToString();
        }

        void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            //if escape was pressed, exit the application
            if (e.KeyCode == Keys.Escape)
            {
                this.Dispose();
            }
            //if R was pressed, reset
            if (e.KeyCode == Keys.R)
            {
                ogl1.reset();
            }          
        }

        void Form1_MouseWheel(object sender, MouseEventArgs e)
        {
            
            float scale=1.0f;
            if(e.Button!=MouseButtons.Middle)
            if(e.Delta >0)
            {
                scale = 0.9f;
            }
            else
            {
                scale = 1.1f;
            }
            GL.glScalef(scale, scale, scale);
            ogl1.PlotGLKey();
        }
        private void Form1_SizeChanged(object sender, EventArgs e)
        {
            ogl1.PlotGL();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            w1.GetOGL(ogl1);
            w1.GenerateNetPo();
            de.GetOGL(ogl1);
            de.Initial(w1);
            de.SearchPoint(3);
            ogl1.PlotGL();
            this.KeyPreview = true;
            //PO p0,p1,p2,p4;
            //p0 = new PO(1,2,3);
            //p1 = new PO(-1,-2,-3);
            //p2 = new PO(2,4,5);
            //p4 = new PO(0,1,2);
            //util u1 = new util();
            //u1.InFile(u1.infopath,u1.Cal_surface_angle(p0,p1,p2,p4));
        }
        private void button1_Click(object sender, EventArgs e)
        {
            PO p1, p2, p3;
            p1 = new PO();
            p2 = new PO();
            p3 = new PO();
            p1.x = float.Parse(textBox5.Text);
            p1.y = float.Parse(textBox1.Text);
            p1.z = float.Parse(textBox7.Text);
            p2.x = float.Parse(textBox4.Text);
            p2.y = float.Parse(textBox2.Text);
            p2.z = float.Parse(textBox8.Text);
            p3.x = float.Parse(textBox6.Text);
            p3.y = float.Parse(textBox3.Text);
            p3.z = float.Parse(textBox9.Text);
            ogl1.Seek_Tag(p1,p2,p3);
        }
         private void clear_Click(object sender, EventArgs e)
        {

            textBox1.Text = "";
            textBox2.Text = "";
            textBox3.Text = "";
            textBox4.Text = "";
            textBox5.Text = "";
            textBox6.Text = "";
            textBox7.Text = "";
            textBox8.Text = "";
            textBox9.Text = "";
        }

        private void button2_Click(object sender, EventArgs e)
        {
            PO p1, p2;
            p1 = new PO();
            p2 = new PO();
            p1.x = float.Parse(textBox14.Text);
            p1.y = float.Parse(textBox15.Text);
            p1.z = float.Parse(textBox13.Text);
            p2.x = float.Parse(textBox11.Text);
            p2.y = float.Parse(textBox12.Text);
            p2.z = float.Parse(textBox10.Text);
            ogl1.Seek_Tag_LINE(p1, p2);
        }

        private void button1_Click_1(object sender, EventArgs e)
        {
            textBox14.Text = "";
            textBox15.Text = "";
            textBox13.Text = "";
            textBox11.Text = "";
            textBox12.Text = "";
            textBox10.Text = "";
        }

        private void button3_Click(object sender, EventArgs e)
        {
            ogl1.sl_tag.Clear();
            ogl1.l_tag=null;
            ogl1 .p_tags =null;
            ogl1.PlotGL();
        }

        private void label12_Click(object sender, EventArgs e)
        {

        }

        private void button5_Click(object sender, EventArgs e)
        {
            PO p1;
            p1 = new PO();
            p1.x = float.Parse(textBox17.Text);
            p1.y = float.Parse(textBox18.Text);
            p1.z = float.Parse(textBox16.Text);
            ogl1.Seek_Tag_PO(p1);
        }
        private void button4_Click(object sender, EventArgs e)
        {
            textBox17.Text = "";
            textBox18.Text = "";
            textBox16.Text = "";
        }

        private void ogl1_Click(object sender, EventArgs e)
        {

        }
    }
}