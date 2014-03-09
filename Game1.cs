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
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Game1 : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        Camera camera;

        Ship player;

        Random R = new Random();

        List<AIShip> EnemyShips = new List<AIShip>();
        List<Explosion> Explosions = new List<Explosion>();
        List<Explosion> Sparks = new List<Explosion>();
        AIPath aiPath = new AIPath();

        AIVader aiVader;

        Model player_model, trench_model, red_laser_model, green_laser_model, Obstacle_model, Exhaust_model;
        BoundingBoxes.BoundingBoxComponent Obstacle_box, red_laser_box;
        BoundingBox trench_left_box, trench_right_box, trench_ground_box;

        SoundEffect XWing_Shoot_SFX, XWing_Engines_SFX, XWing_Hit_SFX;
        SoundEffect TrenchRun_MSC, ImpMarch_MSC, Victory_MSC, Title_MSC;
        SoundEffect TIE_Shoot_SFX, TIE_Engine_SFX, TIE_Explode_SFX, TIE_Hit_SFX;
        SoundEffect R5_GoingIn, R5_No;
        SoundEffect R2_WEOOW;
        SoundEffect Ben_Use;
        SoundEffect Vader_Have;
        SoundEffect Obst_Hit_SFX;
        SoundEffect Falcon_Quad_SFX;
        SoundEffect Boom_SFX;

        SoundEffectInstance XWing_Shoot_SFXi, TrenchRun_MSCi, XWing_Engines_SFXi, ImpMarch_MSCi, Victory_MSCi, Title_MSCi;

        Matrix trench_world = Matrix.Identity;

        Texture2D HBar, Explosion, Reticle, Spark, Menu_BG, Menu_GO, Menu_HS;

        GamePadState currentGPS, previousGPS; KeyboardState currentKBS, previousKBS;

        int score = 0;

        double catchUpTime = 0.0f, elapsed = 0.0f, lastShot = 0.0f;
        bool justShot = false, R2_Scream = false, Ben_Force = false, Vader_Now = false, Vader_Despawn = false;
        double last_reset = 0, active_time = 0, pause_time = 0, pause_time_total = 0;
        double last_reset_mil = 0, active_time_mil = 0, pause_time_mil = 0, pause_time_totel_mil = 0, elapsed_mil = 0;

        //TIE Variables
        Model TIE_Model, TIE_Vader;

        SpriteFont SWFont, SWFont_Big;

        Menu menu = new Menu();

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            trench_world.Translation = new Vector3(0, -2.0f, 0);

            trench_left_box = new BoundingBox(new Vector3(2.0f, -0.7f, 0f), new Vector3(5f, 3.35f, 500f));
            trench_ground_box = new BoundingBox(new Vector3(-2.0f, -1.5f, 0f), new Vector3(2.0f, -0.7f, 500f));
            trench_right_box = new BoundingBox(new Vector3(-5.0f, -0.7f, 0f), new Vector3(-2.0f, 3.5f, 500f));

            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            //3D Models
            player_model = Content.Load<Model>("XWing\\XWing");
            trench_model = Content.Load<Model>("DeathStar\\Trench_BASE");
            red_laser_model = Content.Load<Model>("Laser\\Laser_Red_MESH");
            green_laser_model = Content.Load<Model>("Laser\\Laser_Green_MESH");
            TIE_Model = Content.Load<Model>("TIE\\TieFighter");
            TIE_Vader = Content.Load<Model>("TIE\\TIE_Vader");
            Obstacle_model = Content.Load<Model>("Obstacle\\Trench_OBST");
            Exhaust_model = Content.Load<Model>("Port\\DS_Exhaust");


            //Textures
            HBar = Content.Load<Texture2D>("UI\\HBar");
            Explosion = Content.Load<Texture2D>("Explosion");
            Reticle = Content.Load<Texture2D>("Reticle");
            Spark = Content.Load<Texture2D>("Spark");
            Menu_BG = Content.Load<Texture2D>("Menu\\Menu_BG");
            Menu_GO = Content.Load<Texture2D>("Menu\\Menu_GO");
            Menu_HS = Content.Load<Texture2D>("Menu\\Menu_HS");

            //Sounds
            XWing_Shoot_SFX = Content.Load<SoundEffect>("Audio\\X_Wing_Shoot");
            XWing_Shoot_SFXi = XWing_Shoot_SFX.CreateInstance();
            XWing_Shoot_SFXi.Volume = 0.5f;
            XWing_Hit_SFX = Content.Load<SoundEffect>("Audio\\X_Wing_Hit");
            XWing_Engines_SFX = Content.Load<SoundEffect>("Audio\\X_Wing_Engines");
            XWing_Engines_SFXi = XWing_Engines_SFX.CreateInstance();
            XWing_Engines_SFXi.IsLooped = true;
            TrenchRun_MSC = Content.Load<SoundEffect>("Audio\\44-the-trench-run");
            TrenchRun_MSCi = TrenchRun_MSC.CreateInstance();
            TrenchRun_MSCi.Volume = menu.bg_vol / 10;
            TrenchRun_MSCi.IsLooped = true;
            ImpMarch_MSC = Content.Load<SoundEffect>("Audio\\40-imperial-march");
            ImpMarch_MSCi = ImpMarch_MSC.CreateInstance();
            ImpMarch_MSCi.Volume = menu.bg_vol / 10;
            ImpMarch_MSCi.IsLooped = true;
            TIE_Shoot_SFX = Content.Load<SoundEffect>("Audio\\TIE\\TIE_Shoot");
            TIE_Engine_SFX = Content.Load<SoundEffect>("Audio\\TIE\\TIE_Engine");
            TIE_Explode_SFX = Content.Load<SoundEffect>("Audio\\TIE\\TIE_Explode");
            TIE_Hit_SFX = Content.Load<SoundEffect>("Audio\\TIE\\TIE_Hit");
            R5_GoingIn = Content.Load<SoundEffect>("Audio\\Luke\\R5GoingIn");
            R5_No = Content.Load<SoundEffect>("Audio\\Luke\\Nooo");
            R2_WEOOW = Content.Load<SoundEffect>("Audio\\R2\\WEOOW");
            Ben_Use = Content.Load<SoundEffect>("Audio\\Ben\\UseTheForce");
            Vader_Have = Content.Load<SoundEffect>("Audio\\Vader\\HaveYouNow");
            Obst_Hit_SFX = Content.Load<SoundEffect>("Audio\\Obst_Hit");
            Falcon_Quad_SFX = Content.Load<SoundEffect>("Audio\\Falcon\\QuadLaser");
            Boom_SFX = Content.Load<SoundEffect>("Audio\\DeathStarBoom");
            Victory_MSC = Content.Load<SoundEffect>("Audio\\Victory");
            Victory_MSCi = Victory_MSC.CreateInstance();
            Victory_MSCi.Volume = menu.bg_vol / 10;
            Victory_MSCi.IsLooped = true;
            Title_MSC = Content.Load<SoundEffect>("Audio\\Title");
            Title_MSCi = Title_MSC.CreateInstance();
            Title_MSCi.Volume = menu.bg_vol / 10;
            Title_MSCi.IsLooped = true;
            Title_MSCi.Play();

            //Fonts
            SWFont = Content.Load<SpriteFont>("UI\\SWFont");
            SWFont_Big = Content.Load<SpriteFont>("UI\\SWFont_Big");

            //Pre-determined collision boxes
            red_laser_box = new BoundingBoxes.BoundingBoxComponent(red_laser_model, GraphicsDevice);
            Obstacle_box = new BoundingBoxes.BoundingBoxComponent(Obstacle_model, GraphicsDevice);

            //Populate Trench with obstacles
            Random R = new Random();
            float Z = 5.0f;
            for (int i = 0; i < 60; i++)
            {
                float positionY = R.Next(0, 4);
                if (positionY == 2.0f)
                    positionY -= 0.3f;
                aiPath.Obstacles[i] = new Obstacle(new Vector3(0f, positionY, Z), Obstacle_box.ReturnBox(), GraphicsDevice);
                Z += 6.0f;
            }
            aiPath.InitCurve();

            //Classes dependent on Asset Load
            player = new Ship(new Vector3(0, 0, 350f), player_model, red_laser_box, GraphicsDevice);
            aiVader = new AIVader(new Vector3(0, 0, 355), TIE_Vader, red_laser_box, TIE_Shoot_SFX, GraphicsDevice);
            camera = new Camera(new Vector3(player.position.X, player.position.Y, player.position.Z - 2.0f), player.position,
                (float)graphics.GraphicsDevice.Viewport.Width / graphics.GraphicsDevice.Viewport.Height);
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
            Content.Unload();
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            elapsed = gameTime.TotalGameTime.TotalSeconds;
            elapsed_mil = gameTime.TotalGameTime.TotalMilliseconds;

            currentGPS = GamePad.GetState(PlayerIndex.One);
            currentKBS = Keyboard.GetState();

            if (menu.Restart)
            {
                RestartGame(gameTime);
                Title_MSCi.Stop();
                Victory_MSCi.Stop();
            }

            if (player.health <= 0 && !menu.Active)
            {
                menu.menu_stage = 4;
                menu.Active = true;
                R5_No.Play();
                TrenchRun_MSCi.Stop();
                XWing_Engines_SFXi.Stop();
                ImpMarch_MSCi.Play();
            }

            if (menu.Active)
            {
                if (menu.GamePadMode)
                    menu.GPUpdate(currentGPS, previousGPS);
                else
                    menu.KBUpdate(currentKBS, previousKBS);
                TrenchRun_MSCi.Volume = (float)(menu.bg_vol / 10);
                ImpMarch_MSCi.Volume = (float)(menu.bg_vol / 10);
                Title_MSCi.Volume = (float)(menu.bg_vol / 10);
                Victory_MSCi.Volume = (float)(menu.bg_vol / 10);
            }

            // Allows the game to exit
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || menu.menu_stage == 3)
                this.Exit();

            if (menu.Active && currentGPS.Buttons.A == ButtonState.Pressed && previousGPS.Buttons.A != ButtonState.Pressed && menu.selected == 0 && (menu.menu_stage == 1))
            {
                menu.Active = false;
                pause_time_total += elapsed - pause_time;
                pause_time_totel_mil += elapsed_mil - pause_time_mil;
                TrenchRun_MSCi.Resume();
                XWing_Engines_SFXi.Resume();
            }
            if ((currentKBS.IsKeyDown(Keys.Escape) && !previousKBS.IsKeyDown(Keys.Escape)) || (currentGPS.Buttons.Start == ButtonState.Pressed && previousGPS.Buttons.Start != ButtonState.Pressed) && menu.menu_stage == 1)
            {
                if (!menu.Active)
                {
                    pause_time = elapsed;
                    pause_time_mil = elapsed_mil;
                    TrenchRun_MSCi.Pause();
                    XWing_Engines_SFXi.Pause();
                }
                if (menu.Active)
                {
                    pause_time_total += elapsed - pause_time;
                    pause_time_totel_mil += elapsed_mil - pause_time_mil;
                    TrenchRun_MSCi.Resume();
                    XWing_Engines_SFXi.Resume();
                }
                menu.Active = !menu.Active;
            }
            if (!menu.Active)
            {
                if (justShot)
                {
                    if (elapsed >= lastShot + 0.3)
                        justShot = false;
                }

                if (EnemyShips.Count == 0 && player.position.Z < 349.0f)
                {

                    catchUpTime += 500;

                    if (aiPath.CurvePosition.GetPointOnCurve((float)(catchUpTime)).Z >= player.position.Z - 3.5f)
                    {
                        if (R.Next(0, 2) == 1)
                        {
                            EnemyShips.Add(new AIShip(Vector3.Zero, 1.0f, catchUpTime, TIE_Model, red_laser_box, TIE_Shoot_SFX, GraphicsDevice));
                            EnemyShips.Add(new AIShip(Vector3.Zero, -1.0f, catchUpTime, TIE_Model, red_laser_box, TIE_Shoot_SFX, GraphicsDevice));
                        }
                        else
                            EnemyShips.Add(new AIShip(Vector3.Zero, 0, catchUpTime, TIE_Model, red_laser_box, TIE_Shoot_SFX, GraphicsDevice));
                        catchUpTime = 0;

                        TIE_Engine_SFX.Play();
                    }
                }

                foreach (AIShip aiship in EnemyShips)
                {
                    if (!aiship.spawn_time_set)
                    {
                        aiship.spawnTime = active_time_mil;
                        aiship.spawn_time_set = true;
                    }

                    for (int i = 0; i < player.Lasers.Count; i++)
                    {
                        if (player.Lasers[i].box.ReturnBox().Intersects(aiship.box.ReturnBox()))
                        {
                            player.Lasers.RemoveAt(i);
                            aiship.TakeHit();
                            Sparks.Add(new Explosion(aiship.position, 3));
                            TIE_Hit_SFX.Play();
                        }
                    }

                    if (player.position.Z < 350.0f)
                        aiship.position = aiPath.CurvePosition.GetPointOnCurve((float)((active_time_mil - aiship.spawnTime) + aiship.catchUpTime));
                    else
                        aiship.position.Y += 0.8f;
                    aiship.CheckCollide(player);
                    aiship.Update(gameTime, GraphicsDevice, player.position);
                }
                for (int i = 0; i < EnemyShips.Count; i++)
                {
                    foreach (Obstacle obstacle in aiPath.Obstacles)
                    {
                        for (int j = 0; j < EnemyShips[i].Lasers.Count; j++)
                        {
                            if (EnemyShips[i].Lasers[j].box.ReturnBox().Intersects(obstacle.box.ReturnBox()))
                            {
                                EnemyShips[i].Lasers.RemoveAt(j);
                                continue;
                            }
                        }
                    }
                    if (EnemyShips[i].box.ReturnBox().Intersects(player.box.ReturnBox()))
                    {
                        score += 25;
                        player.TakeHit(15);
                        Explosions.Add(new Explosion(EnemyShips[i].position, 25));

                        if (player.position.X > EnemyShips[i].position.X)
                        {
                            player.bounceLeft = true;
                            player.bouncedLeft = elapsed;
                            XWing_Hit_SFX.Play();
                        }
                        else
                        {
                            player.bounceRight = true;
                            player.bouncedRight = elapsed;
                            XWing_Hit_SFX.Play();
                        }

                        EnemyShips.RemoveAt(i);
                        TIE_Explode_SFX.Play();

                        continue;
                    }
                    if (EnemyShips[i].Health <= 0)
                    {
                        score += 100;
                        Explosions.Add(new Explosion(EnemyShips[i].position, 25));
                        EnemyShips.RemoveAt(i);
                        TIE_Explode_SFX.Play();
                        continue;
                    }
                    if (EnemyShips[i].position.Y > 6.0f)
                    {
                        EnemyShips.RemoveAt(i);
                        continue;
                    }
                    if (player.position.Z - EnemyShips[i].position.Z > 15.0f)
                    {
                        EnemyShips.RemoveAt(i);
                        continue;
                    }
                }
                foreach (Obstacle obstacle in aiPath.Obstacles)
                {
                    for (int i = 0; i < player.Lasers.Count; i++)
                    {
                        if (player.Lasers[i].box.ReturnBox().Intersects(obstacle.box.ReturnBox()))
                        {
                            Sparks.Add(new Explosion(player.Lasers[i].Position, 3));
                            Obst_Hit_SFX.Play();
                            player.Lasers.RemoveAt(i);
                            continue;
                        }
                    }

                    if (player.box.ReturnBox().Intersects(obstacle.box.ReturnBox()) && player.position.Y <= obstacle.World.Translation.Y && !player.obj_collide)
                    {
                        player.obj_collide = true;
                        player.bounceDown = true;
                        player.bouncedDown = elapsed;
                        player.TakeHit(50);
                        XWing_Hit_SFX.Play();
                    }
                    else if (player.box.ReturnBox().Intersects(obstacle.box.ReturnBox()) && !player.obj_collide)
                    {
                        player.obj_collide = true;
                        player.bounceUp = true;
                        player.bouncedUp = elapsed;
                        player.TakeHit(50);
                        XWing_Hit_SFX.Play();
                    }
                }
                for (int i = 0; i < Explosions.Count; i++)
                {
                    if (Explosions[i].Frame > 25)
                        Explosions.RemoveAt(i);
                }

                //Bounce back on Collision
                if (player.box.ReturnBox().Intersects(trench_left_box))
                {
                    player.bounceRight = true; player.bounceLeft = false;
                    player.bounceUp = false; player.bounceDown = false;
                    player.bouncedRight = elapsed;
                    player.TakeHit(25);
                    XWing_Hit_SFX.Play();
                }
                if (player.box.ReturnBox().Intersects(trench_right_box))
                {
                    player.bounceRight = false; player.bounceLeft = true;
                    player.bounceUp = false; player.bounceDown = false;
                    player.bouncedLeft = elapsed;
                    player.TakeHit(25);
                    XWing_Hit_SFX.Play();
                }
                if (player.box.ReturnBox().Intersects(trench_ground_box))
                {
                    player.bounceRight = false; player.bounceLeft = false;
                    player.bounceUp = true; player.bounceDown = false;
                    player.bouncedUp = elapsed;
                    player.TakeHit(25);
                    XWing_Hit_SFX.Play();
                }

                if (player.shields <= 0 && !R2_Scream)
                {
                    R2_WEOOW.Play();
                    R2_Scream = true;
                }

                
                if (player.position.Z > 360 && !Vader_Now)
                {
                    Vader_Have.Play();
                    Vader_Now = true;
                }
                if (player.position.Z > 475 && !Vader_Despawn)
                {
                    Falcon_Quad_SFX.Play();
                    Explosions.Add(new Explosion(aiVader.position, 25));
                    Vader_Despawn = true;
                }
                if (player.position.Z > 495 && !Ben_Force)
                {
                    Ben_Use.Play();
                    Ben_Force = true;
                }

                if (Vader_Now && !Vader_Despawn)
                {
                    aiVader.Update(gameTime, GraphicsDevice, player.position);

                    for (int i = 0; i < aiVader.Lasers.Count; i++)
                    {
                        if (aiVader.Lasers[i].box.ReturnBox().Intersects(player.box.ReturnBox()))
                        {
                            aiVader.Lasers.RemoveAt(i);
                            player.TakeHit(15);
                        }
                    }
                }

                if (player.position.Z > 505 && menu.GamePadMode && currentGPS.IsButtonDown(menu.fire_button))
                {
                    menu.Active = true;
                    menu.menu_stage = 6;
                    Boom_SFX.Play();
                    Victory_MSCi.Play();
                    TrenchRun_MSCi.Stop();
                    XWing_Engines_SFXi.Stop();
                }
                if (player.position.Z > 505 && !menu.GamePadMode && currentKBS.IsKeyDown(menu.fire_key))
                {
                    menu.Active = true;
                    menu.menu_stage = 6;
                    Boom_SFX.Play();
                    Victory_MSCi.Play();
                    TrenchRun_MSCi.Stop();
                    XWing_Engines_SFXi.Stop();
                }
                if (player.position.Z > 519)
                {
                    menu.menu_stage = 4;
                    menu.Active = true;
                    R5_No.Play();
                    TrenchRun_MSCi.Stop();
                    XWing_Engines_SFXi.Stop();
                    ImpMarch_MSCi.Play();
                }


                //Update Player
                player.Update(gameTime);

                if (menu.GamePadMode)
                    input_GamePad(currentGPS, previousGPS);
                else
                    input_KeyBoard(currentKBS, previousKBS);
                //Update Camera based on Player
                if (player.position.Z < 360 || player.position.Z > 480)
                    camera.Position = new Vector3(player.position.X, player.position.Y + 0.25f, player.position.Z - 1.25f);
                else
                    camera.Position = new Vector3(player.position.X, player.position.Y + 0.25f, player.position.Z - 3.25f);
                camera.LookAt = player.position;
                camera.Update();
                //Ensure the BG Music is play
                if (TrenchRun_MSCi.State == SoundState.Stopped)
                    TrenchRun_MSCi.Play();
            }

            previousGPS = currentGPS;
            previousKBS = currentKBS;

            base.Update(gameTime);
        }

        public void input_GamePad(GamePadState currentGPS, GamePadState previousGPS)
        {
            if (!player.bounceLeft && !player.bounceRight)
                player.position.X -= (GamePad.GetState(PlayerIndex.One).ThumbSticks.Left.X / 14.0f);
            if (!player.bounceDown && !player.bounceUp)
                player.position.Y += (GamePad.GetState(PlayerIndex.One).ThumbSticks.Left.Y / 14.0f);

            if (currentGPS.IsButtonDown(menu.fire_button) && !justShot)
            {
                for (int i = 0; i < 2; i++)
                {
                    player.Fire(GraphicsDevice);
                }
                XWing_Shoot_SFX.Play();
                lastShot = elapsed;
                justShot = true;
            }

            if (currentGPS.IsButtonDown(menu.brake_button))
            {
                player.accelaration = 0.02f;
                XWing_Engines_SFXi.Pitch = -1.0f;
            }
            else if (currentGPS.IsButtonDown(menu.boost_button) && !player.energy_depleted)
            {
                player.use_energy = 0.5f;
                player.accelaration = 0.1f;
                XWing_Engines_SFXi.Pitch = 1.0f;
            }
            else
            {
                XWing_Engines_SFXi.Pitch = 0.0f;
            }
        }
        public void input_KeyBoard(KeyboardState currentKBS, KeyboardState previousKBS)
        {
            if (!player.bounceLeft && !player.bounceRight)
            {
                if (currentKBS.IsKeyDown(menu.right_key))
                    player.position.X -= (1.0f / 14.0f);
                if (currentKBS.IsKeyDown(menu.left_key))
                    player.position.X += (1.0f / 14.0f);
            }
            if (!player.bounceDown && !player.bounceUp)
            {
                if (currentKBS.IsKeyDown(menu.down_key))
                    player.position.Y -= (1.0f / 14.0f);
                if (currentKBS.IsKeyDown(menu.up_key))
                    player.position.Y += (1.0f / 14.0f);
            }

            if (currentKBS.IsKeyDown(menu.fire_key) && !justShot)
            {
                for (int i = 0; i < 2; i++)
                {
                    player.Fire(GraphicsDevice);
                }
                XWing_Shoot_SFX.Play();
                lastShot = elapsed;
                justShot = true;
            }

            if (currentKBS.IsKeyDown(menu.brake_key))
            {
                player.accelaration = 0.02f;
                XWing_Engines_SFXi.Pitch = -1.0f;
            }
            else if (currentKBS.IsKeyDown(menu.boost_key))
            {
                if (!player.energy_depleted)
                {
                    player.use_energy = 0.5f;
                    player.accelaration = 0.1f;
                    XWing_Engines_SFXi.Pitch = 1.0f;
                }
            }
            else
            {
                XWing_Engines_SFXi.Pitch = 0.0f;
            }
        }

        //Main Draw Function
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);

            GraphicsDevice.DepthStencilState = DepthStencilState.Default;

            if (!menu.Active)
            {
                //Draw 3D Models
                DrawModel(player_model, player.World);
                DrawModel(trench_model, trench_world);
                //Draw all lasers
                foreach (Laser laser in player.Lasers)
                {
                    DrawModel(red_laser_model, laser.World);
                }
                foreach (AIShip aiship in EnemyShips)
                {
                    aiship.DrawLasers(camera, green_laser_model, GraphicsDevice);
                    DrawModel(TIE_Model, aiship.World);
                }
                if (Vader_Now && !Vader_Despawn)
                {
                    DrawModel(TIE_Vader, aiVader.World);
                    aiVader.DrawLasers(camera, green_laser_model, GraphicsDevice);
                }
                foreach (Obstacle obstacle in aiPath.Obstacles)
                {
                    DrawModel(Obstacle_model, obstacle.World);
                }
                DrawModel(Exhaust_model, Matrix.CreateTranslation(0, -0.3f, 520f));

                //Draw 2D UI
                spriteBatch.Begin();
                foreach (AIShip aiship in EnemyShips)
                {
                    aiship.DrawHP(camera, GraphicsDevice, spriteBatch, HBar, player.position);
                }
                foreach (Explosion explosion in Explosions)
                {
                    explosion.Draw(camera, GraphicsDevice, spriteBatch, Explosion, player.position);
                }
                foreach (Explosion spark in Sparks)
                {
                    spark.Draw(camera, GraphicsDevice, spriteBatch, Spark, player.position);
                }
                player.DrawReticle(camera, GraphicsDevice, spriteBatch, Reticle);
                spriteBatch.Draw(HBar, new Rectangle(10, 10, (int)player.health / 2, 10), Color.LimeGreen);
                spriteBatch.Draw(HBar, new Rectangle(10, 25, (int)player.shields * 2, 10), Color.DeepSkyBlue);
                spriteBatch.Draw(HBar, new Rectangle(10, 40, (int)player.energy * 2, 10), Color.Yellow);
                spriteBatch.DrawString(SWFont, "Score: " + score, new Vector2(10, 60), Color.Yellow);
                active_time = elapsed - last_reset - pause_time_total;
                active_time_mil = elapsed_mil - last_reset_mil - pause_time_totel_mil;
                spriteBatch.DrawString(SWFont, "Time: " + active_time.ToString("###.##"), new Vector2(10, 90), Color.Yellow);
                if (player.position.Z > 505 && menu.GamePadMode)
                    spriteBatch.DrawString(SWFont_Big, "press " + menu.fire_button.ToString().ToLower() + " to fire proton torpedo!", new Vector2(10, 420), Color.Red);
                if (player.position.Z > 505 && !menu.GamePadMode)
                    spriteBatch.DrawString(SWFont, "press " + menu.fire_key.ToString().ToLower() + " to fire proton torpedo!", new Vector2(10, 420), Color.Red);


                spriteBatch.End();
            }
            else
            {
                spriteBatch.Begin();

                spriteBatch.Draw(Menu_BG, new Rectangle(0, 0, Menu_BG.Width, Menu_BG.Height), Color.White);
                if (menu.menu_stage == 2)
                {
                    for (int i = 0; i < menu.bg_vol; i++)
                    {
                        spriteBatch.Draw(HBar, new Rectangle(10 + i * 30, 125, 20, 20), Color.Green);
                    }
                }
                if (menu.menu_stage == 4)
                {
                    spriteBatch.Draw(Menu_GO, new Rectangle(0, 0, Menu_GO.Width, Menu_GO.Height), Color.White);
                }
                if (menu.menu_stage == 5)
                {
                    spriteBatch.Draw(Menu_BG, new Rectangle(0, 0, Menu_BG.Width, Menu_BG.Height), Color.White);
                }
                if (menu.menu_stage == 6)
                {
                    spriteBatch.Draw(Menu_HS, new Rectangle(0, 0, Menu_HS.Width, Menu_HS.Height), Color.White);
                    spriteBatch.DrawString(SWFont_Big, "score: " + score.ToString(), new Vector2(450, 100), Color.Yellow);
                }
                menu.Draw(spriteBatch, SWFont_Big, SWFont);

                spriteBatch.End();
            }

            base.Draw(gameTime);
        }

        //3D Model Draw Function
        private void DrawModel(Model model, Matrix world)
        {
            Matrix[] transforms = new Matrix[model.Bones.Count];
            model.CopyAbsoluteBoneTransformsTo(transforms);

            foreach (ModelMesh mesh in model.Meshes)
            {
                foreach (BasicEffect effect in mesh.Effects)
                {
                    effect.EnableDefaultLighting();
                    effect.World = transforms[mesh.ParentBone.Index] * world;
                    effect.View = camera.View;
                    effect.Projection = camera.Projection;
                }
                mesh.Draw();
            }
        }

        //Restart game
        private void RestartGame(GameTime gameTime)
        {
            menu.Restart = false;
            menu.Active = false;

            player = new Ship(new Vector3(0, 0, -2), player_model, red_laser_box, GraphicsDevice);
            score = 0;

            last_reset = gameTime.TotalGameTime.TotalSeconds;
            last_reset_mil = gameTime.TotalGameTime.TotalMilliseconds;
            active_time = 0; active_time_mil = 0;
            pause_time = 0; pause_time_mil = 0;
            pause_time_total = 0; pause_time_totel_mil = 0;

            for (int i = 0; i < EnemyShips.Count; )
                EnemyShips.RemoveAt(i);
            for (int i = 0; i < player.Lasers.Count; )
                player.Lasers.RemoveAt(i);

            EnemyShips.Add(new AIShip(new Vector3(1.0f, 0, -2.0f), 1f, 0, TIE_Model, red_laser_box, TIE_Shoot_SFX, GraphicsDevice));
            EnemyShips.Add(new AIShip(new Vector3(-1.0f, 0, -2.0f), -1f, 0, TIE_Model, red_laser_box, TIE_Shoot_SFX, GraphicsDevice));

            R5_GoingIn.Play();
            TrenchRun_MSCi.Stop();
            ImpMarch_MSCi.Stop();
            R2_Scream = false;
            Vader_Now = false;
            Ben_Force = false;
            Vader_Despawn = false;
            
        }
    }
}
