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
    public class EnemyManager : Microsoft.Xna.Framework.DrawableGameComponent
    {
        Texture2D gr_enemyBlue, gr_enemyGreen, gr_projectile1;
        public List<Enemy> enemies;
        List<Texture2D> gr_explosions;
        Vector2 topPos, middlePos, bottomPos;
        SoundEffect au_projectile1, au_explosion;
        Random rand;

        ContentManager content;

        int enemyPos, enemyPath, screenHeight, screenWidth, enemyCounter;
        int standardSpeed, standardGunSpeed, enemyCount;
        float standardEnemySize, time, spawnDelay;
        float baseSpawnDelay = 0.5f;
        int enemyLimit = 10;
        public bool drawShot1, isAlive, exploding;
        
        public EnemyManager(Game game, int screenWidth, int screenHeight)
        :base(game)
        {
            this.screenWidth = screenWidth;
            this.screenHeight = screenHeight;

            time = 0;
            enemyCounter = 0;
            standardSpeed = 8;
            standardGunSpeed = 15;
            enemyPos = 0;
            enemyPath = 0;
            enemyCount = 0;

            enemies = new List<Enemy>();
            gr_explosions = new List<Texture2D>();

            topPos = new Vector2 (screenWidth+100,80); 
            middlePos = new Vector2 (screenWidth+100,screenHeight/2);
            bottomPos = new Vector2 (screenWidth+100,screenHeight-80);

            standardEnemySize = 0.5f;
            rand = new Random();
        }

        public override void Initialize()
        {
            base.Initialize();
        }

        public void LoadContent(ContentManager Content)
        {
            gr_projectile1 = Content.Load<Texture2D>("Textures\\shot0bigblue");
            au_projectile1 = Game.Content.Load<SoundEffect>("Sounds\\laser2");
            au_explosion = Game.Content.Load<SoundEffect>("Sounds\\boom2");
            gr_enemyBlue = Content.Load<Texture2D>("Textures\\enemy_blue");
            gr_enemyGreen = Content.Load<Texture2D>("Textures\\enemy_green");

            //loads explosion textures into list
            for (int i = 1; i <= 6; i++)
            {
                String temp = "Textures\\exp" + i;
                gr_explosions.Add(Content.Load<Texture2D>(temp));
            }
            this.content = Content;
            base.LoadContent();
        }

        public override void Update(GameTime gameTime)
        {
            time += (float) gameTime.ElapsedGameTime.TotalSeconds;

            // spawns an enemy after some delay until limit is reached, resets the time
            // zufällige häufigkeit = zufälliges delay
            spawnDelay = baseSpawnDelay + (float)rand.NextDouble();
            if (time >= spawnDelay && enemyCount < enemyLimit)
            {
                //loadStandardEnemy(1);
                loadEnemyAlternatingPosRandom();
                enemyCounter++;
                time -= spawnDelay;
            }

            if (!(enemies == null))
            {
                foreach (Enemy e in enemies)
                    e.Update(gameTime);
                enemies.RemoveAll(enemy => !enemy.isAlive);
                enemyCount = enemies.Count();
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            
            if (!(enemies == null))
            {
                foreach (Enemy e in enemies)
                {
                    e.Draw(spriteBatch);
                }
            }

        }

        private void loadEnemyAlternatingPos()
        {
            if (enemyPos > 2)
                enemyPos = 0;
            loadStandardEnemy(enemyPos);
            enemyPos++;
        }
        private void loadEnemyAlternatingPosRandom()
        {
            enemyPos = (int)rand.Next(0, 3);
            loadStandardEnemy(enemyPos);
        }

        // 0=top , 1=middle , 2=bottom
        private void loadStandardEnemy(int enemyPos)
        {
            if(enemyPos == 0)
            {
                Enemy newEn = new Enemy(this.Game, gr_enemyBlue, gr_projectile1, au_projectile1, au_explosion, topPos,gr_explosions, standardSpeed,
                    standardGunSpeed, enemyPath, standardEnemySize);
                newEn.LoadContent(content);
                enemies.Add(newEn);
            }
            else if (enemyPos == 1)
            {
                Enemy newEn = new Enemy
                    (this.Game, gr_enemyBlue, gr_projectile1, au_projectile1, au_explosion, middlePos, gr_explosions, standardSpeed,
                    standardGunSpeed, enemyPath, standardEnemySize);
                newEn.LoadContent(content);
                enemies.Add(newEn);
            }
            else if (enemyPos == 2)
            {
                Enemy newEn = new Enemy
                    (this.Game, gr_enemyBlue, gr_projectile1, au_projectile1, au_explosion, bottomPos,gr_explosions, standardSpeed,
                    standardGunSpeed, enemyPath, standardEnemySize);
                newEn.LoadContent(content);
                enemies.Add(newEn);
            }
             /*   enemies.Add(new Enemy
                    (this.Game, gr_enemyBlue, gr_projectile1, au_projectile1, au_explosion, topPos,gr_explosions, standardSpeed,
                    standardGunSpeed, enemyPath, standardEnemySize));
            else if (enemyPos == 1)
            {
                enemies.Add(new Enemy
                    (this.Game, gr_enemyBlue, gr_projectile1, au_projectile1, au_explosion, middlePos, gr_explosions, standardSpeed,
                    standardGunSpeed, enemyPath, standardEnemySize));
            }
            else if (enemyPos == 2)
                enemies.Add(new Enemy
                    (this.Game, gr_enemyBlue, gr_projectile1, au_projectile1, au_explosion, bottomPos,gr_explosions, standardSpeed,
                    standardGunSpeed, enemyPath, standardEnemySize));*/
        }
        
    }
}


