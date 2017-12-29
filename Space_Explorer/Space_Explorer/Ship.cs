using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Space_Explorer
{
    class Ship
    {
        private Vector2 loc, vel, camFocusLoc;
        private Planet closestBody;
        private AsteroidBelt currentBelt;
        private float rotation, health = 100, Hittimer, camFocusZoom,camFocusRot, money = 0f;
        private int screenW, screenH;

        public Ship(Vector2 loc, int screenW, int screenH)
        {
            this.loc = loc;
            this.screenW = screenW;
            this.screenH = screenH;
            this.closestBody = new Planet(5000 * Vector2.One, 1, 0, Color.Black);
        }

        public void Update(Camera cam, KeyboardState oldK, KeyboardState newK, MouseState oldM, MouseState newM, List<Particle> particleList)
        {
            Random rand = new Random();
            if (newK.IsKeyDown(Keys.W))
            {
                vel += 0.02f * Vector2.Transform(-Vector2.UnitY, Matrix.CreateRotationZ(rotation));
                particleList.Add(
                    new Particle(
                        loc - 6/cam.Zoom * Vector2.Transform(-Vector2.UnitY, Matrix.CreateRotationZ(rotation)),
                        vel + Vector2.Transform((float)rand.NextDouble() * Vector2.UnitX, Matrix.CreateRotationZ((float)rand.NextDouble() * MathHelper.TwoPi)) / 4/cam.Zoom,
                        Color.FromNonPremultiplied(rand.Next(200, 255), rand.Next(100, 200), rand.Next(0, 100), 255),
                        50,
                        3/cam.Zoom,
                        0f
                        )
                    );
            }
            if (newK.IsKeyDown(Keys.S)) vel -= 0.01f * Vector2.Transform(-Vector2.UnitY, Matrix.CreateRotationZ(rotation));
            if (newK.IsKeyDown(Keys.A)) rotation -= 0.05f;
            if (newK.IsKeyDown(Keys.D)) rotation += 0.05f;
            if (newK.IsKeyDown(Keys.Space) && currentBelt != null) 
                foreach(Asteroid asteroid in currentBelt.Asteroids)
                {
                    if ((asteroid.Loc - loc).Length() < 250f / cam.Zoom)
                    {
                        Vector2 Difference = Vector2.Transform(asteroid.Loc - loc,Matrix.CreateRotationZ(-rotation + MathHelper.PiOver2));
                        if (Math.Abs(Math.Atan2(Difference.Y, Difference.X)) < Math.PI / 12)
                        {
                            asteroid.Radius -= 0.03f;
                            money += (float)(Math.Pow(asteroid.Radius + 0.03f, 2) - Math.Pow(asteroid.Radius, 2))/1000f;
                        }
                    }
                        
                }

            loc += vel;


            camFocusLoc = loc;
            if (currentBelt != null)
            {
                camFocusZoom = screenH / 250f;
                camFocusRot = -rotation;
            }
            else
            {
                camFocusZoom = screenH / 2 / (closestBody.Loc - loc).Length();
                camFocusRot = -MathHelper.PiOver2 - (float)Math.Atan2((loc - closestBody.Loc).Y, (loc - closestBody.Loc).X);
            }

            cam.X += (camFocusLoc.X - cam.X) / 20;
            cam.Y += (camFocusLoc.Y - cam.Y) / 20;
            cam.Zoom += (camFocusZoom - cam.Zoom) / 20;
            float RotDifference = ((camFocusRot - cam.Rotation) % MathHelper.TwoPi);
            cam.Rotation += RotDifference / 20;
            if (health < 0) { health = 0; }
            if (Hittimer > 0) { Hittimer--; }
        }

        public void PlanetInteraction(Camera cam, float frametime, Planet planet)
        {
            Vector2 difference = (planet.Loc - loc);
            if (difference.Length() < (closestBody.Loc - loc).Length()) closestBody = planet;
            vel += difference * planet.Mass * (float)Math.Pow(difference.Length(), -3);
            if (difference.Length() < planet.Radius + 10f/cam.Zoom)
            {
                Vector2 parallel      = difference / difference.Length();
                Vector2 perpendicular = Vector2.Transform(parallel, Matrix.CreateRotationZ(-MathHelper.PiOver2));
                vel = perpendicular * Vector2.Dot(perpendicular, vel) - parallel * Vector2.Dot(parallel, vel);
                if (vel.Length() > 0.5f) { health -= vel.Length() * 10; Hittimer = 100; }
                if (vel.Length() < 0.1f) { vel = Vector2.Zero; }
                vel /= 1.5f;
                loc += (difference.Length() - planet.Radius - 10f / cam.Zoom) * difference / difference.Length();
            }
        }

        public void BeltInteraction(Camera cam,KeyboardState newK, AsteroidBelt belt)
        {
            currentBelt = null;
            if ((loc-belt.loc).Length()>belt.inRad && (loc - belt.loc).Length() < belt.outRad)
            {
                currentBelt = belt;
            }
            if (newK.IsKeyDown(Keys.Space)) { }
        }

        public void CamDraw(Camera cam, KeyboardState newK, SpriteBatch sB, Texture2D[] textures, SpriteFont[] fonts)
        {
            if (newK.IsKeyDown(Keys.Space) && currentBelt != null)
                sB.Draw(textures[1], loc, null, Color.FromNonPremultiplied(0,new Random().Next(50,150),new Random().Next(150,200), 100),
                rotation, new Vector2(textures[1].Width / 2, textures[1].Height), 0.5f / cam.Zoom, SpriteEffects.None, 0f);
            sB.Draw(textures[0], loc, null, Color.White, rotation, new Vector2(textures[0].Width / 2, textures[0].Height / 2), 0.05f/cam.Zoom, SpriteEffects.None, 0f);
            sB.DrawString(fonts[0], health.ToString(), loc - Vector2.Transform(30 * Vector2.UnitY, Matrix.CreateRotationZ(-cam.Rotation)), Color.Red, -cam.Rotation, fonts[0].MeasureString(health.ToString())/2, 1f,SpriteEffects.None,1f);
        }
        public void StaticDraw(GraphicsDeviceManager graphics, SpriteBatch sB, Texture2D[] textures, SpriteFont[] fonts)
        {
            sB.Draw(textures[0], new Rectangle(0, 0, graphics.PreferredBackBufferWidth, graphics.PreferredBackBufferHeight), null, Color.FromNonPremultiplied(255, 0, 0, (int)(100 * (Hittimer / 100))), 0f, Vector2.Zero, SpriteEffects.None, 0.5f);
            sB.DrawString(fonts[0], Math.Round(money, 2).ToString(), Vector2.UnitY * screenH + Vector2.UnitX * screenW, Color.Green, 0f, fonts[0].MeasureString(Math.Round(money, 2).ToString()), 1f, SpriteEffects.None, 0f);
            sB.Draw(textures[1], new Rectangle(new Vector2(screenW - fonts[0].MeasureString(Math.Round(money, 2).ToString()).X, screenH).ToPoint(), new Vector2(1+fonts[0].MeasureString(Math.Round(money, 2).ToString()).Y * textures[1].Width/textures[1].Height, fonts[0].MeasureString(Math.Round(money, 2).ToString()).Y).ToPoint()), null, Color.Green, 0f, textures[1].Bounds.Size.ToVector2(), SpriteEffects.None, 0f);
        }
    }
}
