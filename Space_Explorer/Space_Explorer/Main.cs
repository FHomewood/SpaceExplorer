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
        Texture2D texShip, texCircle, texVignette;
        SpriteFont fontDebug;
        int screenW, screenH;
        float elapsedTime;

        List<Planet> planetList = new List<Planet>();
        List<AsteroidBelt> beltList = new List<AsteroidBelt>();
        List<Ship>   shipList = new List<Ship>();
        List<Particle> particleList = new List<Particle>();

        public Main()
        {
            graphics = new GraphicsDeviceManager(this);
            graphics.PreferredBackBufferHeight = 720;
            graphics.PreferredBackBufferWidth = 1280;
            graphics.IsFullScreen = false;
            IsMouseVisible = true;
            Content.RootDirectory = "Content";
        }
        protected override void Initialize()
        {
            base.Initialize();
            screenH = graphics.PreferredBackBufferHeight;
            screenW = graphics.PreferredBackBufferWidth;
            shipList.Add(new Ship(new Vector2(screenW / 2, screenH / 2)));
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
            planetList.Add(new Planet(new Vector2(400, 360), 20, 20, Color.ForestGreen));
            planetList.Add(new Planet(new Vector2(400, 360), 4, 4, Color.Navy, 100, 27, 0));

            //Asteroid Belts
            beltList.Add(new AsteroidBelt(new Vector2(400, 360), 150, 200));
            beltList.Add(new AsteroidBelt(new Vector2(800, 360), 0, 100));
        }
        protected override void LoadContent()
        {
            sB = new SpriteBatch(GraphicsDevice);
            texShip = Content.Load<Texture2D>("Ship");
            texCircle = Content.Load<Texture2D>("Circle500");
            texVignette = Content.Load<Texture2D>("Transparent Vignette");
            fontDebug = Content.Load<SpriteFont>("Debug");
        }
        protected override void UnloadContent()
        {  }
        protected override void Update(GameTime gameTime)
        {
            newK = Keyboard.GetState();
            newM = Mouse.GetState();
            elapsedTime += (float)gameTime.ElapsedGameTime.TotalSeconds;
            if (particleList.Count > 50) particleList.Remove(particleList[0]);

            foreach (Particle particle in particleList) particle.Update();
            foreach (Ship ship in shipList)
            {
                foreach (Planet planet in planetList) ship.PlanetInteraction((float)gameTime.ElapsedGameTime.TotalSeconds,planet);
                ship.Update(oldK, newK, oldM, newM, particleList);
            }
            foreach (Planet planet in planetList) planet.Update(elapsedTime);
            foreach (AsteroidBelt belt in beltList) belt.Update();
            
            oldK = newK;
            oldM = newM;

            if (Keyboard.GetState().IsKeyDown(Keys.Escape)) Exit();
            base.Update(gameTime);
        }
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.TransparentBlack);

            sB.Begin();
            foreach (AsteroidBelt belt in beltList) belt.UnderDraw(sB, new Texture2D[] { texCircle }, new SpriteFont[] { fontDebug });
            foreach (Particle particle in particleList) particle.Draw(sB, new Texture2D[] { texCircle }, new SpriteFont[] { });
            foreach (Ship ship in shipList)
            {
                ship.CamDraw(sB, new Texture2D[] { texShip, texVignette }, new SpriteFont[] { fontDebug });
                ship.StaticDraw(sB, graphics, new Texture2D[] { texVignette }, new SpriteFont[] { });
            }

            foreach (Planet planet in planetList) planet.Draw(sB, new Texture2D[] { texCircle }, new SpriteFont[] { });
            foreach (AsteroidBelt belt in beltList) belt.OverDraw(sB, new Texture2D[] { texCircle }, new SpriteFont[] { fontDebug });
            sB.DrawString(fontDebug, "Elapsed Time: " + elapsedTime.ToString() + "s", Vector2.Zero, Color.White);
            sB.End();

            base.Draw(gameTime);
        }
    }
}
