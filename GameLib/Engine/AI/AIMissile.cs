using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace GameLib.Engine.AI
{
    class AIMissile : AI
    {
        private bool sploded;

        private float attackForce;
        private float lifeTime;
        private float attackDelay;

        Random rand;

        public AIMissile(Actor actor) :
            base(actor)
        {
            //assign the correct values for the actor
            speed = 10;
            attackRange = 0; //attack range doesn't matter for missles as we go until we hit something
            attackForce = 1;
            lifeTime = 7.0f;
        }

        public override void Initialize(Stage stage)
        {
            base.Initialize(stage);


            if (stage.Parm.HasParm("AIRangedDamageScalar"))
                attackDmg *= stage.Parm.GetFloat("AIRangedDamageScalar");

            if (actor.Parm.HasParm("ExplosiveForce"))
                attackForce = actor.Parm.GetFloat("ExplosiveForce");
            if (actor.Parm.HasParm("ExplosionDelay"))
                attackDelay = actor.Parm.GetFloat("ExplosionDelay");
            if (actor.Parm.HasParm("Lifetime"))
                lifeTime = stage.Parm.GetFloat("Lifetime");

            if (actor.PhysicsObject.physicsType == PhysicsObject.PhysicsType.Box)
            {
                actor.PhysicsObject.CollisionInformation.CollisionRules.Group = PhysicsQB.missileGroup;
                actor.PhysicsObject.CollisionInformation.Entity.IsAffectedByGravity = false;
                actor.PhysicsObject.CollisionInformation.Events.ContactCreated += HitObject;
            }
            shouldAttack = true;

            sploded = false;

            
            BEPUphysics.MathExtensions.Matrix3X3 m = new BEPUphysics.MathExtensions.Matrix3X3(0,0,0,0,0,0,0,0,0);
            //actor.PhysicsObject.CharacterController.
            ((BEPUphysics.Entities.Entity)this.actor.PhysicsObject.SpaceObject).LocalInertiaTensor = m;

            Vector3 missileDir = PlayerAgent.Player.PhysicsObject.Position - actor.PhysicsObject.Position;
            missileDir.Normalize();
            //to do, determine if x or z axis

            //actor.PhysicsObject.SimpleCharController.MovementDirection = new Vector2(missileDir.X, missileDir.Y);
            actor.PhysicsObject.LinearVelocity = missileDir * speed;
            AimInDir();

            state = AIState.Moving;
            type = EnemyType.Missile;
            spawnedFromTrigger = true;
            actor.RegisterUpdateFunction(Update);
            timer = lifeTime;

            rand = new Random();
        }

        //reinits the missile to spawn again
        public void ReInit(ref Vector3 pos)
        {
            shouldAttack = true;
            sploded = false;
            //move to position
            actor.PhysicsObject.Position = pos;

            Vector3 missileDir = PlayerAgent.Player.PhysicsObject.Position - actor.PhysicsObject.Position;
            missileDir.Normalize();

            //to do, determine if x or z axis
            //TO DO have missiles dissapear after going so far without hitting anything
            actor.PhysicsObject.LinearVelocity = missileDir * speed;
            AimInDir();

            state = AIState.Moving;
            timer = lifeTime;


            //play a random missile firing sound
            const int NUM_MISSILE_SOUNDS = 4;
            string soundName = null;

            switch (rand.Next(NUM_MISSILE_SOUNDS))
            {
                case 0:
                    soundName = "Missile1_16";
                    break;
                case 1:
                    soundName = "Missile2_16";
                    break;
                case 2:
                    soundName = "Missile3_16";
                    break;
                case 3:
                    soundName = "Missile4_16";
                    break;
            }
            Stage.ActiveStage.GetQB<AudioQB>().PlaySound(soundName, 1.0f, 0.0f, 0.0f);
        }

        public override void Update(float dt)
        {
            timer -= dt;

            if (timer <= 0)
            {
                Vector3 place = actor.PhysicsObject.Position;
                if (sploded && !actor.IsShutdown)
                    Attack(ref place);
                else if(!sploded)
                    StartAttack(ref place);
            }
        }

        public override void Attack(ref Vector3 place)
        {
            Stage.ActiveStage.GetQB<AttackSystem.PlayerAttackSystemQB>().EnemyAttack(attackDmg, attackForce,
                new Vector3(4, 4, 4), this.actor, AIQB.MoveDirection); //do damage
            
            this.actor.ShutDown();
        }

        public override float StartAttack(ref Vector3 place)
        {
            //explosion particle FX
            Stage.ActiveStage.GetQB<Particles.ParticleQB>().AddParticleEmitter(null, place, true, -1, 30, 40, .75f, 1.0f,
                Vector2.One * 1.5f, Vector2.One * 2.0f, Vector3.Zero, Vector3.Zero, Vector3.One * 5, "explosion");
            Stage.ActiveStage.GetQB<AudioQB>().PlaySound("MissileExplosion_16", 1.0f, 0.0f, 0.0f);
            actor.PhysicsObject.LinearVelocity = Vector3.Zero;
            actor.Shown = false;
            sploded = true;
            timer = attackDelay;
            return attackTime;
        }

        public void AimInDir()
        {
            Vector3 missileDir = this.actor.PhysicsObject.LinearVelocity;
            if (missileDir == Vector3.Zero) return;
            Vector3 crossVec = Vector3.Zero;
            switch (AIQB.MoveDirection)
            {
                case PlayerDirection.Backward:
                case PlayerDirection.Forward:
                    crossVec = Vector3.Right;
                    break;
                case PlayerDirection.Left:
                case PlayerDirection.Right:
                    crossVec = Vector3.Backward;
                    break;
            }
            missileDir.Normalize();
            BEPUphysics.Entities.Entity e = ((BEPUphysics.Entities.Entity)this.actor.PhysicsObject.SpaceObject);
            BEPUphysics.MathExtensions.Matrix3X3 o = e.OrientationMatrix;
            o.Up = missileDir;
            o.Forward = Vector3.Cross(missileDir, crossVec);
            o.Right = crossVec;
            e.OrientationMatrix = o;
        }

        //this is called when we hit something
        public void HitObject(BEPUphysics.Collidables.MobileCollidables.EntityCollidable sender, BEPUphysics.Collidables.Collidable other, 
            BEPUphysics.NarrowPhaseSystems.Pairs.CollidablePairHandler pair, BEPUphysics.CollisionTests.ContactData contact)
        {
            if (!sploded && !actor.IsShutdown && !actor.MarkedForDeath)
            {
                Vector3 mypos = actor.PhysicsObject.Position;
                StartAttack(ref mypos);
            }
        }

        public override void MoveTowards(ref Vector3 dest)
        {

            //actor.PhysicsObject.SimpleCharController.MovementDirection = new Vector2(move.X, 0);
        }

        public void ChangeDir(Vector3 missileDir)
        {
            actor.PhysicsObject.LinearVelocity = missileDir * speed;
        }

        public override void ConvertTo(EnemyType e, ref ParameterSet parm) { }
    }
}