using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Content;

// InputAction
// reads data off keyboard/mouse/controller

// todo:
// possibly convert to be more like control system at work.
// declare start, stop during handlers
// individual control files for each controller type?
// they call into same handlers with same values no matter type

namespace GameLib
{
    public class InputActionData
    {
        public string name;
        public string rate;
        public string keyLowName;
        public string keyHighName;
        public string buttonLowName;
        public string buttonHighName;
        public string axisName;

        public float GetRate()
        {
            return float.Parse(rate);
        }

        public Buttons? GetButton(bool low)
        {

            if (low)
            {
                if (!string.IsNullOrEmpty(buttonLowName))
                    return (Buttons?)Enum.Parse(typeof(Buttons), buttonLowName, true);
                else
                    return null;
            }
            else
            {
                if (!string.IsNullOrEmpty(buttonHighName))
                    return (Buttons?)Enum.Parse(typeof(Buttons), buttonHighName, true);
                else
                    return null;
            }
            
            // xbox 360 may not support the fancy Enum.Parse from above...
            // heres some old code I saved just in case...
            /*
            switch (buttonName.ToUpper())
            {
                case "A":
                    return Buttons.A;
                case "B":
                    return Buttons.B;
                case "X":
                    return Buttons.X;
                case "Y":
                    return Buttons.Y;
                case "RIGHTBUMPER":
                    return Buttons.RightShoulder;
                case "LEFTBUMPER":
                    return Buttons.LeftShoulder;
                case "START":
                    return Buttons.Start;
                case "BACK":
                    return Buttons.Back;
                case "LEFTSTICK":
                    return Buttons.LeftStick;
                case "RIGHTSTICK":
                    return Buttons.RightStick;
                case "DPADDOWN":
                     return Buttons.DPadDown;
                case "DPADUP":
                     return Buttons.DPadUp;
                case "DPADRIGHT":
                     return Buttons.DPadRight;
                case "DPADLEFT":
                     return Buttons.DPadLeft;
                case "LEFTTRIGGER":
                     return Buttons.LeftTrigger;
                case "RIGHTTRIGGER":
                     return Buttons.RightTrigger;
                default:
                    return null;
            }
             * */
        }

        public Keys? GetKey(bool low)
        {
            if (low)
            {
                if (!string.IsNullOrEmpty(keyLowName))
                    return (Keys?)Enum.Parse(typeof(Keys), keyLowName, true);
                else
                    return null;
            }
            else
            {
                if (!string.IsNullOrEmpty(keyHighName))
                    return (Keys?)Enum.Parse(typeof(Keys), keyHighName, true);
                else
                    return null;
            }

            /*
            keyName.ToUpper();

            // alphanumeric chars can get converted straight to keys
            if (keyName.Length == 1 && char.IsLetterOrDigit(keyName[0]))
                return (Keys)((int)keyName[0]);

            switch (keyName)
            {                    
                case "LEFTCONTROL":
                    return Keys.LeftControl;
                case "RIGHTCONTROL":
                    return Keys.RightControl;
                case "LEFTSHIFT":
                    return Keys.LeftShift;
                case "RIGHTSHIFT":
                    return Keys.RightShift;
                case "LEFTALT":
                    return Keys.LeftAlt;
                case "RIGHTALT":
                    return Keys.RightAlt;
                case "TAB":
                    return Keys.Tab;
                default:
                    return null;
            }
             * */
        }

        public InputAction.AxisType? GetAxisType()
        {
            if (!string.IsNullOrEmpty(axisName))
                return (InputAction.AxisType?)Enum.Parse(typeof(InputAction.AxisType), axisName, true);
            else
                return null;
        }
    }

    public class InputAction : IComparable<InputAction>
    {        
        public string name { get; private set; }
        public float value { get; private set; }
        public float lastValue { get; private set; }

        public enum AxisType
        {
            RightThumbstickX,
            RightThumbstickY,
            LeftThumbstickX,
            LeftThumbstickY,
            RightTrigger,
            LeftTrigger,
        }

        private Buttons? buttonLow;
        private Buttons? buttonHigh;
        private Keys? keyLow;
        private Keys? keyHigh;
        private AxisType? axisType;
        private float rate;

        /// <summary>
        /// Returns true if last frame value was 0, and this frame it is not 0.
        /// </summary>
        public bool IsNewAction 
        {
            get { return lastValue == 0.0f && value != 0.0f; }
        }

