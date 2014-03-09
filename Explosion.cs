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
    class Explosion
    {
        Vector3 Position, PositionMax;
        public int Frame, Frames;
        int FrameWidth, FrameHeight;

        public Explosion(Vector3 LOCATION, int FRAMES)
        {
            Position = LOCATION;
            Position.Z += 2.0f;
            PositionMax = Position;
            PositionMax.X -= 2.0f;
            PositionMax.Y += 2.0f;
            Frames = FRAMES;
        }

        public void Draw(Camera camera, GraphicsDevice GFX, SpriteBatch SB, Texture2D EXPLOSION, Vector3 player_pos)
        {
            FrameWidth = EXPLOSION.Width / Frames;
            FrameHeight = EXPLOSION.Height;

            Vector3 center = GFX.Viewport.Project(Position,
                            camera.Projection, camera.View, Matrix.Identity);
            Vector3 screencorner = GFX.Viewport.Project(new Vector3(Position.X + 1.0f, Position.Y + 1.0f, Position.Z + 1.0f),
                            camera.Projection, camera.View, Matrix.Identity);

            float distance = Vector3.Distance(screencorner, center);

            float scale = distance / ((float)(FrameWidth * 1.5));

            Rectangle sourcerect = new Rectangle(FrameWidth * Frame, 0,
            FrameWidth, EXPLOSION.Height);
            if (player_pos.Z < Position.Z)
                SB.Draw(EXPLOSION, new Vector2(center.X, center.Y), sourcerect, Color.White,
                         0, new Vector2(FrameWidth/2,FrameHeight/2), scale, SpriteEffects.None, 0);

            Frame++;
        }
    }
}
