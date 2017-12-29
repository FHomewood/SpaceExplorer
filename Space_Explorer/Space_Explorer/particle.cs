using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Space_Explorer
{
    class Particle
    {
        private Vector2 loc, vel;
        private Color col;
        private float lifespan , life, radius;
        private float renderDepth;

        public Particle(Vector2 loc, Vector2 vel, Color col, float lifespan, float radius, float renderDepth)
        {
            this.loc = loc;
            this.vel = vel;
            this.col = col;
            this.lifespan = lifespan;
            this.life = lifespan;
            this.radius = radius;
            this.renderDepth = renderDepth;
        }

        public void Update()
        {
            loc += vel;
            life--;
        }

        public void Draw(SpriteBatch sB, Texture2D[] textures, SpriteFont[] fonts)
        {
            sB.Draw(textures[0], loc, null, Color.FromNonPremultiplied(col.R, col.G, col.B, (int)(255*life / lifespan)), 0f, textures[0].Bounds.Size.ToVector2() / 2,2*radius/textures[0].Width, SpriteEffects.None, renderDepth);
        }

    }
}
