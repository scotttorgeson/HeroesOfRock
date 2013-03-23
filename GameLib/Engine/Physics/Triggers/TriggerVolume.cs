using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

// inherit from this class to create specific trigger responses
// Implement OnTriggerEnter/OnTriggerStay/OnTriggerExit to respond to the trigger

// this must be attached to an actor with the PhysicsType of TriggerVolume

namespace GameLib
{
    public class TriggerVolume : Agent
    {
        private Actor collidingActor;
        protected bool DieOnTrigger = false;
        protected bool UsingOnTriggerEnter = true;
        protected bool UsingOnTriggerStay = true;
        protected bool UsingOnTriggerExit = true;

        private bool requiresButtonPush = false;
        private InputAction inputAction;

        public TriggerVolume(Actor actor)
            : base(actor)
        {
            if (Name == null)
                Name = "TriggerVolume"; // child class didn't name us (like they should have)
        }

        public override void Initialize(Stage stage)
        {
            if (actor.Parm.HasParm("DieOnTrigger"))
                DieOnTrigger = actor.Parm.GetBool("DieOnTrigger");

            // if there is an input action param, we require a button push to trigger
            if (actor.Parm.HasParm("InputAction"))
            {
                requiresButtonPush = true;
                ControlsQB cqb = stage.GetQB<ControlsQB>();
                inputAction = cqb.GetInputAction(actor.Parm.GetString("InputAction"));
            }

            // only pay for what we use
            if ( UsingOnTriggerEnter )
                actor.RegisterBeginCollideFunction(BeginCollide);
            if ( UsingOnTriggerExit )
                actor.RegisterEndCollideFunction(EndCollide);
            if ( UsingOnTriggerStay )
                actor.RegisterUpdateFunction(Update);
        }

        public void BeginCollide(Actor otherActor)
        {
            if (otherActor.PhysicsObject.CollisionInformation.CollisionRules.Group != PhysicsQB.playerGroup)
                return;
            collidingActor = otherActor;

            if (requiresButtonPush)
                if (!inputAction.IsNewAction)
                    return;
            OnTriggerEnter(otherActor);
            if (DieOnTrigger)
            {
                actor.MarkForDeath();                
            }
        }

        public void Update(float dt)
        {
            if (requiresButtonPush)
                if (!inputAction.IsNewAction)
                    return;
            if (collidingActor != null)
            {
                OnTriggerStay(collidingActor);
            }
        }

        public void EndCollide(Actor otherActor)
        {
            if (otherActor.PhysicsObject.CollisionInformation.CollisionRules.Group != PhysicsQB.playerGroup)
                return;
            if (requiresButtonPush)
            {
                if (!inputAction.IsNewAction)
                {
                    collidingActor = null;
                    return;
                }
            }
            OnTriggerExit(otherActor);
            collidingActor = null;
        }

        public virtual void OnTriggerEnter(Actor triggeringActor)
        {            
        }

        public virtual void OnTriggerStay(Actor triggeringActor)
        {
        }

        public virtual void OnTriggerExit(Actor triggeringActor)
        {
        }

        public virtual void Serialize(ref ParameterSet parm)
        {
            parm.AddParm("DieOnTrigger", DieOnTrigger);

            if (requiresButtonPush)
            {
                parm.AddParm("InputAction", inputAction.name);
            }
        }

        public static void ParseParmSet(ref ParameterSet actorParm, ref ParameterSet worldParm)
        {
            if (worldParm.HasParm("DieOnTrigger"))
                actorParm.AddParm("DieOnTrigger", worldParm.GetBool("DieOnTrigger"));
            if (worldParm.HasParm("InputAction"))
                actorParm.AddParm("InputAction", worldParm.GetString("InputAction"));
        }
    }
}
