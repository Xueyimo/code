using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Text;
using System.Collections;
using System.Drawing;
using System.Windows.Forms;
using CsGL.OpenGL;
using System.IO;

namespace arcball
{
    public partial class csgl : Component
    {
        public csgl()
        {
            InitializeComponent();
        }

        public csgl(IContainer container)
        {
            container.Add(this);

            InitializeComponent();
        }

        private void tableLayoutPanel1_Paint(object sender, PaintEventArgs e)
        {

        }

    }
    #region OpenGL Control
    public class OGL : OpenGLControl
    {
        #region variable definitions

        private Point mouseStartDrag;
        private Point mouseEndDrag;
        private static int height = 0;
        private static int width = 0;

        private System.Object matrixLock = new System.Object();
        private arcball arcBall = new arcball(640.0f, 480.0f);
        private float[] matrix = new float[16];
        private float ThisScale = 1.0f;
        private float LastScale = 1.0f;
        private Matrix4f LastRot = new Matrix4f();
        private Matrix4f ThisRot = new Matrix4f();

        private static bool isLeftDrag = false;
        private static bool isRightDrag = false;
        private static bool isMiddleDrag = false;

        public LinkList<PO> pl;
        public LinkList<PO> pl_tag;
        public LinkList<SURFACE> sl,sl_tag;//sl_tag用于标定要寻找的面
        private LinkList<LINE> ll;

        public LINE l_tag;//将要被标记的边
        public SURFACE s_tag;//将要被标记的面

        private Vector3f  p_tag;//记录被标记的点
        public PO p_tags;//记录被标记的点

        #endregion

        public OGL()
            : base()
        {
            GC.Collect();
            pl = new LinkList<PO>();
            ll = new LinkList<LINE>();
            sl = new LinkList<SURFACE>();
            sl_tag = new LinkList<SURFACE>();
        }

        protected override void InitGLContext()
        {

            GL.glShadeModel(GL.GL_SMOOTH);								// enable smooth shading
            GL.glClearColor(0.0f, 0.0f, 0.0f, 0.5f);					// black background
            GL.glClearDepth(1.0f);										// depth buffer setup
            GL.glEnable(GL.GL_DEPTH_TEST);								// enables depth testing
            GL.glDepthFunc(GL.GL_LEQUAL);								// type of depth testing
            // GL.glHint(GL.GL_PERSPECTIVE_CORRECTION_HINT, GL.GL_NICEST);	// nice perspective calculations
            //GL.glPointSize(4.0f);

            GL.gluPerspective(25.0, (double)width / (double)height, 1.0, 15.0);
            //GL.glTranslatef(0.0f, 0.0f, -4.0f);
            //GL.gluLookAt(0.0,100.0,600.0,0.0,0.0,0.0,0.0,1.0,0.0);

            GL.glEnable(GL.GL_LIGHTING);                                            // Enable Lighting
            GL.glEnable(GL.GL_LIGHT0);                                            // Enable Default Light

            GL.glEnable(GL.GL_COLOR_MATERIAL);   // Enable Color Material
            GL.glLoadIdentity();

            LastRot.setIdentity(); // Reset Rotation
            ThisRot.setIdentity(); // Reset Rotation
            ThisRot.get_Renamed(matrix);

            LastScale = 1.0f;
            ThisScale = 1.0f;



            #region mouse handles
            MouseControl mouseControl = new MouseControl(this);
            mouseControl.AddControl(this);
            mouseControl.LeftMouseDown += new MouseEventHandler(glOnLeftMouseDown);
            mouseControl.LeftMouseUp += new MouseEventHandler(glOnLeftMouseUp);

            mouseControl.RightMouseDown += new MouseEventHandler(glOnRightMouseDown);
            mouseControl.RightMouseUp += new MouseEventHandler(glOnRightMouseUp);
            mouseControl.MiddleMouseDown += new MouseEventHandler(glOnMiddleMouseDown);
            mouseControl.MiddleMouseUp += new MouseEventHandler(glOnMiddleMouseUp);

            #endregion
        }

