using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GameLib
{
    public class ExplosionAgent : Agent
    {
        public ExplosionAgent(Actor actor)
            : base(actor)
        {
            this.Name = "ExplosionAgent";
            actor.RegisterUpdateFunction(Update);
        }

        private AnimationPlayer animation;

        public void Update(float dt)
        {
            SkinnedRModelInstance modelInstance = actor.modelInstance as SkinnedRModelInstance;
            modelInstance.UpdateAnimations(dt);
            if (animation.playState == AnimationPlayer.PlayState.Pause)
                actor.MarkForDeath();
        }

        public override void Initialize(Stage stage)
        {
            SkinnedRModelInstance modelInstance = actor.modelInstance as SkinnedRModelInstance;
            animation = new AnimationPlayer(AnimationClip.LoadClip(actor.Parm.GetString("ModelName")), modelInstance);
            animation.Looping = false;
            animation.Rewind();
            animation.playState = AnimationPlayer.PlayState.Play;
        }
    }
}
