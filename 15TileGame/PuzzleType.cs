using Microsoft.Xna.Framework.Graphics;

namespace _15TileGame
{
    //PuzzleType is a collection of info about the puzzles that can be chosen between in-game
    public struct PuzzleType
    {
        //Constructor
        public PuzzleType(string name, Texture2D texture)
        {
            this.name = name;
            this.texture = texture;
        }

        public string name;         //The name of the puzzle that will be displayed in the UI
        public Texture2D texture;   //The texture to use when drawing this puzzle type as a grid
    }
}