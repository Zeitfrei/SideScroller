using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

namespace Game1
{
    public class Game1 : Microsoft.Xna.Framework.Game
    {
        enum GameState { Intro = 0,TitleScreen, GameStarted, GameEnded , GamePaused}

        GraphicsDeviceManager graphics;
        VideoPlayer videoplayer;
        Video introVid;
        Vector2 videoDimensions;
        Texture2D videoTexture;
        Rectangle videoScreen;
        SpriteBatch spriteBatch;
        Player player;
        EnemyManager em;
        PowerUpManager pum;
        Song au_bgm;
        Song au_intro1;
        Background_Space bgSpace;
        SpriteFont sFont1,sFont2;
        Vector2 txt_IntroPos, txt_ContinuePos, txt_ScorePos, txt_GameOverPos;
        Vector2 txt_ExitPos, txt_ScorePos2, txt_ControlsPos, txt_ControlsPos1;
        Vector2 txt_ControlsPos2, pos_pause_title, pos_pause_continue, pos_pause_restart;
        Vector2 pos_pause_exit, pos_main_title, pos_main_pause, pos_main_quit;
        KeyboardState kbState;
        int height, width, scrollSpeed;
        string score;
        bool fadeout;
        GameState gameState;

        public Game1()
        {
            fadeout = false;
            gameState = GameState.Intro;
            height = 640;
            width = 1424;
            graphics = new GraphicsDeviceManager(this);
            graphics.IsFullScreen = false;
            graphics.PreferredBackBufferHeight = height;
            graphics.PreferredBackBufferWidth = width;
            this.Window.Title = "Super Space Game";
            Content.RootDirectory = "Content";
            scrollSpeed = 4;
            videoplayer = new VideoPlayer();
            videoScreen = new Rectangle(0, 0, width, height);
        }

        protected override void Initialize()
        {
            //initialize Background-Object
            bgSpace = new Background_Space(this);
            bgSpace.Initialize();
            base.Initialize();
        }

        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);

            au_bgm = Content.Load<Song>("Sounds\\level7");
            au_intro1 = Content.Load<Song>("Sounds\\au_intro1");

            introVid = Content.Load<Video>("intro1");
            videoDimensions = new Vector2(introVid.Width, introVid.Height);
            videoplayer.Play(introVid);
            videoplayer.IsMuted = true;

            sFont1 = Content.Load<SpriteFont>("SpriteFont1");
            sFont2 = Content.Load<SpriteFont>("SpriteFont2");

