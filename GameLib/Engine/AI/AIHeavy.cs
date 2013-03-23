using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace GameLib.Engine.AI
{
    class AIHeavy : AI
    {

        private float turnSpeed;
        private float percAnimBeforeAttackRight;
        private float timeForAttackSpecial;
        private float percAnimBeforeAttackSpecial;
        private float attackSpecialDmg;
        private bool shouldSlam;
        private float attackForce;

        private int numPunches;
        private int minPunches;
        private int maxPunches;

        private HeavyEnemyAnimationAgent.AnimationTypes playingAttack;
        private HeavyEnemyAnimationAgent animatedAgent;
        private float animationTime;

        public AIHeavy(Actor actor) :
            base(actor)
        {
            //assign the correct values for the actor
            speed = 2.5f;
            attackRange = 5;
            timer = 0;
            turnSpeed = 5.0f;
            attackForce = 1;
        }

        public override void Initialize(Stage stage)
        {
            base.Initialize(stage);

            if (stage.Parm.HasParm("AIHeavySpeedScalar"))
                speed *= stage.Parm.GetFloat("AIHeavySpeedScalar");

            //look for more parms here
            if (actor.Parm.HasParm("TurnSpeed"))
                turnSpeed = actor.Parm.GetFloat("TurnSpeed");
            if (actor.Parm.HasParm("TimeForAttackSpecial"))
                timeForAttackSpecial = actor.Parm.GetInt("TimeForAttackSpecial");
            if (actor.Parm.HasParm("PercAnimationBeforeAttackRight"))
                percAnimBeforeAttackRight = actor.Parm.GetFloat("PercAnimationBeforeAttackRight");
            if (actor.Parm.HasParm("PercAnimationBeforeAttackSpecial"))
                percAnimBeforeAttackSpecial = actor.Parm.GetFloat("PercAnimationBeforeAttackSpecial");
            if (actor.Parm.HasParm("AttackSpecialDamage"))
                attackSpecialDmg = actor.Parm.GetFloat("AttackSpecialDamage");
            if (actor.Parm.HasParm("MinPunches"))
                minPunches = actor.Parm.GetInt("MinPunches");
            if (actor.Parm.HasParm("MaxPunches"))
                maxPunches = actor.Parm.GetInt("MaxPunches");
            if(actor.Parm.HasParm("SlamAttackForce"))
                attackForce = actor.Parm.GetFloat("SlamAttackForce");
            if (actor.Parm.HasParm("EndWithSlam"))
                shouldSlam = actor.Parm.GetBool("EndWithSlam");

            if (stage.Parm.HasParm("AIHeavyDamageScalar"))
            {
                float increase = stage.Parm.GetFloat("AIHeavyDamageScalar");
                attackDmg *= increase;
                attackSpecialDmg *= increase;
            }

            type = EnemyType.Heavy;
            if (actor.Parm.HasParm("SpecialEnemy"))
            {
                type = EnemyType.HeavyAOE;
            }

            jumpSpeed = 10;

            if (actor.PhysicsObject.physicsType == PhysicsObject.PhysicsType.CylinderCharacter)
            {
                actor.PhysicsObject.CylinderCharController.HorizontalMotionConstraint.Speed = speed;
                actor.PhysicsObject.CollisionInformation.CollisionRules.Group = PhysicsQB.heavyAIGroup;
            }
            
            shouldAttack = true;
            target = PlayerAgent.Player;
            state = AIState.Moving;
            actor.RegisterUpdateFunction(Update);
            actor.RegisterDeathFunction(DieFX);
            FaceTargetSnappedToPlane();
            animatedAgent = actor.GetAgent<HeavyEnemyAnimationAgent>();
        }

        //note if we keep the base values the same, a lot of this code can be removed
        public override void ConvertTo(EnemyType e, ref ParameterSet parm)
        {
            if ((e != EnemyType.Heavy && e != EnemyType.HeavyAOE) || e == type)
                return;

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

            if (Stage.ActiveStage.Parm.HasParm("AIHeavySpeedScalar"))
                speed *= Stage.ActiveStage.Parm.GetFloat("AIHeavySpeedScalar");

            if (Stage.ActiveStage.Parm.HasParm("AIHeavyDamageScalar"))
            {
                float increase = Stage.ActiveStage.Parm.GetFloat("AIHeavyDamageScalar");
                attackDmg *= increase;
                attackSpecialDmg *= increase;
            }

            //look for more parms here
            if (parm.HasParm("TurnSpeed"))
                turnSpeed = parm.GetFloat("TurnSpeed");
            if (parm.HasParm("TimeForAttackSpecial"))
                timeForAttackSpecial = parm.GetInt("TimeForAttackSpecial");
            if (parm.HasParm("PercAnimationBeforeAttackRight"))
                percAnimBeforeAttackRight = parm.GetFloat("PercAnimationBeforeAttackRight");
            if (parm.HasParm("PercAnimationBeforeAttackSpecial"))
                percAnimBeforeAttackSpecial = parm.GetFloat("PercAnimationBeforeAttackSpecial");
            if (parm.HasParm("AttackSpecialDamage"))
                attackSpecialDmg = parm.GetFloat("AttackSpecialDamage");
            if (parm.HasParm("MinPunches"))
                minPunches = parm.GetInt("MinPunches");
            if (parm.HasParm("MaxPunches"))
                maxPunches = parm.GetInt("MaxPunches");
            if (parm.HasParm("SlamAttackForce"))
                attackForce = parm.GetFloat("SlamAttackForce");
            if (parm.HasParm("EndWithSlam"))
                shouldSlam = parm.GetBool("EndWithSlam");

            jumpSpeed = 10;

            if (actor.PhysicsObject.physicsType == PhysicsObject.PhysicsType.CylinderCharacter)
            {
                actor.PhysicsObject.CylinderCharController.HorizontalMotionConstraint.Speed = speed;
                actor.PhysicsObject.CollisionInformation.CollisionRules.Group = PhysicsQB.heavyAIGroup;
            }

            actor.GetAgent<HealthAgent>().Convert(ref parm);

            shouldAttack = true;
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

                        animatedAgent.PlayAnimation(HeavyEnemyAnimationAgent.AnimationTypes.Walk, -1.0f);
                        
                        float distance = AIQB.DistanceSquared(actor.PhysicsObject.Position, targetPos);
                        Vector3 enemyPos = actor.PhysicsObject.Position;
                        bool onScreen = AIQB.OnScreen(ref targetPos, ref enemyPos);

                        if(!onScreen)
                        {
                            if (spawnedFromTrigger)
                            {
                                timer += dt;
                                if (timer >= 7.0f)
                                {
                                    this.actor.PhysicsObject.CylinderCharController.Body.LinearVelocity = Vector3.Zero;
                                    this.actor.PhysicsObject.Position = spawnPos;
                                    timer = 0;
                                }
                            }
                        }

                        if (distance < attackRange)
                        {
                            if (!IsFacing(ref targetPos))
                            {
                                actor.PhysicsObject.CylinderCharController.HorizontalMotionConstraint.MovementDirection = Vector2.Zero;
                                state = AIState.Turning;
                            }
                            else if (shouldAttack)
                            {
                                //check if the enemy is facing the player
                                state = AIState.WaitingToAttack;
                            }
                            else
                            {
                                DoneWithTarget();
                            }
                        }
                        else
                            MoveTowards(ref targetPos);
                        break;
                    case AIState.Turning:
                        animatedAgent.PlayAnimation(HeavyEnemyAnimationAgent.AnimationTypes.Walk, -1.0f);
                        RotateTowards(ref targetPos, dt);
                        break;
                    case AIState.WaitingToAttack:
                        float d = AIQB.DistanceSquared(actor.PhysicsObject.Position, targetPos);

                        if (d > attackRange || !IsFacing(ref targetPos))
                        {
                            state = AIState.Moving;
                        }
                        break;
                    case AIState.Attacking:
                        animatedAgent.PlayAnimation(playingAttack, animationTime);
                        if (timer <= 0)
                        {
                            if (hasAttacked)
                                determineNextAttack();
                            else
                                Attack(ref targetPos);
                        }
                        else
                            timer -= dt;
                        break;
                    case AIState.Stunned:
                        animatedAgent.PlayAnimation(HeavyEnemyAnimationAgent.AnimationTypes.Block, -1.0f);
                        if (timer <= 0)
                        {
                            state = AIState.Moving;
                        }
                        else
                            timer -= dt;
                        break;
                }
            }
        }

        public override void Attack(ref Vector3 place)
        {
            hasAttacked = true;
            switch (playingAttack)
            {
                case HeavyEnemyAnimationAgent.AnimationTypes.AttackRight:
                    Stage.ActiveStage.GetQB<AttackSystem.PlayerAttackSystemQB>().EnemyAttack(attackDmg, 0, new Vector3(0, 5, 2), actor, GetFacingDir());
                    timer = (1-percAnimBeforeAttackRight) * attackTime;
                    break;
                case HeavyEnemyAnimationAgent.AnimationTypes.AttackLeft:
                    Stage.ActiveStage.GetQB<AttackSystem.PlayerAttackSystemQB>().EnemyAttack(attackDmg, 0, new Vector3(0, 5, 2), actor, GetFacingDir());
                    timer = (1 - percAnimBeforeAttack) * attackTime;
                    break;
                case HeavyEnemyAnimationAgent.AnimationTypes.AttackSmash:
                    Stage.ActiveStage.GetQB<AttackSystem.PlayerAttackSystemQB>().EnemyAttack(attackSpecialDmg, attackForce, new Vector3(5, 5, 2), actor, GetFacingDir());
                    timer = (1 - percAnimBeforeAttackSpecial) * timeForAttackSpecial;
                    break;
            }
        }

        public override float StartAttack(ref Vector3 place)
        {
            int totalPunches = AIQB.rand.Next((maxPunches - minPunches + 1)) + minPunches;
            numPunches = totalPunches;
            determineNextAttack();
            return totalPunches * attackTime + ((shouldSlam) ? timeForAttackSpecial : 0);
        }

        private void determineNextAttack()
        {
            hasAttacked = false;
            state = AIState.Attacking;
            actor.PhysicsObject.CylinderCharController.HorizontalMotionConstraint.MovementDirection = Vector2.Zero;
            if (numPunches > 0)
            {
                animationTime = attackTime;
                int coinflip = AIQB.rand.Next(2);
                if (coinflip == 0) //left punch
                {
                    playingAttack = HeavyEnemyAnimationAgent.AnimationTypes.AttackLeft;
                    timer = attackTime * percAnimBeforeAttack;
                }
                else //right punch
                {
                    playingAttack = HeavyEnemyAnimationAgent.AnimationTypes.AttackRight;
                    timer = attackTime * percAnimBeforeAttackRight;
                }
            }
            else if (shouldSlam && numPunches == 0)
            {
                playingAttack = HeavyEnemyAnimationAgent.AnimationTypes.AttackSmash;
                timer = percAnimBeforeAttackSpecial * timeForAttackSpecial;
                animationTime = timeForAttackSpecial;
            }
            else
                state = AIState.Moving;

            numPunches--;
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
                case PlayerDirection.Up:
                    goto case PlayerDirection.Down;
                case PlayerDirection.Down:
                    //no moving in the Y dimension yet
                    break;
                case PlayerDirection.Forward:
                    goto case PlayerDirection.Down;
                case PlayerDirection.Backward:
                    totalMovement.Y = (move.Z < 0) ? -1 : 1;
                    //position.X = track;
                    break;
            }

            //we shouldn't need to normalize this, I will leave it in until I am sure this is the case
            actor.PhysicsObject.CylinderCharController.HorizontalMotionConstraint.MovementDirection = totalMovement;


            BEPUphysics.MathExtensions.Matrix3X3 o = actor.PhysicsObject.CylinderCharController.Body.OrientationMatrix;

            Vector3 prev = o.Forward;

            move *= -1;

            if (prev.X < 0 && move.X > 0 ||
                prev.X > 0 && move.X < 0 ||
                prev.Z < 0 && move.Z > 0 ||
                prev.Z > 0 && move.Z < 0)
            {
                actor.PhysicsObject.CylinderCharController.HorizontalMotionConstraint.MovementDirection = Vector2.Zero;
                state = AIState.Turning;
            }
            else
            {
                o.Forward = move;
                actor.PhysicsObject.CylinderCharController.Body.OrientationMatrix = o;
            }
        }

        public void RotateTowards(ref Vector3 dest, float dt)
        {
            Vector3 move = target.PhysicsObject.Position - actor.PhysicsObject.Position;

            BEPUphysics.MathExtensions.Matrix3X3 o = actor.PhysicsObject.CylinderCharController.Body.OrientationMatrix;

            Vector3 prev = o.Forward;

            move *= -1;

            float goalAngle = (float)Math.Atan2(move.Z, move.X);
            float currAngle = (float)Math.Atan2(prev.Z, prev.X);

            Lerp(ref goalAngle, ref currAngle, turnSpeed * dt);

            prev.X = (float)Math.Cos(currAngle);
            prev.Z = (float)Math.Sin(currAngle);


            o.Forward = prev;

            actor.PhysicsObject.CylinderCharController.Body.OrientationMatrix = o;

            if (goalAngle == currAngle) state = AIState.Moving;
        }

        private void Lerp(ref float goal, ref float current, float lerpAmount)
        {
            if (goal < current)
            {
                current -= lerpAmount;
                if (current < goal) current = goal;
            }
            else
            {
                current += lerpAmount;
                if (current > goal) current = goal;
            }
        }

        public void DieFX()
        {
            Stage.ActiveStage.GetQB<Particles.ParticleQB>().AddParticleEmitter(null, actor.PhysicsObject.Position, true, -1, 20, 40, .25f, .5f,
                                                            Vector2.One, Vector2.One * 2.0f, Vector3.Zero, Vector3.Zero, 
                                                            2 * Vector3.One, "blood2");
            //play death sound
            Stage.ActiveStage.GetQB<AudioQB>().PlaySound("bloodsplat_16", 0.3f, 0.0f, 0.0f);
        }
    }
}