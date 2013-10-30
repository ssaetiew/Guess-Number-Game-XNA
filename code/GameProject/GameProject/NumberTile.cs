using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace GameProject
{
    /// <remarks>
    /// A number tile
    /// </remarks>
    class NumberTile
    {
        #region Fields

        // original length of each side of the tile
        int originalSideLength;

        // whether or not this tile is the correct number
        bool isCorrectNumber;

        // drawing support
        Texture2D texture;
        Rectangle drawRectangle;
        Rectangle sourceRectangle;
        Texture2D blinkingTile;
        Texture2D currentTile;

        // blinking support
        const int TOTAL_BLINK_MILLISECONDS = 50000;
        int elapsedBlinkMilliseconds = 100;
        const int FRAME_BLINK_MILLISECONDS = 2;
        int elapsedFrameMilliseconds = 100;

        bool isTileVisible = true;
        bool isTileblink = false;
        bool isTileShrink = false;

        bool clickStarted = false;
        bool buttonReleased = true;

        
        //timer
        const int TOTAL_SHRINK_MILLISECONDS = 500;
        int elapsedShrinkMilliseconds = 0;

        //Sound
        SoundBank soundBank;
        #endregion

        #region Constructors

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="contentManager">the content manager</param>
        /// <param name="center">the center of the tile</param>
        /// <param name="sideLength">the side length for the tile</param>
        /// <param name="number">the number for the tile</param>
        /// <param name="correctNumber">the correct number</param>
        /// <param name="soundBank">the sound bank for playing cues</param>
        public NumberTile(ContentManager contentManager, Vector2 center, int sideLength,
            int number, int correctNumber, SoundBank soundBank)
        {
            // set original side length field
            this.originalSideLength = sideLength;

            // set sound bank field
            this.soundBank = soundBank;

            // load content for the tile and create draw rectangle
            LoadContent(contentManager, number);
            drawRectangle = new Rectangle((int)center.X - sideLength / 2,
                 (int)center.Y - sideLength / 2, sideLength, sideLength);

            // set isCorrectNumber flag
            isCorrectNumber = number == correctNumber;
        }

        #endregion

        #region Public methods

        /// <summary>
        /// Updates the tile based on game time and mouse state
        /// </summary>
        /// <param name="gameTime">the current GameTime</param>
        /// <param name="mouse">the current mouse state</param>
        /// <return>true if the correct num ber was guessed, false otherwise</return>
        public bool Update(GameTime gameTime, MouseState mouse)
        {   
         
            if (isTileVisible)
            {
                // check for mouse over button
                if (drawRectangle.Contains(mouse.X, mouse.Y))
                {
                    // highlight button
                    sourceRectangle.X = texture.Width / 2;

                    // check for click started on button
                    if (mouse.LeftButton == ButtonState.Pressed &&
                        buttonReleased)
                    {
                        clickStarted = true;
                    }
                    else if (mouse.LeftButton == ButtonState.Released)
                    {
                        buttonReleased = true;

                        // if click finished on button and get correct number
                        if (clickStarted)
                        {
                         //click is no longer started on button
                            clickStarted = false;
                            if (isCorrectNumber)
                            {
                                isTileblink = true;
                                soundBank.PlayCue("correctGuess");
                                currentTile = blinkingTile;
                                sourceRectangle.X = 0;
                               
                            }
                            else
                            {
                                isTileShrink = true;
                                soundBank.PlayCue("incorrectGuess");
                            }
                           
                        }
                    }
                }
                else
                {
                    sourceRectangle.X = 0;

                    // no clicking on this button
                    clickStarted = false;
                    buttonReleased = false;
                }
            }

            //Check if tile is blinking
            if(isTileblink)
            {
                    elapsedBlinkMilliseconds = elapsedBlinkMilliseconds + gameTime.ElapsedGameTime.Milliseconds;


                    //Switch the frames to blink
                    if (elapsedBlinkMilliseconds < TOTAL_BLINK_MILLISECONDS)
                    {
                        elapsedFrameMilliseconds = elapsedFrameMilliseconds + gameTime.ElapsedGameTime.Milliseconds;

                        if (elapsedFrameMilliseconds >= FRAME_BLINK_MILLISECONDS)
                        {
                            elapsedFrameMilliseconds -= FRAME_BLINK_MILLISECONDS;
                            if (sourceRectangle.X == 0)
                            {
                                sourceRectangle.X = currentTile.Width / 2;
                            }
                            else
                            {
                                sourceRectangle.X = 0;
                            }
                            //Resetting the Frame time  
                            elapsedFrameMilliseconds = 0;
                        }
                        if (elapsedBlinkMilliseconds >= TOTAL_BLINK_MILLISECONDS)
                        {
                            isTileVisible = true;
                            
                        }


                    }
                    else
                    {
                        isTileVisible = false;
                    }
                    return isTileVisible;
                    
            }
            else if (isTileShrink)
            {
                elapsedShrinkMilliseconds += gameTime.ElapsedGameTime.Milliseconds;
                int newSideLength = (originalSideLength * (TOTAL_SHRINK_MILLISECONDS - elapsedShrinkMilliseconds)) / TOTAL_SHRINK_MILLISECONDS;
                if (newSideLength > 0)
                {
                    drawRectangle.Width = newSideLength;
                    drawRectangle.Height = newSideLength;
                }
                else
                {
                    isTileVisible = false;
                }            
            }
            return false;
        }

        /// <summary>
        /// Draws the number tile
        /// </summary>
        /// <param name="spriteBatch">the SpriteBatch to use for the drawing</param>
        public void Draw(SpriteBatch spriteBatch)
        {
            // draw the tile
            if (isTileVisible)
            {
                spriteBatch.Begin();
                spriteBatch.Draw(currentTile, drawRectangle, sourceRectangle, Color.White);
                spriteBatch.End();
            }
        }

        #endregion

        #region Private methods

        /// <summary>
        /// Loads the content for the tile
        /// </summary>
        /// <param name="contentManager">the content manager</param>
        /// <param name="number">the tile number</param>
        private void LoadContent(ContentManager contentManager, int number)
        {
            // convert the number to a string
            string numberString = ConvertIntToString(number);

            // load content for the tile and set source rectangle
            texture = contentManager.Load<Texture2D>(numberString);

            currentTile = texture;

            //Load blinking number
            string blinkString = ConvertIntToString(number);
            blinkingTile = contentManager.Load<Texture2D>("blinking" + blinkString);

            sourceRectangle = new Rectangle(0, 0, texture.Width / 2, texture.Height);

        }

        /// <summary>
        /// Converts an integer to a string for the corresponding number
        /// </summary>
        /// <param name="number">the integer to convert</param>
        /// <returns>the string for the corresponding number</returns>
        private String ConvertIntToString(int number)
        {
            switch (number)
            {
                case 1:
                    return "one";
                case 2:
                    return "two";
                case 3:
                    return "three";
                case 4:
                    return "four";
                case 5:
                    return "five";
                case 6:
                    return "six";
                case 7:
                    return "seven";
                case 8:
                    return "eight";
                case 9:
                    return "nine";
                default:
                    throw new Exception("Unsupported number for number tile");
            }

        }

        #endregion
    }
}
