using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace _15TileGame
{
    public class UITooltip
    {
        private bool m_bVisible = false;    //Is the element being drawn
        private Texture2D m_texture;        //The texture for the tooltip background
        private SpriteFont m_font;          //The font for the tooltip text
        private Vector2 m_position;         //The x/y position of where the tooltip is drawn
        private string m_sText = "";        //The text to display within the tooltip
        private Vector2 m_fontPosition;     //The position of the text within the button, defaults to centred

        //Constructor
        public UITooltip(Texture2D texture, SpriteFont font)
        {
            m_texture = texture;
            m_font = font;
        }

        //Destructor
        ~UITooltip()
        { }


        private void UpdateFontPosition()
        {
            //Get the size that the text will be when drawn
            Vector2 textSize = m_font.MeasureString(m_sText);
            //Set the font position to be in the exact centre of the tooltip
            m_fontPosition = new Vector2(m_position.X + (m_texture.Width / 2) - (textSize.X / 2),
                                         m_position.Y + (m_texture.Height / 2) - (textSize.Y / 2));
        }

        public virtual void Update(int mouseX, int mouseY)
        {
            if (m_bVisible)
            {
                //If the tooltip is being shown, update the position of the background and text
                m_position = new Vector2(mouseX, mouseY);
                UpdateFontPosition();
            }
        }

        public void Show(string text)
        {
            //Set the tooltip's text to the passed string and make it visible
            m_sText = text;
            m_bVisible = true;
        }

        public void Hide()
        {
            //Make the tooltip invisible
            m_bVisible = false;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            if (m_bVisible)
            {
                //If the tooltip is being shown, draw both the background and text at their respective positions
                spriteBatch.Draw(m_texture, m_position, Color.White);
                spriteBatch.DrawString(m_font, m_sText, m_fontPosition, Color.Black);
            }
        }
    }
}
