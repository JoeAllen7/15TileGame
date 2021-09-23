using Microsoft.Xna.Framework;

namespace _15TileGame
{
    public class Grid
    {
        private int m_iGridSize;            //The number of tiles for each dimension of the grid (e.g. grid size of 3 would be a 3x3 grid of tiles)
        private int m_iFullGridDimensions;  //The visual size of the width and height of the entire grid in pixels when drawn
        private int m_iTileSpacing;         //The amount of empty space between each grid tile in pixels
        private Tile[,] m_tiles;            //A 2D array of tile objects that make up the grid
        private Tile m_blankTile;           //The tile object that acts as an empty space


        //=====================\\
        //  Getters & setters  \\
        //=====================\\
        public int GetGridSize() { return m_iGridSize; }
        public int GetFullGridDimensions() { return m_iFullGridDimensions; }
        public int GetTileSpacing() { return m_iTileSpacing; }
        public Tile GetBlankTile() { return m_blankTile; }
        public Tile[,] GetTiles() { return m_tiles; }


        //Constructor
        public Grid(int gridSize, int fullGridDimensions, int tileSpacing)
        {
            m_iGridSize = gridSize;
            m_iFullGridDimensions = fullGridDimensions;
            m_iTileSpacing = tileSpacing;
        }

        //Destructor
        ~Grid()
        { }


        public void SwapTiles(Vector2 tile1Index, Vector2 tile2Index)
        {
            //Get both of the tile objects ar the grid positions that were passed
            Tile tile1 = m_tiles[(int)tile1Index.X, (int)tile1Index.Y];
            Tile tile2 = m_tiles[(int)tile2Index.X, (int)tile2Index.Y];

            //Get some temporary info from tile 1 before it is updated
            Vector2 tempTile1Pos = tile1.GetOverallVisualPosition();     //Visual position that the tile should reach
            Vector2 tempTile1GridPos = tile1.GetGridCurrentPosition();  //Current 2D grid position of the tile

            //Swap the visual and grid positions of each tile
            tile1.SetTargetVisualPosition(tile2.GetOverallVisualPosition(), false);
            tile1.SetGridCurrentPosition(tile2.GetGridCurrentPosition());
            //For tile 2, the temp variables are used since tile 1 was already updated
            tile2.SetTargetVisualPosition(tempTile1Pos, false);
            tile2.SetGridCurrentPosition(tempTile1GridPos);

            //Swap the positions of each tile in the tiles array so there is not a disconnect 
            //  between where they think they are, and where the rest of the game considers them to be
            m_tiles[(int)tile1Index.X, (int)tile1Index.Y] = tile2;
            m_tiles[(int)tile2Index.X, (int)tile2Index.Y] = tile1;
        }

        public void SetupGrid(int screenWidth, int screenHeight)
        {
            //Calculate each tile's width/height (in pixels)
            int tileDimensions = (m_iFullGridDimensions / m_iGridSize) - m_iTileSpacing;
            //Find the position at screen centre so the grid can be centered
            Vector2 centreScreenPos = new Vector2(screenWidth / 2f, screenHeight / 2f);

            m_tiles = new Tile[m_iGridSize, m_iGridSize];
            //Loop through each tile position in the grid
            for (int x = 0; x < m_iGridSize; x++)
            {
                for (int y = 0; y < m_iGridSize; y++)
                {
                    //Get a visual position for where tiles at this position will be drawn
                    Vector2 currentTilePos = new Vector2(centreScreenPos.X + x * tileDimensions + x * m_iTileSpacing,
                                                        centreScreenPos.Y + y * tileDimensions + y * m_iTileSpacing);
                    //Minusing (grid dimensions / 2) so that tile origin is at the centre
                    currentTilePos -= new Vector2(m_iFullGridDimensions / 2f, m_iFullGridDimensions / 2f);

                    //If at the bottom-right position, create a tile object that is the blank tile
                    if (x == m_iGridSize - 1 && y == m_iGridSize - 1)
                    {
                        m_tiles[x, y] = new Tile(new Vector2(x, y), true, m_iGridSize);
                        m_blankTile = m_tiles[x, y];
                    }
                    //Otherwise create a non-blank tile object
                    else
                    {
                        m_tiles[x, y] = new Tile(new Vector2(x, y), false, m_iGridSize);
                    }
                    //Set the target visual position of the new tile object to it's drawn in the correct place
                    m_tiles[x, y].SetTargetVisualPosition(currentTilePos, true);
                }
            }
        }

        public bool IsGridSolved()
        {
            //Loop through each tile in the grid. If any tile is not at its starting position, the grid cannot be
            //  solved so return false. Otherwise, return true as the grid is in its original solved state
            for (int x = 0; x < m_iGridSize; x++)
            {
                for (int y = 0; y < m_iGridSize; y++)
                {
                    Tile currentTile = m_tiles[x, y];
                    if(currentTile.GetGridStartPosition() != currentTile.GetGridCurrentPosition())
                    {
                        return false;
                    }
                }
            }
            return true;
        }
    }
}
