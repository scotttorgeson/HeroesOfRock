using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GameLib.Engine.AI;
using GameLib.Engine.AttackSystem;

// Agents are attached to actors and run an update loop.
// Main place for putting code that is associated with an actor.

namespace GameLib
{
    public class Agent
    {
        public bool MarkedForDeath = false;
        
        public readonly Actor actor;
        public string Name { get; protected set; }

        //public virtual void Update(float dt)
        //{

        //}

        public Agent(Actor actor)
        {
            this.actor = actor;
        }

        public static Agent CreateAgent(string agentType, Actor actor)
        {
            switch (agentType)
            {
                case "PlayerAgent":
                    return new PlayerAgent(actor);
                case "HeavyEnemyAnimationAgent":
                    return new HeavyEnemyAnimationAgent(actor);
                case "RangedEnemyAnimationAgent":
                    return new RangedEnemyAnimationAgent(actor);
                case "WeakEnemyAnimationAgent":
                    return new WeakEnemyAnimationAgent(actor);
                case "TriggerVolume":
                     return new TriggerVolume(actor);
                case "PlaySoundTriggerVolume":
                     return new PlaySoundTriggerVolume(actor);
                case "SpawnActorTriggerVolume":
                     return new SpawnActorTriggerVolume(actor);
                case "RotateCameraTriggerVolume":
                    return new RotateCameraTriggerVolume(actor);
                case "HealthTriggerVolume":
                    return new HealthTriggerVolume(actor);
                case "TextToScreenTriggerVolume":
                    return new TextToScreenTriggerVolume(actor);
                case "AirBurstTriggerVolume":
                    return new AirBurstTriggerVolume(actor);
                case "TutorialStopTriggerVolume":
                    return new TutorialStopTriggerVolume(actor);
                case "EndLevelTriggerVolume":
                    return new EndLevelTriggerVolume(actor);
                case "AIHeavy":
                    return new AIHeavy(actor);
                case "AIRanged":
                    return new AIRanged(actor);
                case "AIWeak":
                    return new AIWeak(actor);
                case "AIDumb":
                    return new AIDumb(actor);
                case "AIMissile":
                    return new AIMissile(actor);
                case "HealthAgent":
                    return new HealthAgent(actor);
                case "RockMeter":
                    return new RockMeter(actor);
                case "PlayerAnimationAgent":
                    return new PlayerAnimationAgent(actor);
                case "Explosion":
                    return new ExplosionAgent(actor);
                default:
                    System.Diagnostics.Debug.Assert(false, "Agent::CreateAgent - Invalid agent type.");
                    return null;
            }
        }

        public virtual void Initialize(Stage stage)
        {

        }

        public void SerializeStage(string key, ParameterSet parms)
        {
            //whatever agents do!
            
        }
    }
}
