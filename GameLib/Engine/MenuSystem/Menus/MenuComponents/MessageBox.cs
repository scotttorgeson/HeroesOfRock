using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
namespace GameLib.Engine.MenuSystem.Menus {
   public  class MessageBoxScreen : GameScreen {
        string message;
        Texture2D smokedTexture;
        public event EventHandler<EventArgs> Accepted;
        public event EventHandler<EventArgs> Cancelled;

        public MessageBoxScreen (string message)
            : this(message, true) { }

        public MessageBoxScreen (string message, bool includeUsageText) {
            const string usageText = "\nA button = ok" +
                                     "\nB button = cancel";

            if (includeUsageText)
                this.message = message + usageText;
            else
                this.message = message;

            IsPopup = true;

            TransitionOnTime = TimeSpan.FromSeconds(0.2);
            TransitionOffTime = TimeSpan.FromSeconds(0.2);
            smokedTexture = Stage.Content.Load<Texture2D>("Menu/smoked");
        }


        public override void LoadContent () {
            ContentManager content = Stage.Content;
        }
         
        public override void HandleInput (MenuInput input) {
           if (input.IsMenuSelect()) {
                    Accepted(this, new EventArgs());

                ExitScreen();
            } else if (input.IsMenuCancel()) {
                // Raise the cancelled event, then exit the message box.
                if (Cancelled != null)
                    Cancelled(this, new EventArgs());

                ExitScreen();
            }
        }

        public override void Draw (float dt) {
            SpriteBatch spriteBatch = MenuSystem.SpriteBatch;
            SpriteFont font = MenuSystem.Font;

            MenuSystem.FadeBackBufferToBlack(TransitionAlpha * 2 / 3);


            Viewport viewport = Renderer.Instance.GraphicsDevice.Viewport;
            Vector2 viewportSize = new Vector2(viewport.Width, viewport.Height);
            Vector2 textSize = font.MeasureString(message);
            Vector2 textPosition = (viewportSize - textSize) / 2;

            const int hPad = 32;
            const int vPad = 16;

            Rectangle backgroundRectangle = new Rectangle((int)textPosition.X - hPad,
                                                          (int)textPosition.Y - vPad,
                                                          (int)textSize.X + hPad * 2,
                                                          (int)textSize.Y + vPad * 2);

            Color color = Color.White * TransitionAlpha;

            spriteBatch.Begin();

            // Draw the background rectangle.
            spriteBatch.Draw(smokedTexture, backgroundRectangle, Color.Black);

            // Draw the message box text.
            spriteBatch.DrawString(font, message, textPosition, color);

            spriteBatch.End();
        }

    }
}
