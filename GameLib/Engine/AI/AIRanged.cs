using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace GameLib.Engine.AI
{
    class AIRanged : AI
    {

        private Actor _myMissile;
        private Vector2 missileOffset;

        private int maxRocketJumps;
        private int minRocketJumps;
        private int numJumps;

        private int maxRocketAttacks;
        private int minRocketAttacks;
        private int numAttacks;

        private float pogoStartTime;
        private float timeForPogoSkip;
        private float percAnimForSplat;
        
        private bool peaked;
        private bool hasFoundSupport;

        RangedEnemyAnimationAgent enemyAnimationAgent;

        public AIRanged(Actor actor) :
            base(actor)
        {
            //assign the correct values for the actor
            speed = 7;
            attackRange = 50;
        }

        public override void Initialize(Stage stage)
        {
            base.Initialize(stage);

            if (stage.Parm.HasParm("AIRangedSpeedScalar"))
                speed *= stage.Parm.GetFloat("AIRangedSpeedScalar");

            missileOffset = Vector2.Zero;

            if(actor.Parm.HasParm("MissileOffset"))
                missileOffset = actor.Parm.GetVector2("MissileOffset");


            timeForPogoSkip = .1f;
            percAnimForSplat = .45f;
            if (actor.Parm.HasParm("MaxJumps"))
                maxRocketJumps = actor.Parm.GetInt("MaxJumps");
            if (actor.Parm.HasParm("MinJumps"))
                minRocketJumps = actor.Parm.GetInt("MinJumps");
            if (actor.Parm.HasParm("MaxAttacks"))
                maxRocketAttacks = actor.Parm.GetInt("MaxAttacks");
            if (actor.Parm.HasParm("MinAttacks"))
                minRocketAttacks = actor.Parm.GetInt("MinAttacks");
            if (actor.Parm.HasParm("PogoStartTime"))
                pogoStartTime = actor.Parm.GetFloat("PogoStartTime");
            if (actor.Parm.HasParm("PogoSkipTime"))
                timeForPogoSkip = actor.Parm.GetFloat("PogoSkipTime");
            if (actor.Parm.HasParm("PercAnimForSplat"))
                percAnimForSplat = actor.Parm.GetFloat("PercAnimForSplat");

            jumpSpeed = 22;

            spawnerIndex = -1;

            GetNumJumps();

            if (actor.PhysicsObject.physicsType == PhysicsObject.PhysicsType.CylinderCharacter)
            {
                actor.PhysicsObject.CylinderCharController.HorizontalMotionConstraint.Speed = speed;
                actor.PhysicsObject.CollisionInformation.CollisionRules.Group = PhysicsQB.normalAIGroup;
            }

            shouldAttack = true;
            target = PlayerAgent.Player;
            state = AIState.Moving;
            type = EnemyType.Ranged;
            actor.RegisterUpdateFunction(Update);
            actor.RegisterDeathFunction(DieFX);

            enemyAnimationAgent = actor.GetAgent<RangedEnemyAnimationAgent>();
            FaceTargetSnappedToPlane();


            Vector3 zero = Vector3.Zero;
            _myMissile = stage.GetQB<ActorQB>().CreateActor("Missile", "Missile", ref zero, ref zero, stage);
            _myMissile.ShutDown();
        }

        public override void Revive()
        {
            base.Revive();
            GetNumJumps();
        }

        public override void ConvertTo(EnemyType e, ref ParameterSet parm) 
        {
            if ((e != EnemyType.Ranged && e != EnemyType.RangedPogo) || e == type)
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

            if (Stage.ActiveStage.Parm.HasParm("AIRangedSpeedScalar"))
                speed *= Stage.ActiveStage.Parm.GetFloat("AIRangedSpeedScalar");

            if (Stage.ActiveStage.Parm.HasParm("AIRangedDamageScalar"))
                attackDmg *= Stage.ActiveStage.Parm.GetFloat("AIRangedDamageScalar");

            missileOffset = Vector2.Zero;

            if (actor.Parm.HasParm("MissileOffset"))
                missileOffset = actor.Parm.GetVector2("MissileOffset");


            timeForPogoSkip = .1f;
            percAnimForSplat = .45f;
            maxRocketJumps = minRocketJumps = minRocketAttacks = maxRocketAttacks = 0;

            if (parm.HasParm("MaxJumps"))
                maxRocketJumps = parm.GetInt("MaxJumps");
            if (parm.HasParm("MinJumps"))
                minRocketJumps = parm.GetInt("MinJumps");
            if (parm.HasParm("MaxAttacks"))
                maxRocketAttacks = parm.GetInt("MaxAttacks");
            if (parm.HasParm("MinAttacks"))
                minRocketAttacks = parm.GetInt("MinAttacks");
            if (parm.HasParm("PogoStartTime"))
                pogoStartTime = parm.GetFloat("PogoStartTime");

            jumpSpeed = 22;

            if (actor.PhysicsObject.physicsType == PhysicsObject.PhysicsType.CylinderCharacter)
            {
                actor.PhysicsObject.CylinderCharController.HorizontalMotionConstraint.Speed = speed;
                actor.PhysicsObject.CollisionInformation.CollisionRules.Group = PhysicsQB.normalAIGroup;
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

                        if (numJumps != 0) //if jumping
                        {
                            if (hasFoundSupport)
                            {
                                //if we are on the ground and aren't waiting jump
                                if (timer < 0 && actor.PhysicsObject.CylinderCharController.SupportFinder.HasSupport)
                                {
                                    actor.PhysicsObject.CylinderCharController.Jump(jumpSpeed);
                                    RocketJump();
                                    peaked = false;
                                    state = AIState.Jumping;
                                }
                                else if (timer < timeForPogoSkip) //shoot our rocket so it looks like we bounce
                                {
                                    timer -= dt;
                                    if (!hasAttacked)
                                    {
                                        ShootDown(false);
                                    }
                                }
                                else
                                {
                                    timer -= dt;
                                    enemyAnimationAgent.PlayAnimation(RangedEnemyAnimationAgent.AnimationTypes.StartPogo, pogoStartTime);
                                }
                            }
                            else
                            {
                                if (actor.PhysicsObject.CylinderCharController.SupportFinder.HasSupport)
                                    hasFoundSupport = true;
                                enemyAnimationAgent.PlayAnimation(RangedEnemyAnimationAgent.AnimationTypes.Idle, -1.0f);
                            }
                        }
                        else
                        {
                            GetNumAttacks();
                            if (numAttacks == 0)
                            {
                                GetNumJumps();
                                return;
                            }
                            hasAttacked = false;
                            float distance = AIQB.DistanceSquared(actor.PhysicsObject.Position, targetPos);

                            Vector3 myPos = actor.PhysicsObject.Position;
                            if (distance < attackRange && AIQB.OnScreen(ref targetPos, ref myPos)) //if on screen and in range attack
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
                            else //we are too far away so jump
                            {
                                numJumps = 1;
                            }

                        }
                        //there is no walk animation for ranged enemies
                        break;
                    case AIState.WaitingToAttack:
                        float d = AIQB.DistanceSquared(actor.PhysicsObject.Position, targetPos);

                        Vector3 p = actor.PhysicsObject.Position;
                        if (d > attackRange || !AIQB.OnScreen(ref targetPos, ref p))
                        {
                            GetNumJumps();
                            state = AIState.Moving;
                        }
                        FaceTargetSnappedToPlane();
                        enemyAnimationAgent.PlayAnimation(RangedEnemyAnimationAgent.AnimationTypes.Idle, -1.0f);
                        break;
                    case AIState.Attacking:
                        
                        timer -= dt;
                        if (hasAttacked)
                        {
                            if (timer > 0)
                                enemyAnimationAgent.PlayAnimation(RangedEnemyAnimationAgent.AnimationTypes.Attack, animationTime);
                            else
                                enemyAnimationAgent.PlayAnimation(RangedEnemyAnimationAgent.AnimationTypes.Idle, -1.0f);

                            //AI ranged does nothing until his missile has been destroyed
                            if (_myMissile.IsShutdown)
                            {
                                hasAttacked = false;
                                if (numAttacks != 0)
                                    this.state = AIState.WaitingToAttack;
                                else
                                {
                                    GetNumJumps();
                                    this.state = AIState.Moving;
                                }
                            }
                        }
                        else
                        {
                            if (timer <= 0)
                            {
                                Attack(ref targetPos);
                            }
                            //play reload animation
                            enemyAnimationAgent.PlayAnimation(RangedEnemyAnimationAgent.AnimationTypes.Idle, -1.0f);
                        }

                        break;
                    case AIState.Stunned:
                        if (timer <= 0)
                        {
                            GetNumJumps();
                            state = AIState.Moving;
                        }
                        else
                            timer -= dt;

                        enemyAnimationAgent.PauseAnimation();
                        //enemyAnimationAgent.PlayAnimation(RangedEnemyAnimationAgent.AnimationTypes.TakeDamage, animationTime);

                        break;
                    case AIState.Jumping:

                        timer -= dt;
                        if (actor.PhysicsObject.LinearVelocity.Y < 0)
                        {
                            peaked = true;   
                        }
                        if (actor.PhysicsObject.CylinderCharController.SupportFinder.HasSupport && peaked)
                        {
                            this.actor.PhysicsObject.CylinderCharController.HorizontalMotionConstraint.MovementDirection = Vector2.Zero;
                            this.actor.PhysicsObject.CylinderCharController.Body.LinearVelocity = Vector3.Zero;
                            if(timer <= 0)
                                state = AIState.Moving;
                        }
                        if (timer < timeForPogoSkip)
                        {
                            if (!hasAttacked && numJumps > 0)
                            {
                                ShootDown(false);
                            }
                        }

                        if (numJumps == 0)
                            enemyAnimationAgent.PlayAnimation(RangedEnemyAnimationAgent.AnimationTypes.EndPogo, animationTime);
                        else
                            enemyAnimationAgent.PlayAnimation(RangedEnemyAnimationAgent.AnimationTypes.Pogo, -1.0f);
                        break;
                }
            }
        }

        public override void Attack(ref Vector3 place)
        {
            if(!hasAttacked)
            {
                numAttacks--;
                hasAttacked = true;

                actor.PhysicsObject.CylinderCharController.HorizontalMotionConstraint.MovementDirection = Vector2.Zero;
                Vector3 pos = this.actor.PhysicsObject.Position;

                //we may have issues with the missile passing to the side of the player.

                Vector3 toPlayer = PlayerAgent.Player.PhysicsObject.Position - pos;
                toPlayer.Normalize();

                //offset the missile to the rocket launcher
                pos.Y += missileOffset.Y;
                if (AIQB.MoveDirection == PlayerDirection.Left || AIQB.MoveDirection == PlayerDirection.Right)
                {
                    if (toPlayer.X > 0)
                        pos.X += missileOffset.X;
                    else
                        pos.X -= missileOffset.X;
                }
                else
                {
                    if (toPlayer.Z > 0)
                        pos.Z += missileOffset.X;
                    else
                        pos.Z -= missileOffset.X;
                }

                this.state = AIState.Attacking;
                Vector3 rot = Vector3.Zero;

                if (_myMissile == null)
                    _myMissile = Stage.ActiveStage.GetQB<ActorQB>().CreateActor("Missile", "Missile", ref pos, ref rot, Stage.ActiveStage);
                else
                {
                    _myMissile.GetAgent<AIMissile>().ReInit(ref pos);
                    _myMissile.WakeUp();
                }
                Stage.ActiveStage.GetQB<Particles.ParticleQB>().AddParticleEmitter(_myMissile, Vector3.Zero, false, -1, 10, 
                                                15, .5f, .75f, 0, 0, Vector2.One * .9f, Vector2.One, Vector3.Zero, 
                                                Vector3.Zero, Vector3.Zero, "smoke");

                animationTime = 1.0f;
                timer = 1.0f;
            }
        }

        public override float StartAttack(ref Vector3 place)
        {
            FaceTargetSnappedToPlane();
            state = AIState.Attacking;
            actor.PhysicsObject.CylinderCharController.HorizontalMotionConstraint.MovementDirection = Vector2.Zero;

            timer = attackTime;
            animationTime = attackTime;

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

            FaceTargetSnappedToPlane();
        }

        //calculates the position to which we should jump to and what the speed will need to be to make it there
        private void RocketJump()
        {
            Vector3 pos = AIQB.RandomPosOnScreen(this.actor.PhysicsObject.Position, 3.0f);


            Vector2 moveDir = new Vector2(pos.X - this.actor.PhysicsObject.Position.X,
                                            pos.Z - this.actor.PhysicsObject.Position.Z);
            FacePosSnappedToPlane(ref pos);

            moveDir.Normalize();

            float dist = Vector3.Distance(pos, this.actor.PhysicsObject.Position);
            float jumpVel = this.actor.PhysicsObject.CylinderCharController.JumpSpeed -
                            this.actor.PhysicsObject.CylinderCharController.Body.LinearVelocity.Y;
            float timeOfJump = 2 * -jumpVel / 
                Stage.ActiveStage.GetQB<PhysicsQB>().Space.ForceUpdater.Gravity.Y;;

            float speed = dist / timeOfJump;

            this.actor.PhysicsObject.CylinderCharController.Body.LinearVelocity = new Vector3(moveDir.X * speed, 
                jumpVel, moveDir.Y * speed);

            numJumps--;
            timer = timeOfJump;
            if (numJumps == 0)
            {
                timer /= percAnimForSplat;
                animationTime = timer;
            }
            hasAttacked = false;
        }

        private void ShootDown(bool pFX)
        {
            //shoot missile

            Vector3 missilePos = this.actor.PhysicsObject.Position;

            //offset the missile to the rocket launcher
            missilePos.Y -= 2;
            /*if (AIQB.MoveDirection == PlayerDirection.Left || AIQB.MoveDirection == PlayerDirection.Right)
            {
                if (toPlayer.X > 0)
                    pos.X += missileOffset.X;
                else
                    pos.X -= missileOffset.X;
            }
            else
            {
                if (toPlayer.Z > 0)
                    pos.Z += missileOffset.X;
                else
                    pos.Z -= missileOffset.X;
            }*/

            Vector3 rot = Vector3.Zero;


            AIMissile m = _myMissile.GetAgent<AIMissile>();
            m.ReInit(ref missilePos);
            m.ChangeDir(Vector3.Down * 3);
            m.AimInDir();
            _myMissile.WakeUp();

            if(pFX)
                Stage.ActiveStage.GetQB<Particles.ParticleQB>().AddParticleEmitter(_myMissile, Vector3.Zero, false, -1, 10,
                                            15, .5f, .75f, 0, 0, Vector2.One * .9f, Vector2.One, Vector3.Zero,
                                            Vector3.Zero, Vector3.Zero, "smoke");
            hasAttacked = true;
        }

        private void DieFX()
        {
            if (_myMissile != null)
            {
                //make sure the missile dies when it explodes so it doesn't get stuck in memory
                /*_myMissile.GetAgent<AIMissile>().DestroyOnExplosion = true;
                actor.MarkForDeath();*/
            }

            Stage.ActiveStage.GetQB<Particles.ParticleQB>().AddParticleEmitter(null, actor.PhysicsObject.Position, true, -1, 20, 40, .25f, .5f,
                                                            0, 0, Vector2.One, Vector2.One * 2.0f, Vector3.Zero, Vector3.Zero, 
                                                            2*Vector3.One, "blood2");
            //play death sound
            Stage.ActiveStage.GetQB<AudioQB>().PlaySound("bloodsplat_16", 0.3f, 0.0f, 0.0f);
        }

        //randomize the number of jumps we will do
        private void GetNumJumps()
        {
            hasFoundSupport = false;
            if (minRocketJumps >= 0 && maxRocketJumps >= 0)
                numJumps = AIQB.rand.Next(maxRocketJumps - minRocketJumps + 1) + minRocketJumps;
            else numJumps = -1;

            if(numJumps != 0)
                timer = pogoStartTime;
        }

        //randomize the number of attacks we will do
        private void GetNumAttacks()
        {
            if (minRocketAttacks >= 0 && maxRocketAttacks >= 0)
                numAttacks = AIQB.rand.Next(maxRocketAttacks - minRocketAttacks + 1) + minRocketAttacks;
            else numAttacks = -1;
        }

    }
}
