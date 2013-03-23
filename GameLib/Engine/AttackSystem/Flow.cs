using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GameLib
{
    public class Flow
    {
        public int flow_level = 0;

        private static readonly float[] rates = { 1.0f, 2.0f, 3.0f, 4.0f };
        private int flow_level_count { get { return rates.Length; } }
        
        private float window_time = 0.4f;
        private float offset_time = 0.0f;
        private float timer = 0.0f;
        private float changing_attack_timer = 0.0f;

        private Engine.AttackSystem.Move previous_attack = null;

        public Flow()
        {

        }

        enum FlowState
        {
            outside_window,
            inside_window,
            offset_time,
            doing_attack,
        }

        FlowState flowState = FlowState.outside_window;

        public void Update(float dt)
        {
            switch (flowState)
            {
                // inside window means we decrease timer
                // if timer goes to zero and we are still
                // in the window, we lose flow, go to outside
                // window
                case FlowState.inside_window:
                    timer -= dt;
                    if (timer < 0.0f)
                    {
                        timer = changing_attack_timer;
                        flowState = FlowState.outside_window;
                    }
                    break;
                // outside window means flow level 0
                case FlowState.outside_window:
                    timer -= dt;
                    break;
                // if doing attack wait until its time to go into window
                case FlowState.doing_attack:
                    timer -= dt;
                    if (timer < 0.0f)
                    {
                        if (offset_time == 0.0f)
                        {
                            flowState = FlowState.inside_window;
                            timer = window_time;
                        }
                        else
                        {
                            flowState = FlowState.offset_time;
                            timer = offset_time;
                        }
                    }
                    break;
                case FlowState.offset_time:
                    timer -= dt;
                    if (timer < 0.0f)
                    {
                        flowState = FlowState.inside_window;
                        timer = window_time;
                    }
                    break;
            }
        }

        public void DoingAttack(float t, bool increaseFlow)
        {
            switch (flowState)
            {
                // inside window means its time to increase the flow
                // and go into doing attack mode
                case FlowState.inside_window:
                    if (increaseFlow)
                    {
                        flow_level++;
                        if (flow_level >= flow_level_count)
                            flow_level = flow_level_count - 1;
                    }
                    flowState = FlowState.doing_attack;
                    timer = t;
                    break;
                // already doing an attack, so this does nothing
                case FlowState.doing_attack:
                    // too soon
                    break;
                // outside window means its time to go into doing attack mode
                case FlowState.outside_window:
                    flow_level = 0;
                    timer = t;
                    flowState = FlowState.doing_attack;
                    break;
                case FlowState.offset_time:
                    flow_level = 0;
                    timer = t;
                    flowState = FlowState.doing_attack;
                    break;
            }
            //offset_time = attack.TimeAfterAttack * this.getRate() + attack.FlowStart;
            //window_time = attack.FlowDuration;
        }

        public float getRate()
        {
            return rates[flow_level];
        }

        public void setTimer(float t)
        {
            timer = t + window_time;
            flowState = FlowState.inside_window;
        }

    }
}
