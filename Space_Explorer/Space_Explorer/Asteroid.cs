using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Space_Explorer
{
    class Asteroid
    {
        private AsteroidBelt motherBelt;
        private Vector2 loc;
        private Color col;
        private float radius;
        private float[] itemDrops;

        public Asteroid(AsteroidBelt motherBelt, Random rand)
        {
            this.motherBelt = motherBelt;
            double u = rand.NextDouble() + rand.NextDouble();
            if (u>1)
                this.loc = motherBelt.loc + (motherBelt.inRad + (float)(2 - u) * (motherBelt.outRad - motherBelt.inRad)) * Vector2.Transform(Vector2.UnitY, Matrix.CreateRotationZ((float)(2 * rand.NextDouble() * Math.PI)));
            else this.loc = motherBelt.loc + (motherBelt.inRad + (float)(u) * (motherBelt.outRad - motherBelt.inRad)) * Vector2.Transform(Vector2.UnitY, Matrix.CreateRotationZ((float)(2 * rand.NextDouble() * Math.PI)));
            this.radius = 2f+10*(float)rand.NextDouble();
            col = Color.FromNonPremultiplied(rand.Next(50, 100), rand.Next(50, 70), rand.Next(50, 60), 255);
            
            itemDrops = new float[64];
            float percentRemainder = 1;
            for(int j = motherBelt.itemIDDrops.Length-1; j > 0; j--)
            {
                itemDrops[j] = motherBelt.percent[j] + 2f*((float)rand.NextDouble() - 0.5f) * motherBelt.uncertainty[j];
                percentRemainder -= itemDrops[j];
            }
            itemDrops[0] = percentRemainder;
            int i = 0;
            while ( i < motherBelt.Asteroids.Count())
            {
                if ((loc - motherBelt.Asteroids[i].loc).Length() < radius + motherBelt.Asteroids[i].Radius)
                {
                    u = rand.NextDouble() + rand.NextDouble();
                    if (u > 1)
                        this.loc = motherBelt.loc + (motherBelt.inRad + (float)(2 - u) * (motherBelt.outRad - motherBelt.inRad)) * Vector2.Transform(Vector2.UnitY, Matrix.CreateRotationZ((float)(2 * rand.NextDouble() * Math.PI)));
                    else this.loc = motherBelt.loc + (motherBelt.inRad + (float)(u) * (motherBelt.outRad - motherBelt.inRad)) * Vector2.Transform(Vector2.UnitY, Matrix.CreateRotationZ((float)(2 * rand.NextDouble() * Math.PI)));
                    i = -1;
                }
                i++;
            }
        }

        public void Update()
        {

        }

        public void Draw(SpriteBatch sB, Texture2D[] textures, SpriteFont[] fonts)
        {
            sB.Draw(textures[0], loc, null, col, 0f, textures[0].Bounds.Size.ToVector2() / 2, 2 * radius / textures[0].Width, SpriteEffects.None, 0f);
        }
        public Vector2 Loc
        {
            get { return loc; }
            set { loc = value; }
        }
        public float Radius
        {
            get { return radius; }
            set { radius = value; }
        }
        public float GetItem(int i)
        {
            return itemDrops[i];
        }
        public void SetItem(int i , float value)
        {
            itemDrops[i] = value;
        }
        public float[] ItemDrops
        {
            get { return itemDrops; }
            set { itemDrops = value; }
        }
    }
}
