using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace _15TileGame
{
    public class UILabel : UIElement
    {
        private string m_sLabelText;        //The text to be displayed in this label
        private SpriteFont m_font;          //The font that will be used when drawing label text
        private Color m_colour;             //The colour of the text being drawn
        private Vector2 m_fontPosition;     //The position of the text within the label rectangle


        //=====================\\
        //  Getters & setters  \\
        //=====================\\
        public void SetText(string text)
        {
            m_sLabelText = text;
            //Ensure text is centered within the label
            UpdateFontPosition();
        }


        //Constructor
        public UILabel(string name, int xPos, int yPos, int width, int height, bool visible, EGameMode gameModeShow, Action<UIElement> actionClick, string labelText, SpriteFont font, Color colour)
                        : base(name, xPos, yPos, width, height, visible, gameModeShow, actionClick)
        {
            m_sLabelText = labelText;
            m_font = font;
            m_colour = colour;

            //Ensure text is centered within the label
            UpdateFontPosition();
        }

        //Destructor
        ~UILabel()
        { }


        private void UpdateFontPosition()
        {
            //Get the size that the labels's text will be when drawn
            Vector2 textSize = m_font.MeasureString(m_sLabelText);
            //Centre the text within label bounds
            m_fontPosition = new Vector2(m_rectangle.X + (m_rectangle.Width / 2) - (textSize.X / 2),
                                         m_rectangle.Y + (m_rectangle.Height / 2) - (textSize.Y / 2));
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            //If visible, draw the text for this label
            if (m_bVisible)
            {
                spriteBatch.DrawString(m_font, m_sLabelText, m_fontPosition, m_colour);
            }
        }
    }
}
