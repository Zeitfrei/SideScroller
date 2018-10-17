using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Audio;

namespace Game1
{
    class PowerUp : Microsoft.Xna.Framework.DrawableGameComponent
    {
        int screenWidth, screenHeight;
        public int type;
        float speed;
        public bool pickedUp;
        Texture2D gr_texture;
        SoundEffect au_sound;
        Vector2 position;
        public Rectangle puRect;

        

        public PowerUp(Game game, int type,int screenWidth, int screenHeight,float speed, Texture2D texture, SoundEffect au_sound, Vector2 position)
        :base(game)
        {
            this.type = type;
            this.screenHeight = screenHeight;
            this.screenWidth = screenWidth;
            this.position = position;
            this.au_sound = au_sound;
            this.speed = speed;
            gr_texture = texture;

            pickedUp = false;
            puRect = new Rectangle((int)position.X,(int)position.Y,50,50);
        }

        public override void Initialize()
        {
            base.Initialize();
        }

        public void LoadContent(ContentManager Content)
        {
            base.LoadContent();
        }
        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(gr_texture, puRect, Color.White);
        }

        public override void Update(GameTime gameTime)
        {
            //update position, when leaving the screen -> pickedUp true
            if(position.X>=0)
                position.X -= speed;
            else
                pickedUp = true;

            puRect.X = (int)position.X;
        }
        public void pickUp()
        {
            au_sound.Play();
            pickedUp = true;
        }
    }
}
