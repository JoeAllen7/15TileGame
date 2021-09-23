using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace _15TileGame
{
    public enum EGameType
    {
        Freeplay,   //The standard game type, solve puzzles without a time limit
        Challenge   //Solve puzzles against the clock
    }

    public enum EGameMode
    {
        Menu,   //The main menu that is displayed on load
        Info,   //The screen displaying info about the story/game
        Game,   //The screen where the tile game is played
    }

    public enum EPlayState
    {
        Ready,      //Waiting for the player to press the play button
        Playing,    //The player is solving the puzzle and being timed/scored
        Solved,     //The player has successfully solved a puzzle
        Failed      //The player ran out of time in a timed challenge
    }

    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public class Game1 : Game
    {
        private static Texture2D[] texturesBg;          //Array of background textures
        private static Texture2D textureOverlayStart;   //Texture for the overlay showing info before starting a puzzle
        private static Texture2D textureOverlaySolved;  //Texture for the overlay showing info after solving a puzzle
        private static Texture2D textureOverlayFailed;  //Texture for the overlay showing info after failing a timed challenge
        private static Texture2D textureOverlayTitle;   //Texture for the overlay showing the game title for the menu screen
        private static PuzzleType[] puzzles;            //Array of the puzzles that can be cycled through

        private static int moveCount = 0;                               //How many times the player has moved a tile
        private static int chosenPuzzleIndex = 0;                       //Array index of the chosen PuzzleType
        private static float menuBackgroundYPos = 0;                      //The Y position at which the menu background should be drawn

        public static EGameType currentGameType { get; private set; } = EGameType.Freeplay;  //Used to keep track of the current game type, changes how the timer works in-game

        public static EGameMode currentGameMode { get; private set; } = EGameMode.Menu;      //Used to keep track of the current game mode
                                                                                             //  so appropriate UI/logic can be applied

        public static EGameMode previousGameMode { get; private set; } = EGameMode.Menu;     //Used for certain checks such as button clicks to ensure
                                                                                             //  the action started 

        public static EPlayState currentPlayState { get; private set; } = EPlayState.Ready;  //Used to determine the current play state - affects UI interactions

        public static GridController gridController { get; private set; }   //The grid controller is used to interact with
                                                                            //  the grid and its different functions

        private GraphicsDeviceManager graphics; //'Handles the configuration and management of the graphics device.' (Microsoft XNA Documentation)
        private SpriteBatch spriteBatch;        //'Enables a group of sprites to be drawn using the same settings.'  (Microsoft XNA Documentation)
        private SpriteFont fontTiles;           //The font to use for text on tiles

        public Game1()
        {
            
            graphics = new GraphicsDeviceManager(this)
            {
                //Set the window size to 1280x720
                PreferredBackBufferWidth = 1280,
                PreferredBackBufferHeight = 720
            };
            graphics.ApplyChanges();
            Content.RootDirectory = "Content";
            //Show mouse pointer and set window title text
            Window.Title = "Pierre's Artisan Tile Co.";
            IsMouseVisible = true;
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
            //Load textures that will be used for background images
            texturesBg = new Texture2D[] {
                Content.Load<Texture2D>("Images/background_menu"),
                Content.Load<Texture2D>("Images/background_info"),
                Content.Load<Texture2D>("Images/background_game"),
                Content.Load<Texture2D>("Images/background_gameChallenge")
            };
            //Load overlay textures, used to display info in-game
            textureOverlayStart = Content.Load<Texture2D>("Images/overlayStart");
            textureOverlaySolved = Content.Load<Texture2D>("Images/overlaySolved");
            textureOverlayFailed = Content.Load<Texture2D>("Images/overlayFailed");
            textureOverlayTitle = Content.Load<Texture2D>("Images/overlayTitle");
            //Load the font to be used for text on tiles
            fontTiles = Content.Load<SpriteFont>("Fonts/fontTiles");
            //Load the textures that will be used for the tile puzzles
            puzzles = new PuzzleType[] {
                new PuzzleType("Visage parfait", Content.Load<Texture2D>("Images/puzzlePierre")),
                new PuzzleType("Girafe majestueuse", Content.Load<Texture2D>("Images/puzzleGiraffe")),
                new PuzzleType("Patron dans le bain", Content.Load<Texture2D>("Images/puzzleBath")),
                new PuzzleType("Virtual rea-lait-y", Content.Load<Texture2D>("Images/puzzleCow")),
                new PuzzleType("Arc-en-ciel", Content.Load<Texture2D>("Images/puzzleRainbow"))
            };
            //Load UI content (button sprites/fonts)
            InputManager.LoadUIContent(Content);
            //Load audio content (music and sound effects)
            AudioManager.LoadAudioContent(Content);

            //Setup remaining objects/classes such as the grid and timer that required content to be loaded
            Start();
        }


        //Start is called after initialising and loading content
        protected void Start()
        {
            //Instantiate a GridController object, used for updating/drawing/interacting with the grid
            gridController = new GridController(graphics.PreferredBackBufferWidth, graphics.PreferredBackBufferHeight);
            //Setup the timer by passing the UILabel used to display time
            Timer.Initialise((UILabel)InputManager.GetUIElementWithName("label_game_timer"));
            //Set the default GameType
            ChangeGameType(EGameType.Freeplay);
        }


        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// game-specific content.
        /// </summary>
        protected override void UnloadContent()
        { }


        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            //Get the time elapsed since the last frame
            var deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;
            //Call update in the InputManager to check for user input
            InputManager.Update(previousGameMode, currentGameMode, currentPlayState, gridController);

            //These updates are only required when playing the game (not in a menu)
            if (currentGameMode == EGameMode.Game)
            {
                //Update the GridController and timer
                gridController.Update(currentPlayState, currentGameType, deltaTime);
                Timer.UpdateTimer(deltaTime, currentGameType);
            }
            else if(currentGameMode == EGameMode.Menu)
            {
                //When in the main menu, move the background up or reset its position
                //  if the animation loop is complete
                if (menuBackgroundYPos > -(graphics.PreferredBackBufferHeight))
                {
                    menuBackgroundYPos -= (deltaTime * 40f);
                }
                else
                {
                    menuBackgroundYPos = 0f;
                }
            }

            //Update the previous GameMode so it's correct for the next update loop
            previousGameMode = currentGameMode;

            base.Update(gameTime);
        }


        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            //Clear the screen
            GraphicsDevice.Clear(Color.Black);

            spriteBatch.Begin();

            //Draw the appropriate screen based on the current GameMode
            switch (currentGameMode)
            {
                case EGameMode.Menu:
                    DrawMenuScreen(spriteBatch);
                    break;
                case EGameMode.Info:
                    DrawInfoScreen(spriteBatch);
                    break;
                case EGameMode.Game:
                    DrawGameScreen(spriteBatch);
                    break;
                default:
                    break;
            }

            //Draw the UI for the current GameMode
            InputManager.DrawUI(spriteBatch, currentGameMode, GetCurrentPuzzle().texture);

            spriteBatch.End();

            base.Draw(gameTime);
        }

        private void DrawMenuScreen(SpriteBatch spriteBatch)
        {
            //Draw the background image and title for the main menu
            spriteBatch.Draw(texturesBg[0], new Rectangle(0, (int)menuBackgroundYPos, graphics.PreferredBackBufferWidth, graphics.PreferredBackBufferHeight * 2), null, Color.White);
            spriteBatch.Draw(textureOverlayTitle, new Rectangle(0, 0, graphics.PreferredBackBufferWidth, graphics.PreferredBackBufferHeight), null, Color.White);
        }

        private void DrawInfoScreen(SpriteBatch spriteBatch)
        {
            //Draw the background image for the info/instructions screen
            spriteBatch.Draw(texturesBg[1], new Rectangle(0, 0, graphics.PreferredBackBufferWidth, graphics.PreferredBackBufferHeight), null, Color.White);
        }

        private void DrawGameScreen(SpriteBatch spriteBatch)
        {
            //Draw the background image for the game screen, changes depending on whether freeplay or challenge mode is selected
            if(currentGameType == EGameType.Freeplay)
            {
                spriteBatch.Draw(texturesBg[2], new Rectangle(0, 0, graphics.PreferredBackBufferWidth, graphics.PreferredBackBufferHeight), null, Color.White);
            }
            else
            {
                spriteBatch.Draw(texturesBg[3], new Rectangle(0, 0, graphics.PreferredBackBufferWidth, graphics.PreferredBackBufferHeight), null, Color.White);
            }
            //Draw the grid itself using the current puzzle type to determine which image is drawn
            gridController.DrawGrid(spriteBatch, currentPlayState, puzzles[chosenPuzzleIndex], fontTiles);

            //If the player has not yet started solving, draw the start overlay which has some info on how to get started
            if(currentPlayState == EPlayState.Ready)
            {
                spriteBatch.Draw(textureOverlayStart, new Rectangle(0, 0, graphics.PreferredBackBufferWidth, graphics.PreferredBackBufferHeight), null, Color.White);
            }
            //If the puzzle has been solved, draw the solved overlay which shows how much time was taken and how many moves were made
            else if(currentPlayState == EPlayState.Solved)
            {
                spriteBatch.Draw(textureOverlaySolved, new Rectangle(0, 0, graphics.PreferredBackBufferWidth, graphics.PreferredBackBufferHeight), null, Color.White);
            }
            //If the puzzle challenge was failed, draw the solved overlay which tells the player they failed
            else if (currentPlayState == EPlayState.Failed)
            {
                spriteBatch.Draw(textureOverlayFailed, new Rectangle(0, 0, graphics.PreferredBackBufferWidth, graphics.PreferredBackBufferHeight), null, Color.White);
            }
        }

        public static PuzzleType GetCurrentPuzzle()
        {
            return puzzles[chosenPuzzleIndex];
        }

        public static void ChangeGameType(EGameType newGameType)
        {
            //Set the current GameType to the one that was passed
            currentGameType = newGameType;

            //Update UI elements depending on the new GameTyoe
            if(newGameType == EGameType.Freeplay)
            {
                //Controls to adjust maximum time only need to be shown in Challenge mode
                ((UIButton)InputManager.GetUIElementWithName("button_game_decreaseTime")).SetVisible(false);
                ((UIButton)InputManager.GetUIElementWithName("button_game_increaseTime")).SetVisible(false);
                ((UILabel)InputManager.GetUIElementWithName("label_game_maxTime")).SetVisible(false);
            }
            else
            {
                ((UIButton)InputManager.GetUIElementWithName("button_game_decreaseTime")).SetVisible(true);
                ((UIButton)InputManager.GetUIElementWithName("button_game_increaseTime")).SetVisible(true);
                ((UILabel)InputManager.GetUIElementWithName("label_game_maxTime")).SetVisible(true);
            }
        }

        public static void ChangeGameMode(EGameMode newGameMode)
        {
            //Set the current GameMode to the one that was passed
            currentGameMode = newGameMode;

            //Ensure that the grid is setup and UI is up-to-date if switching to the game screen
            UIButton buttonAudio = (UIButton)InputManager.GetUIElementWithName("button_all_toggleAudio");
            if (currentGameMode == EGameMode.Game)
            {
                int gridSize = gridController.GetGrid().GetGridSize();
                ((UILabel)InputManager.GetUIElementWithName("label_game_gridSize")).SetText("Size: " + gridSize + "x" + gridSize);
                ChangePuzzleType(chosenPuzzleIndex);

                //Move audio button to game position
                buttonAudio.SetPosition((int)InputManager.audioButtonPositionGame.X, (int)InputManager.audioButtonPositionGame.Y);

                //Ensure the current PlayState is set to ready
                ChangePlayState(EPlayState.Ready);

            }
            else
            {
                //Move audio button to menu position
                buttonAudio.SetPosition((int)InputManager.audioButtonPositionMenu.X, (int)InputManager.audioButtonPositionMenu.Y);
            }
            //Ensure the audio button is shown in the current GameMode
            buttonAudio.SetGameModeShow(newGameMode);
        }

        public static void ChangePlayState(EPlayState newPlayState)
        {
            //Set the current PlayState to the one that was passed
            currentPlayState = newPlayState;

            if (currentPlayState == EPlayState.Ready)
            {
                //If waiting for gameplay to start, reset  the game and hide solved UI
                ((UILabel)InputManager.GetUIElementWithName("label_game_solved")).SetVisible(false);
                ResetGame();
            }
            else if (currentPlayState == EPlayState.Playing)
            {
                //If playing, start the timer and scramble grid (multiplier of 20 ensures more randomness)
                Timer.StartTimer();
                gridController.ScrambleGrid(20);
            }
            else if (currentPlayState == EPlayState.Solved)
            {
                //If the puzzle was solved, show solved UI and reset the game to prevent further grid interactions
                UILabel labelSolved = (UILabel)InputManager.GetUIElementWithName("label_game_solved");
                labelSolved.SetVisible(true);
                labelSolved.SetText("Puzzle solved!\nTotal moves: " + moveCount + "\nTime taken: " + Timer.GetTimeString(currentGameType == EGameType.Challenge));
                ResetGame();
            }
            else if (currentPlayState == EPlayState.Failed)
            {
                //If the puzzle was failed, show failed UI and reset the game to prevent further grid interactions
                UILabel labelSolved = (UILabel)InputManager.GetUIElementWithName("label_game_solved");
                labelSolved.SetVisible(true);
                labelSolved.SetText("Challenge failed!\nYou made " + moveCount + " moves");
                ResetGame();
            }
            //Trigger any further UI updates depending on the new PlayState
            InputManager.ChangeUIPlayState(currentPlayState);
        }

        //Reset all core game systems; the move count, timer and grid
        private static void ResetGame()
        {
            ResetMoveCount();
            Timer.ResetTimer(currentGameType);
            //Reset grid forces the grid back to being solved using a new instance of the grid object,
            //  but keeps the chosen puzzle type/grid size the same
            gridController.ResetGrid();
        }

        public static void IncrementPuzzleType()
        {
            //Increase the chosen puzzle type by 1, or reset back to 0 if the last puzzle was reached
            if (chosenPuzzleIndex < puzzles.Length - 1)
            {
                ChangePuzzleType(chosenPuzzleIndex + 1);
            }
            else
            {
                ChangePuzzleType(0);
            }
        }
        public static void DecrementPuzzleType()
        {
            //Decrease the chosen puzzle type by 1, or go to the final puzzle if 0 was reached
            if (chosenPuzzleIndex > 0)
            {
                ChangePuzzleType(chosenPuzzleIndex - 1);
            }
            else
            {
                ChangePuzzleType(puzzles.Length - 1);
            }
        }

        public static void ChangePuzzleType(int index)
        {
            //Set the puzzle type to the passed index and update UI text to reflect this change
            chosenPuzzleIndex = index;
            ((UILabel)InputManager.GetUIElementWithName("label_game_patternName")).SetText("Style: \"" + Game1.GetCurrentPuzzle().name + "\"");
        }

        public static void IncreaseMoveCount()
        {
            //Add a move to the counter and update UI text to reflect this change
            moveCount++;
            ((UILabel)InputManager.GetUIElementWithName("label_game_moves")).SetText(moveCount.ToString());
        }
        public static void ResetMoveCount()
        {
            //Set the move counter back to 0 and update UI text to reflect this change
            moveCount = 0;
            ((UILabel)InputManager.GetUIElementWithName("label_game_moves")).SetText(moveCount.ToString());
        }
    }
}
