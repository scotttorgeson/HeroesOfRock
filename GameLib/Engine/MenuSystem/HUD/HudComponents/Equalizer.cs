using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using GameLib.Engine.AttackSystem;

namespace GameLib.Engine.MenuSystem.HUD.HudComponents {
    class Equalizer {

        //Texture Eq. Pieces
        private Texture2D knobBG;
        private Texture2D knob;
        private Texture2D knobGlow;

        private Rectangle area;
        private Rectangle knobArea;
        private Rectangle knobBGArea;
        private Rectangle knobGlowArea;
        
        private float rotationAngle;
        private float knobScale;
        private float glowAlpha;

        private Vector2 knobPosition;
        private Vector2 knobOrigin;
        private Vector2[] times;


        private Random r;
        private int glowDir;
        private int currentKnobNumber;

        public Equalizer (Rectangle area) {
            this.area = area;
            this.currentKnobNumber = 1;
            this.rotationAngle = 0;
            this.glowAlpha = 0;
            this.glowDir = 1;
            this.r = new Random();
        }
       
        public void Update (float dt) {
            //rotate knob
            if ((int)PlayerAgent.Player.GetAgent<RockMeter>().RockLevel != currentKnobNumber)
                RotateKnobTo((int)PlayerAgent.Player.GetAgent<RockMeter>().RockLevel);

            //logic for knob glow
            if ((int)PlayerAgent.Player.GetAgent<RockMeter>().RockLevel > 10) {
                if (glowAlpha > 1)
                    glowDir = -1;
                else if (glowAlpha < 0)
                    glowDir = 1;
                glowAlpha += (0.05f * glowDir);
            } else {
                glowAlpha = 0;
            }

#if DEBUG && WINDOWS
            ControlsQB c = Stage.ActiveStage.GetQB<ControlsQB>();
            if (c.CurrentKeyboardState.IsKeyDown(Keys.Up) && c.LastKeyboardState.IsKeyUp(Keys.Up)) {
                if(currentKnobNumber != 11)
                    RotateKnobTo(currentKnobNumber + 1);
            }
            if (c.CurrentKeyboardState.IsKeyDown(Keys.Down) && c.LastKeyboardState.IsKeyUp(Keys.Down)) {
                if (currentKnobNumber != 1)
                    RotateKnobTo(currentKnobNumber - 1);
            }
#endif
        }

        public void LoadContent () {
            knobBG = Stage.Content.Load<Texture2D>("UI/HUD/knob_bg");
            knob = Stage.Content.Load<Texture2D>("UI/HUD/knob");
            knobGlow = Stage.Content.Load<Texture2D>("UI/HUD/knobGlow");

            //set up scales
            this.knobScale = .70f;

            //setup backgrounds rec
            this.knobBGArea = new Rectangle(area.Center.X - (int)(knobBG.Width * knobScale) / 2, area.Top - (int)(knobBG.Height * knobScale) / 2, (int)(knobBG.Width * knobScale), (int)(knobBG.Height * knobScale));

            //set up knob areas
            int gh = (int)(knobGlow.Height * knobScale);
            int gw = (int)(knobGlow.Width * knobScale);
            this.knobPosition = new Vector2(knobBGArea.Center.X, knobBGArea.Center.Y);
            this.knobOrigin = new Vector2(knob.Width / 2, knob.Height / 2);
            this.knobGlowArea = new Rectangle((int)knobPosition.X - (gw / 2), (int)knobPosition.Y - (gh / 2), gw, gh);
        }

        
        public void Draw () {
            Stage.renderer.SpriteBatch.Draw(knobBG, knobBGArea, Color.White);
            Stage.renderer.SpriteBatch.Draw(knobGlow, knobGlowArea, Color.White * glowAlpha);
            Stage.renderer.SpriteBatch.Draw(knob, knobPosition, null, Color.White, rotationAngle, knobOrigin, knobScale, SpriteEffects.None, 0);
        }

        /// <summary>
        /// Rotates the knob.
        /// </summary>
        /// <param name="number"></param>
        private void RotateKnobTo (int number) {
            int delta = currentKnobNumber - number;
            float degree = .45f;

           // if (number > 9 || currentKnobNumber > 9) { degree += .05f; }

            rotationAngle += (degree * delta);
            currentKnobNumber = number;
        }
    }
}
