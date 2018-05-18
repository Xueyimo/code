using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.IO;
using CsGL.OpenGL;

namespace arcball
{
    public partial class _3Dshow : Form
    {
        Vector3f p_tag = new Vector3f();//选取的点
        Delaunay_2D de = new Delaunay_2D();
        well2d w1 = new well2d();
        int num;//表示当前行进的步数
        public _3Dshow(int step)
        {
            InitializeComponent();
            ogl1.MouseMove += new System.Windows.Forms.MouseEventHandler(ogl1.glOnMouseMove);
            ogl1.Dock = DockStyle.Fill;
            this.KeyDown += _3Dshow_KeyDown;
            this.MouseWheel += _3Dshow_MouseWheel;
            num = step;;
        }

        void _3Dshow_MouseDown(object sender, MouseEventArgs e)
        {
            MessageBox.Show("here");
            Point tempAux = new Point(e.X, e.Y);
            arcball arcBall = new arcball(640.0f, 480.0f);
            arcBall.mapToSphere(tempAux, p_tag);
        }

        void _3Dshow_KeyDown(object sender, KeyEventArgs e)
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

        void _3Dshow_MouseWheel(object sender, MouseEventArgs e)
        {

            float scale = 1.0f;
            if (e.Button != MouseButtons.Middle)
                if (e.Delta > 0)
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
        private void _3Dshow_SizeChanged(object sender, EventArgs e)
        {
            ogl1.PlotGL();
        }
        private void _3Dshow_Load(object sender, EventArgs e)
        {
            util u1=new util();
            FileStream stream = File.Open(u1.infopath, FileMode.OpenOrCreate, FileAccess.Write);
            stream.Seek(0, SeekOrigin.Begin);
            stream.SetLength(0);
            stream.Close();//清空文件内容
            w1.GetOGL(ogl1);
            w1.GenerateNetPo();
            de.GetOGL(ogl1);
            de.Initial(w1);
            de.SearchPoint(num);
            ogl1.PlotGL();
            list_box_add(de);
            add_text();
            this.KeyPreview = true;
        }
        private void ogl1_Click(object sender, EventArgs e)
        {

        }
        private void treeView1_AfterSelect(object sender, TreeViewEventArgs e)
        {

        }
        private void list_box_add(Delaunay_2D de0)
        {
            LinkList<PO> plt = de0.work_pl;
            Node<PO> pnt = plt.Head;
            int i = 0;
            listBox1.Items.Add("---"+(--num)+"---");
            while (pnt != null)
            {
                listBox1.Items.Add(i+pnt.Data.ToString());
                i++;
                pnt = pnt.Next;
            }
        }
        private void list(int num,Delaunay_2D de0)
        {
            Node<PO> pnt = de0.work_pl.Head;
            for(int i=0;i<num;i++)
            {
                if (pnt != null)
                    pnt = pnt.Next;
                else
                    MessageBox.Show("null is appearing!!");
            }
            if (listBox2.Items.Count > 0)
                listBox2.Items.Clear();
            Node<LINE> lnt = pnt.Data.ll.Head;
            int j = 0;
            while (lnt != null)
            {
                listBox2.Items.Add(j+lnt.Data.ToString());
                j++;
                lnt = lnt.Next;
            }
        }
        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            list(listBox1.SelectedIndex-1, de);
            util u1 = new util();
            PO[] pnc;
            pnc = u1.Get_po_from_string(listBox1.SelectedItem.ToString());
            ogl1.p_tags = pnc[0];
            ogl1.PlotGLKey();
        }
        private void add_text()
        {
            System.IO.StreamReader st;
            util u1=new util();
            st = new System.IO.StreamReader(u1.infopath, System.Text.Encoding.UTF8);
            string str = st.ReadToEnd();
            st.Close();
            textBox1.Text = str;
        }
        private void listBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            util u1 = new util();
            PO[] pnc;
            pnc=u1.Get_po_from_string(listBox2.SelectedItem.ToString());
            ogl1.l_tag = new LINE(pnc[0],pnc[1]);
            ogl1.PlotGLKey();
        }
        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void ogl1_Click_1(object sender, EventArgs e)
        {
            ogl1.Focus();
        }
    }
}
