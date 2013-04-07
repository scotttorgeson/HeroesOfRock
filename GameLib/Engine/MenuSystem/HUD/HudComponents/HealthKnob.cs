using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using GameLib.Engine.AttackSystem;

namespace GameLib.Engine.MenuSystem.HUD.HudComponents {
    class HealthKnob {

        private Texture2D numberBG;
        private Texture2D knobBG;
        private Texture2D knob;
        private Texture2D knobGlow;
        private Texture2D pointer;
        private Texture2D[] numberPieces;

        private Color[] colors;

        private Rectangle area;
        private Rectangle numberArea;
        private Rectangle numberPieceArea;
        private Rectangle pointerArea;
        private Rectangle knobBGArea;
        private Rectangle knobGlowArea;
        
        private float knobScale;
        private float glowAlpha;

        private Vector2 knobPosition;
        private Vector2 knobOrigin;

        private int glowDir;
        private int currentKnobNumber;

        private float[] knobAngles = new float[] { 0, -.495f, -.959f, -1.46f, -1.959f, -2.429f, -2.931f, -3.412f, -3.901f, -4.474f, -5.104f };

        public HealthKnob (Rectangle area) {
            this.area = area;
            this.currentKnobNumber = 1;
            this.glowAlpha = 0;
            this.glowDir = 1;
            this.numberPieces = new Texture2D[11];
            
            //set up rank order of colors
            colors = new Color[] { Color.Red, Color.Red, Color.DarkOrange, Color.DarkOrange, Color.Gold, Color.Gold, Color.YellowGreen, Color.YellowGreen, Color.Green, Color.Green, Color.White };
        }
       
        public void Update (float dt) {
            int rockLevel = (int)PlayerAgent.Player.GetAgent<RockMeter>().RockLevel;
            //rotate knob
            currentKnobNumber = rockLevel;
            
            //logic for knob glow
            if (rockLevel > 10) {
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
                if (currentKnobNumber != 11)
                    currentKnobNumber++;
            }
            if (c.CurrentKeyboardState.IsKeyDown(Keys.Down) && c.LastKeyboardState.IsKeyUp(Keys.Down)) {
                if (currentKnobNumber != 1)
                    currentKnobNumber--;
            }
#endif
        }

        public void LoadContent () {
            numberBG = Stage.Content.Load<Texture2D>("UI/HUD/number_bg");
            knobBG = Stage.Content.Load<Texture2D>("UI/HUD/knob_bg");
            knob = Stage.Content.Load<Texture2D>("UI/HUD/knob");
            knobGlow = Stage.Content.Load<Texture2D>("UI/HUD/knobGlow");
            pointer = Stage.Content.Load<Texture2D>("UI/HUD/pointer");
            
            
            for(int i = 0; i < 11; i++){
                numberPieces[i] = Stage.Content.Load<Texture2D>("UI/HUD/number"+ (i+1).ToString());
            }

            //setup scalers
            this.knobScale = 0.35f;
            int backgroundScaledWidth = (int)(numberBG.Width * knobScale);
            int backgroundScaledHeight = (int)(numberBG.Height * knobScale);

            //setup backgrounds rec
            this.knobBGArea = new Rectangle(area.Left, area.Top, (int)(knobBG.Width * knobScale), (int)(knobBG.Height * knobScale));
            this.numberArea = new Rectangle(knobBGArea.Right - 15 - (knobBGArea.Width / 2), knobBGArea.Center.Y - (backgroundScaledHeight / 2), backgroundScaledWidth, backgroundScaledHeight);
            this.numberPieceArea = new Rectangle(numberArea.Right - 10 - (int)(numberPieces[0].Width * knobScale),
                numberArea.Center.Y - (int)(numberPieces[0].Height * knobScale) / 2, (int)(numberPieces[0].Width * knobScale), (int)(numberPieces[0].Height * knobScale));

            //set up knob areas
            int gh = (int)(knobGlow.Height * knobScale);
            int gw = (int)(knobGlow.Width * knobScale);
            this.knobPosition = new Vector2(knobBGArea.Center.X - 8, knobBGArea.Center.Y);
            this.knobOrigin = new Vector2(knob.Width / 2, knob.Height / 2);
            this.knobGlowArea = new Rectangle((int)knobPosition.X - (gw / 2), (int)knobPosition.Y - (gh / 2), gw, gh);
            this.pointerArea = new Rectangle(knobBGArea.Right - (int)(pointer.Width * knobScale) - 10, knobBGArea.Center.Y - (int)(pointer.Height * knobScale) / 2, (int)(pointer.Width * knobScale), (int)(pointer.Height * knobScale));
        }

        public void Draw () {
            Stage.renderer.SpriteBatch.Draw(numberBG, numberArea, Color.White);
            Stage.renderer.SpriteBatch.Draw(knobBG, knobBGArea, Color.White);
            Stage.renderer.SpriteBatch.Draw(pointer, pointerArea, colors[currentKnobNumber-1]);
            Stage.renderer.SpriteBatch.Draw(knobGlow, knobGlowArea, Color.White * glowAlpha);
            Stage.renderer.SpriteBatch.Draw(knob, knobPosition, null, Color.White, knobAngles[currentKnobNumber-1], knobOrigin, knobScale, SpriteEffects.None, 0);
            Stage.renderer.SpriteBatch.Draw(numberPieces[currentKnobNumber-1], numberPieceArea, Color.White);
        }
    }
}
