using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework.Graphics;
using System.Text;
using Microsoft.Xna.Framework;

namespace GameLib
{
    public class TempoQB : Quarterback
    {
        private ParameterSet Parm;

        static float beatFreq;
        public static float BeatFrequency
        {
            get { return beatFreq; }
            set { beatFreq = value; }
        }

        float windowBeforeBeat;
        public float WindowBeforeBeat
        {
            get { return windowBeforeBeat; }
            set { windowBeforeBeat = value; }
        }

        float windowAfterBeat;
        public float WindowAferBeat
        {
            get { return windowAfterBeat; }
            set { windowAfterBeat = value; }
        }

        private float beatTimer;

        private int playerBeatPauseCount;
        private int aiBeatPauseCount;

        private Texture2D note;
        private Rectangle indicatorDim;

        private bool beatPrev;
        private bool beat;
        //we know if beatTimer > beatFreq that means it hasn't been reset because it is in the windowafter beat time
        /*public bool OnBeat
        {
            get { return beat; }
        }

        public bool isNewBeat
        {
            get { return beat && !beatPrev; }
        }

        public bool PlayerBeat
        {
            get { return OnBeat && playerBeatPauseCount < 1; }
        }

        public bool AIBeat
        {
            get { return OnBeat && aiBeatPauseCount < 1; }
        }*/

        public override string Name()
        {
            return "TempoQB";
        }

        public TempoQB()
        {
        }

        public override void PreLoadInit(ParameterSet parm)
        {
            Parm = parm;

            //default values
            beatFreq = .5f;
            windowBeforeBeat = 0.0f;
            windowAfterBeat = .3f;
            indicatorDim = new Rectangle(200, 200, 40, 40);
        }

        public override void LoadContent()
        {
            note = Stage.Content.Load<Texture2D>("ParticleFX/FlowIndicator");
        }

        public override void Update(float dt)
        {
            if (IsPaused) return;

            beatPrev = beat;
            beatTimer += dt;

            if (beatTimer > beatFreq + windowAfterBeat)
            {
                beatTimer -= beatFreq;
                playerBeatPauseCount--;
                aiBeatPauseCount--;
                beat = false;
            }
            else if (beatTimer > beatFreq - windowBeforeBeat)
                beat = true;
            
        }

        public void HitBeat(int numBeatsToWait)
        {
            //note: feedback that the player hit the note should be played now
            playerBeatPauseCount = numBeatsToWait;
        }

        public void AIBeatPause(int numBeatsToWait)
        {
            aiBeatPauseCount = numBeatsToWait;
        }

        public override void DrawUI(float dt)
        {
            /*if (PlayerBeat)
            {
                //draw image
                Stage.renderer.SpriteBatch.Draw(note, indicatorDim, Color.White);
            }*/

            base.DrawUI(dt);

        }

        public override void Serialize(ParameterSet parm)
        {
            
        }

    }
}
