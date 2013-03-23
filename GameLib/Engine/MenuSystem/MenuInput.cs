using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework;

namespace GameLib.Engine.MenuSystem {
    /// <summary>
    /// Helper for reading gamepad input. This class 
    /// tracks both the current and previous state of the input and implements input actions  
    /// </summary>
    public class MenuInput {

        public GamePadState CurrentGamePadState;
        public GamePadState LastGamePadState;

        public KeyboardState CurrentKeyboardState;
        public KeyboardState LastKeyboardState;

        public bool GamePadWasConnected;

        public ControlsQB qb;

        public MenuInput () {
            CurrentGamePadState = new GamePadState();
            LastGamePadState = new GamePadState();

            CurrentKeyboardState = new KeyboardState();
            LastKeyboardState = new KeyboardState();
            
            GamePadWasConnected = new bool();
        }

        public void Update () {
            LastGamePadState = CurrentGamePadState;
            LastKeyboardState = CurrentKeyboardState;

            CurrentGamePadState = GamePad.GetState(PlayerIndex.One);
            CurrentKeyboardState = Keyboard.GetState();
        }
    
        /// <summary>
        /// Helper for checking if a button was newly pressed during this update.
        /// </summary>
        public bool IsNewButtonPress (Buttons button) {
                return (LastGamePadState.IsButtonDown(button) &&
                        CurrentGamePadState.IsButtonUp(button));
        }

        /// <summary>
        /// Helper for checking if a button was newly pressed during this update.
        /// </summary>
        public bool IsNewKeyPress (Keys keys) {
            return (LastKeyboardState.IsKeyDown(keys) &&
                    CurrentKeyboardState.IsKeyUp(keys));
        }

        /// <summary>
        /// Checks for a "menu select" input action.
        /// </summary>
        public bool IsMenuSelect () {
            return  IsNewButtonPress(Buttons.A) || IsNewButtonPress(Buttons.Start) || IsNewKeyPress(Keys.Enter);
        }


        /// <summary>
        /// Checks for a "menu cancel" input action.
        /// </summary>
        public bool IsMenuCancel () {
            return IsNewButtonPress(Buttons.B) || IsNewButtonPress(Buttons.Back);
        }


        /// <summary>
        /// Checks for a "menu up" input action.
        /// </summary>
        public bool IsMenuUp () {
            return IsNewButtonPress(Buttons.DPadUp) ||  IsNewButtonPress(Buttons.LeftThumbstickUp);
        }


        /// <summary>
        /// Checks for a "menu down" input action.
        /// </summary>
        public bool IsMenuDown () {
             return IsNewButtonPress(Buttons.DPadDown) || IsNewButtonPress(Buttons.LeftThumbstickDown);
        }


        /// <summary>
        /// Checks for a "pause the game" input action.
        /// </summary>
        public bool IsPauseGame () {
            return IsNewButtonPress(Buttons.Back) || IsNewButtonPress(Buttons.Start) || IsNewKeyPress(Keys.Tab);
        }
    }
}
