using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;

namespace Space_Explorer
{
    public class Main : Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch sB;
        KeyboardState oldK, newK;
        MouseState oldM, newM;
        Texture2D texPixel, texShip, texCircle, texMarkedCircle,
            texSector, texVignette, texCurrency, texThrottleSlider,
            texBackParallax, texBackground, texFuelTank, texFuelBar;
        SpriteFont fontDebug, fontBoldArial;
        Camera cam;
        int screenW, screenH, frames, fps;
        float elapsedTime;

        List<Planet> planetList = new List<Planet>();
        List<AsteroidBelt> beltList = new List<AsteroidBelt>();
        List<Ship>   shipList = new List<Ship>();
        List<Particle> particleList = new List<Particle>();

        public Main()
        {
            graphics = new GraphicsDeviceManager(this);
            graphics.PreferredBackBufferHeight = 1080;
            graphics.PreferredBackBufferWidth = 1920;
            graphics.IsFullScreen = false;
            IsMouseVisible = true;
            Content.RootDirectory = "Content";
        }
        protected override void Initialize()
        {
            base.Initialize();
            cam = new Camera(GraphicsDevice.Viewport);
            screenH = graphics.PreferredBackBufferHeight;
            screenW = graphics.PreferredBackBufferWidth;
            shipList.Add(new Ship(new Vector2(0, -700), screenW, screenH));
            oldK = Keyboard.GetState();
            oldM = Mouse.GetState();
            Random rand = new Random();
            for (int i = 0; i < 50; i++)
            {
                particleList.Add(
                    new Particle(
                        new Vector2(graphics.PreferredBackBufferWidth / 2, graphics.PreferredBackBufferHeight / 2),
                        Vector2.Transform((float)rand.NextDouble() * Vector2.UnitX, Matrix.CreateRotationZ(i*MathHelper.TwoPi/50)),
                        Color.FromNonPremultiplied(rand.Next(200, 255), rand.Next(100, 200), rand.Next(0, 100), 255),
                        120,
                        8,
                        0.6f
                        )
                    );
            }

            //Define Map
            //Planets
            planetList.Add(new Planet(Vector2.Zero, 700, 1400, Color.Goldenrod)); //Sun
            planetList.Add(new Planet(Vector2.Zero, 100, 200, Color.Brown      , new float[] { 1600 }       , new float[] { (float)1E4 }, new float[] { 0 }    ));
            planetList.Add(new Planet(Vector2.Zero, 100, 200, Color.Brown      , new float[] { 5000 }       , new float[] { 200 }       , new float[] { 0 }    )); //
            planetList.Add(new Planet(Vector2.Zero, 150, 300, Color.Purple     , new float[] { 10000 }      , new float[] { 300 }       , new float[] { 0 }    )); //
            planetList.Add(new Planet(Vector2.Zero, 200, 400, Color.ForestGreen, new float[] { 20000 }      , new float[] { 600 }       , new float[] { 0 }    )); //
            planetList.Add(new Planet(Vector2.Zero, 50 , 100, Color.Gray       , new float[] { 20000, 1000 }, new float[] { 600, 50 }   , new float[] { 0, 0 } )); //
            
            //Asteroid Belts
            beltList.Add(new AsteroidBelt(Vector2.Zero, 1070, 1400, new int[]   { 1    , 2    , 3    , 4    , 5    , 6    , 7    , 8    , 9    , 10   , 11   , 12   , 13   , 14   , 15   , 16   , 17   , 18   , 19   , 20   , 21   , 22   , 23   , 24    },
                                                                    new float[] { 0.15f, 0.05f, 0.01f, 0.01f, 0.01f, 0.01f, 0.01f, 0.01f, 0.01f, 0.01f, 0.01f, 0.01f, 0.01f, 0.01f, 0.01f, 0.01f, 0.01f, 0.01f, 0.01f, 0.01f, 0.01f, 0.01f, 0.01f, 0.01f },
                                                                    new float[] { 0.5f , 0.05f, 0.01f, 0.01f, 0.01f, 0.01f, 0.01f, 0.01f, 0.01f, 0.01f, 0.01f, 0.01f, 0.01f, 0.01f, 0.01f, 0.01f, 0.01f, 0.01f, 0.01f, 0.01f, 0.01f, 0.01f, 0.01f, 0.01f }));
            beltList.Add(new AsteroidBelt(Vector2.Zero, 1000, 1050, new int[]   { 1    , 2    , 3    , 4    , 5    , 6    , 7    , 8    , 9    , 10   , 11   , 12   , 13   , 14   , 15   , 16   , 17   , 18   , 19   , 20   , 21   , 22   , 23   , 24    },
                                                                    new float[] { 0.15f, 0.05f, 0.01f, 0.01f, 0.01f, 0.01f, 0.01f, 0.01f, 0.01f, 0.01f, 0.01f, 0.01f, 0.01f, 0.01f, 0.01f, 0.01f, 0.01f, 0.01f, 0.01f, 0.01f, 0.01f, 0.01f, 0.01f, 0.01f },
                                                                    new float[] { 0.5f , 0.05f, 0.01f, 0.01f, 0.01f, 0.01f, 0.01f, 0.01f, 0.01f, 0.01f, 0.01f, 0.01f, 0.01f, 0.01f, 0.01f, 0.01f, 0.01f, 0.01f, 0.01f, 0.01f, 0.01f, 0.01f, 0.01f, 0.01f }));
            }
        protected override void LoadContent()
        {
            sB = new SpriteBatch(GraphicsDevice);
            texPixel = Content.Load<Texture2D>("EvenPixel");
            texShip = Content.Load<Texture2D>("Ship");
            texCircle = Content.Load<Texture2D>("Circle500");
            texMarkedCircle = Content.Load<Texture2D>("MarkedCircle500");
            texSector = Content.Load<Texture2D>("500px30degSector");
            texVignette = Content.Load<Texture2D>("Transparent Vignette");
            texCurrency = Content.Load<Texture2D>("Currency");
            texThrottleSlider = Content.Load<Texture2D>("ThrottleSlider");
            texBackParallax = Content.Load<Texture2D>("BackgroundParallax");
            texBackground = Content.Load<Texture2D>("BackgroundSpace");
            texFuelTank = Content.Load<Texture2D>("FuelTank");
            texFuelBar = Content.Load<Texture2D>("FuelBar");

            fontDebug = Content.Load<SpriteFont>("Debug");
            fontBoldArial = Content.Load<SpriteFont>("BoldArial");
        }

