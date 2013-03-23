using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;


namespace GameLib.Engine.MenuSystem.Menus {
    /// <summary>
    /// The background screen sits behind all the other menu screens.
    /// </summary>
    public class BackgroundScreen : GameScreen {

        ContentManager content;
        Texture2D backgroundTexture;

        public BackgroundScreen () {
            TransitionOnTime = TimeSpan.FromSeconds(0.5);
            TransitionOffTime = TimeSpan.FromSeconds(0.5);
        }


        public override void LoadContent () {
            if (content == null)
                content = new ContentManager(MenuSystem.Game.Services, "Content");

            backgroundTexture = content.Load<Texture2D>("Menu/background");
        }

        public override void UnloadContent () {
            content.Unload();
        }

        public override void Update (GameTime gameTime, bool otherScreenHasFocus,
                                                       bool coveredByOtherScreen) {
            base.Update(gameTime, otherScreenHasFocus, false);
        }


        /// <summary>
        /// Draws the background screen.
        /// </summary>
        public override void Draw (GameTime gameTime) {
            SpriteBatch spriteBatch = MenuSystem.SpriteBatch;
            Viewport viewport = MenuSystem.GraphicsDevice.Viewport;
            Rectangle fullscreen = new Rectangle(0, 0, viewport.Width, viewport.Height);

            spriteBatch.Begin();

            spriteBatch.Draw(backgroundTexture, fullscreen,
                             new Color(TransitionAlpha, TransitionAlpha, TransitionAlpha));

            spriteBatch.End();
        }
    }
}
