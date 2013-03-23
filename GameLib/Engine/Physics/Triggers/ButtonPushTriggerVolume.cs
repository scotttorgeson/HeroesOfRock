using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GameLib
{
    // calls Trigger() when InputAction.IsNewAction is true and something is in the trigger
    // todo: add filtering for who is in the trigger, better support for implementing a response
    class ButtonPushTriggerVolume : TriggerVolume
    {
        InputAction inputAction;
        bool killOnButtonPush = false;

        public ButtonPushTriggerVolume(Actor actor)
            : base(actor)
        {
            Name = "ButtonPushTriggerVolume";
        }

        new public static void ParseParmSet(ref ParameterSet actorParm, ref ParameterSet worldParm)
        {
            System.Diagnostics.Debug.Assert(worldParm.HasParm("InputAction"), "ButtonPushTriggerVolume requires an input action!");
            actorParm.AddParm("InputAction", worldParm.GetString("InputAction"));

            if (worldParm.HasParm("KillOnButtonPush"))
                actorParm.AddParm("KillOnButtonPush", worldParm.GetBool("KillOnButtonPush"));
        }

        public override void Serialize(ref ParameterSet parm)
        {
            parm.AddParm("InputAction", inputAction.name);
            parm.AddParm("KillOnButtonPush", killOnButtonPush);

            base.Serialize(ref parm);
        }

        public sealed override void Initialize(Stage stage)
        {
            UsingOnTriggerEnter = false;
            UsingOnTriggerStay = true;
            UsingOnTriggerExit = false;

            string inputActionName = actor.Parm.GetString("InputAction");
            ControlsQB cqb = Stage.ActiveStage.GetQB<ControlsQB>();
            inputAction = cqb.GetInputAction(inputActionName);

            System.Diagnostics.Debug.Assert(inputAction != null);

            if (actor.Parm.HasParm("KillOnButtonPush"))
                killOnButtonPush = actor.Parm.GetBool("KillOnButtonPush");

            base.Initialize(stage);
        }

        public virtual void Trigger()
        {

        }

        public override void OnTriggerStay(Actor triggeringActor)
        {
            if (inputAction.IsNewAction)
            {
                Trigger();
            }
        }
    }
}