        protected override void OnSizeChanged(EventArgs e)
        {
            base.OnSizeChanged(e);
            Size s = Size;
            height = s.Height;
            width = s.Width;

            GL.glViewport(0, 0, width, height);

            GL.glPushMatrix();
            GL.glMatrixMode(GL.GL_PROJECTION);
            GL.glLoadIdentity();
            GL.gluPerspective(25.0, (double)width / (double)height, 1.0, 15.0);
            GL.glTranslatef(0.0f, 0.0f, -4.0f);
            GL.glMatrixMode(GL.GL_MODELVIEW);
            GL.glPopMatrix();
            arcBall.setBounds((float)width, (float)height); //*NEW* Update mouse bounds for arcball
        }

        #region CsGL - Plot Here
        public void GetPL(PO pl1)
        {
            pl.Insert(pl1);
            return;
        }
        public void SetPL(LinkList<PO> pl)
        {
            pl_tag = pl;
            return;
        }
        public void GetLL(LINE l0)
        {
            ll.Insert(l0);
            return;
        }
        public void GetSL(SURFACE s0)
        {
            sl.Insert(s0);
            return;
        }
        public bool Equal(PO p1,PO p2)
        {
            if (System.Math.Abs(p1.x - p2.x) < 0.1 && System.Math.Abs(p1.y - p2.y) < 0.1 && System.Math.Abs(p1.z - p2.z) < 0.1)
                return true;
            else
                return false;
        }
        public void Seek_Tag(PO p1,PO p2,PO p3)
        {
            Node<SURFACE> sn = sl.Head;
            while(sn!=null)
            {
                if (Equal(sn.Data.p1, p1) && Equal(sn.Data.p2, p2) && Equal(sn.Data.p3, p3))
                    break;
                if (Equal(sn.Data.p1, p1) && Equal(sn.Data.p2, p3) && Equal(sn.Data.p3, p1))
                    break;
                if (Equal(sn.Data.p1, p2) && Equal(sn.Data.p2, p1) && Equal(sn.Data.p3, p3))
                    break;
                if (Equal(sn.Data.p1, p2) && Equal(sn.Data.p2, p3) && Equal(sn.Data.p3, p1))
                    break;
                if (Equal(sn.Data.p1, p3) && Equal(sn.Data.p2, p2) && Equal(sn.Data.p3, p1))
                    break;
                if (Equal(sn.Data.p1, p3) && Equal(sn.Data.p2, p1) && Equal(sn.Data.p3, p2))
                    break;
                sn = sn.Next;
            }
            if (sn != null)
            {
                sl_tag.Insert(sn.Data);
                this.PlotGLKey();
            }
            else
                MessageBox.Show("no Existing!!");
        }
        public void Seek_Tag_LINE(PO p1, PO p2)
        {
            Node<LINE> ln = ll.Head;
            while (ln != null)
            {
                if (Equal(ln.Data.p1, p1) && Equal(ln.Data.p2, p2))
                    break;
                if (Equal(ln.Data.p1, p2) && Equal(ln.Data.p2, p1))
                    break;
                ln = ln.Next;
            }
            if (ln != null)
            {
                l_tag = ln.Data;
                this.PlotGLKey();
            }
            else
                MessageBox.Show("no Existing!!");
        }
        public void Seek_Tag_PO(PO p0)
        {
            Node<PO> pn = pl_tag.Head;
            while (pn != null)
            {
                if (Equal(pn.Data,p0))
                    break;
                pn = pn.Next;
            }
            if (pn != null)
            {
                p_tags = pn.Data;
                //p_tags.ll.Dispaly();
                this.PlotGLKey();
            }
            else
                MessageBox.Show("no Existing!!");
        }
        public void Map_to_Space(int x,int y)
        {
            unsafe
            {
                double* modelview=stackalloc double[16];
                double* projection=stackalloc double[16];
                int* viewport =stackalloc int[16];
                PO pn, pf;
                pn = new PO();
                pf = new PO();
                GLU.glGetDoublev(GLU.GL_MODELVIEW_MATRIX, modelview);
                GLU.glGetDoublev(GLU.GL_PROJECTION_MATRIX, projection);
                GLU.glGetIntegerv(GLU.GL_VIEWPORT,viewport);
                double world_x, world_y, world_z;
                GLU.gluUnProject((double)x,(double)y,0.0,
                    modelview ,projection,viewport ,
                    &world_x,&world_y,&world_z);
                pn.x = (float)world_x;
                pn.y = (float)world_y;
                pn.z = (float)world_z;
                GLU.gluUnProject((double)x, (double)y, 1.0,
                   modelview, projection, viewport,
                   &world_x, &world_y, &world_z);
                pf.x = (float)world_x;
                pf.y = (float)world_y;
                pf.z = (float)world_z;
                ll.Insert(new LINE(pn,pf));
            }

        }
        public void PlotGL()
        {
            try
            {
                lock (matrixLock)
                {
                    ThisRot.get_Renamed(matrix);
                }
                GL.glClear(GL.GL_COLOR_BUFFER_BIT | GL.GL_DEPTH_BUFFER_BIT); // Clear screen and DepthBuffer

                GL.glPushMatrix();                  // NEW: Prepare Dynamic Transform
                GL.glMultMatrixf(matrix);           // NEW: Apply Dynamic Transform

                GL.glPolygonMode(GL.GL_FRONT_AND_BACK, GL.GL_FILL);

                #region plot something

                GL.glColor3f(0.8f, 0.3f, 0.1f);

                this.display();


                #endregion plot something

                GL.glPopMatrix(); // NEW: Unapply Dynamic Transform
                GL.glFlush();     // Flush the GL Rendering Pipeline
                this.Invalidate();

            }
            catch
            {
                return;
            }

        }
        public void PlotGLKey()
        {
            try
            {
                GL.glClear(GL.GL_COLOR_BUFFER_BIT | GL.GL_DEPTH_BUFFER_BIT); // Clear screen and DepthBuffer
                GL.glPushMatrix();                  // NEW: Prepare Dynamic Transform

                GL.glMultMatrixf(matrix);


                GL.glPolygonMode(GL.GL_FRONT_AND_BACK, GL.GL_FILL);

                #region plot something

                GL.glColor3f(0.8f, 0.3f, 0.1f);

                this.display();


                #endregion plot something

                GL.glPopMatrix(); // NEW: Unapply Dynamic Transform
                GL.glFlush();     // Flush the GL Rendering Pipeline
                this.Invalidate();

            }
            catch (Exception e)
            {
                return;
            }

        }
        private void torus(float MinorRadius, float MajorRadius)                    // Draw A Torus With Normals
        {
            int i, j;
            int stacks = 20;
            int slices = 20;
            GL.glBegin(GL.GL_TRIANGLE_STRIP);                                    // Start A Triangle Strip
            for (i = 0; i < stacks; i++)                                        // Stacks
            {
                for (j = -1; j < slices; j++)                                    // Slices
                {
                    float wrapFrac = (j % stacks) / (float)slices;
                    double phi = Math.PI * 2.0 * wrapFrac;
                    float sinphi = (float)(Math.Sin(phi));
                    float cosphi = (float)(Math.Cos(phi));

                    float r = MajorRadius + MinorRadius * cosphi;

                    GL.glNormal3d(
                            (Math.Sin(Math.PI * 2.0 * (i % stacks + wrapFrac) / (float)slices)) * cosphi,
                            sinphi,
                            (Math.Cos(Math.PI * 2.0 * (i % stacks + wrapFrac) / (float)slices)) * cosphi);
                    GL.glVertex3d(
                            (Math.Sin(Math.PI * 2.0 * (i % stacks + wrapFrac) / (float)slices)) * r,
                            MinorRadius * sinphi,
                            (Math.Cos(Math.PI * 2.0 * (i % stacks + wrapFrac) / (float)slices)) * r);

                    GL.glNormal3d(
                            (Math.Sin(Math.PI * 2.0 * (i + 1 % stacks + wrapFrac) / (float)slices)) * cosphi,
                            sinphi,
                            (Math.Cos(Math.PI * 2.0 * (i + 1 % stacks + wrapFrac) / (float)slices)) * cosphi);
                    GL.glVertex3d(
                            (Math.Sin(Math.PI * 2.0 * (i + 1 % stacks + wrapFrac) / (float)slices)) * r,
                            MinorRadius * sinphi,
                            (Math.Cos(Math.PI * 2.0 * (i + 1 % stacks + wrapFrac) / (float)slices)) * r);
                }
            }
            GL.glEnd();                                                        // Done Torus
        }
        private void display()
        {
            util u1 = new util();
            //po_tag
            if (p_tags != null)
            {
                GLU.glPointSize(5);
                GLU.glColor3f(1.0f, 0.0f, 0.0f);
                GLU.glBegin(GLU.GL_POINTS);
                GLU.glVertex3f(p_tags.x, p_tags.y, p_tags.z);
                GLU.glEnd();
                Node<LINE> ln_tags = p_tags.ll.Head;
                while (ln_tags != null)
                {
                    GLU.glColor3f(1.0f, 0.0f, 0.0f);
                    GLU.glBegin(GLU.GL_LINES);
                    GLU.glVertex3f(ln_tags.Data.p1.x, ln_tags.Data.p1.y, ln_tags.Data.p1.z);
                    GLU.glVertex3f(ln_tags.Data.p2.x, ln_tags.Data.p2.y, ln_tags.Data.p2.z);
                    GLU.glEnd();
                    ln_tags = ln_tags.Next;
                }
            }
            Node<PO> nt = pl.Head;
            //StreamWriter sw = new StreamWriter("e:/e.txt",true);
            GLU.glColor3f(1.0f, 0.0f, 0.0f);
            GLU.glBegin(GLU.GL_LINE_LOOP);
            GLU.glVertex3f(0, 0, 0);
            GLU.glVertex3f(0, 2, 0);
            GLU.glVertex3f(2, 0, 0);
            GLU.glEnd();

            GLU.glPointSize(3);
            GLU.glColor3f(1.0f, 0.0f, 0.0f);
            GLU.glBegin(GLU.GL_LINES);
            GLU.glVertex3f(0, 0, 0);
            GLU.glVertex3f(0, 1, 0);
            GLU.glEnd();

            GLU.glBegin(GLU.GL_LINES);
            GLU.glVertex3f(0, 0, 0);
            GLU.glVertex3f(1, 0, 0);
            GLU.glEnd();

            GLU.glBegin(GLU.GL_LINES);
            GLU.glVertex3f(0, 0, 0);
            GLU.glVertex3f(0, 0, 1);
            GLU.glEnd();
            while (nt != null)
            {
                GLU.glPointSize(3);
                GLU.glColor3f(0.0f, 1.0f, 1.0f);
                GLU.glBegin(GLU.GL_POINTS);
                GLU.glVertex3f(nt.Data.x, nt.Data.y, nt.Data.z);
                GLU.glEnd();
                //sw.WriteLine("({0},{1},{2})", nt.Data.x, nt.Data.y, nt.Data.z);
                nt = nt.Next;
            }
            Node<LINE> ln = ll.Head;
            while (ln != null)
            {
                GLU.glColor3f(0.0f, 1.0f, 1.0f);
                GLU.glBegin(GLU.GL_LINES);
                GLU.glVertex3f(ln.Data.p1.x, ln.Data.p1.y, ln.Data.p1.z);
                GLU.glVertex3f(ln.Data.p2.x, ln.Data.p2.y, ln.Data.p2.z);
                GLU.glEnd();
                ln = ln.Next;
            }
            if (s_tag != null)
            {
                GLU.glColor3f(1.0f, 0.0f, 0.0f);
                GLU.glBegin(GLU.GL_LINE_LOOP);
                GLU.glVertex3f(s_tag.p1.x, s_tag.p1.y, s_tag.p1.z);
                GLU.glVertex3f(s_tag.p2.x, s_tag.p2.y, s_tag.p2.z);
                GLU.glVertex3f(s_tag.p3.x, s_tag.p3.y, s_tag.p3.z);
                GLU.glEnd();
            }
            Node<SURFACE> sn = sl_tag.Head;
            while (sn != null)
            {
                GLU.glColor3f(1.0f, 0.0f, 0.0f);
                GLU.glBegin(GLU.GL_LINE_LOOP);
                GLU.glVertex3f(sn.Data.p1.x, sn.Data.p1.y, sn.Data.p1.z);
                GLU.glVertex3f(sn.Data.p2.x, sn.Data.p2.y, sn.Data.p2.z);
                GLU.glVertex3f(sn.Data.p3.x, sn.Data.p3.y, sn.Data.p3.z);
                GLU.glEnd();
                sn = sn.Next;
            }
            if (l_tag != null)
            {
                GLU.glColor3f(1.0f, 0.0f, 0.0f);
                GLU.glBegin(GLU.GL_LINES);
                GLU.glVertex3f(l_tag.p1.x, l_tag.p1.y, l_tag.p1.z);
                GLU.glVertex3f(l_tag.p2.x, l_tag.p2.y, l_tag.p2.z);
                GLU.glEnd();               
            }
        }
        public void reset()
        {
            lock (matrixLock)
            {
                LastRot.setIdentity();                                // Reset Rotation
                ThisRot.setIdentity();                                // Reset Rotation
                GLU.glLoadIdentity();
            }

            this.PlotGL();
        }

