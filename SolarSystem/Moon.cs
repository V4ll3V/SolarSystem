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
    class Moon : Planet
    {

        private int imagePathID;
        float radius, moonRotation, orbit;
        Vector3 position;
        static Random r = new Random();

        public Moon(float radius, String name, Vector3 position, String imagePath)
        {
            this.radius = radius;
            this.name = name;
            this.imagePathID = loadTexture(imagePath);
            this.position = position;
            this.position.X = 30f;
            this.moonRotation = r.Next(360);
            orbit = (float)r.NextDouble() * 10f;

        }

        public void createMoon()
        {
            GL.Enable(EnableCap.Texture2D);
            GL.BindTexture(TextureTarget.Texture2D, imagePathID);

            GL.PushMatrix();
            Vertex[] SphereVertices = CalculateVertices2(this.radius, 50, 50);
            ushort[] SphereElements = CalculateElements(this.radius, 50, 50);

            //First rotation to rotate the Moon on it`s orbit
            GL.Rotate(moonRotation, 0, 1, 0);
            //Then translate to the given Position
            GL.Translate(this.position.X,0,0);
            //Second Rotation for the Moon itself
            GL.Rotate(moonRotation, 0, 1, 0);

            GL.Begin(PrimitiveType.Triangles);
            foreach (var element in SphereElements)
            {
                var vertex = SphereVertices[element];
                GL.TexCoord2(vertex.TexCoord);
                GL.Normal3(vertex.Normal);
                GL.Vertex3(vertex.Position);
            }
            GL.End();
            GL.PopMatrix();


        }
        public void updateMoon()
        {
            moonRotation += orbit;
            createMoon();

        }

    }
}
