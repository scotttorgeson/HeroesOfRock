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
                content = Stage.Content;

            backgroundTexture = content.Load<Texture2D>("UI/Menu/background");
        }

        public override void Update (float dt, bool otherScreenHasFocus,
                                                       bool coveredByOtherScreen) {
            base.Update(dt, otherScreenHasFocus, false);
        }


        /// <summary>
        /// Draws the background screen.
        /// </summary>
        public override void Draw (float dt) {
            Rectangle fullscreen = new Rectangle(0, 0, Renderer.ScreenWidth, Renderer.ScreenHeight);

            Stage.renderer.SpriteBatch.Draw(backgroundTexture, fullscreen,
                             new Color(TransitionAlpha, TransitionAlpha, TransitionAlpha));
        }
    }
}
