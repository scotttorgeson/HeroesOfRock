using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using GameLib.Engine.MenuSystem.Menus;

namespace GameLib.Engine.MenuSystem.Screens {
    class ElevenTenLogoScreen : GameMenu {
        ContentManager content;
        Texture2D logoTexture;
        float duration;

        public ElevenTenLogoScreen (float seconds)
            : base("") {
            duration = seconds;
        }

        /* OVERRIDES */
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

        public override void HandleInput (MenuInput input) {}

        /// <summary>
        /// Draws the background screen.
        /// </summary>
        public override void Draw (float dt) {
            Rectangle background = new Rectangle(0, 0, Renderer.ScreenWidth, Renderer.ScreenHeight);
            Rectangle logoSize = new Rectangle(background.Center.X-(logoTexture.Width/2), 
                background.Center.Y-(logoTexture.Height/2), logoTexture.Width,logoTexture.Height);

            Stage.renderer.SpriteBatch.Draw(MenuSystem.BlankTexture, background, Color.Black);

            Stage.renderer.SpriteBatch.Draw(logoTexture, logoSize,
                             new Color(TransitionAlpha, TransitionAlpha, TransitionAlpha));
        }

        protected override void OnCancel () {}

        /* END OVERRIDES */

        private void LogoScreenOut () {
            this.MarkedForRemove = true;
            MenuSystem.AddScreen(new BepuPhysicsLogoScreen(2));
        }
    }
}
