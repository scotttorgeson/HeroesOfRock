using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

/*this must be attached to an actor with the PhysicsType of Character*/

namespace GameLib {
    public class HealthAgent : Agent {
        private float health;

        public float Health {
            get { return health; }
        }

        private float startingHealth;
        private bool startsWithShield;
        private bool infiniteHealth;
        private bool shutdownInsteadOfKill;
        private int numHitsForDisarm;
        private int startingHitsForDisarm;

        private bool shielded;

        public bool Shielded
        {
            get { return shielded; }
        }

        private float percDamageAbsorbedByShield;

        public float PercDamageAbsorbedByShield
        {
            get { return percDamageAbsorbedByShield; }
        }

        public HealthAgent (Actor actor)
            : base(actor) {
            this.health = 100;

            actor.RegisterReviveFunction(ResetHealth);
            Name = "HealthAgent";
        }

        public override void Initialize (Stage stage) {
            if (actor.Parm.HasParm("Health"))
                startingHealth = this.health = actor.Parm.GetFloat("Health");

            switch (actor.Name)
            {
                case "EnemyWeak":
                    if (stage.Parm.HasParm("AIWeakHealthScalar"))
                        health *= stage.Parm.GetFloat("AIWeakHealthScalar");
                    break;
                case "EnemyRanged":
                    if (stage.Parm.HasParm("AIRangedHealthScalar"))
                        health *= stage.Parm.GetFloat("AIRangedHealthScalar");
                    break;
                case "EnemyHeavy":
                    if (stage.Parm.HasParm("AIHeavyHealthScalar"))
                        health *= stage.Parm.GetFloat("AIHeavyHealthScalar");
                    break;
            }

            infiniteHealth = false;
            if (actor.Parm.HasParm("InfiniteHealth"))
                infiniteHealth = actor.Parm.GetBool("InfiniteHealth");
            if (actor.Parm.HasParm("Shielded"))
                startsWithShield = this.shielded = actor.Parm.GetBool("Shielded");
            if (actor.Parm.HasParm("ShieldDmgAbsorbPerc"))
                this.percDamageAbsorbedByShield = actor.Parm.GetFloat("ShieldDmgAbsorbPerc");
            if (actor.Parm.HasParm("ShutdownInsteadOfKill"))
                shutdownInsteadOfKill = actor.Parm.GetBool("ShutdownInsteadOfKill");
            if (actor.Parm.HasParm("NumHitsForDisarm"))
                numHitsForDisarm = startingHitsForDisarm = actor.Parm.GetInt("NumHitsForDisarm");
        }

        public void Convert(ref ParameterSet parm)
        {
            if (parm.HasParm("Health"))
                startingHealth = this.health = parm.GetFloat("Health");

            switch (actor.Name)
            {
                case "EnemyWeak":
                    if (Stage.ActiveStage.Parm.HasParm("AIWeakHealthScalar"))
                        health *= Stage.ActiveStage.Parm.GetFloat("AIWeakHealthScalar");
                    break;
                case "EnemyRanged":
                    if (Stage.ActiveStage.Parm.HasParm("AIRangedHealthScalar"))
                        health *= Stage.ActiveStage.Parm.GetFloat("AIRangedHealthScalar");
                    break;
                case "EnemyHeavy":
                    if (Stage.ActiveStage.Parm.HasParm("AIHeavyHealthScalar"))
                        health *= Stage.ActiveStage.Parm.GetFloat("AIHeavyHealthScalar");
                    break;
            }

            infiniteHealth = false;
            startsWithShield = shielded = false;
            percDamageAbsorbedByShield = 0;
            numHitsForDisarm = 0;
            if (parm.HasParm("InfiniteHealth"))
                infiniteHealth = parm.GetBool("InfiniteHealth");
            if (parm.HasParm("Shielded"))
                startsWithShield = this.shielded = parm.GetBool("Shielded");
            if (parm.HasParm("ShieldDmgAbsorbPerc"))
                this.percDamageAbsorbedByShield = parm.GetFloat("ShieldDmgAbsorbPerc");
            if (parm.HasParm("ShutdownInsteadOfKill"))
                shutdownInsteadOfKill = parm.GetBool("ShutdownInsteadOfKill");
            if (parm.HasParm("NumHitsForDisarm"))
                numHitsForDisarm = startingHitsForDisarm = parm.GetInt("NumHitsForDisarm");
        }

        public void ResetHealth()
        {
            health = startingHealth;
            shielded = startsWithShield;
            numHitsForDisarm = startingHitsForDisarm;
        }

        public void Update (float dt) {
        }

        public virtual void Serialize (ref ParameterSet parm) {
            parm.AddParm("Health", health);
        }

        public static void ParseParmSet (ref ParameterSet actorParm, ref ParameterSet worldParm) {
            if (worldParm.HasParm("Health"))
                actorParm.AddParm("Health", worldParm.GetFloat("Health"));
        }

        //should the enemy always be stunned?
        public float ModifyHealth (float healthModifer, Vector3 attackPos, bool disarming) {


            if (infiniteHealth) //if we have negative health that means we have infinite health
                return health + healthModifer;

            //if we are shielded and taking damage
            if (shielded && healthModifer < 0)
            {
                Vector3 forward = Vector3.Backward;
                switch (actor.PhysicsObject.physicsType)
                {
                    case PhysicsObject.PhysicsType.Character:
                        forward = actor.PhysicsObject.CharacterController.Body.OrientationMatrix.Forward;
                        break;
                    case PhysicsObject.PhysicsType.CylinderCharacter:
                        forward = actor.PhysicsObject.CylinderCharController.Body.OrientationMatrix.Forward;
                        break;
                }

                Vector3 attackDir = actor.PhysicsObject.Position - attackPos;
 
                //should there be seperate shield front/back/top variables?

                //we are going to find the angle between the attack and the orientation. the orientation
                // is always normalized so if we normalize attackDir we can eliminate the denominator of the fraction
                attackDir.Normalize();

                double angle = Math.Acos(Vector3.Dot(forward, attackDir));

                //to-do not sure if we need an abs here
                if (Math.Abs(angle) < 1.30899694) // < 75 degrees if more it will not be sheilded (attack from above, below, side)
                {
                    healthModifer *= (1 - percDamageAbsorbedByShield);
                }

            }

            if (disarming && numHitsForDisarm > 0)
            {
                numHitsForDisarm--;
                if (numHitsForDisarm <= 0)
                {
                    shielded = false;
                    actor.NotifyDisarmList();
                }
            }

            if ((health += healthModifer) <= 0)
            {
                if (shutdownInsteadOfKill)
                    actor.KillShutDown();
                else
                    actor.MarkForDeath();
            }


            return health;
        }
    }
}