        #endregion CsGL

        #region Mouse Control
        private void startDrag(Point MousePt)
        {
            lock (matrixLock)
            {
                LastRot.set_Renamed(ThisRot); // Set Last Static Rotation To Last Dynamic One
            }
            arcBall.click(MousePt); // Update Start Vector And Prepare For Dragging

            mouseStartDrag = MousePt;

        }

        private void drag(Point MousePt)
        {
            Quat4f ThisQuat = new Quat4f();

            arcBall.drag(MousePt, ThisQuat); // Update End Vector And Get Rotation As Quaternion

            lock (matrixLock)
            {
                //lock 的目的很明确：就是不想让别人使用这段代码，
                //体现在多线程情况下，只允许当前线程执行该代码区域，
                //其他线程等待直到该线程执行结束；这样可以多线程避免同时使用某一方法造成数据混乱。
                ThisRot.Rotation = ThisQuat; // Convert Quaternion Into Matrix3fT
                ThisRot.mul(ThisRot, LastRot); // Accumulate Last Rotation Into This One
            }
        }
        private void drag_Transfer(Point MousePt)
        {
            Quat4f ThisQuat = new Quat4f();

            arcBall.drag(MousePt, ThisQuat); // Update End Vector And Get Rotation As Quaternion

            lock (matrixLock)
            {
                //lock 的目的很明确：就是不想让别人使用这段代码，
                //体现在多线程情况下，只允许当前线程执行该代码区域，
                //其他线程等待直到该线程执行结束；这样可以多线程避免同时使用某一方法造成数据混乱。
                ThisRot.Transfer = ThisQuat; // Convert Quaternion Into Matrix3fT
                ThisRot.mul(ThisRot, LastRot); // Accumulate Last Rotation Into This One
            }
        }


