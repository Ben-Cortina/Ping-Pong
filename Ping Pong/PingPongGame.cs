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
using Microsoft.Xna.Framework.Net;
using Microsoft.Xna.Framework.Storage;

namespace Ping_Pong
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class PingPongGame : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        // the score
        int m_Score1 = 0;
        int m_Score2 = 0;
        SpriteFont font;

        // the ball
        Ball m_ball;
        Texture2D m_textureBall;

        //String credits
        //String author = "Customized by Ben Cortina";
        String songAuthor = "Song courtesy of Flexstyle";

        // the paddles
        Paddle m_paddle1;
        Paddle m_paddle2;
        Texture2D m_texturePaddle;
        Texture2D m_texturePaddleHit;

        // how much to move paddle each frame
        private const float PADDLE_STRIDE = 10.0f;

        //paddle hit detection
        bool hit1 = false;
        bool hit2 = false;

        // constants
        const int SCREEN_WIDTH = 640;
        const int SCREEN_HEIGHT = 480;

        // the background
        Rectangle SCREEN = new Rectangle(0, 0, SCREEN_WIDTH, SCREEN_HEIGHT);
        Texture2D textureBackground;

        // sounds
        Song songBack;
        SoundEffect soundEffect_paddle1;
        SoundEffect soundEffect_paddle2;
        SoundEffectInstance soundInstance_paddle1;
        SoundEffectInstance soundInstance_paddle2;
        SoundEffect soundEffect_point;
        SoundEffectInstance soundInstance_point;

        // previous controller state, for press, release detection
        GamePadState pad1_old = GamePad.GetState(PlayerIndex.One);
        GamePadState pad2_old = GamePad.GetState(PlayerIndex.Two);
        KeyboardState keyb_old = Keyboard.GetState();

        // struct for control states
        struct Controls
        {
            // player movement
            public bool p1_Up, p1_Down;
            public bool p2_Up, p2_Down;

            // computer player
            public bool p1_comp;
            public bool p2_comp;

            //game controls
            public bool audio_back;
            public bool game_reset;
            public bool game_exit;
        }
        Controls controlState;

        public PingPongGame()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            // use a fixed frame rate of 30 frames per second
            IsFixedTimeStep = true;
            TargetElapsedTime = new TimeSpan(0, 0, 0, 0, 33);
            controlState.audio_back = true;

            InitScreen();
            InitGameObjects();

            base.Initialize();
        }

        // screen-related init tasks
        public void InitScreen()
        {
            // back buffer
            graphics.PreferredBackBufferHeight = SCREEN_HEIGHT;
            graphics.PreferredBackBufferWidth = SCREEN_WIDTH;
            graphics.PreferMultiSampling = false;
            graphics.ApplyChanges();
        }

        // game-related init tasks
        public void InitGameObjects()
        {
            // create an instance of our ball
            m_ball = new Ball();

            // set the size of the ball
            m_ball.Width = 15.0f;
            m_ball.Height = 15.0f;

            // create 2 instances of our paddle
            m_paddle1 = new Paddle();
            m_paddle2 = new Paddle();

            // set the size of the paddles
            m_paddle1.Width = 15.0f;
            m_paddle1.Height = 100.0f;
            m_paddle2.Width = 15.0f;
            m_paddle2.Height = 100.0f;

            ResetGame();
        }

        // initial play state, called when the game is first
        // run, and whever a player scores 100 goals
        public void ResetGame()
        {
            Random speed = new Random();
            // reset scores
            m_Score1 = 0;
            m_Score2 = 0;

            // place the ball at the center of the screen
            m_ball.X =
                SCREEN_WIDTH / 2 - m_ball.Width / 2;
            m_ball.Y =
                SCREEN_HEIGHT / 2 - m_ball.Height / 2;

            // set a speed and direction for the ball
            m_ball.DX = speed.Next(4, 6);
            m_ball.DY = speed.Next(3, 5);
            if (speed.Next(0, 2) == 1)
                m_ball.DX = -m_ball.DX;
            if (speed.Next(0, 2) == 1)
                m_ball.DY = -m_ball.DY;

            // place the paddles at either end of the screen
            m_paddle1.X = 0;
            m_paddle1.Y =
                SCREEN_HEIGHT / 2 - m_paddle1.Height / 2;
            m_paddle2.X =
                SCREEN_WIDTH - m_paddle2.Width;
            m_paddle2.Y =
                SCREEN_HEIGHT / 2 - m_paddle1.Height / 2;

            // turn off computer players
            controlState.p1_comp = false;
            controlState.p2_comp = false;
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            // load images from disk
            LoadGameGraphics();

            // load audio from disk
            LoadGameAudio();
        }

        // load our textures from disk
        protected void LoadGameGraphics()
        {
            // load the texture for the ball
            m_textureBall =
                Content.Load<Texture2D>(@"media\images\ball");
            m_ball.Visual = m_textureBall;

            // load the texture for the paddles
            m_texturePaddle =
                Content.Load<Texture2D>(@"media\images\paddle");
            m_paddle1.Visual = m_texturePaddle;
            m_paddle2.Visual = m_texturePaddle;

            // load the texture for Paddle Hit effect
            m_texturePaddleHit =
                Content.Load<Texture2D>(@"media\images\paddlehit");
            m_paddle1.VisualHit = m_texturePaddleHit;
            m_paddle2.VisualHit = m_texturePaddleHit;

            //load the texture for the background
            textureBackground =
                Content.Load<Texture2D>(@"media\images\back");

            //Load the Font for the score
            font = Content.Load<SpriteFont> ("Quartz");
        }

        // load audio from disk
        protected void LoadGameAudio()
        {
            // load background song
            songBack = Content.Load<Song>(@"media\audio\Flexstyle - Foliage Collective");

            // load sound effect
            soundEffect_paddle1 = Content.Load<SoundEffect>(@"media\audio\ping");
            soundEffect_paddle2 = Content.Load<SoundEffect>(@"media\audio\pong");
            soundEffect_point = Content.Load<SoundEffect>(@"media\audio\point");

            //create sound instance
            soundInstance_paddle1 = soundEffect_paddle1.CreateInstance();
            soundInstance_paddle2 = soundEffect_paddle2.CreateInstance();
            soundInstance_point = soundEffect_point.CreateInstance();

            // set sound settings
            soundInstance_paddle1.Volume = 0.1f;
            soundInstance_paddle2.Volume = 0.15f;
            soundInstance_paddle1.Pan = -0.7f;
            soundInstance_paddle2.Pan = 0.7f;
        }


        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            // Allows the game to exit
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
                this.Exit();

            // update the ball's location on the screen
            MoveBall();
            // check controller states
            ControlCheck();
            // actually move the paddles
            MovePaddles();
            // update settings based on controlState
            SettingsUpdate();

            base.Update(gameTime);
        }

        // move the ball based on it's current DX and DY 
        // settings. check for collisions
        private void MoveBall()
        {
            // actually move the ball
            m_ball.X += m_ball.DX;
            m_ball.Y += m_ball.DY;

            // did ball touch top or bottom side?
            if (m_ball.Y <= 0 ||
                m_ball.Y >= SCREEN_HEIGHT - m_ball.Height)
            {
                // reverse vertical direction
                m_ball.DY *= -1;
                
            }

            // did ball hit the paddle from the front?
            int collision = CollisionOccurred();
            while(collision !=0)
            {

                // check which paddle it hit and fix any pass throughs
                if (collision == 1)
                {
                    hit1 = true;
                    m_ball.X = 2*(m_paddle1.X + m_paddle1.Width) - m_ball.X;
                }
                else
                {
                    hit2 = true;
                    m_ball.X = 2*m_paddle2.X - (m_ball.X + 2*m_ball.Width);
                }
                // reverse hoizontal direction
                m_ball.DX *= -1;

                // increase the speed a little.
                if (m_ball.DX < SCREEN_WIDTH && m_ball.DX > -SCREEN_WIDTH)
                    m_ball.DX *= 1.15f;

                collision = CollisionOccurred();
            }

            // did ball touch the left side?
            if (m_ball.X <= 0)
            {
                // at higher speeds, the ball can leave the 
                // playing field, make sure that doesn't happen
                m_ball.X = 0;

                // increment player 2's score
                m_Score2++;

                // reduce speed, reverse direction
                m_ball.DX = 5.0f;

                // play point sound effect
                soundInstance_point.Pan = -0.8f;
                soundInstance_point.Play();
            }

            // did ball touch the right side?
            if (m_ball.X >= SCREEN_WIDTH - m_ball.Width)
            {
                // at higher speeds, the ball can leave the 
                // playing field, make sure that doesn't happen
                m_ball.X = SCREEN_WIDTH - m_ball.Width;

                // increment player 1's score
                m_Score1++;

                // reduce speed, reverse direction
                m_ball.DX = -5.0f;

                // play point sound effect
                soundInstance_point.Pan = 0.8f;
                soundInstance_point.Play();
            }

            // reset game if a player scores 100 goals
            if (m_Score1 > 99 || m_Score2 > 99)
            {
                ResetGame();
            }

        }

        // check for a collision between the ball and paddles
        private int CollisionOccurred()
        {
            // assume no collision
            int retval = 0;

            // heading towards player one
            if (m_ball.DX < 0)
            {
                Rectangle b = m_ball.Rect;
                Rectangle p = m_paddle1.Rect;
                if (
                    b.Left < p.Right &&
                    b.Top < p.Bottom &&
                    b.Bottom > p.Top)
                    retval = 1;
            }
            // heading towards player two
            else // m_ball.DX > 0
            {
                Rectangle b = m_ball.Rect;
                Rectangle p = m_paddle2.Rect;
                if(
                    b.Right > p.Left &&
                    b.Top < p.Bottom &&
                    b.Bottom > p.Top)
                    retval=2;
            }

            return retval;
        }

        // check controlls
        private void ControlCheck()
        {

            // get player input
            GamePadState pad1 =
                GamePad.GetState(PlayerIndex.One);
            GamePadState pad2 =
                GamePad.GetState(PlayerIndex.Two);
            KeyboardState keyb =
                Keyboard.GetState();

            // check the controller, PLAYER ONE
            controlState.p1_Up =
                pad1.DPad.Up == ButtonState.Pressed;
            controlState.p1_Down =
                pad1.DPad.Down == ButtonState.Pressed;
            if (pad1_old.IsButtonDown(Buttons.A) && pad1.IsButtonDown(Buttons.A))
                controlState.p1_comp = !controlState.p1_comp;


            // also check the keyboard, PLAYER ONE
            controlState.p1_Up |= keyb.IsKeyDown(Keys.W);
            controlState.p1_Down |= keyb.IsKeyDown(Keys.S);
            if (keyb_old.IsKeyUp(Keys.D1) && keyb.IsKeyDown(Keys.D1))
                controlState.p1_comp = !controlState.p1_comp;

            // check the controller, PLAYER TWO
            controlState.p2_Up =
                pad2.DPad.Up == ButtonState.Pressed;
            controlState.p2_Down =
                pad2.DPad.Down == ButtonState.Pressed;
            if (pad1_old.IsButtonDown(Buttons.A) && pad1.IsButtonDown(Buttons.A))
                controlState.p2_comp = !controlState.p2_comp;

            // also check the keyboard, PLAYER TWO
            controlState.p2_Up |= keyb.IsKeyDown(Keys.Up);
            controlState.p2_Down |= keyb.IsKeyDown(Keys.Down);
            if (keyb_old.IsKeyUp(Keys.D2) && keyb.IsKeyDown(Keys.D2))
                controlState.p2_comp = !controlState.p2_comp;

            //check other options
            controlState.game_reset = keyb_old.IsKeyUp(Keys.R) && keyb.IsKeyDown(Keys.R);
            controlState.game_exit = keyb_old.IsKeyUp(Keys.Q) && keyb.IsKeyDown(Keys.Q);
            controlState.game_exit |= keyb_old.IsKeyUp(Keys.Escape) && keyb.IsKeyDown(Keys.Escape);
            if (keyb_old.IsKeyUp(Keys.P) && keyb.IsKeyDown(Keys.P))
                controlState.audio_back = !controlState.audio_back;

            // update old controller states
            pad1_old = pad1;
            pad2_old = pad2;
            keyb_old = keyb;
        }

        // actually move the paddles
        private void MovePaddles()
        {
            // define bounds for the paddles
            float MIN_Y = 0.0f;
            float MAX_Y = SCREEN_HEIGHT - m_paddle1.Height;            

            // move player 1 paddle
            if (controlState.p1_comp)
            {
                controlState.p1_Up = m_paddle1.Y + m_paddle1.Height / 2 - PADDLE_STRIDE > m_ball.Y ;
                controlState.p1_Down = m_paddle1.Y + m_paddle1.Height / 2  + PADDLE_STRIDE < m_ball.Y ;
            }
            if (controlState.p1_Up)
            {
                m_paddle1.Y -= PADDLE_STRIDE;
                if (m_paddle1.Y < MIN_Y)
                {
                    m_paddle1.Y = MIN_Y;
                }
            }
            else if (controlState.p1_Down)
            {
                m_paddle1.Y += PADDLE_STRIDE;
                if (m_paddle1.Y > MAX_Y)
                {
                    m_paddle1.Y = MAX_Y;
                }
            }
            

            // move player 2 paddle
            if (controlState.p2_comp)
            {
                controlState.p2_Up = m_paddle2.Y + m_paddle2.Height / 2 - PADDLE_STRIDE > m_ball.Y;
                controlState.p2_Down = m_paddle2.Y + m_paddle2.Height / 2 + PADDLE_STRIDE < m_ball.Y;
            }
            if (controlState.p2_Up)
            {
                m_paddle2.Y -= PADDLE_STRIDE;
                if (m_paddle2.Y < MIN_Y)
                {
                    m_paddle2.Y = MIN_Y;
                }
            }
            else if (controlState.p2_Down)
            {
                m_paddle2.Y += PADDLE_STRIDE;
                if (m_paddle2.Y > MAX_Y)
                {
                    m_paddle2.Y = MAX_Y;
                }
            }
        }

        // updates settings based on controlState
        private void SettingsUpdate()
        {
            if (controlState.game_exit)
                this.Exit();
            if (controlState.game_reset)
                ResetGame();
            if (controlState.audio_back)
            {
                if (MediaPlayer.State == MediaState.Stopped)
                {
                    MediaPlayer.Play(songBack);
                    MediaPlayer.Volume = 0.7f;
                }
                else if (MediaPlayer.State == MediaState.Paused)
                {
                    MediaPlayer.Resume();
                }
            }
            else if (MediaPlayer.State == MediaState.Playing)
            {
                MediaPlayer.Pause();
            }
         
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            // our game-specific drawing logic
            Render();

            base.Draw(gameTime);
        }

        // draw the score at the specified location
        public void DrawScore(float x, float y, int score)
        {
            spriteBatch.DrawString(font, score.ToString(), new Vector2(x, y), new Color(30, 30, 30), 0, new Vector2(-3, -3), 1.0f, SpriteEffects.None, 0.6f);
            spriteBatch.DrawString(font, score.ToString(), new Vector2(x, y), Color.White);
            
        }

        // actually draw our game objects
        public void Render()
        {
            // black background
            graphics.GraphicsDevice.Clear(Color.Black);

            // start rendering our game graphics
            spriteBatch.Begin();

            // draw background pic
            spriteBatch.Draw(textureBackground,
                SCREEN, Color.White);

            // display song credits when song is playings
            if (MediaPlayer.State == MediaState.Playing)
            {
                spriteBatch.DrawString(font, songAuthor, new Vector2(SCREEN_WIDTH - 400, SCREEN_HEIGHT - 20), new Color(10, 10, 10), 0, new Vector2(-5, -5), 0.3f, SpriteEffects.None, 1.0f);
                spriteBatch.DrawString(font, songAuthor, new Vector2(SCREEN_WIDTH - 400, SCREEN_HEIGHT - 20), Color.Gray, 0, new Vector2(0, 0), 0.3f, SpriteEffects.None, 1.0f);
            }
            else
            {
                //Write name
                //spriteBatch.DrawString(font, author, new Vector2(SCREEN_WIDTH - 360, SCREEN_HEIGHT - 20), new Color(10, 10, 10), 0, new Vector2(-5, -5), 0.3f, SpriteEffects.None, 1.0f);
                //spriteBatch.DrawString(font, author, new Vector2(SCREEN_WIDTH - 360, SCREEN_HEIGHT - 20), Color.Gray, 0, new Vector2(0, 0), 0.3f, SpriteEffects.None, 1.0f);
            }

            // draw the score first, so the ball can
            // move over it without being obscured
            DrawScore((float)SCREEN_WIDTH * 0.25f,
                20, m_Score1);
            DrawScore((float)SCREEN_WIDTH * 0.65f,
                20, m_Score2);

            // render the game objects
            spriteBatch.Draw((Texture2D)m_ball.Visual,
                m_ball.Rect, Color.White);
            
            //Check for which paddle texture to use and wether to play a sound
            if (hit1)
            {
                spriteBatch.Draw((Texture2D)m_paddle1.VisualHit, m_paddle1.Rect, Color.White);
                hit1 = false;
                soundInstance_paddle1.Play();
            }
            else
            {
                spriteBatch.Draw((Texture2D)m_paddle1.Visual, m_paddle1.Rect, Color.White);
            }
            if (hit2)
            {
                spriteBatch.Draw((Texture2D)m_paddle2.VisualHit, m_paddle2.Rect, Color.White);
                hit2 = false;
                soundInstance_paddle2.Play();
            }
            else
            {
                spriteBatch.Draw((Texture2D)m_paddle2.Visual, m_paddle2.Rect, Color.White);
            }

            // we're done drawing
            spriteBatch.End();
        }


    }
}
