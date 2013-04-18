using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Graphics;

// Works with InputAction to read controls off input devices

namespace GameLib {
    public class ControlsQB : Quarterback
    {
        static GamePadState currentGamePadState;
        static GamePadState lastGamePadState;
        static GamePadType gamePadType;

#if WINDOWS
        static KeyboardState currentKeyboardState;
        static KeyboardState lastKeyboardState;
        static MouseState currentMouseState;
        static MouseState lastMouseState;
#endif

        public GamePadState CurrentGamePadState { get { return currentGamePadState; } }
        public GamePadState LastGamePadState { get { return lastGamePadState; } }
        public GamePadType GamePadType { get { return gamePadType; } }
#if WINDOWS
        public KeyboardState CurrentKeyboardState { get { return currentKeyboardState; } }
        public KeyboardState LastKeyboardState { get { return lastKeyboardState; } }
        public MouseState CurrentMouseState { get { return currentMouseState; } }
        public MouseState LastMouseState { get { return lastMouseState; } }
#endif

        public static List<InputAction> actions;
        PlayerIndex playerIndex;

        public PlayerIndex PlayerIndex
        {
            set
            {
                playerIndex = value;
                currentGamePadState = GamePad.GetState(playerIndex);
            }
        }


#if DEBUG
        // set record to true and the game will record all your controller input and save it at the end when you exit the game
        // set playback to true and it will read the previous saved file and playback the controller input
        // great for automated testing!
        // kind of hacky at the moment. every frame saves the controller state. so if you record at 60 fps, you need to playback at 60fps
        // todo: based on time, not on frames
        bool record = false;
        bool playback = false;
        int framenumber = 0;
        List<GamePadState> gamepadStates;
        List<KeyboardState> keyboardStates;
#endif
        public override string Name ()
        {
            return "ControlsQB";
        }

        public ControlsQB ()
        {
        }

        public GamePadType GetGamePadType()
        {
            return GamePad.GetCapabilities(playerIndex).GamePadType;
        }


        public override void PreLoadInit (ParameterSet Parm) {
            // should be looking at all the controllers, then the first one that presses a button
            // becomes the one we use for the rest of the game, instead of hard coding player one
            //if (Stage.PlayerIndex != null)
            { 
                this.playerIndex = Stage.PlayerIndex;
                currentGamePadState = GamePad.GetState(playerIndex);
            }

#if WINDOWS
            currentKeyboardState = Keyboard.GetState();
            currentMouseState = Mouse.GetState();
#endif
        }

        public override void LoadContent ()
        {
            // load actions
            if (actions == null)
            {
                actions = new List<InputAction>();
                InputActionData[] actionDatas = Stage.Content.Load<InputActionData[]>("Controls");
                foreach (InputActionData actionData in actionDatas)
                    actions.Add(new InputAction(actionData.name, actionData.GetRate(), actionData.GetButton(true), actionData.GetButton(false), actionData.GetKey(true), actionData.GetKey(false), actionData.GetAxisType()));
            }

#if DEBUG
            if (record)
            {
                gamepadStates = new List<GamePadState>();
                keyboardStates = new List<KeyboardState>();
            }
            else if (playback)
            {
                gamepadStates = new List<GamePadState>();
                keyboardStates = new List<KeyboardState>();
                ReadGamePadStates("GamePadStates");
            }
#endif

            //keep for testing purposes
            //TODO: remove this
            controlScheme = controlSchemEnum.NULL;
        }

        public override void PauseQB()
        {
            //unpausable
        }

