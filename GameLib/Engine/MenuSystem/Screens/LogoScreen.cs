using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using GameLib.Engine.MenuSystem.Menus;

namespace GameLib.Engine.MenuSystem.Screens {
    class LogoScreen : GameMenu {
        ContentManager content;
        Texture2D logoTexture;
        float duration;

        public LogoScreen (float seconds): base("") {
            duration = seconds;
        }


        public override void LoadContent () {
            if (content == null)
                content = Stage.Content;

            logoTexture = content.Load<Texture2D>("UI/Menu/11outOf10_Logo_White");
        }

        public override void Update (float dt, bool otherScreenHasFocus,
                                                       bool coveredByOtherScreen) {
            duration -= dt;
            if (duration < 0.0f)
                LogoScreenOut();
            base.Update(dt, otherScreenHasFocus, false);
        }

        private void LogoScreenOut () {
                MenuSystem.RemoveScreen(this);
                MenuSystem.AddScreen(new BackgroundScreen());
                MenuSystem.AddScreen(new SplashMenu());
        }

        /// <summary>
        /// Draws the background screen.
        /// </summary>
        public override void Draw (float dt) {
            SpriteBatch spriteBatch = MenuSystem.SpriteBatch;
            Viewport viewport = Renderer.Instance.GraphicsDevice.Viewport;
            Rectangle background = new Rectangle(0, 0, viewport.Width, viewport.Height);
            Rectangle logoSize = new Rectangle(background.Center.X-(logoTexture.Width/2), 
                background.Center.Y-(logoTexture.Height/2), logoTexture.Width,logoTexture.Height);

            spriteBatch.Begin();

            spriteBatch.Draw(MenuSystem.BlankTexture, background, Color.Black);

            spriteBatch.Draw(logoTexture, logoSize,
                             new Color(TransitionAlpha, TransitionAlpha, TransitionAlpha));

            spriteBatch.End();
        }
    }
}
