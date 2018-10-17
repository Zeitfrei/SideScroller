using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Audio;

namespace Game1
{
    public class Enemy : Microsoft.Xna.Framework.DrawableGameComponent
    {
        Texture2D texture, shot1;
        SoundEffect au_gun, au_explosion;
        Vector2 position;
        List<Texture2D> gr_explosions;
        public List<Rectangle> gr_projectiles;
        public Rectangle enemyRect;
        public Rectangle enemyBounds;
        Rectangle weapon;

        Animator explosionAnim;

        int enemyWidth, enemyHeight, explosionCounter, height, width, explosionTime;
        float speed, shotSpeed, fireDelay, time, enemySize;
        public bool drawShot1, isAlive, exploding, exploded, onScreen;


        public Enemy(Game game, Texture2D enemyTexture, Texture2D shot1, SoundEffect au_gun, 
            SoundEffect au_explosion, Vector2 position, List<Texture2D> gr_explosions, int speed, 
            int shotSpeed, int path, float enemySize)
        :base(game)
        {
            this.texture = enemyTexture;
            this.shot1 = shot1;
            this.au_gun = au_gun;
            this.au_explosion = au_explosion;
            this.position = position;
            this.speed = speed;
            this.shotSpeed = shotSpeed;
            this.gr_explosions = gr_explosions;
            this.enemySize = enemySize;

            explosionCounter = 0;
            explosionTime = 0;
            gr_projectiles = new List<Rectangle>();
            drawShot1 = false;
            isAlive = true;
            exploding = false;
            exploded = false;
            onScreen = true;
            height = 0;
            width = 0;
            time = 0f;
            fireDelay = 1.3f;
            enemyHeight = texture.Height;
            enemyWidth = texture.Width;

            enemyRect = new Rectangle((int)position.X, (int)position.Y, enemyWidth/2, enemyHeight/2);
            weapon = new Rectangle((int)position.X + 10, (int)position.Y + 30, 40, 20);
            enemyBounds = new Rectangle((int)position.X+30, (int)position.Y+20,(enemyWidth / 2)-10,(enemyHeight/2)-30);

            explosionAnim = new Animator(this.Game, 2, position);
            explosionAnim.setPlayerSize(enemyHeight, enemyWidth);
        }

        public override void Initialize()
        {
            base.Initialize();
        }

        public void LoadContent(ContentManager Content)
        {
            explosionAnim.LoadContent(Content);
            base.LoadContent();
        }
        public void Draw(SpriteBatch spriteBatch)
        {
            //draws enemy
            if (isAlive && !exploding && !exploded)
            {
                spriteBatch.Draw(texture, enemyRect, Color.White);
                spriteBatch.Draw(shot1, weapon, Color.White);
                //"calibrate" bounding rectangle:
                //spriteBatch.Draw(shot1,enemyBounds,Color.White);
            }
            //draws projectile   
            if (!(gr_projectiles == null))
            {
                foreach (Rectangle r in gr_projectiles)
                {
                    spriteBatch.Draw(shot1, r, Color.White);
                }
            }

            if (exploding)
            {
                if (!explosionAnim.running())
                {
                    explosionAnim.updatePosition(ref position);
                    explosionAnim.animate();
                    au_explosion.Play();
                    enemyBounds = new Rectangle(0,2000,0,0);
                }
                explosionAnim.Draw(spriteBatch);
            }
        }

        public override void Update(GameTime gameTime)
        {
            time += (float)gameTime.ElapsedGameTime.TotalSeconds;

            updatePosition();
            handleShooting();

            if (exploding)
            {
                explosionAnim.Update(gameTime);
            }

            // enemy is exploding but not killed until enemy left screen
            if (exploding && !explosionAnim.running())
            {
                exploding = false;
                exploded = true;
                //isAlive = false;
            }

            //enemy is killed after leaving the screen
            if (position.X < -20 && isAlive)
            {
                onScreen = false;
                killEnemy();
            }

            enemyRect.X = (int)position.X;
            enemyBounds.X = (int)position.X;
            weapon.X = (int)position.X;
            base.Update(gameTime);
        }

        // new rectangle in Update will be added to a list
        public void shootStandard()
        {
            if (isAlive && !exploding && !exploded)
                drawShot1 = true;
        }
        //no explosion on leaving the screen
        public void killEnemy()
        {
            if (onScreen)
                exploding = true;
            else
                isAlive = false;
        }

        
        private void updatePosition()
        {
            //moves enemy in straight line from starting position;
            position.X = (int)position.X - speed;
            enemyRect.X = (int)position.X;
            enemyBounds.X = (int)position.X;

            //handles position of each projectile
            if (!(gr_projectiles == null))
            {
                for (int i = 0; i < gr_projectiles.Count; i++)
                {
                    gr_projectiles[i] = new Rectangle((int)gr_projectiles[i].X - (int)shotSpeed, (int)gr_projectiles[i].Y, 20, 20);
                }
            }
        }

        
        private void handleShooting()
        {
            //adds a Rectangle to a list, later representing the shot texture
            if (drawShot1)
            {
                drawShot1 = false;
                gr_projectiles.Add(new Rectangle((int)position.X, (int)position.Y + 30, 20, 20));
            }

            // shoots once, waits for fireDelay
            if (time >= fireDelay && isAlive)
            {
                au_gun.Play();
                shootStandard();
                time -= fireDelay;
            }
        }
        //GET/SET:

        //sets height/width to height, width of Game-Window
        public void setSize(int height, int width)
        {
            this.height = height;
            this.width = width;
        }
        
        public bool Intersects(Rectangle r) { return enemyBounds.Intersects(r); }
    }
}
