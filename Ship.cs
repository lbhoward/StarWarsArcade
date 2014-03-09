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
    class Ship
    {
        public Vector3 position;

        public List<Laser> Lasers = new List<Laser>();

        public Matrix World
        {
            get { return world; }
        }
        private Matrix world;

        public float energy = 0;
        public float use_energy = 0;
        public bool energy_depleted = true, obj_collide = false;

        public float shields = 100, health = 400, accelaration;

        const int TOGGLE_CAP = 3;
        int laser_toggle = 0;
        BoundingBoxes.BoundingBoxComponent laser_box;

        public BoundingBoxes.BoundingBoxComponent box;

        public bool bounceLeft, bounceRight, bounceUp, bounceDown;
        public double bouncedLeft, bouncedRight, bouncedUp, bouncedDown;
        public float friction = 0.009f;

        public Ship(Vector3 STARTPOS, Model MODEL, BoundingBoxes.BoundingBoxComponent L_BOX, GraphicsDevice GFX)
        {
            position = STARTPOS;
            accelaration = 0.04f;

            laser_box = L_BOX;

            world = Matrix.Identity;
            world.Translation = position;

            box = new BoundingBoxes.BoundingBoxComponent(MODEL, GFX);
        }

        public void TakeHit(float damage)
        {
            if (shields > 0)
                shields -= damage;
            else
                health -= damage;
        }

        public void Update(GameTime gameTime)
        {
            if (energy == 0)
                energy_depleted = true;
            if (energy == 100)
                energy_depleted = false;
            if (shields > 1)
                shields += 0.05f;

            position.Z += accelaration;
            world.Translation = position;
            accelaration = 0.06f;
            box.Update(World, position);

            if (use_energy == 0)
                energy += 0.5f;
            else
                energy -= 1.5f;

            energy = MathHelper.Clamp(energy, 0, 100);
            shields = MathHelper.Clamp(shields, 0, 100);

                if (bounceLeft)
                {
                    position.X += (float)((accelaration * 1.35f)); //- (friction / (gameTime.TotalGameTime.TotalSeconds - (bouncedLeft + 0.01f))));

                    if (gameTime.TotalGameTime.TotalSeconds >= bouncedLeft + 0.35f)
                    {
                        bounceLeft = false;
                        obj_collide = false;
                    }
                }
                if (bounceRight)
                {
                    position.X -= (float)((accelaration * 1.35f));

                    if (gameTime.TotalGameTime.TotalSeconds >= bouncedRight + 0.35f)
                    {
                        bounceRight = false;
                        obj_collide = false;
                    }
                }
                if (bounceDown)
                {
                    position.Y -= (float)((accelaration * 1.35f));

                    if (gameTime.TotalGameTime.TotalSeconds >= bouncedDown + 0.35f)
                    {
                        bounceDown = false;
                        obj_collide = false;
                    }
                }
                if (bounceUp)
                {
                    position.Y += (float)((accelaration * 1.35f));

                    if (gameTime.TotalGameTime.TotalSeconds >= bouncedUp + 0.35f)
                    {
                        bounceUp = false;
                        obj_collide = false;
                    }
                }

            foreach (Laser laser in Lasers)
            {
                laser.Update();
            }

            use_energy = 0;
            position.Y = MathHelper.Clamp(position.Y, -10.0f, 2.75f);
        }

        public void Fire(GraphicsDevice GFX)
        {
            Lasers.Add(new Laser(position, Vector3.Zero, 10.0f, laser_toggle, laser_box.ReturnBox(), GFX));

            if (laser_toggle == TOGGLE_CAP)
                laser_toggle = 0;
            else
                laser_toggle++;
        }

        public void DrawReticle(Camera camera, GraphicsDevice GFX, SpriteBatch SB, Texture2D RETICLE)
        {
            Vector3 center = GFX.Viewport.Project(new Vector3(position.X, position.Y, position.Z + 10),
                            camera.Projection, camera.View, Matrix.Identity);

            Rectangle sourcerect = new Rectangle(0,0,RETICLE.Width, RETICLE.Height);
            SB.Draw(RETICLE, new Vector2(center.X, center.Y), sourcerect, Color.White,
                         0, new Vector2(RETICLE.Width/2,RETICLE.Height/2), 1, SpriteEffects.None, 0);
        }
    }
}