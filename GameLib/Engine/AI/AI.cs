using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace GameLib.Engine.AI
{
    public enum EnemyType
    {
        Dumb = 0,
        Weak,
        WeakShielded,
        Heavy,
        HeavyAOE,
        Ranged,
        RangedPogo,
        Missile
    }
    public abstract class AI : Agent
    {
        
        public enum AIState
        {
            Idle = 0,
            Moving,
            Attacking,
            Stunned,
            Turning,
            WaitingToAttack,
            Jumping
        }

        public int spawnerIndex;
        public float speed { get; protected set; }
        public float attackDmg { get; protected set; }
        public float attackRange { get; protected set; }
        public float jumpSpeed { get; protected set; }
        public float percAnimBeforeAttack { get; protected set; }
        public int pointsForKilling { get; protected set; }
        public bool bloodOnDamage { get; protected set; }
        public AIState state { get; protected set; }
        public EnemyType type { get; protected set; }
        public bool spawnedFromTrigger;
        public Vector3 spawnPos;

        protected bool stunnable = true;

        protected Actor target;
        protected bool shouldAttack;
        protected bool hasAttacked;
        protected float timer;
        protected float animationTime;

        protected float attackTime;

        public AI(Actor actor) :
            base(actor)
        {
            actor.RegisterReviveFunction(Revive);
        }

        public void giveNewTarget(Actor newTarget, bool attackTarget)
        {
            target = newTarget;
            shouldAttack = attackTarget;
            state = AIState.Moving;
        }

        protected void DoneWithTarget()
        {
            target = null;
            shouldAttack = false;
        }

        public abstract void Update(float dt);
        public abstract void MoveTowards(ref Vector3 dest);
        public abstract void Attack(ref Vector3 place);
        public abstract float StartAttack(ref Vector3 place);
        public abstract void ConvertTo(EnemyType e, ref ParameterSet parm);
        public void Stun(float length)
        {
            if (type == EnemyType.Heavy && state == AIState.Attacking || stunnable == false) return;
            animationTime = length;
            timer = length;
            actor.PhysicsObject.CylinderCharController.HorizontalMotionConstraint.MovementDirection = Vector2.Zero; //stop their movement
            state = AIState.Stunned;
        }

        public override void Initialize(Stage stage)
        {
            spawnedFromTrigger = false;
            bloodOnDamage = true;
            if (actor.Parm.HasParm("Speed"))
                speed = actor.Parm.GetFloat("Speed");
            if (actor.Parm.HasParm("AttackDamage"))
                attackDmg = actor.Parm.GetFloat("AttackDamage");
            if(actor.Parm.HasParm("AttackRange"))
                attackRange = (float)Math.Pow(actor.Parm.GetFloat("AttackRange"),2);
            if (actor.Parm.HasParm("PercAnimationBeforeAttack"))
                percAnimBeforeAttack = actor.Parm.GetFloat("PercAnimationBeforeAttack");
            if (actor.Parm.HasParm("TimeForAttack"))
                attackTime = actor.Parm.GetFloat("TimeForAttack");

            pointsForKilling = 50;
            if (actor.Parm.HasParm("PointsForKilling"))
                pointsForKilling = actor.Parm.GetInt("PointsForKilling");
            spawnerIndex = -1;
        }

        protected void FaceTargetSnappedToPlane()
        {
            if (actor.PhysicsObject.physicsType != PhysicsObject.PhysicsType.CylinderCharacter)
                return;

            switch(AIQB.MoveDirection)
            {
                case PlayerDirection.Left:
                    goto case PlayerDirection.Right;
                case PlayerDirection.Right:
                    if(target.PhysicsObject.Position.X > actor.PhysicsObject.Position.X)
                        actor.PhysicsObject.CylinderCharController.Body.Orientation = PlayerAgent.faceRight;
                    else
                        actor.PhysicsObject.CylinderCharController.Body.Orientation = PlayerAgent.faceLeft;
                    break;
                case PlayerDirection.Forward:
                    goto case PlayerDirection.Backward;
                case PlayerDirection.Backward:
                    if (target.PhysicsObject.Position.Z > actor.PhysicsObject.Position.Z)
                        actor.PhysicsObject.CylinderCharController.Body.Orientation = PlayerAgent.faceBackward;
                    else
                        actor.PhysicsObject.CylinderCharController.Body.Orientation = PlayerAgent.faceForward;
                    break;
            }
        }

        protected void FacePosSnappedToPlane(ref Vector3 targetPos)
        {
            if (actor.PhysicsObject.physicsType != PhysicsObject.PhysicsType.CylinderCharacter)
                return;

            switch (AIQB.MoveDirection)
            {
                case PlayerDirection.Left:
                    goto case PlayerDirection.Right;
                case PlayerDirection.Right:
                    if (targetPos.X > actor.PhysicsObject.Position.X)
                        actor.PhysicsObject.CylinderCharController.Body.Orientation = PlayerAgent.faceRight;
                    else
                        actor.PhysicsObject.CylinderCharController.Body.Orientation = PlayerAgent.faceLeft;
                    break;
                case PlayerDirection.Forward:
                    goto case PlayerDirection.Backward;
                case PlayerDirection.Backward:
                    if (targetPos.Z > actor.PhysicsObject.Position.Z)
                        actor.PhysicsObject.CylinderCharController.Body.Orientation = PlayerAgent.faceBackward;
                    else
                        actor.PhysicsObject.CylinderCharController.Body.Orientation = PlayerAgent.faceForward;
                    break;
            }
        }

        /// <summary>
        /// this should be called when the enemy is reused after being killed
        /// </summary>
        public virtual void Revive()
        {
            timer = 0;
            shouldAttack = true;
            target = PlayerAgent.Player;
            state = AIState.Moving;
            spawnPos = this.actor.PhysicsObject.Position;
            FaceTargetSnappedToPlane();
        }

        public void Kill()
        {
            actor.ShutDown();
        }

        public bool IsFacing(ref Vector3 pos)
        {
            Vector3 facing = -actor.PhysicsObject.CylinderCharController.Body.OrientationMatrix.Forward;
            Vector3 shouldBeFacing = pos - actor.PhysicsObject.Position;

            switch (AIQB.MoveDirection)
            {
                case PlayerDirection.Backward:
                    goto case PlayerDirection.Forward;
                case PlayerDirection.Forward:
                    return (Math.Sign(facing.Z) + Math.Sign(shouldBeFacing.Z) != 0);
                case PlayerDirection.Left:
                    goto case PlayerDirection.Right;
                case PlayerDirection.Right:
                    return (Math.Sign(facing.X) + Math.Sign(shouldBeFacing.X) != 0);
            }
            return false;
        }

        public float AttackTarget()
        {
            if (target != null)
            {
                Vector3 targetPos = target.PhysicsObject.Position;
                return StartAttack(ref targetPos);
            }
            return 0;
        }

        public PlayerDirection GetFacingDir()
        {
            PlayerDirection dir = PlayerDirection.Backward;

            Vector3 facing = actor.PhysicsObject.CylinderCharController.Body.OrientationMatrix.Forward;

            switch (AIQB.MoveDirection)
            {
                case PlayerDirection.Backward:
                    if (facing.Z >= 0)
                        dir = PlayerDirection.Forward;
                    else
                        dir = PlayerDirection.Backward;
                    break;
                case PlayerDirection.Forward:
                    if (facing.Z >= 0)
                        dir = PlayerDirection.Forward;
                    else
                        dir = PlayerDirection.Backward;
                    break;
                case PlayerDirection.Left:
                    if (facing.X >= 0)
                        dir = PlayerDirection.Right;
                    else
                        dir = PlayerDirection.Left;
                    break;
                case PlayerDirection.Right:
                    if (facing.X >= 0)
                        dir = PlayerDirection.Right;
                    else
                        dir = PlayerDirection.Left;
                    break;
            }

            return dir;
        }

        public static EnemyType stringToEnemyType(string enemyType)
        {
            switch (enemyType)
            {
                case "weak":
                    return EnemyType.Weak;
                case "heavy":
                    return EnemyType.Heavy;
                case "ranged":
                    return EnemyType.Ranged;
                case "dumb":
                    return EnemyType.Dumb;
                default: //return dumb by default if we are passed an unknown string
                    return EnemyType.Dumb;
            }
        }
    }
}
