using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GameLib
{
    class HealthTriggerVolume : TriggerVolume
    {
        InputAction inputAction;
        bool killOnButtonPush = true;
        int heal = 100;

        /// <summary>
        /// Read trigger specific parameters from the world parm and add them to the actor parm
        /// </summary>
        /// <param name="parm"></param>
        /// <returns></returns>
        new public static void ParseParmSet(ref ParameterSet actorParm, ref ParameterSet worldParm)
        {
            System.Diagnostics.Debug.Assert(worldParm.HasParm("InputAction"), "HealthTriggerVolume requires an input action!");
            actorParm.AddParm("InputAction", worldParm.GetString("InputAction"));

            if (worldParm.HasParm("KillOnButtonPush"))
                actorParm.AddParm("KillOnButtonPush", worldParm.GetBool("KillOnButtonPush"));

            if (worldParm.HasParm("Heal"))
                actorParm.AddParm("Heal", worldParm.GetInt("Heal"));
        }

        public HealthTriggerVolume(Actor actor)
            : base(actor)
        {
            Name = "HealthTriggerVolume";            
        }

        public override void Initialize(Stage stage)
        {
            UsingOnTriggerEnter = true;
            UsingOnTriggerExit = false;
            UsingOnTriggerStay = true;

            string inputActionName = actor.Parm.GetString("InputAction");
            ControlsQB cqb = Stage.ActiveStage.GetQB<ControlsQB>();
            inputAction = cqb.GetInputAction(inputActionName);

            System.Diagnostics.Debug.Assert(inputAction != null);


            if (actor.Parm.HasParm("KillOnButtonPush"))
                killOnButtonPush = actor.Parm.GetBool("KillOnButtonPush");

            if (actor.Parm.HasParm("Heal"))
                heal = actor.Parm.GetInt("Heal");

            base.Initialize(stage); // let the trigger volume base class initialize itself
        }

        public override void OnTriggerStay(Actor triggeringActor)
        {
                //super broken, needs to be fixed if we want to use it
                /*if (inputAction.value != 0.0f && inputAction.value != inputAction.lastValue)
                {
                    foreach (Agent agent in triggeringActor.agents)
                    {
                        if (agent.Name == "HealthAgent")
                        {
                            HealthAgent healthAgent = (HealthAgent)agent;
                            healthAgent.ModifyHealth((float)heal, actor.PhysicsObject.Position);
                            if (killOnButtonPush)
                            {
                                actor.MarkForDeath();
                            }
                            return;
                        }
                    }
                }*/
        }

        public override void Serialize(ref ParameterSet parm)
        {
            parm.AddParm("InputAction", inputAction.name);
            parm.AddParm("KillOnButtonPush", killOnButtonPush);
            parm.AddParm("Heal", heal);
            base.Serialize(ref parm);
        }
    
    }
}
