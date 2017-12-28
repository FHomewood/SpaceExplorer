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
        public Vector2 loc, vel, camFocusLoc;
        Planet closestBody;
        AsteroidBelt currentBelt;
        float rotation, health = 100, Hittimer, camFocusZoom,camFocusRot;
        int screenW, screenH;

        public Ship(Vector2 loc, int screenW, int screenH)
        {
            this.loc = loc;
            this.screenW = screenW;
            this.screenH = screenH;
            this.closestBody = new Planet(5000 * Vector2.One, 1, 0, Color.AliceBlue);
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
                        2/cam.Zoom,
                        0f
                        )
                    );
            }
            if (newK.IsKeyDown(Keys.S)) vel -= 0.01f * Vector2.Transform(-Vector2.UnitY, Matrix.CreateRotationZ(rotation));
            if (newK.IsKeyDown(Keys.A)) rotation -= 0.05f;
            if (newK.IsKeyDown(Keys.D)) rotation += 0.05f;

            loc += vel;


            camFocusLoc = loc;
            if (currentBelt != null)
            {
                camFocusZoom = screenH / 1000f;
                camFocusRot = -rotation;
            }
            else
            {
                camFocusZoom = screenH / 2 / (closestBody.GetLoc() - loc).Length();
                camFocusRot = -MathHelper.PiOver2 - (float)Math.Atan2((loc - closestBody.GetLoc()).Y, (loc - closestBody.GetLoc()).X);
            }

            cam.X += (camFocusLoc.X - cam.X) / 20;
            cam.Y += (camFocusLoc.Y - cam.Y) / 20;
            cam.Zoom += (camFocusZoom - cam.Zoom) / 20;
            cam.Rotation += (camFocusRot - cam.Rotation) / 20;

            currentBelt = null;
            if (health < 0) { health = 0; }
            if (Hittimer > 0) { Hittimer--; }
        }

        public void PlanetInteraction(Camera cam, float frametime, Planet planet)
        {
            Vector2 difference = (planet.GetLoc() - loc);
            if (difference.Length() < (closestBody.GetLoc() - loc).Length()) closestBody = planet;
            vel += difference * planet.GetMass() * (float)Math.Pow(difference.Length(), -3);
            if (difference.Length() < planet.GetRadius() + 10f/cam.Zoom)
            {
                Vector2 parallel      = difference / difference.Length();
                Vector2 perpendicular = Vector2.Transform(parallel, Matrix.CreateRotationZ(-MathHelper.PiOver2));
                vel = perpendicular * Vector2.Dot(perpendicular, vel) - parallel * Vector2.Dot(parallel, vel);
                if (vel.Length() > 0.5f) { health -= vel.Length() * 10; Hittimer = 100; }
                if (vel.Length() < 0.1f) { vel = Vector2.Zero; }
                vel /= 1.5f;
                loc += (difference.Length() - planet.GetRadius() - 10f / cam.Zoom) * difference / difference.Length();
            }
        }

        public void BeltInteraction(Camera cam, AsteroidBelt belt)
        {
            if ((loc-belt.loc).Length()>belt.inRad && (loc - belt.loc).Length() < belt.outRad)
            {
                currentBelt = belt;
            }
        }

        public void CamDraw(Camera cam, SpriteBatch sB, Texture2D[] textures, SpriteFont[] fonts)
        {
            sB.Draw(textures[0], loc, null, Color.White, rotation, new Vector2(textures[0].Width / 2, textures[0].Height / 2), 0.05f/cam.Zoom, SpriteEffects.None, 0f);
            sB.DrawString(fonts[0], health.ToString(), loc - Vector2.Transform(30 * Vector2.UnitY, Matrix.CreateRotationZ(-cam.Rotation)), Color.Red, -cam.Rotation, fonts[0].MeasureString(health.ToString())/2, 1f,SpriteEffects.None,1f);
        }
        public void StaticDraw(SpriteBatch sB, GraphicsDeviceManager graphics, Texture2D[] textures, SpriteFont[] fonts)
        {
            sB.Draw(textures[0], new Rectangle(0, 0, graphics.PreferredBackBufferWidth, graphics.PreferredBackBufferHeight),null, Color.FromNonPremultiplied(255,0,0,(int)(100 * (Hittimer/100))), 0f, Vector2.Zero, SpriteEffects.None, 0.5f);
        }
    }
}
