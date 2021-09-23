using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace _15TileGame
{
    public static class InputManager
    {
        private static MouseState currentMouseState;    //State of the mouse on the current frame
        private static MouseState previousMouseState;   //State of the mouse on the previous frame
        private static KeyboardState currentKeyState;   //State of the keyboard on the current frame
        private static KeyboardState previousKeyState;  //State of the keyboard on the previous frame
        private static bool mouseLeftClicked;           //Whether or not the mouse was left clicked on hte current frame
        private static bool invertKeyControls;          //Should the keyboard controls be inverted from their default state

        private static UIElement[] uiElements;          //Array of all the UI elements in the game
        private static UITooltip tooltip;               //The tooltip object used to show extra info

        private static Texture2D textureButtonNormal;           //Texture used for buttons in their default state
        private static Texture2D textureButtonReturn;           //Texture for button used to return to menu
        private static Texture2D[] texturesButtonInvert;        //2 textures (inverted/non-inverted) - used for the button that inverts controls
        private static Texture2D[] texturesButtonSound;         //2 textures (sound on/muted) - used for the button that toggles audio
        private static Texture2D[] texturesButtonTileNumbers;   //2 textures (tile numbers on/off) - used for the button that toggles numbers on tiles
        private static Texture2D textureButtonHover;            //Texture for buttons when the mouse pointer is over them
        private static Texture2D textureButtonDisabled;         //Texture for buttons when they cannot be clicked
        private static Texture2D textureTooltip;                //Texture for the background of the tooltip
        private static SpriteFont fontLarge;                    //The default large font used in the UI
        private static SpriteFont fontSmall;                    //The default small font used in the UI

        public static readonly Vector2 audioButtonPositionMenu = new Vector2(10, 10);  //The position of the audio button when in the main menu/info screen
        public static readonly Vector2 audioButtonPositionGame = new Vector2(80, 10);  //The position of the audio button when playing
        private static readonly Color colourFontSecondary = new Color(211, 202, 201);  //The secondary colour used for UI text 

        public static UITooltip GetTooltip()
        {
            return tooltip;
        }

        public static void LoadUIContent(ContentManager content)
        {
            //Load the standard button textures and fill the array
            textureButtonNormal = content.Load<Texture2D>("Images/buttonNormal");
            //Load the texture for the 'Return to Menu' button
            textureButtonReturn = content.Load<Texture2D>("Images/buttonReturn");
            //Load the textures for the 'Invert Keyboard Controls' button
            texturesButtonInvert = new Texture2D[]
            {
                content.Load<Texture2D>("Images/buttonInvert"),
                content.Load<Texture2D>("Images/buttonInvert2")
            };
            //Load the textures for the 'Toggle Audio' button
            texturesButtonSound = new Texture2D[]
            {
                content.Load<Texture2D>("Images/buttonSound"),
                content.Load<Texture2D>("Images/buttonSound2")
            };
            //Load the textures for the 'Toggle Tile Numbers' button
            texturesButtonTileNumbers = new Texture2D[]
            {
                content.Load<Texture2D>("Images/buttonTileNumbers"),
                content.Load<Texture2D>("Images/buttonTileNumbers2")
            };
            //Load the other button textures
            textureButtonHover = content.Load<Texture2D>("Images/buttonHover");
            textureButtonDisabled = content.Load<Texture2D>("Images/buttonDisabled");
            textureTooltip = content.Load<Texture2D>("Images/tooltip");
            //Load the fonts that will be used for UI
            fontLarge = content.Load<SpriteFont>("Fonts/fontMain");
            fontSmall = content.Load<SpriteFont>("Fonts/fontSmall");

            //Create all of the UI elements that are needed for the game
            SetupUIElements();
        }

        public static void SetupUIElements()
        {
            //Populate the uiElements array with the game's UI objects
            uiElements = new UIElement[]
            {
                //Menu buttons
                new UIButton("button_menu_modeFreeplay", 430, 485, 207, 45, true, EGameMode.Menu, ButtonMenuModeFreeplay_Click, "Freeplay Mode",
                            fontSmall, textureButtonHover, textureButtonHover, textureButtonDisabled, Color.White, true, "Solve tile puzzles\nwithout a time limit", false),
                new UIButton("button_menu_modeChallenge", 643, 485, 207, 45, true, EGameMode.Menu, ButtonMenuModeChallenge_Click, "Challenge Mode",
                            fontSmall, textureButtonNormal, textureButtonHover, textureButtonDisabled, colourFontSecondary, true, "Solve tile puzzles\nagainst the clock", false),
                new UIButton("button_menu_play", 430, 535, 420, 91, true, EGameMode.Menu, ButtonMenuPlay_Click, "Play", 
                            fontLarge, textureButtonNormal, textureButtonHover, textureButtonDisabled, Color.White),

                //Info screen buttons
                new UIButton("button_info_continue", 950, 580, 298, 81, true, EGameMode.Info, ButtonInfoContinue_Click, "Continue",
                            fontLarge, textureButtonNormal, textureButtonHover, textureButtonDisabled, Color.White),

                //Game buttons
                new UIButton("button_game_start", 932, 228, 124, 58, true, EGameMode.Game, ButtonGameStart_Click, "Start",
                            fontSmall, textureButtonNormal, textureButtonHover, textureButtonDisabled, Color.White),
                new UIButton("button_game_prevPattern", 939, 421, 40, 37, true, EGameMode.Game, ButtonGamePrevPattern_Click, "<",
                            fontSmall, textureButtonNormal, textureButtonHover, textureButtonDisabled, Color.White),
                new UIButton("button_game_nextPattern", 1010, 421, 40, 37, true, EGameMode.Game, ButtonGameNextPattern_Click, ">",
                            fontSmall, textureButtonNormal, textureButtonHover, textureButtonDisabled, Color.White),
                new UIButton("button_game_decreaseGrid", 939, 520, 40, 37, true, EGameMode.Game, ButtonGameDecreaseGrid_Click, "<",
                            fontSmall, textureButtonNormal, textureButtonHover, textureButtonDisabled, Color.White),
                new UIButton("button_game_increaseGrid", 1010, 520, 40, 37, true, EGameMode.Game, ButtonGameIncreaseGrid_Click, ">",
                            fontSmall, textureButtonNormal, textureButtonHover, textureButtonDisabled, Color.White),
                new UIButton("button_game_decreaseTime", 10, 120, 40, 40, true, EGameMode.Game, ButtonGameDecreaseTime_Click, "<",
                            fontSmall, textureButtonNormal, textureButtonHover, textureButtonDisabled, Color.White),
                new UIButton("button_game_increaseTime", 170, 120, 40, 40, true, EGameMode.Game, ButtonGameIncreaseTime_Click, ">",
                            fontSmall, textureButtonNormal, textureButtonHover, textureButtonDisabled, Color.White),
                new UIButton("button_game_returnToMenu", 10, 10, 60, 60, true, EGameMode.Game, ButtonGameReturnToMenu_Click, "",
                            fontSmall, textureButtonReturn, textureButtonReturn, textureButtonDisabled, Color.White, true, "Return to Menu"),
                new UIButton("button_game_invertControls", 150, 10, 60, 60, true, EGameMode.Game, ButtonGameInvertControls_Click, "",
                            fontSmall, texturesButtonInvert[0], texturesButtonInvert[0], textureButtonDisabled, Color.White, true, "Invert Keyboard\nControls"),
                new UIButton("button_game_toggleTileNumbers", 220, 10, 60, 60, true, EGameMode.Game, ButtonGameToggleTileNumbers_Click, "",
                            fontSmall, texturesButtonTileNumbers[1], texturesButtonTileNumbers[1], textureButtonDisabled, Color.White, true, "Toggle Tile Numbers"),
                
                //Buttons shown in all GameModes
                new UIButton("button_all_toggleAudio", (int)audioButtonPositionMenu.X, (int)audioButtonPositionMenu.Y, 60, 60, true, EGameMode.Menu, ButtonAllToggleAudio_Click, "",
                            fontSmall, texturesButtonSound[0], texturesButtonSound[0], textureButtonDisabled, Color.White, true, "Toggle Audio"),

                //Game labels
                new UILabel("label_game_gridSize", 932, 476, 124, 44, true, EGameMode.Game, null, "", fontSmall, Color.Black),
                new UILabel("label_game_patternName", 361, 38, 561, 56, true, EGameMode.Game, null, "", fontSmall, Color.White),
                new UILabel("label_game_moves", 933, 78, 123, 50, true, EGameMode.Game, null, "0", fontSmall, Color.Black),
                new UILabel("label_game_timer", 933, 167, 123, 50, true, EGameMode.Game, null, "00:00", fontSmall, Color.Black),
                new UILabel("label_game_solved", 361, 638, 560, 82, false, EGameMode.Game, null, "", fontSmall, Color.White),
                new UILabel("label_game_maxTime", 50, 118, 120, 40, true, EGameMode.Game, null, "01:00", fontSmall, Color.Black)
            };

            //Create a tooltip object that will be used to display extra info when hovering over certain UI elements
            tooltip = new UITooltip(textureTooltip, fontSmall);
        }

        public static void Update(EGameMode previousGameMode, EGameMode currentGameMode, EPlayState currentPlayState, GridController gridController)
        {
            //Get the current state of the keyboard and mouse
            currentMouseState = Mouse.GetState();
            currentKeyState = Keyboard.GetState();
            //Check if left mouse button was pressed last frame then released, i.e. the player left clicked
            mouseLeftClicked = (previousMouseState.LeftButton == ButtonState.Pressed && currentMouseState.LeftButton == ButtonState.Released);

            //Update all UI elements
            for (int i = 0; i < uiElements.Length; i++)
            {
                uiElements[i].Update(previousGameMode, currentGameMode, mouseLeftClicked, currentMouseState.X, currentMouseState.Y);
            }
            //Also update the tooltip
            tooltip.Update(currentMouseState.X, currentMouseState.Y);

            //If the player is currently trying to solve the puzzle
            if (currentGameMode == EGameMode.Game && currentPlayState == EPlayState.Playing)
            {
                //Get the tile array and position of the blank tile on this frame
                Grid grid = gridController.GetGrid();
                Tile[,] tiles = grid.GetTiles();
                Vector2 blankTilePos = gridController.GetGrid().GetBlankTile().GetGridCurrentPosition();

                //Loop through each tile in the grid
                for (int x = 0; x < gridController.GetGrid().GetGridSize(); x++)
                {
                    for (int y = 0; y < gridController.GetGrid().GetGridSize(); y++)
                    {
                        //No need to do checks for the blank tile, so ignore its position
                        if (!(x == blankTilePos.X && y == blankTilePos.Y))
                        {
                            //Reset tile highlights so they do not persist after the mouse was moved
                            tiles[x, y].SetHighlighted(false);

                            //If the mouse is over the current tile
                            if (tiles[x, y].GetStandardRectangle(grid).Contains(currentMouseState.X, currentMouseState.Y))
                            {
                                //Mouse click tile move checks
                                if (mouseLeftClicked)
                                {
                                    Vector2 clickedTilePos = tiles[x, y].GetGridCurrentPosition();
                                    //The clicked tile is to the right of the blank tile
                                    if (clickedTilePos.X - 1 == blankTilePos.X && clickedTilePos.Y == blankTilePos.Y)
                                    {
                                        gridController.MoveTileToBlankSpace(new Vector2(1, 0), true);
                                    }
                                    //The clicked tile is to the left of the blank tile
                                    else if (clickedTilePos.X + 1 == blankTilePos.X && clickedTilePos.Y == blankTilePos.Y)
                                    {
                                        gridController.MoveTileToBlankSpace(new Vector2(-1, 0), true);
                                    }
                                    //The clicked tile is above the blank tile
                                    else if (clickedTilePos.X == blankTilePos.X && clickedTilePos.Y + 1 == blankTilePos.Y)
                                    {
                                        gridController.MoveTileToBlankSpace(new Vector2(0, -1), true);
                                    }
                                    //The clicked tile is below the blank tile
                                    else if (clickedTilePos.X == blankTilePos.X && clickedTilePos.Y - 1 == blankTilePos.Y)
                                    {
                                        gridController.MoveTileToBlankSpace(new Vector2(0, 1), true);
                                    }
                                    //The clicked tile cannot be moved - start shake effect and play sound
                                    else
                                    {
                                        gridController.SetEffectTile(tiles[x, y]);
                                        AudioManager.PlaySoundEffectWithName("tileCannotMove");
                                    }
                                }
                                //Hovering checks
                                else
                                {
                                    tiles[x, y].SetHighlighted(true);
                                }
                            }
                        }
                    }
                }

                //If up arrow key was pressed, move a tile up/down depending on if controls are inverted
                if (previousKeyState.IsKeyDown(Keys.Up) && currentKeyState.IsKeyUp(Keys.Up))
                {
                    gridController.MoveTileToBlankSpace(new Vector2(0, 1) * (invertKeyControls ? -1 : 1));
                }
                //If down arrow key was pressed, move a tile down/up depending on if controls are inverted
                if (previousKeyState.IsKeyDown(Keys.Down) && currentKeyState.IsKeyUp(Keys.Down))
                {
                    gridController.MoveTileToBlankSpace(new Vector2(0, -1) * (invertKeyControls ? -1 : 1));
                }
                //If left arrow key was pressed, move a tile right/left depending on if controls are inverted
                if (previousKeyState.IsKeyDown(Keys.Left) && currentKeyState.IsKeyUp(Keys.Left))
                {
                    gridController.MoveTileToBlankSpace(new Vector2(1, 0) * (invertKeyControls ? -1 : 1));
                }
                //If right arrow key was pressed, move a tile left/right depending on if controls are inverted
                if (previousKeyState.IsKeyDown(Keys.Right) && currentKeyState.IsKeyUp(Keys.Right))
                {
                    gridController.MoveTileToBlankSpace(new Vector2(-1, 0) * (invertKeyControls ? -1 : 1));
                }
            }

            //Update the previous mouse and keyboard states so they are correct for the next update loop
            previousMouseState = currentMouseState;
            previousKeyState = currentKeyState;
        }

        public static void DrawUI(SpriteBatch spriteBatch, EGameMode currentGameMode, Texture2D currentPuzzleTexture)
        {
            //If on the game screen, draw a preview of the puzzle that is currently selected
            if(currentGameMode == EGameMode.Game)
            {
                spriteBatch.Draw(currentPuzzleTexture, new Rectangle(939, 305, 111, 111), null, Color.White, 0f, Vector2.Zero, SpriteEffects.None, 1f);
            }
            for (int i = 0; i < uiElements.Length; i++)
            {
                //Draw each UI element if they are meant to be shown in the current GameMode
                UIElement currentElement = uiElements[i];
                if (currentElement.GetGameModeShow() == currentGameMode)
                {
                    currentElement.Draw(spriteBatch);
                }
            }

            //Draw the tooltip (will only be shown if hovering over certain buttons)
            tooltip.Draw(spriteBatch);
        }

        public static void ChangeUIPlayState(EPlayState currentPlayState)
        {
            //If the player is solving the puzzle, disable certain options in the UI
            //  and set play button text to 'Reset'
            if(currentPlayState == EPlayState.Playing)
            {
                ((UIButton)GetUIElementWithName("button_game_start")).SetText("Reset");
                ((UIButton)GetUIElementWithName("button_game_prevPattern")).SetEnabled(false);
                ((UIButton)GetUIElementWithName("button_game_nextPattern")).SetEnabled(false);
                ((UIButton)GetUIElementWithName("button_game_increaseGrid")).SetEnabled(false);
                ((UIButton)GetUIElementWithName("button_game_decreaseGrid")).SetEnabled(false);
                ((UIButton)GetUIElementWithName("button_game_increaseTime")).SetEnabled(false);
                ((UIButton)GetUIElementWithName("button_game_decreaseTime")).SetEnabled(false);
            }
            else
            {
                //If the player is waiting to start solving, set play button text to 'Start'
                if (currentPlayState == EPlayState.Ready)
                {
                    ((UIButton)GetUIElementWithName("button_game_start")).SetText("Start");
                }
                //Otherwise, if they have solved the puzzle, set play button text to 'Play again'
                else
                {
                    ((UIButton)GetUIElementWithName("button_game_start")).SetText("Play again");
                }
                //Also ensure all options are enabled in the UI
                ((UIButton)GetUIElementWithName("button_game_prevPattern")).SetEnabled(true);
                ((UIButton)GetUIElementWithName("button_game_nextPattern")).SetEnabled(true);
                ((UIButton)GetUIElementWithName("button_game_increaseGrid")).SetEnabled(true);
                ((UIButton)GetUIElementWithName("button_game_decreaseGrid")).SetEnabled(true);
                ((UIButton)GetUIElementWithName("button_game_increaseTime")).SetEnabled(true);
                ((UIButton)GetUIElementWithName("button_game_decreaseTime")).SetEnabled(true);
            }
        }

        public static UIElement GetUIElementWithName(string name)
        {
            //Loop through the array and find a UI element with the passed name
            for (int i = 0; i < uiElements.Length; i++)
            {
                if(uiElements[i].GetName() == name)
                {
                    //If an element with the specified name was found, return it
                    return uiElements[i];
                }
            }
            //If no elements are found (element with name does not exist), return null
            return null;
        }

        private static void ButtonMenuPlay_Click(UIElement element)
        {
            //The play button switches to the info screen
            Game1.ChangeGameMode(EGameMode.Info);
        }
        private static void ButtonMenuModeFreeplay_Click(UIElement element)
        {
            ((UIButton)element).SetStyle(textureButtonHover, Color.White);
            ((UIButton)GetUIElementWithName("button_menu_modeChallenge")).SetStyle(textureButtonNormal, colourFontSecondary);
            Game1.ChangeGameType(EGameType.Freeplay);
        }
        private static void ButtonMenuModeChallenge_Click(UIElement element)
        {
            ((UIButton)element).SetStyle(textureButtonHover, Color.White);
            ((UIButton)GetUIElementWithName("button_menu_modeFreeplay")).SetStyle(textureButtonNormal, colourFontSecondary);
            Game1.ChangeGameType(EGameType.Challenge);
        }
        private static void ButtonInfoContinue_Click(UIElement element)
        {
            //The info continue button switches to the game screen
            Game1.ChangeGameMode(EGameMode.Game);
        }
        private static void ButtonGameStart_Click(UIElement element)
        {
            //The start button switches the PlayState to either ready or playing depending on its current state
            if (Game1.currentPlayState == EPlayState.Ready)
            {
                Game1.ChangePlayState(EPlayState.Playing);
            }
            else
            {
                Game1.ChangePlayState(EPlayState.Ready);
            }
        }
        private static void ButtonGamePrevPattern_Click(UIElement element)
        {
            //The previous pattern button decrements the current puzzle type
            //  and ensures PlayState is set to ready to avoid this changing while playing
            Game1.DecrementPuzzleType();
            Game1.ChangePlayState(EPlayState.Ready);
        }
        private static void ButtonGameNextPattern_Click(UIElement element)
        {
            //The next pattern button increments the current puzzle type
            //  and ensures PlayState is set to ready to avoid this changing while playing
            Game1.IncrementPuzzleType();
            Game1.ChangePlayState(EPlayState.Ready);
        }
        private static void ButtonGameDecreaseGrid_Click(UIElement element)
        {
            //The decrease grid size button makes the grid smaller and updates relevant UI text to show the correct size,
            //  and ensures PlayState is set to ready to avoid this changing while playing
            int gridSize = Game1.gridController.DecreaseGridSize();
            ((UILabel)GetUIElementWithName("label_game_gridSize")).SetText("Size: " + gridSize + "x" + gridSize);
            Game1.ChangePlayState(EPlayState.Ready);
        }
        private static void ButtonGameIncreaseGrid_Click(UIElement element)
        {
            // The increase grid size button makes the grid larger and updates relevant UI text to show the correct size,
            //  and ensures PlayState is set to ready to avoid this changing while playing
            int gridSize = Game1.gridController.IncreaseGridSize();
            ((UILabel)GetUIElementWithName("label_game_gridSize")).SetText("Size: " + gridSize + "x" + gridSize);
            Game1.ChangePlayState(EPlayState.Ready);
        }
        private static void ButtonGameInvertControls_Click(UIElement element)
        {
            //The invert controls button flips the invertKeyControls bool so arrow key controls are swapped
            invertKeyControls = !invertKeyControls;
            //Toggle between button textures to show the current button state
            if (invertKeyControls)
            {
                ((UIButton)element).SetTextures(texturesButtonInvert[1]);
            }
            else
            {
                ((UIButton)element).SetTextures(texturesButtonInvert[0]);
            }
        }
        private static void ButtonGameToggleTileNumbers_Click(UIElement element)
        {
            //The toggle numbers button controls whether number text should be drawn on top of tiles
            Game1.gridController.SetShowTileNumbers(!Game1.gridController.GetShowTileNumbers());
            //Toggle between button textures to show the current button state
            if (Game1.gridController.GetShowTileNumbers())
            {
                ((UIButton)element).SetTextures(texturesButtonTileNumbers[0]);
            }
            else
            {
                ((UIButton)element).SetTextures(texturesButtonTileNumbers[1]);
            }
        }
        private static void ButtonGameReturnToMenu_Click(UIElement element)
        {
            //The return to menu button ensures PlayState is set to ready in case the player quits mid-game,
            //  and changes GameMode to go to the menu screen
            Game1.ChangePlayState(EPlayState.Ready);
            Game1.ChangeGameMode(EGameMode.Menu);
        }
        private static void ButtonGameDecreaseTime_Click(UIElement element)
        {
            //Decrease the maximum challenge time and reset the timer
            Timer.ChangeMaxChallengeTime(false);
            Timer.ResetTimer(EGameType.Challenge);
            //Update UI to reflect changes
            ((UILabel)GetUIElementWithName("label_game_maxTime")).SetText(Timer.GetTimeString());
            Timer.UpdateLabelText();
            //Reset PlayState to ready to avoid this changing while playing
            Game1.ChangePlayState(EPlayState.Ready);
        }
        private static void ButtonGameIncreaseTime_Click(UIElement element)
        {
            //Increase the maximum challenge time and reset the timer
            Timer.ChangeMaxChallengeTime(true);
            Timer.ResetTimer(EGameType.Challenge);
            //Update UI to reflect changes
            ((UILabel)GetUIElementWithName("label_game_maxTime")).SetText(Timer.GetTimeString());
            Timer.UpdateLabelText();
            //Reset PlayState to ready to avoid this changing while playing
            Game1.ChangePlayState(EPlayState.Ready);
        }
        private static void ButtonAllToggleAudio_Click(UIElement element)
        {
            //Turn sounds and music on/off
            bool muted = AudioManager.ToggleAudio();
            //Update button textures to reflect whether audio is muted
            if (muted)
            {
                ((UIButton)element).SetTextures(texturesButtonSound[1]);
            }
            else
            {
                ((UIButton)element).SetTextures(texturesButtonSound[0]);
            }
        }
    }
}