            initGUIPos();
        }

        protected override void Update(GameTime gameTime)
        {
            kbState = Keyboard.GetState();
            switch (gameState)
            {
                case GameState.Intro:
                    {
                        if (!MediaPlayer.IsRepeating)
                        {
                            MediaPlayer.Play(au_intro1);
                            MediaPlayer.IsRepeating = true;
                        }

                        if (kbState.IsKeyDown(Keys.Space) || kbState.IsKeyDown(Keys.Escape))
                        {
                            gameState = GameState.TitleScreen;
                            videoplayer.Stop();
                        }

                        if (videoplayer.State == MediaState.Stopped)
                        {
                            gameState = GameState.TitleScreen;
                        }
                            
                        break;
                    }
                case GameState.TitleScreen:
                    {
                        if (kbState.IsKeyDown(Keys.E))
                        {
                            resetGame();
                            fadeout = true;
                        }

                        if (kbState.IsKeyDown(Keys.Escape))
                        {
                            Exit();
                        }

                        if (fadeout)
                            FadeOut();

                        break;
                    }
                case GameState.GameStarted:
                    {
                        if (!MediaPlayer.IsRepeating)
                        {
                            MediaPlayer.Play(au_bgm);
                            MediaPlayer.IsRepeating = true;
                        }

                        FadeIn();

                        if (player.isAlive)
                        {
                            bgSpace.Update(gameTime);
                            em.Update(gameTime);
                            pum.Update(gameTime);
                            player.Update(gameTime);
                            handleCollisions();
                            base.Update(gameTime);

                            if (kbState.IsKeyDown(Keys.F))
                                gameState = GameState.GamePaused;
                            else if (kbState.IsKeyDown(Keys.Escape))
                                Exit();
                        }
                        else
                        {
                            if (kbState.IsKeyDown(Keys.E))
                                resetGame();
                            else if (kbState.IsKeyDown(Keys.Escape))
                                Exit();
                        }
                        break;
                    }
                case GameState.GameEnded:
                    {
                        break;
                    }
                case GameState.GamePaused:
                    {
                        if (kbState.IsKeyDown(Keys.R))
                        {
                            gameState = GameState.GameStarted;
                            resetGame();
                        }
                        else if (kbState.IsKeyDown(Keys.Escape))
                            Exit();
                        else if (kbState.IsKeyDown(Keys.E))
                            gameState = GameState.GameStarted;
                        break;
                    }
            }
        }

        protected override void Draw(GameTime gameTime)
        {
            switch (gameState)
            {
                case GameState.Intro:
                    {
                        spriteBatch.Begin();

                        videoTexture = videoplayer.GetTexture();
                        if(videoTexture != null)
                            spriteBatch.Draw(videoTexture, videoScreen, Color.White);
                        spriteBatch.End();
                        break;
                    }
                case GameState.TitleScreen:
                    {
                        spriteBatch.Begin();
                        bgSpace.Draw(gameTime);
                        spriteBatch.DrawString(sFont1, "MAIN MENU", pos_main_title, Color.White);
                        spriteBatch.DrawString(sFont1, "Press 'E' to start the game!", txt_IntroPos, Color.White);
                        spriteBatch.DrawString(sFont1, "Press 'F' to pause the game!", pos_main_pause, Color.White);
                        spriteBatch.DrawString(sFont1, "Press 'Escape' to quit the game!", pos_main_quit, Color.White);
                        spriteBatch.DrawString(sFont1, "Controls:", txt_ControlsPos, Color.White);
                        spriteBatch.DrawString(sFont1, "Use WASD to move", txt_ControlsPos1, Color.White);
                        spriteBatch.DrawString(sFont1, "Press 'Space' to shoot", txt_ControlsPos2, Color.White);
                        spriteBatch.End();
                        break;
                    }
                case GameState.GameStarted:
                    {
                        spriteBatch.Begin();
                        bgSpace.Draw(gameTime);
                        if (player.isAlive)
                        {
                            player.Draw(spriteBatch);
                            em.Draw(spriteBatch);
                            pum.Draw(spriteBatch);
                            //draw Score
                            spriteBatch.DrawString(sFont1, "Score: " + player.score.ToString(), txt_ScorePos, Color.White);
                        }
                        else
                        {
                            spriteBatch.DrawString(sFont2, "Game Over", txt_GameOverPos, Color.White);
                            spriteBatch.DrawString(sFont1, "Score:   " + score, txt_ScorePos2, Color.White);
                            spriteBatch.DrawString(sFont1, "Press 'E' to continue!", txt_ContinuePos, Color.White);
                            spriteBatch.DrawString(sFont1, "Press 'Escape' to Exit", txt_ExitPos, Color.White);
                        }
                        spriteBatch.End();
                        break;
                    }
                case GameState.GameEnded:
                    {
                        break;
                    }
                case GameState.GamePaused:
                    {
                        spriteBatch.Begin();
                        bgSpace.Draw(gameTime);
                        player.Draw(spriteBatch);
                        em.Draw(spriteBatch);
                        pum.Draw(spriteBatch);
                        spriteBatch.DrawString(sFont1, "Score: " + player.score.ToString(), txt_ScorePos, Color.White);
                        spriteBatch.DrawString(sFont1, "PAUSE", pos_pause_title, Color.White);
                        spriteBatch.DrawString(sFont1, "Continue:  E", pos_pause_continue, Color.White);
                        spriteBatch.DrawString(sFont1, "Restart:   R", pos_pause_restart, Color.White);
                        spriteBatch.DrawString(sFont1, "Quit:      Escape", pos_pause_exit, Color.White);
                        spriteBatch.End();
                        break;
                    }
            }
            base.Draw(gameTime);
        }

        private int handleCollisions()
        {
            List<Enemy> enemies = em.enemies;
            //Schiff wird zerstoert am linken Rand
            if (player.position.X < 0)
            {
                padScore();
                player.killPlayer();
            }
            if (!(enemies == null))
            {
                //check for collision with each enemy and with each of its fired shots
                foreach (Enemy e in enemies)
                {
                    //player projectiles right
                    for (int i = 0; i < player.gr_projectiles_right.Count;i++ )
                    {
                        if (e.Intersects(player.gr_projectiles_right[i]))
                        {
                            player.gr_projectiles_right.RemoveAt(i); // delete shot after collision - handled in player in differnt modes?
                            e.killEnemy();
                            player.score += 1000; // should depend on type of enemy 
                        }
                    }
                    //player projectiles right
                    for (int i = 0; i < player.gr_projectiles_left.Count; i++)
                    {
                        if (e.Intersects(player.gr_projectiles_left[i]))
                        {
                            player.gr_projectiles_left.RemoveAt(i);
                            e.killEnemy();
                            player.score += 1000; 
                        }
                    }
                    //collision between enemy and player
                    if (player.Intersects(e.enemyBounds))
                    {
                        padScore();
                        player.killPlayer();
                        e.killEnemy();
                        return 1;
                    }
                    //enemy projectiles
                    foreach (Rectangle r in e.gr_projectiles)
                    {
                        if (player.Intersects(r))
                        {
                            padScore();
                            e.gr_projectiles.Remove(r);
                            player.killPlayer();
                            return 1;
                        }
                    }

                }
            }
            if(pum.powerUps != null)
            {
                for (int i = 0; i < pum.powerUps.Count;i++ )
                {
                    //change properties of player according to type of power up
                    if (player.Intersects(pum.powerUps[i].puRect))
                    {
                        pum.powerUps[i].pickUp();
                        player.addPowerUp(pum.powerUps[i].type);
                    }
                }
            }
            return 0;
        }
        public void resetGame()
        {
            player = new Player(this, scrollSpeed);
            player.setSize(height, width);
            player.Initialize();

            em = new EnemyManager(this, width, height);
            em.Initialize();

            pum = new PowerUpManager(this, width, height);
            pum.Initialize();

            player.LoadContent(Content);
            em.LoadContent(Content);
            pum.LoadContent(Content);

            bgSpace = new Background_Space(this);
            bgSpace.Initialize();
        }
        
        private void padScore()
        {
            string tempscore = player.score.ToString();

            switch(tempscore.Length)
            {
                case 1: score = "0000000"; break;
                case 4: score = "000" + tempscore; break;
                case 5: score = "00" + tempscore; break;
                case 6: score = "0" + tempscore; break;
                default: score = tempscore; break;
            }
        }

        // fades out the music and for now also changes the game state
        private void FadeOut()
        {
            if (MediaPlayer.Volume < .1f)
            {
                MediaPlayer.Stop();
                MediaPlayer.IsRepeating = false;
                gameState = GameState.GameStarted;
            }

            if (MediaPlayer.Volume - .1f > 0)
            {
                MediaPlayer.Volume -= .02f;
            }
        }

        // fades in the music to 1.
        private void FadeIn()
        {
            if (MediaPlayer.Volume == 1f)
            {
                return;
            }

            if (MediaPlayer.Volume + .1f < 1)
            {
                MediaPlayer.Volume += .012f;
            }
            else
                MediaPlayer.Volume = 1f;
        } 

        private void initGUIPos()
        {
            txt_ScorePos.X = 2;
            txt_ScorePos.Y = 2;

            txt_IntroPos.X = width / 2 - 170;
            txt_IntroPos.Y = height / 2 - 200;
            txt_ControlsPos.X = width / 2 - 50;
            txt_ControlsPos.Y = height / 2 + 50;
            txt_ControlsPos1.X = width / 2 - 95;
            txt_ControlsPos1.Y = height / 2 + 90;
            txt_ControlsPos2.X = width / 2 - 135;
            txt_ControlsPos2.Y = height / 2 + 130;

            txt_GameOverPos.X = width / 2 - 100;
            txt_GameOverPos.Y = height / 2 - 200;
            txt_ScorePos2.X = width / 2 - 110;
            txt_ScorePos2.Y = height / 2 - 80;
            txt_ContinuePos.X = width / 2 - 144;
            txt_ContinuePos.Y = height / 2 + 50;
            txt_ExitPos.X = width / 2 - 144;
            txt_ExitPos.Y = height / 2 + 100;

            //PAUSE SCREEN
            pos_pause_title.X = width / 2 - 50;
            pos_pause_title.Y = height / 2 - 120;
            
            pos_pause_continue.X = width / 2 - 100;
            pos_pause_continue.Y = height / 2 - 80;

            pos_pause_restart.X = width / 2 - 100;
            pos_pause_restart.Y = height / 2 - 60;

            pos_pause_exit.X = width / 2 - 100;
            pos_pause_exit.Y = height / 2 - 40;

            //START SCREEN
            pos_main_title.X = width / 2 - 50;
            pos_main_title.Y = height / 2 - 300;

            pos_main_quit.X = width / 2 - 190;
            pos_main_quit.Y = height / 2 - 120;

            pos_main_pause.X = width / 2 - 170;
            pos_main_pause.Y = height / 2 - 160;
        }
    }
}

/*
 * Assets used:
 * 
 * Spaceships: http://opengameart.org/users/skorpio
 * Enemies: http://opengameart.org/users/skorpio
 * Star: http://opengameart.org/users/clint-bellanger
 * Background: http://opengameart.org/users/westbeam 
 * Laser Sounds: http://opengameart.org/users/jan125
 * Background Music: http://opengameart.org/users/dklon
 * Background Music: http://opengameart.org/users/foxsynergy
 * Explosion: http://opengameart.org/users/dklon
 * Explosion Texture: http://m484games.ucoz.com/
 * Laser Texture: Master484 http://m484games.ucoz.com/
 * Power-Up sound: Jesús Lastra (opengameart.org)
 * Intro Footage by Nissam Farin / Footage Island
*/
