using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using GameLib.Engine.AttackSystem;

namespace GameLib.Engine.MenuSystem.HUD.HudComponents {
    class OldEqualizer {

        //Texture Eq. Pieces
        private Texture2D equalizerBarBG;
        private Texture2D equalizerFrame;
        private Texture2D knobBG;
        private Texture2D knob;
        private Texture2D knobGlow;
        private Texture2D pointer;
        private Texture2D barPiece;

        private Color[] colors;
        private Color pointerColor;

        private Rectangle area;
        private Rectangle equalizerArea;
        private Rectangle colorArea;
        private Rectangle pointerArea;
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

        public OldEqualizer (Rectangle area) {
            this.area = area;
            this.currentKnobNumber = 1;
            this.rotationAngle = 0;
            this.glowAlpha = 0;
            this.glowDir = 1;
            this.r = new Random();
            this.pointerColor = Color.Transparent;
            this.times = new Vector2[] { new Vector2(0f, 0), new Vector2(0.5f, 0), new Vector2(1f, 0) };
        }
       
        public void Update (float dt) {
            //rotate knob
            if ((int)PlayerAgent.Player.GetAgent<RockMeter>().RockLevel != currentKnobNumber)
                RotateKnobTo((int)PlayerAgent.Player.GetAgent<RockMeter>().RockLevel);
        
            //change pointer color
            int rockLevel = (currentKnobNumber - (currentKnobNumber%2))/2;
            if(rockLevel > colors.Length-1)
                pointerColor = colors[colors.Length - 1];
            else
                pointerColor = colors[rockLevel];

            //randomizes equalizers
            for(int i = 0; i < times.Length; i++) {
                times[i].X += dt;
            }


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
            equalizerBarBG = Stage.Content.Load<Texture2D>("UI/HUD/equalizerBar_bg");
            equalizerFrame = Stage.Content.Load<Texture2D>("UI/HUD/equalizerFrame");
            knobBG = Stage.Content.Load<Texture2D>("UI/HUD/knob_bg");
            knob = Stage.Content.Load<Texture2D>("UI/Options/knob");
            knobGlow = Stage.Content.Load<Texture2D>("UI/HUD/knobGlow");
            pointer = Stage.Content.Load<Texture2D>("UI/HUD/pointer");
            barPiece = Stage.Content.Load<Texture2D>("UI/HUD/barPiece");

            //set up rank order of colors
            colors = new Color[] { Color.DarkRed, Color.Orange, Color.Yellow, Color.YellowGreen, Color.Green };

            //setup scalers
            this.knobScale = ((float)area.Width / (float)knob.Width) / 2;
            float pointerScale = 0.05f;
            float backgroundScale = ((float)area.Width / (float)equalizerBarBG.Width);
            int backgroundScaledWidth = (int)(equalizerBarBG.Width * backgroundScale);//backgroundScale);
            int backgroundScaledHeight = (int)(equalizerBarBG.Height * backgroundScale);//backgroundScale);

            //setup backgrounds rec
            this.knobBGArea = new Rectangle(area.Left, area.Top, (int)(knobBG.Width * knobScale), (int)(knobBG.Height * knobScale));
            this.equalizerArea = new Rectangle(knobBGArea.Right - (knobBGArea.Width / 3), knobBGArea.Center.Y - (backgroundScaledHeight / 2), backgroundScaledWidth, backgroundScaledHeight);

            //setup color areas
            int h = (int)(barPiece.Height * backgroundScale);
            int w = (int)(barPiece.Width * backgroundScale);
            this.colorArea = new Rectangle(knobBGArea.Right - backgroundScaledWidth / 10, equalizerArea.Top + backgroundScaledHeight / 10, backgroundScaledWidth / 12, backgroundScaledHeight / 4);

            //set up knob areas
            int gh = (int)(knobGlow.Height * knobScale);
            int gw = (int)(knobGlow.Width * knobScale);
            this.knobPosition = new Vector2(knobBGArea.Center.X, knobBGArea.Center.Y);
            this.knobOrigin = new Vector2(knob.Width / 2, knob.Height / 2);
            this.knobGlowArea = new Rectangle((int)knobPosition.X - (gw / 2), (int)knobPosition.Y - (gh / 2), gw, gh);
            this.pointerArea = new Rectangle(knobBGArea.Center.X - ((int)(pointer.Width * pointerScale) / 2), knobBGArea.Top + (int)(pointer.Height * pointerScale) / 2, (int)(pointer.Width * pointerScale), (int)(pointer.Height * pointerScale));
        }
        
        public void Draw () {
            Stage.renderer.SpriteBatch.Draw(equalizerBarBG, equalizerArea, Color.White);

            //Draw ByColumns
            for (int i = 0; i < times.Length; i++) {
                if(times[i].X > 0.5){
                    times[i].Y = r.Next(-1, currentKnobNumber / 2);     
                    times[i].X = 0;
                    // System.Console.Out.WriteLine(i + ", " + times[i].Y);
                }
                int equalize = currentKnobNumber + (int)times[i].Y;

                if (equalize < 0)
                    equalize = 0;
                else if (equalize > 11)
                    equalize = 11;

                DrawRows(equalize, i);
            }
            Stage.renderer.SpriteBatch.Draw(equalizerFrame, equalizerArea, Color.White);
            Stage.renderer.SpriteBatch.Draw(knobBG, knobBGArea, Color.White);
            Stage.renderer.SpriteBatch.Draw(pointer, pointerArea, pointerColor);
            Stage.renderer.SpriteBatch.Draw(knobGlow, knobGlowArea, Color.White * glowAlpha);
            Stage.renderer.SpriteBatch.Draw(knob, knobPosition, null, Color.White, rotationAngle, knobOrigin, knobScale, SpriteEffects.None, 0);
        }

        /// <summary>
        /// Draws one column of health. 
        /// </summary>
        /// <param name="spriteBatch"></param>
        /// <param name="equalize"> the number of colors to draw out of 9. The rest are drawn transparent</param>
        /// <param name="xOffset"> says which column number it is so it know how far over to draw it. s</param>
        private void DrawRows (int equalize, int xOffset) {

            for (int i = 0; i < 11; i++) {
                int rockLevel = (i - (i % 2))/2; //adjusts the rock level to correspond with the colors
                int offset = i;

                if (i == 8 || i == 9)
                    offset = 8;
                else if (i == 10) {
                    offset = 9;
                    rockLevel = 4;
                }
                    
                                
                int x = colorArea.X + ((colorArea.Width + colorArea.Width / 12) * offset);
                int y = colorArea.Y + ((colorArea.Height + colorArea.Height/5) * xOffset);
                Rectangle tmp = new Rectangle(x, y, colorArea.Width - 1, colorArea.Height);


                if (i <= equalize) {
                    //using tints
                    Stage.renderer.SpriteBatch.Draw(barPiece, tmp, colors[rockLevel] * 0.95f);
                } else {
                    Stage.renderer.SpriteBatch.Draw(barPiece, tmp, colors[rockLevel] * 0.15f);
                }

            }
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
