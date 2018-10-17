using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

namespace Game1
{
    class PowerUpManager : Microsoft.Xna.Framework.DrawableGameComponent
    {
        int screenHeight, screenWidth;
        float time, spawnDelay, baseSpawnDelay, speed;
        public List<PowerUp> powerUps;
        Texture2D gr_weaponPU;
        SoundEffect au_weaponPU;
        Random rand;
        
        public PowerUpManager(Game game, int screenWidth, int screenHeight)
        :base(game)
        {
            this.screenWidth = screenWidth;
            this.screenHeight = screenHeight;
            time = 0;
            spawnDelay = 0;
            baseSpawnDelay = 12f;
            speed = 6f;
            powerUps = new List<PowerUp>();
            rand = new Random();
        }

        public override void Initialize()
        {
            base.Initialize();
        }

        public void LoadContent(ContentManager Content)
        {
            gr_weaponPU = Content.Load<Texture2D>("Textures\\shotGreen");
            au_weaponPU = Content.Load<SoundEffect>("Sounds\\FX068");
            base.LoadContent();
        }

        public override void Update(GameTime gameTime)
        {
            //spawn at random intervals
            time += (float)gameTime.ElapsedGameTime.TotalSeconds;
            spawnDelay = baseSpawnDelay + (float)rand.NextDouble();
            if (time >= spawnDelay)
            {
                //spawn power up of random type
                spawnPowerUp(rand.Next(4));
                time -= spawnDelay;
            }
            if(powerUps != null)
            {
                           //is powerup pickedup ? -> delete from list
                for (int i = 0; i < powerUps.Count; i++)
                {
                    powerUps[i].Update(gameTime);
                }
                powerUps.RemoveAll(powerUp => powerUp.pickedUp);
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            if (powerUps != null)
            {
                for (int i = 0; i < powerUps.Count; i++)
                {
                    powerUps[i].Draw(spriteBatch);
                }
            }
        }

        private void spawnPowerUp(int i)
        {
            switch(i)
            {
                case 0: spawnWeaponPU(); break;
                case 1: spawnWeaponPU(); break;
                case 2: spawnWeaponPU(); break;
                case 3: spawnWeaponPU(); break;
                case 4: spawnWeaponPU(); break;
                default: spawnWeaponPU(); break;
            }
        }

        private void spawnWeaponPU()
        {
            //spawn Power Up at random position
            Vector2 position = new Vector2(screenWidth+20,(int) ((screenHeight-50)*rand.NextDouble()));
            powerUps.Add(new PowerUp(this.Game,0,screenWidth,screenHeight,speed,gr_weaponPU,au_weaponPU,position));
        }

        private void spawnShield()
        {
        }
    }
}
