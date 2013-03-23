using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace GameLib.Engine.AI
{
    class AIWeak : AI
    {
        public AIWeak(Actor actor) :
            base(actor)
        {
            //assign the correct values for the actor
            speed = 10;
            attackRange = 5;
            timer = 0;
        }

        WeakEnemyAnimationAgent enemyAnimationAgent;
        bool disarmed;
        Vector3 runAwayPos;
        int runAwayDir;
        bool reachedRunawayPos;
        float wallOffset;

        public override void Initialize(Stage stage)
        {
            base.Initialize(stage);

            if (stage.Parm.HasParm("AIWeakSpeedScalar"))
                speed *= stage.Parm.GetFloat("AIWeakSpeedScalar");
            if (stage.Parm.HasParm("AIWeakDamageScalar"))
                attackDmg *= stage.Parm.GetFloat("AIWeakDamageScalar");

            jumpSpeed = 10;

            if (actor.PhysicsObject.physicsType == PhysicsObject.PhysicsType.CylinderCharacter)
            {
                actor.PhysicsObject.CylinderCharController.HorizontalMotionConstraint.Speed = speed;
                actor.PhysicsObject.CollisionInformation.CollisionRules.Group = PhysicsQB.normalAIGroup;
            }

            shouldAttack = true;
            disarmed = false;
            target = PlayerAgent.Player;
            state = AIState.Moving;
            type = EnemyType.Weak;
            actor.RegisterUpdateFunction(Update);
            actor.RegisterDeathFunction(DieFX);
            actor.RegisterDisarmFunction(Disarmed);

            enemyAnimationAgent = actor.GetAgent<WeakEnemyAnimationAgent>();
            FaceTargetSnappedToPlane();
        }

        public override void ConvertTo(EnemyType e, ref ParameterSet parm) 
        {
            if ((e != EnemyType.Weak && e != EnemyType.WeakShielded) || e == type)
                return;

            //actor.model = new RModel(parm);
            //actor.model.LoadContent(Stage.Content, parm, Stage.ActiveStage);
            bloodOnDamage = true;
            if (parm.HasParm("Speed"))
                speed = parm.GetFloat("Speed");
            if (parm.HasParm("AttackDamage"))
                attackDmg = parm.GetFloat("AttackDamage");
            if (parm.HasParm("AttackRange"))
                attackRange = (float)Math.Pow(parm.GetFloat("AttackRange"), 2);
            if (parm.HasParm("PercAnimationBeforeAttack"))
                percAnimBeforeAttack = parm.GetFloat("PercAnimationBeforeAttack");
            if (parm.HasParm("TimeForAttack"))
                attackTime = parm.GetFloat("TimeForAttack");

            pointsForKilling = 50;
            if (parm.HasParm("PointsForKilling"))
                pointsForKilling = parm.GetInt("PointsForKilling");
            spawnerIndex = -1;

            if (Stage.ActiveStage.Parm.HasParm("AIWeakSpeedScalar"))
                speed *= Stage.ActiveStage.Parm.GetFloat("AIWeakSpeedScalar");

            if (Stage.ActiveStage.Parm.HasParm("AIWeakDamageScalar"))
                attackDmg *= Stage.ActiveStage.Parm.GetFloat("AIWeakDamageScalar");

            jumpSpeed = 10;

            if (actor.PhysicsObject.physicsType == PhysicsObject.PhysicsType.CylinderCharacter)
            {
                actor.PhysicsObject.CylinderCharController.HorizontalMotionConstraint.Speed = speed;
                actor.PhysicsObject.CollisionInformation.CollisionRules.Group = PhysicsQB.normalAIGroup;
            }

            if (parm.HasParm("EdgeDistWhileRunning"))
                wallOffset = parm.GetFloat("EdgeDistWhileRunning");


            actor.GetAgent<HealthAgent>().Convert(ref parm);

            shouldAttack = true;
            disarmed = false;
            target = PlayerAgent.Player;
            state = AIState.Moving;
            FaceTargetSnappedToPlane();
            type = e;
        }

        public override void Update(float dt)
        {
            if (target != null)
            {
                Vector3 targetPos = target.PhysicsObject.Position;


                switch (state)
                {
                    case AIState.Moving:
                        
                        float distance = AIQB.DistanceSquared(actor.PhysicsObject.Position, targetPos);
                        Vector3 enemyPos = actor.PhysicsObject.Position;
                        bool onScreen = AIQB.OnScreen(ref targetPos, ref enemyPos);

                        if(!onScreen)
                        {
                            if (spawnedFromTrigger)
                            {
                                timer += dt;
                                if (timer >= 5.0f)
                                {
                                    this.actor.PhysicsObject.CylinderCharController.Body.LinearVelocity = Vector3.Zero;
                                    this.actor.PhysicsObject.Position = spawnPos;
                                    timer = 0;
                                }
                            }
                        }

                        if (!shouldAttack && disarmed)
                        {
                            if (reachedRunawayPos)
                            {
                                if (!onScreen)
                                {
                                    runAwayDir = -runAwayDir;
                                    runAwayPos = AIQB.PosOnSideOfScreen(actor.PhysicsObject.Position, runAwayDir, 4.0f);
                                    reachedRunawayPos = false;

                                    Vector2 move = new Vector2(runAwayPos.X - actor.PhysicsObject.Position.X, runAwayPos.Z - actor.PhysicsObject.Position.Z);
                                    move.Normalize();
                                    actor.PhysicsObject.CylinderCharController.HorizontalMotionConstraint.MovementDirection = move;
                                    FacePosSnappedToPlane(ref runAwayPos);
                                }

                                enemyAnimationAgent.PlayAnimation(WeakEnemyAnimationAgent.AnimationTypes.Idle, -1.0f);
                            }
                            else
                            {
                                switch (AIQB.MoveDirection)
                                {
                                    case PlayerDirection.Left:
                                    case PlayerDirection.Right:
                                        if (runAwayDir > 0)
                                        {
                                            if (actor.PhysicsObject.Position.X >= runAwayPos.X)
                                            {
                                                reachedRunawayPos = true;
                                                actor.PhysicsObject.CylinderCharController.HorizontalMotionConstraint.MovementDirection = Vector2.Zero;
                                            }
                                            else
                                                actor.PhysicsObject.CylinderCharController.HorizontalMotionConstraint.MovementDirection = Vector2.UnitX;
                                        }
                                        else
                                        {
                                            if (actor.PhysicsObject.Position.X <= runAwayPos.X)
                                            {
                                                reachedRunawayPos = true;
                                                actor.PhysicsObject.CylinderCharController.HorizontalMotionConstraint.MovementDirection = Vector2.Zero;
                                            }
                                            else
                                                actor.PhysicsObject.CylinderCharController.HorizontalMotionConstraint.MovementDirection = -Vector2.UnitX;
                                        }
                                        break;
                                    case PlayerDirection.Backward:
                                    case PlayerDirection.Forward:
                                        if (runAwayDir > 0)
                                        {
                                            if (actor.PhysicsObject.Position.Z >= runAwayPos.Z)
                                            {
                                                reachedRunawayPos = true;
                                                actor.PhysicsObject.CylinderCharController.HorizontalMotionConstraint.MovementDirection = Vector2.Zero;
                                            }
                                            else
                                                actor.PhysicsObject.CylinderCharController.HorizontalMotionConstraint.MovementDirection = Vector2.UnitY;
                                        }
                                        else
                                        {
                                            if (actor.PhysicsObject.Position.Z <= runAwayPos.Z)
                                            {
                                                reachedRunawayPos = true;
                                                actor.PhysicsObject.CylinderCharController.HorizontalMotionConstraint.MovementDirection = Vector2.Zero;
                                            }
                                            else
                                                actor.PhysicsObject.CylinderCharController.HorizontalMotionConstraint.MovementDirection = -Vector2.UnitY;
                                        }
                                        break;
                                }

                                enemyAnimationAgent.PlayAnimation(WeakEnemyAnimationAgent.AnimationTypes.RunScared, -1.0f);
                            }
                        }
                        else
                        {
                            if (distance < attackRange && IsFacing(ref targetPos))
                            {
                                if (shouldAttack)
                                {
                                    state = AIState.WaitingToAttack;
                                }
                                else
                                {
                                    DoneWithTarget();
                                }
                            }
                            else
                                MoveTowards(ref targetPos);

                                enemyAnimationAgent.PlayAnimation(WeakEnemyAnimationAgent.AnimationTypes.Walk, -1.0f);
                        }

                        break;
                    case AIState.WaitingToAttack:
                        this.actor.PhysicsObject.CylinderCharController.HorizontalMotionConstraint.MovementDirection = Vector2.Zero;
                        float d = AIQB.DistanceSquared(actor.PhysicsObject.Position, targetPos);

                        if (d > attackRange || !IsFacing(ref targetPos))
                            state = AIState.Moving;
                        break;
                    case AIState.Attacking:
                        if (timer <= 0)
                        {
                            if (hasAttacked)
                                state = AIState.Moving;
                            else
                                Attack(ref targetPos);
                        }
                        else
                            timer -= dt;

                        enemyAnimationAgent.PlayAnimation(WeakEnemyAnimationAgent.AnimationTypes.Attack, attackTime);

                        break;
                    case AIState.Stunned:
                        if (timer <= 0)
                        {
                            state = AIState.Moving;
                        }
                        else
                            timer -= dt;

                        enemyAnimationAgent.PauseAnimation();
                        //enemyAnimationAgent.PlayAnimation(WeakEnemyAnimationAgent.AnimationTypes.TakeDamage, animationTime);

                        break;
                }
            }
        }

        public override void Attack(ref Vector3 place)
        {
            hasAttacked = true;
            Stage.ActiveStage.GetQB<AttackSystem.PlayerAttackSystemQB>().EnemyAttack(attackDmg, 0, new Vector3(0, 5, 2), actor, GetFacingDir());
            timer = (1 - percAnimBeforeAttack) * attackTime;
        }

        public override float StartAttack(ref Vector3 place)
        {
            hasAttacked = false;

            Vector3 move = target.PhysicsObject.Position - actor.PhysicsObject.Position;

            move.Y = 0;
            move.Normalize();

            state = AIState.Attacking;
            actor.PhysicsObject.CylinderCharController.HorizontalMotionConstraint.MovementDirection = Vector2.Zero;
            timer = percAnimBeforeAttack * attackTime;

            FaceTargetSnappedToPlane();
            return attackTime;
        }

        public override void MoveTowards(ref Vector3 dest)
        {
            Vector3 move = target.PhysicsObject.Position - actor.PhysicsObject.Position;

            move.Y = 0; //we don't have jumping yet
            move.Normalize();

            Vector2 totalMovement = Vector2.Zero;

            switch (AIQB.MoveDirection)
            {
                case PlayerDirection.Right:
                    goto case PlayerDirection.Left;
                case PlayerDirection.Left:
                    totalMovement.X = (move.X < 0) ? -1 : 1;
                    break;
                case PlayerDirection.Forward:
                    goto case PlayerDirection.Backward;
                case PlayerDirection.Backward:
                    totalMovement.Y = (move.Z < 0) ? -1 : 1;
                    break;
            }

            FaceTargetSnappedToPlane();

            //we shouldn't need to normalize this, I will leave it in until I am sure this is the case
            actor.PhysicsObject.CylinderCharController.HorizontalMotionConstraint.MovementDirection = totalMovement;
        }

        public void DieFX()
        {
            Stage.ActiveStage.GetQB<Particles.ParticleQB>().AddParticleEmitter(null, actor.PhysicsObject.Position, true, -1, 20, 40, .25f, .5f,
                                                            Vector2.One, Vector2.One * 2.0f, Vector3.Zero, 
                                                            Vector3.Zero, 2 * Vector3.One, "blood2");

            //play death sound
            Stage.ActiveStage.GetQB<AudioQB>().PlaySound("bloodsplat_16", 0.3f, 0.0f, 0.0f);
        }

        public void Disarmed()
        {
            if (type == EnemyType.WeakShielded)
            {
                shouldAttack = false;
                disarmed = true;
                //get first run away destination (should be away from the player)
                state = AIState.Moving;
                if (AIQB.MoveDirection == PlayerDirection.Left || AIQB.MoveDirection == PlayerDirection.Right)
                {
                    if (target.PhysicsObject.Position.X < actor.PhysicsObject.Position.X)
                        runAwayDir = 1;
                    else
                        runAwayDir = -1;
                }
                else
                {
                    if (target.PhysicsObject.Position.Z < actor.PhysicsObject.Position.Z)
                        runAwayDir = 1;
                    else
                        runAwayDir = -1;
                }
                reachedRunawayPos = false;
                runAwayPos = AIQB.PosOnSideOfScreen(actor.PhysicsObject.Position, runAwayDir, 4.0f);
                Vector2 move = new Vector2(runAwayPos.X - actor.PhysicsObject.Position.X, runAwayPos.Z - actor.PhysicsObject.Position.Z);
                move.Normalize();
                actor.PhysicsObject.CylinderCharController.HorizontalMotionConstraint.MovementDirection = move;
                FacePosSnappedToPlane(ref runAwayPos);
            }
        }
   } 
}