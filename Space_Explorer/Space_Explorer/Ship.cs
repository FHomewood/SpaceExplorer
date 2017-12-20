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
        Vector2 loc, vel;
        float rotation;
        float health = 100;
        float Hittimer;

        public Ship(Vector2 loc)
        {
            this.loc = loc;
        }

        public void Update(KeyboardState oldK, KeyboardState newK, MouseState oldM, MouseState newM, List<Particle> particleList)
        {
            Random rand = new Random();
            if (newK.IsKeyDown(Keys.W))
            {
                vel += 0.02f * Vector2.Transform(-Vector2.UnitY, Matrix.CreateRotationZ(rotation));
                particleList.Add(
                    new Particle(
                        loc - 6f * Vector2.Transform(-Vector2.UnitY, Matrix.CreateRotationZ(rotation)),
                        vel + Vector2.Transform((float)rand.NextDouble() * Vector2.UnitX, Matrix.CreateRotationZ((float)rand.NextDouble() * MathHelper.TwoPi)) / 4,
                        Color.FromNonPremultiplied(rand.Next(200, 255), rand.Next(100, 200), rand.Next(0, 100), 255),
                        50,
                        2,
                        0f
                        )
                    );
            }
            if (newK.IsKeyDown(Keys.S)) vel -= 0.01f * Vector2.Transform(-Vector2.UnitY, Matrix.CreateRotationZ(rotation));
            if (newK.IsKeyDown(Keys.A)) rotation -= 0.05f;
            if (newK.IsKeyDown(Keys.D)) rotation += 0.05f;

            loc += vel;
            if (health < 0) { health = 0; }
            if (Hittimer > 0) { Hittimer--; }
        }

        public void PlanetInteraction(float frametime, Planet planet)
        {
            Vector2 difference = (planet.GetLoc() - loc);
            vel += difference * planet.GetMass() * (float)Math.Pow(difference.Length(), -3);
            if (difference.Length() < planet.GetRadius() + 10f)
            {
                Vector2 parallel      = difference / difference.Length();
                Vector2 perpendicular = Vector2.Transform(parallel, Matrix.CreateRotationZ(MathHelper.PiOver2));
                vel =perpendicular * Vector2.Dot(perpendicular, vel) - parallel * Vector2.Dot(parallel, vel);
                if (vel.Length() > 0.5f) { health -= vel.Length() * 10; Hittimer = 100; }
                vel /= 1.5f;
                loc += (difference.Length() - planet.GetRadius() - 10f) * difference / difference.Length();
            }
        }

        public void CamDraw(SpriteBatch sB, Texture2D[] textures, SpriteFont[] fonts)
        {
            sB.Draw(textures[0], loc, null, Color.White, rotation, new Vector2(textures[0].Width / 2, textures[0].Height / 2), 0.05f, SpriteEffects.None, 0f);
            sB.DrawString(fonts[0], health.ToString(),loc - 30* Vector2.UnitY, Color.Red, 0f, fonts[0].MeasureString(health.ToString())/2, 1f,SpriteEffects.None,1f);
        }
        public void StaticDraw(SpriteBatch sB, GraphicsDeviceManager graphics, Texture2D[] textures, SpriteFont[] fonts)
        {
            sB.Draw(textures[0], new Rectangle(0, 0, graphics.PreferredBackBufferWidth, graphics.PreferredBackBufferHeight),null, Color.FromNonPremultiplied(255,0,0,(int)(100 * (Hittimer/100))), 0f, Vector2.Zero, SpriteEffects.None, 0.5f);
        }
    }
}
