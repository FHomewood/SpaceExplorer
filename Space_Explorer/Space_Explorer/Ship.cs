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
        private bool landed;
        private float rotation, health = 100, Hittimer,
            camFocusZoom, camFocusRot, invScreen_loc,
            money = 0f, throttle, maxFuel = 1000, fuel,
            cargoCapacity = 1000;
        private int screenW, screenH, invScreen_target = 400;
        private Item[] Inventory = new Item[64];

        public Ship(Vector2 loc, int screenW, int screenH)
        {
            this.loc = loc;
            this.screenW = screenW;
            this.screenH = screenH;
            this.closestBody = new Planet(5000 * Vector2.One, 1, 0, Color.Black);
            for (int i = 0; i < 64; i++)
                Inventory[i] = new Item(i);
            fuel = maxFuel;
        }

        public void Update(Camera cam, KeyboardState oldK, KeyboardState newK, MouseState oldM, MouseState newM, List<Particle> particleList, float elapsedTime)
        {
            Random rand = new Random();
            if (newK.IsKeyDown(Keys.W)  && fuel > 0
                )
            {
                if (landed)
                {
                    landed = false;
                    vel = Vector2.Zero;
                    for (int i = 0; i < closestBody.Orbrad.Length; i++)
                        vel += closestBody.Orbrad[i] *MathHelper.TwoPi/closestBody.TPeriod[i]/60* Vector2.Transform(Vector2.UnitY, Matrix.CreateRotationZ(MathHelper.TwoPi * elapsedTime / closestBody.TPeriod[i] + closestBody.Phase[i]));
                }
                vel += 0.02f * throttle * Vector2.Transform(-Vector2.UnitY, Matrix.CreateRotationZ(rotation));
                particleList.Add(
                    new Particle(
                        loc - 6/cam.Zoom * Vector2.Transform(-Vector2.UnitY, Matrix.CreateRotationZ(rotation)),
                        vel + Vector2.Transform((float)rand.NextDouble() * Vector2.UnitX, Matrix.CreateRotationZ((float)rand.NextDouble() * MathHelper.TwoPi)) / 4/cam.Zoom,
                        Color.FromNonPremultiplied(rand.Next(200, 255), rand.Next(100, 200), rand.Next(0, 100), 255),
                        50,
                        3/cam.Zoom,
                        0f));
                fuel -= throttle * 0.1f;
            }
            if (newK.IsKeyDown(Keys.S))
            {
                vel -= throttle * 0.005f * Vector2.Transform(-Vector2.UnitY, Matrix.CreateRotationZ(rotation));
                fuel -= throttle * 0.05f;
            }
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
                            float cargo = 0;
                            foreach (Item item in Inventory) cargo += item.amount;
                            if (cargoCapacity > cargo)
                                for (int i = 0; i < Inventory.Length; i++)
                                    if (cargoCapacity > cargo)
                                        Inventory[i].amount += 100 * asteroid.ItemDrops[i] * (float)(Math.Pow(asteroid.Radius + 0.03f, 2) - Math.Pow(asteroid.Radius, 2)) / 1000f;
                        }
                    }
                }
            if (newK.IsKeyDown(Keys.LeftShift) && throttle < 1f)   throttle += 0.01f;
            if (newK.IsKeyDown(Keys.LeftControl) && throttle > 0f) throttle -= 0.01f;
            if (newK.IsKeyDown(Keys.Tab) && oldK.IsKeyUp(Keys.Tab))
                if (invScreen_target == 0) invScreen_target = 400;
                else invScreen_target = 0;
            invScreen_loc += (invScreen_target - invScreen_loc) / 5;
            loc += vel;
            //if (landed)
            //{
            //    loc = closestBody.Radius * -Vector2.UnitY;
            //    for (int i = 0; i < closestBody.Orbrad.Length; i++)
            //        loc += closestBody.Orbrad[i] * Vector2.Transform(Vector2.UnitY, Matrix.CreateRotationZ(MathHelper.TwoPi * elapsedTime / closestBody.TPeriod[i] + closestBody.Phase[i]));
            //}

            //Order Priorities of camera focus
            if (landed) camFocusLoc = closestBody.Loc;
            else camFocusLoc = loc;

            //Order priorities of Zoom and Focus
            if (landed)
            {
                camFocusZoom = screenH / 4 / (closestBody.Loc - loc).Length();
                camFocusRot = -MathHelper.PiOver2 - (float)Math.Atan2((loc - closestBody.Loc).Y, (loc - closestBody.Loc).X);
            }
            else if (currentBelt != null)
            {
                camFocusZoom = screenH / 250f;
                camFocusRot = -rotation;
            }
            else
            {
                camFocusZoom = screenH / 2 / (closestBody.Loc - loc).Length();
                camFocusRot = -MathHelper.PiOver2 - (float)Math.Atan2((loc - closestBody.Loc).Y, (loc - closestBody.Loc).X);
            }

            //apply smoother transitions.
            cam.X += (camFocusLoc.X - cam.X) / 20;
            cam.Y += (camFocusLoc.Y - cam.Y) / 20;
            cam.Zoom += (camFocusZoom - cam.Zoom) / 20;
            float RotDifference = ((camFocusRot - cam.Rotation) % MathHelper.TwoPi);
            cam.Rotation += RotDifference / 20;

            //Update death checks and hit graphics
            if (health < 0) { health = 0; }
            if (Hittimer > 0) { Hittimer--; }
        }

        public void PlanetInteraction(Camera cam, float frametime, Planet planet, float elapsedTime)
        {
            Vector2 difference = (planet.Loc - loc);
            if (difference.Length() < (closestBody.Loc - loc).Length()) closestBody = planet;
            vel += difference * planet.Mass * (float)Math.Pow(difference.Length(), -3);
            if (difference.Length() < planet.Radius + 10f/cam.Zoom)
            {
                Vector2 parallel      = difference / difference.Length();
                Vector2 perpendicular = Vector2.Transform(parallel, Matrix.CreateRotationZ(-MathHelper.PiOver2));
                vel = perpendicular * Vector2.Dot(perpendicular, vel) - parallel * Vector2.Dot(parallel, vel);
                for (int i = 0; i < closestBody.Orbrad.Length; i++)
                    vel += planet.Orbrad[i] * MathHelper.TwoPi / planet.TPeriod[i] / 60 * Vector2.Transform(Vector2.UnitY, Matrix.CreateRotationZ(MathHelper.TwoPi * elapsedTime / planet.TPeriod[i] + planet.Phase[i]));

                if (vel.Length() > 0.5f) { health -= vel.Length() * 10; Hittimer = 100; }
                if (vel.Length() < 0.1f) { vel = Vector2.Zero; landed = true; }
                vel /= 1.5f;
                loc += (difference.Length() - planet.Radius - 10f / cam.Zoom) * difference / difference.Length();
            }
        }

        public void BeltInteraction(AsteroidBelt belt)
        {
            if ((loc - belt.loc).Length() > belt.inRad && (loc - belt.loc).Length() < belt.outRad)
            {
                currentBelt = belt;
            }
        }

        public void CamDraw(Camera cam, KeyboardState newK, SpriteBatch sB, Texture2D[] textures, SpriteFont[] fonts)
        {
            if (newK.IsKeyDown(Keys.Space) && currentBelt != null)
                sB.Draw(textures[1], loc, null, Color.FromNonPremultiplied(0,new Random().Next(50,150),new Random().Next(150,200), 100),
                rotation, new Vector2(textures[1].Width / 2, textures[1].Height), 0.5f / cam.Zoom, SpriteEffects.None, 0f);
            sB.Draw(textures[0], loc, null, Color.White, rotation, new Vector2(textures[0].Width / 2, textures[0].Height / 2), 0.05f/cam.Zoom, SpriteEffects.None, 0f);
            sB.DrawString(fonts[0], Math.Floor(health).ToString(), loc - Vector2.Transform(15/cam.Zoom * Vector2.UnitY, Matrix.CreateRotationZ(-cam.Rotation)), Color.Red, -cam.Rotation, fonts[0].MeasureString(Math.Floor(health).ToString())/2, 20f/ fonts[0].MeasureString(Math.Floor(health).ToString()).Y/ cam.Zoom,SpriteEffects.None,1f);
        }
        public void StaticDraw(GraphicsDeviceManager graphics, SpriteBatch sB, Texture2D[] textures, SpriteFont[] fonts)
        {
            //Damage screen
            sB.Draw(textures[0], new Rectangle(0, 0, graphics.PreferredBackBufferWidth, graphics.PreferredBackBufferHeight), null, Color.FromNonPremultiplied(255, 0, 0, (int)(100 * (Hittimer / 100))), 0f, Vector2.Zero, SpriteEffects.None, 0.5f);
            //Money
            sB.DrawString(fonts[0], Math.Round(money, 2).ToString(), Vector2.UnitY * screenH + Vector2.UnitX * screenW, Color.Green, 0f, fonts[0].MeasureString(Math.Round(money, 2).ToString()), 1f, SpriteEffects.None, 0f);
            //Currency Symbol
            sB.Draw(textures[1], new Rectangle(new Vector2(screenW - fonts[0].MeasureString(Math.Round(money, 2).ToString()).X, screenH).ToPoint(), new Vector2(1+fonts[0].MeasureString(Math.Round(money, 2).ToString()).Y * textures[1].Width/textures[1].Height, fonts[0].MeasureString(Math.Round(money, 2).ToString()).Y).ToPoint()), null, Color.Green, 0f, textures[1].Bounds.Size.ToVector2(), SpriteEffects.None, 0f);
            //Throttle Bar
            sB.Draw(textures[3], new Vector2(25, screenH - 25), null, Color.White, 0f, new Vector2(0, 100), 1f, SpriteEffects.None, 0f);
            //Throttle Indicator
            sB.Draw(textures[2], new Rectangle(25, screenH - 27 - (int)(96 * throttle), 11, 2), Color.White);
            //Fuel Bar
            sB.Draw(textures[5], new Vector2(61, screenH - 25 - fuel/maxFuel*100),new Rectangle(0,0, 11,(int)(fuel/maxFuel *100)), Color.White, 0f, new Vector2(0, 0), 1f, SpriteEffects.None, 0f);
            //Fuel Tank
            sB.Draw(textures[4], new Vector2(61, screenH - 25), null, Color.White, 0f, new Vector2(0, 100), 1f, SpriteEffects.None, 0f);





            List<Item> DrawInv = new List<Item>();
            for (int i = 0; i < Inventory.Length; i++)
                if (Inventory[i].amount != 0)
                    DrawInv.Add(Inventory[i]);

            //This draws each Inventory Item in the HUD
            foreach (Item item in DrawInv)
            {
                //BoxLoc defines where the box is, it incorporates: 
                    //whether the inventory screen is selected or not,
                    //the seperation of each of the item points,
                    //scrolling due to cursor placement.
                Vector2 Boxloc = new Vector2(screenW + invScreen_loc, (1+item.id) * 52 - (DrawInv.Count()+1) * 52 * Mouse.GetState().Y / (float)screenH + Mouse.GetState().Y);
                Boxloc.X -= 300f/(float)Math.Cosh(0.01f*(Boxloc.Y  - Mouse.GetState().Y));
                //Item box is drawn at boxloc and given an opacity related to how close the cursor is to the item
                sB.Draw(textures[2], new Rectangle((int)Boxloc.X, (int)Boxloc.Y, 300, 50), null, Color.FromNonPremultiplied(255, 255, 255, (int)(128 / (float)Math.Cosh(0.02f * (Boxloc.Y - Mouse.GetState().Y)))),
                    0f, Vector2.UnitY, SpriteEffects.None, 0f);
                sB.DrawString(fonts[1],
                    item.name, new Vector2(Boxloc.X + 15, Boxloc.Y - 20),
                    Color.Black, 0f, Vector2.Zero,
                    18f / fonts[1].MeasureString(item.name).Y,
                    SpriteEffects.None, 0f);
                sB.DrawString(fonts[1],
                    item.desc, new Vector2(Boxloc.X + 15, Boxloc.Y),
                    Color.Black, 0f, Vector2.Zero,
                    14f / fonts[1].MeasureString(item.desc).Y,
                    SpriteEffects.None, 0f);
                sB.DrawString(fonts[1],
                    Math.Round(item.amount * item.mass, 2).ToString() + "kg", new Vector2(Boxloc.X + 280, Boxloc.Y),
                    Color.Black, 0f, new Vector2(fonts[1].MeasureString(Math.Round(item.amount * item.mass, 2).ToString() + "kg").X, 0),
                    14f / fonts[1].MeasureString(Math.Round(item.amount * item.mass, 2).ToString() + "kg").Y,
                    SpriteEffects.None, 0f);
            }
            currentBelt = null;
        }
    }
}