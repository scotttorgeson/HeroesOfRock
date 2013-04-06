//#define INPUTREQUIRED

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using GameLib.Engine.AI;


namespace GameLib
{
    public class TutorialStopTriggerVolume : TriggerVolume
    {
#if INPUTREQUIRED
        InputAction[][] unpauseInput;
        bool[] allInputsRequired;
#else
        InputAction Green;
#endif
        InputAction strum;
        InputAction back;
        bool[] killEnemies;
        bool[] spawnEnemy;
        float[] triggerDelay;
        bool finished;
        bool returnToMenu;
        ControlsQB cQB;

        //variables specifying the current progress
        bool triggered;

        int tutIndex;
        int numTuts;
        float timer;

        public Microsoft.Xna.Framework.Graphics.Texture2D[] tutImg { get; private set; }
        public Rectangle[] imgDim { get; private set; }


        /// <summary>
        /// Read trigger specific parameters from the world parm and add them to the actor parm
        /// </summary>
        /// <param name="parm"></param>
        /// <returns></returns>
        new public static void ParseParmSet(ref ParameterSet actorParm, ref ParameterSet worldParm)
        {
            //check for the bare minimum required parms
            System.Diagnostics.Debug.Assert(worldParm.HasParm("ControllerInput0"), "TutorialStopTriggerVolume requires a ControllerInput0!");
            System.Diagnostics.Debug.Assert(worldParm.HasParm("GuitarInput0"), "TutorialStopTriggerVolume requires a GuitarInput0!");

            System.Diagnostics.Debug.Assert(worldParm.HasParm("ControllerImage0"), "TutorialStopTriggerVolume requires a ControllerImage0!");
            System.Diagnostics.Debug.Assert(worldParm.HasParm("GuitarImage0"), "TutorialStopTriggerVolume requires a GuitarImage0!");

            System.Diagnostics.Debug.Assert(worldParm.HasParm("ControllerImagePos0"), "TutorialStopTriggerVolume requires a ControllerImagePos0!");
            System.Diagnostics.Debug.Assert(worldParm.HasParm("ControllerImageSize0"), "TutorialStopTriggerVolume requires a ControllerImageSize0!");
            System.Diagnostics.Debug.Assert(worldParm.HasParm("GuitarImagePos0"), "TutorialStopTriggerVolume requires a GuitarImagePos0!");
            System.Diagnostics.Debug.Assert(worldParm.HasParm("GuitarImageSize0"), "TutorialStopTriggerVolume requires a GuitarImageSize0!");

            int count = 0;

            while (worldParm.HasParm("ControllerInput"+count))
            {
                actorParm.AddParm("ControllerInput"+count, worldParm.GetString("ControllerInput"+count));

                actorParm.AddParm("GuitarInput"+count, worldParm.GetString("GuitarInput"+count));

                actorParm.AddParm("ControllerImage"+count, worldParm.GetString("ControllerImage"+count));

                actorParm.AddParm("GuitarImage"+count, worldParm.GetString("GuitarImage"+count));

                actorParm.AddParm("ControllerImagePos"+count, worldParm.GetString("ControllerImagePos"+count));
                actorParm.AddParm("ControllerImageSize"+count, worldParm.GetString("ControllerImageSize"+count));
                actorParm.AddParm("GuitarImagePos"+count, worldParm.GetString("GuitarImagePos"+count));
                actorParm.AddParm("GuitarImageSize"+count, worldParm.GetString("GuitarImageSize"+count));

                //optional parms
                if (worldParm.HasParm("StrumRequired"+count))
                    actorParm.AddParm("StrumRequired"+count, worldParm.GetBool("StrumRequired"+count));

                if (worldParm.HasParm("AllInputsRequired"+count))
                    actorParm.AddParm("AllInputsRequired"+count, worldParm.GetBool("AllInputsRequired"+count));

                if (worldParm.HasParm("SpawnEnemy"+count))
                    actorParm.AddParm("SpawnEnemy"+count, worldParm.GetBool("SpawnEnemy"+count));

                if (worldParm.HasParm("KillEnemies"+count))
                    actorParm.AddParm("KillEnemies"+count, worldParm.GetBool("KillEnemies"+count));
                if (worldParm.HasParm("TriggerDelay"+count))
                    actorParm.AddParm("TriggerDelay"+count, worldParm.GetFloat("TriggerDelay"+count));

                count++;
            }


            if (worldParm.HasParm("EndLevel"))
                actorParm.AddParm("EndLevel", worldParm.GetBool("EndLevel"));

            actorParm.AddParm("Num", count);
        }

