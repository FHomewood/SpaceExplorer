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
            itemDrops[0] = 0.6f;
            itemDrops[1] = 0.3f;
            itemDrops[2] = 0.05f;
            itemDrops[3] = 0.01f;
            itemDrops[4] = 0.01f;
            itemDrops[5] = 0.01f;
            itemDrops[6] = 0.01f;
            itemDrops[7] = 0.01f;
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
