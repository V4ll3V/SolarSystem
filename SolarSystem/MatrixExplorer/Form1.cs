using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;


namespace MatrixExplorer
{
    public partial class Form1 : Form
    {
        private bool loaded = false;
        int x = 0;
        public Form1()
        {
            InitializeComponent();
        }
        private void glControl1_Load(object sender, EventArgs e)
        {
            loaded = true;
            GL.ClearColor(Color.Green);
            SetupViewport();
        }

        private void glControl1_Paint(object sender, PaintEventArgs e)
        {
            if(!loaded)
                return;

            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            GL.MatrixMode(MatrixMode.Modelview);
            GL.LoadIdentity();

            GL.Translate(x, 0, 0);

            GL.Color3(Color.Yellow);
            GL.Begin(PrimitiveType.Triangles);
            GL.Vertex3(10, 20, 0);
            GL.Vertex3(100, 20, 0);
            GL.Vertex3(100, 50, 0);
            GL.End();

            GL.Color3(Color.Red);
            GL.Begin(PrimitiveType.Triangles);
            GL.Vertex3(10, 20, -1);
            GL.Vertex3(100, 20, -1);
            GL.Vertex3(100, 50, -1);
            GL.End();

            glControl1.SwapBuffers();
        }
        private void SetupViewport()
        {
            int w = glControl1.Width;
            int h = glControl1.Height;
            GL.MatrixMode(MatrixMode.Projection);
            GL.LoadIdentity();
            GL.Ortho(0, w, 0, h, -1, 1); // Bottom-left corner pixel has coordinate (0, 0)
            GL.Viewport(0, 0, w, h); // Use all of the glControl painting area
        }

        private void glControl1_KeyDown(object sender, KeyEventArgs e)
        {
            if (!loaded)
                return;

            if (e.KeyCode == Keys.Space)
                x++;
            glControl1.Invalidate();
        }
        

    }
}
