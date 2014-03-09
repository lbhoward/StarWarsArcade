using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

namespace StarWarsArcade
{
    class Camera
    {
        public Vector3 LookAt
        {
            get { return lookAt; }
            set { lookAt = value; }
        }
        private Vector3 lookAt;

        public Vector3 Position
        {
            get { return position; }
            set { position = value; }
        }
        private Vector3 position;

        public bool CockpitView = false;
        public float FOV = 45.0f;

        public Matrix Projection
        {
            get { return projection; }
        }
        private Matrix projection;

        public Matrix View
        {
            get { return view; }
        }
        private Matrix view;

        public Camera(Vector3 POSITION, Vector3 LOOKAT, float RATIO)
        {
            view = Matrix.CreateLookAt(POSITION, LOOKAT, Vector3.Up);
            projection = Matrix.CreatePerspectiveFieldOfView(MathHelper.ToRadians(FOV), RATIO, 0.01f, 50.0f);
        }

        public void Update()
        {
            view = Matrix.CreateLookAt(position, lookAt, Vector3.Up);
        }
    }
}
