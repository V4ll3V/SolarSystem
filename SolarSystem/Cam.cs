using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Input;
using OpenTK.Graphics.OpenGL;

namespace SolarSystem
{
    class Cam
    {
        public Vector3 eye, lastEye, camPos, up;
        static float forwardSpeed = 3f;
        int windowW, windowH;
        public Cam(Vector3 eye, Vector3 camPos, Vector3 up, int windowH, int windowW)
        {
            this.eye = eye;
            this.camPos = camPos;
            this.up= up;
            this.windowH = windowH;
            this.windowW = windowW;
            this.lastEye = eye;

        }

        public Matrix4 lookAt(){
            //Looks at te sun, an move the rest! 
            //eye sets the movement vor the cam, camPos is (0,0,0) to look at the sun
            Matrix4 lookat = Matrix4.LookAt(-eye, camPos, up);
            return lookat;
        }

    }
}
