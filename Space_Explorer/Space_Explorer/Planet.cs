using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace Space_Explorer
{
    class Planet
    {
        Vector2 loc, orbloc;
        float rad, mass, tPeriod, orbrad, phase;
        Color col;
        

        public Planet(Vector2 loc, float rad, float mass, Color col)
        {
            this.orbloc  = loc;
            this.loc = loc;
            this.rad = rad;
            this.mass = mass;
            this.col = col;
            this.tPeriod = 1;
            this.orbrad = 0;
            this.phase = 0;
        }
        public Planet(Vector2 orbloc, float rad, float mass, Color col, float orbrad, float tPeriod, float phase)
        {
            this.orbloc  = orbloc;
            this.loc = orbloc;
            this.rad = rad;
            this.mass = mass;
            this.col = col;
            this.tPeriod = tPeriod;
            this.orbrad = orbrad;
            this.phase = phase;
        }
        public void Update(float elapsedTime)
        {
            loc = orbloc + orbrad * Vector2.Transform(Vector2.UnitX, Matrix.CreateRotationZ(MathHelper.TwoPi * elapsedTime / tPeriod + phase));
        }

        public void Draw(SpriteBatch sB, Texture2D[] textures, SpriteFont[] fonts)
        {
            sB.Draw(textures[0], loc, null, col, 0f, new Vector2(textures[0].Width / 2, textures[0].Height / 2), 2*rad/textures[0].Width, SpriteEffects.None, 0.5f);
        }

        public Vector2 GetLoc()  { return loc; }
        public float GetRadius() { return rad; }
        public float GetMass()   { return mass; }
    }
}