        public TutorialStopTriggerVolume(Actor actor)
            : base(actor)
        {
            Name = "TutorialStopTriggerVolume";
        }

        public override void Initialize(Stage stage)
        {
            numTuts = actor.Parm.GetInt("Num");

#if INPUTREQUIRED
            unpauseInput = new InputAction[numTuts][];
            allInputsRequired = new bool[numTuts];
#endif

            killEnemies = new bool[numTuts];
            spawnEnemy = new bool[numTuts];
            triggerDelay = new float[numTuts];
            tutImg = new Microsoft.Xna.Framework.Graphics.Texture2D[numTuts];
            imgDim = new Rectangle[numTuts];

            //get the control scheme
            cQB = stage.GetQB<ControlsQB>();

#if !INPUTREQUIRED
            Green = cQB.GetInputAction("A");
            strum = cQB.GetInputAction("Strum");
#endif

            Microsoft.Xna.Framework.Input.GamePadType gp = cQB.GetGamePadType();

            Vector2 pos, size;
            switch (gp)
            {
                case Microsoft.Xna.Framework.Input.GamePadType.Guitar:
                case Microsoft.Xna.Framework.Input.GamePadType.AlternateGuitar:
                    for (int j = 0; j < numTuts; j++)
                    {
#if INPUTREQUIRED
                        GetInput(j,actor.Parm.GetString("GuitarInput" + j), ref cQB);
#endif
                        tutImg[j] = Stage.Content.Load<Microsoft.Xna.Framework.Graphics.Texture2D>("UI/Tutorial/" + actor.Parm.GetString("GuitarImage" + j));
                        pos = actor.Parm.GetVector2("GuitarImagePos" + j);
                        size = actor.Parm.GetVector2("GuitarImageSize" + j);
                        imgDim[j] = new Rectangle((int)pos.X, (int)pos.Y, (int)size.X, (int)size.Y);
                    }
                    break;
                default:
                    for (int j = 0; j < numTuts; j++)
                    {
#if INPUTREQUIRED
                        GetInput(j, actor.Parm.GetString("ControllerInput" + j), ref cQB);
#endif
                        tutImg[j] = Stage.Content.Load<Microsoft.Xna.Framework.Graphics.Texture2D>("UI/Tutorial/" + actor.Parm.GetString("ControllerImage" + j));
                        pos = actor.Parm.GetVector2("ControllerImagePos" + j);
                        size = actor.Parm.GetVector2("ControllerImageSize" + j);
                        imgDim[j] = new Rectangle((int)pos.X, (int)pos.Y, (int)size.X, (int)size.Y);
                    }
                    break;
            }

            for (int i = 0; i < numTuts; i++)
            {
#if INPUTREQUIRED
                if (actor.Parm.HasParm("AllInputsRequired" + i))
                    allInputsRequired[i] = actor.Parm.GetBool("AllInputsRequired" + i);
                else
                    allInputsRequired[i] = false;
#endif
                if (actor.Parm.HasParm("SpawnEnemy" + i))
                    spawnEnemy[i] = actor.Parm.GetBool("SpawnEnemy" + i);
                else
                    spawnEnemy[i] = false;

                if (actor.Parm.HasParm("KillEnemies" + i))
                    killEnemies[i] = actor.Parm.GetBool("KillEnemies" + i);
                else
                    killEnemies[i] = false;

                if (actor.Parm.HasParm("TriggerDelay" + i))
                    triggerDelay[i] = actor.Parm.GetFloat("TriggerDelay" + i);
                else
                    triggerDelay[i] = 0;
            }

            if (actor.Parm.HasParm("EndLevel"))
                returnToMenu = actor.Parm.GetBool("EndLevel");

            actor.RegisterUpdateFunction(Update);

            UsingOnTriggerEnter = true;
            UsingOnTriggerStay = false;
            UsingOnTriggerExit = false;

            base.Initialize(stage);
        }

        private void GetInput(int index, string input, ref ControlsQB cQB)
        {
#if INPUTREQUIRED
            if (input.Contains(','))
            {
                string[] splitInput = input.Split(',');
                unpauseInput[index] = new InputAction[splitInput.Length];
                for (int i = 0; i < splitInput.Length; i++)
                {
                    unpauseInput[index][i] = cQB.GetInputAction(splitInput[i]);
                }
            }
            else
            {
                unpauseInput[index] = new InputAction[1];
                unpauseInput[index][0] = cQB.GetInputAction(input);
            }
#endif
        }

