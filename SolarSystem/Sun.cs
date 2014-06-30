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
    class Sun : Planet
    {
        private int imagePathID;
        float radius, sunRotation;
        Vector3 position;
        static Random r = new Random();
        public Sun(float radius, String name, Vector3 position, String imagePath, bool hasMoon)
        {
            this.radius = radius;
            this.name = name;
            this.imagePathID = loadTexture(imagePath);
            this.position = position;
        }

        public void createSun()
        {
            GL.Enable(EnableCap.Texture2D);
            GL.BindTexture(TextureTarget.Texture2D, imagePathID);

            GL.PushMatrix();
            Vertex[] SphereVertices = CalculateVertices2(this.radius, 100, 100);
            ushort[] SphereElements = CalculateElements(this.radius, 100, 100);
           
            GL.Translate(position);
            //Rotate the Sun
            GL.Rotate(sunRotation, 0, 1, 0);

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
            sunRotation += 0.3f;

        }
        public void updateSun()
        {
            createSun();
        }
    }
}
