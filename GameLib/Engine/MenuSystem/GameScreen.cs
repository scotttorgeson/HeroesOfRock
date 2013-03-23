using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace GameLib.Engine.MenuSystem {
    /// <summary>
    /// Enum describes the menu transition state.
    /// </summary>
    public enum MenuState
    {
        TransitionOn,
        Active,
        TransitionOff,
        Hidden,
    }

    public abstract class GameScreen
    {

        bool markedForRemove;
        public bool MarkedForRemove
        {
            get { return markedForRemove; }
            set
            {
                if (GetType() != typeof(HudScreen))
                    markedForRemove = value;
            }
        }
        bool isPopup = false;
        MenuSystem menuSystem;
        TimeSpan transitionOnTime = TimeSpan.Zero;
        TimeSpan transitionOffTime = TimeSpan.Zero;
        float transitionPosition = 1;
        MenuState screenState = MenuState.TransitionOn;
        bool isExiting = false;
        bool otherScreenHasFocus;

        public virtual void LoadContent() { }

        public virtual void Update(float dt, bool otherScreenHasFocus,
                                                      bool coveredByOtherScreen)
        {
            this.otherScreenHasFocus = otherScreenHasFocus;

            if (isExiting)
            {
                // If the menu is going away to die, it should transition off.
                screenState = MenuState.TransitionOff;

                if (!UpdateTransition(dt, transitionOffTime, 1))
                {
                    // When the transition finishes, remove the menu.
                    this.MarkedForRemove = true;
                }
            }
            else if (coveredByOtherScreen)
            {
                // If the menu is covered by another, it should transition off.
                if (UpdateTransition(dt, transitionOffTime, 1))
                {
                    // Still busy transitioning.
                    screenState = MenuState.TransitionOff;
                }
                else
                {
                    // Transition finished!
                    screenState = MenuState.Hidden;
                }
            }
            else
            {
                // Otherwise the menu should transition on and become active.
                if (UpdateTransition(dt, transitionOnTime, -1))
                {
                    // Still busy transitioning.
                    screenState = MenuState.TransitionOn;
                }
                else
                {
                    // Transition finished!
                    screenState = MenuState.Active;
                }
            }
        }

        bool UpdateTransition(float dt, TimeSpan time, int direction)
        {
            // How much should we move by?
            float transitionDelta;

            if (time == TimeSpan.Zero)
                transitionDelta = 1;
            else
                transitionDelta = (float)(dt /
                                          time.TotalSeconds);

            // Update the transition position.
            transitionPosition += transitionDelta * direction;

            // Did we reach the end of the transition?
            if (((direction < 0) && (transitionPosition <= 0)) ||
                ((direction > 0) && (transitionPosition >= 1)))
            {
                transitionPosition = MathHelper.Clamp(transitionPosition, 0, 1);
                return false;
            }

            // Otherwise we are still busy transitioning.
            return true;
        }


        /// <summary>
        /// Allows the active menu to handle user input
        /// </summary>
        public virtual void HandleInput(MenuInput input) { }


        /// <summary>
        /// This is called when the menu should draw itself.
        /// </summary>
        public virtual void Draw(float dt) { }


        /// <summary>
        /// Exit menu with transition
        /// </summary>
        public void ExitScreen()
        {
            if (TransitionOffTime == TimeSpan.Zero)
            {
                // If the menu has a zero transition time, remove it immediately.
                this.MarkedForRemove = true;
            }
            else
            {
                // Otherwise flag that it should transition off and then exit.
                isExiting = true;
            }
        }

        public bool IsPopup
        {
            get { return isPopup; }
            protected set { isPopup = value; }
        }

        public TimeSpan TransitionOnTime
        {
            get { return transitionOnTime; }
            protected set { transitionOnTime = value; }
        }

        public TimeSpan TransitionOffTime
        {
            get { return transitionOffTime; }
            protected set { transitionOffTime = value; }
        }

        public float TransitionPosition
        {
            get { return transitionPosition; }
            protected set { transitionPosition = value; }
        }

        public float TransitionAlpha
        {
            get { return 1f - TransitionPosition; }
        }

        public MenuState MenuState
        {
            get { return screenState; }
            protected set { screenState = value; }
        }


        public bool IsExiting
        {
            get { return isExiting; }
            protected internal set { isExiting = value; }
        }

        public bool IsActive
        {
            get
            {
                return !otherScreenHasFocus &&
                       (screenState == MenuState.TransitionOn ||
                        screenState == MenuState.Active);
            }
        }

        public MenuSystem MenuSystem
        {
            get { return menuSystem; }
            internal set { menuSystem = value; }
        }
    }
        
}
