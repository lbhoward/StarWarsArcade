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
    class Menu
    {
        public bool Active = true, Ambiguity = false, GamePadMode = false, Restart = false, Start = true;
        public int clamp_min = 0, clamp_max = 2;
        public int selected = 0, menu_stage = 0;
        public int bg_vol = 10;
        Color c1 = Color.Yellow, c2 = Color.LimeGreen, c3 = Color.Yellow, c4 = Color.Yellow, c5 = Color.Yellow, c6 = Color.Yellow;

        public Buttons fire_button = Buttons.A, boost_button = Buttons.RightTrigger, brake_button = Buttons.LeftTrigger;
        public Keys fire_key = Keys.Space, boost_key = Keys.O, brake_key = Keys.I,
        up_key = Keys.W, down_key = Keys.S, left_key = Keys.A, right_key = Keys.D;
 


        public void Draw(SpriteBatch SB, SpriteFont SWFONT_BIG, SpriteFont SWFONT)
        {
            if (menu_stage == 0)
            {
                SB.DrawString(SWFONT_BIG, "star wars arcade", new Vector2(10, 10), c1);
                SB.DrawString(SWFONT_BIG, "start game", new Vector2(10, 100), c2);
                SB.DrawString(SWFONT_BIG, "options", new Vector2(10, 140), c3);
                SB.DrawString(SWFONT_BIG, "exit game", new Vector2(10, 180), c4);
                clamp_max = 2;
            }

            if (menu_stage == 1)
            {
                SB.DrawString(SWFONT_BIG, "paused", new Vector2(10, 10), c1);
                SB.DrawString(SWFONT_BIG, "resume", new Vector2(10, 100), c2);
                SB.DrawString(SWFONT_BIG, "options", new Vector2(10, 140), c3);
                SB.DrawString(SWFONT_BIG, "restart", new Vector2(10, 180), c4);
                SB.DrawString(SWFONT_BIG, "exit game", new Vector2(10, 220), c5);
                clamp_max = 3;
            }
            if (menu_stage == 2)
            {
                SB.DrawString(SWFONT_BIG, "options", new Vector2(10, 10), c1);
                SB.DrawString(SWFONT, "background music volume", new Vector2(10, 100), c2);
                SB.DrawString(SWFONT, "remap keys (gamepad)", new Vector2(10, 140), c3);
                if (!GamePadMode)
                    SB.DrawString(SWFONT, "enable gamepad", new Vector2(10, 180), c4);
                else
                    SB.DrawString(SWFONT, "disable gamepad", new Vector2(10, 180), c4);
                SB.DrawString(SWFONT, "restore default mapping", new Vector2(10, 220), c5);
                clamp_max = 3;

            }
            if (menu_stage == 4)
            {
                SB.DrawString(SWFONT_BIG, "game over", new Vector2(10, -6), c1);
                SB.DrawString(SWFONT_BIG, "retry?", new Vector2(10, 100), c1);
                SB.DrawString(SWFONT_BIG, "yes", new Vector2(10, 140), c2);
                SB.DrawString(SWFONT_BIG, "no", new Vector2(10, 180), c3);
                clamp_max = 1;
            }
            if (menu_stage == 5)
            {
                SB.DrawString(SWFONT_BIG, "remap keyboard", new Vector2(10, 10), Color.Yellow);
                SB.DrawString(SWFONT, "fire", new Vector2(10, 100), c2);
                SB.DrawString(SWFONT, fire_button.ToString().ToLower(), new Vector2(10, 120), Color.OrangeRed);
                SB.DrawString(SWFONT, "boost", new Vector2(10, 140), c3);
                SB.DrawString(SWFONT, boost_button.ToString().ToLower(), new Vector2(10, 160), Color.OrangeRed);
                SB.DrawString(SWFONT, "brake", new Vector2(10, 180), c4);
                SB.DrawString(SWFONT, brake_button.ToString().ToLower(), new Vector2(10, 200), Color.OrangeRed);
                SB.DrawString(SWFONT, "back", new Vector2(10, 240), c5);
                clamp_max = 3;
            }
            if (menu_stage == 6)
            {
                SB.DrawString(SWFONT_BIG, "mission complete!", new Vector2(10, 10), Color.Yellow);
                SB.DrawString(SWFONT_BIG, "play again?", new Vector2(10, 100), c1);
                SB.DrawString(SWFONT_BIG, "yes", new Vector2(10, 140), c2);
                SB.DrawString(SWFONT_BIG, "no", new Vector2(10, 180), c3);
                clamp_max = 1;
            }
        }

        public void GPUpdate(GamePadState currentGPS, GamePadState previousGPS)
        {
            bg_vol = (int)MathHelper.Clamp(bg_vol, 0, 10);

            if (currentGPS.DPad.Up == ButtonState.Pressed && previousGPS.DPad.Up != ButtonState.Pressed)
                selected--;
            if (currentGPS.DPad.Down == ButtonState.Pressed && previousGPS.DPad.Down != ButtonState.Pressed)
                selected++;

            if (currentGPS != null && menu_stage == 5 && selected == 0)
                RemapGP(currentGPS, fire_button);
            if (currentGPS != null && menu_stage == 5 && selected == 1)
                RemapGP(currentGPS, boost_button);
            if (currentGPS != null && menu_stage == 5 && selected == 2)
                RemapGP(currentGPS, brake_button);
            if (currentGPS.Buttons.A == ButtonState.Pressed && previousGPS.Buttons.A != ButtonState.Pressed && selected == 1 && menu_stage == 2)
            {
                menu_stage = 5;
                selected = 0;
            }
            if (currentGPS.Buttons.A == ButtonState.Pressed && previousGPS.Buttons.A != ButtonState.Pressed && selected == 3 && Start && menu_stage == 5)
                menu_stage = 0;
            if (currentGPS.Buttons.A == ButtonState.Pressed && previousGPS.Buttons.A != ButtonState.Pressed && selected == 3 && !Start && menu_stage == 5)
                menu_stage = 2;
            if (currentGPS.Buttons.A == ButtonState.Pressed && previousGPS.Buttons.A != ButtonState.Pressed && selected == 0 && menu_stage == 0)
            {
                Active = false;
                Start = false;
                menu_stage = 1;
                selected = 0;
                Restart = true;
            }
            if (currentGPS.Buttons.A == ButtonState.Pressed && previousGPS.Buttons.A != ButtonState.Pressed && selected == 0 && menu_stage == 1)
            {
                menu_stage = 1;
                selected = 0;
            }
            if (currentGPS.Buttons.A == ButtonState.Pressed && previousGPS.Buttons.A != ButtonState.Pressed && selected == 2 && menu_stage == 2)
            {
                GamePadMode = !GamePadMode;
            }
            if (currentGPS.Buttons.A == ButtonState.Pressed && previousGPS.Buttons.A != ButtonState.Pressed && selected == 0 && (menu_stage == 4 || menu_stage == 6))
            {
                menu_stage = 1;
                selected = 0;
                Restart = true;
            }
            if (currentGPS.Buttons.A == ButtonState.Pressed && previousGPS.Buttons.A != ButtonState.Pressed && selected == 1 && (menu_stage == 4 || menu_stage == 6))
            {
                menu_stage = 3;
            }
            if (currentGPS.Buttons.B == ButtonState.Pressed && previousGPS.Buttons.B != ButtonState.Pressed && Start && menu_stage == 2)
            {
                menu_stage = 0;
                selected = 0;
            }
            if (currentGPS.Buttons.B == ButtonState.Pressed && previousGPS.Buttons.B != ButtonState.Pressed && !Start && menu_stage == 2)
            {
                menu_stage = 1;
                selected = 0;
            }
            if (currentGPS.Buttons.A == ButtonState.Pressed && previousGPS.Buttons.A != ButtonState.Pressed && selected == 2 && menu_stage == 1)
            {
                Restart = true;
                menu_stage = 1;
                selected = 0;
            }
            if (currentGPS.Buttons.A == ButtonState.Pressed && previousGPS.Buttons.A != ButtonState.Pressed && selected == 1 && (menu_stage == 0 || menu_stage == 1))
            {
                menu_stage = 2;
                selected = 0;
            }
            if (currentGPS.DPad.Left == ButtonState.Pressed && previousGPS.DPad.Left != ButtonState.Pressed && selected == 0 && menu_stage == 2)
                bg_vol--;
            if (currentGPS.DPad.Right == ButtonState.Pressed && previousGPS.DPad.Right != ButtonState.Pressed && selected == 0 && menu_stage == 2)
                bg_vol++;
            if (currentGPS.Buttons.A == ButtonState.Pressed && previousGPS.Buttons.A != ButtonState.Pressed && selected == 2 && menu_stage == 0)
            {
                menu_stage = 3;
            }
            if (currentGPS.Buttons.A == ButtonState.Pressed && previousGPS.Buttons.A != ButtonState.Pressed && selected == 3 && menu_stage == 1)
            {
                menu_stage = 3;
            }
            if (currentGPS.Buttons.A == ButtonState.Pressed && previousGPS.Buttons.A != ButtonState.Pressed && selected == 3 && menu_stage == 2)
            {
                RestoreDefaults();
            }

            selected = (int)MathHelper.Clamp(selected, clamp_min, clamp_max);

            c2 = Color.Yellow; c3 = Color.Yellow; c4 = Color.Yellow; c5 = Color.Yellow; c6 = Color.Yellow;

            switch (selected)
            {
                case 0:
                    c2 = Color.LimeGreen;
                    break;

                case 1:
                    c3 = Color.LimeGreen;
                    break;

                case 2:
                    c4 = Color.LimeGreen;
                    break;

                case 3:
                    c5 = Color.LimeGreen;
                    break;

                case 4:
                    c6 = Color.LimeGreen;
                    break;
            }
        }

        public void KBUpdate(KeyboardState currentKBS, KeyboardState previousKBS)
        {
            bg_vol = (int)MathHelper.Clamp(bg_vol, 0, 10);

            if (currentKBS.IsKeyDown(Keys.W) && !previousKBS.IsKeyDown(Keys.W))
                selected--;
            if (currentKBS.IsKeyDown(Keys.S) && !previousKBS.IsKeyDown(Keys.S))
                selected++;

            if ((currentKBS.IsKeyDown(Keys.Space) && !previousKBS.IsKeyDown(Keys.Space)) && selected == 0 && menu_stage == 0)
            {
                Active = false;
                Start = false;
                menu_stage = 1;
                selected = 0;
                Restart = true;
            }
            if (currentKBS.IsKeyDown(Keys.Space) && !previousKBS.IsKeyDown(Keys.Space) && selected == 0 && menu_stage == 1)
            {
                menu_stage = 1;
                selected = 0;
                Active = false;
            }
            if (currentKBS.IsKeyDown(Keys.Space) && !previousKBS.IsKeyDown(Keys.Space) && selected == 2 && menu_stage == 2)
            {
                GamePadMode = !GamePadMode;
            }
            if (currentKBS.IsKeyDown(Keys.Space) && !previousKBS.IsKeyDown(Keys.Space) && selected == 0 && menu_stage == 4)
            {
                menu_stage = 1;
                selected = 0;
                Restart = true;
            }
            if (currentKBS.IsKeyDown(Keys.Space) && !previousKBS.IsKeyDown(Keys.Space) && selected == 1 && menu_stage == 4)
            {
                menu_stage = 3;
            }
            if (currentKBS.IsKeyDown(Keys.Escape) && previousKBS.IsKeyDown(Keys.Escape) && Start && menu_stage == 2)
            {
                menu_stage = 0;
                selected = 0;
            }
            if (currentKBS.IsKeyDown(Keys.Escape) && previousKBS.IsKeyDown(Keys.Escape) && !Start && menu_stage == 2)
            {
                menu_stage = 1;
                selected = 0;
            }
            if (currentKBS.IsKeyDown(Keys.Space) && !previousKBS.IsKeyDown(Keys.Space) && selected == 2 && menu_stage == 1)
            {
                Restart = true;
                menu_stage = 1;
                selected = 0;
            }
            if (currentKBS.IsKeyDown(Keys.Space) && !previousKBS.IsKeyDown(Keys.Space) && selected == 1 && (menu_stage == 0 || menu_stage == 1))
            {
                menu_stage = 2;
                selected = 0;
            }
            if (currentKBS.IsKeyDown(Keys.A) && !previousKBS.IsKeyDown(Keys.A) && selected == 0 && menu_stage == 2)
                bg_vol--;
            if (currentKBS.IsKeyDown(Keys.D) && !previousKBS.IsKeyDown(Keys.D) && selected == 0 && menu_stage == 2)
                bg_vol++;
            if (currentKBS.IsKeyDown(Keys.Space) && !previousKBS.IsKeyDown(Keys.Space) && selected == 2 && menu_stage == 0)
            {
                menu_stage = 3;
            }
            if (currentKBS.IsKeyDown(Keys.Space) && !previousKBS.IsKeyDown(Keys.Space) && selected == 3 && menu_stage == 1)
            {
                menu_stage = 3;
            }
            if (currentKBS.IsKeyDown(Keys.Space) && !previousKBS.IsKeyDown(Keys.Space) && selected == 3 && menu_stage == 2)
            {
                RestoreDefaults();
            }
            char temp = currentKBS.GetPressedKeys().ToString().ToUpper()[0];
            Keys key;
            key = (Keys)temp;
            selected = (int)MathHelper.Clamp(selected, clamp_min, clamp_max);

            c2 = Color.Yellow; c3 = Color.Yellow; c4 = Color.Yellow; c5 = Color.Yellow; c6 = Color.Yellow;

            switch (selected)
            {
                case 0:
                    c2 = Color.LimeGreen;
                    break;

                case 1:
                    c3 = Color.LimeGreen;
                    break;

                case 2:
                    c4 = Color.LimeGreen;
                    break;

                case 3:
                    c5 = Color.LimeGreen;
                    break;

                case 4:
                    c6 = Color.LimeGreen;
                    break;
            }
        }

        public void RestoreDefaults()
        {
            fire_button = Buttons.A; boost_button = Buttons.RightTrigger; brake_button = Buttons.LeftTrigger;
        }

        public void RemapGP(GamePadState currentGPS, Buttons button)
        {
            int button_mode = 0;
            if (button == fire_button)
                button_mode = 0;
            if (button == boost_button)
                button_mode = 1;
            if (button == brake_button)
                button_mode = 2;

            Buttons previous = button;

            if(currentGPS.IsButtonDown(Buttons.A))
                button = Buttons.A;
            if (currentGPS.IsButtonDown(Buttons.B))
                button = Buttons.B;
            if (currentGPS.IsButtonDown(Buttons.X))
                button = Buttons.X;
            if (currentGPS.IsButtonDown(Buttons.Y))
                button = Buttons.Y;
            if (currentGPS.IsButtonDown(Buttons.LeftShoulder))
                button = Buttons.LeftShoulder;
            if (currentGPS.IsButtonDown(Buttons.RightShoulder))
                button = Buttons.RightShoulder;
            if (currentGPS.IsButtonDown(Buttons.LeftTrigger))
                button = Buttons.LeftTrigger;
            if (currentGPS.IsButtonDown(Buttons.RightTrigger))
                button = Buttons.RightTrigger;

            switch (button_mode)
            {
                case 0:
                    fire_button = button;
                    if (button == boost_button)
                        boost_button = previous;
                    if (button == brake_button)
                        brake_button = previous;
                    break;

                case 1:
                    boost_button = button;
                    if (button == fire_button)
                        fire_button = previous;
                    if (button == brake_button)
                        brake_button = previous;
                    break;

                case 2:
                    brake_button = button;
                    if (button == fire_button)
                        fire_button = previous;
                    if (button == boost_button)
                        boost_button = previous;
                    break;
            }
        }
    }
}
