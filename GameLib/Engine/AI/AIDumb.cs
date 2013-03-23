using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace GameLib.Engine.AI
{
    class AIDumb : AI
    {

        public AIDumb(Actor actor) :
            base(actor)
        {
            //assign the correct values for the actor
            speed = 0;
            attackRange = 0;
        }

        public override void Initialize(Stage stage)
        {
            base.Initialize(stage);

            if (actor.Parm.HasParm("AddToAIQB") && actor.Parm.GetBool("AddToAIQB"))
                stage.GetQB<AIQB>().AddLiveEnemy(actor);

            if (actor.PhysicsObject.physicsType == PhysicsObject.PhysicsType.Box)
            {
                //if we are a platform

                if (actor.Parm.HasParm("Platform") && actor.Parm.GetBool("Platform"))
                {
                    actor.PhysicsObject.CollisionInformation.CollisionRules.Group = PhysicsQB.platformGroup;
                    bloodOnDamage = false;
                }
                else
                    actor.PhysicsObject.CollisionInformation.CollisionRules.Group = PhysicsQB.normalAIGroup;

                actor.PhysicsObject.CollisionInformation.Entity.IsAffectedByGravity = false;
                stunnable = false;
            }
            else if(actor.PhysicsObject.physicsType == PhysicsObject.PhysicsType.CylinderCharacter)
            {
                actor.PhysicsObject.CollisionInformation.CollisionRules.Group = PhysicsQB.normalAIGroup;
                actor.PhysicsObject.CylinderCharController.HorizontalMotionConstraint.Speed = 0;
            }
        }

        //dumb ai's don't move or attack
        public override void Attack(ref Vector3 place) { }
        public override float StartAttack(ref Vector3 place) { return 0; }
        public override void MoveTowards(ref Vector3 dest) { }
        public override void Update(float dt) { }
        public override void ConvertTo(EnemyType e, ref ParameterSet parm) { }
    }
}