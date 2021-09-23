using System;
using Microsoft.Xna.Framework;

namespace _15TileGame
{
    public class Tile
    {
        private Vector2 m_gridStartPosition;          //The position of this tile in the grid array/structure when it is created
                                                      // - used to draw the correct section of the grid texture

        private Vector2 m_gridCurrentPosition;        //The current position of this tile in the grid array/structure


        private Vector2 m_currentVisualPosition;      //The position at which this tile will be drawn to the screen

        private Vector2 m_targetVisualPosition;       //The position this tile should move towards when being drawn to the screen

        private Vector2 m_effectPositionModifier;     //This Vector2 is added to the current visual position to apply extra visual effects
                                                      //    such as shaking tiles

        private bool m_bIsBlankSpace;                 //Whether or not the tile is a blank space
                                                      //  (if blank, no texture is drawn and other tiles can be moved into its space)

        private bool m_bHighlighted;

        private int m_iTileNumber;                    //The number to be drawn on this tile if tile numbers are enabled

        private Color m_colourTint = Color.White;


        //=====================\\
        //  Getters & setters  \\
        //=====================\\
        public Vector2 GetGridStartPosition() { return m_gridStartPosition; }
        public Vector2 GetGridCurrentPosition() { return m_gridCurrentPosition; }
        public void SetGridCurrentPosition(Vector2 gridCurrentPosition) { m_gridCurrentPosition = gridCurrentPosition; }
        public Vector2 GetCurrentVisualPosition() { return m_currentVisualPosition; }
        public void SetCurrentVisualPosition(Vector2 currentVisualPosition) { m_currentVisualPosition = currentVisualPosition; }

        //Overall visual position is the effect modifier position added to the target visual position
        public Vector2 GetOverallVisualPosition() { return m_targetVisualPosition + m_effectPositionModifier; }
        public void SetTargetVisualPosition(Vector2 visualPosition, bool alsoSetCurrent)
        {
            m_targetVisualPosition = visualPosition;
            if (alsoSetCurrent) { m_currentVisualPosition = visualPosition; }
        }
        public bool GetIsBlankSpace() { return m_bIsBlankSpace; }
        public void SetIsBlankSpace(bool isBlankSpace) { m_bIsBlankSpace = isBlankSpace; }
        public Color GetColourTint() { return m_colourTint; }
        public bool GetHighlighted() { return m_bHighlighted; }
        public void SetHighlighted(bool highlighted) { m_bHighlighted = highlighted; }
        public int GetTileNumber() { return m_iTileNumber; }

        public Rectangle GetStandardRectangle(Grid grid)
        {
            int tileDimensions = (grid.GetFullGridDimensions() / grid.GetGridSize()) - grid.GetTileSpacing();
            return new Rectangle((int)m_currentVisualPosition.X, (int)m_currentVisualPosition.Y,
                                    tileDimensions, tileDimensions);
        }

        //Constructor
        public Tile(Vector2 gridStartPosition, bool isBlankSpace, int gridSize)
        {
            m_gridStartPosition = gridStartPosition;
            m_gridCurrentPosition = gridStartPosition;
            m_bIsBlankSpace = isBlankSpace;
            m_iTileNumber = (int)(gridStartPosition.X + (gridStartPosition.Y * gridSize)) + 1;
        }

        //Destructor
        ~Tile()
        { }


        public void UpdateShakeEffect(float shakeMultiplier, float maxShakeAmount)
        {
            //Get random X and Y positions with an intensity based on the shake multiplier
            //  'shakeMultiplier / 2f' is taken away to ensure the shake is centered
            Random rand = new Random();
            float randX = ((float)rand.NextDouble() * shakeMultiplier) - (shakeMultiplier / 2f);
            float randY = ((float)rand.NextDouble() * shakeMultiplier) - (shakeMultiplier / 2f);

            //Set the effectPositionModifier vector to these random x/y values
            m_effectPositionModifier = new Vector2(randX, randY);

            //Apply a colour tint based - higher the shake intensity, the more red the tile will appear
            m_colourTint = new Color(0.85f, 0.9f - (shakeMultiplier / maxShakeAmount), 0.8f - (shakeMultiplier / maxShakeAmount));
        }

        public void RemoveEffects()
        {
            //Reset the effectPositionModifier vector and set the tile colour back to white
            m_effectPositionModifier = Vector2.Zero;
            m_colourTint = Color.White;
        }
    }
}
