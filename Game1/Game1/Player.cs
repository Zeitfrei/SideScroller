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
    public class Player : Microsoft.Xna.Framework.DrawableGameComponent
    {
        enum Orientation {Left = 0, Right }
        Texture2D gr_player, gr_projectile1;
        Texture2D gr_weapon;
        List<Texture2D> gr_explosions;
        SoundEffect au_projectile1, au_explosion;
        public long score;
        int weaponState;

        public Vector2 position;

        public List<Rectangle> gr_projectiles_right, gr_projectiles_left;
        Rectangle playerRect;
        Rectangle playerBounds;
        Rectangle weapon1;
        Rectangle weapon2;
        Rectangle weapon3;

        public KeyboardState kbState;

        int fireDelay = 180;
        int height, width, playerHeight, playerWidth, scrollSpeed, explosionCounter;
        float time = 0f;
        float speed, baseSpeed, acc, accLimit, shotSpeed1;
        public bool drawShot1, isAlive, exploding;
        int wOffset1, wOffset2, wOffset3;

        Animator explosionAnim;
        Animator propulsionAnim;

        Orientation orientation;
        

        public Player(Game game, int scrollSpeed)
        :base(game)
        {
            gr_explosions = new List<Texture2D>();
            explosionCounter = 0;
            this.scrollSpeed = scrollSpeed;
            position = new Vector2(250,240);
           
            speed = 8;
            baseSpeed = speed;
            height = 0;
            width = 0;
            acc = 0.1f;  // lets the ship accelerate while pressing D, reset when pressing A
            accLimit = 14f;
            shotSpeed1 = 15;
            weaponState = 0;

            gr_projectiles_right = new List<Rectangle>();
            gr_projectiles_left = new List<Rectangle>();
            drawShot1 = false;
            isAlive = true;
            exploding = false;

            explosionAnim = new Animator(this.Game, 0, position);
            propulsionAnim = new Animator(this.Game, 3, position);

            orientation = Orientation.Right;
        }

        public override void Initialize()
        {
            base.Initialize();
        }

        public void LoadContent(ContentManager Content)
        {
            for (int i = 1; i <= 6; i++ )
            {
                String temp = "Textures\\exp" + i;
                gr_explosions.Add(Content.Load<Texture2D>(temp));
            }
            gr_player = Content.Load<Texture2D>("Textures\\player");
            gr_projectile1 = Content.Load<Texture2D>("Textures\\shotGreen");
            gr_weapon = Content.Load<Texture2D>("Textures\\shotGreen");
            playerHeight = gr_player.Height;
            playerWidth = gr_player.Width;
            au_projectile1 = Game.Content.Load<SoundEffect>("Sounds\\laser7");
            au_explosion = Game.Content.Load<SoundEffect>("Sounds\\boom9");
            base.LoadContent();

            playerRect = new Rectangle((int)position.X, (int)position.Y, playerWidth, playerHeight);
            playerBounds = new Rectangle((int)position.X+30, (int)position.Y+35, playerWidth-70,playerHeight-75);

            wOffset1 = 50;
            wOffset2 = playerHeight / 2 - 5;
            wOffset3 = playerHeight-60;

            weapon1 = new Rectangle((int)position.X+50, (int)position.Y + wOffset1, 40, 10);
            weapon2 = new Rectangle((int)position.X+50, (int)position.Y + wOffset1, 40, 10);
            weapon3 = new Rectangle((int)position.X+50, (int)position.Y + wOffset1, 40, 10);

            explosionAnim.LoadContent(Content);
            propulsionAnim.LoadContent(Content);
            explosionAnim.setPlayerSize(playerHeight, playerWidth);
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            //wont be in top left corner without changing the origin
            if (isAlive && !exploding)
            {
                // draw textures according to orientation
                if (orientation == Orientation.Right)
                {
                    propulsionAnim.animate();
                    propulsionAnim.Draw(spriteBatch);
                    spriteBatch.Draw(gr_player, playerRect, Color.White);
                    switch (weaponState)
                    {
                        case 0: spriteBatch.Draw(gr_weapon, weapon2, Color.White); break;
                        case 1: spriteBatch.Draw(gr_weapon, weapon1, Color.White); spriteBatch.Draw(gr_weapon, weapon3, Color.White); break;
                        case 2: spriteBatch.Draw(gr_weapon, weapon1, Color.White); spriteBatch.Draw(gr_weapon, weapon2, Color.White);
                            spriteBatch.Draw(gr_weapon, weapon3, Color.White); break;
                        default: spriteBatch.Draw(gr_weapon, weapon1, Color.White); break;
                    }
                    //"calibrate" bounding rectangle:
                    //spriteBatch.Draw(gr_projectile1, playerBounds, Color.White);
                }
                else
                {
                    propulsionAnim.animate();
                    propulsionAnim.Draw(spriteBatch);
                    spriteBatch.Draw(gr_player, playerRect, null, Color.White, 0f, Vector2.Zero, SpriteEffects.FlipHorizontally, 1f);
                    switch (weaponState)
                    {
                        case 0: spriteBatch.Draw(gr_weapon, weapon2, Color.White); break;
                        case 1: spriteBatch.Draw(gr_weapon, weapon1, Color.White); spriteBatch.Draw(gr_weapon, weapon3, Color.White); break;
                        case 2: spriteBatch.Draw(gr_weapon, weapon1, Color.White); spriteBatch.Draw(gr_weapon, weapon2, Color.White);
                            spriteBatch.Draw(gr_weapon, weapon3, Color.White); break;
                        default: spriteBatch.Draw(gr_weapon, weapon1, Color.White); break;
                    }
                }

            }
            //draws projectiles flying right
            if (!(gr_projectiles_right == null))
            {
                foreach (Rectangle r in gr_projectiles_right)
                {
                    spriteBatch.Draw(gr_projectile1, r, Color.White);
                }
            }
            //draws projectiles flying left
            if (!(gr_projectiles_left == null))
            {
                foreach (Rectangle r in gr_projectiles_left)
                {
                    spriteBatch.Draw(gr_projectile1, r, Color.White);
                }
            }


            // draws all explosion textures once
            if (exploding)
            {
                if (!explosionAnim.running())
                {
                    explosionAnim.updatePosition(ref position);
                    explosionAnim.animate();
                    au_explosion.Play();
                }
                explosionAnim.Draw(spriteBatch);
            }
        }

        public override void Update(GameTime gameTime)
        {
            kbState = Keyboard.GetState();

            if (time > 0)
                time -= (int)gameTime.ElapsedGameTime.TotalMilliseconds;
            else if (time <= 0)
                time = 0;

            updatePosition(kbState);
            handleShooting(kbState);

            if(exploding)
            {
                explosionAnim.Update(gameTime);
            }

            if (exploding && !explosionAnim.running())
            {
                exploding = false;
                isAlive = false;
            }

            propulsionAnim.Update(gameTime);
            propulsionAnim.updatePosition(ref position);
            //set orientation for propulsion animation
            if(orientation == Orientation.Left)
                propulsionAnim.setPropDir(true);
            else
                propulsionAnim.setPropDir(false);

            base.Update(gameTime);
        }

        private void updatePosition(KeyboardState kbState)
        {
            //moves ship back at scrollspeed of game
            position.X -= scrollSpeed;

            //updates according to input and keeps player within the bounds
            if (kbState.IsKeyDown(Keys.W) && position.Y >= -playerHeight*0.25d)
                position.Y = (int)position.Y - speed;

            if (kbState.IsKeyDown(Keys.A))
            {
                position.X = position.X - speed;
                speed = baseSpeed;
                orientation = Orientation.Left;
            }

            if (kbState.IsKeyDown(Keys.S) && (position.Y <= height - playerHeight*0.75d))
                position.Y = position.Y + speed;

            if (kbState.IsKeyDown(Keys.D) && (position.X < width - playerWidth / 2))
            {
                position.X = position.X + speed;
                if (speed <= accLimit)
                    speed += acc;
                orientation = Orientation.Right;
            }

            // updating the projectiles
            if (!(gr_projectiles_right == null))
            {
                for (int i = 0; i < gr_projectiles_right.Count; i++)
                {
                    if (gr_projectiles_right[i].X <= width)
                        gr_projectiles_right[i] = new Rectangle((int)gr_projectiles_right[i].X + (int)shotSpeed1, (int)gr_projectiles_right[i].Y, 20, 20);
                    else
                        gr_projectiles_right.RemoveAt(i);
                }
            }
            if (!(gr_projectiles_left == null))
            {
                for (int i = 0; i < gr_projectiles_left.Count; i++)
                {
                    if (gr_projectiles_left[i].X <= width)
                        gr_projectiles_left[i] = new Rectangle((int)gr_projectiles_left[i].X - (int)shotSpeed1*2 + 7, (int)gr_projectiles_left[i].Y, 20, 20);
                    else
                        gr_projectiles_left.RemoveAt(i);
                }
            }

            playerBounds.X = (int)position.X + 30;
            playerBounds.Y = (int)position.Y + 35;
            playerRect.X = (int)position.X;
            playerRect.Y = (int)position.Y;

            weapon1.X = (int)position.X + 35;
            weapon1.Y = (int)position.Y + wOffset1;

            weapon2.X = (int)position.X + 45;
            weapon2.Y = (int)position.Y + wOffset2;

            weapon3.X = (int)position.X + 35;
            weapon3.Y = (int)position.Y + wOffset3;
        }

        private void handleShooting(KeyboardState kbState)
        {
            if (kbState.IsKeyDown(Keys.Space) && time == 0 && isAlive)
            {
                shootStandard();
                time = au_projectile1.Duration.Milliseconds + fireDelay;
            }

            //adds a Rectangle to a list, later representing the shot texture
            if (drawShot1)
            {
                drawShot1 = false;
                if(orientation == Orientation.Right)
                    gr_projectiles_right.Add(new Rectangle((int)position.X, (int)position.Y + 70, 20, 20));

                if (orientation == Orientation.Left)
                    gr_projectiles_left.Add(new Rectangle((int)position.X, (int)position.Y + 70, 20, 20)); 
            }
        }

        //sets height/width to height, width of Game-Window
        public void setSize(int height, int width)
        {
            this.height = height;
            this.width = width;
        }
        // new rectangle in Update will be added to a list
        public void shootStandard()
        {
            if (isAlive)
            {
                if(orientation == Orientation.Right)
                {
                    switch (weaponState)
                    {
                        case 0: gr_projectiles_right.Add(new Rectangle((int)weapon2.X, (int)weapon2.Y, 20, 20));
                            au_projectile1.Play();
                            break;
                        case 1: gr_projectiles_right.Add(new Rectangle((int)weapon1.X, (int)weapon1.Y, 20, 20));
                            gr_projectiles_right.Add(new Rectangle((int)weapon3.X, (int)weapon3.Y, 20, 20));
                            au_projectile1.Play();
                            au_projectile1.Play();
                            break;
                        case 2: gr_projectiles_right.Add(new Rectangle((int)weapon1.X, (int)weapon1.Y, 20, 20));
                            gr_projectiles_right.Add(new Rectangle((int)weapon2.X, (int)weapon2.Y, 20, 20));
                            gr_projectiles_right.Add(new Rectangle((int)weapon3.X, (int)weapon3.Y, 20, 20));
                            au_projectile1.Play();
                            au_projectile1.Play();
                            au_projectile1.Play();
                            break;
                        default: gr_projectiles_right.Add(new Rectangle((int)weapon2.X, (int)weapon2.Y, 20, 20));
                            au_projectile1.Play();
                            break;
                    }
                }
                else if(orientation == Orientation.Left)
                {
                    switch (weaponState)
                    {
                        case 0: gr_projectiles_left.Add(new Rectangle((int)weapon2.X, (int)weapon2.Y, 20, 20));
                            au_projectile1.Play();
                            break;
                        case 1: gr_projectiles_left.Add(new Rectangle((int)weapon1.X, (int)weapon1.Y, 20, 20));
                            gr_projectiles_left.Add(new Rectangle((int)weapon3.X, (int)weapon3.Y, 20, 20));
                            au_projectile1.Play();
                            au_projectile1.Play();
                            break;
                        case 2: gr_projectiles_left.Add(new Rectangle((int)weapon1.X, (int)weapon1.Y, 20, 20));
                            gr_projectiles_left.Add(new Rectangle((int)weapon2.X, (int)weapon2.Y, 20, 20));
                            gr_projectiles_left.Add(new Rectangle((int)weapon3.X, (int)weapon3.Y, 20, 20));
                            au_projectile1.Play();
                            au_projectile1.Play();
                            au_projectile1.Play();
                            break;
                        default: gr_projectiles_left.Add(new Rectangle((int)weapon2.X, (int)weapon2.Y, 20, 20));
                            au_projectile1.Play();
                            break;
                    }
                }
            }
        }

        public void killPlayer()
        {
            exploding = true;
        }

        // i indicates the type of power up picked up
        public void addPowerUp(int i)
        {
            switch (i)
            {
                default:
                {
                    addWeapon(); 
                    break;
                }
            }
        }

        private void addWeapon()
        {
            if (weaponState < 2)
                weaponState++;
            else
                score += 5000;
        }
 
        public bool Intersects(Rectangle other)
        {
            //return playerBounds.Intersects(other);
            return playerBounds.Intersects(other);
        }


    }
}

/* ToDo
 * 
 * int enemypath : 0: straight, 1; parable, 2: diagonal  - random? alternating? - path handled in enemy class
 * other enemies: bigger, different beams, faster, slower
 * fired projectiles by enemies that are killed shouldnt be deleted
 * hitpoints?
 * Power-Ups:   int variable: different modes in update and draw of player   PiercingShot, Double Shot, Increased Speed
 *              special power ups will last for X seconds
 *              shield, shooting faster, penetrating shot
 *              position of weapon on player?
 *              
*/
