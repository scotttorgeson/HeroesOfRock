using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace GameLib
{
    class EndLevelTriggerVolume : TriggerVolume
    {

        bool triggered;

        new public static void ParseParmSet(ref ParameterSet actorParm, ref ParameterSet worldParm)
        {
            
        }

        public override void Serialize(ref ParameterSet parm)
        {

            base.Serialize(ref parm);
        }

        public EndLevelTriggerVolume(Actor actor)
            : base(actor)
        {
            Name = "EndLevelTriggerVolume";
        }

        public override void Initialize(Stage stage)
        {
            triggered = false;

            UsingOnTriggerEnter = true;
            UsingOnTriggerStay = false;
            UsingOnTriggerExit = false;

            base.Initialize(stage);
        }

        public override void OnTriggerEnter(Actor triggeringActor)
        {
            if (!triggered)
            {
                triggered = true;
                Stage.ActiveStage.GetQB<GameLib.Engine.MenuSystem.MenuSystemQB>().ShowEndLevelScreen();
            }
        }
    }
}