        public void glOnMouseMove(object sender, MouseEventArgs e)
        {
            Point tempAux = new Point(e.X, e.Y);
            PO vec = new PO();
            mouseEndDrag = tempAux;

            if (isLeftDrag)
            {
                this.drag(tempAux);
                this.PlotGL();
            }

            if (isRightDrag)
            {               
                
            }

            if(isMiddleDrag)
            {
                arcBall.drag_transfer_vector(tempAux,vec);
                GL.glTranslatef(vec.x,vec.y,vec.z);
                this.PlotGLKey();
            }
            
        }
        public void Get_P_tag(Vector3f p_taged)
        {
            p_taged.x = p_tag.x;
            p_taged.y = p_tag.y;
            p_taged.z = p_tag.z;
        }
        public void glOnLeftMouseDown(object sender, MouseEventArgs e)
        {
            isLeftDrag = true;
            Point tempAux = new Point(e.X, e.Y);
            this.startDrag(tempAux);
            this.PlotGL();
        }

        public void glOnLeftMouseUp(object sender, MouseEventArgs e)
        {
            isLeftDrag = false;
        }

        private void glOnRightMouseDown(object sender, MouseEventArgs e)
        {
            isRightDrag = true;
            Map_to_Space(e.X,e.Y);
        }
        private void glOnRightMouseUp(object sender, MouseEventArgs e)
        {
            isRightDrag = false;
            //this.reset();
        }
        public void glOnMiddleMouseDown(object sender, MouseEventArgs e)
        {
            isMiddleDrag = true;
            Point tempAux = new Point(e.X, e.Y);
            this.startDrag(tempAux);
        }
        public void glOnMiddleMouseUp(object sender, MouseEventArgs e)
        {
            isMiddleDrag = false;
        }
        #endregion
        private void InitializeComponent()
        {
            this.SuspendLayout();
            this.ResumeLayout(false);

        }

    }
    #endregion OpenGL Control

