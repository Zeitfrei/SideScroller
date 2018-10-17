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
    public class Animator : Microsoft.Xna.Framework.DrawableGameComponent
    {
        enum Animation { PlayerExplosion = 0, PlayerShield, EnemyExplosion, PlayerPropulsion }
        Animation animation;

        Texture2D gr_playerexplosion;
        Texture2D gr_enemyexplosion;
        Texture2D gr_propulsion;

        private int height, width;
        private Vector2 position;

        private int spriteWidth, spriteHeight;
        private int columns, rows;
        private int currentColumn, currentRow;
        private double animationSpeed;
        private double currentTime;

        private bool enabled;
        private bool looping;
        private bool left;

        public Animator(Game game, int animation, Vector2 position)
        :base(game)
        {
            this.animation = (Animation) animation;
            this.position = position;

            enabled = false;
            left = false;

            switch (animation)
            {
                case 0:
                {
                    spriteWidth = 32;
                    spriteHeight = 32;
                    columns = 8;
                    rows = 1;
                    animationSpeed = 20;
                    looping = false;
                    break;
                }

                case 2:
                {
                    spriteWidth = 32;
                    spriteHeight = 32;
                    columns = 8;
                    rows = 1;
                    animationSpeed = 20;
                    looping = false;
                    break;
                }

                case 3:
                {
                    spriteWidth = 28;
                    spriteHeight = 13;
                    columns = 1;
                    rows = 32;
                    animationSpeed = 1;
                    looping = true;
                    break;
                }
            }

            currentColumn = 0;
            currentRow = 0;
            currentTime = 0;
        }

        public override void Initialize()
        {
            base.Initialize();
        }

        //Contet is loaded according to type of animation
        public void LoadContent(ContentManager Content)
        {

            switch (animation)
            {
                case Animation.PlayerExplosion:
                    {
                        gr_playerexplosion = Content.Load<Texture2D>("Textures\\expredsheet");
                        break;
                    }

                case Animation.PlayerShield:
                {
                        break;
                }

                case Animation.EnemyExplosion:
                {
                    gr_enemyexplosion = Content.Load<Texture2D>("Textures\\expbluesheet");
                    break;
                }

                case Animation.PlayerPropulsion:
                {
                    gr_propulsion = Content.Load<Texture2D>("Textures\\propulsion");
                    break;
                }
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            if (enabled)
            {
                switch (animation)
                {
                    case Animation.PlayerExplosion:
                    {
                        spriteBatch.Draw(gr_playerexplosion,
                        new Vector2((int)position.X, (int)position.Y + 25),
                        new Rectangle(currentColumn * spriteWidth, currentRow * spriteHeight, spriteWidth, spriteHeight),
                        Color.White, 0f, Vector2.Zero, new Vector2(3, 3), SpriteEffects.None, 0f);

                        break;
                    }

                    case Animation.PlayerShield:
                    {
                        break;
                    }

                    case Animation.EnemyExplosion:
                    {
                        spriteBatch.Draw(gr_enemyexplosion,
                        new Vector2((int)position.X-20, (int)position.Y-10),
                        new Rectangle(currentColumn * spriteWidth, currentRow * spriteHeight, spriteWidth, spriteHeight), 
                        Color.White , 0f, Vector2.Zero,new Vector2 (3,3), SpriteEffects.None, 0f);

                        break;
                    }
                    //drawing propulsion animation according to player direction
                    case Animation.PlayerPropulsion:
                    {
                        if (left)
                        {
                            spriteBatch.Draw(gr_propulsion,
                            new Vector2((int)position.X + 80, (int)position.Y + 66),
                            new Rectangle(currentColumn * spriteWidth, currentRow * spriteHeight, spriteWidth, spriteHeight),
                            Color.White, 0f, Vector2.Zero, new Vector2(1, 1), SpriteEffects.FlipHorizontally, 0f);

                            spriteBatch.Draw(gr_propulsion,
                            new Vector2((int)position.X + 80, (int)position.Y + 81),
                            new Rectangle(currentColumn * spriteWidth, currentRow * spriteHeight, spriteWidth, spriteHeight),
                            Color.White, 0f, Vector2.Zero, new Vector2(1, 1), SpriteEffects.FlipHorizontally, 0f);
                        }
                        else
                        {
                            spriteBatch.Draw(gr_propulsion,
                            new Vector2((int)position.X + 12, (int)position.Y + 66),
                            new Rectangle(currentColumn * spriteWidth, currentRow * spriteHeight, spriteWidth, spriteHeight),
                            Color.White, 0f, Vector2.Zero, new Vector2(1, 1), SpriteEffects.None, 0f);

                            spriteBatch.Draw(gr_propulsion,
                            new Vector2((int)position.X + 12, (int)position.Y + 81),
                            new Rectangle(currentColumn * spriteWidth, currentRow * spriteHeight, spriteWidth, spriteHeight),
                            Color.White, 0f, Vector2.Zero, new Vector2(1, 1), SpriteEffects.None, 0f);
                        }

                        break;
                    }
                }
            }
        }

        public override void Update(GameTime gameTime)
        {
            if (enabled)
            {
                currentTime += gameTime.ElapsedGameTime.TotalMilliseconds;
                if (currentTime > animationSpeed)
                {
                    currentTime -= animationSpeed;
                    currentColumn++;

                    // iterate rows
                    if (currentColumn >= columns)
                    {
                        currentRow++;
                        currentColumn = 0;

                        //to loop or not to loop
                        if (currentRow >= rows)
                        {
                            if (looping)
                                currentRow = 0;
                            else
                                enabled = false;
                        }

                    }
                }
            }
        }

        public void updatePosition(ref Vector2 position)
        {
            this.position = position;
        }

        public void animate()
        {
            enabled = true;
        }

        public bool running()
        {
            return enabled;
        }

        public void setPlayerSize(int height, int width)
        {
            this.height = height;
            this.width = width;
        }

        public void setPropDir(bool dir)
        {
            left = dir;
        }
    }
}
