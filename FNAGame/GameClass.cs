using System;
using System.Threading;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace FNAGame
{
    internal class GameClass : Game
    {
        private readonly GraphicsDeviceManager graphics;
        private readonly Color backColor = new Color(230, 230, 230);

        private SpriteBatch spriteBatch;

        private int tFrames;
        private int vFrames;
        private int currentSecond;

        private Texture2D bg, square;

        private RenderTarget2D target;

        private KeyboardState previousKeyboard = new KeyboardState();

        internal GameClass()
        {
            this.Window.AllowUserResizing = true;

            this.graphics = new GraphicsDeviceManager(this)
            {
                PreferredBackBufferWidth = 640, PreferredBackBufferHeight = 480
            };

            this.graphics.ApplyChanges();

            this.IsMouseVisible = true;
            
            this.TargetElapsedTime = new TimeSpan(166667);

            this.graphics.SynchronizeWithVerticalRetrace = false;
            this.IsFixedTimeStep = true;
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            this.spriteBatch = new SpriteBatch(this.GraphicsDevice);

            this.Content.RootDirectory = "content";
            
            this.bg = this.Content.Load<Texture2D>("bg.png");
            this.square = this.Content.Load<Texture2D>("square.png");

            this.target = new RenderTarget2D(this.graphics.GraphicsDevice, 256, 224);
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            KeyboardState previous = this.previousKeyboard;
            KeyboardState current = Keyboard.GetState();
            
            if (KeyWentDown(previous, Keys.Escape, current))
            {
                this.Exit();
            }

            base.Update(gameTime);

            if (KeyWentDown(previous, Keys.Enter, current) & current.IsKeyDown(Keys.LeftAlt))
            {
                this.graphics.ToggleFullScreen();
            }

            if (DateTime.Now.Second == this.currentSecond)
            {
                this.tFrames++;
            }
            else
            {
                this.Window.Title = "game | " + this.tFrames + " TPS, " + this.vFrames + " FPS";

                this.tFrames = 1;
                this.vFrames = 0;
                this.currentSecond = DateTime.Now.Second;
            }

            this.previousKeyboard = current;
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            if (Thread.CurrentThread.Name != "GameSubThread")
            {
                throw new Exception("do not draw from main thread");
            }

            Rectangle window = this.Window.ClientBounds;

            this.GraphicsDevice.SetRenderTarget(this.target);

            this.GraphicsDevice.Clear(this.backColor);

            this.spriteBatch.Begin();
            
            this.spriteBatch.Draw(this.bg, Vector2.Zero, Color.White);

            this.spriteBatch.End();

            this.GraphicsDevice.SetRenderTarget(null);
            this.GraphicsDevice.Clear(Color.Black);

            this.spriteBatch.Begin();
            this.spriteBatch.Draw(this.target, this.ScaleLetterbox(window.Width, window.Height, 4, 3), Color.White);
            this.spriteBatch.Draw(this.square, new Vector2(0, 3 * tFrames), Color.White);
            this.spriteBatch.End();

            base.Draw(gameTime);

            this.vFrames++;
        }
        
        private Rectangle ScaleLetterbox(int windowWidth, int windowHeight, double widthRatio, double heightRatio)
        {
            if (windowHeight / heightRatio > windowWidth / widthRatio)
            {
                int height = (int)(heightRatio * windowWidth / widthRatio);
                return new Rectangle(0, (windowHeight - height) / 2, windowWidth, height);
            }
            
            int width = (int)(widthRatio * windowHeight / heightRatio);
            return new Rectangle((windowWidth - width) / 2, 0, width, windowHeight);
        }

        private static bool KeyWentDown(KeyboardState previous, Keys key, KeyboardState current)
        {
            return previous.IsKeyUp(key) && current.IsKeyDown(key);
        }
    }
}
