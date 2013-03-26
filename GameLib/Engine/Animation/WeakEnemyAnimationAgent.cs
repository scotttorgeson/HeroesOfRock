using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;

namespace GameLib
{
    class WeakEnemyAnimationAgent : Agent
    {
        public WeakEnemyAnimationAgent(Actor actor)
            : base(actor)
        {
            this.Name = "WeakEnemyAnimationAgent";
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

        public AnimationPlayer[] Animations
        {
            get { return animations; }
            set { animations = value; }
        }

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

        public static AnimationPlayer[] CreateAnimationList(Stage stage, ref ParameterSet parm, ref RModelInstance m)
        {
            SkinnedRModelInstance modelInstance = m as SkinnedRModelInstance;
            AnimationPlayer[] anims = new AnimationPlayer[(int)AnimationTypes.COUNT];
            if (parm.HasParm("AttackAnimation"))
                anims[(int)AnimationTypes.Attack] = new AnimationPlayer(AnimationClip.LoadClip(parm.GetString("AttackAnimation")), modelInstance);
            if (parm.HasParm("IdleAnimation"))
                anims[(int)AnimationTypes.Idle] = new AnimationPlayer(AnimationClip.LoadClip(parm.GetString("IdleAnimation")), modelInstance);
            if (parm.HasParm("DeathAnimation"))
                anims[(int)AnimationTypes.Die] = new AnimationPlayer(AnimationClip.LoadClip(parm.GetString("DeathAnimation")), modelInstance);
            if (parm.HasParm("WalkAnimation"))
                anims[(int)AnimationTypes.Walk] = new AnimationPlayer(AnimationClip.LoadClip(parm.GetString("WalkAnimation")), modelInstance);
            if (parm.HasParm("BlockAnimation"))
                anims[(int)AnimationTypes.Block] = new AnimationPlayer(AnimationClip.LoadClip(parm.GetString("BlockAnimation")), modelInstance);
            if (parm.HasParm("TakeDamageAnimation"))
                anims[(int)AnimationTypes.TakeDamage] = new AnimationPlayer(AnimationClip.LoadClip(parm.GetString("TakeDamageAnimation")), modelInstance);
            return anims;
        }

        float speed = 1.0f;
        AnimationTypes currentType = AnimationTypes.Idle;

        public void PlayAnimation(AnimationTypes animationType, float duration)
        {
#if DEBUG
            System.Diagnostics.Debug.Assert(animationType != AnimationTypes.COUNT, "EnemyAnimationAgent::PlayAnimation cannot play animation type COUNT");
#endif


            if (animationType != currentType)
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
