using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;

namespace GameLib
{
    class EnemyAnimationAgent : Agent
    {
        public EnemyAnimationAgent(Actor actor)
            : base(actor)
        {
            this.Name = "EnemyAnimationAgent";
            actor.RegisterUpdateFunction(Update);
        }

        public enum AnimationTypes
        {
            Idle,
            Walk,
            Attack,
            Die,
            Block,
            TakeDamage,

            COUNT,
        }

        private AnimationPlayer[] animations;

        public override void Initialize(Stage stage)
        {
            SkinnedRModelInstance modelInstance = actor.modelInstance as SkinnedRModelInstance;
            animations = new AnimationPlayer[(int)AnimationTypes.COUNT];
            if (actor.Parm.HasParm("AttackAnimation"))
                animations[(int)AnimationTypes.Attack] = new AnimationPlayer(AnimationClip.LoadClip(actor.Parm.GetString("AttackAnimation")), modelInstance);
            if (actor.Parm.HasParm("IdleAnimation"))
                animations[(int)AnimationTypes.Idle] = new AnimationPlayer(AnimationClip.LoadClip(actor.Parm.GetString("IdleAnimation")), modelInstance);
            if (actor.Parm.HasParm("DeathAnimation"))
                animations[(int)AnimationTypes.Die] = new AnimationPlayer(AnimationClip.LoadClip(actor.Parm.GetString("DeathAnimation")), modelInstance);
            if (actor.Parm.HasParm("WalkAnimation"))
                animations[(int)AnimationTypes.Walk] = new AnimationPlayer(AnimationClip.LoadClip(actor.Parm.GetString("WalkAnimation")), modelInstance);
            if (actor.Parm.HasParm("BlockAnimation"))
                animations[(int)AnimationTypes.Block] = new AnimationPlayer(AnimationClip.LoadClip(actor.Parm.GetString("BlockAnimation")), modelInstance);
            if (actor.Parm.HasParm("TakeDamageAnimation"))
                animations[(int)AnimationTypes.TakeDamage] = new AnimationPlayer(AnimationClip.LoadClip(actor.Parm.GetString("TakeDamageAnimation")), modelInstance);
        }

        float speed = 1.0f;
        AnimationTypes currentType = AnimationTypes.Idle;

        public void PlayAnimation(AnimationTypes animationType, float duration)
        {
#if DEBUG
            System.Diagnostics.Debug.Assert(animationType != AnimationTypes.COUNT, "EnemyAnimationAgent::PlayAnimation cannot play animation type COUNT");
#endif


            if ( animationType != currentType )
            {
                currentType = animationType;
                SkinnedRModelInstance modelInstance = actor.modelInstance as SkinnedRModelInstance;
                modelInstance.SetAnimationPlayer(animations[(int)animationType]);
                animations[(int)animationType].Looping = true;
                animations[(int)animationType].playState = AnimationPlayer.PlayState.Play;
                animations[(int)animationType].Rewind();
            }


            if (duration < 0.0f)
                speed = 1.0f;
            else
                speed = animations[(int)animationType].Duration / duration;
        }

        public void PauseAnimation()
        {
            currentType = AnimationTypes.COUNT;
            SkinnedRModelInstance modelInstance = actor.modelInstance as SkinnedRModelInstance;
            modelInstance.SetAnimationPlayer(null);
        }

        public void Update(float dt)
        {
            SkinnedRModelInstance modelInstance = actor.modelInstance as SkinnedRModelInstance;
            modelInstance.UpdateAnimations(dt * speed);
        }
    }
}
