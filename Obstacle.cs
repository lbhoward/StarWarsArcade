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
    class Obstacle
    {
        public BoundingBoxes.BoundingBoxComponent box;

        public Matrix World
        {
            get { return world; }
        }
        private Matrix world;

        public Obstacle(Vector3 STARTPOS, BoundingBox BOX, GraphicsDevice GFX)
        {
            box = new BoundingBoxes.BoundingBoxComponent(BOX, GFX);

            world = Matrix.Identity;
            world.Translation = STARTPOS;
            box.Update(world, STARTPOS);
        }
    }
}