        protected override void UnloadContent()
        {  }

        protected override void Update(GameTime gameTime)
        {
            newK = Keyboard.GetState();
            newM = Mouse.GetState();
            float oldtime = elapsedTime;
            elapsedTime += (float)gameTime.ElapsedGameTime.TotalSeconds;
            frames++;
            if (elapsedTime % 1 < oldtime % 1) { fps = frames; frames = 0; }
            if (particleList.Count > 50) particleList.Remove(particleList[0]);

            foreach (Particle particle in particleList) particle.Update();
            foreach (Planet planet in planetList) planet.Update(elapsedTime);
            foreach (AsteroidBelt belt in beltList) belt.Update();
            cam.Update();
            foreach (Ship ship in shipList)
            {
                foreach (Planet planet in planetList) ship.PlanetInteraction(cam, (float)gameTime.ElapsedGameTime.TotalSeconds, planet, elapsedTime);
                foreach (AsteroidBelt belt in beltList) ship.BeltInteraction(belt);
                ship.Update(cam,oldK, newK, oldM, newM, particleList, elapsedTime);
            }
            oldK = newK;
            oldM = newM;
            if (Keyboard.GetState().IsKeyDown(Keys.Escape)) Exit();
            base.Update(gameTime);
        }
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.TransparentBlack);

            sB.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, null, null, null, null, cam.Transform);
            foreach (AsteroidBelt belt in beltList) belt.UnderDraw(sB, new Texture2D[] { texCircle }, new SpriteFont[] { fontDebug });
            foreach (Particle particle in particleList) particle.Draw(sB, new Texture2D[] { texCircle }, new SpriteFont[] { });
            foreach (Planet planet in planetList) planet.Draw(sB, new Texture2D[] { texMarkedCircle }, new SpriteFont[] { });
            foreach (AsteroidBelt belt in beltList) belt.OverDraw(sB, new Texture2D[] { texCircle }, new SpriteFont[] { fontDebug });
            foreach (Ship ship in shipList) ship.CamDraw(cam, newK, sB, new Texture2D[] { texShip, texSector }, new SpriteFont[] { fontBoldArial });
            if (newK.IsKeyDown(Keys.Enter))
            { }
            sB.End();


            sB.Begin();
            foreach (Ship ship in shipList)
                ship.StaticDraw(graphics, sB, new Texture2D[] { texVignette, texCurrency, texPixel, texThrottleSlider, texFuelTank, texFuelBar }, new SpriteFont[] { fontDebug, fontBoldArial });
            sB.DrawString(fontDebug, "Elapsed Time: " + elapsedTime.ToString() + "s", Vector2.Zero, Color.White);
            sB.DrawString(fontDebug, "FPS: " + fps.ToString(), 14 * Vector2.UnitY, Color.White);
            sB.End();

            base.Draw(gameTime);
        }
    }
}