        public override void Update (float dt)
        {
            //make sure PlayerIndex and GamePad are in sync
            if (!Stage.PlayerIndex.Equals(this.playerIndex)) 
            {
                this.playerIndex = Stage.PlayerIndex;
                currentGamePadState = GamePad.GetState(playerIndex);
            }

#if XBOX || TEST_GAMEPAD
            if (!currentGamePadState.IsConnected)
            {
                Stage.ActiveStage.GetQB<GameLib.Engine.MenuSystem.MenuSystemQB>().PauseGame();
            }
#endif

            lastGamePadState = currentGamePadState;

#if WINDOWS
            lastKeyboardState = currentKeyboardState;
            lastMouseState = currentMouseState;
#endif

#if DEBUG
            if (playback && framenumber < gamepadStates.Count)
            {
                currentGamePadState = gamepadStates[framenumber++];
            }
            else
#endif
                currentGamePadState = GamePad.GetState(playerIndex);
#if WINDOWS
            currentKeyboardState = Keyboard.GetState();
            currentMouseState = Mouse.GetState();
#endif

#if DEBUG
            if (record)
                gamepadStates.Add(currentGamePadState);
#endif
            foreach (InputAction action in actions)
            {
                action.Update(this, dt);
            }
        }


        /// <summary>
        /// Find a named input action. You should save a reference to it instead of calling this all the time.
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public InputAction GetInputAction (string name)
        {
            foreach (InputAction action in actions)
            {
                if (action.name == name)
                    return action;
            }

            return null;
        }

#if DEBUG
        public void WriteGamePadStates (string file)
        {
            using (System.IO.StreamWriter sw = new System.IO.StreamWriter(file))
            {
                foreach (GamePadState state in gamepadStates)
                {
                    sw.WriteLine(SerializeGamePadState(state));
                }
            }
        }

        public void ReadGamePadStates (string file)
        {
            using (System.IO.StreamReader sr = new System.IO.StreamReader(file))
            {
                while (sr.EndOfStream == false)
                    gamepadStates.Add(ReadGamePadState(sr.ReadLine()));
            }
        }

        public void Exiting ()
        {
            if (record)
                WriteGamePadStates("GamePadStates");
        }

        string SerializeGamePadState (GamePadState state)
        {
            string ret = "";
            ret += state.ThumbSticks.Left.X + " " + state.ThumbSticks.Left.Y + " ";
            ret += state.ThumbSticks.Right.X + " " + state.ThumbSticks.Right.Y + " ";
            ret += state.Triggers.Left + " " + state.Triggers.Right + " ";
            string buttonsString = state.Buttons.ToString();
            buttonsString = buttonsString.Remove(0, 9);
            buttonsString = buttonsString.Remove(buttonsString.Length - 1);
            if (buttonsString != "None") {
                ret += buttonsString;
            }
            return ret;
        }

        GamePadState ReadGamePadState (string s)
        {
            string[] ss = s.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            Vector2 leftThumbStick = new Vector2(float.Parse(ss[0], System.Globalization.CultureInfo.InvariantCulture), float.Parse(ss[1], System.Globalization.CultureInfo.InvariantCulture));
            Vector2 rightThumbStick = new Vector2(float.Parse(ss[2], System.Globalization.CultureInfo.InvariantCulture), float.Parse(ss[3], System.Globalization.CultureInfo.InvariantCulture));
            float leftTrigger = float.Parse(ss[4], System.Globalization.CultureInfo.InvariantCulture);
            float rightTrigger = float.Parse(ss[5], System.Globalization.CultureInfo.InvariantCulture);
            List<Buttons> buttons = new List<Buttons>();
            for (int i = 6; i < ss.Length; i++)
            {
                buttons.Add((Buttons)Enum.Parse(typeof(Buttons), ss[i], true));
            }
            return new GamePadState(leftThumbStick, rightThumbStick, leftTrigger, rightTrigger, buttons.ToArray());
        }
#endif

        public override void Serialize (ParameterSet parm)
        {
            //TODO::
            //figure out what needs to be updated here

        }

        public controlSchemEnum controlScheme;

        public enum controlSchemEnum
        {
            Guitar_buttonStrumAll = 1,
            Guitar_buttonStrumAttacks_strumMove,
            Guitar_buttonAttack_strumMove,
            GamePad,
            NULL,
        }
    }
}
