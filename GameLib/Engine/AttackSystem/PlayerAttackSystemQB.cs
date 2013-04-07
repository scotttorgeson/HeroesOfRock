




using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using GameLib.Engine.Particles;


namespace GameLib.Engine.AttackSystem
{
    class PlayerAttackSystemQB : Quarterback
    {
        Microsoft.Xna.Framework.Graphics.SpriteFont font; //remove once we get an actual gui

        //main character
        Actor mainCharacter;
        RockMeter playerRockMeter;

        Random rand;

        public PlayerAttackSystemQB()
        {

        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        public override void LoadContent()
        {
            //moveList = new MoveList(Stage.Content.Load<Move[]>("MoveList"));
            //inputManager = new InputManager(moveList.LongestMoveLength);
            mainCharacter = PlayerAgent.Player;
            playerRockMeter = mainCharacter.GetAgent<RockMeter>();

            font = Stage.Content.Load<Microsoft.Xna.Framework.Graphics.SpriteFont>("DefaultFont");
            rand = new Random();
        }


        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        public override void Update(float dt)
        {
            //if (IsPaused) return;
        }

        public void PerformAttack(Vector3 position, PlayerDirection facing, Move newMove, float increase)
        {
            bool shieldHit = false;
            string soundName = null;

            if (newMove.Sound_hit != String.Empty)
                soundName = newMove.Sound_hit;

            float damage = newMove.Damage + newMove.Damage * increase;
            float rockMeter = newMove.RockMeterIncrease;

            //bouding box 
            const float DEPTH_CONST = 100;
            const float HEIGHT_CONST = 2;
            BoundingBox hitbox = new BoundingBox();

            float front = newMove.FrontArea + newMove.FrontArea * increase;
            float back = newMove.BackArea + newMove.BackArea * increase;

            hitbox.Min.Y = position.Y - HEIGHT_CONST - HEIGHT_CONST * increase;
            hitbox.Max.Y = position.Y + HEIGHT_CONST + HEIGHT_CONST * increase;

            Vector3 force = Vector3.Zero;

            switch (facing)
            {
                case PlayerDirection.Right:
                    hitbox.Min.X = position.X - back;
                    hitbox.Max.X = position.X + front;
                    hitbox.Min.Z = position.Z - DEPTH_CONST;
                    hitbox.Max.Z = position.Z + DEPTH_CONST;
                    force = Vector3.Right;
                    break;
                case PlayerDirection.Left:
                    hitbox.Min.X = position.X - front;
                    hitbox.Max.X = position.X + back;
                    hitbox.Min.Z = position.Z - DEPTH_CONST;
                    hitbox.Max.Z = position.Z + DEPTH_CONST;
                    force = Vector3.Right;
                    break;
                case PlayerDirection.Forward:
                    hitbox.Min.X = position.X - DEPTH_CONST;
                    hitbox.Max.X = position.X + DEPTH_CONST;
                    hitbox.Min.Z = position.Z - front;
                    hitbox.Max.Z = position.Z + back;
                    force = Vector3.Backward;
                    break;
                case PlayerDirection.Backward:
                    hitbox.Min.X = position.X - DEPTH_CONST;
                    hitbox.Max.X = position.X + DEPTH_CONST;
                    hitbox.Min.Z = position.Z - back;
                    hitbox.Max.Z = position.Z + front;
                    force = Vector3.Backward;
                    break;
            }

            // todo: move to a better place. maybe a function in player agent called "SmashEffects" or something like that. maybe a class called "PlayerAttackEffects" and a static function called "Smash" in there
            if (newMove.Name == "Smash")
            {
                Vector3 difference = hitbox.Max - hitbox.Min;
                float length = difference.Z;
                float height = difference.Y;
                float width = difference.X;
                Vector3 center = new Vector3(hitbox.Max.X - width / 2.0f, hitbox.Max.Y - height / 2.0f, hitbox.Max.Z - length / 2.0f);
                Stage.ActiveStage.GetQB<Engine.Decals.DecalQB>().CreateDecal(new Ray(center, Vector3.Down), 10.0f, "Decals/crack", 10.0f, 20.0f, Decals.DecalLayers.CracksLayer);

                Stage.ActiveStage.GetQB<Particles.ParticleQB>().AddParticleEmitter(null, mainCharacter.PhysicsObject.Position + new Vector3(0.0f, -3.0f, 0.0f), true, -1f, 50,
                                                            75, 1.0f, 1.5f, 4, 5, new Vector2(1.0f), new Vector2(2.0f), new Vector3(1.0f, 0.0f, 1.0f),
                                                            Vector3.Up, new Vector3(8.4f, 0.3f, 8.4f), "dust2");

                Stage.ActiveStage.GetQB<AudioQB>().PlaySound("WeakAOE_16", 1.0f, 0.0f, 0.0f);
                Stage.ActiveStage.GetQB<AudioQB>().PlaySound("asphaltsmash_16", 0.7f, 0.0f, 0.0f);
            }

            Actor actor;
            foreach (GameLib.Engine.AI.AI ai in Stage.ActiveStage.GetQB<GameLib.Engine.AI.AIQB>().aliveEnemies)
            {
                actor = ai.actor;
                if (hitbox.Intersects(actor.PhysicsObject.CollisionInformation.BoundingBox))
                {
                    Vector3 actorPos = actor.PhysicsObject.Position;

                    HealthAgent actorHealth = actor.GetAgent<HealthAgent>();

                    float startingHealth = actorHealth.Health;
                    float remainingHealth = actorHealth.ModifyHealth(-damage, mainCharacter.PhysicsObject.Position, newMove.Disarming);

                    if (remainingHealth < startingHealth)
                    {
                        if (newMove.Force != Vector2.Zero && remainingHealth > 0)
                        {
                            force *= (actorPos - position);
                            force.Normalize();

                            force.X *= newMove.Force.X + newMove.Force.X * increase;
                            force.X *= newMove.Force.X + newMove.Force.X * increase;
                            float jumpVal = newMove.Force.Y + newMove.Force.Y * increase;

                            if (actor.PhysicsObject.physicsType == PhysicsObject.PhysicsType.CylinderCharacter)
                            {
                                actor.PhysicsObject.CylinderCharController.Body.LinearVelocity = Vector3.Zero;
                                if (jumpVal != 0)
                                {
                                    actor.PhysicsObject.CylinderCharController.Jump(jumpVal / actor.PhysicsObject.CylinderCharController.Body.Mass);
                                }
                                if (force != Vector3.Zero)
                                    actor.PhysicsObject.CylinderCharController.Body.ApplyLinearImpulse(ref force);
                            }
                        }

                        //we did damage so increase the multiplier
                        playerRockMeter.PerformedAttack(rockMeter);

                        //stun the enemy
                        if (newMove.StunTime != 0.0f)
                            ai.Stun(newMove.StunTime);

                        if (ai.bloodOnDamage)
                        {
                            Vector3 bloodDir = actorPos - mainCharacter.PhysicsObject.Position;
                            bloodDir.Normalize();
                            bloodDir *= 10;
                            Stage.ActiveStage.GetQB<Particles.ParticleQB>().AddParticleEmitter(null, actor.PhysicsObject.Position, true, -1, 5,
                                                            10, .25f, .5f, 0, 0, Vector2.One, Vector2.One * 2.0f,
                                                            Vector3.Zero, bloodDir, 2 * Vector3.One, "blood1");
                            Stage.ActiveStage.GetQB<Gameplay.BloodSplatterQB>().SplatBlood();
                            Stage.ActiveStage.GetQB<Decals.DecalQB>().CreateDecal(actorPos, new BoundingBox(new Vector3(-20.0f, -20.0f, -20.0f), new Vector3(20.0f, 20.0f, 20.0f)), "Decals/blood", 10.0f, 5.0f,                                                                                                         Decals.DecalLayers.BloodLayer);
                            //Stage.ActiveStage.GetQB<Decals.DecalQB>().CreateDecal(new Ray(actor.PhysicsObject.Position, Vector3.Down), 10.0f, "Decals/blood", 10.0f, 5.0f, Decals.DecalLayers.BloodLayer);
                        }

                        if (remainingHealth <= 0.0f) //killed the enemy
                        {
                            //playerRockMeter.IncreaseRockLevel(ai.rockLevelForKilling);
                            int increaseRockMeter = playerRockMeter.IncreaseScoreDueToKill(ai.pointsForKilling);
                            playerRockMeter.AddKill();

                            Vector3 screenPos = Stage.renderer.GraphicsDevice.Viewport.Project(actorPos,
                                CameraQB.ProjectionMatrix, CameraQB.ViewMatrix, Matrix.Identity);

                            Stage.ActiveStage.GetQB<ParticleQB>().AddFloatingText(new Vector2(screenPos.X, screenPos.Y + 10), -50 * Vector2.UnitY,
                                2.0f, increaseRockMeter.ToString(System.Globalization.CultureInfo.InvariantCulture));
                        }
                    }
                    else //you hit an enemy but dealt no damage 
                    {
                        shieldHit = true;
                        soundName = newMove.Sound_shield;
                    }

                    if (!newMove.AOE)
                        break; //only hit one enemy
                }
            }

            //play a random sound
            if (shieldHit)
                playRandomShieldHitSound(newMove.Disarming);
            else
                playRandomHitSound();
        }

        public void EnemyAttack(float dmg, float force, Vector3 v, Actor attacker, PlayerDirection facing)
        {
            PlayerState state = mainCharacter.GetAgent<PlayerAgent>().State;
            if (!(state == PlayerState.Jumping || state == PlayerState.Dashing))
            {
                BoundingBox hitbox = attacker.PhysicsObject.CollisionInformation.BoundingBox;

                const float DEPTH_CONST = 100;

                Vector3 position = attacker.PhysicsObject.Position;

                //set the Y/height values
                hitbox.Min.Y = position.Y - v.Z;
                hitbox.Max.Y = position.Y + v.Z;


                switch (facing)
                {
                    case PlayerDirection.Right:
                        hitbox.Min.X = position.X - v.Y;
                        hitbox.Max.X = position.X + v.X;
                        hitbox.Min.Z = position.Z - DEPTH_CONST;
                        hitbox.Max.Z = position.Z + DEPTH_CONST;
                        break;
                    case PlayerDirection.Left:
                        hitbox.Min.X = position.X - v.X;
                        hitbox.Max.X = position.X + v.Y;
                        hitbox.Min.Z = position.Z - DEPTH_CONST;
                        hitbox.Max.Z = position.Z + DEPTH_CONST;
                        break;
                    case PlayerDirection.Forward:
                        hitbox.Min.X = position.X - DEPTH_CONST;
                        hitbox.Max.X = position.X + DEPTH_CONST;
                        hitbox.Min.Z = position.Z - v.Y;
                        hitbox.Max.Z = position.Z + v.X;
                        break;
                    case PlayerDirection.Backward:
                        hitbox.Min.X = position.X - DEPTH_CONST;
                        hitbox.Max.X = position.X + DEPTH_CONST;
                        hitbox.Min.Z = position.Z - v.X;
                        hitbox.Max.Z = position.Z + v.Y;
                        break;
                }

                //to-do get a more accurate contact point
                if (hitbox.Intersects(mainCharacter.PhysicsObject.CollisionInformation.BoundingBox))
                {
                    if (force != 0)
                    {
                        Vector3 ForceVec = Vector3.Zero;

                        switch (AI.AIQB.MoveDirection)
                        {
                            case PlayerDirection.Left:
                                goto case PlayerDirection.Right;
                            case PlayerDirection.Right:
                                if (mainCharacter.PhysicsObject.Position.X > attacker.PhysicsObject.Position.X)
                                    ForceVec.X = force;
                                else
                                    ForceVec.X = -force;
                                break;
                            case PlayerDirection.Backward:
                                goto case PlayerDirection.Forward;
                            case PlayerDirection.Forward:
                                if (mainCharacter.PhysicsObject.Position.Z > attacker.PhysicsObject.Position.Z)
                                    ForceVec.Z = force;
                                else
                                    ForceVec.Z = -force;
                                break;

                        }

                        mainCharacter.PhysicsObject.CharacterController.Body.ApplyLinearImpulse(ref ForceVec);

                    }

                    Vector3 bloodDir = mainCharacter.PhysicsObject.Position - attacker.PhysicsObject.Position;
                    bloodDir.Normalize();
                    bloodDir *= 10;

                    Stage.ActiveStage.GetQB<Particles.ParticleQB>().AddParticleEmitter(null, mainCharacter.PhysicsObject.Position, true, -1, 5,
                                                            10, .25f, .5f, 0, 0, Vector2.One, Vector2.One * 2.0f, Vector3.Zero,
                                                            bloodDir, 2 * Vector3.One, "blood1");
                    Stage.ActiveStage.GetQB<Gameplay.BloodSplatterQB>().SplatBlood();
                    Stage.ActiveStage.GetQB<Decals.DecalQB>().CreateDecal(mainCharacter.PhysicsObject.Position, new BoundingBox(new Vector3(-20.0f, -20.0f, -20.0f), new Vector3(20.0f, 20.0f, 20.0f)), "Decals/blood", 10.0f, 5.0f,                                                                             Decals.DecalLayers.BloodLayer);
                    //Stage.ActiveStage.GetQB<Decals.DecalQB>().CreateDecal(new Ray(mainCharacter.PhysicsObject.Position, Vector3.Down), 10.0f, "Decals/blood", 10.0f, 5.0f, Decals.DecalLayers.BloodLayer);

                    playerRockMeter.RockLevelDownDueToDamage(dmg);

                    //play a random take damage sound
                    playRandomTakeDamageSound();
                }
            }
            else
            {
                return;
            }
        }

        public void EnemyAttackStun(int dmg, float stun, Vector2 v, Actor attacker)
        {
            if (mainCharacter.PhysicsObject.CharacterController.Dashing)
            {
                BoundingBox hitbox = attacker.PhysicsObject.CollisionInformation.BoundingBox;

                hitbox.Max.X += v.X;
                hitbox.Max.Y += v.Y;

                hitbox.Min.X -= v.X;
                hitbox.Min.Y -= v.Y;
                hitbox.Min.Z = -10;
                hitbox.Max.Z = 10;

                //to-do get a more accurate contact point
                if (hitbox.Intersects(mainCharacter.PhysicsObject.CollisionInformation.BoundingBox))
                {
                    playerRockMeter.RockLevelDownDueToDamage(dmg);
                    //mainCharacter.GetAgent<CharacterControllerInput>().stun(stun);
                }
            }
        }

        public override string Name()
        {
            return "PlayerAttackSystemQB";
        }

        private void playRandomShieldHitSound(bool isDisarming)
        {
            string soundName = null;
            if (isDisarming)
            {
                //disarming sound
                const int NUM_SHIELD_DISARM_SOUND = 2;
                if (rand.Next(NUM_SHIELD_DISARM_SOUND) == NUM_SHIELD_DISARM_SOUND - 1)
                {
                    soundName = "ShieldDisarm_16";
                }
                else
                {
                    soundName = "ShieldDisarm2_16";
                }
            }
            else
            {
                //shield hit sound
                const int NUM_SHIELD_HIT_SOUND = 3;
                switch (rand.Next(NUM_SHIELD_HIT_SOUND))
                {
                    case 0:
                        soundName = "ShieldHit1_16";
                        break;
                    case 1:
                        soundName = "ShieldHit2_16";
                        break;
                    case 2:
                        soundName = "ShieldHit3_16";
                        break;
                }
            }
            Stage.ActiveStage.GetQB<AudioQB>().PlaySound(soundName, 1.0f, 0.0f, 0.0f);
        }

        private void playRandomHitSound()
        {
            const int NUM_HIT_SOUND = 4;
            string soundName = null;

            switch (rand.Next(NUM_HIT_SOUND))
            {
                case 0:
                    soundName = "Melee1_16";
                    break;
                case 1:
                    soundName = "Melee2_16";
                    break;
                case 2:
                    soundName = "Melee3_16";
                    break;
                case 3:
                    soundName = "Melee4_16";
                    break;
            }
            Stage.ActiveStage.GetQB<AudioQB>().PlaySound(soundName, 1.0f, 0.0f, 0.0f);

        }

        private void playRandomTakeDamageSound()
        {
            string sound = null;
            const int NUM_HIT_SOUND = 5;
            switch (rand.Next(NUM_HIT_SOUND))
            {
                case 0:
                    sound = "Grunt1_16";
                    break;
                case 1:
                    sound = "Grunt2_16";
                    break;
                case 2:
                    sound = "Grunt3_16";
                    break;
                case 3:
                    sound = "GruntAOE_16";
                    break;
                case 4:
                    sound = "GruntMissile_16";
                    break;
            }

            Stage.ActiveStage.GetQB<AudioQB>().PlaySound(sound, 1, 0, 0);
        }
    }
}
