using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace _15TileGame
{
    public class GridController
    {
        private Grid m_grid;                    //The grid object that contains the tiles
        private readonly int m_iScreenWidth;    //The width of the game window
        private readonly int m_iScreenHeight;   //The height of the game window

        private Tile m_effectTile;                              //The tile that was last clicked that should have an effect applied (e.g. shake)
        private float m_fTileShakeAmount;                       //The intensity of the tile shake effect on the current frame
        private bool m_bShowTileNumbers = false;                //Whether or not to draw numbers on each tile

        private const int m_iMaxGridSize = 10;                  //The maximum possible size the player can set the grid to
        private const int m_iMinGridSize = 2;                   //The minimum possible size the player can set the grid to
        private const int m_iGridDimensions = 500;              //The size of the grid in pixels, used for drawing
        private const int m_iGridSpacing = 2;                   //The amount of space between tiles in pixels

        private const float m_fMaxTileShake = 5f;               //The maximum intensity of the tile shake effect
        private const float m_fAnimationSpeed = 10f;            //The default speed for tile sliding animations
        private const float m_fTileShakeReductionSpeed = 18f;   //How quickly the tile shake effect lowers in intensity each frame

        private readonly Color m_colourTileHighlight = new Color(0.85f, 0.9f, 0.8f);     //Colour for drawing highlighted tiles


        //=====================\\
        //  Getters & setters  \\
        //=====================\\
        public Grid GetGrid() { return m_grid; }
        public void SetEffectTile(Tile tile)
        {
            //Resetting ensures any existing tile with an effect applied
            //  will not continue to have this effect after a new tile is clicked
            ResetTileShake();
            m_effectTile = tile;
        }
        public void SetShowTileNumbers(bool showTileNumbers) { m_bShowTileNumbers = showTileNumbers; }
        public bool GetShowTileNumbers() { return m_bShowTileNumbers; }

        //Constructor
        public GridController(int screenWidth, int screenHeight)
        {
            m_iScreenWidth = screenWidth;
            m_iScreenHeight = screenHeight;

            //Create a new grid object with size 4x4, 500px width/height and 2px spacing
            m_grid = new Grid(4, m_iGridDimensions, m_iGridSpacing);
            m_grid.SetupGrid(screenWidth, screenHeight);
        }

        //Destructor
        ~GridController()
        { }


        public bool MoveTileToBlankSpace(Vector2 moveDirection, bool playerMove = true)
        {
            //Returns true/false depending on if the move was possible (i.e. not out of movement bounds)

            //Get the position of the blank tile and tile we are trying to move
            Vector2 blankSpaceGridPos = m_grid.GetBlankTile().GetGridCurrentPosition();
            Vector2 targetTileGridPos = blankSpaceGridPos + moveDirection;

            //Check tile array bounds
            if(targetTileGridPos.X >= 0 && targetTileGridPos.X < m_grid.GetGridSize() &&
                targetTileGridPos.Y >= 0 && targetTileGridPos.Y < m_grid.GetGridSize())
            {
                //Swap the chosen tile with the blank space
                m_grid.SwapTiles(blankSpaceGridPos, targetTileGridPos);
                //If the player started this move (i.e. not triggered by code such as scramble), play a sound and increase move count
                if (playerMove)
                {
                    Game1.IncreaseMoveCount();
                    AudioManager.PlaySoundEffectWithName("tileMove");
                }
                return true;
            }
            return false;
        }

        public void ScrambleGrid(int scrambleMultiplier)
        {
            //The number of times to scramble is dependent on grid size an the passed multiplier value
            int scrambleCount = m_grid.GetGridSize() * m_grid.GetGridSize() * scrambleMultiplier;

            //Cardinal directions used for scrambling
            Vector2[] possibleDirections = new Vector2[]
                { new Vector2(0, 1), new Vector2(0, -1), new Vector2(1, 0), new Vector2(-1, 0) };

            Random rand = new Random();

            //Valid move ensures each movement in the scramble process is valid and not a wasted attempt
            bool validMove;
            for (int i = 0; i < scrambleCount; i++)
            {
                validMove = false;
                while (!validMove)
                {
                    //Continue trying to move a tile in a randomly chosen direction until a valid move was made
                    Vector2 chosenDirection = possibleDirections[rand.Next(0, possibleDirections.Length)];
                    validMove = MoveTileToBlankSpace(chosenDirection, false);
                }
                if(i == scrambleCount - 1)
                {
                    //Check if grid is still unscrambled at the last loop, if it is increase scramblecount
                    //  to ensure the final result is a scrambled grid
                    if (m_grid.IsGridSolved())
                        scrambleCount++;
                }
            }

            //Play a tile break sound to enhance the scramble effect
            AudioManager.PlaySoundEffectWithName("tileBreak");
        }

        public void ResetGrid()
        {
            //Create/setup a new grid object with the current size, dimensions and spacing to reset the object
            m_grid = new Grid(m_grid.GetGridSize(), m_iGridDimensions, m_iGridSpacing);
            m_grid.SetupGrid(m_iScreenWidth, m_iScreenHeight);
        }

        public int IncreaseGridSize()
        {
            //If below the maximum grid size, create/setup a new grid object with a larger size than the current one
            if(m_grid.GetGridSize() < m_iMaxGridSize)
            {
                m_grid = new Grid(m_grid.GetGridSize() + 1, m_iGridDimensions, m_iGridSpacing);
                m_grid.SetupGrid(m_iScreenWidth, m_iScreenHeight);
            }
            //Return the size of the new grid
            return m_grid.GetGridSize();
        }
        public int DecreaseGridSize()
        {
            //If above the minumum grid size, create/setup a new grid object with a smaller size than the current one
            if (m_grid.GetGridSize() > m_iMinGridSize)
            {
                m_grid = new Grid(m_grid.GetGridSize() - 1, m_iGridDimensions, m_iGridSpacing);
                m_grid.SetupGrid(m_iScreenWidth, m_iScreenHeight);
            }
            //Return the size of the new grid
            return m_grid.GetGridSize();
        }

        private void ResetTileShake()
        {
            //Remove effects from the current effect tile
            //  if one exists, and set the effect tile back to null
            if(m_effectTile != null)
            {
                m_effectTile.RemoveEffects();
                m_effectTile = null;
            }
            //Reset the tile shake amount so it doesn't carry over
            m_fTileShakeAmount = m_fMaxTileShake;
        }

        public void Update(EPlayState currentPlayState, EGameType currentGameType, float deltaTime)
        {
            if (currentPlayState == EPlayState.Playing)
            {
                //If playing, check if the grid is solved. If so,
                //  change the PlayState to solved & play a nice sound
                if (m_grid.IsGridSolved())
                {
                    Game1.ChangePlayState(EPlayState.Solved);
                    AudioManager.PlaySoundEffectWithName("puzzleSolved");
                }
                else if (currentGameType == EGameType.Challenge)
                {
                    //If in challenge mode and the timer value is less than 1 (displayed as 00:00 in UI),
                    //  change PlayState to failed and play an alarm sound
                    if(Timer.GetTimerValue() < 1f)
                    {
                        Game1.ChangePlayState(EPlayState.Failed);
                        AudioManager.PlaySoundEffectWithName("tileCannotMove");
                    }
                }
            }

            if (m_effectTile != null)
            {
                //If there is an effectTile, update its shake effect
                m_effectTile.UpdateShakeEffect(m_fTileShakeAmount, m_fMaxTileShake);
                //Reduce shake amount so the shake reduces in intensity over time
                m_fTileShakeAmount -= deltaTime * m_fTileShakeReductionSpeed;
                //Reset the tile shake effect if the shakeAmount has reached 0
                if (m_fTileShakeAmount <= 0)
                {
                    ResetTileShake();
                }
            }

            //Tile movement animations
            int gridSize = m_grid.GetGridSize();
            Tile[,] tileArray = m_grid.GetTiles();
            for (int x = 0; x < gridSize; x++)
            {
                for (int y = 0; y < gridSize; y++)
                {
                    //Get references to the current tiles's current and target positions,
                    //  these will be needed for animating tile movement
                    Tile currTile = tileArray[x, y];
                    Vector2 currentPos = currTile.GetCurrentVisualPosition();
                    Vector2 targetPos = currTile.GetOverallVisualPosition();

                    //If the tile is very close to its target position,
                    //  snap to the targetPos to remove extremely slow/lingering movements
                    if(Vector2.Distance(currentPos, targetPos) < 1f)
                    {
                        currTile.SetCurrentVisualPosition(targetPos);
                    }
                    else
                    {
                        //The animation speed is set to a high number if the current tile is shaking
                        //  to ensure the shake animation is not too smooth
                        float moveSpeed;
                        if (currTile == m_effectTile)
                        {
                            moveSpeed = 100f;
                        }
                        else
                        {
                            moveSpeed = m_fAnimationSpeed;
                        }
                        //Interpolate between the current tile position and target position,
                        //  using deltaTile to ensure consistency across different framerates
                        currTile.SetCurrentVisualPosition(Vector2.Lerp(currentPos, targetPos, deltaTime * moveSpeed));
                    }
                }
            }
        }

        public void DrawGrid(SpriteBatch spriteBatch, EPlayState currentPlayState, PuzzleType puzzle, SpriteFont font)
        {
            //Get grid size and the tile array, these will determine how the grid is drawn
            int gridSize = m_grid.GetGridSize();
            Tile[,] tilesArray = m_grid.GetTiles();

            //Loop through all tiles in the 2D array
            for (int loopX = 0; loopX < gridSize; loopX++)
            {
                for (int loopY = 0; loopY < gridSize; loopY++)
                {
                    Tile currentTile = tilesArray[loopX, loopY];
                    //Get the size of texture needed for this part of the grid
                    float textureSegmentSize = puzzle.texture.Width / gridSize;
                    Rectangle tileDrawRect = currentTile.GetStandardRectangle(m_grid);
                    //The x/y positions are calculated using the current tile's starting position in the grid
                    //  so the correct part of the texture is drawn, regardless of the tile's current position
                    Rectangle textureSegmentRect = new Rectangle((int)(textureSegmentSize * currentTile.GetGridStartPosition().X), (int)(textureSegmentSize * currentTile.GetGridStartPosition().Y), (int)textureSegmentSize, (int)textureSegmentSize);

                    //Only draw non-blank tiles unless the grid was solved/challenge was failed
                    if (!currentTile.GetIsBlankSpace() || currentPlayState == EPlayState.Solved || currentPlayState == EPlayState.Failed)
                    {
                        //tileDrawColour is used for certain effects such as a flashing red tile
                        Color tileDrawColour = currentTile.GetColourTint();
                        //If highlighted and no other colour effects are applied, highlight the tile
                        if(currentTile.GetHighlighted() && tileDrawColour == Color.White)
                        {
                            tileDrawColour = m_colourTileHighlight;
                        }
                        //Draw the current tile image segment with the chosen puzzle texture, using the colour/position determined above
                        spriteBatch.Draw(puzzle.texture, tileDrawRect, textureSegmentRect, tileDrawColour, 0f,
                                            Vector2.Zero, SpriteEffects.None, 0f);
                        //Also draw text on the tile if showTileNumbers is enabled
                        if (m_bShowTileNumbers)
                        {
                            //Text is drawn at the top-left corner of a tile with an offset of (10, 2)
                            Vector2 textDrawPos = new Vector2(tileDrawRect.X + 10, tileDrawRect.Y + 2);
                            spriteBatch.DrawString(font, currentTile.GetTileNumber().ToString(), textDrawPos, Color.White);
                        }
                    }
                }
            }
        }

    }
}