    #region MouseControl
    public class MouseControl
    {
        protected Control newCtrl;
        protected MouseButtons FinalClick;

        public event EventHandler LeftClick;
        public event EventHandler RightClick;
        public event EventHandler MiddleClick;

        public event MouseEventHandler LeftMouseDown;
        public event MouseEventHandler LeftMouseUp;
        public event MouseEventHandler RightMouseDown;
        public event MouseEventHandler RightMouseUp;
        public event MouseEventHandler MiddleMouseDown;
        public event MouseEventHandler MiddleMouseUp;

        public Control Control
        {
            get { return newCtrl; }
            set
            {
                newCtrl = value;
                Initialize();
            }
        }

        public MouseControl()
        {
        }

        public MouseControl(Control ctrl)
        {
            Control = ctrl;
        }

        public void AddControl(Control ctrl)
        {
            Control = ctrl;
        }

        protected virtual void Initialize()
        {
            newCtrl.Click += new EventHandler(OnClick);
            newCtrl.MouseDown += new MouseEventHandler(OnMouseDown);
            newCtrl.MouseUp += new MouseEventHandler(OnMouseUp);
        }

        private void OnClick(object sender, EventArgs e)
        {
            switch (FinalClick)
            {
                case MouseButtons.Left:
                    if (LeftClick != null)
                    {
                        LeftClick(sender, e);
                    }
                    break;

                case MouseButtons.Right:
                    if (RightClick != null)
                    {
                        RightClick(sender, e);
                    }
                    break;
                case MouseButtons.Middle:
                    if (MiddleClick != null)
                    {
                        MiddleClick(sender, e);
                    }
                    break;
            }
        }

