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

namespace GameProject
{
    /// <summary>
    /// This is the main type for game
    /// Author:Sittikorn Saetiew
    /// </summary>
    public class Game1 : Microsoft.Xna.Framework.Game
    {
        const int WINDOW_WIDTH = 800;
        const int WINDOW_HEIGHT = 600;

        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        // game state
        GameState gameState = GameState.Menu;

        
        Texture2D openingScreen;
        Rectangle openingScreenDrawRectangle;

        
        NumberBoard numberBoard;

        bool isCorrectNumber;

        //Randomly assign correct number
        Random rand = new Random();

        //Sound
        AudioEngine audioEngine;
        WaveBank waveBank;
        SoundBank soundBank;

        int sideLength;
        Vector2 boardCenter;
        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";

           
            graphics.PreferredBackBufferWidth = WINDOW_WIDTH;
            graphics.PreferredBackBufferHeight = WINDOW_HEIGHT;

            
            this.IsMouseVisible = true;

        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            // load audio content
            audioEngine = new AudioEngine(@"Content\sounds.xgs");
            waveBank = new WaveBank(audioEngine,@"Content\Wave Bank.xwb");
            soundBank = new SoundBank(audioEngine,@"Content\Sound Bank.xsb");
            // Increment 1: load opening screen and set opening screen draw rectangle
            openingScreen = Content.Load<Texture2D>("openingscreen");
            openingScreenDrawRectangle = new Rectangle(0, 0, openingScreen.Width, openingScreen.Height);
            boardCenter = new Vector2(WINDOW_WIDTH / 2, WINDOW_HEIGHT / 2);
            sideLength = WINDOW_HEIGHT - 100;
            StartGame();
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            KeyboardState keyboard = Keyboard.GetState();
            // Allows the game to exit
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || keyboard.IsKeyDown(Keys.Escape))
                this.Exit();
           
            
            if (gameState == GameState.Menu && Keyboard.GetState().IsKeyDown(Keys.Enter))
            {
                gameState = GameState.Play;
            }

            // if we're actually playing, update mouse state and update board
           
            if (gameState == GameState.Play)
            {
                MouseState mouse = Mouse.GetState();
                isCorrectNumber = numberBoard.Update(gameTime, mouse);
                if (isCorrectNumber)
                {
                    soundBank.PlayCue("newGame");
                    StartGame();
                }
            }
            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            if (gameState == GameState.Menu)
            {
                spriteBatch.Begin();
                spriteBatch.Draw(openingScreen, openingScreenDrawRectangle, Color.White);
                spriteBatch.End();
            }
            else if (gameState == GameState.Play)
            {
                numberBoard.Draw(spriteBatch);
            }

            base.Draw(gameTime);
        }

        /// <summary>
        /// Starts a game
        /// </summary>
        void StartGame()
        {
            // randomly generate new number for game
            int correctNumber = rand.Next(1,10);
            // create the board object (this will be moved before you're done)
            
            numberBoard = new NumberBoard(Content, boardCenter, sideLength, correctNumber, soundBank);
        }
    }
}

