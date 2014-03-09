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
    class AIShip
    {
        public Vector3 position;

        public float offset_X;

        public double spawnTime = 0;
        public bool spawn_time_set = false;
        public double catchUpTime = 0;

        SoundEffect Shoot_SFX;

        public List<Laser> Lasers = new List<Laser>();

        public Matrix World
        {
            get { return world; }
        }
        private Matrix world;

        public int Health
        {
            get { return health; }
        }
        private int health = 100;

        public int Shield
        {
            get { return shield; }
        }
        private int shield = 100;

        private BoundingBoxes.BoundingBoxComponent laser_box;
        public BoundingBoxes.BoundingBoxComponent box;

        public float accelaration;
        double lastShot = 0;
        bool justShot = false;

        public AIShip(Vector3 STARTPOS, float OFFSET_X, double CATCHUP, Model MODEL, BoundingBoxes.BoundingBoxComponent L_BOX, SoundEffect SHOOT, GraphicsDevice GFX)
        {
            position = STARTPOS;
            accelaration = 0.02f;

            catchUpTime = CATCHUP;

            offset_X = OFFSET_X;

            Shoot_SFX = SHOOT;

            laser_box = L_BOX;

            world = Matrix.Identity;
            world.Translation = position;

            box = new BoundingBoxes.BoundingBoxComponent(MODEL, GFX);
        }

        public void Update(GameTime gameTime, GraphicsDevice GFX, Vector3 player_pos)
        {
            double elapsed = gameTime.TotalGameTime.TotalSeconds;

            position.X += offset_X;
            world.Translation = position;
            box.Update(world, position);

            if (position.Z <= player_pos.Z && player_pos.Z - position.Z <= 10)
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

        public void TakeHit()
        {
            if (shield > 0)
                shield -= 25;
            else
                health -= 25;
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

        public void CheckCollide(Ship player)
        {
            for (int i = 0; i < Lasers.Count; i++)
            {
                if (Lasers[i].box.ReturnBox().Intersects(player.box.ReturnBox()))
                {
                    player.TakeHit(10);
                    Lasers.RemoveAt(i);
                }
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

            Vector3 center = GFX.Viewport.Project((box.ReturnBox().Max + box.ReturnBox().Min) / 2,
                            camera.Projection, camera.View, Matrix.Identity);

        }

        public void DrawHP(Camera camera, GraphicsDevice GFX, SpriteBatch SB, Texture2D HBar, Vector3 player_pos)
        {
            Vector3 center = GFX.Viewport.Project((box.ReturnBox().Max + box.ReturnBox().Min) / 2,
                            camera.Projection, camera.View, Matrix.Identity);

            Vector3 screencorner = GFX.Viewport.Project(box.ReturnBox().Max,
                                          camera.Projection, camera.View, Matrix.Identity);
            float distance = Vector3.Distance(screencorner, center);
            float scale = 0;
            if (distance > scale)
                scale = distance / 75;

            if (player_pos.Z < position.Z)
                SB.Draw(HBar, new Rectangle((int)(center.X - ((health*scale)/2)), (int)center.Y + 40, (int)(health * scale), 5), Color.Green);
        }
    }
}