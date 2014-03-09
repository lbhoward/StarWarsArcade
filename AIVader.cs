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
    class AIVader
    {
        public Vector3 position;

        SoundEffect Shoot_SFX;

        public List<Laser> Lasers = new List<Laser>();

        public Matrix World
        {
            get { return world; }
        }
        private Matrix world;

        private BoundingBoxes.BoundingBoxComponent laser_box;

        public float accelaration;
        double lastShot = 0;
        bool justShot = false;

        public AIVader(Vector3 STARTPOS, Model MODEL, BoundingBoxes.BoundingBoxComponent L_BOX, SoundEffect SHOOT, GraphicsDevice GFX)
        {
            position = STARTPOS;

            Shoot_SFX = SHOOT;

            laser_box = L_BOX;

            world = Matrix.Identity;
            world.Translation = position;
        }

        public void Update(GameTime gameTime, GraphicsDevice GFX, Vector3 player_pos)
        {
            double elapsed = gameTime.TotalGameTime.TotalSeconds;

            if (player_pos.X > position.X)
                position.X += 0.015f;
            else if (player_pos.X < position.X)
                position.X -= 0.015f;
            if (player_pos.Y > position.Y)
                position.Y += 0.015f;
            else if (player_pos.Y < position.Y)
                position.Y -= 0.015f;
            position.Z = player_pos.Z - 1.5f;
            world.Translation = position;
            
            Fire(elapsed, GFX, player_pos);

            foreach (Laser laser in Lasers)
            {
                laser.Update();
            }

            for (int i = 0; i < Lasers.Count; i++)
            {
                if (Lasers[i].Position.Z > Lasers[i].Start.Z + 10.0f)
                    Lasers.RemoveAt(i);
            }
        }

        public void Fire(double elapsed, GraphicsDevice GFX, Vector3 player_pos)
        {
            if (!justShot)
            {
                Lasers.Add(new Laser(position, player_pos, 10.0f, 4, laser_box.ReturnBox(), GFX));
                Lasers.Add(new Laser(position, player_pos, 10.0f, 5, laser_box.ReturnBox(), GFX));
                justShot = true;
                lastShot = elapsed;

                Shoot_SFX.Play();
            }
            if (justShot)
            {
                if (elapsed >= lastShot + 0.5)
                    justShot = false;
            }
        }

        public void DrawLasers(Camera camera, Model model, GraphicsDevice GFX)
        {
            Matrix[] transforms = new Matrix[model.Bones.Count];
            model.CopyAbsoluteBoneTransformsTo(transforms);

            foreach (Laser laser in Lasers)
            {
                foreach (ModelMesh mesh in model.Meshes)
                {
                    foreach (BasicEffect effect in mesh.Effects)
                    {
                        effect.EnableDefaultLighting();
                        effect.World = transforms[mesh.ParentBone.Index] * laser.World;
                        effect.View = camera.View;
                        effect.Projection = camera.Projection;
                    }
                    mesh.Draw();
                }
            }
        }
    }
}