using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace _15TileGame
{
    public abstract class UIElement
    {
        protected string m_sName;                   //The unique name for this UI element
        protected Rectangle m_rectangle;            //The rectangle that defines the position/size of this element
        protected Rectangle m_rectangleHover;       //The rectangle that defines the position/size of this element when hovering
        protected bool m_bEnabled = true;           //Whether or not the UI element is enabled and can be interacted with
        protected bool m_bHovering = false;         //Whether or not the player is hovering their mouse over this element
        protected bool m_bVisible = true;           //Is the element being drawn
        protected EGameMode m_gameModeShow;         //The GameMode in which to draw this element
        protected Action<UIElement> m_actionClick;  //The function that will be called when this element is clicked


        //=====================\\
        //  Getters & setters  \\
        //=====================\\
        public string GetName() { return m_sName; }
        public EGameMode GetGameModeShow() { return m_gameModeShow; }
        public void SetGameModeShow(EGameMode gameModeShow) { m_gameModeShow = gameModeShow; }
        public void SetEnabled(bool enabled) { m_bEnabled = enabled; }
        public void SetVisible(bool visible) { m_bVisible = visible; }

        //Constructor
        public UIElement(string name, int xPos, int yPos, int width, int height, bool visible, EGameMode gameModeShow, Action<UIElement> actionClick)
        {
            m_sName = name;
            m_rectangle = new Rectangle(xPos, yPos, width, height);
            m_rectangleHover = new Rectangle(xPos + 2, yPos + 2, width - 4, height - 4);
            m_bVisible = visible;
            m_gameModeShow = gameModeShow;
            m_actionClick = actionClick;
        }

        //Destructor
        ~UIElement()
        { }


        public virtual void Update(EGameMode previousGameMode, EGameMode currentGameMode, bool leftClicked, int mouseX, int mouseY)
        {
            //Only update if in the correct GameMode for the current and previous frame - this prevents button clicks 
            //  from previous screens activating UI elements in the same position after switching GameModes
            if (currentGameMode == m_gameModeShow && previousGameMode == m_gameModeShow)
            {
                //If the mouse is within the bounds of this element
                if (m_rectangle.Contains(mouseX, mouseY))
                {
                    //If the mouse was clicked, trigger the Click method
                    if (leftClicked)
                    {
                        Click();
                    }
                    //If the mouse just entered this element, set hovering to true
                    //  and trigger the Hover method
                    if (!m_bHovering)
                    {
                        m_bHovering = true;
                        Hover();
                    }
                }
                else
                {
                    //If the mouse just moved outside of this element, set hovering to false
                    //  and trigger the EndHover method
                    if (m_bHovering)
                    {
                        m_bHovering = false;
                        EndHover();
                    }
                }
            }
        }

        public abstract void Draw(SpriteBatch spriteBatch);

        public virtual void Click()
        {
            //On click, if the element is enabled and m_actionClick is not null,
            //  call the actionClick function, passing this UIElement as a parameter
            if (m_bEnabled && m_bVisible)
            {
                m_actionClick?.Invoke(this);
            }
        }

        public virtual void Hover() { }
        public virtual void EndHover() { }
    }
}