        public InputAction(string name, float rate, Buttons? buttonLow, Buttons? buttonHigh, Keys? keyLow, Keys? keyHigh, AxisType? axisType)
        {
            this.name = name;
            this.rate = rate;
            this.buttonLow = buttonLow;
            this.buttonHigh = buttonHigh;
            this.keyLow = keyLow;
            this.keyHigh = keyHigh;
            this.axisType = axisType;
            this.value = 0.0f;
            this.lastValue = 0.0f;
        }

        Keys? ParseKey(string key)
        {
            if (!string.IsNullOrEmpty(key))
                return (Keys?)Enum.Parse(typeof(Keys), key, true);
            else
                return null;
        }

        Buttons? ParseButton(string button)
        {
            if (!string.IsNullOrEmpty(button))
                return (Buttons?)Enum.Parse(typeof(Buttons), button, true);
            else
                return null;
        }

        AxisType? ParseAxis(string axis)
        {
            if (!string.IsNullOrEmpty(axis))
                return (InputAction.AxisType?)Enum.Parse(typeof(InputAction.AxisType), axis, true);
            else
                return null;
        }

        public InputAction(string name, string rate, string buttonLow, string buttonHigh, string keyLow, string keyHigh, string axis)
        {
            this.name = name;
            this.rate = float.Parse(rate);
            this.buttonLow = ParseButton(buttonLow);
            this.buttonHigh = ParseButton(buttonHigh);
            this.keyLow = ParseKey(keyLow);
            this.keyHigh = ParseKey(keyHigh);
            this.axisType = ParseAxis(axis);
        }

        public void Update(ControlsQB controller, float dt)
        {
            lastValue = value;

            bool lowPressed = false;
            bool highPressed = false;

            if (buttonLow.HasValue)
            {
                if (controller.CurrentGamePadState.IsButtonDown(buttonLow.Value))
                    lowPressed = true;
            }

#if WINDOWS
            if (keyLow.HasValue)
            {
                if (controller.CurrentKeyboardState.IsKeyDown(keyLow.Value))
                    lowPressed = true;
            }
#endif         

            if (buttonHigh.HasValue)
            {
                if (controller.CurrentGamePadState.IsButtonDown(buttonHigh.Value))
                    highPressed = true;
            }

#if WINDOWS
            if (keyHigh.HasValue)
            {
                if (controller.CurrentKeyboardState.IsKeyDown(keyHigh.Value))
                    highPressed = true;
            }
#endif

            float change = dt * rate;
            if (rate == -1.0f)
                change = 1.0f;

            if (!lowPressed && !highPressed)
            {
                // none pressed, move to zero
                if (lastValue > 0.0f)
                    value = Math.Max(lastValue - change, 0.0f);
                else
                    value = Math.Min(lastValue + change, 0.0f);
            }

            if (lowPressed)
            {
                // low pressed, subtract change
                value -= change;
            }

            if (highPressed)
            {
                // high pressed, add change
                value += change;
            }

            // clamp between -1 and 1
            value = MathHelper.Clamp(value, -1.0f, 1.0f);

            // floats are not precise...if we are close to zero call it zero
            //if (MathHelper.Distance(value, 0.0f) > 0.0001f)
                //value = 0.0f;
            float axisValue = 0.0f;
            if (axisType.HasValue)
            {
                switch (axisType.Value)
                {
                    case AxisType.LeftThumbstickX:
                        axisValue = controller.CurrentGamePadState.ThumbSticks.Left.X;
                        break;
                    case AxisType.LeftThumbstickY:
                        axisValue = controller.CurrentGamePadState.ThumbSticks.Left.Y;
                        break;
                    case AxisType.RightThumbstickX:
                        axisValue = controller.CurrentGamePadState.ThumbSticks.Right.X;
                        break;
                    case AxisType.RightThumbstickY:
                        axisValue = controller.CurrentGamePadState.ThumbSticks.Right.Y;
                        break;
                    case AxisType.LeftTrigger:
                        axisValue = controller.CurrentGamePadState.Triggers.Left;
                        break;
                    case AxisType.RightTrigger:
                        axisValue = controller.CurrentGamePadState.Triggers.Right;
                        break;
                }
            }

            value = MathHelper.Clamp(value + axisValue, -1.0f, 1.0f);
        }

        public int CompareTo(InputAction other)
        {
            return name.CompareTo(other.name);
        }
       
    }

    /*
    public class inputActionSet
    {
        private List<InputAction> data;
        private int length;

        public inputActionSet()
        {
            data = new List<InputAction>();
            length = 0;
        }

        public void Add(InputAction action)
        {
            data.Add(action);
            length++;
        }

        public override string ToString()
        {
            string rtn = "";
            foreach (InputAction action in data)
                rtn += action.name;

            return rtn;
        }

        public int GetLength()
        {
            return length;
        }

    }
     */
}
