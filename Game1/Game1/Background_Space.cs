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
    class Background_Space : Microsoft.Xna.Framework.DrawableGameComponent 
    {
            private SpriteBatch spritebatch;
            private Vector2 bgPosition1, bgPosition2;
            private Vector2 bgSize;
      
            private Texture2D gr_ship_en1;
            private Texture2D gr_ship_en2;
            private Texture2D gr_ship_al1;
            private Texture2D gr_ship_al2;

            private Texture2D gr_background;
            private Texture2D gr_star;

            private int bgScrollSpeed;
            private int width,height;

            private int speed_bgships1;
            private int speed_bgships2;

            private Rectangle ship_en1_1;
            private Rectangle ship_en1_2;
            private Rectangle ship_en1_3;
            private Rectangle ship_en1_4;
            private Rectangle ship_en1_5;

            private Rectangle ship_en2_1;
            private Rectangle ship_en2_2;
            private Rectangle ship_en2_3;

            private Random r;

            #region get and set-Methods for the background positions
            public Vector2 Position1
            {
                get { return bgPosition1; }

                set { bgPosition1 = value; }

            }

            public Vector2 Position2
            {
                get { return bgPosition2; }

                set { bgPosition2 = value; }
            }
            #endregion

            public Background_Space(Game1 game)

                : base(game)
            {
                r = new Random();

                height = 640;                  
                width = 1424;
                bgPosition1 = new Vector2(width, 0);
                bgPosition2 = new Vector2(0, 0);
                
                speed_bgships1 = 5;
                speed_bgships2 = 2;
                bgScrollSpeed = 1;

                ship_en1_1 = new Rectangle(width + 950,r.Next(height)-80,60,70);
                ship_en1_2 = new Rectangle(width + 1050, r.Next(height)-80, 60, 70);
                ship_en1_3 = new Rectangle(width + 1300, r.Next(height)-80, 60, 70);
                ship_en1_4 = new Rectangle(width + 1700, r.Next(height)-80, 60, 70);
                ship_en1_5 = new Rectangle(width + 2000, r.Next(height)-80, 60, 70);

                ship_en2_1 = new Rectangle(width + 1000, 0, 200, 230);
                ship_en2_2 = new Rectangle(width + 250, 205, 200, 230);
                ship_en2_3 = new Rectangle(width + 1800, 410, 200, 200);                
            }

            protected override void LoadContent()
            {
                spritebatch = new SpriteBatch(GraphicsDevice);

                gr_ship_en1 = Game.Content.Load<Texture2D>("Textures\\bgship_en1");
                gr_ship_en2 = Game.Content.Load<Texture2D>("Textures\\bgship_en2");

                gr_ship_al1 = Game.Content.Load<Texture2D>("Textures\\bgship_al2");
                gr_ship_al2 = Game.Content.Load<Texture2D>("Textures\\bgship_al1");

                gr_background = Game.Content.Load<Texture2D>("Textures\\back1");
                bgSize = new Vector2(width, height);
                gr_star = Game.Content.Load<Texture2D>("Textures\\star");

                base.LoadContent();
            }

            public override void Update(GameTime gameTime)
            {
                HandleScrollSpeed();

                UpdateShips();

                base.Update(gameTime);
            }

            void addRandomStars()
            {
                spritebatch.Draw(gr_star, new Rectangle(r.Next(0, width), r.Next(0, height), 40, 40), Color.White);
            }


            public void HandleScrollSpeed()
            {
                //speed horizontal BackgroundScrolling
                bgPosition1.X -= bgScrollSpeed;
                bgPosition2.X -= bgScrollSpeed;

                if (bgPosition1.X <= 0)
                {
                    bgPosition1.X = width;
                    bgPosition2.X = 0;
                }
            }

            public override void Draw(GameTime gameTime)
            {
                spritebatch.Begin();
                spritebatch.Draw(gr_background, new Rectangle((int)Position1.X, (int)Position1.Y, (int)bgSize.X, (int)bgSize.Y), Color.White);
                addRandomStars();
                spritebatch.Draw(gr_background, new Rectangle((int)Position2.X, (int)Position2.Y, (int)bgSize.X, (int)bgSize.Y), Color.White);
                addRandomStars();
                DrawShips(spritebatch);
                spritebatch.End();

                base.Draw(gameTime);
            }

            private void UpdateShips()
            {
                Rectangle tempRec = new Rectangle();

                //update position of enemy-background-rectangles

                tempRec = ship_en1_1;
                tempRec.X -= speed_bgships1;
                if (tempRec.X < -200)
                {
                    tempRec.X = width+700;
                    tempRec.Y = r.Next(height - 80);
                }
                ship_en1_1 = tempRec;


                tempRec = ship_en1_2;
                tempRec.X -= speed_bgships1;
                if (tempRec.X < -200)
                {
                    tempRec.X = width + 1000;
                    tempRec.Y = r.Next(height - 80);
                }
                ship_en1_2 = tempRec;


                tempRec = ship_en1_3;
                tempRec.X -= speed_bgships1;
                if (tempRec.X < -200)
                {
                    tempRec.X = width + 1450;
                    tempRec.Y = r.Next(height - 80);
                }
                ship_en1_3 = tempRec;


                tempRec = ship_en1_4;
                tempRec.X -= speed_bgships1;
                if (tempRec.X < -200)
                {
                    tempRec.X = width + 1750;
                    tempRec.Y = r.Next(height - 80);
                }
                ship_en1_4 = tempRec;


                tempRec = ship_en1_5;
                tempRec.X -= speed_bgships1;
                if (tempRec.X < -200)
                {
                    tempRec.X = width + 2000;
                    tempRec.Y = r.Next(height - 80);
                }
                ship_en1_5 = tempRec;


                tempRec = ship_en2_1;
                tempRec.X -= speed_bgships2;
                if (tempRec.X < -200)
                {
                    tempRec.X = width + 600;
                }
                ship_en2_1 = tempRec;


                tempRec = ship_en2_2;
                tempRec.X -= speed_bgships2;
                if (tempRec.X < -200)
                {
                    tempRec.X = width + 600;
                }
                ship_en2_2 = tempRec;


                tempRec = ship_en2_3;
                tempRec.X -= speed_bgships2;
                if (tempRec.X < -200)
                {
                    tempRec.X = width + 520;
                }
                ship_en2_3 = tempRec;
            }
            
            private void DrawShips(SpriteBatch spriteBatch)
            {
                spriteBatch.Draw(gr_ship_en2, ship_en2_1, Color.White);
                spriteBatch.Draw(gr_ship_en2, ship_en2_2, Color.White);
                spriteBatch.Draw(gr_ship_en2, ship_en2_3, Color.White);

                spriteBatch.Draw(gr_ship_en1, ship_en1_1, Color.White);
                spriteBatch.Draw(gr_ship_en1, ship_en1_2, Color.White);
                spriteBatch.Draw(gr_ship_en1, ship_en1_3, Color.White);
                spriteBatch.Draw(gr_ship_en1, ship_en1_4, Color.White);
                spriteBatch.Draw(gr_ship_en1, ship_en1_5, Color.White);                
            }

            //ScrollSpeed increases by pressing D
            /*
            public void ButtonInput()
            {
                kbState = Keyboard.GetState();

                if (kbState.IsKeyDown(Keys.D))
                {
                    bgPosition1.X -= bgScrollSpeed + 1;
                    bgPosition2.X -= bgScrollSpeed + 1;
                }
               
            } 
            */
        }   
}


