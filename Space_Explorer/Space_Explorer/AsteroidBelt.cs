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
            if (area / 100000 > asteroidList.Count())
                asteroidList.Add(new Asteroid(this, new Random()));
            
        }

        public void UnderDraw(SpriteBatch sB, Texture2D[] textures, SpriteFont[] fonts)
        {
            sB.Draw(textures[0], loc, null, Color.FromNonPremultiplied(10,10,10,255), 0f, textures[0].Bounds.Size.ToVector2() / 2, 2 * outRad / textures[0].Width, SpriteEffects.None, 0f);
            sB.Draw(textures[0], loc, null, Color.Black, 0f, textures[0].Bounds.Size.ToVector2() / 2, 2 * inRad / textures[0].Width, SpriteEffects.None, 0f);
            foreach (Asteroid asteroid in asteroidList) asteroid.Draw(sB, textures, fonts);
        }
        public void OverDraw(SpriteBatch sB, Texture2D[] textures, SpriteFont[] fonts)
        {
            sB.DrawString(fonts[0], area.ToString(), loc - fonts[0].MeasureString(area.ToString()) / 2 - outRad*Vector2.UnitY, Color.White);
        }
    }
}
