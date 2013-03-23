using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GameLib.Engine.AttackSystem
{
    public struct ButtonInput
    {
        public bool Red;
        public bool Yellow;
        public bool Blue;
        public bool Orange;
        public bool Green;

        public ButtonInput(bool value)
        {
            Red = Yellow = Blue = Orange = Green = value;
        }

        public void Reset()
        {
            Red = Yellow = Blue = Orange = Green = false;
        }

        public bool isSet()
        {
            return Red || Yellow || Blue || Orange || Green;
        }
    }

    class ComboSystem
    {
        Dictionary<ButtonInput, Move> moveList;

        private string moveListFile;

        ButtonInput tempInput = new ButtonInput();
        Move tempMove = null;
        Move performedMove;

        public ComboSystem(string path)
        {
            moveListFile = path;
            tempInput = new ButtonInput();
            performedMove = new Move();
            LoadMoves(Stage.Content.Load<Move[]>(moveListFile));
        }

        public void LoadMoves(IEnumerable<Move> moves)
        {
            moveList = new Dictionary<ButtonInput, Move>();

            foreach (Move m in moves)
            {
                //convert string to button input
                ButtonInput buttonCombo = StringToButtonInput(m.ButtonSequence);

                moveList.Add(buttonCombo, m);
            }
        }

        public Move getAttack(ButtonInput input)
        {
            //our list contains our 3 one button moves, plus the 3 button move
            if (moveList.ContainsKey(input))
                return moveList[input];

            //no such move exists so either two buttons are pressed, or no buttons are pressed

            Move strongMove = null;
            tempMove = null;

            if (input.Red)
            {
                tempInput.Red = true;
                tempMove = moveList[tempInput]; //this is the first input we check so tempMove can't be used
                tempInput.Red = false;
            }
            if (input.Yellow)
            {
                tempInput.Yellow = true;
                if (tempMove != null)
                    strongMove = moveList[tempInput];
                else tempMove = moveList[tempInput];
                tempInput.Yellow = false;
            }
            if (input.Blue)
            {
                tempInput.Blue = true;
                strongMove = moveList[tempInput]; //this is the last input we check so tempMove must be used
                tempInput.Blue = false;
            }

            return strongMove;
            /*
            if (strongMove != null && tempMove != null)
            {
                //interpolate between the two moves
                performedMove = strongMove;
                //performedMove.FrontArea = strongMove.FrontArea;
                //performedMove.BackArea = strongMove.BackArea;
                //performedMove.Damage = strongMove.Damage + tempMove.Damage * .5f;
                performedMove.TimeBeforeSecond = tempMove.TimeBeforeAttack;
                performedMove.AnimationType = strongMove.AnimationType;
                performedMove.AnimationSecond = tempMove.AnimationType;
                //performedMove.ParticleEffect = 
                //performedMove.animation 
                return performedMove;
            }
            else
                tempMove = null;

            return null;
             * */
        }

        public void Reset()
        {
            //reset to idle animation,
        }

        public ButtonInput StringToButtonInput(string seq)
        {
            ButtonInput input = new ButtonInput();

            string[] seqSections;
            if (seq.Contains("|")) //split if there is a line (multi button input)
            {
                seqSections = seq.Split('|');
            }
            else //else just use the one string
            {
                seqSections = new string[] { seq };
            }

            for (int i = 0; i < seqSections.Length; i++)
            {
                seqSections[i] = seqSections[i].Trim();
                switch (seqSections[i])
                {
                    case "B": //red
                        input.Red = true;
                        break;
                    case "Y": //yellow
                        input.Yellow = true;
                        break;
                    case "X": //blue
                        input.Blue = true;
                        break;
                }
            }
            return input;
        }
    }
}
