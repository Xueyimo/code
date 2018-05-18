using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using CsGL.OpenGL;

namespace arcball
{
    public partial class show3dmodel : Form
    {
        Delaunay de = new Delaunay();
        int num = 0;//运行步数
        well w1 = new well();
        public show3dmodel()
        {
            InitializeComponent();
            ogl1.MouseMove += new System.Windows.Forms.MouseEventHandler(ogl1.glOnMouseMove);
            this.KeyDown += Form1_KeyDown;
            this.MouseWheel += Form1_MouseWheel;
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
        private void show3dmodel_Load(object sender, EventArgs e)
        {
            w1.GetOGL(ogl1);
            w1.GenerateNetPo();
            w1.Show_ge_p_l();
            de.GetOGL(ogl1);
            de.Initial(w1);
            num = 10;
            de.SearchPoint(num);
            ogl1.PlotGL();
            list_box1();
            list_box2();
            this.KeyPreview = true;
        }
        private void list_box1(){
            Node<PO> pn = de.work_pl.Head;
            int i = 0;
            while(pn!=null){
                listBox1.Items.Add(":" + i + '(' + pn.Data.x + ',' + pn.Data.y + ',' + pn.Data.z + ')');
                i++;
                pn = pn.Next;
            }
        }
        private void list_box2()
        {
            Node<String> st = de.info.Head;
            listBox3.Items.Clear();
            while (st != null)
            {
                listBox4.Items.Add(st.Data);
                st = st.Next;
            }
        }
        private void list(int num, Delaunay de0)
        {
            Node<PO> pnt = de0.work_pl.Head;
            for (int i = 0; i < num; i++)
            {
                if (pnt != null)
                    pnt = pnt.Next;
                else
                {
                    MessageBox.Show("null is appearing!!");
                    return;
                }
            }
            if (listBox2.Items.Count > 0)
                listBox2.Items.Clear();
            Node<LINE> lnt = pnt.Data.ll.Head;
            int j = 0;
            while (lnt != null)
            {
                listBox2.Items.Add(j + lnt.Data.ToString());
                j++;
                lnt = lnt.Next;
            }
        }
        private void list2(int num1,int num2, Delaunay de0)
        {
            Node<PO> pnt = de0.work_pl.Head;
            for (int i = 0; i < num1; i++)
            {
                if (pnt != null)
                    pnt = pnt.Next;
                else
                {
                    MessageBox.Show("null is appearing!!");
                    return;
                }
            }
            Node<LINE> lnt = pnt.Data.ll.Head;
            for (int j = 0; j < num2; j++)
            {
                if (lnt != null)
                    lnt = lnt.Next;
                else
                {
                    MessageBox.Show("null is appearing!!");
                    return;
                }
            }
            Node<SURFACE> snt = lnt.Data.sl.Head;
            listBox3.Items.Clear();
            int k = 0;
            while (snt != null)
            {
                listBox3.Items.Add(":"+k +'('+ snt.Data.p1.x+ ',' + snt.Data.p1.y + ',' + snt.Data.p1.z + ')'
                + '(' + snt.Data.p2.x+ ',' + snt.Data.p2.y + ',' + snt.Data.p2.z + ')'
                + '(' + snt.Data.p3.x+ ',' + snt.Data.p3.y + ',' + snt.Data.p3.z + ')');
                k++;
                snt = snt.Next;
            }
        }
        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            list(listBox1.SelectedIndex, de);
            util u1 = new util();
            PO[] pnc;
            pnc = u1.Get_po_from_string(listBox1.SelectedItem.ToString());
            ogl1.p_tags = pnc[0];
            ogl1.PlotGLKey();
        }
        private void listBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            util u1 = new util();
            list2(listBox1.SelectedIndex,listBox2.SelectedIndex, de);
            PO[] pnc;
            pnc = u1.Get_po_from_string(listBox2.SelectedItem.ToString());
            ogl1.l_tag = new LINE(pnc[0], pnc[1]);
            ogl1.PlotGLKey();
            ogl1.s_tag = null;
        }
        private void listBox3_SelectedIndexChanged(object sender, EventArgs e)
        {
            util u1 = new util();
            PO[] pnc;
            pnc = u1.Get_po_from_string(listBox3.SelectedItem.ToString());
            ogl1.s_tag=new SURFACE(pnc[0],pnc[1],pnc[2]);
            ogl1.PlotGLKey();
        }
    }
}
