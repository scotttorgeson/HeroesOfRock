
using Microsoft.Xna.Framework.Content;
using System;
using Microsoft.Xna.Framework.Input;
using BEPUphysics.CollisionShapes.ConvexShapes;
using Microsoft.Xna.Framework;
using GameLib.Engine.AttackSystem;

namespace GameLib.Engine.AttackSystem {
    /// <summary>
    /// Describes a sequences of buttons which must be pressed to active the move.
    /// A real game might add a virtual PerformMove() method to this class.
    /// </summary> 
    /// 

    public class Move {
       

        private string name;
        private float frontArea;
        private float backArea;
        private string buttonSequence;
        private float timeBefore;
        private float timeAfter;
        private float stunTime;
        private float damage;
        private float rockMeterIncrease;
        private float flowStart;
        private float flowDuration;
        private float movement;
        private string animation;
        private string sound_hit;
        private string sound_shield;
        private bool aoe;
        private Vector2 force;
        private string particleEffect;
        private PlayerAnimationAgent.AnimationTypes animationEnum;
        private PlayerAnimationAgent.AnimationTypes animationSecond;
        private float damageSecond;
        private float timeBeforeSecond;
        private bool disarming;

        ///GETTERS SETTERS

        public string Name {
            get { return name; }
            set { name = value; }
        }

        public string ButtonSequence {
            get { return buttonSequence; }
            set {
                buttonSequence = value;
            }
        }

        public float TimeBeforeAttack {
            get { return timeBefore; }
            set { timeBefore = value; }
        }

        public float TimeAfterAttack
        {
            get { return timeAfter; }
            set { timeAfter = value; }
        }

        public float Damage {
            get { return damage; }
            set { damage = value; }
        }

        public float StunTime
        {
            get { return stunTime; }
            set { stunTime = value; }
        }

        public bool Disarming
        {
            get { return disarming; }
            set { disarming = value; }
        }
        
        public float RockMeterIncrease
        {
            get { return rockMeterIncrease; }
            set { rockMeterIncrease = value; }
        }

        public float FlowStart
        {
            get { return flowStart; }
            set { flowStart = value; }
        }

        public float FlowDuration
        {
            get { return flowDuration; }
            set { flowDuration = value; }
        }

        public float FrontArea
        {
            get { return frontArea; }
            set { frontArea = value; }
        }

        public float BackArea
        {
            get { return backArea; }
            set { backArea = value; }
        }

        public Vector2 Force
        {
            get { return force; }
            set { force = value; }
        }

        public string ParticleEffect {
            get { return particleEffect; }
            set { particleEffect = value; }
        }

        public float Movement
        {
            get { return movement; }
            set { movement = value; }
        }

        public string Animation
        {
            get { return animation; }
            set
            {
                animationEnum = (PlayerAnimationAgent.AnimationTypes)Enum.Parse(typeof(PlayerAnimationAgent.AnimationTypes), value, true);
                animationSecond = PlayerAnimationAgent.AnimationTypes.None;
                animation = value;
            }
        }

        public string Sound_hit
        {
            get { return sound_hit; }
            set { sound_hit = value; }
        }

        public string Sound_shield
        {
            get { return sound_shield; }
            set { sound_shield = value; }
        }

        public bool AOE
        {
            get { return aoe; }
            set { aoe = value; }
        }

        [ContentSerializerIgnore]

        public PlayerAnimationAgent.AnimationTypes AnimationType
        {
            get { return animationEnum; }
            set { animationEnum = value; }

        }

        [ContentSerializerIgnore]

        public PlayerAnimationAgent.AnimationTypes AnimationSecond
        {
            get { return animationSecond; }
            set { animationSecond = value; }
        }
      
        [ContentSerializerIgnore]

        public float TimeBeforeSecond
        {
            get { return timeBeforeSecond; }
            set { timeBeforeSecond = value; }
        }
    }
}
