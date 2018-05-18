using System;
using System.Drawing;
using System.Windows.Forms;
using CsGL.OpenGL;

namespace arcball
{

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

        #endregion

        public OGL()
            : base()
        {
            this.KeyDown += new KeyEventHandler(OurView_OnKeyDown);
            GC.Collect();
        }

        protected override void InitGLContext()
        {

            GL.glShadeModel(GL.GL_SMOOTH);								// enable smooth shading
            GL.glClearColor(0.0f, 0.0f, 0.0f, 0.5f);					// black background
            GL.glClearDepth(1.0f);										// depth buffer setup
            GL.glEnable(GL.GL_DEPTH_TEST);								// enables depth testing
            GL.glDepthFunc(GL.GL_LEQUAL);								// type of depth testing
            GL.glHint(GL.GL_PERSPECTIVE_CORRECTION_HINT, GL.GL_NICEST);	// nice perspective calculations
            GL.glPointSize(4.0f);

            GL.gluPerspective(25.0, (double)width / (double)height, 1.0, 15.0);
            GL.glTranslatef(0.0f, 0.0f, -4.0f);


            GL.glEnable(GL.GL_LIGHTING);                                            // Enable Lighting
            GL.glEnable(GL.GL_LIGHT0);                                            // Enable Default Light

            GL.glEnable(GL.GL_COLOR_MATERIAL);                                    // Enable Color Material


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

        public void PlotGL()
        {
            try
            {
                lock (matrixLock)
                {
                    ThisRot.get_Renamed(matrix);
                }
                GL.glClear(GL.GL_COLOR_BUFFER_BIT | GL.GL_DEPTH_BUFFER_BIT); // Clear screen and DepthBuffer
                GL.glLoadIdentity();

                GL.glPushMatrix();                  // NEW: Prepare Dynamic Transform
                GL.glMultMatrixf(matrix);           // NEW: Apply Dynamic Transform

                GL.glPolygonMode(GL.GL_FRONT_AND_BACK, GL.GL_FILL);

                #region plot something

                GL.glColor3f(0.8f, 0.3f, 0.1f);

                this.torus(0.3f, 0.5f);


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

        public void reset()
        {
            lock (matrixLock)
            {
                LastRot.setIdentity();                                // Reset Rotation
                ThisRot.setIdentity();                                // Reset Rotation
            }

            this.PlotGL();
        }

        #endregion CsGL

        #region Keyboard Control
        protected void OurView_OnKeyDown(object Sender, KeyEventArgs kea)
        {
            //if escape was pressed, exit the application
            if (kea.KeyCode == Keys.Escape)
            {
                this.Parent.Dispose();
            }
            //if R was pressed, reset
            if (kea.KeyCode == Keys.R)
            {
                this.reset();
            }
        }
        #endregion Keyboard Control

        #region Mouse Control
        private void startDrag(Point MousePt)
        {
            lock (matrixLock)
            {
                LastRot.set_Renamed(ThisRot); // Set Last Static Rotation To Last Dynamic One
            }
            arcBall.click(MousePt); // Update Start Vector And Prepare For Dragging

            mouseStartDrag = MousePt;
            LastScale = ThisScale;

        }

        private void drag(Point MousePt)
        {
            Quat4f ThisQuat = new Quat4f();

            arcBall.drag(MousePt, ThisQuat); // Update End Vector And Get Rotation As Quaternion

            lock (matrixLock)
            {
                ThisRot.Rotation = ThisQuat; // Convert Quaternion Into Matrix3fT
                ThisRot.mul(ThisRot, LastRot); // Accumulate Last Rotation Into This One
            }
        }

        public void glOnMouseMove(object sender, MouseEventArgs e)
        {
            Point tempAux = new Point(e.X, e.Y);
            mouseEndDrag = tempAux;

            if (isLeftDrag)
            {
                this.drag(tempAux);
                this.PlotGL();
            }

            if (isRightDrag)
            {
                this.PlotGL();
            }
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
            Point tempAux = new Point(e.X, e.Y);
            this.startDrag(tempAux);
        }

        private void glOnRightMouseUp(object sender, MouseEventArgs e)
        {
            isRightDrag = false;
            this.reset();
        }
        #endregion

    }
    #endregion OpenGL Control

    #region MouseControl
    public class MouseControl
    {
        protected Control newCtrl;
        protected MouseButtons FinalClick;

        public event EventHandler LeftClick;
        public event EventHandler RightClick;
        public event MouseEventHandler LeftMouseDown;
        public event MouseEventHandler LeftMouseUp;
        public event MouseEventHandler RightMouseDown;
        public event MouseEventHandler RightMouseUp;

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
            }
        }
    }
    #endregion MouseControl

}