        new public void Update(float dt)
        {
            if (!triggered) return;
            if (finished)
            {
                timer -= dt;
                if (timer <= 0)
                {
                    Lock();
                }
            }
            
            bool metRequirements = false;

            Microsoft.Xna.Framework.Input.GamePadType gp = cQB.GetGamePadType();
            if (gp == Microsoft.Xna.Framework.Input.GamePadType.Guitar ||
                gp == Microsoft.Xna.Framework.Input.GamePadType.AlternateGuitar)
            {
                if (strum.IsNewAction)
                {
#if INPUTREQUIRED
                    if (unpauseInput.Length == 1 && unpauseInput[tutIndex][0].IsNewAction)
                    {
                        UnLock(dt);
                        return;
                    }
                    else if (allInputsRequired[tutIndex]) //have to make sure all buttons are pressed
                    {
                        metRequirements = true;
                        for (int i = 0; i < unpauseInput.Length; i++)
                            if (unpauseInput[tutIndex][i].value == 0) metRequirements = false;
                    }
                    else //we only care if any of the buttons are pressed
                    {
                        for (int i = 0; i < unpauseInput.Length; i++)
                            if (unpauseInput[tutIndex][i].value != 0) metRequirements = true;
                    }
#else
                    metRequirements = (Green.value != 0);
#endif
                    if (metRequirements) UnLock(dt);
                }
            }
            else
            {
#if INPUTREQUIRED
                if (unpauseInput.Length == 1 && unpauseInput[tutIndex][0].IsNewAction)
                {
                    UnLock(dt);
                    return;
                }
                else if (allInputsRequired[tutIndex])
                {
                    metRequirements = true;
                    for (int i = 0; i < unpauseInput.Length; i++)
                        if (unpauseInput[tutIndex][i].value == 0) metRequirements = false;
                }
                else
                {
                    for (int i = 0; i < unpauseInput.Length; i++)
                        if (unpauseInput[tutIndex][i].value != 0) metRequirements = true;
                }
#else
                metRequirements = Green.IsNewAction;
#endif
                if (metRequirements) UnLock(dt);
            }
        }

        public override void OnTriggerEnter(Actor triggeringActor)
        {
            if (!triggered && triggeringActor.PhysicsObject.CollisionInformation.CollisionRules.Group == PhysicsQB.playerGroup)
            {
                triggered = true;

                Lock();
                
            }
            base.OnTriggerStay(triggeringActor);
        }

        private void UnLock(float dt)
        {

            finished = true;
            timer = triggerDelay[tutIndex];

            Stage.ActiveStage.GetQB<TriggerQB>().tutorialPic.showing = false;

            if (killEnemies[tutIndex])
                Stage.ActiveStage.GetQB<AIQB>().KillAll();

            if (++tutIndex >= numTuts && returnToMenu)
            {
                Stage.LoadStage("MainMenu", true);
                Stage.ActiveStage.GetQB<GameLib.Engine.MenuSystem.MenuSystemQB>().GoToLevelSelect();
            }
        }

        private void Lock()
        {

            Stage.ActiveStage.GetQB<TriggerQB>().tutorialPic.changeImage(tutImg[tutIndex], imgDim[tutIndex], true);

            if (spawnEnemy[tutIndex])
            {
                Vector3 pos = actor.PhysicsObject.Position;
                pos.Y += .5f;
                Vector3 rot = Vector3.Zero;
                Stage.ActiveStage.GetQB<AIQB>().Spawn("EnemyDumb", ref pos, ref rot, -1);
            }
        }

        //this will break the game
        public override void Serialize(ref ParameterSet parm)
        {
            parm.AddParm("ControllerInput", actor.Parm.GetString("ControllerInput"));
            parm.AddParm("GuitarInput", actor.Parm.GetString("GuitarInput"));

            parm.AddParm("GuitarImage", actor.Parm.GetString("GuitarImage"));
            parm.AddParm("ControllerImage", actor.Parm.GetString("ControllerImage"));

            parm.AddParm("SpawnEnemy", spawnEnemy);
            parm.AddParm("KillEnemies", killEnemies);

            base.Serialize(ref parm);
        }
    }
}
