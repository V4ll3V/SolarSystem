using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Input;
using OpenTK.Graphics.OpenGL;


namespace SolarSystem
{
    public partial class Form1 : Form
    {
        private bool loaded = false;
        int w, h;
        float zoom = 1.0f;
        bool leftPress = false, rightPress = false;
        Planet  mercury, venus, earth, mars, jupiter, saturn, neptune, uranus;
        Sun sun;
        List<Planet> planets = new List<Planet>();
        Timer timer;
        Cam cam;
        Matrix4 lookat;
        public Form1()
        {
            InitializeComponent();
        }
        private void glControl1_Load(object sender, EventArgs e)
        {
            loaded = true;
            initLightning();
            SetupViewport();

            //create the Sun
            sun = new Sun(50f, "Sun", new Vector3(0, 0, 0), "textures/sun.jpg", false);
            //Create the Planets and save them to the Planet list with more or less real data from : http://www.astrokramkiste.de/planeten-tabelle
            mercury = new Planet(4.8f, "Mercury", new Vector3(70, 0, 0), "textures/mercury.jpg", false, 1.72f);
            planets.Add(mercury);

            venus = new Planet(12.1f, "Venus", new Vector3(108, 0, 0), "textures/venus.jpg", false, 1.26f);
            planets.Add(venus);

            earth = new Planet(12.7f, "Earth", new Vector3(150, 0, 0), "textures/earth.jpg", true, 1.07f);
            planets.Add(earth);

            mars = new Planet(6.7f, "Mars", new Vector3(228, 0, 0), "textures/mars.jpg", true, 0.86f);
            planets.Add(mars);

            jupiter = new Planet(138.3f, "Jupiter", new Vector3(778, 0, 0), "textures/jupiter.jpg", false, 0.47f);
            planets.Add(jupiter);

            saturn = new Planet(114.6f, "Saturn", new Vector3(1433, 0, 0), "textures/saturn.jpg", false, 0.34f);
            planets.Add(saturn);

            uranus = new Planet(50.5f, "Uranus", new Vector3(2872, 0, 0), "textures/uranus.jpg", false, 0.24f);
            planets.Add(uranus);

            neptune = new Planet(49.1f, "Neptune", new Vector3(4495, 0, 0), "textures/neptune.jpg", false, 0.19f);
            planets.Add(neptune);

            //Update timer
            timer = new Timer();
            timer.Interval = 1;
            timer.Tick += new EventHandler(update_Tick);
            timer.Start();
    
        }
       
        private void SetupViewport()
        {
            w = glControl1.Width;
            h = glControl1.Height;
            GL.Viewport(0, 0, w, h); // Use all of the glControl painting area
            GL.MatrixMode(MatrixMode.Projection);
            GL.LoadIdentity();
            GL.Ortho(zoom*-w, zoom*w, zoom*-h,zoom* h,-6000, 6000); // Bottom-left corner pixel has coordinate (0, 0)
            //Init cam
            cam = new Cam(new Vector3(10.0f, 5.0f, 0.0f),new Vector3( 0.0f, 0.0f, 0.0f), new Vector3(0.0f, 1.0f, 0.0f), w, h);

        }
        private void initLightning()
        {
            GL.ClearColor(Color.Black);
            

            //Enable depth testing
            GL.Enable(EnableCap.DepthTest);
            GL.DepthFunc(DepthFunction.Greater); 
            GL.ClearDepth(0.1f);

            //Enable lighting
            GL.Light(LightName.Light1, LightParameter.Ambient, OpenTK.Graphics.Color4.White);
            GL.Light(LightName.Light1, LightParameter.Specular, OpenTK.Graphics.Color4.White);
            GL.Light(LightName.Light1, LightParameter.Diffuse, OpenTK.Graphics.Color4.White);
            GL.Light(LightName.Light1, LightParameter.Position, (new Vector4(0f, 30f, 0f, 0f)));

            GL.Light(LightName.Light2, LightParameter.Ambient, OpenTK.Graphics.Color4.White);
            GL.Light(LightName.Light2, LightParameter.Specular, OpenTK.Graphics.Color4.White);
            GL.Light(LightName.Light1, LightParameter.Diffuse, OpenTK.Graphics.Color4.White);
            GL.Light(LightName.Light2, LightParameter.Position, (new Vector4(0f, -30f, 0f, 0f)));
            
            GL.Enable(EnableCap.Light1);
            GL.Enable(EnableCap.Light2);
            GL.Enable(EnableCap.Lighting);

            //Enable Backfaceculling
            //GL.Enable(EnableCap.CullFace);
        }

        void render()
        {
            if (!loaded)
                return;
            //Left Mouse Button to zoom in
            if (leftPress)
            {
                zoom -= 0.01f;
            }
            //right Mouse Button to zoom out
            if (rightPress)
            {
                zoom += 0.01f;
            }
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            GL.MatrixMode(MatrixMode.Projection);
            GL.LoadIdentity();
            //Set Ortho to get the zoom effect
            GL.Ortho(zoom * -w, zoom * w, zoom * -h, zoom * h, -6000, 6000);

            GL.MatrixMode(MatrixMode.Modelview);
            GL.LoadIdentity();

            lookat = cam.lookAt();
            GL.LoadMatrix(ref lookat);
            //Draw the Planets and the sun
            foreach(var item in planets){

                item.updatePlanet();
                
            }
            sun.updateSun();
            glControl1.SwapBuffers();
        }

        private void glControl1MouseMove(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            if (leftPress)
            {
                //Set the current mouse position to the objects X and Y eye Value
                cam.eye.X = e.X;
                cam.eye.Y = e.Y;
                
                render();
                Invalidate();
            } 
            if (rightPress)
            {
                //Set the current mouse position to the objects X and Y eye Value
                cam.eye.X = e.X;
                cam.eye.Y = e.Y;
                cam.eye.Z -= 0.1f;

                render();
                Invalidate();
            }
        }

        private void canvasMouseDown(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            //start of Mouse drag
           if (e.Button == MouseButtons.Left)
            {

                leftPress = true;
                
            }
            if (e.Button == MouseButtons.Right)
            {

                rightPress = true;

            }

            
        }

        private void canvasMouseUp(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            //release Mouse drag
            if (e.Button == MouseButtons.Left)
            {
                leftPress = false;

            }
            if (e.Button == MouseButtons.Right)
            {
                rightPress = false;
            }
        }

        private void update_Tick(object sender, EventArgs e)
        {
            render();
        }

    }
}
