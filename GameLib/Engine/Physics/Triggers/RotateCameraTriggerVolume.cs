using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

/*
 * Locks the camera when a player enters the trigger, unlocks it when they leave the trigger.
 * todo: let designer specify the position and direction to lock the camera to.
 *       possibly turn the trigger volume into a real collider to trap the player in it
 *       then detect when we should remove the collider and let the play continue?
 */

namespace GameLib
{
    class RotateCameraTriggerVolume : TriggerVolume
    {
        private Microsoft.Xna.Framework.Vector3 newForwardDir; //moving forward for our character actually means moving right
        private PlayerDirection newDir; //the direction characters will move in after the trigger
        private bool rotateRight;
        private float rotationTime;

        private bool triggered;

        public RotateCameraTriggerVolume(Actor actor)
            : base(actor)
        {
            Name = "RotateCameraTriggerVolume";
        }

        public override void Initialize(Stage stage)
        {
            triggered = false;
            UsingOnTriggerEnter = true;
            UsingOnTriggerStay = false;
            UsingOnTriggerExit = false;

            rotateRight = true;
            rotationTime = 1.0f;

            newForwardDir = actor.Parm.GetVector3("NewDir");
            newDir = Vector3ToMoveDir(newForwardDir);

            if (actor.Parm.HasParm("RotateRight"))
                rotateRight = actor.Parm.GetBool("RotateRight");
            if (actor.Parm.HasParm("RotationTime"))
                rotationTime = actor.Parm.GetFloat("RotationTime");
            base.Initialize(stage);
        }


        // this takes in any vector that has 2 zero elements and returns the corresponding movement direction
        // returns right if no conditions are met
        private PlayerDirection Vector3ToMoveDir(Microsoft.Xna.Framework.Vector3 dir)
        {
            if (dir.Y != 0)
            {
                return (dir.Y < 0) ? PlayerDirection.Down : PlayerDirection.Up;
            }
            else if (dir.Z != 0)
            {
                return (dir.Z < 0) ? PlayerDirection.Backward : PlayerDirection.Forward;
            }
            else
            {
                return (dir.X < 0) ? PlayerDirection.Left : PlayerDirection.Right;
            }
        }

        public override void OnTriggerEnter(Actor triggeringActor)
        {
            if (!triggered)
            {
                triggered = true;
                GameLib.Engine.AI.AIQB.MoveDirection = newDir;
                //make move direction static to reduce calling cost?
                PlayerAgent.Player.GetAgent<PlayerAgent>().ChangeMoveDirection(newDir, rotateRight, rotationTime);
            }
        }

        new public static void ParseParmSet(ref ParameterSet actorParm, ref ParameterSet worldParm)
        {
            System.Diagnostics.Debug.Assert(worldParm.HasParm("NewDir"), "Rotate Camera requires a new direction!");
            actorParm.AddParm("NewDir", worldParm.GetVector3("NewDir"));

            if (worldParm.HasParm("RotateRight"))
                actorParm.AddParm("RotateRight", worldParm.GetBool("RotateRight"));
            if(worldParm.HasParm("RotationTime"))
                actorParm.AddParm("RotationTime", worldParm.GetFloat("RotationTime"));
        }

        public override void Serialize(ref ParameterSet parm)
        {
            parm.AddParm("NewDir", newForwardDir);
            parm.AddParm("RotateRight", rotateRight);
            parm.AddParm("RotationTime", rotationTime);
            base.Serialize(ref parm);
        }
    }
}
