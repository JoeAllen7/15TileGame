using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace _15TileGame
{
    public class UIButton : UIElement
    {
        private string m_sButtonText;           //The text displayed on this button
        private SpriteFont m_font;              //The font used for button text
        private Texture2D m_textureNormal;      //The texture of the button in its default state
        private Texture2D m_textureHover;       //The texture of the button when hovering
        private Texture2D m_textureDisabled;    //The texture of the button when not enabled
        private Color m_colourText;             //The colour of the text on this button
        private Vector2 m_fontPosition;         //The position of the text within the button, defaults to centred
        private Vector4 m_visualSize;           //The x/y/width/height values used when drawing the button (may be interpolated between normal and hover rects)
        private bool m_bShowTooltip;            //Should hovering over this button cause a tooltip to be shown?
        private bool m_bEnableHoverShrink;      //Should hovering over this button cause it to become slightly smaller?
        private string m_sTooltipText;          //If a ShowTooltip is true, this text will be displayed in the tooltip


        //=====================\\
        //  Getters & setters  \\
        //=====================\\
        public void SetText(string text)
        {
            m_sButtonText = text;
            //Ensure the text is still centered within the button
            UpdateFontPosition();
        }
        public void SetTextures(Texture2D texture)
        {
            //Set both normal and hover textures
            m_textureNormal = texture;
            m_textureHover = texture;
        }
        public void SetPosition(int xPos, int yPos)
        {
            //Update the x/y positions of the button rectangle, as well as the hover rect
            //  so the button doesn't shift out of place when hovering
            m_rectangle = new Rectangle(xPos, yPos, m_rectangle.Width, m_rectangle.Height);
            m_rectangleHover = new Rectangle(xPos + 2, yPos + 2, m_rectangleHover.Width, m_rectangleHover.Height);
        }
        public void SetStyle(Texture2D textureNormal, Color colourText)
        {
            //Set the button's texture and font colour
            m_textureNormal = textureNormal;
            m_colourText = colourText;
        }

        //Constructor
        public UIButton(string name, int xPos, int yPos, int width, int height, bool visible, EGameMode gameModeShow, Action<UIElement> actionClick,
                        string buttonText, SpriteFont font, Texture2D textureNormal, Texture2D textureHover, Texture2D textureDisabled, Color colourText,
                        bool showTooltip = false, string tooltipText = "", bool enableHoverShrink = true)
                        : base(name, xPos, yPos, width, height, visible, gameModeShow, actionClick)
        {
            m_sButtonText = buttonText;
            m_font = font;
            m_textureNormal = textureNormal;
            m_textureHover = textureHover;
            m_textureDisabled = textureDisabled;
            m_colourText = colourText;
            m_bShowTooltip = showTooltip;
            m_sTooltipText = tooltipText;
            m_bEnableHoverShrink = enableHoverShrink;
            m_visualSize = new Vector4(xPos, yPos, width, height);

            //Centre text within button bounds
            UpdateFontPosition();
        }

        //Destructor
        ~UIButton()
        { }


        private void UpdateFontPosition()
        {
            //Get the size that the button's text will be when drawn
            Vector2 textSize = m_font.MeasureString(m_sButtonText);
            //Centre the text within button bounds
            m_fontPosition = new Vector2(m_rectangle.X + (m_rectangle.Width / 2) - (textSize.X / 2),
                                         m_rectangle.Y + (m_rectangle.Height / 2) - (textSize.Y / 2));
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            //Only draw if visible
            if (m_bVisible)
            {
                Texture2D targetTex;
                //If enabled, use the normal/hover texture depending on if mouse is over button
                if (m_bEnabled)
                {
                    targetTex = (m_bHovering ? m_textureHover : m_textureNormal);
                }
                //Otherwise use the disabled texture
                else
                {
                    targetTex = m_textureDisabled;
                }

                //If mouse is over button, button is enabled and shrink on hover is enabled,
                //  use the hover rect for drawing
                Rectangle targetRect;
                if(m_bHovering && m_bEnabled && m_bEnableHoverShrink)
                {
                    targetRect = m_rectangleHover;
                }
                //Otherwise use the standard rectangle
                else
                {
                    targetRect = m_rectangle;
                }

                //Interpolate between the current button size and determined rectangle size
                m_visualSize = Vector4.Lerp(m_visualSize, new Vector4(targetRect.X, targetRect.Y, targetRect.Width, targetRect.Height), 1f);

                //Draw the button background image and text
                spriteBatch.Draw(targetTex, new Rectangle((int)m_visualSize.X, (int)m_visualSize.Y, (int)m_visualSize.Z, (int)m_visualSize.W), null, Color.White, 0f, Vector2.Zero, SpriteEffects.None, 0f);
                spriteBatch.DrawString(m_font, m_sButtonText, m_fontPosition, m_colourText);
            }
        }

        public override void Click()
        {
            //On click, play a button click sound if enabled & visible
            if (m_bEnabled && m_bVisible)
            {
                AudioManager.PlaySoundEffectWithName("buttonClick");
            }
            //Ensure no tooltip is showing - this prevents tooltips persisting between different game screens
            InputManager.GetTooltip().Hide();
            base.Click();
        }

        public override void Hover()
        {
            //On hover, play a button hover sound if enabled & visible
            if (m_bEnabled && m_bVisible)
            {
                AudioManager.PlaySoundEffectWithName("buttonHover");
                //If tooltip is enabled for this button, show a tooltip with the appropriate message
                if (m_bShowTooltip)
                {
                    InputManager.GetTooltip().Show(m_sTooltipText);
                }
            }
        }

        public override void EndHover()
        {
            //Hide the tooltip when mouse moves out of button bounds
            InputManager.GetTooltip().Hide();
        }
    }
}
