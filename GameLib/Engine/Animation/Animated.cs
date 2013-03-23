using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GameLib
{
    class Animated : Agent
    {
        public Animated(Actor actor)
            : base(actor)
        {
            this.Name = "Animated";
            actor.RegisterFunction(Update);
        }

        public override void Initialize()
        {
            pausePlay = ControlsQB.Instance.GetInputAction("Jump");
        }

        AnimationPlayer player;
        InputAction pausePlay;

        public void Update(float dt)
        {
            if (player == null)
            {
                player = this.actor.model.AnimatedModel.PlayClip(this.actor.model.AnimatedModel.Clips[0]);
                player.Looping = true;
            }

            if (pausePlay.IsNewAction)
            {
                switch (player.playState)
                {
                    case AnimationPlayer.PlayState.Pause:
                        player.playState = AnimationPlayer.PlayState.Play;
                        break;
                    case AnimationPlayer.PlayState.Play:
                        player.playState = AnimationPlayer.PlayState.Pause;
                        break;
                }
            }

            player.Update(dt);
        }
    }
}
