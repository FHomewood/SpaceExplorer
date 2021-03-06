﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace Space_Explorer
{
    class Planet
    {
        private Vector2 loc, orbloc;
        private float rad, mass;
        private float[] tPeriod, orbrad, phase;
        private Color col;


        public Planet(Vector2 loc, float rad, float mass, Color col)
        {
            this.orbloc = loc;
            this.loc = loc;
            this.rad = rad;
            this.mass = mass;
            this.col = col;
            this.tPeriod = new float[] { 1 };
            this.orbrad = new float[] { 0 };
            this.phase = new float[] { 0 };
        }
        public Planet(Vector2 orbloc, float rad, float mass, Color col, float[] orbrad, float[] tPeriod, float[] phase)
        {
            this.orbloc = orbloc;
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
            loc = orbloc;
            for (int i = 0; i < orbrad.Length; i++)
                loc += orbrad[i] * Vector2.Transform(Vector2.UnitX, Matrix.CreateRotationZ(MathHelper.TwoPi * elapsedTime / tPeriod[i] + phase[i]));
        }

        public void Draw(SpriteBatch sB, Texture2D[] textures, SpriteFont[] fonts)
        {
            sB.Draw(textures[0], loc, null, col, 0f, new Vector2(textures[0].Width / 2, textures[0].Height / 2), 2*rad/textures[0].Width, SpriteEffects.None, 0.5f);
        }

        public Vector2 Loc
        {
            get { return loc; }
            set { loc = value; }
        }
        public Vector2 OrbLoc
        {
            get { return orbloc; }
            set { orbloc = value; }
        }
        public float Radius
        {
            get { return rad; }
            set { rad = value; }
        }
        public float Mass
        {
            get { return mass; }
            set { mass = value; }
        }
        public float[] TPeriod
        {
            get { return tPeriod; }
            set { tPeriod = value; }
        }
        public float[] Orbrad
        {
            get { return orbrad; }
            set { orbrad = value; }
        }
        public float[] Phase
        {
            get { return phase; }
            set { phase = value; }
        }
    }
}