        private void OnMouseDown(object sender, MouseEventArgs e)
        {
            FinalClick = e.Button;

            switch (e.Button)
            {
                case MouseButtons.Left:
                    {
                        if (LeftMouseDown != null)
                        {
                            LeftMouseDown(sender, e);
                        }
                        break;
                    }

                case MouseButtons.Right:
                    {
                        if (RightMouseDown != null)
                        {
                            RightMouseDown(sender, e);
                        }
                        break;
                    }
                case MouseButtons.Middle:
                    {
                        if (MiddleMouseDown != null)
                        {
                            MiddleMouseDown(sender, e);
                        }
                        break;
                    }
            }
        }

        private void OnMouseUp(object sender, MouseEventArgs e)
        {
            switch (e.Button)
            {
                case MouseButtons.Left:
                    {
                        if (LeftMouseUp != null)
                        {
                            LeftMouseUp(sender, e);
                        }
                        break;
                    }

                case MouseButtons.Right:
                    {
                        if (RightMouseUp != null)
                        {
                            RightMouseUp(sender, e);
                        }
                        break;
                    }
                case MouseButtons.Middle:
                    {
                        if (MiddleMouseUp != null)
                        {
                            MiddleMouseUp(sender, e);
                        }
                        break;
                    }
            }
        }
    }
    #endregion MouseControl
}
