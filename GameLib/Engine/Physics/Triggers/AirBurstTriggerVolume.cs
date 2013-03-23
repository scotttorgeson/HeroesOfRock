using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace GameLib
{
    class AirBurstTriggerVolume : TriggerVolume
    {
        public float strength = 1.0f;
        public Vector3 direction = Vector3.Up;

        private Vector3 impulse;

        new public static void ParseParmSet(ref ParameterSet actorParm, ref ParameterSet worldParm)
        {
            if (worldParm.HasParm("Strength"))
                actorParm.AddParm("Strength", worldParm.GetFloat("Strength"));
            if (worldParm.HasParm("Direction"))
                actorParm.AddParm("Direction", worldParm.GetVector3("Direction"));
        }

        public override void Serialize(ref ParameterSet parm)
        {
            parm.AddParm("Strength", strength);
            parm.AddParm("Direction", direction);

            base.Serialize(ref parm);
        }

        public AirBurstTriggerVolume(Actor actor)
            : base(actor)
        {
            Name = "AirBurstTriggerVolume";
        }

        public override void Initialize(Stage stage)
        {
            if (actor.Parm.HasParm("Strength"))
                strength = actor.Parm.GetFloat("Strength");
            if (actor.Parm.HasParm("Direction"))
                direction = actor.Parm.GetVector3("Direction");

            System.Diagnostics.Debug.Assert(direction.Length() > 0.0f);
            System.Diagnostics.Debug.Assert(strength > 0.0f);

            direction.Normalize();

            impulse = direction * strength;

            UsingOnTriggerEnter = true;
            UsingOnTriggerStay = false;
            UsingOnTriggerExit = false;

            base.Initialize(stage);
        }

        public void BurstActor(Actor theActor)
        {
            if (theActor.PhysicsObject.CharacterController != null)
            {
                theActor.PhysicsObject.CharacterController.Body.ApplyLinearImpulse(ref impulse);
                float jumpFactor = theActor.PhysicsObject.CharacterController.JumpForceFactor;
                theActor.PhysicsObject.CharacterController.JumpForceFactor = strength;
                theActor.PhysicsObject.CharacterController.Jump();
                theActor.PhysicsObject.CharacterController.JumpForceFactor = jumpFactor;
            }
            else if (theActor.PhysicsObject.CylinderCharController != null)
            {
                theActor.PhysicsObject.CylinderCharController.Body.ApplyLinearImpulse(ref impulse);
            }
            else if (theActor.PhysicsObject.SpaceObject is BEPUphysics.Entities.Entity)
            {
                (theActor.PhysicsObject.SpaceObject as BEPUphysics.Entities.Entity).ApplyLinearImpulse(ref impulse);
            }
        }

        public override void OnTriggerEnter(Actor triggeringActor)
        {
            BurstActor(triggeringActor);
        }
    }
}
