using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Input;
using OpenTK.Graphics.OpenGL;
using System.Drawing;
using System.Drawing.Imaging;

namespace SolarSystem
{
    class Planet
    {
        private int  imagePathID;
        float radius, planetRotation,orbitRotation, orbitSpeed;
        Vector3 position;
        public String name;
        bool hasMoon;
        static Random r = new Random();
        Moon moon;
        public Planet(float radius, String name,Vector3 position, String imagePath, bool hasMoon, float orbitSpeed)
        {
            this.radius = radius;
            this.name = name;
            this.imagePathID = loadTexture(imagePath);
            this.position = position;
            this.hasMoon = hasMoon;
            this.orbitRotation = r.Next(360);
            this.orbitSpeed = orbitSpeed;
            if (hasMoon)
            {
                moon = new Moon(radius/5, "Moon", this.position, "textures/moon.jpg");
            }
        }
        public Planet() {}
        public struct Vertex
        {
            public Vector2 TexCoord;
            public Vector3 Normal;
            public Vector3 Position;
        }

        public static Vertex[] CalculateVertices2(float radius, byte segments, byte rings)
        {
            var data = new Vertex[segments * rings];

            int i = 0;

            for (double y = 0; y < rings; y++)
            {
                double phi = (y / (rings - 1)) * Math.PI; //was /2 
                for (double x = 0; x < segments; x++)
                {
                    double theta = (x / (segments - 1)) * 2 * Math.PI;

                    Vector3 v = new Vector3()
                    {
                        X = (float)(radius * Math.Sin(phi) * Math.Cos(theta)),
                        Y = (float)(radius * Math.Cos(phi)),
                        Z = (float)(radius * Math.Sin(phi) * Math.Sin(theta)),
                    };
                    Vector3 n = Vector3.Normalize(v);
                    Vector2 uv = new Vector2()
                    {
                        X = (float)(x / (segments - 1)),
                        Y = (float)(y / (rings - 1))
                    };
                    // Using data[i++] causes i to be incremented multiple times in Mono 2.2 (bug #479506).
                    data[i] = new Vertex() { Position = v, Normal = n, TexCoord = uv };
                    i++;
                }

            }

            return data;
        }

        public static ushort[] CalculateElements(float radius, byte segments, byte rings)
        {
            var num_vertices = segments * rings;
            var data = new ushort[num_vertices * 6];

            ushort i = 0;

            for (byte y = 0; y < rings - 1; y++)
            {
                for (byte x = 0; x < segments - 1; x++)
                {
                    data[i++] = (ushort)((y + 0) * segments + x);
                    data[i++] = (ushort)((y + 1) * segments + x);
                    data[i++] = (ushort)((y + 1) * segments + x + 1);

                    data[i++] = (ushort)((y + 1) * segments + x + 1);
                    data[i++] = (ushort)((y + 0) * segments + x + 1);
                    data[i++] = (ushort)((y + 0) * segments + x);
                }
            }

            // Verify that we don't access any vertices out of bounds:
            foreach (int index in data)
                if (index >= segments * rings)
                    throw new IndexOutOfRangeException();

            return data;
        }


        public void updatePlanet()
        {

            GL.Enable(EnableCap.Texture2D);
            GL.BindTexture(TextureTarget.Texture2D, imagePathID);

            GL.PushMatrix();
            Vertex[] SphereVertices = CalculateVertices2(this.radius, 25, 25);
            ushort[] SphereElements = CalculateElements(this.radius, 25, 25);
            //First rotation to rotate the Planet on it`s orbit
            GL.Rotate(orbitRotation, 0, 1, 0);
            //Then translate to the given Position
            GL.Translate(position);
            //Second Rotation for the Planet itself
            GL.Rotate(planetRotation, 0, 1, 0);

            //Start drwaing the sphere
            GL.Begin(PrimitiveType.Triangles);
            foreach (var element in SphereElements)
            {
                var vertex = SphereVertices[element];
                GL.TexCoord2(vertex.TexCoord);
                GL.Normal3(vertex.Normal);
                GL.Vertex3(vertex.Position);
            }
            GL.End();
            //To draw the rings of Saturn
            if (name.Equals("Saturn"))
            {
                for (int i = 0; i <=15; i++)
                {
                    //Call drawOrbit two time because of the diffrent density of the rings, and the gap!
                    drawOrbit(150+i*2);
                    drawOrbit(190+i*3);
                }
            } 
           
           if (hasMoon)
           {
               moon.updateMoon();
           }
           GL.PopMatrix();

           orbitRotation += orbitSpeed/2;
           planetRotation += 0.6f;
           drawOrbit(this.position.X);
        }
        public void drawOrbit(float radius)
        {
            GL.Begin(PrimitiveType.LineStrip);

            for (int i = 0; i < 361; i++)
            {
                GL.Vertex3(radius* (float)Math.Sin(i * Math.PI / 180), 0, radius * (float)Math.Cos(i * Math.PI / 180));
            }
            GL.End();
         }

        public static int loadTexture(string filename)
        {
            if (String.IsNullOrEmpty(filename))
                throw new ArgumentException(filename);

            int id = GL.GenTexture();
            GL.BindTexture(TextureTarget.Texture2D, id);

            Bitmap bmp = new Bitmap(filename);
            BitmapData bmp_data = bmp.LockBits(new Rectangle(0, 0, bmp.Width, bmp.Height), ImageLockMode.ReadOnly, System.Drawing.Imaging.PixelFormat.Format32bppArgb);

            GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, bmp_data.Width, bmp_data.Height, 0,
                OpenTK.Graphics.OpenGL.PixelFormat.Bgra, PixelType.UnsignedByte, bmp_data.Scan0);

            bmp.UnlockBits(bmp_data);

            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Linear);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);

            return id;
        }
    }
}
