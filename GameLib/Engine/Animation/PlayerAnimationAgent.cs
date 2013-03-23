using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;

namespace GameLib
{
    public class PlayerAnimationAgent: Agent
    {
        public PlayerAnimationAgent(Actor actor)
            : base(actor)
        {
            this.Name = "PlayerAnimationAgent";
            actor.RegisterUpdateFunction(Update);
        }

        public enum AnimationTypes
        {
            None,
            Idle,
            Walk,
            WeakAttack,
            MediumAttack,
            HeavyAttack,
            SmashAttack,
            Dash,
            Die,
            TakeDamage,
            Jump,

            COUNT,
        }

        private AnimationPlayer[] animations;

        public override void Initialize(Stage stage)
        {
            SkinnedRModelInstance modelInstance = actor.modelInstance as SkinnedRModelInstance;
            animations = new AnimationPlayer[(int)AnimationTypes.COUNT];
            if (actor.Parm.HasParm("WeakAttackAnimation"))
                animations[(int)AnimationTypes.WeakAttack] = new AnimationPlayer(AnimationClip.LoadClip(actor.Parm.GetString("WeakAttackAnimation")), modelInstance);
            if (actor.Parm.HasParm("MediumAttackAnimation"))
                animations[(int)AnimationTypes.MediumAttack] = new AnimationPlayer(AnimationClip.LoadClip(actor.Parm.GetString("MediumAttackAnimation")), modelInstance);
            if (actor.Parm.HasParm("HeavyAttackAnimation"))
                animations[(int)AnimationTypes.HeavyAttack] = new AnimationPlayer(AnimationClip.LoadClip(actor.Parm.GetString("HeavyAttackAnimation")), modelInstance);
            if (actor.Parm.HasParm("SmashAttackAnimation"))
                animations[(int)AnimationTypes.SmashAttack] = new AnimationPlayer(AnimationClip.LoadClip(actor.Parm.GetString("SmashAttackAnimation")), modelInstance);
            if (actor.Parm.HasParm("DashAnimation"))
                animations[(int)AnimationTypes.Dash] = new AnimationPlayer(AnimationClip.LoadClip(actor.Parm.GetString("DashAnimation")), modelInstance);
            if (actor.Parm.HasParm("IdleAnimation"))
                animations[(int)AnimationTypes.Idle] = new AnimationPlayer(AnimationClip.LoadClip(actor.Parm.GetString("IdleAnimation")), modelInstance);
            if (actor.Parm.HasParm("DeathAnimation"))
                animations[(int)AnimationTypes.Die] = new AnimationPlayer(AnimationClip.LoadClip(actor.Parm.GetString("DeathAnimation")), modelInstance);
            if (actor.Parm.HasParm("WalkAnimation"))
                animations[(int)AnimationTypes.Walk] = new AnimationPlayer(AnimationClip.LoadClip(actor.Parm.GetString("WalkAnimation")), modelInstance);
            if (actor.Parm.HasParm("TakeDamageAnimation"))
                animations[(int)AnimationTypes.TakeDamage] = new AnimationPlayer(AnimationClip.LoadClip(actor.Parm.GetString("TakeDamageAnimation")), modelInstance);
            if (actor.Parm.HasParm("JumpAnimation"))
                animations[(int)AnimationTypes.Jump] = new AnimationPlayer(AnimationClip.LoadClip(actor.Parm.GetString("JumpAnimation")), modelInstance);
        }

        float speed = 1.0f;
        AnimationTypes currentType = AnimationTypes.Idle;
        bool animationPlaying = false;

        public void PlayAnimation(AnimationTypes animationType, float duration)
        {
#if DEBUG
            System.Diagnostics.Debug.Assert(animationType != AnimationTypes.COUNT, "PlayerAnimationAgent::PlayAnimation cannot play animation type COUNT");
#endif
            if ( animationType != currentType || !animationPlaying )
            {
                animationPlaying = true;
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

        public void Update(float dt)
        {
            SkinnedRModelInstance modelInstance = actor.modelInstance as SkinnedRModelInstance;
            modelInstance.UpdateAnimations(dt * speed);
        }

        public void resetAnimation(AnimationTypes animationType)
        {
            animations[(int)animationType].Rewind();
        }
    }
}
