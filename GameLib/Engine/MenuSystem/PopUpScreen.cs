using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
namespace GameLib.Engine.MenuSystem.Menus {
    public class PopUpScreen : GameMenu {
        string message;
        Texture2D smokedTexture;
        protected string Message { get; set; }
        protected Rectangle backgroundRectangle;

        public PopUpScreen (string message):
        base("")
        { 
            smokedTexture = Stage.Content.Load<Texture2D>("UI/Menu/smoked");
            TransitionOnTime = TimeSpan.FromSeconds(0.2);
            TransitionOffTime = TimeSpan.FromSeconds(0.2);
            IsPopup = true;
            this.message = message;
        }


        public override void LoadContent () {
            ContentManager content = Stage.Content;
        }

        public override void HandleInput (MenuInput input) {
            if (input.IsMenuCancel())
                ExitScreen();
        }

        public override void Draw (float dt) {
            base.Draw(dt);
            SpriteFont font = MenuSystem.Font;

            MenuSystem.FadeBackBufferToBlack(TransitionAlpha * 2 / 3);


            Vector2 viewportSize = new Vector2(Stage.renderer.GraphicsDevice.Viewport.Bounds.Width, Stage.renderer.GraphicsDevice.Viewport.Bounds.Height);
            Vector2 textSize = font.MeasureString(message);
            Vector2 textPosition = (viewportSize - textSize) / 2;

            const int hPad = 32;
            const int vPad = 16;

            backgroundRectangle = new Rectangle((int)textPosition.X - hPad,
                                                     (int)textPosition.Y - vPad,
                                                     (int)textSize.X + hPad * 2,
                                                      (int)textSize.Y + vPad * 2);

            Color color = Color.White * TransitionAlpha;

            // Draw the background rectangle.
            Stage.renderer.SpriteBatch.Draw(MenuSystem.BlankTexture, backgroundRectangle, Color.Black);

            // Draw the message box text.
            Stage.renderer.SpriteBatch.DrawString(font, message, textPosition, color);
        }

    }
}
