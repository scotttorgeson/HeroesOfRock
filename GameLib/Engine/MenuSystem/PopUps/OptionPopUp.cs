using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
namespace GameLib.Engine.MenuSystem.Menus {

    public class OptionPopUp : PopUpScreen {
        private Texture2D amp;
        private Texture2D knob;
        private Texture2D meterNeedle;
        private Texture2D meter;
        private Texture2D sliderBar;
        private Texture2D sliderKnob;
        private Texture2D ampSwitch;
        private Texture2D selected;

        private float rotationAngle;

        private Vector2 knobPosition;
        private Vector2 knobOrigin;

        public OptionPopUp (string message)
            : this(message, true) { }

        public OptionPopUp (string message, bool includeUsageText)
            : base(message) {
                rotationAngle = 0;
        }

        public override void LoadContent () {
            base.LoadContent();
             this.amp = Stage.Content.Load<Texture2D>("UI/Options/ampBody");
             this.knob = Stage.Content.Load<Texture2D>("UI/Options/knob");
             this.meterNeedle = Stage.Content.Load<Texture2D>("UI/Options/meterNeedle");
             this.meter = Stage.Content.Load<Texture2D>("UI/Options/meter");
             this.sliderBar = Stage.Content.Load<Texture2D>("UI/Options/sliderBar");
             this.sliderKnob = Stage.Content.Load<Texture2D>("UI/Options/sliderKnob");
             this.ampSwitch = Stage.Content.Load<Texture2D>("UI/Options/switch");

            int x = Stage.renderer.GraphicsDevice.Viewport.Bounds.Center.X;
            int y = Stage.renderer.GraphicsDevice.Viewport.Bounds.Center.Y;
            this.backgroundRectangle = new Rectangle(x - amp.Width / 2, y - amp.Height / 2, amp.Width, amp.Height);
            this.knobPosition = new Vector2(amp.Bounds.Right + knob.Width, amp.Bounds.Top + 25);
            this.knobOrigin = new Vector2(knob.Width / 2, knob.Height / 2);
        }

        public override void Draw (float dt) {
            SpriteFont font = MenuSystem.Font;

            MenuSystem.FadeBackBufferToBlack(TransitionAlpha * 2 / 3);

            Color color = Color.White * TransitionAlpha;

            // Draw the background rectangle.
            Stage.renderer.SpriteBatch.Draw(amp, backgroundRectangle, color);
            Stage.renderer.SpriteBatch.Draw(knob, knobPosition, null, color, rotationAngle, knobOrigin, 1f, SpriteEffects.None, 0);

            // Draw the message box text.
            // Stage.renderer.SpriteBatch.DrawString(font, message, textPosition, color);
        }

        public override void HandleInput (MenuInput input) {
            base.HandleInput(input);

            if (input.IsMenuLeft()) {
            } else if (input.IsMenuRight()) {

            }
        }
        /// <summary>
        /// Rotates the knob.
        /// </summary>
        /// <param name="number"></param>
        private void RotateKnobTo (int dir) {
            if(selected==knob){
             float degree = .45f;
            // if (number > 9 || currentKnobNumber > 9) { degree += .05f; }
            rotationAngle += (degree * dir);
            }

        }
    }
}
