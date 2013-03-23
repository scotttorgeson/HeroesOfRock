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
        InputAction[] unpauseInput;
        InputAction strum;
        bool strumRequired;
        bool allInputsRequired;
        bool killEnemies;
        bool spawnEnemy;
        bool delayNextTrigger;
        float triggerDelay;
        bool finished;
        bool performUpdate;
        bool returnToMenu;

        //variables specifying the current progress
        bool triggered;

        public int index { get; private set; }
        public bool active;

        public Microsoft.Xna.Framework.Graphics.Texture2D tutImg { get; private set; }
        public Rectangle imgDim { get; private set; }
        public bool moveHereSign { get; private set; }


        /// <summary>
        /// Read trigger specific parameters from the world parm and add them to the actor parm
        /// </summary>
        /// <param name="parm"></param>
        /// <returns></returns>
        new public static void ParseParmSet(ref ParameterSet actorParm, ref ParameterSet worldParm)
        {
            System.Diagnostics.Debug.Assert(worldParm.HasParm("ControllerInput"), "TutorialStopTriggerVolume requires a ControllerInput!");
            actorParm.AddParm("ControllerInput", worldParm.GetString("ControllerInput"));
            System.Diagnostics.Debug.Assert(worldParm.HasParm("GuitarInput"), "TutorialStopTriggerVolume requires a GuitarInput!");
            actorParm.AddParm("GuitarInput", worldParm.GetString("GuitarInput"));
            System.Diagnostics.Debug.Assert(worldParm.HasParm("Index"), "TutorialStopTriggerVolume requires an index!");
            actorParm.AddParm("Index", worldParm.GetInt("Index"));

            System.Diagnostics.Debug.Assert(worldParm.HasParm("ControllerImage"), "TutorialStopTriggerVolume requires a ControllerImage!");
            actorParm.AddParm("ControllerImage", worldParm.GetString("ControllerImage"));
            System.Diagnostics.Debug.Assert(worldParm.HasParm("GuitarImage"), "TutorialStopTriggerVolume requires a GuitarImage!");
            actorParm.AddParm("GuitarImage", worldParm.GetString("GuitarImage"));

            System.Diagnostics.Debug.Assert(worldParm.HasParm("ControllerImagePos"), "TutorialStopTriggerVolume requires a ControllerImagePos!");
            actorParm.AddParm("ControllerImagePos", worldParm.GetString("ControllerImagePos"));
            System.Diagnostics.Debug.Assert(worldParm.HasParm("ControllerImageSize"), "TutorialStopTriggerVolume requires a ControllerImageSize!");
            actorParm.AddParm("ControllerImageSize", worldParm.GetString("ControllerImageSize"));
            System.Diagnostics.Debug.Assert(worldParm.HasParm("GuitarImagePos"), "TutorialStopTriggerVolume requires a GuitarImagePos!");
            actorParm.AddParm("GuitarImagePos", worldParm.GetString("GuitarImagePos"));
            System.Diagnostics.Debug.Assert(worldParm.HasParm("GuitarImageSize"), "TutorialStopTriggerVolume requires a GuitarImageSize!");
            actorParm.AddParm("GuitarImageSize", worldParm.GetString("GuitarImageSize"));
            

            if (worldParm.HasParm("StrumRequired"))
                actorParm.AddParm("StrumRequired",worldParm.GetBool("StrumRequired"));

            if (worldParm.HasParm("AllInputsRequired"))
                actorParm.AddParm("AllInputsRequired", worldParm.GetBool("AllInputsRequired"));

            if(worldParm.HasParm("MoveHereSign"))
                actorParm.AddParm("MoveHereSign", worldParm.GetBool("MoveHereSign"));

            if(worldParm.HasParm("SpawnEnemy"))
                actorParm.AddParm("SpawnEnemy", worldParm.GetBool("SpawnEnemy"));

            if(worldParm.HasParm("KillEnemies"))
                actorParm.AddParm("KillEnemies", worldParm.GetBool("KillEnemies"));
            if (worldParm.HasParm("TriggerDelay"))
                actorParm.AddParm("TriggerDelay", worldParm.GetFloat("TriggerDelay"));
            if (worldParm.HasParm("PerformUpdate"))
                actorParm.AddParm("PerformUpdate", worldParm.GetBool("PerformUpdate"));
            if (worldParm.HasParm("EndLevel"))
                actorParm.AddParm("EndLevel", worldParm.GetBool("EndLevel"));
        }

        public TutorialStopTriggerVolume(Actor actor)
            : base(actor)
        {
            Name = "TutorialStopTriggerVolume";
        }

        public override void Initialize(Stage stage)
        {
            strumRequired = allInputsRequired = spawnEnemy = killEnemies = false;
            index = actor.Parm.GetInt("Index");
            if (index == 0)
                active = true;

            //get the control scheme
            Microsoft.Xna.Framework.Input.GamePadType gamePadType = Stage.ActiveStage.GetQB<ControlsQB>().GetGamePadType();

            Vector2 pos, size;
            switch (gamePadType)
            {
                case Microsoft.Xna.Framework.Input.GamePadType.Guitar:
                case Microsoft.Xna.Framework.Input.GamePadType.AlternateGuitar:
                    tutImg = Stage.Content.Load<Microsoft.Xna.Framework.Graphics.Texture2D>("UI/Tutorial/" + actor.Parm.GetString("GuitarImage"));
                    pos = actor.Parm.GetVector2("GuitarImagePos");
                    size = actor.Parm.GetVector2("GuitarImageSize");
                    imgDim = new Rectangle((int)pos.X, (int)pos.Y, (int)size.X, (int)size.Y);
                    break;
                default:
                    tutImg = Stage.Content.Load<Microsoft.Xna.Framework.Graphics.Texture2D>("UI/Tutorial/" + actor.Parm.GetString("ControllerImage"));
                    pos = actor.Parm.GetVector2("ControllerImagePos");
                    size = actor.Parm.GetVector2("ControllerImageSize");
                    imgDim = new Rectangle((int)pos.X, (int)pos.Y, (int)size.X, (int)size.Y);
                    break;
            }

            if(actor.Parm.HasParm("AllInputsRequired"))
                allInputsRequired = actor.Parm.GetBool("AllInputsRequired");

            if (actor.Parm.HasParm("SpawnEnemy"))
                spawnEnemy = actor.Parm.GetBool("SpawnEnemy");
            if (actor.Parm.HasParm("KillEnemies"))
                killEnemies = actor.Parm.GetBool("KillEnemies");

            if (actor.Parm.HasParm("MoveHereSign"))
                moveHereSign = actor.Parm.GetBool("MoveHereSign");

            if (actor.Parm.HasParm("TriggerDelay"))
            {
                delayNextTrigger = true;
                triggerDelay = actor.Parm.GetFloat("TriggerDelay");
            }

            if (actor.Parm.HasParm("PerformUpdate"))
                performUpdate = actor.Parm.GetBool("PerformUpdate");

            if (actor.Parm.HasParm("EndLevel"))
                returnToMenu = actor.Parm.GetBool("EndLevel");

            ControlsQB cQB = stage.GetQB<ControlsQB>();
            Microsoft.Xna.Framework.Input.GamePadType gp = cQB.GetGamePadType();
            switch (gp)
            {
                case Microsoft.Xna.Framework.Input.GamePadType.GamePad:
                    GetInput(actor.Parm.GetString("ControllerInput"), ref cQB);
                    break;
                case Microsoft.Xna.Framework.Input.GamePadType.Guitar:
                    if (actor.Parm.HasParm("StrumRequired") && actor.Parm.GetBool("StrumRequired"))
                    {
                        strumRequired = true;
                        strum = cQB.GetInputAction("Strum");
                    }
                    GetInput(actor.Parm.GetString("GuitarInput"), ref cQB);
                    break;
                case Microsoft.Xna.Framework.Input.GamePadType.AlternateGuitar:
                    goto case Microsoft.Xna.Framework.Input.GamePadType.Guitar;
                case Microsoft.Xna.Framework.Input.GamePadType.Unknown:
                    goto case Microsoft.Xna.Framework.Input.GamePadType.GamePad;
            }

            actor.RegisterUpdateFunction(Update);

            UsingOnTriggerEnter = true;
            UsingOnTriggerStay = true;
            UsingOnTriggerExit = true;

            base.Initialize(stage);
        }

        private void GetInput(string input, ref ControlsQB cQB)
        {
            
            if (input.Contains(','))
            {
                string[] splitInput = input.Split(',');
                unpauseInput = new InputAction[splitInput.Length];
                for (int i = 0; i < splitInput.Length; i++)
                {
                    unpauseInput[i] = cQB.GetInputAction(splitInput[i]);
                }
            }
            else
            {
                unpauseInput = new InputAction[1];
                unpauseInput[0] = cQB.GetInputAction(input);
            }
        }

        new public void Update(float dt)
        {
            if (!triggered) return;
            if (finished)
            {
                triggerDelay -= dt;
                if (triggerDelay <= 0)
                {
                    active = false;
                    Stage.ActiveStage.GetQB<TriggerQB>().ActivateNextTutorialTrigger(index);
                    actor.MarkForDeath();
                }
            }
            bool metRequirements = false;
            if (strumRequired)
            {
                if (strum.IsNewAction)
                {
                    if (unpauseInput.Length == 1 && unpauseInput[0].IsNewAction)
                    {
                        UnLock(dt);
                        return;
                    }
                    else if (allInputsRequired) //have to make sure all buttons are pressed
                    {
                        metRequirements = true;
                        for (int i = 0; i < unpauseInput.Length; i++)
                            if (unpauseInput[i].value == 0) metRequirements = false;
                    }
                    else //we only care if any of the buttons are pressed
                    {
                        for (int i = 0; i < unpauseInput.Length; i++)
                            if (unpauseInput[i].value != 0) metRequirements = true;
                    }
                    if (metRequirements) UnLock(dt);
                }
            }
            else
            {
                if (unpauseInput.Length == 1 && unpauseInput[0].IsNewAction)
                {
                    UnLock(dt);
                    return;
                }
                else if (allInputsRequired)
                {
                    metRequirements = true;
                    for (int i = 0; i < unpauseInput.Length; i++)
                        if (unpauseInput[i].value == 0) metRequirements = false;
                }
                else
                {
                    for (int i = 0; i < unpauseInput.Length; i++)
                        if (unpauseInput[i].value != 0) metRequirements = true;
                }
                if (metRequirements) UnLock(dt);
            }
        }

        public override void OnTriggerStay(Actor triggeringActor)
        {
            if (active && !triggered && triggeringActor.PhysicsObject.CollisionInformation.CollisionRules.Group == PhysicsQB.playerGroup)
            {
                triggered = true;

                Lock();
                
            }
            base.OnTriggerStay(triggeringActor);
        }

        private void UnLock(float dt)
        {
            PlayerAgent c = PlayerAgent.Player.GetAgent<PlayerAgent>();
            
            if(performUpdate)
                c.Update(dt); //call update again so that the move happens

            if (delayNextTrigger)
            {
                if (triggerDelay == -1)
                    triggerDelay = c.TimeLeftInCurrentAction() + .05f;
                else if(triggerDelay == -2)
                    triggerDelay = c.TimeToFlow() + .35f;
                finished = true;
            }
            else
            {
                active = false;
                Stage.ActiveStage.GetQB<TriggerQB>().ActivateNextTutorialTrigger(index);
                actor.MarkForDeath();
            }
            Stage.ActiveStage.ResumeGame();
            Stage.ActiveStage.GetQB<TriggerQB>().tutorialPic.showing = false;

            if (killEnemies)
                Stage.ActiveStage.GetQB<AIQB>().KillAll();

            if (returnToMenu)
            {
                Stage.LoadStage("MainMenu", true);
                Stage.ActiveStage.GetQB<GameLib.Engine.MenuSystem.MenuSystemQB>().GoToLevelSelect();
            }
        }

        private void Lock()
        {
            Stage.ActiveStage.PauseGame();

            Stage.ActiveStage.GetQB<TriggerQB>().tutorialPic.changeImage(tutImg, imgDim, true);

            if (spawnEnemy)
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
            parm.AddParm("Index", index);

            parm.AddParm("GuitarImage", actor.Parm.GetString("GuitarImage"));
            parm.AddParm("ControllerImage", actor.Parm.GetString("ControllerImage"));

            parm.AddParm("MoveHereSign", moveHereSign);

            parm.AddParm("StrumRequired", strumRequired);
            parm.AddParm("AllInputsRequired", allInputsRequired);
            parm.AddParm("SpawnEnemy", spawnEnemy);
            parm.AddParm("KillEnemies", killEnemies);

            base.Serialize(ref parm);
        }
    }
}
