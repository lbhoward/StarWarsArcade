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
    class Laser
    {
        public Vector3 Position
        {
            get { return position; }
        }
        private Vector3 position;

        public Vector3 Start
        {
            get { return start; }
        }
        private Vector3 start;

        public Vector3 Target
        {
            get { return target; }
        }
        private Vector3 target;

        float acceleration = 0.02f;
        float damage;
        float offsetX = 0.0f;
        float offsetY = 0.0f;

        public Matrix World
        {
            get { return world; }
        }
        private Matrix world;

        public BoundingBoxes.BoundingBoxComponent box;

        public Laser(Vector3 STARTPOS, Vector3 TARGET, float DAMAGE, int ORI, BoundingBox BOX, GraphicsDevice GFX)
        {
            position = STARTPOS;
            start = STARTPOS;

            switch (ORI)
            {
                case 0: //TL
                position.X += 0.4f; position.Y += 0.12f; position.Z += 0.35f;
                offsetX = -0.02f; offsetY = -0.01f;
                break;

                case 1: //BR
                position.X -= 0.4f; position.Y -= 0.12f; position.Z += 0.35f;
                offsetX = 0.02f; offsetY = 0.01f;
                break;

                case 2: //TR
                position.X -= 0.4f; position.Y += 0.12f; position.Z += 0.35f;
                offsetX = 0.02f; offsetY = -0.01f;
                break;

                case 3: //BL
                position.X += 0.4f; position.Y -= 0.12f; position.Z += 0.35f;
                offsetX = -0.02f; offsetY = 0.01f;
                break;

                case 4: //TIE L
                position.X += 0.2f; position.Y -= 0.06f; position.Z += 0.35f;
                break;

                case 5: //TIE R
                position.X -= 0.2f; position.Y -= 0.06f; position.Z += 0.35f;
                break;
            }
            target = TARGET;
            damage = DAMAGE;

            world = Matrix.Identity;
            world.Translation = position;

            box = new BoundingBoxes.BoundingBoxComponent(BOX, GFX);
        }

        public void Update()
        {
            position.Z += 0.4f;
            if (target == Vector3.Zero)
            {
                position.X += offsetX;
                position.Y += offsetY;
            }
            else
            {
                if (position.X > target.X)
                    position.X -= 0.05f;
                else
                    position.X += 0.05f;

                if (position.Y > target.Y)
                    position.Y -= 0.05f;
                else
                    position.Y += 0.05f;
            }
            world = Matrix.Identity;
            world.Translation = position;
            box.Update(world, position);
        }
    }
}
