using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Space_Explorer
{
    class AsteroidBelt
    {
        public Vector2 loc;
        List<Asteroid> asteroidList = new List<Asteroid>();
        public float inRad, outRad;
        float area;

        public AsteroidBelt(Vector2 loc, float inRad, float outRad)
        {
            this.loc = loc;
            this.inRad = inRad;
            this.outRad = outRad;
            area = MathHelper.Pi * (outRad * outRad - inRad * inRad);
        }

        public void Update()
        {
            int count = asteroidList.Count();
            Random rand = new Random();
            while (area / 1000 > count)
            {
                asteroidList.Add(new Asteroid(this, rand));
                count++;
            }
            for (int i = 0; i < asteroidList.Count(); i++)
            {
                asteroidList[i].Update();
                if (asteroidList[i].Radius <= 0) asteroidList.Remove(asteroidList[i]);
            }

                   
        }

        public void UnderDraw(SpriteBatch sB, Texture2D[] textures, SpriteFont[] fonts)
        {
            sB.Draw(textures[0], loc, null, Color.FromNonPremultiplied(10,10,10,255), 0f, textures[0].Bounds.Size.ToVector2() / 2, 2 * outRad / textures[0].Width, SpriteEffects.None, 0f);
            sB.Draw(textures[0], loc, null, Color.Black, 0f, textures[0].Bounds.Size.ToVector2() / 2, 2 * inRad / textures[0].Width, SpriteEffects.None, 0f);
            foreach (Asteroid asteroid in asteroidList) asteroid.Draw(sB, textures, fonts);
        }
        public void OverDraw(SpriteBatch sB, Texture2D[] textures, SpriteFont[] fonts)
        {  }

        public List<Asteroid> Asteroids
        {
            get { return asteroidList; }
            set { asteroidList = value; }
        }
    }
}
