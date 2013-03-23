using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework;

namespace GameLib.Engine.MenuSystem {

    public class MenuInput {

        public ControlsQB input;

        InputAction Green;
        InputAction Red;
        InputAction PauseGame;
        InputAction MenuSelect;
        InputAction MenuCancel;
        InputAction MenuUp;
        InputAction MenuDown;
        InputAction MenuRight;
        InputAction MenuLeft;
        bool dontPause = false;

        public MenuInput (ControlsQB input) {

            this.input = input;
        }

        public void PostLoadInit(ParameterSet parm)
        {
            Green = input.GetInputAction("A");
            Red = input.GetInputAction("B");
            PauseGame = input.GetInputAction("IsPauseGame");
            MenuSelect = input.GetInputAction("IsMenuSelect");
            MenuCancel = input.GetInputAction("IsMenuCancel");
            MenuUp = input.GetInputAction("IsMenuUp");
            MenuDown = input.GetInputAction("IsMenuDown");
            MenuRight = input.GetInputAction("IsMenuRight");
            MenuLeft = input.GetInputAction("IsMenuLeft");

            if (parm.HasParm("DontPause"))
                dontPause = parm.GetBool("DontPause");
        }

        /// <summary>
        /// Checks for a "menu select" input action.
        /// </summary>
        public bool IsMenuSelect () {
            return MenuSelect.IsNewAction;
        }


        /// <summary>
        /// Checks for a "menu cancel" input action.
        /// </summary>
        public bool IsMenuCancel () {
            return MenuCancel.IsNewAction;
        }


        /// <summary>
        /// Checks for a "menu up" input action.
        /// </summary>
        public bool IsMenuUp () {
            return (input.LastGamePadState.ThumbSticks.Left.Y <= 0
                && input.CurrentGamePadState.ThumbSticks.Left.Y > 0) || MenuUp.IsNewAction;
        }


        /// <summary>
        /// Checks for a "menu down" input action.
        /// </summary>
        public bool IsMenuDown () {
            return (input.LastGamePadState.ThumbSticks.Left.Y >= 0
                && input.CurrentGamePadState.ThumbSticks.Left.Y < 0) || MenuDown.IsNewAction;
        }

        /// <summary>
        /// Checks for a "menu up" input action.
        /// </summary>
        public bool IsMenuRight () {
            return (input.LastGamePadState.ThumbSticks.Left.X <= 0
                && input.CurrentGamePadState.ThumbSticks.Left.X > 0) || MenuRight.IsNewAction;
        }


        /// <summary>
        /// Checks for a "menu down" input action.
        /// </summary>
        public bool IsMenuLeft () {
            return (input.LastGamePadState.ThumbSticks.Left.X >= 0
                && input.CurrentGamePadState.ThumbSticks.Left.X < 0) || MenuLeft.IsNewAction;
        }
        //input.LastGamePadState.ThumbSticks.Left.Y >= 0 
                //&& input.CurrentGamePadState.ThumbSticks.Left.Y<0)

        /// <summary>
        /// Checks for a "pause the game" input action.
        /// </summary>
        public bool IsPauseGame ()
        {
           return !dontPause && PauseGame.IsNewAction;
        }

        public bool IsGreen()
        {
            return Green.IsNewAction;
        }

        public bool IsRed()
        {
            return Red.IsNewAction;
        }
    }
}
